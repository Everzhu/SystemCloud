using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Models.DictMajor
{
    public class List
    {
        public List<Entity.tbDictMajor> DataList { get; set; } = new List<Entity.tbDictMajor>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}