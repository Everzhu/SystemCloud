using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Course.Models.OrgStudent
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public List<Dto.OrgStudent.Import> ImportList { get; set; } = new List<Dto.OrgStudent.Import>();

        public int? OrgId { get; set; } = 0;

        public bool Status { get; set; }
    }
}
