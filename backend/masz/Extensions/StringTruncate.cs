namespace masz.Extensions
{
    public static class StringTruncate
    {
        public static string Truncate(this string currentString, int max=2000)
        {
            return currentString.Length > max - 6 ? currentString.Substring(0, max - 6) + " [...]" : currentString;
        }
    }
}