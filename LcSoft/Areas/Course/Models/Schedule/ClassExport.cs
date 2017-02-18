using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.Schedule
{
    public class ClassExport
    {
        /// <summary>
        /// 学年
        /// </summary>
        public string year { get; set; } = System.Web.HttpContext.Current.Request["year"];

        /// <summary>
        /// 查询内容
        /// </summary>
        public string searchText { get; set; } = System.Web.HttpContext.Current.Request["searchText"];
    }
}