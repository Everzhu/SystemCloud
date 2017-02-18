using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Models.DormApply
{
    public class ProvidedDorm
    {
        public Dto.DormApply.ProvidedDorm DormApplyProvidedDorm { get; set; } = new Dto.DormApply.ProvidedDorm();

        public List<System.Web.Mvc.SelectListItem> RoomList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> BuildList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}