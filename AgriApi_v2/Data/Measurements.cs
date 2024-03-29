﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AgriApi_v2.Data
{
    public class Measurements
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public float UpLevel { get; set; }

        public float DownLevel { get; set; }

        public Relay Relay { get; set; }
    }
}
