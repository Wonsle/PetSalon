using System;
using System.Linq;
using System.Collections.Generic;

namespace PetSalon.Models
{
	public  class ReserverRepository : EFRepository<Reserver>, IReserverRepository
	{

	}

	public  interface IReserverRepository : IRepository<Reserver>
	{

	}
}