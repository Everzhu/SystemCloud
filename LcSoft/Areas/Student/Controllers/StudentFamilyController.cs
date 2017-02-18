using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Student.Controllers
{
    public class StudentFamilyController : Controller
    {
        public ActionResult List(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudentFamily.List();
                vm.StudentId = id;
                var tb = from p in db.Table<Student.Entity.tbStudentFamily>()
                         where p.tbStudent.Id == id
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.FamilyName.Contains(vm.SearchText));
                }

                vm.StudentFamilyList = (from p in tb
                                        orderby p.tbDictKinship.KinshipName
                                        select new Dto.StudentFamily.List
                                        {
                                            Id = p.Id,
                                            FamilyName = p.FamilyName,
                                            EducationName = p.tbDictEducation.EducationName,
                                            KinshipName = p.tbDictKinship.KinshipName,
                                            UnitName = p.UnitName,
                                            Job = p.Job,
                                            Mobile = p.Mobile,
                                        }).ToList();

                return PartialView(vm);
            }
        }

        [HttpPost]
        public ActionResult List(Models.StudentFamily.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        public ActionResult Delete(string guid)
        {
            var error = new List<string>();


            return Code.MvcHelper.Post(error);
        }

        public ActionResult Edit(int FamilyId = 0, string json = null)
        {

            var vm = new Models.StudentFamily.Edit();
            if (FamilyId != 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.StudentFamilyEdit = (from p in db.Table<Student.Entity.tbStudentFamily>()
                                            where p.Id == FamilyId
                                            select new Dto.StudentFamily.Edit()
                                            {
                                                Id = p.Id,
                                                FamilyName = p.FamilyName,
                                                EducationId = p.tbDictEducation.Id,
                                                Job = p.Job,
                                                Mobile = p.Mobile,
                                                KinshipId = p.tbDictKinship.Id,
                                                UnitName = p.UnitName
                                            }).FirstOrDefault();
                }
            }
            vm.KinshipList = Dict.Controllers.DictKinshipController.SelectList();
            vm.EducationList = Dict.Controllers.DictEducationController.SelectList();
            if (!string.IsNullOrEmpty(json))
            {
                vm.StudentFamilyEdit = Newtonsoft.Json.JsonConvert.DeserializeObject<Dto.StudentFamily.Edit>(json);
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.StudentFamily.Edit vm)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                Id = 0,
                FamilyName = vm.StudentFamilyEdit.FamilyName,
                Job = vm.StudentFamilyEdit.Job,
                Mobile = vm.StudentFamilyEdit.Mobile,
                Relation = vm.StudentFamilyEdit.KinshipId,
                UnitName = vm.StudentFamilyEdit.UnitName
            });

            return Content("<script>window.parent.FamilyEditCallBack('" + json + "');</script>");
        }

        public bool InsertFamily(XkSystem.Models.DbContext db, Student.Entity.tbStudent student, string json)
        {
            var error = new List<string>();
            if (json.Length > 20)
            {
                List<Dto.StudentFamily.InsertFamily> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dto.StudentFamily.InsertFamily>>(json);

                foreach (var v in list)
                {
                    if (v.Id > 0)
                    {
                        #region 修改
                        var tb = (from p in db.Table<Student.Entity.tbStudentFamily>()
                                  where p.tbStudent.StudentCode == student.StudentCode && p.Id == v.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.FamilyName = v.FamilyName;
                            tb.tbDictKinship = db.Set<Dict.Entity.tbDictKinship>().Find(v.KinshipId);
                            tb.UnitName = v.UnitName;
                            tb.Job = v.Job;
                            tb.Mobile = v.Mobile;
                            tb.tbDictEducation = db.Set<Dict.Entity.tbDictEducation>().Find(v.EducationId);
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改学生家庭成员");
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                            return false;
                        }
                        #endregion
                    }
                    else
                    {
                        #region 新增
                        var tb = new Student.Entity.tbStudentFamily();
                        tb.tbStudent = student;
                        tb.FamilyName = string.IsNullOrEmpty(v.FamilyName) ? "" : v.FamilyName;
                        tb.tbDictKinship = db.Set<Dict.Entity.tbDictKinship>().Find(v.KinshipId);
                        tb.UnitName = string.IsNullOrEmpty(v.UnitName) ? "" : v.UnitName;
                        tb.Job = string.IsNullOrEmpty(v.Job) ? "" : v.Job;
                        tb.Mobile = string.IsNullOrEmpty(v.Mobile) ? "" : v.Mobile;
                        tb.tbDictEducation = db.Set<Dict.Entity.tbDictEducation>().Find(v.EducationId);
                        db.Set<Student.Entity.tbStudentFamily>().Add(tb);
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加学生家庭成员");
                        #endregion
                    }
                }

                #region 删除ID不属于json的全部数据
                var familyList = (from p in db.Table<Student.Entity.tbStudentFamily>()
                                  where p.tbStudent.StudentCode == student.StudentCode
                                  select p).ToList();
                foreach (var v in familyList)
                {
                    if (list.Where(d => d.Id == v.Id).Count() == 0)
                    {
                        v.IsDeleted = true;
                    }
                }
                #endregion

            }
            return true;
        }

        public ActionResult FamilyList(int id)
        {
            var vm = new Models.StudentFamily.FamilyList();
            vm.StudentId = id;

            using (var db = new XkSystem.Models.DbContext())
            {
                vm.StudentFamilyList = (from p in db.Table<Student.Entity.tbStudentFamily>()
                                        where p.tbStudent.Id == id && p.FamilyName.Contains(vm.SearchText)
                                        select new Dto.StudentFamily.FamilyList()
                                        {
                                            Id = p.Id,
                                            FamilyName = p.FamilyName,
                                            Job = p.Job,
                                            Mobile = p.Mobile,
                                            Email = p.Email,
                                            EducationName = p.tbDictEducation.EducationName,
                                            KinshipName = p.tbDictKinship.KinshipName,
                                            UnitName = p.UnitName
                                        }).ToList();
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult FamilyList(Models.StudentFamily.FamilyList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("FamilyList", new
            {
                id = vm.StudentId,
                searchText = vm.SearchText
            }));
        }

        public ActionResult EditFamily(int familyId = 0, int studentId = 0)
        {
            var vm = new Models.StudentFamily.Edit();
            vm.StudentId = studentId;

            if (familyId > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.StudentFamilyEdit = (from p in db.Table<Student.Entity.tbStudentFamily>()
                                            where p.Id == familyId
                                            select new Dto.StudentFamily.Edit()
                                            {
                                                Id = p.Id,
                                                FamilyName = p.FamilyName,
                                                Job = p.Job,
                                                Email = p.Email,
                                                EducationId = p.tbDictEducation.Id,
                                                Mobile = p.Mobile,
                                                KinshipId = p.tbDictKinship.Id,
                                                UnitName = p.UnitName
                                            }).FirstOrDefault();
                }
            }

            vm.KinshipList = Dict.Controllers.DictKinshipController.SelectList();
            vm.EducationList = Dict.Controllers.DictEducationController.SelectList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFamily(Models.StudentFamily.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = new Student.Entity.tbStudentFamily();
                if (vm.StudentFamilyEdit.Id > 0)
                {
                    tb = db.Set<Student.Entity.tbStudentFamily>().Find(vm.StudentFamilyEdit.Id);
                    tb.FamilyName = vm.StudentFamilyEdit.FamilyName;
                    tb.Job = vm.StudentFamilyEdit.Job;
                    tb.Email = vm.StudentFamilyEdit.Email;
                    tb.Mobile = vm.StudentFamilyEdit.Mobile;
                    tb.tbDictKinship = db.Set<Dict.Entity.tbDictKinship>().Find(vm.StudentFamilyEdit.KinshipId);
                    tb.UnitName = vm.StudentFamilyEdit.UnitName;
                    tb.tbDictEducation = db.Set<Dict.Entity.tbDictEducation>().Find(vm.StudentFamilyEdit.EducationId);
                }
                else
                {
                    tb = new Student.Entity.tbStudentFamily()
                    {
                        FamilyName = vm.StudentFamilyEdit.FamilyName,
                        Job = vm.StudentFamilyEdit.Job,
                        Mobile = vm.StudentFamilyEdit.Mobile,
                        Email = vm.StudentFamilyEdit.Email,
                        tbDictKinship = db.Set<Dict.Entity.tbDictKinship>().Find(vm.StudentFamilyEdit.KinshipId),
                        UnitName = vm.StudentFamilyEdit.UnitName,
                        tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId),
                        tbDictEducation = db.Set<Dict.Entity.tbDictEducation>().Find(vm.StudentFamilyEdit.EducationId)
                    };

                    //当添加父亲、母亲时，判断数据库中是否已经有父母记录
                    var kinshipName = db.Set<Dict.Entity.tbDictKinship>().Find(vm.StudentFamilyEdit.KinshipId).KinshipName;
                    if (new string[] { "父亲", "母亲" }.Contains(kinshipName))
                    {
                        if (db.Table<Student.Entity.tbStudentFamily>().Where(d => d.tbDictKinship.Id == vm.StudentFamilyEdit.KinshipId && d.tbStudent.Id == vm.StudentId).Count() > 0)
                        {
                            var error = new List<string>();
                            error.Add("此亲属关系记录已经存在！");
                            return Code.MvcHelper.Post(error);
                        }
                    }

                    db.Set<Student.Entity.tbStudentFamily>().Add(tb);
                }

                db.SaveChanges();
            }

            return Code.MvcHelper.Post();
        }

        public ActionResult DeleteFamily(List<int> ids)
        {
            int studentId = Request["studentId"].ConvertToInt();
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Student.Entity.tbStudentFamily>().Where(d => ids.Contains(d.Id)).ToList();
                foreach (var v in tb)
                {
                    v.IsDeleted = true;
                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了学生家庭信息");
                }
            }
            return Code.MvcHelper.Post(null, Url.Action("FamilyList", new { id = studentId }));
            //return Content(Code.Common.Redirect(Url.Action("FamilyList", new { id = studentId })));
        }
    }
}