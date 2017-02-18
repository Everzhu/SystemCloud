using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralOption
{
    public class Edit
    {
        public int MoralItemId { get; set; } = System.Web.HttpContext.Current.Request["MoralItemId"].ConvertToInt();

        public Dto.MoralOption.Edit MoralOptionEdit { get; set; } = new Dto.MoralOption.Edit();

    }
}