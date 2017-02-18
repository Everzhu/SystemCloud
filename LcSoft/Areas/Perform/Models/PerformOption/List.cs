using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformOption
{
    public class List
    {
        public List<Dto.PerformOption.List> OptionList { get; set; } = new List<Dto.PerformOption.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int PerformItemId { get; set; }
    }
}