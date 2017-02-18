using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveInput
{
    public class List
    {
        public List<Dto.ElectiveInput.List> ElectiveInputList { get; set; } = new List<Dto.ElectiveInput.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}