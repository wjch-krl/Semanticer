using Semanticer.Common.Enums;

namespace Semanticer.Common.DataModel
{
    public class WebsiteFragmentDescriptor
    {
        /// <summary>
        /// String identyfikuj¹cy poszczególny element
        /// </summary>
        public string Descriptor { get; set; }

        /// <summary>
        /// Typ identyfikatora np. nazwa klasy, œcie¿ka xPath 
        /// </summary>
        public HtmlElementIdentifier DescriptorType { get; set; }

        /// <summary>
        /// Jeœli true wyszukiwane po¿¹danych treœci odbywa siê na ró¿nych poziomach zagnie¿d¿enia
        /// </summary>
        public bool AgileLookup { get; set; }

        /// <summary>
        /// Poziom na którym znajduj¹ siê w³aœciwe treœci
        /// </summary>
        public int TargetNodeLevel { get; set; }

        /// <summary>
        /// Czy ³¹czyæ z uprzednio pobranymi elementami
        /// </summary>
        public bool Concat { get; set; }

        public WebsiteFragmentDescriptor()
        {
        }

        public WebsiteFragmentDescriptor(string desriptor)
        {
            Descriptor = desriptor;
            DescriptorType = HtmlElementIdentifier.ClassName;
            AgileLookup = true;
        }

        public override bool Equals(object obj)
        {
            WebsiteFragmentDescriptor that = obj as WebsiteFragmentDescriptor;
            if (that == null)
            {
                return false;
            }
            return Descriptor == that.Descriptor && DescriptorType == that.DescriptorType &&
                   AgileLookup == that.AgileLookup && TargetNodeLevel == that.TargetNodeLevel && Concat == that.Concat;
        }

        public override int GetHashCode()
        {
            return string.Format("{0}{1}{2}{3}", Descriptor, DescriptorType, TargetNodeLevel, AgileLookup).GetHashCode();
        }
    }
}