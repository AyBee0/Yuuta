using RestSharp;
using System;

namespace RestSharpStreamTest {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello World!");
            var restClient = new RestClient("https://the-beacon-team-battles.firebaseio.com/info/310279910264406017/.json");
            var req = new RestRequest(Method.GET);
            req.AddHeader("Accept", "text/event-stream");
            var response = restClient.Execute(req);
            Console.ReadKey();
        }
    }
}
