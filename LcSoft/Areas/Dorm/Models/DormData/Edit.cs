using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Models.DormData
{
    public class Edit
    {
        public Dto.DormData.Edit DormDataEdit { get; set; } = new Dto.DormData.Edit();

        public List<System.Web.Mvc.SelectListItem> DormOptionList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}