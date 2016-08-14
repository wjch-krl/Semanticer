using System.Collections.Generic;
using Semanticer.Common.Enums;
using Semanticer.Common.Utils;

namespace Semanticer.Common.DataModel
{
    public class SetializableTagDictionary
    {
        public DictionaryItem<ForumStuctureElement, List<WebsiteFragmentDescriptor>>[] Items { get; set; } 
        public TagDicionaryProperties Properties { get; set; }
    }
}