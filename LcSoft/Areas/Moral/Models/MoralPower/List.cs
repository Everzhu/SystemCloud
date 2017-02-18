using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralPower
{
    public class List
    {

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public List<Dto.MoralPower.List> MoralPowerList { get; set; } = new List<Dto.MoralPower.List>();

        public int MoralItemId { get; set; } = System.Web.HttpContext.Current.Request["MoralItemId"].ConvertToInt();

        public int MoralId { get; set; } = System.Web.HttpContext.Current.Request["MoralId"].ConvertToInt();
        
    }
}