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

using System;
using System.Collections.Generic;
using Semanticer.Classifier.Common;
using Semanticer.Common.Enums;

namespace Semanticer.Classifier.Bayesian
{
    public class BayesianClassifier : ClassifierBase, ITrainableClassifier
    {
        private readonly IPivotWordProvider pivotWordProvider;
        private readonly bool categorized;
        public bool IsCaseSensitive { get; set; }
        public IWordsDataSource WordsDataSource { get; private set; }
        public ITokenizer Tokenizer { get; private set; }
        public IStopWordProvider StopWordProvider { get; private set; }

        public BayesianClassifier(IWordsDataSource wd, ITokenizer tokenizer, IPivotWordProvider pwProvider)
            : this(wd, tokenizer, new DefaultStopWordProvider(), pwProvider)
        {
        }

        public BayesianClassifier(IWordsDataSource wd, ITokenizer tokenizer, IStopWordProvider swp,
            IPivotWordProvider pwProvider, bool caseSensitive = false, bool categorized = true)
        {
            IsCaseSensitive = caseSensitive;
            WordsDataSource = wd;
            Tokenizer = tokenizer;
            StopWordProvider = swp;
            pivotWordProvider = pwProvider;
            this.categorized = categorized;
            if (categorized)
            {
                CheckCategoriesSupported();
            }
        }

        public override IDictionary<PostMarkType, double> Classify(string input)
        {
            if (!categorized)
            {
                var matchPropabilty = Classify(CategorizedClassifierConstants.DefaultCategory, input);
                return new Dictionary<PostMarkType, double>
                {
                    {PostMarkType.Positive, matchPropabilty},
                    {PostMarkType.Neutral, -Math.Abs(2*matchPropabilty - 1) + 1},
                    {PostMarkType.Negative, 1 - matchPropabilty}
                };
            }
            var positivePropabilty = Classify(PostMarkType.Positive.ToString(), input);
            return new Dictionary<PostMarkType, double>
            {{PostMarkType.Positive, positivePropabilty},
                {PostMarkType.Neutral, Classify(PostMarkType.Neutral.ToString(), input)},
                {PostMarkType.Negative, 1 - positivePropabilty}
            };
        }

        public TimeSpan ReTrain(ITrainingData trainingData) 
        {
            var startTime = DateTime.Now;
//            if (trainingData.LoadWords)
//            {
//                WordsDataSource.Clear();
//                var words = trainingData.DatabaseProvider.AllWords(1,1);
//
//                foreach (var word in words)
//                {
//                    for (int i = 0; i < Math.Abs(word.WordMark); i++)
//                    {
//                        if (word.MarkType == PostMarkType.Negative)
//                        {
//                            if (categorized)
//                            {
//                                TeachMatch(PostMarkType.Negative.ToString(), word.Word);
//                                TeachNonMatch(PostMarkType.Positive.ToString(), word.Word);
//                            }
//                            else
//                            {
//                                TeachNonMatch(CategorizedClassifierConstants.DefaultCategory, word.Word);                                
//                            }
//                        }
//                        else
//                        {
//                            if (categorized)
//                            {
//                                TeachMatch(PostMarkType.Positive.ToString(), word.Word);
//                                TeachNonMatch(PostMarkType.Negative.ToString(), word.Word);
//                            }
//                            else
//                            {
//                                TeachMatch(CategorizedClassifierConstants.DefaultCategory, word.Word);                                
//                            }
//                        }
//                    }
//                    if (word.MarkType == PostMarkType.Neutral && categorized)
//                    {
//                        TeachMatch(word.MarkType.ToString(), word.Word);
//                    }
//                }
//            }
            var reader = trainingData.Reader;
            while (reader.HasNext())
            {
                var input = reader.ReadNextEvent();
                var type = (PostMarkType)Enum.Parse(typeof(PostMarkType), input.Outcome);
                if (type == PostMarkType.Negative)
                {
                    if (categorized)
                    {
                        TeachNonMatch(PostMarkType.Positive.ToString(), input.GetContext());
                        TeachNonMatch(PostMarkType.Neutral.ToString(), input.GetContext());
                    }
                    else
                    {
                        TeachNonMatch(CategorizedClassifierConstants.DefaultCategory, input.GetContext());
                    }
                }
                if (type == PostMarkType.Positive)
                {
                    if (categorized)
                    {
                        TeachMatch(PostMarkType.Positive.ToString(), input.GetContext());
                        TeachNonMatch(PostMarkType.Neutral.ToString(), input.GetContext());
                    }
                    else
                    {
                        TeachMatch(CategorizedClassifierConstants.DefaultCategory, input.GetContext());
                    }
                }
                if (type == PostMarkType.Neutral && categorized)
                {
                    TeachMatch(PostMarkType.Neutral.ToString(), input.GetContext());
                    TeachNonMatch(PostMarkType.Positive.ToString(), input.GetContext());
                }
            }
            return DateTime.Now - startTime;
        }

        public double Classify(string category, string input)
        {
            if (category == null)
                throw new ArgumentNullException("Category cannot be null.");
            if (input == null)
                throw new ArgumentNullException("Input cannot be null.");

            return Classify(category, Tokenizer.Tokenize(input));
        }

        public double Classify(string category, string[] words)
        {
            var wps = CalcWordsProbability(category, words);
            return NormalizeSignificance(CalculateOverallProbability(wps));
        }

        public void TeachMatch(string input)
        {
            TeachMatch(CategorizedClassifierConstants.DefaultCategory, input);
        }

        public void TeachNonMatch(string input)
        {
            TeachNonMatch(CategorizedClassifierConstants.DefaultCategory, input);
        }

        public void TeachMatch(string category, string input)
        {
            if (category == null)
                throw new ArgumentNullException("Category cannot be null.");
            if (input == null)
                throw new ArgumentNullException("Input cannot be null.");

            TeachMatch(category, Tokenizer.Tokenize(input));
        }

        public void TeachNonMatch(string category, string input)
        {
            if (category == null)
                throw new ArgumentNullException("Category cannot be null.");
            if (input == null)
                throw new ArgumentNullException("Input cannot be null.");

            TeachNonMatch(category, Tokenizer.Tokenize(input));
        }

        public bool IsMatch(string category, string[] input)
        {
            if (category == null)
                throw new ArgumentNullException("Category cannot be null.");
            if (input == null)
                throw new ArgumentNullException("Input cannot be null.");

            double matchProbability = Classify(category, input);

            return (matchProbability >= Cutoff);
        }

        /// <summary>
        /// Uczy przynale¿noœci s³ów do danej kategorii
        /// </summary>
        /// <param name="category">Kategoria</param>
        /// <param name="words">Pasuj¹ce s³owa</param>
        public void TeachMatch(string category, string[] words)
        {
            int pivotRange = 1;
            double multiper = 0.0;
            //Dla ka¿dego s³owa
            foreach (string element in words)
            {
                //jeœli s³owo zmienia znaczenie s¹siadów zapisanie mno¿nika
                if (pivotWordProvider.IsPivot(element))
                {
                    pivotRange = 2;
                    multiper = pivotWordProvider.Multiper(element);
                    continue;
                }
                //Jeœli s³owo mo¿e byæ klasyfikowane tzn. nie jest wyrazem pomijalnym
                if (IsClassifiableWord(element))
                {
                    if (pivotRange-- > 0 && multiper < 0.0)
                    {
                        //jeœli negacja dodanie slowa jako nie pasuj¹cego do kategorii
                        if (categorized)
                            ((ICategorizedWordsDataSource) WordsDataSource).AddNonMatch(category, TransformWord(element));
                        else
                            WordsDataSource.AddNonMatch(TransformWord(element));
                    }
                    else
                    {
                        //dodanie slowa jako pasuj¹cego do kategorii
                        if (categorized)
                            ((ICategorizedWordsDataSource) WordsDataSource).AddMatch(category, TransformWord(element));
                        else
                            WordsDataSource.AddMatch(TransformWord(element));
                    }
                }
            }
        }

        public void TeachNonMatch(string category, string[] words)
        {
            int pivotRange = 1;
            double multiper = 0.0;
            foreach (string element in words)
            {
                if (pivotWordProvider.IsPivot(element))
                {
                    pivotRange = 2;
                    multiper = pivotWordProvider.Multiper(element);
                    continue;
                }
                if (IsClassifiableWord(element))
                {
                    if (pivotRange-- > 0 && multiper < 0.0)
                    {
                        if (categorized)
                            ((ICategorizedWordsDataSource) WordsDataSource).AddMatch(category, TransformWord(element));
                        else
                            WordsDataSource.AddMatch(TransformWord(element));
                    }
                    else
                    {
                        if (categorized)
                            ((ICategorizedWordsDataSource) WordsDataSource).AddNonMatch(category, TransformWord(element));
                        else
                            WordsDataSource.AddNonMatch(TransformWord(element));
                    }
                }
            }
        }

        /// <summary>
        /// Allows transformations to be done to the given word.
        /// </summary>
        /// <param name="word">The word to transform.</param>
        /// <returns>The transformed word.</returns>
        public string TransformWord(string word)
        {
            if (word != null)
            {
                if (!IsCaseSensitive)
                    return word.ToLower();
                return word;
            }
            throw new ArgumentNullException("Word cannot be null.");
        }

        public double CalculateOverallProbability(WordProbability[] wps)
        {
            if (wps == null || wps.Length == 0)
                return ClassifierConstants.NeutralProbability;

            // we need to calculate xy/(xy + z) where z = (1 - x)(1 - y)

            // first calculate z and xy
            double z = 0d;
            double xy = 0d;
            foreach (WordProbability probability in wps)
            {
                if (z == 0)
                    z = (1 - probability.Probability);
                else
                    z = z*(1 - probability.Probability);

                if (xy == 0)
                    xy = probability.Probability;
                else
                    xy = xy*probability.Probability;
            }

            double numerator = xy;
            double denominator = xy + z;

            return numerator/denominator;
        }

        private WordProbability[] CalcWordsProbability(string category, string[] words)
        {
            if (category == null)
                throw new ArgumentNullException("Category cannot be null.");

            if (words == null)
                return new WordProbability[0];
            var wps = new List<WordProbability>();
            int pivotRange = 0;
            double multiper = 1;
            foreach (string word in words)
            {
                if (pivotWordProvider.IsPivot(word))
                {
                    pivotRange = 2;
                    multiper = pivotWordProvider.Multiper(word);
                    continue;
                }
                if (IsClassifiableWord(word))
                {
                    WordProbability wp = categorized
                        ? ((ICategorizedWordsDataSource) WordsDataSource).GetWordProbability(category,
                            TransformWord(word))
                        : WordsDataSource.GetWordProbability(TransformWord(word));
                    if (pivotRange-- > 0)
                    {
                        wp.Probability *= multiper; // > 0 ? multiper : (-1/multiper);
                    }
                    if (wp != null)
                        wps.Add(wp);
                }
            }
            return wps.ToArray();
        }

        private void CheckCategoriesSupported()
        {
            // if the category is not the default
            if (!(WordsDataSource is ICategorizedWordsDataSource))
                throw new ArgumentException("Word Data Source does not support categories.");
        }

        private bool IsClassifiableWord(string word)
        {
            return !string.IsNullOrEmpty(word) && !StopWordProvider.IsStopWord(word);
        }

        public static double NormalizeSignificance(double sig)
        {
            if (ClassifierConstants.UpperBound < sig)
                return ClassifierConstants.UpperBound;
            if (ClassifierConstants.LowerBound > sig)
                return ClassifierConstants.LowerBound;
            return sig;
        }
    }
}