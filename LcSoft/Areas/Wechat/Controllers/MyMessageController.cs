using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Wechat.Controllers
{
    public class MyMessageController : Controller
    {
        // GET: Wechat/MyMessage
        public ActionResult MyMessageIndex(Sys.Models.SysMessage.List vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                //var vm = new Sys.Models.SysMessage.List();
                var q = (from c in db.Table<Sys.Entity.tbSysMessage>().Where(d => d.IsPermit == false)
                          select new Sys.Dto.SysMessage.List { Id = c.Id, InputDate = c.InputDate, MessageTitle = c.MessageTitle, MessageContent = c.MessageContent }
                          ).Concat(from p in db.Table<Sys.Entity.tbSysMessageUser>()
                                   join m in db.Table<Sys.Entity.tbSysMessage>() on p.tbSysMessage.Id equals m.Id
                                   where p.tbSysUser.Id == Code.Common.UserId && m.IsPermit == true
                                   select new Sys.Dto.SysMessage.List { Id = m.Id, InputDate = m.InputDate, MessageTitle = m.MessageTitle, MessageContent = m.MessageContent }
                          );
                var MsgList = q.OrderByDescending(d => d.InputDate).ToPageList(vm.Page);
                vm.MessageList = MsgList;
                vm.Page.PageCount = (int)Math.Ceiling(vm.Page.TotalCount * 1.0 / vm.Page.PageSize);
                vm.Page.PageCount = vm.Page.PageCount == 0 ? 1 : vm.Page.PageCount;
                if (Request.IsAjaxRequest())
                {
                    return PartialView("MList", vm);
                }
                return View(vm);
            }
        }


        public ActionResult AddMessage()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MyMessage.MyMessageEditModel();
                List<SelectListItem> ll = new List<SelectListItem>();
                var tb = (from p in db.Table<Sys.Entity.tbSysRole>()
                          where p.RoleCode != Code.EnumHelper.SysRoleCode.Administrator
                          orderby p.RoleName
                          select new
                          {
                              Value = p.Id,
                              Text = p.RoleName
                          }).ToList();
                foreach (var item in tb)
                {
                    SelectListItem ii = new SelectListItem();
                    ii.Value = item.Value.ConvertToString();
                    ii.Text = item.Text;
                    ll.Add(ii);
                }
                vm.RoleList = ll;
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMessage(Models.MyMessage.MyMessageEditModel vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    string[] RoleIDs = null;
                    if (Request["RoleIDs"] != null)
                    {
                        RoleIDs = Request["RoleIDs"].Split(',');
                    }

                    List<int> GobalUserIds = new List<int>();
                    string[] UserIDs = null;
                    if (!string.IsNullOrEmpty(vm.MyMessageEditDto.UserIds))
                    {
                        UserIDs = vm.MyMessageEditDto.UserIds.Split(',');
                        foreach (var id in UserIDs)
                        {
                            int uid = int.Parse(id);
                            GobalUserIds.Add(uid);
                        }
                    }

                    var tb = new Sys.Entity.tbSysMessage();
                    tb.MessageTitle = vm.MyMessageEditDto.MessageTitle;
                    tb.IsSms = false;
                    tb.IsEmail = false;
                    tb.IsPermit = true;//私信
                    tb.Url = "";
                    tb.MessageContent = vm.MyMessageEditDto.MessageContent;
                    tb.tbProgram = db.Set<Admin.Entity.tbProgram>().Find(Code.Common.ProgramId);
                    tb.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                    tb.InputDate = DateTime.Now;
                    db.Set<Sys.Entity.tbSysMessage>().Add(tb);
                    if (RoleIDs != null)
                    {
                        foreach (var id in RoleIDs)
                        {
                            int rid = int.Parse(id);
                            var UserIds = (from p in db.Table<Sys.Entity.tbSysUserRole>()
                                           where p.tbSysRole.Id == rid && p.IsDeleted == false
                                           select p.tbSysUser.Id).ToList();
                            GobalUserIds.AddRange(UserIds);
                        }
                    }
                    GobalUserIds = GobalUserIds.Distinct().ToList();
                    foreach (var item in GobalUserIds)
                    {
                        var userid = item;
                        var tbUser = new Sys.Entity.tbSysMessageUser();
                        tbUser.IsRead = false;
                        tbUser.tbSysMessage = tb;
                        tbUser.ReadDate = DateTime.Now;
                        tbUser.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(userid);
                        tbUser.UpdateTime = DateTime.Now;
                        db.Set<Sys.Entity.tbSysMessageUser>().Add(tbUser);
                    }

                    db.SaveChanges();
                }
                //if (error.Count > 0)
                //{
                //    return Content("<script type='text/javascript'>$(function(){  mui.alert('" + string.Join("\r\n", error) + "');});</script>");
                //}
                //return Content("<script type='text/javascript'> window.parent.location.href = '" + Url.Action("MyMessageIndex", "MyMessage", new { area = "wechat" }) + "';</script>");
                return Code.MvcHelper.Post(error, Url.Action("MyMessageIndex"));
            }
        }
    }
}