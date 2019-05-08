using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
                if (jsonResponse != null || jsonResponse.Answers != null || !jsonResponse.Answers.Length.Equals(0) ||
                    jsonResponse.Answers[0].Score > 49)
                {
                    if (jsonResponse.Answers[0].answer == "No good match found in KB.")
                    {
                        return Messages.NotUnderstand;
                    }

                    return jsonResponse.Answers[0].answer;
                }

                return Messages.NotUnderstand;
            }
        }

        public class Rootobject
        {
            public Answer[] Answers { get; set; }
        }

        public class Answer
        {
            public string[] Questions { get; set; }

            public string answer { get; set; }

            public float Score { get; set; }

            public int Id { get; set; }

            public string Source { get; set; }

            public Metadata[] Metadata { get; set; }
        }

        public class Metadata
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }
    }
}