using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Semanticer.Common.Enums;
using Semanticer.Common.Utils;

namespace Semanticer.Common.DataModel
{
    public class TagDictionary : Dictionary<ForumStuctureElement, List<WebsiteFragmentDescriptor>> //, IXmlSerializable
    {
        public TagDictionary()
        {       
        }

        public TagDictionary(IDictionary<ForumStuctureElement, List<WebsiteFragmentDescriptor>> dict,
            TagDicionaryProperties props) : base(dict)
        {
            this.Properties = props;
        }

        public void SerializeToFile(string path)
        {
            var tmp = this.Select(x => new DictionaryItem<ForumStuctureElement, List<WebsiteFragmentDescriptor>>(x)).ToArray();
            var toSerialize = new SetializableTagDictionary {Items = tmp, Properties = this.Properties};
            var serializer = new XmlSerializer(typeof(SetializableTagDictionary));
            using (var writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, toSerialize);
            }
        }

        public string Serialize()
        {
            var tmp = this.Select(x => new DictionaryItem<ForumStuctureElement, List<WebsiteFragmentDescriptor>>(x)).ToArray();
            var toSerialize = new SetializableTagDictionary { Items = tmp, Properties = this.Properties };
            var serializer = new XmlSerializer(typeof(SetializableTagDictionary));
            string result;
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, toSerialize);
                result = writer.ToString();
            }
            return result;
        }

        public static TagDictionary DeserializeFile(string path)
        {
            using (var reader = new StreamReader(path))
            {
                return DeserializeReader(reader);
            }
        }

        private static TagDictionary DeserializeReader(TextReader reader)
        {
            var serializer = new XmlSerializer(typeof(SetializableTagDictionary));
            var deserialized = (SetializableTagDictionary)serializer.Deserialize(reader);
            var tagDict = deserialized.Items.ToDictionary(x => x.Key, x => x.Value);
            return new TagDictionary(tagDict, deserialized.Properties);
        }

        public static TagDictionary DeserializeString(string xml)
        {
            using (var reader = new StringReader(xml))
            {
                return DeserializeReader(reader);
            }
        }

        public TagDicionaryProperties Properties { get; set; }
//
//        public XmlSchema GetSchema()
//        {
//            return null;
//        }
//
//        public void ReadXml(XmlReader reader)
//        {
//            var deserialized = DeserializeReader(new StringReader(reader.ReadElementContentAsString()));
//            
//        }
//
//        public void WriteXml(XmlWriter writer)
//        {
//            throw new System.NotImplementedException();
//        }
    }
}
