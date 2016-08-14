using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;
using Semanticer.Common.Utils;

namespace Semanticer.Classifier.Doc2Vec
{
    public class Doc2VecPostTransformer : IPostTransformer
    {
        private readonly int numericalFeaturesCount;
        private readonly string trainDataFilePath;

        public Doc2VecPostTransformer(string trainDataFilePath, int numericalFeaturesCount = 10)
        {
            this.numericalFeaturesCount = numericalFeaturesCount;
            this.trainDataFilePath = trainDataFilePath;
        }

        public IEnumerable<ClassifiableSentence> Transform(IEnumerable<Post> posts)
        {
            var messagesFilePath = CreateMessagesFilePath(trainDataFilePath);
            lock (this)
            {
                int messagesCount = WriteMessagesToFile(posts, messagesFilePath);
                var pythonProcess = StartPythonDoc2VecScript(messagesFilePath);
                var transformed = ReadTransformedMessages(pythonProcess, messagesCount).ToArray();
                FinalizeProcess(pythonProcess);
                return transformed;
            }
        }

        public ClassificatorData Transform(ICollection<Post> evalPosts, ICollection<Post> trainPosts)
        {
            var messagesFilePath = CreateMessagesFilePath(trainDataFilePath);
            lock (this)
            {
                int messagesCount = WriteMessagesToFile(evalPosts, messagesFilePath);
                int trainMessagesCount = WriteMessagesToFile(trainPosts, messagesFilePath, true);
                var pythonProcess = StartPythonDoc2VecScript(messagesFilePath);
                var transformed = ReadTransformedMessages(pythonProcess, messagesCount + trainMessagesCount).ToArray();
                FinalizeProcess(pythonProcess);
                return CreateClassificatorData(transformed,trainPosts);
            }
        }

        private ClassificatorData CreateClassificatorData(ICollection<ClassifiableSentence> transformed, ICollection<Post> trainPosts)
        {
            var tmpDict = trainPosts.ToDictionary(x => x.FullId, x => x.MarkType);
            List<ClassifiableSentence> toEval = new List<ClassifiableSentence>();
            List<ClassifiedSentence> trainData = new List<ClassifiedSentence>();
            foreach (var sentence in transformed)
            {
                PostMarkType trainValue;
                if (tmpDict.TryGetValue(sentence.FullId,out trainValue))
                {
                    trainData.Add(new ClassifiedSentence(sentence, trainValue));
                }
                else
                {
                    toEval.Add(sentence);
                }
            }
            return new ClassificatorData(trainData,toEval);
        }

        private Process StartPythonDoc2VecScript(string messagesFilePath)
        {
            const string python = "python";
            const string pythonScriptPath = "doc2Vec.py";
            var myProcessStartInfo = new ProcessStartInfo(python)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = string.Format("{0} {1} {2} {3}", pythonScriptPath, trainDataFilePath, messagesFilePath, numericalFeaturesCount)
            };
            var myProcess = new Process {StartInfo = myProcessStartInfo};
            myProcess.Start();
            return myProcess;
        }

        private IEnumerable<ClassifiableSentence> ReadTransformedMessages(Process pythonProcess, int messagesCount)
        {
            using (var reader = pythonProcess.StandardOutput)
            {
                do
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(3));
                        continue;
                    }
                    string[] splitedLine = line.SplitByWhitespaces();
                    string orgId = splitedLine[0];
                    int srcId = int.Parse(splitedLine[1]);
                    int sentenceNumber = int.Parse(splitedLine[2]);
                    double[] features = splitedLine.Skip(3).Select(x =>double.Parse(x,CultureInfo.InvariantCulture)).ToArray();
                    yield return new ClassifiableSentence(features, sentenceNumber, orgId, srcId);
                } while (messagesCount-- > 0);
            }
        }

        private static int WriteMessagesToFile(IEnumerable<Post> posts, string path, bool append = false)
        {
            using (var fileWriter = new StreamWriter(path, append))
            {
                return WriteMessages(posts, fileWriter);
            }
        }

        private static string CreateMessagesFilePath(string trainDataFilePath)
        {
            var path = string.Format("{0}_custom.{1}",
                Path.GetFileNameWithoutExtension(trainDataFilePath),
                Path.GetExtension(trainDataFilePath));
            return path;
        }

        private static int WriteMessages(IEnumerable<Post> posts, TextWriter fileWriter)
        {
            int linesCount = 0;
            foreach (var post in posts)
            {
                for (int i = 0; i < post.NormalizeMessage.Sentences.Length; i++)
                {
                    fileWriter.WriteLine("{0} {1}\t{2}\t{3}", post.OrgId, post.SourceId, i, post.NormalizeMessage.Sentences[i]);
                    linesCount++;
                }
            }
            return linesCount;
        }

        private void FinalizeProcess(Process pythonProcess)
        {
            pythonProcess.WaitForExit();
            pythonProcess.Close();
            pythonProcess.Dispose();
        }
    }
}
