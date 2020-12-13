using System;
using System.Linq;
using System.Collections.Generic;

namespace PetSalon.Models
{
	public  class SubscriptionRepository : EFRepository<Subscription>, ISubscriptionRepository
	{

	}

	public  interface ISubscriptionRepository : IRepository<Subscription>
	{

	}
}