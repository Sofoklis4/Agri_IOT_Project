using AgriWebSite_v2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using AgriWebSite_v2.Classes;
using AgriWebSite_v2.Data;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AgriWebSite_v2.Pages
{
    [Authorize]
    public class DiplomatikiModel : PageModel
    {
        [BindProperty]
        public string Temperature_webpage { get; set; }
        [BindProperty]
        public string Pressure { get; set; }
        [BindProperty]
        public string Humidity { get; set; }

        public string SoilMoisture { get; set; }

        public string Lum { get; set; }

        public  bool IsWatered { get; set; }

        public bool IsRelayOnNotification { get; set; }
        public bool IsR1On { get; set; }
        public string IsR1OnString { get; set; }

        public bool IsR2On { get; set; }
        public string IsR2OnString { get; set; }

        public bool IsR3On { get; set; }
        public string IsR3OnString { get; set; }
        public string IsRelayOnNotificationString { get; set; }


        [BindProperty]
        public DateTime Date { get; set; }

        public string DateString { get; set; }
        [BindProperty]
        public long DateFormatted => (long)(Date - new DateTime(1970, 1, 1)).TotalMilliseconds;

        public string ChartViewTemperature { get; set; }
        public string ChartViewHumidity { get; set; }
        public string ChartViewPressure { get; set; }
        public string ChartViewSoilMoisture { get; set; }
        public string ChartViewLum{ get; set; }
        private readonly ApplicationDbContext _context;
        public DiplomatikiModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task OnGetAsync()
        {
            var _fromApi = await CallSIoTApi.GetAPI($"AgriDiplomatiki");
            var jsonToObj = JsonConvert.DeserializeObject<AgriData>(_fromApi);

            Temperature_webpage = jsonToObj.Temperature.ToString("0.0");
            Humidity = jsonToObj.Humidity.ToString("0.0");
            Pressure = jsonToObj.Pressure.ToString("0.0");
            SoilMoisture = jsonToObj.SoilMoisture.ToString("0.0");
            Lum = jsonToObj.Lum.ToString("0");
            DateString = jsonToObj.Date.ToString("dd MMM yyyy HH:mm");
            // IsWatered = jsonToObj.IsWatered;
            IsR1On = jsonToObj.IsR1On;
            IsR2On = jsonToObj.IsR2On;
            IsR3On = jsonToObj.IsR3On;
            IsRelayOnNotification = jsonToObj.IsRelayOnNotification;

            if(IsRelayOnNotification==true)
            {
                IsRelayOnNotificationString = "Ναι";
            }

            else
            {
                IsRelayOnNotificationString = "Όχι";
            }

            if (IsR1On == true)
            {
                IsR1OnString = "Ναι";
            }

            else
            {
                IsR1OnString = "Όχι";
            }
            if (IsR2On == true)
            {
                IsR2OnString = "Ναι";
            }

            else
            {
                IsR2OnString = "Όχι";
            }
            if (IsR3On == true)
            {
                IsR3OnString = "Ναι";
            }

            else
            {
                IsR3OnString = "Όχι";
            }

            //Temperature
            var fromDbTemperature = await _context.MeasurementsValues
                 //  .Include(s => s.Measurement.Component.Device.Asset.Sector.AssignedUsersSectors)
                 //  .Include(mrv => mrv.Measurement)
                 //.ThenInclude(m => m.Component)
                 //.ThenInclude(c => c.Device)
                 // .Include(d => d.Measurement.Component.Device.Asset.Sector)
                 //.ThenInclude(se => se.Sector)
                 //.ThenInclude(s => s.Site)
                 //.ThenInclude(s => s.Company)
                 .Where(s => s.Measurement.Relay.RelayName == "Relay1")
                .Where(s => s.Measurement.Name == "Temperature")
                //.Where(s => s.DateTime > DateTime.Today.AddDays(-days))
                .Select(s => new MeasurementsValues()
                {
                    Id = s.Id,
                    Value = s.Value,
                    DateTime = s.DateTime
                })
                .ToListAsync();

            ChartViewTemperature = JsonConvert.SerializeObject(fromDbTemperature);
            ChartViewTemperature = "{\"jsonarray\":" + ChartViewTemperature + "}".Trim();

            //Humidity
            
            var fromDbHumidity = await _context.MeasurementsValues
                //  .Include(s => s.Measurement.Component.Device.Asset.Sector.AssignedUsersSectors)
                //  .Include(mrv => mrv.Measurement)
                //.ThenInclude(m => m.Component)
                //.ThenInclude(c => c.Device)
                // .Include(d => d.Measurement.Component.Device.Asset.Sector)
                //.ThenInclude(se => se.Sector)
                //.ThenInclude(s => s.Site)
                //.ThenInclude(s => s.Company)

                .Where(s => s.Measurement.Name == "Humidity")
                 .Where(s => s.Measurement.Relay.RelayName == "Relay1")
                //.Where(s => s.DateTime > DateTime.Today.AddDays(-days))
                .Select(s => new MeasurementsValues()
                {
                    Id = s.Id,
                    Value = s.Value,
                    DateTime = s.DateTime
                })
                .ToListAsync();

            ChartViewHumidity = JsonConvert.SerializeObject(fromDbHumidity);
            ChartViewHumidity = "{\"jsonarray\":" + ChartViewHumidity + "}".Trim();

            //Pressure

            var fromDbPressure = await _context.MeasurementsValues
                //  .Include(s => s.Measurement.Component.Device.Asset.Sector.AssignedUsersSectors)
                //  .Include(mrv => mrv.Measurement)
                //.ThenInclude(m => m.Component)
                //.ThenInclude(c => c.Device)
                // .Include(d => d.Measurement.Component.Device.Asset.Sector)
                //.ThenInclude(se => se.Sector)
                //.ThenInclude(s => s.Site)
                //.ThenInclude(s => s.Company)

                .Where(s => s.Measurement.Name == "Pressure")
                 .Where(s => s.Measurement.Relay.RelayName == "Relay1")
                //.Where(s => s.DateTime > DateTime.Today.AddDays(-days))
                .Select(s => new MeasurementsValues()
                {
                    Id = s.Id,
                    Value = s.Value,
                    DateTime = s.DateTime
                })
                .ToListAsync();

            ChartViewPressure= JsonConvert.SerializeObject(fromDbPressure);
            ChartViewPressure = "{\"jsonarray\":" + ChartViewPressure + "}".Trim();

            //SoilMoisture

            var fromDbSoilMoisture = await _context.MeasurementsValues
                //  .Include(s => s.Measurement.Component.Device.Asset.Sector.AssignedUsersSectors)
                //  .Include(mrv => mrv.Measurement)
                //.ThenInclude(m => m.Component)
                //.ThenInclude(c => c.Device)
                // .Include(d => d.Measurement.Component.Device.Asset.Sector)
                //.ThenInclude(se => se.Sector)
                //.ThenInclude(s => s.Site)
                //.ThenInclude(s => s.Company)

                .Where(s => s.Measurement.Name == "SoilMoisture")
                 .Where(s => s.Measurement.Relay.RelayName == "Relay1")
                //.Where(s => s.DateTime > DateTime.Today.AddDays(-days))
                .Select(s => new MeasurementsValues()
                {
                    Id = s.Id,
                    Value = s.Value,
                    DateTime = s.DateTime
                })
                .ToListAsync();

            ChartViewSoilMoisture = JsonConvert.SerializeObject(fromDbSoilMoisture);
            ChartViewSoilMoisture = "{\"jsonarray\":" + ChartViewSoilMoisture + "}".Trim();

            //Lum

            var fromDbLum = await _context.MeasurementsValues
                //  .Include(s => s.Measurement.Component.Device.Asset.Sector.AssignedUsersSectors)
                //  .Include(mrv => mrv.Measurement)
                //.ThenInclude(m => m.Component)
                //.ThenInclude(c => c.Device)
                // .Include(d => d.Measurement.Component.Device.Asset.Sector)
                //.ThenInclude(se => se.Sector)
                //.ThenInclude(s => s.Site)
                //.ThenInclude(s => s.Company)

                .Where(s => s.Measurement.Name == "Lum")
                 .Where(s => s.Measurement.Relay.RelayName == "Relay1")
                //.Where(s => s.DateTime > DateTime.Today.AddDays(-days))
                .Select(s => new MeasurementsValues()
                {
                    Id = s.Id,
                    Value = s.Value,
                    DateTime = s.DateTime
                })
                .ToListAsync();

            ChartViewLum = JsonConvert.SerializeObject(fromDbLum);
            ChartViewLum = "{\"jsonarray\":" + ChartViewLum + "}".Trim();
        }
    }
}