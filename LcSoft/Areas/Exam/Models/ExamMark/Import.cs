using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamMark
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        [Display(Name = "自动生成排名")]
        public bool IsAutoRank { get; set; }

        [Display(Name = "对于系统中已存在的数据做更新操作")]
        public bool IsUpdate { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        [Display(Name = "唯一标识")]
        public int NameTypeId { get; set; }

        public List<System.Web.Mvc.SelectListItem> NameTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public bool IsRemove { get; set; }

        public List<System.Web.Mvc.SelectListItem> ExamList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> OrgList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> CourseList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Dto.ExamMark.Import> ImportList { get; set; } = new List<Dto.ExamMark.Import>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int CourseId { get; set; } = System.Web.HttpContext.Current.Request["CourseId"].ConvertToInt();

        public int OrgId { get; set; } = System.Web.HttpContext.Current.Request["OrgId"].ConvertToInt();

        public int ExamId { get; set; } = System.Web.HttpContext.Current.Request["examId"].ConvertToInt();

        public bool Status { get; set; }
    }
}
