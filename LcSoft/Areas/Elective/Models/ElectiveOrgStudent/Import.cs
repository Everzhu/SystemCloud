using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveOrgStudent
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        
        public bool IsUpdate { get; set; }
        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public List<Dto.ElectiveOrgStudent.Import> ImportList { get; set; } = new List<Dto.ElectiveOrgStudent.Import>();

        public bool Status { get; set; }
    }
}