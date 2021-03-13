using System;
using System.Globalization;
using System.Text;

namespace Functions
{
    static class Generic
    {
        public static void LogEvent(string LcTexto)
        {
            Console.WriteLine($"{System.DateTime.Now} - {LcTexto}");
        }
        public static void LogException(Exception e)
        {
            Console.WriteLine($"{e.GetType().Name}: {e.Message}");
        }

        public static string RemoveAccents(this string text)
        {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            return sbReturn.ToString();
        }
    }
}