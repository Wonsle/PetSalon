using System;
using System.Linq;
using System.Collections.Generic;

namespace PetSalon.Models
{
	public  class MemberRepository : EFRepository<Member>, IMemberRepository
	{

	}

	public  interface IMemberRepository : IRepository<Member>
	{

	}
}