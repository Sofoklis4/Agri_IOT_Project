using AgriApi_v2.Data;
using AgriApi_v2.Drivers;
using AgriApi_v2.Drivers.BME280;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace AgriApi_v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgriDiplomatikiController : ControllerBase
    {
        private bool _IsBME280On = true;
        private bool _IsLumOn = true;
        private bool _IsRelayOn = true;
        private bool _IsSoilMoistureOn = true;

        #region Context

        private readonly ApplicationDbContext _context;

        #endregion Context

        public AgriDiplomatikiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET api/values
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult<IEnumerable<string>>> GetAsync()
        {
            try
            {
                Pi.Init<BootstrapWiringPi>();

                var pinrelay = 26;
                var delay = 1000;

                if (_IsRelayOn)
                {
                    Pi.Gpio[pinrelay].PinMode = GpioPinDriveMode.Output;
                    System.Threading.Thread.Sleep(delay);
                }

                AgriData agriData = new AgriData();

                //bme280
                if (_IsBME280On)
                {
                    BME280 bme280 = new BME280();
                    Console.WriteLine("0aa");
                    await bme280.InitializeAsync();

                    agriData.Temperature = await bme280.ReadTemperatureAsync();
                    agriData.Humidity = await bme280.ReadHumidityAsync();
                    agriData.Pressure = await bme280.ReadPreasureAsync();
                    agriData.Date = DateTime.Now;
                    Console.WriteLine("0b");
                    var MeasurementTemperature = _context.Measurements
                        .SingleOrDefault(s => s.Name == "Temperature");
                    var MeasurementHumidity = _context.Measurements
                        .SingleOrDefault(s => s.Name == "Humidity");
                    var MeasurementPressure = _context.Measurements
                        .SingleOrDefault(s => s.Name == "Pressure");
                    Console.WriteLine("0c");
                    MeasurementsValues mvTemperature = new MeasurementsValues()
                    {
                        Measurement = MeasurementTemperature,
                        Value = agriData.Temperature,
                        DateTime = agriData.Date
                    };
                    MeasurementsValues mvHumidity = new MeasurementsValues()
                    {
                        Measurement = MeasurementHumidity,
                        Value = agriData.Humidity,
                        DateTime = agriData.Date
                    };
                    MeasurementsValues mvPressure = new MeasurementsValues()
                    {
                        Measurement = MeasurementPressure,
                        Value = agriData.Pressure,
                        DateTime = agriData.Date
                    };
                    Console.WriteLine("0a");
                    _context.Add(mvTemperature);
                    _context.Add(mvHumidity);
                    _context.Add(mvPressure);
                    _context.SaveChanges();
                }
                Console.WriteLine("1");
                //SoilMoisture
                if (_IsSoilMoistureOn)
                {
                    Pi.Spi.Channel0Frequency = SpiChannel.MinFrequency;

                    byte[] response = new byte[3];
                    byte[] request = new byte[3] { 0x01, 0x80, 0 };
                    response = Pi.Spi.Channel0.SendReceive(request);

                    int result = 0;
                    result = response[1] & 0x03;
                    result <<= 8;
                    result += response[2];

                    double moisture_percentage = (100 - ((result / 1023.00) * 100));
                    agriData.SoilMoisture = moisture_percentage;

                    var MeasurementSoilMoisture = _context.Measurements
                        .SingleOrDefault(s => s.Name == "SoilMoisture");

                    MeasurementsValues mvSoilMoisture = new MeasurementsValues()
                    {
                        Measurement = MeasurementSoilMoisture,
                        Value = Convert.ToSingle(agriData.SoilMoisture), //for float
                        DateTime = agriData.Date
                    };
                    Console.WriteLine("0an");
                    _context.Add(mvSoilMoisture);
                    _context.SaveChanges();
                }
                Console.WriteLine("2");
                //lum
                if (_IsLumOn)
                {
                    var readBuffer = new byte[2];

                    var lumDevice = Pi.I2C.AddDevice(0x23);

                    lumDevice.Write(0x23);

                    for (var i = 0; i < readBuffer.Length; i++)
                    {
                        readBuffer[i] = lumDevice.Read();
                    }

                    var lightLevel = -1.1;

                    lightLevel = readBuffer[0] << 8 | readBuffer[1];
                    Console.WriteLine("Lum ReadBuffer:" + readBuffer);
                    agriData.Lum = lightLevel / 1.2;

                    //Select From Database
                    var MeasurementLum = _context.Measurements
                        .SingleOrDefault(s => s.Name == "Lum");

                    MeasurementsValues mvLum = new MeasurementsValues()
                    {
                        Measurement = MeasurementLum,
                        Value = Convert.ToSingle(agriData.Lum), //for float
                        DateTime = agriData.Date
                    };
                    Console.WriteLine("Lum:"+ lightLevel);
                    _context.Add(mvLum);
                    _context.SaveChanges();
                }
                Console.WriteLine("3");
                //Pi.Gpio[pinrelay].PinMode = GpioPinDriveMode.Output;
                //System.Threading.Thread.Sleep(delay);
                if (_IsRelayOn)
                {

                   

                    var SoilMoistureDownLevel = _context.Measurements
                        .SingleOrDefault(s => s.Name == "SoilMoisture").DownLevel;

                    var SoilMoistureUpLevel = _context.Measurements
                        .SingleOrDefault(s => s.Name == "SoilMoisture").UpLevel;

                    var LumDownLevel = _context.Measurements
                    .SingleOrDefault(s => s.Name == "Lum").DownLevel;

                    if (agriData.SoilMoisture < SoilMoistureDownLevel && agriData.Lum < LumDownLevel && StaticFunc._firstTime==1)
                    {
                        StaticFunc._IsWatered = false;
                    }
                    if (agriData.SoilMoisture > SoilMoistureUpLevel)
                    {
                        StaticFunc._IsWatered = true;
                    }

                    if (StaticFunc._IsWatered == false)
                    {
                        Pi.Gpio[pinrelay].Write(false); // Relay On
                        System.Threading.Thread.Sleep(delay);
                       agriData.IsRelayOnNotification= StaticFunc._IsRelayNotificaton = true;
                    }
                    else
                    {
                        Pi.Gpio[pinrelay].Write(true); // Relay Off
                        System.Threading.Thread.Sleep(delay);
                        agriData.IsRelayOnNotification = StaticFunc._IsRelayNotificaton = false;
                    }

                    StaticFunc._firstTime = 1;
                }
                Console.WriteLine("4");

                agriData.IsActive = true;
                agriData.IsWatered = StaticFunc._IsWatered;

                return Ok(agriData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lathos:{ex.Message}");
                return NotFound($"Lathos:{ex.Message}");
            }
        }

        //[HttpGet, Route("soilmoisture")]
        //public ActionResult<IEnumerable<string>> Getsoilmoisture()
        //{
        //    try
        //    {
        //        Pi.Init<BootstrapWiringPi>();
        //        WeatherData weatherData = new WeatherData();
        //        Pi.Spi.Channel0Frequency = SpiChannel.MinFrequency;

        //        byte[] response = new byte[3];
        //        byte[] request = new byte[3] { 0x01, 0x80, 0 };
        //        response = Pi.Spi.Channel0.SendReceive(request);

        //        int result = 0;
        //        result = response[1] & 0x03;
        //        result <<= 8;
        //        result += response[2];

        //        double moisture_percentage = (100 - ((result / 1023.00) * 100));
        //        weatherData.SoilMoisture = moisture_percentage;

        //        return Ok(weatherData);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Lathos:{ex.Message}");
        //        return NotFound($"Lathos:{ex.Message}");
        //    }
        //}

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            //Sensor TH DHT11
            var dht = new DHT(22);

            var readFromDHT11 = dht.ReadData();
            var currentDateTime = DateTime.Now;

            return Ok($"Temp= {readFromDHT11.TempCelcius} and Hum= {readFromDHT11.Humidity} , DateTime= {currentDateTime}");
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}