using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetSalon.Models.EntityModels
{
    /// <summary>
    /// CodeType 部分類別實作 IEntity 介面
    /// </summary>
    public partial class CodeType : IEntity
    {
        /// <summary>
        /// 取得完整代碼類型顯示名稱
        /// </summary>
        public string DisplayName => $"{CodeType1} - {Name}";
    }
}
