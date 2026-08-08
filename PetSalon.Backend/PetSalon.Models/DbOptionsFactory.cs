using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PetSalon.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetSalon.Models
{
    public class DbOptionsFactory
    {
        public static DbContextOptions<PetSalonContext> DbContextOptions { get; }
        public static string ConnectionString { get; }

        static DbOptionsFactory()
        {

            ConnectionString = Environment.GetEnvironmentVariable(
                "ConnectionStrings__DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Missing required configuration: ConnectionStrings:DefaultConnection.");
            DbContextOptions = new DbContextOptionsBuilder<PetSalonContext>()
                               .UseSqlServer(ConnectionString)
                               .Options;
        }
    }
}
