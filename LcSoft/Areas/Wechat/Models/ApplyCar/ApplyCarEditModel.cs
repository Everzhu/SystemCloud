using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Models.ApplyCar
{
    public class ApplyCarEditModel
    {
        public Dto.ApplyCar.ApplyCarEditDto ApplyCarEditDto { get; set; } = new Dto.ApplyCar.ApplyCarEditDto();

        public string DepartListJson { get; set; }

        public string ErrorMsg { get; set; }
    }
}