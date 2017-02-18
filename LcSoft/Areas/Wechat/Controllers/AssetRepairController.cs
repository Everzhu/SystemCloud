using LcSoft.Areas.Asset.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LcSoft.Areas.Wechat.Controllers
{
    public class AssetRepairController : Controller
    {
        #region 报修单列表
        public ActionResult AssetRepairIndex(Asset.Models.AssetRepair.List vm)
        {
            using (var db = new LcSoft.Models.DbContext())
            {
                var tb = (from p in db.Table<Areas.Asset.Entity.tbAssetRepair>() select p)
                    .Include(m => m.tbAsset.tbAssetCatalog.tbAssetType)
                    .Include(m => m.tbAssetRepairLevel)
                    .Include(m => m.tbProcessUser)
                    .Include(m => m.tbSysUser);

                tb = tb.Where(m => m.tbSysUser.Id == Code.Common.UserId || m.tbProcessUser.Id == Code.Common.UserId);//申请人或者受理人只看自己的数据

                var types = (from p in db.Table<Areas.Asset.Entity.tbAssetType>()
                             select new Code.MuiJsonDataBind
                             {
                                 text = p.AssetTypeName,
                                 value = p.Id.ToString()
                             });
                vm.AssetTypeListJson = JsonConvert.SerializeObject(types);
                if (string.IsNullOrEmpty(vm.SubmitDate) == false && vm.SubmitDate != "0")
                {
                    var date = Convert.ToDateTime(vm.SubmitDate);
                    tb = tb.Where(m => m.InputDate >= date);
                }
                if (vm.Statu != null && vm.Statu > 0)
                {
                    tb = tb.Where(m => m.AssetRepairStatusCode == (Code.EnumHelper.AssetRepair)vm.Statu);
                }
                if (vm.AssetTypeId != null && vm.AssetTypeId != 0)
                {
                    tb = tb.Where(m => m.tbAsset.tbAssetCatalog.tbAssetType.Id == vm.AssetTypeId);
                }
                vm.AssetRepairList = (from p in tb
                                      join a in db.Table<Areas.Asset.Entity.tbAssetRepairAppraise>() on p.Id equals a.tbAssetRepair.Id into g
                                      from x in g.DefaultIfEmpty()
                                      select new Asset.Dto.AssetRepair.List()
                                      {

                                          AssetName = p.tbAsset.AssetName,
                                          AssetCatalogName = p.tbAsset.tbAssetCatalog.AssetCatalogName,
                                          AssetRepairStatusCode = p.AssetRepairStatusCode,
                                          Id = p.Id,
                                          InputDate = p.InputDate,
                                          AcceptDate = p.AcceptDate,
                                          CompleteDate = p.CompleteDate,
                                          AssetRepairLevelName = p.tbAssetRepairLevel.AssetRepairLevelName,
                                          ProcessUser = p.tbProcessUser.UserName,
                                          CreateUser = p.tbSysUser.UserName,
                                          CreateUserId = p.tbSysUser.Id,
                                          ProcessUserId = p.tbProcessUser == null ? 0 : p.tbProcessUser.Id,
                                          Remark = p.Remark,
                                          FileName = p.FileName,
                                          IsAppraise = x != null
                                          //IsAppraise = db.Table<Entity.tbAssetRepairAppraise>().Where(m=>m.tbAssetRepair.Id == p.Id).FirstOrDefault() != null
                                      }).ToList();
                if (Request.IsAjaxRequest())
                {
                    return PartialView("PList", vm);
                }

                return View(vm);
            }
        }
        #endregion

        #region 分派报修受理人
        public ActionResult RepairAssign()
        {
            using (var db = new LcSoft.Models.DbContext())
            {
                var vm = new Asset.Models.AssetRepair.List();
                var Users = (from p in db.Table<Sys.Entity.tbSysUserRole>().Include(d => d.tbSysUser)
                             where p.tbSysRole.RoleCode == Code.EnumHelper.SysRoleCode.Repair
                             select new Code.MuiJsonDataBind
                             {
                                 text = p.tbSysUser.UserName,
                                 value = p.tbSysUser.Id.ToString()
                             });
                vm.AssetTypeListJson = JsonConvert.SerializeObject(Users);
                var tb = (from p in db.Table<Asset.Entity.tbAssetRepair>() where p.AssetRepairStatusCode == Code.EnumHelper.AssetRepair.UnProcessed select p).Include(m => m.tbAsset.tbAssetCatalog.tbAssetType).Include(m => m.tbAssetRepairLevel).Include(m => m.tbProcessUser).Include(m => m.tbSysUser);
                vm.AssetRepairList = (from p in tb
                                      select new Asset.Dto.AssetRepair.List()
                                      {
                                          AssetName = p.tbAsset.AssetName,
                                          AssetCatalogName = p.tbAsset.tbAssetCatalog.AssetCatalogName,
                                          AssetRepairStatusCode = p.AssetRepairStatusCode,
                                          Id = p.Id,
                                          InputDate = p.InputDate,
                                          AssetRepairLevelName = p.tbAssetRepairLevel.AssetRepairLevelName,
                                          ProcessUser = p.tbProcessUser.UserName,
                                          CreateUser = p.tbSysUser.UserName,
                                          //CreateUserId = p.tbSysUser.Id,
                                          //ProcessUserId = p.tbProcessUser.Id,
                                          Remark = p.Remark,
                                          FileName = p.FileName
                                      }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult AssetRepairAssign(int AssetRepairId, int UserId)
        {
            var error = new List<string>();
            using (var db = new LcSoft.Models.DbContext())
            {
                var tb = (from p in db.Table<Asset.Entity.tbAssetRepair>() select p).Where(d => d.Id == AssetRepairId).FirstOrDefault();
                if (tb != null)
                {
                    tb.tbProcessUser = db.Set<Sys.Entity.tbSysUser>().Find(UserId);
                    tb.AssetRepairStatusCode = Code.EnumHelper.AssetRepair.Processing;
                    tb.AcceptDate = System.DateTime.Now;
                    db.SaveChanges();
                }
                else
                {
                    error.AddError(Resources.LocalizedText.MsgNotFound);
                }
            }
            return Code.MvcHelper.Post(error);
        }
        #endregion

        #region 申请报修
        public ActionResult AddAssetRepair()
        {
            using (var db = new LcSoft.Models.DbContext())
            {
                var vm = new Asset.Models.AssetRepair.Edit();
                List<SelectListItem> ll = new List<SelectListItem>();
                var tb = (from p in db.Table<Asset.Entity.tbAssetRepairLevel>()
                          orderby p.AssetRepairLevelName
                          select new
                          {
                              Value = p.Id,
                              Text = p.AssetRepairLevelName
                          }).ToList();
                foreach (var item in tb)
                {
                    SelectListItem ii = new SelectListItem();
                    ii.Value = item.Value.ConvertToString();
                    ii.Text = item.Text;
                    ll.Add(ii);
                }
                vm.AssetRepairLevelList = ll;
                return View(vm);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAssetRepair(Asset.Models.AssetRepair.Edit vm)
        {
            var error = new List<string>();
            using (var db = new LcSoft.Models.DbContext())
            {
                if (vm.AssetRepairEdit.Id == 0)
                {
                    var file = Request.Files[nameof(vm.AssetRepairEdit) + "." + nameof(vm.AssetRepairEdit.FileName)];
                    vm.AssetRepairEdit.FileName = file.FileName;
                    string FileName = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
                    string ConType = System.IO.Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath("~/Files/AssetRepair/") + FileName + "." + ConType);

                    //var tbAsset = db.Set<Asset.Entity.tbAsset>().Find(vm.AssetRepairEdit.AssetId);

                    var assetName = vm.AssetRepairEdit.AssetName.Trim();

                    var tbAsset = (from p in db.Table<Asset.Entity.tbAsset>().Include(p=>p.tbAssetStatus) where p.AssetName.Equals(assetName) && p.tbAssetCatalog.tbAssetType.AssetTypeCode== Code.EnumHelper.AssetType.FixedAssets select p).FirstOrDefault();

                    if (tbAsset == null)
                    {
                        //资产不存在，创建资产
                        tbAsset = new Asset.Entity.tbAsset()
                        {
                            AssetNo = Guid.NewGuid().ToString().Replace("-", string.Empty),
                            AssetName = assetName,
                            Count = 1,
                            InputDate = DateTime.Now,
                            Remark = string.Empty,
                            tbAssetCatalog = db.Table<Asset.Entity.tbAssetCatalog>().Where(p => p.tbAssetType.AssetTypeCode == Code.EnumHelper.AssetType.FixedAssets).FirstOrDefault(),
                            tbAssetStatus = (from p in db.Table<tbAssetStatus>() where p.AssetStatusCode == Code.EnumHelper.AssetStatus.Normal select p).FirstOrDefault(),
                            tbAssetStorage = db.Table<tbAssetStorage>().FirstOrDefault()
                        };
                        db.Set<tbAsset>().Add(tbAsset);
                        db.SaveChanges();
                    }
                    else
                    {
                        if (tbAsset.tbAssetStatus.AssetStatusCode != Code.EnumHelper.AssetStatus.Normal)
                        {
                            return Content($"<script type='text/javascript'>$(function(){{mui.alert('当前资产状态为{tbAsset.tbAssetStatus.AssetStatusCode.GetDescription()},无法进行维修');}});</script>");
                        }
                    }
                    var tbAssetRepair = new Asset.Entity.tbAssetRepair()
                    {
                        InputDate = DateTime.Now,
                        No = vm.AssetRepairEdit.No,
                        Remark = vm.AssetRepairEdit.Remark,
                        tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId),
                        tbAsset = tbAsset,
                        tbAssetRepairLevel = db.Set<Asset.Entity.tbAssetRepairLevel>().Find(vm.AssetRepairEdit.AssetRepairLevelId),
                        FileName = FileName + "." + ConType
                    };
                    tbAsset.tbAssetStatus = db.Set<Asset.Entity.tbAssetStatus>().Find((int)Code.EnumHelper.AssetStatus.Repair);
                    db.Set<Asset.Entity.tbAssetRepair>().Add(tbAssetRepair);
                    if (db.SaveChanges() > 0)
                    {
                        Sys.Controllers.SysUserLogController.Insert("添加了消耗品维修!");
                    }
                }
                else
                {
                    var tbAssetRepair = (from p in db.Table<Asset.Entity.tbAssetRepair>().Include(p => p.tbAsset).Include(p => p.tbAsset.tbAssetStatus) where p.Id == vm.AssetRepairEdit.Id select p).FirstOrDefault();
                    if (tbAssetRepair != null)
                    {
                        tbAssetRepair.No = vm.AssetRepairEdit.No;
                        tbAssetRepair.Remark = vm.AssetRepairEdit.Remark;
                        tbAssetRepair.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);

                        if (tbAssetRepair.tbAsset.Id != vm.AssetRepairEdit.AssetId)
                        {
                            tbAssetRepair.tbAsset.tbAssetStatus = db.Set<Asset.Entity.tbAssetStatus>().Find((int)Code.EnumHelper.AssetStatus.Normal);       //恢复修改前的资产的状态
                            var tbAsset = db.Set<Asset.Entity.tbAsset>().Find(vm.AssetRepairEdit.AssetId);
                            tbAssetRepair.tbAsset = tbAsset;
                            tbAsset.tbAssetStatus = db.Set<Asset.Entity.tbAssetStatus>().Find((int)Code.EnumHelper.AssetStatus.Repair);     //设置新资产状态
                        }

                        if (db.SaveChanges() > 0)
                        {
                            Sys.Controllers.SysUserLogController.Insert("修改了消耗品维修!");
                        }
                    }
                    else
                    {
                        return Content("<script type='text/javascript'>$(function(){  mui.alert('" + Resources.LocalizedText.MsgNotFound + "');});</script>");
                    }
                }
            }
            if (error.Count > 0)
            {
                return Content("<script type='text/javascript'>$(function(){  mui.alert('" + string.Join("\r\n", error) + "');});</script>");
            }
            return Content("<script type='text/javascript'> window.parent.location.href = '" + Url.Action("AssetRepairIndex", "AssetRepair", new { area = "wechat" }) + "';</script>");
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddAssetRepair(Asset.Models.AssetRepair.Edit vm)
        //{
        //    var error = new List<string>();
        //    using (var db = new LcSoft.Models.DbContext())
        //    {
        //        if (vm.AssetRepairEdit.Id == 0)
        //        {
        //            var file = Request.Files[nameof(vm.AssetRepairEdit) + "." + nameof(vm.AssetRepairEdit.FileName)];
        //            vm.AssetRepairEdit.FileName = file.FileName;
        //            string FileName = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
        //            string ConType = System.IO.Path.GetExtension(file.FileName);
        //            file.SaveAs(Server.MapPath("~/Files/AssetRepair/") + FileName + "." + ConType);

        //            var tbAsset = db.Set<Asset.Entity.tbAsset>().Find(vm.AssetRepairEdit.AssetId);
        //            var tbAssetRepair = new Asset.Entity.tbAssetRepair()
        //            {
        //                InputDate = DateTime.Now,
        //                No = vm.AssetRepairEdit.No,
        //                Remark = vm.AssetRepairEdit.Remark,
        //                tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId),
        //                tbAsset = tbAsset,
        //                tbAssetRepairLevel = db.Set<Asset.Entity.tbAssetRepairLevel>().Find(vm.AssetRepairEdit.AssetRepairLevelId),
        //                FileName = FileName + "." + ConType
        //            };
        //            tbAsset.tbAssetStatus = db.Set<Asset.Entity.tbAssetStatus>().Find((int)Code.EnumHelper.AssetStatus.Repair);
        //            db.Set<Asset.Entity.tbAssetRepair>().Add(tbAssetRepair);
        //            if (db.SaveChanges() > 0)
        //            {
        //                Sys.Controllers.SysUserLogController.Insert("添加了消耗品维修!");
        //            }
        //        }
        //        else
        //        {
        //            var tbAssetRepair = (from p in db.Table<Asset.Entity.tbAssetRepair>().Include(p => p.tbAsset).Include(p => p.tbAsset.tbAssetStatus) where p.Id == vm.AssetRepairEdit.Id select p).FirstOrDefault();
        //            if (tbAssetRepair != null)
        //            {
        //                tbAssetRepair.No = vm.AssetRepairEdit.No;
        //                tbAssetRepair.Remark = vm.AssetRepairEdit.Remark;
        //                tbAssetRepair.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);

        //                if (tbAssetRepair.tbAsset.Id != vm.AssetRepairEdit.AssetId)
        //                {
        //                    tbAssetRepair.tbAsset.tbAssetStatus = db.Set<Asset.Entity.tbAssetStatus>().Find((int)Code.EnumHelper.AssetStatus.Normal);       //恢复修改前的资产的状态
        //                    var tbAsset = db.Set<Asset.Entity.tbAsset>().Find(vm.AssetRepairEdit.AssetId);
        //                    tbAssetRepair.tbAsset = tbAsset;
        //                    tbAsset.tbAssetStatus = db.Set<Asset.Entity.tbAssetStatus>().Find((int)Code.EnumHelper.AssetStatus.Repair);     //设置新资产状态
        //                }

        //                if (db.SaveChanges() > 0)
        //                {
        //                    Sys.Controllers.SysUserLogController.Insert("修改了消耗品维修!");
        //                }
        //            }
        //            else
        //            {
        //                return Content("<script type='text/javascript'>$(function(){  mui.alert('" + Resources.LocalizedText.MsgNotFound + "');});</script>");
        //            }
        //        }
        //    }
        //    if (error.Count > 0)
        //    {
        //        return Content("<script type='text/javascript'>$(function(){  mui.alert('" + string.Join("\r\n", error) + "');});</script>");
        //    }
        //    return Content("<script type='text/javascript'> window.parent.location.href = '" + Url.Action("AssetRepairIndex", "AssetRepair", new { area = "wechat" }) + "';</script>");
        //}
        #endregion

        #region 受理人接单
        [HttpPost]
        public ActionResult AcceptAsset(int AssetRepairId)
        {
            var error = new List<string>();
            using (var db = new LcSoft.Models.DbContext())
            {
                var tb = (from p in db.Table<Asset.Entity.tbAssetRepair>() select p).Where(d => d.Id == AssetRepairId).FirstOrDefault();
                if (tb != null)
                {
                    tb.tbProcessUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                    tb.AssetRepairStatusCode = Code.EnumHelper.AssetRepair.Processing;
                    tb.AcceptDate = System.DateTime.Now;
                    db.SaveChanges();
                }
                else
                {
                    error.AddError(Resources.LocalizedText.MsgNotFound);
                }
            }
            return Code.MvcHelper.Post(error);
        }
        #endregion

        #region 受理人关单，反馈
        /// <summary>
        /// 查看反馈结果
        /// </summary>
        /// <param name="AssetRepairId"></param>
        /// <returns></returns>
        public ActionResult ViewAssetFeedBack(int AssetRepairId)
        {
            var vm = new Models.AssetRepair.AssetFeedBackEditModel();
            using (var db = new LcSoft.Models.DbContext())
            {
                vm.AssetFeedBackEditDto = db.Table<Asset.Entity.tbAssetFeedBack>()
                    .Where(m => m.tbAssetRepair.Id == AssetRepairId)
                    .Select(m => new Dto.AssetRepair.AssetFeedBackEditDto()
                    {
                        FileName = m.FileName,
                        Remark = m.Remark,
                        tbAssetRepairId = m.tbAssetRepair.Id
                    }).FirstOrDefault();
            }
            return View(vm);
        }
        /// <summary>
        /// 受理人反馈编辑页面
        /// </summary>
        /// <param name="AssetRepairId"></param>
        /// <returns></returns>
        public ActionResult AssetFeedBack(int AssetRepairId)
        {
            var vm = new Models.AssetRepair.AssetFeedBackEditModel();
            vm.AssetFeedBackEditDto.tbAssetRepairId = AssetRepairId;
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewAssetFeedBack(Models.AssetRepair.AssetFeedBackEditModel vm)
        {
            var error = new List<string>();
            using (var db = new LcSoft.Models.DbContext())
            {
                var file = Request.Files[nameof(vm.AssetFeedBackEditDto) + "." + nameof(vm.AssetFeedBackEditDto.FileName)];
                vm.AssetFeedBackEditDto.FileName = file.FileName;
                string FileName = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
                string ConType = System.IO.Path.GetExtension(file.FileName);
                if (!string.IsNullOrEmpty(ConType))
                {
                    file.SaveAs(Server.MapPath("~/Files/AssetRepair/") + FileName + "." + ConType);
                }

                var tbAssetRepair = db.Set<Asset.Entity.tbAssetRepair>().Find(vm.AssetFeedBackEditDto.tbAssetRepairId);
                var tbAssetFeedBack = new Asset.Entity.tbAssetFeedBack()
                {
                    tbAssetRepair = tbAssetRepair,
                    Remark = vm.AssetFeedBackEditDto.Remark,
                    FileName = string.IsNullOrEmpty(ConType)?"": FileName + "." + ConType
                };
                db.Set<Asset.Entity.tbAssetFeedBack>().Add(tbAssetFeedBack);

                //修改状态,完成报修
                tbAssetRepair.tbProcessUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                tbAssetRepair.AssetRepairStatusCode = Code.EnumHelper.AssetRepair.Processed;
                tbAssetRepair.CompleteDate = System.DateTime.Now;
                db.SaveChanges();

                if (db.SaveChanges() > 0)
                {
                    Sys.Controllers.SysUserLogController.Insert("完成受理，反馈成功!");
                }
            }
            if (error.Count > 0)
            {
                return Content("<script type='text/javascript'>$(function(){  mui.alert('" + string.Join("\r\n", error) + "');});</script>");
            }
            return Content("<script type='text/javascript'> window.parent.location.href = '" + Url.Action("AssetRepairIndex", "AssetRepair", new { area = "Wechat" }) + "';</script>");
        }
        #endregion

        #region 申请人评价
        public ActionResult Appraise(int AssetRepairId, bool IsView)
        {
            var vm = new Models.AssetRepair.AssetRepairAppraiseEditModel();
            if (IsView)
            {
                using (var db = new LcSoft.Models.DbContext())
                {
                    vm.AssetRepairAppraiseEditDto = db.Table<Areas.Asset.Entity.tbAssetRepairAppraise>()
                        .Where(m => m.tbAssetRepair.Id == AssetRepairId)
                        .Select(m => new Dto.AssetRepair.AssetRepairAppraiseEditDto()
                        {
                            IsPleased = m.IsPleased,
                            IsService = m.IsService,
                            Opinion = m.Opinion,
                            tbAssetRepairId = m.tbAssetRepair.Id
                        }).FirstOrDefault();
                }

            }
            else
            {
                vm.AssetRepairAppraiseEditDto.tbAssetRepairId = AssetRepairId;
            }

            vm.AssetRepairAppraiseEditDto.IsView = IsView;
            return View(vm);
        }

        [HttpPost]
        public ActionResult AssetRepairAppraise(bool IsPleased, bool IsService, string Opinion, int AssetRepairId)
        {
            var error = new List<string>();
            using (var db = new LcSoft.Models.DbContext())
            {
                var tbAssetRepairA = new Areas.Asset.Entity.tbAssetRepairAppraise();
                tbAssetRepairA.No = 0;
                tbAssetRepairA.IsPleased = IsPleased;
                tbAssetRepairA.IsService = IsService;
                tbAssetRepairA.Opinion = Opinion;
                tbAssetRepairA.tbAssetRepair = db.Set<Areas.Asset.Entity.tbAssetRepair>().Find(AssetRepairId);
                tbAssetRepairA.tbSysUser = db.Set<Sys.Entity.tbSysUser>().Find(Code.Common.UserId);
                db.Set<Areas.Asset.Entity.tbAssetRepairAppraise>().Add(tbAssetRepairA);
                db.SaveChanges();
            }
            return Code.MvcHelper.Post(error);
        }
        #endregion

        #region 查看申请描述
        public ActionResult ViewAssetApply(int AssetRepairId)
        {
            var vm = new Asset.Dto.AssetRepair.List();
            using (var db = new LcSoft.Models.DbContext())
            {
                vm = db.Set<Areas.Asset.Entity.tbAssetRepair>()
                    .Where(m => m.Id == AssetRepairId)
                    .Select(m => new Asset.Dto.AssetRepair.List()
                    {
                        FileName = m.FileName,
                        Remark = m.Remark,
                    }).FirstOrDefault();
            }
            return View(vm);
        }
        #endregion

        #region 报修统计
        public ActionResult RepairStatistics(Models.AssetRepair.RepairStatisticsListModel vm)
        {
            using (var db = new LcSoft.Models.DbContext())
            {
                var RepairTb = (from p in db.Table<Areas.Asset.Entity.tbAssetRepair>() select p).Include(m => m.tbAsset.tbAssetCatalog.tbAssetType).Include(m => m.tbAssetRepairLevel).Include(m => m.tbProcessUser).Include(m => m.tbSysUser).ToList();

                var AppraiseTb = (from p in db.Table<Areas.Asset.Entity.tbAssetRepairAppraise>() select p).Include(m => m.tbAssetRepair).Include(m => m.tbAssetRepair.tbProcessUser).ToList();

                var Users = from p in db.Table<Sys.Entity.tbSysUserRole>().Include(d => d.tbSysUser)
                            where p.tbSysRole.RoleCode == Code.EnumHelper.SysRoleCode.Repair
                            select new Dto.AssetRepair.RepairStatisticsListDto
                            {
                                Id = p.tbSysUser.Id,
                                UserName = p.tbSysUser.UserName
                            };
                var RepairStatisticsList = Users.OrderByDescending(d => d.Id).ToPageList(vm.Page);
                vm.RepairStatisticsListDto = RepairStatisticsList;
                foreach (var item in vm.RepairStatisticsListDto)
                {
                    item.AcceptedCount = RepairTb.Where(d => d.AssetRepairStatusCode == Code.EnumHelper.AssetRepair.Processing && d.tbProcessUser.Id == item.Id).Count();
                    item.FinishCount = RepairTb.Where(d => d.AssetRepairStatusCode == Code.EnumHelper.AssetRepair.Processed && d.tbProcessUser.Id == item.Id).Count();
                    item.SatisfiedCount = AppraiseTb.Where(d => d.IsPleased == true && d.IsService == true && d.tbAssetRepair.tbProcessUser.Id == item.Id).Count();
                    item.BadReviewCount = AppraiseTb.Where(d => (d.IsPleased == false || d.IsService == false) && d.tbAssetRepair.tbProcessUser.Id == item.Id).Count();
                }
                vm.Page.PageCount = (int)Math.Ceiling(vm.Page.TotalCount * 1.0 / vm.Page.PageSize);
                vm.Page.PageCount = vm.Page.PageCount == 0 ? 1 : vm.Page.PageCount;
                if (Request.IsAjaxRequest())
                {
                    return PartialView("RList", vm);
                }
                return View(vm);
            }
        }

        public ActionResult RepairStatisticsInfo(Models.AssetRepair.RepairStatisticsInfoModel vm)
        {
            using (var db = new LcSoft.Models.DbContext())
            {
                var tb = (from p in db.Table<Areas.Asset.Entity.tbAssetRepair>() select p)
                            .Include(m => m.tbAsset.tbAssetCatalog.tbAssetType)
                            .Include(m => m.tbAssetRepairLevel)
                            .Include(m => m.tbProcessUser)
                            .Include(m => m.tbSysUser);
                var AppraiseList = (from p in db.Table<Areas.Asset.Entity.tbAssetRepairAppraise>() where p.tbAssetRepair.tbProcessUser.Id == vm.UserId select p).Include(m => m.tbAssetRepair).Include(m => m.tbAssetRepair.tbProcessUser).ToList();
                if (vm.InfoType == 0)
                {
                    tb = tb.Where(m => m.AssetRepairStatusCode == Code.EnumHelper.AssetRepair.Processing && m.tbProcessUser.Id == vm.UserId);//
                }
                else if (vm.InfoType == 1)
                {
                    tb = tb.Where(m => m.AssetRepairStatusCode == Code.EnumHelper.AssetRepair.Processed && m.tbProcessUser.Id == vm.UserId);//
                }
                else if (vm.InfoType == 2)
                {
                    var StatiList = AppraiseList.Where(d => d.IsPleased == true && d.IsService == true).Select(p => p.tbAssetRepair.Id);
                    tb = tb.Where(m => m.AssetRepairStatusCode == Code.EnumHelper.AssetRepair.Processed && StatiList.Contains(m.Id));//
                }
                else if (vm.InfoType == 3)
                {
                    var BadList = AppraiseList.Where(d => d.IsPleased == false || d.IsService == false).Select(p => p.tbAssetRepair.Id);
                    tb = tb.Where(m => m.AssetRepairStatusCode == Code.EnumHelper.AssetRepair.Processed && BadList.Contains(m.Id));//
                }

                var q = from p in tb
                        join a in db.Table<Areas.Asset.Entity.tbAssetRepairAppraise>() on p.Id equals a.tbAssetRepair.Id into g
                        from x in g.DefaultIfEmpty()
                        select new Asset.Dto.AssetRepair.List()
                        {

                            AssetName = p.tbAsset.AssetName,
                            AssetCatalogName = p.tbAsset.tbAssetCatalog.AssetCatalogName,
                            AssetRepairStatusCode = p.AssetRepairStatusCode,
                            Id = p.Id,
                            InputDate = p.InputDate,
                            AcceptDate = p.AcceptDate,
                            CompleteDate = p.CompleteDate,
                            AssetRepairLevelName = p.tbAssetRepairLevel.AssetRepairLevelName,
                            ProcessUser = p.tbProcessUser.UserName,
                            CreateUser = p.tbSysUser.UserName,
                            CreateUserId = p.tbSysUser.Id,
                            ProcessUserId = p.tbProcessUser == null ? 0 : p.tbProcessUser.Id,
                            Remark = p.Remark,
                            FileName = p.FileName,
                            IsAppraise = x != null
                        };
                var AssetRepairList = q.OrderByDescending(d => d.InputDate).ToPageList(vm.Page);
                vm.AssetRepairList = AssetRepairList;
                vm.Page.PageCount = (int)Math.Ceiling(vm.Page.TotalCount * 1.0 / vm.Page.PageSize);
                vm.Page.PageCount = vm.Page.PageCount == 0 ? 1 : vm.Page.PageCount;
                if (Request.IsAjaxRequest())
                {
                    return PartialView("SList", vm);
                }
                return View(vm);
            }
        } 
        #endregion
    }
}