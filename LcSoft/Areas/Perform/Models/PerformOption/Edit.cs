using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformOption
{
    public class Edit
    {
        public Dto.PerformOption.Edit OptionEdit { get; set; } = new Dto.PerformOption.Edit();
        public string PerformItemId { get; set; }
    }
}