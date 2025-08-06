namespace PetSalon.Models.DTOs
{
    public class FileUploadSettings
    {
        public string BaseUploadPath { get; set; } = "wwwroot/uploads";
        public string[] AllowedExtensions { get; set; } = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        public int MaxFileSizeInMB { get; set; } = 10;
    }
}