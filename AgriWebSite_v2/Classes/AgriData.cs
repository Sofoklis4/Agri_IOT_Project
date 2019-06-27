using System;

namespace AgriWebSite_v2.Classes
{
    public class AgriData
    {
        public float Temperature { get; set; }

        public float Pressure { get; set; }

        public float Humidity { get; set; }

        public DateTime Date { get; set; }

        public float SoilMoisture { get; set; }

        public float Lum { get; set; }

        public bool IsWatered { get; set; }

        public bool IsRelayOnNotification { get; set; }

        public long DateFormatted => (long)(Date - new DateTime(1970, 1, 1)).TotalMilliseconds;


        //public override string ToString()
        //{
        //    return $"Temperature: {Temperature}, Pressure: {Pressure}, Humidity: {Humidity}";
        //}
    }
}