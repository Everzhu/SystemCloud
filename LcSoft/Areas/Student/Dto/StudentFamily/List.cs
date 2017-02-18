using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Student.Dto.StudentFamily
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 家长姓名
        /// </summary>
        [Display(Name = "成员姓名")]
        public string FamilyName { get; set; }

        /// <summary>
        /// 家庭关系
        /// </summary>
        [Display(Name = "家庭关系")]
        public string KinshipName { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        [Display(Name = "学历")]
        public string EducationName { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        [Display(Name = "单位名称")]
        public string UnitName { get; set; }

        /// <summary>
        /// 岗位职务
        /// </summary>
        [Display(Name = "岗位职务")]
        public string Job { get; set; }

        /// <summary>
        /// 联系手机
        /// </summary>
        [Display(Name = "联系手机")]
        public string Mobile { get; set; }
    }
}
