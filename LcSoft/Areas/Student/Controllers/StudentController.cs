using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.IO;

namespace XkSystem.Areas.Student.Controllers
{
    public class StudentController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Student.List();
                vm.StudentTypeList = StudentTypeController.SelectList();
                vm.StudentStudyTypeList = StudentStudyTypeController.SelectList();
                vm.StudentSessionList = StudentSessionController.SelectList();
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                vm.ClassList = Basis.Controllers.ClassController.SelectList(0, vm.GradeId);

                vm.UserType = Code.Common.UserType;

                var tb = from p in db.Table<Entity.tbStudent>()
                         select p;

                var tbYear = (from p in db.Table<Basis.Entity.tbYear>()
                              join t in db.Table<Basis.Entity.tbYear>() on p.Id equals t.tbYearParent.Id
                              join s in db.Table<Basis.Entity.tbYear>() on t.Id equals s.tbYearParent.Id
                              where s.IsDefault
                              select p.Id);

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.StudentCode.Contains(vm.SearchText) || d.StudentName.Contains(vm.SearchText));
                }
                else if (vm.GradeId != 0 || vm.ClassId != 0)
                {
                    var classStudent = from p in db.Table<Basis.Entity.tbClassStudent>()
                                       where p.tbStudent.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbClass.tbGrade.IsDeleted == false
                                        && p.tbClass.tbClassType.IsDeleted == false
                                        && tbYear.Contains(p.tbClass.tbYear.Id)
                                       select p;

                    if (vm.GradeId != 0)
                    {
                        classStudent = classStudent.Where(d => d.tbClass.tbGrade.Id == vm.GradeId);
                    }

                    if (vm.ClassId != 0)
                    {
                        classStudent = classStudent.Where(d => d.tbClass.Id == vm.ClassId);
                    }

                    tb = from p in classStudent
                         select p.tbStudent;
                }

                if (vm.StudentTypeId != 0)
                {
                    tb = tb.Where(d => d.tbStudentType.Id == vm.StudentTypeId);
                }

                if (vm.StudyTypeId != 0)
                {
                    tb = tb.Where(d => d.tbStudentStudyType.Id == vm.StudyTypeId);
                }

                if (vm.StudentSessionId != 0)
                {
                    tb = tb.Where(d => d.tbStudentSession.Id == vm.StudentSessionId);
                }

                vm.StudentList = (from p in tb
                                  orderby p.StudentCode descending
                                  select new Dto.Student.List
                                  {
                                      Id = p.Id,
                                      Birthday = p.Birthday.ToString(),
                                      SexName = p.tbSysUser.tbSex.SexName,
                                      DictNationName = p.tbDictNation.NationName,
                                      DictPartyName = p.tbDictParty.PartyName,
                                      BloodTypeName = p.tbDictBlood.BloodName,
                                      CardNo = p.CardNo,
                                      Email = p.tbSysUser.Email,
                                      IdentityNumber = p.tbSysUser.IdentityNumber,
                                      LibraryNo = p.LibraryNo,
                                      Mobile = p.tbSysUser.Mobile,
                                      Photo = p.Photo,
                                      Profile = p.Profile,
                                      Qq = p.tbSysUser.Qq,
                                      StudentCode = p.StudentCode,
                                      StudentName = p.StudentName,
                                      StudentNameEn = p.StudentNameEn,
                                      StudentStudyTypeName = p.tbStudentStudyType.StudyTypeName,
                                      StudentTypeName = p.tbStudentType.StudentTypeName,
                                      TicketNumber = p.TicketNumber,
                                      Address = p.Address,
                                  }).ToPageList(vm.Page);
                var studentIds = vm.StudentList.Select(d => d.Id).ToList();
                var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                        where p.tbClass.IsDeleted == false
                                            && tbYear.Contains(p.tbClass.tbYear.Id)
                                            && p.tbClass.tbYear.IsDeleted == false
                                            && studentIds.Contains(p.tbStudent.Id)
                                        select new
                                        {
                                            StudentId = p.tbStudent.Id,
                                            p.tbClass.ClassName
                                        }).ToList();
                foreach (var a in vm.StudentList)
                {
                    a.ClassName = classStudentList.Where(d => d.StudentId == a.Id).Select(d => d.ClassName).FirstOrDefault();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Student.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                studentTypeId = vm.StudentTypeId,
                studyTypeId = vm.StudyTypeId,
                studentSessionId = vm.StudentSessionId,
                gradeId = vm.GradeId,
                classId = vm.ClassId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult InfoList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Student.InfoList();

                if (Code.Common.UserType == Code.EnumHelper.SysUserType.Teacher && db.Table<Course.Entity.tbOrgTeacher>().Where(d => d.tbTeacher.Id == Code.Common.UserId).Count() > 0 && false)
                {
                    vm.IsClass = false;
                    vm.OrgList = Course.Controllers.OrgController.SelectList();
                    var tb = db.Table<Course.Entity.tbOrgStudent>();
                    if (vm.OrgId > 0)
                    {
                        tb = tb.Where(d => d.tbOrg.Id == vm.OrgId);
                    }
                    if (!string.IsNullOrEmpty(vm.SearchText))
                    {
                        tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText)
                                      || d.tbStudent.StudentName.Contains(vm.SearchText)
                                      || d.tbStudent.StudentNameEn.Contains(vm.SearchText));
                    }
                    vm.List = (from p in tb
                               orderby p.No
                               select new Dto.Student.InfoList()
                               {
                                   #region
                                   Id = p.tbStudent.Id,
                                   Birthday = p.tbStudent.Birthday.ToString(),
                                   SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                   DictNationName = p.tbStudent.tbDictNation.NationName,
                                   DictPartyName = p.tbStudent.tbDictParty.PartyName,
                                   BloodTypeName = p.tbStudent.tbDictBlood.BloodName,
                                   CardNo = p.tbStudent.CardNo,
                                   Email = p.tbStudent.tbSysUser.Email,
                                   IdentityNumber = p.tbStudent.tbSysUser.IdentityNumber,
                                   LibraryNo = p.tbStudent.LibraryNo,
                                   Mobile = p.tbStudent.tbSysUser.Mobile,
                                   Photo = p.tbStudent.Photo,
                                   Profile = p.tbStudent.Profile,
                                   Qq = p.tbStudent.tbSysUser.Qq,
                                   StudentCode = p.tbStudent.StudentCode,
                                   StudentName = p.tbStudent.StudentName,
                                   StudentNameEn = p.tbStudent.StudentNameEn,
                                   StudentStudyTypeName = p.tbStudent.tbStudentStudyType.StudyTypeName,
                                   StudentTypeName = p.tbStudent.tbStudentType.StudentTypeName,
                                   TicketNumber = p.tbStudent.TicketNumber,
                                   Address = p.tbStudent.Address,
                                   ClassName = p.tbOrg.OrgName
                                   #endregion
                               }).ToPageList(vm.Page);
                }
                else
                {
                    vm.IsClass = true;
                    vm.ClassList = Basis.Controllers.ClassController.SelectList();
                    var tb = db.Table<Basis.Entity.tbClassStudent>();
                    if (vm.ClassId > 0)
                    {
                        tb = tb.Where(d => d.tbClass.Id == vm.ClassId);
                    }
                    if (!string.IsNullOrEmpty(vm.SearchText))
                    {
                        tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText)
                                      || d.tbStudent.StudentName.Contains(vm.SearchText)
                                      || d.tbStudent.StudentNameEn.Contains(vm.SearchText));
                    }
                    vm.List = (from p in tb
                               orderby p.No
                               select new Dto.Student.InfoList()
                               {
                                   #region
                                   Id = p.tbStudent.Id,
                                   Birthday = p.tbStudent.Birthday.ToString(),
                                   SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                   DictNationName = p.tbStudent.tbDictNation.NationName,
                                   DictPartyName = p.tbStudent.tbDictParty.PartyName,
                                   BloodTypeName = p.tbStudent.tbDictBlood.BloodName,
                                   CardNo = p.tbStudent.CardNo,
                                   Email = p.tbStudent.tbSysUser.Email,
                                   IdentityNumber = p.tbStudent.tbSysUser.IdentityNumber,
                                   LibraryNo = p.tbStudent.LibraryNo,
                                   Mobile = p.tbStudent.tbSysUser.Mobile,
                                   Photo = p.tbStudent.Photo,
                                   Profile = p.tbStudent.Profile,
                                   Qq = p.tbStudent.tbSysUser.Qq,
                                   StudentCode = p.tbStudent.StudentCode,
                                   StudentName = p.tbStudent.StudentName,
                                   StudentNameEn = p.tbStudent.StudentNameEn,
                                   StudentStudyTypeName = p.tbStudent.tbStudentStudyType.StudyTypeName,
                                   StudentTypeName = p.tbStudent.tbStudentType.StudentTypeName,
                                   TicketNumber = p.tbStudent.TicketNumber,
                                   Address = p.tbStudent.Address,
                                   ClassName = p.tbClass.ClassName
                                   #endregion
                               }).ToPageList(vm.Page);
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InfoList(Models.Student.InfoList vm)
        {
            if (vm.IsClass)
            {
                return Code.MvcHelper.Post(null, Url.Action("InfoList", new
                {
                    SearchText = vm.SearchText,
                    ClassId = vm.ClassId,
                    pageIndex = vm.Page.PageIndex,
                    pageSize = vm.Page.PageSize
                }));
            }
            else
            {
                return Code.MvcHelper.Post(null, Url.Action("InfoList", new
                {
                    SearchText = vm.SearchText,
                    OrgId = vm.OrgId,
                    pageIndex = vm.Page.PageIndex,
                    pageSize = vm.Page.PageSize
                }));
            }
        }

        [AllowAnonymous]
        public ActionResult StudentInfoAll(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Student.Info2();
                vm.StudentFamilyInfo = new List<Dto.StudentFamily.Info>();
                vm.StudentHonorInfo = new List<Dto.StudentHonor.Info>();

                var student = db.TableRoot<Entity.tbStudent>()
                    .Include(d => d.tbDictBlood)
                    .Include(d => d.tbDictNation)
                    .Include(d => d.tbDictParty)
                    .Include(d => d.tbStudentSession)
                    .Include(d => d.tbStudentStudyType)
                    .Include(d => d.tbStudentType)
                    .Include(d => d.tbSysUser)
                    .Include(d => d.tbStudentSource)
                    .Include(d => d.tbSysUser.tbSex)
                    .Where(d => d.Id == id).FirstOrDefault();
                if (student != null)
                {
                    vm.StudentInfo = new Dto.Student.Info2();
                    vm.StudentInfo.Address = student.Address;
                    vm.StudentInfo.Birthday = student.Birthday;
                    vm.StudentInfo.BloodType = student.tbDictBlood != null ? student.tbDictBlood.BloodName : string.Empty;
                    vm.StudentInfo.CardNo = student.CardNo;
                    vm.StudentInfo.CMIS = student.CMIS;
                    vm.StudentInfo.DictNation = student.tbDictNation != null ? student.tbDictNation.NationName : string.Empty;
                    vm.StudentInfo.DictParty = student.tbDictParty != null ? student.tbDictParty.PartyName : string.Empty;
                    vm.StudentInfo.Email = student.tbSysUser.Email;
                    vm.StudentInfo.Id = student.Id;
                    vm.StudentInfo.IdentityNumber = student.tbSysUser.IdentityNumber;
                    vm.StudentInfo.LibraryNo = student.LibraryNo;
                    vm.StudentInfo.Mobile = student.tbSysUser.Mobile;
                    vm.StudentInfo.Photo = student.Photo;
                    vm.StudentInfo.PinYin = student.PinYin;
                    vm.StudentInfo.Profile = student.Profile;
                    vm.StudentInfo.Qq = student.tbSysUser.Qq;
                    vm.StudentInfo.Sex = student.tbSysUser.tbSex != null ? student.tbSysUser.tbSex.SexName : string.Empty;
                    vm.StudentInfo.StudentCode = student.StudentCode;
                    vm.StudentInfo.StudentName = student.StudentName;
                    vm.StudentInfo.StudentNameEn = student.StudentNameEn;
                    vm.StudentInfo.StudentSession = student.tbStudentSession != null ? student.tbStudentSession.StudentSessionName : string.Empty;
                    vm.StudentInfo.StudentStudyType = student.tbStudentStudyType != null ? student.tbStudentStudyType.StudyTypeName : string.Empty;
                    vm.StudentInfo.StudentType = student.tbStudentType != null ? student.tbStudentType.StudentTypeName : string.Empty;
                    vm.StudentInfo.TicketNumber = student.TicketNumber;
                    vm.StudentInfo.EntranceDate = student.EntranceDate;
                    vm.StudentInfo.EntranceScore = student.EntranceScore;
                    vm.StudentInfo.PostCode = student.PostCode;
                    vm.StudentInfo.StudentSourceName = student.tbStudentSource == null ? "" : student.tbStudentSource.StudentSourceName;

                    var studentFamilyList = db.TableRoot<Entity.tbStudentFamily>()
                        .Include(d => d.tbDictKinship).Where(d => d.tbStudent.Id == id).ToList();
                    var studentHonorList = db.TableRoot<Entity.tbStudentHonor>()
                        .Include(d => d.tbstudentHonorLevel)
                        .Include(d => d.tbStudentHonorType)
                        .Where(d => d.tbStudent.Id == id).ToList();
                    foreach (var v in studentFamilyList)
                    {
                        vm.StudentFamilyInfo.Add(new Dto.StudentFamily.Info()
                        {
                            Id = v.Id,
                            FamilyName = v.FamilyName,
                            Job = v.Job,
                            Mobile = v.Mobile,
                            Email = v.Email,
                            KinshipId = v.tbDictKinship.Id,
                            Relation = v.tbDictKinship.KinshipName,
                            UnitName = v.UnitName
                        });
                    }
                    foreach (var v in studentHonorList)
                    {
                        vm.StudentHonorInfo.Add(new Dto.StudentHonor.Info()
                        {
                            Id = v.Id,
                            StudentHonorLevel = v.tbstudentHonorLevel.StudentHonorLevelName,
                            StudentHonorType = v.tbStudentHonorType.StudentHonorTypeName,
                            HonorFile = Url.Content("~/Files/StudentHonor/") + v.HonorFile,
                            HonorName = v.HonorName
                        });
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbStudent>()
                            .Include(d => d.tbSysUser)
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    a.tbSysUser.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除学生");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Student.Edit();
                vm.StudentTypeList = Student.Controllers.StudentTypeController.SelectList();
                vm.StudentStudyTypeList = Student.Controllers.StudentStudyTypeController.SelectList();
                vm.SexList = Dict.Controllers.DictSexController.SelectList();
                vm.BloodTypeList = Dict.Controllers.DictBloodController.SelectList();
                vm.DictNationList = Dict.Controllers.DictNationController.SelectList();
                vm.DictPartyList = Dict.Controllers.DictPartyController.SelectList();
                vm.StudentSessionList = StudentSessionController.SelectList();

                vm.StudentFamilyJson = "family";
                vm.StudentHonorJson = "honor";

                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbStudent>()
                              where p.Id == id
                              select new Dto.Student.Edit
                              {
                                  #region Dto.Student.Edit
                                  Id = p.Id,
                                  Birthday = p.Birthday,
                                  PinYin = p.PinYin,
                                  CMIS = p.CMIS,
                                  SexId = p.tbSysUser.tbSex.Id,
                                  BloodTypeId = p.tbDictBlood.Id,
                                  DictNationId = p.tbDictNation.Id,
                                  DictPartyId = p.tbDictParty.Id,
                                  CardNo = p.CardNo,
                                  Email = p.tbSysUser.Email,
                                  IdentityNumber = p.tbSysUser.IdentityNumber,
                                  LibraryNo = p.LibraryNo,
                                  Mobile = p.tbSysUser.Mobile,
                                  Photo = p.Photo,
                                  Profile = p.Profile,
                                  Qq = p.tbSysUser.Qq,
                                  StudentCode = p.StudentCode,
                                  StudentName = p.StudentName,
                                  StudentNameEn = p.StudentNameEn,
                                  StudentStudyTypeId = p.tbStudentStudyType.Id,
                                  StudentTypeId = p.tbStudentType.Id,
                                  StudentSessionId = p.tbStudentSession.Id,
                                  TicketNumber = p.TicketNumber,
                                  Address = p.Address,
                                  EntranceDate = p.EntranceDate,
                                  EntranceScore = p.EntranceScore,
                                  PostCode = p.PostCode,
                                  StudentSourceName = p.tbStudentSource == null ? "" : p.tbStudentSource.StudentSourceName
                                  #endregion 
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.StudentEdit = tb;
                    }
                    var honorTemp = (from p in db.Table<Entity.tbStudentHonor>()
                                     where p.tbStudent.Id == id
                                     select new
                                     {
                                         ID = p.Id,
                                         HonorName = p.HonorName,
                                         FileName = p.HonorFile
                                     }).ToList();
                    if (honorTemp != null && honorTemp.Count > 0)
                    {
                        vm.StudentHonorJson = Newtonsoft.Json.JsonConvert.SerializeObject(honorTemp);
                    }

                    var familyTemp = (from p in db.Table<Entity.tbStudentFamily>()
                                      where p.tbStudent.Id == id
                                      select new
                                      {
                                          Id = 0,
                                          FamilyName = p.FamilyName,
                                          Job = p.Job,
                                          Mobile = p.Mobile,
                                          KinshipId = p.tbDictKinship.Id,
                                          UnitName = p.UnitName
                                      }).ToList();
                    if (familyTemp != null && familyTemp.Count > 0)
                    {
                        vm.StudentFamilyJson = Newtonsoft.Json.JsonConvert.SerializeObject(familyTemp);
                    }
                }

                return View(vm);
            }
        }

        public ActionResult Info(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Student.Info2();
                vm.StudentFamilyInfo = new List<Dto.StudentFamily.Info>();
                vm.StudentHonorInfo = new List<Dto.StudentHonor.Info>();

                var student = db.Table<Entity.tbStudent>()
                    .Include(d => d.tbDictBlood)
                    .Include(d => d.tbDictNation)
                    .Include(d => d.tbDictParty)
                    .Include(d => d.tbStudentSession)
                    .Include(d => d.tbStudentStudyType)
                    .Include(d => d.tbStudentType)
                    .Include(d => d.tbSysUser)
                    .Include(d => d.tbStudentSource)
                    .Include(d => d.tbSysUser.tbSex)
                    .Where(d => d.Id == id).FirstOrDefault();

                vm.StudentInfo = new Dto.Student.Info2();
                vm.StudentInfo.Address = student.Address;
                vm.StudentInfo.Birthday = student.Birthday;
                vm.StudentInfo.BloodType = student.tbDictBlood != null ? student.tbDictBlood.BloodName : string.Empty;
                vm.StudentInfo.CardNo = student.CardNo;
                vm.StudentInfo.CMIS = student.CMIS;
                vm.StudentInfo.DictNation = student.tbDictNation != null ? student.tbDictNation.NationName : string.Empty;
                vm.StudentInfo.DictParty = student.tbDictParty != null ? student.tbDictParty.PartyName : string.Empty;
                vm.StudentInfo.Email = student.tbSysUser.Email;
                vm.StudentInfo.Id = student.Id;
                vm.StudentInfo.IdentityNumber = student.tbSysUser.IdentityNumber;
                vm.StudentInfo.LibraryNo = student.LibraryNo;
                vm.StudentInfo.Mobile = student.tbSysUser.Mobile;
                vm.StudentInfo.Photo = student.Photo;
                vm.StudentInfo.PinYin = student.PinYin;
                vm.StudentInfo.Profile = student.Profile;
                vm.StudentInfo.Qq = student.tbSysUser.Qq;
                vm.StudentInfo.Sex = student.tbSysUser.tbSex != null ? student.tbSysUser.tbSex.SexName : string.Empty;
                vm.StudentInfo.StudentCode = student.StudentCode;
                vm.StudentInfo.StudentName = student.StudentName;
                vm.StudentInfo.StudentNameEn = student.StudentNameEn;
                vm.StudentInfo.StudentSession = student.tbStudentSession != null ? student.tbStudentSession.StudentSessionName : string.Empty;
                vm.StudentInfo.StudentStudyType = student.tbStudentStudyType != null ? student.tbStudentStudyType.StudyTypeName : string.Empty;
                vm.StudentInfo.StudentType = student.tbStudentType != null ? student.tbStudentType.StudentTypeName : string.Empty;
                vm.StudentInfo.TicketNumber = student.TicketNumber;
                vm.StudentInfo.EntranceDate = student.EntranceDate;
                vm.StudentInfo.EntranceScore = student.EntranceScore;
                vm.StudentInfo.PostCode = student.PostCode;
                vm.StudentInfo.StudentSourceName = student.tbStudentSource == null ? "" : student.tbStudentSource.StudentSourceName;

                var studentFamilyList = db.Table<Entity.tbStudentFamily>()
                    .Include(d => d.tbDictKinship)
                    .Where(d => d.tbStudent.Id == id).ToList();
                var studentHonorList = db.Table<Entity.tbStudentHonor>()
                    .Include(d => d.tbstudentHonorLevel)
                    .Include(d => d.tbStudentHonorType)
                    .Where(d => d.tbStudent.Id == id).ToList();
                foreach (var v in studentFamilyList)
                {
                    vm.StudentFamilyInfo.Add(new Dto.StudentFamily.Info()
                    {
                        Id = v.Id,
                        FamilyName = v.FamilyName,
                        Job = v.Job,
                        Mobile = v.Mobile,
                        Email = v.Email,
                        KinshipId = v.tbDictKinship.Id,
                        Relation = v.tbDictKinship.KinshipName,
                        UnitName = v.UnitName
                    });
                }
                foreach (var v in studentHonorList)
                {
                    vm.StudentHonorInfo.Add(new Dto.StudentHonor.Info()
                    {
                        Id = v.Id,
                        StudentHonorLevel = v.tbstudentHonorLevel.StudentHonorLevelName,
                        StudentHonorType = v.tbStudentHonorType.StudentHonorTypeName,
                        HonorFile = Url.Content("~/Files/StudentHonor/") + v.HonorFile,
                        HonorName = v.HonorName
                    });
                }

                return View(vm);
            }
        }

        /// <summary>
        /// 学籍表
        /// </summary>
        public ActionResult StudentStatus(int id)
        {
            var vm = new Models.Student.StudentStatus();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Student.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                var file = Request.Files["StudentEdit.Photo"];

                if (file.ContentLength > 0 && Code.Common.GetFileType(file.FileName) != Code.FileType.Image)
                {
                    return Content("<script >alert('图片格式必须是jpg、jpeg、png、bmp格式！');history.go(-1);</script >", "text/html");
                }

                if (db.Table<Entity.tbStudent>().Where(d => d.StudentCode == vm.StudentEdit.StudentCode && d.Id != vm.StudentEdit.Id).Any())
                {
                    //error.AddError("该学号已存在!");
                    return Content("<script >alert('该学号已存在！');history.go(-1);</script >", "text/html");
                }

                var studentHonor = new StudentHonorController();
                var studentFamily = new StudentFamilyController();

                if (vm.StudentEdit.Id == 0)
                {
                    var tb = new Entity.tbStudent();
                    tb.StudentCode = vm.StudentEdit.StudentCode;
                    tb.StudentName = vm.StudentEdit.StudentName;
                    tb.Address = vm.StudentEdit.Address;
                    tb.PinYin = vm.StudentEdit.PinYin;
                    tb.CMIS = vm.StudentEdit.CMIS;
                    tb.EntranceDate = vm.StudentEdit.EntranceDate;
                    tb.EntranceScore = vm.StudentEdit.EntranceScore;
                    tb.StudentNameEn = vm.StudentEdit.StudentNameEn;
                    tb.Profile = vm.StudentEdit.Profile;
                    tb.PostCode = vm.StudentEdit.PostCode;

                    tb.tbSysUser = new Sys.Entity.tbSysUser();
                    tb.tbSysUser.UserCode = vm.StudentEdit.StudentCode;
                    tb.tbSysUser.UserName = vm.StudentEdit.StudentName;
                    tb.tbSysUser.Password = Code.Common.DESEnCode("123456");
                    tb.tbSysUser.PasswordMd5 = Code.Common.CreateMD5Hash("123456");
                    tb.tbSysUser.tbSex = db.Set<Dict.Entity.tbDictSex>().Find(vm.StudentEdit.SexId);
                    tb.tbSysUser.UserType = Code.EnumHelper.SysUserType.Student;
                    tb.tbSysUser.IdentityNumber = vm.StudentEdit.IdentityNumber;
                    tb.tbSysUser.Email = vm.StudentEdit.Email;
                    tb.tbSysUser.Mobile = vm.StudentEdit.Mobile;
                    tb.tbSysUser.Qq = vm.StudentEdit.Qq;

                    tb.tbStudentType = db.Set<Entity.tbStudentType>().Find(vm.StudentEdit.StudentTypeId);
                    tb.tbStudentStudyType = db.Set<Entity.tbStudentStudyType>().Find(vm.StudentEdit.StudentStudyTypeId);
                    tb.tbStudentSession = db.Set<Entity.tbStudentSession>().Find(vm.StudentEdit.StudentSessionId);
                    tb.tbDictBlood = db.Set<Dict.Entity.tbDictBlood>().Find(vm.StudentEdit.BloodTypeId);
                    tb.tbDictNation = db.Set<Dict.Entity.tbDictNation>().Find(vm.StudentEdit.DictNationId);
                    tb.tbDictParty = db.Set<Dict.Entity.tbDictParty>().Find(vm.StudentEdit.DictPartyId);
                    tb.Birthday = vm.StudentEdit.Birthday;

                    if (db.Table<Entity.tbStudentSource>().Where(d => d.StudentSourceName == vm.StudentEdit.StudentSourceName).Count() > 0)
                    {
                        tb.tbStudentSource = db.Table<Entity.tbStudentSource>().Where(d => d.StudentSourceName == vm.StudentEdit.StudentSourceName).FirstOrDefault();
                    }
                    else
                    {
                        tb.tbStudentSource = new Entity.tbStudentSource()
                        {
                            StudentSourceName = vm.StudentEdit.StudentSourceName
                        };
                    }

                    if (file.ContentLength > 0)
                    {
                        var fileSave = Server.MapPath("~/Files/StudentPhoto/");
                        Random r = new Random();
                        var fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + r.Next(10000, 99999).ToString() + "." + file.FileName.Split('.').Last();
                        file.SaveAs(fileSave + fileName);
                        tb.Photo = fileName;
                    }

                    #region 新增家长

                    tb.tbSysUserFamily = new Sys.Entity.tbSysUser()
                    {
                        UserCode = "F" + vm.StudentEdit.StudentCode,
                        UserName = "F" + vm.StudentEdit.StudentCode,
                        UserType = Code.EnumHelper.SysUserType.Family,
                        Password = Code.Common.DESEnCode("123456"),
                        PasswordMd5 = Code.Common.CreateMD5Hash("123456")
                    };

                    //新增家长角色成员
                    var familyUserRole = new Sys.Entity.tbSysUserRole()
                    {
                        tbSysRole = db.Table<Sys.Entity.tbSysRole>().Where(d => d.RoleCode == Code.EnumHelper.SysRoleCode.Family).FirstOrDefault(),
                        tbSysUser = tb.tbSysUserFamily
                    };
                    db.Set<Sys.Entity.tbSysUserRole>().Add(familyUserRole);

                    #endregion

                    db.Set<Entity.tbStudent>().Add(tb);

                    bool b1 = studentHonor.InsertHonor(db, tb, vm.StudentHonorJson);
                    bool b2 = studentFamily.InsertFamily(db, tb, vm.StudentFamilyJson);

                    #region tbSysUserRole:新增角色成员
                    var tbUserRole = new Sys.Entity.tbSysUserRole();
                    tbUserRole.tbSysRole = db.Table<Sys.Entity.tbSysRole>().Where(d => d.RoleCode == Code.EnumHelper.SysRoleCode.Student).FirstOrDefault();
                    tbUserRole.tbSysUser = tb.tbSysUser;
                    db.Set<Sys.Entity.tbSysUserRole>().Add(tbUserRole);
                    #endregion

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加学生");
                    }
                }
                else
                {
                    var tb = (from p in db.Table<Entity.tbStudent>()
                                .Include(d => d.tbDictBlood)
                                .Include(d => d.tbDictNation)
                                .Include(d => d.tbDictParty)
                                .Include(d => d.tbStudentStudyType)
                                .Include(d => d.tbStudentType)
                                .Include(d => d.tbSysUser)
                              where p.Id == vm.StudentEdit.Id
                              select p).FirstOrDefault();

                    if (tb != null)
                    {
                        tb.StudentCode = vm.StudentEdit.StudentCode;
                        tb.StudentName = vm.StudentEdit.StudentName;
                        tb.Address = vm.StudentEdit.Address;
                        tb.PinYin = vm.StudentEdit.PinYin;
                        tb.CMIS = vm.StudentEdit.CMIS;
                        tb.EntranceDate = vm.StudentEdit.EntranceDate;
                        tb.EntranceScore = vm.StudentEdit.EntranceScore;
                        tb.StudentNameEn = vm.StudentEdit.StudentNameEn;
                        tb.Profile = vm.StudentEdit.Profile;
                        tb.PostCode = vm.StudentEdit.PostCode;

                        tb.tbSysUser.UserCode = vm.StudentEdit.StudentCode;
                        tb.tbSysUser.UserName = vm.StudentEdit.StudentName;
                        tb.tbSysUser.tbSex = db.Set<Dict.Entity.tbDictSex>().Find(vm.StudentEdit.SexId);
                        tb.tbSysUser.UserType = Code.EnumHelper.SysUserType.Student;
                        tb.tbSysUser.IdentityNumber = vm.StudentEdit.IdentityNumber;
                        tb.tbSysUser.Email = vm.StudentEdit.Email;
                        tb.tbSysUser.Mobile = vm.StudentEdit.Mobile;
                        tb.tbSysUser.Qq = vm.StudentEdit.Qq;

                        tb.tbStudentType = db.Set<Entity.tbStudentType>().Find(vm.StudentEdit.StudentTypeId);
                        tb.tbStudentStudyType = db.Set<Entity.tbStudentStudyType>().Find(vm.StudentEdit.StudentStudyTypeId);
                        tb.tbStudentSession = db.Set<Entity.tbStudentSession>().Find(vm.StudentEdit.StudentSessionId);
                        tb.tbDictBlood = db.Set<Dict.Entity.tbDictBlood>().Find(vm.StudentEdit.BloodTypeId);
                        tb.tbDictNation = db.Set<Dict.Entity.tbDictNation>().Find(vm.StudentEdit.DictNationId);
                        tb.tbDictParty = db.Set<Dict.Entity.tbDictParty>().Find(vm.StudentEdit.DictPartyId);
                        tb.Birthday = vm.StudentEdit.Birthday;

                        if (db.Table<Entity.tbStudentSource>().Where(d => d.StudentSourceName == vm.StudentEdit.StudentSourceName).Count() > 0)
                        {
                            tb.tbStudentSource = db.Table<Entity.tbStudentSource>().Where(d => d.StudentSourceName == vm.StudentEdit.StudentSourceName).FirstOrDefault();
                        }
                        else
                        {
                            tb.tbStudentSource = new Entity.tbStudentSource()
                            {
                                StudentSourceName = vm.StudentEdit.StudentSourceName
                            };
                        }

                        if (file.ContentLength > 0)
                        {
                            var fileSave = Server.MapPath("~/Files/StudentPhoto/");
                            Random r = new Random();
                            var fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + r.Next(10000, 99999).ToString() + "." + file.FileName.Split('.').Last();
                            file.SaveAs(fileSave + fileName);
                            tb.Photo = fileName;
                        }

                        bool b1 = studentHonor.InsertHonor(db, tb, vm.StudentHonorJson);
                        bool b2 = studentFamily.InsertFamily(db, tb, vm.StudentFamilyJson);

                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改学生");
                        }
                    }
                    else
                    {
                        error.AddError(Resources.LocalizedText.MsgNotFound);
                    }
                }

                return RedirectToAction("List", new { pageIndex = vm.Page.PageIndex, pageSize = vm.Page.PageSize });
            }
        }

        public ActionResult EditStudent(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Student.EditStudent();
                if (Code.Common.UserType == Code.EnumHelper.SysUserType.Student || Code.Common.UserType == Code.EnumHelper.SysUserType.Family)
                {
                    var myStudent = (from p in db.Table<Entity.tbStudent>()
                                     where p.tbSysUser.Id == Code.Common.UserId || p.tbSysUserFamily.Id == Code.Common.UserId
                                     select p).FirstOrDefault();
                    if (myStudent != null)
                    {
                        id = myStudent.Id;
                    }
                }
                if (id > 0)
                {
                    vm.StudentEdit = (from p in db.Table<Entity.tbStudent>()
                                      where p.Id == id
                                      select new Dto.Student.EditStudent()
                                      {
                                          #region
                                          CardNo = p.CardNo,
                                          CMIS = p.CMIS,
                                          EntranceDate = p.EntranceDate,
                                          EntranceScore = p.EntranceScore,
                                          Id = p.Id,
                                          IdentityNumber = p.tbSysUser.IdentityNumber,
                                          LibraryNo = p.LibraryNo,
                                          Photo = p.Photo,
                                          PinYin = p.PinYin,
                                          Profile = p.Profile,
                                          SexId = p.tbSysUser.tbSex.Id,
                                          StudentCode = p.StudentCode,
                                          StudentName = p.StudentName,
                                          StudentNameEn = p.StudentNameEn,
                                          StudentSessionId = p.tbStudentSession.Id,
                                          StudentSourceName = p.tbStudentSource.StudentSourceName,
                                          StudentStudyTypeId = p.tbStudentStudyType.Id,
                                          StudentTypeId = p.tbStudentType.Id,
                                          TicketNumber = p.TicketNumber,
                                          EduNo = p.EduNo
                                          #endregion
                                      }).FirstOrDefault();
                }
                vm.SexList = Dict.Controllers.DictSexController.SelectList();
                vm.StudentSessionList = StudentSessionController.SelectList();
                vm.StudentStudyTypeList = StudentStudyTypeController.SelectList();
                vm.StudentTypeList = StudentTypeController.SelectList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudent(Models.Student.EditStudent vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                var file = Request.Files["StudentEdit.Photo"];

                if (file.ContentLength > 0 && Code.Common.GetFileType(file.FileName) != Code.FileType.Image)
                {
                    return Content("<script >alert('图片格式必须是jpg、jpeg、png、bmp格式！');history.go(-1);</script >", "text/html");
                }

                #region 验证学号重复
                if (db.Table<Entity.tbStudent>().Where(d => d.StudentCode == vm.StudentEdit.StudentCode && d.Id != vm.StudentEdit.Id).Any())
                {
                    return Content("<script >alert('该学号已存在！');history.go(-1);</script >", "text/html");
                }

                if (!string.IsNullOrEmpty(vm.StudentEdit.EduNo))
                {
                    if (db.Table<Entity.tbStudent>().Where(d => d.EduNo == vm.StudentEdit.EduNo && d.Id != vm.StudentEdit.Id).Any())
                    {
                        return Content("<script >alert('该学生教育编号已存在！');history.go(-1);</script >", "text/html");
                    }
                }

                #endregion

                var tb = new Entity.tbStudent();
                if (vm.StudentEdit.Id > 0)
                {
                    #region 修改
                    tb = db.Table<Entity.tbStudent>().Where(d => d.Id == vm.StudentEdit.Id)
                        .Include(d => d.tbStudentSession)
                        .Include(d => d.tbStudentSource)
                        .Include(d => d.tbStudentStudyType)
                        .Include(d => d.tbStudentType)
                        .Include(d => d.tbSysUser)
                        .Include(d => d.tbSysUser.tbSex).FirstOrDefault();

                    tb.StudentCode = vm.StudentEdit.StudentCode;
                    tb.StudentName = vm.StudentEdit.StudentName;
                    tb.tbSysUser.UserCode = vm.StudentEdit.StudentCode;
                    tb.tbSysUser.UserName = vm.StudentEdit.StudentName;
                    tb.StudentNameEn = vm.StudentEdit.StudentNameEn;
                    tb.TicketNumber = vm.StudentEdit.TicketNumber;
                    tb.Profile = vm.StudentEdit.Profile;
                    tb.PinYin = vm.StudentEdit.PinYin;
                    tb.CMIS = vm.StudentEdit.CMIS;
                    tb.EntranceScore = vm.StudentEdit.EntranceScore;
                    tb.EntranceDate = vm.StudentEdit.EntranceDate;
                    tb.tbSysUser.IdentityNumber = vm.StudentEdit.IdentityNumber;
                    tb.EduNo = vm.StudentEdit.EduNo;

                    if (vm.StudentEdit.SexId > 0)
                    {
                        tb.tbSysUser.tbSex = db.Set<Dict.Entity.tbDictSex>().Find(vm.StudentEdit.SexId);
                    }
                    if (vm.StudentEdit.StudentTypeId > 0)
                    {
                        tb.tbStudentType = db.Set<Entity.tbStudentType>().Find(vm.StudentEdit.StudentTypeId);
                    }
                    if (vm.StudentEdit.StudentStudyTypeId > 0)
                    {
                        tb.tbStudentStudyType = db.Set<Entity.tbStudentStudyType>().Find(vm.StudentEdit.StudentStudyTypeId);
                    }
                    if (vm.StudentEdit.StudentSessionId > 0)
                    {
                        tb.tbStudentSession = db.Set<Entity.tbStudentSession>().Find(vm.StudentEdit.StudentSessionId);
                    }
                    if (!string.IsNullOrEmpty(vm.StudentEdit.StudentSourceName))
                    {
                        if (db.Table<Entity.tbStudentSource>().Where(d => d.StudentSourceName == vm.StudentEdit.StudentSourceName).Count() > 0)
                        {
                            tb.tbStudentSource = db.Table<Entity.tbStudentSource>().Where(d => d.StudentSourceName == vm.StudentEdit.StudentSourceName).FirstOrDefault();
                        }
                        else
                        {
                            tb.tbStudentSource = new Entity.tbStudentSource()
                            {
                                StudentSourceName = vm.StudentEdit.StudentSourceName
                            };
                        }
                    }
                    #endregion
                }
                else
                {


                    #region 添加
                    tb = new Entity.tbStudent()
                    {
                        StudentCode = vm.StudentEdit.StudentCode,
                        StudentName = vm.StudentEdit.StudentName,
                        StudentNameEn = vm.StudentEdit.StudentNameEn,
                        TicketNumber = vm.StudentEdit.TicketNumber,
                        Profile = vm.StudentEdit.Profile,
                        PinYin = vm.StudentEdit.PinYin,
                        CMIS = vm.StudentEdit.CMIS,
                        EntranceScore = vm.StudentEdit.EntranceScore,
                        EntranceDate = vm.StudentEdit.EntranceDate,
                        EduNo = vm.StudentEdit.EduNo,
                        tbSysUser = new Sys.Entity.tbSysUser()
                        {
                            UserCode = vm.StudentEdit.StudentCode,
                            UserName = vm.StudentEdit.StudentName,
                            Password = Code.Common.DESEnCode("123456"),
                            PasswordMd5 = Code.Common.CreateMD5Hash("123456"),
                            IdentityNumber = vm.StudentEdit.IdentityNumber,
                            UserType = Code.EnumHelper.SysUserType.Student
                        },
                    };
                    if (vm.StudentEdit.SexId > 0)
                    {
                        tb.tbSysUser.tbSex = db.Set<Dict.Entity.tbDictSex>().Find(vm.StudentEdit.SexId);
                    }
                    if (vm.StudentEdit.StudentTypeId > 0)
                    {
                        tb.tbStudentType = db.Set<Entity.tbStudentType>().Find(vm.StudentEdit.StudentTypeId);
                    }
                    if (vm.StudentEdit.StudentStudyTypeId > 0)
                    {
                        tb.tbStudentStudyType = db.Set<Entity.tbStudentStudyType>().Find(vm.StudentEdit.StudentStudyTypeId);
                    }
                    if (vm.StudentEdit.StudentSessionId > 0)
                    {
                        tb.tbStudentSession = db.Set<Entity.tbStudentSession>().Find(vm.StudentEdit.StudentSessionId);
                    }
                    if (!string.IsNullOrEmpty(vm.StudentEdit.StudentSourceName))
                    {
                        if (db.Table<Entity.tbStudentSource>().Where(d => d.StudentSourceName == vm.StudentEdit.StudentSourceName).Count() > 0)
                        {
                            tb.tbStudentSource = db.Table<Entity.tbStudentSource>().Where(d => d.StudentSourceName == vm.StudentEdit.StudentSourceName).FirstOrDefault();
                        }
                        else
                        {
                            tb.tbStudentSource = new Entity.tbStudentSource()
                            {
                                StudentSourceName = vm.StudentEdit.StudentSourceName
                            };
                        }
                    }

                    #region 新增家长

                    tb.tbSysUserFamily = new Sys.Entity.tbSysUser()
                    {
                        UserCode = "F" + vm.StudentEdit.StudentCode,
                        UserName = "F" + vm.StudentEdit.StudentCode,
                        UserType = Code.EnumHelper.SysUserType.Family,
                        Password = Code.Common.DESEnCode("123456"),
                        PasswordMd5 = Code.Common.CreateMD5Hash("123456")
                    };

                    //新增家长角色成员
                    var familyUserRole = new Sys.Entity.tbSysUserRole()
                    {
                        tbSysRole = db.Table<Sys.Entity.tbSysRole>().Where(d => d.RoleCode == Code.EnumHelper.SysRoleCode.Family).FirstOrDefault(),
                        tbSysUser = tb.tbSysUserFamily
                    };
                    db.Set<Sys.Entity.tbSysUserRole>().Add(familyUserRole);

                    #endregion

                    db.Set<Entity.tbStudent>().Add(tb);

                    #region tbSysUserRole:新增角色成员
                    var tbUserRole = new Sys.Entity.tbSysUserRole()
                    {
                        tbSysUser = tb.tbSysUser,
                        tbSysRole = db.Table<Sys.Entity.tbSysRole>().Where(d => d.RoleCode == Code.EnumHelper.SysRoleCode.Student).FirstOrDefault()
                    };
                    db.Set<Sys.Entity.tbSysUserRole>().Add(tbUserRole);
                    #endregion
                    #endregion
                }

                if (file.ContentLength > 0)
                {
                    var fileSave = Server.MapPath("~/Files/StudentPhoto/");
                    Random r = new Random();
                    var fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + r.Next(10000, 99999).ToString() + "." + file.FileName.Split('.').Last();
                    file.SaveAs(fileSave + fileName);
                    tb.Photo = fileName;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加/修改学生信息");
                }
                if (!string.IsNullOrEmpty(vm.Step))
                {
                    return Content(Code.Common.Redirect(Url.Action("EditStudentContact", new { id = tb.Id })));
                }
                else
                {
                    return Content(Code.Common.Redirect(Url.Action("List")));
                }
            }
        }

        public ActionResult EditStudentContact(int id)
        {
            var vm = new Models.Student.EditStudentContact();

            using (var db = new XkSystem.Models.DbContext())
            {
                vm.StudentContact = (from p in db.Table<Entity.tbStudent>()
                                     where p.Id == id
                                     select new Dto.Student.EditStudentContact()
                                     {
                                         Id = p.Id,
                                         Address = p.Address,
                                         PostCode = p.PostCode,
                                         Email = p.tbSysUser.Email,
                                         Mobile = p.tbSysUser.Mobile,
                                         Qq = p.tbSysUser.Qq
                                     }).FirstOrDefault();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudentContact(Models.Student.EditStudentContact vm)
        {
            var tb = new Entity.tbStudent();
            using (var db = new XkSystem.Models.DbContext())
            {
                tb = db.Table<Entity.tbStudent>()
                   .Include(d => d.tbSysUser).Where(d => d.Id == vm.StudentContact.Id).FirstOrDefault();
                tb.Address = vm.StudentContact.Address;
                tb.PostCode = vm.StudentContact.PostCode;
                tb.tbSysUser.Email = vm.StudentContact.Email;
                tb.tbSysUser.Mobile = vm.StudentContact.Mobile;
                tb.tbSysUser.Qq = vm.StudentContact.Qq;
                db.SaveChanges();
            }

            if (!string.IsNullOrEmpty(vm.Step))
            {
                return Code.MvcHelper.Post(null, Url.Action("EditStudentExtend", new { id = tb.Id }));
            }
            else
            {
                return Code.MvcHelper.Post(null, Url.Action("List"));
            }
        }

        public ActionResult EditStudentExtend(int id)
        {
            var vm = new Models.Student.EditStudentExtend();

            using (var db = new XkSystem.Models.DbContext())
            {
                vm.StudentExtend = (from p in db.Table<Entity.tbStudent>()
                                    where p.Id == id
                                    select new Dto.Student.EditStudentExtend()
                                    {
                                        Id = p.Id,
                                        Birthday = p.Birthday,
                                        BloodTypeId = p.tbDictBlood.Id,
                                        DictNationId = p.tbDictNation.Id,
                                        DictPartyId = p.tbDictParty.Id,
                                        ClassId = p.tbClass.Id,

                                        BirthPlace = p.BirthPlace,
                                        SchoolDistance = p.SchoolDistance,
                                        HouseholdRegister = p.HouseholdRegister,
                                        IsOnlyChild = p.IsOnlyChild,
                                        IsSuiqian = p.IsSuiqian,
                                        NativePlace = p.NativePlace,
                                        SchoolTransportationType = p.SchoolTransportationType
                                    }).FirstOrDefault();
                var classStudent = db.Table<Basis.Entity.tbClassStudent>()
                    .Where(d => d.tbStudent.Id == vm.StudentExtend.Id).Include(d => d.tbClass).ToList();
                if (classStudent.Count > 0)
                {
                    vm.StudentExtend.ClassId = classStudent.FirstOrDefault().tbClass.Id;
                }
            }

            vm.DictBloodTypeList = Dict.Controllers.DictBloodController.SelectList();
            vm.DictNationList = Dict.Controllers.DictNationController.SelectList();
            vm.DictPartyList = Dict.Controllers.DictPartyController.SelectList();
            vm.ClassList = Basis.Controllers.ClassController.SelectList();
            vm.SchoolTransportationTypeList = typeof(Code.EnumHelper.SchoolTransportationType).ToItemList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudentExtend(Models.Student.EditStudentExtend vm)
        {
            var tb = new Entity.tbStudent();
            using (var db = new XkSystem.Models.DbContext())
            {
                tb = db.Table<Entity.tbStudent>().Where(d => d.Id == vm.StudentExtend.Id).FirstOrDefault();
                tb.tbDictBlood = db.Set<Dict.Entity.tbDictBlood>().Find(vm.StudentExtend.BloodTypeId);
                tb.tbDictNation = db.Set<Dict.Entity.tbDictNation>().Find(vm.StudentExtend.DictNationId);
                tb.tbDictParty = db.Set<Dict.Entity.tbDictParty>().Find(vm.StudentExtend.DictPartyId);
                tb.tbClass = db.Set<Basis.Entity.tbClass>().Find(vm.StudentExtend.ClassId);
                tb.Birthday = vm.StudentExtend.Birthday;
                tb.BirthPlace = vm.StudentExtend.BirthPlace;
                tb.SchoolDistance = vm.StudentExtend.SchoolDistance;
                tb.HouseholdRegister = vm.StudentExtend.HouseholdRegister;
                tb.IsOnlyChild = vm.StudentExtend.IsOnlyChild;
                tb.IsSuiqian = vm.StudentExtend.IsSuiqian;
                tb.NativePlace = vm.StudentExtend.NativePlace;
                tb.SchoolTransportationType = vm.StudentExtend.SchoolTransportationType;
                db.SaveChanges();
            }

            if (!string.IsNullOrEmpty(vm.Step))
            {
                return Code.MvcHelper.Post(null, Url.Action("FamilyList", "StudentFamily", new { id = tb.Id }));
            }
            else
            {
                return Code.MvcHelper.Post(null, Url.Action("List"));
            }
        }

        public ActionResult Import()
        {
            var vm = new Models.Student.Import();
            vm.ImportList = new List<Dto.Student.Import>();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.Student.Import vm)
        {
            vm.ImportList = new List<Dto.Student.Import>();
            if (ModelState.IsValid)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);

                using (var db = new XkSystem.Models.DbContext())
                {
                    if (Code.Common.GetFileType(file.FileName) != Code.FileType.Excel)
                    {
                        ModelState.AddModelError("", "上传的文件不是正确的EXCLE文件!");
                        return View(vm);
                    }
                    var dt = Code.NpoiHelper.ExcelToDataTable(fileSave, System.IO.Path.GetExtension(file.FileName), string.Empty);
                    if (dt == null)
                    {
                        //error.AddError("无法读取上传的文件，请检查文件格式是否正确!");
                        ModelState.AddModelError("", "无法读取上传的文件，请检查文件格式是否正确!");
                        return View(vm);
                    }
                    var tbList = new List<string>() { "学生学号", "学生姓名", "姓名拼音", "英文名", "CMIS", "性别", "学届", "学生类型", "就读方式", "手机号码", "家庭住址", "身份证号", "邮箱", "QQ", "血型", "民族", "政治面貌", "出生日期", "入学成绩", "入学时间", "毕业院校", "父亲姓名", "父亲单位", "父亲职务", "父亲手机", "父亲邮箱", "母亲姓名", "母亲单位", "母亲职务", "母亲手机", "母亲邮箱" };

                    var Text = string.Empty;
                    foreach (var a in tbList)
                    {
                        if (!dt.Columns.Contains(a.ToString()))
                        {
                            Text += a + ",";
                        }
                    }

                    if (!string.IsNullOrEmpty(Text))
                    {
                        ModelState.AddModelError("", "上传的EXCEL内容与预期不一致，缺少对应的字段：" + Text);
                        return View(vm);
                    }

                    //将DataTable转为List
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        //if (string.IsNullOrEmpty(dr["学生学号"].ToString()) && string.IsNullOrEmpty(dr["学生姓名"].ToString()))
                        //{
                        //    continue;
                        //}

                        Dto.Student.Import import = new Dto.Student.Import()
                        {
                            StudentCode = dr["学生学号"] != null ? dr["学生学号"].ToString().Trim() : "",
                            StudentName = dr["学生姓名"] != null ? dr["学生姓名"].ToString().Trim() : "",
                            PinYin = dr["姓名拼音"] != null ? dr["姓名拼音"].ToString().Trim() : "",
                            CMIS = dr["CMIS"] != null ? dr["CMIS"].ToString().Trim() : "",
                            SexName = dr["性别"] != null ? dr["性别"].ToString().Trim() : "",
                            StudentSessionName = dr["学届"] != null ? dr["学届"].ToString().Trim() : "",
                            StudentTypeName = dr["学生类型"] != null ? dr["学生类型"].ToString().Trim() : "",
                            StudyTypeName = dr["就读方式"] != null ? dr["就读方式"].ToString().Trim() : "",
                            Mobile = dr["手机号码"] != null ? dr["手机号码"].ToString().Trim() : "",
                            Address = dr["家庭住址"] != null ? dr["家庭住址"].ToString().Trim() : "",
                            IdentityNumber = dr["身份证号"] != null ? dr["身份证号"].ToString().Trim() : "",
                            Email = dr["邮箱"] != null ? dr["邮箱"].ToString().Trim() : "",
                            Qq = dr["QQ"] != null ? dr["QQ"].ToString().Trim() : "",
                            BloodName = dr["血型"] != null ? dr["血型"].ToString().Trim() : "",
                            NationName = dr["民族"] != null ? dr["民族"].ToString().Trim() : "",
                            PartyName = dr["政治面貌"] != null ? dr["政治面貌"].ToString().Trim() : "",
                            StudentNameEn = dr["英文名"].ConvertToString(),

                            FatherName = dr["父亲姓名"] != null ? dr["父亲姓名"].ToString().Trim() : "",
                            FatherCompany = dr["父亲单位"] != null ? dr["父亲单位"].ToString().Trim() : "",
                            FatherJob = dr["父亲职务"] != null ? dr["父亲职务"].ToString().Trim() : "",
                            FatherPhone = dr["父亲手机"] != null ? dr["父亲手机"].ToString().Trim() : "",
                            MotherName = dr["母亲姓名"] != null ? dr["母亲姓名"].ToString().Trim() : "",
                            MotherCompany = dr["母亲单位"] != null ? dr["母亲单位"].ToString().Trim() : "",
                            MotherJob = dr["母亲职务"] != null ? dr["母亲职务"].ToString().Trim() : "",
                            MotherPhone = dr["母亲手机"] != null ? dr["母亲手机"].ToString().Trim() : "",
                            StudentSourceName = dr["毕业院校"].ConvertToString(),
                            FatherEmail = dr["父亲邮箱"].ConvertToString(),
                            MotherEmail = dr["母亲邮箱"].ConvertToString()
                        };

                        DateTime timeTemp = new DateTime();
                        if (!string.IsNullOrEmpty(Convert.ToString(dr["出生日期"])))
                        {
                            if (!DateTime.TryParse(dr["出生日期"].ToString().Trim(), out timeTemp))
                            {
                                import.Error = import.Error + "出生日期格式错误，必须为时间格式或不填！";
                            }
                            else
                            {
                                import.Birthday = timeTemp;
                            }
                        }
                        //import.Birthday = dr["出生日期"] != null && dr["出生日期"].ToString().Trim() != "" ? Convert.ToDateTime(dr["出生日期"].ToString().Trim()) : Convert.ToDateTime("1970-01-01");
                        if (!string.IsNullOrEmpty(dr["入学时间"].ConvertToString()))
                        {
                            if (!DateTime.TryParse(dr["入学时间"].ConvertToString(), out timeTemp))
                            {
                                import.Error = import.Error + "入学时间格式错误，必须为时间格式或不填！";
                            }
                            else
                            {
                                import.EntranceDate = timeTemp;
                            }
                        }
                        decimal d = 0;
                        if (!string.IsNullOrEmpty(dr["入学成绩"].ConvertToString()))
                        {
                            if (!decimal.TryParse(dr["入学成绩"].ConvertToString(), out d))
                            {
                                import.Error = import.Error + "入学成绩格式错误，必须为数字格式或不填！";
                            }
                            else
                            {
                                import.EntranceScore = d;
                            }
                        }


                        vm.ImportList.Add(import);
                    }

                    vm.ImportList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.StudentCode) &&
                        string.IsNullOrEmpty(d.StudentName) &&
                        string.IsNullOrEmpty(d.PinYin) &&
                        string.IsNullOrEmpty(d.CMIS) &&
                        string.IsNullOrEmpty(d.SexName) &&
                        string.IsNullOrEmpty(d.StudentSessionName) &&
                        string.IsNullOrEmpty(d.StudentTypeName) &&
                        string.IsNullOrEmpty(d.StudyTypeName) &&
                        string.IsNullOrEmpty(d.Mobile) &&
                        string.IsNullOrEmpty(d.Address) &&
                        string.IsNullOrEmpty(d.IdentityNumber) &&
                        string.IsNullOrEmpty(d.Qq) &&
                        string.IsNullOrEmpty(d.BloodName) &&
                        string.IsNullOrEmpty(d.NationName) &&
                        string.IsNullOrEmpty(d.PartyName) &&
                        string.IsNullOrEmpty(d.StudentSourceName) &&
                        d.Birthday == null &&
                        string.IsNullOrEmpty(d.FatherName) &&
                        string.IsNullOrEmpty(d.FatherCompany) &&
                        string.IsNullOrEmpty(d.FatherJob) &&
                        string.IsNullOrEmpty(d.FatherPhone) &&
                        string.IsNullOrEmpty(d.MotherName) &&
                        string.IsNullOrEmpty(d.MotherCompany) &&
                        string.IsNullOrEmpty(d.MotherJob) &&
                        string.IsNullOrEmpty(d.MotherPhone)
                    );

                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }

                    var studentList = (from p in db.Table<Entity.tbStudent>()
                                       select p).ToList();

                    var studentFamily = (from p in db.Table<Entity.tbStudentFamily>()
                                            .Include(d => d.tbStudent)
                                            .Include(d => d.tbDictKinship)
                                         select p).ToList();

                    var sessionList = (from p in db.Table<Entity.tbStudentSession>()
                                       select p).ToList();

                    var sexList = (from p in db.Table<Dict.Entity.tbDictSex>()
                                   select p).ToList();

                    var bloodList = (from p in db.Table<Dict.Entity.tbDictBlood>()
                                     select p).ToList();

                    var nationList = (from p in db.Table<Dict.Entity.tbDictNation>()
                                      select p).ToList();

                    var partyList = (from p in db.Table<Dict.Entity.tbDictParty>()
                                     select p).ToList();

                    var studentTypeList = (from p in db.Table<Entity.tbStudentType>()
                                           select p).ToList();

                    var studyTypeList = (from p in db.Table<Entity.tbStudentStudyType>()
                                         select p).ToList();
                    var studentSourceList = db.Table<Entity.tbStudentSource>().ToList();

                    //验证数据格式是否正确
                    foreach (var item in vm.ImportList)
                    {
                        if (!string.IsNullOrEmpty(item.FatherEmail))
                        {
                            Regex reg = new Regex(@"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$");
                            if (!reg.IsMatch(item.FatherEmail))
                            {
                                item.Error += "父亲邮箱格式不正确；";
                            }
                        }
                        if (!string.IsNullOrEmpty(item.MotherEmail))
                        {
                            Regex reg = new Regex(@"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$");
                            if (!reg.IsMatch(item.MotherEmail))
                            {
                                item.Error += "母亲邮箱格式不正确；";
                            }
                        }
                        if (string.IsNullOrEmpty(item.StudentName))
                        {
                            item.Error = item.Error + "学生姓名不能为空!";
                        }

                        if (string.IsNullOrEmpty(item.StudentCode))
                        {
                            item.Error = item.Error + "学生学号不能为空!";
                        }

                        if (string.IsNullOrEmpty(item.StudentSessionName) == false && sessionList.Where(d => d.StudentSessionName == item.StudentSessionName).Count() == 0)
                        {
                            item.Error = item.Error + "学届不存在!";
                        }

                        if (vm.ImportList.Where(d => d.StudentCode == item.StudentCode).Count() > 1)
                        {
                            item.Error = item.Error + "该条数据重复!";
                        }

                        if (string.IsNullOrEmpty(item.SexName) == false && sexList.Where(d => d.SexName == item.SexName).Count() == 0)
                        {
                            item.Error = item.Error + "性别格式不正确!";
                        }

                        if (string.IsNullOrEmpty(item.StudentTypeName) == false && studentTypeList.Where(d => d.StudentTypeName == item.StudentTypeName).Count() == 0)
                        {
                            item.Error = item.Error + "学生类型不正确!";
                        }

                        if (string.IsNullOrEmpty(item.StudyTypeName) == false && studyTypeList.Where(d => d.StudyTypeName == item.StudyTypeName).Count() == 0)
                        {
                            item.Error = item.Error + "就读方式不正确!";
                        }

                        if (string.IsNullOrEmpty(item.BloodName) == false && bloodList.Where(d => d.BloodName == item.BloodName).Count() == 0)
                        {
                            item.Error = item.Error + "血型格式不正确!";
                        }

                        if (string.IsNullOrEmpty(item.NationName) == false && nationList.Where(d => d.NationName == item.NationName).Count() == 0)
                        {
                            item.Error = item.Error + "民族格式不正确!";
                        }

                        if (string.IsNullOrEmpty(item.PartyName) == false && partyList.Where(d => d.PartyName == item.PartyName).Count() == 0)
                        {
                            item.Error = item.Error + "政治面貌格式不正确!";
                        }

                        if (string.IsNullOrEmpty(item.Mobile) == false)
                        {
                            if (new Regex(Code.Common.RegTel).IsMatch(item.Mobile) == false
                                && new Regex(Code.Common.RegMobil).IsMatch(item.Mobile) == false)
                            {
                                item.Error = item.Error + "手机号码格式不正确！";
                            }
                        }

                        if (string.IsNullOrEmpty(item.Email) == false && !(new Regex(@"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$").IsMatch(item.Email)))
                        {
                            item.Error = item.Error + "邮箱格式不正确！";
                        }

                        if (vm.IsUpdate == false && studentList.Where(d => d.StudentCode == item.StudentCode).Count() > 0)
                        {
                            item.Error = item.Error + "系统中已存在该记录!";
                        }
                    }

                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }

                    //数据导入，判断关键字，存在的数据做更新，不存在的做新增
                    var myStudentList = new List<Entity.tbStudent>();
                    var myUserRoleList = new List<Sys.Entity.tbSysUserRole>();
                    var myStudentFamilyList = new List<Entity.tbStudentFamily>();
                    var studentRole = db.Table<Sys.Entity.tbSysRole>().Where(d => d.RoleCode == Code.EnumHelper.SysRoleCode.Student).FirstOrDefault();
                    foreach (var import in vm.ImportList)
                    {
                        if (vm.IsUpdate && studentList.Where(d => d.StudentCode == import.StudentCode).Count() > 0)
                        {
                            #region 修改学生表

                            if (studentList.Where(d => d.StudentCode == import.StudentCode).Count() > 1)
                            {
                                import.Error = "系统中该学生数据存在重复，无法确认需要更新的记录!";
                                vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                                return View(vm);
                            }

                            var tb = (from p in db.Table<Entity.tbStudent>()
                                        .Include(d => d.tbDictBlood)
                                        .Include(d => d.tbDictNation)
                                        .Include(d => d.tbDictParty)
                                        .Include(d => d.tbStudentStudyType)
                                        .Include(d => d.tbStudentType)
                                        .Include(d => d.tbSysUser)
                                      where p.StudentCode == import.StudentCode
                                      select p).FirstOrDefault();

                            tb.StudentName = import.StudentName;
                            tb.Address = import.Address;
                            tb.PinYin = import.PinYin;
                            tb.CMIS = import.CMIS;

                            tb.tbSysUser.UserName = import.StudentName;

                            tb.tbSysUser.tbSex = sexList.Where(d => d.SexName == import.SexName).FirstOrDefault();
                            tb.tbSysUser.IdentityNumber = import.IdentityNumber;
                            tb.tbSysUser.Email = import.Email;
                            tb.tbSysUser.Mobile = import.Mobile;
                            tb.tbSysUser.Qq = import.Qq;

                            tb.tbStudentSession = sessionList.Where(d => d.StudentSessionName == import.StudentSessionName).FirstOrDefault();
                            tb.tbStudentType = studentTypeList.Where(d => d.StudentTypeName == import.StudentTypeName).FirstOrDefault();
                            tb.tbStudentStudyType = studyTypeList.Where(d => d.StudyTypeName == import.StudyTypeName).FirstOrDefault();
                            tb.tbDictBlood = bloodList.Where(d => d.BloodName == import.BloodName).FirstOrDefault();
                            tb.tbDictNation = nationList.Where(d => d.NationName == import.NationName).FirstOrDefault();
                            tb.tbDictParty = partyList.Where(d => d.PartyName == import.PartyName).FirstOrDefault();

                            tb.Birthday = import.Birthday;
                            tb.EntranceDate = import.EntranceDate != null ? (DateTime)import.EntranceDate : new DateTime(DateTime.Now.Year, 9, 1);
                            tb.EntranceScore = import.EntranceScore;
                            tb.StudentNameEn = import.StudentNameEn;

                            if (!string.IsNullOrEmpty(import.StudentSourceName))
                            {
                                if (studentSourceList.Where(d => d.StudentSourceName == import.StudentSourceName).Count() > 0)
                                {
                                    tb.tbStudentSource = studentSourceList.Where(d => d.StudentSourceName == import.StudentSourceName).FirstOrDefault();
                                }
                                else
                                {
                                    tb.tbStudentSource = new Entity.tbStudentSource()
                                    {
                                        StudentSourceName = import.StudentSourceName
                                    };
                                }
                            }

                            var tbF1 = (from p in studentFamily
                                        where p.tbStudent.Id == tb.Id && p.tbDictKinship.KinshipName.Contains("父亲")
                                        select p).FirstOrDefault();

                            if (tbF1 == null)
                            {
                                tbF1 = new Entity.tbStudentFamily();
                                tbF1.tbStudent = tb;
                                tbF1.Email = import.FatherEmail;
                                tbF1.FamilyName = import.FatherName;
                                tbF1.tbDictKinship = db.Table<Dict.Entity.tbDictKinship>().Where(d => d.KinshipName.Contains("父亲")).FirstOrDefault();
                                tbF1.UnitName = import.FatherCompany;
                                tbF1.Job = import.FatherJob;
                                tbF1.Mobile = import.FatherPhone;
                                myStudentFamilyList.Add(tbF1);
                            }
                            else
                            {
                                tbF1.FamilyName = import.FatherName;
                                tbF1.Email = import.FatherEmail;
                                tbF1.UnitName = import.FatherCompany;
                                tbF1.Job = import.FatherJob;
                                tbF1.Mobile = import.FatherPhone;
                            }

                            var tbF2 = (from p in studentFamily
                                        where p.tbStudent.Id == tb.Id && p.tbDictKinship.KinshipName.Contains("母")
                                        select p).FirstOrDefault();
                            if (tbF2 == null)
                            {
                                tbF2 = new Entity.tbStudentFamily();
                                tbF2.tbStudent = tb;
                                tbF2.Email = import.MotherEmail;
                                tbF2.FamilyName = import.MotherName;
                                tbF2.tbDictKinship = db.Table<Dict.Entity.tbDictKinship>().Where(d => d.KinshipName.Contains("母亲")).FirstOrDefault();
                                tbF2.UnitName = import.MotherCompany;
                                tbF2.Job = import.MotherJob;
                                tbF2.Mobile = import.MotherPhone;
                                myStudentFamilyList.Add(tbF2);
                            }
                            else
                            {
                                tbF2.FamilyName = import.MotherName;
                                tbF2.Email = import.MotherEmail;
                                tbF2.UnitName = import.MotherCompany;
                                tbF2.Job = import.MotherJob;
                                tbF2.Mobile = import.MotherPhone;
                            }

                            #endregion
                        }
                        else
                        {
                            #region 添加学生表

                            var tb = new Entity.tbStudent();
                            tb.StudentCode = import.StudentCode;
                            tb.StudentName = import.StudentName;
                            tb.Address = import.Address;
                            tb.PinYin = import.PinYin;
                            tb.CMIS = import.CMIS;
                            tb.StudentNameEn = import.StudentNameEn;

                            tb.tbSysUser = new Sys.Entity.tbSysUser();
                            tb.tbSysUser.UserCode = import.StudentCode;
                            tb.tbSysUser.UserName = import.StudentName;
                            tb.tbSysUser.Password = Code.Common.DESEnCode("123456");
                            tb.tbSysUser.PasswordMd5 = Code.Common.CreateMD5Hash("123456");

                            tb.tbSysUser.tbSex = sexList.Where(d => d.SexName == import.SexName).FirstOrDefault();
                            tb.tbSysUser.UserType = Code.EnumHelper.SysUserType.Student;
                            tb.tbSysUser.IdentityNumber = import.IdentityNumber;
                            tb.tbSysUser.Email = import.Email;
                            tb.tbSysUser.Mobile = import.Mobile;
                            tb.tbSysUser.Qq = import.Qq;

                            if (sessionList.Where(d => d.StudentSessionName == import.StudentSessionName).FirstOrDefault() != null)
                            {
                                tb.tbStudentSession = sessionList.Where(d => d.StudentSessionName == import.StudentSessionName).FirstOrDefault();
                            }
                            else
                            {
                                tb.tbStudentSession = sessionList.OrderByDescending(d => d.No).FirstOrDefault();
                            }
                            tb.tbStudentType = studentTypeList.Where(d => d.StudentTypeName == import.StudentTypeName).FirstOrDefault();
                            tb.tbStudentStudyType = studyTypeList.Where(d => d.StudyTypeName == import.StudyTypeName).FirstOrDefault();
                            tb.tbDictBlood = bloodList.Where(d => d.BloodName == import.BloodName).FirstOrDefault();
                            tb.tbDictNation = nationList.Where(d => d.NationName == import.NationName).FirstOrDefault();
                            tb.tbDictParty = partyList.Where(d => d.PartyName == import.PartyName).FirstOrDefault();

                            tb.Birthday = import.Birthday;
                            tb.EntranceDate = import.EntranceDate != null ? (DateTime)import.EntranceDate : new DateTime(DateTime.Now.Year, 9, 1);
                            tb.EntranceScore = import.EntranceScore;
                            tb.Photo = "";//System.IO.Path.GetTempFileName();

                            if (!string.IsNullOrEmpty(import.StudentSourceName))
                            {
                                if (studentSourceList.Where(d => d.StudentSourceName == import.StudentSourceName).Count() > 0)
                                {
                                    tb.tbStudentSource = studentSourceList.Where(d => d.StudentSourceName == import.StudentSourceName).FirstOrDefault();
                                }
                                else
                                {
                                    tb.tbStudentSource = new Entity.tbStudentSource()
                                    {
                                        StudentSourceName = import.StudentSourceName
                                    };
                                }
                            }

                            #region 新增家长

                            tb.tbSysUserFamily = new Sys.Entity.tbSysUser()
                            {
                                UserCode = "F" + import.StudentCode,
                                UserName = "F" + import.StudentCode,
                                UserType = Code.EnumHelper.SysUserType.Family,
                                Password = Code.Common.DESEnCode("123456"),
                                PasswordMd5 = Code.Common.CreateMD5Hash("123456")
                            };

                            //新增家长角色成员
                            var familyUserRole = new Sys.Entity.tbSysUserRole()
                            {
                                tbSysRole = db.Table<Sys.Entity.tbSysRole>().Where(d => d.RoleCode == Code.EnumHelper.SysRoleCode.Family).FirstOrDefault(),
                                tbSysUser = tb.tbSysUserFamily
                            };
                            myUserRoleList.Add(familyUserRole);

                            #endregion

                            myStudentList.Add(tb);

                            if (!string.IsNullOrEmpty(import.FatherName))
                            {
                                var tbF1 = new Entity.tbStudentFamily();
                                tbF1.tbStudent = tb;
                                tbF1.Email = import.FatherEmail;
                                tbF1.FamilyName = import.FatherName;
                                tbF1.tbDictKinship = db.Table<Dict.Entity.tbDictKinship>().Where(d => d.KinshipName.Contains("父亲")).FirstOrDefault();
                                tbF1.UnitName = import.FatherCompany;
                                tbF1.Job = import.FatherJob;
                                tbF1.Mobile = import.FatherPhone;
                                myStudentFamilyList.Add(tbF1);
                            }

                            if (!string.IsNullOrEmpty(import.MotherName))
                            {
                                var tbF2 = new Entity.tbStudentFamily();
                                tbF2.tbStudent = tb;
                                tbF2.Email = import.MotherEmail;
                                tbF2.FamilyName = import.MotherName;
                                tbF2.tbDictKinship = db.Table<Dict.Entity.tbDictKinship>().Where(d => d.KinshipName.Contains("母亲")).FirstOrDefault();
                                tbF2.UnitName = import.MotherCompany;
                                tbF2.Job = import.MotherJob;
                                tbF2.Mobile = import.MotherPhone;
                                myStudentFamilyList.Add(tbF2);
                            }

                            #region tbSysUserRole:新增角色成员
                            var tbUserRole = new Sys.Entity.tbSysUserRole();
                            tbUserRole.tbSysRole = studentRole;
                            tbUserRole.tbSysUser = tb.tbSysUser;
                            myUserRoleList.Add(tbUserRole);
                            #endregion

                            #endregion
                        }
                    }

                    db.Set<Entity.tbStudent>().AddRange(myStudentList);
                    db.Set<Entity.tbStudentFamily>().AddRange(myStudentFamilyList);
                    db.Set<Sys.Entity.tbSysUserRole>().AddRange(myUserRoleList);

                    if (db.SaveChanges() > 0)
                    {
                        vm.Status = true;
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量添加学生");
                    }
                }
            }
            vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }

        public ActionResult Export(int typeId = 0, int studyTypeId = 0, int sessionId = 0, int gradeId = 0, int classId = 0, int orgId = 0, string searchText = "")
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var tbStudent = from p in db.Table<Entity.tbStudent>()
                                select p;

                if (string.IsNullOrEmpty(searchText) == false)
                {
                    tbStudent = tbStudent.Where(d => d.StudentCode.Contains(searchText) || d.StudentName.Contains(searchText));
                }

                if (typeId != 0)
                {
                    tbStudent = tbStudent.Where(d => d.tbStudentType.Id == typeId);
                }

                if (studyTypeId != 0)
                {
                    tbStudent = tbStudent.Where(d => d.tbStudentStudyType.Id == studyTypeId);
                }

                if (sessionId != 0)
                {
                    tbStudent = tbStudent.Where(d => d.tbStudentSession.Id == sessionId);
                }

                if (gradeId != 0 || classId != 0)
                {
                    tbStudent = from p in db.Table<Basis.Entity.tbClassStudent>()
                                where (p.tbClass.Id == classId || classId == 0)
                                    && (p.tbClass.tbGrade.Id == gradeId || gradeId == 0)
                                   && p.tbStudent.IsDeleted == false
                                select p.tbStudent;
                }
                else if (orgId > 0)
                {
                    tbStudent = from p in db.Table<Course.Entity.tbOrgStudent>()
                                where (p.tbOrg.Id == orgId || orgId == 0)
                                    && (p.tbOrg.tbGrade.Id == gradeId || gradeId == 0)
                                   && p.tbStudent.IsDeleted == false
                                select p.tbStudent;
                }

                var studentList = (from p in tbStudent
                                   orderby p.StudentCode
                                   select new
                                   {
                                       Id = p.Id,
                                       p.StudentCode,
                                       p.StudentName,
                                       p.PinYin,
                                       p.CMIS,
                                       p.tbSysUser.tbSex.SexName,
                                       p.tbStudentType.StudentTypeName,
                                       p.tbStudentStudyType.StudyTypeName,
                                       p.tbStudentSession.StudentSessionName,
                                       p.tbSysUser.Mobile,
                                       p.Address,
                                       p.tbSysUser.IdentityNumber,
                                       p.tbSysUser.Email,
                                       p.tbSysUser.Qq,
                                       p.tbDictBlood.BloodName,
                                       p.tbDictNation.NationName,
                                       p.tbDictParty.PartyName,
                                       p.Birthday,
                                       p.StudentNameEn,
                                       p.tbStudentSource.StudentSourceName
                                   }).ToList();

                var familyList = (from p in db.Table<Entity.tbStudentFamily>()
                                  where (p.tbDictKinship.KinshipName.Contains("父亲") || p.tbDictKinship.KinshipName.Contains("母亲"))
                                  select new
                                  {
                                      StudentId = p.tbStudent.Id,
                                      FamilyName = p.FamilyName,
                                      UnitName = p.UnitName,
                                      Job = p.Job,
                                      Email = p.Email,
                                      Mobile = p.Mobile,
                                      KinshipName = p.tbDictKinship.KinshipName
                                  }).ToList();

                var list = (from p in studentList
                            select new Dto.Student.Export()
                            {
                                Id = p.Id,
                                StudentCode = p.StudentCode,
                                StudentName = p.StudentName,
                                PinYin = p.PinYin,
                                CMIS = p.CMIS,
                                SexName = p.SexName,
                                StudentTypeName = p.StudentTypeName,
                                StudyTypeName = p.StudyTypeName,
                                StudentSessionName = p.StudentSessionName,
                                Mobile = p.Mobile,
                                Address = p.Address,
                                IdentityNumber = p.IdentityNumber,
                                Email = p.Email,
                                QQ = p.Qq,
                                BloodName = p.BloodName,
                                NationName = p.NationName,
                                PartyName = p.PartyName,
                                StudentNameEn = p.StudentNameEn,
                                Birthday = p.Birthday,
                                StudentSourceName = p.StudentSourceName,
                                FatherName = familyList.Where(d => d.KinshipName.Contains("父亲") && d.StudentId == p.Id).Select(d => d.FamilyName).FirstOrDefault(),
                                FatherCompany = familyList.Where(d => d.KinshipName.Contains("父亲") && d.StudentId == p.Id).Select(d => d.UnitName).FirstOrDefault(),
                                FatherJob = familyList.Where(d => d.KinshipName.Contains("父亲") && d.StudentId == p.Id).Select(d => d.Job).FirstOrDefault(),
                                FatherPhone = familyList.Where(d => d.KinshipName.Contains("父亲") && d.StudentId == p.Id).Select(d => d.Mobile).FirstOrDefault(),
                                FatherEmail = familyList.Where(d => d.KinshipName.Contains("父亲") && d.StudentId == p.Id).Select(d => d.Email).FirstOrDefault(),
                                MotherName = familyList.Where(d => d.KinshipName.Contains("母亲") && d.StudentId == p.Id).Select(d => d.FamilyName).FirstOrDefault(),
                                MotherCompany = familyList.Where(d => d.KinshipName.Contains("母亲") && d.StudentId == p.Id).Select(d => d.UnitName).FirstOrDefault(),
                                MotherJob = familyList.Where(d => d.KinshipName.Contains("母亲") && d.StudentId == p.Id).Select(d => d.Job).FirstOrDefault(),
                                MotherPhone = familyList.Where(d => d.KinshipName.Contains("母亲") && d.StudentId == p.Id).Select(d => d.Mobile).FirstOrDefault(),
                                MotherEmail = familyList.Where(d => d.KinshipName.Contains("母亲") && d.StudentId == p.Id).Select(d => d.Email).FirstOrDefault()
                            }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学生学号"),
                        new System.Data.DataColumn("学生姓名"),
                        new System.Data.DataColumn("姓名拼音"),
                        new System.Data.DataColumn("英文名"),
                        new System.Data.DataColumn("CMIS"),
                        new System.Data.DataColumn("性别"),
                        new System.Data.DataColumn("学生类型"),
                        new System.Data.DataColumn("就读方式"),
                        new System.Data.DataColumn("学届"),
                        new System.Data.DataColumn("手机号码"),
                        new System.Data.DataColumn("家庭住址"),
                        new System.Data.DataColumn("身份证号"),
                        new System.Data.DataColumn("邮箱"),
                        new System.Data.DataColumn("QQ"),
                        new System.Data.DataColumn("血型"),
                        new System.Data.DataColumn("民族"),
                        new System.Data.DataColumn("政治面貌"),
                        new System.Data.DataColumn("出生日期"),
                        new System.Data.DataColumn("毕业院校"),
                        new System.Data.DataColumn("父亲姓名"),
                        new System.Data.DataColumn("父亲单位"),
                        new System.Data.DataColumn("父亲职务"),
                        new System.Data.DataColumn("父亲手机"),
                        new System.Data.DataColumn("父亲邮箱"),
                        new System.Data.DataColumn("母亲姓名"),
                        new System.Data.DataColumn("母亲单位"),
                        new System.Data.DataColumn("母亲职务"),
                        new System.Data.DataColumn("母亲手机"),
                        new System.Data.DataColumn("母亲邮箱")
                    });
                foreach (var a in list)
                {
                    var dr = dt.NewRow();
                    dr["学生学号"] = a.StudentCode;
                    dr["学生姓名"] = a.StudentName;
                    dr["姓名拼音"] = a.PinYin;
                    dr["英文名"] = a.StudentNameEn;
                    dr["CMIS"] = a.CMIS;
                    dr["性别"] = a.SexName;
                    dr["学生类型"] = a.StudentTypeName;
                    dr["就读方式"] = a.StudyTypeName;
                    dr["学届"] = a.StudentSessionName;
                    dr["手机号码"] = a.Mobile;
                    dr["家庭住址"] = a.Address;
                    dr["身份证号"] = a.IdentityNumber;
                    dr["邮箱"] = a.Email;
                    dr["QQ"] = a.QQ;
                    dr["血型"] = a.BloodName;
                    dr["民族"] = a.NationName;
                    dr["政治面貌"] = a.PartyName;
                    dr["出生日期"] = a.Birthday;
                    dr["毕业院校"] = a.StudentSourceName;
                    dr["父亲姓名"] = a.FatherName;
                    dr["父亲单位"] = a.FatherCompany;
                    dr["父亲职务"] = a.FatherJob;
                    dr["父亲手机"] = a.FatherPhone;
                    dr["父亲邮箱"] = a.FatherEmail;
                    dr["母亲姓名"] = a.MotherName;
                    dr["母亲单位"] = a.MotherCompany;
                    dr["母亲职务"] = a.MotherJob;
                    dr["母亲手机"] = a.MotherPhone;
                    dr["母亲邮箱"] = a.MotherEmail;
                    dt.Rows.Add(dr);
                }

                Code.NpoiHelper.DataTableToExcel(file, dt);

                if (string.IsNullOrEmpty(file) == false)
                {
                    return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
                }
                else
                {
                    return View();
                }
            }
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Student/Views/Student/Student.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        public ActionResult ViewList()
        {
            if (Teacher.Controllers.TeacherGradeController.IsGradeTeacher())
            {
                return RedirectToAction("ViewListGrade");
            }
            else
            {
                return RedirectToAction("ViewListClass");
            }
        }

        public ActionResult ViewListGrade()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Student.ViewListGrade();
                vm.IsGradeTeacher = Teacher.Controllers.TeacherGradeController.IsGradeTeacher();
                vm.GradeList = Teacher.Controllers.TeacherGradeController.GetGradeByTeacher();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.GradeId != 0)
                {
                    vm.ClassList = Basis.Controllers.ClassController.SelectList(0, vm.GradeId);
                    if (vm.ClassId != 0)
                    {
                        if (vm.ClassList.Where(d => d.Value == vm.ClassId.ToString()).Any() == false)
                        {
                            vm.ClassId = 0;
                        }
                    }
                    if (vm.ClassId == 0)
                    {
                        if (vm.ClassList.Count > 0)
                        {
                            vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                        }
                    }

                    var tb = db.Table<Basis.Entity.tbClassStudent>().Where(d => d.tbClass.Id == vm.ClassId);

                    if (!string.IsNullOrEmpty(vm.SearchText))
                    {
                        tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText) || d.tbStudent.StudentNameEn.Contains(vm.SearchText));
                    }

                    vm.List = (from p in tb
                               orderby p.No
                               select new Dto.Student.List()
                               {
                                   Id = p.tbStudent.Id,
                                   Birthday = p.tbStudent.Birthday.ToString(),
                                   SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                   DictNationName = p.tbStudent.tbDictNation.NationName,
                                   DictPartyName = p.tbStudent.tbDictParty.PartyName,
                                   BloodTypeName = p.tbStudent.tbDictBlood.BloodName,
                                   CardNo = p.tbStudent.CardNo,
                                   Email = p.tbStudent.tbSysUser.Email,
                                   IdentityNumber = p.tbStudent.tbSysUser.IdentityNumber,
                                   LibraryNo = p.tbStudent.LibraryNo,
                                   Mobile = p.tbStudent.tbSysUser.Mobile,
                                   Photo = p.tbStudent.Photo,
                                   Profile = p.tbStudent.Profile,
                                   Qq = p.tbStudent.tbSysUser.Qq,
                                   StudentCode = p.tbStudent.StudentCode,
                                   StudentName = p.tbStudent.StudentName,
                                   StudentNameEn = p.tbStudent.StudentNameEn,
                                   StudentStudyTypeName = p.tbStudent.tbStudentStudyType.StudyTypeName,
                                   StudentTypeName = p.tbStudent.tbStudentType.StudentTypeName,
                                   TicketNumber = p.tbStudent.TicketNumber,
                                   Address = p.tbStudent.Address,
                                   ClassName = p.tbClass.ClassName
                               }).ToList();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewListGrade(Models.Student.ViewListGrade vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ViewListGrade", new { SearchText = vm.SearchText, GradeId = vm.GradeId, ClassId = vm.ClassId }));
        }

        public ActionResult ViewListClass()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Student.ViewListClass();
                vm.IsGradeTeacher = Teacher.Controllers.TeacherGradeController.IsGradeTeacher();
                vm.ClassList = Basis.Controllers.ClassTeacherController.GetClassByClassTeacher();
                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                var tb = db.Table<Basis.Entity.tbClassStudent>().Where(d => d.tbClass.Id == vm.ClassId);

                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText) || d.tbStudent.StudentNameEn.Contains(vm.SearchText));
                }

                vm.List = (from p in tb
                           orderby p.No
                           select new Dto.Student.List()
                           {
                               Id = p.tbStudent.Id,
                               Birthday = p.tbStudent.Birthday.ToString(),
                               SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                               DictNationName = p.tbStudent.tbDictNation.NationName,
                               DictPartyName = p.tbStudent.tbDictParty.PartyName,
                               BloodTypeName = p.tbStudent.tbDictBlood.BloodName,
                               CardNo = p.tbStudent.CardNo,
                               Email = p.tbStudent.tbSysUser.Email,
                               IdentityNumber = p.tbStudent.tbSysUser.IdentityNumber,
                               LibraryNo = p.tbStudent.LibraryNo,
                               Mobile = p.tbStudent.tbSysUser.Mobile,
                               Photo = p.tbStudent.Photo,
                               Profile = p.tbStudent.Profile,
                               Qq = p.tbStudent.tbSysUser.Qq,
                               StudentCode = p.tbStudent.StudentCode,
                               StudentName = p.tbStudent.StudentName,
                               StudentNameEn = p.tbStudent.StudentNameEn,
                               StudentStudyTypeName = p.tbStudent.tbStudentStudyType.StudyTypeName,
                               StudentTypeName = p.tbStudent.tbStudentType.StudentTypeName,
                               TicketNumber = p.tbStudent.TicketNumber,
                               Address = p.tbStudent.Address,
                               ClassName = p.tbClass.ClassName
                           }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewListClass(Models.Student.ViewListClass vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ViewListClass", new { SearchText = vm.SearchText, ClassId = vm.ClassId }));
        }

        #region 在读证明
        public ActionResult StudentReadList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Student.List();
                var tb = (from p in db.Table<Entity.tbStudent>()
                          where (p.StudentCode == vm.SearchText || p.StudentName == vm.SearchText)
                          && p.tbSysUser.IsDeleted == false
                          select new
                          {
                              StudentId = p.Id,
                              p.StudentCode,
                              p.StudentName,
                              p.tbSysUser.IdentityNumber,
                              EntranceDate = p.EntranceDate,
                              p.tbSysUser.tbSex.SexName,
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                     && p.tbClass.IsDeleted == false
                                     && p.tbClass.tbGrade.IsDeleted == false
                                     && p.tbClass.tbClassType.IsDeleted == false
                                     && p.tbClass.tbYear.IsDeleted == false
                                     && (p.tbStudent.StudentCode == vm.SearchText || p.tbStudent.StudentName == vm.SearchText)
                                    orderby p.tbClass.tbYear.No descending
                                    select new
                                    {
                                        p.tbStudent.Id,
                                        p.tbClass.ClassName
                                    }).ToList();
                vm.StudentList = (from p in tb
                                  select new Dto.Student.List
                                  {
                                      StudentCode = p.StudentCode,
                                      StudentName = p.StudentName,
                                      SexName = p.SexName,
                                      IdentityNumber = p.IdentityNumber,
                                      ClassName = classStudent.Where(c => c.Id == p.StudentId).Select(c => c.ClassName).FirstOrDefault(),
                                      EntranceDate = XkSystem.Code.Common.dateToUpperYM(p.EntranceDate),
                                      PrintDate = XkSystem.Code.Common.dateToUpper(System.DateTime.Now),
                                  }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentReadList(Models.Student.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("StudentReadList", new
            {
                searchText = vm.SearchText
            }));
        }
        #endregion

        public ActionResult SelectStudent()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Student.SelectStudent();
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                vm.ClassList = Basis.Controllers.ClassController.SelectList(0, vm.GradeId);

                var tb = from p in db.Table<Entity.tbStudent>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.StudentCode.Contains(vm.SearchText) || d.StudentName.Contains(vm.SearchText));
                }
                else if (vm.GradeId != 0 || vm.ClassId != 0)
                {
                    var classStudent = from p in db.Table<Basis.Entity.tbClassStudent>()
                                       where p.tbStudent.IsDeleted == false
                                       select p;

                    if (vm.GradeId != 0)
                    {
                        classStudent = classStudent.Where(d => d.tbClass.tbGrade.Id == vm.GradeId);
                    }

                    if (vm.ClassId != 0)
                    {
                        classStudent = classStudent.Where(d => d.tbClass.Id == vm.ClassId);
                    }


                    tb = from p in classStudent
                         select p.tbStudent;
                }

                var yearId = Basis.Controllers.YearController.GetDefaultYearId(db);

                vm.StudentList = (from p in tb
                                  join q in db.Table<Basis.Entity.tbClassStudent>().Where(d => d.tbClass.tbYear.Id == yearId)
                                  on p.Id equals q.tbStudent.Id into temp
                                  from g in temp.DefaultIfEmpty()
                                  orderby p.StudentCode
                                  select new Dto.Student.SelectStudent
                                  {
                                      Id = p.Id,
                                      StudentCode = p.StudentCode,
                                      StudentName = p.StudentName,
                                      SexName = p.tbSysUser.tbSex.SexName,
                                      ClassName = g.tbClass.ClassName
                                  }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        /// <summary>
        /// 导入学生照片
        /// </summary>
        public ActionResult ImportPicture()
        {
            var vm = new Models.Student.ImportPicture();
            vm.NameTypeList = new List<SelectListItem>() {
                new SelectListItem() { Value="1",Text="学号"},
                new SelectListItem() { Value="2",Text="中考号"},
                new SelectListItem() { Value="3",Text="身份证号"}
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportPicture(Models.Student.ImportPicture vm)
        {
            if (ModelState.IsValid)
            {
                vm.NameTypeList = new List<SelectListItem>() {
                    new SelectListItem() { Value="1",Text="学号"},
                    new SelectListItem() { Value="2",Text="中考号"},
                    new SelectListItem() { Value="3",Text="身份证号"}
                };

                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);

                using (var db = new XkSystem.Models.DbContext())
                {
                    #region 解压文件
                    if (Code.Common.GetFileType(file.FileName) != Code.FileType.Zip)
                    {
                        ModelState.AddModelError("", "上传的文件不是正确的zip压缩文件!");
                        return View(vm);
                    }

                    var photos = Server.MapPath("~/Files/StudentPhoto/ZIP");
                    Directory.Delete(photos, true);
                    Directory.CreateDirectory(photos);
                    var zipResult = Code.ZipHelper.ExtractZip(fileSave, photos);
                    if (zipResult == "Cannot find central directory")
                    {
                        ModelState.AddModelError("", "上传的文件不是正确的zip压缩文件,或者文件是由其他压缩格式强制修改了扩展名，请重新压缩成zip格式的压缩文件！");
                        return View(vm);
                    }
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(photos);

                    if (dir.GetFiles().Length + dir.GetDirectories().Length == 0)
                    {
                        ModelState.AddModelError("", "无法读取上传的文件，请检查文件格式是否正确!");
                        return View(vm);
                    }
                    #endregion

                    var studentList = db.Table<Entity.tbStudent>().Include(d => d.tbSysUser).ToList();

                    #region 验证数据格式是否正确
                    List<string[]> fileNameList = Code.Common.GetFileNames(dir);
                    if (vm.NameTypeId == 1)
                    {
                        foreach (var v in fileNameList)
                        {
                            if (Code.Common.GetFileType(v[0]) != Code.FileType.Image)
                            {
                                vm.ImportPictureList.Add(new Dto.Student.ImportPicture()
                                {
                                    PicName = v[0],
                                    Error = "上传的文件不是正确的图片文件，图片文件必须为jpg,jpeg,png,bmp格式；"
                                });
                            }
                            var studentCode = v[0].Split('.')[0];
                            if (studentList.Where(d => d.StudentCode == studentCode).Count() == 0)
                            {
                                vm.ImportPictureList.Add(new Dto.Student.ImportPicture()
                                {
                                    PicName = v[0],
                                    Error = "学生信息不存在；"
                                });
                            }
                            if (!vm.IsUpdate && studentList.Where(d => d.StudentCode == studentCode && !string.IsNullOrEmpty(d.Photo)).Count() > 0)
                            {
                                vm.ImportPictureList.Add(new Dto.Student.ImportPicture()
                                {
                                    PicName = v[0],
                                    Error = "此学生已有照片信息；"
                                });
                            }
                        }
                    }
                    else if (vm.NameTypeId == 2)
                    {
                        foreach (var v in fileNameList)
                        {
                            if (Code.Common.GetFileType(v[0]) != Code.FileType.Image)
                            {
                                vm.ImportPictureList.Add(new Dto.Student.ImportPicture()
                                {
                                    PicName = v[0],
                                    Error = "上传的文件不是正确的图片文件，图片文件必须为jpg,jpeg,png,bmp格式；"
                                });
                            }
                            var ticketNumber = v[0].Split('.')[0];
                            if (studentList.Where(d => d.TicketNumber == ticketNumber).Count() == 0)
                            {
                                vm.ImportPictureList.Add(new Dto.Student.ImportPicture()
                                {
                                    PicName = v[0],
                                    Error = "学生信息不存在；"
                                });
                            }
                            if (!vm.IsUpdate && studentList.Where(d => d.TicketNumber == ticketNumber && !string.IsNullOrEmpty(d.Photo)).Count() > 0)
                            {
                                vm.ImportPictureList.Add(new Dto.Student.ImportPicture()
                                {
                                    PicName = v[0],
                                    Error = "此学生已有照片信息；"
                                });
                            }
                        }
                    }
                    else
                    {
                        foreach (var v in fileNameList)
                        {
                            if (Code.Common.GetFileType(v[0]) != Code.FileType.Image)
                            {
                                vm.ImportPictureList.Add(new Dto.Student.ImportPicture()
                                {
                                    PicName = v[0],
                                    Error = "上传的文件不是正确的图片文件，图片文件必须为jpg,jpeg,png,bmp格式；"
                                });
                            }
                            var identityNumber = v[0].Split('.')[0];
                            if (studentList.Where(d => d.tbSysUser.IdentityNumber == identityNumber).Count() == 0)
                            {
                                vm.ImportPictureList.Add(new Dto.Student.ImportPicture()
                                {
                                    PicName = v[0],
                                    Error = "学生信息不存在；"
                                });
                            }
                            if (!vm.IsUpdate && studentList.Where(d => d.tbSysUser.IdentityNumber == identityNumber && !string.IsNullOrEmpty(d.Photo)).Count() > 0)
                            {
                                vm.ImportPictureList.Add(new Dto.Student.ImportPicture()
                                {
                                    PicName = v[0],
                                    Error = "此学生已有照片信息；"
                                });
                            }
                        }
                    }

                    if (vm.ImportPictureList.Count > 0)
                    {
                        return View(vm);
                    }
                    #endregion

                    #region 数据导入，判断关键字，存在的数据做更新，不存在的做新增
                    if (vm.NameTypeId == 1)
                    {
                        foreach (var v in fileNameList)
                        {
                            var studentCode = v[0].Split('.')[0];
                            var student = studentList.Where(d => d.StudentCode == studentCode).FirstOrDefault();

                            var path = Server.MapPath("~/Files/StudentPhoto/");
                            var newName = Guid.NewGuid().ConvertToString().ToLower() + "." + v[0].Split('.').Last();
                            System.IO.File.Move(v[1], path + newName);
                            student.Photo = newName;
                        }
                    }
                    else if (vm.NameTypeId == 2)
                    {
                        foreach (var v in fileNameList)
                        {
                            var ticketNumber = v[0].Split('.')[0];
                            var student = studentList.Where(d => d.TicketNumber == ticketNumber).FirstOrDefault();

                            var path = Server.MapPath("~/Files/StudentPhoto/");
                            var newName = Guid.NewGuid().ConvertToString().ToLower() + "." + v[0].Split('.').Last();
                            System.IO.File.Move(v[1], path + newName);
                            student.Photo = newName;
                        }
                    }
                    else
                    {
                        foreach (var v in fileNameList)
                        {
                            var identityNumber = v[0].Split('.')[0];
                            var student = studentList.Where(d => d.tbSysUser.IdentityNumber == identityNumber).FirstOrDefault();

                            var path = Server.MapPath("~/Files/StudentPhoto/");
                            var newName = Guid.NewGuid().ConvertToString().ToLower() + "." + v[0].Split('.').Last();
                            System.IO.File.Move(v[1], path + newName);
                            student.Photo = newName;
                        }
                    }
                    #endregion

                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert("批量导入学生照片");
                        vm.Status = true;
                    }
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectStudent(Models.Student.SelectStudent vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SelectStudent",
                new
                {
                    searchText = vm.SearchText,
                    pageIndex = vm.Page.PageIndex,
                    pageSize = vm.Page.PageSize,
                    GradeId = vm.GradeId,
                    ClassId = vm.ClassId
                }));
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(string SearchText = "")
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbStudent>()
                          where p.StudentName.Contains(SearchText) || string.IsNullOrEmpty(SearchText)
                          orderby p.StudentName
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.StudentName,
                              Value = p.StudentCode.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<Dto.Student.Info> SelectInfoList(string SearchText = "")
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = from p in db.Table<Entity.tbStudent>()
                         select p;

                if (string.IsNullOrEmpty(SearchText) == false)
                {
                    tb = tb.Where(d => d.StudentCode.Contains(SearchText) || d.StudentName.Contains(SearchText));
                }

                var list = (from p in tb
                            orderby p.StudentName
                            select new Dto.Student.Info
                            {
                                Id = p.Id,
                                StudentCode = p.StudentCode,
                                StudentName = p.StudentName
                            }).ToList();
                return list;
            }
        }

        [NonAction]
        public static List<Dto.Student.Info> SelectInfoList(int classId, string SearchText = "")
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbClassStudent>()
                          where p.tbClass.Id == classId
                            && p.tbStudent.IsDeleted == false
                            && (p.tbStudent.StudentName.Contains(SearchText) || p.tbStudent.StudentCode.Contains(SearchText) || string.IsNullOrEmpty(SearchText))
                          orderby p.tbClass.tbGrade.No, p.tbClass.No, p.tbStudent.No
                          select new Dto.Student.Info
                          {
                              Id = p.tbStudent.Id,
                              StudentCode = p.tbStudent.StudentCode,
                              StudentName = p.tbStudent.StudentName
                          }).ToList();

                return tb;
            }
        }

        [NonAction]
        public static List<Dto.Student.List> SelectStudentInfoList(int classId, string SearchText = "")
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbClassStudent>()
                          where p.tbClass.Id == classId
                            && p.tbStudent.IsDeleted == false
                          orderby p.tbClass.tbGrade.No, p.tbClass.No, p.tbStudent.No
                          select new Dto.Student.List
                          {
                              Id = p.tbStudent.Id,
                              StudentCode = p.tbStudent.StudentCode,
                              StudentName = p.tbStudent.StudentName,
                              SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                              ClassName = p.tbClass.ClassName
                          });
                if (string.IsNullOrEmpty(SearchText) == false)
                {
                    tb = tb.Where(d => d.StudentCode.Contains(SearchText) || d.StudentName.Contains(SearchText));
                }
                return tb.ToList();
            }
        }

        [NonAction]
        public static List<Dto.Student.List> SelectStudentList(int yearId, int classId, string SearchText = "")
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = from p in db.Table<Entity.tbStudent>()
                         select p;

                if (string.IsNullOrEmpty(SearchText) == false)
                {
                    tb = tb.Where(d => d.StudentCode.Contains(SearchText) || d.StudentName.Contains(SearchText));
                }
                else if (classId != 0)
                {
                    var classStudent = from p in db.Table<Basis.Entity.tbClassStudent>()
                                       where p.tbStudent.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbClass.tbGrade.IsDeleted == false
                                        && p.tbClass.tbClassType.IsDeleted == false
                                        && p.tbClass.tbYear.Id == yearId
                                       select p;

                    if (classId != 0)
                    {
                        classStudent = classStudent.Where(d => d.tbClass.Id == classId);
                    }

                    tb = from p in classStudent
                         select p.tbStudent;
                }

                var tbS = (from p in tb
                           join q in db.Table<Basis.Entity.tbClassStudent>().Where(d => d.tbClass.tbYear.Id == yearId)
                           on p.Id equals q.tbStudent.Id into temp
                           from g in temp.DefaultIfEmpty()
                           orderby p.StudentCode
                           select new Dto.Student.List
                           {
                               Id = p.Id,
                               Birthday = p.Birthday.ToString(),
                               SexName = p.tbSysUser.tbSex.SexName,
                               DictNationName = p.tbDictNation.NationName,
                               DictPartyName = p.tbDictParty.PartyName,
                               BloodTypeName = p.tbDictBlood.BloodName,
                               CardNo = p.CardNo,
                               Email = p.tbSysUser.Email,
                               IdentityNumber = p.tbSysUser.IdentityNumber,
                               LibraryNo = p.LibraryNo,
                               Mobile = p.tbSysUser.Mobile,
                               Profile = p.Profile,
                               Qq = p.tbSysUser.Qq,
                               StudentCode = p.StudentCode,
                               StudentName = p.StudentName,
                               StudentNameEn = p.StudentNameEn,
                               StudentStudyTypeName = p.tbStudentStudyType.StudyTypeName,
                               StudentTypeName = p.tbStudentType.StudentTypeName,
                               TicketNumber = p.TicketNumber,
                               Address = p.Address,
                               ClassName = g.tbClass.ClassName
                           }).ToList();
                return tbS;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> GetStudentById(string SearchText = "", int studentId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbStudent>()
                          where (p.StudentName.Contains(SearchText) || string.IsNullOrEmpty(SearchText))
                            && (p.Id == studentId || studentId == 0)
                          orderby p.StudentCode
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.StudentName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<Dto.Student.Info> GetStudentById(int studentId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbStudent>()
                          where p.Id == studentId
                          orderby p.StudentCode
                          select new Dto.Student.Info
                          {
                              Id = p.Id,
                              StudentCode = p.StudentCode,
                              StudentName = p.StudentName
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static Dto.Student.Info GetStudentInfoByUserId(int userId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbStudent>()
                          where p.tbSysUser.Id == userId
                          orderby p.StudentCode
                          select new Dto.Student.Info
                          {
                              Id = p.Id,
                              StudentCode = p.StudentCode,
                              StudentName = p.StudentName,
                              Photo = p.Photo
                          }).FirstOrDefault();
                return tb;
            }
        }


        [NonAction]
        internal static List<Dto.Student.Info> GetStudentInfoListByClassIds(List<int> classIds)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbClassStudent>()
                          where classIds.Contains(p.tbClass.Id)
                          orderby p.tbStudent.StudentCode
                          select new Dto.Student.Info
                          {
                              Id = p.tbStudent.Id,
                              StudentCode = p.tbStudent.StudentCode,
                              StudentName = p.tbStudent.StudentName,
                              ClassId = p.tbClass.Id,
                              ClassName = p.tbClass.ClassName
                          }).ToList();
                return tb;
            }
        }



        /// <summary>
        /// 判断删除学生，仅适用于学生异动
        /// </summary>
        [NonAction]
        public static bool Delete(XkSystem.Models.DbContext db, string userCode)
        {
            var student = db.Set<Entity.tbStudent>().Where(d => d.StudentCode == userCode).FirstOrDefault();
            if (student.IsDeleted == false)
            {
                student.IsDeleted = true;
            }

            var user = db.Set<Sys.Entity.tbSysUser>().Where(d => d.UserCode == userCode).FirstOrDefault();
            if (user.IsDeleted == false)
            {
                user.IsDeleted = true;
            }
            return true;
        }

        [NonAction]
        public static List<Entity.tbStudent> BuildList(XkSystem.Models.DbContext db, List<Dto.Student.Edit> editList)
        {
            List<Entity.tbStudent> list = new List<Entity.tbStudent>();

            foreach (var v in editList)
            {
                var student = new Entity.tbStudent()
                {
                    StudentCode = v.StudentCode,
                    StudentName = v.StudentName,
                    tbSysUser = new Sys.Entity.tbSysUser()
                    {
                        UserCode = v.StudentCode,
                        UserName = v.StudentName,
                        Password = Code.Common.DESEnCode("123456"),
                        PasswordMd5 = Code.Common.CreateMD5Hash("123456"),
                        UserType = Code.EnumHelper.SysUserType.Student
                    },
                };

                var tbUserRole = new Sys.Entity.tbSysUserRole();
                tbUserRole.tbSysRole = db.Table<Sys.Entity.tbSysRole>().Where(d => d.RoleCode == Code.EnumHelper.SysRoleCode.Student).FirstOrDefault();
                tbUserRole.tbSysUser = student.tbSysUser;
                db.Set<Sys.Entity.tbSysUserRole>().Add(tbUserRole);

                list.Add(student);
            }

            return list;
        }

        public ActionResult SelectUserJson(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Student.SelectStudent();
                var tb = (from p in db.Table<Entity.tbStudent>()
                          where ids.Contains(p.Id)
                          select new Dto.Student.Info
                          {
                              Id = p.tbSysUser.Id,
                              StudentName = p.tbSysUser.UserName
                          }).ToList();
                return Json(tb, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetStudentSource(string q)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                q = q.ConvertToString();
                var tb = (from p in db.Table<Entity.tbStudentSource>()
                          where (p.StudentSourceName.Contains(q))
                          orderby p.StudentSourceName
                          select p.StudentSourceName).Take(10).ToList();

                return Json(tb, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetStudent(string q)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                q = q.ConvertToString();
                var tb = (from p in db.Table<Entity.tbStudent>()
                          where (p.StudentCode.Contains(q)) || (p.StudentName.Contains(q)) || (p.StudentNameEn.Contains(q))
                          orderby p.No
                          select p.StudentCode + "(" + p.StudentName + ")").Take(6).ToList();

                return Json(tb, JsonRequestBehavior.AllowGet);
            }
        }
    }
}