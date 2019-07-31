using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgriWebSite_v2.Data
{
    public class RulesForRelay
    {
        public int Id { get; set; }

        public Relay Relay { get; set; }

        public Measurements Measurement { get; set; }

    }
}
