using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
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

            var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
            DbContextOptions = new DbContextOptionsBuilder<PetSalonContext>()
                               .UseSqlServer(ConnectionString)
                               .Options;
        }
    }
}
