using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralStat
{
    public class StudentStat:ModelBase
    {   
        public List<Dto.MoralStat.StudentStat> StatList { get; set; } = new List<Dto.MoralStat.StudentStat>();

        public List<Student.Dto.Student.Info> MoralStudentList { get; set; } = new List<Student.Dto.Student.Info>();

    }
}