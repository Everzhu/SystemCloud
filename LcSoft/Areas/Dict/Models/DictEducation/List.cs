using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Models.DictEducation
{
    public class List
    {
        public List<Entity.tbDictEducation> DataList { get; set; } = new List<Entity.tbDictEducation>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}