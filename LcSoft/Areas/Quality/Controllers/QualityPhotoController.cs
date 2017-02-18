using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace XkSystem.Areas.Quality.Controllers
{
    public class QualityPhotoController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityPhoto.List();

                vm.Page.PageSize = 12;

                var tb = (from t in db.Table<Quality.Entity.tbQualityPhoto>()
                          where t.tbStudent.tbSysUser.Id == Code.Common.UserId
                            && (String.IsNullOrEmpty(vm.SearchText) || t.PhotoTitle.Contains(vm.SearchText))
                          orderby t.No, t.Id descending
                          select t).ToPageList(vm.Page);

                vm.QualityPhotoList = tb;

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.QualityPhoto.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult Upload()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(Models.QualityPhoto.Edit vm)
        {
            HttpPostedFileBase file = Request.Files["fileData"];

            if (file.ContentLength > 0)
            {
                var filePath = Server.MapPath("~/Files/Quality/");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string fileTitle = Path.GetFileName(file.FileName).Split(new char[] { '.' }).First();
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + new Random().Next(10000, 99999).ToString() + "." + file.FileName.Split('.').Last();
                string fileContent = file.FileName.Split('.').Last();

                file.SaveAs(filePath + fileName);

                using (var db = new XkSystem.Models.DbContext())
                {
                    var tb = new Quality.Entity.tbQualityPhoto();
                    tb.PhotoTitle = fileTitle;
                    tb.PhotoFile = fileName;
                    tb.InputDate = DateTime.Now;
                    tb.tbStudent = (from t in db.Table<Student.Entity.tbStudent>()
                                    where t.tbSysUser.Id == Code.Common.UserId
                                    select t).FirstOrDefault();

                    db.Set<Quality.Entity.tbQualityPhoto>().Add(tb);

                    db.SaveChanges();
                }
            }

            return Code.MvcHelper.Post();
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityPhoto.Edit();

                if (id != 0)
                {
                    var tb = (from t in db.Table<Quality.Entity.tbQualityPhoto>()
                              where t.Id == id
                              select t).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.QualityPhotoEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.QualityPhoto.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.QualityPhotoEdit.Id != 0)
                    {
                        var tb = (from p in db.Table<Quality.Entity.tbQualityPhoto>()
                                  where p.Id == vm.QualityPhotoEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.PhotoTitle = vm.QualityPhotoEdit.PhotoTitle;
                        }
                    }
                }

                db.SaveChanges();

                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityPhoto.List();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Quality.Entity.tbQualityPhoto>()
                              where p.Id == id && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.IsDeleted = true;
                    }
                }

                db.SaveChanges();

                return Code.MvcHelper.Post(null, Url.Action("List"), "删除成功！");
            }
        }
    }
}