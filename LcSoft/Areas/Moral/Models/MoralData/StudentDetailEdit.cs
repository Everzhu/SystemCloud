using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralData
{
    public class StudentDetailEdit
    {

        public int MoralItemId { get; set; } = System.Web.HttpContext.Current.Request["ItemId"].ConvertToInt();

        public int MoralDataId { get; set; } = System.Web.HttpContext.Current.Request["Id"].ConvertToInt();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public int StudentGroupId { get; set; } = System.Web.HttpContext.Current.Request["GroupId"].ConvertToInt();

        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public DateTime MoralDate { get; set; } = System.Web.HttpContext.Current.Request["Date"].ConvertToDateTime();

        public bool IsScoreOperate { get; set; }

        public string OperateType { get; set; } = HttpContext.Current.Request["Op"].ConvertToString();

        public Dto.MoralData.StudentDetailEdit Edit { get; set; } = new Dto.MoralData.StudentDetailEdit();

        public List<SelectListItem> MoralExpress { get; set; } = typeof(Code.EnumHelper.MoralExpress).ToItemList();

        public List<SelectListItem> MoralDataReasonList { get; set; } = new List<SelectListItem>();
        
        

    }
}