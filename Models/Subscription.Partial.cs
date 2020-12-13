namespace PetSalon.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(SubscriptionMetaData))]
    public partial class Subscription
    {
    }
    
    public partial class SubscriptionMetaData
    {
        [Required]
        public int Sid { get; set; }
        [Required]
        public int PetID { get; set; }
        [Required]
        public int Bath { get; set; }
        [Required]
        public int Bueaty { get; set; }
        [Required]
        public System.DateTime SubStartDate { get; set; }
        [Required]
        public System.DateTime SubEndDate { get; set; }
        
        [StringLength(10, ErrorMessage="欄位長度不得大於 10 個字元")]
        [Required]
        public string CreateUser { get; set; }
        
        [StringLength(10, ErrorMessage="欄位長度不得大於 10 個字元")]
        [Required]
        public string CreateTime { get; set; }
        
        [StringLength(10, ErrorMessage="欄位長度不得大於 10 個字元")]
        [Required]
        public string ModifyUser { get; set; }
        
        [StringLength(10, ErrorMessage="欄位長度不得大於 10 個字元")]
        [Required]
        public string ModifyTime { get; set; }
    
        public virtual Pet Pet { get; set; }
    }
}
