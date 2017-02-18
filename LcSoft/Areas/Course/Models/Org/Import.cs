using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Course.Models.Org
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public bool IsUpdate { get; set; }

        public bool IsRemove { get; set; }

        public bool IsAdd { get; set; }

        public List<Dto.Org.ImportOrg> ImportOrgList { get; set; } = new List<Dto.Org.ImportOrg>();

        public List<Dto.Org.ImportOrgStudent> ImportOrgStudentList { get; set; } = new List<Dto.Org.ImportOrgStudent>();

        public bool Status { get; set; }
    }
}
