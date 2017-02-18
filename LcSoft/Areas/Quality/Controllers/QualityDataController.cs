using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace XkSystem.Areas.Quality.Controllers
{
    public class QualityDataController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        #region 学生评价
        public ActionResult Input()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Student)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityData.Input();

                //获取学段信息
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }
                //获取学年
                var yearId = (from p in db.Table<Basis.Entity.tbYear>()
                              where p.Id == vm.YearId
                              select p.tbYearParent.tbYearParent.Id).FirstOrDefault();

                //评教下拉框数据源逻辑
                var qualityList = (from p in db.Table<Quality.Entity.tbQuality>()
                                   where p.IsDeleted == false
                                   && p.tbYear.Id == vm.YearId
                                   && p.IsOpen == true
                                   select p).Distinct().ToList();
                vm.QualityList = (from p in qualityList
                                  where (p.FromDate <= DateTime.Now && DateTime.Now <= (p.ToDate.ToShortDateString() + " 23:59:59").ConvertToDateTime())
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.QualityName,
                                      Value = p.Id.ToString()
                                  }).Distinct().ToList();
                //默认取第一个评教Id去查询后面数据
                if (vm.QualityId == 0 && vm.QualityList.Count > 0)
                {
                    vm.QualityId = vm.QualityList.FirstOrDefault().Value.ConvertToInt();
                }
                else if (vm.QualityList.Count <= 0)
                {
                    vm.QualityId = 0;
                }

                //获取评价分组
                vm.QualityItemGroupList = (from p in db.Table<Quality.Entity.tbQualityItemGroup>()
                                           where p.IsDeleted == false
                                           && p.tbQuality.Id == vm.QualityId
                                           orderby p.No ascending
                                           select new System.Web.Mvc.SelectListItem
                                           {
                                               Text = p.QualityItemGroupName,
                                               Value = p.Id.ToString()
                                           }).Distinct().ToList();
                //默认取第一个评教Id去查询后面数据
                if (vm.QualityItemGroupId == 0 && vm.QualityItemGroupList.Count > 0)
                {
                    vm.QualityItemGroupId = vm.QualityItemGroupList.FirstOrDefault().Value.ConvertToInt();
                }

                //获取评价内容
                vm.QualityItemList = (from p in db.Table<Quality.Entity.tbQualityItem>()
                                      .Include(d => d.tbQualityItemGroup)
                                      .Include(d => d.tbQualityItemGroup.tbQuality)
                                      where p.IsDeleted == false
                                      && p.tbQualityItemGroup.IsDeleted == false
                                      && p.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                                      orderby p.No ascending
                                      select p).Distinct().ToList();

                //默认取第一个评教Id去查询后面数据
                if (vm.QualityItemId == 0 && vm.QualityItemList.Count > 0)
                {
                    vm.QualityItemId = vm.QualityItemList.FirstOrDefault().Id;
                }
                var itemIds = vm.QualityItemList.Select(d => d.Id).ToList();
                vm.ItemIds = string.Join(",", itemIds);

                //获取评价选项
                vm.QualityOptionList = (from p in db.Table<Quality.Entity.tbQualityOption>()
                                      .Include(d => d.tbQualityItem)
                                        where p.IsDeleted == false
                                        && p.tbQualityItem.IsDeleted == false
                                        && p.tbQualityItem.tbQualityItemGroup.IsDeleted == false
                                        && p.tbQualityItem.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                                        orderby p.No
                                        select p).Distinct().ToList();
                //if (vm.StudentId <= 0)
                //{
                //    vm.StudentId = Code.Common.UserId;
                //}
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && p.tbSysUser.Id == Code.Common.UserId
                               select p).FirstOrDefault();
                if (student != null)
                {
                    vm.UserId = student.Id;
                }

                //获取学生所在班学生信息
                var cla = (from p in db.Table<Basis.Entity.tbClassStudent>()
                          .Include(d => d.tbStudent.tbSysUser)
                          .Include(d => d.tbStudent)
                           where p.IsDeleted == false
                          && p.tbClass.IsDeleted == false
                          && p.tbStudent.IsDeleted == false
                          && p.tbClass.tbYear.Id == yearId
                          && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                           select new { p.tbClass, p.tbStudent }).Distinct().FirstOrDefault();
                if (cla != null)
                {
                    //获取自己所在班信息
                    vm.StudentList.Add((from p in db.Table<Basis.Entity.tbClassStudent>()
                                .Include(d => d.tbStudent)
                                .Include(d => d.tbStudent.tbSysUser)
                                        where p.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbClass.Id == cla.tbClass.Id
                                        && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        select p.tbStudent).FirstOrDefault());
                    //获取除自己外所在班同学信息
                    vm.StudentList.AddRange((from p in db.Table<Basis.Entity.tbClassStudent>()
                                .Include(d => d.tbStudent)
                                 .Include(d => d.tbStudent.tbSysUser)
                                             where p.IsDeleted == false
                                      && p.tbClass.IsDeleted == false
                                      && p.tbStudent.IsDeleted == false
                                      && p.tbClass.Id == cla.tbClass.Id
                                      && p.tbStudent.tbSysUser.Id != Code.Common.UserId
                                             select p.tbStudent).ToList());
                }

                //获取评价结果信息
                vm.QualityDataList = (from p in db.Table<Quality.Entity.tbQualityData>()
                                      where p.tbSysUser.Id == Code.Common.UserId
                                      && p.IsUserType == 1
                                      && p.tbQualityItem.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                                      && p.tbQualityItem.IsDeleted == false
                                      && p.tbQualityItem.tbQualityItemGroup.IsDeleted == false
                                      select new Dto.QualityData.Edit
                                      {
                                          ItemId = p.tbQualityItem.Id,
                                          OptionId = p.tbQualityOption != null ? p.tbQualityOption.Id : 0,
                                          QualityText = p.QualityText,
                                          StudentId = p.tbStudent.Id,
                                      }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Input(Models.QualityData.Input vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("Input", new { qualityId = vm.QualityId, yearId = vm.YearId, }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InputSave(Models.QualityData.Input vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (string.IsNullOrEmpty(vm.ItemIds) == false)
                {
                    var result = new List<string>();
                    var resultArea = new List<int>();
                    var itemIds = vm.ItemIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();

                    //获取评价内容
                    var qualityItemIds = vm.ItemIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt());
                    var qualityItemList = (from p in db.Table<Quality.Entity.tbQualityItem>()
                                           where qualityItemIds.Contains(p.Id)
                                           select p).ToList();

                    foreach (var itemId in qualityItemIds)
                    {
                        var item = qualityItemList.Where(d => d.Id == itemId).FirstOrDefault();
                        if (item != null)
                        {
                            if (item.QualityItemType == Code.EnumHelper.QualityItemType.Radio)
                            {
                                if (Request["Radio" + item.Id] == null)
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                                else
                                {
                                    result.Add(Request["Radio" + item.Id]);
                                }
                            }
                            else if (item.QualityItemType == Code.EnumHelper.QualityItemType.CheckBox)
                            {
                                if (Request["Cbox" + item.Id] == null)
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                                else
                                {
                                    result.AddRange(Request["Cbox" + item.Id].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d));
                                }
                            }
                            else if (item.QualityItemType == Code.EnumHelper.QualityItemType.TextBox)
                            {
                                if (Request["textareaText" + item.Id] == null || Request["textareaText" + item.Id] == "")
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                                else
                                {
                                    resultArea.Add(Request["textareaId" + item.Id].ConvertToInt());
                                }
                            }
                        }
                        else
                        {
                            error.AddError("请填写完整!");
                            break;
                        }
                    }

                    if (error.Count > 0)
                    {
                        return Code.MvcHelper.Post(error);
                    }

                    #region 保存结果
                    var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                        .Include(p => p.tbClass)
                                        where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        select p).FirstOrDefault();

                    var quality = db.Set<Quality.Entity.tbQuality>().Find(vm.QualityId);

                    //根据评价获取评价结果信息
                    var del = (from p in db.Table<Quality.Entity.tbQualityData>()
                               where p.tbQualityItem.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                               && p.IsUserType == 1
                               && p.tbStudent.Id == vm.StudentId
                               select p).ToList();
                    foreach (var a in del)
                    {
                        a.IsDeleted = true;
                    }

                    var dataList = new List<Quality.Entity.tbQualityData>();
                    var student = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                    foreach (var a in result)
                    {
                        var b = a.Split('|');
                        if (b.Length > 1)
                        {
                            var tb = new Quality.Entity.tbQualityData();
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            tb.tbQualityOption = db.Set<Quality.Entity.tbQualityOption>().Find(b[0].ConvertToInt());
                            tb.tbQualityItem = db.Set<Quality.Entity.tbQualityItem>().Find(b[1].ConvertToInt());
                            tb.tbStudent = student;
                            tb.IsUserType = 1;//学生自己
                            db.Set<Quality.Entity.tbQualityData>().Add(tb);
                        }
                    }
                    foreach (var a in resultArea)
                    {
                        var tb = new Quality.Entity.tbQualityData();
                        tb.QualityText = Request["textareaText" + a];
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        tb.tbQualityItem = db.Set<Quality.Entity.tbQualityItem>().Find(a);
                        tb.tbStudent = student;
                        tb.IsUserType = 1;//学生自己
                        db.Set<Quality.Entity.tbQualityData>().Add(tb);
                    }
                    db.Set<Quality.Entity.tbQualityData>().AddRange(dataList);

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("提交了学生评价数据。");
                    }
                    #endregion
                }
                else
                {
                    error.AddError("请填写完整!");
                }

                return Code.MvcHelper.Post(error, null, "提交成功!");
            }
        }
        #endregion

        #region 班主任评价
        public ActionResult ClassInput()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator && Code.Common.UserType != Code.EnumHelper.SysUserType.Teacher)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityData.ClassInput();

                //获取学段信息
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }
                //获取学期信息
                var yearId = (from p in db.Table<Basis.Entity.tbYear>()
                              where p.Id == vm.YearId
                              select p.tbYearParent.tbYearParent.Id).FirstOrDefault();

                //获取教师所在行政班信息
                vm.ClassList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                where p.IsDeleted == false
                                && p.tbClass.IsDeleted == false
                                && p.tbTeacher.IsDeleted == false
                                && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                 && p.tbClass.tbYear.Id == yearId
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.tbClass.ClassName,
                                    Value = p.tbClass.Id.ToString(),
                                }).Distinct().ToList();
                //默认取第一个评教Id去查询后面数据
                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }
                else if (vm.ClassList.Count <= 0)
                {
                    vm.ClassId = 0;
                }

                //评教下拉框数据源逻辑
                var qualityList = (from p in db.Table<Quality.Entity.tbQuality>()
                                   where p.IsDeleted == false
                                   && p.tbYear.Id == vm.YearId
                                   && p.IsOpen == true
                                   select p).Distinct().ToList();
                if (vm.ClassList.Count > 0)
                {
                    vm.QualityList = (from p in qualityList
                                      where (p.FromDate <= DateTime.Now && DateTime.Now <= (p.ToDate.ToShortDateString() + " 23:59:59").ConvertToDateTime())
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.QualityName,
                                          Value = p.Id.ToString()
                                      }).Distinct().ToList();

                    //默认取第一个评教Id去查询后面数据
                    if (vm.QualityId == 0 && vm.QualityList.Count > 0)
                    {
                        vm.QualityId = vm.QualityList.FirstOrDefault().Value.ConvertToInt();
                    }
                    else if (vm.QualityList.Count <= 0)
                    {
                        vm.QualityId = 0;
                    }
                }

                //获取评价分组
                vm.QualityItemGroupList = (from p in db.Table<Quality.Entity.tbQualityItemGroup>()
                                           where p.IsDeleted == false
                                           && p.tbQuality.Id == vm.QualityId
                                           orderby p.No descending
                                           select new System.Web.Mvc.SelectListItem
                                           {
                                               Text = p.QualityItemGroupName,
                                               Value = p.Id.ToString()
                                           }).Distinct().ToList();
                //默认取第一个评教Id去查询后面数据
                if (vm.QualityItemGroupId == 0 && vm.QualityItemGroupList.Count > 0)
                {
                    vm.QualityItemGroupId = vm.QualityItemGroupList.FirstOrDefault().Value.ConvertToInt();
                }

                //获取评价内容
                vm.QualityItemList = (from p in db.Table<Quality.Entity.tbQualityItem>()
                                      .Include(d => d.tbQualityItemGroup)
                                      .Include(d => d.tbQualityItemGroup.tbQuality)
                                      where p.IsDeleted == false
                                      && p.tbQualityItemGroup.IsDeleted == false
                                      && p.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                                      orderby p.No ascending
                                      select p).Distinct().ToList();

                //默认取第一个评教Id去查询后面数据
                if (vm.QualityItemId == 0 && vm.QualityItemList.Count > 0)
                {
                    vm.QualityItemId = vm.QualityItemList.FirstOrDefault().Id;
                }
                var itemIds = vm.QualityItemList.Select(d => d.Id).ToList();
                vm.ItemIds = string.Join(",", itemIds);

                //获取评价选项信息
                vm.QualityOptionList = (from p in db.Table<Quality.Entity.tbQualityOption>()
                                      .Include(d => d.tbQualityItem)
                                        where p.IsDeleted == false
                                        && p.tbQualityItem.IsDeleted == false
                                        && p.tbQualityItem.tbQualityItemGroup.IsDeleted == false
                                        && p.tbQualityItem.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                                        orderby p.No
                                        select p).Distinct().ToList();

                //获取学生所在行政班信息
                vm.StudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                .Include(d => d.tbStudent.tbSysUser)
                                  where p.IsDeleted == false
                                  && p.tbClass.IsDeleted == false
                                  //&& p.tbClass.tbYear.IsDefault == true
                                  && p.tbStudent.IsDeleted == false
                                  && p.tbClass.Id == vm.ClassId
                                  select p.tbStudent).ToList();


                //获取评价结果信息
                vm.QualityDataList = (from p in db.Table<Quality.Entity.tbQualityData>()
                                      where p.tbSysUser.Id == Code.Common.UserId
                                      && p.IsUserType == 2
                                      && p.tbQualityItem.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                                      && p.tbQualityItem.IsDeleted == false
                                      && p.tbQualityItem.tbQualityItemGroup.IsDeleted == false
                                      select new Dto.QualityData.Edit
                                      {
                                          ItemId = p.tbQualityItem != null ? p.tbQualityItem.Id : 0,
                                          OptionId = p.tbQualityOption != null ? p.tbQualityOption.Id : 0,
                                          QualityText = p.QualityText,
                                          StudentId = p.tbStudent != null ? p.tbStudent.Id : 0,
                                      }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassInput(Models.QualityData.ClassInput vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassInput", new
            {
                qualityId = vm.QualityId,
                classId = vm.ClassId,
                studentId = vm.StudentId,
                yearId = vm.YearId,
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassInputSave(Models.QualityData.ClassInput vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (string.IsNullOrEmpty(vm.ItemIds) == false)
                {
                    var result = new List<string>();
                    var resultArea = new List<int>();
                    var itemIds = vm.ItemIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();

                    //获取评价内容
                    var qualityItemIds = vm.ItemIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt());
                    var qualityItemList = (from p in db.Table<Quality.Entity.tbQualityItem>()
                                           where qualityItemIds.Contains(p.Id)
                                           select p).ToList();

                    foreach (var itemId in qualityItemIds)
                    {
                        var item = qualityItemList.Where(d => d.Id == itemId).FirstOrDefault();
                        if (item != null)
                        {
                            if (item.QualityItemType == Code.EnumHelper.QualityItemType.Radio)
                            {
                                if (Request["Radio" + item.Id] == null)
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                                else
                                {
                                    result.Add(Request["Radio" + item.Id]);
                                }
                            }
                            else if (item.QualityItemType == Code.EnumHelper.QualityItemType.CheckBox)
                            {
                                if (Request["Cbox" + item.Id] == null)
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                                else
                                {
                                    result.AddRange(Request["Cbox" + item.Id].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d));
                                }
                            }
                            else if (item.QualityItemType == Code.EnumHelper.QualityItemType.TextBox)
                            {
                                if (Request["textareaText" + item.Id] == null || Request["textareaText" + item.Id] == "")
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                                else
                                {
                                    resultArea.Add(Request["textareaId" + item.Id].ConvertToInt());
                                }
                            }
                        }
                        else
                        {
                            error.AddError("请填写完整!");
                            break;
                        }
                    }

                    if (error.Count > 0)
                    {
                        return Code.MvcHelper.Post(error);
                    }

                    #region 保存结果
                    var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    .Include(p => p.tbClass)
                                        where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        select p).FirstOrDefault();

                    var survey = db.Set<Quality.Entity.tbQuality>().Find(vm.QualityId);

                    //根据评价获取评价结果信息
                    var del = (from p in db.Table<Quality.Entity.tbQualityData>()
                               where p.tbQualityItem.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                               && p.IsUserType == 2
                               && p.tbStudent.Id == vm.StudentId
                               select p).ToList();
                    foreach (var a in del)
                    {
                        a.IsDeleted = true;
                    }

                    var dataList = new List<Quality.Entity.tbQualityData>();
                    var student = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                    foreach (var a in result)
                    {
                        var b = a.Split('|');
                        if (b.Length > 1)
                        {
                            var tb = new Quality.Entity.tbQualityData();
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            tb.tbQualityOption = db.Set<Quality.Entity.tbQualityOption>().Find(b[0].ConvertToInt());
                            tb.tbQualityItem = db.Set<Quality.Entity.tbQualityItem>().Find(b[1].ConvertToInt());
                            tb.tbStudent = student;
                            tb.IsUserType = 2;//班主任
                            db.Set<Quality.Entity.tbQualityData>().Add(tb);
                        }
                    }
                    foreach (var a in resultArea)
                    {
                        var tb = new Quality.Entity.tbQualityData();
                        tb.QualityText = Request["textareaText" + a];
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        tb.tbQualityItem = db.Set<Quality.Entity.tbQualityItem>().Find(a);
                        tb.tbStudent = student;
                        tb.IsUserType = 2;//班主任
                        db.Set<Quality.Entity.tbQualityData>().Add(tb);
                    }
                    db.Set<Quality.Entity.tbQualityData>().AddRange(dataList);

                    if (db.SaveChanges() > 0)
                    {
                        vm.StudentId = student.Id;
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("提交了学生评价数据。");
                    }
                    #endregion
                }
                else
                {
                    error.AddError("请填写完整!");
                }

                return Code.MvcHelper.Post(error, null, "提交成功!");
            }
        }
        #endregion

        #region 任课教师评价
        public ActionResult OrgInput()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator && Code.Common.UserType != Code.EnumHelper.SysUserType.Teacher)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityData.OrgInput();

                //获取学段信息
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                //获取学年信息
                var yearId = (from p in db.Table<Basis.Entity.tbYear>()
                              where p.Id == vm.YearId
                              select p.tbYearParent.tbYearParent.Id).FirstOrDefault();

                //获取教师所在教学班信息
                vm.OrgList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                              where p.IsDeleted == false
                              && p.tbOrg.IsDeleted == false
                              && p.tbTeacher.IsDeleted == false
                              && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                              && p.tbOrg.tbYear.Id == vm.YearId
                              select new System.Web.Mvc.SelectListItem
                              {
                                  Text = p.tbOrg.OrgName,
                                  Value = p.tbOrg.Id.ToString(),
                              }).Distinct().ToList();
                //默认取第一个评教Id去查询后面数据
                if (vm.OrgId == 0 && vm.OrgList.Count > 0)
                {
                    vm.OrgId = vm.OrgList.FirstOrDefault().Value.ConvertToInt();
                }
                else if (vm.OrgList.Count <= 0)
                {
                    vm.OrgId = 0;
                }

                if (vm.OrgList.Count > 0)
                {
                    //评教下拉框数据源逻辑
                    var qualityList = (from p in db.Table<Quality.Entity.tbQuality>()
                                       where p.IsDeleted == false
                                       && p.tbYear.Id == vm.YearId
                                       && p.IsOpen == true
                                       select p).Distinct().ToList();
                    vm.QualityList = (from p in qualityList
                                      where (p.FromDate <= DateTime.Now && DateTime.Now <= (p.ToDate.ToShortDateString() + " 23:59:59").ConvertToDateTime())
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.QualityName,
                                          Value = p.Id.ToString()
                                      }).Distinct().ToList();
                    //默认取第一个评教Id去查询后面数据
                    if (vm.QualityId == 0 && vm.QualityList.Count > 0)
                    {
                        vm.QualityId = vm.QualityList.FirstOrDefault().Value.ConvertToInt();
                    }
                    else if (vm.QualityList.Count <= 0)
                    {
                        vm.QualityId = 0;
                    }
                }

                //获取评价分组
                vm.QualityItemGroupList = (from p in db.Table<Quality.Entity.tbQualityItemGroup>()
                                           where p.IsDeleted == false
                                           && p.tbQuality.Id == vm.QualityId
                                           orderby p.No ascending
                                           select new System.Web.Mvc.SelectListItem
                                           {
                                               Text = p.QualityItemGroupName,
                                               Value = p.Id.ToString()
                                           }).Distinct().ToList();
                //默认取第一个评教Id去查询后面数据
                if (vm.QualityItemGroupId == 0 && vm.QualityItemGroupList.Count > 0)
                {
                    vm.QualityItemGroupId = vm.QualityItemGroupList.FirstOrDefault().Value.ConvertToInt();
                }

                //获取评价内容
                vm.QualityItemList = (from p in db.Table<Quality.Entity.tbQualityItem>()
                                      .Include(d => d.tbQualityItemGroup)
                                      .Include(d => d.tbQualityItemGroup.tbQuality)
                                      where p.IsDeleted == false
                                      && p.tbQualityItemGroup.IsDeleted == false
                                      && p.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                                      orderby p.No ascending
                                      select p).Distinct().ToList();

                //默认取第一个评教Id去查询后面数据
                if (vm.QualityItemId == 0 && vm.QualityItemList.Count > 0)
                {
                    vm.QualityItemId = vm.QualityItemList.FirstOrDefault().Id;
                }
                var itemIds = vm.QualityItemList.Select(d => d.Id).ToList();
                vm.ItemIds = string.Join(",", itemIds);

                //获取评价选项信息
                vm.QualityOptionList = (from p in db.Table<Quality.Entity.tbQualityOption>()
                                      .Include(d => d.tbQualityItem)
                                        where p.IsDeleted == false
                                        && p.tbQualityItem.IsDeleted == false
                                        && p.tbQualityItem.tbQualityItemGroup.IsDeleted == false
                                        && p.tbQualityItem.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                                        orderby p.No
                                        select p).Distinct().ToList();

                //获取学生所在教学班信息
                vm.StudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                .Include(d => d.tbStudent.tbSysUser)
                                  where p.IsDeleted == false
                                  && p.tbOrg.IsDeleted == false
                                  //&& p.tbOrg.tbYear.IsDefault == true
                                  && p.tbStudent.IsDeleted == false
                                  && p.tbOrg.Id == vm.OrgId
                                  select p.tbStudent).ToList();
                //行政班模式
                if (vm.StudentList.Count() <= 0)
                {
                    //获取行政班
                    var cla = (from p in db.Table<Course.Entity.tbOrg>()
                                .Include(d => d.tbClass)
                               where p.IsDeleted == false
                               && p.Id == vm.OrgId
                               select p.tbClass).FirstOrDefault();

                    if (cla != null)
                    {
                        //获取学生所在行政班信息
                        vm.StudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                               .Include(d => d.tbStudent.tbSysUser)
                                          where p.IsDeleted == false
                                          && p.tbClass.IsDeleted == false
                                          //&& p.tbClass.tbYear.IsDefault == true
                                          && p.tbStudent.IsDeleted == false
                                          && p.tbClass.Id == cla.Id
                                          select p.tbStudent).ToList();
                    }
                }


                //获取评价结果信息
                vm.QualityDataList = (from p in db.Table<Quality.Entity.tbQualityData>()
                                      where p.tbSysUser.Id == Code.Common.UserId
                                      && p.IsUserType == 3
                                      && p.tbQualityItem.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                                      && p.tbQualityItem.IsDeleted == false
                                      && p.tbQualityItem.tbQualityItemGroup.IsDeleted == false
                                      && p.tbOrg.Id == vm.OrgId
                                      select new Dto.QualityData.Edit
                                      {
                                          ItemId = p.tbQualityItem.Id,
                                          OptionId = p.tbQualityOption != null ? p.tbQualityOption.Id : 0,
                                          QualityText = p.QualityText,
                                          StudentId = p.tbStudent.Id,
                                      }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrgInput(Models.QualityData.OrgInput vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("OrgInput", new
            {
                qualityId = vm.QualityId,
                orgId = vm.OrgId,
                studentId = vm.StudentId,
                yearId = vm.YearId,
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrgInputSave(Models.QualityData.OrgInput vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (string.IsNullOrEmpty(vm.ItemIds) == false)
                {
                    var result = new List<string>();
                    var resultArea = new List<int>();
                    var itemIds = vm.ItemIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();

                    //获取评价内容信息
                    var qualityItemIds = vm.ItemIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt());
                    var qualityItemList = (from p in db.Table<Quality.Entity.tbQualityItem>()
                                           where qualityItemIds.Contains(p.Id)
                                           select p).ToList();

                    foreach (var itemId in qualityItemIds)
                    {
                        var item = qualityItemList.Where(d => d.Id == itemId).FirstOrDefault();
                        if (item != null)
                        {
                            if (item.QualityItemType == Code.EnumHelper.QualityItemType.Radio)
                            {
                                if (Request["Radio" + item.Id] == null)
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                                else
                                {
                                    result.Add(Request["Radio" + item.Id]);
                                }
                            }
                            else if (item.QualityItemType == Code.EnumHelper.QualityItemType.CheckBox)
                            {
                                if (Request["Cbox" + item.Id] == null)
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                                else
                                {
                                    result.AddRange(Request["Cbox" + item.Id].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d));
                                }
                            }
                            else if (item.QualityItemType == Code.EnumHelper.QualityItemType.TextBox)
                            {
                                if (Request["textareaText" + item.Id] == null || Request["textareaText" + item.Id] == "")
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                                else
                                {
                                    resultArea.Add(Request["textareaId" + item.Id].ConvertToInt());
                                }
                            }
                        }
                        else
                        {
                            error.AddError("请填写完整!");
                            break;
                        }
                    }

                    if (error.Count > 0)
                    {
                        return Code.MvcHelper.Post(error);
                    }

                    #region 保存结果
                    var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    .Include(p => p.tbClass)
                                        where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        select p).FirstOrDefault();

                    var survey = db.Set<Quality.Entity.tbQuality>().Find(vm.QualityId);

                    //根据评价获取评价结果信息
                    var del = (from p in db.Table<Quality.Entity.tbQualityData>()
                               where p.tbQualityItem.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                               && p.tbOrg.Id == vm.OrgId
                               && p.IsUserType == 3
                               && p.tbStudent.Id == vm.StudentId
                               select p).ToList();
                    foreach (var a in del)
                    {
                        a.IsDeleted = true;
                    }

                    var dataList = new List<Quality.Entity.tbQualityData>();
                    var student = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                    foreach (var a in result)
                    {
                        var b = a.Split('|');
                        if (b.Length > 1)
                        {
                            var tb = new Quality.Entity.tbQualityData();
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            tb.tbQualityOption = db.Set<Quality.Entity.tbQualityOption>().Find(b[0].ConvertToInt());
                            tb.tbQualityItem = db.Set<Quality.Entity.tbQualityItem>().Find(b[1].ConvertToInt());
                            tb.tbOrg = db.Set<Course.Entity.tbOrg>().Find(vm.OrgId);
                            tb.tbStudent = student;
                            tb.IsUserType = 3;//任课教师
                            db.Set<Quality.Entity.tbQualityData>().Add(tb);
                        }
                    }
                    foreach (var a in resultArea)
                    {
                        var tb = new Quality.Entity.tbQualityData();
                        tb.QualityText = Request["textareaText" + a];
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        tb.tbQualityItem = db.Set<Quality.Entity.tbQualityItem>().Find(a);
                        tb.tbOrg = db.Set<Course.Entity.tbOrg>().Find(vm.OrgId);
                        tb.tbStudent = student;
                        tb.IsUserType = 3;//任课教师
                        db.Set<Quality.Entity.tbQualityData>().Add(tb);
                    }
                    db.Set<Quality.Entity.tbQualityData>().AddRange(dataList);

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("提交了学生评价数据。");
                    }
                    #endregion
                }
                else
                {
                    error.AddError("请填写完整!");
                }

                return Code.MvcHelper.Post(error, null, "提交成功!");
            }
        }
        #endregion

        #region 家长评语
        public ActionResult FamilyInput()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Family)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityData.FamilyInput();

                //获取学段信息
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }
                //获取学年信息
                var yearId = (from p in db.Table<Basis.Entity.tbYear>()
                              where p.Id == vm.YearId
                              select p.tbYearParent.tbYearParent.Id).FirstOrDefault();

                //评教下拉框数据源逻辑
                var qualityList = (from p in db.Table<Quality.Entity.tbQuality>()
                                   where p.IsDeleted == false
                                   && p.tbYear.Id == vm.YearId
                                   && p.IsOpen == true
                                   select p).Distinct().ToList();
                vm.QualityList = (from p in qualityList
                                  where (p.FromDate <= DateTime.Now && DateTime.Now <= (p.ToDate.ToShortDateString() + " 23:59:59").ConvertToDateTime())
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.QualityName,
                                      Value = p.Id.ToString()
                                  }).Distinct().ToList();
                //默认取第一个评教Id去查询后面数据
                if (vm.QualityId == 0 && vm.QualityList.Count > 0)
                {
                    vm.QualityId = vm.QualityList.FirstOrDefault().Value.ConvertToInt();
                }
                else if (vm.QualityList.Count <= 0)
                {
                    vm.QualityId = 0;
                }

                //获取评价分组信息
                vm.QualityItemGroupList = (from p in db.Table<Quality.Entity.tbQualityItemGroup>()
                                           where p.IsDeleted == false
                                           && p.tbQuality.Id == vm.QualityId
                                           orderby p.No ascending
                                           select new System.Web.Mvc.SelectListItem
                                           {
                                               Text = p.QualityItemGroupName,
                                               Value = p.Id.ToString()
                                           }).Distinct().ToList();
                //默认取第一个评教Id去查询后面数据
                if (vm.QualityItemGroupId == 0 && vm.QualityItemGroupList.Count > 0)
                {
                    vm.QualityItemGroupId = vm.QualityItemGroupList.FirstOrDefault().Value.ConvertToInt();
                }

                //获取评价内容信息
                vm.QualityItemList = (from p in db.Table<Quality.Entity.tbQualityItem>()
                                      .Include(d => d.tbQualityItemGroup)
                                      .Include(d => d.tbQualityItemGroup.tbQuality)
                                      where p.IsDeleted == false
                                      && p.tbQualityItemGroup.IsDeleted == false
                                      && p.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                                      orderby p.No ascending
                                      select p).Distinct().ToList();

                //默认取第一个评教Id去查询后面数据
                if (vm.QualityItemId == 0 && vm.QualityItemList.Count > 0)
                {
                    vm.QualityItemId = vm.QualityItemList.FirstOrDefault().Id;
                }
                var itemIds = vm.QualityItemList.Select(d => d.Id).ToList();
                vm.ItemIds = string.Join(",", itemIds);

                //获取评价选项信息
                vm.QualityOptionList = (from p in db.Table<Quality.Entity.tbQualityOption>()
                                      .Include(d => d.tbQualityItem)
                                        where p.IsDeleted == false
                                        && p.tbQualityItem.IsDeleted == false
                                        && p.tbQualityItem.tbQualityItemGroup.IsDeleted == false
                                        && p.tbQualityItem.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                                        orderby p.No
                                        select p).Distinct().ToList();
                //if (vm.StudentId <= 0)
                //{
                //    vm.StudentId = Code.Common.UserId;
                //}
                //根据家长获取学生信息
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               .Include(d => d.tbSysUser)
                               where p.IsDeleted == false
                               && p.tbSysUser.IsDeleted == false
                               && (p.tbSysUser.Id == Code.Common.UserId || p.tbSysUserFamily.Id == Code.Common.UserId)
                               select p).FirstOrDefault();
                if (student != null)
                {
                    vm.StudentList.Add(student);
                    //获取评价结果信息
                    vm.QualityDataList = (from p in db.Table<Quality.Entity.tbQualityData>()
                                          where p.tbSysUser.Id == Code.Common.UserId
                                          && p.IsUserType == 4
                                          && p.tbQualityItem.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                                          && p.tbQualityItem.IsDeleted == false
                                          && p.tbQualityItem.tbQualityItemGroup.IsDeleted == false
                                          select new Dto.QualityData.Edit
                                          {
                                              ItemId = p.tbQualityItem.Id,
                                              OptionId = p.tbQualityOption != null ? p.tbQualityOption.Id : 0,
                                              QualityText = p.QualityText,
                                              StudentId = p.tbStudent.Id,
                                          }).ToList();
                }
                return View(vm);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FamilyInputSave(Models.QualityData.FamilyInput vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (string.IsNullOrEmpty(vm.ItemIds) == false)
                {
                    var result = new List<string>();
                    var resultArea = new List<int>();
                    var itemIds = vm.ItemIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();

                    //获取评价内容信息
                    var qualityItemIds = vm.ItemIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt());
                    var qualityItemList = (from p in db.Table<Quality.Entity.tbQualityItem>()
                                           where qualityItemIds.Contains(p.Id)
                                           select p).ToList();

                    foreach (var itemId in qualityItemIds)
                    {
                        var item = qualityItemList.Where(d => d.Id == itemId).FirstOrDefault();
                        if (item != null)
                        {
                            if (item.QualityItemType == Code.EnumHelper.QualityItemType.Radio)
                            {
                                if (Request["Radio" + item.Id] == null)
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                                else
                                {
                                    result.Add(Request["Radio" + item.Id]);
                                }
                            }
                            else if (item.QualityItemType == Code.EnumHelper.QualityItemType.CheckBox)
                            {
                                if (Request["Cbox" + item.Id] == null)
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                                else
                                {
                                    result.AddRange(Request["Cbox" + item.Id].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d));
                                }
                            }
                            else if (item.QualityItemType == Code.EnumHelper.QualityItemType.TextBox)
                            {
                                if (Request["textareaText" + item.Id] == null || Request["textareaText" + item.Id] == "")
                                {
                                    error.AddError("请填写完整!");
                                    break;
                                }
                                else
                                {
                                    resultArea.Add(Request["textareaId" + item.Id].ConvertToInt());
                                }
                            }
                        }
                        else
                        {
                            error.AddError("请填写完整!");
                            break;
                        }
                    }

                    if (error.Count > 0)
                    {
                        return Code.MvcHelper.Post(error);
                    }

                    #region 保存结果

                    var quality = db.Set<Quality.Entity.tbQuality>().Find(vm.QualityId);

                    //根据评价获取评价结果信息
                    var del = (from p in db.Table<Quality.Entity.tbQualityData>()
                               where p.tbQualityItem.tbQualityItemGroup.tbQuality.Id == vm.QualityId
                               && p.IsUserType == 4
                               && p.tbStudent.Id == vm.StudentId
                               select p).ToList();
                    foreach (var a in del)
                    {
                        a.IsDeleted = true;
                    }

                    var dataList = new List<Quality.Entity.tbQualityData>();
                    var student = db.Set<Student.Entity.tbStudent>().Find(vm.StudentId);
                    foreach (var a in result)
                    {
                        var b = a.Split('|');
                        if (b.Length > 1)
                        {
                            var tb = new Quality.Entity.tbQualityData();
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            tb.tbQualityOption = db.Set<Quality.Entity.tbQualityOption>().Find(b[0].ConvertToInt());
                            tb.tbQualityItem = db.Set<Quality.Entity.tbQualityItem>().Find(b[1].ConvertToInt());
                            tb.tbStudent = student;
                            tb.IsUserType = 4;//家长
                            db.Set<Quality.Entity.tbQualityData>().Add(tb);
                        }
                    }
                    foreach (var a in resultArea)
                    {
                        var tb = new Quality.Entity.tbQualityData();
                        tb.QualityText = Request["textareaText" + a];
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        tb.tbQualityItem = db.Set<Quality.Entity.tbQualityItem>().Find(a);
                        tb.tbStudent = student;
                        tb.IsUserType = 4;//家长
                        db.Set<Quality.Entity.tbQualityData>().Add(tb);
                    }
                    db.Set<Quality.Entity.tbQualityData>().AddRange(dataList);

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("提交了家长评价数据。");
                    }
                    #endregion
                }
                else
                {
                    error.AddError("请填写完整!");
                }

                return Code.MvcHelper.Post(error, null, "提交成功!");
            }
        }
        #endregion
    }
}