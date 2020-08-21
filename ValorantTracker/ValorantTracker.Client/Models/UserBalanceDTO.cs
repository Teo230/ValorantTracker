using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValorantTracker.Client.Models
{
    public class UserBalanceDTO
    {
        public Balances balances { get; set; }
        public class Balances
        {
            [JsonProperty("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")]
            public int ValorantPoints { get; set; }
            [JsonProperty("e59aa87c-4cbf-517a-5983-6e81511be9b7")]
            public int RadianitePoints { get; set; }
            [JsonProperty("f08d4ae3-939c-4576-ab26-09ce1f23bb37")]
            public int Unknown { get; set; }
        }
    }
}
