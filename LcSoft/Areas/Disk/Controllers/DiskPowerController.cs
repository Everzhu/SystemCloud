using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Disk.Controllers
{
    public class DiskPowerController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DiskPower.List();
                var tb = db.Set<Disk.Entity.tbDiskFolder>().Find(vm.FolderId);
                vm.FolderName = tb.DiskFolderName;

                vm.DiskPowerList = (from t in db.Table<Disk.Entity.tbDiskPower>()
                                    where t.tbDiskFolder.Id == vm.FolderId
                                      && (String.IsNullOrEmpty(vm.SearchText)
                                          || t.tbSysUser.UserCode.Contains(vm.SearchText)
                                          || t.tbSysUser.UserName.Contains(vm.SearchText)
                                      )
                                    orderby t.tbSysUser.UserCode
                                    select new Dto.DiskPower.List
                                    {
                                        Id = t.Id,
                                        DiskFolderId = t.tbDiskFolder.Id,
                                        UserId = t.tbSysUser.Id,
                                        UserCode = t.tbSysUser.UserCode,
                                        UserName = t.tbSysUser.UserName,
                                        UserType = t.tbSysUser.UserType,
                                        IsAdmin = t.IsAdmin,
                                        IsInput = t.IsInput,
                                        IsView = t.IsView
                                    }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DiskPower.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                folderId = vm.FolderId,
                searchText = vm.SearchText,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Disk.Entity.tbDiskPower>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var t in tb)
                {
                    t.IsDeleted = true;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(List<int> ids, int folderId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var folder = db.Set<Disk.Entity.tbDiskFolder>().Find(folderId);

                List<Disk.Entity.tbDiskPower> lst = new List<Disk.Entity.tbDiskPower>();
                foreach (int id in ids)
                {
                    var tb = (from t in db.Table<Disk.Entity.tbDiskPower>()
                              where t.tbSysUser.Id == id
                                && t.tbDiskFolder.Id == folderId
                              select t).FirstOrDefault();
                    if (tb == null)
                    {
                        tb = new Disk.Entity.tbDiskPower();
                        tb.IsView = true;
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(id);
                        tb.tbDiskFolder = folder;

                        lst.Add(tb);
                    }
                    else
                    {
                        tb.IsView = true;
                    }
                }

                db.Set<Disk.Entity.tbDiskPower>().AddRange(lst);
                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetAdmin(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskPower>()
                          where t.Id == id
                          select t).FirstOrDefault();
                if (tb != null)
                {
                    tb.IsAdmin = !tb.IsAdmin;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetInput(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskPower>()
                          where t.Id == id
                          select t).FirstOrDefault();
                if (tb != null)
                {
                    tb.IsInput = !tb.IsInput;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetView(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskPower>()
                          where t.Id == id
                          select t).FirstOrDefault();
                if (tb != null)
                {
                    tb.IsView = !tb.IsView;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        [NonAction]
        public static bool CheckAdmin(int folderId, int userId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskPower>()
                          where t.tbDiskFolder.Id == folderId
                            && t.tbSysUser.Id == userId
                            && t.IsAdmin == true
                          select t).Any();

                return tb;
            }
        }

        [NonAction]
        public static List<int> SelectAdminFolderIds(int userId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskPower>()
                          where t.tbSysUser.Id == userId
                            && t.tbDiskFolder.IsDeleted == false
                            && t.tbDiskFolder.DiskPermit == Code.EnumHelper.DiskPermit.Authorize
                            && t.IsAdmin == true
                          select t.tbDiskFolder.Id).ToList();

                return tb;
            }
        }

        [NonAction]
        public static List<int> SelectInputFolderIds(int userId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskPower>()
                          where t.tbSysUser.Id == userId
                            && t.tbDiskFolder.IsDeleted == false
                            && t.tbDiskFolder.DiskPermit == Code.EnumHelper.DiskPermit.Authorize
                            && t.IsInput == true
                          select t.tbDiskFolder.Id).ToList();

                return tb;
            }
        }

        [NonAction]
        public static List<int> SelectViewFolderIds(int userId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskPower>()
                          where t.tbSysUser.Id == userId
                            && t.tbDiskFolder.IsDeleted == false
                            && t.tbDiskFolder.DiskPermit == Code.EnumHelper.DiskPermit.Authorize
                            && t.IsView == true
                          select t.tbDiskFolder.Id).ToList();

                return tb;
            }
        }

        [NonAction]
        public static bool CheckInput(int folderId, int userId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskPower>()
                          where t.tbDiskFolder.Id == folderId
                            && t.tbSysUser.Id == userId
                            && t.IsInput == true
                          select t).Any();

                return tb;
            }
        }

        [NonAction]
        public static void DeleteByFolderId(int folderId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskPower>()
                          where t.tbDiskFolder.Id == folderId
                          select t).ToList();

                if (tb.Count > 0)
                {
                    db.Set<Disk.Entity.tbDiskPower>().RemoveRange(tb);
                    db.SaveChanges();
                }
            }
        }

        [NonAction]
        public static List<Dto.DiskFolder.ReportUser> SelectReportUserList(int parentId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskPower>()
                          where t.tbDiskFolder.tbDiskFolderParent.Id == parentId
                           && t.tbDiskFolder.IsDeleted == false
                          select new Dto.DiskFolder.ReportUser
                          {
                              FolderId = t.tbDiskFolder.Id,
                              UserId = t.tbSysUser.Id,
                              UserCode = t.tbSysUser.UserCode,
                              UserName = t.tbSysUser.UserName
                          }).Distinct().ToList();

                return tb;
            }
        }
    }
}