using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Models.DictMarriage
{
    public class List
    {
        public List<Entity.tbDictMarriage> DataList { get; set; } = new List<Entity.tbDictMarriage>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}