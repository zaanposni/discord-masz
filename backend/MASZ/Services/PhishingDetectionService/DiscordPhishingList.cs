using MASZ.Models;
using RestSharp;

namespace MASZ.Services
{
    class DiscordPhishingList : IDomainList
    {
        private readonly string url = "https://raw.githubusercontent.com/nikolaischunk/discord-phishing-links/main/txt/domain-list.txt";
        public string[] ReloadDomainList()
        {
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
                if (line.Length > 0)
                {
                    localList.Add(line);
                }
            }

            return localList.ToArray();
        }
    }
}