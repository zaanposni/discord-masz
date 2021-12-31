namespace MASZ.Extensions
{
    public static class StringTruncate
    {
        public static string Truncate(this string currentString, int max = 2000)
        {
            return currentString.Length > max - 6 ? currentString[..(max - 6)] + " [...]" : currentString;
        }
    }
}