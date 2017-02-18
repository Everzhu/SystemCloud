using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralRedFlag
{
    public class List
    {
        /// <summary>
        /// 本年度第几周
        /// </summary>
        public int WeekNum { get; set; }


        /// <summary>
        /// 本周第一天
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 本周最后一天
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public string tbClassName { get; set; }


        /// <summary>
        /// 录入日期
        /// </summary>
        public DateTime InputDate { get; set; }
    }
}