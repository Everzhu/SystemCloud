using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Study.Controllers
{
    public class StudyDataController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyData.List();

                vm.StudyList = Controllers.StudyController.SelectList();

                if (string.IsNullOrEmpty(vm.DateSearch))
                {
                    vm.DateSearch = DateTime.Now.AddDays(0).ToString("yyyy-MM-dd");
                }

                var vDateSearch = Convert.ToDateTime(vm.DateSearch);

                if (vm.StudyId == 0 && vm.StudyList.Count > 0)
                {
                    vm.StudyId = vm.StudyList.FirstOrDefault().Value.ConvertToInt();
                }

                var tbStudy = (from p in db.Table<Study.Entity.tbStudy>()
                               where p.Id == vm.StudyId
                               && p.tbYear.IsDeleted == false
                               select p).FirstOrDefault();

                if (tbStudy != null)
                {
                    vm.IsRoomType = tbStudy.IsRoom;//赋值自习模式

                    var tbStudyDataList = from p in db.Table<Study.Entity.tbStudyData>()
                                          where p.InputDate.Day == vDateSearch.Day 
                                          && p.tbStudy.Id == vm.StudyId
                                          && p.tbStudent.IsDeleted == false
                                           && p.tbStudyOption.IsDeleted == false
                                           && p.tbSysUser.IsDeleted == false
                                          select p;
                    //教室模式
                    if (tbStudy.IsRoom)
                    {
                        vm.RoomOrClassList = Controllers.StudyRoomController.SelectList(vm.StudyId);
                        vm.RoomOrClassList.Insert(0, new SelectListItem { Text = "", Value = "0" });

                        var tb = from p in db.Table<Study.Entity.tbStudyRoomStudent>()
                                 where p.tbStudy.Id == vm.StudyId && p.tbRoom.Id == vm.RoomOrClassId
                                 && p.tbStudy.IsDeleted == false
                                 && p.tbRoom.IsDeleted == false
                                 && p.tbStudent.IsDeleted == false
                                 select p;

                        if (string.IsNullOrEmpty(vm.SearchText) == false)
                        {
                            tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                        }

                        vm.StudyDataList = (from p in tb
                                            orderby p.tbStudent.StudentCode
                                            select new Dto.StudyData.List
                                            {
                                                Id = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.Id).FirstOrDefault(),
                                                InputDate = vDateSearch,
                                                RoomName = p.tbRoom.RoomName,
                                                StudentCode = p.tbStudent.StudentCode,
                                                StudentName = p.tbStudent.StudentName,
                                                SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                                StudentId = p.tbStudent.Id,
                                                StudyId = vm.StudyId,
                                                StudyOptionName = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.tbStudyOption.StudyOptionName).FirstOrDefault(),
                                                Remark = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.Remark).FirstOrDefault(),
                                                SysUserName = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.tbSysUser.UserName).FirstOrDefault()
                                            }).ToList();
                    }
                    else//班级模式
                    {
                        vm.RoomOrClassList = Controllers.StudyClassController.SelectList(vm.StudyId);
                        vm.RoomOrClassList.Insert(0, new SelectListItem { Text = "", Value = "0" });

                        var tb = from p in db.Table<Study.Entity.tbStudyClassStudent>()
                                 where p.tbStudy.Id == vm.StudyId && p.tbClass.Id == vm.RoomOrClassId
                                 && p.tbStudy.IsDeleted == false
                                 && p.tbClass.IsDeleted == false
                                 && p.tbStudent.IsDeleted == false
                                 select p;

                        if (string.IsNullOrEmpty(vm.SearchText) == false)
                        {
                            tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                        }

                        vm.StudyDataList = (from p in tb
                                            orderby p.tbStudent.StudentCode
                                            select new Dto.StudyData.List
                                            {
                                                Id = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.Id).FirstOrDefault(),
                                                InputDate = vDateSearch,
                                                RoomName = p.tbClass.tbRoom.RoomName,
                                                StudentCode = p.tbStudent.StudentCode,
                                                StudentName = p.tbStudent.StudentName,
                                                SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                                StudentId = p.tbStudent.Id,
                                                StudyId = vm.StudyId,
                                                StudyOptionName = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.tbStudyOption.StudyOptionName).FirstOrDefault(),
                                                Remark = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.Remark).FirstOrDefault(),
                                                SysUserName = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.tbSysUser.UserName).FirstOrDefault()
                                            }).ToList();
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudyData.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                studyId = vm.StudyId,
                roomOrClassId = vm.RoomOrClassId,
                dateSearch = vm.DateSearch
            }));
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyData.Edit();
                vm.StudyOptionList = Controllers.StudyOptionController.SelectList();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Study.Entity.tbStudyData>()
                              where p.Id == id
                              select new Dto.StudyData.Edit
                              {
                                  Id = p.Id,
                                  StudyOptionId = p.tbStudyOption.Id,
                                  Remark = p.Remark,
                              }).FirstOrDefault();

                    if (tb != null)
                    {
                        vm.StudyDataEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.StudyData.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.StudyDataEdit.Id == 0)
                    {
                        var tb = new Study.Entity.tbStudyData();
                        tb.InputDate = Convert.ToDateTime(vm.DateSearch);
                        tb.Remark = vm.StudyDataEdit.Remark;
                        tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                        tb.tbStudy = db.Set<Study.Entity.tbStudy>().Find(vm.StudyId);
                        tb.tbStudyOption = db.Set<Study.Entity.tbStudyOption>().Find(vm.StudyDataEdit.StudyOptionId);
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        db.Set<Study.Entity.tbStudyData>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加晚自习表现");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Study.Entity.tbStudyData>()
                                  where p.Id == vm.StudyDataEdit.Id
                                  select p).FirstOrDefault();

                        if (tb != null)
                        {
                            tb.tbStudyOption = db.Set<Study.Entity.tbStudyOption>().Find(vm.StudyDataEdit.StudyOptionId);
                            tb.Remark = vm.StudyDataEdit.Remark;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改晚自习表现");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }

        public ActionResult Export()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyData.Export();
                var file = System.IO.Path.GetTempFileName();
                var vDateSearch = Convert.ToDateTime(vm.DateSearch);
                var tbStudy = (from p in db.Table<Study.Entity.tbStudy>()
                               where p.Id == vm.StudyId
                               && p.tbYear.IsDeleted == false
                               select p).FirstOrDefault();

                if (tbStudy != null)
                {
                    var tbStudyDataList = from p in db.Table<Study.Entity.tbStudyData>()
                                          where p.InputDate.Day == vDateSearch.Day && p.tbStudy.Id == vm.StudyId
                                          && p.tbStudent.IsDeleted == false
                                           && p.tbStudyOption.IsDeleted == false
                                           && p.tbSysUser.IsDeleted == false
                                          select p;
                    //教室模式
                    if (tbStudy.IsRoom)
                    {
                        var tb = from p in db.Table<Study.Entity.tbStudyRoomStudent>()
                                 where p.tbStudy.Id == vm.StudyId && p.tbRoom.Id == vm.RoomOrClassId
                                 && p.tbStudy.IsDeleted == false
                                 && p.tbRoom.IsDeleted == false
                                 && p.tbStudent.IsDeleted == false
                                 select p;

                        if (string.IsNullOrEmpty(vm.SearchText) == false)
                        {
                            tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                        }

                        vm.ExportList = (from p in tb
                                         orderby p.tbStudent.StudentCode
                                         select new Dto.StudyData.Export
                                         {
                                             Id = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.Id).FirstOrDefault(),
                                             InputDate = vDateSearch,
                                             RoomName = p.tbRoom.RoomName,
                                             StudentCode = p.tbStudent.StudentCode,
                                             StudentName = p.tbStudent.StudentName,
                                             SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                             StudentId = p.tbStudent.Id,
                                             StudyId = vm.StudyId,
                                             StudyOptionName = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.tbStudyOption.StudyOptionName).FirstOrDefault(),
                                             Remark = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.Remark).FirstOrDefault(),
                                             SysUserName = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.tbSysUser.UserName).FirstOrDefault()
                                         }).ToList();
                    }
                    else//班级模式
                    {
                        var tb = from p in db.Table<Study.Entity.tbStudyClassStudent>()
                                 where p.tbStudy.Id == vm.StudyId && p.tbClass.Id == vm.RoomOrClassId
                                 && p.tbStudy.IsDeleted == false
                                 && p.tbClass.IsDeleted == false
                                 && p.tbStudent.IsDeleted == false
                                 select p;

                        if (string.IsNullOrEmpty(vm.SearchText) == false)
                        {
                            tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                        }

                        vm.ExportList = (from p in tb
                                         orderby p.tbStudent.StudentCode
                                         select new Dto.StudyData.Export
                                         {
                                             Id = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.Id).FirstOrDefault(),
                                             InputDate = vDateSearch,
                                             RoomName = p.tbClass.tbRoom.RoomName,
                                             StudentCode = p.tbStudent.StudentCode,
                                             StudentName = p.tbStudent.StudentName,
                                             SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                             StudentId = p.tbStudent.Id,
                                             StudyId = vm.StudyId,
                                             StudyOptionName = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.tbStudyOption.StudyOptionName).FirstOrDefault(),
                                             Remark = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.Remark).FirstOrDefault(),
                                             SysUserName = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.tbSysUser.UserName).FirstOrDefault()
                                         }).ToList();
                    }
                }

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("序号"),
                        new System.Data.DataColumn("自习时间"),
                        new System.Data.DataColumn("教室"),
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("表现"),
                        new System.Data.DataColumn("备注"),
                        new System.Data.DataColumn("录入人员")
                    });

                var index = 0;
                foreach (var a in vm.ExportList)
                {
                    index++;
                    var dr = dt.NewRow();
                    dr["序号"] = index.ToString();
                    dr["自习时间"] = a.InputDate.ToString(XkSystem.Code.Common.StringToDate);
                    dr["教室"] = a.RoomName;
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["表现"] = a.StudyOptionName;
                    dr["备注"] = a.Remark;
                    dr["录入人员"] = a.SysUserName;
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
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyData.Import();

                vm.StudyList = Controllers.StudyController.SelectList();
                if (string.IsNullOrEmpty(vm.DateSearch))
                {
                    vm.DateSearch = DateTime.Now.AddDays(0).ToString("yyyy-MM-dd");
                }
                if (vm.StudyId == 0 && vm.StudyList.Count > 0)
                {
                    vm.StudyId = vm.StudyList.FirstOrDefault().Value.ConvertToInt();
                }

                var tbStudy = (from p in db.Table<Study.Entity.tbStudy>()
                               where p.Id == vm.StudyId
                               && p.tbYear.IsDeleted == false
                               select p).FirstOrDefault();

                if (tbStudy != null)
                {
                    //教室模式
                    if (tbStudy.IsRoom)
                    {
                        vm.RoomOrClassList = Controllers.StudyRoomController.SelectList(vm.StudyId);
                    }
                    else//班级模式
                    {
                        vm.RoomOrClassList = Controllers.StudyClassController.SelectList(vm.StudyId);
                    }
                }
                return View(vm);
            }
        }

        public ActionResult ImportTemplate(int studyId, int roomOrClassId, string dateSearch)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var vDateSearch = Convert.ToDateTime(dateSearch);

                var tbStudy = (from p in db.Table<Study.Entity.tbStudy>()
                               where p.Id == studyId
                               && p.tbYear.IsDeleted == false
                               select p).FirstOrDefault();

                var ExportList = new List<Dto.StudyData.Export>();
                if (tbStudy != null)
                {
                    var tbStudyDataList = from p in db.Table<Study.Entity.tbStudyData>()
                                          where p.InputDate.Day == vDateSearch.Day && p.tbStudy.Id == studyId
                                          && p.tbStudent.IsDeleted == false
                                           && p.tbStudyOption.IsDeleted == false
                                           && p.tbSysUser.IsDeleted == false
                                          select p;
                    //教室模式
                    if (tbStudy.IsRoom)
                    {
                        var tb = from p in db.Table<Study.Entity.tbStudyRoomStudent>()
                                 where p.tbStudy.Id == studyId && p.tbRoom.Id == roomOrClassId
                                 && p.tbStudy.IsDeleted == false
                                 && p.tbRoom.IsDeleted == false
                                 && p.tbStudent.IsDeleted == false
                                 select p;


                        ExportList = (from p in tb
                                      orderby p.tbStudent.StudentCode
                                      select new Dto.StudyData.Export
                                      {
                                          Id = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.Id).FirstOrDefault(),
                                          InputDate = vDateSearch,
                                          StudyName = p.tbStudy.StudyName,
                                          RoomName = p.tbRoom.RoomName,
                                          StudentCode = p.tbStudent.StudentCode,
                                          StudentName = p.tbStudent.StudentName,
                                          SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                          StudentId = p.tbStudent.Id,
                                          StudyId = studyId,
                                          StudyOptionName = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.tbStudyOption.StudyOptionName).FirstOrDefault(),
                                          Remark = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.Remark).FirstOrDefault(),
                                          SysUserName = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.tbSysUser.UserName).FirstOrDefault()
                                      }).ToList();
                    }
                    else//班级模式
                    {
                        var tb = from p in db.Table<Study.Entity.tbStudyClassStudent>()
                                 where p.tbStudy.Id == studyId && p.tbClass.Id == roomOrClassId
                                 && p.tbStudy.IsDeleted == false
                                 && p.tbClass.IsDeleted == false
                                 && p.tbStudent.IsDeleted == false
                                 select p;

                        ExportList = (from p in tb
                                      orderby p.tbStudent.StudentCode
                                      select new Dto.StudyData.Export
                                      {
                                          Id = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.Id).FirstOrDefault(),
                                          InputDate = vDateSearch,
                                          StudyName = p.tbStudy.StudyName,
                                          RoomName = p.tbClass.tbRoom.RoomName,
                                          StudentCode = p.tbStudent.StudentCode,
                                          StudentName = p.tbStudent.StudentName,
                                          SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                          StudentId = p.tbStudent.Id,
                                          StudyId = studyId,
                                          StudyOptionName = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.tbStudyOption.StudyOptionName).FirstOrDefault(),
                                          Remark = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.Remark).FirstOrDefault(),
                                          SysUserName = tbStudyDataList.Where(d => d.InputDate.Day == vDateSearch.Day && d.tbStudent.Id == p.tbStudent.Id).Select(d => d.tbSysUser.UserName).FirstOrDefault()
                                      }).ToList();
                    }
                }

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("晚自习名称"),
                        new System.Data.DataColumn("自习日期"),
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("表现名称"),
                        new System.Data.DataColumn("备注")
                    });

                foreach (var a in ExportList)
                {
                    var dr = dt.NewRow();
                    dr["晚自习名称"] = a.StudyName;
                    dr["自习日期"] = a.InputDate.ToString(XkSystem.Code.Common.StringToDate);
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["表现名称"] = a.StudyOptionName;
                    dr["备注"] = a.Remark;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.StudyData.Import vm)
        {
            if (ModelState.IsValid)
            {
                vm.StudyList = Controllers.StudyController.SelectList();
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);

                using (var db = new XkSystem.Models.DbContext())
                {
                    #region 0、Excel初始数据
                    if (string.IsNullOrEmpty(vm.DateSearch))
                    {
                        vm.DateSearch = DateTime.Now.AddDays(0).ToString("yyyy-MM-dd");
                    }
                    if (vm.StudyId == 0 && vm.StudyList.Count > 0)
                    {
                        vm.StudyId = vm.StudyList.FirstOrDefault().Value.ConvertToInt();
                    }

                    var tbStudy = (from p in db.Table<Study.Entity.tbStudy>()
                                   where p.Id == vm.StudyId
                                   && p.tbYear.IsDeleted == false
                                   select p).FirstOrDefault();

                    if (tbStudy != null)
                    {
                        //教室模式
                        if (tbStudy.IsRoom)
                        {
                            vm.RoomOrClassList = Controllers.StudyRoomController.SelectList(vm.StudyId);
                        }
                        else//班级模式
                        {
                            vm.RoomOrClassList = Controllers.StudyClassController.SelectList(vm.StudyId);
                        }
                    }
                    #endregion

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
                    var tbList = new List<string>() { "晚自习名称", "自习日期", "学号", "姓名", "表现名称", "备注" };

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
                        var dtoStudy = new Dto.StudyData.Import()
                        {
                            StudyName = Convert.ToString(dr["晚自习名称"]),
                            InputDate = Convert.ToString(dr["自习日期"]),
                            StudentCode = Convert.ToString(dr["学号"]),
                            StudentName = Convert.ToString(dr["姓名"]),
                            StudyOptionName = Convert.ToString(dr["表现名称"]),
                            Remark = Convert.ToString(dr["备注"])
                        };

                        if (vm.ImportList.Where(d => d.StudyName == dtoStudy.StudyName
                                                && d.InputDate == dtoStudy.InputDate
                                                && d.StudentCode == dtoStudy.StudentCode
                                                && d.StudentName == dtoStudy.StudentName
                                                && d.StudyOptionName == dtoStudy.StudyOptionName
                                                && d.Remark == dtoStudy.Remark).Count() == 0)
                        {
                            vm.ImportList.Add(dtoStudy);
                        }
                    }
                    vm.ImportList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.StudyName) &&
                        string.IsNullOrEmpty(d.InputDate) &&
                        string.IsNullOrEmpty(d.StudentCode) &&
                        string.IsNullOrEmpty(d.StudentName) &&
                        string.IsNullOrEmpty(d.StudyOptionName) &&
                        string.IsNullOrEmpty(d.Remark)
                    );
                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "Excel未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    #region 3、Excel格式校验
                    var vDateSearch = Convert.ToDateTime(vm.DateSearch);

                    var tbStudyDataList = (from p in db.Table<Study.Entity.tbStudyData>()
                                           .Include(d => d.tbStudent)
                                           .Include(d => d.tbStudy)
                                           .Include(d => d.tbStudyOption)
                                           where p.InputDate.Day == vDateSearch.Day && p.tbStudy.Id == vm.StudyId
                                           && p.tbStudent.IsDeleted == false
                                           && p.tbStudyOption.IsDeleted == false
                                           && p.tbSysUser.IsDeleted == false
                                           select p).ToList();

                    var StudentList = new List<Dto.StudyData.Import>();
                    if (tbStudy != null)
                    {
                        //教室模式
                        if (tbStudy.IsRoom)
                        {
                            StudentList = (from p in db.Table<Study.Entity.tbStudyRoomStudent>()
                                           where p.tbStudy.Id == vm.StudyId && p.tbRoom.Id == vm.RoomOrClassId
                                           && p.tbStudy.IsDeleted == false
                                           && p.tbRoom.IsDeleted == false
                                           && p.tbStudent.IsDeleted == false
                                           select new Dto.StudyData.Import
                                           {
                                               StudentCode = p.tbStudent.StudentCode,
                                               StudentName = p.tbStudent.StudentName
                                           }).ToList();
                        }
                        else//班级模式
                        {
                            StudentList = (from p in db.Table<Study.Entity.tbStudyClassStudent>()
                                           where p.tbStudy.Id == vm.StudyId && p.tbClass.Id == vm.RoomOrClassId
                                           && p.tbStudy.IsDeleted == false
                                           && p.tbClass.IsDeleted == false
                                           && p.tbStudent.IsDeleted == false
                                           select new Dto.StudyData.Import
                                           {
                                               StudentCode = p.tbStudent.StudentCode,
                                               StudentName = p.tbStudent.StudentName
                                           }).ToList();
                        }
                    }

                    //表现列表
                    var StudyOptionList = (from p in db.Table<Study.Entity.tbStudyOption>() select p).ToList();

                    foreach (var item in vm.ImportList)
                    {
                        if (string.IsNullOrEmpty(item.StudyName))
                        {
                            item.Error = item.Error + "晚自习名称不能为空!";
                            continue;
                        }
                        if (string.IsNullOrEmpty(item.InputDate))
                        {
                            item.Error = item.Error + "自习日期不能为空!";
                            continue;
                        }
                        else
                        {
                            DateTime timeTemp = new DateTime();
                            if (!DateTime.TryParse(item.InputDate, out timeTemp))
                            {
                                item.Error += "【自习日期】格式不正确，请输入正确的时间格式如：" + DateTime.Now.ToString(XkSystem.Code.Common.StringToDateTime);
                                continue;
                            }
                        }
                        if (string.IsNullOrEmpty(item.StudentCode))
                        {
                            item.Error = item.Error + "学号不能为空!";
                            continue;
                        }
                        else
                        {
                            if (StudentList.Where(d => d.StudentCode == item.StudentCode).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "学号不存在数据库!";
                                continue;
                            }
                        }
                        if (string.IsNullOrEmpty(item.StudentName))
                        {
                            item.Error = item.Error + "姓名不能为空!";
                            continue;
                        }
                        else
                        {
                            if (StudentList.Where(d => d.StudentName == item.StudentName).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "姓名不存在数据库!";
                                continue;
                            }
                        }
                        if (string.IsNullOrEmpty(item.StudyOptionName))
                        {
                            item.Error = item.Error + "表现名称不能为空!";
                            continue;
                        }
                        else
                        {
                            if (StudyOptionList.Where(d => d.StudyOptionName == item.StudyOptionName).Count() == decimal.Zero)
                            {
                                item.Error = item.Error + "表现名称不存在数据库!";
                                continue;
                            }
                        }
                        if (vm.IsUpdate)
                        {
                            if (tbStudyDataList.Where(d => d.tbStudy.Id == vm.StudyId && d.InputDate.Day == vDateSearch.Day && d.tbStudent.StudentCode == item.StudentCode).Count() > 1)
                            {
                                item.Error += "系统中该晚自习数据存在重复，无法确认需要更新的记录!";
                                continue;
                            }
                        }
                        else
                        {
                            if (tbStudyDataList.Where(d => d.tbStudy.Id == vm.StudyId && d.InputDate.Day == vDateSearch.Day && d.tbStudent.StudentCode == item.StudentCode).Count() > 0)
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
                    var addStudyList = new List<Study.Entity.tbStudyData>();
                    foreach (var item in vm.ImportList)
                    {
                        Study.Entity.tbStudyData tb = null;
                        if (tbStudyDataList.Where(d => d.tbStudy.Id == vm.StudyId && d.InputDate.Day == vDateSearch.Day && d.tbStudent.StudentCode == item.StudentCode).Count() > 0)
                        {
                            if (vm.IsUpdate)
                            {
                                tb = tbStudyDataList.Where(d => d.tbStudy.Id == vm.StudyId && d.InputDate.Day == vDateSearch.Day && d.tbStudent.StudentCode == item.StudentCode).FirstOrDefault();
                                tb.Remark = item.Remark;
                                tb.tbStudyOption = db.Table<Study.Entity.tbStudyOption>().Where(d => d.StudyOptionName == item.StudyOptionName).FirstOrDefault();
                            }
                        }
                        else
                        {
                            tb = new Study.Entity.tbStudyData();
                            tb.tbStudy = db.Set<Study.Entity.tbStudy>().Find(vm.StudyId);
                            tb.tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == item.StudentCode).FirstOrDefault();
                            tb.InputDate = vDateSearch;
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            tb.Remark = item.Remark;
                            tb.tbStudyOption = db.Table<Study.Entity.tbStudyOption>().Where(d => d.StudyOptionName == item.StudyOptionName).FirstOrDefault();
                            addStudyList.Add(tb);
                        }
                    }
                    db.Set<Study.Entity.tbStudyData>().AddRange(addStudyList);
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量导入了晚自习表现");
                        vm.Status = true;
                    }
                    #endregion
                }
            }
            vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
            return View(vm);
        }

        public ActionResult GetRoomOrClassByStudyId(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tbStudy = (from p in db.Table<Study.Entity.tbStudy>()
                               where p.Id == id
                               && p.tbYear.IsDeleted == false
                               select p).FirstOrDefault();

                var RoomOrClassList = new List<System.Web.Mvc.SelectListItem>();
                if (tbStudy != null)
                {
                    //教室模式
                    if (tbStudy.IsRoom)
                    {
                        RoomOrClassList = Controllers.StudyRoomController.SelectList(id);
                    }
                    else//班级模式
                    {
                        RoomOrClassList = Controllers.StudyClassController.SelectList(id);
                    }
                }
                return Json(RoomOrClassList, JsonRequestBehavior.AllowGet);
            }
        }
    }
}