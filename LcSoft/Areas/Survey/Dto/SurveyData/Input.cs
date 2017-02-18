using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Dto.SurveyData
{
    public class Input
    {
        public class OrgTeacher
        {
            /// <summary>
            /// 教学班
            /// </summary>
            [Display(Name = "教学班")]
            public int OrgId { get; set; }

            /// <summary>
            /// 评教Id
            /// </summary>
            [Display(Name = "评教Id")]
            public int SurveyId { get; set; }
            /// <summary>
            /// 评教分组
            /// </summary>
            [Display(Name = "评教分组")]
            public int SurveyGroupId { get; set; }
            /// <summary>
            /// 评教分组
            /// </summary>
            [Display(Name = "评教分组")]
            public string SurveyGroupName { get; set; }
            /// <summary>
            /// 教师Id
            /// </summary>
            [Display(Name = "教师Id")]
            public int TeacherId { get; set; }
            /// <summary>
            /// 教师工号
            /// </summary>
            [Display(Name = "教师工号")]
            public string TeacherCode { get; set; }
            /// <summary>
            /// 教师姓名
            /// </summary>
            [Display(Name = "教师姓名")]
            public string TeacherName { get; set; }

            /// <summary>
            /// 教学班
            /// </summary>
            [Display(Name = "教学班")]
            public string OrgName { get; set; }

            /// <summary>
            /// 是否选中
            /// </summary>
            [Display(Name = "是否选中")]
            public bool IsChecked { get; set; }

            /// <summary>
            /// 教学评价-评价设置-评价分组类型
            /// </summary>
            public bool IsClass { get; set; }

            /// <summary>
            /// 教学评价-评价设置-评价分组类型
            /// </summary>
            public int Ranking { get; set; }

            /// <summary>
            /// 是否已评
            /// </summary>
            [Display(Name = "是否已评")]
            public bool IsHaveInput { get; set; }
        }
    }
}