using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Dict.Models.Sex
{
    public class List
    {
        public List<Entity.tbDictSex> SexList { get; set; } = new List<Entity.tbDictSex>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}
