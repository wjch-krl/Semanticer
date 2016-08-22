namespace Semanticer.Common
{
    public static class Patterns
    {
        public static string Sentence = @"(\S.+?[.!?])(?=\s+|$)";
        public static string Link =
            @"((([A-Za-z]{3,9}:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?(?:[\w]*))?)";
        public const string NormalizedSentenceDeparse = "\\|{2}(.*?)\\|{2}#{2}(.*?)#{2}&{2}(.*?)&{2}";
        public const string WordSeparators = @"[^\w|ź|Ż|ż|Ż|ć|Ć|ł|Ł|ś|Ś|ą|Ą|ę|Ę|ó|Ó|ń|Ń]|[0-9]";
        //public static string SpojnikPL = @"a|i|oraz|tudzież|albo|bądź|czy|lub|ani|ni|aczkolwiek|ale|jednak|lecz|natomiast|zaś|czyli|mianowicie|dlatego|przeto|tedy|więc|zatem|toteż|aby|bo|choć|jeżeli|ponieważ|że|ale|bowiem|chociaż|gdyż|jeśli|żeby"; // i jeszcze więcej
        //public static string ZaimekPL = @"ja|ty|on|ona|ono|my|wy|oni|one|mnie|ciebie|cię|jego|go|niego|jej|niej|nas|was|ich|nich|mi|mną|tobie|ci|tobą|jemu|mu|niemu|nim|ją|nią|je|nie|nam|nami|wam|wami|im|nim|nimi"; // i dużo więcej
        //public static string PrzyimekPL = @"(z|do|na|bez|za|pod|u|w|nad|o|od|po)+"; // przydało by się jeszcze więcej słów
        //public static Dictionary<string, string> IgnoreWords = new Dictionary<string, string>()
        //                    {
        //                        {"pl-PL",string.Format(@"^({0}|{1}|{2}|doctype|html)$", SpojnikPL, ZaimekPL, PrzyimekPL)}
        //                    };
        public const string ReplicatedChars = "(\\w)\\1+";
    }
}
