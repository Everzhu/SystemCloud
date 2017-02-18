using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Basis.Entity
{
    public class tbSchool : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学校名称
        /// </summary>
        [Required]
        [Display(Name = "学校名称"), RegularExpression(Code.Common.RegUserName, ErrorMessage = "学校名称只允许输入中英文字符、数字和_或-，特殊字符都不能包含")]
        public string SchoolName { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        [Display(Name = "英文名"), RegularExpression(Code.Common.RegEnglishName, ErrorMessage = "英文名只允许输入英文字符、数字")]
        public string SchoolNameEn { get; set; }

        /// <summary>
        /// 学校类型
        /// </summary>
        [Display(Name = "学校类型")]
        public string SchoolType { get; set; }

        /// <summary>
        /// 校长
        /// </summary>
        [Display(Name = "校长"), RegularExpression(Code.Common.RegUserName, ErrorMessage = "校长只允许输入中英文字符、数字和_或-，特殊字符都不能包含")]
        public string SchoolMaster { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Display(Name = "联系电话"), RegularExpression(Code.Common.RegMobil, ErrorMessage = "联系电话格式不正确")]
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Display(Name = "邮箱"), RegularExpression(Code.Common.RegEmail, ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [Display(Name = "邮编")]
        [MaxLength(6), MinLength(6)]
        public string PostCode { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Display(Name = "地址")]
        public string Address { get; set; }

        /// <summary>
        /// 学校简介
        /// </summary>
        [Display(Name = "学校简介")]
        public string Remark { get; set; }
    }
}
