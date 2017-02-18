using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyData
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        [Display(Name = "对于系统中已存在的数据做更新操作")]
        public bool IsUpdate { get; set; }

        public List<Dto.StudyData.Import> ImportList { get; set; } = new List<Dto.StudyData.Import>();
        public List<System.Web.Mvc.SelectListItem> StudyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> RoomOrClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        [Display(Name = "班级教室")]
        public int RoomOrClassId { get; set; } = System.Web.HttpContext.Current.Request["RoomOrClassId"].ConvertToInt();
        [Display(Name = "自习日期")]
        public string DateSearch { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearch"]);
        [Display(Name = "晚自习")]
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();
        public bool Status { get; set; }
    }
}