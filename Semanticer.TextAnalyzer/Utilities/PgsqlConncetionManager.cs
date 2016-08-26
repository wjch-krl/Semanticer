using System.Data;
using Npgsql;
using Semanticer.Classifier.Textual.Bayesian;

namespace Semanticer.TextAnalyzer.Utilities
{
    class PgsqlConncetionManager : IDbConnectionManager
    {
        private readonly string connectionString;

        public PgsqlConncetionManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return connection;
        }
        
        public void ReturnConnection(IDbConnection connection)
        {
            if (connection != null)
            {
                connection.Close();
            }
        }
    }
}
