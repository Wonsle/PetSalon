using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetSalon.Tools
{
    public static class Utility
    {
        public static DateTime GetSysCurrentTime() => DateTime.UtcNow.AddHours(8);
        public static DateTime GetSysCurrentDate() => DateTime.UtcNow.AddHours(8).Date;
    }
}
