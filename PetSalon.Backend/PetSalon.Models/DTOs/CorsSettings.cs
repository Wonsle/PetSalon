namespace PetSalon.Models.DTOs
{
    /// <summary>
    /// CORS 設定模型
    /// </summary>
    public class CorsSettings
    {
        /// <summary>
        /// 允許的來源清單
        /// </summary>
        public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 允許的 HTTP 方法
        /// </summary>
        public string[] AllowedMethods { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 允許的 HTTP 標頭
        /// </summary>
        public string[] AllowedHeaders { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 是否允許認證 (Credentials)
        /// </summary>
        public bool AllowCredentials { get; set; } = true;

        /// <summary>
        /// Preflight 請求快取時間（秒）
        /// </summary>
        public int PreflightMaxAge { get; set; } = 600;
    }
}
