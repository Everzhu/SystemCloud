using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Dict.Models.Blood
{
    public class List
    {
        public List<Entity.tbDictBlood> tbDictBloodList { get; set; } = new List<Entity.tbDictBlood>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}
