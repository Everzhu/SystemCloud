using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralStat
{
    public class GroupStat:DtoBase
    {
        public int GroupId { get; set; }

        [Display(Name ="小组")]
        public string GroupName { get; set; }

        public int ClassId { get; set; }

        [Display(Name ="班级")]
        public string ClassName { get; set; }
    }
}