using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveApply
{
    public class Detail:List
    {


        [Display(Name ="科目")]
        public string SubjectName { get; set; }



    }
}