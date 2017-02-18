using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyItem
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        [Display(Name = "对于系统中已存在的数据做更新操作")]
        public bool IsUpdate { get; set; }

        public List<Dto.SurveyItem.Import> ImportList { get; set; } = new List<Dto.SurveyItem.Import>();

        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();

        /// <summary>
        /// 上传状态
        /// </summary>
        public bool Status { get; set; }
    }
}