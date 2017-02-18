using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static XkSystem.Code.PageHelper;

namespace XkSystem.Areas.Elective.Models.ElectiveReport
{
    public class List
    {
        public List<Dto.ElectiveReport.List> ElectiveReportList { get; set; } = new List<Dto.ElectiveReport.List>();

        public List<Dto.ElectiveReport.UnElectiveList> UnElectiveList { get; set; } = new List<Dto.ElectiveReport.UnElectiveList>();

        public List<System.Web.Mvc.SelectListItem> ElectiveList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> OrgList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Dto.ElectiveOrg.List> ElectiveOrgList { get; set; } = new List<Dto.ElectiveOrg.List>();

        //public List<Dto.ElectiveSection.List> ElectiveSectionList { get; set; } = new List<Dto.ElectiveSection.List>();

        public List<Areas.Basis.Dto.ClassStudent.List> ElectiveStudentList { get; set; } = new List<Areas.Basis.Dto.ClassStudent.List>();

        public bool IsWeekPeriod { get; set; }

        public bool IsHiddenSection { get; set; }

        public bool IsHiddenGroup { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public int? OrgId { get; set; } = System.Web.HttpContext.Current.Request["OrgId"].ConvertToInt();

        public int UeType { get; set; } = System.Web.HttpContext.Current.Request["UeType"].ConvertToInt();
        
        public XkSystem.Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}