using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralStat
{
    public class List
    {
        public int MoralId { get; set; } = HttpContext.Current.Request["MoralId"].ConvertToInt();

        public int? ClassId { get; set; } =HttpContext.Current.Request["ClassId"].ConvertToInt(); 

        public List<Dto.MoralStat.List> StatList { get; set; } = new List<Dto.MoralStat.List>();

        public List<SelectListItem> MoralList { get; set; } = new List<SelectListItem>();
        
        public List<Student.Dto.Student.Info> MoralStudentList { get; set; } = new List<Student.Dto.Student.Info>();

        public List<SelectListItem> MoralClassList { get; internal set; } = new List<SelectListItem>();
        
        public List<Dto.MoralItem.Info> MoralItemList { get; internal set; }

        public bool DataIsNull { get; set; }
    }
}