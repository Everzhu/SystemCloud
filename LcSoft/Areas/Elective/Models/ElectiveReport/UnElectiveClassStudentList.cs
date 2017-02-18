using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveReport
{
    public class UnElectiveClassStudentList
    {
        public List<Dto.ElectiveReport.UnElectiveClassStudentList> List { get; set; } = new List<Dto.ElectiveReport.UnElectiveClassStudentList>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public int ClassId { get; set; }= HttpContext.Current.Request["ClassId"].ConvertToInt();

        public int ElectiveId { get; set; } = HttpContext.Current.Request["ElectiveId"].ConvertToInt();
    }
}