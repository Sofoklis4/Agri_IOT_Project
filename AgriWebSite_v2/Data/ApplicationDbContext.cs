using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AgriWebSite_v2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Measurements> Measurements { get; set; }
        public DbSet<MeasurementsValues> MeasurementsValues { get; set; }

        public DbSet<Relay> Relays { get; set; }
        public DbSet<RulesForRelay> RulesForRelays { get; set; }

        public DbSet<Raspberries> Raspberries { get; set; }
    }
}
