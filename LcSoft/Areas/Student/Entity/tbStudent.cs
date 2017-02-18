using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Entity
{
    /// <summary>
    /// 学生信息
    /// </summary>
    public class tbStudent : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name = "学生学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名"), Required]
        public string StudentName { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        [Display(Name = "英文名")]
        public string StudentNameEn { get; set; }

        /// <summary>
        /// 学生帐号
        /// </summary>
        [Display(Name = "学生帐号"), Required]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 所属校区
        /// </summary>
        [Display(Name = "所属校区")]
        public virtual Basis.Entity.tbSchool tbSchool { get; set; }

        /// <summary>
        /// 家长帐号
        /// </summary>
        [Display(Name = "家长帐号")]
        public virtual Sys.Entity.tbSysUser tbSysUserFamily { get; set; }

        /// <summary>
        /// 学生类型
        /// </summary>
        [Display(Name = "学生类型")]
        public virtual tbStudentType tbStudentType { get; set; }

        /// <summary>
        /// 就读方式
        /// </summary>
        [Display(Name = "就读方式")]
        public virtual tbStudentStudyType tbStudentStudyType { get; set; }

        /// <summary>
        /// 学届
        /// </summary>
        [Display(Name = "学届")]
        public virtual tbStudentSession tbStudentSession { get; set; }

        /// <summary>
        /// 中考号
        /// </summary>
        [Display(Name = "中考号")]
        public string TicketNumber { get; set; }

        /// <summary>
        /// 借阅证号
        /// </summary>
        [Display(Name = "借阅证号")]
        public string LibraryNo { get; set; }

        /// <summary>
        /// 一卡通
        /// </summary>
        [Display(Name = "一卡通")]
        public string CardNo { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Display(Name = "出生日期")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        [Display(Name = "政治面貌")]
        public virtual Dict.Entity.tbDictParty tbDictParty { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [Display(Name = "民族")]
        public virtual Dict.Entity.tbDictNation tbDictNation { get; set; }

        /// <summary>
        /// 血型
        /// </summary>
        [Display(Name = "血型")]
        public virtual Dict.Entity.tbDictBlood tbDictBlood { get; set; }

        /// <summary>
        /// 家庭住址
        /// </summary>
        [Display(Name = "家庭住址")]
        public string Address { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [Display(Name = "邮政编码")]
        public string PostCode { get; set; }

        /// <summary>
        /// 毕业学校（生源地）
        /// </summary>
        [Display(Name = "毕业学校")]
        public virtual tbStudentSource tbStudentSource { get; set; }

        /// <summary>
        /// 个人简介
        /// </summary>
        [Display(Name = "个人简介")]
        public string Profile { get; set; }

        /// <summary>
        /// 学生照片
        /// </summary>
        [Display(Name = "学生照片")]
        public string Photo { get; set; }

        /// <summary>
        /// 姓名拼音
        /// </summary>
        [Display(Name = "姓名拼音")]
        public string PinYin { get; set; }

        /// <summary>
        /// 入学成绩
        /// </summary>
        [Display(Name = "入学成绩")]
        public decimal? EntranceScore { get; set; }

        /// <summary>
        /// 入学时间
        /// </summary>
        [Display(Name = "入学时间")]
        public DateTime? EntranceDate { get; set; }

        /// <summary>
        /// cmis账号(北京学生的数据都需要汇总到cmis系统中)
        /// </summary>
        [Display(Name = "cmis账号")]
        public string CMIS { get; set; }

        /// <summary>
        /// 教育编号
        /// </summary>
        [Display(Name = "教育编号")]
        public string EduNo { get; set; }




        /// <summary>
        /// 所在班级
        /// </summary>
        [Display(Name = "所在班级")]
        public Basis.Entity.tbClass tbClass { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        [Display(Name = "籍贯")]
        public string NativePlace { get; set; }

        /// <summary>
        /// 出生地
        /// </summary>
        [Display(Name = "出生地")]
        public string BirthPlace { get; set; }

        /// <summary>
        /// 户籍所在地
        /// </summary>
        [Display(Name = "户籍所在地")]
        public string HouseholdRegister { get; set; }

        /// <summary>
        /// 是否独生子女
        /// </summary>
        [Display(Name = "是否独生子女")]
        public bool IsOnlyChild { get; set; }

        /// <summary>
        /// 上下学交通工具
        /// </summary>
        [Display(Name = "上下学交通工具")]
        public Code.EnumHelper.SchoolTransportationType SchoolTransportationType { get; set; }

        /// <summary>
        /// 上下学距离（米）
        /// </summary>
        [Display(Name = "上下学距离（米）")]
        public decimal SchoolDistance { get; set; }

        /// <summary>
        /// 是否进城务工人员随迁子女
        /// </summary>
        [Display(Name = "是否进城务工人员随迁子女")]
        public bool IsSuiqian { get; set; }
    }
}