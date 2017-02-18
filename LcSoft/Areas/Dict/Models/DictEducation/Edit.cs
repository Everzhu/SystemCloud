using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Models.DictEducation
{
    public class Edit
    {
        public Entity.tbDictEducation DataEdit { get; set; } = new Entity.tbDictEducation();

        public List<System.Web.Mvc.SelectListItem> DictDegreeList
        {
            get;
            set;
        } = XkSystem.Areas.Dict.Controllers.DictDegreeController.SelectList();
    }
}