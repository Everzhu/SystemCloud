using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Dict.Models.DictParty
{
    public class List
    {
        public List<Entity.tbDictParty> PartyList { get; set; } = new List<Entity.tbDictParty>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}
