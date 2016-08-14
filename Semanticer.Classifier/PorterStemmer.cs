#region Copyright (c) 2004, Ryan Whitaker
/*********************************************************************************
'
' Copyright (c) 2004 Ryan Whitaker
'
' This software is provided 'as-is', without any express or implied warranty. In no 
' event will the authors be held liable for any damages arising from the use of this 
' software.
' 
' Permission is granted to anyone to use this software for any purpose, including 
' commercial applications, and to alter it and redistribute it freely, subject to the 
' following restrictions:
'
' 1. The origin of this software must not be misrepresented; you must not claim that 
' you wrote the original software. If you use this software in a product, an 
' acknowledgment (see the following) in the product documentation is required.
'
' This product uses software written by the developers of NClassifier
' (http://nclassifier.sourceforge.net).  NClassifier is a .NET port of the Nick
' Lothian's Java text classification engine, Classifier4J 
' (http://classifier4j.sourceforge.net).
'
' 2. Altered source versions must be plainly marked as such, and must not be 
' misrepresented as being the original software.
'
' 3. This notice may not be removed or altered from any source distribution.
'
'********************************************************************************/
#endregion

namespace Semanticer.Classifier
{
	/// <summary>
	/// Stemmer, implementing the Porter Stemming algorithm
	/// </summary>
	public class PorterStemmer
	{
		char[] wordToStem;
		int i, // offset into _wordToStem
			iEnd, // offset to end of stemmed word
			j, k;

		const int Inc = 50;

		public int ResultLength => iEnd;
	    public char[] ResultBuffer => wordToStem;

	    public PorterStemmer()
		{
			wordToStem = new char[Inc];
			i = 0;
			iEnd = 0;
		}

		/// <summary>
		/// Add a character to the word being stemmed.  When you are finished adding
		/// characters, you can call stem() to stem the word.
		/// </summary>
		public void Add(char ch)
		{
			if (i == wordToStem.Length)
			{
				char[] newB = new char[i + Inc];
				for (int c = 0; c < i; c++)
					newB[c] = wordToStem[c];
				wordToStem = newB;
			}
			wordToStem[i++] = ch;
		}

		/// <summary>
		/// After a word has been stemmed, it can be retrieved by Tostring(), or a
		/// reference to the internal buffer can be retrieved by the ResultBuffer
		/// and ResultLength properties (which is generally more efficient).
		/// </summary>
		public override string ToString()
		{
			return new string(wordToStem, 0, iEnd);
		}

		private bool Cons(int index)
		{
			switch (wordToStem[this.i])
			{
				case 'a' :
				case 'e' :
				case 'i' :
				case 'o' :
				case 'u' : 
					return false;
				case 'y' :
					return (this.i == 0) || !Cons(this.i - 1);
				default :
					return true;
			}
		}

		/// <summary>
		/// Measures the number of consonant sequences between 0 and j.  If c is a consonant
		/// sequence and v a vowel sequence, and <..> indicates arbitrary presence,
		/// 
		///		<c><v>			gives 0
		///		<c>vc<v>		gives 1
		///		<c>vCvc<v>		gives 2
		///		<c>vCvcvc<v>	gives 3
		///		....
		///		
		/// </summary>
		private int M()
		{
			int n = 0;
			int i = 0;
			while (true)
			{
				if (i > j)
					return n;
				if (!Cons(i))
					break;
				i++;
			}
			i++;
			while (true) 
			{
				while (true) 
				{
					if (i > j) 
					{                
						return n;
					}
					if (Cons(i)) 
					{
						break;
					}                    
					i++;
				}
				i++;
				n++;
				while (true) 
				{
					if (i > j) 
					{                
						return n;
					}
					if (Cons(i)) 
					{
						break;
					}                    
					i++;
				}
				i++;
			}
		}

		/* VowelInStem() is true <=> 0,...j contains a vowel */
		private bool VowelInStem() 
		{
			int i;
			for (i = 0; i <= j; i++) 
			{
				if (!Cons(i)) 
				{
					return true;
				}
			}

			return false;
		}

		/* DoubleC(j) is true <=> j,(j-1) contain a double consonant. */
		private bool DoubleC(int j)
		{
		    if (j < 1) 
			{
				return false;
			}
		    if (wordToStem[j] != wordToStem[j - 1]) 
		    {
		        return false;
		    }
		    return Cons(j);
		}

	    /* Cvc(i) is true <=> i-2,i-1,i has the form consonant - vowel - consonant
		   and also if the second c is not w,x or y. this is used when trying to
		   restore an e at the end of a short word. e.g.
    
			  cav(e), lov(e), hop(e), crim(e), but
			  snow, box, tray.
    
		*/
		private bool Cvc(int i) 
		{
			if (i < 2 || !Cons(i) || Cons(i - 1) || !Cons(i - 2)) 
			{
				return false;
			}
		    int ch = wordToStem[i];
		    if (ch == 'w' || ch == 'x' || ch == 'y') 
		    {
		        return false;
		    }
		    return true;
		}

		private bool End(string s) 
		{
			int l = s.Length;
			int o = k - l + 1;
			if (o < 0) 
			{
				return false;
			}

			for (int i = 0; i < l; i++) 
			{
				if (wordToStem[o + i] != s[i]) 
				{
					return false;            
				}                
			} 
			j = k - l;
			return true;
		}

		/* setto(s) sets (j+1),...k to the characters in the string s, readjusting
		   k. */
		private void Setto(string s) 
					  {
						  int l = s.Length;
						  int o = j + 1;
						  for (int i = 0; i < l; i++)
							  wordToStem[o + i] = s[i];
						  k = j + l;
					  }

		/* r(s) is used further down. */
		private void R(string s) 
					  {
						  if (M() > 0)
							  Setto(s);
					  }

		/* Step1() gets rid of plurals and -ed or -ing. e.g.
    
			   caresses  ->  caress
			   ponies    ->  poni
			   ties      ->  ti
			   caress    ->  caress
			   cats      ->  cat
    
			   feed      ->  feed
			   agreed    ->  agree
			   disabled  ->  disable
    
			   matting   ->  mat
			   mating    ->  mate
			   meeting   ->  meet
			   milling   ->  mill
			   messing   ->  mess
    
			   meetings  ->  meet
    
		*/
		private void Step1() 
					  {
						  if (wordToStem[k] == 's') 
						  {
							  if (End("sses"))
								  k -= 2;
							  else if (End("ies"))
								  Setto("i");
							  else if (wordToStem[k - 1] != 's')
								  k--;
						  }
						  if (End("eed")) 
						  {
							  if (M() > 0)
								  k--;
						  } 
						  else if ((End("ed") || End("ing")) && VowelInStem()) 
						  {
							  k = j;
							  if (End("at"))
								  Setto("ate");
							  else if (End("bl"))
								  Setto("ble");
							  else if (End("iz"))
								  Setto("ize");
							  else if (DoubleC(k)) 
							  {
								  k--;
							  {
								  int ch = wordToStem[k];
								  if (ch == 'l' || ch == 's' || ch == 'z')
									  k++;
							  }
							  } 
							  else if (M() == 1 && Cvc(k))
								  Setto("e");
						  }
					  }

		/* Step2() turns terminal y to i when there is another vowel in the stem. */
		private void Step2() 
					  {
						  if (End("y") && VowelInStem())
							  wordToStem[k] = 'i';
					  }

		/* Step3() maps double suffices to single ones. so -ization ( = -ize plus
		   -ation) maps to -ize etc. note that the string before the suffix must give
		   m() > 0. */

		private void Step3() 
		{
			if (k == 0)
				return; /* For Bug 1 */
			switch (wordToStem[k - 1]) 
			{
				case 'a' :
					if (End("ational")) 
					{
						R("ate");
						break;
					}
					if (End("tional")) 
					{
						R("tion");
					}
					break;
				case 'c' :
					if (End("enci")) 
					{
						R("ence");
						break;
					}
					if (End("anci")) 
					{
						R("ance");
					}
					break;
				case 'e' :
					if (End("izer")) 
					{
						R("ize");
					}
					break;
				case 'l' :
					if (End("bli")) 
					{
						R("ble");
						break;
					}
					if (End("alli")) 
					{
						R("al");
						break;
					}
					if (End("entli")) 
					{
						R("ent");
						break;
					}
					if (End("eli")) 
					{
						R("e");
						break;
					}
					if (End("ousli")) 
					{
						R("ous");
					}
					break;
				case 'o' :
					if (End("ization")) 
					{
						R("ize");
						break;
					}
					if (End("ation")) 
					{
						R("ate");
						break;
					}
					if (End("ator")) 
					{
						R("ate");
					}
					break;
				case 's' :
					if (End("alism")) 
					{
						R("al");
						break;
					}
					if (End("iveness")) 
					{
						R("ive");
						break;
					}
					if (End("fulness")) 
					{
						R("ful");
						break;
					}
					if (End("ousness")) 
					{
						R("ous");
					}
					break;
				case 't' :
					if (End("aliti")) 
					{
						R("al");
						break;
					}
					if (End("iviti")) 
					{
						R("ive");
						break;
					}
					if (End("biliti")) 
					{
						R("ble");
					}
					break;
				case 'g' :
					if (End("logi")) 
					{
						R("log");
					}
					break;
			}
		}

		/* Step4() deals with -ic-, -full, -ness etc. similar strategy to Step3. */

		private void Step4() 
		{
			switch (wordToStem[k]) 
			{
				case 'e' :
					if (End("icate")) 
					{
						R("ic");
						break;
					}
					if (End("ative")) 
					{
						R("");
						break;
					}
					if (End("alize")) 
					{
						R("al");
					}
					break;
				case 'i' :
					if (End("iciti")) 
					{
						R("ic");
					}
					break;
				case 'l' :
					if (End("ical")) 
					{
						R("ic");
						break;
					}
					if (End("ful")) 
					{
						R("");
					}
					break;
				case 's' :
					if (End("ness")) 
					{
						R("");
					}
					break;
			}
		}

		/* Step5() takes off -ant, -ence etc., in context <c>vCvc<v>. */

		private void Step5() 
		{
			if (k == 0)
				return; /* for Bug 1 */
			switch (wordToStem[k - 1]) 
			{
				case 'a' :
					if (End("al"))
						break;
					return;
				case 'c' :
					if (End("ance"))
						break;
					if (End("ence"))
						break;
					return;
				case 'e' :
					if (End("er"))
						break;
					return;
				case 'i' :
					if (End("ic"))
						break;
					return;
				case 'l' :
					if (End("able"))
						break;
					if (End("ible"))
						break;
					return;
				case 'n' :
					if (End("ant"))
						break;
					if (End("ement"))
						break;
					if (End("ment"))
						break;
					/* element etc. not stripped before the m */
					if (End("ent"))
						break;
					return;
				case 'o' :
					if (End("ion") && j >= 0 && (wordToStem[j] == 's' || wordToStem[j] == 't'))
						break;
					/* j >= 0 fixes Bug 2 */
					if (End("ou"))
						break;
					return;
					/* takes care of -ous */
				case 's' :
					if (End("ism"))
						break;
					return;
				case 't' :
					if (End("ate"))
						break;
					if (End("iti"))
						break;
					return;
				case 'u' :
					if (End("ous"))
						break;
					return;
				case 'v' :
					if (End("ive"))
						break;
					return;
				case 'z' :
					if (End("ize"))
						break;
					return;
				default :
					return;
			}
			if (M() > 1)
				k = j;
		}

		/* Step6() removes a -e if m() > 1. */

		private void Step6() 
		{
			j = k;
			if (wordToStem[k] == 'e') 
			{
				int a = M();
				if (a > 1 || a == 1 && !Cvc(k - 1))
					k--;
			}
			if (wordToStem[k] == 'l' && DoubleC(k) && M() > 1)
				k--;
		}

		/** Stem the word placed into the Stemmer buffer through calls to add().
		 * Returns true if the stemming process resulted in a word different
		 * from the input.  You can retrieve the result with
		 * getResultLength()/getResultBuffer() or tostring().
		 */
		public void Stem() 
		{
			k = i - 1;
			if (k > 1) 
			{
				Step1();
				Step2();
				Step3();
				Step4();
				Step5();
				Step6();
			}
			iEnd = k + 1;
			i = 0;
		}
	}
}