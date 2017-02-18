using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralComment
{
    public class MoralHappeningEdit
    {
        public Dto.MoralComment.MoralHappeningEdit MHappeningEdit { get; set; } = new Dto.MoralComment.MoralHappeningEdit();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public int HappeningId { get; set; } = System.Web.HttpContext.Current.Request["HappeningId"].ConvertToInt();

        public string InputDate { get; set; } = System.Web.HttpContext.Current.Request["InputDate"].ToString();

        public List<Dto.MoralComment.MoralHappeningEdit> MHappeningEditList { get; set; } = new List<Dto.MoralComment.MoralHappeningEdit>();

        public string StudentCode = string.Empty;

        public string StudentName = string.Empty;
    }
}