using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyRoomTeacher
{
    public class Edit
    {
        public int Id { get; set; }
        /// <summary>
        /// 是否主要负责人
        /// </summary>
        [Display(Name = "是否主要负责人"), Required]
        public bool IsMaster { get; set; } = false;
    }
}