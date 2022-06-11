using System.Linq;

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
    }
}