using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveReport
{
    public class UnElectiveList
    {

        public int OrgId { get; set; }
        public int ElectiveId { get; set; }

        
        public string OrgName { get; set; }
        public bool IsPermitClass { get; set; }
        public string TeacherName { get; set; }


        public int ClassId { get; set; }
        public string ClassName { get; set; }

        public string HeadTeacherName { get; set; }
        public int StudentNum { get; set; }
        public int ExistsStudentNum { get; set; }

    }
}