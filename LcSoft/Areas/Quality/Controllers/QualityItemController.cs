using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace XkSystem.Areas.Quality.Controllers
{
    public class QualityItemController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityItem.List();
                vm.QualityItemTypeList = typeof(Code.EnumHelper.QualityItemType).ToItemList();
                var tb = from p in db.Table<Quality.Entity.tbQualityItem>()
                         .Include(d=>d.tbQualityItemGroup)
                         where p.tbQualityItemGroup.tbQuality.Id==vm.QualityId
                         && p.IsDeleted==false
                         && p.tbQualityItemGroup.IsDeleted==false
                         && ((vm.QualityItemGroupId==null || vm.QualityItemGroupId==0)? true :p.tbQualityItemGroup.Id==vm.QualityItemGroupId)
                         select p;
                var aa = tb.ToList();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.QualityItemName.Contains(vm.SearchText));
                }

                vm.QualityItemGroupList = (from p in db.Table<Quality.Entity.tbQualityItemGroup>()
                                           where p.tbQuality.Id == vm.QualityId
                                           orderby p.No
                                           select new System.Web.Mvc.SelectListItem
                                           {
                                               Text = p.QualityItemGroupName,
                                               Value = p.Id.ToString(),
                                           }).Distinct().ToList();

                vm.QualityItemList = (from p in tb
                                      orderby p.No
                                      select new Dto.QualityItem.List
                                      {
                                          Id = p.Id,
                                          No = p.No,
                                          QualityItemName = p.QualityItemName,
                                          QualityItemType = p.QualityItemType,
                                          QualityItemGroupName=p.tbQualityItemGroup.QualityItemGroupName,
                                          IsVertical = p.IsVertical,
                                      }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.QualityItem.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, qualityItemGroupId = vm.QualityItemGroupId,qualityId=vm.QualityId }));
        }

        public ActionResult Edit(int id=0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityItem.Edit();
                vm.QualityItemTypeList = typeof(Code.EnumHelper.QualityItemType).ToItemList();
                vm.QualityItemGroupList = (from p in db.Table<Quality.Entity.tbQualityItemGroup>()
                                           where p.tbQuality.Id==vm.QualityId
                                           orderby p.No
                                           select new System.Web.Mvc.SelectListItem
                                           {
                                               Text = p.QualityItemGroupName,
                                               Value = p.Id.ToString(),
                                      }).Distinct().ToList();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Quality.Entity.tbQualityItem>()
                               .Include(d => d.tbQualityItemGroup)
                              where p.Id == id
                              select new Dto.QualityItem.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  QualityItemName = p.QualityItemName,
                                  IsVertical = p.IsVertical,
                                  QualityItemType = p.QualityItemType,
                                  QualityItemGroupId=p.tbQualityItemGroup.Id,
                              }).FirstOrDefault();
                   
                    if (tb != null)
                    {
                        vm.QualityItemEdit = tb;
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.QualityItem.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (Request.Form["txtIsVertical"] == "1")
                {
                    vm.QualityItemEdit.IsVertical = true;
                }
                else if (Request.Form["txtIsVertical"] == "0")
                {
                    vm.QualityItemEdit.IsVertical = false;
                }

                if (Request.Form["txtQualityItemTypeId"] != null)
                {
                    if (Request.Form["txtQualityItemTypeId"] =="0")
                    {
                        vm.QualityItemEdit.QualityItemType = Code.EnumHelper.QualityItemType.Radio;
                    }
                    else if (Request.Form["txtQualityItemTypeId"] == "1")
                    {
                        vm.QualityItemEdit.QualityItemType = Code.EnumHelper.QualityItemType.CheckBox;
                    }
                    else if (Request.Form["txtQualityItemTypeId"] == "2")
                    {
                        vm.QualityItemEdit.QualityItemType = Code.EnumHelper.QualityItemType.TextBox;
                    }
                }
                if (error.Count == decimal.Zero)
                {
                    var tb = new Quality.Entity.tbQualityItem();
                    if (vm.QualityItemEdit.Id == 0)
                    {
                        tb = new Quality.Entity.tbQualityItem();
                        tb.No = vm.QualityItemEdit.No == null ? db.Table<Quality.Entity.tbQualityItem>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.QualityItemEdit.No;
                        tb.QualityItemName = vm.QualityItemEdit.QualityItemName;
                        tb.QualityItemType = vm.QualityItemEdit.QualityItemType;
                        tb.IsVertical = vm.QualityItemEdit.IsVertical;
                        tb.tbQualityItemGroup = db.Set<Quality.Entity.tbQualityItemGroup>().Find(vm.QualityItemEdit.QualityItemGroupId);
                        db.Set<Quality.Entity.tbQualityItem>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价内容");
                        }
                    }
                    else
                    {
                        tb = (from p in db.Table<Quality.Entity.tbQualityItem>()
                                  where p.Id == vm.QualityItemEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.QualityItemEdit.No == null ? db.Table<Quality.Entity.tbQualityItem>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.QualityItemEdit.No;
                            tb.tbQualityItemGroup = db.Set<Quality.Entity.tbQualityItemGroup>().Find(vm.QualityItemEdit.QualityItemGroupId);
                            tb.QualityItemName = vm.QualityItemEdit.QualityItemName;
                            tb.QualityItemType = vm.QualityItemEdit.QualityItemType;
                            tb.IsVertical = vm.QualityItemEdit.IsVertical;
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价内容");
                        }
                    }

                    using (TransactionScope scope = new TransactionScope())
                    {
                        if (Request["txtId"] != null)
                        {
                            //保存评价选项数据
                            new QualityOptionController().SaveQualityOption(db, tb, Request);
                        }
                        db.SaveChanges();
                        scope.Complete();
                    }
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
                var tb = (from p in db.Table<Quality.Entity.tbQualityItem>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价内容");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult GetJsonQualityItemType(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityItem.List();
                vm.QualityItemTypeList = typeof(Code.EnumHelper.QualityItemType).ToItemList();
                return Json(vm.QualityItemTypeList, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 保存评价内容页面数据
        /// </summary>
        /// <param name="db">数据库连接</param>
        /// <param name="surveyGroup">分组Entity</param>
        /// <param name="request">请求参数</param>
        /// <returns></returns>
        [NonAction]
        public bool SaveQualityItem(XkSystem.Models.DbContext db, Quality.Entity.tbQualityItemGroup surveyGroup, HttpRequestBase request, List<Quality.Entity.tbQualityItem> surveyItems)
        {
            var txtId = request["txtId"].Split(',');
            var txtNo = request["txtNo"].Split(',');
            var txtName = request["txtName"].Split(',');
            var txtIsVertical = request["txtIsVertical"].Split(',');
            var txtQualityItemTypeId = request["txtQualityItemTypeId"].Split(',');
            var QualityItemTypeList = typeof(Code.EnumHelper.QualityItemType).ToItemList();
            var list = (from p in db.Table<Quality.Entity.tbQualityItem>()
                        where p.tbQualityItemGroup.Id == surveyGroup.Id
                        select p).ToList();
            foreach (var a in list.Where(d => txtId.Contains(d.Id.ToString()) == false))
            {
                a.IsDeleted = true;
            }

            for (var i = 0; i < txtId.Count(); i++)
            {
                if (string.IsNullOrEmpty(txtName[i]))
                {
                    //输入内容为空,判断是否存在Id
                    if (string.IsNullOrEmpty(txtId[i]) == false)
                    {
                        //如果是有id的，那就是数据库中记录的，应该做删除
                        var tb = list.Where(d => d.Id == txtId[i].ConvertToInt()).FirstOrDefault();
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价内容");
                        tb.IsDeleted = true;
                        surveyItems.Add(tb);
                    }
                }
                else
                {
                    //输入内容不为空，判断是否存在id并执行对应的操作
                    if (string.IsNullOrEmpty(txtId[i]) == false)
                    {
                        //如果有id的，执行更新操作
                        var tb = list.Where(d => d.Id == txtId[i].ConvertToInt()).FirstOrDefault();
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价内容");
                        tb.No = txtNo[i].ConvertToInt();
                        tb.QualityItemName = txtName[i];
                        tb.tbQualityItemGroup = surveyGroup;
                        tb.IsVertical = txtIsVertical[i] == "1" ? true : false;
                        tb.QualityItemType = EnumExtend.GetEnumName<Code.EnumHelper.QualityItemType>(QualityItemTypeList.Where(d => d.Value == txtQualityItemTypeId[i]).Select(d => d.Text).FirstOrDefault());
                        surveyItems.Add(tb);
                    }
                    else
                    {
                        //没有id的，执行插入操作
                        var tb = new Quality.Entity.tbQualityItem();
                        tb.No = txtNo[i].ConvertToInt();
                        tb.QualityItemName = txtName[i];
                        tb.tbQualityItemGroup = surveyGroup;
                        tb.IsVertical = txtIsVertical[i] == "1" ? true : false;
                        tb.QualityItemType = EnumExtend.GetEnumName<Code.EnumHelper.QualityItemType>(QualityItemTypeList.Where(d => d.Value == txtQualityItemTypeId[i]).Select(d => d.Text).FirstOrDefault());
                        surveyItems.Add(tb);
                        db.Set<Quality.Entity.tbQualityItem>().Add(tb);
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价内容");
                    }
                }
            }

            return true;

        }
    }
}