using System.Collections.Generic;
using System.Linq;
using Semanticer.Common.DataModel;
using SharpEntropy;

namespace Semanticer.Classifier.Common
{
    class DatabaseTrainReader : ITrainingEventReader
    {
        private readonly ITokenizer tokenizer;
        // private readonly IDatabaseProvider provider;
        private IEnumerator<Post> posts;
        private Post currentPost;

        public DatabaseTrainReader(ITextAnalizerDataProvider provider, ITokenizer tokenizer, int langId, int tradeId, double trainProportion)
        {
            this.tokenizer = tokenizer;
            //this.provider = provider;
            var postsList = provider.TrainMessages(langId, tradeId).ToList();
            postsList = postsList.Take((int) (postsList.Count*trainProportion)).ToList();
            posts = postsList.GetEnumerator();
            posts.MoveNext();
            currentPost = posts.Current;
        }

        public TrainingEvent ReadNextEvent()
        {
            var result = new TrainingEvent(currentPost.MarkType.ToString(),
                tokenizer.Tokenize(currentPost.NormalizeMessage.ToString()));
            posts.MoveNext();
            currentPost = posts.Current;
            return result;
        }

        public bool HasNext()
        {
            return currentPost != null;
        }
    }
}
