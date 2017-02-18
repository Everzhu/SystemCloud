using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveOrg
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public List<Dto.ElectiveOrg.Import> ImportList { get; set; } = new List<Dto.ElectiveOrg.Import>();

        public bool IsUpdate { get; set; }

        public bool Status { get; set; }
    }
}