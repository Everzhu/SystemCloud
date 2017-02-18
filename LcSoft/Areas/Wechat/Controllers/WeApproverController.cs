using XkSystem.Areas.Sys.Controllers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Wechat.Controllers
{
    public class WeApproverController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.WeApprover.List();
                var tb = db.Table<Wechat.Entity.tbWeOAFlowNode>().Include(m=>m.tbSysOAFlowType);

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.FlowApprovalNode.Contains(vm.SearchText));
                }

                vm.RoleList = (from p in tb
                               orderby p.tbSysOAFlowType.Id
                               select new Dto.WeApprover.List
                               {
                                   Id = p.Id,
                                   FlowTypeName = p.tbSysOAFlowType.FlowTypeName,
                                   ApproveNodeName = p.FlowApprovalNode
                               }).ToList();
                return View(vm);
            }
        }

        public ActionResult ApproverList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.WeApprover.ApproverList();

                vm.FlowApprovalNodeName = db.Set<Wechat.Entity.tbWeOAFlowNode>().Find(vm.FlowApprovalNodeId).FlowApprovalNode;

                var tb = from p in db.Table<Wechat.Entity.tbWeOAFlowApprover>()
                         where p.tbWeOAFlowNode.Id == vm.FlowApprovalNodeId
                            && p.tbSysUser.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbSysUser.UserCode.Contains(vm.SearchText) || d.tbSysUser.UserName.Contains(vm.SearchText));
                }

                vm.WeApproverList = (from p in tb
                                      orderby p.tbSysUser.UserCode
                                      select new Dto.WeApprover.ApproverList
                                      {
                                          Id = p.Id,
                                          SysUserCode = p.tbSysUser.UserCode,
                                          SysUserName = p.tbSysUser.UserName
                                      }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproverList(Models.WeApprover.ApproverList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ApproverList", new { searchText = vm.SearchText, FlowApprovalNodeId = vm.FlowApprovalNodeId, pageIndex = vm.Page.PageIndex, pageSize = vm.Page.PageSize }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(List<int> ids, int FlowApprovalNodeId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                var vm = new Sys.Models.SysUser.Edit();

                var userRoleList = (from p in db.Table<Wechat.Entity.tbWeOAFlowApprover>()
                                    where p.tbWeOAFlowNode.Id == FlowApprovalNodeId
                                    select p.tbSysUser.Id).ToList();

                var userList = (from p in db.Table<Sys.Entity.tbSysUser>()
                                where ids.Contains(p.Id)
                                    && userRoleList.Contains(p.Id) == false
                                select p).ToList();
                foreach (var user in userList)
                {
                    var tb = new Wechat.Entity.tbWeOAFlowApprover();
                    tb.tbWeOAFlowNode = db.Set<Wechat.Entity.tbWeOAFlowNode>().Find(FlowApprovalNodeId);
                    tb.tbSysUser = user;
                    db.Set<Wechat.Entity.tbWeOAFlowApprover>().Add(tb);
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了审批成员");
                    //System.Web.HttpContext.Current.Cache["Power"] = SysRolePowerController.GetPower();
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Wechat.Entity.tbWeOAFlowApprover>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除审批角色");
                    //System.Web.HttpContext.Current.Cache["Power"] = SysRolePowerController.GetPower();
                }

                return Code.MvcHelper.Post();
            }
        }
    }
}