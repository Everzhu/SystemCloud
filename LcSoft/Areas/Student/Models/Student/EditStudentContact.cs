using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.Student
{
    public class EditStudentContact
    {
        public Dto.Student.EditStudentContact StudentContact { get; set; } = new Dto.Student.EditStudentContact();

        public string Step { get; set; } = System.Web.HttpContext.Current.Request["Step"].ConvertToString();
    }
}