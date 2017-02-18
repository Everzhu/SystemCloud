using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveApplyController : Controller
    {
        // GET: Elective/ElectiveApply
        public ActionResult List()
        {
            var vm = new Models.ElectiveApply.List();
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveApply>() select p);
                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(p => p.tbCourse.CourseName.Contains(vm.SearchText) || p.tbElective.ElectiveName.Contains(vm.SearchText));
                }

                if (vm.ElectiveId > 0)
                {
                    tb = tb.Where(p => p.tbElective.Id == vm.ElectiveId);
                }

                vm.ElectiveApplyList = (from p in tb
                                        orderby p.No
                                        select new Dto.ElectiveApply.List()
                                        {
                                            Id = p.Id,
                                            ElectiveName = p.tbElective.ElectiveName,
                                            CourseName = p.tbCourse.CourseName,
                                            InputTime = p.InputTime,
                                            Hour = p.Hour,
                                            Point = p.tbCourse.Point,
                                            CheckStatus = p.CheckStatus,
                                            CheckOpinion = p.CheckOpinion,
                                            MaxStudent = p.MaxStudent,
                                            RoomName = p.tbRoom.RoomName,
                                            IsMultiClass = p.IsMultiClass,
                                            //ElectiveApplyFileList = db.Set<Entity.tbElectiveApplyFile>().Where(c => c.tbElectiveApply.Id == p.Id).Select(c => new Dto.ElectiveApplyFile.List()
                                            //{
                                            //    FileName = c.FileName,
                                            //    FileTitle = c.FileTitle
                                            //}).ToList(),
                                            //ElectiveApplyScheduleList = db.Set<Entity.tbElectiveApplySchedule>().Where(c => c.tbElectiveApply.Id == p.Id).Select(c => new Dto.ElectiveApplySchedule.List()
                                            //{
                                            //    WeekId = c.tbWeek.Id,
                                            //    WeekName = c.tbWeek.WeekName,
                                            //    PeriodId = c.tbPeriod.Id,
                                            //    PeriodName = c.tbPeriod.PeriodName
                                            //}).ToList(),
                                            IsWeekPeriod = p.tbElective.tbElectiveType.ElectiveTypeCode == Code.EnumHelper.ElectiveType.WeekPeriod
                                        }).ToPageList(vm.Page);

                vm.ElectiveList = ElectiveController.SelectList();
                vm.ElectiveList.Insert(0, new SelectListItem() { Text = "全部", Value = "0" });
            }
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ElectiveApply.List vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("List", new
            {
                SearchText = vm.SearchText,
                PageIndex = vm.Page.PageIndex,
                PageSize = vm.Page.PageSize,
                ElectiveId = vm.ElectiveId
            }));
        }



        public ActionResult EditFirstStep(int id = 0)
        {
            var vm = new Models.ElectiveApply.FirstStepEdit();
            using (var db = new XkSystem.Models.DbContext())
            {

                vm.ElectiveList = ElectiveController.SelectList();
                if (vm.ElectiveId == 0 && vm.ElectiveList != null && vm.ElectiveList.Count > 0)
                {
                    vm.ElectiveId = vm.ElectiveList[0].Value.ConvertToInt();
                }

                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {

                    var tb = (from p in db.Table<Course.Entity.tbCourse>() where p.CourseName.Contains(vm.SearchText) || p.CourseNameEn.Contains(vm.SearchText) select p);

                    vm.CourseList = (from p in tb
                                     join e in db.Table<Entity.tbElectiveSubject>() on p.tbSubject.Id equals e.tbSubject.Id
                                     where e.tbElective.Id == vm.ElectiveId
                                     select new Course.Dto.Course.List()
                                     {
                                         Id = p.Id,
                                         CourseCode = p.CourseCode,
                                         CourseName = p.CourseName,
                                         CourseDomainName = p.tbCourseDomain.CourseDomainName,
                                         CourseNameEn = p.CourseNameEn,
                                         Hour = p.Hour,
                                         Point = p.Point,
                                         CourseTypeName = p.tbCourseType.CourseTypeName,
                                         SubjectName = p.tbSubject.SubjectName,
                                         CourseGroupName = p.tbCourseGroup.CourseGroupName
                                     }).ToList();
                }
            }
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFirstStep(Models.ElectiveApply.FirstStepEdit vm)
        {

            return Code.MvcHelper.Post(returnUrl: Url.Action("EditFirstStep", new { SearchText = vm.SearchText, ElectiveId = vm.ElectiveId }));
        }


        public ActionResult Edit(int id = 0, int courseId = 0, int electiveId = 0)
        {
            var vm = new Models.ElectiveApply.Edit();


            using (var db = new XkSystem.Models.DbContext())
            {
                if (id == 0)
                {
                    //vm.CourseList = Course.Controllers.CourseController.SelectList();                   
                }
                else
                {
                    var tb = (from p in db.Table<Entity.tbElectiveApply>()
                              where p.Id == id
                              select new Dto.ElectiveApply.Edit()
                              {                                 
                                  Id = p.Id,
                                  ElectiveId = p.tbElective.Id,
                                  CourseId = p.tbCourse.Id,
                                  Hour = p.Hour,
                                  Point = p.tbCourse.Point,
                                  TeachPlan = p.TeachPlan,
                                  StudyTarger = p.StudyTarger,
                                  MaxStudent = p.MaxStudent,
                                  RoomId = p.tbRoom.Id,
                                  SubjectId = p.tbCourse.tbSubject.Id,
                                  TeacherId = p.tbSysUser.Id,
                                  CheckOpinion = p.CheckOpinion,
                                  CheckStatus = p.CheckStatus,
                                  tbElectiveApplySchedule = db.Set<Entity.tbElectiveApplySchedule>().Where(c => c.tbElectiveApply.Id == p.Id).Select(c => new Dto.ElectiveApplySchedule.List()
                                  {
                                      WeekId = c.tbWeek.Id,
                                      WeekName = c.tbWeek.WeekName,
                                      PeriodId = c.tbPeriod.Id,
                                      PeriodName = c.tbPeriod.PeriodName
                                  }).ToList(),
                                  tbElectiveApplyFile = db.Set<Entity.tbElectiveApplyFile>().Where(c => c.tbElectiveApply.Id == p.Id).Select(c => new Dto.ElectiveApplyFile.List()
                                  {
                                      FileName = c.FileName,
                                      FileTitle = c.FileTitle
                                  }).ToList()
                              }).FirstOrDefault();
                    vm.ElectiveApplyEdit = tb;

                    courseId = courseId == 0 ? tb.CourseId : courseId;
                    electiveId = electiveId == 0 ? tb.ElectiveId : electiveId;
                }

                vm.ElectiveApplyEdit.CourseId = courseId;
                vm.ElectiveApplyEdit.ElectiveId = electiveId;

                if (courseId > 0 && electiveId > 0)
                {
                    var tbCourse = (from p in db.Table<Course.Entity.tbCourse>()
                                    where p.Id == courseId
                                    select new
                                    {
                                        SubjectId = p.tbSubject.Id,
                                        SubjectName = p.tbSubject.SubjectName,
                                        CourseId = p.Id,
                                        CourseName = p.CourseName,
                                        Hour = p.Hour,
                                        Point = p.Point
                                    }).FirstOrDefault();
                    vm.ElectiveApplyEdit.SubjectId = tbCourse.SubjectId;
                    //vm.CourseList = new List<SelectListItem>() {
                    //    new SelectListItem() { Text=tbCourse.CourseName,Value=tbCourse.CourseId.ConvertToString() }
                    //};
                    //vm.CourseSubject = new List<SelectListItem>() {
                    //    new SelectListItem() { Text=tbCourse.SubjectName,Value=tbCourse.SubjectId.ToString() }
                    //};
                    vm.ElectiveApplyEdit.Hour = tbCourse.Hour;
                    vm.ElectiveApplyEdit.Point = tbCourse.Point;

                    var tbElective = (from p in db.Table<Entity.tbElective>()
                                      where p.Id == vm.ElectiveApplyEdit.ElectiveId
                                      select new
                                      {
                                          Id = p.Id,
                                          Name = p.ElectiveName,
                                          ElectiveType = p.tbElectiveType.ElectiveTypeCode
                                      }).FirstOrDefault();
                    if (tbElective == null)
                    {
                        return Code.MvcHelper.Post(new List<string>() { Resources.LocalizedText.MsgNotFound });
                    }
                    //vm.ElectiveList = new List<SelectListItem>() { new SelectListItem() { Value = tbElective.Id.ToString(), Text = tbElective.Name } };
                    vm.IsWeekPeriod = tbElective.ElectiveType == Code.EnumHelper.ElectiveType.WeekPeriod;
                }
                else
                {

                    //electiveId = vm.ElectiveList[0].Value.ConvertToInt();

                }

                vm.ElectiveList = ElectiveController.SelectList();
                vm.RoomList = Basis.Controllers.RoomController.SelectList();
                vm.CourseSubject = ElectiveSubjectController.SelectSubjectList(electiveId);
                vm.CourseList = Course.Controllers.CourseController.SelectList(vm.ElectiveApplyEdit.SubjectId > 0 ? vm.ElectiveApplyEdit.SubjectId : vm.CourseSubject[0].Value.ConvertToInt());

                //if (vm.IsWeekPeriod)
                //{
                vm.PeriodList = Basis.Controllers.PeriodController.SelectList();
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                //}

            }
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Models.ElectiveApply.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tbElectiveApply = new Entity.tbElectiveApply();
                if (vm.ElectiveApplyEdit.Id > 0)
                {
                    tbElectiveApply = db.Set<Entity.tbElectiveApply>().Find(vm.ElectiveApplyEdit.Id);
                    if (tbElectiveApply == null)
                    {
                        return Code.MvcHelper.Post(new List<string>() { Resources.LocalizedText.MsgNotFound });
                    }
                }

                var tbElective = (from p in db.Set<Entity.tbElective>().Include(p => p.tbElectiveType) where p.Id == vm.ElectiveApplyEdit.ElectiveId select p).FirstOrDefault();
                tbElectiveApply.tbElective = tbElective;
                tbElectiveApply.tbCourse = db.Set<Course.Entity.tbCourse>().Find(vm.ElectiveApplyEdit.CourseId);
                tbElectiveApply.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.ElectiveApplyEdit.RoomId);
                tbElectiveApply.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                tbElectiveApply.Hour = vm.ElectiveApplyEdit.Hour;
                tbElectiveApply.TeachPlan = vm.ElectiveApplyEdit.TeachPlan;
                tbElectiveApply.StudyTarger = vm.ElectiveApplyEdit.StudyTarger;
                tbElectiveApply.CheckStatus = Code.EnumHelper.CheckStatus.None;
                tbElectiveApply.MaxStudent = vm.ElectiveApplyEdit.MaxStudent;
                tbElectiveApply.IsMultiClass = vm.ElectiveApplyEdit.IsMultiClass;

                if (vm.ElectiveApplyEdit.Id == 0)
                {
                    tbElectiveApply.InputTime = DateTime.Now;
                    db.Set<Entity.tbElectiveApply>().Add(tbElectiveApply);
                }


                //星期节次
                if (tbElective.tbElectiveType.ElectiveTypeCode == Code.EnumHelper.ElectiveType.WeekPeriod)
                {
                    var weekPeriod = Request["cBox"];
                    if (string.IsNullOrWhiteSpace(weekPeriod))
                    {
                        return Code.MvcHelper.Post(new List<string>() { "请至少选择一项星期节次" });
                    }

                    //删除原有 (可以做判断只删除不在本次选择范围内的，待优化。)
                    var tbWeekPeriod = (from p in db.Table<Entity.tbElectiveApplySchedule>()/*.Include(p=>p.tbWeek).Include(p=>p.tbPeriod)*/ where p.tbElectiveApply.Id == tbElectiveApply.Id select p);
                    foreach (var item in tbWeekPeriod)
                    {
                        item.IsDeleted = true;
                        item.UpdateTime = DateTime.Now;
                    }

                    var weekPeriodList = weekPeriod.Split(',').ToList();

                    db.Set<Entity.tbElectiveApplySchedule>().AddRange(weekPeriodList.Select(p => new Entity.tbElectiveApplySchedule()
                    {
                        tbElectiveApply = tbElectiveApply,
                        tbWeek = db.Set<Basis.Entity.tbWeek>().Find(p.Split('_')[0].ConvertToInt()),
                        tbPeriod = db.Set<Basis.Entity.tbPeriod>().Find(p.Split('_')[1].ConvertToInt())
                    }));
                }


                //选课申报附件
                if (!string.IsNullOrWhiteSpace(vm.SelectFiles))
                {
                    var fileNames = vm.SelectFiles.Split('|').ToList();
                    fileNames.RemoveAll(p => string.IsNullOrWhiteSpace(p));

                    //编辑时，删除原有的附件
                    if (vm.ElectiveApplyEdit.Id > 0)
                    {
                        var files = (from p in db.Table<Entity.tbElectiveApplyFile>() where p.tbElectiveApply.Id == vm.ElectiveApplyEdit.Id select p);
                        foreach (var file in files)
                        {
                            file.IsDeleted = true;
                            file.UpdateTime = DateTime.Now;
                        }
                    }

                    if (fileNames != null && fileNames.Count > 0)
                    {
                        var tbElectiveApplyFile = fileNames.Select(p => new Entity.tbElectiveApplyFile()
                        {
                            FileTitle = p.Split('/')[0],
                            FileName = p.Split('/')[1],
                            tbElectiveApply = tbElectiveApply
                        }).ToList();

                        db.Set<Entity.tbElectiveApplyFile>().AddRange(tbElectiveApplyFile);
                    }
                }
                db.SaveChanges();
            }
            return Code.MvcHelper.Post(returnUrl: Url.Action("List", "ElectiveApply"), message: "操作成功！");
        }


        [HttpPost]
        public JsonResult Uploader()
        {
            var file = Request.Files["Filedata"];
            var result = Code.Common.SaveFile(file, "~/Files/ElectiveApply/", true);
            return Json(new { FileTitle = result.Item1, FileName = result.Item2 });
        }


        /// <summary>
        /// 查看课程申报详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(int id = 0)
        {
            var vm = new Models.ElectiveApply.Detail();
            using (var db = new XkSystem.Models.DbContext())
            {
                if (id > 0)
                {
                    var tb = (from p in db.Table<Entity.tbElectiveApply>()
                              .Include(p => p.tbCourse)
                              .Include(p => p.tbElective)
                              .Include(p => p.tbRoom)
                              where p.Id == id
                              select new Dto.ElectiveApply.Detail()
                              {
                                  Id = p.Id,
                                  ElectiveName = p.tbElective.ElectiveName,
                                  CourseName = p.tbCourse.CourseName,
                                  Hour = p.Hour,
                                  Point = p.tbCourse.Point,
                                  RoomName = p.tbRoom.RoomName,
                                  MaxStudent = p.MaxStudent,
                                  TeachPlan = p.TeachPlan,
                                  StudyTarger = p.StudyTarger,
                                  SubjectName = p.tbCourse.tbSubject.SubjectName,
                                  UserName = p.tbSysUser.UserName,
                                  ElectiveApplyFileList = db.Set<Entity.tbElectiveApplyFile>().Where(c => c.tbElectiveApply.Id == id).Select(c => new Dto.ElectiveApplyFile.List() { FileTitle = c.FileTitle, FileName = c.FileName }).ToList(),
                                  ElectiveApplyScheduleList = db.Set<Entity.tbElectiveApplySchedule>().Where(c => c.tbElectiveApply.Id == id).Select(c => new Dto.ElectiveApplySchedule.List() { PeriodId = c.tbPeriod.Id, PeriodName = c.tbPeriod.PeriodName, WeekId = c.tbWeek.Id, WeekName = c.tbWeek.WeekName }).ToList()
                              }).FirstOrDefault();
                    if (tb == null)
                    {
                        return Code.MvcHelper.Post(new List<string>() { Resources.LocalizedText.MsgNotFound });
                    }
                    vm.ElectiveApplyDetail = tb;
                    vm.IsWeekPeriod = tb.ElectiveApplyScheduleList != null && tb.ElectiveApplyScheduleList.Count > 0;
                }
                else
                {
                    return View(vm);
                }
            }
            vm.PeriodList = Basis.Controllers.PeriodController.SelectList();
            vm.WeekList = Basis.Controllers.WeekController.SelectList();
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detail(Models.ElectiveApply.Detail vm)
        {
            if (vm.IsWeekPeriod)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var weekPeriod = Request["cBox"];
                    if (string.IsNullOrWhiteSpace(weekPeriod))
                    {
                        return Code.MvcHelper.Post(new List<string>() { "请至少选择一项星期节次." });
                    }

                    //删除原有 (可以做判断只删除不在本次选择范围内的，待优化。)
                    var tbWeekPeriod = (from p in db.Table<Entity.tbElectiveApplySchedule>()/*.Include(p=>p.tbWeek).Include(p=>p.tbPeriod)*/ where p.tbElectiveApply.Id == vm.ElectiveApplyDetail.Id select p);
                    foreach (var item in tbWeekPeriod)
                    {
                        item.IsDeleted = true;
                        item.UpdateTime = DateTime.Now;
                    }

                    var weekPeriodList = weekPeriod.Split(',').ToList();

                    var tbElectiveApply = db.Set<Entity.tbElectiveApply>().Find(vm.ElectiveApplyDetail.Id);
                    db.Set<Entity.tbElectiveApplySchedule>().AddRange(weekPeriodList.Select(p => new Entity.tbElectiveApplySchedule()
                    {
                        tbElectiveApply = tbElectiveApply,
                        tbWeek = db.Set<Basis.Entity.tbWeek>().Find(p.Split('_')[0].ConvertToInt()),
                        tbPeriod = db.Set<Basis.Entity.tbPeriod>().Find(p.Split('_')[1].ConvertToInt())
                    }));

                    db.SaveChanges();
                }
            }

            return Code.MvcHelper.Post();
        }

        public ActionResult Approve()
        {
            var vm = new Models.ElectiveApply.List();
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveApply>() select p);
                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(p => p.tbCourse.CourseName.Contains(vm.SearchText) || p.tbElective.ElectiveName.Contains(vm.SearchText));
                }

                if (vm.ElectiveId > 0)
                {
                    tb = tb.Where(p => p.tbElective.Id == vm.ElectiveId);
                }

                vm.ElectiveApplyList = (from p in tb
                                        orderby p.No
                                        select new Dto.ElectiveApply.List()
                                        {
                                            Id = p.Id,
                                            ElectiveName = p.tbElective.ElectiveName,
                                            CourseName = p.tbCourse.CourseName,
                                            InputTime = p.InputTime,
                                            Hour = p.Hour,
                                            Point = p.tbCourse.Point,
                                            CheckStatus = p.CheckStatus,
                                            CheckOpinion = p.CheckOpinion,
                                            MaxStudent = p.MaxStudent,
                                            RoomName = p.tbRoom.RoomName,
                                            UserName = p.tbSysUser.UserName,
                                            IsMultiClass = p.IsMultiClass
                                            //ElectiveApplyFileList = db.Set<Entity.tbElectiveApplyFile>().Where(c => c.tbElectiveApply.Id == p.Id).Select(c => new Dto.ElectiveApplyFile.List()
                                            //{
                                            //    FileName = c.FileName,
                                            //    FileTitle = c.FileTitle
                                            //}).ToList(),
                                            //ElectiveApplyScheduleList = db.Set<Entity.tbElectiveApplySchedule>().Where(c => c.tbElectiveApply.Id == p.Id).Select(c => new Dto.ElectiveApplySchedule.List()
                                            //{
                                            //    WeekId = c.tbWeek.Id,
                                            //    WeekName = c.tbWeek.WeekName,
                                            //    PeriodId = c.tbPeriod.Id,
                                            //    PeriodName = c.tbPeriod.PeriodName
                                            //}).ToList(),
                                        }).ToPageList(vm.Page);

                vm.ElectiveList = ElectiveController.SelectList();
                vm.ElectiveList.Insert(0, new SelectListItem() { Text = "全部", Value = "0" });
            }
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(Models.ElectiveApply.List vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("Approve", new
            {
                SearchText = vm.SearchText,
                PageIndex = vm.Page.PageIndex,
                PageSize = vm.Page.PageSize,
                ElectiveId = vm.ElectiveId
            }));
        }


        [HttpPost]
        public JsonResult ApprovePost()
        {

            var ids = Request["ids"].Split(',').Select(int.Parse).ToList();
            var operate = Request["operate"].ConvertToInt();
            var opinion = Request["opinion"];

            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveApply>()
                          .Include(p => p.tbSysUser)
                          .Include(p => p.tbRoom)
                          .Include(p => p.tbCourse)
                          .Include(p => p.tbElective)
                          where ids.Contains(p.Id)
                          select p).ToList();

                if (operate == 0)
                {
                    foreach (var item in tb)
                    {
                        item.CheckStatus = Code.EnumHelper.CheckStatus.Fail;
                        item.CheckOpinion = opinion;
                        item.UpdateTime = DateTime.Now;
                    }
                }
                else
                {
                    foreach (var item in tb)
                    {
                        item.CheckStatus = Code.EnumHelper.CheckStatus.Success;
                        item.CheckOpinion = opinion;
                        item.UpdateTime = DateTime.Now;


                        var tbElectiveGroup = (from p in db.Table<Entity.tbElectiveGroup>() where p.tbElective.Id == item.tbElective.Id select p).FirstOrDefault();
                        var tbElectiveSection = (from p in db.Table<Entity.tbElectiveSection>() where p.tbElective.Id == item.tbElective.Id select p).FirstOrDefault();
                        var tbTeacher = (from p in db.Table<Teacher.Entity.tbTeacher>() where p.tbSysUser.Id == item.tbSysUser.Id select p).FirstOrDefault();

                        var tbElectiveApplySchedule = (from p in db.Table<Entity.tbElectiveApplySchedule>().Include(p => p.tbPeriod).Include(p => p.tbWeek)
                                                       where p.tbElectiveApply.Id == item.Id
                                                       select p).ToList();

                        //根据星期节次创建多个ElectiveOrg
                        if (item.IsMultiClass)
                        {
                            //获取星期、节次
                            foreach (var schedule in tbElectiveApplySchedule)
                            {
                                //插入ElectiveOrg
                                var tbElectiveOrg = new Entity.tbElectiveOrg()
                                {
                                    tbElective = item.tbElective,
                                    IsPermitClass = false,
                                    MaxCount = item.MaxStudent,
                                    OrgName = $"{item.tbCourse.CourseName}_{tbElectiveApplySchedule.IndexOf(schedule)}",
                                    tbCourse = item.tbCourse,
                                    RemainCount = item.MaxStudent,
                                    tbElectiveGroup = tbElectiveGroup,
                                    tbElectiveSection = tbElectiveSection,
                                    tbRoom = item.tbRoom,
                                    tbTeacher = tbTeacher
                                };
                                db.Set<Entity.tbElectiveOrg>().Add(tbElectiveOrg);

                                //插入tbElectiveOrgSchedule
                                db.Set<Entity.tbElectiveOrgSchedule>().Add(new Entity.tbElectiveOrgSchedule()
                                {
                                    tbElectiveOrg = tbElectiveOrg,
                                    tbWeek = schedule.tbWeek,
                                    tbPeriod = schedule.tbPeriod
                                });

                            }
                        }
                        else
                        {
                            //根据星期节次创建一个ElectiveOrg
                            var tbElectiveOrg = new Entity.tbElectiveOrg()
                            {
                                tbElective = item.tbElective,
                                IsPermitClass = false,
                                MaxCount = item.MaxStudent,
                                OrgName = item.tbCourse.CourseName,
                                tbCourse = item.tbCourse,
                                RemainCount = item.MaxStudent,
                                tbElectiveGroup = tbElectiveGroup,
                                tbElectiveSection = tbElectiveSection,
                                tbRoom = item.tbRoom,
                                tbTeacher = tbTeacher
                            };

                            db.Set<Entity.tbElectiveOrg>().Add(tbElectiveOrg);

                            db.Set<Entity.tbElectiveOrgSchedule>().AddRange(tbElectiveApplySchedule.Select(p => new Entity.tbElectiveOrgSchedule()
                            {
                                tbElectiveOrg = tbElectiveOrg,
                                tbPeriod = p.tbPeriod,
                                tbWeek = p.tbWeek
                            }));
                        }
                    }
                }

                db.SaveChanges();
            }

            return Code.MvcHelper.Post(message: "操作成功!");
        }


        public ActionResult Schedule(int applyId)
        {
            var vm = new Models.ElectiveApplySchedule.List();


            using (var db = new XkSystem.Models.DbContext())
            {

                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectList();

                if (applyId == 0)
                {
                    return View(vm);
                }
                var tb = (from p in db.Table<Entity.tbElectiveApplySchedule>()
                          where p.tbElectiveApply.Id == applyId
                          select new Dto.ElectiveApplySchedule.List()
                          {
                              WeekId = p.tbWeek.Id,
                              WeekName = p.tbWeek.WeekName,
                              PeriodId = p.tbPeriod.Id,
                              PeriodName = p.tbPeriod.PeriodName
                          }).ToList();

                vm.ScheduleList = tb;

                return View(vm);

            }

        }



        /// <summary>
        /// 直接输入课程名称并创建
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateCourse()
        {
            var vm = new Models.ElectiveApply.CourseEdit();
            using (var db = new XkSystem.Models.DbContext())
            {
                if (vm.ElectiveId == 0)
                {
                    vm.IsError = true;
                    return View(vm);
                }

                vm.ApplyCourse.CourseName = vm.CourseName;
                vm.CourseSubjectList = ElectiveSubjectController.SelectSubjectList(vm.ElectiveId);
                vm.CourseTypeList = Course.Controllers.CourseTypeController.SelectList();
                vm.CourseDomainList = Course.Controllers.CourseDomainController.SelectList();
                vm.CourseGroupList = Course.Controllers.CourseGroupController.SelectList();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCourse(Models.ElectiveApply.CourseEdit vm)
        {

            using (var db = new XkSystem.Models.DbContext())
            {
                if (db.Table<Course.Entity.tbCourse>().Where(d => d.CourseName == vm.ApplyCourse.CourseName && d.Id != vm.ApplyCourse.Id).Any())
                {
                    return Code.MvcHelper.Post(new List<string>() { "系统中已存在相同名称的课程！" });
                }

                var tbCourse = new Course.Entity.tbCourse()
                {
                    tbSubject = db.Set<Course.Entity.tbSubject>().Find(vm.ApplyCourse.SubjectId),
                    tbCourseType = db.Set<Course.Entity.tbCourseType>().Find(vm.ApplyCourse.CourseTypeId),
                    CourseName = vm.ApplyCourse.CourseName,
                    CourseCode = vm.ApplyCourse.CourseCode,
                    CourseNameEn = vm.ApplyCourse.CourseNameEn,
                    tbCourseDomain = db.Set<Course.Entity.tbCourseDomain>().Find(vm.ApplyCourse.CourseDomainId),
                    tbCourseGroup = db.Set<Course.Entity.tbCourseGroup>().Find(vm.ApplyCourse.CourseGroupId),
                    Point = vm.ApplyCourse.Point,
                    Hour = vm.ApplyCourse.Hour,
                    Remark = vm.ApplyCourse.Remark,
                };
                db.Set<Course.Entity.tbCourse>().Add(tbCourse);
                db.SaveChanges();


                return Code.MvcHelper.Post(returnUrl: Url.Action("Edit", new
                {
                    ElectiveId = vm.ElectiveId,
                    CourseId = tbCourse.Id
                }));
            }


        }


    }
}