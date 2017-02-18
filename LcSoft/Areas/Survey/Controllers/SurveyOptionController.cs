using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Survey.Controllers
{
    public class SurveyOptionController : Controller
    {
        public ActionResult List(int SurveyItemId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyOption.List();

                var tb = from p in db.Table<Entity.tbSurveyOption>()
                         where p.tbSurveyItem.Id == vm.SurveyItemId
                         select p;
                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.OptionName.Contains(vm.SearchText));
                }

                vm.OptionList = (from p in tb
                                 orderby p.No
                                 select new Dto.SurveyOption.List
                                 {
                                     Id = p.Id,
                                     No = p.No,
                                     OptionName = p.OptionName,
                                     OptionValue = p.OptionValue,
                                     SurveyItemName = p.tbSurveyItem.SurveyItemName
                                 }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.SurveyOption.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbSurveyOption>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价选项");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Delete(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbSurveyOption>()
                          where p.Id == id
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价选项");
                }

                return Json("ok", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyOption.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Entity.tbSurveyOption>()
                              where p.Id == id
                              select new Dto.SurveyOption.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  OptionName = p.OptionName,
                                  OptionValue = p.OptionValue
                                  //SurveyItemId = p.tbSurveyItem.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.OptionEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.SurveyOption.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.OptionEdit.Id == 0)
                    {
                        var tb = new Entity.tbSurveyOption();
                        tb.No = vm.OptionEdit.No == null ? db.Table<Entity.tbSurveyOption>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.OptionEdit.No;
                        tb.tbSurveyItem = db.Set<Entity.tbSurveyItem>().Find(vm.SurveyItemId);
                        tb.OptionName = vm.OptionEdit.OptionName;
                        tb.OptionValue = vm.OptionEdit.OptionValue;
                        db.Set<Entity.tbSurveyOption>().Add(tb);

                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价选项");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Entity.tbSurveyOption>()
                                  where p.Id == vm.OptionEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.OptionEdit.No == null ? db.Table<Entity.tbSurveyOption>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.OptionEdit.No;
                            tb.tbSurveyItem = db.Set<Entity.tbSurveyItem>().Find(vm.SurveyItemId);
                            tb.OptionName = vm.OptionEdit.OptionName;
                            tb.OptionValue = vm.OptionEdit.OptionValue;

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价选项");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        /// <summary>
        /// 保存评价选项页面数据
        /// </summary>
        /// <param name="db">数据库连接</param>
        /// <param name="surveyItems">评价内容数组</param>
        /// <param name="request">请求参数</param>
        /// <returns></returns>
        [NonAction]
        public bool SaveSurveyOption(XkSystem.Models.DbContext db, List<Entity.tbSurveyItem> surveyItems, HttpRequestBase request)
        {
            var txtOptionId = request["txtOptionId"].Split(',');
            var txtOptionName = request["txtOptionName"].Split(',');
            var txtOptionValue = request["txtOptionValue"].Split(',');

            //没数据直接跳过
            if (txtOptionId.Length <= 0 && txtOptionName.Length <= 0)
                return true;

            for (var i = 0; i < surveyItems.Count(); i++)
            {
                int id = surveyItems[i].Id;

                var list = (from p in db.Table<Entity.tbSurveyOption>()
                            where p.tbSurveyItem.Id == id
                            select p).ToList();

                var optionIds = txtOptionId[i].Split(new char[] { ';' });

                //默认Item下全部Delete状态，方便Id传的数量和Name不相等情况处理
                foreach (var a in list)
                {
                    a.IsDeleted = true;
                }

                //Item控制器可能删除了该数据，所以要判断状态
                if (surveyItems[i].IsDeleted == false)
                {
                    var optionNames = txtOptionName[i].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    for (int j = 0; j < optionNames.Length; j++)
                    {
                        //如果有id的，执行更新操作,可能填了X个Name，但是Id的总数是小于X个的
                        if (optionIds.Length >= (j + 1) && string.IsNullOrEmpty(optionIds[j]) == false)
                        {
                            var tb = list.Where(d => d.Id == optionIds[j].ConvertToInt()).FirstOrDefault();
                            tb.tbSurveyItem = surveyItems[i];
                            tb.OptionName = optionNames[j];
                        }
                        else
                        {
                            //没有id的，执行插入操作
                            var tb = new Entity.tbSurveyOption();
                            tb.tbSurveyItem = surveyItems[i];
                            tb.OptionName = optionNames[j];
                            tb.OptionValue = txtOptionValue[j].ConvertToDecimal();
                            db.Set<Entity.tbSurveyOption>().Add(tb);
                        }
                    }
                }
            }

            return true;
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int itemId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbSurveyOption>()
                          where p.tbSurveyItem.IsDeleted == false
                            && p.tbSurveyItem.Id == itemId
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.OptionName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<Dto.SurveyOption.Info> SelectInfoListBySurveyGroup(int surveyGroupId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbSurveyOption>()
                          where p.tbSurveyItem.tbSurveyGroup.Id == surveyGroupId
                          orderby p.tbSurveyItem.No, p.No
                          select new Areas.Survey.Dto.SurveyOption.Info
                          {
                              Id = p.Id,
                              OptionName = p.OptionName,
                              OptionValue = p.OptionValue,
                              SurveyItemId = p.tbSurveyItem.Id
                          }).ToList();
                return tb;
            }
        }
    }
}