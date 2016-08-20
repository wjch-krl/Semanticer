using System.Collections.Generic;
using System.Linq;

namespace Semanticer.TextAnalyzer.SpellChekers
{
    public class DictionaryNode
    {
        public DictionaryNode(char letter)
        {
            Letter = letter;
            ChildNodes = new Dictionary<char, DictionaryNode>();
        }

        public bool IsWord { get; set; }
        public Dictionary<char, DictionaryNode> ChildNodes { get; }
        public char Letter { get; }

        public int Level
        {
            get { return 1 + (ChildNodes.Count == 0 ? 0 : ChildNodes.Max(x => x.Value.Level)); }
        }

        public bool HierachyEquals(DictionaryNode node)
        {
            return HierachyEquals(this, node);
        }

        private static bool HierachyEquals(DictionaryNode n1, DictionaryNode n2)
        {
            if (n2 == null || n1 == null)
            {
                return false;
            }
            if (n1.Letter != n2.Letter)
            {
                return false;
            }
            if (n1.ChildNodes.Count != n2.ChildNodes.Count)
            {
                return false;
            }
            if (n1.ChildNodes.Count == 0 && n2.ChildNodes.Count == 0)
            {
                return true;
            }
            if (n1.IsWord != n2.IsWord)
            {
                return false;
            }
            return !(from childNode1 in n1.ChildNodes
                let childNode2 = n2.ChildNodes.FirstOrDefault(x => x.Key == childNode1.Key).Value
                where !HierachyEquals(childNode1.Value, childNode2)
                select childNode1).Any();
        }
    }
}