using System.Collections.Generic;

namespace Semanticer.Common.Utils
{
    public class DictionaryItem<TKey, TValue>
    {
        public DictionaryItem(KeyValuePair<TKey, TValue> element)
        {
            Key = element.Key;
            Value = element.Value;
        }

        public DictionaryItem()
        {
        }

        public TKey Key { get; set; }
        public TValue Value { get; set; }
    }
}