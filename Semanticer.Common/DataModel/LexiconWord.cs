﻿using Semanticer.Common.Enums;

namespace Semanticer.Common.DataModel
{
    public class LexiconWord
    {
        private double wordMark;

        public double WordMark
        {
            get { return wordMark; }
            set
            {
                wordMark = value;
                MarkType = value.ToMarkType();                
            }
        }

        public double Propabilty { get; set; }
        public string Word { get; set; }
        public int Occurences { get; set; }
        public PartOfSpeech PartOfSpeech { get; set; }
        public int Id { get; set; }

        public MarkType MarkType { get; private set; }
    }
}
