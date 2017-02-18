using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Dict.Models.DictNation
{
    public class List
    {
        public List<Entity.tbDictNation> NationList { get; set; } = new List<Entity.tbDictNation>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}
