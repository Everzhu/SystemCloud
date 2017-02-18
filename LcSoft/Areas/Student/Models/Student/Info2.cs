using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.Student
{
    public class Info2
    {
        public Dto.Student.Info2 StudentInfo { get; set; } = new Dto.Student.Info2();

        public List<Dto.StudentFamily.Info> StudentFamilyInfo { get; set; } = new List<Dto.StudentFamily.Info>();

        public List<Dto.StudentHonor.Info> StudentHonorInfo { get; set; } = new List<Dto.StudentHonor.Info>();

        public int IsPrint { get; set; } = HttpContext.Current.Request["IsPrint"].ConvertToInt();
    }
}