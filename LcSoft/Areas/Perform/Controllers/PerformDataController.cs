using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Perform.Controllers
{
    public class PerformDataController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformData.List();

                vm.PerformList = Perform.Controllers.PerformController.SelectList();

                if (vm.PerformId == 0 && vm.PerformList.Count > 0)
                {
                    vm.PerformId = vm.PerformList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.ClassList = SelectOrgList(vm.PerformId);

                if (vm.ClassList.Count > 0 && vm.ClassId > 0)
                {
                    if (vm.ClassList.Where(d => d.Value.ConvertToInt() == vm.ClassId).Count() == decimal.Zero)
                    {
                        vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                    }
                }

                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.ClassList.Count() == decimal.Zero)
                {
                    vm.ClassId = 0;
                }

                vm.OrgSelectInfo = PerformChangeController.SelectOrgSelectInfo(db, vm.ClassList, vm.PerformId);

                var tbOrg = (from p in db.Table<Course.Entity.tbOrg>()
                            .Include(d => d.tbClass)
                            .Include(d => d.tbYear)
                            .Include(d => d.tbCourse)
                             where p.Id == vm.ClassId
                             select p).FirstOrDefault();

                if (tbOrg == null)
                {
                    return View(vm);
                }

                var tbPerformGroupIds = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                         where p.tbCourse.Id == tbOrg.tbCourse.Id
                                         && p.tbPerformGroup.tbPerform.Id == vm.PerformId
                                         && p.tbPerformGroup.IsDeleted == false
                                         && p.tbCourse.IsDeleted == false
                                         select p.tbPerformGroup.Id).ToList();


                vm.PerformItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                      where tbPerformGroupIds.Contains(p.tbPerformGroup.Id)
                                      && p.tbPerformGroup.tbPerform.Id == vm.PerformId
                                      && p.tbPerformGroup.IsDeleted == false
                                      && p.IsMany == false
                                      && p.IsSelect == false
                                      orderby p.No
                                      select new Dto.PerformItem.List
                                      {
                                          Id = p.Id,
                                          Rate = p.Rate,
                                          ScoreMax = p.ScoreMax,
                                          PerformItemName = p.PerformItemName
                                      }
                                    ).ToList();

                vm.PerformDataAllList = (from p in db.Table<Perform.Entity.tbPerformData>()
                                         where p.tbPerformItem.tbPerformGroup.tbPerform.Id == vm.PerformId
                                         && p.tbPerformItem.IsDeleted == false
                                         && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                         && p.tbStudent.IsDeleted == false
                                         && p.tbCourse.IsDeleted == false
                                         && p.tbPerformItem.IsMany == false
                                         && p.tbPerformItem.IsSelect == false
                                         select new Dto.PerformData.List
                                         {
                                             PerformItemId = p.tbPerformItem.Id,
                                             Score = p.Score,
                                             StudentId = p.tbStudent.Id,
                                             CourseId = p.tbCourse.Id
                                         }).ToList();

                vm.PerformTotalList = (from p in db.Table<Perform.Entity.tbPerformTotal>()
                                       where p.tbPerform.Id == vm.PerformId
                                       && p.tbCourse.Id == tbOrg.tbCourse.Id
                                       && p.tbStudent.IsDeleted == false
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbPerform.IsDeleted == false
                                       select new Dto.PerformTotal.List
                                       {
                                           Id = p.Id,
                                           PerformId = p.tbPerform.Id,
                                           CourseId = p.tbCourse.Id,
                                           PerformName = p.tbPerform.PerformName,
                                           CourseName = p.tbCourse.CourseName,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           TotalScore = p.TotalScore
                                       }).ToList();

                if (tbOrg != null)
                {
                    var orgStudentList = new List<Dto.PerformData.List>();
                    if (tbOrg.IsClass)
                    {
                        if (tbOrg.tbClass != null)
                        {
                            orgStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                              where p.tbClass.Id == tbOrg.tbClass.Id
                                              && p.tbStudent.IsDeleted == false
                                              && p.tbClass.IsDeleted == false
                                              orderby p.No, p.tbStudent.StudentCode
                                              select new Dto.PerformData.List
                                              {
                                                  No = p.No.ToString(),
                                                  StudentId = p.tbStudent.Id,
                                                  StudentCode = p.tbStudent.StudentCode,
                                                  StudentName = p.tbStudent.StudentName,
                                                  CourseId = tbOrg.tbCourse.Id,
                                              }).ToList();
                        }
                    }
                    else
                    {
                        orgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                          where p.tbOrg.Id == vm.ClassId
                                          && p.tbStudent.IsDeleted == false
                                          && p.tbOrg.IsDeleted == false
                                          orderby p.No, p.tbStudent.StudentCode
                                          select new Dto.PerformData.List
                                          {
                                              No = p.No.ToString(),
                                              StudentId = p.tbStudent.Id,
                                              StudentCode = p.tbStudent.StudentCode,
                                              StudentName = p.tbStudent.StudentName,
                                              CourseId = tbOrg.tbCourse.Id
                                          }).ToList();
                    }

                    if (string.IsNullOrEmpty(vm.SearchText) == false)
                    {
                        orgStudentList = (from p in orgStudentList
                                          where p.StudentCode.Contains(vm.SearchText) || p.StudentName.Contains(vm.SearchText)
                                          select p).ToList();
                    }

                    vm.PerformDataList = (from p in orgStudentList
                                          select new Dto.PerformData.List
                                          {
                                              No = p.No == null ? "" : p.No.ToString(),
                                              StudentCode = p.StudentCode.ToString(),
                                              StudentName = p.StudentName.ToString(),
                                              StudentId = p.StudentId,
                                              CourseId = p.CourseId,
                                              PerformItemList = vm.PerformItemList
                                          }).ToList();
                }
                return View(vm);
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectOrgList(int performId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var year = (from p in db.Table<Perform.Entity.tbPerform>()
                            where p.Id == performId
                            select new
                            {
                                YearId = p.tbYear.Id,
                            }).FirstOrDefault();

                var tb = new List<System.Web.Mvc.SelectListItem>();
                if (year != null)
                {
                    //所属组合
                    var tbPerformGroupIds = (from p in db.Table<Perform.Entity.tbPerformGroup>()
                                             where p.tbPerform.Id == performId
                                             && p.tbPerform.IsDeleted == false
                                             select p.Id).ToList();
                    //所属评价
                    var tbCourseIds = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                       where tbPerformGroupIds.Contains(p.tbPerformGroup.Id)
                                       && p.tbPerformGroup.IsDeleted == false
                                       && p.tbCourse.IsDeleted == false
                                       select p.tbCourse.Id).ToList();
                    //所属班级
                    var tbOrgIds = (from p in db.Table<Course.Entity.tbOrg>()
                                    where tbCourseIds.Contains(p.tbCourse.Id)
                                       && p.tbYear.Id == year.YearId
                                    select p.Id).ToList();

                    tb = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                          where tbOrgIds.Contains(p.tbOrg.Id)
                                && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                && p.tbOrg.IsDeleted == false
                                && p.tbTeacher.IsDeleted == false
                          orderby p.tbOrg.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.tbOrg.OrgName,
                              Value = p.tbOrg.Id.ToString()
                          }).ToList();
                }

                return tb;
            }
        }

        public ActionResult GetSectionByTermId(int id)
        {
            return Json(SelectOrgList(id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.PerformData.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, performId = vm.PerformId, classId = vm.ClassId }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Perform.Entity.tbPerformData>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价数据");
                }


                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformData.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Perform.Entity.tbPerformData>()
                              where p.Id == id
                              select new Dto.PerformData.Edit
                              {
                                  Id = p.Id,
                                  CourseId = p.tbCourse.Id,
                                  InputDate = p.InputDate,
                                  Score = p.Score,
                                  PerformItemId = p.tbPerformItem.Id,
                                  StudentId = p.tbStudent.Id,
                                  SysUserId = p.tbSysUser.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.PerformDataEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.PerformData.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.PerformDataEdit.Id == 0)
                    {
                        var tb = new Perform.Entity.tbPerformData();
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        db.Set<Perform.Entity.tbPerformData>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价数据");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Perform.Entity.tbPerformData>()
                                  where p.Id == vm.PerformDataEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价数据");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Models.PerformData.List vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                string strTxtId = "";
                string strStudentId = "";
                string strTotalId = "";//总分Id
                string strTotalScode = "";//总分数据
                try
                {
                    strTxtId = Request["txtId"].ToString();
                    strStudentId = Request["txtStudentId"].ToString();
                    strTotalScode = Request["txtTotalScode"].ToString();
                    strTotalId = Request["txtTotalId"].ToString();
                }
                catch
                {
                    return Code.MvcHelper.Post(null, Url.Action("List"), "暂无数据!");
                }

                var txtId = strTxtId.Split(',');
                var txtStudentId = strStudentId.Split(',');
                var arrStudentIds = txtStudentId.Select(d => d.ConvertToInt()).ToList();
                var txtTotalScode = strTotalScode.Split(',');
                var txtTotalId = strTotalId.Split(',');

                var tbOrg = (from p in db.Table<Course.Entity.tbOrg>()
                            .Include(d => d.tbClass)
                            .Include(d => d.tbYear)
                            .Include(d => d.tbCourse)
                             where p.Id == vm.ClassId
                             select p).FirstOrDefault();

                var tbPerformGroupIds = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                         where p.tbCourse.Id == tbOrg.tbCourse.Id
                                         && p.tbPerformGroup.tbPerform.Id == vm.PerformId
                                         && p.tbPerformGroup.IsDeleted == false
                                         && p.tbCourse.IsDeleted == false
                                         select p.tbPerformGroup.Id).ToList();


                var PerformItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                       where tbPerformGroupIds.Contains(p.tbPerformGroup.Id)
                                       && p.tbPerformGroup.tbPerform.Id == vm.PerformId
                                       && p.tbPerformGroup.IsDeleted == false
                                       && p.IsMany == false
                                       && p.IsSelect == false
                                       orderby p.No
                                       select new Dto.PerformItem.List
                                       {
                                           Id = p.Id,
                                           PerformItemName = p.PerformItemName
                                       }
                                    ).ToList();

                var dataList = (from p in db.Table<Perform.Entity.tbPerformData>()
                                where p.tbPerformItem.tbPerformGroup.tbPerform.Id == vm.PerformId
                                && p.tbPerformItem.IsDeleted == false
                                && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                && p.tbStudent.IsDeleted == false
                                && p.tbCourse.IsDeleted == false
                                && p.tbPerformItem.IsMany == false
                                && p.tbPerformItem.IsSelect == false
                                select p).ToList();
                //总分数据
                var totalList = (from p in db.Table<Perform.Entity.tbPerformTotal>()
                                 where p.tbPerform.Id == vm.PerformId
                                    && p.tbCourse.Id == tbOrg.tbCourse.Id
                                    && p.tbStudent.IsDeleted == false
                                    && p.tbCourse.IsDeleted == false
                                    && p.tbPerform.IsDeleted == false
                                 select p).ToList();
                var list = new List<Perform.Entity.tbPerformData>();
                foreach (var item in PerformItemList)
                {
                    var txtPerformItemScode = Request["txt_" + item.Id].Split(',');
                    for (var i = 0; i < txtStudentId.Count(); i++)
                    {
                        if (string.IsNullOrEmpty(txtStudentId[i]))
                        {
                            //输入内容为空,判断是否存在Id
                            if (string.IsNullOrEmpty(txtId[i]) == false)
                            {
                                //如果是有id的，那就是数据库中记录的，应该做删除
                                var tf = dataList.Where(d => d.Id == txtId[i].ConvertToInt()).FirstOrDefault();
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价数据");
                                tf.IsDeleted = true;
                            }
                        }
                        else
                        {
                            var courseid = tbOrg.tbCourse.Id;
                            var studentId = txtStudentId[i].ConvertToInt();
                            var performitemscode = txtPerformItemScode[i].ConvertToDecimal();
                            //输入内容不为空，判断是否存在id并执行对应的操作
                            var countList = (from p in db.Table<Perform.Entity.tbPerformData>()
                                             where p.tbStudent.Id == studentId
                                                 && p.tbCourse.Id == courseid
                                                 && p.tbPerformItem.Id == item.Id
                                                 && p.tbCourse.IsDeleted == false
                                                 && p.tbStudent.IsDeleted == false
                                             select p).ToList();
                            if (countList.Count() > 0)
                            {
                                //如果有id的，执行更新操作
                                var tf = countList.FirstOrDefault();
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价数据");
                                if (string.IsNullOrEmpty(txtPerformItemScode[i].Trim()))
                                {
                                    tf.Score = txtPerformItemScode[i].ConvertToDecimal();
                                    tf.IsDeleted = true;
                                }
                                tf.Score = txtPerformItemScode[i].ConvertToDecimal();
                                tf.tbStudent = db.Set<Student.Entity.tbStudent>().Find(txtStudentId[i].ConvertToInt());
                                tf.tbCourse = db.Set<Course.Entity.tbCourse>().Find(courseid);
                                tf.tbPerformItem = db.Set<Perform.Entity.tbPerformItem>().Find(item.Id);
                                tf.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(txtPerformItemScode[i].Trim()))
                                {
                                    var tf = new Perform.Entity.tbPerformData();
                                    tf.InputDate = DateTime.Now;
                                    tf.Score = txtPerformItemScode[i].ConvertToDecimal();
                                    tf.tbStudent = db.Set<Student.Entity.tbStudent>().Find(txtStudentId[i].ConvertToInt());
                                    tf.tbCourse = db.Set<Course.Entity.tbCourse>().Find(courseid);
                                    tf.tbPerformItem = db.Set<Perform.Entity.tbPerformItem>().Find(item.Id);
                                    tf.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                                    list.Add(tf);
                                }
                            }
                        }
                    }
                }

                db.Set<Perform.Entity.tbPerformData>().AddRange(list);
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("保存评价数据");
                }

                #region 保存总分
                var oldTotalList = (from p in db.Table<Perform.Entity.tbPerformTotal>()
                                    where p.tbPerform.Id == vm.PerformId
                                       && p.tbCourse.Id == tbOrg.tbCourse.Id
                                       && arrStudentIds.Contains(p.tbStudent.Id)
                                       && p.tbStudent.IsDeleted == false
                                       && p.tbCourse.IsDeleted == false
                                       && p.tbPerform.IsDeleted == false
                                    select p).ToList();

                foreach (var a in oldTotalList)
                {
                    a.IsDeleted = true;
                    a.UpdateTime = DateTime.Now;
                }

                var studentSumScore = (from p in db.Table<Entity.tbPerformData>()
                                       where p.tbCourse.Id == tbOrg.tbCourse.Id
                                       && arrStudentIds.Contains(p.tbStudent.Id)
                                       && p.tbPerformItem.tbPerformGroup.tbPerform.Id == vm.PerformId
                                       && p.tbPerformItem.IsMany == false
                                       && p.tbPerformItem.IsSelect == false
                                       select new
                                       {
                                           studentId = p.tbStudent.Id,
                                           sum = p.Score * p.tbPerformItem.Rate / 100,
                                       }).ToList();

                studentSumScore= (from p in studentSumScore
                                  group p by p.studentId into g
                                  select new
                                  {
                                      studentId = g.Key,
                                      sum = g.Select(d => d.sum).Sum()
                                  }).ToList();

                var studentDaySumScore = (from p in db.Table<Entity.tbPerformData>()
                                          where p.tbCourse.Id == tbOrg.tbCourse.Id
                                          && arrStudentIds.Contains(p.tbStudent.Id)
                                          && p.tbPerformItem.tbPerformGroup.tbPerform.Id == vm.PerformId
                                          && ((p.tbPerformItem.IsSelect == true && p.tbPerformItem.IsMany == true) || (p.tbPerformItem.IsMany == true && p.tbPerformItem.IsSelect == false))
                                          group p by new { p.tbStudent.Id } into g
                                          select new
                                          {
                                              studentId = g.Key.Id,
                                              sum = g.Select(d => d.Score).Sum()
                                          }).ToList();

                studentSumScore = studentSumScore.Union(studentDaySumScore).ToList();

                var sumStudentScore = (from p in studentSumScore
                                       group p by p.studentId into g
                                       select new
                                       {
                                           studentId = g.Key,
                                           sum = g.Select(d => d.sum).Sum()
                                       }).ToList();

                var entityAddTotal = new List<Entity.tbPerformTotal>();
                foreach (var studentId in arrStudentIds)
                {
                    var tfTotal = new Perform.Entity.tbPerformTotal();
                    var allSumScore = sumStudentScore.Where(d => d.studentId == studentId).FirstOrDefault();
                    if (allSumScore == null)
                    {
                        continue;
                    }
                    else
                    {
                        tfTotal.TotalScore = sumStudentScore.Where(d => d.studentId == studentId).FirstOrDefault().sum;
                        tfTotal.tbPerform = db.Set<Perform.Entity.tbPerform>().Find(vm.PerformId);
                        tfTotal.tbStudent = db.Set<Student.Entity.tbStudent>().Find(studentId);
                        tfTotal.tbCourse = db.Set<Course.Entity.tbCourse>().Find(tbOrg.tbCourse.Id);
                        entityAddTotal.Add(tfTotal);
                    }
                }
                db.Set<Perform.Entity.tbPerformTotal>().AddRange(entityAddTotal);

                if (db.SaveChanges() > decimal.Zero)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("新增学习表现总分");
                }
                #endregion

                return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, performId = vm.PerformId, classId = vm.ClassId }), "提交成功!");
            }
        }

        public ActionResult Export(int PerformId, int ClassId, string SearchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformData.List();
                var file = System.IO.Path.GetTempFileName();
                var PerformList = Perform.Controllers.PerformController.SelectList();
                if (PerformId == 0 && PerformList.Count > 0)
                {
                    PerformId = PerformList.FirstOrDefault().Value.ConvertToInt();
                }
                var ClassList = SelectOrgList(PerformId);

                if (ClassList.Count > 0 && ClassId > 0)
                {
                    if (ClassList.Where(d => d.Value.ConvertToInt() == ClassId).Count() == decimal.Zero)
                    {
                        ClassId = ClassList.FirstOrDefault().Value.ConvertToInt();
                    }
                }

                if (ClassId == 0 && ClassList.Count > 0)
                {
                    ClassId = ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                if (ClassList.Count() == decimal.Zero)
                {
                    ClassId = 0;
                }

                var tbOrg = (from p in db.Table<Course.Entity.tbOrg>()
                            .Include(d => d.tbClass)
                            .Include(d => d.tbYear)
                            .Include(d => d.tbCourse)
                             where p.Id == ClassId
                             select p).FirstOrDefault();
                if (tbOrg != null)
                {
                    var tbPerformGroupIds = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                             where p.tbCourse.Id == tbOrg.tbCourse.Id
                                             && p.tbPerformGroup.tbPerform.Id == PerformId
                                             && p.tbCourse.IsDeleted == false
                                             && p.tbPerformGroup.IsDeleted == false
                                             select p.tbPerformGroup.Id).ToList();

                    var PerformItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                           where tbPerformGroupIds.Contains(p.tbPerformGroup.Id)
                                           && p.tbPerformGroup.tbPerform.Id == PerformId
                                           && p.tbPerformGroup.IsDeleted == false
                                           && p.IsMany == false
                                           && p.IsSelect == false
                                           orderby p.No
                                           select new Dto.PerformItem.List
                                           {
                                               Id = p.Id,
                                               Rate = p.Rate,
                                               ScoreMax = p.ScoreMax,
                                               PerformItemName = p.PerformItemName
                                           }).ToList();

                    var PerformDataAllList = (from p in db.Table<Perform.Entity.tbPerformData>()
                                              where p.tbPerformItem.tbPerformGroup.tbPerform.Id == PerformId
                                              && p.tbPerformItem.IsDeleted == false
                                              && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                              && p.tbStudent.IsDeleted == false
                                              && p.tbCourse.IsDeleted == false
                                              && p.tbPerformItem.IsMany == false
                                              && p.tbPerformItem.IsSelect == false
                                              select new Dto.PerformData.List
                                              {
                                                  PerformItemId = p.tbPerformItem.Id,
                                                  Score = p.Score,
                                                  StudentId = p.tbStudent.Id,
                                                  CourseId = p.tbCourse.Id
                                              }).ToList();

                    var orgStudentList = new List<Dto.PerformData.List>();
                    if (tbOrg.IsClass)
                    {
                        if (tbOrg.tbClass != null)
                        {
                            orgStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                              where p.tbClass.Id == tbOrg.tbClass.Id
                                              && p.tbStudent.IsDeleted == false
                                              && p.tbClass.IsDeleted == false
                                              orderby p.No, p.tbStudent.StudentCode
                                              select new Dto.PerformData.List
                                              {
                                                  No = p.No.ToString(),
                                                  StudentId = p.tbStudent.Id,
                                                  StudentCode = p.tbStudent.StudentCode,
                                                  StudentName = p.tbStudent.StudentName,
                                                  CourseId = tbOrg.tbCourse.Id,
                                              }).ToList();
                        }
                    }
                    else
                    {
                        orgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                          where p.tbOrg.Id == ClassId
                                          && p.tbStudent.IsDeleted == false
                                          && p.tbOrg.IsDeleted == false
                                          orderby p.No, p.tbStudent.StudentCode
                                          select new Dto.PerformData.List
                                          {
                                              No = p.No.ToString(),
                                              StudentId = p.tbStudent.Id,
                                              StudentCode = p.tbStudent.StudentCode,
                                              StudentName = p.tbStudent.StudentName,
                                              CourseId = tbOrg.tbCourse.Id
                                          }).ToList();
                    }

                    if (string.IsNullOrEmpty(SearchText) == false)
                    {
                        orgStudentList = (from p in orgStudentList
                                          where p.StudentCode.Contains(SearchText) || p.StudentName.Contains(SearchText)
                                          select p).ToList();
                    }

                    var PerformDataList = (from p in orgStudentList
                                           select new Dto.PerformData.List
                                           {
                                               No = p.No.ToString(),
                                               StudentCode = p.StudentCode.ToString(),
                                               StudentName = p.StudentName.ToString(),
                                               StudentId = p.StudentId,
                                               CourseId = p.CourseId,
                                               PerformItemList = PerformItemList
                                           }).ToList();

                    var PerformTotalList = (from p in db.Table<Perform.Entity.tbPerformTotal>()
                                            where p.tbPerform.Id == PerformId
                                            && p.tbCourse.Id == tbOrg.tbCourse.Id
                                            && p.tbStudent.IsDeleted == false
                                            && p.tbCourse.IsDeleted == false
                                            && p.tbPerform.IsDeleted == false
                                            select new Dto.PerformTotal.List
                                            {
                                                Id = p.Id,
                                                PerformId = p.tbPerform.Id,
                                                CourseId = p.tbCourse.Id,
                                                PerformName = p.tbPerform.PerformName,
                                                CourseName = p.tbCourse.CourseName,
                                                StudentId = p.tbStudent.Id,
                                                StudentName = p.tbStudent.StudentName,
                                                TotalScore = p.TotalScore
                                            }).ToList();

                    var countL = PerformItemList.Count;
                    var countR = 2;
                    string[] arrColumns = new string[countL * 2 + 4];

                    for (int i = 0; i < arrColumns.Length; i++)
                    {
                        arrColumns[i] = (i + 1).ToString();
                    }

                    List<NPOI.SS.Util.CellRangeAddress> regions = new List<NPOI.SS.Util.CellRangeAddress>();
                    #region 表头
                    DataTable dt = Code.Common.ArrayToDataTable(arrColumns);

                    string[] weekArr = new string[countL * 2 + 3];
                    string[] weekIdArr = new string[countL * 2 + 3];
                    string[] periodArr = new string[countL * 2 + 3];

                    for (int i = 0; i < countL; i++)
                    {
                        for (int j = 0; j < countR; j++)
                        {
                            if (j == 0)
                            {
                                weekArr[i * countR + j + 3] = PerformItemList[i].PerformItemName;
                                weekIdArr[i * countR + j + 3] = PerformItemList[i].Id.ToString();
                                periodArr[i * countR + j + 3] = "满分值：" + PerformItemList[i].ScoreMax.ToString();
                            }
                            else
                            {
                                weekArr[i * countR + j + 3] = PerformItemList[i].PerformItemName;
                                weekIdArr[i * countR + j + 3] = PerformItemList[i].Id.ToString();
                                periodArr[i * countR + j + 3] = "比例：" + PerformItemList[i].Rate.ToString() + "%";
                            }
                        }
                        regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 1, i * countR + 3, (i + 2) * countR));
                    }
                    dt.Rows.Add(weekIdArr);
                    dt.Rows.Add(weekArr);
                    dt.Rows.Add(periodArr);
                    #endregion

                    for (int i = 0; i < PerformDataList.Count; i++)
                    {
                        string[] tempArr = new string[countL * 2 + 4];
                        tempArr[0] = PerformDataList[i].No;
                        tempArr[1] = PerformDataList[i].StudentCode;
                        tempArr[2] = PerformDataList[i].StudentName;
                        for (int j = 3; j < dt.Columns.Count - 1; j++)
                        {
                            var scodeL = PerformDataAllList.Where(d => d.StudentId == PerformDataList[i].StudentId && d.PerformItemId == Convert.ToInt32(dt.Rows[0][j]) && d.CourseId == PerformDataList[i].CourseId);
                            if (scodeL.Count() > 0)
                            {
                                tempArr[j] = scodeL.FirstOrDefault().Score.ToString();
                            }
                            else
                            {
                                tempArr[j] = "";
                            }
                        }
                        for (int n = 0; n < countL; n++)
                        {
                            regions.Add(new NPOI.SS.Util.CellRangeAddress(i + 2 + 1, i + 2 + 1, n * countR + 3, (n + 2) * countR));
                        }

                        var totalScode = PerformTotalList.Where(d => d.PerformId == PerformId && d.CourseId == PerformDataList[i].CourseId && d.StudentId == PerformDataList[i].StudentId);
                        if (totalScode.Count() > 0)
                        {
                            tempArr[tempArr.Length - 1] = totalScode.FirstOrDefault().TotalScore.ToString();
                        }
                        else
                        {
                            tempArr[tempArr.Length - 1] = "";
                        }
                        dt.Rows.Add(tempArr);
                    }
                    dt.Rows.RemoveAt(0);

                    dt.Rows[0][0] = "座位号";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 0, 0));
                    dt.Rows[0][1] = "学号";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 1, 1));
                    dt.Rows[0][2] = "姓名";
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, 2, 2));
                    dt.Rows[0][(countL * 2 + 4) - 1] = "评价总分";
                    var s = countL * 2 + 4 - 1;
                    regions.Add(new NPOI.SS.Util.CellRangeAddress(1, 2, s, s));

                    Code.NpoiHelper.DataTableToExcel(file, dt, false, regions, "评价数据");
                }

                return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
            }
        }

        /// <summary>
        /// 评价查看
        /// </summary>
        /// <returns></returns>
        public ActionResult PerformDataAll()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformData.PerformDataAll();

                vm.PerformList = Perform.Controllers.PerformController.SelectList();

                vm.SubjectList = Areas.Course.Controllers.SubjectController.SelectList();

                if (vm.PerformId == 0 & vm.PerformList.Count > 0)
                {
                    vm.PerformId = vm.PerformList.FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.SubjectId == 0 & vm.SubjectList.Count > 0)
                {
                    vm.SubjectId = vm.SubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SubjectList.Insert(0, new SelectListItem { Text = "==全部==", Value = "-1" });

                var tbCourseId = (from p in db.Table<Course.Entity.tbCourse>()
                                  where (p.tbSubject.Id == vm.SubjectId || vm.SubjectId == -1)
                                  select p.Id).ToList();

                var tbPerformGroupIds = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                         where tbCourseId.Contains(p.tbCourse.Id)
                                         && p.tbPerformGroup.tbPerform.Id == vm.PerformId
                                         && p.tbCourse.IsDeleted == false
                                         && p.tbPerformGroup.IsDeleted == false
                                         select p.tbPerformGroup.Id).ToList();


                vm.PerformItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                      where (tbPerformGroupIds.Contains(p.tbPerformGroup.Id)
                                      && p.tbPerformGroup.tbPerform.Id == vm.PerformId)
                                      && p.tbPerformGroup.IsDeleted == false
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.PerformItemName,
                                          Value = p.Id.ToString()
                                      }
                                    ).ToList();

                var year = (from p in db.Table<Perform.Entity.tbPerform>()
                            where p.Id == vm.PerformId
                            select new Dto.Perform.Edit
                            {
                                Id = p.Id,
                                YearId = p.tbYear.Id,
                            }).ToList().FirstOrDefault();

                if (year == null)
                {
                    return View(vm);
                }

                var tbOrgIds = (from p in db.Table<Course.Entity.tbOrg>()
                                where p.tbYear.Id == year.YearId
                                && tbCourseId.Contains(p.tbCourse.Id)
                                select p.Id).ToList();

                vm.OrgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                     where tbOrgIds.Contains(p.tbOrg.Id)
                                     && p.tbOrg.IsDeleted == false
                                     && p.tbTeacher.IsDeleted == false
                                     orderby p.No
                                     select new Areas.Course.Dto.OrgTeacher.List
                                     {
                                         Id = p.tbOrg.Id,
                                         OrgName = p.tbOrg.OrgName,
                                         TeacherName = p.tbTeacher.TeacherName
                                     }).ToList();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    vm.OrgTeacherList = (from p in vm.OrgTeacherList
                                         where p.OrgName.Contains(vm.SearchText)
                                         select p).ToList();
                }
                var tbOrgList = (from p in db.Table<Course.Entity.tbOrg>()
                                 .Include(d => d.tbClass)
                                 .Include(d => d.tbCourse)
                                 where tbOrgIds.Contains(p.Id)
                                 select p).ToList();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbOrgList = (from p in tbOrgList
                                 where p.OrgName.Contains(vm.SearchText)
                                 select p).ToList();
                }

                var OrgStudentList = new List<Course.Dto.OrgStudent.List>();

                foreach (var org in tbOrgList)
                {
                    if (org.IsClass)
                    {
                        var tb = from p in db.Table<Basis.Entity.tbClassStudent>()
                                 where p.tbClass.Id == org.tbClass.Id
                                 && p.tbStudent.IsDeleted == false
                                 && p.tbClass.IsDeleted == false
                                 select p;

                        var student = (from p in tb
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Course.Dto.OrgStudent.List
                                       {
                                           Id = org.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           ClassName = p.tbClass.ClassName,
                                           No = org.tbCourse.Id
                                       }).ToList();
                        if (student != null && student.Count > 0)
                        {
                            OrgStudentList.AddRange(student);
                        }
                    }
                    else
                    {
                        var tb = from p in db.Table<Course.Entity.tbOrgStudent>()
                                 where p.tbOrg.Id == org.Id
                                 && p.tbStudent.IsDeleted == false
                                 && p.tbOrg.IsDeleted == false
                                 select p;

                        var student = (from p in tb
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Course.Dto.OrgStudent.List
                                       {
                                           Id = org.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           ClassName = p.tbOrg.tbClass.ClassName,
                                           No = org.tbCourse.Id
                                       }).ToList();
                        if (student != null && student.Count > 0)
                        {
                            OrgStudentList.AddRange(student);
                        }
                    }
                }

                if (OrgStudentList.Count() > 0)
                {
                    var tbPerformDataInfo = (from p in db.Table<Perform.Entity.tbPerformData>()
                                             where p.tbPerformItem.tbPerformGroup.tbPerform.Id == vm.PerformId
                                             && p.tbPerformItem.IsDeleted == false
                                             && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                             && tbCourseId.Contains(p.tbCourse.Id)
                                             && p.tbStudent.IsDeleted == false
                                             && p.tbCourse.IsDeleted == false
                                             select new Dto.PerformData.List
                                             {
                                                 StudentId = p.tbStudent.Id,
                                                 CourseId = p.tbCourse.Id,
                                                 PerformItemId = p.tbPerformItem.Id
                                             }).ToList();
                    var fff = new List<Dto.PerformData.Info>();
                    foreach (var c in tbPerformDataInfo)
                    {
                        var ff = (from p in OrgStudentList
                                  where p.StudentId == c.StudentId && p.No == c.CourseId
                                  select p.Id).ToList().FirstOrDefault();
                        var dd = new Dto.PerformData.Info();
                        dd.OrgId = ff;
                        dd.PerformItemId = c.PerformItemId;
                        fff.Add(dd);
                    }
                    var gList = from p in fff
                                group p by new { p.OrgId, p.PerformItemId } into g
                                select new
                                {
                                    ItemKey = g.Key,
                                    CountNum = g.Count()
                                };

                    var item = new List<Dto.PerformData.Info>();
                    foreach (var a in gList)
                    {
                        var orgCount = (from p in OrgStudentList
                                        where p.Id == a.ItemKey.OrgId
                                        select p).Count();
                        if (orgCount > 0)
                        {
                            var dd = new Dto.PerformData.Info();
                            dd.OrgId = a.ItemKey.OrgId;
                            dd.PerformItemId = a.ItemKey.PerformItemId;
                            dd.ScoreRate = ((decimal)a.CountNum / orgCount * 100).ToString();
                            item.Add(dd);
                        }
                    }

                    vm.PerformDataInfoList = item;
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PerformDataAll(XkSystem.Areas.Perform.Models.PerformData.PerformDataAll vm)
        {
            var error = new List<string>();
            return Code.MvcHelper.Post(error, Url.Action("PerformDataAll", new
            {
                searchText = vm.SearchText,
                PerformId = vm.PerformId,
                SubjectId = vm.SubjectId
            }));
        }

        public ActionResult PerformDataAllExport(int PerformId, int SubjectId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var tbCourseId = (from p in db.Table<Course.Entity.tbCourse>()
                                  where (p.tbSubject.Id == SubjectId || SubjectId == -1)
                                  select p.Id).ToList();

                var tbPerformGroupIds = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                         where tbCourseId.Contains(p.tbCourse.Id)
                                         && p.tbPerformGroup.tbPerform.Id == PerformId
                                         && p.tbCourse.IsDeleted == false
                                         && p.tbPerformGroup.IsDeleted == false
                                         select p.tbPerformGroup.Id).ToList();

                var PerformItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                       where (tbPerformGroupIds.Contains(p.tbPerformGroup.Id)
                                       && p.tbPerformGroup.tbPerform.Id == PerformId)
                                       && p.tbPerformGroup.IsDeleted == false
                                       orderby p.No
                                       select new System.Web.Mvc.SelectListItem
                                       {
                                           Text = p.PerformItemName,
                                           Value = p.Id.ToString()
                                       }
                                    ).ToList();

                var year = (from p in db.Table<Perform.Entity.tbPerform>()
                            where p.Id == PerformId
                            select new Dto.Perform.Edit
                            {
                                Id = p.Id,
                                YearId = p.tbYear.Id,
                            }).ToList().FirstOrDefault();

                var tbOrgIds = (from p in db.Table<Course.Entity.tbOrg>()
                                where p.tbYear.Id == year.YearId
                                && tbCourseId.Contains(p.tbCourse.Id)
                                select p.Id).ToList();

                var OrgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                      where tbOrgIds.Contains(p.tbOrg.Id)
                                      && p.tbOrg.IsDeleted == false
                                     && p.tbTeacher.IsDeleted == false
                                      orderby p.No
                                      select new Areas.Course.Dto.OrgTeacher.List
                                      {
                                          Id = p.tbOrg.Id,
                                          OrgName = p.tbOrg.OrgName,
                                          TeacherName = p.tbTeacher.TeacherName
                                      }).ToList();

                var tbOrgStudent = from p in db.Table<Course.Entity.tbOrgStudent>()
                                    .Include(d => d.tbOrg)
                                    .Include(d => d.tbStudent)
                                   where tbOrgIds.Contains(p.tbOrg.Id)
                                   && p.tbStudent.IsDeleted == false
                                   && p.tbOrg.IsDeleted == false
                                   select p;

                var tbOrgList = (from p in db.Table<Course.Entity.tbOrg>()
                                 .Include(d => d.tbClass)
                                 .Include(d => d.tbCourse)
                                 where tbOrgIds.Contains(p.Id)
                                 select p).ToList();

                var OrgStudentList = new List<Course.Dto.OrgStudent.List>();

                foreach (var org in tbOrgList)
                {
                    if (org.IsClass)
                    {
                        var tb = from p in db.Table<Basis.Entity.tbClassStudent>()
                                 where p.tbClass.Id == org.tbClass.Id
                                 && p.tbStudent.IsDeleted == false
                                 && p.tbClass.IsDeleted == false
                                 select p;

                        var student = (from p in tb
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Course.Dto.OrgStudent.List
                                       {
                                           Id = org.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           ClassName = p.tbClass.ClassName,
                                           No = org.tbCourse.Id
                                       }).ToList();
                        if (student != null && student.Count > 0)
                        {
                            OrgStudentList.AddRange(student);
                        }
                    }
                    else
                    {
                        var tb = from p in db.Table<Course.Entity.tbOrgStudent>()
                                 where p.tbOrg.Id == org.Id
                                 && p.tbStudent.IsDeleted == false
                                 && p.tbOrg.IsDeleted == false
                                 select p;

                        var student = (from p in tb
                                       orderby p.No, p.tbStudent.StudentCode
                                       select new Course.Dto.OrgStudent.List
                                       {
                                           Id = org.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentName = p.tbStudent.StudentName,
                                           StudentCode = p.tbStudent.StudentCode,
                                           ClassName = p.tbOrg.tbClass.ClassName,
                                           No = org.tbCourse.Id
                                       }).ToList();
                        if (student != null && student.Count > 0)
                        {
                            OrgStudentList.AddRange(student);
                        }
                    }
                }

                if (OrgStudentList.Count() > 0)
                {
                    var tbPerformDataInfo = (from p in db.Table<Perform.Entity.tbPerformData>()
                                             where p.tbPerformItem.tbPerformGroup.tbPerform.Id == PerformId
                                             && p.tbPerformItem.IsDeleted == false
                                             && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                             && tbCourseId.Contains(p.tbCourse.Id)
                                             && p.tbStudent.IsDeleted == false
                                             && p.tbCourse.IsDeleted == false
                                             select new Dto.PerformData.List
                                             {
                                                 StudentId = p.tbStudent.Id,
                                                 CourseId = p.tbCourse.Id,
                                                 PerformItemId = p.tbPerformItem.Id
                                             }).ToList();
                    var fff = new List<Dto.PerformData.Info>();
                    foreach (var c in tbPerformDataInfo)
                    {
                        var ff = (from p in OrgStudentList
                                  where p.StudentId == c.StudentId && p.No == c.CourseId
                                  select p.Id).ToList().FirstOrDefault();
                        var dd = new Dto.PerformData.Info();
                        dd.OrgId = ff;
                        dd.PerformItemId = c.PerformItemId;
                        fff.Add(dd);
                    }
                    var gList = from p in fff
                                group p by new { p.OrgId, p.PerformItemId } into g
                                select new
                                {
                                    ItemKey = g.Key,
                                    CountNum = g.Count()
                                };

                    var PerformDataInfoList = new List<Dto.PerformData.Info>();
                    foreach (var a in gList)
                    {
                        var orgCount = (from p in OrgStudentList
                                        where p.Id == a.ItemKey.OrgId
                                        select p).Count();
                        if (orgCount > 0)
                        {
                            var dd = new Dto.PerformData.Info();
                            dd.OrgId = a.ItemKey.OrgId;
                            dd.PerformItemId = a.ItemKey.PerformItemId;
                            dd.ScoreRate = ((decimal)a.CountNum / orgCount * 100).ToString();
                            PerformDataInfoList.Add(dd);
                        }
                    }
                    var dt = new System.Data.DataTable();
                    dt.Columns.AddRange(new System.Data.DataColumn[]
                        {
                        new System.Data.DataColumn("教学班"),
                        new System.Data.DataColumn("教师")
                        });

                    foreach (var a in PerformItemList)
                    {
                        dt.Columns.AddRange(new System.Data.DataColumn[]
                        {
                        new System.Data.DataColumn(a.Text)
                        });
                    }

                    foreach (var a in OrgTeacherList)
                    {
                        var dr = dt.NewRow();
                        dr["教学班"] = a.OrgName;
                        dr["教师"] = a.TeacherName;
                        foreach (var item in PerformItemList)
                        {
                            var schedule = PerformDataInfoList.Where(d => d.OrgId == a.Id && d.PerformItemId.ToString() == item.Value);
                            if (schedule.Count() > 0)
                            {
                                dr[item.Text] = Decimal.Round(schedule.FirstOrDefault().ScoreRate.ConvertToDecimal(), 2) + "%";
                            }
                            else
                            {
                                dr[item.Text] = "0%";
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                    Code.NpoiHelper.DataTableToExcel(file, dt);
                }

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

        /// <summary>
        /// 评价查看
        /// </summary>
        /// <returns></returns>
        public ActionResult PerformDataDetail()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformData.PerformDataDetail();

                var tbOrg = (from p in db.Table<Course.Entity.tbOrg>()
                            .Include(d => d.tbClass)
                            .Include(d => d.tbYear)
                            .Include(d => d.tbCourse)
                             where p.Id == vm.OrgId
                             select p).FirstOrDefault();
                if (tbOrg != null)
                {
                    var tbCourseId = (from p in db.Table<Course.Entity.tbCourse>()
                                      where (p.tbSubject.Id == vm.SubjectId || vm.SubjectId == -1)
                                      select p.Id).ToList();

                    var tbPerformGroupIds = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                             where tbCourseId.Contains(p.tbCourse.Id)
                                             && p.tbCourse.Id == tbOrg.tbCourse.Id
                                             && p.tbPerformGroup.tbPerform.Id == vm.PerformId
                                             && p.tbCourse.IsDeleted == false
                                             && p.tbPerformGroup.IsDeleted == false
                                             select p.tbPerformGroup.Id).ToList();


                    vm.PerformItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                          where (tbPerformGroupIds.Contains(p.tbPerformGroup.Id)
                                          && p.tbPerformGroup.tbPerform.Id == vm.PerformId)
                                          && p.tbPerformGroup.IsDeleted == false
                                          orderby p.No
                                          select new System.Web.Mvc.SelectListItem
                                          {
                                              Text = p.PerformItemName,
                                              Value = p.Id.ToString()
                                          }
                                        ).ToList();

                    var itemIds = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                   where (tbPerformGroupIds.Contains(p.tbPerformGroup.Id)
                                   && p.tbPerformGroup.tbPerform.Id == vm.PerformId)
                                   && p.tbPerformGroup.IsDeleted == false
                                   select p.Id).ToList();

                    if (tbOrg.IsClass)
                    {
                        if (tbOrg.tbClass != null)
                        {
                            vm.OrgStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                                 where p.tbClass.Id == tbOrg.tbClass.Id
                                                 && p.tbStudent.IsDeleted == false
                                                 && p.tbClass.IsDeleted == false
                                                 orderby p.No, p.tbStudent.StudentCode
                                                 select new Areas.Course.Dto.OrgStudent.List
                                                 {
                                                     Id = p.tbStudent.Id,
                                                     No = p.No,
                                                     StudentCode = p.tbStudent.StudentCode,
                                                     StudentName = p.tbStudent.StudentName
                                                 }).ToList();
                        }
                    }
                    else
                    {
                        vm.OrgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                             where p.tbOrg.Id == vm.OrgId
                                             && p.tbStudent.IsDeleted == false
                                             && p.tbOrg.IsDeleted == false
                                             orderby p.No, p.tbStudent.StudentCode
                                             select new Areas.Course.Dto.OrgStudent.List
                                             {
                                                 Id = p.tbStudent.Id,
                                                 No = p.No,
                                                 StudentCode = p.tbStudent.StudentCode,
                                                 StudentName = p.tbStudent.StudentName
                                             }).ToList();
                    }

                    if (string.IsNullOrEmpty(vm.SearchText) == false)
                    {
                        vm.OrgStudentList = (from p in vm.OrgStudentList
                                             where (p.StudentCode.Contains(vm.SearchText) || p.StudentName.Contains(vm.SearchText))
                                             select p).ToList();
                    }

                    vm.PerformDataDetailList = (from p in db.Table<Perform.Entity.tbPerformData>()
                                                where itemIds.Contains(p.tbPerformItem.Id)
                                                && tbCourseId.Contains(p.tbCourse.Id)
                                                && p.tbStudent.IsDeleted == false
                                                && p.tbCourse.IsDeleted == false
                                                select new Dto.PerformData.Detail
                                                {
                                                    StudentId = p.tbStudent.Id.ToString(),
                                                    Score = p.Score,
                                                    Id = p.tbPerformItem.Id
                                                }).ToList();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PerformDataDetail(XkSystem.Areas.Perform.Models.PerformData.PerformDataDetail vm)
        {
            var error = new List<string>();
            return Code.MvcHelper.Post(error, Url.Action("PerformDataDetail", new
            {
                OrgId = vm.OrgId,
                PerformId = vm.PerformId,
                SubjectId = vm.SubjectId
            }));
        }

        #region 导入数据
        /// <summary>
        /// 准备导入
        /// </summary>
        /// <param name="PerformId"></param>
        /// <param name="ClassId"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public ActionResult Import(int PerformId, int ClassId, List<string> error = null)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformData.Import();
                if (error != null && error.Count > decimal.Zero)
                {
                    ModelState.AddModelError("", error[0]);
                }
                return View(vm);
            }
        }

        /// <summary>
        /// 执行导入
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.PerformData.Import vm)
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
                                var tbList = new List<string>() { "座位号", "学号", "姓名" };

                                var tbOrg = (from p in db.Table<Course.Entity.tbOrg>()
                                                            .Include(d => d.tbClass)
                                                            .Include(d => d.tbYear)
                                                            .Include(d => d.tbCourse)
                                             where p.Id == vm.ClassId
                                             select p).FirstOrDefault();

                                var tbPerformGroupIds = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                                         where p.tbCourse.Id == tbOrg.tbCourse.Id
                                                         && p.tbPerformGroup.tbPerform.Id == vm.PerformId
                                                         && p.tbPerformGroup.IsDeleted == false
                                                         && p.tbCourse.IsDeleted == false
                                                         select p.tbPerformGroup.Id).ToList();


                                var PerformItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                                       where tbPerformGroupIds.Contains(p.tbPerformGroup.Id)
                                                       && p.tbPerformGroup.tbPerform.Id == vm.PerformId
                                                       && p.tbPerformGroup.IsDeleted == false
                                                       && p.IsMany == false
                                                       && p.IsSelect == false
                                                       orderby p.No
                                                       select new Dto.PerformItem.List
                                                       {
                                                           Id = p.Id,
                                                           Rate = p.Rate,
                                                           ScoreMax = p.ScoreMax,
                                                           PerformItemName = p.PerformItemName
                                                       }
                                                    ).ToList();

                                foreach (var a in PerformItemList)
                                {
                                    if (tbList.Contains(a.PerformItemName + "[Max:" + a.ScoreMax + "|Rate:" + a.Rate + "%]") == false)
                                    {
                                        tbList.Add(a.PerformItemName + "[Max:" + a.ScoreMax + "|Rate:" + a.Rate + "%]");
                                    }
                                }

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
                                    error.AddError("上传的EXCEL内容与预期不一致!缺少字段:" + Text);
                                }

                                //1、开始执行导入逻辑和校验逻辑

                                var orgStudentList = new List<Dto.PerformData.List>();
                                if (tbOrg != null)
                                {

                                    if (tbOrg.IsClass)
                                    {
                                        if (tbOrg.tbClass != null)
                                        {
                                            orgStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                                              where p.tbClass.Id == tbOrg.tbClass.Id
                                                              && p.tbStudent.IsDeleted == false
                                                              && p.tbClass.IsDeleted == false
                                                              orderby p.No, p.tbStudent.StudentCode
                                                              select new Dto.PerformData.List
                                                              {
                                                                  No = p.No.ToString(),
                                                                  StudentId = p.tbStudent.Id,
                                                                  StudentCode = p.tbStudent.StudentCode,
                                                                  StudentName = p.tbStudent.StudentName,
                                                                  CourseId = tbOrg.tbCourse.Id,
                                                              }).ToList();
                                        }
                                    }
                                    else
                                    {
                                        orgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                                          where p.tbOrg.Id == vm.ClassId
                                                          && p.tbStudent.IsDeleted == false
                                                          && p.tbOrg.IsDeleted == false
                                                          orderby p.No, p.tbStudent.StudentCode
                                                          select new Dto.PerformData.List
                                                          {
                                                              No = p.No.ToString(),
                                                              StudentId = p.tbStudent.Id,
                                                              StudentCode = p.tbStudent.StudentCode,
                                                              StudentName = p.tbStudent.StudentName,
                                                              CourseId = tbOrg.tbCourse.Id
                                                          }).ToList();
                                    }

                                    var performDataList = from p in db.Table<Perform.Entity.tbPerformData>()
                                                          where p.tbPerformItem.tbPerformGroup.tbPerform.Id == vm.PerformId
                                                          && p.tbPerformItem.IsDeleted == false
                                                          && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                                          && p.tbCourse.Id == tbOrg.tbCourse.Id
                                                          && p.tbStudent.IsDeleted == false
                                                          && p.tbCourse.IsDeleted == false
                                                          && p.tbPerformItem.IsMany == false
                                                          && p.tbPerformItem.IsSelect == false
                                                          select p;

                                    var PerformTotalList = (from p in db.Table<Perform.Entity.tbPerformTotal>()
                                                            .Include(d => d.tbCourse)
                                                            .Include(d => d.tbPerform)
                                                            .Include(d => d.tbStudent)
                                                            where p.tbPerform.Id == vm.PerformId
                                                            && p.tbCourse.Id == tbOrg.tbCourse.Id
                                                            && p.tbStudent.IsDeleted == false
                                                            && p.tbCourse.IsDeleted == false
                                                            && p.tbPerform.IsDeleted == false
                                                            select p).ToList();

                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        var studentCode = dr["学号"].ToString().Trim();
                                        var studentName = dr["姓名"].ToString().Trim();
                                        if (string.IsNullOrEmpty(studentCode) || string.IsNullOrEmpty(studentName))
                                        {
                                            continue;
                                        }
                                        if (orgStudentList.Where(d => d.StudentCode == studentCode && d.StudentName == studentName).Count() == decimal.Zero)
                                        {
                                            continue;
                                        }
                                        //总分
                                        decimal totalScode = decimal.Zero;
                                        foreach (DataColumn dc in dt.Columns)
                                        {
                                            if (PerformItemList.Where(d => (d.PerformItemName + "[Max:" + d.ScoreMax + "|Rate:" + d.Rate + "%]") == dc.ColumnName).Count() == decimal.Zero)
                                            {
                                                continue;
                                            }
                                            var performItem = PerformItemList.Where(d => (d.PerformItemName + "[Max:" + d.ScoreMax + "|Rate:" + d.Rate + "%]") == dc.ColumnName).FirstOrDefault();
                                            var studentScode = dr[performItem.PerformItemName + "[Max:" + performItem.ScoreMax + "|Rate:" + performItem.Rate + "%]"].ToString().Trim().ConvertToDecimal();//分数
                                            studentScode = studentScode * 1;
                                            if (studentScode > performItem.ScoreMax.ConvertToDecimal())
                                            {
                                                var strmes = string.Format("学生:{1}的【{3}】项目分数:【{2}】超过满分值:{0}", performItem.ScoreMax, studentCode, studentScode, performItem.PerformItemName);
                                                error.AddError(strmes);
                                                continue;
                                            }
                                            var tb = (from p in performDataList
                                                      where p.tbStudent.StudentCode == studentCode
                                                       && p.tbPerformItem.Id == performItem.Id
                                                       && p.tbCourse.Id == tbOrg.tbCourse.Id
                                                      select p).FirstOrDefault();
                                            if (tb == null)
                                            {
                                                var tf = new Perform.Entity.tbPerformData();
                                                tf.tbStudent = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == studentCode).FirstOrDefault();
                                                var courseId = tbOrg.tbCourse.Id;
                                                tf.Score = studentScode;
                                                tf.InputDate = System.DateTime.Now;
                                                tf.tbCourse = db.Set<Course.Entity.tbCourse>().Find(courseId);
                                                tf.tbPerformItem = db.Set<Perform.Entity.tbPerformItem>().Find(performItem.Id);
                                                tf.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                                                db.Set<Perform.Entity.tbPerformData>().Add(tf);
                                                totalScode += studentScode * performItem.Rate / 100;//计算总分
                                            }
                                            else
                                            {
                                                tb.Score = studentScode;
                                                totalScode += studentScode * performItem.Rate / 100;//计算总分
                                            }
                                        }

                                        var studentId = db.Table<Student.Entity.tbStudent>().Where(d => d.StudentCode == studentCode).FirstOrDefault().Id;
                                        //总分导入
                                        if (string.IsNullOrEmpty(studentId.ToString()) == false)
                                        {
                                            var tfTotal = PerformTotalList.Where(d => d.tbPerform.Id == vm.PerformId && d.tbCourse.Id == tbOrg.tbCourse.Id && d.tbStudent.Id == studentId);
                                            if (tfTotal.Count() > 0)
                                            {
                                                var tfTotalScode = tfTotal.FirstOrDefault();
                                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量修改评价总分数据");
                                                tfTotalScode.TotalScore = Math.Round(totalScode, 2, MidpointRounding.AwayFromZero);
                                            }
                                            else
                                            {
                                                var tfTotalScode = new Perform.Entity.tbPerformTotal();
                                                tfTotalScode.TotalScore = Math.Round(totalScode, 2, MidpointRounding.AwayFromZero);
                                                tfTotalScode.tbPerform = db.Set<Perform.Entity.tbPerform>().Find(vm.PerformId);
                                                tfTotalScode.tbStudent = db.Set<Student.Entity.tbStudent>().Find(studentId);
                                                var courseId = tbOrg.tbCourse.Id;
                                                tfTotalScode.tbCourse = db.Set<Course.Entity.tbCourse>().Find(courseId);
                                                db.Set<Perform.Entity.tbPerformTotal>().Add(tfTotalScode);
                                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量添加评价总分数据");
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }

                                    if (db.SaveChanges() > decimal.Zero)
                                    {
                                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量添加学生评分");
                                        vm.Status = true;
                                    }
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
            if (error != null && error.Count > decimal.Zero)
            {
                foreach (var msg in error)
                {
                    ModelState.AddModelError("", msg);
                }
                vm.Status = false;
            }
            return View(vm);
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        /// <param name="PerformId"></param>
        /// <param name="ClassId"></param>
        /// <returns></returns>
        public ActionResult ImportTemplate(int PerformId, int ClassId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var tbOrg = (from p in db.Table<Course.Entity.tbOrg>()
                            .Include(d => d.tbClass)
                            .Include(d => d.tbYear)
                            .Include(d => d.tbCourse)
                             where p.Id == ClassId
                             select p).FirstOrDefault();

                if (tbOrg != null)
                {
                    var tbPerformGroupIds = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                             where p.tbCourse.Id == tbOrg.tbCourse.Id
                                             && p.tbPerformGroup.tbPerform.Id == PerformId
                                             && p.tbPerformGroup.IsDeleted == false
                                             && p.tbCourse.IsDeleted == false
                                             select p.tbPerformGroup.Id).ToList();


                    var PerformItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                           where tbPerformGroupIds.Contains(p.tbPerformGroup.Id)
                                           && p.tbPerformGroup.tbPerform.Id == PerformId
                                           && p.tbPerformGroup.IsDeleted == false
                                           && p.IsMany == false
                                           && p.IsSelect == false
                                           orderby p.No
                                           select new Dto.PerformItem.List
                                           {
                                               Id = p.Id,
                                               Rate = p.Rate,
                                               ScoreMax = p.ScoreMax,
                                               PerformItemName = p.PerformItemName
                                           }
                                        ).ToList();

                    var PerformDataAllList = (from p in db.Table<Perform.Entity.tbPerformData>()
                                              where p.tbPerformItem.tbPerformGroup.tbPerform.Id == PerformId
                                              && p.tbPerformItem.IsDeleted == false
                                              && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                              && p.tbCourse.Id == tbOrg.tbCourse.Id
                                              && p.tbStudent.IsDeleted == false
                                              && p.tbCourse.IsDeleted == false
                                              && p.tbPerformItem.IsMany == false
                                              && p.tbPerformItem.IsSelect == false
                                              select new Dto.PerformData.List
                                              {
                                                  PerformItemId = p.tbPerformItem.Id,
                                                  Score = p.Score,
                                                  StudentId = p.tbStudent.Id,
                                                  CourseId = p.tbCourse.Id
                                              }).ToList();

                    var orgStudentList = new List<Dto.PerformData.List>();
                    if (tbOrg.IsClass)
                    {
                        if (tbOrg.tbClass != null)
                        {
                            orgStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                              where p.tbClass.Id == tbOrg.tbClass.Id
                                              && p.tbStudent.IsDeleted == false
                                              && p.tbClass.IsDeleted == false
                                              orderby p.No, p.tbStudent.StudentCode
                                              select new Dto.PerformData.List
                                              {
                                                  No = p.No.ToString(),
                                                  StudentId = p.tbStudent.Id,
                                                  StudentCode = p.tbStudent.StudentCode,
                                                  StudentName = p.tbStudent.StudentName,
                                                  CourseId = tbOrg.tbCourse.Id,
                                              }).ToList();
                        }
                    }
                    else
                    {
                        orgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                          where p.tbOrg.Id == ClassId
                                          && p.tbStudent.IsDeleted == false
                                          && p.tbOrg.IsDeleted == false
                                          orderby p.No, p.tbStudent.StudentCode
                                          select new Dto.PerformData.List
                                          {
                                              No = p.No.ToString(),
                                              StudentId = p.tbStudent.Id,
                                              StudentCode = p.tbStudent.StudentCode,
                                              StudentName = p.tbStudent.StudentName,
                                              CourseId = tbOrg.tbCourse.Id
                                          }).ToList();
                    }
                    var dt = new System.Data.DataTable();
                    dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("座位号"),
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名")
                    });

                    foreach (var a in PerformItemList)
                    {
                        dt.Columns.AddRange(new System.Data.DataColumn[]
                        {
                            new System.Data.DataColumn(a.PerformItemName+"[Max:"+a.ScoreMax+"|Rate:"+a.Rate+"%]")
                        });
                    }

                    foreach (var a in orgStudentList)
                    {
                        var dr = dt.NewRow();
                        dr["座位号"] = a.No;
                        dr["学号"] = a.StudentCode;
                        dr["姓名"] = a.StudentName;
                        foreach (var item in PerformItemList)
                        {
                            var scodeL = PerformDataAllList.Where(d => d.StudentId == a.StudentId && d.PerformItemId == item.Id && d.CourseId == a.CourseId);
                            if (scodeL.Count() > 0)
                            {
                                dr[item.PerformItemName + "[Max:" + item.ScoreMax + "|Rate:" + item.Rate + "%]"] = scodeL.FirstOrDefault().Score;
                            }
                            else
                            {
                                dr[item.PerformItemName + "[Max:" + item.ScoreMax + "|Rate:" + item.Rate + "%]"] = "";
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                    Code.NpoiHelper.DataTableToExcel(file, dt);
                }
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
        #endregion

        public ActionResult PerformDataStudentAll()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformData.PerformDataStudentAll();

                vm.PerformList = Perform.Controllers.PerformController.SelectList();
                vm.SubjectList = Areas.Course.Controllers.SubjectController.SelectList();

                if (vm.PerformId == 0 & vm.PerformList.Count > 0)
                {
                    vm.PerformId = vm.PerformList.FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.SubjectId == 0 & vm.SubjectList.Count > 0)
                {
                    vm.SubjectId = vm.SubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.ClassList = Areas.Course.Controllers.OrgController.SelectOrgList(vm.SubjectId);

                if (vm.ClassList.Count > 0 && vm.ClassId > 0)
                {
                    if (vm.ClassList.Where(d => d.Value.ConvertToInt() == vm.ClassId).Count() == decimal.Zero)
                    {
                        vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                    }
                }

                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.ClassList.Count() == decimal.Zero)
                {
                    vm.ClassId = -1;
                }

                vm.ClassList.Insert(0, new SelectListItem { Text = "==班级==", Value = "-1" });

                var tbOrg = (from p in db.Table<Course.Entity.tbOrg>()
                            .Include(d => d.tbClass)
                            .Include(d => d.tbYear)
                            .Include(d => d.tbCourse)
                             where p.Id == vm.ClassId
                             select p).FirstOrDefault();

                if (tbOrg != null)
                {
                    var tbCourseIds = (from p in db.Table<Course.Entity.tbCourse>()
                                       where (p.tbSubject.Id == vm.SubjectId || vm.SubjectId == -1)
                                       && p.tbSubject.IsDeleted == false
                                       select p.Id).ToList();

                    var tbPerformGroupIds = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                             where tbCourseIds.Contains(p.tbCourse.Id)
                                             && p.tbCourse.Id == tbOrg.tbCourse.Id
                                             && p.tbPerformGroup.tbPerform.Id == vm.PerformId
                                             && p.tbCourse.IsDeleted == false
                                             && p.tbPerformGroup.IsDeleted == false
                                             select p.tbPerformGroup.Id).ToList();

                    vm.PerformItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                          where (tbPerformGroupIds.Contains(p.tbPerformGroup.Id)
                                          && p.tbPerformGroup.tbPerform.Id == vm.PerformId)
                                          && p.tbPerformGroup.IsDeleted == false
                                          orderby p.No
                                          select new System.Web.Mvc.SelectListItem
                                          {
                                              Text = p.PerformItemName,
                                              Value = p.Id.ToString()
                                          }
                                        ).ToList();

                    vm.PerformDataStudentAllList = (from p in db.Table<Perform.Entity.tbPerformData>()
                                                    where p.tbPerformItem.tbPerformGroup.tbPerform.Id == vm.PerformId
                                                    && p.tbPerformItem.IsDeleted == false
                                                    && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                                    && tbCourseIds.Contains(p.tbCourse.Id)
                                                    && p.tbStudent.IsDeleted == false
                                                    && p.tbCourse.IsDeleted == false
                                                    select new Dto.PerformData.StudentAll
                                                    {
                                                        PerformItemId = p.tbPerformItem.Id,
                                                        Score = p.Score,
                                                        StudentId = p.tbStudent.Id,
                                                        CourseId = p.tbCourse.Id
                                                    }).ToList();

                    var orgStudentList = new List<Dto.PerformData.List>();
                    if (tbOrg.IsClass)
                    {
                        if (tbOrg.tbClass != null)
                        {
                            orgStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                              where p.tbClass.Id == tbOrg.tbClass.Id
                                              && p.tbStudent.IsDeleted == false
                                              && p.tbClass.IsDeleted == false
                                              orderby p.No, p.tbStudent.StudentCode
                                              select new Dto.PerformData.List
                                              {
                                                  No = p.No.ToString(),
                                                  StudentId = p.tbStudent.Id,
                                                  StudentCode = p.tbStudent.StudentCode,
                                                  StudentName = p.tbStudent.StudentName,
                                                  CourseId = tbOrg.tbCourse.Id,
                                                  CourseName = tbOrg.tbCourse.CourseName
                                              }).ToList();
                        }
                    }
                    else
                    {
                        orgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                          where p.tbOrg.Id == vm.ClassId
                                          && p.tbStudent.IsDeleted == false
                                          && p.tbOrg.IsDeleted == false
                                          orderby p.No, p.tbStudent.StudentCode
                                          select new Dto.PerformData.List
                                          {
                                              No = p.No.ToString(),
                                              StudentId = p.tbStudent.Id,
                                              StudentCode = p.tbStudent.StudentCode,
                                              StudentName = p.tbStudent.StudentName,
                                              CourseId = tbOrg.tbCourse.Id,
                                              CourseName = tbOrg.tbCourse.CourseName
                                          }).ToList();
                    }

                    if (string.IsNullOrEmpty(vm.SearchText) == false)
                    {
                        orgStudentList = (from p in orgStudentList
                                          where (p.StudentCode.Contains(vm.SearchText) || p.StudentName.Contains(vm.SearchText))
                                          select p).ToList();
                    }

                    vm.PerformDataList = (from p in orgStudentList
                                          select new Dto.PerformData.List
                                          {
                                              No = p.No != null ? p.No.ToString() : string.Empty,
                                              StudentCode = p.StudentCode,
                                              StudentName = p.StudentName,
                                              StudentId = p.StudentId,
                                              CourseId = p.CourseId
                                          }).ToList();
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PerformDataStudentAll(XkSystem.Areas.Perform.Models.PerformData.PerformDataStudentAll vm)
        {
            var error = new List<string>();
            return Code.MvcHelper.Post(error, Url.Action("PerformDataStudentAll", new
            {
                SearchText = vm.SearchText,
                PerformId = vm.PerformId,
                SubjectId = vm.SubjectId,
                ClassId = vm.ClassId
            }));
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="PerformId"></param>
        /// <param name="SubjectId"></param>
        /// <param name="ClassId"></param>
        /// <returns></returns>
        public ActionResult PerformDataStudentAllExport(int PerformId, int SubjectId, int ClassId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var PerformList = Perform.Controllers.PerformController.SelectList();
                var SubjectList = Areas.Course.Controllers.SubjectController.SelectList();
                var ClassList = Areas.Course.Controllers.OrgController.SelectList();

                if (PerformId == 0 & PerformList.Count > 0)
                {
                    PerformId = PerformList.FirstOrDefault().Value.ConvertToInt();
                }

                if (SubjectId == 0 & SubjectList.Count > 0)
                {
                    SubjectId = SubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                if (ClassId == 0 & ClassList.Count > 0)
                {
                    ClassId = ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                SubjectList.Insert(0, new SelectListItem { Text = "==全部==", Value = "-1" });

                ClassList.Insert(0, new SelectListItem { Text = "==全部==", Value = "-1" });

                var tbOrg = (from p in db.Table<Course.Entity.tbOrg>()
                            .Include(d => d.tbClass)
                            .Include(d => d.tbYear)
                            .Include(d => d.tbCourse)
                             where p.Id == ClassId
                             select p).FirstOrDefault();

                if (tbOrg != null)
                {
                    var tbCourseIds = (from p in db.Table<Course.Entity.tbCourse>()
                                       where (p.tbSubject.Id == SubjectId || SubjectId == -1)
                                       && p.tbSubject.IsDeleted == false
                                       select p.Id).ToList();

                    var tbPerformGroupIds = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                             where tbCourseIds.Contains(p.tbCourse.Id) && p.tbCourse.Id == tbOrg.tbCourse.Id
                                             && p.tbPerformGroup.tbPerform.Id == PerformId
                                             && p.tbCourse.IsDeleted == false
                                             && p.tbPerformGroup.IsDeleted == false
                                             select p.tbPerformGroup.Id).ToList();

                    var PerformItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                           where (tbPerformGroupIds.Contains(p.tbPerformGroup.Id)
                                           && p.tbPerformGroup.tbPerform.Id == PerformId)
                                           && p.tbPerformGroup.IsDeleted == false
                                           orderby p.No
                                           select new System.Web.Mvc.SelectListItem
                                           {
                                               Text = p.PerformItemName,
                                               Value = p.Id.ToString()
                                           }
                                        ).ToList();

                    var PerformDataStudentAllList = (from p in db.Table<Perform.Entity.tbPerformData>()
                                                     where p.tbPerformItem.tbPerformGroup.tbPerform.Id == PerformId
                                                     && p.tbPerformItem.IsDeleted == false
                                                     && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                                     && tbCourseIds.Contains(p.tbCourse.Id)
                                                     && p.tbStudent.IsDeleted == false
                                                     && p.tbCourse.IsDeleted == false
                                                     select new Dto.PerformData.StudentAll
                                                     {
                                                         PerformItemId = p.tbPerformItem.Id,
                                                         Score = p.Score,
                                                         StudentId = p.tbStudent.Id,
                                                         CourseId = p.tbCourse.Id
                                                     }).ToList();

                    var orgStudentList = new List<Dto.PerformData.List>();
                    if (tbOrg.IsClass)
                    {
                        if (tbOrg.tbClass != null)
                        {
                            orgStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                              where p.tbClass.Id == tbOrg.tbClass.Id
                                              && p.tbStudent.IsDeleted == false
                                              && p.tbClass.IsDeleted == false
                                              orderby p.No, p.tbStudent.StudentCode
                                              select new Dto.PerformData.List
                                              {
                                                  No = p.No.ToString(),
                                                  StudentId = p.tbStudent.Id,
                                                  StudentCode = p.tbStudent.StudentCode,
                                                  StudentName = p.tbStudent.StudentName,
                                                  CourseId = tbOrg.tbCourse.Id,
                                                  CourseName = tbOrg.tbCourse.CourseName
                                              }).ToList();
                        }
                    }
                    else
                    {
                        orgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                          where p.tbOrg.Id == ClassId
                                          && p.tbStudent.IsDeleted == false
                                          && p.tbOrg.IsDeleted == false
                                          orderby p.No, p.tbStudent.StudentCode
                                          select new Dto.PerformData.List
                                          {
                                              No = p.No.ToString(),
                                              StudentId = p.tbStudent.Id,
                                              StudentCode = p.tbStudent.StudentCode,
                                              StudentName = p.tbStudent.StudentName,
                                              CourseId = tbOrg.tbCourse.Id,
                                              CourseName = tbOrg.tbCourse.CourseName
                                          }).ToList();
                    }
                    var PerformDataList = (from p in orgStudentList
                                           select new Dto.PerformData.List
                                           {
                                               No = p.No.ToString(),
                                               StudentCode = p.StudentCode.ToString(),
                                               StudentName = p.StudentName.ToString(),
                                               StudentId = p.StudentId,
                                               CourseId = p.CourseId
                                           }).ToList();
                    var dt = new System.Data.DataTable();
                    dt.Columns.AddRange(new System.Data.DataColumn[]
                        {
                        new System.Data.DataColumn("座位号"),
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名")
                        });

                    foreach (var a in PerformItemList)
                    {
                        dt.Columns.AddRange(new System.Data.DataColumn[]
                        {
                        new System.Data.DataColumn(a.Text)
                        });
                    }

                    foreach (var a in PerformDataList)
                    {
                        var dr = dt.NewRow();
                        dr["座位号"] = a.No;
                        dr["学号"] = a.StudentCode;
                        dr["姓名"] = a.StudentName;
                        foreach (var item in PerformItemList)
                        {
                            var schedule = PerformDataStudentAllList.Where(d => d.StudentId == a.StudentId && d.PerformItemId.ToString() == item.Value && d.CourseId == a.CourseId);
                            if (schedule.Count() > 0)
                            {
                                dr[item.Text] = schedule.Select(d => d.Score).Sum();
                            }
                            else
                            {
                                dr[item.Text] = "";
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                    Code.NpoiHelper.DataTableToExcel(file, dt);
                }
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

        public ActionResult PerformDataStudentDetail()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformData.PerformDataStudentDetail();

                var tbPerformIds = (from p in db.Table<Perform.Entity.tbPerformData>()
                                    where p.tbStudent.Id == vm.StudentId
                                    && p.tbStudent.IsDeleted == false
                                    && p.tbCourse.IsDeleted == false
                                    && p.tbPerformItem.IsDeleted == false
                                    && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                    && p.tbPerformItem.tbPerformGroup.tbPerform.IsDeleted == false
                                    select p.tbPerformItem.tbPerformGroup.tbPerform.Id).Distinct().ToList();

                var tbCourseIds = (from p in db.Table<Course.Entity.tbCourse>()
                                   where p.tbSubject.IsDeleted == false
                                   && p.tbSubject.Id == vm.SubjectId
                                   select p.Id).Distinct().ToList();

                var tbPerformGroupIds = (from p in db.Table<Perform.Entity.tbPerformGroup>()
                                         where tbPerformIds.Contains(p.tbPerform.Id)
                                         && p.tbPerform.IsDeleted == false
                                         select p.Id).Distinct().ToList();

                vm.PerformDataStudentDetailList = (from p in db.Table<Perform.Entity.tbPerformData>()
                                                      .Include(d => d.tbCourse)
                                                      .Include(d => d.tbPerformItem)
                                                      .Include(d => d.tbStudent)
                                                   where p.tbStudent.Id == vm.StudentId
                                                   && p.tbStudent.IsDeleted == false
                                                   && p.tbCourse.IsDeleted == false
                                                   && p.tbPerformItem.IsDeleted == false
                                                   && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                                   && p.tbPerformItem.tbPerformGroup.tbPerform.IsDeleted == false
                                                   && tbCourseIds.Contains(p.tbCourse.Id)
                                                   select new Dto.PerformData.StudentDetail
                                                   {
                                                       PerformItemId = p.tbPerformItem.Id,
                                                       StudentCode = p.tbStudent.StudentCode,
                                                       StudentId = p.tbStudent.Id,
                                                       StudentName = p.tbStudent.StudentName,
                                                       Score = p.Score
                                                   }).ToList();

                vm.PerformList = (from p in db.Table<Perform.Entity.tbPerform>()
                                  where tbPerformIds.Contains(p.Id)
                                  orderby p.No
                                  select new Dto.Perform.List
                                  {
                                      Id = p.Id,
                                      PerformName = p.PerformName
                                  }).Distinct().ToList();

                vm.PerformItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                      where tbPerformIds.Contains(p.tbPerformGroup.tbPerform.Id)
                                      && p.tbPerformGroup.IsDeleted == false
                                      && tbPerformGroupIds.Contains(p.tbPerformGroup.Id)
                                      orderby p.No
                                      select new Dto.PerformItem.List
                                      {
                                          Id = p.Id,
                                          PerformItemName = p.PerformItemName,
                                          No = p.tbPerformGroup.tbPerform.Id,
                                          ScoreMax = p.ScoreMax,
                                          Rate = p.Rate
                                      }
                        ).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PerformDataStudentDetail(XkSystem.Areas.Perform.Models.PerformData.PerformDataStudentDetail vm)
        {
            var error = new List<string>();
            return Code.MvcHelper.Post(error, Url.Action("PerformDataStudentDetail", new
            {
                PerformId = vm.PerformId,
                SubjectId = vm.SubjectId,
                ClassId = vm.ClassId,
                StudentId = vm.StudentId
            }));
        }

        public ActionResult My()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformData.My();
                //vm.PerformList = Perform.Controllers.PerformController.SelectList();
                if (vm.PerformId == 0 & vm.PerformList.Count > 0)
                {
                    //vm.PerformId = vm.PerformList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SubjectList = Areas.Course.Controllers.SubjectController.SelectList();
                if (vm.SubjectId == 0 && vm.SubjectList.Count > 0)
                {
                    vm.SubjectId = vm.SubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                var tbPerformIds = (from p in db.Table<Perform.Entity.tbPerformData>()
                                    where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                    && p.tbStudent.IsDeleted == false
                                    && p.tbCourse.IsDeleted == false
                                    && p.tbPerformItem.IsDeleted == false
                                    && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                    && p.tbPerformItem.tbPerformGroup.tbPerform.IsDeleted == false
                                    select p.tbPerformItem.tbPerformGroup.tbPerform.Id).Distinct().ToList();

                var tbCourseIds = (from p in db.Table<Course.Entity.tbCourse>()
                                   where p.tbSubject.IsDeleted == false
                                   && p.tbSubject.Id == vm.SubjectId
                                   select p.Id).Distinct().ToList();

                var tbPerformGroupIds = (from p in db.Table<Perform.Entity.tbPerformGroup>()
                                         where tbPerformIds.Contains(p.tbPerform.Id)
                                         && p.tbPerform.IsDeleted == false
                                         select p.Id).Distinct().ToList();

                vm.PerformDataStudentDetailList = (from p in db.Table<Perform.Entity.tbPerformData>()
                                                      .Include(d => d.tbCourse)
                                                      .Include(d => d.tbPerformItem)
                                                      .Include(d => d.tbStudent)
                                                   where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                                   && p.tbStudent.IsDeleted == false
                                                   && p.tbCourse.IsDeleted == false
                                                   && p.tbPerformItem.IsDeleted == false
                                                   && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                                   && p.tbPerformItem.tbPerformGroup.tbPerform.IsDeleted == false
                                                   && tbCourseIds.Contains(p.tbCourse.Id)
                                                   select new Dto.PerformData.StudentDetail
                                                   {
                                                       PerformId = p.tbPerformItem.tbPerformGroup.tbPerform.Id,
                                                       PerformItemId = p.tbPerformItem.Id,
                                                       StudentCode = p.tbStudent.StudentCode,
                                                       StudentId = p.tbStudent.Id,
                                                       StudentName = p.tbStudent.StudentName,
                                                       CourseName = p.tbCourse.CourseName,
                                                       Score = p.Score
                                                   }).ToList();

                vm.PerformList = (from p in db.Table<Perform.Entity.tbPerform>()
                                  where tbPerformIds.Contains(p.Id)
                                  orderby p.No
                                  select new Dto.Perform.List
                                  {
                                      Id = p.Id,
                                      PerformName = p.PerformName
                                  }).Distinct().ToList();

                vm.PerformItemList = (from p in db.Table<Perform.Entity.tbPerformItem>()
                                      where tbPerformIds.Contains(p.tbPerformGroup.tbPerform.Id)
                                      && p.tbPerformGroup.IsDeleted == false
                                      && tbPerformGroupIds.Contains(p.tbPerformGroup.Id)
                                      orderby p.No
                                      select new Dto.PerformItem.List
                                      {
                                          Id = p.Id,
                                          PerformItemName = p.PerformItemName,
                                          No = p.tbPerformGroup.tbPerform.Id,
                                          ScoreMax = p.ScoreMax,
                                          Rate = p.Rate
                                      }
                        ).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult My(XkSystem.Areas.Perform.Models.PerformData.My vm)
        {
            var error = new List<string>();
            return Code.MvcHelper.Post(error, Url.Action("PerformDataStudentDetail", new
            {
                PerformId = vm.PerformId,
                SubjectId = vm.SubjectId,
                ClassId = vm.ClassId
            }));
        }
    }
}