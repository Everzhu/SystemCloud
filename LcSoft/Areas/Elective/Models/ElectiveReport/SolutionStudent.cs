using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveReport
{
    public class SolutionStudent
    {
        public List<Dto.ElectiveReport.Student> StudentList { get; set; } = new List<Dto.ElectiveReport.Student>();
    }
}