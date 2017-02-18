using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Models.PaperSize
{
    public class List
    {
        public List<Entity.tbDictPaperSize> PaperSizeList { get; set; } = new List<Entity.tbDictPaperSize>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}