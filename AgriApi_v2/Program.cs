using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AgriApi_v2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
            StaticFunc._IsWateredSoilMoistureR1 = true;
            StaticFunc._IsWateredLumR1 = true;
            StaticFunc._IsWateredTemperatureR1 = true;

            StaticFunc._IsWateredSoilMoistureR2 = true;
            StaticFunc._IsWateredLumR2 = true;
            StaticFunc._IsWateredTemperatureR2 = true;

            StaticFunc._IsWateredSoilMoistureR3 = true;
            StaticFunc._IsWateredLumR3 = true;
            StaticFunc._IsWateredTemperatureR3 = true;


        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseKestrel(options =>
            {
                options.Listen(System.Net.IPAddress.Any, 5000);
            })
                .UseStartup<Startup>();
    }
}
