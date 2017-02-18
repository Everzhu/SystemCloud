using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dorm.Controllers
{
    public class DormStudentController : Controller
    {
        public ActionResult List()
        {
            var vm = new Models.DormStudent.List();

            using (var db = new XkSystem.Models.DbContext())
            {
                vm.BuildList = Areas.Basis.Controllers.BuildController.SelectList();
                vm.SexList = Dict.Controllers.DictSexController.SelectList();

                var tb = db.Table<Dorm.Entity.tbDormStudent>();
                var dormStudentList = db.Table<Dorm.Entity.tbDormStudent>().ToList();

                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbRoom.RoomName.Contains(vm.SearchText));
                }
                if (vm.BuildId > 0)
                {
                    tb = tb.Where(d => d.tbRoom.tbBuild.Id == vm.BuildId);
                    vm.RoomList = Basis.Controllers.RoomController.SelectList(0, Convert.ToInt32(vm.BuildId));
                }
                else
                {
                    vm.RoomList = Areas.Basis.Controllers.RoomController.SelectList();
                }
                if (vm.SexId > 0)
                {
                    tb = tb.Where(d => d.tbStudent.tbSysUser.tbSex.Id == vm.SexId);
                }
                if (vm.RoomId > 0)
                {
                    tb = tb.Where(d => d.tbRoom.Id == vm.RoomId);
                }

                vm.DormStudentList = (from p in tb
                                      orderby p.No
                                      select new Dto.DormStudent.List()
                                      {
                                          Id = p.Id,
                                          RoomName = p.tbRoom.RoomName,
                                          BuildName = p.tbRoom.tbBuild.BuildName,
                                          YearName = p.tbDorm.tbYear.YearName,
                                          Sex = p.tbStudent.tbSysUser.tbSex.SexName,
                                          StudentCode = p.tbStudent.StudentCode,
                                          StudentName = p.tbStudent.StudentName
                                      }).ToPageList(vm.Page);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DormStudent.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                SearchText = vm.SearchText,
                SexId = vm.SexId,
                RoomId = vm.RoomId,
                BuildId = vm.BuildId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Dorm.Entity.tbDormStudent>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了住宿学生");
                }
                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.DormStudent.Edit();
            var dormStudentList = new List<Dorm.Entity.tbDormStudent>();
            var RoomList = new List<Basis.Entity.tbRoom>();

            using (var db = new XkSystem.Models.DbContext())
            {
                if (id > 0)
                {
                    vm.DormStudentEdit = (from p in db.Table<Dorm.Entity.tbDormStudent>()
                                          where p.Id == id
                                          select new Dto.DormStudent.Edit()
                                          {
                                              Id = p.Id,
                                              DormId = p.tbDorm.Id,
                                              RoomId = p.tbRoom.Id,
                                              BuildId = p.tbRoom.tbBuild.Id,
                                              StudentCode = p.tbStudent.StudentCode
                                          }).FirstOrDefault();
                    vm.RoomList = Basis.Controllers.RoomController.SelectList(vm.DormStudentEdit.RoomId, vm.DormStudentEdit.BuildId);
                }
                dormStudentList = db.Table<Dorm.Entity.tbDormStudent>().Include(d => d.tbRoom).ToList();
                RoomList = db.Table<Basis.Entity.tbRoom>().ToList();
            }
            vm.BuildList = Basis.Controllers.BuildController.SelectList(vm.DormStudentEdit.BuildId);
            vm.DormList = DormController.SelectList(vm.DormStudentEdit.DormId);
            foreach (var v in vm.RoomList)
            {
                if (dormStudentList.Where(d => d.tbRoom.Id == v.Value.ConvertToInt()).Count()
                    >= RoomList.Where(d => d.Id == v.Value.ConvertToInt()).FirstOrDefault().MaxUser)
                {
                    v.Text += "(满)";
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DormStudent.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    //判断是否宿舍超出住宿人数
                    var maxCount = db.Set<Basis.Entity.tbRoom>().Find(vm.DormStudentEdit.RoomId).MaxUser;
                    var nowNum = db.Table<Dorm.Entity.tbDormStudent>().Where(d => d.tbRoom.Id == vm.DormStudentEdit.RoomId).Count();
                    if (nowNum >= maxCount)
                    {
                        error.Add("此宿舍已经住满!");
                        return Code.MvcHelper.Post(error);
                    }

                    if (vm.DormStudentEdit.Id > 0)
                    {
                        var tb = db.Set<Dorm.Entity.tbDormStudent>().Find(vm.DormStudentEdit.Id);
                        tb.tbDorm = db.Set<Dorm.Entity.tbDorm>().Find(vm.DormStudentEdit.DormId);
                        tb.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.DormStudentEdit.RoomId);
                        tb.tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == vm.DormStudentEdit.StudentCode).FirstOrDefault();
                    }
                    else
                    {
                        var tb = new Dorm.Entity.tbDormStudent()
                        {
                            tbDorm = db.Set<Dorm.Entity.tbDorm>().Find(vm.DormStudentEdit.DormId),
                            tbRoom = db.Set<Basis.Entity.tbRoom>().Find(vm.DormStudentEdit.RoomId),
                            tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == vm.DormStudentEdit.StudentCode).FirstOrDefault()
                        };
                        db.Set<Dorm.Entity.tbDormStudent>().Add(tb);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("新增/修改了住宿学生");
                    }
                }
            }
            return Code.MvcHelper.Post(error);
        }

        public ActionResult Export()
        {
            var vm = new Models.DormStudent.Export();
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var tb = db.Table<Dorm.Entity.tbDormStudent>();
                if (vm.BuildId > 0)
                {
                    tb = tb.Where(d => d.tbRoom.tbBuild.Id == vm.BuildId);
                }
                if (vm.RoomId > 0)
                {
                    tb = tb.Where(d => d.tbRoom.Id == vm.RoomId);
                }
                if (vm.SexId > 0)
                {
                    tb = tb.Where(d => d.tbStudent.tbSysUser.tbSex.Id == vm.SexId);
                }
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }

                vm.ExportList = (from p in tb
                                 select new Dto.DormStudent.Export
                                 {
                                     Id = p.Id,
                                     RoomName = p.tbRoom.RoomName,
                                     BuildName = p.tbRoom.tbBuild.BuildName,
                                     YearName = p.tbDorm.tbYear.YearName,
                                     Sex = p.tbStudent.tbSysUser.tbSex.SexName,
                                     StudentCode = p.tbStudent.StudentCode,
                                     StudentName = p.tbStudent.StudentName
                                 }).ToList();
                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学年"),
                        new System.Data.DataColumn("学生学号"),
                        new System.Data.DataColumn("学生姓名"),
                        new System.Data.DataColumn("性别"),
                        new System.Data.DataColumn("宿舍名称"),
                        new System.Data.DataColumn("宿舍楼")
                    });
                foreach (var a in vm.ExportList)
                {
                    var dr = dt.NewRow();
                    dr["学年"] = a.YearName;
                    dr["学生学号"] = a.StudentCode;
                    dr["学生姓名"] = a.StudentName;
                    dr["性别"] = a.Sex;
                    dr["宿舍名称"] = a.RoomName;
                    dr["宿舍楼"] = a.BuildName;
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
            var vm = new Models.DormStudent.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Dorm/Views/DormStudent/DormStudentTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.DormStudent.Import vm)
        {
            vm.ImportList = new List<Dto.DormStudent.Import>();
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

                    var tbList = new List<string>() { "住宿名称", "学生学号", "学生姓名", "宿舍名称", "宿舍楼" };

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
                        var dto = new Dto.DormStudent.Import()
                        {
                            DormName = dr["住宿名称"].ConvertToString(),
                            RoomName = dr["宿舍名称"].ConvertToString(),
                            BuildName = dr["宿舍楼"].ConvertToString(),
                            StudentCode = dr["学生学号"].ConvertToString(),
                            StudentName = dr["学生姓名"].ConvertToString()
                        };
                        if (vm.ImportList.Where(d => d.DormName == dto.DormName
                                                 && d.RoomName == dto.RoomName
                                                 && d.BuildName == dto.BuildName
                                                 && d.StudentCode == dto.StudentCode
                                                 && d.StudentName == dto.StudentName).Any() == false)
                        {
                            vm.ImportList.Add(dto);
                        }
                    }

                    vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.RoomName)
                                            && string.IsNullOrEmpty(d.DormName)
                                            && string.IsNullOrEmpty(d.BuildName)
                                            && string.IsNullOrEmpty(d.StudentName)
                                            && string.IsNullOrEmpty(d.StudentCode));

                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    var RoomList = db.Table<Basis.Entity.tbRoom>()
                        .Include(d => d.tbBuild).ToList();
                    var BuildList = db.Table<Basis.Entity.tbBuild>().ToList();
                    var dormStudentList = db.Table<Dorm.Entity.tbDormStudent>()
                        .Include(d => d.tbStudent)
                        .Include(d => d.tbRoom)
                        .Include(d => d.tbRoom.tbBuild).ToList();
                    var studentList = db.Table<Student.Entity.tbStudent>().ToList();
                    var dormList = db.Table<Dorm.Entity.tbDorm>().ToList();

                    #region 验证数据格式是否正确
                    foreach (var item in vm.ImportList)
                    {
                        if (string.IsNullOrEmpty(item.RoomName)
                                        || string.IsNullOrEmpty(item.DormName)
                                        || string.IsNullOrEmpty(item.BuildName)
                                        || string.IsNullOrEmpty(item.StudentName)
                                        || string.IsNullOrEmpty(item.StudentCode))
                        {
                            item.Error += "字段不能为空!";
                        }
                        if (studentList.Where(d => d.StudentCode == item.StudentCode && d.StudentName == item.StudentName).Any() == false)
                        {
                            item.Error += "学生不存在；";
                        }
                        if (RoomList.Where(d => d.RoomName == item.RoomName && d.tbBuild.BuildName == item.BuildName).Any() == false)
                        {
                            item.Error += "宿舍不存在；";
                        }
                        if (!vm.IsUpdate && dormStudentList.Where(d => d.tbStudent.StudentCode == item.StudentCode).Count() > 0)
                        {
                            item.Error += "系统中已存在该记录!";
                        }
                    }

                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 数据导入
                    List<Dorm.Entity.tbDormStudent> tbDormStudentList = new List<Dorm.Entity.tbDormStudent>();
                    foreach (var v in vm.ImportList)
                    {
                        if (dormStudentList.Where(d => d.tbStudent.StudentCode == v.StudentCode && d.tbStudent.StudentName == v.StudentName).Count() > 0)
                        {
                            if (vm.IsUpdate)
                            {
                                var tb = dormStudentList.Where(d => d.tbStudent.StudentCode == v.StudentCode && d.tbStudent.StudentName == v.StudentName).FirstOrDefault();
                                tb.tbRoom = RoomList.Where(d => d.RoomName == v.RoomName && d.tbBuild.BuildName == v.BuildName).FirstOrDefault();
                                tb.tbDorm = dormList.Where(d => d.DormName == v.DormName).FirstOrDefault();
                            }
                        }
                        else
                        {
                            var tb = new Dorm.Entity.tbDormStudent();
                            tb.tbDorm = dormList.Where(d => d.DormName == v.DormName).FirstOrDefault();
                            tb.tbStudent = studentList.Where(d => d.StudentCode == v.StudentCode && d.StudentName == v.StudentName).FirstOrDefault();
                            tb.tbRoom = RoomList.Where(d => d.RoomName == v.RoomName && d.tbBuild.BuildName == v.BuildName).FirstOrDefault();
                            
                            tbDormStudentList.Add(tb);
                        }
                    }
                    #endregion

                    db.Set<Dorm.Entity.tbDormStudent>().AddRange(tbDormStudentList);

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

        public ActionResult DormStudents(int id)
        {
            var vm = new Models.DormStudent.DormStudents();

            using (var db = new XkSystem.Models.DbContext())
            {
                vm.DormStudentList = (from p in db.Table<Dorm.Entity.tbDormStudent>()
                                      where p.tbRoom.Id == id
                                      orderby p.No
                                      select new Dto.DormStudent.DormStudents()
                                      {
                                          Id = p.Id,
                                          Address = p.tbStudent.Address,
                                          StudentCode = p.tbStudent.StudentCode,
                                          StudentName = p.tbStudent.StudentName
                                      }).ToList();
            }

            return View(vm);
        }
    }
}