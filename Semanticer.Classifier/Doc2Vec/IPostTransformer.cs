using System.Collections.Generic;
using Semanticer.Common.DataModel;

namespace Semanticer.Classifier.Doc2Vec
{
    public interface IPostTransformer
    {
        ClassificatorData Transform(ICollection<Post> evalPosts, ICollection<Post> trainPosts);
    }
}