using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveData
{
    public class List
    {
        public List<Dto.ElectiveData.List> ElectiveDataList { get; set; } = new List<Dto.ElectiveData.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}
