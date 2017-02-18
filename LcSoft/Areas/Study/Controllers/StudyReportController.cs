using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Study.Controllers
{
    public class StudyReportController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyReport.List();

                vm.StudyList = Controllers.StudyController.SelectList();
                vm.StudyList.Insert(0, new SelectListItem { Text = "==全部==", Value = "0" });

                var dateTimeNow = DateTime.Now;

                if (string.IsNullOrEmpty(vm.DateSearchFrom))
                {
                    vm.DateSearchFrom = new DateTime(dateTimeNow.Year, dateTimeNow.Month, 1).ToString(Code.Common.StringToDate);
                }

                if (string.IsNullOrEmpty(vm.DateSearchTo))
                {
                    vm.DateSearchTo = new DateTime(dateTimeNow.Year, dateTimeNow.Month, 1).AddMonths(1).AddDays(-1).ToString(Code.Common.StringToDate);
                }

                var fromDate = Convert.ToDateTime(vm.DateSearchFrom);

                var toDate = Convert.ToDateTime(vm.DateSearchTo).AddDays(1);

                var tbStudyDataList = from p in db.Table<Study.Entity.tbStudyData>()
                                      .Include(d => d.tbStudy)
                                      .Include(d => d.tbStudent)
                                      .Include(d => d.tbStudyOption)
                                      where p.InputDate >= fromDate && p.InputDate < toDate
                                      && p.tbStudent.IsDeleted == false
                                      && p.tbStudyOption.IsDeleted == false
                                      && p.tbSysUser.IsDeleted == false
                                      select p;

                var studyIds = (from p in tbStudyDataList
                                select p.tbStudy.Id).Distinct().ToList();

                var tbStudy = from p in db.Table<Study.Entity.tbStudy>()
                              where studyIds.Contains(p.Id)
                              select p;

                if (vm.StudyId != 0)
                {
                    tbStudy = tbStudy.Where(d => d.Id == vm.StudyId);
                }

                var tbStudyList = (from p in tbStudy
                                   orderby p.No descending
                                   select p).ToList();

                var classList = new List<Dto.StudyClassStudent.List>();

                foreach (var study in tbStudyList)
                {
                    if (study.IsRoom)//教室模式
                    {
                        var tb = from p in db.Table<Study.Entity.tbStudyRoomStudent>()
                                 where p.tbStudy.Id == study.Id
                                 && p.tbStudy.IsDeleted == false
                                 && p.tbRoom.IsDeleted == false
                                 && p.tbStudent.IsDeleted == false
                                 select p;

                        if (string.IsNullOrEmpty(vm.SearchText) == false)
                        {
                            tb = tb.Where(d => d.tbRoom.RoomName.Contains(vm.SearchText));
                        }

                        var tbClassList = (from p in tb
                                           select new Dto.StudyClassStudent.List
                                           {
                                               ClassId = p.tbRoom.Id,
                                               ClassName = p.tbRoom.RoomName,
                                               StudentId = p.tbStudent.Id,
                                               StudentCode = p.tbStudent.StudentCode,
                                               StudentName = p.tbStudent.StudentName,
                                               StudyId = p.tbStudy.Id,
                                               StudyName = p.tbStudy.StudyName
                                           }).ToList();

                        classList.AddRange(tbClassList);
                    }
                    else//班级模式
                    {
                        var tb = from p in db.Table<Study.Entity.tbStudyClassStudent>()
                                 where p.tbStudy.Id == study.Id
                                 && p.tbStudy.IsDeleted == false
                                 && p.tbClass.IsDeleted == false
                                 && p.tbStudent.IsDeleted == false
                                 select p;

                        if (string.IsNullOrEmpty(vm.SearchText) == false)
                        {
                            tb = tb.Where(d => d.tbClass.ClassName.Contains(vm.SearchText));
                        }

                        var tbClassList = (from p in tb
                                           select new Dto.StudyClassStudent.List
                                           {
                                               ClassId = p.tbClass.Id,
                                               ClassName = p.tbClass.ClassName,
                                               StudentId = p.tbStudent.Id,
                                               StudentCode = p.tbStudent.StudentCode,
                                               StudentName = p.tbStudent.StudentName,
                                               StudyId = p.tbStudy.Id,
                                               StudyName = p.tbStudy.StudyName
                                           }).ToList();

                        classList.AddRange(tbClassList);
                    }
                }

                vm.StudyReportList = (from p in classList
                                      group p by new { p.StudyId, p.StudyName, p.ClassId, p.ClassName } into g
                                      select new Dto.StudyReport.List
                                      {
                                          StudyId = g.Key.StudyId,
                                          StudyName = g.Key.StudyName,
                                          ClassId = g.Key.ClassId,
                                          ClassName = g.Key.ClassName,
                                          StudentCount = 0
                                      }).ToList();

                var StudyDataList = (from p in tbStudyDataList
                                     select new Dto.StudyData.List
                                     {
                                         StudyId = p.tbStudy.Id,
                                         StudentId = p.tbStudent.Id,
                                         ClassId = 0
                                     }).Distinct().ToList();

                foreach (var item in StudyDataList)
                {

                    var studyData = classList.Where(d => d.StudyId == item.StudyId && d.StudentId == item.StudentId).FirstOrDefault();
                    if (studyData != null)
                    {
                        item.ClassId = studyData.ClassId;
                    }
                }
                var StudyDataGroupList = (from p in StudyDataList
                                          group p by new { p.StudyId, p.ClassId } into g
                                          select new
                                          {
                                              StudyId = g.Key.StudyId,
                                              ClassId = g.Key.ClassId,
                                              StudentCount = g.Count()
                                          }).ToList();
                foreach (var item in vm.StudyReportList)
                {
                    item.StudentCount = StudyDataGroupList.Where(d => d.StudyId == item.StudyId && d.ClassId == item.ClassId).Select(d => d.StudentCount).DefaultIfEmpty().FirstOrDefault();
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudyReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                studyId = vm.StudyId,
                dateSearchFrom = vm.DateSearchFrom,
                dateSearchTo = vm.DateSearchTo
            }));
        }

        public ActionResult FullList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyReport.FullList();

                var fromDate = Convert.ToDateTime(vm.DateSearchFrom);
                var toDate = Convert.ToDateTime(vm.DateSearchTo).AddDays(1);

                var tbStudyDataList = (from p in db.Table<Study.Entity.tbStudyData>()
                                      .Include(d => d.tbStudy)
                                      .Include(d => d.tbStudent)
                                      .Include(d => d.tbStudyOption)
                                       where p.InputDate >= fromDate && p.InputDate < toDate
                                       && p.tbStudy.Id == vm.StudyId
                                       && p.tbStudent.IsDeleted == false
                                       && p.tbStudyOption.IsDeleted == false
                                       && p.tbSysUser.IsDeleted == false
                                       select new Dto.StudyData.List
                                       {
                                           Id = p.tbStudy.Id,
                                           StudyId = p.tbStudy.Id,
                                           StudyName = p.tbStudy.StudyName,
                                           StudentId = p.tbStudent.Id,
                                           StudentCode = p.tbStudent.StudentCode,
                                           StudentName = p.tbStudent.StudentName,
                                           StudyOptionName = p.tbStudyOption.StudyOptionName,
                                           Remark = p.Remark,
                                           SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                           InputDate = p.InputDate,
                                           ClassId = 0,
                                           ClassName = "",
                                           SysUserName = p.tbSysUser.UserName
                                       }).ToList();


                var tbStudy = (from p in db.Table<Study.Entity.tbStudy>()
                               where p.Id == vm.StudyId
                               && p.tbYear.IsDeleted == false
                               select p).FirstOrDefault();

                var classList = new List<Dto.StudyClassStudent.List>();
                if (tbStudy.IsRoom)//教室模式
                {
                    var tbClassList = (from p in db.Table<Study.Entity.tbStudyRoomStudent>()
                                       where p.tbStudy.Id == tbStudy.Id
                                       && p.tbRoom.Id == vm.ClassId
                                       && p.tbStudy.IsDeleted == false
                                       && p.tbRoom.IsDeleted == false
                                       && p.tbStudent.IsDeleted == false
                                       select new Dto.StudyClassStudent.List
                                       {
                                           ClassId = p.tbRoom.Id,
                                           ClassName = p.tbRoom.RoomName,
                                           StudentId = p.tbStudent.Id,
                                           StudentCode = p.tbStudent.StudentCode,
                                           StudentName = p.tbStudent.StudentName,
                                           StudyId = p.tbStudy.Id,
                                           StudyName = p.tbStudy.StudyName
                                       }).ToList();
                    classList.AddRange(tbClassList);
                }
                else//班级模式
                {
                    var tbClassList = (from p in db.Table<Study.Entity.tbStudyClassStudent>()
                                       where p.tbStudy.Id == tbStudy.Id
                                       && p.tbClass.Id == vm.ClassId
                                       && p.tbStudy.IsDeleted == false
                                       && p.tbClass.IsDeleted == false
                                       && p.tbStudent.IsDeleted == false
                                       select new Dto.StudyClassStudent.List
                                       {
                                           ClassId = p.tbClass.Id,
                                           ClassName = p.tbClass.ClassName,
                                           StudentId = p.tbStudent.Id,
                                           StudentCode = p.tbStudent.StudentCode,
                                           StudentName = p.tbStudent.StudentName,
                                           StudyId = p.tbStudy.Id,
                                           StudyName = p.tbStudy.StudyName
                                       }).ToList();

                    classList.AddRange(tbClassList);
                }
                foreach (var item in tbStudyDataList)
                {
                    var data = classList.Where(d => d.StudyId == item.StudyId && d.StudentId == item.StudentId).FirstOrDefault();
                    if (data != null)
                    {
                        item.ClassId = data.ClassId;
                        item.ClassName = data.ClassName;
                    }
                }
                vm.StudyReportFullList = (from p in tbStudyDataList
                                          where p.ClassId != 0
                                          select new Dto.StudyReport.FullList
                                          {
                                              StudyName = p.StudyName,
                                              ClassName = p.ClassName,
                                              InputDate = p.InputDate,
                                              Remark = p.Remark,
                                              SysUserName = p.SysUserName,
                                              SexName = p.SexName,
                                              StudentCode = p.StudentCode,
                                              StudentName = p.StudentName,
                                              StudyOptionName = p.StudyOptionName
                                          }).ToList();
                return View(vm);
            }
        }

        public ActionResult Detail()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyReport.Detail();

                var dateTimeNow = DateTime.Now;

                if (string.IsNullOrEmpty(vm.DateSearchFrom))
                {
                    vm.DateSearchFrom = new DateTime(dateTimeNow.Year, dateTimeNow.Month, 1).ToString(Code.Common.StringToDate);
                }

                if (string.IsNullOrEmpty(vm.DateSearchTo))
                {
                    vm.DateSearchTo = new DateTime(dateTimeNow.Year, dateTimeNow.Month, 1).AddMonths(1).AddDays(-1).ToString(Code.Common.StringToDate);
                }

                var fromDate = Convert.ToDateTime(vm.DateSearchFrom);

                var toDate = Convert.ToDateTime(vm.DateSearchTo).AddDays(1);

                var tbStudyDataList = (from p in db.Table<Study.Entity.tbStudyData>()
                                      .Include(d => d.tbStudy)
                                      .Include(d => d.tbStudent)
                                      .Include(d => d.tbStudyOption)
                                       where p.InputDate >= fromDate && p.InputDate < toDate
                                       && p.tbStudent.IsDeleted == false
                                       && p.tbStudyOption.IsDeleted == false
                                       && p.tbSysUser.IsDeleted == false
                                       select new Dto.StudyData.List
                                       {
                                           StudyId = p.tbStudy.Id,
                                           StudyName = p.tbStudy.StudyName,
                                           StudentId = p.tbStudent.Id,
                                           StudentCode = p.tbStudent.StudentCode,
                                           StudentName = p.tbStudent.StudentName,
                                           StudyOptionName = p.tbStudyOption.StudyOptionName,
                                           Remark = p.Remark,
                                           SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                           InputDate = p.InputDate,
                                           ClassId = 0,
                                           ClassName = "",
                                           SysUserName = p.tbSysUser.UserName
                                       }).ToList();

                var studyIds = (from p in tbStudyDataList
                                select p.StudyId).Distinct().ToList();

                var tbStudyList = (from p in db.Table<Study.Entity.tbStudy>()
                                   where studyIds.Contains(p.Id)
                                   && p.tbYear.IsDeleted == false
                                   select p).ToList();

                var classList = new List<Dto.StudyClassStudent.List>();
                foreach (var tbStudy in tbStudyList)
                {
                    if (tbStudy.IsRoom)//教室模式
                    {
                        var tbClassList = (from p in db.Table<Study.Entity.tbStudyRoomStudent>()
                                           where p.tbStudy.Id == tbStudy.Id
                                           && p.tbStudy.IsDeleted == false
                                           && p.tbRoom.IsDeleted == false
                                           && p.tbStudent.IsDeleted == false
                                           select new Dto.StudyClassStudent.List
                                           {
                                               ClassId = p.tbRoom.Id,
                                               ClassName = p.tbRoom.RoomName,
                                               StudentId = p.tbStudent.Id,
                                               StudentCode = p.tbStudent.StudentCode,
                                               StudentName = p.tbStudent.StudentName,
                                               StudyId = p.tbStudy.Id,
                                               StudyName = p.tbStudy.StudyName
                                           }).ToList();
                        classList.AddRange(tbClassList);
                    }
                    else//班级模式
                    {
                        var tbClassList = (from p in db.Table<Study.Entity.tbStudyClassStudent>()
                                           where p.tbStudy.Id == tbStudy.Id
                                           && p.tbStudy.IsDeleted == false
                                           && p.tbClass.IsDeleted == false
                                           && p.tbStudent.IsDeleted == false
                                           select new Dto.StudyClassStudent.List
                                           {
                                               ClassId = p.tbClass.Id,
                                               ClassName = p.tbClass.ClassName,
                                               StudentId = p.tbStudent.Id,
                                               StudentCode = p.tbStudent.StudentCode,
                                               StudentName = p.tbStudent.StudentName,
                                               StudyId = p.tbStudy.Id,
                                               StudyName = p.tbStudy.StudyName
                                           }).ToList();

                        classList.AddRange(tbClassList);
                    }
                }
                foreach (var item in tbStudyDataList)
                {
                    var data = classList.Where(d => d.StudyId == item.StudyId && d.StudentId == item.StudentId).FirstOrDefault();
                    if (data != null)
                    {
                        item.ClassId = data.ClassId;
                        item.ClassName = data.ClassName;
                    }
                }
                vm.StudyReportDetailList = (from p in tbStudyDataList
                                            where p.ClassId != 0
                                            select new Dto.StudyReport.Detail
                                            {
                                                StudyName = p.StudyName,
                                                ClassName = p.ClassName,
                                                InputDate = p.InputDate,
                                                Remark = p.Remark,
                                                SysUserName = p.SysUserName,
                                                SexName = p.SexName,
                                                StudentCode = p.StudentCode,
                                                StudentName = p.StudentName,
                                                StudyOptionName = p.StudyOptionName
                                            }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detail(Models.StudyReport.Detail vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("Detail", new
            {
                searchText = vm.SearchText,
                studyId = vm.StudyId,
                dateSearchFrom = vm.DateSearchFrom,
                dateSearchTo = vm.DateSearchTo
            }));
        }

        public ActionResult ListExport()
        {
            var vm = new Models.StudyReport.List();

            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var fromDate = Convert.ToDateTime(vm.DateSearchFrom);

                var toDate = Convert.ToDateTime(vm.DateSearchTo).AddDays(1);

                var tbStudyDataList = from p in db.Table<Study.Entity.tbStudyData>()
                                      .Include(d => d.tbStudy)
                                      .Include(d => d.tbStudent)
                                      .Include(d => d.tbStudyOption)
                                      where p.InputDate >= fromDate && p.InputDate < toDate
                                      && p.tbStudent.IsDeleted == false
                                      && p.tbStudyOption.IsDeleted == false
                                      && p.tbSysUser.IsDeleted == false
                                      select p;

                var studyIds = (from p in tbStudyDataList
                                select p.tbStudy.Id).Distinct().ToList();

                var tbStudy = from p in db.Table<Study.Entity.tbStudy>()
                              where studyIds.Contains(p.Id)
                              select p;

                if (vm.StudyId != 0)
                {
                    tbStudy = tbStudy.Where(d => d.Id == vm.StudyId);
                }

                var tbStudyList = (from p in tbStudy
                                   orderby p.No descending
                                   select p).ToList();

                var classList = new List<Dto.StudyClassStudent.List>();

                foreach (var study in tbStudyList)
                {
                    if (study.IsRoom)//教室模式
                    {
                        var tb = from p in db.Table<Study.Entity.tbStudyRoomStudent>()
                                 where p.tbStudy.Id == study.Id
                                 && p.tbStudy.IsDeleted == false
                                 && p.tbRoom.IsDeleted == false
                                 && p.tbStudent.IsDeleted == false
                                 select p;

                        if (string.IsNullOrEmpty(vm.SearchText) == false)
                        {
                            tb = tb.Where(d => d.tbRoom.RoomName.Contains(vm.SearchText));
                        }

                        var tbClassList = (from p in tb
                                           select new Dto.StudyClassStudent.List
                                           {
                                               ClassId = p.tbRoom.Id,
                                               ClassName = p.tbRoom.RoomName,
                                               StudentId = p.tbStudent.Id,
                                               StudentCode = p.tbStudent.StudentCode,
                                               StudentName = p.tbStudent.StudentName,
                                               StudyId = p.tbStudy.Id,
                                               StudyName = p.tbStudy.StudyName
                                           }).ToList();

                        classList.AddRange(tbClassList);
                    }
                    else//班级模式
                    {
                        var tb = from p in db.Table<Study.Entity.tbStudyClassStudent>()
                                 where p.tbStudy.Id == study.Id
                                 && p.tbStudy.IsDeleted == false
                                 && p.tbClass.IsDeleted == false
                                 && p.tbStudent.IsDeleted == false
                                 select p;

                        if (string.IsNullOrEmpty(vm.SearchText) == false)
                        {
                            tb = tb.Where(d => d.tbClass.ClassName.Contains(vm.SearchText));
                        }

                        var tbClassList = (from p in tb
                                           select new Dto.StudyClassStudent.List
                                           {
                                               ClassId = p.tbClass.Id,
                                               ClassName = p.tbClass.ClassName,
                                               StudentId = p.tbStudent.Id,
                                               StudentCode = p.tbStudent.StudentCode,
                                               StudentName = p.tbStudent.StudentName,
                                               StudyId = p.tbStudy.Id,
                                               StudyName = p.tbStudy.StudyName
                                           }).ToList();

                        classList.AddRange(tbClassList);
                    }
                }

                vm.StudyReportList = (from p in classList
                                      group p by new { p.StudyId, p.StudyName, p.ClassId, p.ClassName } into g
                                      select new Dto.StudyReport.List
                                      {
                                          StudyId = g.Key.StudyId,
                                          StudyName = g.Key.StudyName,
                                          ClassId = g.Key.ClassId,
                                          ClassName = g.Key.ClassName,
                                          StudentCount = 0
                                      }).ToList();

                var StudyDataList = (from p in tbStudyDataList
                                     select new Dto.StudyData.List
                                     {
                                         StudyId = p.tbStudy.Id,
                                         StudentId = p.tbStudent.Id,
                                         ClassId = 0
                                     }).Distinct().ToList();

                foreach (var item in StudyDataList)
                {

                    var studyData = classList.Where(d => d.StudyId == item.StudyId && d.StudentId == item.StudentId).FirstOrDefault();
                    if (studyData != null)
                    {
                        item.ClassId = studyData.ClassId;
                    }
                }
                var StudyDataGroupList = (from p in StudyDataList
                                          group p by new { p.StudyId, p.ClassId } into g
                                          select new
                                          {
                                              StudyId = g.Key.StudyId,
                                              ClassId = g.Key.ClassId,
                                              StudentCount = g.Count()
                                          }).ToList();
                foreach (var item in vm.StudyReportList)
                {
                    item.StudentCount = StudyDataGroupList.Where(d => d.StudyId == item.StudyId && d.ClassId == item.ClassId).Select(d => d.StudentCount).DefaultIfEmpty().FirstOrDefault();
                }

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("序号"),
                        new System.Data.DataColumn("晚自习名称"),
                        new System.Data.DataColumn("班级"),
                        new System.Data.DataColumn("异常人数")
                    });
                var index = 0;
                foreach (var a in vm.StudyReportList)
                {
                    index++;
                    var dr = dt.NewRow();
                    dr["序号"] = index.ToString();
                    dr["晚自习名称"] = a.StudyName;
                    dr["班级"] = a.ClassName;
                    dr["异常人数"] = a.StudentCount;
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

        public ActionResult DetailExport()
        {
            var vm = new Models.StudyReport.Detail();

            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var fromDate = Convert.ToDateTime(vm.DateSearchFrom);
                var toDate = Convert.ToDateTime(vm.DateSearchTo).AddDays(1);

                var tbStudyDataList = (from p in db.Table<Study.Entity.tbStudyData>()
                                      .Include(d => d.tbStudy)
                                      .Include(d => d.tbStudent)
                                      .Include(d => d.tbStudyOption)
                                       where p.InputDate >= fromDate && p.InputDate < toDate
                                       && p.tbStudent.IsDeleted == false
                                       && p.tbStudyOption.IsDeleted == false
                                       && p.tbSysUser.IsDeleted == false
                                       select new Dto.StudyData.List
                                       {
                                           StudyId = p.tbStudy.Id,
                                           StudyName = p.tbStudy.StudyName,
                                           StudentId = p.tbStudent.Id,
                                           StudentCode = p.tbStudent.StudentCode,
                                           StudentName = p.tbStudent.StudentName,
                                           StudyOptionName = p.tbStudyOption.StudyOptionName,
                                           Remark = p.Remark,
                                           SexName = p.tbStudent.tbSysUser.tbSex.SexName,
                                           InputDate = p.InputDate,
                                           ClassId = 0,
                                           ClassName = "",
                                           SysUserName = p.tbSysUser.UserName
                                       }).ToList();

                var studyIds = (from p in tbStudyDataList
                                select p.StudyId).Distinct().ToList();

                var tbStudyList = (from p in db.Table<Study.Entity.tbStudy>()
                                   where studyIds.Contains(p.Id)
                                   && p.tbYear.IsDeleted == false
                                   select p).ToList();

                var classList = new List<Dto.StudyClassStudent.List>();
                foreach (var tbStudy in tbStudyList)
                {
                    if (tbStudy.IsRoom)//教室模式
                    {
                        var tbClassList = (from p in db.Table<Study.Entity.tbStudyRoomStudent>()
                                           where p.tbStudy.Id == tbStudy.Id
                                           && p.tbStudy.IsDeleted == false
                                           && p.tbRoom.IsDeleted == false
                                           && p.tbStudent.IsDeleted == false
                                           select new Dto.StudyClassStudent.List
                                           {
                                               ClassId = p.tbRoom.Id,
                                               ClassName = p.tbRoom.RoomName,
                                               StudentId = p.tbStudent.Id,
                                               StudentCode = p.tbStudent.StudentCode,
                                               StudentName = p.tbStudent.StudentName,
                                               StudyId = p.tbStudy.Id,
                                               StudyName = p.tbStudy.StudyName
                                           }).ToList();
                        classList.AddRange(tbClassList);
                    }
                    else//班级模式
                    {
                        var tbClassList = (from p in db.Table<Study.Entity.tbStudyClassStudent>()
                                           where p.tbStudy.Id == tbStudy.Id
                                           && p.tbStudy.IsDeleted == false
                                           && p.tbClass.IsDeleted == false
                                           && p.tbStudent.IsDeleted == false
                                           select new Dto.StudyClassStudent.List
                                           {
                                               ClassId = p.tbClass.Id,
                                               ClassName = p.tbClass.ClassName,
                                               StudentId = p.tbStudent.Id,
                                               StudentCode = p.tbStudent.StudentCode,
                                               StudentName = p.tbStudent.StudentName,
                                               StudyId = p.tbStudy.Id,
                                               StudyName = p.tbStudy.StudyName
                                           }).ToList();

                        classList.AddRange(tbClassList);
                    }
                }
                foreach (var item in tbStudyDataList)
                {
                    var data = classList.Where(d => d.StudyId == item.StudyId && d.StudentId == item.StudentId).FirstOrDefault();
                    if (data != null)
                    {
                        item.ClassId = data.ClassId;
                        item.ClassName = data.ClassName;
                    }
                }
                vm.StudyReportDetailList = (from p in tbStudyDataList
                                            where p.ClassId != 0
                                            select new Dto.StudyReport.Detail
                                            {
                                                StudyName = p.StudyName,
                                                ClassName = p.ClassName,
                                                InputDate = p.InputDate,
                                                Remark = p.Remark,
                                                SysUserName = p.SysUserName,
                                                SexName = p.SexName,
                                                StudentCode = p.StudentCode,
                                                StudentName = p.StudentName,
                                                StudyOptionName = p.StudyOptionName
                                            }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("序号"),
                        new System.Data.DataColumn("晚自习"),
                        new System.Data.DataColumn("行政班"),
                        new System.Data.DataColumn("自习日期"),
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("性别"),
                        new System.Data.DataColumn("表现名称"),
                        new System.Data.DataColumn("备注"),
                        new System.Data.DataColumn("录入人员")
                    });

                var index = 0;
                foreach (var a in vm.StudyReportDetailList)
                {
                    index++;
                    var dr = dt.NewRow();
                    dr["序号"] = index.ToString();
                    dr["晚自习"] = a.StudyName;
                    dr["行政班"] = a.ClassName;
                    dr["自习日期"] = a.InputDate.ToString(Code.Common.StringToDate);
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["性别"] = a.SexName;
                    dr["表现名称"] = a.StudyOptionName;
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
    }
}