using System;
using Semanticer.Classifier;

namespace Semanticer.TextAnalyzer.Utilities
{
    public class BigramPostTokenizer : ITokenizer
    {
        public string[] Tokenize(string input)
        {
            var splitted = input.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            return CreateBigram(splitted);
        }

        //Funkcja tworzy bigramy na podstawie zadanych slow
        private static string[] CreateBigram(string[] splitted)
        {
            if (splitted.Length == 0)
            {
                return splitted;
            }
            //Jest 2*N - 1 bigramow, gdzie N to liczba unigramow (rozmiar tablicy splitted)
            var bigram = new string[splitted.Length*2 - 1];
            string lastWord = null;
            int i = 0;
            //Tworzenie bigramu - polaczenie akualnego slowa z poprzednim
            foreach (var element in splitted)
            {
                if (lastWord != null)
                {
                    bigram[i++] = string.Join(" ", lastWord, element);
                }
                lastWord = element;
                bigram[i++] = element;
            }
            return bigram;
        }
    }
}