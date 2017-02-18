using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.ComponentModel;

namespace XkSystem.Areas.Disk.Controllers
{
    public class DiskFolderController : Controller
    {
        [Description("文件管理-文件夹-编辑显示")]
        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DiskFolder.Edit();

                var tbDiskType = DiskTypeController.SelectInfo(vm.DiskTypeId);
                if (tbDiskType != null && tbDiskType.DiskType == Code.EnumHelper.DiskType.Public)
                {
                    if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator)
                    {
                        return Content("只有管理员才可以新建公开文件夹！");
                    }
                    else
                    {
                        // 管理员只能新建【学校共享文件夹】的子文件夹
                        var tbDiskFolder = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                                            where t.DiskFolderName.Equals("学校共享文件夹")
                                               && t.tbDiskType.DiskType == Code.EnumHelper.DiskType.Public
                                            select t).FirstOrDefault();
                        if (tbDiskFolder != null)
                        {
                            List<int> ids = new List<int>();
                            ids.Add(tbDiskFolder.Id);

                            if (!DiskFolderController.SelectDescendants(ids).Select(t => t.Id).Contains(vm.FolderId))
                            {
                                return Content("只能在学校共享文件夹中新建子文件夹！");
                            }
                            else
                            {
                                vm.DiskFolderEdit.DiskPermit = Code.EnumHelper.DiskPermit.Public;
                            }
                        }
                    }
                }

                vm.DiskTypeList = Disk.Controllers.DiskTypeController.SelectInfoList();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Disk.Entity.tbDiskFolder>()
                              where p.Id == id
                              select new Dto.DiskFolder.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  DiskFolderName = p.DiskFolderName,
                                  DiskTypeId = p.tbDiskType.Id,
                                  DiskPermit = p.DiskPermit
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.DiskFolderEdit = tb;
                    }
                }
                else
                {
                    vm.DiskFolderEdit.DiskTypeId = (tbDiskType == null || tbDiskType.DiskType == Code.EnumHelper.DiskType.Private)
                        ? vm.DiskTypeList.Where(t => t.DiskType == Code.EnumHelper.DiskType.Private).FirstOrDefault().Id
                        : vm.DiskTypeList.Where(t => t.DiskType == Code.EnumHelper.DiskType.Public).FirstOrDefault().Id;
                }

                return View(vm);
            }
        }

        [Description("文件管理-文件夹-编辑提交")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DiskFolder.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                #region 新建公共文件夹校验

                var diskType = db.Set<Disk.Entity.tbDiskType>().Find(vm.DiskFolderEdit.DiskTypeId);
                if (diskType.DiskType == Code.EnumHelper.DiskType.Public && Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator)
                {
                    error.AddError("需要管理员身份新建公开文件夹!");
                }

                #endregion

                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Disk.Entity.tbDiskFolder>().Where(d => d.DiskFolderName == vm.DiskFolderEdit.DiskFolderName
                        && d.tbSysUser.Id == Code.Common.UserId
                        && d.tbDiskFolderParent.Id == vm.DiskFolderEdit.ParentId
                        && d.Id != vm.DiskFolderEdit.Id).Any())
                    {
                        error.AddError("该文件夹名称已存在!");
                    }
                    else
                    {
                        if (vm.DiskFolderEdit.Id == 0)
                        {
                            var tb = new Disk.Entity.tbDiskFolder();
                            tb.No = vm.DiskFolderEdit.No == null
                                ? db.Table<Disk.Entity.tbDiskFolder>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1
                                : (int)vm.DiskFolderEdit.No;
                            tb.DiskFolderName = vm.DiskFolderEdit.DiskFolderName;
                            tb.tbDiskType = diskType;
                            tb.DiskPermit = vm.DiskFolderEdit.DiskPermit;
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);

                            if (vm.FolderId != 0)
                            {
                                if (diskType.DiskType == Code.EnumHelper.DiskType.Public)
                                {
                                    tb.tbDiskFolderParent = db.Table<Disk.Entity.tbDiskFolder>()
                                        .Where(t => t.Id == vm.FolderId)
                                        .FirstOrDefault();
                                }
                                else
                                {
                                    tb.tbDiskFolderParent = db.Table<Disk.Entity.tbDiskFolder>()
                                        .Where(t => t.Id == vm.FolderId && t.tbSysUser.Id == Code.Common.UserId)
                                        .FirstOrDefault();
                                }
                            }

                            db.Set<Disk.Entity.tbDiskFolder>().Add(tb);
                        }
                        else
                        {
                            var tb = (from p in db.Table<Disk.Entity.tbDiskFolder>()
                                      where p.Id == vm.DiskFolderEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.DiskFolderEdit.No == null ? db.Table<Disk.Entity.tbDiskFolder>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.DiskFolderEdit.No;
                                tb.DiskFolderName = vm.DiskFolderEdit.DiskFolderName;
                                tb.DiskPermit = vm.DiskFolderEdit.DiskPermit;

                                // 同时设置子文件夹权限
                                var tbDiskFolderDescendants = DiskFolderController.SelectDescendants(new List<int> { tb.Id });
                                foreach (var folder in tbDiskFolderDescendants)
                                {
                                    if (folder.Id != vm.DiskFolderEdit.Id)
                                    {
                                        var tbDiskFolder = db.Set<Disk.Entity.tbDiskFolder>().Find(folder.Id);
                                        if (tbDiskFolder != null)
                                        {
                                            tbDiskFolder.DiskPermit = vm.DiskFolderEdit.DiskPermit;
                                        }
                                    }

                                    // 删除文件夹授权
                                    if (vm.DiskFolderEdit.DiskPermit != Code.EnumHelper.DiskPermit.Authorize)
                                    {
                                        DiskPowerController.DeleteByFolderId(folder.Id);
                                    }
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                db.SaveChanges();

                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id = 0, int folderId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (id != 0)
                {
                    var tb = (from p in db.Table<Disk.Entity.tbDiskFolder>()
                              where p.Id == id && p.tbSysUser.Id == Code.Common.UserId
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.IsDeleted = true;
                    }
                }

                db.SaveChanges();

                return Code.MvcHelper.Post(null, Url.Action("List", "DiskFile", new
                {
                    folderId = folderId
                }), "删除成功！");
            }
        }

        public ActionResult MoveTo(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DiskFolder.Edit();

                if (id != 0)
                {
                    vm.DiskFolderEdit.Id = id;
                }

                if (vm.UserId == 0)
                {
                    vm.UserId = Code.Common.UserId;
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoveTo(Models.DiskFolder.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                List<int> ids = new List<int>();
                ids.Add(vm.DiskFolderEdit.Id);
                List<Dto.DiskFolder.Info> folderDescendants = DiskFolderController.SelectDescendants(ids);

                if (folderDescendants.Select(t => t.Id).Contains(vm.DiskFolderEdit.ParentId))
                {
                    error.AddError("文件夹不能移动到自身及子文件夹中!");
                }

                if (error.Count() == 0)
                {
                    var tb = (from p in db.Table<Disk.Entity.tbDiskFolder>()
                              .Include(p => p.tbDiskFolderParent)
                              where p.Id == vm.DiskFolderEdit.Id && p.tbSysUser.Id == Code.Common.UserId
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        if (vm.DiskFolderEdit.ParentId == 0)
                        {
                            tb.tbDiskFolderParent = null;
                        }
                        else
                        {
                            tb.tbDiskFolderParent = db.Set<Disk.Entity.tbDiskFolder>().Find(vm.DiskFolderEdit.ParentId);
                        }
                    }

                    db.SaveChanges();
                }

                return Code.MvcHelper.Post(error, Url.Action("List", "DiskFile", new
                {
                    folderId = vm.FolderId
                }), "移动成功！");
            }
        }

        public ActionResult GetFolderTree(int userId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Disk.Entity.tbDiskFolder>()
                          where p.tbSysUser.Id == userId
                          select new Dto.DiskFolder.Info
                          {
                              Id = p.Id,
                              DiskFolderName = p.DiskFolderName,
                              No = p.No,
                              ParentId = p.tbDiskFolderParent == null ? 0 : p.tbDiskFolderParent.Id
                          }).ToList();
                var result = new List<Code.TreeHelper>();

                result.Add(new Code.TreeHelper()
                {
                    Id = 0,
                    name = "根目录",
                    open = true,
                    children = AddChildrenTreeByFolderId(tb, 0)
                });

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        private List<Code.TreeHelper> AddChildrenTreeByFolderId(List<Dto.DiskFolder.Info> tb, int parentId)
        {
            List<Code.TreeHelper> folderTree = new List<Code.TreeHelper>();

            foreach (var folder in tb.Where(d => d.ParentId == parentId).OrderBy(d => d.No))
            {
                List<Code.TreeHelper> cn = AddChildrenTreeByFolderId(tb, folder.Id);

                folderTree.Add(new Code.TreeHelper()
                {
                    Id = folder.Id,
                    pId = folder.ParentId,
                    name = folder.DiskFolderName,
                    open = true,
                    children = cn
                });
            }

            return folderTree;
        }

        public ActionResult Report()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DiskFolder.Report();

                if (vm.ParentId == 0)
                {
                    var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                              where t.DiskFolderName == "学校共享文件夹"
                                && t.tbDiskType.DiskType == Code.EnumHelper.DiskType.Public
                              select t).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ParentId = tb.Id;
                    }
                }

                var expr = from t in db.Table<Disk.Entity.tbDiskFolder>()
                           where t.tbDiskType.DiskType == Code.EnumHelper.DiskType.Public
                             && t.DiskPermit != Code.EnumHelper.DiskPermit.Private
                             && t.tbDiskFolderParent.Id == vm.ParentId
                           select new Dto.DiskFolder.Report
                           {
                               Id = t.Id,
                               DiskFolderName = t.DiskFolderName,
                               DiskPermit = t.DiskPermit
                           };
                if (!String.IsNullOrWhiteSpace(vm.SearchText))
                {
                    expr = expr.Where(t => t.DiskFolderName.Contains(vm.SearchText));
                }

                vm.FolderList = expr.ToList();
                vm.DiskPowerUserList = DiskPowerController.SelectReportUserList(vm.ParentId);
                vm.DiskFileUserList = DiskFileController.SelectReportUserList(vm.ParentId);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Report(Models.DiskFolder.Report vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("Report", new
            {
                parentId = vm.ParentId,
                searchText = vm.SearchText
            }));
        }

        [NonAction]
        public static List<Dto.DiskFile.List> SelectPublicRootFolder()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var diskType = (from t in db.Table<Disk.Entity.tbDiskType>()
                                where t.DiskType == Code.EnumHelper.DiskType.Public
                                select t).FirstOrDefault();
                var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                          where t.tbDiskFolderParent == null
                            && t.tbDiskType.Id == diskType.Id
                          orderby t.No
                          select new Dto.DiskFile.List
                          {
                              Id = t.Id,
                              FileType = "Folder",
                              DiskTypeId = t.tbDiskType.Id,
                              FileTitle = t.DiskFolderName,
                              DiskPermit = t.DiskPermit,
                              IsAdmin = false
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<Dto.DiskFile.List> SelectPublicFolderByParentId(int parentId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                          where t.tbDiskFolderParent.Id == parentId
                            && t.tbDiskType.DiskType == Code.EnumHelper.DiskType.Public
                          orderby t.No
                          select new Dto.DiskFile.List
                          {
                              Id = t.Id,
                              FileType = "Folder",
                              DiskTypeId = t.tbDiskType.Id,
                              FileTitle = t.DiskFolderName,
                              DiskPermit = t.DiskPermit
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<Dto.DiskFile.List> SelectPublicFolderByParentId(int parentId, Code.EnumHelper.DiskPermit enumDiskPermit)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                          where t.tbDiskFolderParent.Id == parentId
                            && t.DiskPermit == enumDiskPermit
                            && t.tbDiskType.DiskType == Code.EnumHelper.DiskType.Public
                          orderby t.No
                          select new Dto.DiskFile.List
                          {
                              Id = t.Id,
                              FileType = "Folder",
                              DiskTypeId = t.tbDiskType.Id,
                              FileTitle = t.DiskFolderName,
                              DiskPermit = t.DiskPermit
                          }).ToList();
                return tb;
            }
        }
        [NonAction]
        public static List<Dto.DiskFile.List> SelectPublicFolderAuthorizeByParentId(int parentId, Code.EnumHelper.DiskPermit enumDiskPermit)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskPower>()
                          where t.tbDiskFolder.tbDiskType.DiskType == Code.EnumHelper.DiskType.Public
                            && t.tbDiskFolder.tbDiskFolderParent.Id == parentId
                            && t.tbDiskFolder.DiskPermit == Code.EnumHelper.DiskPermit.Authorize
                            && t.tbSysUser.Id == Code.Common.UserId
                            && t.IsView == true
                          select new Dto.DiskFile.List
                          {
                              Id = t.tbDiskFolder.Id,
                              FileType = "Folder",
                              DiskTypeId = t.tbDiskFolder.tbDiskType.Id,
                              FileTitle = t.tbDiskFolder.DiskFolderName,
                              DiskPermit = t.tbDiskFolder.DiskPermit
                          }).ToList();

                return tb;
            }
        }

        [NonAction]
        public static List<Dto.DiskFile.List> SelectByParentId(int parentId, string searchText, int diskTypeId = 0, int userId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tbDiskType = DiskTypeController.SelectInfo(diskTypeId);
                bool isPublic = tbDiskType != null && tbDiskType.DiskType == Code.EnumHelper.DiskType.Public;

                if (parentId == 0)
                {
                    var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                              where (isPublic || t.tbSysUser.Id == Code.Common.UserId)
                                 && t.tbDiskFolderParent == null
                                 && !t.DiskFolderName.Equals("公开文件夹")
                                 && (String.IsNullOrEmpty(searchText) || t.DiskFolderName.Contains(searchText))
                              orderby t.No
                              select new Dto.DiskFile.List
                              {
                                  Id = t.Id,
                                  FileType = "Folder",
                                  DiskTypeId = diskTypeId == 0 ? t.tbDiskType.Id : diskTypeId,
                                  UserId = userId,
                                  FileTitle = t.DiskFolderName,
                                  DiskPermit = t.DiskPermit
                              }).ToList();
                    return tb;
                }
                else
                {
                    var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                              where (isPublic || t.tbSysUser.Id == Code.Common.UserId)
                                 && t.tbDiskFolderParent.Id == parentId
                                 && (String.IsNullOrEmpty(searchText) || t.DiskFolderName.Contains(searchText))
                              orderby t.No
                              select new Dto.DiskFile.List
                              {
                                  Id = t.Id,
                                  FileType = "Folder",
                                  DiskTypeId = diskTypeId == 0 ? t.tbDiskType.Id : diskTypeId,
                                  UserId = userId,
                                  FileTitle = t.DiskFolderName,
                                  DiskPermit = t.DiskPermit
                              }).ToList();
                    return tb;
                }
            }
        }

        [NonAction]
        public static bool Delete(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (id != 0)
                {
                    var tb = (from p in db.Table<Disk.Entity.tbDiskFolder>()
                              where p.Id == id && p.tbSysUser.Id == Code.Common.UserId
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.IsDeleted = true;
                    }
                }

                db.SaveChanges();

                return true;
            }
        }

        [NonAction]
        public static List<Dto.DiskFolder.Info> SelectDescendants(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                List<Dto.DiskFolder.Info> folderDescendants = new List<Dto.DiskFolder.Info>();

                foreach (int id in ids)
                {
                    var tbDiskFolder = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                                        where t.Id == id
                                        select new Dto.DiskFolder.Info
                                        {
                                            Id = t.Id,
                                            DiskFolderName = t.DiskFolderName,
                                            FolderPath = t.DiskFolderName + "/"
                                        }).FirstOrDefault();
                    if (tbDiskFolder != null)
                    {
                        folderDescendants.Add(tbDiskFolder);

                        SelectLoopFolderChildren(folderDescendants, tbDiskFolder);
                    }
                }

                return folderDescendants;
            }
        }

        private static List<Dto.DiskFolder.Info> SelectLoopFolderChildren(List<Dto.DiskFolder.Info> folderDescendants, Dto.DiskFolder.Info parent)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tbDiskFolder = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                                    where t.tbDiskFolderParent.Id == parent.Id
                                    select new Dto.DiskFolder.Info
                                    {
                                        Id = t.Id,
                                        DiskFolderName = t.DiskFolderName,
                                        FolderPath = parent.FolderPath + t.DiskFolderName + "/"
                                    }).ToList();
                foreach (var folder in tbDiskFolder)
                {
                    folderDescendants.Add(folder);

                    SelectLoopFolderChildren(folderDescendants, folder);
                }

                return folderDescendants;
            }
        }

        [NonAction]
        public static List<Dto.DiskFolder.Info> SelectFolderPath(int id, int diskTypeId = 0, int userId = 0)
        {
            List<Dto.DiskFolder.Info> folderPath = new List<Dto.DiskFolder.Info>();

            if (id != 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var tbDiskFolder = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                                        where t.Id == id
                                        select new Dto.DiskFolder.Info
                                        {
                                            Id = t.Id,
                                            DiskFolderName = t.DiskFolderName,
                                            ParentId = t.tbDiskFolderParent == null ? 0 : t.tbDiskFolderParent.Id,
                                            DiskTypeId = diskTypeId == 0 ? t.tbDiskType.Id : diskTypeId,
                                            UserId = userId
                                        }).FirstOrDefault();

                    if (tbDiskFolder != null)
                    {
                        folderPath.Add(tbDiskFolder);

                        if (tbDiskFolder.ParentId != 0)
                        {
                            var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                                      where t.Id == tbDiskFolder.ParentId
                                      select new Dto.DiskFolder.Info
                                      {
                                          Id = t.Id,
                                          DiskFolderName = t.DiskFolderName,
                                          ParentId = t.tbDiskFolderParent == null ? 0 : t.tbDiskFolderParent.Id,
                                          DiskTypeId = diskTypeId == 0 ? t.tbDiskType.Id : diskTypeId,
                                          UserId = userId
                                      }).FirstOrDefault();
                            if (tb != null)
                            {
                                SelectLoopFolderPath(folderPath, tb.Id, diskTypeId, userId);
                            }
                        }

                        folderPath.Reverse();
                    }
                }
            }

            return folderPath;
        }

        private static List<Dto.DiskFolder.Info> SelectLoopFolderPath(List<Dto.DiskFolder.Info> folderPath, int id, int diskTypeId = 0, int userId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tbDiskFolder = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                                    where t.Id == id
                                    select new Dto.DiskFolder.Info
                                    {
                                        Id = t.Id,
                                        DiskFolderName = t.DiskFolderName,
                                        DiskTypeId = diskTypeId == 0 ? t.tbDiskType.Id : diskTypeId,
                                        ParentId = t.tbDiskFolderParent == null ? 0 : t.tbDiskFolderParent.Id,
                                        UserId = userId
                                    }).FirstOrDefault();

                if (tbDiskFolder != null)
                {
                    folderPath.Add(tbDiskFolder);

                    if (tbDiskFolder.ParentId != 0)
                    {
                        var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                                  where t.Id == tbDiskFolder.ParentId
                                  select new Dto.DiskFolder.Info
                                  {
                                      Id = t.Id,
                                      DiskFolderName = t.DiskFolderName,
                                      DiskTypeId = diskTypeId == 0 ? t.tbDiskType.Id : diskTypeId,
                                      ParentId = t.tbDiskFolderParent == null ? 0 : t.tbDiskFolderParent.Id,
                                      UserId = userId
                                  }).FirstOrDefault();

                        if (tb != null)
                        {
                            SelectLoopFolderPath(folderPath, tb.Id, diskTypeId, userId);
                        }
                    }
                }

                return folderPath;
            }
        }

        [NonAction]
        public static List<Dto.DiskFile.List> SelectByPublic(Code.EnumHelper.DiskType enumDiskType, int diskTypeId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                          where t.tbDiskType.DiskType == enumDiskType
                            && t.DiskPermit == Code.EnumHelper.DiskPermit.Public
                          select new Dto.DiskFile.List
                          {
                              UserId = t.tbSysUser.Id,
                              FileType = "Folder",
                              DiskTypeId = diskTypeId == 0 ? t.tbDiskType.Id : diskTypeId,
                              FileTitle = t.tbSysUser.UserName
                          }).Distinct().ToList();

                return tb;
            }
        }

        [NonAction]
        public static List<Dto.DiskFile.List> SelectByAuthorize(Code.EnumHelper.DiskType enumDiskType, int diskTypeId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskPower>()
                          where t.tbDiskFolder.tbDiskType.DiskType == enumDiskType
                            && t.tbDiskFolder.DiskPermit == Code.EnumHelper.DiskPermit.Authorize
                            && t.tbSysUser.Id == Code.Common.UserId
                            && t.IsView == true
                          select new Dto.DiskFile.List
                          {
                              UserId = t.tbDiskFolder.tbSysUser.Id,
                              FileType = "Folder",
                              DiskTypeId = diskTypeId == 0 ? t.tbDiskFolder.tbDiskType.Id : diskTypeId,
                              FileTitle = t.tbDiskFolder.tbSysUser.UserName
                          }).Distinct().ToList();

                return tb;
            }
        }

        [NonAction]
        public static List<Dto.DiskFile.List> SelectByDiskPermitPublic(int userId, int diskTypeId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                          where t.tbDiskType.DiskType == Code.EnumHelper.DiskType.Private
                            && t.tbDiskFolderParent == null
                            && t.DiskPermit == Code.EnumHelper.DiskPermit.Public
                            && t.tbSysUser.Id == userId
                          select new Dto.DiskFile.List
                          {
                              Id = t.Id,
                              UserId = userId,
                              FileType = "Folder",
                              DiskTypeId = diskTypeId == 0 ? t.tbDiskType.Id : diskTypeId,
                              FileTitle = t.DiskFolderName
                          }).ToList();

                return tb;
            }
        }

        [NonAction]
        public static List<Dto.DiskFile.List> SelectByDiskPermitAuthorize(int userId, int diskTypeId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskPower>()
                          where t.tbDiskFolder.tbDiskType.DiskType == Code.EnumHelper.DiskType.Private
                            && t.tbDiskFolder.DiskPermit == Code.EnumHelper.DiskPermit.Authorize
                            && t.tbSysUser.Id == Code.Common.UserId
                            && t.IsView == true
                            && t.tbDiskFolder.tbSysUser.Id == userId
                          select new Dto.DiskFile.List
                          {
                              Id = t.tbDiskFolder.Id,
                              UserId = userId,
                              FileType = "Folder",
                              DiskTypeId = diskTypeId == 0 ? t.tbDiskFolder.tbDiskType.Id : diskTypeId,
                              FileTitle = t.tbDiskFolder.DiskFolderName
                          }).Distinct().ToList();

                return tb;
            }
        }

        [NonAction]
        public static Dto.DiskFolder.Info SelectInfo(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                          where t.Id == id
                          select new Dto.DiskFolder.Info
                          {
                              Id = t.Id,
                              DiskFolderName = t.DiskFolderName,
                              DiskTypeId = t.tbDiskType.Id,
                              UserId = t.tbSysUser.Id,
                              DiskPermit = t.DiskPermit
                          }).FirstOrDefault();
                return tb;
            }
        }

        [NonAction]
        public static bool CheckDickTypePublic(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                          where t.tbDiskType.DiskType == Code.EnumHelper.DiskType.Public
                            && t.DiskPermit == Code.EnumHelper.DiskPermit.Public
                          select t).Any();

                return tb;
            }
        }

        [NonAction]
        public static List<int> SelectOwnFolderIdsByUserId(int userId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                          where t.tbSysUser.Id == userId
                            && !new string[] { "公开文件夹", "学校共享文件夹", "教师共享文件夹" }.Contains(t.DiskFolderName)
                          select t.Id).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<int> SelectDiskPermitPublicFolderIdsByUserId(Code.EnumHelper.DiskType diskType, int userId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                          where t.DiskPermit == Code.EnumHelper.DiskPermit.Public
                            && t.tbDiskType.DiskType == diskType
                            && !new string[] { "公开文件夹", "学校共享文件夹", "教师共享文件夹" }.Contains(t.DiskFolderName)
                            && t.tbSysUser.Id != userId
                          select t.Id).ToList();
                return tb;
            }
        }
    }
}