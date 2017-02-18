using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.StudentHonor
{
    public class InsertHonor
    {
        public int ID { get; set; }

        public string HonorName { get; set; }

        public string FileName { get; set; }

        public int StudentHonorLevelId { get; set; }
        public int StudentHonorTypeId { get; set; }
        public int StudentHonorSourceId { get; set; }
        public string StudentHonorLevelName { get; set; }
        public string StudentHonorTypeName { get; set; }
        public string StudentHonorSourceName { get; set; }
        public int? YearId { get; set; }
    }
}