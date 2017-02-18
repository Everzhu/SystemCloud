using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;


namespace XkSystem.Models
{
    public class DbContext : System.Data.Entity.DbContext
    {
        static DbContext()
        {
            System.Data.Entity.Database.SetInitializer<System.Data.Entity.DbContext>(null);
        }

        public DbContext()
            : base("Name=DbEntity")
        {
            this.Configuration.AutoDetectChangesEnabled = true;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
            this.Configuration.UseDatabaseNullSemantics = true;
        }

        public override int SaveChanges()
        {
            var tenant = this.Set<Areas.Admin.Entity.tbTenant>().Find(Code.Common.TenantId);
            //过滤所有修改了的实体，包括：增加 / 修改 / 删除
            var entries = from obj in this.ChangeTracker.Entries()
                          where obj.State != EntityState.Unchanged
                          select obj;
            foreach (var item in entries)
            {
                switch (item.State)
                {
                    case EntityState.Added:
                        if (item.Entity is Code.EntityHelper.EntityRoot)
                        {
                            ((Code.EntityHelper.EntityRoot)item.Entity).UpdateTime = DateTime.Now;
                        }
                        if (item.Entity is Code.EntityHelper.EntityBase)
                        {
                            if (((Code.EntityHelper.EntityBase)item.Entity).tbTenant == null)
                            {
                                ((Code.EntityHelper.EntityBase)item.Entity).tbTenant = tenant;
                            }
                        }
                        break;
                    case EntityState.Deleted:
                        break;
                    case EntityState.Modified:
                        if (item.Entity is Code.EntityHelper.EntityRoot)
                        {
                            ((Code.EntityHelper.EntityRoot)item.Entity).UpdateTime = DateTime.Now;
                        }
                        break;
                    default:
                        break;
                }
            }

            return base.SaveChanges();
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.ManyToManyCascadeDeleteConvention>();
        }

        //平台设置
        private DbSet<Areas.Admin.Entity.tbConfig> tbConfig { get; set; }
        private DbSet<Areas.Admin.Entity.tbProgram> tbProgram { get; set; }
        private DbSet<Areas.Admin.Entity.tbTenant> tbTenant { get; set; }
        private DbSet<Areas.Admin.Entity.tbTenantProgram> tbTenantProgram { get; set; }

        //资产管理
        //private DbSet<Areas.Asset.Entity.tbAsset> tbAsset { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetApply> tbAssetApply { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetCatalog> tbAssetCatalog { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetIn> tbAssetIn { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetOut> tbAssetOut { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetPhoto> tbAssetPhoto { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetPower> tbAssetPower { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetPowerCatalog> tbAssetPowerCatalog { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetPowerDept> tbAssetPowerDept { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetRepair> tbAssetRepair { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetRepairLevel> tbAssetRepairLevel { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetRepairPhoto> tbAssetRepairPhoto { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetService> tbAssetService { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetServicePhoto> tbAssetServicePhoto { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetServiceType> tbAssetServiceType { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetSource> tbAssetSource { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetStatus> tbAssetStatus { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetStorage> tbAssetStorage { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetSupplier> tbAssetSupplier { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetType> tbAssetType { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetUse> tbAssetUse { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetFeedBack> tbAssetFeedBack { get; set; }
        //private DbSet<Areas.Asset.Entity.tbAssetRepairAppraise> tbAssetRepairAppraise { get; set; }

        //基本设置
        private DbSet<Areas.Basis.Entity.tbCalendar> tbCalendar { get; set; }
        private DbSet<Areas.Basis.Entity.tbClass> tbClass { get; set; }
        private DbSet<Areas.Basis.Entity.tbClassAllotClass> tbClassAllotClass { get; set; }
        private DbSet<Areas.Basis.Entity.tbClassAllotResult> tbClassAllotResult { get; set; }
        private DbSet<Areas.Basis.Entity.tbClassAllotStudent> tbClassAllotStudent { get; set; }
        private DbSet<Areas.Basis.Entity.tbClassGroup> tbClassGroup { get; set; }
        private DbSet<Areas.Basis.Entity.tbClassManager> tbClassManager { get; set; }
        private DbSet<Areas.Basis.Entity.tbClassStudent> tbClassStudent { get; set; }
        private DbSet<Areas.Basis.Entity.tbClassTeacher> tbClassTeacher { get; set; }
        private DbSet<Areas.Basis.Entity.tbClassType> tbClassType { get; set; }
        private DbSet<Areas.Basis.Entity.tbGrade> tbGrade { get; set; }
        private DbSet<Areas.Basis.Entity.tbGradeType> tbGradeType { get; set; }
        private DbSet<Areas.Basis.Entity.tbPeriod> tbPeriod { get; set; }
        private DbSet<Areas.Basis.Entity.tbPeriodType> tbPeriodType { get; set; }
        private DbSet<Areas.Basis.Entity.tbRoom> tbRoom { get; set; }
        private DbSet<Areas.Basis.Entity.tbBuild> tbBuild { get; set; }
        private DbSet<Areas.Basis.Entity.tbBuildType> tbBuildType { get; set; }
        private DbSet<Areas.Basis.Entity.tbRoomType> tbRoomType { get; set; }
        private DbSet<Areas.Basis.Entity.tbSchool> tbSchool { get; set; }
        private DbSet<Areas.Basis.Entity.tbWeek> tbWeek { get; set; }
        private DbSet<Areas.Basis.Entity.tbYear> tbYear { get; set; }

        //课程模块
        private DbSet<Areas.Course.Entity.tbCourse> tbCourse { get; set; }
        private DbSet<Areas.Course.Entity.tbCourseDomain> tbCourseDomain { get; set; }
        private DbSet<Areas.Course.Entity.tbCourseGroup> tbCourseGroup { get; set; }
        private DbSet<Areas.Course.Entity.tbCourseType> tbCourseType { get; set; }
        private DbSet<Areas.Course.Entity.tbOrg> tbOrg { get; set; }
        private DbSet<Areas.Course.Entity.tbOrgCalendar> tbOrgCalendar { get; set; }
        private DbSet<Areas.Course.Entity.tbOrgManager> tbOrgManager { get; set; }
        private DbSet<Areas.Course.Entity.tbOrgSchedule> tbOrgSchedule { get; set; }
        private DbSet<Areas.Course.Entity.tbOrgStudent> tbOrgStudent { get; set; }
        private DbSet<Areas.Course.Entity.tbOrgTeacher> tbOrgTeacher { get; set; }
        private DbSet<Areas.Course.Entity.tbSubject> tbSubject { get; set; }

        //数据字典
        private DbSet<Areas.Dict.Entity.tbDictBlood> tbDictBlood { get; set; }
        private DbSet<Areas.Dict.Entity.tbDictCensus> tbDictCensus { get; set; }
        private DbSet<Areas.Dict.Entity.tbDictEducation> tbDictEducations { get; set; }
        private DbSet<Areas.Dict.Entity.tbDictHealth> tbDictHealth { get; set; }
        private DbSet<Areas.Dict.Entity.tbDictKinship> tbDictKinship { get; set; }
        private DbSet<Areas.Dict.Entity.tbDictDegree> tbDictDegrees { get; set; }
        private DbSet<Areas.Dict.Entity.tbDictMajor> tbDictMajors { get; set; }
        private DbSet<Areas.Dict.Entity.tbDictMarriage> tbDictMarriages { get; set; }
        private DbSet<Areas.Dict.Entity.tbDictNation> tbDictNation { get; set; }
        private DbSet<Areas.Dict.Entity.tbDictOverseas> tbDictOverseas { get; set; }
        private DbSet<Areas.Dict.Entity.tbDictPaperSize> tbDictPaperSize { get; set; }
        private DbSet<Areas.Dict.Entity.tbDictParty> tbDictParty { get; set; }
        private DbSet<Areas.Dict.Entity.tbDictRegion> tbDictRegion { get; set; }
        private DbSet<Areas.Dict.Entity.tbDictSex> tbDictSex { get; set; }

        //网盘
        private DbSet<Areas.Disk.Entity.tbDiskConfig> tbDiskConfig { get; set; }
        private DbSet<Areas.Disk.Entity.tbDiskFile> tbDiskFile { get; set; }
        private DbSet<Areas.Disk.Entity.tbDiskFileType> tbDiskFileType { get; set; }
        private DbSet<Areas.Disk.Entity.tbDiskFolder> tbDiskFolder { get; set; }
        private DbSet<Areas.Disk.Entity.tbDiskPower> tbDiskPower { get; set; }
        private DbSet<Areas.Disk.Entity.tbDiskType> tbDiskType { get; set; }

        //住宿管理
        private DbSet<Areas.Dorm.Entity.tbDorm> tbDorm { get; set; }
        private DbSet<Areas.Dorm.Entity.tbDormApply> tbDormApply { get; set; }
        private DbSet<Areas.Dorm.Entity.tbDormData> tbDormData { get; set; }
        private DbSet<Areas.Dorm.Entity.tbDormOption> tbDormOption { get; set; }
        private DbSet<Areas.Dorm.Entity.tbDormStudent> tbDormStudent { get; set; }
        private DbSet<Areas.Dorm.Entity.tbDormTeacher> tbDormTeacher { get; set; }

        //选课管理
        private DbSet<Areas.Elective.Entity.tbElective> tbElective { get; set; }
        private DbSet<Areas.Elective.Entity.tbElectiveClass> tbElectiveClass { get; set; }
        private DbSet<Areas.Elective.Entity.tbElectiveData> tbElectiveData { get; set; }
        private DbSet<Areas.Elective.Entity.tbElectiveGroup> tbElectiveGroup { get; set; }
        private DbSet<Areas.Elective.Entity.tbElectiveOrg> tbElectiveOrg { get; set; }
        private DbSet<Areas.Elective.Entity.tbElectiveOrgClass> tbElectiveOrgClass { get; set; }
        private DbSet<Areas.Elective.Entity.tbElectiveOrgSchedule> tbElectiveOrgSchedule { get; set; }
        private DbSet<Areas.Elective.Entity.tbElectiveOrgStudent> tbElectiveOrgStudent { get; set; }
        private DbSet<Areas.Elective.Entity.tbElectiveRule> tbElectiveRule { get; set; }
        private DbSet<Areas.Elective.Entity.tbElectiveSection> tbElectiveSection { get; set; }
        private DbSet<Areas.Elective.Entity.tbElectiveType> tbElectiveType { get; set; }
        private DbSet<Areas.Elective.Entity.tbElectiveApply> tbElectiveApply { get; set; }
        private DbSet<Areas.Elective.Entity.tbElectiveApplyFile> tbElectiveApplyFile { get; set; }
        private DbSet<Areas.Elective.Entity.tbElectiveApplySchedule> tbElectiveApplySchedule { get; set; }

        private DbSet<Areas.Elective.Entity.tbElectiveSubject> tbElectiveSubject { get; set; }



        //考试成绩
        private DbSet<Areas.Exam.Entity.tbExam> tbExam { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamAllotCourse> tbExamAllotCourse { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamAllotRoom> tbExamAllotRoom { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamAllotStudent> tbExamAllotStudent { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamAllotTeacher> tbExamAllotTeacher { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamAppraiseMark> tbExamAppraiseMark { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamCourse> tbExamCourse { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamLevel> tbExamLevel { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamLevelGroup> tbExamLevelGroup { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamMark> tbExamMark { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamPower> tbExamPower { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamRoom> tbExamRoom { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamSchedule> tbExamSchedule { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamSection> tbExamSection { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamSegmentMark> tbExamSegmentMark { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamStatus> tbExamStatus { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamStudent> tbExamStudent { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamTeacher> tbExamTeacher { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamType> tbExamType { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamImportSegmentMark> tbExamImportSegmentMark { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamSegmentGroup> tbExamSegmentGroup { get; set; }
        private DbSet<Areas.Exam.Entity.tbExamCourseRate> tbExamCourseRate { get; set; }

        //德育
        private DbSet<Areas.Moral.Entity.tbMoral> tbMoral { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralClass> tbMoralClass { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralData> tbMoralData { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralGroup> tbMoralGroup { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralItem> tbMoralItem { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralOption> tbMoralOption { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralPhoto> tbMoralPhoto { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralPower> tbMoralPower { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralPowerClass> tbMoralPowerClass { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralComment> tbMoralComment { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralSelf> tbMoralSelf { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralFamily> tbMoralFamily { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralHappening> tbMoralHappening { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralSuggest> tbMoralSuggest { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralHappeningReply> tbMoralHappeningReply { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralSuggestReply> tbMoralSuggestReply { get; set; }
        private DbSet<Areas.Moral.Entity.tbMoralStar> tbMoralStar { get; set; }

        private DbSet<Areas.Moral.Entity.tbMoralRedFlag> tbMoralRedFlag { get; set; }

        //学习表现
        private DbSet<Areas.Perform.Entity.tbPerform> tbPerform { get; set; }
        private DbSet<Areas.Perform.Entity.tbPerformComment> tbPerformComment { get; set; }
        private DbSet<Areas.Perform.Entity.tbPerformCourse> tbPerformCourse { get; set; }
        private DbSet<Areas.Perform.Entity.tbPerformData> tbPerformData { get; set; }
        private DbSet<Areas.Perform.Entity.tbPerformGroup> tbPerformGroup { get; set; }
        private DbSet<Areas.Perform.Entity.tbPerformItem> tbPerformItem { get; set; }
        private DbSet<Areas.Perform.Entity.tbPerformOption> tbPerformOption { get; set; }
        private DbSet<Areas.Perform.Entity.tbPerformOrg> tbPerformOrg { get; set; }
        private DbSet<Areas.Perform.Entity.tbPerformSubject> tbPerformSubject { get; set; }
        private DbSet<Areas.Perform.Entity.tbPerformTotal> tbPerformTotal { get; set; }

        //考勤
        private DbSet<Areas.Attendance.Entity.tbAttendance> tbAttendance { get; set; }
        private DbSet<Areas.Attendance.Entity.tbAttendanceLog> tbAttendanceLog { get; set; }
        private DbSet<Areas.Attendance.Entity.tbAttendanceTeacher> tbAttendanceTeacher { get; set; }
        private DbSet<Areas.Attendance.Entity.tbAttendanceType> tbAttendanceType { get; set; }

        //学生素养
        private DbSet<Areas.Quality.Entity.tbQuality> tbQuality { get; set; }
        private DbSet<Areas.Quality.Entity.tbQualityComment> tbQualityComment { get; set; }
        private DbSet<Areas.Quality.Entity.tbQualityData> tbQualityData { get; set; }
        private DbSet<Areas.Quality.Entity.tbQualityFamily> tbQualityFamily { get; set; }
        private DbSet<Areas.Quality.Entity.tbQualityHope> tbQualityHope { get; set; }
        private DbSet<Areas.Quality.Entity.tbQualityItem> tbQualityItem { get; set; }
        private DbSet<Areas.Quality.Entity.tbQualityItemGroup> tbQualityItemGroup { get; set; }
        private DbSet<Areas.Quality.Entity.tbQualityOption> tbQualityOption { get; set; }
        private DbSet<Areas.Quality.Entity.tbQualityPhoto> tbQualityPhoto { get; set; }
        private DbSet<Areas.Quality.Entity.tbQualityPlan> tbQualityPlan { get; set; }
        private DbSet<Areas.Quality.Entity.tbQualityPortrait> tbQualityPortrait { get; set; }
        private DbSet<Areas.Quality.Entity.tbQualityRemark> tbQualityRemark { get; set; }
        private DbSet<Areas.Quality.Entity.tbQualitySelf> tbQualitySelf { get; set; }
        private DbSet<Areas.Quality.Entity.tbQualitySummary> tbQualitySummary { get; set; }

        //短信
        private DbSet<Areas.Sms.Entity.tbSms> tbSms { get; set; }
        private DbSet<Areas.Sms.Entity.tbSmsConfig> tbSmsConfig { get; set; }
        private DbSet<Areas.Sms.Entity.tbSmsLog> tbSmsLog { get; set; }
        private DbSet<Areas.Sms.Entity.tbSmsTo> tbSmsTo { get; set; }
        private DbSet<Areas.Sms.Entity.tbSmsTemplet> tbSmsTemplet { get; set; }


        //学生管理
        private DbSet<Areas.Student.Entity.tbStudent> tbStudent { get; set; }
        private DbSet<Areas.Student.Entity.tbStudentBest> tbStudentBest { get; set; }
        private DbSet<Areas.Student.Entity.tbStudentChange> tbStudentChange { get; set; }
        private DbSet<Areas.Student.Entity.tbStudentChangeType> tbStudentChangeType { get; set; }
        private DbSet<Areas.Student.Entity.tbStudentFamily> tbStudentFamily { get; set; }
        private DbSet<Areas.Student.Entity.tbStudentHonor> tbStudentHonor { get; set; }
        private DbSet<Areas.Student.Entity.tbStudentHonorLevel> tbStudentHonorLevel { get; set; }
        private DbSet<Areas.Student.Entity.tbStudentHonorType> tbStudentHonorType { get; set; }
        private DbSet<Areas.Student.Entity.tbStudentSession> tbStudentSession { get; set; }
        private DbSet<Areas.Student.Entity.tbStudentSource> tbStudentSource { get; set; }
        private DbSet<Areas.Student.Entity.tbStudentStudyType> tbStudentStudyType { get; set; }
        private DbSet<Areas.Student.Entity.tbStudentType> tbStudentType { get; set; }

        //晚自习管理
        private DbSet<Areas.Study.Entity.tbStudy> tbStudy { get; set; }
        private DbSet<Areas.Study.Entity.tbStudyApply> tbStudyApply { get; set; }
        private DbSet<Areas.Study.Entity.tbStudyClass> tbStudyClass { get; set; }
        private DbSet<Areas.Study.Entity.tbStudyClassStudent> tbStudyClassStudent { get; set; }
        private DbSet<Areas.Study.Entity.tbStudyClassTeacher> tbStudyClassTeacher { get; set; }
        private DbSet<Areas.Study.Entity.tbStudyData> tbStudyData { get; set; }
        private DbSet<Areas.Study.Entity.tbStudyOption> tbStudyOption { get; set; }
        private DbSet<Areas.Study.Entity.tbStudyCost> tbStudyCost { get; set; }
        private DbSet<Areas.Study.Entity.tbStudyRoom> tbStudyRoom { get; set; }
        private DbSet<Areas.Study.Entity.tbStudyRoomStudent> tbStudyRoomStudent { get; set; }
        private DbSet<Areas.Study.Entity.tbStudyRoomTeacher> tbStudyRoomTeacher { get; set; }
        private DbSet<Areas.Study.Entity.tbStudyTimetable> tbStudyTimetable { get; set; }

        //教学评估
        private DbSet<Areas.Survey.Entity.tbSurvey> tbSurvey { get; set; }
        private DbSet<Areas.Survey.Entity.tbSurveyClass> tbSurveyClass { get; set; }
        private DbSet<Areas.Survey.Entity.tbSurveyCourse> tbSurveyCourse { get; set; }
        private DbSet<Areas.Survey.Entity.tbSurveyData> tbSurveyData { get; set; }
        private DbSet<Areas.Survey.Entity.tbSurveyGroup> tbSurveyGroup { get; set; }
        private DbSet<Areas.Survey.Entity.tbSurveyItem> tbSurveyItem { get; set; }
        private DbSet<Areas.Survey.Entity.tbSurveyOption> tbSurveyOption { get; set; }
        private DbSet<Areas.Survey.Entity.tbSurveyOrg> tbSurveyOrg { get; set; }
        private DbSet<Areas.Survey.Entity.tbSurveySubject> tbSurveySubject { get; set; }

        //系统设定
        private DbSet<Areas.Sys.Entity.tbSysConfig> tbSysConfig { get; set; }
        private DbSet<Areas.Sys.Entity.tbSysMenu> tbSysMenu { get; set; }
        private DbSet<Areas.Sys.Entity.tbSysMessage> tbSysMessage { get; set; }
        private DbSet<Areas.Sys.Entity.tbSysMessageUser> tbSysMessageUser { get; set; }
        private DbSet<Areas.Sys.Entity.tbSysMessageRole> tbSysMessageRole { get; set; }
        private DbSet<Areas.Sys.Entity.tbSysRole> tbSysRole { get; set; }
        private DbSet<Areas.Sys.Entity.tbSysRolePower> tbSysRolePower { get; set; }
        private DbSet<Areas.Sys.Entity.tbSysShortcut> tbSysShortcut { get; set; }
        private DbSet<Areas.Sys.Entity.tbSysUser> tbSysUser { get; set; }
        private DbSet<Areas.Sys.Entity.tbSysUserLog> tbSysUserLog { get; set; }
        private DbSet<Areas.Sys.Entity.tbSysUserPower> tbSysUserPower { get; set; }
        private DbSet<Areas.Sys.Entity.tbSysUserRole> tbSysUserRole { get; set; }

        //教师管理
        private DbSet<Areas.Teacher.Entity.tbTeacher> tbTeacher { get; set; }
        private DbSet<Areas.Teacher.Entity.tbTeacherDept> tbTeacherDept { get; set; }
        private DbSet<Areas.Teacher.Entity.tbTeacherGrade> tbTeacherGrade { get; set; }
        private DbSet<Areas.Teacher.Entity.tbTeacherHonor> tbTeacherHonor { get; set; }
        private DbSet<Areas.Teacher.Entity.tbTeacherHonorLevel> tbTeacherHonorLevel { get; set; }
        private DbSet<Areas.Teacher.Entity.tbTeacherHonorType> tbTeacherHonorType { get; set; }
        private DbSet<Areas.Teacher.Entity.tbTeacherJob> tbTeacherJob { get; set; }
        private DbSet<Areas.Teacher.Entity.tbTeacherLevel> tbTeacherLevel { get; set; }
        private DbSet<Areas.Teacher.Entity.tbTeacherType> tbTeacherType { get; set; }
        private DbSet<Areas.Teacher.Entity.tbTeacherSubject> tbTeacherSubject { get; set; }
        private DbSet<Areas.Teacher.Entity.tbTeacherWithDept> tbTeacherWithDept { get; set; }

        //微信
        private DbSet<Areas.Wechat.Entity.tbWeOAFlowType> tbWeOAFlowType { get; set; }
        private DbSet<Areas.Wechat.Entity.tbWeOAFlowNode> tbWeOAFlowNode { get; set; }
        private DbSet<Areas.Wechat.Entity.tbWeOAFlowDetail> tbWeOAFlowDetail { get; set; }
        private DbSet<Areas.Wechat.Entity.tbWeOAFlowApprover> tbWeOAFlowApprover { get; set; }
        private DbSet<Areas.Wechat.Entity.tbWeOAOffice> tbWeOAOffice { get; set; }
        private DbSet<Areas.Wechat.Entity.tbWeOAApplyCar> tbWeOAApplyCar { get; set; }
        private DbSet<Areas.Wechat.Entity.tbWeOAApplyLeave> tbWeOAApplyLeave { get; set; }
        private DbSet<Areas.Wechat.Entity.tbWeOALeaveType> tbWeOALeaveType { get; set; }
    }
}