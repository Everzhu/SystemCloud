using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.OrgTeacher
{
    public class Edit
    {
        public Dto.OrgTeacher.Edit OrgTeacherEdit { get; set; } = new Dto.OrgTeacher.Edit();

        public int? OrgId { get; set; } = 0;
    }
}