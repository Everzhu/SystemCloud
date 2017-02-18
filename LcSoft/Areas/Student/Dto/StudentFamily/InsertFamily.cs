using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.StudentFamily
{
    public class InsertFamily
    {
        public int? Id { get; set; }

        /// <summary>
        /// 家长姓名
        /// </summary>
        [Display(Name = "成员姓名"), Required, RegularExpression(Code.Common.RegUserName, ErrorMessage = "请输入正确的成员姓名，不得包括特殊符号")]
        public string FamilyName { get; set; }

        /// <summary>
        /// 家庭关系
        /// </summary>
        [Display(Name = "家庭关系"), Required]
        public int? KinshipId { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        [Display(Name = "学历")]
        public int? EducationId { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        [Display(Name = "单位名称"), RegularExpression(Code.Common.RegUserName, ErrorMessage = "请输入正确的单位名称，不得包括特殊符号")]
        public string UnitName { get; set; }

        /// <summary>
        /// 岗位职务
        /// </summary>
        [Display(Name = "岗位职务"), RegularExpression(Code.Common.RegUserName, ErrorMessage = "请输入正确的岗位职务，不得包括特殊符号")]
        public string Job { get; set; }

        /// <summary>
        /// 联系手机
        /// </summary>
        [Display(Name = "联系手机"), RegularExpression(Code.Common.RegMobil, ErrorMessage = "手机号格式不正确")]
        public string Mobile { get; set; }
    }
}