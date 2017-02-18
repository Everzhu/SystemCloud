using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Elective.Dto.ElectiveOrg
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 所属选课
        /// </summary>
        [Display(Name = "所属选课"), Required]
        public int ElectiveId { get; set; }

        /// <summary>
        /// 所属课程
        /// </summary>
        [Display(Name = "所属课程"), Required]
        public int CourseId { get; set; }


        [Display(Name ="科目"),Required]
        public int SubjectId { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        [Display(Name = "班级名称"), Required]
        public string OrgName { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        [Display(Name = "教室")]
        public int? RoomId { get; set; }

        /// <summary>
        /// 任课教师
        /// </summary>
        [Display(Name = "任课教师")]
        public int? TeacherId { get; set; }

        /// <summary>
        /// 总名额
        /// </summary>
        [Display(Name = "总名额"), Required, Range(0, 10000)]
        public int MaxCount { get; set; }

        /// <summary>
        /// 授权状态
        /// </summary>
        [Display(Name = "授权状态"), Required]
        public int Permit { get; set; }

        /// <summary>
        /// 限制班级
        /// </summary>
        [Display(Name = "限制班级"), Required]
        public bool IsPermitClass { get; set; }

        /// <summary>
        /// 选课分段
        /// </summary>
        [Display(Name = "选课分段"), Required]
        public int ElectiveSectionId { get; set; }

        /// <summary>
        /// 选课分组
        /// </summary>
        [Display(Name = "选课分组"), Required]
        public int ElectiveGroupId { get; set; }
    }
}
