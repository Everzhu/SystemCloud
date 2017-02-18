using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XkSystem.Areas.Wechat.Entity;
using XkSystem.Areas.Wechat.Models.ApplyLeave;

namespace XkSystem.Areas.Wechat.Controllers
{
    public class ApplyLeaveController : Controller
    {
        static string flowTypeCode = "LEAVE";

        #region 查看流程
        public ActionResult ApplyLeaveIndex(Models.ApplyLeave.ApplyLeaveListModel vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var workflowApproveList = OAWeWorkFlow.GetWorkFlowApproveList(flowTypeCode);
                var allApproveList = OAWeWorkFlow.GetAllApprovedList(flowTypeCode);

                WaitApproveList(vm, workflowApproveList);
                ApprovedList(vm, workflowApproveList, allApproveList);
                MyApplyList(vm, workflowApproveList, allApproveList);

                if (Request.IsAjaxRequest())
                {
                    return PartialView("ApplyLeaveApproveBody", vm);
                }

                //发起流程权限
                var approver = db.Table<Wechat.Entity.tbWeOAFlowApprover>().Where(m => m.tbWeOAFlowNode.tbSysOAFlowType.Code == flowTypeCode && m.tbSysUser.Id == XkSystem.Code.Common.UserId && m.tbWeOAFlowNode.FlowStep == 1).FirstOrDefault();
                vm.StartFlowPermission = approver != null;

                return View(vm);
            }
        }

        //我的申请
        private static void MyApplyList(ApplyLeaveListModel vm, IEnumerable<tbWeOAFlowDetail> workflowApproveList, IEnumerable<tbWeOAFlowDetail> allApproveList)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var myApplyList = OAWeWorkFlow.GetMyApplyList(db, flowTypeCode).ToList();
                var my = (from a in myApplyList
                          join b in db.Table<Wechat.Entity.tbWeOAApplyLeave>().Include(m => m.tbTeacherDept).Include(m => m.tbWeOALeaveType) on a.ApproveBodyId equals b.Id
                          select new Dto.ApplyLeave.ApplyLeaveListDto
                          {
                              CaseFileName = b.CaseFileName,
                              LeaveDayCount = b.LeaveDayCount,
                              LeaveFromTime = b.LeaveFromTime,
                              LeaveToTime = b.LeaveToTime,
                              tbTeacherDeptName = b.tbTeacherDept.TeacherDeptName,
                              tbWeOALeaveTypeName = b.tbWeOALeaveType.LeaveTypeName,
                              Reason = b.Reason,

                              Id = b.Id,
                              ApplyUser = workflowApproveList.Where(m => m.ApproveBodyId == b.Id).FirstOrDefault().tbSysUser.UserName,
                              ApproveDate = a.ApproveDate,
                              ApproveOpinion = a.ApproveOpinion,
                              ApproveUserName = a.tbSysUser.UserName,
                              NodeApproveStatus = allApproveList.Where(m => m.ApproveBodyId == a.ApproveBodyId && m.NodeApproveStatus == Code.EnumHelper.OAFlowNodeStatus.WithoutApproval).FirstOrDefault() == null ? Code.EnumHelper.OAFlowNodeStatus.Approved : Code.EnumHelper.OAFlowNodeStatus.WithoutApproval,
                              IsComplete = allApproveList.Where(m => m.ApproveBodyId == a.ApproveBodyId && m.tbWeOAFlowNextNode == null).FirstOrDefault() != null
                          }).Distinct(new Code.EqualComparer<Dto.ApplyLeave.ApplyLeaveListDto>((x, y) => x.Id == y.Id)).OrderByDescending(m => m.ApproveDate);
                MyListPage(vm, my);
            }
        }

        //已审批
        private static void ApprovedList(ApplyLeaveListModel vm, IEnumerable<tbWeOAFlowDetail> workflowApproveList, IEnumerable<tbWeOAFlowDetail> allApproveList)
        {
            using (var db = new XkSystem.Models.DbContext())
            {

                var approvedList = OAWeWorkFlow.GetApprovedList(db, flowTypeCode);
                var approved = (from a in approvedList
                                join b in db.Table<Wechat.Entity.tbWeOAApplyLeave>().Include(m => m.tbTeacherDept).Include(m => m.tbWeOALeaveType) on a.ApproveBodyId equals b.Id
                                select new Dto.ApplyLeave.ApplyLeaveListDto
                                {
                                    CaseFileName = b.CaseFileName,
                                    LeaveDayCount = b.LeaveDayCount,
                                    LeaveFromTime = b.LeaveFromTime,
                                    LeaveToTime = b.LeaveToTime,
                                    tbTeacherDeptName = b.tbTeacherDept.TeacherDeptName,
                                    tbWeOALeaveTypeName = b.tbWeOALeaveType.LeaveTypeName,
                                    Reason = b.Reason,

                                    Id = b.Id,
                                    ApplyUser = workflowApproveList.Where(m => m.ApproveBodyId == b.Id).FirstOrDefault().tbSysUser.UserName,
                                    ApproveDate = a.ApproveDate,
                                    ApproveOpinion = a.ApproveOpinion,
                                    ApproveUserName = a.tbSysUser.UserName,
                                    NodeApproveStatus = allApproveList.Where(m => m.ApproveBodyId == a.ApproveBodyId && m.NodeApproveStatus == Code.EnumHelper.OAFlowNodeStatus.WithoutApproval).FirstOrDefault() == null ? Code.EnumHelper.OAFlowNodeStatus.Approved : Code.EnumHelper.OAFlowNodeStatus.WithoutApproval,
                                    IsComplete = allApproveList.Where(m => m.ApproveBodyId == a.ApproveBodyId && m.tbWeOAFlowNextNode == null).FirstOrDefault() != null
                                }).Distinct(new Code.EqualComparer<Dto.ApplyLeave.ApplyLeaveListDto>((x, y) => x.Id == y.Id)).OrderByDescending(m => m.ApproveDate);
                ApprovedListPage(vm, approved);
            }
        }

        //待审批
        private void WaitApproveList(ApplyLeaveListModel vm, IEnumerable<tbWeOAFlowDetail> workflowApproveList)
        {
            using (var db = new XkSystem.Models.DbContext())
            {

                var waitApproveList = OAWeWorkFlow.GetWaitApproveList(db, flowTypeCode);
                var wait = (from a in waitApproveList
                            join b in db.Table<Wechat.Entity.tbWeOAApplyLeave>().Include(m => m.tbTeacherDept).Include(m => m.tbWeOALeaveType) on a.ApproveBodyId equals b.Id
                            select new Dto.ApplyLeave.ApplyLeaveListDto
                            {
                                CaseFileName = b.CaseFileName,
                                LeaveDayCount = b.LeaveDayCount,
                                LeaveFromTime = b.LeaveFromTime,
                                LeaveToTime = b.LeaveToTime,
                                tbTeacherDeptName = b.tbTeacherDept.TeacherDeptName,
                                tbWeOALeaveTypeName = b.tbWeOALeaveType.LeaveTypeName,
                                Reason = b.Reason,

                                Id = b.Id,
                                ApplyUser = workflowApproveList.Where(m => m.ApproveBodyId == b.Id).FirstOrDefault().tbSysUser.UserName,
                                ApproveDate = a.ApproveDate,
                                ApproveOpinion = a.ApproveOpinion,
                                ApproveUserName = a.tbSysUser.UserName,
                                NodeApproveStatus = a.NodeApproveStatus
                            }).Distinct(new Code.EqualComparer<Dto.ApplyLeave.ApplyLeaveListDto>((x, y) => x.Id == y.Id)).OrderByDescending(m => m.ApproveDate);
                WaitListPage(vm, wait);
            }
        }

        #region 分页
        private static void ApprovedListPage(Models.ApplyLeave.ApplyLeaveListModel vm, IEnumerable<Dto.ApplyLeave.ApplyLeaveListDto> approved)
        {
            vm.ApprovedDto = approved.ToPageList(vm.approvedPage);
            vm.approvedPage.PageCount = (int)Math.Ceiling(vm.approvedPage.TotalCount * 1.0 / vm.approvedPage.PageSize);
            vm.approvedPage.PageCount = vm.approvedPage.PageCount == 0 ? 1 : vm.approvedPage.PageCount;
        }

        private static void WaitListPage(Models.ApplyLeave.ApplyLeaveListModel vm, IEnumerable<Dto.ApplyLeave.ApplyLeaveListDto> wait)
        {
            vm.WaitApproveDto = wait.ToPageList(vm.waitPage);
            vm.waitPage.PageCount = (int)Math.Ceiling(vm.waitPage.TotalCount * 1.0 / vm.waitPage.PageSize);
            vm.waitPage.PageCount = vm.waitPage.PageCount == 0 ? 1 : vm.waitPage.PageCount;
        }

        private static void MyListPage(Models.ApplyLeave.ApplyLeaveListModel vm, IEnumerable<Dto.ApplyLeave.ApplyLeaveListDto> my)
        {
            //1:审批中，2:已完成,3:已驳回，0：全部
            if (vm.ApproveStatus == 1)
            {
                vm.MyApplyDto = my.Where(m => m.IsComplete == false && m.NodeApproveStatus != Code.EnumHelper.OAFlowNodeStatus.WithoutApproval).ToPageList(vm.myPage);
            }
            else if (vm.ApproveStatus == 2)
            {
                vm.MyApplyDto = my.Where(m => m.IsComplete == true && m.NodeApproveStatus != Code.EnumHelper.OAFlowNodeStatus.WithoutApproval).ToPageList(vm.myPage);
            }
            else if (vm.ApproveStatus == 3)
            {
                vm.MyApplyDto = my.Where(m => m.NodeApproveStatus == Code.EnumHelper.OAFlowNodeStatus.WithoutApproval).ToPageList(vm.myPage);
            }
            else
            {
                vm.MyApplyDto = my.ToPageList(vm.myPage);
            }
            vm.myPage.PageCount = (int)Math.Ceiling(vm.myPage.TotalCount * 1.0 / vm.myPage.PageSize);
            vm.myPage.PageCount = vm.myPage.PageCount == 0 ? 1 : vm.myPage.PageCount;
        }
        #endregion

        public ActionResult WorkFlowList(int approveBodyId, string segmentedTab)
        {
            Models.ApplyLeave.ApplyLeaveListModel vm = new Models.ApplyLeave.ApplyLeaveListModel();
            vm.SegmentedTab = segmentedTab;
            using (var db = new XkSystem.Models.DbContext())
            {
                //查询当前审批角色对应的是哪个节点
                var flowApprovalNodes = db.Table<Wechat.Entity.tbWeOAFlowApprover>().Where(m => m.tbWeOAFlowNode.tbSysOAFlowType.Code == flowTypeCode && m.tbWeOAFlowNode.FlowStep == 2).Include(m => m.tbSysUser).Include(m => m.tbWeOAFlowNode).ToList();

                var firstNode = db.Table<Wechat.Entity.tbWeOAFlowNode>().Where(m => m.tbSysOAFlowType.Code == flowTypeCode && m.FlowStep == 1).FirstOrDefault();
                var workFlowList = OAWeWorkFlow.GetWorkFlowList(db, flowTypeCode, approveBodyId);
                var workflowApproveList = OAWeWorkFlow.GetWorkFlowApproveList(flowTypeCode);

                vm.WorkFlowListDto = (from a in workFlowList
                                      join b in db.Table<Wechat.Entity.tbWeOAApplyLeave>().Include(m => m.tbTeacherDept).Include(m => m.tbWeOALeaveType) on a.ApproveBodyId equals b.Id
                                      select new Dto.ApplyLeave.ApplyLeaveListDto
                                      {
                                          CaseFileName = b.CaseFileName,
                                          LeaveDayCount = b.LeaveDayCount,
                                          LeaveFromTime = b.LeaveFromTime,
                                          LeaveToTime = b.LeaveToTime,
                                          tbTeacherDeptName = b.tbTeacherDept.TeacherDeptName,
                                          tbWeOALeaveTypeName = b.tbWeOALeaveType.LeaveTypeName,
                                          Reason = b.Reason,

                                          Id = b.Id,
                                          ApplyUser = workflowApproveList.Where(m => m.ApproveBodyId == b.Id).FirstOrDefault().tbSysUser.UserName,
                                          ApproveDate = a.ApproveDate,
                                          ApproveOpinion = a.ApproveOpinion,
                                          ApproveUserId = a.tbSysUser == null ? 0 : a.tbSysUser.Id,
                                          ApproveUserName = a.tbSysUser == null ? "" : a.tbSysUser.UserName,
                                          NodeApproveStatus = a.NodeApproveStatus,
                                          FlowComplete = a.tbWeOAFlowPreviousNode != null ? a.tbWeOAFlowPreviousNode.FlowComplete : firstNode.FlowComplete,
                                          FlowApprovalNode = a.tbWeOAFlowPreviousNode != null ? a.tbWeOAFlowPreviousNode.FlowApprovalNode : firstNode.FlowApprovalNode,
                                          FlowStep = a.tbWeOAFlowPreviousNode != null ? a.tbWeOAFlowPreviousNode.FlowStep : firstNode.FlowStep,
                                          ConditionalFormula = a.tbWeOAFlowPreviousNode != null ? a.tbWeOAFlowPreviousNode.ConditionalFormula : firstNode.ConditionalFormula,
                                      }).ToList();

                //处理流程数据
                if (segmentedTab == "wait")
                {
                    vm.WorkFlowListDto = vm.WorkFlowListDto.FindAll(delegate (Dto.ApplyLeave.ApplyLeaveListDto item)
                    {
                        if (item.ConditionalFormula == "day=0.5" && item.LeaveDayCount <= 0.5)
                        {
                            return true;
                        }
                        else if (item.ConditionalFormula == "day<=2&day>0.5" && item.LeaveDayCount <= 2 && item.LeaveDayCount > 0.5)
                        {
                            return true;
                        }
                        else if (item.ConditionalFormula == "day>=3" && item.LeaveDayCount >= 3)
                        {
                            return true;
                        }
                        else
                        {
                            if (item.ConditionalFormula == "")
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    });

                    //过滤分支节点
                    var currApproveUser = db.Table<Entity.tbWeOAFlowApprover>().Where(m=>m.tbWeOAFlowNode.tbSysOAFlowType.Code == flowTypeCode && m.tbWeOAFlowNode.FlowStep == 2 && m.tbSysUser.Id == XkSystem.Code.Common.UserId).Include(m=>m.tbSysUser).FirstOrDefault();
                    if (currApproveUser != null)
                    {
                        vm.WorkFlowListDto = vm.WorkFlowListDto.Where(m => (m.FlowApprovalNode == currApproveUser.tbWeOAFlowNode.FlowApprovalNode && m.FlowStep == 2) || m.FlowStep != 2).ToList();
                    }

                    //下个节点审批人列表
                    var currFlow = vm.WorkFlowListDto.Where(m => string.IsNullOrEmpty(m.ApproveUserName)).FirstOrDefault();
                    if (currFlow != null)
                    {
                        var nextFlow = vm.WorkFlowListDto.Where(m => m.FlowStep == currFlow.FlowStep + 1).FirstOrDefault();
                        if (nextFlow != null)
                        {
                            var nextApproveUsers = db.Table<Wechat.Entity.tbWeOAFlowApprover>().Where(m => m.tbWeOAFlowNode.FlowStep == nextFlow.FlowStep && m.tbWeOAFlowNode.FlowApprovalNode == nextFlow.FlowApprovalNode && m.tbWeOAFlowNode.tbSysOAFlowType.Code == flowTypeCode).Include(m => m.tbSysUser).ToList();
                            var users = (from p in nextApproveUsers
                                         select new Code.MuiJsonDataBind
                                         {
                                             text = p.tbSysUser.UserName,
                                             value = p.tbSysUser.Id.ToString(),
                                         }).ToList();
                            vm.UserListJson = JsonConvert.SerializeObject(users);
                        }
                    }
                    
                }
                else
                {
                    //修复分支情况部门负责人和年级负责人名称显示不正确的问题
                    vm.WorkFlowListDto.ForEach(m =>
                    {
                        if (m.FlowStep == 2)
                        {
                            m.FlowApprovalNode = flowApprovalNodes.Where(n => m.ApproveUserId != 0 && n.tbSysUser.Id == m.ApproveUserId).Select(k => k.tbWeOAFlowNode.FlowApprovalNode).FirstOrDefault();
                        }
                        
                    });
                }
                return View(vm);
            }
        }
        #endregion

        #region 发起流程
        public ActionResult AddApplyLeave()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ApplyLeave.ApplyLeaveEditModel();
                var depts = (from p in db.Table<Teacher.Entity.tbTeacherDept>()
                             select new Code.MuiJsonDataBind
                             {
                                 text = p.TeacherDeptName,
                                 value = p.Id.ToString(),
                             }).ToList();

                var leaves = (from p in db.Table<Wechat.Entity.tbWeOALeaveType>()
                              select new Code.MuiJsonDataBind
                              {
                                  text = p.LeaveTypeName,
                                  value = p.Id.ToString(),
                              }).ToList();
                vm.DepartListJson = JsonConvert.SerializeObject(depts);
                vm.LeaveTypeListJson = JsonConvert.SerializeObject(leaves);
                return View(vm);
            }
        }

        //获取审批人列表
        public JsonResult GetNextApproveUsers(string conditionalFormula)
        {
            var approveUsers = OAWeWorkFlow.GetNextApproveUsers(flowTypeCode, conditionalFormula);
            return Json(approveUsers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddApplyLeave(Models.ApplyLeave.ApplyLeaveEditModel vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    //审批内容表记录
                    var tb = new Wechat.Entity.tbWeOAApplyLeave();
                    tb.CaseFileName = vm.ApplyLeaveEditDto.CaseFileName;
                    tb.LeaveDayCount = vm.ApplyLeaveEditDto.LeaveDayCount;
                    tb.LeaveFromTime = vm.ApplyLeaveEditDto.LeaveFromTime;
                    tb.LeaveToTime = vm.ApplyLeaveEditDto.LeaveToTime;
                    tb.Reason = vm.ApplyLeaveEditDto.Reason;
                    tb.tbWeOALeaveType = db.Set<Wechat.Entity.tbWeOALeaveType>().Find(vm.ApplyLeaveEditDto.tbWeOALeaveTypeId);
                    tb.tbTeacherDept = db.Set<Teacher.Entity.tbTeacherDept>().Find(vm.ApplyLeaveEditDto.tbTeacherDeptId);
                    db.Set<Wechat.Entity.tbWeOAApplyLeave>().Add(tb);
                    db.SaveChanges();

                    if (vm.ApplyLeaveEditDto.LeaveDayCount <= 0.5)
                    {
                        vm.ApplyLeaveEditDto.ConditionalFormula = "day=0.5";
                    }
                    else if (vm.ApplyLeaveEditDto.LeaveDayCount <= 2 && vm.ApplyLeaveEditDto.LeaveDayCount > 0.5)
                    {
                        vm.ApplyLeaveEditDto.ConditionalFormula = "day<=2&day>0.5";
                    }
                    else if (vm.ApplyLeaveEditDto.LeaveDayCount >= 3)
                    {
                        vm.ApplyLeaveEditDto.ConditionalFormula = "day>=3";
                    }

                    //执行流程引擎
                    OAWeWorkFlow.ExecuteWorkFlowEngine(db, flowTypeCode, tb.Id, vm.ApplyLeaveEditDto.ConditionalFormula, "发起流程", false, vm.ApplyLeaveEditDto.NextApproveUserId);
                }
                else
                {
                    vm.ErrorMsg = string.Join("\r\n", error);
                    return View(vm);//包含文件表单的方式
                }
                return Content("<script type='text/javascript'> window.parent.location.href = '" + Url.Action("ApplyLeaveIndex", "ApplyLeave", new { area = "wechat" }) + "';</script>");
            }
        }
        #endregion

        #region 审批流程
        [HttpPost]
        public ActionResult ApproveWorkFlow(int approveBodyId = 0, string conditionalFormula = "", string approveOpinion = "", int nextApproveUserId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                OAWeWorkFlow.ExecuteWorkFlowEngine(db, flowTypeCode, approveBodyId, conditionalFormula, approveOpinion, false, nextApproveUserId);
            }
            return Content("<script type='text/javascript'> window.parent.location.href = '" + Url.Action("ApplyLeaveIndex", "ApplyLeave", new { area = "wechat" }) + "';</script>");
        }
        #endregion

        #region 驳回流程
        public ActionResult RejectWorkFlow(int approveBodyId = 0, string conditionalFormula = "", string approveOpinion = "")
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                OAWeWorkFlow.ExecuteWorkFlowEngine(db, flowTypeCode, approveBodyId, conditionalFormula, approveOpinion, true);
            }
            return Content("<script type='text/javascript'> window.parent.location.href = '" + Url.Action("ApplyLeaveIndex", "ApplyLeave", new { area = "wechat" }) + "';</script>");
        }
        #endregion
    }
}