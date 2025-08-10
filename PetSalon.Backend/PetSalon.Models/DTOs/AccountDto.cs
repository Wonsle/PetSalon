namespace PetSalon.Models.DTOs
{
    /// <summary>
    /// 登入請求 DTO
    /// </summary>
    public class Logon
    {
        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        
        /// <summary>
        /// 密碼
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// 登入回應 DTO
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// JWT Token
        /// </summary>
        public string Token { get; set; } = string.Empty;
        
        /// <summary>
        /// 使用者資訊
        /// </summary>
        public UserInfo User { get; set; } = new();
        
        /// <summary>
        /// Token 過期時間（秒）
        /// </summary>
        public int ExpiresIn { get; set; }
    }

    /// <summary>
    /// 使用者資訊 DTO
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 使用者 ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        
        /// <summary>
        /// 顯示名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 角色列表
        /// </summary>
        public string[] Roles { get; set; } = Array.Empty<string>();
        
        /// <summary>
        /// 最後登入時間
        /// </summary>
        public DateTime LastLogin { get; set; }
    }
}