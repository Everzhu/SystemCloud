using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Admin.Models.Program
{
    public class List
    {
        public List<Entity.tbProgram> ProgramList { get; set; } = new List<Entity.tbProgram>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}