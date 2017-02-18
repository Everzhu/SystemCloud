using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dorm.Controllers
{
    public class DormTeacherController : Controller
    {
        public ActionResult List()
        {
            var vm = new Models.DormTeacher.List();
            vm.BuildList = Areas.Basis.Controllers.BuildController.SelectList();

            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Dorm.Entity.tbDormTeacher>();
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbRoom.RoomName.Contains(vm.SearchText)
                                    || d.tbTeacher.TeacherName.Contains(vm.SearchText)
                                    || d.tbTeacher.TeacherCode.Contains(vm.SearchText));
                }
                if (vm.BuildId > 0)
                {
                    tb = tb.Where(d => d.tbRoom.tbBuild.Id == vm.BuildId);
                }
                vm.DormTeacherList = (from p in tb
                                      orderby p.No
                                      select new Dto.DormTeacher.List()
                                      {
                                          Id = p.Id,
                                          No = p.No,
                                          BuildName = p.tbRoom.tbBuild.BuildName,
                                          RoomName = p.tbRoom.RoomName,
                                          TeacherCode = p.tbTeacher.TeacherCode,
                                          TeacherName = p.tbTeacher.TeacherName
                                      }).ToPageList(vm.Page);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DormTeacher.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                SearchText = vm.SearchText,
                BuildId = vm.BuildId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Dorm.Entity.tbDormTeacher>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了住宿教管");
                }
            }
            return Code.MvcHelper.Post();
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.DormTeacher.Edit();

            if (id > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.DormTeacherEdit = (from p in db.Table<Dorm.Entity.tbDormTeacher>()
                                          where p.Id == id
                                          select new Dto.DormTeacher.Edit()
                                          {
                                              Id = p.Id,
                                              No = p.No,
                                              TeacherCode = p.tbTeacher.TeacherCode,
                                              BuildId = p.tbRoom.tbBuild.Id,
                                              RoomId = p.tbRoom.Id
                                          }).FirstOrDefault();
                }
                vm.RoomList = Basis.Controllers.RoomController.SelectList(vm.DormTeacherEdit.RoomId, vm.DormTeacherEdit.BuildId);
            }
            vm.BuildList = Basis.Controllers.BuildController.SelectList(vm.DormTeacherEdit.BuildId);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DormTeacher.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    if (db.Table<Teacher.Entity.tbTeacher>().Where(d => d.TeacherCode == vm.DormTeacherEdit.TeacherCode).Count() == 0)
                    {
                        error.Add("无法获取教师信息!");
                        return Code.MvcHelper.Post(error);
                    }
                    if (vm.DormTeacherEdit.Id > 0)
                    {
                        var tb = db.Set<Dorm.Entity.tbDormTeacher>().Find(vm.DormTeacherEdit.Id);
                        tb.No = vm.DormTeacherEdit.No;
                        tb.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.DormTeacherEdit.RoomId);
                        tb.tbTeacher = db.Table<Teacher.Entity.tbTeacher>().Where(d => d.TeacherCode == vm.DormTeacherEdit.TeacherCode).FirstOrDefault();
                    }
                    else
                    {
                        var tb = new Dorm.Entity.tbDormTeacher()
                        {
                            No = vm.DormTeacherEdit.No,
                            tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.DormTeacherEdit.RoomId),
                            tbTeacher = db.Table<Teacher.Entity.tbTeacher>().Where(d => d.TeacherCode == vm.DormTeacherEdit.TeacherCode).FirstOrDefault()
                        };
                        db.Set<Dorm.Entity.tbDormTeacher>().Add(tb);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("新增/修改了住宿教管");
                    }
                }
            }
            return Code.MvcHelper.Post(error);
        }

        public ActionResult Import()
        {
            var vm = new Models.DormTeacher.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Dorm/Views/DormTeacher/RoomTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.DormTeacher.Import vm)
        {
            vm.ImportList = new List<Dto.DormTeacher.Import>();
            if (ModelState.IsValid)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);

                using (var db = new XkSystem.Models.DbContext())
                {
                    #region 上传数据
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

                    var tbList = new List<string>() { "排序", "宿舍名称", "宿舍楼", "容纳人数", "宿管教职工号", "宿管姓名" };

                    var Text = string.Empty;
                    foreach (var a in tbList)
                    {
                        if (dt.Columns.Contains(a.ToString()) == false)
                        {
                            Text += a + ",";
                        }
                    }

                    if (string.IsNullOrEmpty(Text) == false)
                    {
                        ModelState.AddModelError("", "上传的EXCEL内容与预期不一致，缺少对应的字段：" + Text);
                        return View(vm);
                    }
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dto = new Dto.DormTeacher.Import()
                        {
                            No = dr["排序"].ConvertToString(),
                            RoomName = dr["宿舍名称"].ConvertToString(),
                            BuildName = dr["宿舍楼"].ConvertToString(),
                            MaxCount = dr["容纳人数"].ConvertToString(),
                            TeacherCode = dr["宿管教职工号"].ConvertToString(),
                            TeacherName = dr["宿管姓名"].ConvertToString()
                        };
                        if (vm.ImportList.Where(d => d.No == dto.No
                                                 && d.RoomName == dto.RoomName
                                                 && d.BuildName == dto.BuildName
                                                 && d.TeacherCode == dto.TeacherCode
                                                 && d.TeacherName == dto.TeacherName
                                                 && d.MaxCount == dto.MaxCount).Any() == false)
                        {
                            vm.ImportList.Add(dto);
                        }
                    }

                    vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.RoomName)
                                            && string.IsNullOrEmpty(d.BuildName)
                                            && string.IsNullOrEmpty(d.No)
                                            && string.IsNullOrEmpty(d.TeacherCode)
                                            && string.IsNullOrEmpty(d.TeacherName)
                                            && string.IsNullOrEmpty(d.MaxCount));

                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    var RoomList = db.Table<Basis.Entity.tbRoom>().Include(d => d.tbBuild).ToList();
                    var BuildList = db.Table<Basis.Entity.tbBuild>().ToList();
                    var dormTeacherList = db.Table<Dorm.Entity.tbDormTeacher>()
                        .Include(d => d.tbRoom)
                        .Include(d => d.tbRoom.tbBuild)
                        .Include(d => d.tbTeacher).ToList();
                    var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();

                    #region 验证数据格式是否正确
                    foreach (var item in vm.ImportList)
                    {
                        if (string.IsNullOrEmpty(item.RoomName))
                        {
                            item.Error += "宿舍名称不能为空!";
                        }
                        if (string.IsNullOrEmpty(item.TeacherCode) || string.IsNullOrEmpty(item.TeacherName))
                        {
                            item.Error += "教师编码和教师名称不能为空!";
                        }
                        if (string.IsNullOrEmpty(item.BuildName))
                        {
                            item.Error += "宿舍楼不能为空!";
                        }
                        if (BuildList.Where(d => d.BuildName == item.BuildName).Count() == 0)
                        {
                            item.Error += "宿舍楼不存在；";
                        }
                        if (vm.IsUpdate)
                        {
                            //if (dormTeacherList.Where(d => d.tbTeacher.TeacherCode == item.TeacherCode
                            //                            && d.tbTeacher.TeacherName == item.TeacherName
                            //                            && d.tbRoom.RoomName == item.RoomName
                            //                            && d.tbRoom.tbBuild.BuildName == item.BuildName).Any())
                            //{
                            //    item.Error += "该条数据重复!";
                            //}
                        }
                        else
                        {
                            if (dormTeacherList.Where(d => d.tbTeacher.TeacherCode == item.TeacherCode
                                && d.tbRoom.RoomName == item.RoomName).Any())
                            {
                                item.Error += "系统中已存在该记录!";
                            }
                        }
                        if (teacherList.Where(d => d.TeacherCode == item.TeacherCode && d.TeacherName == item.TeacherName).Any() == false)
                        {
                            item.Error += "该教职工不存在!";
                        }
                        int num = 0;
                        if (!string.IsNullOrEmpty(item.No))
                        {
                            if (!int.TryParse(item.No, out num) || num < 0)
                            {
                                item.Error += "容纳人数必须是正整数!";
                            }
                        }
                    }

                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 数据导入，判断关键字，存在的数据做更新，不存在的做新增
                    List<Dorm.Entity.tbDormTeacher> tbDormTeacherList = new List<Dorm.Entity.tbDormTeacher>();
                    foreach (var item in vm.ImportList)
                    {
                        if (dormTeacherList.Where(d => d.tbRoom.RoomName == item.RoomName
                            && d.tbRoom.tbBuild.BuildName == item.BuildName).Count() > 0)
                        {
                            if (vm.IsUpdate)
                            {
                                var tb = dormTeacherList.Where(d => d.tbRoom.RoomName == item.RoomName
                                            && d.tbRoom.tbBuild.BuildName == item.BuildName).FirstOrDefault();
                                tb.No = item.No.ConvertToInt();
                                tb.tbTeacher = teacherList.Where(d => d.TeacherCode == item.TeacherCode && d.TeacherName == item.TeacherName).FirstOrDefault();
                            }
                        }
                        else
                        {
                            var tb = new Dorm.Entity.tbDormTeacher()
                            {
                                No = item.No.ConvertToInt(),
                                tbRoom = RoomList.Where(d => d.RoomName == item.RoomName && d.tbBuild.BuildName == item.BuildName).FirstOrDefault(),
                                tbTeacher = teacherList.Where(d => d.TeacherCode == item.TeacherCode && d.TeacherName == item.TeacherName).FirstOrDefault()
                            };

                            tbDormTeacherList.Add(tb);
                        }
                    }
                    #endregion

                    if (tbDormTeacherList.Count > 0)
                    {
                        db.Set<Dorm.Entity.tbDormTeacher>().AddRange(tbDormTeacherList);
                    }

                    if (db.SaveChanges() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入宿舍信息");
                        vm.Status = true;
                    }
                }
            }

            return View(vm);
        }

        public JsonResult SelectTeacher(string SearchText)
        {
            var tb = new List<Teacher.Entity.tbTeacher>();
            if (!string.IsNullOrEmpty(SearchText))
            {
                tb = Teacher.Controllers.TeacherController.SelectTeacher(SearchText);
            }
            return Json(tb, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        public static List<Dorm.Entity.tbDormTeacher> BuildList(XkSystem.Models.DbContext db
            , List<Dto.DormTeacher.Edit> editList
            , List<Basis.Entity.tbRoom> tbRoomList
            , List<Teacher.Entity.tbTeacher> tbTeacherList)
        {
            List<Dorm.Entity.tbDormTeacher> list = new List<Dorm.Entity.tbDormTeacher>();
            var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();
            var RoomList = db.Table<Basis.Entity.tbRoom>().Include(d => d.tbBuild).ToList();
            var dormTeacherList = db.Table<Dorm.Entity.tbDormTeacher>()
                .Include(d => d.tbRoom)
                .Include(d => d.tbRoom.tbBuild)
                .Include(d => d.tbTeacher).ToList();

            foreach (var v in editList)
            {
                var dormTeacher = new Dorm.Entity.tbDormTeacher();
                if (dormTeacherList.Where(d => d.tbRoom.RoomName == v.RoomName && d.tbRoom.tbBuild.BuildName == v.BuildName).Count() > 0)
                {
                    dormTeacher = dormTeacherList.Where(d => d.tbRoom.RoomName == v.RoomName && d.tbRoom.tbBuild.BuildName == v.BuildName).FirstOrDefault();
                }
                else
                {
                    dormTeacher = new Dorm.Entity.tbDormTeacher();
                    if (RoomList.Where(d => d.RoomName == v.RoomName && d.tbBuild.BuildName == v.BuildName).Count() > 0)
                    {
                        dormTeacher.tbRoom = RoomList.Where(d => d.RoomName == v.RoomName && d.tbBuild.BuildName == v.BuildName).FirstOrDefault();
                    }
                    else
                    {
                        dormTeacher.tbRoom = tbRoomList.Where(d => d.RoomName == v.RoomName && d.tbBuild.BuildName == v.BuildName).FirstOrDefault();
                    }
                }

                if (teacherList.Where(d => d.TeacherCode == v.TeacherCode && d.TeacherName == v.TeacherName).Count() > 0)
                {
                    dormTeacher.tbTeacher = teacherList.Where(d => d.TeacherCode == v.TeacherCode && d.TeacherName == v.TeacherName).FirstOrDefault();
                }
                else
                {
                    dormTeacher.tbTeacher = tbTeacherList.Where(d => d.TeacherCode == v.TeacherCode && d.TeacherName == v.TeacherName).FirstOrDefault();
                }

                list.Add(dormTeacher);
            }

            return list;
        }
    }
}