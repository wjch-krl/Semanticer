using System.Collections.Generic;
using Semanticer.Common.DataModel;

namespace Semanticer.Classifier.Doc2Vec
{
    public interface IPostClassyfier
    {
        IEnumerable<Post> ClassyfyPost(ICollection<Post> posts, ICollection<Post> trainData);
    }
}