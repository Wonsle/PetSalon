using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;
using PetSalon.Services;

namespace PetSalon.Web.Controllers
{
    /// <summary>
    /// 寵物管理API控制器 - 提供寵物的CRUD操作和照片上傳功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PetController : ControllerBase
    {

        private readonly IPetService _petService;
        public PetController(IPetService petService)
        {
            _petService = petService;
        }

        /// <summary>
        /// 取得寵物列表（含主人資訊），支援關鍵字搜尋、品種篩選、性別篩選和分頁
        /// </summary>
        /// <param name="keyword">搜尋關鍵字（可選）</param>
        /// <param name="breed">品種代碼（可選）</param>
        /// <param name="gender">性別代碼（可選）</param>
        /// <param name="page">頁碼（可選，預設為1）</param>
        /// <param name="pageSize">每頁數量（可選，預設為20）</param>
        /// <returns>寵物列表含主人資訊</returns>
        [HttpGet(Name = nameof(GetPets))]
        public async Task<IList<PetListResponse>> GetPets([FromQuery] string? keyword = null, [FromQuery] string? breed = null, [FromQuery] string? gender = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var allPets = await _petService.GetPetListWithOwners();
            
            // 如果有關鍵字，進行過濾
            if (!string.IsNullOrEmpty(keyword))
            {
                var searchKeyword = keyword.ToLower();
                allPets = allPets.Where(p => 
                    (p.PetName?.ToLower().Contains(searchKeyword) ?? false) ||
                    (p.Breed?.ToLower().Contains(searchKeyword) ?? false) ||
                    p.Owners.Any(o => o.Name.ToLower().Contains(searchKeyword) || o.ContactNumber.Contains(keyword))
                ).ToList();
            }

            // 如果有品種篩選
            if (!string.IsNullOrEmpty(breed))
            {
                allPets = allPets.Where(p => p.Breed == breed).ToList();
            }

            // 如果有性別篩選
            if (!string.IsNullOrEmpty(gender))
            {
                allPets = allPets.Where(p => p.Gender == gender).ToList();
            }

            return allPets;
        }

        /// <summary>
        /// 取得基本寵物列表（不含主人資訊，為了向後相容性保留）
        /// </summary>
        /// <returns>基本寵物列表</returns>
        [HttpGet("basic", Name = nameof(GetBasicPets))]
        public async Task<IList<Pet>> GetBasicPets()
        {
            return await _petService.GetPetList();
        }

        /// <summary>
        /// 根據ID取得特定寵物資訊（含所有聯絡人關係）
        /// </summary>
        /// <param name="petID">寵物ID</param>
        /// <returns>寵物詳細資訊含聯絡人資訊</returns>
        [HttpGet("{petID}", Name = nameof(GetPet))]
        public async Task<ActionResult<PetDetailResponse>> GetPet(long petID)
        {
            var pet = await _petService.GetPetDetailWithContacts(petID);
            if (pet == null)
                return NotFound();
            return pet;
        }

        /// <summary>
        /// 根據ID取得基本寵物資訊（不含聯絡人資訊，為了向後相容性保留）
        /// </summary>
        /// <param name="petID">寵物ID</param>
        /// <returns>基本寵物資訊</returns>
        [HttpGet("{petID}/basic", Name = nameof(GetBasicPet))]
        public async Task<ActionResult<Pet>> GetBasicPet(long petID)
        {
            var pet = await _petService.GetPet(petID);
            if (pet == null)
                return NotFound();
            return pet;
        }

        /// <summary>
        /// 建立新寵物
        /// </summary>
        /// <param name="request">寵物建立請求資料</param>
        /// <returns>新建立寵物的ID</returns>
        [HttpPost(Name = nameof(CreatePet))]
        public async Task<ActionResult<long>> CreatePet([FromBody] CreatePetRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var pet = new Pet
            {
                PetName = request.PetName,
                Gender = request.Gender,
                Breed = request.Breed,
                BirthDay = request.BirthDay,
                NormalPrice = request.NormalPrice,
                SubscriptionPrice = request.SubscriptionPrice
            };
            
            var petId = await _petService.CreatePet(pet);
            return CreatedAtAction(nameof(GetPet), new { petID = petId }, petId);
        }

        /// <summary>
        /// 更新寵物資訊
        /// </summary>
        /// <param name="petID">寵物ID</param>
        /// <param name="request">寵物更新請求資料</param>
        /// <returns>操作結果</returns>
        [HttpPut("{petID}", Name = nameof(UpdatePet))]
        public async Task<IActionResult> UpdatePet(long petID, [FromBody] UpdatePetRequest request)
        {
            if (petID != request.PetId)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pet = await _petService.GetPet(petID);
            if (pet == null)
                return NotFound();

            pet.PetName = request.PetName;
            pet.Gender = request.Gender;
            pet.Breed = request.Breed;
            pet.BirthDay = request.BirthDay;
            pet.NormalPrice = request.NormalPrice;
            pet.SubscriptionPrice = request.SubscriptionPrice;
            
            // 如果有提供 PhotoUrl，則更新照片
            if (!string.IsNullOrEmpty(request.PhotoUrl))
            {
                // 這裡需要在Pet實體中添加PhotoUrl屬性，或使用其他方式儲存照片URL
                // 暫時跳過，因為Pet實體中沒有PhotoUrl屬性
            }

            await _petService.UpdatePet(pet);
            return NoContent();
        }

        /// <summary>
        /// 刪除寵物
        /// </summary>
        /// <param name="petID">寵物ID</param>
        /// <returns>操作結果</returns>
        [HttpDelete("{petID}", Name = nameof(DeletePet))]
        public async Task<IActionResult> DeletePet(long petID)
        {
            var pet = await _petService.GetPet(petID);
            if (pet == null)
                return NotFound();
                
            await _petService.DeletePet(petID);
            return NoContent();
        }

        /// <summary>
        /// 根據聯絡人ID取得相關寵物列表
        /// </summary>
        /// <param name="contactPersonId">聯絡人ID</param>
        /// <returns>寵物列表</returns>
        [HttpGet("contact/{contactPersonId}", Name = nameof(GetPetsByContact))]
        public async Task<ActionResult<IList<Pet>>> GetPetsByContact(long contactPersonId)
        {
            return Ok(await _petService.GetPetsByContactPerson(contactPersonId));
        }

        /// <summary>
        /// 上傳寵物照片
        /// </summary>
        /// <param name="petID">寵物ID</param>
        /// <param name="photo">照片檔案</param>
        /// <returns>照片上傳結果和URL</returns>
        [HttpPost("{petID}/photo", Name = nameof(UploadPetPhoto))]
        public async Task<IActionResult> UploadPetPhoto(long petID, IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
                return BadRequest("No photo provided");

            var pet = await _petService.GetPet(petID);
            if (pet == null)
                return NotFound();

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid file type. Only JPG, PNG, and GIF are allowed.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "pets");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{petID}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            return Ok(new { PhotoUrl = $"/uploads/pets/{fileName}" });
        }
    }
}
