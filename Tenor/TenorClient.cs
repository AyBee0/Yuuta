using RestSharp;
using System;
using Types.Tenor;
using System.Linq;

namespace Tenor {

    public class TenorClient {

        private const string TenorApiKey = "I31L2AVXMVSV";
        private RestClient RestClient;
        private Random Random;

        public TenorClient() {
            RestClient = new RestClient("https://api.tenor.com/v1");
            Random = new Random();
        }

        public string[] SearchForGif(string keyword, int resultLimit = 8) {
            var req = new RestRequest($"search?q={keyword}&key={TenorApiKey}&limit={resultLimit}");
            var response = RestClient.Execute<TenorResult>(req);
            var tenorResult = response.Data.Results;
            return tenorResult.Select(x => x.Media[0].Tinygif.Url).ToArray();
        }

        public string GetRandomGif(string keyword, int resultLimit = 8) {
            var resultUrls = SearchForGif(keyword, resultLimit);
            return resultUrls == null || resultUrls.Length <= 0 ? null : resultUrls[Random.Next(0, resultUrls.Length)];
        }

    }

}
