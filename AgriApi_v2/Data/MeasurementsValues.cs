using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace AgriApi_v2.Data
{
    public class MeasurementsValues
    {
        public int Id { get; set; }

        public Measurements Measurement { get; set; }

        public float Value { get; set; }

        public  DateTime DateTime { get; set; }
    }
}
