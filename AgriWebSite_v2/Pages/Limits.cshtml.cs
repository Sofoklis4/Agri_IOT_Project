using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgriWebSite_v2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AgriWebSite_v2.Pages
{
    [Authorize]
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
        public float LumDownLimit { get; set; }
        public void OnGet()
        {
            var entity = _context.Measurements.FirstOrDefault(item => item.Name == "SoilMoisture");
            SoilMoistureDownLimit = entity.DownLevel;
            SoilMoistureUpLimit = entity.UpLevel;

            var entity2 = _context.Measurements.FirstOrDefault(item => item.Name == "Lum");
            LumDownLimit = entity2.DownLevel;
            
        }

        public void OnPost()
        {
            var entity = _context.Measurements.FirstOrDefault(item => item.Name=="SoilMoisture");
            entity.DownLevel = SoilMoistureDownLimit;
            entity.UpLevel = SoilMoistureUpLimit;
            _context.Measurements.Update(entity);
            _context.SaveChanges();
            SoilMoistureDownLimit = entity.DownLevel;
            SoilMoistureUpLimit = entity.UpLevel;

            var entity2 = _context.Measurements.FirstOrDefault(item => item.Name == "Lum");
            entity2.DownLevel = LumDownLimit;
            _context.Measurements.Update(entity2);
            _context.SaveChanges();

            LumDownLimit = entity2.DownLevel;
        }
    }
}