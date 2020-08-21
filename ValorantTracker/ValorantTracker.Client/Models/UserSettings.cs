using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ValorantTracker.Client.Utilities.GlobalEnum;

namespace ValorantTracker.Client.Models
{
    public class UserSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public EndpointsEnum? Region { get; set; }
    }
}
