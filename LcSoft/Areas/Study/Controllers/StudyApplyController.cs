using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Study.Controllers
{
    public class StudyApplyController : Controller
    {
        public ActionResult List()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Student)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyApply.List();

                var tbStudy = from p in db.Table<Study.Entity.tbStudy>()
                              where p.tbYear.IsDeleted == false
                              && p.tbYear.IsDisable == false && p.tbYear.No == DateTime.Now.Year
                              select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbStudy = tbStudy.Where(d => d.StudyName.Contains(vm.SearchText));
                }

                var tbStudyApply = from p in db.Table<Study.Entity.tbStudyApply>()
                                   where p.tbStudent.IsDeleted == false
                                   && p.tbStudy.IsDeleted == false
                                   && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                   select p;

                vm.StudyApplyList = (from p in tbStudy
                                     orderby p.No descending
                                     select new Dto.StudyApply.List
                                     {
                                         Id = tbStudyApply.Where(d => d.tbStudy.Id == p.Id).Select(d => d.Id).FirstOrDefault(),
                                         No = p.No,
                                         StudyName = p.StudyName,
                                         ApplyFrom = p.ApplyFrom,
                                         ApplyTo = p.ApplyTo,
                                         IsApply = p.IsApply,
                                         IsRoom = p.IsRoom,
                                         YearName = p.tbYear.YearName,
                                         StudyId = p.Id,
                                         Remark = tbStudyApply.Where(d => d.tbStudy.Id == p.Id).Select(d => d.Remark).FirstOrDefault(),
                                         CheckStatus = tbStudyApply.Where(d => d.tbStudy.Id == p.Id).Select(d => d.CheckStatus).FirstOrDefault(),
                                         CheckDate = tbStudyApply.Where(d => d.tbStudy.Id == p.Id).Select(d => d.CheckDate).FirstOrDefault(),
                                         CheckRemark = tbStudyApply.Where(d => d.tbStudy.Id == p.Id).Select(d => d.CheckRemark).FirstOrDefault(),
                                         CheckUserName = tbStudyApply.Where(d => d.tbStudy.Id == p.Id).Select(d => d.tbCheckUser.UserName).FirstOrDefault()
                                     }).ToPageList(vm.Page);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudyApply.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
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
                var tb = (from p in db.Table<Study.Entity.tbStudyApply>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除晚自习申请");
                }

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var tb = (from p in db.Table<Study.Entity.tbStudyApply>()
                              where p.Id == id
                              select p).ToList();

                    foreach (var a in tb)
                    {
                        a.IsDeleted = true;
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("撤销晚自习申请");
                    }
                }
                return Code.MvcHelper.Post(error, Url.Action("List"), "撤销申请成功！");
            }
        }

        public ActionResult Edit(int id = 0, int studyId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyApply.Edit();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Study.Entity.tbStudyApply>()
                              where p.Id == id
                              select new Dto.StudyApply.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  Remark = p.Remark
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.StudyApplyEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.StudyApply.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.StudyApplyEdit.Id == 0)
                    {
                        var tb = new Study.Entity.tbStudyApply();
                        tb.Remark = vm.StudyApplyEdit.Remark;
                        tb.tbStudy = db.Set<Study.Entity.tbStudy>().Find(vm.StudyId);
                        tb.tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.tbSysUser.Id == Code.Common.UserId).FirstOrDefault();
                        tb.CheckStatus = XkSystem.Code.EnumHelper.CheckStatus.None;
                        tb.InputDate = DateTime.Now;
                        db.Set<Study.Entity.tbStudyApply>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加晚自习申请");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Study.Entity.tbStudyApply>()
                                  where p.Id == vm.StudyApplyEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.Remark = vm.StudyApplyEdit.Remark;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改晚自习申请");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }
                return Code.MvcHelper.Post(error, Url.Action("List"));
            }
        }

        public ActionResult Info(int id = 0, int studyId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyApply.Info();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Study.Entity.tbStudyApply>()
                              where p.Id == id
                              select new Dto.StudyApply.Info
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  Remark = p.Remark,
                                  CheckStatus = p.CheckStatus,
                                  CheckDate = p.CheckDate,
                                  CheckRemark = p.CheckRemark,
                                  CheckUserName = p.tbCheckUser.UserName
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.StudyApplyInfo = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Info(Models.StudyApply.Info vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.StudyApplyInfo.Id == 0)
                    {
                        var tb = new Study.Entity.tbStudyApply();
                        tb.Remark = vm.StudyApplyInfo.Remark;
                        tb.tbStudy = db.Set<Study.Entity.tbStudy>().Find(vm.StudyId);
                        tb.tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.tbSysUser.Id == Code.Common.UserId).FirstOrDefault();
                        tb.InputDate = DateTime.Now;
                        db.Set<Study.Entity.tbStudyApply>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加晚自习申请");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Study.Entity.tbStudyApply>()
                                  where p.Id == vm.StudyApplyInfo.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.Remark = vm.StudyApplyInfo.Remark;
                            tb.tbStudy = db.Set<Study.Entity.tbStudy>().Find(vm.StudyId);
                            tb.tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.tbSysUser.Id == Code.Common.UserId).FirstOrDefault();
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改晚自习申请");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }
                return Code.MvcHelper.Post(error, Url.Action("List"));
            }
        }

        public ActionResult CheckList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyApply.CheckList();
                vm.CheckStatusList = typeof(Code.EnumHelper.CheckStatus).ToItemList();
                vm.CheckStatusList.Insert(0, new SelectListItem { Text = "==全部==", Value = "-2" });

                var tbStudyApply = from p in db.Table<Study.Entity.tbStudyApply>()
                                   where p.tbStudy.IsDeleted == false
                                   && p.tbStudent.IsDeleted == false
                                   && p.tbStudy.tbYear.IsDeleted == false
                                   && p.tbStudy.tbYear.No == DateTime.Now.Year
                                   select p;

                if (vm.CheckStatuId != -2)
                {
                    if (vm.CheckStatuId == decimal.Zero)
                    {
                        tbStudyApply = tbStudyApply.Where(d => d.CheckStatus == Code.EnumHelper.CheckStatus.None);
                    }
                    else if (vm.CheckStatuId == decimal.One)
                    {
                        tbStudyApply = tbStudyApply.Where(d => d.CheckStatus == Code.EnumHelper.CheckStatus.Success);
                    }
                    else if (vm.CheckStatuId == -decimal.One)
                    {
                        tbStudyApply = tbStudyApply.Where(d => d.CheckStatus == Code.EnumHelper.CheckStatus.Fail);
                    }
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbStudyApply = tbStudyApply.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText) || d.tbStudy.StudyName.Contains(vm.SearchText));
                }

                vm.CheckStudyApplyList = (from p in tbStudyApply
                                          orderby p.No descending
                                          select new Dto.StudyApply.CheckList
                                          {
                                              Id = p.Id,
                                              No = p.No,
                                              StudyName = p.tbStudy.StudyName,
                                              ApplyFrom = p.tbStudy.ApplyFrom,
                                              ApplyTo = p.tbStudy.ApplyTo,
                                              IsApply = p.tbStudy.IsApply,
                                              IsRoom = p.tbStudy.IsRoom,
                                              YearName = p.tbStudy.tbYear.YearName,
                                              StudyId = p.Id,
                                              Remark = p.Remark,
                                              CheckStatus = p.CheckStatus,
                                              CheckDate = p.CheckDate,
                                              CheckRemark = p.CheckRemark,
                                              CheckUserName = p.tbCheckUser.UserName,
                                              StudentCode = p.tbStudent.StudentCode,
                                              StudentName = p.tbStudent.StudentName,
                                              SexName = p.tbStudent.tbSysUser.tbSex.SexName
                                          }).ToPageList(vm.Page);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckList(Models.StudyApply.CheckList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("CheckList", new
            {
                searchText = vm.SearchText,
                checkStatuId = vm.CheckStatuId,
                PageSize = vm.Page.PageSize,
                PageCount = vm.Page.PageCount,
                PageIndex = vm.Page.PageIndex
            }));
        }

        public ActionResult CheckEdit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyApply.CheckEdit();
                if (id != 0)
                {
                    vm.CheckStatusList = typeof(Code.EnumHelper.CheckStatus).ToItemList();
                    var tb = (from p in db.Table<Study.Entity.tbStudyApply>()
                              where p.Id == id
                              select new Dto.StudyApply.CheckEdit
                              {
                                  Id = p.Id,
                                  Remark = p.Remark,
                                  CheckStatus = p.CheckStatus,
                                  CheckRemark = p.CheckRemark
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.CheckStudyApplyEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckEdit(Models.StudyApply.CheckEdit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.CheckStudyApplyEdit.Id != 0)
                    {
                        var tb = (from p in db.Table<Study.Entity.tbStudyApply>()
                                  where p.Id == vm.CheckStudyApplyEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.tbCheckUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            tb.CheckStatus = vm.CheckStudyApplyEdit.CheckStatus;
                            tb.CheckDate = DateTime.Now;
                            tb.CheckRemark = vm.CheckStudyApplyEdit.CheckRemark;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改晚自习审核");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }
                return Code.MvcHelper.Post(error, Url.Action("CheckList"));
            }
        }

        public ActionResult Export()
        {
            var vm = new Models.StudyApply.List();

            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var tbStudy = from p in db.Table<Study.Entity.tbStudy>()
                              where p.tbYear.IsDeleted == false
                              && p.tbYear.IsDisable == false && p.tbYear.No == DateTime.Now.Year
                              select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbStudy = tbStudy.Where(d => d.StudyName.Contains(vm.SearchText));
                }

                var tbStudyApply = from p in db.Table<Study.Entity.tbStudyApply>()
                                   where p.tbStudent.IsDeleted == false
                                   && p.tbStudy.IsDeleted == false
                                   && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                   select p;

                vm.StudyApplyList = (from p in tbStudy
                                     orderby p.No descending
                                     select new Dto.StudyApply.List
                                     {
                                         Id = tbStudyApply.Where(d => d.tbStudy.Id == p.Id).Select(d => d.Id).FirstOrDefault(),
                                         No = p.No,
                                         StudyName = p.StudyName,
                                         ApplyFrom = p.ApplyFrom,
                                         ApplyTo = p.ApplyTo,
                                         IsApply = p.IsApply,
                                         IsRoom = p.IsRoom,
                                         YearName = p.tbYear.YearName,
                                         StudyId = p.Id,
                                         Remark = tbStudyApply.Where(d => d.tbStudy.Id == p.Id).Select(d => d.Remark).FirstOrDefault(),
                                         CheckStatus = tbStudyApply.Where(d => d.tbStudy.Id == p.Id).Select(d => d.CheckStatus).FirstOrDefault(),
                                         CheckDate = tbStudyApply.Where(d => d.tbStudy.Id == p.Id).Select(d => d.CheckDate).FirstOrDefault(),
                                         CheckRemark = tbStudyApply.Where(d => d.tbStudy.Id == p.Id).Select(d => d.CheckRemark).FirstOrDefault(),
                                         CheckUserName = tbStudyApply.Where(d => d.tbStudy.Id == p.Id).Select(d => d.tbCheckUser.UserName).FirstOrDefault()
                                     }).ToPageList(vm.Page);

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("序号"),
                        new System.Data.DataColumn("晚自习名称"),
                        new System.Data.DataColumn("学年学期学段"),
                        new System.Data.DataColumn("晚自习模式"),
                        new System.Data.DataColumn("开放申请"),
                        new System.Data.DataColumn("申请开始时间"),
                        new System.Data.DataColumn("申请结束时间"),
                        new System.Data.DataColumn("审批状态")
                    });
                var index = 0;
                foreach (var a in vm.StudyApplyList)
                {
                    index++;
                    var dr = dt.NewRow();
                    dr["序号"] = index.ToString();
                    dr["晚自习名称"] = a.StudyName;
                    dr["学年学期学段"] = a.YearName;
                    dr["晚自习模式"] = a.IsRoom ? "教室模式" : "班级模式";
                    dr["开放申请"] = a.IsApply ? "开放" : "关闭";
                    dr["申请开始时间"] = a.ApplyFrom.ToString(Code.Common.StringToDateTime);
                    dr["申请结束时间"] = a.ApplyTo.ToString(Code.Common.StringToDateTime);
                    dr["审批状态"] = a.Id != 0 ? a.CheckStatusName : "";
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
    }
}