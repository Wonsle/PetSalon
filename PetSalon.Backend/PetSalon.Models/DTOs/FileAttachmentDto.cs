using System;

namespace PetSalon.Models.DTOs
{
    /// <summary>
    /// 檔案附件 DTO
    /// </summary>
    public class FileAttachmentDto
    {
        public long FileId { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string MimeType { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string FileHash { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public long EntityId { get; set; }
        public string AttachmentType { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string CreateUser { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
        public string ModifyUser { get; set; } = string.Empty;
        public DateTime ModifyTime { get; set; }
    }

    /// <summary>
    /// 檔案上傳請求 DTO
    /// </summary>
    public class FileUploadRequest
    {
        public string EntityType { get; set; } = string.Empty;
        public long EntityId { get; set; }
        public string AttachmentType { get; set; } = "Photo";
    }
}
