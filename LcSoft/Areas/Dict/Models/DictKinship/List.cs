using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Models.DictKinship
{
    public class List
    {
        public List<Entity.tbDictKinship> KinshipList { get; set; } = new List<Entity.tbDictKinship>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}