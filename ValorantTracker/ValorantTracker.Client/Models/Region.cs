using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValorantTracker.Client.Utilities;
using static ValorantTracker.Client.Utilities.GlobalEnum;

namespace ValorantTracker.Client.Models
{
    public class Region
    {
        public string RegionName { get; set; }

        public string RegionTag { get; set; }

        public EndpointsEnum RegionEnum { get; set; }
    }
}
