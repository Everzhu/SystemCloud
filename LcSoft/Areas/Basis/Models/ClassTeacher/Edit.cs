using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassTeacher
{
    public class Edit
    {
        public Dto.ClassTeacher.Edit ClassTeacherEdit { get; set; } = new Dto.ClassTeacher.Edit();

        public int? ClassId { get; set; } = 0;
    }
}