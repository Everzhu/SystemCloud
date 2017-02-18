using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Perform.Controllers
{
    public class PerformChangeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PerformChange.List();

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

                vm.OrgSelectInfo = SelectOrgSelectInfo(db, vm.ClassList, vm.PerformId);

                var tbOrg = (from p in db.Table<Course.Entity.tbOrg>()
                            .Include(d => d.tbClass)
                            .Include(d => d.tbYear)
                            .Include(d => d.tbCourse)
                             where p.Id == vm.ClassId
                             select p).FirstOrDefault();

                if (tbOrg != null)
                {
                    var tbPerformGroupIds = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                             where p.tbCourse.Id == tbOrg.tbCourse.Id
                                             && p.tbPerformGroup.tbPerform.Id == vm.PerformId
                                             && p.tbCourse.IsDeleted == false
                                             && p.tbPerformGroup.IsDeleted == false
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


                    vm.PerformChangeDataList = (from p in db.Table<Perform.Entity.tbPerformData>()
                                                where p.tbPerformItem.tbPerformGroup.tbPerform.Id == vm.PerformId
                                                && p.tbPerformItem.IsDeleted == false
                                                && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                                && p.tbStudent.IsDeleted == false
                                                && p.tbCourse.IsDeleted == false
                                                && p.tbPerformItem.IsMany == false
                                                && p.tbPerformItem.IsSelect == false
                                                select new Dto.PerformChange.List
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
                    vm.PerformChangeList = (from p in orgStudentList
                                            select new Dto.PerformChange.List
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
                    tb = (from p in db.Table<Course.Entity.tbOrg>()
                          where tbCourseIds.Contains(p.tbCourse.Id)
                             && p.tbYear.Id == year.YearId
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.OrgName,
                              Value = p.Id.ToString()
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
        public ActionResult List(Models.PerformChange.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, performId = vm.PerformId, classId = vm.ClassId }));
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
                                         && p.tbCourse.IsDeleted == false
                                         && p.tbPerformGroup.IsDeleted == false
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

                var list = (from p in db.Table<Perform.Entity.tbPerformData>()
                            where p.tbPerformItem.tbPerformGroup.tbPerform.Id == vm.PerformId
                            && p.tbPerformItem.IsDeleted == false
                            && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                            && p.tbStudent.IsDeleted == false
                            && p.tbCourse.IsDeleted == false
                            && p.tbPerformItem.IsMany == false
                            && p.tbPerformItem.IsSelect == false
                            select p).ToList();

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
                                var tf = list.Where(d => d.Id == txtId[i].ConvertToInt()).FirstOrDefault();
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价数据");
                                tf.IsDeleted = true;
                            }
                        }
                        else
                        {
                            var courseid = (from p in db.Table<Course.Entity.tbOrg>().Where(d => d.Id == vm.ClassId)
                                            select p.tbCourse.Id).ToList().FirstOrDefault();
                            var studengid = txtStudentId[i].ConvertToInt();
                            var performitemscode = txtPerformItemScode[i].ConvertToDecimal();
                            //输入内容不为空，判断是否存在id并执行对应的操作
                            var countList = (from p in db.Table<Perform.Entity.tbPerformData>()
                                             where p.tbStudent.Id == studengid &&
                                             p.tbCourse.Id == courseid &&
                                             p.tbPerformItem.Id == item.Id
                                             select p).ToList();
                            if (countList.Count() > 0)
                            {
                                //如果有id的，执行更新操作
                                var tf = countList.FirstOrDefault();
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价数据");
                                tf.InputDate = DateTime.Now;
                                tf.Score = txtPerformItemScode[i].ConvertToDecimal();
                                if (string.IsNullOrEmpty(txtPerformItemScode[i].Trim()))
                                {
                                    tf.Score = txtPerformItemScode[i].ConvertToDecimal();
                                    tf.IsDeleted = true;
                                }
                                tf.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(txtPerformItemScode[i].Trim()))
                                {
                                    //没有id的，执行插入操作
                                    var tf = new Perform.Entity.tbPerformData();
                                    tf.InputDate = DateTime.Now;
                                    tf.Score = txtPerformItemScode[i].ConvertToDecimal();
                                    tf.tbStudent = db.Set<Student.Entity.tbStudent>().Find(txtStudentId[i].ConvertToInt());
                                    tf.tbCourse = db.Set<Course.Entity.tbCourse>().Find(courseid);
                                    tf.tbPerformItem = db.Set<Perform.Entity.tbPerformItem>().Find(item.Id);
                                    tf.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                                    db.Set<Perform.Entity.tbPerformData>().Add(tf);
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价数据");
                                }
                            }
                        }
                    }
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

                studentSumScore = (from p in studentSumScore
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

                if (tbOrg == null)
                {
                    return View(vm);
                }
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
                                          select new Dto.PerformData.List
                                          {
                                              PerformItemId = p.tbPerformItem.Id,
                                              Score = p.Score,
                                              StudentId = p.tbStudent.Id,
                                              CourseId = p.tbCourse.Id
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

        public static List<Dto.PerformData.OrgSelectInfo> SelectOrgSelectInfo(XkSystem.Models.DbContext db, List<System.Web.Mvc.SelectListItem> orgSelectList, int performId)
        {
            var orgSelectInfoList = new List<Dto.PerformData.OrgSelectInfo>();
            if (performId == 0)
            {
                return orgSelectInfoList;
            }
            var tbPerform = (from p in db.Table<Perform.Entity.tbPerform>()
                             join m in db.Table<Perform.Entity.tbPerformGroup>() on p.Id equals m.tbPerform.Id
                             join n in db.Table<Perform.Entity.tbPerformCourse>() on m.Id equals n.tbPerformGroup.Id
                             join l in db.Table<Perform.Entity.tbPerformItem>() on m.Id equals l.tbPerformGroup.Id
                             where m.tbPerform.IsDeleted == false
                             && n.tbPerformGroup.IsDeleted == false
                             && l.tbPerformGroup.IsDeleted == false
                             where p.Id == performId
                             select new
                             {
                                 PerformId = p.Id,
                                 PerformGroupId = m.Id,
                                 CourseId = n.tbCourse.Id,
                                 PerformItemId = l.Id
                             }).Distinct().ToList();

            var tbPerformData = (from p in db.Table<Perform.Entity.tbPerformData>()
                                 where p.tbPerformItem.tbPerformGroup.tbPerform.Id == performId
                                 && p.tbPerformItem.IsDeleted == false
                                 && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                                 && p.tbCourse.IsDeleted == false
                                 && p.tbStudent.IsDeleted == false
                                 select new
                                 {
                                     CourseId = p.tbCourse.Id,
                                     PerformItemId = p.tbPerformItem.Id,
                                     StudentId = p.tbStudent.Id
                                 }).Distinct().ToList();

            foreach (var org in orgSelectList)
            {
                var orgId = org.Value.ConvertToInt();
                var orgSelectInfo = new Dto.PerformData.OrgSelectInfo();
                var tbOrg = (from p in db.Table<Course.Entity.tbOrg>()
                               .Include(d => d.tbClass)
                               .Include(d => d.tbCourse)
                             where p.Id == orgId
                             select p).ToList().FirstOrDefault();

                if (tbOrg != null)
                {
                    var orgStudentList = new List<Dto.PerformData.List>();
                    #region 加载学生
                    //行政班级
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
                                                  StudentId = p.tbStudent.Id,
                                                  StudentCode = p.tbStudent.StudentCode,
                                                  CourseId = tbOrg.tbCourse.Id,
                                              }).ToList();
                        }
                    }
                    else//教学班级
                    {
                        orgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                          where p.tbOrg.Id == tbOrg.Id
                                          && p.tbStudent.IsDeleted == false
                                          && p.tbOrg.IsDeleted == false
                                          orderby p.No, p.tbStudent.StudentCode
                                          select new Dto.PerformData.List
                                          {
                                              StudentId = p.tbStudent.Id,
                                              StudentCode = p.tbStudent.StudentCode,
                                              CourseId = tbOrg.tbCourse.Id
                                          }).ToList();
                    }
                    #endregion
                    if (orgStudentList != null)
                    {
                        orgSelectInfo.OrgId = tbOrg.Id;
                        orgSelectInfo.OrgName = tbOrg.OrgName;
                        orgSelectInfo.No = tbOrg.No;
                        orgSelectInfo.SumCount = orgStudentList.Distinct().Count();//赋值总人数

                        foreach (var student in orgStudentList)
                        {
                            var perform = (from p in tbPerform
                                           where p.CourseId == student.CourseId
                                           select p).Distinct().ToList();
                            var i = 0;
                            foreach (var p in perform)
                            {
                                if (tbPerformData.Where(d => d.StudentId == student.StudentId && d.CourseId == p.CourseId && d.PerformItemId == p.PerformItemId).Count() > 0)
                                {
                                    i++;
                                }
                            }
                            if (i > 0 && i == perform.Count())
                            {
                                orgSelectInfo.Count += 1;
                            }
                        }
                        orgSelectInfoList.Add(orgSelectInfo);
                    }
                }
            }
            return orgSelectInfoList;
        }
    }
}