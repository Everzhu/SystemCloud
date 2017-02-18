using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Sys.Controllers
{
    public class SysMessageController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysMessage.List();

                var tb = from p in db.Table<Sys.Entity.tbSysMessage>()
                         where p.IsPermit == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.MessageTitle.Contains(vm.SearchText));
                }

                vm.MessageList = (from p in tb
                                  orderby p.InputDate descending
                                  select new Dto.SysMessage.List
                                  {
                                      Id = p.Id,
                                      InputDate = p.InputDate,
                                      IsEmail = p.IsEmail,
                                      IsSms = p.IsSms,
                                      MessageContent = p.MessageContent,
                                      MessageTitle = p.MessageTitle,
                                      SysUserName = p.tbSysUser.UserName,
                                      Url = p.Url
                                  }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.SysMessage.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, pageIndex = vm.Page.PageIndex, pageSize = vm.Page.PageSize }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysMessage>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                var messageUserList = (from p in db.Table<Sys.Entity.tbSysMessageUser>()
                                        .Include(d => d.tbSysMessage)
                                       where ids.Contains(p.tbSysMessage.Id)
                                       select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    foreach (var user in messageUserList.Where(d => d.tbSysMessage.Id == a.Id))
                    {
                        user.IsDeleted = true;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除消息");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysMessage.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Sys.Entity.tbSysMessage>()
                              where p.Id == id
                              select new Dto.SysMessage.Edit
                              {
                                  Id = p.Id,
                                  InputDate = p.InputDate,
                                  IsEmail = p.IsEmail,
                                  IsSms = p.IsSms,
                                  MessageContent = p.MessageContent,
                                  MessageTitle = p.MessageTitle,
                                  SysUserId = p.tbSysUser.Id,
                                  Url = p.Url,
                                  IsPublic = p.IsPublic
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        if (!tb.IsPublic)
                        {
                            vm.RoleIds = string.Join(",", (from p in db.Table<Entity.tbSysMessageRole>() where p.tbSysMessage.Id == tb.Id select p.tbSysRole.Id).ToList());
                        }
                        vm.MessageEdit = tb;
                    }
                }
                vm.RoleList = SysRoleController.SelectList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Models.SysMessage.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.MessageEdit.Id == 0)
                    {
                        var tb = new Sys.Entity.tbSysMessage();
                        tb.MessageTitle = vm.MessageEdit.MessageTitle;
                        tb.tbProgram = db.Set<Admin.Entity.tbProgram>().Find(Code.Common.ProgramId);
                        //tb.IsSms = vm.MessageEdit.IsSms;
                        //tb.IsEmail = vm.MessageEdit.IsEmail;
                        //tb.Url = vm.MessageEdit.Url;
                        tb.MessageContent = vm.MessageEdit.MessageContent;
                        tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                        tb.InputDate = DateTime.Now;
                        tb.IsPublic = vm.MessageEdit.IsPublic;
                        db.Set<Sys.Entity.tbSysMessage>().Add(tb);

                        if (!vm.MessageEdit.IsPublic)
                        {
                            var roleIds = vm.RoleIds.Split(',').ToList();
                            roleIds.RemoveAll(p => string.IsNullOrWhiteSpace(p));
                            if (roleIds != null || roleIds.Count > 0)
                            {
                                var tbSysRoleList = (from p in db.Table<Entity.tbSysRole>() where roleIds.Contains(p.Id.ToString()) select p);
                                var tbSysMessageRole = vm.RoleIds.Split(',').Select(p => new Entity.tbSysMessageRole()
                                {
                                    tbSysMessage = tb,
                                    tbSysRole = tbSysRoleList.First(r => r.Id.ToString() == p)
                                }).ToList();
                                db.Set<Entity.tbSysMessageRole>().AddRange(tbSysMessageRole);
                            }
                        }

                        if (db.SaveChanges() > 0)
                        {
                            SysUserLogController.Insert("添加消息");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Sys.Entity.tbSysMessage>()
                                  where p.Id == vm.MessageEdit.Id
                                  select p).FirstOrDefault();
                        var oldIsPublic = tb.IsPublic;
                        if (tb != null)
                        {
                            tb.MessageTitle = vm.MessageEdit.MessageTitle;
                            //tb.IsSms = vm.MessageEdit.IsSms;
                            //tb.IsEmail = vm.MessageEdit.IsEmail;
                            //tb.Url = vm.MessageEdit.Url;
                            tb.MessageContent = vm.MessageEdit.MessageContent;
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            tb.InputDate = DateTime.Now;
                            tb.IsPublic = vm.MessageEdit.IsPublic;
                            if (vm.MessageEdit.IsPublic)
                            {
                                if (!oldIsPublic)   //非公开变成公开
                                {
                                    var tbSysMessageRole = (from p in db.Table<Entity.tbSysMessageRole>() where p.tbSysMessage.Id == tb.Id select p);
                                    foreach (var item in tbSysMessageRole)
                                    {
                                        item.IsDeleted = true;
                                        item.UpdateTime = DateTime.Now;
                                    }
                                }
                            }
                            else
                            {
                                var roleIds = vm.RoleIds.Split(',').ToList();
                                roleIds.RemoveAll(p => string.IsNullOrWhiteSpace(p));
                                if (roleIds != null || roleIds.Count > 0)
                                {
                                    var existsRoleId = (from p in db.Table<Entity.tbSysMessageRole>() where p.tbSysMessage.Id == tb.Id select p.tbSysRole.Id).ToList();
                                    roleIds.RemoveAll(p => existsRoleId.Contains(p.ConvertToInt()));

                                    if (roleIds != null || roleIds.Count > 0)
                                    {
                                        var tbSysRoleList = (from p in db.Table<Entity.tbSysRole>() where roleIds.Contains(p.Id.ToString()) select p);
                                        var tbSysMessageRole = vm.RoleIds.Split(',').Select(p => new Entity.tbSysMessageRole()
                                        {
                                            tbSysMessage = tb,
                                            tbSysRole = tbSysRoleList.First(r => r.Id.ToString() == p)
                                        }).ToList();
                                        db.Set<Entity.tbSysMessageRole>().AddRange(tbSysMessageRole);
                                    }
                                }
                            }
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改消息");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }

                return Code.MvcHelper.Post(error, Url.Action("List"), "提交成功!");
            }
        }

        public ActionResult Details(int id = 0, int mesId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysMessage.Info();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Sys.Entity.tbSysMessage>()
                              where p.Id == id
                              select new Dto.SysMessage.Info
                              {
                                  Id = p.Id,
                                  InputDate = p.InputDate,
                                  IsEmail = p.IsEmail,
                                  IsSms = p.IsSms,
                                  MessageContent = p.MessageContent,
                                  MessageTitle = p.MessageTitle,
                                  SysUserName = p.tbSysUser.UserName,
                                  Url = p.Url
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        var toMesUser = (from p in db.Table<Sys.Entity.tbSysMessageUser>()
                                         where p.tbSysMessage.Id == id
                                         && p.tbSysMessage.IsDeleted == false
                                         && p.tbSysUser.IsDeleted == false
                                         select new
                                         {
                                             userName = p.tbSysUser.UserName
                                         }).ToList();

                        foreach (var a in toMesUser)
                        {
                            tb.ToUserName = string.Join(",", toMesUser.Select(d => d.userName));
                        }
                        vm.MessageInfo = tb;
                    }

                    if (mesId != 0)
                    {
                        var tbMesUser = (from p in db.Table<Sys.Entity.tbSysMessageUser>()
                                         where p.Id == mesId && p.tbSysMessage.Id == id
                                         select p).ToList();
                        foreach (var msg in tbMesUser)
                        {
                            if (!msg.IsRead)
                            {
                                msg.IsRead = true;
                                msg.ReadDate = DateTime.Now;
                            }
                        }
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("查阅私信");
                        }
                    }
                }

                return View(vm);
            }
        }

        public ActionResult MessageUserList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysMessageUser.List();
                vm.MessageUserList = (from p in db.Table<Sys.Entity.tbSysMessageUser>()
                                      where p.tbSysUser.UserName.Contains(vm.SearchText)
                                      orderby p.tbSysUser.UserName
                                      select new Dto.SysMessageUser.List
                                      {
                                          Id = p.Id,
                                          IsRead = p.IsRead,
                                          ReadDate = p.ReadDate,
                                          SysMessageName = p.tbSysMessage.MessageContent,
                                          SysUserName = p.tbSysUser.UserName
                                      }).ToList();
                return View(vm);
            }
        }

        public ActionResult MessageUserDelete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysMessageUser>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除消息接收人");
                }

                return Code.MvcHelper.Post(null, Url.Action("MessageUserList"));
            }
        }

        public string MessageUserInsert(Models.SysMessageUser.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = new Sys.Entity.tbSysMessageUser();
                tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(vm.MessageUserEdit.SysUserId);
                tb.IsRead = false;
                db.Set<Sys.Entity.tbSysMessageUser>().Add(tb);
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加消息接收人");
                }

                return string.Empty;
            }
        }
        /// <summary>
        /// 私信列表
        /// </summary>
        /// <returns></returns>
        public ActionResult PrivateMessageList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysMessage.PrivateMessageList();

                var tb = from p in db.Table<Sys.Entity.tbSysMessage>()
                         where p.tbSysUser.Id == Code.Common.UserId && p.IsPermit == true
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.MessageTitle.Contains(vm.SearchText));
                }

                vm.PrivateMessageMyList = (from p in tb
                                           orderby p.InputDate descending
                                           select new Dto.SysMessage.PrivateMessageList
                                           {
                                               Id = p.Id,
                                               InputDate = p.InputDate,
                                               IsEmail = p.IsEmail,
                                               IsSms = p.IsSms,
                                               MessageContent = p.MessageContent,
                                               MessageTitle = p.MessageTitle,
                                               SysUserName = p.tbSysUser.UserName,
                                               Url = p.Url
                                           }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrivateMessageList(Models.SysMessage.PrivateMessageList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("PrivateMessageList",
                new
                {
                    searchText = vm.SearchText,
                    pageSize = vm.Page.PageSize,
                    pageCount = vm.Page.PageCount,
                    pageIndex = vm.Page.PageIndex
                }));
        }

        public ActionResult PrivateMyMessageList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysMessage.PrivateMyMessageList();

                var tb = from p in db.Table<Sys.Entity.tbSysMessageUser>()
                         join m in db.Table<Sys.Entity.tbSysMessage>() on p.tbSysMessage.Id equals m.Id
                         where p.tbSysUser.Id == Code.Common.UserId && m.IsPermit == true
                         select new Dto.SysMessage.PrivateMyMessageList
                         {
                             Id = p.Id,
                             InputDate = m.InputDate,
                             IsEmail = m.IsEmail,
                             IsSms = m.IsSms,
                             MessageContent = m.MessageContent,
                             MessageTitle = m.MessageTitle,
                             SysUserName = m.tbSysUser.UserName,
                             MessageId = p.tbSysMessage.Id,
                             Url = m.Url,
                             IsRead = p.IsRead,
                             ReadDate = p.ReadDate
                         };

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.MessageTitle.Contains(vm.SearchText));
                }

                vm.PrivateMessageMyList = (from p in tb
                                           orderby p.InputDate descending
                                           select p).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrivateMyMessageList(Models.SysMessage.PrivateMyMessageList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("PrivateMyMessageList",
                new
                {
                    searchText = vm.SearchText,
                    pageSize = vm.Page.PageSize,
                    pageCount = vm.Page.PageCount,
                    pageIndex = vm.Page.PageIndex
                }));
        }

        public ActionResult PrivateMessageEdit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysMessage.PrivateMessageEdit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Sys.Entity.tbSysMessage>()
                              where p.Id == id && p.IsPermit == true
                              select new Dto.SysMessage.PrivateMessageEdit
                              {
                                  Id = p.Id,
                                  InputDate = p.InputDate,
                                  IsEmail = p.IsEmail,
                                  IsSms = p.IsSms,
                                  MessageContent = p.MessageContent,
                                  MessageTitle = p.MessageTitle,
                                  SysUserId = p.tbSysUser.Id,
                                  ReceiverUserIds = "",
                                  Url = p.Url
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.PrivateMessageMyEdit = tb;
                        vm.SysUserList = (from p in db.Table<Sys.Entity.tbSysMessageUser>()
                                          where p.tbSysMessage.Id == id
                                          select new Dto.SysUser.List
                                          {
                                              Id = p.tbSysUser.Id,
                                              UserName = p.tbSysUser.UserName
                                          }).ToList();
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult PrivateMessageEdit(Models.SysMessage.PrivateMessageEdit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.PrivateMessageMyEdit.Id == 0)
                    {
                        if (string.IsNullOrEmpty(vm.PrivateMessageMyEdit.ReceiverUserIds) == false)
                        {
                            var userIds = vm.PrivateMessageMyEdit.ReceiverUserIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            var tb = new Sys.Entity.tbSysMessage();
                            tb.MessageTitle = vm.PrivateMessageMyEdit.MessageTitle;
                            tb.IsSms = false;
                            tb.IsEmail = false;
                            tb.IsPermit = true;//私信
                            tb.Url = "";
                            tb.MessageContent = vm.PrivateMessageMyEdit.MessageContent;
                            tb.tbProgram = db.Set<Admin.Entity.tbProgram>().Find(Code.Common.ProgramId);
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            tb.InputDate = DateTime.Now;
                            db.Set<Sys.Entity.tbSysMessage>().Add(tb);
                            foreach (var id in userIds)
                            {
                                var userid = id.ConvertToInt();
                                var tbUser = new Sys.Entity.tbSysMessageUser();
                                tbUser.IsRead = false;
                                tbUser.tbSysMessage = tb;
                                tbUser.ReadDate = DateTime.Now;
                                tbUser.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(userid);
                                tbUser.UpdateTime = DateTime.Now;
                                db.Set<Sys.Entity.tbSysMessageUser>().Add(tbUser);
                            }
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加私信");
                            }
                        }
                        else
                        {
                            error.AddError("请选择接收私信的用户");
                            return Code.MvcHelper.Post(error);
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Sys.Entity.tbSysMessage>()
                                  where p.Id == vm.PrivateMessageMyEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.MessageTitle = vm.PrivateMessageMyEdit.MessageTitle;
                            tb.IsSms = false;
                            tb.IsEmail = false;
                            tb.Url = "";
                            tb.IsPermit = true;
                            tb.MessageContent = vm.PrivateMessageMyEdit.MessageContent;
                            tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                            tb.tbProgram = db.Set<Admin.Entity.tbProgram>().Find(Code.Common.ProgramId);
                            tb.InputDate = DateTime.Now;
                            //删除之前
                            var tbUser = (from p in db.Table<Sys.Entity.tbSysMessageUser>()
                                          where p.tbSysMessage.Id == vm.PrivateMessageMyEdit.Id
                                          select p).ToList();
                            foreach (var a in tbUser)
                            {
                                a.IsDeleted = true;
                                a.UpdateTime = DateTime.Now;
                            }
                            //重新增加
                            var userIds = vm.PrivateMessageMyEdit.ReceiverUserIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var id in userIds)
                            {
                                var userid = id.ConvertToInt();
                                var tbUserNew = new Sys.Entity.tbSysMessageUser();
                                tbUserNew.IsRead = false;
                                tbUserNew.tbSysMessage = tb;
                                tbUserNew.ReadDate = DateTime.Now;
                                tbUserNew.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(userid);
                                tbUserNew.UpdateTime = DateTime.Now;
                                db.Set<Sys.Entity.tbSysMessageUser>().Add(tbUserNew);
                            }
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改私信");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }

                return Code.MvcHelper.Post(null, Url.Action("PrivateMessageList"), "提交成功!");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrivateMessageDelete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysMessage>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                var messageUserList = (from p in db.Table<Sys.Entity.tbSysMessageUser>()
                                        .Include(d => d.tbSysMessage)
                                       where ids.Contains(p.tbSysMessage.Id)
                                       select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                    foreach (var user in messageUserList.Where(d => d.tbSysMessage.Id == a.Id))
                    {
                        user.IsDeleted = true;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除私信消息");
                }

                return Code.MvcHelper.Post();
            }
        }
        public ActionResult SelectUser()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysMessage.SelectUser();
                if (Request["UserType"] != null)
                {
                    Code.EnumHelper.SysUserType userType;
                    Enum.TryParse(Request["UserType"], out userType);
                    vm.UserType = userType;
                }

                var tb = from p in db.Table<Sys.Entity.tbSysUser>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.UserCode.Contains(vm.SearchText) || d.UserName.Contains(vm.SearchText) || d.Mobile.Contains(vm.SearchText));
                }

                if (vm.UserType != null)
                {
                    tb = tb.Where(d => d.UserType == vm.UserType);
                }

                vm.SelectUserList = (from p in tb
                                     orderby p.UserCode, p.UserName
                                     select new Dto.SysMessage.SelectUser
                                     {
                                         Id = p.Id,
                                         UserCode = p.UserCode,
                                         UserName = p.UserName,
                                         SexName = p.tbSex.SexName,
                                         Mobile = p.Mobile,
                                         UserType = p.UserType
                                     }).ToPageList(vm.Page);
                return View(vm);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectUser(Models.SysMessage.SelectUser vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SelectUser", new { searchText = vm.SearchText, userType = vm.UserType, pageSize = vm.Page.PageSize, pageCount = vm.Page.PageCount, pageIndex = vm.Page.PageIndex }));
        }

        [NonAction]
        public static List<Dto.SysMessage.List> GetLatestMessageList(int records)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysMessage>()
                          where p.IsPermit == false
                          orderby p.InputDate descending
                          select new Dto.SysMessage.List
                          {
                              Id = p.Id,
                              InputDate = p.InputDate,
                              IsEmail = p.IsEmail,
                              IsSms = p.IsSms,
                              MessageContent = p.MessageContent,
                              MessageTitle = p.MessageTitle,
                              SysUserName = p.tbSysUser.UserName,
                              Url = p.Url
                          }).Take(records).ToList();
                return tb;
            }
        }
        [NonAction]
        public static List<Dto.SysMessage.PrivateMyMessageList> GetPrivateMyMessageList(int userId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysMessage.PrivateMyMessageList();

                var tb = from p in db.Table<Sys.Entity.tbSysMessage>()
                         join m in db.Table<Sys.Entity.tbSysMessageUser>() on p.Id equals m.tbSysMessage.Id
                         where m.tbSysUser.Id == userId && m.IsRead == false && p.IsPermit == true
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.MessageTitle.Contains(vm.SearchText));
                }

                var PrivateMessageMyList = (from p in tb
                                            orderby p.InputDate descending
                                            select new Dto.SysMessage.PrivateMyMessageList
                                            {
                                                Id = p.Id,
                                                InputDate = p.InputDate,
                                                IsEmail = p.IsEmail,
                                                IsSms = p.IsSms,
                                                MessageContent = p.MessageContent,
                                                MessageTitle = p.MessageTitle,
                                                SysUserName = p.tbSysUser.UserName,
                                                Url = p.Url
                                            }).ToList();
                return PrivateMessageMyList;
            }
        }

        public ActionResult ImportUser()
        {
            var vm = new Models.SysMessageUser.ImportUser();
            vm.ImportList = new List<Dto.SysMessageUser.ImportUser>();
            return View(vm);
        }

        public ActionResult ImportUserTemplate()
        {
            var file = Server.MapPath("~/Areas/Sys/Views/SysMessage/MessageUserTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        public ActionResult ImportJsonUser()
        {
            var vm = new Models.SysMessageUser.ImportUser();
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                if (ModelState.IsValid)
                {

                    var file = Request.Files[0];
                    var fileSave = System.IO.Path.GetTempFileName();
                    file.SaveAs(fileSave);
                    using (var db = new XkSystem.Models.DbContext())
                    {
                        if (Code.Common.GetFileType(file.FileName) != Code.FileType.Excel)
                        {
                            error.AddError("上传的文件不是正确的EXCLE文件!");
                        }
                        var dt = Code.NpoiHelper.ExcelToDataTable(fileSave, System.IO.Path.GetExtension(file.FileName), string.Empty);

                        if (dt == null)
                        {
                            error.AddError("无法读取上传的文件，请检查文件格式是否正确!");
                        }
                        var tbOrgList = new List<string>() { "用户账号", "用户姓名" };
                        var Text = string.Empty;
                        foreach (var a in tbOrgList)
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
                        var tbSysUserList = db.Table<Sys.Entity.tbSysUser>().ToList();//全部用户
                        vm.ImportList = new List<Dto.SysMessageUser.ImportUser>();
                        var index = 0;
                        foreach (System.Data.DataRow dr in dt.Rows)
                        {
                            index++;
                            var SysUser = new Dto.SysMessageUser.ImportUser()
                            {
                                UserCode = Convert.ToString(dr["用户账号"]),
                                UserName = Convert.ToString(dr["用户姓名"])
                            };
                            var user = tbSysUserList.Where(d => d.UserCode == SysUser.UserCode);

                            if (user != null && user.Count() > 0)
                            {
                                SysUser.Id = user.FirstOrDefault().Id;
                                SysUser.UserName = user.FirstOrDefault().UserName;
                            }
                            else
                            {
                                var errorMsg = string.Format("第{0}行:用户{1}[{2}]不存在系统中;", index.ToString(), SysUser.UserCode, SysUser.UserName);
                                error.AddError(errorMsg);
                                continue;
                            }
                            if (vm.ImportList.Where(d => d.Id == SysUser.Id && d.UserCode == SysUser.UserCode && d.UserName == SysUser.UserName).Count() == 0)
                            {
                                vm.ImportList.Add(SysUser);
                            }
                        }
                    }
                }
            }
            var dynamic = new { Status = decimal.Zero, Message = vm.ImportList };
            if (error != null && error.Count > decimal.Zero)
            {
                return Json(new { Status = decimal.One, Message = error }, JsonRequestBehavior.AllowGet);
            }
            return Json(dynamic, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImportMessage()
        {
            var vm = new Models.SysMessageUser.ImportMessage();
            vm.ImportMsgList = new List<Dto.SysMessageUser.ImportMessage>();
            return View(vm);
        }

        public ActionResult ImportMessageTemplate()
        {
            var file = Server.MapPath("~/Areas/Sys/Views/SysMessage/ImportMessageTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportMessage(Models.SysMessageUser.ImportMessage vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                if (ModelState.IsValid)
                {
                    var file = Request.Files[nameof(vm.UploadFile)];
                    var fileSave = System.IO.Path.GetTempFileName();
                    file.SaveAs(fileSave);
                    using (var db = new XkSystem.Models.DbContext())
                    {
                        if (Code.Common.GetFileType(file.FileName) != Code.FileType.Excel)
                        {
                            error.AddError("上传的文件不是正确的EXCLE文件!");
                        }
                        var dt = Code.NpoiHelper.ExcelToDataTable(fileSave, System.IO.Path.GetExtension(file.FileName), string.Empty);

                        if (dt == null)
                        {
                            error.AddError("无法读取上传的文件，请检查文件格式是否正确!");
                        }
                        var tbOrgList = new List<string>() { "用户账号", "用户姓名", "私信标题", "私信内容" };
                        var Text = string.Empty;
                        foreach (var a in tbOrgList)
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
                        var tbSysUserList = db.Table<Sys.Entity.tbSysUser>().ToList();//全部用户
                        vm.ImportMsgList = new List<Dto.SysMessageUser.ImportMessage>();
                        var index = 0;
                        foreach (System.Data.DataRow dr in dt.Rows)
                        {
                            index++;
                            var SysUser = new Dto.SysMessageUser.ImportMessage()
                            {
                                UserCode = Convert.ToString(dr["用户账号"]),
                                UserName = Convert.ToString(dr["用户姓名"]),
                                MessageTitle = Convert.ToString(dr["私信标题"]),
                                MessageContent = Convert.ToString(dr["私信内容"])
                            };
                            var user = tbSysUserList.Where(d => d.UserCode == SysUser.UserCode);
                            if (user != null && user.Count() > 0)
                            {
                                SysUser.Id = user.FirstOrDefault().Id;
                                SysUser.UserName = user.FirstOrDefault().UserName;
                            }
                            else
                            {
                                var errorMsg = string.Format("第{0}行:用户{1}[{2}]不存在系统中;", index.ToString(), SysUser.UserCode, SysUser.UserName);
                                error.AddError(errorMsg);
                                continue;
                            }
                            if (string.IsNullOrEmpty(SysUser.MessageTitle))
                            {
                                var errorMsg = string.Format("第{0}行:私信标题为空，请输入私信标题;", index.ToString());
                                error.AddError(errorMsg);
                                continue;
                            }
                            if (string.IsNullOrEmpty(SysUser.MessageContent))
                            {
                                var errorMsg = string.Format("第{0}行:私信内容为空，请输入私信内容;", index.ToString());
                                error.AddError(errorMsg);
                                continue;
                            }
                            if (vm.ImportMsgList.Where(d => d.Id == SysUser.Id && d.UserCode == SysUser.UserCode && d.UserName == SysUser.UserName && d.MessageTitle == SysUser.MessageTitle && d.MessageContent == SysUser.MessageContent).Count() == 0)
                            {
                                vm.ImportMsgList.Add(SysUser);
                            }
                        }
                        if (error != null && error.Count > decimal.Zero)
                        {

                        }
                        else
                        {
                            foreach (var message in vm.ImportMsgList)
                            {
                                var tb = new Sys.Entity.tbSysMessage();
                                tb.MessageTitle = message.MessageTitle;
                                tb.IsSms = false;
                                tb.IsEmail = false;
                                tb.IsPermit = true;//私信
                                tb.Url = "";
                                tb.MessageContent = message.MessageContent;
                                tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                                tb.tbProgram = db.Set<Admin.Entity.tbProgram>().Find(Code.Common.ProgramId);
                                tb.InputDate = DateTime.Now;
                                db.Set<Sys.Entity.tbSysMessage>().Add(tb);

                                var tbUser = new Sys.Entity.tbSysMessageUser();
                                tbUser.IsRead = false;
                                tbUser.tbSysMessage = tb;
                                tbUser.ReadDate = DateTime.Now;
                                tbUser.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(message.Id);
                                tbUser.UpdateTime = DateTime.Now;
                                db.Set<Sys.Entity.tbSysMessageUser>().Add(tbUser);
                            }
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入私信");
                            }
                            vm.Status = true;
                        }
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
    }
}