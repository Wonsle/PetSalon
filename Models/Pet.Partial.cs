namespace PetSalon.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    
    [MetadataType(typeof(PetMetaData))]
    public partial class Pet
    {
    }
    
    public partial class PetMetaData
    {
        [Required]
        public int Sid { get; set; }
        
        [StringLength(10, ErrorMessage="欄位長度不得大於 10 個字元")]
        [Required]
        [DisplayName("姓名")]
        public string Name { get; set; }
        [Required]
        [DisplayName("性別")]
        public short Sex { get; set; }
        [Required]
        [DisplayName("種類")]
        public short Breed { get; set; }
        
        [StringLength(200, ErrorMessage="欄位長度不得大於 200 個字元")]
        [DisplayName("照片連結")]
        public string PicUrl { get; set; }
        
        [StringLength(20, ErrorMessage="欄位長度不得大於 20 個字元")]
        [DisplayName("照片檔名")]
        public string PicFileName { get; set; }
        [Required]
        [DisplayName("聯絡人")]
        public int ContactPersonID { get; set; }
        
        [StringLength(500, ErrorMessage="欄位長度不得大於 500 個字元")]
        [DisplayName("註記")]
        public string Memo { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual ICollection<Reserver> Reserver { get; set; }
        public virtual ICollection<Subscription> Subscription { get; set; }
    }
}
