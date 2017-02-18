using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralGroup
{
    public class Edit
    {

        public Dto.MoralGroup.Edit MoralGroupEdit { get; set; } = new Dto.MoralGroup.Edit();

        public int MoralId { get; set; } = System.Web.HttpContext.Current.Request["MoralId"].ConvertToInt();

    }
}