using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Teacher.Controllers
{
    public class TeacherDeptController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.TeacherDept.List();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.TeacherDept.List vm)
        {
            return Code.MvcHelper.Post(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Teacher.Entity.tbTeacherDept>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                //if (db.Table<Entity.tbTeacher>().Where(d => ids.Contains(d.tbTeacherDept.Id)).Count() > 0)
                //{
                //    return Code.MvcHelper.Post(new List<string>() { "系统中有关联的数据，不能直接删除此数据！" });
                //}

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除部门");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.TeacherDept.Edit();
                vm.TeacherDeptParentList = SelectList();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Teacher.Entity.tbTeacherDept>()
                              where p.Id == id
                              select new Dto.TeacherDept.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  TeacherDeptName = p.TeacherDeptName,
                                  TeacherDeptParentId = p.tbTeacherDeptParent.Id,
                                  TeacherDeptParentName = p.tbTeacherDeptParent.TeacherDeptName
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.TeacherDeptEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.TeacherDept.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Teacher.Entity.tbTeacherDept>().Where(d => d.TeacherDeptName == vm.TeacherDeptEdit.TeacherDeptName && d.Id != vm.TeacherDeptEdit.Id).Any())
                    {
                        error.AddError("部门名称已存在!");
                    }
                    else
                    {
                        if (vm.TeacherDeptEdit.Id == 0)
                        {
                            var tb = new Teacher.Entity.tbTeacherDept();
                            tb.No = vm.TeacherDeptEdit.No == null ? db.Table<Teacher.Entity.tbTeacherDept>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.TeacherDeptEdit.No;
                            tb.TeacherDeptName = vm.TeacherDeptEdit.TeacherDeptName;
                            tb.tbTeacherDeptParent = db.Set<Teacher.Entity.tbTeacherDept>().Find(vm.TeacherDeptEdit.TeacherDeptParentId);
                            db.Set<Teacher.Entity.tbTeacherDept>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加部门");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Teacher.Entity.tbTeacherDept>()
                                      where p.Id == vm.TeacherDeptEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                if (tb.Id == vm.TeacherDeptEdit.TeacherDeptParentId)
                                {
                                    error.AddError("不能选择自己作为父节点！");
                                    return Code.MvcHelper.Post(error);
                                }


                                if (vm.TeacherDeptEdit.TeacherDeptParentId.HasValue)
                                {
                                    var sunList = GetSon(db, tb.Id);
                                    if (sunList.ToList().Contains(vm.TeacherDeptEdit.TeacherDeptParentId.Value))
                                    {
                                        error.AddError("不能选择自己的子节点作为父节点！");
                                        return Code.MvcHelper.Post(error);
                                    }
                                    else
                                    {
                                        tb.tbTeacherDeptParent = db.Set<Teacher.Entity.tbTeacherDept>().Find(vm.TeacherDeptEdit.TeacherDeptParentId);

                                    }
                                }
                                else
                                {
                                    //删除父级
                                    db.Entry<Entity.tbTeacherDept>(tb).Reference(p => p.tbTeacherDeptParent).Load();
                                    tb.tbTeacherDeptParent = null;
                                }

                                tb.No = vm.TeacherDeptEdit.No == null ? db.Table<Teacher.Entity.tbTeacherDept>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.TeacherDeptEdit.No;
                                tb.TeacherDeptName = vm.TeacherDeptEdit.TeacherDeptName;


                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改部门");
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


        private IEnumerable<int> GetSon(XkSystem.Models.DbContext db, int id)
        {
            var tb = (from p in db.Table<Entity.tbTeacherDept>() where p.tbTeacherDeptParent.Id == id select p.Id);
            return tb.ToList().Concat(tb.ToList().SelectMany(p => GetSon(db, p)));
        }

        public ActionResult Import()
        {
            var vm = new Models.TeacherDept.Import();
            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Teacher/Views/TeacherDept/TeacherDept.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.TeacherDept.Import vm)
        {
            if (ModelState.IsValid)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    #region 上传文件
                    var file = Request.Files[nameof(vm.UploadFile)];
                    var fileSave = System.IO.Path.GetTempFileName();
                    file.SaveAs(fileSave);

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

                    var tbList = new List<string>() { "部门名称", "上级部门" };

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
                        var dtoTemp = new Dto.TeacherDept.Import()
                        {
                            DeptName = dr["部门名称"].ConvertToString(),
                            ParentDeptName = dr["上级部门"].ConvertToString()
                        };
                        if (vm.ImportList.Where(d => d.DeptName == dtoTemp.DeptName && d.ParentDeptName == dtoTemp.ParentDeptName).Any() == false)
                        {
                            vm.ImportList.Add(dtoTemp);
                        }
                    }

                    vm.ImportList.RemoveAll(d =>
                           string.IsNullOrEmpty(d.DeptName)
                        && string.IsNullOrEmpty(d.ParentDeptName));

                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    #region 验证数据
                    var teacherDeptList = db.Table<Teacher.Entity.tbTeacherDept>().ToList();
                    foreach (var item in vm.ImportList)
                    {
                        if (string.IsNullOrEmpty(item.DeptName))
                        {
                            item.Error += "部门名称不能为空!";
                        }
                        if (!string.IsNullOrWhiteSpace(item.ParentDeptName))
                        {
                            if (vm.IsCover)
                            {
                                if (vm.ImportList.Where(d => d.DeptName == item.ParentDeptName).Any() == false)
                                {
                                    item.Error += "上级部门不存在!";
                                }
                                if (vm.ImportList.Where(d => d.DeptName == item.ParentDeptName).Count() > 1)
                                {
                                    item.Error += "存在多个相同的上级部门!";
                                }
                            }
                            else
                            {
                                if (vm.ImportList.Where(d => d.DeptName == item.ParentDeptName).Any() == false
                                    && teacherDeptList.Where(d => d.TeacherDeptName == item.ParentDeptName).Any() == false)
                                {
                                    item.Error += "上级部门不存在!";
                                }
                                if (vm.ImportList.Where(d => d.DeptName == item.ParentDeptName).Count()
                                    + teacherDeptList.Where(d => d.TeacherDeptName == item.ParentDeptName).Count() > 1)
                                {
                                    item.Error += "存在多个相同的上级部门!";
                                }
                            }
                        }
                    }

                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 保存数据
                    if (vm.IsCover)
                    {
                        foreach (var v in teacherDeptList)
                        {
                            v.IsDeleted = true;
                        }
                    }
                    var tbTeacherDeptList = new List<Teacher.Entity.tbTeacherDept>();
                    foreach (var v in vm.ImportList)
                    {
                        var teacherDept = new Teacher.Entity.tbTeacherDept()
                        {
                            TeacherDeptName = v.DeptName,
                            UpdateTime = DateTime.Now
                        };
                        if (!string.IsNullOrWhiteSpace(v.ParentDeptName))
                        {
                            if (tbTeacherDeptList.Where(d => d.TeacherDeptName == v.ParentDeptName).Any())
                            {
                                teacherDept.tbTeacherDeptParent = tbTeacherDeptList.Where(d => d.TeacherDeptName == v.ParentDeptName).FirstOrDefault();
                            }
                            else
                            {
                                teacherDept.tbTeacherDeptParent = teacherDeptList.Where(d => d.TeacherDeptName == v.ParentDeptName).FirstOrDefault();
                            }
                        }
                        tbTeacherDeptList.Add(teacherDept);
                    }

                    db.Set<Teacher.Entity.tbTeacherDept>().AddRange(tbTeacherDeptList);

                    if (db.SaveChanges() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入教师部门");
                        vm.Status = true;
                    }
                    #endregion
                }
            }

            return View(vm);
        }


        public ActionResult GetDeptTree()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Teacher.Entity.tbTeacherDept>()
                            .Include(d => d.tbTeacherDeptParent)
                          select p).ToList();

                var all = new List<Code.TreeHelper>();
                foreach (var dept in tb.Where(d => d.tbTeacherDeptParent == null).OrderBy(d => d.No).ThenBy(d => d.TeacherDeptName))
                {
                    var cn = DeptDeep(tb, dept.Id);

                    all.Add(new Code.TreeHelper() { name = dept.TeacherDeptName, Id = dept.Id, open = true, children = cn });
                }

                //var treeList = new List<Code.TreeHelper>();
                //var root = new Code.TreeHelper();
                //root.name = "全部";
                //root.Id = 0;
                //root.open = true;
                //root.isChecked = false;
                //root.children = all;
                //treeList.Add(root);

                return Json(all, JsonRequestBehavior.AllowGet);
            }
        }

        private static List<Code.TreeHelper> DeptDeep(List<Teacher.Entity.tbTeacherDept> deptList, int parentId)
        {
            var pn = new List<Code.TreeHelper>();

            foreach (var dept in deptList.Where(d => d.tbTeacherDeptParent != null && d.tbTeacherDeptParent.Id == parentId).OrderBy(d => d.No).ThenBy(d => d.TeacherDeptName))
            {
                var cn = DeptDeep(deptList, dept.Id);

                pn.Add(new Code.TreeHelper() { name = dept.TeacherDeptName, Id = dept.Id, children = cn });
            }

            return pn;
        }

        public static List<System.Web.Mvc.SelectListItem> SelectNormalList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var list = db.Table<Entity.tbTeacherDept>().OrderBy(d => d.tbTeacherDeptParent != null)
                    .Select(d => new System.Web.Mvc.SelectListItem()
                    {
                        Value = d.Id.ToString(),
                        Text = d.TeacherDeptName
                    }).ToList();
                return list;
            }
        }

        public static List<System.Web.Mvc.SelectListItem> SelectList(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var list = new List<System.Web.Mvc.SelectListItem>();

                var deptList = (from p in db.Table<Teacher.Entity.tbTeacherDept>()
                                    .Include(d => d.tbTeacherDeptParent)
                                orderby p.No, p.TeacherDeptName
                                select p).ToList();
                var dept = (from p in deptList
                            where p.tbTeacherDeptParent == null
                            select p).ToList();
                foreach (var a in dept)
                {
                    list.Add(new System.Web.Mvc.SelectListItem() { Text = a.TeacherDeptName, Value = a.Id.ToString() });

                    DeepDept(deptList, ref list, a.Id, 1);
                }

                if (id > 0)
                {
                    list.Where(d => d.Value == id.ConvertToString()).FirstOrDefault().Selected = true;
                }

                return list;
            }
        }

        private static void DeepDept(List<Teacher.Entity.tbTeacherDept> tbDept, ref List<System.Web.Mvc.SelectListItem> DeptList, int parentId, int levelNo)
        {
            if (tbDept.Where(d => d.tbTeacherDeptParent != null && d.tbTeacherDeptParent.Id == parentId).Count() > decimal.Zero)
            {
                var dept = (from p in tbDept
                            where p.tbTeacherDeptParent != null
                            && p.tbTeacherDeptParent.Id == parentId
                            orderby p.No, p.TeacherDeptName
                            select p).ToList();
                foreach (var a in dept)
                {
                    DeptList.Add(new System.Web.Mvc.SelectListItem() { Text = "".PadLeft(levelNo - 1, '│') + '├' + a.TeacherDeptName, Value = a.Id.ToString() });

                    DeepDept(tbDept, ref DeptList, a.Id, levelNo + 1);
                }
            }
        }
    }
}