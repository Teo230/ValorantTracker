using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValorantTracker.Client.Utilities
{
    public class EventValArgs
    {
        public bool? PlayerIdReceived { get; set; } = null;
        public bool? PlayerReceived { get; set; } = null;
        public bool? MatchReceived { get; set; } = null;
        public bool? CompMatchReceived { get; set; } = null;
        public bool? StoreReceived { get; set; } = null;
        public bool? BalanceReceived { get; set; } = null;
        public bool? ContentReceived { get; set; } = null;
        public bool? TabItemReceived { get; set; } = null;
        public bool? PasswordChanged { get; set; } = null;
    }
}