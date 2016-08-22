﻿using Semanticer.Classifier;

namespace Semanticer.TextAnalyzer
{
    class PostStopWordsProvider: IStopWordProvider
    {
        public bool IsStopWord(string word)
        {
            return false; //Stop słowa zostały usunięte wcześniej.
        }
    }
}
