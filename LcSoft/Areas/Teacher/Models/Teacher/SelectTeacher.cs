using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.Teacher
{
    public class SelectTeacher
    {
        public List<Dto.Teacher.SelectTeacher> TeacherList { get; set; } = new List<Dto.Teacher.SelectTeacher>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int TeacherDeptId { get; set; } = System.Web.HttpContext.Current.Request["TeacherDeptId"].ConvertToInt();
        public string TeacherDeptName { get; set; } = System.Web.HttpContext.Current.Request["TeacherDeptName"].ConvertToString();
    }
}