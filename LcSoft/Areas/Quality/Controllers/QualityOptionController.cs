using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Quality.Controllers
{
    public class QualityOptionController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int QualityItemId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityOption.List();
                var tb = from p in db.Table<Quality.Entity.tbQualityOption>()
                         where p.tbQualityItem.Id == QualityItemId
                         select p;
                vm.OptionList = (from p in tb
                                 orderby p.No
                                 select p).ToList();

                return PartialView(vm);
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
        public bool SaveQualityOption(XkSystem.Models.DbContext db, Quality.Entity.tbQualityItem qualityItem, HttpRequestBase request)
        {
            var txtId = request["txtId"].Split(',');
            var txtNo = request["txtNo"].Split(',');
            var txtName = request["txtName"].Split(',');
            var txtValue = request["txtValue"].Split(',');

                var list = (from p in db.Table<Quality.Entity.tbQualityOption>()
                            where p.tbQualityItem.Id == qualityItem.Id
                            select p).ToList();

            foreach (var a in list.Where(d => txtId.Contains(d.Id.ToString()) == false))
            {
                a.IsDeleted = true;
            }

            for (var i = 0; i < txtId.Count(); i++)
            {
                //输入内容不为空，判断是否存在id并执行对应的操作
                if (string.IsNullOrEmpty(txtId[i]) == false)
                {
                    //如果有id的，执行更新操作
                    var tb = list.Where(d => d.Id == txtId[i].ConvertToInt()).FirstOrDefault();
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改评价选项");
                    tb.No = txtNo[i].ConvertToInt();
                    tb.OptionName = txtName[i];
                    tb.tbQualityItem = qualityItem;
                    tb.OptionValue = txtValue[i].ConvertToDecimal();
                }
                else
                {
                    //没有id的，执行插入操作
                    var tb = new Quality.Entity.tbQualityOption();
                    tb.No = txtNo[i].ConvertToInt();
                    tb.OptionName = txtName[i];
                    tb.tbQualityItem = qualityItem;
                    tb.OptionValue = txtValue[i].ConvertToDecimal();
                    db.Set<Quality.Entity.tbQualityOption>().Add(tb);
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价内容");
                }
            }

            return true;
        }
    }
}