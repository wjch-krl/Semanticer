using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Semanticer.Common.Utils
{
    public static class DictionarySerializer
    {
        public static void Serialize<TKey, TValue>(IDictionary<TKey, TValue> values, string path)
        {
            var toSerialize = values.Select(x => new DictionaryItem<TKey, TValue>(x)).ToArray();
            var serializer = new XmlSerializer(typeof (DictionaryItem<TKey, TValue>[]));
            using (var writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, toSerialize);
            }
        }

        public static IDictionary<TKey, TValue> Deserialize<TKey, TValue>(string path)
        {
            var serializer = new XmlSerializer(typeof (DictionaryItem<TKey, TValue>[]));
            DictionaryItem<TKey, TValue>[] deserialized;
            using (var reader = new StreamReader(path))
            {
                deserialized = (DictionaryItem<TKey, TValue>[]) serializer.Deserialize(reader);
            }
            return deserialized.ToDictionary(x => x.Key, y => y.Value);
        }
    }
}