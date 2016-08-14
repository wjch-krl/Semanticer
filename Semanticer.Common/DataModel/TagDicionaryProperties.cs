using Semanticer.Common.Enums;

namespace Semanticer.Common.DataModel
{
    public class TagDicionaryProperties
    {
        public bool DumbLinkSearch { get; set; }
        public string PostUrlDecriptor { get; set; }
        public ForumScriptType ScriptType { get; set; }
        public WebsiteFragmentDescriptor ScriptTypeDescriptor { get; set; } 
    }
}