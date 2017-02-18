using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassStudent
{
    public class ChangeClass
    {
        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name = "班级")]
        public int ClassId { get; set; }

        /// <summary>
        /// 座位号
        /// </summary>
        [Display(Name = "座位号")]
        public int? No { get; set; }

        public int Id { get; set; }
    }
}