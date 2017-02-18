using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Basis.Models.Class
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public bool IsUpdate { get; set; }

        public bool IsRemove { get; set; }

        [Display(Name = "学年")]
        public int? YearId { get; set; }

        public List<Dto.Class.ImportClass> ImportClassList { get; set; } = new List<Dto.Class.ImportClass>();

        public List<Dto.Class.ImportStudent> ImportStudentList { get; set; } = new List<Dto.Class.ImportStudent>();

        public List<Dto.Class.ImportClassType> ImportClassTypeList { get; set; } = new List<Dto.Class.ImportClassType>();

        public List<Dto.Class.ImportGrade> ImportGradeList { get; set; } = new List<Dto.Class.ImportGrade>();

        public List<Dto.Class.ImportTeacher> ImportTeacherList { get; set; } = new List<Dto.Class.ImportTeacher>();

        public List<Dto.Class.ImportRoom> ImportRoomList { get; set; } = new List<Dto.Class.ImportRoom>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public bool Status { get; set; }
    }
}
