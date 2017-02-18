using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Models.DictDegree
{
    public class List
    {
        public List<Entity.tbDictDegree> DataList { get; set; } = new List<Entity.tbDictDegree>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}