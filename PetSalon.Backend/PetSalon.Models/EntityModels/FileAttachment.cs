using System;
using System.Collections.Generic;
using PetSalon.Models;

namespace PetSalon.Models.EntityModels
{
    /// <summary>
    /// 檔案附件 - 統一管理檔案上傳、儲存和關聯
    /// </summary>
    public partial class FileAttachment : IEntity
    {
        public long FileId { get; set; }

        /// <summary>
        /// 原始檔案名稱
        /// </summary>
        public string OriginalFileName { get; set; } = string.Empty;

        /// <summary>
        /// 儲存的檔案名稱（含GUID）
        /// </summary>
        public string StoredFileName { get; set; } = string.Empty;

        /// <summary>
        /// 檔案相對路徑
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 檔案大小（bytes）
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// MIME類型
        /// </summary>
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// 副檔名
        /// </summary>
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// 檔案SHA256 Hash值（用於檢測重複檔案）
        /// </summary>
        public string FileHash { get; set; } = string.Empty;

        /// <summary>
        /// 實體類型（Pet, ContactPerson, Service等）
        /// </summary>
        public string EntityType { get; set; } = string.Empty;

        /// <summary>
        /// 實體ID
        /// </summary>
        public long EntityId { get; set; }

        /// <summary>
        /// 附件類型（Avatar, Photo, Document等）
        /// </summary>
        public string AttachmentType { get; set; } = string.Empty;

        /// <summary>
        /// 顯示順序
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsActive { get; set; }

        public string CreateUser { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
        public string ModifyUser { get; set; } = string.Empty;
        public DateTime ModifyTime { get; set; }
    }
}
