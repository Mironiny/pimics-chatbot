// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date:
// ===

using Newtonsoft.Json.Linq;

namespace PimBot.State
{
    /// <summary>
    /// OnTurnState state object.
    /// </summary>
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
