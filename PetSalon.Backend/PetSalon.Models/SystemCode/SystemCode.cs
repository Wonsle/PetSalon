using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetSalon.Models.EntityModels
{
    public partial class SystemCode : IEntity
    {
        public string CodeName => $"{Code}-{Name}";

    }
}
