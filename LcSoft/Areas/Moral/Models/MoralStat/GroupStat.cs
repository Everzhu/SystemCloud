using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralStat
{
    public class GroupStat:ModelBase
    {
        public List<Dto.MoralStat.GroupStat> StatList { get; set; } = new List<Dto.MoralStat.GroupStat>();

        public List<Basis.Dto.ClassGroup.Info> MoralGroupList { get; set; } = new List<Areas.Basis.Dto.ClassGroup.Info>();

    }
}