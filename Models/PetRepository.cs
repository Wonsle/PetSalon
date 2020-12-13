using System;
using System.Linq;
using System.Collections.Generic;

namespace PetSalon.Models
{
	public  class PetRepository : EFRepository<Pet>, IPetRepository
	{

	}

	public  interface IPetRepository : IRepository<Pet>
	{

	}
}