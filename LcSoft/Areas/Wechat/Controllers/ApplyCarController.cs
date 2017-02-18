using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XkSystem.Areas.Wechat.Entity;
using XkSystem.Areas.Wechat.Models.ApplyCar;

namespace XkSystem.Areas.Wechat.Controllers
{
    public class ApplyCarController : Controller
    {
        string flowTypeCode = "CAR";

        #region 查看流程
        public ActionResult ApplyCarIndex(Models.ApplyCar.ApplyCarListModel vm)
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
                    return PartialView("ApplyCarApproveBody", vm);
                }

                //发起流程权限
                var approver = db.Table<Wechat.Entity.tbWeOAFlowApprover>().Where(m => m.tbWeOAFlowNode.tbSysOAFlowType.Code == flowTypeCode && m.tbSysUser.Id == XkSystem.Code.Common.UserId && m.tbWeOAFlowNode.FlowStep == 1).FirstOrDefault();
                vm.StartFlowPermission = approver != null;

                return View(vm);
            }
        }

        //我的申请
        private void MyApplyList(ApplyCarListModel vm, IEnumerable<tbWeOAFlowDetail> workflowApproveList, IEnumerable<tbWeOAFlowDetail> allApproveList)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var myApplyList = OAWeWorkFlow.GetMyApplyList(db, flowTypeCode).ToList();
                var my = (from a in myApplyList
                          join b in db.Table<Wechat.Entity.tbWeOAApplyCar>().Include(m => m.tbTeacherDept) on a.ApproveBodyId equals b.Id
                          select new Dto.ApplyCar.ApplyCarListDto
                          {
                              CarTime = b.CarTime,
                              Destination = b.Destination,
                              tbTeacherDeptName = b.tbTeacherDept.TeacherDeptName,
                              Reason = b.Reason,
                              OtherUsers = b.OtherUsers,
                              Remark = b.Remark,

                              Id = b.Id,
                              ApplyUser = workflowApproveList.Where(m => m.ApproveBodyId == b.Id).FirstOrDefault().tbSysUser.UserName,
                              ApproveDate = a.ApproveDate,
                              ApproveOpinion = a.ApproveOpinion,
                              ApproveUserName = a.tbSysUser.UserName,
                              NodeApproveStatus = allApproveList.Where(m => m.ApproveBodyId == a.ApproveBodyId && m.NodeApproveStatus == Code.EnumHelper.OAFlowNodeStatus.WithoutApproval).FirstOrDefault() == null ? Code.EnumHelper.OAFlowNodeStatus.Approved : Code.EnumHelper.OAFlowNodeStatus.WithoutApproval,
                              IsComplete = allApproveList.Where(m => m.ApproveBodyId == a.ApproveBodyId && m.tbWeOAFlowNextNode == null).FirstOrDefault() != null
                          }).Distinct(new Code.EqualComparer<Dto.ApplyCar.ApplyCarListDto>((x, y) => x.Id == y.Id)).OrderByDescending(m => m.ApproveDate);
                MyListPage(vm, my);
            }
        }

        //已审批
        private void ApprovedList(ApplyCarListModel vm, IEnumerable<tbWeOAFlowDetail> workflowApproveList, IEnumerable<tbWeOAFlowDetail> allApproveList)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var approvedList = OAWeWorkFlow.GetApprovedList(db, flowTypeCode);
                var approved = (from a in approvedList
                                join b in db.Table<Wechat.Entity.tbWeOAApplyCar>().Include(m => m.tbTeacherDept) on a.ApproveBodyId equals b.Id
                                select new Dto.ApplyCar.ApplyCarListDto
                                {
                                    CarTime = b.CarTime,
                                    Destination = b.Destination,
                                    tbTeacherDeptName = b.tbTeacherDept.TeacherDeptName,
                                    Reason = b.Reason,
                                    OtherUsers = b.OtherUsers,
                                    Remark = b.Remark,

                                    Id = b.Id,
                                    ApplyUser = workflowApproveList.Where(m => m.ApproveBodyId == b.Id).FirstOrDefault().tbSysUser.UserName,
                                    ApproveDate = a.ApproveDate,
                                    ApproveOpinion = a.ApproveOpinion,
                                    ApproveUserName = a.tbSysUser.UserName,
                                    NodeApproveStatus = allApproveList.Where(m => m.ApproveBodyId == a.ApproveBodyId && m.NodeApproveStatus == Code.EnumHelper.OAFlowNodeStatus.WithoutApproval).FirstOrDefault() == null ? Code.EnumHelper.OAFlowNodeStatus.Approved : Code.EnumHelper.OAFlowNodeStatus.WithoutApproval,
                                    IsComplete = allApproveList.Where(m => m.ApproveBodyId == a.ApproveBodyId && m.tbWeOAFlowNextNode == null).FirstOrDefault() != null
                                }).Distinct(new Code.EqualComparer<Dto.ApplyCar.ApplyCarListDto>((x, y) => x.Id == y.Id)).OrderByDescending(m => m.ApproveDate);
                ApprovedListPage(vm, approved);
            }
        }

        //待审批
        private void WaitApproveList(ApplyCarListModel vm, IEnumerable<tbWeOAFlowDetail> workflowApproveList)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var waitApproveList = OAWeWorkFlow.GetWaitApproveList(db, flowTypeCode);
                var wait = (from a in waitApproveList
                            join b in db.Table<Wechat.Entity.tbWeOAApplyCar>().Include(m => m.tbTeacherDept) on a.ApproveBodyId equals b.Id
                            select new Dto.ApplyCar.ApplyCarListDto
                            {
                                CarTime = b.CarTime,
                                Destination = b.Destination,
                                tbTeacherDeptName = b.tbTeacherDept.TeacherDeptName,
                                Reason = b.Reason,
                                OtherUsers = b.OtherUsers,
                                Remark = b.Remark,

                                Id = b.Id,
                                ApplyUser = workflowApproveList.Where(m => m.ApproveBodyId == b.Id).FirstOrDefault().tbSysUser.UserName,
                                ApproveDate = a.ApproveDate,
                                ApproveOpinion = a.ApproveOpinion,
                                ApproveUserName = a.tbSysUser.UserName,
                                NodeApproveStatus = a.NodeApproveStatus
                            }).Distinct(new Code.EqualComparer<Dto.ApplyCar.ApplyCarListDto>((x, y) => x.Id == y.Id)).OrderByDescending(m => m.ApproveDate);
                WaitListPage(vm, wait);
            }
        }


        #region 分页
        private static void ApprovedListPage(Models.ApplyCar.ApplyCarListModel vm, IEnumerable<Dto.ApplyCar.ApplyCarListDto> approved)
        {
            vm.ApprovedDto = approved.ToPageList(vm.approvedPage);
            vm.approvedPage.PageCount = (int)Math.Ceiling(vm.approvedPage.TotalCount * 1.0 / vm.approvedPage.PageSize);
            vm.approvedPage.PageCount = vm.approvedPage.PageCount == 0 ? 1 : vm.approvedPage.PageCount;
        }

        private static void WaitListPage(Models.ApplyCar.ApplyCarListModel vm, IEnumerable<Dto.ApplyCar.ApplyCarListDto> wait)
        {
            vm.WaitApproveDto = wait.ToPageList(vm.waitPage);
            vm.waitPage.PageCount = (int)Math.Ceiling(vm.waitPage.TotalCount * 1.0 / vm.waitPage.PageSize);
            vm.waitPage.PageCount = vm.waitPage.PageCount == 0 ? 1 : vm.waitPage.PageCount;
        }

        private static void MyListPage(Models.ApplyCar.ApplyCarListModel vm, IEnumerable<Dto.ApplyCar.ApplyCarListDto> my)
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
            Models.ApplyCar.ApplyCarListModel vm = new Models.ApplyCar.ApplyCarListModel();
            vm.SegmentedTab = segmentedTab;
            using (var db = new XkSystem.Models.DbContext())
            {
                var firstNode = db.Table<Wechat.Entity.tbWeOAFlowNode>().Where(m => m.tbSysOAFlowType.Code == flowTypeCode && m.FlowStep == 1).FirstOrDefault();
                var workFlowList = OAWeWorkFlow.GetWorkFlowList(db, flowTypeCode, approveBodyId);
                var workflowApproveList = OAWeWorkFlow.GetWorkFlowApproveList(flowTypeCode);
                vm.WorkFlowListDto = (from a in workFlowList
                                      join b in db.Table<Wechat.Entity.tbWeOAApplyCar>().Include(m => m.tbTeacherDept) on a.ApproveBodyId equals b.Id
                                      select new Dto.ApplyCar.ApplyCarListDto
                                      {
                                          CarTime = b.CarTime,
                                          Destination = b.Destination,
                                          tbTeacherDeptName = b.tbTeacherDept.TeacherDeptName,
                                          Reason = b.Reason,
                                          OtherUsers = b.OtherUsers,
                                          Remark = b.Remark,

                                          Id = b.Id,
                                          ApplyUser = workflowApproveList.Where(m => m.ApproveBodyId == b.Id).FirstOrDefault().tbSysUser.UserName,
                                          ApproveDate = a.ApproveDate,
                                          ApproveOpinion = a.ApproveOpinion,
                                          ApproveUserName = a.tbSysUser == null ? "" : a.tbSysUser.UserName,
                                          NodeApproveStatus = a.NodeApproveStatus,
                                          FlowApprovalNode = a.tbWeOAFlowPreviousNode != null ? a.tbWeOAFlowPreviousNode.FlowApprovalNode : firstNode.FlowApprovalNode,
                                          FlowStep = a.tbWeOAFlowPreviousNode != null ? a.tbWeOAFlowPreviousNode.FlowStep : firstNode.FlowStep,
                                          ConditionalFormula = a.tbWeOAFlowPreviousNode != null ? a.tbWeOAFlowPreviousNode.ConditionalFormula : firstNode.ConditionalFormula,
                                      }).ToList();

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

                return View(vm);
            }
        }
        #endregion

        #region 发起流程
        public ActionResult AddApplyCar()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ApplyCar.ApplyCarEditModel();
                var depts = (from p in db.Table<Teacher.Entity.tbTeacherDept>()
                             select new Code.MuiJsonDataBind
                             {
                                 text = p.TeacherDeptName,
                                 value = p.Id.ToString(),
                             }).ToList();
                vm.DepartListJson = JsonConvert.SerializeObject(depts);

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
        public ActionResult AddApplyCar(Models.ApplyCar.ApplyCarEditModel vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    //审批内容表记录
                    var tb = new Wechat.Entity.tbWeOAApplyCar();
                    tb.CarTime = vm.ApplyCarEditDto.CarTime;
                    tb.Destination = vm.ApplyCarEditDto.Destination;
                    tb.OtherUsers = vm.ApplyCarEditDto.OtherUsers;
                    tb.Remark = vm.ApplyCarEditDto.Remark;
                    tb.tbTeacherDept = db.Set<Teacher.Entity.tbTeacherDept>().Find(vm.ApplyCarEditDto.tbTeacherDeptId);
                    db.Set<Wechat.Entity.tbWeOAApplyCar>().Add(tb);
                    db.SaveChanges();

                    //执行流程引擎
                    OAWeWorkFlow.ExecuteWorkFlowEngine(db, flowTypeCode, tb.Id, "", "发起流程", false, vm.ApplyCarEditDto.NextApproveUserId);
                }
                else
                {
                    return Content(string.Join("\r\n", error));//不提交文件表单的方式
                }
                return Code.MvcHelper.Post(error, Url.Action("ApplyCarIndex"));
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
            return Content("<script type='text/javascript'> window.parent.location.href = '" + Url.Action("ApplyCarIndex", "ApplyCar", new { area = "wechat" }) + "';</script>");
        }
        #endregion

        #region 驳回流程
        public ActionResult RejectWorkFlow(int approveBodyId = 0, string conditionalFormula = "", string approveOpinion = "")
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                OAWeWorkFlow.ExecuteWorkFlowEngine(db, flowTypeCode, approveBodyId, conditionalFormula, approveOpinion, true);
            }
            return Content("<script type='text/javascript'> window.parent.location.href = '" + Url.Action("ApplyCarIndex", "ApplyCar", new { area = "wechat" }) + "';</script>");
        }
        #endregion
    }
}