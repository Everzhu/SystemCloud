using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformData
{
    public class PerformDataStudentDetail
    {
        public Dto.PerformData.StudentDetail PerformDataStudent { get; set; } = new Dto.PerformData.StudentDetail();
        public List<Dto.PerformData.StudentDetail> PerformDataStudentDetailList { get; set; } = new List<Dto.PerformData.StudentDetail>();
        public List<Dto.PerformData.List> PerformDataList { get; set; } = new List<Dto.PerformData.List>();
        public List<Dto.Perform.List> PerformList { get; set; } = new List<Dto.Perform.List>();
        public List<Dto.PerformItem.List> PerformItemList { get; set; } = new List<Dto.PerformItem.List>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int PerformId { get; set; } = System.Web.HttpContext.Current.Request["PerformId"].ConvertToInt();
        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();
        public int SubjectId { get; set; } = System.Web.HttpContext.Current.Request["SubjectId"].ConvertToInt();
        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();
    }
}