namespace MASZ.Extensions
{
    public static class StringExtension
    {
        public static List<string> GetAllPossiblePartsFromUrl(this string url)
        {
            List<string> allPossibleViolations = new List<string>();

            string[] domainParts = url.Split(".");
            for (int i = 0; i < domainParts.Length-1; i++)
            {
                string subDomain = string.Join(".", domainParts.Skip(i).ToArray());
                allPossibleViolations.Add(subDomain);
            }

            if (url.Contains("/"))
            {
                allPossibleViolations.AddRange(GetAllPossiblePartsFromUrl(url.Split("/")[0]));
            }

            return allPossibleViolations;
        }
    }
}