using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValorantTracker.Client.Models
{
    public class Match
    {
        public List<HistoryDTO> history { get; set; }

        public class HistoryDTO
        {
            public string MatchID { get; set; }

            public DateTime GameStartTime { get; set; }

            public string TeamID { get; set; }

            public int TierUpdate { get; set; }

            public string Movement { get; set; }
        }  
    }
}
