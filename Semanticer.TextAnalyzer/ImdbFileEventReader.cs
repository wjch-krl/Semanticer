using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Semanticer.Classifier;
using SharpEntropy;
using Semanticer.Common.Enums;

namespace Semanticer.TextAnalyzer
{
	class ImdbFileEventReader : ITrainingEventReader
	{
		string path;
		ITokenizer tokenizer;
		bool hasNext;
		string [] posFiles;
		string [] negFiles;
		string [] neutFiles;
		int idx;

		public ImdbFileEventReader (string path, ITokenizer tokenizer)
		{
			var trainPath = Path.Combine (path, "train");
			posFiles = Directory.GetFiles (Path.Combine (trainPath, "pos"), "*.txt", SearchOption.TopDirectoryOnly);
			negFiles = Directory.GetFiles (Path.Combine (trainPath, "neg"), "*.txt", SearchOption.TopDirectoryOnly);
			neutFiles = Directory.GetFiles (Path.Combine (trainPath, "unsup"), "*.txt", SearchOption.TopDirectoryOnly);
			hasNext = posFiles.Any () || negFiles.Any () || neutFiles.Any ();
			this.tokenizer = tokenizer;
		}

		public bool HasNext ()
		{
			return hasNext;
		}

		public TrainingEvent ReadNextEvent ()
		{
			this.idx++;
			hasNext = this.idx < posFiles.Length + negFiles.Length + neutFiles.Length - 1;
			var idx = this.idx;
			if (idx < posFiles.Length) 
			{
				return new TrainingEvent (PostMarkType.Positive.ToString (), tokenizer.Tokenize (ReadFile (posFiles [idx])));
			}
			var tmpIdx = idx - posFiles.Length;
			if (tmpIdx < negFiles.Length)
			{
				return new TrainingEvent (PostMarkType.Negative.ToString (), tokenizer.Tokenize (ReadFile (negFiles [tmpIdx])));
			}
			tmpIdx = tmpIdx - negFiles.Length;
			return new TrainingEvent (PostMarkType.Neutral.ToString (), tokenizer.Tokenize (ReadFile (neutFiles [tmpIdx])));
		}

		string ReadFile (string path)
		{
			return File.ReadAllText (path);
		}
	}
}