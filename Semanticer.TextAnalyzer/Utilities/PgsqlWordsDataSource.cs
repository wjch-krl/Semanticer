using System;
using System.Data;
using System.Diagnostics;
using Npgsql;
using Semanticer.Classifier;
using Semanticer.Classifier.Bayesian;

namespace Semanticer.TextAnalyzer.Utilities
{
    class PgsqlWordsDataSource : ICategorizedWordsDataSource
    {
        protected IDbConnectionManager connectionManager;
		protected string tableName = "WordProbability";
		protected string wordColumn = "Word";
		protected string categoryColumn = "Category";
		protected string matchCountColumn = "Matches";
		protected string nonMatchCountColumn = "NonMatches";

		/// <summary>
		/// Create a SqlWordsDataSource using the DEFAULT_CATEGORY ("DEFAULT")
		/// </summary>
		/// <param name="conMgr">The connection manager to use.</param>
		/// <param name="aTableName">The name of the table storing word probabilities.</param>
		/// <param name="aWordColumn">The word column.</param>
		/// <param name="aCategoryColumn">The category column.</param>
		/// <param name="aMatchCountColumn">The match count column.</param>
		/// <param name="aNonMatchCountColumn">The non-match count column.</param>
		public PgsqlWordsDataSource(IDbConnectionManager conMgr, string aTableName, string aWordColumn, string aCategoryColumn, 
			string aMatchCountColumn, string aNonMatchCountColumn) 
        {
			connectionManager = conMgr;
			tableName = aTableName;
			wordColumn = aWordColumn;
			categoryColumn = aCategoryColumn;
			matchCountColumn = aMatchCountColumn;
			nonMatchCountColumn = aNonMatchCountColumn;
            CreateTable();
        }

        public PgsqlWordsDataSource(IDbConnectionManager conMgr,string lang) 
        {
            tableName = string.Format("{0}_{1}",tableName, lang.Replace("-", ""));
            connectionManager = conMgr;
            CreateTable();
        }
        

		public WordProbability GetWordProbability(string category, string word)
		{
			WordProbability wp = null;
			int matchingCount = 0;
			int nonMatchingCount = 0;

			NpgsqlConnection connection = null;
			try
			{
				connection = (NpgsqlConnection)connectionManager.GetConnection();
                IDbCommand command = new NpgsqlCommand(string.Format("SELECT {0}, {1} FROM {2} WHERE {3} = @Word AND {4} = @Category", matchCountColumn, nonMatchCountColumn, tableName, wordColumn, categoryColumn), connection);
                command.Parameters.Add(new NpgsqlParameter("Word",word));
			    command.Parameters.Add(new NpgsqlParameter("Category", category));
                IDataReader reader = command.ExecuteReader();
				if (reader.Read())
				{
					matchingCount = (int)reader[matchCountColumn];
					nonMatchingCount = (int)reader[nonMatchCountColumn];
				}
				reader.Close();
				wp = new WordProbability(word, matchingCount, nonMatchingCount);
			}
			catch (Exception ex)
			{
				throw new WordsDataSourceException("Problem obtaining WordProbability from database.", ex);
			}
			finally
			{
				if (connection != null)
				{
					try
					{
						connectionManager.ReturnConnection(connection);
					}
					catch {}
				}
			}

			Debug.WriteLine("GetWordProbability() WordProbability loaded [" + wp + "]");

			return wp;
		}

		public WordProbability GetWordProbability(string word)
		{
			return GetWordProbability(CategorizedClassifierConstants.DefaultCategory, word);
		}

		private void UpdateWordProbability(string category, string word, bool isMatch)
		{
			string fieldName = isMatch ? matchCountColumn : nonMatchCountColumn;

			// truncate word at 255 characters
			if (word.Length > 255)
				word = word.Substring(0, 255);

			NpgsqlConnection connection = null;
			try
			{
				connection = (NpgsqlConnection)connectionManager.GetConnection();
				IDbCommand command = null;
				IDataReader reader = null;

				// see if the word exists in the table
                command = new NpgsqlCommand(string.Format("SELECT * FROM {0} WHERE {1} = @Word AND {2} = @Category", tableName, wordColumn, categoryColumn), connection);
				command.Parameters.Add(new NpgsqlParameter("Word", word));
				command.Parameters.Add(new NpgsqlParameter("Category",category));
				reader = command.ExecuteReader(CommandBehavior.SingleResult);

				if (!reader.Read()) // word is not in table, so insert the word
				{
					reader.Close();
                    command = new NpgsqlCommand(string.Format("INSERT INTO {0} ({1}, {2}) VALUES (@Word, @Category)", tableName, wordColumn, categoryColumn), connection);
                    command.Parameters.Add(new NpgsqlParameter("Word", word));
					command.Parameters.Add(new NpgsqlParameter("Category", category));
					command.ExecuteNonQuery();
				}
				else
					reader.Close();

				// update the word count
                command = new NpgsqlCommand(string.Format("UPDATE {0} SET {1} = {1} + 1 WHERE {2} = @Word AND {3} = @Category", tableName, fieldName, wordColumn, categoryColumn), connection);
				command.Parameters.Add(new NpgsqlParameter("Word",word));
				command.Parameters.Add(new NpgsqlParameter("Category", category));
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				throw new WordsDataSourceException("Problem updating WordProbability.", ex);
			}
			finally
			{
				if (connection != null)
				{
					try
					{
						connectionManager.ReturnConnection(connection);
					}
					catch {}
				}
			}
		}

        public void AddMatch(string category, string word)
		{
			if (category == null)
				throw new ArgumentNullException("Category cannot be null.");
			UpdateWordProbability(category, word, true);
		}

		public void AddMatch(string word)
		{
			UpdateWordProbability(CategorizedClassifierConstants.DefaultCategory, word, true);
		}

		public void AddNonMatch(string category, string word)
		{
			if (category == null)
				throw new ArgumentNullException("Category cannot be null.");
			UpdateWordProbability(category, word, false);
		}

		public void AddNonMatch(string word)
		{
			UpdateWordProbability(CategorizedClassifierConstants.DefaultCategory, word, false);
		}

        public void Clear()
        {
            NpgsqlConnection connection = null;
            try
            {
                connection = (NpgsqlConnection) connectionManager.GetConnection();
                NpgsqlCommand command = new NpgsqlCommand(string.Format("delete from {0};", tableName), connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new WordsDataSourceException("Problem clearing table.", ex);
            }
            finally
            {
                if (connection != null)
                {
                    connectionManager.ReturnConnection(connection);
                }
            }
        }

        /// <summary>
		/// Create the table if it does not already exist.
		/// </summary>
		private void CreateTable()
		{
			NpgsqlConnection connection = null;
			try
			{
				connection = (NpgsqlConnection)connectionManager.GetConnection();
                NpgsqlCommand command = new NpgsqlCommand(string.Format("select 1 from pg_tables where schemaname='public' and tablename = '{0}';", tableName.ToLower()), connection);
			    var ifExists = command.ExecuteScalar();
				if (ifExists == null)
				{
					command = new NpgsqlCommand(string.Format("CREATE TABLE {0} ({1} VARCHAR(255) NOT NULL, {2} VARCHAR(20) NOT NULL, {3} INT DEFAULT 0 NOT NULL, {4} INT DEFAULT 0 NOT NULL, PRIMARY KEY({1}, {2}))", tableName, wordColumn, categoryColumn, matchCountColumn, nonMatchCountColumn), connection);
					command.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				throw new WordsDataSourceException("Problem creating table.", ex);
			}
			finally
			{
				if (connection != null)
				{
				    connectionManager.ReturnConnection(connection);
				}
			}
		}
    }
}
