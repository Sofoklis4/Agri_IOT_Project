using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgriApi_v2
{
    public static class StaticFunc
    {
        public static bool _IsWateredSoilMoistureR1 = true;
        public static bool _IsWateredLumR1 = true;
        public static bool _IsWateredTemperatureR1 = true;
        public static bool _IsWateredPressureR1 = true;
        public static bool _IsWateredHumidityR1 = true;

        public static bool _IsWateredSoilMoistureR2 = true;
        public static bool _IsWateredLumR2 = true;
        public static bool _IsWateredTemperatureR2 = true;

        public static bool _IsWateredSoilMoistureR3 = true;
        public static bool _IsWateredLumR3 = true;
        public static bool _IsWateredTemperatureR3 = true;

        public static int _firstTime = 0;

        public static bool _IsRelayNotificaton = false;
    }
}
