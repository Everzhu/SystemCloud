using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Elective.Entity
{
    /// <summary>
    /// 选课开班
    /// </summary>
    public class tbElectiveOrg : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 班级名称
        /// </summary>
        [Required]
        [Display(Name = "班级名称")]
        public string OrgName { get; set; }

        /// <summary>
        /// 所属选课
        /// </summary>
        [Required]
        [Display(Name = "所属选课")]
        public virtual tbElective tbElective { get; set; }

        /// <summary>
        /// 选课分段
        /// </summary>
        [Display(Name = "选课分段")]
        public virtual tbElectiveSection tbElectiveSection { get; set; }

        /// <summary>
        /// 选课分组
        /// </summary>
        [Display(Name = "选课分组")]
        public virtual tbElectiveGroup tbElectiveGroup { get; set; }

        /// <summary>
        /// 所属课程
        /// </summary>
        [Required]
        [Display(Name = "所属课程")]
        public virtual Course.Entity.tbCourse tbCourse { get; set; }

        /// <summary>
        /// 任课教师
        /// </summary>
        [Display(Name = "任课教师")]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        [Display(Name = "教室")]
        public virtual Basis.Entity.tbRoom tbRoom { get; set; }

        /// <summary>
        /// 总名额
        /// </summary>
        [Required]
        [Display(Name = "总名额")]
        public int MaxCount { get; set; }

        /// <summary>
        /// 剩余名额
        /// </summary>
        [Required]
        [Display(Name = "剩余名额")]
        public int RemainCount { get; set; }

        /// <summary>
        /// 授权状态(默认不处理，-1为开启黑名单模式，1为开启白名单模式)
        /// </summary>
        [Required]
        [Display(Name = "授权状态")]
        public int Permit { get; set; }

        /// <summary>
        /// 是否限制班级
        /// </summary>
        [Required]
        [Display(Name = "是否限制班级")]
        public bool IsPermitClass { get; set; }

        /// <summary>
        /// 并发控制
        /// </summary>
        [Timestamp]
        [Display(Name = "并发控制")]
        public byte[] RowVersion { get; set; }
    }
}
