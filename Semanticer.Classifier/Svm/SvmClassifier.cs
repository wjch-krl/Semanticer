using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LibSVMsharp;
using Semanticer.Classifier.Common;
using Semanticer.Common.Utils;

namespace Semanticer.Classifier.Svm
{
    /// <summary>
    /// Klasyfikator wykorzystujący algorytm Maszyny wektorów nośnych
    /// </summary>
    public class SvmClassifier : SvmClassifierBase
    {
        private const int WordTreshold = 0;

        public SvmClassifier(ITokenizer tokenizer, string langCode, bool useRegresion = false)
            : base(tokenizer, langCode, useRegresion)
        {
            //  DeserializeHelperData();
        }

        private Dictionary<string, int> DeserializeWordDict()
        {
            XElement xElem2;
            using (var file = File.OpenRead(DictPath))
            {
                xElem2 = XElement.Load(file);
            }
            return xElem2.Descendants("item")
                .ToDictionary(key => (string) key.Attribute("id"), value => int.Parse(value.Value));
        }

        protected void DeserializeHelperData()
        {
            WordDict = DeserializeWordDict();
            Classifier = SVM.LoadModel(ModelPath);
        }

        private void CreateTrainFile(List<KeyValuePair<string, double>> trainMessages, string path)
        {
            var wordsMartrix = trainMessages.Select(x => x.Key)
                .ToArray();
            WriteTrainFile(trainMessages, wordsMartrix, path);
        }

        private void WriteTrainFile(List<KeyValuePair<string, double>> trainMessages, string[] wordsMartrix, string path)
        {
            int index = 0;
            WordDict = wordsMartrix.SelectMany(x => x.SplitByWhitespaces())
                .GroupBy(x => x)
                .Where(x => x.Count() > WordTreshold)
                .ToDictionary(x => x.Key, y => index++);
            var transformed = TransformToWrite(wordsMartrix);
            using (var file = new StreamWriter(path, false))
            {
                for (int i = 0; i < trainMessages.Count; i++)
                {
                    var element = trainMessages[i];
                    file.Write(element.Value);
                    foreach (var value in transformed[i])
                    {
                        file.Write(string.Format(CultureInfo.InvariantCulture, " {0}:{1}", value.Key + 1, value.Value));
                    }
                    file.WriteLine();
                }
            }
        }

        protected override SVMProblem CreateTrainProblem(List<KeyValuePair<string, double>> trainMessages)
        {
            var wordsMartrix = trainMessages.Select(x => x.Key)
                .ToArray();
            int index = 1;
            WordDict = wordsMartrix.SelectMany(x => x.SplitByWhitespaces())
                .GroupBy(x => x)
                .Where(x => x.Count() > WordTreshold).ToDictionary(x => x.Key, y => index++);
            SVMProblem problem = new SVMProblem();
            var transformed = TransformToWrite(wordsMartrix);
            for (int i = 0; i < trainMessages.Count; i++)
            {
                var element = trainMessages[i];
                var nodes = transformed[i].Select(x => new SVMNode(x.Key, x.Value));
                problem.Add(nodes.ToArray(), element.Value);
            }
            CreateTrainFile(trainMessages, TrainFilePath);
            return problem;
        }

        protected override void SerializeHelper()
        {
            SerializeWordDict(WordDict);
            SVM.SaveModel(Classifier, ModelPath);
        }

        private void SerializeWordDict(Dictionary<string, int> wordDictionary)
        {
            var xml = new XElement("WordDictionary", wordDictionary.Select(
                x => new XElement("item", new XAttribute("id", x.Key), new XElement("value", x.Value))));
            using (var file = new FileStream(DictPath, FileMode.Create, FileAccess.Write))
            {
                xml.Save(file);
            }
        }

        private IList<Dictionary<int, double>> TransformToWrite(string[] wordsMartrix)
        {
            var problem = new List<Dictionary<int, double>>();
            foreach (var msg in wordsMartrix)
            {
                var valeus = new Dictionary<int, double>();
                string[] splitted = msg.SplitByWhitespaces();
                foreach (var word in splitted)
                {
                    //Jeśli słowo nie występuje w słowniku przejdź do kolejnego
                    if (!WordDict.ContainsKey(word))
                        continue;
                    //Pobranie id danej cechy (słowa)
                    int id = WordDict[word];
                    //Jeśli słowo zostało już dodane -> przejście do kolejnej iteracji
                    if (valeus.ContainsKey(id))
                        continue;
                    //Obliczenie częstości występowania słowa w zbiorze
                    double wordFrequency = (double) splitted.Count(x => x == word)/splitted.Length;
                    valeus.Add(id, wordFrequency);
                }
                problem.Add(valeus);
            }
            return problem;
        }

        /// <summary>
        /// Dokonuje transformacji dokumentów na macierz zawierającą znormalizowane częstości występowania słów
        /// </summary>
        /// <param name="wordsMartrix"></param>
        /// <returns></returns>
        protected override List<List<double>> Transform(string[] wordsMartrix)
        {
            var problem = new List<List<double>>();
            int j = 0;
            foreach (var msg in wordsMartrix)
            {
                var valeus = new Dictionary<int, double>();
                var splitted = msg.SplitByWhitespaces();
                foreach (var word in splitted)
                {
                    //Jeśli słowo nie występuje w słowniku przejdź do kolejnego
                    if (!WordDict.ContainsKey(word))
                        continue;
                    //Pobranie id danej cechy (słowa)
                    int id = WordDict[word];
                    //Jeśli słowo zostało już dodane -> przejście do kolejnej iteracji
                    if (valeus.ContainsKey(id))
                        continue;
                    //Obliczenie częstości występowania słowa w zbiorze
                    double wordFrequency = (double) splitted.Count(x => x == word)/splitted.Length;
                    valeus.Add(id, wordFrequency);
                }
                problem.Add(new List<double>());
                //Przypisanie wartości poszczególnym cechom
                for (int i = 0; i < WordDict.Count; i++)
                {
                    problem[j].Add(valeus.ContainsKey(i) ? valeus[i] : 0);
                }
                j++;
            }
            return problem;
        }
    }
}