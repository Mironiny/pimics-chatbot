using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PimBotDp.State
{
    public class Keyword
    {
        public string Keyword_ID { get; set; }
        public string Description { get; set; }
        public string Beschreibung { get; set; }
        public bool Usage_Table_of_Contents { get; set; }
        public string Group_System_Number { get; set; }
        public DateTime Created_On { get; set; }
        public string Updated_By { get; set; }
        public string ETag { get; set; }
    }
}
