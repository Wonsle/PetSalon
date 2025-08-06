using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetSalon.Models
{
    public interface IEntity
    {
        [Column(TypeName = "varchar")] string CreateUser { get; set; }
        [Column(TypeName = "datetime")] DateTime CreateTime { get; set; }
        [Column(TypeName = "varchar")] string ModifyUser { get; set; }
        [Column(TypeName = "datetime")] DateTime ModifyTime { get; set; }

    }
}
