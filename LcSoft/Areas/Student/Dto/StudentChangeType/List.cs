using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Student.Dto.StudentChangeType
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 学生调动类型名称
        /// </summary>
        [Display(Name = "学生调动类型")]
        public string StudentChangeTypeName { get; set; }

        /// <summary>
        /// 异动类型
        /// </summary>
        [Display(Name = "异动类型")]
        public string StudentChangeTypeCodeName { get; set; }
        public int StudentChangeTypeCode { get; set; }
    }
}
