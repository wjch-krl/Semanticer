using LibSVMsharp;
using Semanticer.Classifier.Common;
using SharpEntropy;

namespace Semanticer.Classifier.Svm
{
    public class SvmTrainigData : ITrainingData
    {
        private readonly ITrainingData data;

        public SvmTrainigData(ITrainingData data, SVMParameter svmParameter)
        {
            this.data = data;
            SvmParameter = svmParameter;
        }

        public SVMParameter SvmParameter { get; private set; }

        public ITrainingEventReader Reader => data.Reader;

        public ITextAnalizerDataProvider DatabaseProvider => data.DatabaseProvider;

        public bool LoadWords => data.LoadWords;
    }
}