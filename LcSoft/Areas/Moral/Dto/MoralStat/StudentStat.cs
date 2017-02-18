using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralStat
{
    public class StudentStat:DtoBase
    {
        public int StudentId { get; set; }

        [Display(Name ="学生")]
        public string StudentName { get; set; }

        public int ClassId { get; set; }

        [Display(Name ="班级")]
        public string ClassName { get; set; }
    }
}