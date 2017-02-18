using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Course.Controllers
{
    public class CourseController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Course.List();
                vm.SubjectList = SubjectController.SelectList();
                vm.CourseTypeList = CourseTypeController.SelectList();
                vm.CourseDomainList = CourseDomainController.SelectList();
                vm.CourseGroupList = CourseGroupController.SelectList();

                var tb = from p in db.Table<Course.Entity.tbCourse>()
                         select p;
                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.CourseName.Contains(vm.SearchText));
                }

                if (vm.SubjectId != 0)
                {
                    tb = tb.Where(d => d.tbSubject.Id == vm.SubjectId);
                }

                if (vm.CourseTypeId != 0)
                {
                    tb = tb.Where(d => d.tbCourseType.Id == vm.CourseTypeId);
                }

                if (vm.CourseDomainId != 0)
                {
                    tb = tb.Where(d => d.tbCourseDomain.Id == vm.CourseDomainId);
                }

                if (vm.CourseGroupId != 0)
                {
                    tb = tb.Where(d => d.tbCourseGroup.Id == vm.CourseGroupId);
                }

                vm.CourseSubjectList = (from p in tb
                                        orderby p.tbSubject.No
                                        select new Dto.Subject.Info
                                        {
                                            Id = p.tbSubject.Id,
                                            SubjectName = p.tbSubject.SubjectName
                                        }).Distinct().ToList();

                vm.CourseList = (from p in tb
                                 orderby
                                    p.tbCourseDomain.No, p.tbCourseDomain.CourseDomainName,
                                    p.tbSubject.No, p.tbSubject.SubjectName, p.CourseCode, p.CourseName
                                 select new Dto.Course.List
                                 {
                                     Id = p.Id,
                                     CourseName = p.CourseName,
                                     CourseNameEn = p.CourseNameEn,
                                     CourseTypeName = p.tbCourseType.CourseTypeName,
                                     SubjectName = p.tbSubject.SubjectName,
                                     Point = p.Point,
                                     Hour = p.Hour,
                                     CourseCode = p.CourseCode,
                                     CourseDomainName = p.tbCourseDomain.CourseDomainName,
                                     CourseGroupName = p.tbCourseGroup.CourseGroupName,
                                     IsLevel=p.IsLevel
                                 }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Course.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                showModel = vm.ShowModel,
                searchText = vm.SearchText,
                courseTypeId = vm.CourseTypeId,
                subjectId = vm.SubjectId,
                CourseDomainTypeId = vm.CourseDomainTypeId,
                CourseDomainId = vm.CourseDomainId,
                CourseGroupId=vm.CourseGroupId
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbCourse>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了课程");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Course.Edit();
                vm.SubjectList = Course.Controllers.SubjectController.SelectList();
                vm.CourseTypeList = Course.Controllers.CourseTypeController.SelectList();
                vm.CourseDomainList = Course.Controllers.CourseDomainController.SelectList();
                vm.CourseGroupList = Course.Controllers.CourseGroupController.SelectList();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Course.Entity.tbCourse>()
                              where p.Id == id
                              select new Dto.Course.Edit
                              {
                                  Id = p.Id,
                                  CourseName = p.CourseName,
                                  CourseNameEn = p.CourseNameEn,
                                  CourseTypeId = p.tbCourseType.Id,
                                  SubjectId = p.tbSubject.Id,
                                  Point = p.Point,
                                  Hour = p.Hour,
                                  CourseCode = p.CourseCode,
                                  Remark = p.Remark,
                                  CourseDomainId = p.tbCourseDomain.Id,
                                  CourseGroupId = p.tbCourseGroup.Id,
                                  IsLevel=p.IsLevel
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.CourseEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Models.Course.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == 0)
                {
                    if (db.Table<Course.Entity.tbCourse>().Where(d => d.CourseName == vm.CourseEdit.CourseName && d.Id != vm.CourseEdit.Id).Any())
                    {
                        error.AddError("该课程已存在!");
                    }
                    else
                    {
                        if (vm.CourseEdit.Id == 0)
                        {
                            var tb = new Course.Entity.tbCourse();
                            tb.tbSubject = db.Set<Course.Entity.tbSubject>().Find(vm.CourseEdit.SubjectId);
                            tb.tbCourseType = db.Set<Course.Entity.tbCourseType>().Find(vm.CourseEdit.CourseTypeId);
                            tb.CourseName = vm.CourseEdit.CourseName;
                            tb.CourseCode = vm.CourseEdit.CourseCode;
                            tb.CourseNameEn = vm.CourseEdit.CourseNameEn;
                            tb.tbCourseDomain = db.Set<Course.Entity.tbCourseDomain>().Find(vm.CourseEdit.CourseDomainId);
                            tb.tbCourseGroup = db.Set<Course.Entity.tbCourseGroup>().Find(vm.CourseEdit.CourseGroupId);
                            tb.Point = vm.CourseEdit.Point;
                            tb.Hour = vm.CourseEdit.Hour;
                            tb.Remark = vm.CourseEdit.Remark;
                            tb.IsLevel = vm.CourseEdit.IsLevel;
                            db.Set<Course.Entity.tbCourse>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了课程");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Course.Entity.tbCourse>()
                                        .Include(d => d.tbCourseDomain)
                                      where p.Id == vm.CourseEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.CourseName = vm.CourseEdit.CourseName;
                                tb.CourseCode = vm.CourseEdit.CourseCode;
                                tb.CourseNameEn = vm.CourseEdit.CourseNameEn;
                                tb.tbSubject = db.Set<Course.Entity.tbSubject>().Find(vm.CourseEdit.SubjectId);
                                tb.tbCourseType = db.Set<Course.Entity.tbCourseType>().Find(vm.CourseEdit.CourseTypeId);
                                tb.tbCourseDomain = db.Set<Course.Entity.tbCourseDomain>().Find(vm.CourseEdit.CourseDomainId);
                                tb.tbCourseGroup = db.Set<Course.Entity.tbCourseGroup>().Find(vm.CourseEdit.CourseGroupId);
                                tb.Point = vm.CourseEdit.Point;
                                tb.Hour = vm.CourseEdit.Hour;
                                tb.Remark = vm.CourseEdit.Remark;
                                tb.IsLevel = vm.CourseEdit.IsLevel;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了课程");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetLevel(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Course.Entity.tbCourse>().Find(id);
                if (tb != null)
                {
                    tb.IsLevel = !tb.IsLevel;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }
        public ActionResult Import()
        {
            var vm = new Models.Course.Import();
            vm.ImportList = new List<Dto.Course.Import>();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.Course.Import vm)
        {
            vm.ImportList = new List<Dto.Course.Import>();
            if (ModelState.IsValid)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);

                using (var db = new XkSystem.Models.DbContext())
                {
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
                    var tbList = new List<string>() { "课程名称", "课程编码", "课程领域", "课程分组", "科目名称", "英文名", "课程类型", "学分", "课时", "课程说明" };

                    var Text = string.Empty;
                    foreach (var a in tbList)
                    {
                        if (!dt.Columns.Contains(a))
                        {
                            Text += a + ",";
                        }
                    }

                    if (!string.IsNullOrEmpty(Text))
                    {
                        ModelState.AddModelError("", "上传的EXCEL内容与预期不一致，缺少对应的字段：" + Text);
                        return View(vm);
                    }

                    #region 将DataTable转为List
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dto = new Dto.Course.Import()
                        {
                            CourseName = dr["课程名称"].ToString().Trim(),
                            CourseCode = dr["课程编码"].ConvertToString(),
                            CourseDomainName = Convert.ToString(dr["课程领域"]),
                            CourseGroupName = Convert.ToString(dr["课程分组"]),
                            CourseTypeName = dr["课程类型"].ToString().Trim(),
                            SubjectName = dr["科目名称"].ToString().Trim(),
                            CourseNameEn = Convert.ToString(dr["英文名"]),
                            Hour = Convert.ToString(dr["课时"]),
                            Point = Convert.ToString(dr["学分"]),
                            Remark = Convert.ToString(dr["课程说明"]),
                        };

                        vm.ImportList.Add(dto);
                    }

                    vm.ImportList.RemoveAll(d =>
                        string.IsNullOrEmpty(d.CourseName) &&
                        string.IsNullOrEmpty(d.CourseCode) &&
                        string.IsNullOrEmpty(d.CourseTypeName) &&
                        string.IsNullOrEmpty(d.CourseGroupName) &&
                        string.IsNullOrEmpty(d.CourseDomainName) &&
                        string.IsNullOrEmpty(d.SubjectName) &&
                        string.IsNullOrEmpty(d.CourseNameEn) &&
                        string.IsNullOrEmpty(d.Hour) &&
                        string.IsNullOrEmpty(d.Point) &&
                        string.IsNullOrEmpty(d.Remark)
                    );

                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    var subjectList = db.Table<Course.Entity.tbSubject>().ToList();
                    var courseTypeList = db.Table<Course.Entity.tbCourseType>().ToList();
                    var courseList = (from p in db.Table<Course.Entity.tbCourse>()
                                        .Include(d => d.tbSubject)
                                        .Include(d => d.tbCourseType)
                                      select p).ToList();
                    var courseDomainList = db.Table<Course.Entity.tbCourseDomain>().ToList();
                    var courseGroupList = db.Table<Course.Entity.tbCourseGroup>().ToList();

                    #region 验证数据格式是否正确
                    foreach (var item in vm.ImportList)
                    {
                        if (string.IsNullOrEmpty(item.CourseName))
                        {
                            item.Error = item.Error + "课程名称不能为空!";
                        }

                        if (string.IsNullOrEmpty(item.CourseTypeName))
                        {
                            item.Error = item.Error + "课程类型不能为空!";
                        }
                        else
                        {
                            if (courseTypeList.Where(d => d.CourseTypeName == item.CourseTypeName).Count() <= 0)
                            {
                                item.Error = item.Error + "课程类型不存在;";
                            }
                        }

                        if (string.IsNullOrEmpty(item.SubjectName))
                        {
                            item.Error = item.Error + "科目名称不能为空!";
                        }
                        else
                        {
                            if (subjectList.Where(d => d.SubjectName == item.SubjectName).Count() <= 0)
                            {
                                item.Error = item.Error + "科目不存在;";
                            }
                        }

                        if (string.IsNullOrEmpty(item.CourseDomainName) == false)
                        {
                            if (courseDomainList.Where(d => d.CourseDomainName == item.CourseDomainName).Any() == false)
                            {
                                item.Error = item.Error + "课程领域不存在;";
                            }
                        }

                        if (string.IsNullOrEmpty(item.CourseGroupName) == false)
                        {
                            if (courseGroupList.Where(d => d.CourseGroupName == item.CourseGroupName).Any() == false)
                            {
                                item.Error = item.Error + "课程分组不存在;";
                            }
                        }

                        if (vm.ImportList.Where(d => d.CourseName == item.CourseName).Count() > 1)
                        {
                            item.Error = item.Error + "该条数据重复!";
                        }

                        int Hour = 0;
                        if ((int.TryParse(item.Hour, out Hour) == false || Hour < 0))
                        {
                            item.Error = item.Error + "课时必须为正整数!";
                        }

                        decimal Point = 0;
                        if ((decimal.TryParse(item.Point, out Point) == false || Point < 0))
                        {
                            item.Error = item.Error + "学分必须为数字!";
                        }

                        if (vm.IsUpdate == false && courseList.Where(d => d.CourseName == item.CourseName).Count() > 0)
                        {
                            item.Error = item.Error + "系统中已存在该课程!";
                        }
                    }

                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    foreach (var item in vm.ImportList)
                    {
                        #region 数据导入，判断关键字，存在的数据做更新，不存在的做新增
                        if (vm.IsUpdate && courseList.Where(d => d.CourseName == item.CourseName).Count() > 0)
                        {
                            if (courseList.Where(d => d.CourseName == item.CourseName).Count() > 1)
                            {
                                item.Error = item.Error + "系统中该数据存在重复，无法确认需要更新的记录!";
                            }
                            else
                            {
                                var tb = courseList.Where(d => d.CourseName == item.CourseName).FirstOrDefault();
                                tb.CourseName = item.CourseName;
                                tb.CourseCode = item.CourseCode;
                                tb.CourseNameEn = item.CourseNameEn;
                                tb.tbSubject = subjectList.Where(d => d.SubjectName == item.SubjectName).FirstOrDefault();
                                tb.tbCourseType = courseTypeList.Where(d => d.CourseTypeName == item.CourseTypeName).FirstOrDefault();
                                tb.Point = item.Point.ConvertToDecimal();
                                tb.Hour = item.Hour.ConvertToInt();
                                tb.Remark = item.Remark;
                                tb.IsLevel = false;
                                if (string.IsNullOrEmpty(item.CourseDomainName) == false)
                                {
                                    tb.tbCourseDomain = courseDomainList.Where(d => d.CourseDomainName == item.CourseDomainName).FirstOrDefault();
                                }

                                if (string.IsNullOrEmpty(item.CourseGroupName) == false)
                                {
                                    tb.tbCourseGroup = courseGroupList.Where(d => d.CourseGroupName == item.CourseGroupName).FirstOrDefault();
                                }
                            }
                        }
                        else
                        {
                            var tb = new Course.Entity.tbCourse();
                            tb.CourseName = item.CourseName;
                            tb.CourseCode = item.CourseCode;
                            tb.CourseNameEn = item.CourseNameEn;
                            tb.tbSubject = subjectList.Where(d => d.SubjectName == item.SubjectName).FirstOrDefault();
                            tb.tbCourseType = courseTypeList.Where(d => d.CourseTypeName == item.CourseTypeName).FirstOrDefault();
                            tb.Point = item.Point.ConvertToDecimal();
                            tb.Hour = item.Hour.ConvertToInt();
                            tb.Remark = item.Remark;
                            tb.IsLevel = false;
                            if (string.IsNullOrEmpty(item.CourseDomainName) == false)
                            {
                                tb.tbCourseDomain = courseDomainList.Where(d => d.CourseDomainName == item.CourseDomainName).FirstOrDefault();
                            }

                            if (string.IsNullOrEmpty(item.CourseGroupName) == false)
                            {
                                tb.tbCourseGroup = courseGroupList.Where(d => d.CourseGroupName == item.CourseGroupName).FirstOrDefault();
                            }

                            db.Set<Course.Entity.tbCourse>().Add(tb);
                        }
                        #endregion
                    }

                    if (db.SaveChanges() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量添加了课程");
                        vm.Status = true;
                    }
                }
            }
            return View(vm);
        }


   

        public ActionResult Export()
        {
            var vm = new Models.Course.Export();
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var tb = db.Table<Course.Entity.tbCourse>();
                if (vm.CourseTypeId != 0)
                {
                    tb = tb.Where(d => d.tbCourseType.Id == vm.CourseTypeId);
                }
                if (vm.SubjectId != 0)
                {
                    tb = tb.Where(d => d.tbSubject.Id == vm.SubjectId);
                }

                vm.ExportList = (from p in tb
                                 select new Dto.Course.Export
                                 {
                                     CourseCode = p.CourseCode,
                                     CourseName = p.CourseName,
                                     CourseNameEn = p.CourseNameEn,
                                     SubjectName = p.tbSubject.SubjectName,
                                     CourseTypeName = p.tbCourseType.CourseTypeName,
                                     Point = p.Point,
                                     Hour = p.Hour,
                                     CourseDomainName = p.tbCourseDomain.CourseDomainName,
                                     CourseGroupName = p.tbCourseGroup.CourseGroupName
                                 }).ToList();
                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("课程名称"),
                        new System.Data.DataColumn("课程编码"),
                        new System.Data.DataColumn("科目"),
                        new System.Data.DataColumn("课程领域"),
                        new System.Data.DataColumn("课程分组"),
                        new System.Data.DataColumn("英文名"),
                        new System.Data.DataColumn("课程类型"),
                        new System.Data.DataColumn("学分"),
                        new System.Data.DataColumn("课时")
                    });
                foreach (var a in vm.ExportList)
                {
                    var dr = dt.NewRow();
                    dr["课程名称"] = a.CourseName;
                    dr["课程编码"] = a.CourseCode;
                    dr["科目"] = a.SubjectName;
                    dr["课程领域"] = a.CourseDomainName;
                    dr["课程分组"] = a.CourseGroupName;
                    dr["英文名"] = a.CourseNameEn;
                    dr["课程类型"] = a.CourseTypeName;
                    dr["学分"] = a.Point;
                    dr["课时"] = a.Hour;
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

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Course/Views/Course/CourseTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        public ActionResult Info(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {

                var vm = new Models.Course.Info();
                vm.CourseInfo = (from p in db.Table<Course.Entity.tbCourse>()
                                 where p.Id == id
                                 select new Dto.Course.Info
                                 {
                                     Id = p.Id,
                                     CourseName = p.CourseName,
                                     CourseNameEn = p.CourseNameEn,
                                     CourseTypeName = p.tbCourseType.CourseTypeName,
                                     SubjectName = p.tbSubject.SubjectName,
                                     Point = p.Point,
                                     CourseDomainName = p.tbCourseDomain.CourseDomainName,
                                     CourseGroupName = p.tbCourseGroup.CourseGroupName,
                                     CourseCode = p.CourseCode,
                                     Hour = p.Hour,
                                     Remark = p.Remark
                                 }).FirstOrDefault();
                return View(vm);
            }
        }

        public ActionResult Remark(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var remark = "";
                var tb = db.Set<Course.Entity.tbCourse>().Find(id);
                if (tb != null)
                {
                    remark = Code.StringHelper.ToHtml(tb.Remark);
                }

                return Json(new { Remark = remark }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 课程查看列表
        /// </summary>
        public ActionResult InfoList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Course.InfoList();
                
                var tb= (from p in db.Table<Entity.tbCourse>() select p);

                if (vm.TypeId > 0)
                {
                    tb = tb.Where(p => p.tbCourseType.Id == vm.TypeId);
                }

                if (vm.GroupId > 0)
                {
                    tb = tb.Where(p => p.tbCourseGroup.Id == vm.GroupId);
                }
                if (vm.DomainId > 0)
                {
                    tb = tb.Where(p => p.tbCourseDomain.Id == vm.DomainId);
                }
                if (vm.SubjectId > 0)
                {
                    tb = tb.Where(p => p.tbSubject.Id == vm.SubjectId);
                }

                vm.CourseInfoList = (from p in tb
                                     orderby p.No
                                     select new Dto.Course.InfoList()
                                     {
                                         Id = p.Id,
                                         CourseName = p.CourseName,
                                         CourseNameEn = p.CourseNameEn,
                                         CourseTypeName = p.tbCourseType.CourseTypeName,
                                         SubjectName = p.tbSubject.SubjectName,
                                         Point = p.Point,
                                         Hour = p.Hour,
                                         CourseCode = p.CourseCode,
                                         CourseDomainName = p.tbCourseDomain != null ? p.tbCourseDomain.CourseDomainName : string.Empty,
                                         CourseGroupName = p.tbCourseGroup != null ? p.tbCourseGroup.CourseGroupName : string.Empty
                                     }).ToList();
             
                vm.CourseSubjectList = SubjectController.SelectList();
                vm.CourseDomainList = CourseDomainController.SelectList();
                vm.CourseGroupList = CourseGroupController.SelectList();
                vm.CourseTypeList = CourseTypeController.SelectList();

                return View(vm);
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InfoList(Models.Course.InfoList vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("InfoList", new
            {
                TypeId=vm.TypeId,
                SubjectId=vm.SubjectId,
                GroupId=vm.GroupId,
                DomainId=vm.DomainId
            }));
        }


        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int subjectId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbCourse>()
                          where (p.tbSubject.Id == subjectId || subjectId == 0)
                          orderby p.tbSubject.No, p.tbSubject.SubjectName, p.CourseCode, p.CourseName
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.CourseName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        internal static List<SelectListItem> SelectListForElectiveOrg(int electiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                return (from p in db.Table<Entity.tbCourse>()
                        join e in db.Table<Elective.Entity.tbElectiveSubject>() on p.tbSubject.Id equals e.tbSubject.Id
                        where e.tbElective.Id == electiveId
                        orderby p.No
                        select new SelectListItem() {
                            Text=p.CourseName,
                            Value=p.Id.ToString()
                        }).ToList();
            }
        }


        [HttpPost]
        public JsonResult GetListBySubjectId(int subjectId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbCourse>()
                          where (p.tbSubject.Id == subjectId || subjectId == 0)
                          orderby p.Id
                          select new 
                          {
                              CourseName=p.CourseName,
                              Id=p.Id
                          }).ToList();
                return Json(tb);
            }
        }


        [HttpPost]
        public JsonResult GetCourseInfo(int courseId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Entity.tbCourse>().Find(courseId);
                return Json(new { Hour = tb.Hour, Point = tb.Point });
            }
        }

    }
}