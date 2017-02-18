using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace XkSystem.Areas.Moral.Dto.MoralClass
{
    public class Info
    {
        /// <summary>
        /// Id
        /// </summary>
        public int ClassId { get; set; }

        /// <summary>
        /// 参评班级 
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        public int GradeId { get; set; }
    }
}