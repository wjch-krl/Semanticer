using System;
using System.Collections.Generic;
using Semanticer.Classifier.Numeric.kNN;
using Semanticer.Common.Enums;
using Semanticer.Classifier.Numeric.Svm;
using Semanticer.Classifier.Transformers;
using Semanticer.Classifier.Common;
using LibSVMsharp;
using LibSVMsharp.Extensions;
using SharpEntropy;
using System.Linq;
using Semanticer.Common.Utils;

namespace Semanticer.Classifier
{
	public class Svm : NumericClassiferBase, ITrainable
	{
		private SVMModel svMachine;
		private double [,] Scale;

		public Svm (ITextTransformer transformer) : base(transformer)
		{
		}

		public override IDictionary<PostMarkType, double> Classify (string input)
		{
			var porob = Transformer.Transform (input);
			var problem = SvmProblem (porob);
			ScaleProblem (problem);
			var x = problem.X [0];
			var prediction = svMachine.Predict (x);
			return new Dictionary<PostMarkType, double> { { ((PostMarkType)((int)prediction)), 1 } };
		}

		SVMProblem SvmProblem (IEnumerable<SparseNumericFeature> numericFeatures)
		{
			var problem = new SVMProblem ();
			var nodes = NodeFromSparseFeature (numericFeatures);
			problem.Add (nodes.ToArray (), 0);
			return problem;
		}

		public TimeSpan ReTrain (ITrainingData data)
		{
			SVMParameter parameter = new SVMParameter 
			{
				Kernel = SVMKernelType.RBF,
				C = 0.1767767,
				Gamma = 0.3535534,
			};
			DateTime starTime = DateTime.Now;
			var events = ExtranctEnumerableTrainEvents (data);
			var problem = CreateTrainProblem (events);
			ScaleNewProblem (problem);

			this.svMachine = problem.Train (parameter);
			return DateTime.Now - starTime;
		}

		private void ScaleNewProblem (SVMProblem problem)
		{
			Scale = new double [problem.X.SelectMany (x => x).Max (x => x.Index), 2];
			foreach (var featureGroups in problem.X.SelectMany (x => x).GroupBy (x => x.Index)) {
				var featureValue = featureGroups.Select (x => x.Value).ToArray ();
				Scale [featureGroups.Key - 1, 0] = featureValue.Average ();
				Scale [featureGroups.Key - 1, 1] = featureValue.CalculateStdDev ();
			}
			ScaleProblem (problem);
		}

		protected void ScaleProblem (SVMProblem problem)
		{
			for (int i = 0; i < problem.X.Count; i++) {
				problem.X [i] = problem.X [i].Select (x => {
					x.Value = (x.Value - Scale [x.Index - 1, 0]) / Scale [x.Index - 1, 1];
					return x;
				}).ToArray ();
			}
		}

		private SVMProblem CreateTrainProblem (IEnumerable<TrainingEvent> msgs)
		{
			var trainableSentences = ProccesTrainingData (msgs);
			SVMProblem problem = new SVMProblem ();
			foreach(var element in trainableSentences)
			{
				var nodes = NodeFromSparseFeature (element.Features);
				problem.Add(nodes, (int) element.Label);
			}
			return problem;
		}

		public SVMNode [] NodeFromSparseFeature (IEnumerable<SparseNumericFeature> features)
		{
			var nodes = features.Select (x => new SVMNode ((int)x.FeatureId, x.Value));
			return nodes.ToArray ();
		}
}
}

