using System;
using System.Collections.Generic;
using System.Linq;
using Semanticer.TextAnalyzer.Utilities;

namespace Semanticer.TextAnalyzer.SpellChekers
{
    public class DictionaryTree
    {
        public DictionaryNode RootNode { get; set; }
        public Boolean IsMutable { get; private set; }

        public DictionaryTree()
        {
            IsMutable = true;
            RootNode = new DictionaryNode('\0');
        }

        public void LoadWords(IEnumerable<string> words, int optimalizationLevel = 3)
        {
            if (!IsMutable)
            {
                throw new InvalidOperationException("Can't update dictionary after optimalization");
            }
            foreach (var word in words)
            {
                TryAdd(word);
            }
            Optimize(optimalizationLevel);
        }

        private void Optimize(int level)
        {
            if (level > 0)
            {
                IsMutable = false;
                Optimize(RootNode, new Dictionary<char, DictionaryNode>());
            }
            for (int i = 2; i < level; i++)
            {
                Optimize(RootNode, new List<DictionaryNode>(), i);
            }
        }

        private static void Optimize(DictionaryNode node, List<DictionaryNode> endNodes, int level)
        {
            foreach (var childNode in node.ChildNodes.Values.ToList())
            {
                if (childNode.Level == level)
                {
                    var newNode = endNodes.SingleOrDefault(x => x.HierachyEquals(childNode));
                    if (newNode != null)
                    {
                        node.ChildNodes[childNode.Letter] = newNode;
                    }
                    else
                    {
                        endNodes.Add(childNode);
                    }
                }
                else if (childNode.Level > level)
                {
                    Optimize(childNode, endNodes, level);
                }
            }
        }

        private static void Optimize(DictionaryNode node, Dictionary<char, DictionaryNode> endNodes)
        {
            foreach (var childNode in node.ChildNodes.Values.ToList())
            {
                if (childNode.ChildNodes.Count == 0)
                {
                    if (endNodes.ContainsKey(childNode.Letter))
                    {
                        node.ChildNodes[childNode.Letter] = endNodes[childNode.Letter];
                    }
                    else
                    {
                        endNodes.Add(childNode.Letter, childNode);
                    }
                }
                else
                {
                    Optimize(childNode, endNodes);
                }
            }
        }

        public bool TryAdd(string word)
        {
            bool added = false;
            DictionaryNode node = RootNode;
            foreach (var element in word)
            {
                if (!added && node.ChildNodes.ContainsKey(element))
                {
                    node = node.ChildNodes[element];
                }
                else
                {
                    var tmpNode = new DictionaryNode(element);
                    node.ChildNodes.Add(element, tmpNode);
                    node = tmpNode;
                    added = true;
                }
            }
            node.IsWord = true;
            return added;
        }

        public bool Spell(string word)
        {
            DictionaryNode node = RootNode;
            foreach (var element in word)
            {
                if (node.ChildNodes.ContainsKey(element))
                {
                    node = node.ChildNodes[element];
                }
                else
                {
                    return false;
                }
            }
            return node.IsWord;
        }

        internal IEnumerable<string> WordsWithRedundantLetters(string simpleWord)
        {
            List<string> buffer = new List<string>();
            simpleWord = simpleWord.ToLower();
            if (!RootNode.ChildNodes.ContainsKey(simpleWord[0]))
            {
                return buffer;
            }
            var node = RootNode.ChildNodes[simpleWord[0]];
            WordsWithRedundantLetters(simpleWord, 0, node, buffer, "");
            return buffer;
        }

        private static void WordsWithRedundantLetters(string word, int index, DictionaryNode node, List<string> buffer,
            string newWord)
        {
            index++;
            newWord += node.Letter;
            if (index < word.Length)
            {
                var currentLetter = word[index];
                var lastLetter = word[index - 1];
                if (node.ChildNodes.ContainsKey(currentLetter))
                {
                    WordsWithRedundantLetters(word, index, node.ChildNodes[currentLetter], buffer, newWord);
                }
                if (node.ChildNodes.ContainsKey(lastLetter))
                {
                    WordsWithRedundantLetters(word, index - 1, node.ChildNodes[lastLetter], buffer, newWord);
                }
            }
            if (node.IsWord && newWord.Length >= word.Length)
            {
                buffer.Add(newWord);
            }
        }

        public IEnumerable<string> WordsWithLetters(string word)
        {
            List<string> buffer = new List<string>();
            word = word.ToLower();
            if (!RootNode.ChildNodes.ContainsKey(word[0]))
            {
                return buffer;
            }
            var node = RootNode.ChildNodes[word[0]];
            WordsWithLetters(word, 0, node, buffer, "");
            return buffer;
        }

        private static void WordsWithLetters(string word, int index, DictionaryNode node, List<string> buffer,
            string newWord, bool checkGrandChildren = true)
        {
            index++;
            newWord += node.Letter;
            if (index < word.Length)
            {
                //wybranie aktualnej przetwarzanej litery
                var element = word[index];
                //sprawdzenie czy dana litera znajduje siê w wêŸle s³ownika wybranym na podstawie poprzednich liter
                if (node.ChildNodes.ContainsKey(element))
                {
                    //jeœli tak -> przetwarzanie w g³¹b wêz³a
                    WordsWithLetters(word, index, node.ChildNodes[element], buffer, newWord);
                }
                if (checkGrandChildren)
                {
                    // Jeœli tak w poszukiwaniu wyst¹pieñ danej litery przeszukiwane s¹ równie¿ wêz³y dwa poziomy w g³¹b
                    var possibleNodes = node.ChildNodes.Where(x => x.Value.ChildNodes.ContainsKey(element));
                    foreach (var nodeValuePair in possibleNodes)
                    {
                        //przetwarzanie w w g³¹b dla wêz³ów zawieraj¹cych litery -> bez uwzglêdniania wnuków
                        WordsWithLetters(word, index - 1, nodeValuePair.Value, buffer, newWord, false);
                    }
                }
            }
            if (node.IsWord && newWord.Length > word.Length)
            {
                //dodanie pasuj¹cego s³owa do wyniku
                buffer.Add(newWord);
            }
        }

        public string[] AllWords()
        {
            var buffer = new List<string>();
            foreach (var childNode in RootNode.ChildNodes)
            {
                AllWords(buffer, childNode.Value, string.Empty);
            }
            return buffer.ToArray();
        }

        private static void AllWords(List<string> buffer, DictionaryNode node, string newWord)
        {
            newWord += node.Letter;
            foreach (var childNode in node.ChildNodes)
            {
                AllWords(buffer, childNode.Value, newWord);
            }
            if (node.IsWord)
            {
                buffer.Add(newWord);
            }
        }

        public IEnumerable<string> CorrectSpelling(string misSpelled)
        {
            var buffer = new List<string>();
            foreach (var childNode in RootNode.ChildNodes)
            {
                CorrectSpelling(buffer, childNode.Value, string.Empty, int.MaxValue, int.MaxValue, misSpelled);
            }
            return buffer;
        }

        //TODO dodaæ obs³ugê najczêstszych b³êdów np ¿ -> rz
        private static void CorrectSpelling(List<string> buffer, DictionaryNode node, string newWord, int lastDistance,
            int historyDistance, string misSpelled)
        {
            newWord += node.Letter;
            var distance = WordsHelper.ComputeLevenshteinDistance(newWord, misSpelled);
            foreach (var childNode in node.ChildNodes)
            {
                if (distance > lastDistance || (lastDistance == historyDistance && distance == historyDistance))
                {
                    return;
                }
                CorrectSpelling(buffer, childNode.Value, newWord, distance, lastDistance, misSpelled);
            }
            if (node.IsWord && distance < 2)
            {
                buffer.Add(newWord);
            }
        }
    }
}