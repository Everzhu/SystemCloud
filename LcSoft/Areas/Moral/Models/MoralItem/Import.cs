using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralItem
{
    public class Import
    {
        public List<Dto.MoralItem.Import> ImportList { get; set; } = new List<Dto.MoralItem.Import>();

        public int MoralId { get; set; } = System.Web.HttpContext.Current.Request["MoralId"].ConvertToInt();

        public bool Status { get; internal set; } = false;

        [Display(Name ="上传文件"),Required]
        public string UploadFile { get; set; }
    }
}