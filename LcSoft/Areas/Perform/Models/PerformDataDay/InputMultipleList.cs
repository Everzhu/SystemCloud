﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformDataDay
{
    public class InputMultipleList
    {
        public List<Dto.PerformDataDay.InputMultipleList> PerformInputMultipleList { get; set; } = new List<Dto.PerformDataDay.InputMultipleList>();
        public List<System.Web.Mvc.SelectListItem> PerformOptionList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public int? PerformId { get; set; } = System.Web.HttpContext.Current.Request["PerformId"].ConvertToInt();
        public int? StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();
        public int? PerformItemId { get; set; } = System.Web.HttpContext.Current.Request["PerformItemId"].ConvertToInt();
        public int? PerformCourseId { get; set; } = System.Web.HttpContext.Current.Request["PerformCourseId"].ConvertToInt();
    }
}