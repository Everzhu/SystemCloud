using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.Moral
{
    public class Edit
    {

        public Dto.Moral.Edit MoralEdit { get; set; } = new Dto.Moral.Edit();

        public List<SelectListItem> YearList { get; set; } = new List<SelectListItem>();

        public string CreateWay { get; set; } = "全新创建";

        public int CopyMoralId { get; set; }

        public List<SelectListItem> MoralList { get; set; } = new List<SelectListItem>();

    }
}