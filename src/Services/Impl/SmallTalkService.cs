using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PimBot.Service;

namespace PimBot.Services.Impl
{
    public class SmallTalkService : ISmallTalkService
    {
        public async Task<string> GetSmalltalkAnswer(string inputMessage)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // POST method
                request.Method = HttpMethod.Post;

                // Add Host + service to get full URI
                request.RequestUri = new Uri(Constants.Host + Constants.Route);

                // set question
                string question = @"{'question': '" + inputMessage + @"','top': 1}";

                request.Content = new StringContent(question, Encoding.UTF8, "application/json");

                // set authorization
                request.Headers.Add("Authorization", "EndpointKey " + Constants.EndpointKey);

                // Send request to Azure service, get response
                var response = client.SendAsync(request).Result;
                var jsonResponse = response.Content.ReadAsAsync<Rootobject>().Result;
                if (jsonResponse != null || jsonResponse.answers != null || !jsonResponse.answers.Length.Equals(0) ||
                    jsonResponse.answers[0].score > 49)
                {
                    if (jsonResponse.answers[0].answer == "No good match found in KB.")
                    {
                        return Messages.NotUnderstand;
                    }

                    return jsonResponse.answers[0].answer;
                }

                return Messages.NotUnderstand;
            }
        }

        public class Rootobject
        {
            public Answer[] answers { get; set; }
        }

        public class Answer
        {
            public string[] questions { get; set; }
            public string answer { get; set; }
            public float score { get; set; }
            public int id { get; set; }
            public string source { get; set; }
            public Metadata[] metadata { get; set; }
        }

        public class Metadata
        {
            public string name { get; set; }
            public string value { get; set; }
        }
    }

}

