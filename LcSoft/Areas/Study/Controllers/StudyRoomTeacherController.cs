using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Study.Controllers
{
    public class StudyRoomTeacherController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyRoomTeacher.List();

                vm.RoomName = db.Set<Basis.Entity.tbRoom>().Find(vm.RoomId).RoomName;

                vm.WeekList = Basis.Controllers.WeekController.SelectList();

                var tb = from p in db.Table<Study.Entity.tbStudyRoomTeacher>()
                         orderby p.tbRoom.No
                         where p.tbStudy.Id == vm.StudyId
                         && p.tbRoom.Id == vm.RoomId
                         && p.tbRoom.IsDeleted == false
                         && p.tbTeacher.IsDeleted == false
                         && p.tbStudy.IsDeleted == false
                         && p.tbWeek.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText));
                }

                vm.StudyRoomTeacherList = (from p in tb
                                           orderby p.tbTeacher.TeacherName
                                           select new Dto.StudyRoomTeacher.List
                                           {
                                               Id = p.Id,
                                               RoomId = p.tbRoom.Id,
                                               RoomName = p.tbRoom.RoomName,
                                               IsMaster = p.IsMaster,
                                               TeacherId = p.tbTeacher.Id,
                                               TeacherCode = p.tbTeacher.TeacherCode,
                                               TeacherName = p.tbTeacher.TeacherName,
                                               WeekId = p.tbWeek.Id
                                           }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudyRoomTeacher.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                studyId = vm.StudyId,
                roomId = vm.RoomId
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int studyId, int roomId, int teacherId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Study.Entity.tbStudyRoomTeacher>()
                          where p.tbStudy.Id == studyId
                          && p.tbRoom.Id == roomId
                          && p.tbTeacher.Id == teacherId
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除晚自习教管");
                }
                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int roomId = 0, int teacherId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyRoomTeacher.Edit();
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.TeacherList = Teacher.Controllers.TeacherController.SelectList();
                if (roomId != 0 && teacherId != 0 && vm.StudyId != 0)
                {
                    vm.StudyRoomTeacherEdit = (from p in db.Table<Study.Entity.tbStudyRoomTeacher>()
                                               where p.tbStudy.Id == vm.StudyId
                                               && p.tbRoom.Id == roomId
                                               && p.tbTeacher.Id == teacherId
                                               && p.tbRoom.IsDeleted == false
                                               && p.tbStudy.IsDeleted == false
                                               && p.tbTeacher.IsDeleted == false
                                               select new Dto.StudyRoomTeacher.Edit
                                               {
                                                   Id = p.Id,
                                                   IsMaster = p.IsMaster
                                               }).FirstOrDefault();

                    vm.WeekIdList = (from p in db.Table<Study.Entity.tbStudyRoomTeacher>()
                                     where p.tbStudy.Id == vm.StudyId
                                     && p.tbRoom.Id == roomId
                                     && p.tbTeacher.Id == teacherId
                                     && p.tbRoom.IsDeleted == false
                                     && p.tbStudy.IsDeleted == false
                                     && p.tbTeacher.IsDeleted == false
                                     select p.tbWeek.Id).Distinct().ToList();
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.StudyRoomTeacher.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var weekIds = new List<int>();
                    if (Request["CboxWeek"] != null)
                    {
                        weekIds = Request["CboxWeek"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                    }

                    var tbStudyRoomTeacherList = (from p in db.Table<Study.Entity.tbStudyRoomTeacher>()
                                                   .Include(d => d.tbRoom)
                                                   .Include(d => d.tbTeacher)
                                                   .Include(d => d.tbWeek)
                                                  where p.tbRoom.Id == vm.RoomId
                                                  && p.tbStudy.Id == vm.StudyId
                                                  && p.tbTeacher.Id == vm.TeacherId
                                                  && p.tbRoom.IsDeleted == false
                                                  && p.tbStudy.IsDeleted == false
                                                  && p.tbTeacher.IsDeleted == false
                                                  select p).ToList();

                    foreach (var a in tbStudyRoomTeacherList.Where(d => weekIds.Contains(d.tbWeek.Id) == false))
                    {
                        a.IsDeleted = true;
                    }

                    foreach (var a in weekIds.Where(d => tbStudyRoomTeacherList.Select(q => q.tbWeek.Id).Contains(d) == false))
                    {
                        var tb = new Study.Entity.tbStudyRoomTeacher();
                        tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.TeacherId);
                        tb.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.RoomId);
                        tb.tbWeek = db.Set<Basis.Entity.tbWeek>().Find(a);
                        tb.tbStudy = db.Set<Study.Entity.tbStudy>().Find(vm.StudyId);
                        tb.IsMaster = vm.StudyRoomTeacherEdit.IsMaster;
                        db.Set<Study.Entity.tbStudyRoomTeacher>().Add(tb);
                    }

                    foreach (var a in tbStudyRoomTeacherList.Where(d => weekIds.Contains(d.tbWeek.Id) == true))
                    {
                        a.IsMaster = vm.StudyRoomTeacherEdit.IsMaster;
                    }

                    if (db.SaveChanges() > 0)
                    {
                        if (vm.StudyRoomTeacherEdit.IsMaster)
                        {
                            var tbStudyRoomTeacher = (from p in db.Table<Study.Entity.tbStudyRoomTeacher>()
                                                       .Include(d => d.tbRoom)
                                                      where p.tbRoom.Id == vm.RoomId
                                                      && p.tbStudy.Id == vm.StudyId
                                                      && p.tbTeacher.Id != vm.TeacherId
                                                      && p.tbRoom.IsDeleted == false
                                                      && p.tbStudy.IsDeleted == false
                                                      && p.tbTeacher.IsDeleted == false
                                                      select p).ToList();
                            foreach (var a in tbStudyRoomTeacher)
                            {
                                a.IsMaster = false;
                            }
                            db.SaveChanges();
                        }
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改晚自习教管");
                    }
                }
                return Code.MvcHelper.Post(null, Url.Action("List", new
                {
                    searchText = vm.SearchText,
                    studyId = vm.StudyId,
                    roomId = vm.RoomId
                }));
            }
        }
    }
}