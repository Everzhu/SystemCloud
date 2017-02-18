using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Elective.Dto.ElectiveData
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 选课学生
        /// </summary>
        [Display(Name = "选课学生")]
        public string StudentName { get; set; }

        /// <summary>
        /// 选课班级
        /// </summary>
        [Display(Name = "选课班级")]
        public string ElectiveOrgName { get; set; }

        /// <summary>
        /// 是否预选
        /// </summary>
        [Display(Name = "是否预选")]
        public bool IsPreElective { get; set; }

        /// <summary>
        /// 是否不可调整
        /// </summary>
        [Display(Name = "是否不可调整")]
        public bool IsFixed { get; set; }

        /// <summary>
        /// 对应的tbElectiveOrg.Id
        /// </summary>
        public int ElectiveOrgId { get; set; }
    }
}
