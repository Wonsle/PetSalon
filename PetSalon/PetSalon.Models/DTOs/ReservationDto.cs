namespace PetSalon.Models.DTOs
{
    public class ReservationCreateDto
    {
        public long PetId { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public List<long> ServiceIds { get; set; } = new List<long>();
        public List<long> AddonIds { get; set; } = new List<long>();
        public bool UseSubscription { get; set; }
        public long? SubscriptionId { get; set; }
        public string Memo { get; set; }
        public string Status { get; set; } = "PENDING";
    }

    public class ReservationUpdateDto
    {
        public long ReservationId { get; set; }
        public DateTime? ReservationDate { get; set; }
        public TimeSpan? ReservationTime { get; set; }
        public string Status { get; set; }
        public string Memo { get; set; }
        public List<long> ServiceIds { get; set; } = new List<long>();
        public List<long> AddonIds { get; set; } = new List<long>();
    }

    public class ReservationDetailsDto
    {
        public long ReservationId { get; set; }
        public long PetId { get; set; }
        public string PetName { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public string Status { get; set; }
        public string StatusName { get; set; }
        public string Memo { get; set; }
        public bool UsedSubscription { get; set; }
        public long? SubscriptionId { get; set; }
        public List<ServiceItemDto> Services { get; set; } = new List<ServiceItemDto>();
        public List<ServiceAddonDto> Addons { get; set; } = new List<ServiceAddonDto>();
        public decimal TotalAmount { get; set; }
        public int TotalDuration { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<ContactPersonDto> ContactPersons { get; set; } = new List<ContactPersonDto>();
    }

    public class ContactPersonDto
    {
        public long ContactPersonId { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string ContactNumber { get; set; }
        public string RelationshipType { get; set; }
        public string RelationshipName { get; set; }
    }

    public class ReservationCalendarDto
    {
        public DateTime Date { get; set; }
        public List<ReservationSlotDto> Reservations { get; set; } = new List<ReservationSlotDto>();
        public int TotalReservations => Reservations.Count;
        public bool IsFullyBooked { get; set; }
    }

    public class ReservationSlotDto
    {
        public long ReservationId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string PetName { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string Status { get; set; }
        public string StatusName { get; set; }
        public List<string> ServiceNames { get; set; } = new List<string>();
        public bool IsSubscription { get; set; }
    }
}