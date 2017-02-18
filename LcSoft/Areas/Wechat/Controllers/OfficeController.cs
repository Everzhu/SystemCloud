using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XkSystem.Areas.Wechat.Entity;
using XkSystem.Areas.Wechat.Models.Office;
using XkSystem.Models;
using Newtonsoft.Json;

namespace XkSystem.Areas.Wechat.Controllers
{
    public class OfficeController : Controller
    {
        static string flowTypeCode = "OF";

        #region 查看流程
        public ActionResult OfficeIndex(Models.Office.OfficeListModel vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var workflowApproveList = OAWeWorkFlow.GetWorkFlowApproveList(flowTypeCode);
                var allApproveList = OAWeWorkFlow.GetAllApprovedList(flowTypeCode);

                WaitApproveList(vm, workflowApproveList);
                ApprovedList(vm, workflowApproveList, allApproveList);
                MyApplyList(vm, workflowApproveList);

                if (Request.IsAjaxRequest())
                {
                    return PartialView("OfficeApproveBody", vm);

                }

                //发起流程权限
                var approver = db.Table<Wechat.Entity.tbWeOAFlowApprover>().Where(m => m.tbWeOAFlowNode.tbSysOAFlowType.Code == flowTypeCode && m.tbSysUser.Id == XkSystem.Code.Common.UserId && m.tbWeOAFlowNode.FlowStep == 1).FirstOrDefault();
                vm.StartFlowPermission = approver != null;

                return View(vm);
            }
        }

        //我的申请
        private void MyApplyList(OfficeListModel vm, IEnumerable<tbWeOAFlowDetail> workflowApproveList)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var allApproveList = OAWeWorkFlow.GetAllApprovedList(flowTypeCode);
                var myApplyList = OAWeWorkFlow.GetMyApplyList(db, flowTypeCode).ToList();
                var my = (from a in myApplyList
                          join b in db.Table<Wechat.Entity.tbWeOAOffice>() on a.ApproveBodyId equals b.Id
                          select new Dto.Office.OfficeListDto
                          {
                              Title = b.Title,
                              LimitDateTo = b.LimitDateTo,
                              OfficeFileFrom = b.OfficeFileFrom,
                              OfficeFileName = b.OfficeFileName,
                              OfficeFileNameSeq = b.OfficeFileNameSeq,
                              ReceiveFileTime = b.ReceiveFileTime,

                              Id = b.Id,
                              ApplyUser = workflowApproveList.Where(m => m.ApproveBodyId == b.Id).FirstOrDefault().tbSysUser.UserName,
                              ApproveDate = a.ApproveDate,
                              ApproveOpinion = a.ApproveOpinion,
                              ApproveUserName = a.tbSysUser.UserName,
                              NodeApproveStatus = allApproveList.Where(m => m.ApproveBodyId == a.ApproveBodyId && m.NodeApproveStatus == Code.EnumHelper.OAFlowNodeStatus.WithoutApproval).FirstOrDefault() == null ? Code.EnumHelper.OAFlowNodeStatus.Approved : Code.EnumHelper.OAFlowNodeStatus.WithoutApproval,
                              IsComplete = allApproveList.Where(m => m.ApproveBodyId == a.ApproveBodyId && m.tbWeOAFlowNextNode == null).FirstOrDefault() != null
                          }).Distinct(new Code.EqualComparer<Dto.Office.OfficeListDto>((x, y) => x.Id == y.Id)).OrderByDescending(m => m.ApproveDate);
                //分页
                MyListPage(vm, my);
            }
        }

        //已审批
        private void ApprovedList(OfficeListModel vm, IEnumerable<tbWeOAFlowDetail> workflowApproveList, IEnumerable<tbWeOAFlowDetail> allApproveList)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var approvedList = OAWeWorkFlow.GetApprovedList(db, flowTypeCode);
                var my = (from a in approvedList
                          join b in db.Table<Wechat.Entity.tbWeOAOffice>() on a.ApproveBodyId equals b.Id
                          select new Dto.Office.OfficeListDto
                          {
                              Title = b.Title,
                              LimitDateTo = b.LimitDateTo,
                              OfficeFileFrom = b.OfficeFileFrom,
                              OfficeFileName = b.OfficeFileName,
                              OfficeFileNameSeq = b.OfficeFileNameSeq,
                              ReceiveFileTime = b.ReceiveFileTime,

                              Id = b.Id,
                              ApplyUser = workflowApproveList.Where(m => m.ApproveBodyId == b.Id).FirstOrDefault().tbSysUser.UserName,
                              ApproveDate = a.ApproveDate,
                              ApproveOpinion = a.ApproveOpinion,
                              ApproveUserName = a.tbSysUser.UserName,
                              NodeApproveStatus = allApproveList.Where(m => m.ApproveBodyId == a.ApproveBodyId && m.NodeApproveStatus == Code.EnumHelper.OAFlowNodeStatus.WithoutApproval).FirstOrDefault() == null ? Code.EnumHelper.OAFlowNodeStatus.Approved : Code.EnumHelper.OAFlowNodeStatus.WithoutApproval,
                              IsComplete = allApproveList.Where(m => m.ApproveBodyId == a.ApproveBodyId && m.tbWeOAFlowNextNode == null).FirstOrDefault() != null
                          }).Distinct(new Code.EqualComparer<Dto.Office.OfficeListDto>((x, y) => x.Id == y.Id)).OrderByDescending(m => m.ApproveDate);
                //分页
                ApprovedListPage(vm, my);
            }
        }

        //待审批
        private void WaitApproveList(OfficeListModel vm, IEnumerable<tbWeOAFlowDetail> workflowApproveList)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var waitApproveList = OAWeWorkFlow.GetWaitApproveList(db, flowTypeCode);
                var wait = (from a in waitApproveList
                            join b in db.Table<Wechat.Entity.tbWeOAOffice>() on a.ApproveBodyId equals b.Id
                            select new Dto.Office.OfficeListDto
                            {
                                Title = b.Title,
                                LimitDateTo = b.LimitDateTo,
                                OfficeFileFrom = b.OfficeFileFrom,
                                OfficeFileName = b.OfficeFileName,
                                OfficeFileNameSeq = b.OfficeFileNameSeq,
                                ReceiveFileTime = b.ReceiveFileTime,

                                Id = b.Id,
                                ApplyUser = workflowApproveList.Where(m => m.ApproveBodyId == b.Id).FirstOrDefault().tbSysUser.UserName,
                                ApproveDate = a.ApproveDate,
                                ApproveOpinion = a.ApproveOpinion,
                                ApproveUserName = a.tbSysUser.UserName,
                                NodeApproveStatus = a.NodeApproveStatus
                            }).Distinct(new Code.EqualComparer<Dto.Office.OfficeListDto>((x, y) => x.Id == y.Id)).OrderByDescending(m => m.ApproveDate);
                //分页
                WaitListPage(vm, wait);
            }
        }

        #region 分页
        private static void ApprovedListPage(Models.Office.OfficeListModel vm, IEnumerable<Dto.Office.OfficeListDto> approved)
        {
            vm.ApprovedDto = approved.ToPageList(vm.approvedPage);
            vm.approvedPage.PageCount = (int)Math.Ceiling(vm.approvedPage.TotalCount * 1.0 / vm.approvedPage.PageSize);
            vm.approvedPage.PageCount = vm.approvedPage.PageCount == 0 ? 1 : vm.approvedPage.PageCount;
        }

        private static void WaitListPage(Models.Office.OfficeListModel vm, IEnumerable<Dto.Office.OfficeListDto> wait)
        {
            vm.WaitApproveDto = wait.ToPageList(vm.waitPage);
            vm.waitPage.PageCount = (int)Math.Ceiling(vm.waitPage.TotalCount * 1.0 / vm.waitPage.PageSize);
            vm.waitPage.PageCount = vm.waitPage.PageCount == 0 ? 1 : vm.waitPage.PageCount;
        }

        private static void MyListPage(Models.Office.OfficeListModel vm, IEnumerable<Dto.Office.OfficeListDto> my)
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
            Models.Office.OfficeListModel vm = new Models.Office.OfficeListModel();
            vm.SegmentedTab = segmentedTab;
            using (var db = new XkSystem.Models.DbContext())
            {
                var firstNode = db.Table<Wechat.Entity.tbWeOAFlowNode>().Where(m => m.tbSysOAFlowType.Code == flowTypeCode && m.FlowStep == 1).FirstOrDefault();
                var workFlowList = OAWeWorkFlow.GetWorkFlowList(db, flowTypeCode, approveBodyId);
                var workflowApproveList = OAWeWorkFlow.GetWorkFlowApproveList(flowTypeCode);
                vm.WorkFlowListDto = (from a in workFlowList
                                      join b in db.Table<Wechat.Entity.tbWeOAOffice>() on a.ApproveBodyId equals b.Id
                                      select new Dto.Office.OfficeListDto
                                      {
                                          Title = b.Title,
                                          LimitDateTo = b.LimitDateTo,
                                          OfficeFileFrom = b.OfficeFileFrom,
                                          OfficeFileName = b.OfficeFileName,
                                          OfficeFileNameSeq = b.OfficeFileNameSeq,
                                          ReceiveFileTime = b.ReceiveFileTime,

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
        public ActionResult AddOffice()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Office.OfficeEditModel();
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
        public ActionResult AddOffice(Models.Office.OfficeEditModel vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (string.IsNullOrEmpty(vm.OfficeEditDto.OfficeFileName) && !string.IsNullOrEmpty(vm.OfficeEditDto.FileContent))
                {
                    ModelState.Remove("OfficeEditDto.OfficeFileName");
                }
                else if (!string.IsNullOrEmpty(vm.OfficeEditDto.OfficeFileName) && string.IsNullOrEmpty(vm.OfficeEditDto.FileContent))
                {
                    ModelState.Remove("OfficeEditDto.FileContent");
                }
                else if (string.IsNullOrEmpty(vm.OfficeEditDto.OfficeFileName) && string.IsNullOrEmpty(vm.OfficeEditDto.FileContent))
                {
                    ModelState.Remove("OfficeEditDto.FileContent");
                }
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    //保存附件,rar,word(doc),pdf
                    string FileName = string.Empty;
                    if (!string.IsNullOrEmpty(vm.OfficeEditDto.OfficeFileName))
                    {
                        var file = Request.Files[nameof(vm.OfficeEditDto) + "." + nameof(vm.OfficeEditDto.OfficeFileName)];
                        var suffix = file.FileName.Substring(file.FileName.LastIndexOf("."));
                        FileName = System.IO.Path.GetRandomFileName().Replace(".", string.Empty) + suffix;
                        vm.OfficeEditDto.OfficeFileName = file.FileName;
                        file.SaveAs(Server.MapPath("~/Files/OfficeFile/") + FileName);
                    }

                    //审批内容表记录
                    var tb = new Wechat.Entity.tbWeOAOffice();
                    tb.OfficeFileFrom = vm.OfficeEditDto.OfficeFileFrom;
                    tb.OfficeFileName = vm.OfficeEditDto.OfficeFileName;
                    tb.OfficeFileNameSeq = FileName;
                    tb.ReceiveFileTime = vm.OfficeEditDto.ReceiveFileTime;
                    tb.Title = vm.OfficeEditDto.Title;
                    tb.FileContent = vm.OfficeEditDto.FileContent;
                    tb.LimitDateTo = vm.OfficeEditDto.LimitDateTo;
                    db.Set<Wechat.Entity.tbWeOAOffice>().Add(tb);
                    db.SaveChanges();

                    //执行流程引擎
                    OAWeWorkFlow.ExecuteWorkFlowEngine(db, flowTypeCode, tb.Id, "", "发起流程", false, vm.OfficeEditDto.NextApproveUserId);
                }
                else
                {
                    vm.ErrorMsg = string.Join("\r\n", error);
                    return View(vm);//包含文件表单的方式
                }
                return Content("<script type='text/javascript'> window.parent.location.href = '" + Url.Action("OfficeIndex", "Office", new { area = "wechat" }) + "';</script>");
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
            return Content("<script type='text/javascript'> window.parent.location.href = '" + Url.Action("OfficeIndex", "Office", new { area = "wechat" }) + "';</script>");
        }
        #endregion

        #region 驳回流程
        public ActionResult RejectWorkFlow(int approveBodyId = 0, string conditionalFormula = "", string approveOpinion = "")
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                OAWeWorkFlow.ExecuteWorkFlowEngine(db, flowTypeCode, approveBodyId, conditionalFormula, approveOpinion, true);
            }
            return Content("<script type='text/javascript'> window.parent.location.href = '" + Url.Action("OfficeIndex", "Office", new { area = "wechat" }) + "';</script>");
        }
        #endregion

    }

    /// <summary>
    /// 微信端审批流引擎
    /// </summary>
    public static class OAWeWorkFlow
    {
        /// <summary>
        /// 审批流程
        /// </summary>
        /// <param name="flowTypeCode">流程类型</param>
        /// <param name="approveBodyId">流程审批内容ID</param>
        /// <param name="conditionalFormula">匹配公式</param>
        /// <param name="approveOpinion">审批意见</param>
        /// <param name="reject">是否驳回</param>
        /// <param name="nextApproveUserId">下一个节点指定的审批人</param>
        public static void ExecuteWorkFlowEngine(XkSystem.Models.DbContext db, string flowTypeCode, int approveBodyId, string conditionalFormula, string approveOpinion, bool reject = false, int nextApproveUserId = 0)
        {
            //流程类型
            var flowType = db.Table<Wechat.Entity.tbWeOAFlowType>().Where(m => m.Code == flowTypeCode).FirstOrDefault();

            //当前审批人或流程发起人,如果登录角色非发起人角色则没有提交审批申请单的功能
            var approveUser = db.Set<Sys.Entity.tbSysUser>().Find(XkSystem.Code.Common.UserId);

            //获取该审批项目的流程列表，排除驳回的流程
            var approveDetailList = db.Table<Wechat.Entity.tbWeOAFlowDetail>()
                .Where(m => m.tbWeOAFlowType.Code == flowTypeCode && m.ApproveBodyId == approveBodyId && m.NodeApproveStatus != Code.EnumHelper.OAFlowNodeStatus.WithoutApproval)
                .Include(m => m.tbWeOAFlowNextNode)
                .Include(m => m.tbWeOAFlowPreviousNode);

            //上次申请或审批的节点，可能有可能多个分支，e.g.:2个FlowStep=2
            var approveDetailNode = approveDetailList.OrderByDescending(m => m.tbWeOAFlowNextNode.FlowStep).FirstOrDefault();

            //查询当前审批流程是否有并列分支,判断走哪条分支，过滤流程节点，保留一个节点
            if (approveDetailNode != null)
            {
                var mulStepNo = (from m in approveDetailList group m by m.tbWeOAFlowNextNode.FlowStep into g where g.Count() > 1 select g.Key).FirstOrDefault();//???
                if (mulStepNo != 0)
                {
                    int step = Convert.ToInt32(mulStepNo);
                    //查询当前审批角色对应的是哪个节点
                    var flowApprovalNode = db.Table<Wechat.Entity.tbWeOAFlowApprover>().Where(m => m.tbWeOAFlowNode.tbSysOAFlowType.Code == flowTypeCode && m.tbSysUser.Id == approveUser.Id && m.tbWeOAFlowNode.FlowStep == step).Select(m => m.tbWeOAFlowNode.FlowApprovalNode).FirstOrDefault();
                    approveDetailNode = approveDetailList.Where(m => m.tbWeOAFlowNextNode.FlowStep == step && m.tbWeOAFlowNextNode.FlowApprovalNode == flowApprovalNode).FirstOrDefault();
                }
            }

            //1.发起流程
            if (approveDetailNode == null)
            {
                //发起流程可能有多个分支,则产生多个detail流程实例,该分支是或者关系
                var thisNextNodeList = db.Table<Wechat.Entity.tbWeOAFlowNode>().Where(m => m.tbSysOAFlowType.Code == flowTypeCode);
                if (!string.IsNullOrEmpty(conditionalFormula))
                {
                    thisNextNodeList = thisNextNodeList.Where(m => m.ConditionalFormula == conditionalFormula).OrderBy(m => m.FlowStep);
                }
                else
                {
                    thisNextNodeList = thisNextNodeList.Where(m => m.FlowStep == 2).OrderBy(m => m.FlowStep);//正常顺序流下一个流程是2
                }
                Wechat.Entity.tbWeOAFlowDetail tbWeOAFlowDetail = null;
                Sys.Entity.tbSysUser nextApproveUser = null;
                if (nextApproveUserId != 0)
                {
                    nextApproveUser = db.Set<Sys.Entity.tbSysUser>().Find(nextApproveUserId);
                }
                var step = 0;
                foreach (var thisNextNode in thisNextNodeList)
                {
                    if (step == thisNextNode.FlowStep || step == 0)
                    {
                        tbWeOAFlowDetail = new Wechat.Entity.tbWeOAFlowDetail();
                        tbWeOAFlowDetail.tbSysUser = approveUser;
                        tbWeOAFlowDetail.NodeApproveStatus = Code.EnumHelper.OAFlowNodeStatus.Approved;
                        tbWeOAFlowDetail.tbWeOAFlowType = flowType;
                        tbWeOAFlowDetail.ApproveBodyId = approveBodyId;
                        tbWeOAFlowDetail.ApproveDate = System.DateTime.Now;
                        tbWeOAFlowDetail.ApproveOpinion = approveOpinion;
                        tbWeOAFlowDetail.tbWeOAFlowPreviousNode = null;//我的申请则查询该字段为null的数据
                        tbWeOAFlowDetail.tbWeOAFlowNextNode = thisNextNode;
                        tbWeOAFlowDetail.AssignNextApproveUser = nextApproveUser;
                        db.Set<Wechat.Entity.tbWeOAFlowDetail>().Add(tbWeOAFlowDetail);
                    }
                    step = thisNextNode.FlowStep;
                }
                db.SaveChanges();
            }
            //2.审批流程,流程节点链条：P1-N1-P2(N1)-N2-P3(N2)-N3
            else
            {
                //执行下一个流程,**不考虑审批分支**
                if (approveDetailNode.tbWeOAFlowNextNode != null)
                {
                    //本次审批的上一个节点
                    var thisPreNode = db.Table<Wechat.Entity.tbWeOAFlowNode>().Where(m => m.tbSysOAFlowType.Code == flowTypeCode && m.FlowStep == approveDetailNode.tbWeOAFlowNextNode.FlowStep).FirstOrDefault();

                    //检查当前审批后是否还有下一个流程,比如用车申请司机关闭流程
                    Wechat.Entity.tbWeOAFlowNode thisNextNode = null;//为null则流程结束
                    int nextNodeNo = approveDetailNode.tbWeOAFlowNextNode.FlowStep;
                    if (approveDetailNode.tbWeOAFlowNextNode.FlowComplete == false)
                    {
                        nextNodeNo += 1;
                        //本次审批的下一个节点,如果驳回则为null
                        if (reject == false)
                        {
                            thisNextNode = db.Table<Wechat.Entity.tbWeOAFlowNode>().Where(m => m.tbSysOAFlowType.Code == flowTypeCode && m.FlowStep == nextNodeNo).FirstOrDefault();
                        }
                    }

                    var tbWeOAFlowDetail = new Wechat.Entity.tbWeOAFlowDetail();
                    tbWeOAFlowDetail.tbSysUser = approveUser;
                    tbWeOAFlowDetail.NodeApproveStatus = reject ? Code.EnumHelper.OAFlowNodeStatus.WithoutApproval : Code.EnumHelper.OAFlowNodeStatus.Approved;
                    tbWeOAFlowDetail.tbWeOAFlowType = flowType;
                    tbWeOAFlowDetail.ApproveBodyId = approveBodyId;
                    tbWeOAFlowDetail.ApproveDate = System.DateTime.Now;
                    tbWeOAFlowDetail.ApproveOpinion = approveOpinion;
                    tbWeOAFlowDetail.tbWeOAFlowPreviousNode = thisPreNode;
                    tbWeOAFlowDetail.tbWeOAFlowNextNode = thisNextNode;
                    if (nextApproveUserId != 0)
                    {
                        tbWeOAFlowDetail.AssignNextApproveUser = db.Set<Sys.Entity.tbSysUser>().Find(nextApproveUserId);
                    }
                    db.Set<Wechat.Entity.tbWeOAFlowDetail>().Add(tbWeOAFlowDetail);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 我的申请
        /// </summary>
        /// <param name="db"></param>
        /// <param name="flowTypeCode">流程类型</param>
        /// <returns></returns>
        public static IEnumerable<Wechat.Entity.tbWeOAFlowDetail> GetMyApplyList(XkSystem.Models.DbContext db, string flowTypeCode)
        {
            //我的申请，Detail的PreviousNode是null的情况
            var myApplyList = db.Table<Wechat.Entity.tbWeOAFlowDetail>().Where(m => m.tbWeOAFlowType.Code == flowTypeCode && m.tbWeOAFlowPreviousNode == null && m.tbSysUser.Id == XkSystem.Code.Common.UserId).Include(m => m.tbSysUser).Include(m => m.tbWeOAFlowPreviousNode).Include(m => m.tbWeOAFlowNextNode);

            return myApplyList;
        }

        /// <summary>
        /// 待审批
        /// </summary>
        /// <param name="db"></param>
        /// <param name="flowTypeCode">流程类型</param>
        /// <returns></returns>
        public static IEnumerable<Wechat.Entity.tbWeOAFlowDetail> GetWaitApproveList(XkSystem.Models.DbContext db, string flowTypeCode)
        {
            //该审批项目所有流程
            var waitApproveList = db.Table<Wechat.Entity.tbWeOAFlowDetail>().Where(m => m.tbWeOAFlowType.Code == flowTypeCode && m.tbWeOAFlowNextNode != null).Include(m => m.tbSysUser).Include(m => m.tbWeOAFlowPreviousNode).Include(m => m.tbWeOAFlowNextNode);

            //如果有指定审批人则要进行过滤(可以默认审批人也可以指定审批人，所以AssignNextApproveUser为null或者不为null)
            //waitApproveList = waitApproveList.Where(m => m.AssignNextApproveUser == null || (m.AssignNextApproveUser != null && m.AssignNextApproveUser.Id == XkSystem.Code.Common.UserId));

            //如果NextNode是null则流程结束，移除该待审批记录
            var maxNextNodes = from a in waitApproveList where a.tbWeOAFlowNextNode.FlowStep == (from b in waitApproveList where a.ApproveBodyId == b.ApproveBodyId select b.tbWeOAFlowNextNode.FlowStep).Max() select a;
            var nextNodes = maxNextNodes.Select(m => m.tbWeOAFlowNextNode.FlowStep).ToList();
            var filterNodes = db.Table<Wechat.Entity.tbWeOAFlowDetail>().Where(m => m.tbWeOAFlowType.Code == flowTypeCode && m.tbWeOAFlowNextNode == null && nextNodes.Contains(m.tbWeOAFlowPreviousNode.FlowStep)).Select(m => m.ApproveBodyId);

            //排除已经审批完的Detail//waitApproveList.Intersect(filterApproveList)
            var result = maxNextNodes.Where(m => !filterNodes.Contains(m.ApproveBodyId));

            //排除驳回的审批
            var rejectBodyIds = db.Table<Wechat.Entity.tbWeOAFlowDetail>().Where(m => m.tbWeOAFlowType.Code == flowTypeCode && m.NodeApproveStatus == Code.EnumHelper.OAFlowNodeStatus.WithoutApproval).Select(m => m.ApproveBodyId);
            result = result.Where(m => !rejectBodyIds.Contains(m.ApproveBodyId));

            //待审批,Detail的NextNode是自己的情况则是待审批
            var approverNodeList = db.Table<Wechat.Entity.tbWeOAFlowApprover>().Where(m => m.tbSysUser.Id == XkSystem.Code.Common.UserId).Select(m => m.tbWeOAFlowNode.FlowApprovalNode).ToList();
            result = result.Where(m => approverNodeList.Contains(m.tbWeOAFlowNextNode.FlowApprovalNode));

            //必须指定审批人的查询过滤,该代码放最后
            result = result.Where(m => m.AssignNextApproveUser != null && m.AssignNextApproveUser.Id == XkSystem.Code.Common.UserId);

            return result;
        }

        /// <summary>
        /// 已审批
        /// </summary>
        /// <param name="db"></param>
        /// <param name="flowTypeCode">流程类型</param>
        /// <returns></returns>
        public static IEnumerable<Wechat.Entity.tbWeOAFlowDetail> GetApprovedList(XkSystem.Models.DbContext db, string flowTypeCode)
        {
            //已审批,Detail的审批人是自己的情况
            var approvedList = db.Table<Wechat.Entity.tbWeOAFlowDetail>().Where(m => m.tbWeOAFlowType.Code == flowTypeCode && m.tbWeOAFlowPreviousNode != null && m.tbSysUser.Id == XkSystem.Code.Common.UserId).Include(m => m.tbSysUser).Include(m => m.tbWeOAFlowPreviousNode).Include(m => m.tbWeOAFlowNextNode);

            return approvedList;
        }

        /// <summary>
        /// 流程图
        /// </summary>
        /// <param name="db"></param>
        /// <param name="flowTypeCode"></param>
        /// <param name="approveBodyId"></param>
        /// <returns></returns>
        public static IEnumerable<Wechat.Entity.tbWeOAFlowDetail> GetWorkFlowList(XkSystem.Models.DbContext db, string flowTypeCode, int approveBodyId)
        {
            var approveList = db.Table<Wechat.Entity.tbWeOAFlowDetail>().Where(m => m.ApproveBodyId == approveBodyId && m.tbWeOAFlowType.Code == flowTypeCode).Include(m => m.tbSysUser).Include(m => m.tbWeOAFlowPreviousNode).Include(m => m.tbWeOAFlowNextNode).OrderBy(m => m.ApproveDate).ToList();
            var startFlows = approveList.Where(m => m.tbWeOAFlowPreviousNode == null).ToList();

            //拼接未审批的流程
            List<Wechat.Entity.tbWeOAFlowDetail> otherDetail = new List<Wechat.Entity.tbWeOAFlowDetail>();
            var maxNode = approveList.OrderByDescending(m => m.ApproveDate).FirstOrDefault();
            if (maxNode.tbWeOAFlowNextNode != null)
            {
                List<Wechat.Entity.tbWeOAFlowNode> otherNodes = new List<tbWeOAFlowNode>();
                otherNodes = db.Table<Wechat.Entity.tbWeOAFlowNode>().Where(m => m.tbSysOAFlowType.Code == flowTypeCode && m.FlowStep > maxNode.tbWeOAFlowNextNode.FlowStep).OrderBy(m => m.FlowStep).ToList();

                Wechat.Entity.tbWeOAFlowDetail detail = null;
                Wechat.Entity.tbWeOAFlowNode tempPreNode = null;
                for (int i = 0; i <= otherNodes.Count(); i++)
                {
                    detail = new Wechat.Entity.tbWeOAFlowDetail();
                    detail.ApproveBodyId = approveBodyId;
                    detail.tbWeOAFlowPreviousNode = i == 0 ? maxNode.tbWeOAFlowNextNode : tempPreNode;
                    if (i == otherNodes.Count())
                    {
                        detail.tbWeOAFlowNextNode = null;
                        tempPreNode = null;
                    }
                    else
                    {
                        detail.tbWeOAFlowNextNode = otherNodes[i];
                        tempPreNode = otherNodes[i];
                    }

                    otherDetail.Add(detail);

                }
                approveList = approveList.Concat(otherDetail).ToList();
            }

            //有2个分支,重构流程链
            if (startFlows.Count > 1)
            {
                approveList[2].tbWeOAFlowPreviousNode = startFlows[1].tbWeOAFlowNextNode;
                approveList[1].tbWeOAFlowPreviousNode = startFlows[0].tbWeOAFlowNextNode;
                approveList[1].tbSysUser = null;
            }
            return approveList;
        }

        /// <summary>
        /// 获取所有的发起流程列表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="flowTypeCode"></param>
        /// <param name="approveBodyId"></param>
        /// <returns></returns>
        public static IEnumerable<Wechat.Entity.tbWeOAFlowDetail> GetWorkFlowApproveList(string flowTypeCode)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var approveList = db.Table<Wechat.Entity.tbWeOAFlowDetail>().Where(m => m.tbWeOAFlowType.Code == flowTypeCode && m.tbWeOAFlowPreviousNode == null).Include(m => m.tbSysUser).ToList();
                return approveList;
            }
        }

        /// <summary>
        /// 获取所有已审批列表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="flowTypeCode">流程类型</param>
        /// <returns></returns>
        public static IEnumerable<Wechat.Entity.tbWeOAFlowDetail> GetAllApprovedList(string flowTypeCode)
        {
            //已审批,Detail的审批人是自己的情况
            using (var db = new XkSystem.Models.DbContext())
            {
                var approvedList = db.Table<Wechat.Entity.tbWeOAFlowDetail>().Where(m => m.tbWeOAFlowType.Code == flowTypeCode && m.tbWeOAFlowPreviousNode != null).Include(m => m.tbSysUser).Include(m => m.tbWeOAFlowPreviousNode).Include(m => m.tbWeOAFlowNextNode).ToList();

                return approvedList;
            }
        }

        /// <summary>
        /// 指定审批人，获取审批人列表
        /// 页面的条件不同，审批人不一样
        /// </summary>
        /// <param name="flowTypeCode"></param>
        /// <param name="conditionalFormula">公式</param>
        /// <returns></returns>
        public static IEnumerable<Code.MuiJsonDataBind> GetNextApproveUsers(string flowTypeCode, string conditionalFormula)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                List<Code.MuiJsonDataBind> approveUsers = new List<Code.MuiJsonDataBind>();

                var users = (from a in db.Table<Entity.tbWeOAFlowNode>()
                             join b in db.Table<Wechat.Entity.tbWeOAFlowApprover>()
                             on new { a.FlowStep, a.FlowApprovalNode, a.tbSysOAFlowType.Code } equals new { b.tbWeOAFlowNode.FlowStep, b.tbWeOAFlowNode.FlowApprovalNode, b.tbWeOAFlowNode.tbSysOAFlowType.Code }
                             where a.tbSysOAFlowType.Code == flowTypeCode
                             select new
                             {
                                 UserName = b.tbSysUser.UserName,
                                 UserId = b.tbSysUser.Id,
                                 FlowStep = a.FlowStep,
                                 ConditionalFormula = a.ConditionalFormula
                             }).ToList();

                //如果公式参数为空，则走顺序工作流
                if (string.IsNullOrEmpty(conditionalFormula))
                {
                    approveUsers = users.Where(m => m.FlowStep == 2).Select(m => new Code.MuiJsonDataBind
                    {
                        text = m.UserName,
                        value = m.UserId.ToString()
                    }).ToList();
                }
                //如果公式参数不为空，则按找公式字段匹配流程节点，如果匹配到多个节点，则按照节点顺序获取审批人
                else
                {
                    var flowStep = users.Where(m => m.ConditionalFormula == conditionalFormula).GroupBy(m => m.FlowStep).Select(m => m.Key).OrderBy(m => m).FirstOrDefault();
                    approveUsers = users.Where(m => m.ConditionalFormula == conditionalFormula && m.FlowStep == flowStep).Select(m => new Code.MuiJsonDataBind
                    {
                        text = m.UserName,
                        value = m.UserId.ToString()
                    }).ToList();
                }
                return approveUsers;
            }
        }
    }
}