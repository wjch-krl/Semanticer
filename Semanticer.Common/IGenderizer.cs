using System;
using System.Collections.Generic;
using System.Linq;
using Semanticer.Common.DataModel;
using Semanticer.Common.Utils;

namespace Semanticer.Common
{
    public abstract class Genderizer
    {
        public abstract char AssignGender(string fullName, string lang);

        public Author AssignGender(Author author, string lang, bool force = false)
        {
            if (author.Sex == default (char) || force)
            {
                author.Sex = AssignGender(author.FullName, author.Locale ?? lang);
            }
            return author;
        }
    }

    public class SimpleGenderizer : Genderizer
    {
        public override char AssignGender(string fullName,string lang)
        {
            if (lang == "pl-PL")
            {
                var name = fullName.SplitByWhitespaces().FirstOrDefault();
                if (name != null)
                {
                    return name[name.Length - 1] == 'a' ? 'f' : 'm';
                }
            }
            return 'u';
        }
    }

    public class BayessianGenderizer : Genderizer
    {
        private readonly int nGramsize;
        private readonly bool useAllnameParts;

        public BayessianGenderizer(int nGramsize = 2, bool useAllnameParts = false)
        {
            this.nGramsize = nGramsize;
            this.useAllnameParts = useAllnameParts;
        }

        public void Train(IEnumerable<Tuple<char, string>> trainNames)
        {
            throw new NotImplementedException();
        }

        public override char AssignGender(string fullName, string lang)
        {
            throw new NotImplementedException();
        }
    }
}
