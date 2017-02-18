using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassManager
{
    public class EditClass
    {
        public Dto.ClassManager.EditClass DataEdit { get; set; } = new Dto.ClassManager.EditClass();

        public int TeacherId { get; set; }

        public List<Dto.ClassManager.EditClassList> ClassList { get; set; } = new List<Dto.ClassManager.EditClassList>();

        //public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();  
    }
}