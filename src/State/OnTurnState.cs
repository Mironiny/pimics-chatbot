using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
