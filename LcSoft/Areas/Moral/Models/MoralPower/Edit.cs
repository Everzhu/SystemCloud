using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralPower
{
    public class Edit
    {

        public Dto.MoralPower.Edit MoralPowerEdit { get; set; } = new Dto.MoralPower.Edit();

        public List<SelectListItem> TeacherList { get; set; } = new List<SelectListItem>();

        public List<Dto.MoralClass.Info> MoralClassList { get; set; } = new List<Dto.MoralClass.Info>();

        public int MoralItemId { get; set; } = HttpContext.Current.Request["MoralItemId"].ConvertToInt();

        public int MoralId { get; set; } = HttpContext.Current.Request["MoralId"].ConvertToInt();

        [Display(Name ="评价班级"),Required]
        [MinLength(1,ErrorMessage ="请选择评价班级")]
        public string MoralClassId { get; set; }

        public List<int> MoralClassIds { get; set; } = new List<int>();


    }
}