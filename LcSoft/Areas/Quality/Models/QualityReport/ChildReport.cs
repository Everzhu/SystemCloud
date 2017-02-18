using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityReport
{
    public class ChildReport
    {
        public string ClassName { get; set; }
        public string StudentCode { get; set; }
        public string StudentName { get; set; }
        public string ClassTeacher { get; set; }
        public int GradeId { get; set; } = System.Web.HttpContext.Current.Request["GradeId"].ConvertToInt();
        public int YearTermId { get; set; } = System.Web.HttpContext.Current.Request["YearTermId"].ConvertToInt();
        public Dto.QualityReport.ChildReport SelfComment { get; set; }
        public Dto.QualityReport.ChildReport PlanComment { get; set; }
        public Dto.QualityReport.ChildReport SummaryComment { get; set; }
        public Dto.QualityReport.ChildReport ClassComment { get; set; }
        public List<Dto.QualityReport.ChildReport> OrgTeacherComment { get; set; } = new List<Dto.QualityReport.ChildReport>();
        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> CourseGroupItemList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<Course.Entity.tbCourseDomain> CourseDomainItemList { get; set; } = new List<Course.Entity.tbCourseDomain>();
        public List<Dto.QualityReport.ChildReport> QualityDataList { get; set; } = new List<Dto.QualityReport.ChildReport>();
        public List<Dto.QualityReport.ChildReport> HonorList { get; set; } = new List<Dto.QualityReport.ChildReport>();
        //public List<Dto.QualityReport.ChildReport> CourseGroupList { get; set; } = new List<Dto.QualityReport.ChildReport>();
        public List<Dto.QualityReport.StudentReport> CourseDomainList { get; set; } = new List<Dto.QualityReport.StudentReport>();
        public List<Dto.QualityReport.ChildReport> ExamMarkList { get; set; } = new List<Dto.QualityReport.ChildReport>();
        public List<Basis.Entity.tbClassStudent> GradeYearList { get; set; } = new List<Basis.Entity.tbClassStudent>();
        public List<Entity.tbQuality> QualityList { get; set; } = new List<Entity.tbQuality>();
        public List<Basis.Entity.tbYear> YearTermList { get; set; } = new List<Basis.Entity.tbYear>();
        public List<Basis.Entity.tbYear> YearSectionList { get; set; } = new List<Basis.Entity.tbYear>();
        public List<Exam.Entity.tbExam> ExamList { get; set; } = new List<Exam.Entity.tbExam>();
        public string CourseDemainNames { get; set; }
        public string CourseDemainAvgs { get; set; }
        public List<Dto.QualityReport.ChildReport> CourseDemainAvgList { get; set; } = new List<Dto.QualityReport.ChildReport>();
        public List<Dto.QualityReport.ChildReport> ClassLevelAvgList { get; set; } = new List<Dto.QualityReport.ChildReport>();
        public List<Dto.QualityReport.ChildReport> GradeLevelAvgList { get; set; } = new List<Dto.QualityReport.ChildReport>();
        public int? CourseGroupItemId { get; set; } = System.Web.HttpContext.Current.Request["CourseGroupItemId"].ConvertToInt();
        public List<System.Web.Mvc.SelectListItem> OrgList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<Attendance.Entity.tbAttendanceType> AttendanceTypeList { get; set; } = new List<Attendance.Entity.tbAttendanceType>();
        public List<Attendance.Entity.tbAttendance> AttendanceList { get; set; } = new List<Attendance.Entity.tbAttendance>();
    }
}