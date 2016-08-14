//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Xml.Serialization;
//using AForge.Neuro;
//using AForge.Neuro.Learning;
//using ComarchBI.SocialIntelligence.Classifier.Common;
//using ComarchBI.SocialIntelligence.Common;
//using ComarchBI.SocialIntelligence.Common.Enums;
//using ComarchBI.SocialIntelligence.DatabaseProvider;
//using LibSVMsharp;
//using LibSVMsharp.Helpers;
//using SharpEntropy;
//using StatDescriptive;
//
//namespace ComarchBI.SocialIntelligence.Classifier.Svm
//{
//    /// <summary>
//    /// Klasyfikator wykorzystujący algorytm Maszyny wektorów nośnych
//    /// </summary>
//    public class NeuralStatisticalClassifier : SvmClassifierBase
//    {
//        private readonly string lang;
//        private readonly int tradeId;
//        private readonly IDatabaseProvider dbProvider;
//        private IDictionary<int, UsedStats> usedStats;
//        private string usedStatsPath;
//        private bool tuneMinMax;
//        private const double Eps = 0.00000001;
//
//        public NeuralStatisticalClassifier(ITokenizer tokenizer, string lang, int tradeId, IDatabaseProvider dbProvider,
//            bool useRegresion = false)
//            : base(tokenizer, string.Format("{0}_{1}", lang, tradeId), useRegresion)
//        {
//            usedStatsPath = String.Format("UsedStats_{0}.xml", base.TradeLang);
//            this.lang = lang;
//            this.tradeId = tradeId;
//            this.dbProvider = dbProvider;
//            DeserializeHelperData();
//            MatchCutoff = 0.6;
//        }
//
//        protected void DeserializeHelperData()
//        {
//            if (File.Exists(usedStatsPath))
//            {
//                XmlSerializer serializer = new XmlSerializer(typeof (UsedStatDictItem[]));
//                using (var stream = new FileStream(usedStatsPath, FileMode.Open))
//                {
//                    var stats = (UsedStatDictItem[]) serializer.Deserialize(stream);
//                    usedStats = stats.ToDictionary(key => key.Id, value => value.MinMax);
//                    tuneMinMax = false;
//                }
//            }
//            else
//            {
//                usedStats = CreateUsedStats();
//            }
//           // if (File.Exists(ModelPath))
//            //    Classifier = UseRegresion ? (SVM) new Epsilon_SVR(ModelPath) : new C_SVC(ModelPath);
//        }
//
//        private static Dictionary<int, UsedStats> CreateUsedStats()
//        {
//            return new Dictionary<int, UsedStats>()
//            {
//                {1, new UsedStats()},
//                {3, new UsedStats()},
//                {12, new UsedStats()},
//                {13, new UsedStats()}
//            };
//        }
//
//        /// <summary>
//        /// Tworzy plik reprezentujący dane treningowe
//        /// </summary>
//        /// <param name="trainMessages"></param>
//        /// <param name="path"></param>
//        /// <param name="loadWords"></param>
//        /// <param name="databaseProvider"></param>
//        protected void CreateTrainFile(List<KeyValuePair<string, double>> trainMessages, string path,
//            bool loadWords, IDatabaseProvider databaseProvider)
//        {
//            using (var file = new StreamWriter(path, false))
//            {
//                var wordsMartrix = trainMessages.Select(x => x.Key)
//                    .ToArray();
//                var transformed = Transform(wordsMartrix);
//                int i = 0;
//                foreach (var element in trainMessages)
//                {
//                    file.Write(element.Value);
//                    for (int j = 0; j < transformed[i].Count; j++)
//                    {
//                        if (transformed[i][j] != 0.0)
//                            file.Write(string.Format(CultureInfo.InvariantCulture, " {0}:{1}", j + 1, transformed[i][j]));
//                    }
//                    file.WriteLine();
//                    i++;
//                }}
//        }
//
//        public DiagnosticLogElement Train(IDictionary<int, UsedStats> stats, double c = 1000, double eps = 0.02,
//            double rbfParam = 0.01)
//        {
//            DateTime starTime = DateTime.Now;
//            var problem = SVMProblemHelper.Load(trainFilePath);
//
//            var network = new ActivationNetwork(new SigmoidFunction(), 5, 4, 4, 3);PerceptronLearning learning = new PerceptronLearning(network);
//            double[][] input = { new[] { 0.0 } };
//            double[][] output = { new[] { 0.0 } };
//
//            int iteration = 0;
//            double error;
//            do
//            {
//                error = learning.RunEpoch(input,output);
//                if (error < eps)
//                    break;
//                System.Diagnostics.Debug.WriteLine("{0} {1}", error, iteration);
//            } while (error < eps && iteration++ < 1000);
//
//            XmlSerializer serializer = new XmlSerializer(typeof (UsedStatDictItem[]));
//            var statArray = stats.Select(x => new UsedStatDictItem {Id = x.Key, MinMax = x.Value}).ToArray();
//            using (var stream = new FileStream(usedStatsPath, FileMode.OpenOrCreate))
//            {
//                serializer.Serialize(stream, statArray);
//            }
//            tuneMinMax = false;
//            var trainTime = DateTime.Now - starTime;
//            return new DiagnosticLogElement
//            {
//                Processed = problem.Length,
//                CompletitionTime = trainTime,
//                JobType = LoggerEventType.ClassifierTrainingStop,
//                Date = DateTime.UtcNow,
//            };
//        }
//
//        protected override SVMProblem CreateTrainProblem(List<KeyValuePair<string, double>> trainMessages, bool loadWords, IDatabaseProvider databaseProvider)
//        {
//            throw new NotImplementedException();
//        }
//
//        protected override void SerializeHelper()
//        {
//            XmlSerializer serializer = new XmlSerializer(typeof (UsedStatDictItem[]));
//            serializer.Serialize(new FileStream(usedStatsPath, FileMode.OpenOrCreate),
//                usedStats.Select(x => new UsedStatDictItem {Id = x.Key, MinMax = x.Value}).ToArray());
//        }
//
//        /// <summary>
//        /// Dokonuje transformacji dokumentów na macierz zawierającą statystki występowania poszczególnych słów
//        /// </summary>
//        /// <param name="wordsMartrix"></param>
//        /// <returns></returns>
//        protected override List<List<double>> Transform(string[] wordsMartrix)
//        {
//            var problem = new List<List<double>>(wordsMartrix.Length);
//            foreach (var word in wordsMartrix)
//            {
//                var dbNotes = dbProvider.ExecuteFNote(word, lang); //TODO
//
//                var statistics = CreateStatistics(dbNotes.Select(x => (double) x.Item2).ToArray());
//
//                var stats = new List<double>();
//                for (int i = 0; i < statistics.Count; i++)
//                {
//                    if (usedStats.ContainsKey(i + 2))
//                    {
//                        if (tuneMinMax)
//                        {
//                            usedStats[i].StatMax = usedStats[i].StatMax > statistics[i]
//                                ? usedStats[i].StatMax
//                                : statistics[i];
//                            usedStats[i].StatMin = usedStats[i].StatMin < statistics[i]
//                                ? usedStats[i].StatMin
//                                : statistics[i];
//                        }
//                        stats.Add(statistics[i]);
//                    }
//                }
//                problem.Add(stats);
//            }
//            return Normalize(problem);
//        }
//
//        private static List<double> CreateStatistics(double[] data)
//        {
//            Descriptive desp = new Descriptive(data);
//            desp.Analyze();
//            var statistics = new[]
//            {
//                desp.Result.Count,
//                desp.Result.Mean,
//                desp.Result.StdDev,
//                desp.Result.Median,
//                desp.Result.GeometricMean,
//                desp.Result.HarmonicMean,
//                desp.Result.FirstQuartile,
//                desp.Result.ThirdQuartile,
//                desp.Result.IQR,
//                desp.Result.Kurtosis,
//                desp.Result.Skewness,
//                desp.Result.Max,
//                desp.Result.Min,
//                desp.Result.Range,
//                desp.Result.Sum,
//                desp.Result.Variance,
//            }.Select(x => double.IsNaN(x) || double.IsInfinity(x) ? 0 : x).ToList();
//            return statistics;
//        }
//
//        private List<List<double>> Normalize(List<List<double>> problem)
//        {
//            var maxMins = usedStats.Values.ToList();
//            foreach (var element in problem)
//            {
//                for (int i = 0; i < element.Count; i++)
//                {
//                    if (Math.Abs(maxMins[i].StatMin - maxMins[i].StatMax) > Eps)
//                    {
//                        element[i] = (element[i] - maxMins[i].StatMin)/(maxMins[i].StatMax - maxMins[i].StatMin);
//                        if (double.IsInfinity(element[i]) || double.IsNaN(element[i]))
//                        {
//                            element[i] = 0;
//                        }
//                    }
//                }
//            }
//            return problem;
//        }
//    }
//}