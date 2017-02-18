using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace XkSystem.Areas.Quality.Controllers
{
    public class QualityPortraitController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityPortrait.List();

                var tb = (from t in db.Table<Quality.Entity.tbQualityPortrait>()
                          where t.tbStudent.tbSysUser.Id == Code.Common.UserId
                          orderby t.No, t.Id descending
                          select t).ToList();

                vm.QualityPortraitList = tb;

                return View(vm);
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityPortrait.Edit();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.QualityPortraitEdit.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.QualityPortraitEdit.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                if (id != 0)
                {
                    var tb = (from t in db.Table<Quality.Entity.tbQualityPortrait>()
                              where t.Id == id && t.tbStudent.tbSysUser.Id == Code.Common.UserId
                              select new Dto.QualityPortrait.Edit
                              {
                                  Id = t.Id,
                                  PhotoTitle = t.PhotoTitle,
                                  PhotoFile = t.PhotoFile,
                                  YearId = t.tbYear.Id,
                                  Remark = t.Remark
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.QualityPortraitEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.QualityPortrait.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.QualityPortraitEdit.Id == 0)
                    {
                        var tb = new Quality.Entity.tbQualityPortrait();
                        tb.PhotoTitle = vm.QualityPortraitEdit.PhotoTitle;
                        tb.PhotoFile = vm.QualityPortraitEdit.PhotoFile;
                        tb.Remark = vm.QualityPortraitEdit.Remark;
                        tb.InputDate = DateTime.Now;
                        tb.tbStudent = (from t in db.Table<Student.Entity.tbStudent>()
                                        where t.tbSysUser.Id == Code.Common.UserId
                                        select t).FirstOrDefault();
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.QualityPortraitEdit.YearId);

                        db.Set<Quality.Entity.tbQualityPortrait>().Add(tb);
                    }
                    else
                    {
                        var tb = (from p in db.Table<Quality.Entity.tbQualityPortrait>()
                                  where p.Id == vm.QualityPortraitEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.PhotoTitle = vm.QualityPortraitEdit.PhotoTitle;
                            tb.PhotoFile = vm.QualityPortraitEdit.PhotoFile;
                            tb.Remark = vm.QualityPortraitEdit.Remark;
                        }
                    }
                }

                db.SaveChanges();

                return Code.MvcHelper.Post(error);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(Models.QualityPhoto.Edit vm)
        {
            HttpPostedFileBase file = Request.Files["fileData"];

            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + new Random().Next(10000, 99999).ToString() + "." + file.FileName.Split('.').Last();

            if (file.ContentLength > 0)
            {
                var filePath = Server.MapPath("~/Files/Quality/");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string fileTitle = Path.GetFileName(file.FileName).Split(new char[] { '.' }).First();
                string fileContent = file.FileName.Split('.').Last();

                file.SaveAs(filePath + fileName);
            }

            return Code.MvcHelper.Post(null, String.Empty, fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (id != 0)
                {
                    var tb = (from p in db.Table<Quality.Entity.tbQualityPortrait>()
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