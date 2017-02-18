using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralPhoto
{
    public class List
    {

        public int MoralDataId { get; set; } = HttpContext.Current.Request["MoralDataId"].ConvertToInt();

        public List<Entity.tbMoralPhoto> MoralPhotoList { get; set; } = new List<Entity.tbMoralPhoto>();

    }
}