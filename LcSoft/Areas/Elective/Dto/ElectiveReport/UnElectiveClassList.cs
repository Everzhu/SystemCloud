using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveReport
{
    public class UnElectiveClassList
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        [Display(Name ="所属选课班级")]
        public string ElectiveOrgName { get; set; }
        /// <summary>
        /// 行政班
        /// </summary>
        [Display(Name = "行政班")]
        public string ClassName { get; set; }

        /// <summary>
        /// 班主任
        /// </summary>
        [Display(Name = "班主任")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 班级人数
        /// </summary>
        [Display(Name = "班级人数")]
        public int Num { get; set; }

        /// <summary>
        /// 未选人数
        /// </summary>
        [Display(Name = "未选人数")]
        public int UnNum { get; set; }
    }
}