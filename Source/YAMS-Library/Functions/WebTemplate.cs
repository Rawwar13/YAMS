using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YAMS
{
    public class WebTemplate
    {
        private static string OpenTag = @"\{\?Y\:";
        private static string CloseTag = @"\?\}";
        private static Regex TagFinder = new Regex("(" + OpenTag + ")([^]+)(" + CloseTag + ")");

        public static string ReplaceTags(string strInput, Dictionary<string, string> dicTags)
        {
            string strOutput = strInput;

            MatchCollection results = TagFinder.Matches(strInput);
            foreach (Match match in results)
            {
                Regex replacer = new Regex(OpenTag + match.Value + CloseTag);
                if (dicTags.ContainsKey(match.Value)) strOutput = replacer.Replace(strOutput, dicTags[match.Value]);
            }

            return strOutput;
        }

    }
}
