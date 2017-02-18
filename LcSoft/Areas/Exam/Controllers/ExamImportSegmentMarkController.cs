using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamImportSegmentMarkController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamImportSegmentMark.List();
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                var tb = from p in db.Table<Exam.Entity.tbExamImportSegmentMark>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.SegmentName.Contains(vm.SearchText));
                }

                if (vm.GradeId != 0)
                {
                    tb = tb.Where(d => d.tbGrade.Id == vm.GradeId);
                }

                vm.ExamImportSegmentMarkList = (from p in tb
                               orderby p.tbGrade.No,p.No,p.SegmentName
                               select new Dto.ExamImportSegmentMark.List
                               {
                                   Id = p.Id,
                                   No = p.No,
                                   SegmentName=p.SegmentName,
                                   GradeName=p.tbGrade.GradeName,
                                   MinMark=p.MinMark,
                                   MaxMark=p.MaxMark,
                               }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamImportSegmentMark.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                gradeId = vm.GradeId,
                searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamImportSegmentMark>()
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

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamImportSegmentMark.Edit();
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamImportSegmentMark>()
                              where p.Id == id
                              select new Dto.ExamImportSegmentMark.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  SegmentName=p.SegmentName,
                                  GradeId=p.tbGrade.Id,
                                  MinMark=p.MinMark,
                                  MaxMark=p.MaxMark
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamImportSegmentMarkEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamImportSegmentMark.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Exam.Entity.tbExamImportSegmentMark>().Where(d=>d.SegmentName == vm.ExamImportSegmentMarkEdit.SegmentName && !string.IsNullOrEmpty(vm.ExamImportSegmentMarkEdit.SegmentName) && d.tbGrade.Id == vm.ExamImportSegmentMarkEdit.GradeId  && d.Id != vm.ExamImportSegmentMarkEdit.Id).Any())
                    {
                        error.AddError("该年级分数段已存在!");
                    }
                    else
                    {
                        if (vm.ExamImportSegmentMarkEdit.Id == 0)
                        {
                            var tb = new Exam.Entity.tbExamImportSegmentMark();
                            tb.No = vm.ExamImportSegmentMarkEdit.No == null ? db.Table<Exam.Entity.tbExamImportSegmentMark>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamImportSegmentMarkEdit.No;
                            tb.SegmentName = vm.ExamImportSegmentMarkEdit.SegmentName;
                            tb.tbGrade = db.Set<Basis.Entity.tbGrade>().Find(vm.ExamImportSegmentMarkEdit.GradeId);
                            tb.MinMark = vm.ExamImportSegmentMarkEdit.MinMark;
                            tb.MaxMark = vm.ExamImportSegmentMarkEdit.MaxMark;
                            db.Set<Exam.Entity.tbExamImportSegmentMark>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加分数段");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Exam.Entity.tbExamImportSegmentMark>()
                                      where p.Id == vm.ExamImportSegmentMarkEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.ExamImportSegmentMarkEdit.No == null ? db.Table<Exam.Entity.tbExamImportSegmentMark>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamImportSegmentMarkEdit.No;
                                tb.SegmentName = vm.ExamImportSegmentMarkEdit.SegmentName;
                                tb.tbGrade = db.Set<Basis.Entity.tbGrade>().Find(vm.ExamImportSegmentMarkEdit.GradeId);
                                tb.MinMark = vm.ExamImportSegmentMarkEdit.MinMark;
                                tb.MaxMark = vm.ExamImportSegmentMarkEdit.MaxMark;
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

        public ActionResult Import(Models.ExamImportSegmentMark.Import vm,List<string> error = null)
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
        public ActionResult Import(Models.ExamImportSegmentMark.Import vm)
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
                                var tbList = new List<string>() { "排序", "年级", "分数段","最低百分数", "最高百分数"};

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

                                var gradeList = (from p in db.Table<Basis.Entity.tbGrade>()
                                                      orderby p.No
                                                      select new
                                                      {
                                                          GradeName = p.GradeName,
                                                          GradeId = p.Id
                                                      }).ToList();


                                var segmentList = from p in db.Table<Exam.Entity.tbExamImportSegmentMark>()
                                                  where p.tbGrade.IsDeleted == false
                                                  select p;

                                foreach (DataRow dr in dt.Rows)
                                {
                                    var No = dr["排序"].ToString().Trim();
                                    var grade = dr["年级"].ToString().Trim();
                                    var segmentName = dr["分数段"].ToString();
                                    var mixMark = dr["最低百分数"].ToString().Trim();
                                    var maxMark = dr["最高百分数"].ToString().Trim();
                                    if (string.IsNullOrEmpty(grade) ||  string.IsNullOrEmpty(segmentName))
                                    {
                                        continue;
                                    }
                                    if (gradeList.Where(d => d.GradeName == grade).Count() == decimal.Zero)
                                    {
                                        var strmes = string.Format("年级不存在({0})", grade);
                                        error.AddError(strmes);
                                    }
                                    var tb = (from p in segmentList
                                              where p.tbGrade.GradeName==grade
                                               && p.SegmentName==segmentName
                                              select p).FirstOrDefault();
                                    if (tb == null)
                                    {
                                        var tf = new Exam.Entity.tbExamImportSegmentMark();
                                        tf.No = No.ConvertToIntWithNull();
                                        tf.tbGrade = db.Table<Basis.Entity.tbGrade>().Where(d => d.GradeName == grade).FirstOrDefault();
                                        tf.SegmentName = segmentName;
                                        tf.MinMark =mixMark.ConvertToDecimal();
                                        tf.MaxMark =maxMark.ConvertToDecimal();
                                        db.Set<Exam.Entity.tbExamImportSegmentMark>().Add(tf);
                                    }
                                    else
                                    {
                                        tb.No = No.ConvertToIntWithNull();
                                        tb.MinMark = mixMark.ConvertToDecimal();
                                        tb.MaxMark = maxMark.ConvertToDecimal();
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
            var file = Server.MapPath("~/Areas/Exam/Views/ExamImportSegmentMark/ExamImportSegmentTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        #endregion
    }
}