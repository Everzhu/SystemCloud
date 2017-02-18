using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyRoomStudent
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }
        public List<Dto.StudyRoomStudent.Import> ImportList { get; set; } = new List<Dto.StudyRoomStudent.Import>();
        public int RoomId { get; set; } = System.Web.HttpContext.Current.Request["RoomId"].ConvertToInt();
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();
        public bool Status { get; set; }
    }
}