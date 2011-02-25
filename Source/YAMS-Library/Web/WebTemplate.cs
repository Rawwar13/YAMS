using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace YAMS
{
    public class WebTemplate
    {
        private static string OpenTag = @"\{\?Y\:";
        private static string CloseTag = @"\?\}";
        private static Regex TagFinder;

        public static string ReplaceTags(string strInput, Dictionary<string, string> dicInputTags)
        {
            TagFinder = new Regex(@"(" + OpenTag + @")([^}]+)(" + CloseTag + @")");
            string strOutput = strInput;

            MatchCollection results = TagFinder.Matches(strInput);
            foreach (Match match in results)
            {
                Regex replacer = new Regex(OpenTag + match.Groups[2].Value + CloseTag);
                if (dicInputTags.ContainsKey(match.Groups[2].Value)) strOutput = replacer.Replace(strOutput, dicInputTags[match.Groups[2].Value]);
            }

            return strOutput;
        }

    }
}
