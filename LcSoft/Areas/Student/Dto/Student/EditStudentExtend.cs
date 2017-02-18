using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.Student
{
    public class EditStudentExtend
    {
        public int Id { get; set; }

        /// <summary>
        /// 血型
        /// </summary>
        [Display(Name = "血型"), Required]
        public int? BloodTypeId { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        [Display(Name = "政治面貌"), Required]
        public int? DictPartyId { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [Display(Name = "民族"), Required]
        public int? DictNationId { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Display(Name = "出生日期"), Required]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 所在班级
        /// </summary>
        [Display(Name = "所在班级"), Required]
        public int? ClassId { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        [Display(Name = "籍贯"), Required]
        public string NativePlace { get; set; }

        /// <summary>
        /// 出生地
        /// </summary>
        [Display(Name = "出生地"), Required]
        public string BirthPlace { get; set; }

        /// <summary>
        /// 户籍所在地
        /// </summary>
        [Display(Name = "户籍所在地"), Required]
        public string HouseholdRegister { get; set; }

        /// <summary>
        /// 是否独生子女
        /// </summary>
        [Display(Name = "是否独生子女"), Required]
        public bool IsOnlyChild { get; set; }

        /// <summary>
        /// 上下学交通工具
        /// </summary>
        [Display(Name = "上下学交通工具"), Required]
        public Code.EnumHelper.SchoolTransportationType SchoolTransportationType { get; set; }

        /// <summary>
        /// 上下学距离上下学距离（米）
        /// </summary>
        [Display(Name = "上下学距离上下学距离（米）"), Required]
        [RegularExpression(Code.Common.RegIntAndDecimal, ErrorMessage = "请输入数字")]
        public decimal SchoolDistance { get; set; }

        /// <summary>
        /// 是否进城务工人员随迁子女
        /// </summary>
        [Display(Name = "是否进城务工人员随迁子女"), Required]
        public bool IsSuiqian { get; set; }
    }
}