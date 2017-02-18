using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dorm.Controllers
{
    public class DormDataController : Controller
    {
        public ActionResult List()
        {
            var vm = new Models.DormData.List();
            vm.DormOptionList = DormOptionController.SelectList();

            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Dorm.Entity.tbDormData>();
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }
                if (vm.DormOptionId > 0)
                {
                    tb = tb.Where(d => d.tbDormOption.Id == vm.DormOptionId);
                }
                vm.DormDataList = (from p in tb
                                   orderby p.No
                                   select new Dto.DormData.List()
                                   {
                                       Id = p.Id,
                                       DormOptionName = p.tbDormOption.DormOptionName,
                                       DormOptionValue = p.tbDormOption.DormOptionValue,
                                       InputDate = p.InputDate,
                                       Sex = p.tbStudent.tbSysUser.tbSex.SexName,
                                       StudentCode = p.tbStudent.StudentCode,
                                       StudentName = p.tbStudent.StudentName
                                   }).ToPageList(vm.Page);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DormData.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                SearchText = vm.SearchText,
                DormOptionId = vm.DormOptionId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Dorm.Entity.tbDormData>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了住宿表现");
                }
            }
            return Code.MvcHelper.Post();
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.DormData.Edit();

            if (id > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.DormDataEdit = (from p in db.Table<Dorm.Entity.tbDormData>()
                                       where p.Id == id
                                       select new Dto.DormData.Edit()
                                       {
                                           Id = p.Id,
                                           DormOptionId = p.tbDormOption.Id,
                                           Remark = p.Remark,
                                           StudentCode = p.tbStudent.StudentCode,
                                           StudentName = p.tbStudent.StudentName
                                       }).FirstOrDefault();
                }
            }
            vm.DormOptionList = DormOptionController.SelectList(vm.DormDataEdit.DormOptionId);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DormData.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    if (db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == vm.DormDataEdit.StudentCode && d.StudentName == vm.DormDataEdit.StudentName).Count() == 0)
                    {
                        error.Add("无法获取学生信息!");
                        return Code.MvcHelper.Post(error);
                    }
                    if (vm.DormDataEdit.Id > 0)
                    {
                        var tb = db.Set<Dorm.Entity.tbDormData>().Find(vm.DormDataEdit.Id);
                        tb.InputDate = DateTime.Now;
                        tb.Remark = vm.DormDataEdit.Remark;
                        tb.tbDormOption = db.Set<Dorm.Entity.tbDormOption>().Find(vm.DormDataEdit.DormOptionId);
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                    }
                    else
                    {
                        var tb = new Dorm.Entity.tbDormData()
                        {
                            InputDate = DateTime.Now,
                            Remark = vm.DormDataEdit.Remark,
                            tbDormOption = db.Set<Dorm.Entity.tbDormOption>().Find(vm.DormDataEdit.DormOptionId),
                            tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == vm.DormDataEdit.StudentCode && d.StudentName == vm.DormDataEdit.StudentName).FirstOrDefault(),
                            tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId)
                        };
                        db.Set<Dorm.Entity.tbDormData>().Add(tb);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("新增/修改了住宿表现");
                    }
                }
            }
            return Code.MvcHelper.Post(error);
        }

        public ActionResult Info(int id)
        {
            var vm = new Models.DormData.Info();

            using (var db = new XkSystem.Models.DbContext())
            {
                vm.DormDataInfo = (from p in db.Table<Dorm.Entity.tbDormData>()
                                   where p.Id == id
                                   select new Dto.DormData.Info()
                                   {
                                       DormOptionName = p.tbDormOption.DormOptionName,
                                       DormOptionValue = p.tbDormOption.DormOptionValue,
                                       Id = p.Id,
                                       InputDate = p.InputDate,
                                       Remark = p.Remark,
                                       Sex = p.tbStudent.tbSysUser.tbSex.SexName,
                                       StudentCode = p.tbStudent.StudentCode,
                                       StudentName = p.tbStudent.StudentName,
                                       SysUserName = p.tbSysUser.UserName
                                   }).FirstOrDefault();
            }

            return View(vm);
        }
    }
}