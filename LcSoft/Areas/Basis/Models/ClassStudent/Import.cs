using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Basis.Models.ClassStudent
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public bool IsUpdate { get; set; }

        public bool IsRemove { get; set; }

        /// <summary>
        /// 是否自动创建行政班小组
        /// </summary>
        [Display(Name ="是否自动创建行政班小组")]
        public bool IsAddClassGroup { get; set; }

        public List<Dto.ClassStudent.Import> ImportEdit { get; set; } = new List<Dto.ClassStudent.Import>();

        public bool Status { get; set; }
    }
}
