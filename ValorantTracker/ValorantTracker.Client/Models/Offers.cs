using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValorantTracker.Client.Models
{
    public class Offers
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public TimeSpan RemainSeconds { get; set; }
        public int ValorantPrice { get; set; }
    }
}
