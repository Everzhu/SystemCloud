using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Models.Office
{
    public class OfficeEditModel
    {
        public Dto.Office.OfficeEditDto OfficeEditDto { get; set; } = new Dto.Office.OfficeEditDto();

        public List<System.Web.Mvc.SelectListItem> DepartList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string ErrorMsg { get; set; }
    }
}