using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamCourseRateController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamCourseRate.List();
                vm.ExamList = Exam.Controllers.ExamController.SelectPublishList();
                if (vm.ExamUionId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamUionId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }
                vm.SubjectList = Exam.Controllers.ExamCourseController.GetExamSubjectList(vm.ExamUionId);
               
                var tc = (from p in db.Table<Exam.Entity.tbExamCourse>()
                          where  p.tbExam.IsDeleted ==false
                              && p.tbCourse.IsDeleted == false
                              && p.tbCourse.tbSubject.IsDeleted==false
                              && p.Id==vm.ExamCourseId
                          select new
                          {
                              ExamName = p.tbExam.ExamName,
                              CourseName = p.tbCourse.CourseName,
                              SubjectId = p.tbCourse.tbSubject.Id,
                          }).FirstOrDefault();
                if (tc != null && vm.SubjectId==0)
                {
                    vm.SubjectId = tc.SubjectId;
                }
                var tb =(from p in db.Table<Exam.Entity.tbExamCourseRate>()
                         where p.tbExamCourse.Id == vm.ExamCourseId
                         select new Dto.ExamCourseRate.List
                         {
                             Id=p.Id,
                             ExamCourseId=p.tbExamCourse.Id,
                             ExamCourseId1=p.tbExamCourse1.Id,
                             CourseName=p.tbExamCourse1.tbCourse.CourseName,
                             ExamName=p.tbExamCourse1.tbExam.ExamName,
                             Status = p.tbExamCourse1.Id ==vm.ExamCourseId ? false : true,
                             Rate = p.Rate
                         }).ToList();
                
                if (tb.Where(c => c.ExamCourseId == vm.ExamCourseId && c.ExamCourseId1 == vm.ExamCourseId).FirstOrDefault() == null)
                {
                    tb.Add(new Dto.ExamCourseRate.List
                    {
                        Id = db.Table<Entity.tbExamCourseRate>().Select(d => d.Id).DefaultIfEmpty(0).Max() + 1,
                        ExamCourseId = vm.ExamCourseId,
                        ExamCourseId1 = vm.ExamCourseId,
                        CourseName = tc.CourseName,
                        ExamName = tc.ExamName,
                        Status = false,
                        Rate = null
                    });
                }
                vm.ExamCourseRateList = tb;
                //关联考试
                var tv = (from p in db.Table<Exam.Entity.tbExamCourse>()
                          where p.tbCourse.IsDeleted == false
                              && p.tbExam.Id == vm.ExamUionId
                              && p.tbCourse.tbSubject.Id == vm.SubjectId
                          orderby p.tbCourse.No
                          select new Dto.ExamCourseRate.List
                          {
                              ExamCourseId = p.Id,
                              CourseName = p.tbCourse.CourseName,
                              Status = p.Id != vm.ExamCourseId
                          }).ToList();
                vm.ExamCourseUnionList = tv;

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamCourseRate.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { ExamUionId = vm.ExamUionId, subjectId=vm.SubjectId, ExamCourseId =vm.ExamCourseId,examId=vm.ExamId}));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids,int ExamCourseId,int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamCourseRate>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamCourseRate>()
                          where p.tbExamCourse.Id == ExamCourseId
                          select p).ToList();
                if (tf.Count() - tb.Count() == 1)
                {
                    var tv = (from p in db.Table<Exam.Entity.tbExamCourseRate>()
                              where p.tbExamCourse.Id == ExamCourseId
                              select p).ToList();
                    foreach (var t in tv)
                    {
                        t.IsDeleted = true;
                    }
                }
                else
                {
                    foreach (var a in tb)
                    {
                        a.IsDeleted = true;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除考试课程比例");
                }

                return Code.MvcHelper.Post();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Models.ExamCourseRate.List vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var arrystr = new string[] { };
                var txtId = Request["txtId"] != null ? Request["txtId"].Split(',') : arrystr;
                var txtExamCourseId = Request["txtExamCourseId"] != null ? Request["txtExamCourseId"].Split(',') : arrystr;
                var txtExamCourseId1 = Request["txtExamCourseId1"] != null ? Request["txtExamCourseId1"].Split(',') : arrystr;
                var txtCourseRate = Request["txtRate"] != null ? Request["txtRate"].Split(',') : null;

                //验证
                var list = from p in db.Table<Exam.Entity.tbExamCourseRate>()
                            where p.tbExamCourse.IsDeleted==false && p.tbExamCourse1.IsDeleted==false
                            select p;

                for (var i = 0; i < txtId.Count(); i++)
                {
                    if (string.IsNullOrEmpty(txtId[i])==false)
                    {
                        var courseRateId = txtId[i].ConvertToInt();
                        var tb = list.Where(d => d.Id == courseRateId).FirstOrDefault();
                        var courseRate = txtCourseRate != null ? txtCourseRate[i].ConvertToDecimal() : decimal.Zero;
                        if (tb!=null)
                        {
                            tb.Rate = courseRate;
                        }
                        else
                        {
                            //没有id的，执行插入操作
                            if (txtExamCourseId[i].ConvertToInt() == txtExamCourseId1[i].ConvertToInt())
                            {
                                tb = new Exam.Entity.tbExamCourseRate();
                                tb.tbExamCourse = db.Set<Exam.Entity.tbExamCourse>().Find(txtExamCourseId[i].ConvertToInt());
                                tb.tbExamCourse1 = db.Set<Exam.Entity.tbExamCourse>().Find(txtExamCourseId1[i].ConvertToInt());
                                tb.Rate = courseRate;
                                db.Set<Exam.Entity.tbExamCourseRate>().Add(tb);
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考试课程比例");
                            }
                        }
                    }
                }
                //关联的考试课程比例
                var txtUnionCourseId = Request["txtUnionCourseId"] != null ? Request["txtUnionCourseId"].Split(',') : arrystr;
                var txtUnionRate = Request["txtUnionRate"] != null ? Request["txtUnionRate"].Split(',') : null;
                var cboxlist = new List<int>();
                var CboxIdUnion = Request["CboxIdUnion"] != null ? Request["CboxIdUnion"].Split(',') : null;
                if(CboxIdUnion==null && txtId.Count()<=1)
                {
                    return Code.MvcHelper.Post(null, string.Empty, "至少选择一次考试才能保存!");
                }
                if (CboxIdUnion != null)
                {
                    for (var i = 0; i < CboxIdUnion.Count(); i++)
                    {
                        cboxlist.Add(CboxIdUnion[i].ConvertToInt());
                    }
                }
                for (var i = 0; i < txtUnionCourseId.Count(); i++)
                {
                    if (cboxlist.Contains(txtUnionCourseId[i].ConvertToInt()))
                    {
                        var examCourse1Id = txtUnionCourseId[i].ConvertToInt();
                        var tb = list.Where(d => d.tbExamCourse1.Id == examCourse1Id && d.tbExamCourse.Id == vm.ExamCourseId).FirstOrDefault();
                        //var tb = db.Table<Exam.Entity.tbExamCourseRate>().Where(d => d.tbExamCourse1.Id == examCourse1Id && d.tbExamCourse.Id == vm.ExamCourseId).FirstOrDefault();
                        var courseRate = txtUnionRate != null ? txtUnionRate[i].ConvertToDecimal() : decimal.Zero;
                        if (tb == null)
                        {
                            tb = new Exam.Entity.tbExamCourseRate();
                            tb.tbExamCourse = db.Set<Exam.Entity.tbExamCourse>().Find(vm.ExamCourseId);
                            tb.tbExamCourse1 = db.Set<Exam.Entity.tbExamCourse>().Find(examCourse1Id);
                            tb.Rate = courseRate;
                            db.Set<Exam.Entity.tbExamCourseRate>().Add(tb);
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考试课程比例");
                        }
                        else
                        {
                            tb.Rate = courseRate;
                        }
                    }
                }
                
                db.SaveChanges();
                return Code.MvcHelper.Post(null, string.Empty, "提交成功!");
            }
        }

    }
}