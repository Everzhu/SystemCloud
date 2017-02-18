using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Teacher.Controllers
{
    public class TeacherController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Teacher.List();
                vm.TeacherDeptList = TeacherDeptController.SelectList();
                var tb = from p in db.Table<Teacher.Entity.tbTeacher>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.TeacherCode.Contains(vm.SearchText) || d.TeacherName.Contains(vm.SearchText));
                }
                if (vm.TeacherDeptId > 0)
                {
                    var teacherIds = db.Table<Entity.tbTeacherWithDept>().Where(d => d.tbTeacherDept.Id == vm.TeacherDeptId).Select(d => d.tbTeacher.Id).ToList();
                    tb = tb.Where(d => teacherIds.Contains(d.Id));
                }

                vm.TeacherList = (from p in tb
                                  orderby p.TeacherName
                                  select new Dto.Teacher.List
                                  {
                                      Id = p.Id,
                                      TeacherCode = p.TeacherCode,
                                      TeacherName = p.TeacherName,
                                      TeacherTypeName = p.tbTeacherType.TeacherTypeName,
                                      SexName = p.tbSysUser.tbSex.SexName,
                                      EducationName = p.tbDictEducation.EducationName,
                                      IdentityNumber = p.tbSysUser.IdentityNumber,
                                      Mobile = p.tbSysUser.Mobile,
                                      Email = p.tbSysUser.Email,
                                      Qq = p.tbSysUser.Qq
                                  }).ToPageList(vm.Page);
                //获取教师部门
                var tIds = vm.TeacherList.Select(d => d.Id).ToList();
                var teacherWithDept = db.Table<Entity.tbTeacherWithDept>()
                    .Where(d => tIds.Contains(d.tbTeacher.Id)).Select(d => new { d.tbTeacherDept.TeacherDeptName, d.tbTeacher.Id }).ToList();
                foreach (var v in vm.TeacherList)
                {
                    v.TeacherDeptName = string.Join(",", teacherWithDept.Where(d => d.Id == v.Id).Select(d => d.TeacherDeptName).Distinct().ToList());
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Teacher.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                TeacherDeptId = vm.TeacherDeptId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Teacher.Entity.tbTeacher>()
                            .Include(d => d.tbSysUser)
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    a.tbSysUser.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除教师");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Teacher.Edit();
                vm.SexList = Dict.Controllers.DictSexController.SelectList();
                vm.TeacherDeptList = Teacher.Controllers.TeacherDeptController.SelectNormalList();
                //vm.TeacherDeptList = Teacher.Controllers.TeacherDeptController.SelectList();
                vm.TeacherTypeList = TeacherTypeController.SelectList();
                vm.DictPartyList = Dict.Controllers.DictPartyController.SelectList();
                vm.DictNationList = Dict.Controllers.DictNationController.SelectList();
                vm.DictMarriageList = Dict.Controllers.DictMarriageController.SelectList();
                vm.DictRegionList = Dict.Controllers.DictRegionController.SelectList2();
                vm.DictHealthList = Dict.Controllers.DictHealthController.SelectList();
                vm.EducationList = Dict.Controllers.DictEducationController.SelectList();

                if (id != 0)
                {
                    var teacherDept = db.Table<Entity.tbTeacherWithDept>()
                        .Include(d => d.tbTeacherDept)
                        .Where(d => d.tbTeacher.Id == id).ToList();

                    var tb = (from p in db.Table<Teacher.Entity.tbTeacher>()
                              where p.Id == id
                              select new Dto.Teacher.Edit
                              {
                                  Id = p.Id,
                                  TeacherCode = p.TeacherCode,
                                  TeacherName = p.TeacherName,
                                  TeacherTypeId = p.tbTeacherType.Id,
                                  //TeacherDeptId = string.Join(",", teacherDept.Select(d => d.tbTeacherDept.Id)), //p.tbTeacherDept.Id,
                                  //TeacherDeptName = string.Join(",", teacherDept.Select(d => d.tbTeacherDept.TeacherDeptName)), //p.tbTeacherDept.TeacherDeptName,
                                  IdentityNumber = p.tbSysUser.IdentityNumber,
                                  Mobile = p.tbSysUser.Mobile,
                                  Email = p.tbSysUser.Email,
                                  Qq = p.tbSysUser.Qq,
                                  EducationId = p.tbDictEducation.Id,
                                  SexId = p.tbSysUser.tbSex.Id,
                                  DictHealthId = p.tbDictHealth.Id,
                                  DictMarriageId = p.tbDictMarriage.Id,
                                  DictNationId = p.tbDictNation.Id,
                                  DictPartyId = p.tbDictParty.Id,
                                  DictRegionId = p.tbDictRegion.Id,
                                  Profile = p.Profile
                              }).FirstOrDefault();
                    vm.TeacherDeptIds = string.Join(",", teacherDept.Select(d => d.tbTeacherDept.Id));
                    //vm.TeacherDeptNameJson = Code.JsonHelper.ToJsonString(vm.TeacherDeptList).Replace("│", "").Replace("├", "");

                    if (tb != null)
                    {
                        vm.TeacherEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Teacher.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Teacher.Entity.tbTeacher>().Where(d => d.TeacherCode == vm.TeacherEdit.TeacherCode && d.Id != vm.TeacherEdit.Id).Any())
                    {
                        error.AddError("该教职工号已存在!");
                        return Code.MvcHelper.Post(error);
                    }
                    else
                    {
                        if (vm.TeacherEdit.Id == 0)
                        {
                            var tb = new Teacher.Entity.tbTeacher();
                            tb.TeacherCode = vm.TeacherEdit.TeacherCode;
                            tb.TeacherName = vm.TeacherEdit.TeacherName;
                            tb.tbTeacherType = db.Set<Teacher.Entity.tbTeacherType>().Find(vm.TeacherEdit.TeacherTypeId);
                            //tb.tbTeacherDept = db.Set<Teacher.Entity.tbTeacherDept>().Find(vm.TeacherEdit.TeacherDeptId);

                            tb.tbDictEducation = db.Set<Dict.Entity.tbDictEducation>().Find(vm.TeacherEdit.EducationId);
                            tb.tbDictParty = db.Set<Dict.Entity.tbDictParty>().Find(vm.TeacherEdit.DictPartyId);
                            tb.tbDictNation = db.Set<Dict.Entity.tbDictNation>().Find(vm.TeacherEdit.DictNationId);
                            tb.tbDictMarriage = db.Set<Dict.Entity.tbDictMarriage>().Find(vm.TeacherEdit.DictMarriageId);
                            tb.tbDictRegion = db.Set<Dict.Entity.tbDictRegion>().Find(vm.TeacherEdit.DictRegionId);
                            tb.tbDictHealth = db.Set<Dict.Entity.tbDictHealth>().Find(vm.TeacherEdit.DictHealthId);
                            tb.Profile = vm.TeacherEdit.Profile;

                            tb.tbSysUser = new Sys.Entity.tbSysUser();
                            tb.tbSysUser.UserCode = vm.TeacherEdit.TeacherCode;
                            tb.tbSysUser.UserName = vm.TeacherEdit.TeacherName;
                            tb.tbSysUser.Password = Code.Common.DESEnCode("123456");
                            tb.tbSysUser.PasswordMd5 = Code.Common.CreateMD5Hash("123456");
                            tb.tbSysUser.UserType = Code.EnumHelper.SysUserType.Teacher;
                            tb.tbSysUser.tbSex = db.Set<Dict.Entity.tbDictSex>().Find(vm.TeacherEdit.SexId);
                            tb.tbSysUser.IdentityNumber = vm.TeacherEdit.IdentityNumber;
                            tb.tbSysUser.Mobile = vm.TeacherEdit.Mobile;
                            tb.tbSysUser.Email = vm.TeacherEdit.Email;
                            tb.tbSysUser.Qq = vm.TeacherEdit.Qq;

                            if (vm.TeacherDeptIds.Length > 0)
                            {
                                var tbTeacherWithDeptList = new List<Entity.tbTeacherWithDept>();
                                var ids = new List<int>();
                                vm.TeacherDeptIds.Split(',').ToList().ForEach(a => { ids.Add(Convert.ToInt32(a)); });
                                var teacherDeptList = db.Table<Entity.tbTeacherDept>().Where(d => ids.Contains(d.Id)).ToList();
                                teacherDeptList.ForEach(t =>
                                {
                                    tbTeacherWithDeptList.Add(new Entity.tbTeacherWithDept()
                                    {
                                        tbTeacher = tb,
                                        tbTeacherDept = t
                                    });
                                });
                                db.Set<Entity.tbTeacherWithDept>().AddRange(tbTeacherWithDeptList);
                            }

                            db.Set<Teacher.Entity.tbTeacher>().Add(tb);

                            #region tbSysUserRole:新增角色成员
                            var tbUserRole = new Sys.Entity.tbSysUserRole();
                            tbUserRole.tbSysRole = db.Table<Sys.Entity.tbSysRole>().Where(d => d.RoleCode == Code.EnumHelper.SysRoleCode.Teacher).FirstOrDefault();
                            tbUserRole.tbSysUser = tb.tbSysUser;
                            db.Set<Sys.Entity.tbSysUserRole>().Add(tbUserRole);
                            #endregion

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加教师");
                                //刷新缓存
                                System.Web.HttpContext.Current.Cache["Power"] = Sys.Controllers.SysRolePowerController.GetPower();
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Teacher.Entity.tbTeacher>()
                                        .Include(d => d.tbSysUser)
                                          //.Include(d => d.tbTeacherDept)
                                      where p.Id == vm.TeacherEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.TeacherCode = vm.TeacherEdit.TeacherCode;
                                tb.TeacherName = vm.TeacherEdit.TeacherName;
                                tb.tbTeacherType = db.Set<Teacher.Entity.tbTeacherType>().Find(vm.TeacherEdit.TeacherTypeId);
                                //tb.tbTeacherDept = db.Set<Teacher.Entity.tbTeacherDept>().Find(vm.TeacherEdit.TeacherDeptId);

                                tb.tbDictEducation = db.Set<Dict.Entity.tbDictEducation>().Find(vm.TeacherEdit.EducationId);
                                tb.tbDictParty = db.Set<Dict.Entity.tbDictParty>().Find(vm.TeacherEdit.DictPartyId);
                                tb.tbDictNation = db.Set<Dict.Entity.tbDictNation>().Find(vm.TeacherEdit.DictNationId);
                                tb.tbDictMarriage = db.Set<Dict.Entity.tbDictMarriage>().Find(vm.TeacherEdit.DictMarriageId);
                                tb.tbDictRegion = db.Set<Dict.Entity.tbDictRegion>().Find(vm.TeacherEdit.DictRegionId);
                                tb.tbDictHealth = db.Set<Dict.Entity.tbDictHealth>().Find(vm.TeacherEdit.DictHealthId);
                                tb.Profile = vm.TeacherEdit.Profile;

                                tb.tbSysUser.UserCode = vm.TeacherEdit.TeacherCode;
                                tb.tbSysUser.UserName = vm.TeacherEdit.TeacherName;
                                tb.tbSysUser.tbSex = db.Set<Dict.Entity.tbDictSex>().Find(vm.TeacherEdit.SexId);
                                tb.tbSysUser.IdentityNumber = vm.TeacherEdit.IdentityNumber;
                                tb.tbSysUser.Mobile = vm.TeacherEdit.Mobile;
                                tb.tbSysUser.Email = vm.TeacherEdit.Email;
                                tb.tbSysUser.Qq = vm.TeacherEdit.Qq;

                                if (vm.TeacherDeptIds.Length > 0)
                                {
                                    //先删除此教师部门，再添加
                                    var tempList = db.Table<Entity.tbTeacherWithDept>()
                                        .Where(d => d.tbTeacher.Id == vm.TeacherEdit.Id).ToList();
                                    tempList.ForEach(t => { t.IsDeleted = true; });

                                    var tbTeacherWithDeptList = new List<Entity.tbTeacherWithDept>();
                                    var ids = new List<int>();
                                    vm.TeacherDeptIds.Split(',').ToList().ForEach(a => { ids.Add(Convert.ToInt32(a)); });
                                    var teacherDeptList = db.Table<Entity.tbTeacherDept>().Where(d => ids.Contains(d.Id)).ToList();
                                    teacherDeptList.ForEach(t =>
                                    {
                                        tbTeacherWithDeptList.Add(new Entity.tbTeacherWithDept()
                                        {
                                            tbTeacher = tb,
                                            tbTeacherDept = t
                                        });
                                    });
                                    db.Set<Entity.tbTeacherWithDept>().AddRange(tbTeacherWithDeptList);
                                }

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改教师");
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

        public ActionResult Import()
        {
            var vm = new Models.Teacher.Import();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.Teacher.Import vm)
        {
            vm.ImportList = new List<Dto.Teacher.Import>();
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

                    var tbList = new List<string>() { "教职工号", "教师姓名", "教师类型", "学历", "部门", "性别", "身份证号", "手机号码", "邮箱", "QQ", "政治面貌", "民族", "婚姻状况", "籍贯", "健康状况", "个人简介" };

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

                    //将DataTable转为List
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dto = new Dto.Teacher.Import()
                        {
                            TeacherCode = dr["教职工号"].ConvertToString(),
                            TeacherName = dr["教师姓名"].ConvertToString(),
                            TeacherTypeName = dr["教师类型"].ConvertToString(),
                            SexName = dr["性别"].ConvertToString(),
                            TeacherDeptName = dr["部门"].ConvertToString().Split(','),
                            IdentityNumber = dr["身份证号"].ConvertToString(),
                            Mobile = dr["手机号码"].ConvertToString(),
                            Email = dr["邮箱"].ConvertToString(),
                            Qq = dr["QQ"].ConvertToString(),
                            EducationName = dr["学历"].ConvertToString(),
                            DictPartyName = dr["政治面貌"].ConvertToString(),
                            DictNationName = dr["民族"].ConvertToString(),
                            DictMarriageName = dr["婚姻状况"].ConvertToString(),
                            DictRegionName = dr["籍贯"].ConvertToString(),
                            DictHealthName = dr["健康状况"].ConvertToString(),
                            Profile = dr["个人简介"].ConvertToString(),
                        };
                        vm.ImportList.Add(dto);
                    }

                    //清空无效的空数据
                    vm.ImportList.RemoveAll(d =>
                           string.IsNullOrEmpty(d.TeacherCode)
                        && string.IsNullOrEmpty(d.TeacherName)
                        && string.IsNullOrEmpty(d.TeacherTypeName)
                        && d.TeacherDeptName.Length == 0
                        && string.IsNullOrEmpty(d.SexName)
                        && string.IsNullOrEmpty(d.IdentityNumber)
                        && string.IsNullOrEmpty(d.Mobile)
                        && string.IsNullOrEmpty(d.Email)
                        && string.IsNullOrEmpty(d.Qq));

                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }

                    var sexList = (from p in db.Table<Dict.Entity.tbDictSex>()
                                   select p).ToList();
                    var teacherList = (from p in db.Table<Teacher.Entity.tbTeacher>()
                                        .Include(d => d.tbSysUser)
                                       select p).ToList();
                    var teacherTypeList = db.Table<Teacher.Entity.tbTeacherType>().ToList();
                    var teacherDeptList = db.Table<Teacher.Entity.tbTeacherDept>().ToList();
                    var teacherRole = db.Table<Sys.Entity.tbSysRole>().Where(d => d.RoleCode == Code.EnumHelper.SysRoleCode.Teacher).FirstOrDefault();
                    //tbDictParty\tbDictNation\tbDictMarriage\tbDictRegion\tbDictHealth\Profile
                    var dictPartyList = db.Table<Dict.Entity.tbDictParty>().ToList();
                    var dictNationList = db.Table<Dict.Entity.tbDictNation>().ToList();
                    var dictMarriageList = db.Table<Dict.Entity.tbDictMarriage>().ToList();
                    var dictRegionList = db.Table<Dict.Entity.tbDictRegion>().ToList();
                    var dictHealthList = db.Table<Dict.Entity.tbDictHealth>().ToList();
                    var educationList = db.Table<Dict.Entity.tbDictEducation>().ToList();

                    #region 验证数据格式是否正确
                    foreach (var item in vm.ImportList)
                    {
                        if (!string.IsNullOrEmpty(item.EducationName)
                            && educationList.Where(d => d.EducationName == item.EducationName).Any() == false)
                        {
                            item.Error += "学历不存在；";
                        }

                        if (!string.IsNullOrEmpty(item.DictNationName) && dictNationList.Where(d => d.NationName == item.DictNationName).Count() == 0)
                        {
                            item.Error += "民族不存在；";
                        }

                        if (!string.IsNullOrEmpty(item.DictMarriageName) && dictMarriageList.Where(d => d.MarriageName == item.DictMarriageName).Count() == 0)
                        {
                            item.Error += "婚姻状况不存在；";
                        }

                        if (!string.IsNullOrEmpty(item.DictRegionName) && dictRegionList.Where(d => d.RegionName == item.DictRegionName).Count() == 0)
                        {
                            item.Error += "籍贯不存在；";
                        }

                        if (!string.IsNullOrEmpty(item.DictHealthName) && dictHealthList.Where(d => d.HealthName == item.DictHealthName).Count() == 0)
                        {
                            item.Error += "健康状况不存在；";
                        }

                        if (!string.IsNullOrEmpty(item.DictPartyName) && dictPartyList.Where(d => d.PartyName == item.DictPartyName).Count() == 0)
                        {
                            item.Error += "党派不存在；";
                        }

                        if (string.IsNullOrEmpty(item.TeacherCode))
                        {
                            item.Error = item.Error + "教职工号不能为空!";
                        }
                        else
                        {
                            var regex = new Regex(Code.Common.RegUserCode);
                            if (regex.IsMatch(item.TeacherCode) == false)
                            {
                                item.Error = item.Error + "教职工号：只允许输入中英文字符、数字和_或-，特殊字符都不能包含!";
                            }
                        }

                        if (string.IsNullOrEmpty(item.TeacherTypeName) == false && teacherTypeList.Where(d => d.TeacherTypeName == item.TeacherTypeName).Any() == false)
                        {
                            item.Error += "教师类型不存在！";
                        }

                        if (item.TeacherDeptName.Length > 0)
                        {
                            foreach (var v in item.TeacherDeptName)
                            {
                                if (string.IsNullOrEmpty(v) == false && teacherDeptList.Where(d => d.TeacherDeptName == v).Any() == false)
                                {
                                    item.Error += "部门不存在！";
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(item.TeacherName))
                        {
                            item.Error = item.Error + "教师姓名不能为空!";
                        }
                        else
                        {
                            var regex = new Regex(Code.Common.RegUserName);
                            if (regex.IsMatch(item.TeacherName) == false)
                            {
                                item.Error = item.Error + "教职姓名：只允许输入中英文字符、数字和_或-，特殊字符都不能包含!";
                            }
                        }

                        #region 格式校验
                        if (string.IsNullOrEmpty(item.IdentityNumber) == false)
                        {
                            var regex = new Regex(Code.Common.RegIdentityNumber);
                            if (regex.IsMatch(item.IdentityNumber) == false)
                            {
                                item.Error = item.Error + "身份证号格式不正确!";
                            }
                        }

                        if (string.IsNullOrEmpty(item.Mobile) == false)
                        {
                            var regex = new Regex(Code.Common.RegMobil);
                            if (regex.IsMatch(item.Mobile) == false)
                            {
                                item.Error = item.Error + "手机号码格式不正确!";
                            }
                        }

                        if (string.IsNullOrEmpty(item.Email) == false)
                        {
                            var regex = new Regex(Code.Common.RegEmail);
                            if (regex.IsMatch(item.Email) == false)
                            {
                                item.Error = item.Error + "邮箱格式不正确!";
                            }
                        }

                        if (string.IsNullOrEmpty(item.Qq) == false)
                        {
                            var regex = new Regex(Code.Common.RegQQNumber);
                            if (regex.IsMatch(item.Qq) == false)
                            {
                                item.Error = item.Error + "QQ号格式不正确!";
                            }
                        }
                        #endregion

                        if (vm.ImportList.Where(d => d.TeacherCode == item.TeacherCode).Count() > 1)
                        {
                            item.Error = item.Error + "该条数据重复!";
                        }

                        if (string.IsNullOrEmpty(item.SexName) == false && sexList.Where(d => d.SexName == item.SexName).Count() == 0)
                        {
                            item.Error = item.Error + "性别格式不正确!";
                        }

                        if (vm.IsUpdate == false && teacherList.Where(d => d.TeacherCode == item.TeacherCode).Count() > 0)
                        {
                            item.Error = item.Error + "系统中已存在该记录!";
                        }
                    }

                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 数据导入，判断关键字，存在的数据做更新，不存在的做新增

                    var myTeacherList = new List<Teacher.Entity.tbTeacher>();
                    var myUserRoleList = new List<Sys.Entity.tbSysUserRole>();
                    var myTeacherWithDeptList = new List<Entity.tbTeacherWithDept>();
                    foreach (var item in vm.ImportList)
                    {
                        if (vm.IsUpdate && teacherList.Where(d => d.TeacherCode == item.TeacherCode).Count() > 0)
                        {
                            if (teacherList.Where(d => d.TeacherCode == item.TeacherCode).Count() > 1)
                            {
                                item.Error = item.Error + "系统中该教职工号数据存在重复，无法确认需要更新的记录!";
                                vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                                return View(vm);
                            }

                            var tb = teacherList.Where(d => d.TeacherCode == item.TeacherCode).FirstOrDefault();
                            tb.TeacherCode = item.TeacherCode;
                            tb.TeacherName = item.TeacherName;
                            if (string.IsNullOrEmpty(item.TeacherTypeName) == false)
                            {
                                tb.tbTeacherType = teacherTypeList.Where(d => d.TeacherTypeName == item.TeacherTypeName).FirstOrDefault();
                            }
                            foreach (var v in item.TeacherDeptName)
                            {
                                if (string.IsNullOrEmpty(v) == false)
                                {
                                    var teacherWithDept = new Entity.tbTeacherWithDept()
                                    {
                                        tbTeacher = tb,
                                        tbTeacherDept = teacherDeptList.Where(d => d.TeacherDeptName == v).FirstOrDefault()
                                    };
                                    myTeacherWithDeptList.Add(teacherWithDept);
                                }
                            }
                            tb.Profile = item.Profile;
                            if (!string.IsNullOrEmpty(item.DictPartyName))
                            {
                                tb.tbDictParty = dictPartyList.Where(d => d.PartyName == item.DictPartyName).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.DictNationName))
                            {
                                tb.tbDictNation = dictNationList.Where(d => d.NationName == item.DictNationName).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.DictMarriageName))
                            {
                                tb.tbDictMarriage = dictMarriageList.Where(d => d.MarriageName == item.DictMarriageName).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.DictRegionName))
                            {
                                tb.tbDictRegion = dictRegionList.Where(d => d.RegionName == item.DictRegionName).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.DictHealthName))
                            {
                                tb.tbDictHealth = dictHealthList.Where(d => d.HealthName == item.DictHealthName).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.EducationName))
                            {
                                tb.tbDictEducation = educationList.Where(d => d.EducationName == item.EducationName).FirstOrDefault();
                            }

                            tb.tbSysUser.UserCode = item.TeacherCode;
                            tb.tbSysUser.UserName = item.TeacherName;
                            tb.tbSysUser.UserType = Code.EnumHelper.SysUserType.Teacher;
                            tb.tbSysUser.tbSex = sexList.Where(d => d.SexName == item.SexName).FirstOrDefault();
                            tb.tbSysUser.IdentityNumber = item.IdentityNumber;
                            tb.tbSysUser.Mobile = item.Mobile;
                            tb.tbSysUser.Email = item.Email;
                            tb.tbSysUser.Qq = item.Qq;
                        }
                        else
                        {
                            var tb = new Teacher.Entity.tbTeacher();
                            tb.TeacherCode = item.TeacherCode;
                            tb.TeacherName = item.TeacherName;
                            if (string.IsNullOrEmpty(item.TeacherTypeName) == false)
                            {
                                tb.tbTeacherType = teacherTypeList.Where(d => d.TeacherTypeName == item.TeacherTypeName).FirstOrDefault();
                            }
                            foreach (var v in item.TeacherDeptName)
                            {
                                if (string.IsNullOrEmpty(v) == false)
                                {
                                    var teacherWithDept = new Entity.tbTeacherWithDept()
                                    {
                                        tbTeacher = tb,
                                        tbTeacherDept = teacherDeptList.Where(d => d.TeacherDeptName == v).FirstOrDefault()
                                    };
                                    myTeacherWithDeptList.Add(teacherWithDept);
                                }
                            }
                            tb.Profile = item.Profile;
                            if (!string.IsNullOrEmpty(item.DictPartyName))
                            {
                                tb.tbDictParty = dictPartyList.Where(d => d.PartyName == item.DictPartyName).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.DictNationName))
                            {
                                tb.tbDictNation = dictNationList.Where(d => d.NationName == item.DictNationName).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.DictMarriageName))
                            {
                                tb.tbDictMarriage = dictMarriageList.Where(d => d.MarriageName == item.DictMarriageName).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.DictRegionName))
                            {
                                tb.tbDictRegion = dictRegionList.Where(d => d.RegionName == item.DictRegionName).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.DictHealthName))
                            {
                                tb.tbDictHealth = dictHealthList.Where(d => d.HealthName == item.DictHealthName).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.EducationName))
                            {
                                tb.tbDictEducation = educationList.Where(d => d.EducationName == item.EducationName).FirstOrDefault();
                            }

                            tb.tbSysUser = new Sys.Entity.tbSysUser();
                            tb.tbSysUser.UserCode = item.TeacherCode;
                            tb.tbSysUser.UserName = item.TeacherName;
                            tb.tbSysUser.Password = Code.Common.DESEnCode("123456");
                            tb.tbSysUser.PasswordMd5 = Code.Common.CreateMD5Hash("123456");
                            tb.tbSysUser.UserType = Code.EnumHelper.SysUserType.Teacher;
                            tb.tbSysUser.tbSex = sexList.Where(d => d.SexName == item.SexName).FirstOrDefault();
                            tb.tbSysUser.IdentityNumber = item.IdentityNumber;
                            tb.tbSysUser.Mobile = item.Mobile;
                            tb.tbSysUser.Email = item.Email;
                            tb.tbSysUser.Qq = item.Qq;

                            myTeacherList.Add(tb);

                            #region tbSysUserRole:新增角色成员
                            var tbUserRole = new Sys.Entity.tbSysUserRole();
                            tbUserRole.tbSysRole = teacherRole;
                            tbUserRole.tbSysUser = tb.tbSysUser;
                            myUserRoleList.Add(tbUserRole);
                            #endregion
                        }
                    }
                    #endregion

                    db.Set<Teacher.Entity.tbTeacher>().AddRange(myTeacherList);
                    db.Set<Sys.Entity.tbSysUserRole>().AddRange(myUserRoleList);
                    db.Set<Entity.tbTeacherWithDept>().AddRange(myTeacherWithDeptList);

                    if (db.SaveChanges() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入教师资料");
                        //刷新缓存
                        System.Web.HttpContext.Current.Cache["Power"] = Sys.Controllers.SysRolePowerController.GetPower();
                        vm.Status = true;
                    }
                }
            }

            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Teacher/Views/Teacher/Teacher.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        public ActionResult Export(string searchText = "")
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();
                var tb = from p in db.Table<Teacher.Entity.tbTeacher>()
                         select p;
                if (string.IsNullOrEmpty(searchText) == false)
                {
                    tb = tb.Where(d => d.TeacherCode.Contains(searchText) || d.TeacherName.Contains(searchText));
                }
                var teacherList = (from p in tb
                                   orderby p.TeacherName
                                   select new
                                   {
                                       #region
                                       p.Id,
                                       p.TeacherCode,
                                       p.TeacherName,
                                       p.tbTeacherType.TeacherTypeName,
                                       TeacherDeptName = "",
                                       p.tbSysUser.tbSex.SexName,
                                       p.tbSysUser.IdentityNumber,
                                       p.tbSysUser.Mobile,
                                       p.tbSysUser.Email,
                                       p.tbSysUser.Qq,
                                       p.tbDictHealth.HealthName,
                                       p.tbDictMarriage.MarriageName,
                                       p.tbDictNation.NationName,
                                       p.tbDictParty.PartyName,
                                       p.tbDictEducation.EducationName,
                                       p.tbDictRegion.RegionName,
                                       p.Profile
                                       #endregion
                                   }).ToList();

                var tIds = teacherList.Select(d => d.Id).ToList();
                var teacherWithDeptList = db.Table<Entity.tbTeacherWithDept>()
                    .Where(d => tIds.Contains(d.tbTeacher.Id))
                    .Select(d => new { d.tbTeacher.Id, d.tbTeacherDept.TeacherDeptName }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        #region
                        new System.Data.DataColumn("教职工号"),
                        new System.Data.DataColumn("教师姓名"),
                        new System.Data.DataColumn("教师类型"),
                        new System.Data.DataColumn("学历"),
                        new System.Data.DataColumn("部门"),
                        new System.Data.DataColumn("性别"),
                        new System.Data.DataColumn("身份证号"),
                        new System.Data.DataColumn("手机号码"),
                        new System.Data.DataColumn("邮箱"),
                        new System.Data.DataColumn("QQ"),
                        new System.Data.DataColumn("政治面貌"),
                        new System.Data.DataColumn("民族"),
                        new System.Data.DataColumn("婚姻状况"),
                        new System.Data.DataColumn("籍贯"),
                        new System.Data.DataColumn("健康状况"),
                        new System.Data.DataColumn("个人简介"),
                        #endregion
                    });
                foreach (var a in teacherList)
                {
                    var dr = dt.NewRow();
                    dr["教职工号"] = a.TeacherCode;
                    dr["学历"] = a.EducationName;
                    dr["教师姓名"] = a.TeacherName;
                    dr["教师类型"] = a.TeacherTypeName;
                    dr["部门"] = string.Join(",", teacherWithDeptList.Where(d => d.Id == a.Id).Select(d => d.TeacherDeptName).ToList());
                    dr["性别"] = a.SexName;
                    dr["身份证号"] = a.IdentityNumber;
                    dr["手机号码"] = a.Mobile;
                    dr["邮箱"] = a.Email;
                    dr["QQ"] = a.Qq;
                    dr["政治面貌"] = a.PartyName;
                    dr["民族"] = a.NationName;
                    dr["婚姻状况"] = a.MarriageName;
                    dr["籍贯"] = a.RegionName;
                    dr["健康状况"] = a.HealthName;
                    dr["个人简介"] = a.Profile;
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

        public ActionResult TeacherDeptImport()
        {
            var vm = new Models.Teacher.TeacherDeptImport();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeacherDeptImport(Models.Teacher.TeacherDeptImport vm)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);

                using (var db = new XkSystem.Models.DbContext())
                {
                    #region 上传excel文件,并转为DataTable
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

                    var tbList = new List<string>() { "教师编号", "教师名称", "部门名称" };

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
                    #endregion

                    #region 将DataTable转为List
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dto = new Dto.Teacher.TeacherDeptImport()
                        {
                            TeacherCode = dr["教师编号"].ConvertToString(),
                            TeacherName = dr["教师名称"].ConvertToString(),
                            TeacherDeptName = dr["部门名称"].ConvertToString()
                        };
                        if (vm.ImportList.Where(d => d.TeacherCode == dto.TeacherCode
                                                 && d.TeacherName == dto.TeacherName
                                                 && d.TeacherDeptName == dto.TeacherDeptName).Count() == 0)
                        {
                            vm.ImportList.Add(dto);
                        }
                    }

                    vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.TeacherCode)
                                            && string.IsNullOrEmpty(d.TeacherName)
                                            && string.IsNullOrEmpty(d.TeacherDeptName));

                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }
                    #endregion

                    var teacherDeptList = db.Table<Teacher.Entity.tbTeacherDept>().ToList();
                    var teacherList = db.Table<Teacher.Entity.tbTeacher>().ToList();
                    var teacherWithDeptList = db.Table<Entity.tbTeacherWithDept>()
                        .Include(d => d.tbTeacher).Include(d => d.tbTeacherDept).ToList();

                    #region 验证数据格式是否正确
                    foreach (var v in vm.ImportList)
                    {
                        if (string.IsNullOrEmpty(v.TeacherDeptName))
                        {
                            v.Error += "部门名称不能为空；";
                        }

                        if (string.IsNullOrEmpty(v.TeacherCode))
                        {
                            v.Error += "教师编号不能为空；";
                        }

                        if (string.IsNullOrEmpty(v.TeacherName))
                        {
                            v.Error += "教师名称不能为空；";
                        }

                        if (teacherList.Where(d => d.TeacherCode == v.TeacherCode && d.TeacherName == v.TeacherName).Count() == 0)
                        {
                            v.Error += "教师不存在；";
                        }

                        if (teacherDeptList.Where(d => d.TeacherDeptName == v.TeacherDeptName).Count() == 0)
                        {
                            v.Error += "部门不存在；";
                        }
                    }

                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 数据导入，判断关键字，存在的数据做更新，不存在的做新增
                    var tbTeacherWithDeptList = new List<Entity.tbTeacherWithDept>();
                    foreach (var item in vm.ImportList)
                    {
                        if (teacherWithDeptList.Where(d => d.tbTeacher.TeacherCode == item.TeacherCode && d.tbTeacherDept.TeacherDeptName == item.TeacherDeptName).Any() == false)
                        {
                            var teacherWithDept = new Entity.tbTeacherWithDept()
                            {
                                tbTeacher = teacherList.Where(d => d.TeacherCode == item.TeacherCode && d.TeacherName == item.TeacherName).FirstOrDefault(),
                                tbTeacherDept = teacherDeptList.Where(d => d.TeacherDeptName == item.TeacherDeptName).FirstOrDefault()
                            };
                            tbTeacherWithDeptList.Add(teacherWithDept);
                        }
                    }
                    #endregion

                    db.Set<Entity.tbTeacherWithDept>().AddRange(tbTeacherWithDeptList);

                    if (db.SaveChanges() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入教师部门");
                        vm.Status = true;
                    }
                }
            }

            return View(vm);
        }

        public ActionResult TeacherDeptImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Teacher/Views/Teacher/TeacherDeptImport.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(string SearchText = "", int teacherId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = from p in db.Table<Teacher.Entity.tbTeacher>()
                         select p;

                if (string.IsNullOrEmpty(SearchText) == false)
                {
                    tb = tb.Where(d => d.TeacherCode.Contains(SearchText) || d.TeacherName.Contains(SearchText));
                }

                if (teacherId != 0)
                {
                    tb = tb.Where(d => d.Id == teacherId);
                }

                var list = (from p in tb
                            orderby p.TeacherName
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.TeacherName,
                                Value = p.Id.ToString()
                            }).ToList();
                return list;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectListBySelected(string teacherIds)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Teacher.Entity.tbTeacher>()
                          select new Dto.Teacher.List
                          {
                              Id = p.Id,
                              TeacherName = p.TeacherName,
                              SysUserId = 0
                          }).ToList();

                if (string.IsNullOrEmpty(teacherIds) == false)
                {
                    var teacherList = teacherIds.Split(',').Select(d => d.ConvertToInt()).ToList();
                    foreach (var a in teacherList)
                    {
                        if (tb.Where(d => d.Id == a).Count() > decimal.Zero)
                        {
                            tb.Where(d => d.Id == a).ToList().ForEach(p =>
                             {
                                 p.SysUserId = 1;
                             });
                        }
                    }
                }

                var list = (from p in tb
                            orderby p.SysUserId descending, p.TeacherName ascending
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.TeacherName,
                                Value = p.Id.ToString()
                            }).ToList();
                return list;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList1(int teacherId = 0)
        {
            var list = new List<System.Web.Mvc.SelectListItem>();
            using (var db = new XkSystem.Models.DbContext())
            {
                list = (from p in db.Table<Teacher.Entity.tbTeacher>()
                        orderby p.No
                        select new System.Web.Mvc.SelectListItem
                        {
                            Text = p.TeacherName,
                            Value = p.Id.ToString()
                        }).ToList();

                if (teacherId > 0)
                {
                    list.Where(d => d.Value == teacherId.ConvertToString()).FirstOrDefault().Selected = true;
                }
            }
            return list;
        }

        [NonAction]
        public static List<Dto.Teacher.Info> SelectInfoList(string SearchText = "", int teacherId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = from p in db.Table<Teacher.Entity.tbTeacher>()
                         select p;

                if (string.IsNullOrEmpty(SearchText) == false)
                {
                    tb = tb.Where(d => d.TeacherCode.Contains(SearchText) || d.TeacherName.Contains(SearchText));
                }

                if (teacherId != 0)
                {
                    tb = tb.Where(d => d.Id == teacherId);
                }

                var list = (from p in tb
                            orderby p.TeacherName
                            select new Dto.Teacher.Info
                            {
                                Id = p.Id,
                                TeacherCode = p.TeacherCode,
                                TeacherName = p.TeacherName
                            }).ToList();
                return list;
            }
        }

        [NonAction]
        public static List<Dto.Teacher.Info> GetTeacherById(int teacherId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Teacher.Entity.tbTeacher>()
                          where p.Id == teacherId
                          orderby p.TeacherCode
                          select new Dto.Teacher.Info
                          {
                              Id = p.Id,
                              TeacherCode = p.TeacherCode,
                              TeacherName = p.TeacherName
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static Dto.Teacher.Info GetTeacherByUserId(int userId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Teacher.Entity.tbTeacher>()
                          where p.tbSysUser.Id == userId
                          orderby p.TeacherCode
                          select new Dto.Teacher.Info
                          {
                              Id = p.Id,
                              TeacherCode = p.TeacherCode,
                              TeacherName = p.TeacherName
                          }).FirstOrDefault();
                return tb;
            }
        }

        [NonAction]
        public static List<Dto.Teacher.Info> GetTeacherByClassId(int classId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                          where p.tbClass.Id == classId
                            && p.tbTeacher.IsDeleted == false
                          orderby p.tbTeacher.No
                          select new Dto.Teacher.Info
                          {
                              Id = p.tbTeacher.Id,
                              TeacherCode = p.tbTeacher.TeacherCode,
                              TeacherName = p.tbTeacher.TeacherName
                          }).ToList();
                return tb;
            }
        }

        public ActionResult SelectTeacher()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Teacher.SelectTeacher();
                var tb = from p in db.Table<Teacher.Entity.tbTeacher>()
                         select p;

                if (vm.TeacherDeptId != 0)
                {
                    var tIds = db.Table<Entity.tbTeacherWithDept>().Where(d => d.tbTeacherDept.Id == vm.TeacherDeptId || d.tbTeacherDept.tbTeacherDeptParent.Id == vm.TeacherDeptId).Select(d => d.tbTeacher.Id).ToList();
                    tb = tb.Where(d => tIds.Contains(d.Id));
                    //tb = tb.Where(d => (d.tbTeacherDept.Id == vm.TeacherDeptId || d.tbTeacherDept.tbTeacherDeptParent.Id == vm.TeacherDeptId));
                }

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.TeacherCode.Contains(vm.SearchText) || d.TeacherName.Contains(vm.SearchText));
                }

                vm.TeacherList = (from p in tb
                                  orderby p.TeacherName
                                  select new Dto.Teacher.SelectTeacher
                                  {
                                      Id = p.Id,
                                      TeacherCode = p.TeacherCode,
                                      //TeacherDeptName = p.tbTeacherDept.TeacherDeptName,
                                      TeacherName = p.TeacherName,
                                      SexName = p.tbSysUser.tbSex.SexName,
                                      IdentityNumber = p.tbSysUser.IdentityNumber,
                                      Mobile = p.tbSysUser.Mobile,
                                      Email = p.tbSysUser.Email,
                                      Qq = p.tbSysUser.Qq,
                                      SysUserId = p.tbSysUser.Id,
                                  }).ToPageList(vm.Page);

                //查询TeacherDeptName
                var ids = vm.TeacherList.Select(d => d.Id).ToList();
                var teacherWithDeptList = db.Table<Entity.tbTeacherWithDept>()
                    .Where(d => ids.Contains(d.tbTeacher.Id))
                    .Select(d => new { d.tbTeacher.Id, d.tbTeacherDept.TeacherDeptName }).ToList();
                foreach (var v in vm.TeacherList)
                {
                    v.TeacherDeptName = string.Join(",", teacherWithDeptList.Where(d => d.Id == v.Id).Select(d => d.TeacherDeptName).ToList());
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectTeacher(Models.Teacher.SelectTeacher vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SelectTeacher", new
            {
                searchText = vm.SearchText,
                teacherDeptId = vm.TeacherDeptId,
                TeacherDeptName = vm.TeacherDeptName,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        /// <summary>
        /// 根据班主任ID获取学生
        /// </summary>
        public static List<Student.Entity.tbStudent> GetStudentForTeacher(int teacherId)
        {
            var studentList = new List<Student.Entity.tbStudent>();
            using (var db = new XkSystem.Models.DbContext())
            {
                var classTeacherList = db.Table<Basis.Entity.tbClassTeacher>()
                    .Where(d => d.tbTeacher.Id == teacherId).Include(d => d.tbClass).ToList();
                var classIdList = new List<int>();
                foreach (var v in classTeacherList)
                {
                    classIdList.Add(v.tbClass.Id);
                }

                var classStudentList = db.Table<Basis.Entity.tbClassStudent>().Where(d => classIdList.Contains(d.tbClass.Id))
                    .Include(d => d.tbStudent).ToList();
                foreach (var v in classStudentList)
                {
                    studentList.Add(v.tbStudent);
                }
            }
            return studentList;
        }

        public static List<Teacher.Entity.tbTeacher> SelectTeacher(string SearchText = null)
        {
            var tb = new List<Teacher.Entity.tbTeacher>();

            using (var db = new XkSystem.Models.DbContext())
            {
                if (!string.IsNullOrEmpty(SearchText))
                {
                    tb = db.Table<Teacher.Entity.tbTeacher>()
                        .Where(d => d.TeacherCode.Contains(SearchText) || d.TeacherName.Contains(SearchText)).ToList();
                }
            }

            return tb;
        }

        [AllowAnonymous]
        public ActionResult GetTeacher(string q)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                q = q.ConvertToString();
                var tb = (from p in db.Table<Teacher.Entity.tbTeacher>()
                          where (p.TeacherCode.Contains(q) || p.TeacherName.Contains(q))
                          orderby p.No
                          select p.TeacherCode + "(" + p.TeacherName + ")").Take(10).ToList();

                return Json(tb, JsonRequestBehavior.AllowGet);
            }
        }
    }
}