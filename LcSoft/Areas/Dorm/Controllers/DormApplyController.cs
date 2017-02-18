using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dorm.Controllers
{
    public class DormApplyController : Controller
    {
        public ActionResult List()
        {
            var vm = new Models.DormApply.List();

            vm.CheckStatusList = new List<SelectListItem>() {
                new SelectListItem() {Value= "-1",Text="不通过"},
                new SelectListItem() {Value= "0",Text="未处理"},
                new SelectListItem() {Value= "1",Text="通过"}
            };

            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Dorm.Entity.tbDormApply>();
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbDorm.DormName.Contains(vm.SearchText)
                                  || d.tbStudent.StudentCode.Contains(vm.SearchText)
                                  || d.tbStudent.StudentName.Contains(vm.SearchText)
                                  || d.tbStudent.StudentNameEn.Contains(vm.SearchText));
                }
                if (vm.CheckStatusId > -2)
                {
                    tb = tb.Where(d => d.CheckStatus == (Code.EnumHelper.CheckStatus)vm.CheckStatusId);
                }
                vm.DormApplyList = (from p in tb
                                    orderby p.No
                                    select new Dto.DormApply.List()
                                    {
                                        #region
                                        CheckDate = p.CheckDate,
                                        CheckRemark = p.CheckRemark,
                                        CheckStatus = p.CheckStatus,
                                        Id = p.Id,
                                        InputDate = p.InputDate,
                                        Remark = p.Remark,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        UserCode = p.tbCheckUser.UserCode,
                                        UserName = p.tbCheckUser.UserName,
                                        Sex = p.tbStudent.tbSysUser.tbSex.SexName
                                        #endregion
                                    }).ToPageList(vm.Page);
                foreach (var v in vm.DormApplyList)
                {
                    switch (v.CheckStatus)
                    {
                        case Code.EnumHelper.CheckStatus.Fail:
                            v.CheckStatusName = "不通过"; break;
                        case Code.EnumHelper.CheckStatus.None:
                            v.CheckStatusName = "未处理"; break;
                        default:
                            v.CheckStatusName = "通过"; break;
                    }
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DormApply.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                SearchText = vm.SearchText,
                CheckStatusId = vm.CheckStatusId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Dorm.Entity.tbDormApply>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了住宿申请");
                }
            }
            return Code.MvcHelper.Post();
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.DormApply.Edit();

            using (var db = new XkSystem.Models.DbContext())
            {
                if (id > 0)
                {
                    vm.DormApplyEdit = (from p in db.Table<Dorm.Entity.tbDormApply>()
                                        where p.Id == id
                                        select new Dto.DormApply.Edit()
                                        {
                                            Id = p.Id,
                                            DormId = p.tbDorm.Id,
                                            Remark = p.Remark
                                        }).FirstOrDefault();
                }
                vm.DormList = DormController.SelectList(vm.DormApplyEdit.DormId);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DormApply.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    if (db.Table<Dorm.Entity.tbDormApply>().Where(d => d.tbStudent.tbSysUser.Id == Code.Common.UserId).Count() > 0)
                    {
                        error.Add("请勿重复申请!");
                        return Code.MvcHelper.Post(error);
                    }

                    var tb = new Dorm.Entity.tbDormApply();
                    if (vm.DormApplyEdit.Id > 0)
                    {
                        tb = db.Set<Dorm.Entity.tbDormApply>().Find(vm.DormApplyEdit.Id);
                        tb.Remark = vm.DormApplyEdit.Remark;
                        tb.tbDorm = db.Set<Dorm.Entity.tbDorm>().Find(vm.DormApplyEdit.DormId);
                    }
                    else
                    {
                        tb = new Dorm.Entity.tbDormApply()
                        {
                            CheckStatus = Code.EnumHelper.CheckStatus.None,
                            CheckDate = DateTime.Now,
                            InputDate = DateTime.Now,
                            Remark = vm.DormApplyEdit.Remark,
                            tbDorm = db.Set<Dorm.Entity.tbDorm>().Find(vm.DormApplyEdit.DormId),
                            tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.tbSysUser.Id == Code.Common.UserId).FirstOrDefault()
                        };
                        db.Set<Dorm.Entity.tbDormApply>().Add(tb);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改/添加了住宿申请");
                    }
                }
            }

            return Code.MvcHelper.Post(error);
        }

        public ActionResult Info(int id)
        {
            var vm = new Models.DormApply.Info();

            using (var db = new XkSystem.Models.DbContext())
            {
                vm.DormApplyInfo = (from p in db.Table<Dorm.Entity.tbDormApply>()
                                    where p.Id == id
                                    select new Dto.DormApply.Info()
                                    {
                                        CheckDate = p.CheckDate,
                                        CheckRemark = p.CheckRemark,
                                        CheckStatus = p.CheckStatus,
                                        Id = p.Id,
                                        InputDate = p.InputDate,
                                        Remark = p.Remark,
                                        Sex = p.tbStudent.tbSysUser.tbSex.SexName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        UserCode = p.tbCheckUser.UserCode,
                                        UserName = p.tbCheckUser.UserName
                                    }).FirstOrDefault();

                switch (vm.DormApplyInfo.CheckStatus)
                {
                    case Code.EnumHelper.CheckStatus.Fail:
                        vm.DormApplyInfo.CheckStatusName = "不通过"; break;
                    case Code.EnumHelper.CheckStatus.None:
                        vm.DormApplyInfo.CheckStatusName = "未处理"; break;
                    default:
                        vm.DormApplyInfo.CheckStatusName = "通过"; break;
                }
                if (vm.DormApplyInfo.CheckStatus == Code.EnumHelper.CheckStatus.Success)
                {
                    var dormStudent = db.Table<Dorm.Entity.tbDormStudent>()
                        .Include(d => d.tbRoom)
                        .Include(d => d.tbRoom.tbBuild)
                        .Where(d => d.tbStudent.StudentCode == vm.DormApplyInfo.StudentCode).FirstOrDefault();
                    if (dormStudent != null)
                    {
                        vm.DormApplyInfo.RoomName = dormStudent.tbRoom.RoomName;
                        vm.DormApplyInfo.BuildName = dormStudent.tbRoom.tbBuild.BuildName;
                    }
                }
            }

            return View(vm);
        }

        public ActionResult Approve(int id)
        {
            var vm = new Models.DormApply.Approve();
            vm.CheckStatusList = new List<SelectListItem>() {
                new SelectListItem() {Value= "-1",Text="不通过"},
                new SelectListItem() {Value= "0",Text="未处理"},
                new SelectListItem() {Value= "1",Text="通过"}
            };

            using (var db = new XkSystem.Models.DbContext())
            {
                vm.DormApplyApprove = (from p in db.Table<Dorm.Entity.tbDormApply>()
                                       where p.Id == id
                                       select new Dto.DormApply.Approve()
                                       {
                                           CheckRemark = p.CheckRemark,
                                           CheckStatus = p.CheckStatus,
                                           Id = p.Id,
                                           InputDate = p.InputDate,
                                           Remark = p.Remark,
                                           Sex = p.tbStudent.tbSysUser.tbSex.SexName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           StudentName = p.tbStudent.StudentName
                                       }).FirstOrDefault();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(Models.DormApply.Approve vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var tb = db.Set<Dorm.Entity.tbDormApply>().Find(vm.DormApplyApprove.Id);
                    tb.CheckDate = DateTime.Now;
                    tb.CheckRemark = vm.DormApplyApprove.CheckRemark;
                    tb.CheckStatus = vm.DormApplyApprove.CheckStatus;
                    tb.tbCheckUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改/添加了住宿申请");
                    }
                }
            }

            return Code.MvcHelper.Post(error);
        }

        public ActionResult ProvidedDorm(int id)
        {
            var vm = new Models.DormApply.ProvidedDorm();

            using (var db = new XkSystem.Models.DbContext())
            {
                var dormStudentList = db.Table<Dorm.Entity.tbDormStudent>().ToList();
                var RoomList = db.Table<Basis.Entity.tbRoom>().ToList();
                vm.BuildList = Areas.Basis.Controllers.BuildController.SelectList();
                vm.RoomList = Basis.Controllers.RoomController.SelectList();
                foreach (var v in vm.RoomList)
                {
                    if (dormStudentList.Where(d => d.tbRoom.Id == v.Value.ConvertToInt()).Count()
                        >= RoomList.Where(d => d.Id == v.Value.ConvertToInt()).FirstOrDefault().MaxUser)
                    {
                        v.Text += "(满)";
                    }
                }

                vm.DormApplyProvidedDorm = (from p in db.Table<Dorm.Entity.tbDormApply>()
                                            where p.Id == id
                                            select new Dto.DormApply.ProvidedDorm()
                                            {
                                                Id = p.Id,
                                                DormId = p.tbDorm.Id,
                                                StudentCode = p.tbStudent.StudentCode,
                                                StudentName = p.tbStudent.StudentName
                                            }).FirstOrDefault();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProvidedDorm(Models.DormApply.ProvidedDorm vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    //判断是否宿舍超出住宿人数
                    var maxCount = db.Set<Basis.Entity.tbRoom>().Find(vm.DormApplyProvidedDorm.RoomId).MaxUser;
                    var nowNum = db.Table<Dorm.Entity.tbDormStudent>().Where(d => d.tbRoom.Id == vm.DormApplyProvidedDorm.RoomId).Count();
                    if (nowNum >= maxCount)
                    {
                        error.Add("此宿舍已经住满!");
                        return Code.MvcHelper.Post(error);
                    }

                    var tb = new Dorm.Entity.tbDormStudent()
                    {
                        tbDorm = db.Set<Dorm.Entity.tbDorm>().Find(vm.DormApplyProvidedDorm.DormId),
                        tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.DormApplyProvidedDorm.RoomId),
                        tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == vm.DormApplyProvidedDorm.StudentCode
                                          && d.StudentName == vm.DormApplyProvidedDorm.StudentName).FirstOrDefault()
                    };
                    db.Set<Dorm.Entity.tbDormStudent>().Add(tb);

                    var dormApply = db.Set<Dorm.Entity.tbDormApply>().Find(vm.DormApplyProvidedDorm.Id);
                    dormApply.IsDeleted = true;

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改/添加了住宿申请");
                    }
                }
            }

            return Code.MvcHelper.Post(error);
        }












    }
}