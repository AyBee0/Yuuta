using Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.IO;

namespace RestSharpStreamTest {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello World!");
            var baseUrl = "https://the-beacon-team-battles.firebaseio.com/Root/.json";
            string tempFile = Path.GetTempFileName();
            using (var writer = File.OpenWrite(tempFile)) {
                var client = new RestClient(baseUrl);
                var request = new RestRequest {
                    ResponseWriter = (responseStream) => responseStream.CopyTo(writer)
                };
                var response = client.Execute(request);
                
            }
            Console.ReadKey();
        }
    }
}
