using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveSubject
{
    public class List
    {
        public int Id { get; set; }

        [Display(Name ="科目名称"),Required]
        public string SubjectName { get; set; }

        
        public int SubjectId { get; set; }

        [Display(Name ="状态")]
        public bool IsOpen { get; set; }
    }
}