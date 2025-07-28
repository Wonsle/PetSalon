using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;
using PetSalon.Services;

namespace PetSalon.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet(Name = nameof(GetReservationList))]
        public async Task<ActionResult<IList<ReserveRecord>>> GetReservationList()
        {
            return Ok(await _reservationService.GetReservationList());
        }

        [HttpGet("{reservationId}", Name = nameof(GetReservation))]
        public async Task<ActionResult<ReserveRecord>> GetReservation(long reservationId)
        {
            var reservation = await _reservationService.GetReservation(reservationId);
            if (reservation == null)
                return NotFound();
            return reservation;
        }

        [HttpGet("date/{date}", Name = nameof(GetReservationsByDate))]
        public async Task<ActionResult<IList<ReserveRecord>>> GetReservationsByDate(DateTime date)
        {
            return Ok(await _reservationService.GetReservationsByDate(date));
        }

        [HttpGet("pet/{petId}", Name = nameof(GetReservationsByPet))]
        public async Task<ActionResult<IList<ReserveRecord>>> GetReservationsByPet(long petId)
        {
            return Ok(await _reservationService.GetReservationsByPet(petId));
        }

        [HttpPost(Name = nameof(CreateReservation))]
        public async Task<ActionResult<long>> CreateReservation(ReservationCreateDto reservation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var reservationId = await _reservationService.CreateReservation(reservation);
                return CreatedAtAction(nameof(GetReservation), 
                    new { reservationId = reservationId }, reservationId);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{reservationId}", Name = nameof(UpdateReservation))]
        public async Task<IActionResult> UpdateReservation(long reservationId, ReservationUpdateDto reservation)
        {
            if (reservationId != reservation.ReservationId)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _reservationService.UpdateReservation(reservation);
            return NoContent();
        }

        [HttpDelete("{reservationId}", Name = nameof(DeleteReservation))]
        public async Task<IActionResult> DeleteReservation(long reservationId)
        {
            var reservation = await _reservationService.GetReservation(reservationId);
            if (reservation == null)
                return NotFound();

            await _reservationService.DeleteReservation(reservationId);
            return NoContent();
        }

        [HttpPost("{reservationId}/status", Name = nameof(UpdateReservationStatus))]
        public async Task<IActionResult> UpdateReservationStatus(long reservationId, [FromBody] string status)
        {
            await _reservationService.UpdateReservationStatus(reservationId, status);
            return NoContent();
        }

        [HttpPost("{reservationId}/complete", Name = nameof(CompleteReservation))]
        public async Task<IActionResult> CompleteReservation(long reservationId)
        {
            await _reservationService.CompleteReservation(reservationId);
            return NoContent();
        }

        [HttpGet("pet/{petId}/subscription-check", Name = nameof(CheckSubscriptionEligibility))]
        public async Task<ActionResult<bool>> CheckSubscriptionEligibility(long petId, [FromQuery] DateTime? date = null)
        {
            var checkDate = date ?? DateTime.Now;
            var isEligible = await _reservationService.CheckSubscriptionEligibility(petId, checkDate);
            return Ok(isEligible);
        }

        [HttpPost("calculate-cost", Name = nameof(CalculateReservationCost))]
        public async Task<ActionResult<ServiceCalculationDto>> CalculateReservationCost([FromBody] ReservationCalculationRequest request)
        {
            var calculation = await _reservationService.CalculateReservationCost(
                request.PetId, request.ServiceIds, request.AddonIds);
            return Ok(calculation);
        }
    }

    public class ReservationCalculationRequest
    {
        public long PetId { get; set; }
        public List<long> ServiceIds { get; set; } = new List<long>();
        public List<long> AddonIds { get; set; } = new List<long>();
    }
}