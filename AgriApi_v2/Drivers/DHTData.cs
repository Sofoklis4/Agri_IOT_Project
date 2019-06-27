namespace AgriApi_v2.Drivers
{
    public class DHTData
    {
        public float TempCelcius { get; set; }
        public float TempFahrenheit { get; set; }
        public float Humidity { get; set; }
        public double HeatIndex { get; set; }
        public int ReadReturn { get; set; }

    }
}