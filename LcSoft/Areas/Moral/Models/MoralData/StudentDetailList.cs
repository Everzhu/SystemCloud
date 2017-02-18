using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralData
{
    public class StudentDetailList
    {
        public int MoralItemId { get; set; } = System.Web.HttpContext.Current.Request["ItemId"].ConvertToInt();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public int StudentGroupId { get; set; } = System.Web.HttpContext.Current.Request["GroupId"].ConvertToInt();

        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public DateTime MoralDate { get; set; } = System.Web.HttpContext.Current.Request["Date"].ConvertToDateTime();

        public List<Dto.MoralData.StudentDetailList> DataList { get; set; } = new List<Dto.MoralData.StudentDetailList>();

        /// <summary>
        /// 操作方式是否打分
        /// </summary>
        public bool IsScoreOperate { get; set; }

        public string OperateType { get; set; } = HttpContext.Current.Request["Op"].ConvertToString();

        public bool IsPower { get; set; } = false;
    }

    
}