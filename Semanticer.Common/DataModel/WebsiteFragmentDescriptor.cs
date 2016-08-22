using Semanticer.Common.Enums;

namespace Semanticer.Common.DataModel
{
    public class WebsiteFragmentDescriptor
    {
        /// <summary>
        /// String identyfikuj�cy poszczeg�lny element
        /// </summary>
        public string Descriptor { get; set; }

        /// <summary>
        /// Typ identyfikatora np. nazwa klasy, �cie�ka xPath 
        /// </summary>
        public HtmlElementIdentifier DescriptorType { get; set; }

        /// <summary>
        /// Je�li true wyszukiwane po��danych tre�ci odbywa si� na r�nych poziomach zagnie�d�enia
        /// </summary>
        public bool AgileLookup { get; set; }

        /// <summary>
        /// Poziom na kt�rym znajduj� si� w�a�ciwe tre�ci
        /// </summary>
        public int TargetNodeLevel { get; set; }

        /// <summary>
        /// Czy ��czy� z uprzednio pobranymi elementami
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