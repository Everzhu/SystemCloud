using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Study.Controllers
{
    public class StudyController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Study.List();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);

                var IsRoomList = new List<System.Web.Mvc.SelectListItem>();
                IsRoomList.Add(new SelectListItem { Text = "全部", Value = "0" });
                IsRoomList.Add(new SelectListItem { Text = "教室模式", Value = "1" });
                IsRoomList.Add(new SelectListItem { Text = "班级模式", Value = "2" });
                vm.IsRoomList = IsRoomList;

                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                var tb = from p in db.Table<Study.Entity.tbStudy>()
                         select p;

                if (vm.YearId != 0)
                {
                    tb = tb.Where(d => d.tbYear.Id == vm.YearId);
                }

                if (vm.IsRoomId != 0)
                {
                    tb = tb.Where(d => d.IsRoom == (vm.IsRoomId == 2 ? false : true));
                }
                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.StudyName.Contains(vm.SearchText));
                }

                vm.StudyList = (from p in tb
                                orderby p.Id descending
                                select new Dto.Study.List
                                {
                                    Id = p.Id,
                                    No = p.No,
                                    IsApply = p.IsApply,
                                    IsRoom = p.IsRoom,
                                    StudyName = p.StudyName,
                                    YearName = p.tbYear.YearName,
                                    ApplyFrom = p.ApplyFrom,
                                    ApplyTo = p.ApplyTo
                                }).ToPageList(vm.Page);

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Study.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                yearId = vm.YearId,
                isRoomId = vm.IsRoomId,
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
                var tb = (from p in db.Table<Study.Entity.tbStudy>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除晚自习");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Study.Edit();
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.StudyEdit.YearId == 0)
                {
                    vm.StudyEdit.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                if (id != 0)
                {
                    var tb = (from p in db.Table<Study.Entity.tbStudy>()
                              where p.Id == id
                              select new Dto.Study.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  IsApply = p.IsApply,
                                  IsRoom = p.IsRoom,
                                  StudyName = p.StudyName,
                                  YearId = p.tbYear.Id,
                                  ApplyFrom = p.ApplyFrom,
                                  ApplyTo = p.ApplyTo
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.StudyEdit = tb;
                        vm.ApplyFrom = tb.ApplyFrom.ToString(XkSystem.Code.Common.StringToDateTime);
                        vm.ApplyTo = tb.ApplyTo.ToString(XkSystem.Code.Common.StringToDateTime);
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Study.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.StudyEdit.Id == 0)
                    {
                        var tb = new Study.Entity.tbStudy();
                        tb.No = vm.StudyEdit.No == null ? db.Table<Study.Entity.tbStudy>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudyEdit.No;
                        tb.StudyName = vm.StudyEdit.StudyName;
                        tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.StudyEdit.YearId);
                        tb.IsApply = vm.StudyEdit.IsApply;
                        tb.IsRoom = vm.StudyEdit.IsRoom;
                        tb.ApplyFrom = DateTime.Parse(vm.ApplyFrom);
                        tb.ApplyTo = DateTime.Parse(vm.ApplyTo);
                        db.Set<Study.Entity.tbStudy>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加晚自习");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Study.Entity.tbStudy>()
                                  where p.Id == vm.StudyEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.StudyEdit.No == null ? db.Table<Study.Entity.tbStudy>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.StudyEdit.No;
                            tb.StudyName = vm.StudyEdit.StudyName;
                            tb.tbYear = db.Set<Basis.Entity.tbYear>().Find(vm.StudyEdit.YearId);
                            tb.IsApply = vm.StudyEdit.IsApply;
                            tb.IsRoom = vm.StudyEdit.IsRoom;
                            tb.ApplyFrom = DateTime.Parse(vm.ApplyFrom);
                            tb.ApplyTo = DateTime.Parse(vm.ApplyTo);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改晚自习");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetIsRoom(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Study.Entity.tbStudy>().Find(id);
                if (tb != null)
                {
                    tb.IsRoom = !tb.IsRoom;

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改晚自习模式");
                    }
                }
                return Code.MvcHelper.Post(null, Url.Action("List"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetIsApply(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Study.Entity.tbStudy>().Find(id);
                if (tb != null)
                {
                    tb.IsApply = !tb.IsApply;
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改晚自习申请状态");
                    }
                }
                return Code.MvcHelper.Post(null, Url.Action("List"));
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Study.Entity.tbStudy>()
                          orderby p.No descending
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.StudyName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        public ActionResult Export()
        {
            var vm = new Models.Study.Export();

            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var tb = from p in db.Table<Study.Entity.tbStudy>()
                         select p;

                if (vm.YearId != 0)
                {
                    tb = tb.Where(d => d.tbYear.Id == vm.YearId);
                }

                if (vm.IsRoomId != 0)
                {
                    tb = tb.Where(d => d.IsRoom == (vm.IsRoomId == 2 ? false : true));
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.StudyName.Contains(vm.SearchText));
                }

                vm.ExportList = (from p in tb
                                 orderby p.Id descending
                                 select new Dto.Study.Export
                                 {
                                     Id = p.Id,
                                     No = p.No,
                                     IsApply = p.IsApply,
                                     IsRoom = p.IsRoom,
                                     StudyName = p.StudyName,
                                     YearName = p.tbYear.YearName,
                                     ApplyFrom = p.ApplyFrom,
                                     ApplyTo = p.ApplyTo
                                 }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("排序"),
                        new System.Data.DataColumn("晚自习名称"),
                        new System.Data.DataColumn("学年学期学段"),
                        new System.Data.DataColumn("晚自习模式"),
                        new System.Data.DataColumn("申请开始时间"),
                        new System.Data.DataColumn("申请结束时间"),
                        new System.Data.DataColumn("开放申请")
                    });
                foreach (var a in vm.ExportList)
                {
                    var dr = dt.NewRow();
                    dr["排序"] = a.No;
                    dr["晚自习名称"] = a.StudyName;
                    dr["学年学期学段"] = a.YearName;
                    dr["晚自习模式"] = a.IsRoom ? "教室模式" : "班级模式";
                    dr["申请开始时间"] = a.ApplyFrom.ToString(Code.Common.StringToDateTime);
                    dr["申请结束时间"] = a.ApplyTo.ToString(Code.Common.StringToDateTime);
                    dr["开放申请"] = a.IsApply ? "开放" : "关闭";
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

        public ActionResult Import()
        {
            var vm = new Models.Study.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Study/Views/Study/StudyTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.Study.Import vm)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);
                using (var db = new XkSystem.Models.DbContext())
                {
                    #region 1、Excel模版校验
                    if (Code.Common.GetFileType(file.FileName) != Code.FileType.Excel)
                    {
                        ModelState.AddModelError("", "上传的文件不是正确的EXCLE文件!");
                        return View(vm);
                    }

                    var dt = Code.NpoiHelper.ExcelToDataTable(fileSave, System.IO.Path.GetExtension(file.FileName), string.Empty);
                    if (dt == null)
                    {
                        ModelState.AddModelError("", "无法读取上传的文件，请检查文件格式是否正确!");
                        return View(vm);
                    }
                    var tbList = new List<string>() { "排序", "晚自习名称", "学年", "晚自习模式", "申请开始时间", "申请结束时间", "开放申请" };

                    var Text = string.Empty;
                    foreach (var a in tbList)
                    {
                        if (!dt.Columns.Contains(a.ToString()))
                        {
                            Text += a + ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(Text))
                    {
                        ModelState.AddModelError("", "上传的EXCEL晚自习内容与预期不一致，缺少对应的字段：" + Text);
                        return View(vm);
                    }
                    #endregion

                    #region 2、Excel数据读取
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dtoStudy = new Dto.Study.Import()
                        {
                            No = Convert.ToString(dr["排序"]),
                            StudyName = Convert.ToString(dr["晚自习名称"]),
                            YearName = Convert.ToString(dr["学年"]),
                            IsRoom = Convert.ToString(dr["晚自习模式"]),
                            IsApply = Convert.ToString(dr["开放申请"]),
                            ApplyFrom = Convert.ToString(dr["申请开始时间"]),
                            ApplyTo = Convert.ToString(dr["申请结束时间"])
                        };
                        if (vm.ImportList.Where(d => d.No == dtoStudy.No
                                                && d.StudyName == dtoStudy.StudyName
                                                && d.YearName == dtoStudy.YearName
                                                && d.IsRoom == dtoStudy.IsRoom
                                                && d.IsApply == dtoStudy.IsApply
                                                && d.ApplyFrom == dtoStudy.ApplyFrom
                                                && d.ApplyTo == dtoStudy.ApplyTo).Count() == 0)
                        {
                            vm.ImportList.Add(dtoStudy);
                        }
                    }
                    vm.ImportList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.No) &&
                        string.IsNullOrEmpty(d.StudyName) &&
                        string.IsNullOrEmpty(d.YearName) &&
                        string.IsNullOrEmpty(d.IsRoom) &&
                        string.IsNullOrEmpty(d.IsApply) &&
                        string.IsNullOrEmpty(d.ApplyFrom) &&
                        string.IsNullOrEmpty(d.ApplyTo)
                    );
                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "Excel未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    #region 3、Excel格式校验

                    //晚自习列表
                    var StudyList = (from p in db.Table<Study.Entity.tbStudy>()
                                     .Include(d => d.tbYear)
                                     select p).ToList();
                    //学年
                    var YearList = (from p in db.Table<Basis.Entity.tbYear>() select p).ToList();
                    //晚自习模式
                    var IsRoomList = new List<string>();
                    IsRoomList.Add("教室模式");
                    IsRoomList.Add("班级模式");
                    //开放申请
                    var IsApplyList = new List<string>();
                    IsRoomList.Add("开放");
                    IsRoomList.Add("关闭");

                    foreach (var item in vm.ImportList)
                    {
                        int No = 0;
                        if (int.TryParse(item.No, out No) == false || No <= 0)
                        {
                            item.Error = item.Error + "排序必须是正整数!";
                            continue;
                        }
                        if (string.IsNullOrEmpty(item.StudyName))
                        {
                            item.Error = item.Error + "晚自习名称不能为空!";
                            continue;
                        }
                        if (string.IsNullOrEmpty(item.YearName))
                        {
                            item.Error = item.Error + "学年不能为空!";
                            continue;
                        }
                        else
                        {
                            if (YearList.Where(d => d.YearName == item.YearName).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "学年不存在数据库!";
                                continue;
                            }
                        }
                        if (string.IsNullOrEmpty(item.IsRoom))
                        {
                            item.IsRoom = "教室模式";
                        }
                        else
                        {
                            if (IsRoomList.Where(d => d == item.IsRoom).Count() == decimal.Zero)
                            {
                                item.Error += "晚自习模式必须是：【教室模式】或者【班级模式】!";
                                continue;
                            }
                        }
                        if (!string.IsNullOrEmpty(item.ApplyFrom))
                        {
                            DateTime timeTemp = new DateTime();
                            if (!DateTime.TryParse(item.ApplyFrom, out timeTemp))
                            {
                                item.Error += "【申请开始时间】格式不正确，请输入正确的时间格式如：" + DateTime.Now.ToString(XkSystem.Code.Common.StringToDateTime);
                                continue;
                            }
                        }
                        else
                        {
                            item.ApplyFrom = DateTime.Now.ToString(XkSystem.Code.Common.StringToDateTime);
                        }
                        if (!string.IsNullOrEmpty(item.ApplyTo))
                        {
                            DateTime timeTemp = new DateTime();
                            if (!DateTime.TryParse(item.ApplyTo, out timeTemp))
                            {
                                item.Error += "【申请结束时间】格式不正确，请输入正确的时间格式如：" + DateTime.Now.ToString(XkSystem.Code.Common.StringToDateTime);
                                continue;
                            }
                        }
                        else
                        {
                            item.ApplyTo = DateTime.Now.ToString(XkSystem.Code.Common.StringToDateTime);
                        }
                        if (string.IsNullOrEmpty(item.IsApply))
                        {
                            item.IsApply = "关闭";
                        }
                        else
                        {
                            if (IsRoomList.Where(d => d == item.IsRoom).Count() == decimal.Zero)
                            {
                                item.Error += "开放申请必须是：【开放】或者【关闭】!";
                                continue;
                            }
                        }
                        if (vm.IsUpdate)
                        {
                            if (StudyList.Where(d => d.tbYear.YearName == item.YearName && d.StudyName == item.StudyName).Count() > 1)
                            {
                                item.Error += "系统中该晚自习数据存在重复，无法确认需要更新的记录!";
                                continue;
                            }
                        }
                        else
                        {
                            if (StudyList.Where(d => d.tbYear.YearName == item.YearName && d.StudyName == item.StudyName).Count() > 0)
                            {
                                item.Error += "系统中已存在该记录!";
                                continue;
                            }
                        }
                    }
                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 4、Excel执行导入
                    var addStudyList = new List<Study.Entity.tbStudy>();
                    foreach (var item in vm.ImportList)
                    {
                        Study.Entity.tbStudy tb = null;
                        if (StudyList.Where(d => d.tbYear.YearName == item.YearName && d.StudyName == item.StudyName).Count() > 0)
                        {
                            if (vm.IsUpdate)
                            {
                                tb = StudyList.Where(d => d.tbYear.YearName == item.YearName && d.StudyName == item.StudyName).FirstOrDefault();
                                tb.No = item.No.ConvertToInt();
                                tb.StudyName = item.StudyName;
                                tb.IsApply = item.IsApply == "开放" ? true : false;
                                tb.IsRoom = item.IsRoom == "教室模式" ? true : false;
                                tb.ApplyFrom = Convert.ToDateTime(item.ApplyFrom);
                                tb.ApplyTo = Convert.ToDateTime(item.ApplyTo);
                            }
                        }
                        else
                        {
                            tb = new Study.Entity.tbStudy();
                            tb.No = item.No.ConvertToInt();
                            tb.StudyName = item.StudyName;
                            tb.tbYear = db.Table<Basis.Entity.tbYear>().Where(d => d.YearName == item.YearName).FirstOrDefault();
                            tb.IsApply = item.IsApply == "开放" ? true : false;
                            tb.IsRoom = item.IsRoom == "教室模式" ? true : false;
                            tb.ApplyFrom = Convert.ToDateTime(item.ApplyFrom);
                            tb.ApplyTo = Convert.ToDateTime(item.ApplyTo);
                            addStudyList.Add(tb);
                        }
                    }
                    db.Set<Study.Entity.tbStudy>().AddRange(addStudyList);
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了晚自习");
                        vm.Status = true;
                    }
                    #endregion
                }
            }
            vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }
    }
}