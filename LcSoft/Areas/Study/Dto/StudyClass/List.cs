using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyClass
{
    public class List
    {
        public int Id { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }
        /// <summary>
        /// 班级Id
        /// </summary>
        [Display(Name = "班级Id")]
        public int ClassId { get; set; }
        /// <summary>
        /// 晚自习Id
        /// </summary>
        [Display(Name = "晚自习Id")]
        public int StudyId { get; set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        [Display(Name = "班级名称")]
        public string ClassName { get; set; }
        /// <summary>
        ///  年级
        /// </summary>
        [Display(Name = "年级")]
        public string GradeName { get; set; }
        /// <summary>
        /// 班级类型
        /// </summary>
        [Display(Name = "班级类型")]
        public string ClassTypeName { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        [Display(Name = "是否选中")]
        public bool IsChecked { get; set; }
        /// <summary>
        /// 教室Id
        /// </summary>
        [Display(Name = "教室Id")]
        public int RoomId { get; set; }
        /// <summary>
        /// 教室名称
        /// </summary>
        [Display(Name = "教室名称")]
        public string RoomName { get; set; }
    }
}