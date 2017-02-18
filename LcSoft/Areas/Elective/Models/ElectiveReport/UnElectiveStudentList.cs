using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveReport
{
    public class UnElectiveStudentList
    {
        public List<Dto.ElectiveReport.UnElectiveStudentList> List { get; set; } = new List<Dto.ElectiveReport.UnElectiveStudentList>();

        public List<System.Web.Mvc.SelectListItem> ElectiveList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public int? ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();
    }
}