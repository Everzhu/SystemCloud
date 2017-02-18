using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveInput
{
    public class History
    {
        public List<Dto.ElectiveInput.List> ElectiveHistoryList { get; set; } = new List<Dto.ElectiveInput.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}