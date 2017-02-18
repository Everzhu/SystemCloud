using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Course.Models.Course
{
    public class InfoList
    {
        public List<Dto.Course.InfoList> CourseInfoList { get; set; } = new List<Dto.Course.InfoList>();

        
        public List<SelectListItem> CourseSubjectList { get; set; } = new List<SelectListItem>();

        [Display(Name ="科目")]
        public int? SubjectId { get; set; } = HttpContext.Current.Request["SubjectId"].ConvertToInt();

        
        public List<SelectListItem> CourseGroupList { get; set; } = new List<SelectListItem>();
        [Display(Name = "课程分组")]
        public int? GroupId { get; set; } = HttpContext.Current.Request["GroupId"].ConvertToInt();

        
        public List<SelectListItem> CourseTypeList { get; set; } = new List<SelectListItem>();
        [Display(Name = "课程类型")]
        public int? TypeId { get; set; } = HttpContext.Current.Request["TypeId"].ConvertToInt();

        
        public List<SelectListItem> CourseDomainList { get; set; } = new List<SelectListItem>();
        [Display(Name = "课程领域")]
        public int? DomainId { get; set; } = HttpContext.Current.Request["DomainId"].ConvertToInt();

        

    }
}