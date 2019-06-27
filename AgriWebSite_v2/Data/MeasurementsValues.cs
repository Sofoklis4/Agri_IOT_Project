using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace AgriWebSite_v2.Data
{
    public class MeasurementsValues
    {
        public int Id { get; set; }

        public Measurements Measurement { get; set; }

        public float Value { get; set; }

        [JsonConverter(typeof(SDateFormatConverter), "dd MMM yyyy HH:mm")]
        public  DateTime DateTime { get; set; }
    }

    public class SDateFormatConverter : IsoDateTimeConverter
    {
        public SDateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
