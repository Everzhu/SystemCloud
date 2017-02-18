using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.Room
{
    public class Edit
    {
        public Dto.Room.Edit RoomEdit { get; set; } = new Dto.Room.Edit();

        public List<System.Web.Mvc.SelectListItem> BuildList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> RoomTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}