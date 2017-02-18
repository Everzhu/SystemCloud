using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralRedFlag
{
    public class StatCourseModel
    {
        public int ClassId { get; set; }
        public int MoralItemId { get; set; }
        public decimal DefaultValue { get; set; }
        public decimal AddScore { get; set; }
        public decimal SubScore { get; set; }
        public decimal Score { get; set; }
    }
}