using System;

namespace AgriApi_v2.Drivers.BME280
{
    public class AgriData
    {
        public float Temperature { get; set; }

        public float Pressure { get; set; }

        public float Humidity { get; set; }

        public DateTime Date { get; set; }
        
        public long DateFormatted => (long)(Date - new DateTime(1970, 1, 1)).TotalMilliseconds;

        public double SoilMoisture { get; set; }

        public double Lum { get; set; }

        public bool IsActive { get; set; }

        public bool IsR1On { get; set; }

        public bool IsR2On { get; set; }

        public bool IsR3On { get; set; }

        public bool IsRelayOnNotification { get; set; }


        public override string ToString()
        {
            return $"Temperature: {Temperature}, Pressure: {Pressure}, Humidity: {Humidity}";
        }
    }
}