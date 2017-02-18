using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.StudentHonor
{
    public class ReViewApplyHonorEdit
    {
        public int Id { get; set; }

        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsReViewYes { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        [Display(Name = "审批意见")]
        public string CheckRemark { get; set; }
    }
}