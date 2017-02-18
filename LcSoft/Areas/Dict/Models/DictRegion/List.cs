using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Dict.Models.DictRegion
{
    public class List
    {
        public List<Entity.tbDictRegion> RegionList { get; set; } = new List<Entity.tbDictRegion>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}
