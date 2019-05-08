using Newtonsoft.Json.Linq;

namespace PimBot.State
{
    public class OnTurnState
    {
        public OnTurnState()
        {
            Intent = null;
            Entities = new JObject();
        }

        public OnTurnState(string intent, JObject entities)
        {
            Intent = intent;
            Entities = entities;
        }

        public string Intent { get; set; }

        public JObject Entities { get; set; }
    }
}
