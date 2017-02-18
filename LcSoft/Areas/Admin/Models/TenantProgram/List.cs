using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Admin.Models.TeanntProgram
{
    public class List
    {
        public List<Dto.TeanntProgram.List> TeanntProgramList { get; set; } = new List<Dto.TeanntProgram.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}