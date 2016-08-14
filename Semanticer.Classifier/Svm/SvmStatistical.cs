using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using LibSVMsharp;
using LibSVMsharp.Helpers;
using Semanticer.Classifier.Common;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;
using Semanticer.Common.Utils;

namespace Semanticer.Classifier.Svm
{
    /// <summary>
    /// Klasyfikator wykorzystujący algorytm Maszyny wektorów nośnych
    /// </summary>
    public class SvmStatisticalClassifier : SvmClassifierBase
    {
        private readonly string lang;
        private readonly int tradeId;
        private IDictionary<int, UsedStats> usedStats;
        private string usedStatsPath;
        private string scaleFilePath;
        private bool tuneMinMax;
        private NoteProvider noteProvider;
        private const double Eps = 0.00000001;

        public SvmStatisticalClassifier(ITokenizer tokenizer, string lang, int tradeId, ITextAnalizerDataProvider dbProvider,
            IPivotWordProviderFactory pivotFactory, bool useRegresion = false, bool forceLoad = false)
            : base(tokenizer, string.Format("{0}_{1}", lang, tradeId), useRegresion)
        {
            usedStatsPath = String.Format("UsedStats_{0}.xml", TradeLang);
            scaleFilePath = String.Format("Svm_{0}.scale", TradeLang);

            this.lang = lang;
            this.tradeId = tradeId;
            noteProvider = new NoteProvider(dbProvider, pivotFactory);
            CreateHelperData(forceLoad);
            MatchCutoff = 0.6;
        }
        
        protected void CreateHelperData(bool forceLoad)
        {
            if (File.Exists(usedStatsPath))
            {
                DeserializeHelperData();
            }
            else
            {
                usedStats = CreateUsedStats();
                tuneMinMax = true;
                Scale = new double[usedStats.Count,2];
            }
            if (forceLoad)
                Classifier = SVM.LoadModel(ModelPath);
        }

        private void DeserializeHelperData()
        {
            XmlSerializer serializer = new XmlSerializer(typeof (UsedStatDictItem[]));
            using (var stream = new FileStream(usedStatsPath, FileMode.Open))
            {
                var stats = (UsedStatDictItem[]) serializer.Deserialize(stream);
                usedStats = stats.ToDictionary(key => key.Id, value => value.MinMax);
                tuneMinMax = false;
            }
            Scale = ReadScaleData();
        }

        private static Dictionary<int, UsedStats> CreateUsedStats()
        {
            return new Dictionary<int, UsedStats>
            {
                {3, new UsedStats()},
                {4, new UsedStats()},
                {5, new UsedStats()},
                {9, new UsedStats()},
                {10, new UsedStats()},
            };
        }

        /// <summary>
        /// Tworzy plik reprezentujący dane treningowe
        /// </summary>
        /// <param name="trainMessages"></param>
        /// <param name="path"></param>
        /// <param name="loadWords"></param>
        /// <param name="databaseProvider"></param>
        protected void CreateTrainFile(List<KeyValuePair<string, double>> trainMessages, string path,
            bool loadWords, ITextAnalizerDataProvider databaseProvider)
        {
            SVMProblemHelper.Save(CreateTrainProblem(trainMessages, loadWords, databaseProvider), path);
        }

        public DiagnosticLogElement LoadFromR(IDictionary<int, UsedStats> stats)
        {
            DateTime starTime = DateTime.Now;
            Classifier = SVM.LoadModel(ModelPath);
            var problem = SVMProblemHelper.Load(TrainFilePath);
            usedStats = stats;
            Scale = ReadScaleData();
            XmlSerializer serializer = new XmlSerializer(typeof (UsedStatDictItem[]));
            var statArray = stats.Select(x => new UsedStatDictItem {Id = x.Key, MinMax = x.Value}).ToArray();
            using (var stream = new FileStream(usedStatsPath, FileMode.Create))
            {
                serializer.Serialize(stream, statArray);
            }
            tuneMinMax = false;
            var trainTime = DateTime.Now - starTime;
//            int[,] confusionMatrix;
//            ScaleProblem(problem);
//            double[] testResults = problem.Predict(Classifier);
//
//            double testAccuracy = problem.EvaluateClassificationProblem(testResults, Classifier.Labels, out confusionMatrix);         
//            foreach (int t in Classifier.Labels)
//                Console.Write(@"{0,5}", string.Format("({0})", t));
//            Console.WriteLine();
//            for (int i = 0; i < confusionMatrix.GetLength(0); i++)
//            {
//                Console.Write(@"{0,5}", string.Format("({0})", Classifier.Labels[i]));
//                for (int j = 0; j < confusionMatrix.GetLength(1); j++)
//                    Console.Write(@"{0,5}", confusionMatrix[j, i]);
//                Console.WriteLine();
//            }
            return new DiagnosticLogElement
            {
                Processed = problem.Length,
                CompletitionTime = trainTime,
                JobType = LoggerEventType.ClassifierTrainingStop,
                Date = DateTime.UtcNow,
            };
        }

        private double[,] ReadScaleData()
        {
            double[,] data = null;
            using (var reader = new StreamReader(scaleFilePath))
            {
                string file = reader.ReadToEnd();
                var rows = file.Split(new[] {'\r', '\n'},StringSplitOptions.RemoveEmptyEntries);
                data = new double[rows.Length,2];
                for (int i = 0; i < rows.Length; i++)
                {
                    var row = rows[i];
                    var splittedRow = row.SplitByWhitespaces();
                    for (int j = 0; j < splittedRow.Length; j++)
                        data[i, j] = double.Parse(splittedRow[j], CultureInfo.InvariantCulture);
                }
            }
            return data;
        }

        protected override SVMProblem CreateTrainProblem(List<KeyValuePair<string, double>> trainMessages, bool loadWords, ITextAnalizerDataProvider databaseProvider)
        {
            var wordsMartrix = trainMessages.Select(x => x.Key)
                    .ToArray();
            var transformed = Transform(wordsMartrix);
            return SvmProblem(transformed);
        }

        protected override void SerializeHelper()
        {
            XmlSerializer serializer = new XmlSerializer(typeof (UsedStatDictItem[]));
            serializer.Serialize(new FileStream(usedStatsPath, FileMode.OpenOrCreate),
                usedStats.Select(x => new UsedStatDictItem {Id = x.Key, MinMax = x.Value}).ToArray());

        }

        /// <summary>
        /// Dokonuje transformacji dokumentów na macierz zawierającą statystki występowania poszczególnych słów
        /// </summary> <param name="wordsMartrix"></param>
        /// <returns></returns>
        protected override List<List<double>> Transform(string[] wordsMartrix)
        {
            var problem = new List<List<double>>(wordsMartrix.Length);
            foreach (var message in wordsMartrix)
            {
                var dbNotes = noteProvider.PrepereNotesInMemory(message, lang, tradeId); //dbProvider.ExecuteFNote(message, lang); //
                var emtNotes = noteProvider.RateEmoticons(message);
                var exclamitonsCount = message.Count(x => x == '!');
                var questionsCount = message.Count(x => x == '?');

                var statistics = CreateStatistics(dbNotes.Select(x => x.Item2).ToArray(), emtNotes.Select(x => x.Item2).ToArray(),exclamitonsCount,questionsCount);

                var stats = new List<double>();
                for (int i = 0; i < statistics.Count; i++)
                {
                    int statIndex = i + 2;
                    if (usedStats.ContainsKey(statIndex))
                    {
                        if (tuneMinMax)
                        {
                            usedStats[statIndex].StatMax = usedStats[statIndex].StatMax > statistics[i]
                                ? usedStats[statIndex].StatMax
                                : statistics[i];
                            usedStats[statIndex].StatMin = usedStats[statIndex].StatMin < statistics[i]
                                ? usedStats[statIndex].StatMin
                                : statistics[i];
                        }
                        stats.Add(statistics[i]);
                    }
                }
                problem.Add(stats);
            }
            return Normalize(problem);
        }

        private static List<double> CreateStatistics(double[] data, double[] emotesNotes, int exclamitonsCount, int questionsCount)
        {
            Descriptive desp = new Descriptive(data);
            desp.Analyze();
            var statistics = new[]
            {
                desp.Result.Count,
                desp.Result.Mean,
                desp.Result.StdDev,
                desp.Result.Median,
                desp.Result.FirstQuartile,
                desp.Result.ThirdQuartile,
                desp.Result.IQR,
                desp.Result.Kurtosis,
                desp.Result.Skewness,
                desp.Result.Max,
                desp.Result.Min,
                desp.Result.Range,
                desp.Result.Sum,
                desp.Result.Variance,
                emotesNotes.Sum(),
                questionsCount,
                exclamitonsCount,
            }.Select(x => double.IsNaN(x) || double.IsInfinity(x) ? 0 : x).ToList();
            return statistics;
        }

        private List<List<double>> Normalize(List<List<double>> problem)
        {
            var maxMins = usedStats.Values.ToList();
            foreach (var element in problem)
            {
                for (int i = 0; i < element.Count; i++)
                {
                    if (Math.Abs(maxMins[i].StatMin - maxMins[i].StatMax) > Eps)
                    {
                        element[i] = (element[i] - maxMins[i].StatMin) / (maxMins[i].StatMax - maxMins[i].StatMin);
                        if (double.IsInfinity(element[i]) || double.IsNaN(element[i]))
                        {
                            element[i] = 0;
                        }
                    }
                }
            }
            return problem;
        }
    }
}