using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity;

namespace XkSystem.Areas.Disk.Controllers
{
    public class DiskFileController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DiskFile.List();

                var tbDiskType = DiskTypeController.SelectInfo(vm.DiskTypeId);

                #region 文件夹路径
                if (vm.UserId == 0)
                {
                    vm.FolderPath = DiskFolderController.SelectFolderPath(vm.FolderId);
                }
                else
                {
                    if (tbDiskType != null && tbDiskType.DiskType == Code.EnumHelper.DiskType.Public)
                    {
                        var tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(vm.UserId);
                        var tbDiskFolder = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                                            where t.DiskFolderName == "教师共享文件夹"
                                            select t).FirstOrDefault();
                        vm.FolderPath = DiskFolderController.SelectFolderPath(tbDiskFolder.Id);
                        vm.FolderPath.Add(new Dto.DiskFolder.Info
                        {
                            DiskFolderName = tbSysUser.UserName,
                            DiskTypeId = vm.DiskTypeId,
                            UserId = tbSysUser.Id,
                            ParentId = 0
                        });
                    }

                    if (vm.FolderId != 0)
                    {
                        vm.FolderPath = vm.FolderPath.Union(DiskFolderController.SelectFolderPath(vm.FolderId, vm.DiskTypeId, vm.UserId)).ToList();
                    }
                }
                #endregion

                #region 文件夹
                if (vm.FolderId == 0)
                {
                    if (vm.DiskTypeId == 0)
                    {
                        // /Disk/DiskFile/List
                        vm.DiskFileList = DiskFolderController.SelectPublicRootFolder();
                        vm.DiskFileList = vm.DiskFileList.Union(DiskFolderController.SelectByParentId(vm.FolderId, vm.SearchText, 0, Code.Common.UserId)).ToList();
                    }

                    if (vm.UserId != 0)
                    {
                        vm.DiskFileList = DiskFolderController.SelectByDiskPermitPublic(vm.UserId, vm.DiskTypeId);
                        vm.DiskFileList = vm.DiskFileList.Union(DiskFolderController.SelectByDiskPermitAuthorize(vm.UserId, vm.DiskTypeId)).ToList();
                    }
                }
                else
                {
                    if (tbDiskType != null && tbDiskType.DiskType == Code.EnumHelper.DiskType.Public)
                    {
                        // 管理员查看全部
                        if (Code.Common.UserType == Code.EnumHelper.SysUserType.Administrator)
                        {
                            vm.DiskFileList = DiskFolderController.SelectPublicFolderByParentId(vm.FolderId);
                        }
                        else
                        {
                            // 学校共享文件夹-共有
                            vm.DiskFileList = DiskFolderController.SelectPublicFolderByParentId(vm.FolderId, Code.EnumHelper.DiskPermit.Public);

                            // 学校共享文件夹-授权
                            var tbDiskFolderAuthorize = DiskFolderController.SelectPublicFolderAuthorizeByParentId(vm.FolderId, Code.EnumHelper.DiskPermit.Public);
                            foreach (Dto.DiskFile.List diskFile in tbDiskFolderAuthorize)
                            {
                                if (!vm.DiskFileList.Any(t => t.FileTitle.Equals(diskFile.FileTitle)))
                                {
                                    vm.DiskFileList.Add(diskFile);
                                }
                            }
                        }

                        var folder = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                                      where t.Id == vm.FolderId
                                      select t).FirstOrDefault();
                        if (folder != null)
                        {
                            if (folder.DiskFolderName.Equals("教师共享文件夹"))
                            {
                                vm.DiskFileList = vm.DiskFileList.Union(DiskFolderController.SelectByPublic(Code.EnumHelper.DiskType.Private, vm.DiskTypeId)).ToList();

                                foreach (Dto.DiskFile.List diskFile in DiskFolderController.SelectByAuthorize(Code.EnumHelper.DiskType.Private, vm.DiskTypeId))
                                {
                                    if (!vm.DiskFileList.Any(t => t.FileTitle.Equals(diskFile.FileTitle)))
                                    {
                                        vm.DiskFileList.Add(diskFile);
                                    }
                                }
                            }
                            else if (!folder.DiskFolderName.Equals("公开文件夹") && !(folder.DiskFolderName.Equals("学校共享文件夹")))
                            {
                                vm.DiskFileList = DiskFolderController.SelectByParentId(vm.FolderId, vm.SearchText, vm.DiskTypeId, vm.UserId);
                            }
                        }
                    }
                    else
                    {
                        vm.DiskFileList = DiskFolderController.SelectByParentId(vm.FolderId, vm.SearchText);
                    }
                }
                #endregion

                #region 文件
                var tbDiskFile = from t in db.Table<Disk.Entity.tbDiskFile>()
                                 select t;
                if (vm.FolderId != 0)
                {
                    tbDiskFile = tbDiskFile.Where(t => t.tbDiskFolder.Id == vm.FolderId);
                }
                else
                {
                    tbDiskFile = tbDiskFile.Where(t => t.tbDiskFolder == null);

                    //if (vm.UserId != 0)
                    //{
                    //    tbDiskFile = tbDiskFile.Where(t => t.tbSysUser.Id == vm.UserId);
                    //}
                    //else
                    //{
                    tbDiskFile = tbDiskFile.Where(t => t.tbSysUser.Id == Code.Common.UserId);
                    //}
                }
                if (!String.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tbDiskFile = tbDiskFile.Where(t => t.FileTitle.Contains(vm.SearchText));
                }

                int diskTypeId = DiskTypeController.SelectByEnumDiskType(Code.EnumHelper.DiskType.Private).Id;

                var tb = (from t in tbDiskFile
                          orderby t.Id descending
                          select new Dto.DiskFile.List
                          {
                              Id = t.Id,
                              DiskTypeId = t.tbDiskFolder == null ? diskTypeId : t.tbDiskFolder.tbDiskType.Id,
                              FileType = "File",
                              FileTitle = t.FileTitle,
                              FileLength = t.FileLength,
                              InputDate = t.InputDate,
                              UserId = t.tbSysUser.Id,
                              UserName = t.tbSysUser.UserName
                          }).ToList();

                vm.DiskFileList = vm.DiskFileList.Union(tb).ToList();
                #endregion

                #region 文件夹文件权限

                // 管理权限
                vm.AdminFolderIds = DiskFolderController.SelectOwnFolderIdsByUserId(Code.Common.UserId);
                if (Code.Common.UserType == Code.EnumHelper.SysUserType.Administrator)
                {
                    vm.AdminFolderIds = vm.AdminFolderIds.Union(DiskFolderController.SelectDiskPermitPublicFolderIdsByUserId(Code.EnumHelper.DiskType.Public, Code.Common.UserId)).ToList();
                }
                vm.AdminFolderIds = vm.AdminFolderIds.Union(DiskFolderController.SelectDiskPermitPublicFolderIdsByUserId(Code.EnumHelper.DiskType.Private, Code.Common.UserId)).ToList();
                vm.AdminFolderIds = vm.AdminFolderIds.Union(DiskPowerController.SelectAdminFolderIds(Code.Common.UserId)).ToList();

                // 查看权限
                vm.ViewFolderIds = DiskFolderController.SelectOwnFolderIdsByUserId(Code.Common.UserId);
                vm.ViewFolderIds = vm.ViewFolderIds.Union(DiskFolderController.SelectDiskPermitPublicFolderIdsByUserId(Code.EnumHelper.DiskType.Public, Code.Common.UserId)).ToList();
                vm.ViewFolderIds = vm.ViewFolderIds.Union(DiskFolderController.SelectDiskPermitPublicFolderIdsByUserId(Code.EnumHelper.DiskType.Private, Code.Common.UserId)).ToList();
                vm.ViewFolderIds = vm.ViewFolderIds.Union(DiskPowerController.SelectViewFolderIds(Code.Common.UserId)).ToList();

                #endregion

                #region 文件权限
                vm.AdminFileIds = DiskFileController.SelectDescendantsByFolder(vm.AdminFolderIds).Select(t => t.Id).ToList();
                vm.ViewFileIds = DiskFileController.SelectDescendantsByFolder(vm.ViewFolderIds).Select(t => t.Id).ToList();

                var MyRootFileIds = (from t in db.Table<Disk.Entity.tbDiskFile>()
                                     where t.tbDiskFolder == null
                                        && t.tbSysUser.Id == Code.Common.UserId
                                     select t.Id).ToList();
                vm.AdminFileIds = vm.AdminFileIds.Union(MyRootFileIds).ToList();
                vm.ViewFileIds = vm.ViewFileIds.Union(MyRootFileIds).ToList();

                #endregion

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DiskFile.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                folderId = vm.FolderId,
                searchText = vm.SearchText
            }));
        }

        public ActionResult Edit()
        {
            var vm = new Models.DiskFile.Edit();

            #region 校验上传文件夹权限
            if (vm.FolderId != 0)
            {
                var diskFolder = DiskFolderController.SelectInfo(vm.FolderId);
                if (diskFolder != null
                    && !diskFolder.UserId.Equals(Code.Common.UserId)
                    && !DiskFolderController.CheckDickTypePublic(vm.FolderId)
                    && !DiskPowerController.CheckInput(vm.FolderId, Code.Common.UserId)
                    && Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator)
                {
                    return Content(String.Format("您不具备当前文件夹：【{0}】的上传权限！", diskFolder.DiskFolderName));
                }
            }
            #endregion

            return View(vm);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(Models.DiskFile.Edit vm)
        {
            HttpPostedFileBase file = Request.Files["fileData"];

            if (file.ContentLength > 0)
            {
                var filePath = Server.MapPath("~/Files/Disk/");
                //if (!Directory.Exists(filePath))
                //{
                //    Directory.CreateDirectory(filePath);
                //}

                string[] fileTypeExts = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".rar", ".zip", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".txt", ".flv", ".avi", ".rmvb", ".rm", ".wmv", ".mp3", ".mp4", ".wma" };
                if (fileTypeExts.Contains(Path.GetExtension(file.FileName)))
                {
                    string fileTitle = Path.GetFileName(file.FileName);
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + new Random().Next(10000, 99999).ToString() + "." + file.FileName.Split('.').Last();
                    string fileContent = file.FileName.Split('.').Last();

                    file.SaveAs(filePath + fileName);

                    using (var db = new XkSystem.Models.DbContext())
                    {
                        var tb = new Disk.Entity.tbDiskFile();
                        tb.FileTitle = fileTitle;
                        tb.FileName = fileName;
                        tb.FileContent = fileContent;
                        tb.FileLength = file.ContentLength;
                        tb.InputDate = DateTime.Now;
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);

                        if (vm.FolderId != 0)
                        {
                            tb.tbDiskFolder = db.Set<Disk.Entity.tbDiskFolder>().Find(vm.FolderId);
                        }

                        db.Set<Disk.Entity.tbDiskFile>().Add(tb);

                        db.SaveChanges();
                    }
                }
            }

            return Code.MvcHelper.Post();
        }

        public ActionResult Rename(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DiskFile.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Disk.Entity.tbDiskFile>()
                              where p.Id == id
                              select new Dto.DiskFile.Edit
                              {
                                  Id = p.Id,
                                  FileTitle = p.FileTitle
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.DiskFileEdit = tb;
                        vm.DiskFileEdit.FileTitle = vm.DiskFileEdit.FileTitle.Replace("." + vm.DiskFileEdit.FileTitle.Split(new char[] { '.' }).Last(), "");
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rename(Models.DiskFile.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.DiskFileEdit.Id != 0)
                    {
                        var tb = (from p in db.Table<Disk.Entity.tbDiskFile>()
                                  where p.Id == vm.DiskFileEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.FileTitle = vm.DiskFileEdit.FileTitle + "." + tb.FileName.Split(new char[] { '.' }).Last();
                        }
                    }
                }

                db.SaveChanges();

                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult MoveTo(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DiskFile.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Disk.Entity.tbDiskFile>()
                              where p.Id == id && p.tbSysUser.Id == Code.Common.UserId
                              select new Dto.DiskFile.Edit
                              {
                                  Id = p.Id,
                                  FileTitle = p.FileTitle
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.DiskFileEdit = tb;
                    }
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
        public ActionResult MoveTo(Models.DiskFile.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Disk.Entity.tbDiskFile>()
                          .Include(p => p.tbDiskFolder)
                          where p.Id == vm.DiskFileEdit.Id && p.tbSysUser.Id == Code.Common.UserId
                          select p).FirstOrDefault();
                if (tb != null)
                {
                    if (vm.DiskFileEdit.FolderId == 0)
                    {
                        tb.tbDiskFolder = null;
                    }
                    else
                    {
                        tb.tbDiskFolder = db.Set<Disk.Entity.tbDiskFolder>().Find(vm.DiskFileEdit.FolderId);
                    }
                }

                db.SaveChanges();

                return Code.MvcHelper.Post(null, Url.Action("List", "DiskFile", new
                {
                    folderId = vm.FolderId
                }), "移动成功！");
            }
        }

        public ActionResult MoveToChecked()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoveToChecked(Models.DiskFile.Edit vm, string ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                List<int> fileIds = new List<int>();
                List<int> folderIds = new List<int>();
                string[] arrIds = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string id in arrIds)
                {
                    string[] arrId = id.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrId[1] == "Folder")
                    {
                        folderIds.Add(Convert.ToInt32(arrId[0]));
                    }
                    else
                    {
                        fileIds.Add(Convert.ToInt32(arrId[0]));
                    }
                }

                List<Dto.DiskFolder.Info> folderDescendants = DiskFolderController.SelectDescendants(folderIds);

                if (folderDescendants.Select(t => t.Id).Contains(vm.DiskFileEdit.FolderId))
                {
                    error.AddError("文件夹不能移动到自身及子文件夹中!");
                }

                if (error.Count() == 0)
                {
                    foreach (int folderId in folderIds)
                    {
                        var tb = (from p in db.Table<Disk.Entity.tbDiskFolder>()
                                  .Include(p => p.tbDiskFolderParent)
                                  where p.Id == folderId && p.tbSysUser.Id == Code.Common.UserId
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            if (vm.DiskFileEdit.FolderId == 0)
                            {
                                tb.tbDiskFolderParent = null;
                            }
                            else
                            {
                                tb.tbDiskFolderParent = db.Set<Disk.Entity.tbDiskFolder>().Find(vm.DiskFileEdit.FolderId);
                            }
                        }
                    }

                    foreach (int fileId in fileIds)
                    {
                        var tb = (from p in db.Table<Disk.Entity.tbDiskFile>()
                                  .Include(p => p.tbDiskFolder)
                                  where p.Id == fileId && p.tbSysUser.Id == Code.Common.UserId
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            if (vm.DiskFileEdit.FolderId == 0)
                            {
                                tb.tbDiskFolder = null;
                            }
                            else
                            {
                                tb.tbDiskFolder = db.Set<Disk.Entity.tbDiskFolder>().Find(vm.DiskFileEdit.FolderId);
                            }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id = 0, int folderId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                string strMessage = String.Empty;
                if (id != 0)
                {
                    var tb = (from p in db.Table<Disk.Entity.tbDiskFile>()
                              .Include(p => p.tbDiskFolder)
                              .Include(p => p.tbSysUser)
                              where p.Id == id && p.tbSysUser.Id == Code.Common.UserId
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        if (tb.tbSysUser.Id == Code.Common.UserId
                            || DiskPowerController.CheckAdmin(tb.tbDiskFolder.Id, Code.Common.UserId))
                        {
                            tb.IsDeleted = true;
                            db.SaveChanges();

                            strMessage = "删除成功！";
                        }
                        else
                        {
                            strMessage = "您不具该文件删除权限！";
                        }
                    }
                }

                return Code.MvcHelper.Post(null, Url.Action("List", new
                {
                    folderId = folderId
                }), strMessage);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteChecked(List<string> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                foreach (string id in ids)
                {
                    string[] arrId = id.Split(new char[] { '-' });
                    int _id = Convert.ToInt32(arrId[0]);
                    if (arrId[1] == "Folder")
                    {
                        DiskFolderController.Delete(_id);
                    }
                    else
                    {
                        if (_id != 0)
                        {
                            var tb = (from p in db.Table<Disk.Entity.tbDiskFile>()
                                      where p.Id == _id && p.tbSysUser.Id == Code.Common.UserId
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.IsDeleted = true;
                            }
                        }
                    }
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Download(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (id != 0)
                {
                    var tb = (from p in db.Table<Disk.Entity.tbDiskFile>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        string filePath = Server.MapPath("~/Files/Disk/") + tb.FileName;

                        return File(filePath, Code.Common.DownloadType, tb.FileTitle);
                    }
                }

                return Content("<script>alert('文件不存在');</script>");
            }
        }

        public ActionResult DownloadFolder(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                          where t.Id == id && t.tbSysUser.Id == Code.Common.UserId
                          select t).FirstOrDefault();

                if (tb != null)
                {
                    List<int> ids = new List<int>();
                    ids.Add(tb.Id);
                    List<Dto.DiskFolder.Info> folderDescendants = DiskFolderController.SelectDescendants(ids);

                    string strTempPathName = Path.GetTempPath() + Guid.NewGuid().ToString();
                    string strTempPath = strTempPathName + "/";
                    string zipFileName = strTempPathName + ".zip";

                    foreach (var folder in folderDescendants)
                    {
                        if (!Directory.Exists(strTempPath + folder.FolderPath))
                        {
                            Directory.CreateDirectory(strTempPath + folder.FolderPath);
                        }
                    }

                    List<Dto.DiskFile.Info> diskFileDescendants = DiskFileController.SelectDescendantsByFolder(ids);
                    string strFilesDiskPath = Server.MapPath("~/Files/Disk/");
                    foreach (var diskFile in diskFileDescendants)
                    {
                        FileInfo file = new FileInfo(strFilesDiskPath + diskFile.FileName);
                        file.CopyTo(strTempPath + diskFile.FolderPath + diskFile.FileTitle);
                    }

                    Code.ZipHelper.CreateZip(zipFileName, strTempPath + tb.DiskFolderName + "/");

                    Code.FileHelper.DeleteDirectory(strTempPath);

                    return File(zipFileName, Code.Common.DownloadType, tb.DiskFolderName + ".zip");
                }

                return RedirectToAction("List");
            }
        }

        public ActionResult DownloadChecked(string ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                List<int> fileIds = new List<int>();
                List<int> folderIds = new List<int>();
                string[] arrIds = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (arrIds.Length == 1)
                {
                    string[] arrId = arrIds[0].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrId[1] == "Folder")
                    {
                        int folderId = arrId[0].ConvertToInt();
                        var tb = (from t in db.Table<Disk.Entity.tbDiskFolder>()
                                  where t.Id == folderId
                                  select t).FirstOrDefault();

                        if (tb != null)
                        {
                            folderIds.Add(folderId);
                            List<Dto.DiskFolder.Info> folderDescendants = DiskFolderController.SelectDescendants(folderIds);

                            string strTempPathName = Path.GetTempPath() + Guid.NewGuid().ToString();
                            string strTempPath = strTempPathName + "/";
                            string zipFileName = strTempPathName + ".zip";

                            foreach (var folder in folderDescendants)
                            {
                                if (!Directory.Exists(strTempPath + folder.FolderPath))
                                {
                                    Directory.CreateDirectory(strTempPath + folder.FolderPath);
                                }
                            }

                            List<Dto.DiskFile.Info> diskFileDescendants = DiskFileController.SelectDescendantsByFolder(folderIds);
                            string strFilesDiskPath = Server.MapPath("~/Files/Disk/");
                            foreach (var diskFile in diskFileDescendants)
                            {
                                FileInfo file = new FileInfo(strFilesDiskPath + diskFile.FileName);
                                file.CopyTo(strTempPath + diskFile.FolderPath + diskFile.FileTitle);
                            }

                            Code.ZipHelper.CreateZip(zipFileName, strTempPath + tb.DiskFolderName + "/");

                            Code.FileHelper.DeleteDirectory(strTempPath);

                            return File(zipFileName, Code.Common.DownloadType, tb.DiskFolderName + ".zip");
                        }
                    }
                    else
                    {
                        int fileId = arrId[0].ConvertToInt();
                        if (fileId != 0)
                        {
                            var tb = (from p in db.Table<Disk.Entity.tbDiskFile>()
                                      where p.Id == fileId
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                string filePath = Server.MapPath("~/Files/Disk/") + tb.FileName;

                                return File(filePath, Code.Common.DownloadType, tb.FileTitle);
                            }
                        }
                    }
                }
                else
                {
                    foreach (string strIds in arrIds)
                    {
                        string[] arrId = strIds.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                        if (arrId[1] == "Folder")
                        {
                            folderIds.Add(arrId[0].ConvertToInt());
                        }
                        else
                        {
                            fileIds.Add(arrId[0].ConvertToInt());
                        }
                    }

                    List<Dto.DiskFolder.Info> folderDescendants = DiskFolderController.SelectDescendants(folderIds);
                    string strTempPathName = Path.GetTempPath() + Guid.NewGuid().ToString();
                    string strTempPath = strTempPathName + "/";
                    string zipFileName = strTempPathName + ".zip";

                    if (!Directory.Exists(strTempPath))
                    {
                        Directory.CreateDirectory(strTempPath);
                    }

                    foreach (var folder in folderDescendants)
                    {
                        if (!Directory.Exists(strTempPath + folder.FolderPath))
                        {
                            Directory.CreateDirectory(strTempPath + folder.FolderPath);
                        }
                    }

                    List<Dto.DiskFile.Info> diskFileDescendants = DiskFileController.SelectDescendantsByFolder(folderIds);
                    foreach (var fileId in fileIds)
                    {
                        var tb = (from t in db.Table<Disk.Entity.tbDiskFile>()
                                  where t.Id == fileId
                                  select new Dto.DiskFile.Info
                                  {
                                      Id = t.Id,
                                      FileTitle = t.FileTitle,
                                      FileName = t.FileName,
                                      FolderId = t.tbDiskFolder == null ? 0 : t.tbDiskFolder.Id
                                  }).FirstOrDefault();
                        if (tb != null)
                        {
                            diskFileDescendants.Add(tb);
                        }
                    }

                    string strFilesDiskPath = Server.MapPath("~/Files/Disk/");
                    foreach (var diskFile in diskFileDescendants)
                    {
                        FileInfo file = new FileInfo(strFilesDiskPath + diskFile.FileName);
                        file.CopyTo(strTempPath + diskFile.FolderPath + diskFile.FileTitle);
                    }

                    Code.ZipHelper.CreateZip(zipFileName, strTempPath + "/");

                    Code.FileHelper.DeleteDirectory(strTempPath);

                    return File(zipFileName, Code.Common.DownloadType, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".zip");
                }

                return Content("<script>alert('文件不存在');</script>");
            }
        }

        [NonAction]
        public static List<Dto.DiskFile.Info> SelectDescendantsByFolder(List<int> folderIds)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                List<Dto.DiskFile.Info> fileDescendants = new List<Dto.DiskFile.Info>();

                List<Dto.DiskFolder.Info> folderDescendants = DiskFolderController.SelectDescendants(folderIds);
                foreach (var folder in folderDescendants)
                {
                    var tb = (from t in db.Table<Disk.Entity.tbDiskFile>()
                              where t.tbDiskFolder.Id == folder.Id
                              select new Dto.DiskFile.Info
                              {
                                  Id = t.Id,
                                  FileTitle = t.FileTitle,
                                  FileName = t.FileName,
                                  FolderId = t.tbDiskFolder.Id,
                                  FolderPath = folder.FolderPath
                              }).ToList();

                    fileDescendants = fileDescendants.Union(tb).ToList();
                }

                return fileDescendants;
            }
        }

        [NonAction]
        public static List<Dto.DiskFolder.ReportUser> SelectReportUserList(int parentId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskFile>()
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