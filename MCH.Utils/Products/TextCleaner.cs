using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MCH.Utils.Products
{
    public static class  TextCleaner
    {
      
        public static string CleanString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            //while (text.Contains('<') && text.Contains('>'))
            //{
            //    var start_tag_index = text.IndexOf('<');
            //    var end_tag_index = text.IndexOf('>');
            //    if (end_tag_index > start_tag_index)
            //    {
            //        text = text.Substring(0, start_tag_index) + " " + text.Substring(end_tag_index + 1);
            //    }
            //}
            text = removeHtmlTags(text);
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
           
            return  new string(text.Where(c => char.IsLetter(c) || char.IsDigit(c) || char.IsSeparator(c)).ToArray());
        }

        public static string CleanNumber(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            return  new string(text.Where(c =>  char.IsDigit(c)).ToArray());
        }

        private static string removeHtmlTags(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            var result = Regex.Replace(text, "<.*?>", String.Empty);
            return result;
        }
    }
}