using System.Diagnostics;
using System.IO;
using Semanticer.Common.Enums;

namespace Semanticer.TextAnalyzer
{
    class ImdbFileInfo
    {
        public ImdbFileInfo(string fileName)
        {
            var plainName = Path.GetFileNameWithoutExtension(fileName);
            Debug.Assert(plainName != null, "plainName != null");
            var nameParts = plainName.Split('_');
            Id = int.Parse(nameParts[0]);
            Note = int.Parse(nameParts[1]);
        }

        public int Note { get; }

        public int Id { get; private set; }

        public MarkType ToMarkType()
        {
            if (Note > 6)
            {
                return MarkType.Positive;
            }
            if (Note < 4)
            {
                return MarkType.Negative;
            }
            return MarkType.Neutral;
        }
    }
}