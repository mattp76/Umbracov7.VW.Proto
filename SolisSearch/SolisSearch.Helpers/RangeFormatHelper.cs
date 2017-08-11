using System;

namespace SolisSearch.Helpers
{
    public class RangeFormatHelper
    {
        public static string FormatRange(string rangeKey)
        {
            string range = rangeKey.Substring(rangeKey.IndexOf(":") + 1);
            if (!range.Contains(" TO "))
                return range;
            int num1 = rangeKey.IndexOf('[');
            if (num1 == -1)
                num1 = rangeKey.IndexOf('{');
            int num2 = rangeKey.LastIndexOf(']');
            if (num2 == -1)
                num2 = rangeKey.LastIndexOf('}');
            if (num1 < 0 || num2 < 0)
                return rangeKey;
            string[] strArray = rangeKey.Substring(num1 + 1, num2 - 1 - num1).Split(new string[1]
            {
        " "
            }, StringSplitOptions.RemoveEmptyEntries);
            string str1 = strArray[0];
            string str2 = strArray[2];
            if (RangeFormatHelper.IsDateRange(range))
                return string.Format("{0} - {1}", (object)DateTime.Parse(str1.Substring(0, 19).Replace("T", " ")).ToShortDateString(), (object)DateTime.Parse(str2.Substring(0, 19).Replace("T", " ")).ToShortDateString());
            return string.Format("{0} - {1}", (object)str1, (object)str2);
        }

        public static bool IsDateRange(string range)
        {
            if (range.Length == 46 && (int)range[11] == 84 && (int)range[20] == 90 || range.Length == 28 && (int)range[2] == 42 && ((int)range[17] == 84 && (int)range[26] == 90))
                return true;
            if (range.Length == 28 && (int)range[26] == 42 && (int)range[12] == 84)
                return (int)range[21] == 90;
            return false;
        }

        public static ParsedRange ParseRange(string range)
        {
            ParsedRange parsedRange = new ParsedRange();
            if (RangeFormatHelper.IsDateRange(range))
            {
                if (range.Length == 46)
                {
                    parsedRange.start = (object)range.Substring(1, 19).Replace("T", " ");
                    parsedRange.end = (object)range.Substring(25, 19).Replace("T", " ");
                }
                else if (range.Length == 28 && (int)range[2] == 42)
                {
                    parsedRange.start = (object)"*";
                    parsedRange.end = (object)range.Substring(7, 19).Replace("T", " ");
                }
                else if (range.Length == 28 && (int)range[26] == 42)
                {
                    parsedRange.start = (object)range.Substring(2, 19).Replace("T", " ");
                    parsedRange.end = (object)"*";
                }
                else
                {
                    parsedRange.start = (object)"*";
                    parsedRange.end = (object)"*";
                }
            }
            else
            {
                string[] strArray = range.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
                parsedRange.start = (object)strArray[0].Substring(1);
                parsedRange.end = (object)strArray[2].Substring(0, strArray[2].Length - 1);
            }
            parsedRange.startinclusive = range.StartsWith("[");
            parsedRange.endinclusive = range.EndsWith("]");
            return parsedRange;
        }
    }
}
