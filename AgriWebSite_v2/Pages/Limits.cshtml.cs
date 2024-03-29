﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgriWebSite_v2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AgriWebSite_v2.Pages
{
   // [Authorize]
    public class LimitsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public LimitsModel(ApplicationDbContext context)
        {
            _context = context;
          
        }

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


        public void OnGet()
        {
            var entity = _context.Measurements.FirstOrDefault(item => item.Name == "SoilMoisture");
            SoilMoistureDownLimit = entity.DownLevel;
            SoilMoistureUpLimit = entity.UpLevel;

            var entity2 = _context.Measurements.FirstOrDefault(item => item.Name == "Lum");
            LumDownLimit = entity2.DownLevel;
            LumUpLimit = entity2.UpLevel;

            var entity3 = _context.Measurements.FirstOrDefault(item => item.Name == "Temperature");
            TemperatureDownLimit = entity3.DownLevel;
            TemperatureUpLimit = entity3.UpLevel;

        }

        public void OnPost()
        {

            if (SoilMoistureIsChecked == true || LumIsChecked == true || TemperatureIsChecked == true)
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
                var entity = _context.Measurements.FirstOrDefault(item => item.Name == "SoilMoisture");
                entity.DownLevel = SoilMoistureDownLimit;
                entity.UpLevel = SoilMoistureUpLimit;
                _context.Measurements.Update(entity);
                _context.SaveChanges();
                SoilMoistureDownLimit = entity.DownLevel;
                SoilMoistureUpLimit = entity.UpLevel;

            
                
                var getSoilMoisture = _context.Measurements.Where(s => s.Name == "SoilMoisture").FirstOrDefault();
                var getRelay = _context.Relays.Where(s => s.RelayName == "Relay1").FirstOrDefault();
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
                var entityLum1 = _context.Measurements.FirstOrDefault(item => item.Name == "Lum");
                entityLum1.DownLevel = LumDownLimit;
                entityLum1.UpLevel = LumUpLimit;
                _context.Measurements.Update(entityLum1);
                _context.SaveChanges();

                LumDownLimit = entityLum1.DownLevel;
                LumUpLimit = entityLum1.UpLevel;

             
                var getMeasurement = _context.Measurements.Where(s => s.Name == "Lum").FirstOrDefault();
                var getRelay = _context.Relays.Where(s => s.RelayName == "Relay1").FirstOrDefault();
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
                var entityTemperature1 = _context.Measurements.FirstOrDefault(item => item.Name == "Temperature");
                entityTemperature1.DownLevel =TemperatureDownLimit;
                entityTemperature1.UpLevel = TemperatureUpLimit;
                _context.Measurements.Update(entityTemperature1);
                _context.SaveChanges();

                TemperatureDownLimit = entityTemperature1.DownLevel;
                TemperatureUpLimit = entityTemperature1.UpLevel;

              
                
                var getMeasurement = _context.Measurements.Where(s => s.Name == "Temperature").FirstOrDefault();
                var getRelay = _context.Relays.Where(s => s.RelayName == "Relay1").FirstOrDefault();
                var rel = new RulesForRelay
                {
                    Measurement = getMeasurement,
                    Relay = getRelay
                };
                _context.RulesForRelays.Add(rel);
                _context.SaveChanges();

            }
        }
    }
}