using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamAnalyze
{
    public class List
    {
        public List<Dto.ExamAnalyze.List> ExamAnalyzeList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> ExamGradeAnalyzeList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> ExamAnalyzeImportSegmentList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> ExamAnalyzeSegmentList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.SegmentList> SegmentList { get; set; } = new List<Dto.ExamAnalyze.SegmentList>();

        public List<Dto.ExamAnalyze.SegmentList> ImortSegmentList { get; set; } = new List<Dto.ExamAnalyze.SegmentList>();

        public List<Dto.ExamAnalyze.List> ClassStudentList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> SubjectTeacherList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> ClassTeacherList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> ClassOrgStudentList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> StudentList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> CompreList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<System.Web.Mvc.SelectListItem> ExamList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Dto.ExamAnalyze.List> ExamTotalList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<System.Web.Mvc.SelectListItem> ExamThanList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> LastExamList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> selctClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> selectSubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> LevelList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> RankList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Entity.tbExamLevel> TotalLevelList { get; set; } = new List<Entity.tbExamLevel>();

        public List<Entity.tbExamLevel> LsTotalLevelList { get; set; } = new List<Entity.tbExamLevel>();

        public List<Entity.tbExamLevel> NtTotalLevelList { get; set; } = new List<Entity.tbExamLevel>();

        public List<Dto.ExamAnalyze.List> SubjectTotalLevelList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> LsClassStudentLevelList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> NtClassStudentLevelList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> SClassStudentLevelList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> SNtClassStudentLevelList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> ExamLsStudentList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> ExamNtStudentList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> FractionalList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public List<Dto.ExamAnalyze.List> TopTotalExamMarkList { get; set; } = new List<Dto.ExamAnalyze.List>();
        public List<Dto.ExamAnalyze.List> DownTotalExamMarkList { get; set; } = new List<Dto.ExamAnalyze.List>();
        public List<Dto.ExamAnalyze.List> StudentExamMarkList { get; set; } = new List<Dto.ExamAnalyze.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ExamId { get; set; } = System.Web.HttpContext.Current.Request["ExamId"].ConvertToInt();
        public int LastExamId { get; set; } = System.Web.HttpContext.Current.Request["LastExamId"].ConvertToInt();

        public int? GradeId { get; set; } = System.Web.HttpContext.Current.Request["GradeId"].ConvertToInt();

        public int? SubjectId { get; set; } = System.Web.HttpContext.Current.Request["SubjectId"].ConvertToInt();

        public string chkSubject { get; set; }= HttpContext.Current.Request["chkSubject"];

        public string chkClass { get; set; }= HttpContext.Current.Request["chkClass"];

        public string chkSelectSubject { get; set; }

        public string chkSelectClass { get; set; }

        public string RankName { get; set; }

        public string ReportScoreGrade { get; set; }

        public string lstComm { get; set; }

        public string  totalCommData { get; set; }

        public List<string> OptionList { get; set; }

        public List<string> ClumnList { get; set; }

        public List<string> GoodPassList { get; set; }
        public decimal goodCount { get; set; }
        public decimal passCount { get; set; }

        public string CheckedAll { get; set; }= HttpContext.Current.Request["CheckedAll"];

        public string chkClassAll { get; set; } = HttpContext.Current.Request["chkClassAll"];

        public decimal MaxTotalMark { get; set; }
    }
}