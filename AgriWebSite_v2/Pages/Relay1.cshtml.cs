using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgriWebSite_v2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AgriWebSite_v2.Pages
{
    public class Relay1Model : PageModel
    {
        private readonly ApplicationDbContext _context;
        public Relay1Model(ApplicationDbContext context)
        {
            _context = context;

        }
        [BindProperty]
        public string TempMessage { get; set; }

        public int StatusMessageShow { get; set; }
        [BindProperty]
        public float SoilMoistureUpLimit { get; set; }
        [BindProperty]
        public float SoilMoistureDownLimit { get; set; }
        [BindProperty]
        public bool SoilMoistureIsChecked { get; set; }

        [BindProperty]
        public float LumUpLimit { get; set; }
        [BindProperty]
        public float LumDownLimit { get; set; }
        [BindProperty]
        public bool LumIsChecked { get; set; }

        [BindProperty]
        public float TemperatureUpLimit { get; set; }
        [BindProperty]
        public float TemperatureDownLimit { get; set; }
        [BindProperty]
        public bool TemperatureIsChecked { get; set; }

        [BindProperty]
        public float PressureUpLimit { get; set; }
        [BindProperty]
        public float PressureDownLimit { get; set; }
        [BindProperty]
        public bool PressureIsChecked { get; set; }

        [BindProperty]
        public float HumidityUpLimit { get; set; }
        [BindProperty]
        public float HumidityDownLimit { get; set; }
        [BindProperty]
        public bool HumidityIsChecked { get; set; }


        public void OnGet()
        {
            StatusMessageShow = 0;
            var getRelay = _context.Relays
 .Where(s => s.RelayName == "Relay1").FirstOrDefault();

            var entity = _context.Measurements
                 .Where(s => s.Relay == getRelay)
                .FirstOrDefault(item => item.Name == "SoilMoisture");
            SoilMoistureDownLimit = entity.DownLevel;
            SoilMoistureUpLimit = entity.UpLevel;

            var entity2 = _context.Measurements
                 .Where(s => s.Relay == getRelay)
                .FirstOrDefault(item => item.Name == "Lum");
            LumDownLimit = entity2.DownLevel;
            LumUpLimit = entity2.UpLevel;

            var entity3 = _context.Measurements
                 .Where(s => s.Relay == getRelay)
                .FirstOrDefault(item => item.Name == "Temperature");
            TemperatureDownLimit = entity3.DownLevel;
            TemperatureUpLimit = entity3.UpLevel;

            var entity4 = _context.Measurements
     .Where(s => s.Relay == getRelay)
    .FirstOrDefault(item => item.Name == "Pressure");
            PressureDownLimit = entity4.DownLevel;
            PressureUpLimit = entity4.UpLevel;

            var entity5 = _context.Measurements
     .Where(s => s.Relay == getRelay)
    .FirstOrDefault(item => item.Name == "Humidity");
            HumidityDownLimit = entity5.DownLevel;
            HumidityUpLimit = entity5.UpLevel;

            var selectedMeas = _context.RulesForRelays.Where(s => s.Relay == getRelay).ToList();

            if (selectedMeas.Any(s => s.Measurement.Name == "SoilMoisture"))
            {
                SoilMoistureIsChecked = true;
            }

            if (selectedMeas.Any(s => s.Measurement.Name == "Lum"))
            {
                LumIsChecked = true;
            }

            if (selectedMeas.Any(s => s.Measurement.Name == "Temperature"))
            {
                TemperatureIsChecked = true;
            }

            if (selectedMeas.Any(s => s.Measurement.Name == "Pressure"))
            {
                PressureIsChecked = true;
            }

            if (selectedMeas.Any(s => s.Measurement.Name == "Humidity"))
            {
                HumidityIsChecked = true;
            }

        }

        public void OnPost()
        {
            var getRelay = _context.Relays
    .Where(s => s.RelayName == "Relay1").FirstOrDefault();
            StatusMessageShow = 1;
            if (SoilMoistureIsChecked == true 
                || LumIsChecked == true 
                || TemperatureIsChecked == true
                || PressureIsChecked==true
                || HumidityIsChecked==true)
            {
                var entity3 = _context.RulesForRelays.
                         Where(s => s.Relay.RelayName == "Relay1")
                         .ToList();

                if (entity3.Count > 0)
                {
                    foreach (var r in entity3)
                    {
                        _context.RulesForRelays.Remove(r);
                        _context.SaveChanges();
                    }
                }

   
            }


            if (SoilMoistureIsChecked)
            {
                var entity = _context.Measurements
                    .Where(s => s.Relay==getRelay)
                   .FirstOrDefault(item => item.Name == "SoilMoisture");
                entity.DownLevel = SoilMoistureDownLimit;
                entity.UpLevel = SoilMoistureUpLimit;
                _context.Measurements.Update(entity);
                _context.SaveChanges();
                SoilMoistureDownLimit = entity.DownLevel;
                SoilMoistureUpLimit = entity.UpLevel;



                var getSoilMoisture = _context.Measurements
                    .Where(s =>s.Relay==getRelay)
                    .Where(s => s.Name == "SoilMoisture").FirstOrDefault();

                var rel = new RulesForRelay
                {
                    Measurement = getSoilMoisture,
                    Relay = getRelay
                };
                _context.RulesForRelays.Add(rel);
                _context.SaveChanges();

            }

            if (LumIsChecked)
            {
                var entityLum1 = _context.Measurements
                     .Where(s => s.Relay == getRelay)
                    .FirstOrDefault(item => item.Name == "Lum");
                entityLum1.DownLevel = LumDownLimit;
                entityLum1.UpLevel = LumUpLimit;
                _context.Measurements.Update(entityLum1);
                _context.SaveChanges();

                LumDownLimit = entityLum1.DownLevel;
                LumUpLimit = entityLum1.UpLevel;


                var getMeasurement = _context.Measurements
                       .Where(s => s.Relay == getRelay)
                    .Where(s => s.Name == "Lum").FirstOrDefault();
             
                var rel = new RulesForRelay
                {
                    Measurement = getMeasurement,
                    Relay = getRelay
                };
                _context.RulesForRelays.Add(rel);
                _context.SaveChanges();

            }

            if (TemperatureIsChecked)
            {
                var entityTemperature1 = _context.Measurements
                     .Where(s => s.Relay == getRelay)
                    .FirstOrDefault(item => item.Name == "Temperature");
                entityTemperature1.DownLevel = TemperatureDownLimit;
                entityTemperature1.UpLevel = TemperatureUpLimit;
                _context.Measurements.Update(entityTemperature1);
                _context.SaveChanges();

                TemperatureDownLimit = entityTemperature1.DownLevel;
                TemperatureUpLimit = entityTemperature1.UpLevel;



                var getMeasurement = _context.Measurements
                       .Where(s => s.Relay == getRelay)
                    .Where(s => s.Name == "Temperature").FirstOrDefault();
               
                var rel = new RulesForRelay
                {
                    Measurement = getMeasurement,
                    Relay = getRelay
                };
                _context.RulesForRelays.Add(rel);
                _context.SaveChanges();

            }

            if (PressureIsChecked)
            {
                var entityPressure1 = _context.Measurements
                     .Where(s => s.Relay == getRelay)
                    .FirstOrDefault(item => item.Name == "Pressure");
                entityPressure1.DownLevel = PressureDownLimit;
                entityPressure1.UpLevel = PressureUpLimit;
                _context.Measurements.Update(entityPressure1);
                _context.SaveChanges();

                PressureDownLimit = entityPressure1.DownLevel;
                PressureUpLimit = entityPressure1.UpLevel;



                var getMeasurement = _context.Measurements
                       .Where(s => s.Relay == getRelay)
                    .Where(s => s.Name == "Pressure").FirstOrDefault();

                var rel = new RulesForRelay
                {
                    Measurement = getMeasurement,
                    Relay = getRelay
                };
                _context.RulesForRelays.Add(rel);
                _context.SaveChanges();

            }

            if (HumidityIsChecked)
            {
                var entityHumidity1 = _context.Measurements
                     .Where(s => s.Relay == getRelay)
                    .FirstOrDefault(item => item.Name == "Humidity");
                entityHumidity1.DownLevel = HumidityDownLimit;
                entityHumidity1.UpLevel = HumidityUpLimit;
                _context.Measurements.Update(entityHumidity1);
                _context.SaveChanges();

                HumidityDownLimit = entityHumidity1.DownLevel;
                HumidityUpLimit = entityHumidity1.UpLevel;



                var getMeasurement = _context.Measurements
                       .Where(s => s.Relay == getRelay)
                    .Where(s => s.Name == "Humidity").FirstOrDefault();

                var rel = new RulesForRelay
                {
                    Measurement = getMeasurement,
                    Relay = getRelay
                };
                _context.RulesForRelays.Add(rel);
                _context.SaveChanges();

            }

            TempMessage = "The Database has been updated";
        }
    }
}