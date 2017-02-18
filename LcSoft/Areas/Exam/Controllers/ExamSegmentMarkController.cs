using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamSegmentMarkController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamSegmentMark.List();
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                vm.SubjectList = Areas.Course.Controllers.SubjectController.SelectList();
                vm.SegmentGroupList = Areas.Exam.Controllers.ExamSegmentGroupController.SelectList();
                var tb = from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.SegmentName.Contains(vm.SearchText));
                }

                if (vm.GradeId != 0)
                {
                    tb = tb.Where(d => d.tbGrade.Id == vm.GradeId);
                }

                if (vm.SubjectId != 0)
                {
                    tb = tb.Where(d => d.tbSubject.Id == vm.SubjectId);
                }
                if (vm.SegmentGroupId != 0)
                {
                    tb = tb.Where(d => d.tbExamSegmentGroup.Id == vm.SegmentGroupId);
                }
                vm.ExamSegmentMarkList = (from p in tb
                               orderby p.tbGrade.No,p.tbSubject.No,p.No,p.SegmentName
                               select new Dto.ExamSegmentMark.List
                               {
                                   Id = p.Id,
                                   No = p.No,
                                   SegmentName=p.SegmentName,
                                   SubjectName=p.tbSubject.SubjectName,
                                   ExamSegmentGroupName=p.tbExamSegmentGroup.ExamSegmentGroupName,
                                   GradeName=p.tbGrade.GradeName,
                                   MinMark=p.MinMark,
                                   MaxMark=p.MaxMark,
                                   IsGood=p.IsGood,
                                   IsPass=p.IsPass,
                                   IsNormal=p.IsNormal,
                                   IsTotal=p.IsTotal,
                                   Rate=p.Rate,
                                   IsGenerate=p.IsGenerate
                               }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamSegmentMark.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                gradeId = vm.GradeId,
                subjectId = vm.SubjectId,
                segmentGroupId=vm.SegmentGroupId,
                searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除分数段");
                }

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetGood(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Exam.Entity.tbExamSegmentMark>().Find(id);
                if (tb != null)
                {
                    tb.IsGood = !tb.IsGood;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetPass(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Exam.Entity.tbExamSegmentMark>().Find(id);
                if (tb != null)
                {
                    tb.IsPass = !tb.IsPass;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetTotal(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Exam.Entity.tbExamSegmentMark>().Find(id);
                if (tb != null)
                {
                    tb.IsTotal = !tb.IsTotal;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetNormal(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Exam.Entity.tbExamSegmentMark>().Find(id);
                if (tb != null)
                {
                    tb.IsNormal = !tb.IsNormal;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamSegmentMark.Edit();
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                vm.SubjectList =Areas.Course.Controllers.SubjectController.SelectList();
                vm.SegmentGroupList = Areas.Exam.Controllers.ExamSegmentGroupController.SelectList();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamSegmentMark>().Include(d=>d.tbExamSegmentGroup)
                              where p.Id == id
                              select new Dto.ExamSegmentMark.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  SegmentName=p.SegmentName,
                                  GradeId=p.tbGrade.Id,
                                  SubjectId=p.tbSubject.Id,
                                  SegmentGroupId=p.tbExamSegmentGroup.Id,
                                  MinMark=p.MinMark,
                                  MaxMark=p.MaxMark,
                                  IsGood=p.IsGood,
                                  IsPass=p.IsPass,
                                  IsNormal=p.IsNormal,
                                  IsTotal=p.IsTotal,
                                  Rate= p.Rate,
                                  IsGenerate= p.IsGenerate
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamSegmentMarkEdit = tb;
                    }
                }

                return View(vm);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetGenerate(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Exam.Entity.tbExamSegmentMark>().Find(id);
                if (tb != null)
                {
                    tb.IsGenerate = !tb.IsGenerate;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamSegmentMark.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Exam.Entity.tbExamSegmentMark>().Where(d=>d.SegmentName == vm.ExamSegmentMarkEdit.SegmentName && d.tbExamSegmentGroup.Id==vm.ExamSegmentMarkEdit.SegmentGroupId
                       && !string.IsNullOrEmpty(vm.ExamSegmentMarkEdit.SegmentName) && d.tbGrade.Id == vm.ExamSegmentMarkEdit.GradeId && d.tbSubject.Id == vm.ExamSegmentMarkEdit.SubjectId && d.Id != vm.ExamSegmentMarkEdit.Id).Any())
                    {
                        error.AddError("该分数段分组年级科目分数段已存在!");
                    }
                    else
                    {
                        if (vm.ExamSegmentMarkEdit.Id == 0)
                        {
                            var tb = new Exam.Entity.tbExamSegmentMark();
                            tb.No = vm.ExamSegmentMarkEdit.No == null ? db.Table<Exam.Entity.tbExamSegmentMark>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamSegmentMarkEdit.No;
                            tb.SegmentName = vm.ExamSegmentMarkEdit.SegmentName;
                            tb.tbExamSegmentGroup = db.Set<Entity.tbExamSegmentGroup>().Find(vm.ExamSegmentMarkEdit.SegmentGroupId);
                            tb.tbSubject = db.Set<Course.Entity.tbSubject>().Find(vm.ExamSegmentMarkEdit.SubjectId);
                            tb.tbGrade = db.Set<Basis.Entity.tbGrade>().Find(vm.ExamSegmentMarkEdit.GradeId);
                            tb.MinMark = vm.ExamSegmentMarkEdit.MinMark;
                            tb.MaxMark = vm.ExamSegmentMarkEdit.MaxMark;
                            tb.IsGood = vm.ExamSegmentMarkEdit.IsGood;
                            tb.IsPass = vm.ExamSegmentMarkEdit.IsPass;
                            tb.IsNormal = vm.ExamSegmentMarkEdit.IsNormal;
                            tb.IsTotal = vm.ExamSegmentMarkEdit.IsTotal;
                            tb.Rate = vm.ExamSegmentMarkEdit.Rate;
                            tb.IsGenerate = vm.ExamSegmentMarkEdit.IsGenerate;
                            db.Set<Exam.Entity.tbExamSegmentMark>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加分数段");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Exam.Entity.tbExamSegmentMark>().Include(d => d.tbSubject).Include(d=>d.tbExamSegmentGroup)
                                      where p.Id == vm.ExamSegmentMarkEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.ExamSegmentMarkEdit.No == null ? db.Table<Exam.Entity.tbExamSegmentMark>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamSegmentMarkEdit.No;
                                tb.SegmentName = vm.ExamSegmentMarkEdit.SegmentName;
                                tb.tbExamSegmentGroup = db.Set<Entity.tbExamSegmentGroup>().Find(vm.ExamSegmentMarkEdit.SegmentGroupId);
                                tb.tbSubject = db.Set<Course.Entity.tbSubject>().Find(vm.ExamSegmentMarkEdit.SubjectId);
                                tb.tbGrade = db.Set<Basis.Entity.tbGrade>().Find(vm.ExamSegmentMarkEdit.GradeId);
                                tb.MinMark = vm.ExamSegmentMarkEdit.MinMark;
                                tb.MaxMark = vm.ExamSegmentMarkEdit.MaxMark;
                                tb.IsGood = vm.ExamSegmentMarkEdit.IsGood;
                                tb.IsPass = vm.ExamSegmentMarkEdit.IsPass;
                                tb.IsNormal = vm.ExamSegmentMarkEdit.IsNormal;
                                tb.IsTotal = vm.ExamSegmentMarkEdit.IsTotal;
                                tb.Rate = vm.ExamSegmentMarkEdit.Rate;
                                tb.IsGenerate = vm.ExamSegmentMarkEdit.IsGenerate;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改分数段");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                return Code.MvcHelper.Post(error, Url.Action("List"));
            }
        }

        #region 导入

        public ActionResult Import(Models.ExamSegmentMark.Import vm,List<string> error = null)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (error != null && error.Count > decimal.Zero)
                {
                    ModelState.AddModelError("", error[0]);
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.ExamSegmentMark.Import vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);

                using (var db = new XkSystem.Models.DbContext())
                {
                    try
                    {
                        var ExList = new List<string>() { ".xlsx" };
                        if (!ExList.Contains(System.IO.Path.GetExtension(file.FileName)))
                        {
                            error.AddError("上传的文件不是正确的EXCLE文件!");
                        }
                        else
                        {
                            var dt = Code.NpoiHelper.ExcelToDataTable(fileSave, System.IO.Path.GetExtension(file.FileName), string.Empty);
                            if (dt == null)
                            {
                                error.AddError("无法读取上传的文件，请检查文件格式是否正确!");
                            }
                            else
                            {
                                var tbList = new List<string>() {"排序", "年级", "科目", "分数段","比例", "最低百分数", "最高百分数","分数段分组", "是否优秀", "是否良好","是否及格","是否总分段"};

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
                                    error.AddError("上传的EXCEL内容与预期不一致!错误详细:" + Text);
                                }
                                var subjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                                   orderby p.No
                                            select new
                                            {
                                               SubjectId=p.Id,
                                               SubjectName=p.SubjectName
                                            }).ToList();

                                var gradeList = (from p in db.Table<Basis.Entity.tbGrade>()
                                                      orderby p.No
                                                      select new
                                                      {
                                                          GradeName = p.GradeName,
                                                          GradeId = p.Id
                                                      }).ToList();

                                var segmentGroupList = (from p in db.Table<Entity.tbExamSegmentGroup>()
                                                 orderby p.No
                                                 select new
                                                 {
                                                     ExamSegmentGroupName = p.ExamSegmentGroupName,
                                                     SegmentGroupId = p.Id
                                                 }).ToList();

                                var segmentList = from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                                  where p.tbGrade.IsDeleted == false
                                                  && p.tbSubject.IsDeleted == false
                                                  select p;

                                foreach (DataRow dr in dt.Rows)
                                {
                                    var No = dr["排序"].ToString().Trim();
                                    var grade = dr["年级"].ToString().Trim();
                                    var subject = dr["科目"].ToString().Trim();
                                    var segmentName = dr["分数段"].ToString();
                                    var rate = dr["比例"].ToString();
                                    var mixMark = dr["最低百分数"].ToString().Trim();
                                    var maxMark = dr["最高百分数"].ToString().Trim();
                                    var segmentGroup = dr["分数段分组"].ToString().Trim();
                                    var isGood = dr["是否优秀"].ToString().Trim();
                                    var isNormal = dr["是否良好"].ToString().Trim();
                                    var isPass = dr["是否及格"].ToString().Trim();
                                    var isTotal = dr["是否总分段"].ToString().Trim();
                                    if (string.IsNullOrEmpty(grade) ||  string.IsNullOrEmpty(segmentName))
                                    {
                                        continue;
                                    }
                                    if (gradeList.Where(d => d.GradeName == grade).Count() == decimal.Zero)
                                    {
                                        var strmes = string.Format("年级不存在({0})", grade);
                                        error.AddError(strmes);
                                    }
                                    if (!string.IsNullOrEmpty(subject))
                                    {
                                        if (subjectList.Where(d => d.SubjectName == subject).Count() == decimal.Zero)
                                        {
                                            var strmes = string.Format("科目不存在({0})", subject);
                                            error.AddError(strmes);
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(segmentGroup))
                                    {
                                        if (segmentGroupList.Where(d => d.ExamSegmentGroupName == segmentGroup).Count() == decimal.Zero)
                                        {
                                            var strmes = string.Format("分数段分组不存在({0})", segmentGroup);
                                            error.AddError(strmes);
                                        }
                                    }
                                    var tb = (from p in segmentList
                                              where p.tbGrade.GradeName==grade
                                               && p.tbSubject.SubjectName==subject
                                               && p.SegmentName==segmentName
                                               && p.tbExamSegmentGroup.ExamSegmentGroupName==segmentGroup
                                              select p).FirstOrDefault();
                                    if (tb == null)
                                    {
                                        var tf = new Exam.Entity.tbExamSegmentMark();
                                        tf.No = No.ConvertToIntWithNull();
                                        tf.tbGrade = db.Table<Basis.Entity.tbGrade>().Where(d => d.GradeName == grade).FirstOrDefault();
                                        tf.tbSubject = db.Set<Course.Entity.tbSubject>().Where(d => d.SubjectName == subject).FirstOrDefault();
                                        tf.tbExamSegmentGroup = db.Set<Entity.tbExamSegmentGroup>().Where(d => d.ExamSegmentGroupName ==segmentGroup).FirstOrDefault();
                                        tf.SegmentName = segmentName;
                                        tf.Rate = rate.ConvertToDecimal();
                                        tf.MinMark =mixMark.ConvertToDecimal();
                                        tf.MaxMark =maxMark.ConvertToDecimal();
                                        tf.IsGood = isGood=="是"?true:false;
                                        tf.IsNormal = isNormal == "是" ? true : false;
                                        tf.IsPass = isPass == "是" ? true : false;
                                        tf.IsTotal =isTotal == "是" ? true : false;
                                        tf.IsGenerate = false;
                                        db.Set<Exam.Entity.tbExamSegmentMark>().Add(tf);
                                    }
                                    else
                                    {
                                        tb.Rate = rate.ConvertToDecimal();
                                        tb.No = No.ConvertToIntWithNull();
                                        tb.MinMark = mixMark.ConvertToDecimal();
                                        tb.MaxMark = maxMark.ConvertToDecimal();
                                        tb.IsGood = isGood == "是" ? true : false;
                                        tb.IsPass = isPass == "是" ? true : false;
                                        tb.IsNormal = isNormal == "是" ? true : false;
                                        tb.IsTotal = isTotal == "是" ? true : false;

                                    }
                                }

                                if (db.SaveChanges() > decimal.Zero)
                                {
                                    error.AddError("导入成功!");
                                    vm.Status = true;
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量添加分数段");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        error.AddError("上传的EXCEL内容与预期不一致!错误详细:" + ex.Message);
                    }
                }
            }

            return this.Import(vm,error);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Exam/Views/ExamSegmentMark/ExamSegmentTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        #endregion
    }
}