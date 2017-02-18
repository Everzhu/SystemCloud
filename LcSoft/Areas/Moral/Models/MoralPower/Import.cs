using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralPower
{
    public class Import
    {

        public List<Dto.MoralPower.Import> ImportList { get; set; } = new List<Dto.MoralPower.Import>();

        public int MoralItemId { get; set; } = System.Web.HttpContext.Current.Request["MoralItemId"].ConvertToInt();

        public int MoralId { get; set; } = System.Web.HttpContext.Current.Request["MoralId"].ConvertToInt();

        [Display(Name ="上传文件"),Required]
        public string UploadFile { get; set; }

        public bool Status { get; internal set; } = false;

    }
}