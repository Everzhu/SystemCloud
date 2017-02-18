using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyApply
{
    public class CheckList
    {
        public int Id { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }
        /// <summary>
        /// 晚自习名称
        /// </summary>
        [Display(Name = "晚自习名称")]
        public string StudyName { get; set; }
        /// <summary>
        /// 晚自习Id
        /// </summary>
        [Display(Name = "晚自习Id")]
        public int StudyId { get; set; }
        /// <summary>
        /// 学年学期学段
        /// </summary>
        [Display(Name = "学年学期学段")]
        public string YearName { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "申请开始时间")]
        public DateTime ApplyFrom { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "申请结束时间")]
        public DateTime ApplyTo { get; set; }
        /// <summary>
        /// 晚自习模式（班级、教室）
        /// </summary>
        [Display(Name = "晚自习模式")]
        public bool IsRoom { get; set; }
        /// <summary>
        /// 开放申请
        /// </summary>
        [Display(Name = "开放申请")]
        public bool IsApply { get; set; }
        /// <summary>
        /// 申请备注
        /// </summary>
        [Display(Name = "申请备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 审批状态，0未审批，-1审批通过，1审批不通过
        /// </summary>
        [Display(Name = "审批状态")]
        public string CheckStatusName
        {
            get
            {
                return CheckStatus.GetDescription();
            }
        }
        /// <summary>
        /// 审批状态
        /// </summary>
        [Display(Name = "审批状态")]
        public XkSystem.Code.EnumHelper.CheckStatus CheckStatus { get; set; }
        /// <summary>
        /// 审批日期
        /// </summary>
        [Display(Name = "审批日期")]
        public DateTime? CheckDate { get; set; }
        /// <summary>
        /// 审批人员
        /// </summary>
        [Display(Name = "审批人员")]
        public string CheckUserName { get; set; }
        /// <summary>
        /// 审批说明
        /// </summary>
        [Display(Name = "审批说明")]
        public string CheckRemark { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }
        /// <summary>
        /// 学号
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string SexName { get; set; }
    }
}