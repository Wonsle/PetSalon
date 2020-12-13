
namespace PetSalon.Models
{
	public static class RepositoryHelper
	{
		public static IUnitOfWork GetUnitOfWork()
		{
			return new EFUnitOfWork();
		}
		

		public static MemberRepository GetMemberRepository()
		{
			var repository = new MemberRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static MemberRepository GetMemberRepository(IUnitOfWork unitOfWork)
		{
			var repository = new MemberRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}


		public static PetRepository GetPetRepository()
		{
			var repository = new PetRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static PetRepository GetPetRepository(IUnitOfWork unitOfWork)
		{
			var repository = new PetRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}


		public static ReserverRepository GetReserverRepository()
		{
			var repository = new ReserverRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static ReserverRepository GetReserverRepository(IUnitOfWork unitOfWork)
		{
			var repository = new ReserverRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}


		public static SubscriptionRepository GetSubscriptionRepository()
		{
			var repository = new SubscriptionRepository();
			repository.UnitOfWork = GetUnitOfWork();
			return repository;
		}

		public static SubscriptionRepository GetSubscriptionRepository(IUnitOfWork unitOfWork)
		{
			var repository = new SubscriptionRepository();
			repository.UnitOfWork = unitOfWork;
			return repository;
		}

	}
}