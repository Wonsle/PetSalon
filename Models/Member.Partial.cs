namespace PetSalon.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(MemberMetaData))]
    public partial class Member
    {
    }
    
    public partial class MemberMetaData
    {
        [Required]
        public int Sid { get; set; }
        
        [StringLength(10, ErrorMessage="欄位長度不得大於 10 個字元")]
        [Required]
        public string Name { get; set; }
        [Required]
        public string Sex { get; set; }
        
        [StringLength(12, ErrorMessage="欄位長度不得大於 12 個字元")]
        [Required]
        public string Phone { get; set; }
        
        [StringLength(30, ErrorMessage="欄位長度不得大於 30 個字元")]
        public string Mail { get; set; }
        
        [StringLength(30, ErrorMessage="欄位長度不得大於 30 個字元")]
        public string Password { get; set; }
        
        [StringLength(200, ErrorMessage="欄位長度不得大於 200 個字元")]
        public string ContactData { get; set; }
    
        public virtual ICollection<Pet> Pet { get; set; }
    }
}
