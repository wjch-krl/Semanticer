using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Semanticer.Common.Enums;
using Semanticer.Common.Utils;

namespace Semanticer.Common.DataModel
{
    [Serializable]
    public class HourStats
    {
        
        public SerializableDictionary<MarkType, long> StatsDictionary { get; set; }

        public HourStats()
        {
            var enumValues = (IEnumerable<MarkType>) Enum.GetValues(typeof(MarkType));
            this.StatsDictionary = enumValues.ToSerializableDictionary(x => x, x => 0L);
        }

        public void Add(SemanticResult semantics)
        {
            StatsDictionary[semantics.Result]++;
        }

        public long this[MarkType key] => StatsDictionary[key];
    }

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public DictionaryItem<TKey, TValue>[] Items
        {
            get { return innerDict.Select(x => new DictionaryItem<TKey, TValue>(x)).ToArray(); }
            set { innerDict = value.ToDictionary(x => x.Key, x => x.Value); }
        }

        private IDictionary<TKey, TValue> innerDict;

        public SerializableDictionary()
        {
            innerDict = new Dictionary<TKey, TValue>();
        }

        public SerializableDictionary(IDictionary<TKey, TValue> innerDict)
        {
            this.innerDict = innerDict;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return innerDict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerDict.GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            innerDict.Add(item);
        }

        public void Clear()
        {
            innerDict.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return innerDict.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            innerDict.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return innerDict.Remove(item);
        }

        [XmlIgnore]
        public int Count
        {
            get { return innerDict.Count; }
        }

        [XmlIgnore]
        public bool IsReadOnly
        {
            get { return innerDict.IsReadOnly; }
        }

        public bool ContainsKey(TKey key)
        {
            return innerDict.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            innerDict.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            return innerDict.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return innerDict.TryGetValue(key, out value);
        }

        [XmlIgnore]
        public TValue this[TKey key]
        {
            get { return innerDict[key]; }
            set { innerDict[key] = value; }
        }

        [XmlIgnore]
        public ICollection<TKey> Keys
        {
            get { return innerDict.Keys; }
        }

        [XmlIgnore]
        public ICollection<TValue> Values
        {
            get { return innerDict.Values; }
        }
    }

    public static class SerializableDictionaryExtensions
    {
        public static SerializableDictionary<TKey, TValue> ToSerializableDictionary<TKey, TValue, TSource>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            var dictionary = source.ToDictionary(keySelector, valueSelector);
            return new SerializableDictionary<TKey, TValue>(dictionary);
        }

        public static SerializableDictionary<TKey, TValue> ToSerializable<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary)
        {
            return new SerializableDictionary<TKey, TValue>(dictionary);
        }
    }
}