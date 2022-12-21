using System.Text.RegularExpressions;
using MASZ.Models;
using RestSharp;

namespace MASZ.Services
{
    class PiHoleList : IDomainList
    {
        private readonly string url;

        public PiHoleList(string url)
        {
            this.url = url;
        }

        public string[] ReloadDomainList()
        {
            Regex domainEntryRegex = new Regex(@"^0\.0\.0\.0 ([a-z0-9\.-]+)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest();
            RestResponse response = client.Execute(request);

            if (response?.Content == null)
            {
                return new string[0];
            }

            if (!response.IsSuccessful)
            {
                return new string[0];
            }

            List<string> localList = new List<string>();

            foreach (string line in response.Content.Split("\n"))
            {
                Match match = domainEntryRegex.Match(line);
                if (match.Success)
                {
                    localList.Add(match.Groups[1].Value);
                }
            }

            return localList.ToArray();
        }
    }
}