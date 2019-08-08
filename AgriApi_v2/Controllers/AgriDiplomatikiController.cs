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

                var pinrelayR1 = 26;
                var pinrelayR2 = 20;
                var pinrelayR3 = 21;
                var delay = 500;

                if (_IsRelayOn)
                {
                    Pi.Gpio[pinrelayR1].PinMode = GpioPinDriveMode.Output;
                    System.Threading.Thread.Sleep(delay);
                    Pi.Gpio[pinrelayR2].PinMode = GpioPinDriveMode.Output;
                    System.Threading.Thread.Sleep(delay);
                    Pi.Gpio[pinrelayR3].PinMode = GpioPinDriveMode.Output;
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
                        .Where(s => s.Relay.RelayName == "Relay1")
                        .SingleOrDefault(s => s.Name == "Temperature");
                    var MeasurementHumidity = _context.Measurements
                          .Where(s => s.Relay.RelayName == "Relay1")
                        .SingleOrDefault(s => s.Name == "Humidity");
                    var MeasurementPressure = _context.Measurements
                          .Where(s => s.Relay.RelayName == "Relay1")
                        .SingleOrDefault(s => s.Name == "Pressure");
                    Console.WriteLine("0c");
                    MeasurementsValues mvTemperature = new MeasurementsValues()
                    {
                        Measurement = MeasurementTemperature,
                        Value = agriData.Temperature,
                        DateTime = agriData.Date,


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
                          .Where(s => s.Relay.RelayName == "Relay1")
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
                          .Where(s => s.Relay.RelayName == "Relay1")
                        .SingleOrDefault(s => s.Name == "Lum");

                    MeasurementsValues mvLum = new MeasurementsValues()
                    {
                        Measurement = MeasurementLum,
                        Value = Convert.ToSingle(agriData.Lum), //for float
                        DateTime = agriData.Date
                    };
                    Console.WriteLine("Lum:" + lightLevel);
                    _context.Add(mvLum);
                    _context.SaveChanges();
                }
                Console.WriteLine("3");
                //Pi.Gpio[pinrelay].PinMode = GpioPinDriveMode.Output;
                //System.Threading.Thread.Sleep(delay);
                if (_IsRelayOn)
                {
                    #region RELAY1

                    //ForRelay1

                    var entity1R1 = _context.RulesForRelays
                        .Where(s => s.Relay.RelayName == "Relay1")
                        .ToList();
                    var getRelayR1 = _context.Relays
                            .Where(s => s.RelayName == "Relay1").FirstOrDefault();

                    var getSoilMoistureR1 = _context.Measurements
                        .Where(s => s.Relay == getRelayR1)
                        .Where(s => s.Name == "SoilMoisture").FirstOrDefault();
                    var getLumR1 = _context.Measurements
                          .Where(s => s.Relay == getRelayR1)
                        .Where(s => s.Name == "Lum").FirstOrDefault();
                    var getTemperatureR1 = _context.Measurements
                          .Where(s => s.Relay == getRelayR1)
                        .Where(s => s.Name == "Temperature").FirstOrDefault();

                    var getPressureR1 = _context.Measurements
                 .Where(s => s.Relay == getRelayR1)
               .Where(s => s.Name == "Pressure").FirstOrDefault();

                    var getHumidityR1 = _context.Measurements
                 .Where(s => s.Relay == getRelayR1)
               .Where(s => s.Name == "Humidity").FirstOrDefault();

                    var findInentity1R1SoilMoisture = entity1R1.Any(s => s.Measurement == getSoilMoistureR1);
                    var findentity1R1Lum = entity1R1.Any(s => s.Measurement == getLumR1);
                    var findentity1R1Temperature = entity1R1.Any(s => s.Measurement == getTemperatureR1);
                    var findentity1R1Pressure = entity1R1.Any(s => s.Measurement == getPressureR1);
                    var findentity1R1Humidity = entity1R1.Any(s => s.Measurement == getHumidityR1);



                    if (findInentity1R1SoilMoisture)
                    {
                        var SoilMoistureDownLevelR1 = _context.Measurements
                              .Where(s => s.Relay == getRelayR1)
                      .SingleOrDefault(s => s.Name == "SoilMoisture").DownLevel;

                        var SoilMoistureUpLevelR1 = _context.Measurements
                              .Where(s => s.Relay == getRelayR1)
                            .SingleOrDefault(s => s.Name == "SoilMoisture").UpLevel;

                        if (agriData.SoilMoisture < SoilMoistureDownLevelR1)
                        {
                            StaticFunc._IsWateredSoilMoistureR1 = false;
                        }
                        if (agriData.SoilMoisture > SoilMoistureUpLevelR1)
                        {
                            StaticFunc._IsWateredSoilMoistureR1 = true;
                        }
                    }
                    else
                    {
                        StaticFunc._IsWateredSoilMoistureR1 = false;
                    }

                    if (findentity1R1Lum)
                    {
                        var LumDownLevelR1 = _context.Measurements
                              .Where(s => s.Relay == getRelayR1)
                      .SingleOrDefault(s => s.Name == "Lum").DownLevel;

                        var LumUpLevelR1 = _context.Measurements
                              .Where(s => s.Relay == getRelayR1)
                            .SingleOrDefault(s => s.Name == "Lum").UpLevel;

                        if (agriData.Lum < LumDownLevelR1)
                        {
                            StaticFunc._IsWateredLumR1 = false;
                        }
                        if (agriData.Lum > LumUpLevelR1)
                        {
                            StaticFunc._IsWateredLumR1 = true;
                        }
                    }
                    else
                    {
                        StaticFunc._IsWateredLumR1 = false;
                    }

                    if (findentity1R1Temperature)
                    {
                        var TemperatureDownLevelR1 = _context.Measurements
                              .Where(s => s.Relay == getRelayR1)
                      .SingleOrDefault(s => s.Name == "Temperature").DownLevel;

                        var TemperatureUpLevelR1 = _context.Measurements
                              .Where(s => s.Relay == getRelayR1)
                            .SingleOrDefault(s => s.Name == "Temperature").UpLevel;

                        if (agriData.Temperature < TemperatureDownLevelR1)
                        {
                            StaticFunc._IsWateredTemperatureR1 = false;
                        }
                        if (agriData.Temperature > TemperatureUpLevelR1)
                        {
                            StaticFunc._IsWateredTemperatureR1 = true;
                        }
                    }
                    else
                    {
                        StaticFunc._IsWateredTemperatureR1 = false;
                    }

                    if (findentity1R1Pressure)
                    {
                        var PressureDownLevelR1 = _context.Measurements
                              .Where(s => s.Relay == getRelayR1)
                      .SingleOrDefault(s => s.Name == "Pressure").DownLevel;

                        var PressureUpLevelR1 = _context.Measurements
                              .Where(s => s.Relay == getRelayR1)
                            .SingleOrDefault(s => s.Name == "Pressure").UpLevel;

                        if (agriData.Pressure < PressureDownLevelR1)
                        {
                            StaticFunc._IsWateredPressureR1 = false;
                        }
                        if (agriData.Pressure > PressureUpLevelR1)
                        {
                            StaticFunc._IsWateredPressureR1 = true;
                        }
                    }
                    else
                    {
                        StaticFunc._IsWateredPressureR1 = false;
                    }

                    if (findentity1R1Humidity)
                    {
                        var HumidityDownLevelR1 = _context.Measurements
                              .Where(s => s.Relay == getRelayR1)
                      .SingleOrDefault(s => s.Name == "Humidity").DownLevel;

                        var HumidityUpLevelR1 = _context.Measurements
                              .Where(s => s.Relay == getRelayR1)
                            .SingleOrDefault(s => s.Name == "Humidity").UpLevel;

                        if (agriData.Humidity < HumidityDownLevelR1)
                        {
                            StaticFunc._IsWateredHumidityR1 = false;
                        }
                        if (agriData.Humidity > HumidityUpLevelR1)
                        {
                            StaticFunc._IsWateredHumidityR1 = true;
                        }
                    }
                    else
                    {
                        StaticFunc._IsWateredHumidityR1 = false;
                    }

                    //if (agriData.SoilMoisture < SoilMoistureDownLevel && agriData.Lum < LumDownLevel && StaticFunc._firstTime==1)
                    //{
                    //    StaticFunc._IsWatered = false;
                    //}
                    //if (agriData.SoilMoisture > SoilMoistureUpLevel)
                    //{
                    //    StaticFunc._IsWatered = true;
                    //}

                    //if (StaticFunc._IsWatered == false)
                    //{
                    //    Pi.Gpio[pinrelay].Write(false); // Relay On
                    //    System.Threading.Thread.Sleep(delay);
                    //    agriData.IsRelayOnNotification= StaticFunc._IsRelayNotificaton = true;
                    //}
                    //else
                    //{
                    //    Pi.Gpio[pinrelay].Write(true); // Relay Off
                    //    System.Threading.Thread.Sleep(delay);
                    //    agriData.IsRelayOnNotification = StaticFunc._IsRelayNotificaton = false;
                    //}
                   

                    if (StaticFunc._IsWateredSoilMoistureR1 == false 
                        && StaticFunc._IsWateredLumR1 == false 
                        && StaticFunc._IsWateredTemperatureR1 == false
                        && StaticFunc._IsWateredPressureR1 == false
                       && StaticFunc._IsWateredHumidityR1 == false)
                    {
                        Pi.Gpio[pinrelayR1].Write(false); // Relay On
                        System.Threading.Thread.Sleep(delay);
                        agriData.IsRelayOnNotification = StaticFunc._IsRelayNotificaton = true;
                        agriData.IsR1On = true;
                    }
                    else
                    {
                        Pi.Gpio[pinrelayR1].Write(true); // Relay Off
                        System.Threading.Thread.Sleep(delay);
                        agriData.IsRelayOnNotification = StaticFunc._IsRelayNotificaton = false;
                        agriData.IsR1On = false;
                    }

                    StaticFunc._firstTime = 1;
                

                #endregion RELAY1

                #region RELAY2

                //ForRelay1

                var entity1R2 = _context.RulesForRelays
                    .Where(s => s.Relay.RelayName == "Relay2")
                    .ToList();

                var getRelayR2 = _context.Relays
                          .Where(s => s.RelayName == "Relay2").FirstOrDefault();

                var getSoilMoistureR2 = _context.Measurements
                     .Where(s => s.Relay == getRelayR2)
                    .Where(s => s.Name == "SoilMoisture").FirstOrDefault();
                var getLumR2 = _context.Measurements
                    .Where(s => s.Relay == getRelayR2)
                    .Where(s => s.Name == "Lum").FirstOrDefault();
                var getTemperatureR2 = _context.Measurements
                    .Where(s => s.Relay == getRelayR2)
                    .Where(s => s.Name == "Temperature").FirstOrDefault();

                    var getPressureR2 = _context.Measurements
                 .Where(s => s.Relay == getRelayR2)
               .Where(s => s.Name == "Pressure").FirstOrDefault();

                    var getHumidityR2 = _context.Measurements
                 .Where(s => s.Relay == getRelayR2)
               .Where(s => s.Name == "Humidity").FirstOrDefault();

                    var findInentity1R2SoilMoisture = entity1R2.Any(s => s.Measurement == getSoilMoistureR2);
                var findentity1R2Lum = entity1R2.Any(s => s.Measurement == getLumR2);
                var findentity1R2Temperature = entity1R2.Any(s => s.Measurement == getTemperatureR2);
                    var findentity1R2Pressure = entity1R2.Any(s => s.Measurement == getPressureR2);
                    var findentity1R2Humidity = entity1R2.Any(s => s.Measurement == getHumidityR2);

                    if (findInentity1R2SoilMoisture)
                {
                    var SoilMoistureDownLevelR2 = _context.Measurements
                        .Where(s => s.Relay == getRelayR2)
                  .SingleOrDefault(s => s.Name == "SoilMoisture").DownLevel;

                    var SoilMoistureUpLevelR2 = _context.Measurements
                        .Where(s => s.Relay == getRelayR2)
                        .SingleOrDefault(s => s.Name == "SoilMoisture").UpLevel;

                    if (agriData.SoilMoisture < SoilMoistureDownLevelR2)
                    {
                        StaticFunc._IsWateredSoilMoistureR2 = false;
                    }
                    if (agriData.SoilMoisture > SoilMoistureUpLevelR2)
                    {
                        StaticFunc._IsWateredSoilMoistureR2 = true;
                    }
                }
                else
                {
                    StaticFunc._IsWateredSoilMoistureR2 = false;
                }

                if (findentity1R2Lum)
                {
                    var LumDownLevelR2 = _context.Measurements
                        .Where(s => s.Relay == getRelayR2)
                  .SingleOrDefault(s => s.Name == "Lum").DownLevel;

                    var LumUpLevelR2 = _context.Measurements
                        .Where(s => s.Relay == getRelayR2)
                        .SingleOrDefault(s => s.Name == "Lum").UpLevel;

                    if (agriData.Lum < LumDownLevelR2)
                    {
                        StaticFunc._IsWateredLumR2 = false;
                    }
                    if (agriData.Lum > LumUpLevelR2)
                    {
                        StaticFunc._IsWateredLumR2 = true;
                    }
                }
                else
                {
                    StaticFunc._IsWateredLumR2 = false;
                }

                if (findentity1R2Temperature)
                {
                    var TemperatureDownLevelR2 = _context.Measurements
                        .Where(s => s.Relay == getRelayR2)
                  .SingleOrDefault(s => s.Name == "Temperature").DownLevel;

                    var TemperatureUpLevelR2 = _context.Measurements
                        .Where(s => s.Relay == getRelayR2)
                        .SingleOrDefault(s => s.Name == "Temperature").UpLevel;

                    if (agriData.Temperature < TemperatureDownLevelR2)
                    {
                        StaticFunc._IsWateredTemperatureR2 = false;
                    }
                    if (agriData.Temperature > TemperatureUpLevelR2)
                    {
                        StaticFunc._IsWateredTemperatureR2 = true;
                    }
                }
                else
                {
                    StaticFunc._IsWateredTemperatureR2 = false;
                }

                    if (findentity1R2Pressure)
                    {
                        var PressureDownLevelR2 = _context.Measurements
                              .Where(s => s.Relay == getRelayR2)
                      .SingleOrDefault(s => s.Name == "Pressure").DownLevel;

                        var PressureUpLevelR2 = _context.Measurements
                              .Where(s => s.Relay == getRelayR2)
                            .SingleOrDefault(s => s.Name == "Pressure").UpLevel;

                        if (agriData.Pressure < PressureDownLevelR2)
                        {
                            StaticFunc._IsWateredPressureR2 = false;
                        }
                        if (agriData.Pressure > PressureUpLevelR2)
                        {
                            StaticFunc._IsWateredPressureR2 = true;
                        }
                    }
                    else
                    {
                        StaticFunc._IsWateredPressureR2 = false;
                    }

                    if (findentity1R2Humidity)
                    {
                        var HumidityDownLevelR2 = _context.Measurements
                              .Where(s => s.Relay == getRelayR2)
                      .SingleOrDefault(s => s.Name == "Humidity").DownLevel;

                        var HumidityUpLevelR2 = _context.Measurements
                              .Where(s => s.Relay == getRelayR2)
                            .SingleOrDefault(s => s.Name == "Humidity").UpLevel;

                        Console.WriteLine($"HumiditySensor: {agriData.Humidity}");
                       

                        Console.WriteLine($"HumidityDownLevel: {HumidityDownLevelR2}");
                        Console.WriteLine($"HumidityUpLevel: {HumidityUpLevelR2}");

                        if (agriData.Humidity < HumidityDownLevelR2)
                        {
                            StaticFunc._IsWateredHumidityR2 = false;
                        }
                        if (agriData.Humidity > HumidityUpLevelR2)
                        {
                            StaticFunc._IsWateredHumidityR2 = true;
                        }
                    }
                    else
                    {
                        StaticFunc._IsWateredHumidityR2 = false;
                    }

                    //if (agriData.SoilMoisture < SoilMoistureDownLevel && agriData.Lum < LumDownLevel && StaticFunc._firstTime==1)
                    //{
                    //    StaticFunc._IsWatered = false;
                    //}
                    //if (agriData.SoilMoisture > SoilMoistureUpLevel)
                    //{
                    //    StaticFunc._IsWatered = true;
                    //}

                    //if (StaticFunc._IsWatered == false)
                    //{
                    //    Pi.Gpio[pinrelay].Write(false); // Relay On
                    //    System.Threading.Thread.Sleep(delay);
                    //    agriData.IsRelayOnNotification= StaticFunc._IsRelayNotificaton = true;
                    //}
                    //else
                    //{
                    //    Pi.Gpio[pinrelay].Write(true); // Relay Off
                    //    System.Threading.Thread.Sleep(delay);
                    //    agriData.IsRelayOnNotification = StaticFunc._IsRelayNotificaton = false;
                    //}

                    if (StaticFunc._IsWateredSoilMoistureR2 == false 
                        && StaticFunc._IsWateredLumR2 == false
                        && StaticFunc._IsWateredTemperatureR2 == false
                        && StaticFunc._IsWateredPressureR2 == false
                       && StaticFunc._IsWateredHumidityR2 == false)
                    {
                    Pi.Gpio[pinrelayR2].Write(false); // Relay On
                    System.Threading.Thread.Sleep(delay);
                    agriData.IsRelayOnNotification = StaticFunc._IsRelayNotificaton = true;
                    agriData.IsR2On = true;
                }
                else
                {
                    Pi.Gpio[pinrelayR2].Write(true); // Relay Off
                    System.Threading.Thread.Sleep(delay);
                    agriData.IsRelayOnNotification = StaticFunc._IsRelayNotificaton = false;
                    agriData.IsR2On = false;
                }

                StaticFunc._firstTime = 1;

                #endregion RELAY2

                #region RELAY3

                //ForRelay1

                var entity1R3 = _context.RulesForRelays
                    .Where(s => s.Relay.RelayName == "Relay3")
                    .ToList();
                var getRelayR3 = _context.Relays
                        .Where(s => s.RelayName == "Relay3").FirstOrDefault();

                var getSoilMoistureR3 = _context.Measurements
                    .Where(s => s.Relay == getRelayR3)
                    .Where(s => s.Name == "SoilMoisture").FirstOrDefault();
                var getLumR3 = _context.Measurements
                      .Where(s => s.Relay == getRelayR3)
                    .Where(s => s.Name == "Lum").FirstOrDefault();
                var getTemperatureR3 = _context.Measurements
                      .Where(s => s.Relay == getRelayR3)
                    .Where(s => s.Name == "Temperature").FirstOrDefault();


                    var getPressureR3 = _context.Measurements
                 .Where(s => s.Relay == getRelayR3)
               .Where(s => s.Name == "Pressure").FirstOrDefault();

                    var getHumidityR3 = _context.Measurements
                 .Where(s => s.Relay == getRelayR3)
               .Where(s => s.Name == "Humidity").FirstOrDefault();

                    var findentity1R3Pressure = entity1R3.Any(s => s.Measurement == getPressureR3);
                    var findentity1R3Humidity = entity1R3.Any(s => s.Measurement == getHumidityR3);



                    var findInentity1R3SoilMoisture = entity1R3.Any(s => s.Measurement == getSoilMoistureR3);
                var findentity1R3Lum = entity1R3.Any(s => s.Measurement == getLumR3);
                var findentity1R3Temperature = entity1R3.Any(s => s.Measurement == getTemperatureR3);

                if (findInentity1R3SoilMoisture)
                {
                    var SoilMoistureDownLevelR3 = _context.Measurements
                          .Where(s => s.Relay == getRelayR3)
                  .SingleOrDefault(s => s.Name == "SoilMoisture").DownLevel;

                    var SoilMoistureUpLevelR3 = _context.Measurements
                          .Where(s => s.Relay == getRelayR3)
                        .SingleOrDefault(s => s.Name == "SoilMoisture").UpLevel;

                    if (agriData.SoilMoisture < SoilMoistureDownLevelR3)
                    {
                        StaticFunc._IsWateredSoilMoistureR3 = false;
                    }
                    if (agriData.SoilMoisture > SoilMoistureUpLevelR3)
                    {
                        StaticFunc._IsWateredSoilMoistureR3 = true;
                    }
                }
                else
                {
                    StaticFunc._IsWateredSoilMoistureR3 = false;
                }

                if (findentity1R3Lum)
                {
                    var LumDownLevelR3 = _context.Measurements
                          .Where(s => s.Relay == getRelayR3)
                  .SingleOrDefault(s => s.Name == "Lum").DownLevel;

                    var LumUpLevelR3 = _context.Measurements
                          .Where(s => s.Relay == getRelayR3)
                        .SingleOrDefault(s => s.Name == "Lum").UpLevel;

                    if (agriData.Lum < LumDownLevelR3)
                    {
                        StaticFunc._IsWateredLumR3 = false;
                    }
                    if (agriData.Lum > LumUpLevelR3)
                    {
                        StaticFunc._IsWateredLumR3 = true;
                    }
                }
                else
                {
                    StaticFunc._IsWateredLumR3 = false;
                }

                if (findentity1R3Temperature)
                {
                    var TemperatureDownLevelR3 = _context.Measurements
                          .Where(s => s.Relay == getRelayR3)
                  .SingleOrDefault(s => s.Name == "Temperature").DownLevel;

                    var TemperatureUpLevelR3 = _context.Measurements
                          .Where(s => s.Relay == getRelayR3)
                        .SingleOrDefault(s => s.Name == "Temperature").UpLevel;

                    if (agriData.Temperature < TemperatureDownLevelR3)
                    {
                        StaticFunc._IsWateredTemperatureR3 = false;
                    }
                    if (agriData.Temperature > TemperatureUpLevelR3)
                    {
                        StaticFunc._IsWateredTemperatureR3 = true;
                    }
                }
                else
                {
                    StaticFunc._IsWateredTemperatureR3 = false;
                }

                    if (findentity1R3Pressure)
                    {
                        var PressureDownLevelR3 = _context.Measurements
                              .Where(s => s.Relay == getRelayR3)
                      .SingleOrDefault(s => s.Name == "Pressure").DownLevel;

                        var PressureUpLevelR3 = _context.Measurements
                              .Where(s => s.Relay == getRelayR3)
                            .SingleOrDefault(s => s.Name == "Pressure").UpLevel;

                        if (agriData.Pressure < PressureDownLevelR3)
                        {
                            StaticFunc._IsWateredPressureR3 = false;
                        }
                        if (agriData.Pressure > PressureUpLevelR3)
                        {
                            StaticFunc._IsWateredPressureR3 = true;
                        }
                    }
                    else
                    {
                        StaticFunc._IsWateredPressureR3 = false;
                    }

                    if (findentity1R3Humidity)
                    {
                        var HumidityDownLevelR3 = _context.Measurements
                              .Where(s => s.Relay == getRelayR3)
                      .SingleOrDefault(s => s.Name == "Humidity").DownLevel;

                        var HumidityUpLevelR3 = _context.Measurements
                              .Where(s => s.Relay == getRelayR3)
                            .SingleOrDefault(s => s.Name == "Humidity").UpLevel;

                        Console.WriteLine($"HumiditySensor: {agriData.Humidity}");


                        Console.WriteLine($"HumidityDownLevel: {HumidityDownLevelR3}");
                        Console.WriteLine($"HumidityUpLevel: {HumidityUpLevelR3}");

                        if (agriData.Humidity < HumidityDownLevelR3)
                        {
                            StaticFunc._IsWateredHumidityR3 = false;
                        }
                        if (agriData.Humidity > HumidityUpLevelR3)
                        {
                            StaticFunc._IsWateredHumidityR3 = true;
                        }
                    }
                    else
                    {
                        StaticFunc._IsWateredHumidityR3 = false;
                    }

                    //if (agriData.SoilMoisture < SoilMoistureDownLevel && agriData.Lum < LumDownLevel && StaticFunc._firstTime==1)
                    //{
                    //    StaticFunc._IsWatered = false;
                    //}
                    //if (agriData.SoilMoisture > SoilMoistureUpLevel)
                    //{
                    //    StaticFunc._IsWatered = true;
                    //}

                    //if (StaticFunc._IsWatered == false)
                    //{
                    //    Pi.Gpio[pinrelay].Write(false); // Relay On
                    //    System.Threading.Thread.Sleep(delay);
                    //    agriData.IsRelayOnNotification= StaticFunc._IsRelayNotificaton = true;
                    //}
                    //else
                    //{
                    //    Pi.Gpio[pinrelay].Write(true); // Relay Off
                    //    System.Threading.Thread.Sleep(delay);
                    //    agriData.IsRelayOnNotification = StaticFunc._IsRelayNotificaton = false;
                    //}

                    Console.WriteLine("R1");
                    Console.WriteLine("_IsWateredSoilMoistureR1=" + StaticFunc._IsWateredSoilMoistureR1);
                    Console.WriteLine("_IsWateredLumR1=" + StaticFunc._IsWateredLumR1);
                    Console.WriteLine("_IsWateredTemperatureR1=" + StaticFunc._IsWateredTemperatureR1);
                    Console.WriteLine("_IsWateredPressureR1=" + StaticFunc._IsWateredPressureR1);
                    Console.WriteLine("_IsWateredHumidityR1=" + StaticFunc._IsWateredHumidityR1);
                    Console.WriteLine("R2");
                    Console.WriteLine("_IsWateredSoilMoistureR2=" + StaticFunc._IsWateredSoilMoistureR2);
                    Console.WriteLine("_IsWateredLumR2=" + StaticFunc._IsWateredLumR2);
                    Console.WriteLine("_IsWateredTemperatureR2=" + StaticFunc._IsWateredTemperatureR2);
                    Console.WriteLine("_IsWateredPressureR1=" + StaticFunc._IsWateredPressureR2);
                    Console.WriteLine("_IsWateredHumidityR1=" + StaticFunc._IsWateredHumidityR2);
                    Console.WriteLine("R3");
                    Console.WriteLine("_IsWateredSoilMoistureR3=" + StaticFunc._IsWateredSoilMoistureR3);
                    Console.WriteLine("_IsWateredLumR3=" + StaticFunc._IsWateredLumR3);
                    Console.WriteLine("_IsWateredTemperatureR3=" + StaticFunc._IsWateredTemperatureR3);
                    Console.WriteLine("_IsWateredPressureR1=" + StaticFunc._IsWateredPressureR3);
                    Console.WriteLine("_IsWateredHumidityR1=" + StaticFunc._IsWateredHumidityR3);

                    if (StaticFunc._IsWateredSoilMoistureR3 == false 
                        && StaticFunc._IsWateredLumR3 == false 
                        && StaticFunc._IsWateredTemperatureR3 == false
                         && StaticFunc._IsWateredPressureR3 == false
                       && StaticFunc._IsWateredHumidityR3 == false)
                    {
                    Pi.Gpio[pinrelayR3].Write(false); // Relay On
                    System.Threading.Thread.Sleep(delay);
                    agriData.IsRelayOnNotification = StaticFunc._IsRelayNotificaton = true;
                    agriData.IsR3On = true;
                }
                else
                {
                    Pi.Gpio[pinrelayR3].Write(true); // Relay Off
                    System.Threading.Thread.Sleep(delay);
                    agriData.IsRelayOnNotification = StaticFunc._IsRelayNotificaton = false;
                    agriData.IsR3On = false;
                }

                StaticFunc._firstTime = 1;


                #endregion
            }

            Console.WriteLine("4");

                agriData.IsActive = true;

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