using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Attendance.Controllers
{
    public class AttendanceTypeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.AttendanceType.List();
                var tb = from p in db.Table<Attendance.Entity.tbAttendanceType>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.AttendanceTypeName.Contains(vm.SearchText));
                }

                vm.AttendanceTypeList = (from p in tb
                                                orderby p.No
                                                select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.AttendanceType.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Attendance.Entity.tbAttendanceType>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除学生考勤类型");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.AttendanceType.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Attendance.Entity.tbAttendanceType>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.AttendanceTypeEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.AttendanceType.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.AttendanceTypeEdit.Id == 0)
                    {
                        var tb = new Attendance.Entity.tbAttendanceType();
                        tb.No = vm.AttendanceTypeEdit.No == null ? db.Table<Attendance.Entity.tbAttendanceType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.AttendanceTypeEdit.No;
                        tb.AttendanceTypeName = vm.AttendanceTypeEdit.AttendanceTypeName;
                        db.Set<Attendance.Entity.tbAttendanceType>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加学生考勤类型");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Attendance.Entity.tbAttendanceType>()
                                  where p.Id == vm.AttendanceTypeEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.AttendanceTypeEdit.No == null ? db.Table<Attendance.Entity.tbAttendanceType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.AttendanceTypeEdit.No;
                            tb.AttendanceTypeName = vm.AttendanceTypeEdit.AttendanceTypeName;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改学生考勤类型");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Attendance.Entity.tbAttendanceType>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.AttendanceTypeName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectAbnormalList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Attendance.Entity.tbAttendanceType>()
                          where p.AttendanceValue != 0
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.AttendanceTypeName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}