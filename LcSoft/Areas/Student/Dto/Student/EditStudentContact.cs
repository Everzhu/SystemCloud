using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.Student
{
    public class EditStudentContact
    {
        public int Id { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [Display(Name = "手机号码"), Required, RegularExpression(Code.Common.RegMobil, ErrorMessage = "手机号码格式不正确")]
        public string Mobile { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Display(Name = "Email"), RegularExpression(Code.Common.RegEmail, ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        [Display(Name = "QQ"), RegularExpression(Code.Common.RegQQNumber, ErrorMessage = "QQ号格式不正确")]
        public string Qq { get; set; }

        /// <summary>
        /// 家庭住址
        /// </summary>
        [Display(Name = "家庭住址"), Required]
        public string Address { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [Display(Name = "邮政编码"), RegularExpression(Code.Common.RegPostalCode, ErrorMessage = "邮政编码格式不正确")]
        public string PostCode { get; set; }
    }
}