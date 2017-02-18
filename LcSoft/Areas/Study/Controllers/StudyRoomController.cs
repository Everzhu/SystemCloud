using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Study.Controllers
{
    public class StudyRoomController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyRoom.List();
                var tb = from p in db.Table<Study.Entity.tbStudyRoom>()
                         where p.tbStudy.Id == vm.StudyId
                         && p.tbRoom.IsDeleted == false
                         && p.tbStudy.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbRoom.RoomName.Contains(vm.SearchText));
                }

                vm.StudyRoomList = (from p in tb
                                    orderby p.tbRoom.No
                                    select new Dto.StudyRoom.List
                                    {
                                        Id = p.Id,
                                        RoomId = p.tbRoom.Id,
                                        RoomName = p.tbRoom.RoomName,
                                        StudyName = p.tbStudy.StudyName,
                                        BuildName = p.tbRoom.tbBuild.BuildName,
                                        RoomTypeName = p.tbRoom.tbRoomType.RoomTypeName,
                                        MaxUser = p.tbRoom.MaxUser,
                                        StudentCount = db.Set<Study.Entity.tbStudyRoomStudent>().Where(d => d.IsDeleted == false && d.tbStudent.IsDeleted == false && d.tbStudy.IsDeleted == false && d.tbRoom.IsDeleted == false && d.tbStudy.Id == p.tbStudy.Id && d.tbRoom.Id == p.tbRoom.Id).Distinct().Count(),
                                        TeacherName = "",
                                    }).ToPageList(vm.Page);

                var roomTeacherList = (from p in db.Table<Study.Entity.tbStudyRoomTeacher>()
                                       where p.tbRoom.IsDeleted == false
                                        && p.tbStudy.IsDeleted == false
                                        && p.tbTeacher.IsDeleted == false
                                        && p.tbWeek.IsDeleted == false
                                        && p.tbStudy.Id == vm.StudyId
                                       select new
                                       {
                                           roomId = p.tbRoom.Id,
                                           TeacherName = p.tbTeacher.TeacherName
                                       }).Distinct().ToList();

                foreach (var a in vm.StudyRoomList)
                {
                    a.TeacherName = string.Join(",", roomTeacherList.Where(d => d.roomId == a.RoomId).Select(d => d.TeacherName));
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudyRoom.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                studyId = vm.StudyId,
                searchText = vm.SearchText,
                pageSize = vm.Page.PageSize,
                pageCount = vm.Page.PageCount,
                pageIndex = vm.Page.PageIndex
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Study.Entity.tbStudyRoom>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了晚自习教室");
                }

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(List<int> ids, int studyId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                var tbStudyRoomIdList = (from p in db.Table<Study.Entity.tbStudyRoom>()
                                         .Include(d => d.tbRoom)
                                         .Include(d => d.tbStudy)
                                         where p.tbStudy.Id == studyId
                                         && p.tbStudy.IsDeleted == false
                                         && p.tbRoom.IsDeleted == false
                                         select p).ToList();

                foreach (var a in tbStudyRoomIdList.Where(d => ids.Contains(d.tbRoom.Id) == false))
                {
                    a.IsDeleted = true;
                }

                foreach (var a in ids.Where(d => tbStudyRoomIdList.Select(q => q.tbRoom.Id).Contains(d) == false))
                {
                    var tb = new Study.Entity.tbStudyRoom();
                    tb.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(a);
                    tb.tbStudy = db.Set<Study.Entity.tbStudy>().Find(studyId);
                    db.Set<Study.Entity.tbStudyRoom>().Add(tb);
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了晚自习教室");
                }

                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult Export()
        {
            var vm = new Models.StudyRoom.List();

            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var tb = from p in db.Table<Study.Entity.tbStudyRoom>()
                         where p.tbStudy.Id == vm.StudyId
                         && p.tbRoom.IsDeleted == false
                         && p.tbStudy.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbRoom.RoomName.Contains(vm.SearchText));
                }

                vm.StudyRoomList = (from p in tb
                                    orderby p.tbRoom.No
                                    select new Dto.StudyRoom.List
                                    {
                                        Id = p.Id,
                                        RoomId = p.tbRoom.Id,
                                        RoomName = p.tbRoom.RoomName,
                                        StudyName = p.tbStudy.StudyName,
                                        BuildName = p.tbRoom.tbBuild.BuildName,
                                        RoomTypeName = p.tbRoom.tbRoomType.RoomTypeName,
                                        MaxUser = p.tbRoom.MaxUser,
                                        StudentCount = db.Set<Study.Entity.tbStudyRoomStudent>().Where(d => d.IsDeleted == false && d.tbStudent.IsDeleted == false && d.tbStudy.IsDeleted == false && d.tbRoom.IsDeleted == false && d.tbStudy.Id == p.tbStudy.Id && d.tbRoom.Id == p.tbRoom.Id).Distinct().Count(),
                                        TeacherName = "",
                                    }).ToList();

                var roomTeacherList = (from p in db.Table<Study.Entity.tbStudyRoomTeacher>()
                                       where p.tbRoom.IsDeleted == false
                                        && p.tbStudy.IsDeleted == false
                                        && p.tbTeacher.IsDeleted == false
                                        && p.tbWeek.IsDeleted == false
                                        && p.tbStudy.Id == vm.StudyId
                                       select new
                                       {
                                           roomId = p.tbRoom.Id,
                                           TeacherName = p.tbTeacher.TeacherName
                                       }).Distinct().ToList();

                foreach (var a in vm.StudyRoomList)
                {
                    a.TeacherName = string.Join(",", roomTeacherList.Where(d => d.roomId == a.RoomId).Select(d => d.TeacherName));
                }

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("序号"),
                        new System.Data.DataColumn("晚自习名称"),
                        new System.Data.DataColumn("教学楼"),
                        new System.Data.DataColumn("教室"),
                        new System.Data.DataColumn("教室类型"),
                        new System.Data.DataColumn("教管"),
                        new System.Data.DataColumn("容纳人数"),
                        new System.Data.DataColumn("学生人数")
                    });
                var index = 0;
                foreach (var a in vm.StudyRoomList)
                {
                    index++;
                    var dr = dt.NewRow();
                    dr["序号"] = index.ToString();
                    dr["晚自习名称"] = a.StudyName;
                    dr["教学楼"] = a.BuildName;
                    dr["教室"] = a.RoomName;
                    dr["教室类型"] = a.RoomTypeName;
                    dr["教管"] = a.TeacherName;
                    dr["容纳人数"] = a.MaxUser;
                    dr["学生人数"] = a.StudentCount;
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

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int studyId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Study.Entity.tbStudyRoom>()
                          where p.tbStudy.Id == studyId
                          && p.tbStudy.IsDeleted == false
                          && p.tbRoom.IsDeleted == false
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.tbRoom.RoomName,
                              Value = p.tbRoom.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}