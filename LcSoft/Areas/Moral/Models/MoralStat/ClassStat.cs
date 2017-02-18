using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralStat
{
    public class ClassStat:ModelBase
    {

        public new List<Dto.MoralClass.Info> MoralClassList { get; set; } = new List<Dto.MoralClass.Info>();

        public List<Dto.MoralStat.ClassStat> StatList { get; set; } = new List<Dto.MoralStat.ClassStat>();

        public List<Dto.MoralStat.ClassStat> StudentStatList { get; set; } = new List<Dto.MoralStat.ClassStat>();
        
    }
}