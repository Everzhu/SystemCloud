using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveTypeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveType.List();
                var tb = from p in db.Table<Entity.tbElectiveType>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ElectiveTypeName.Contains(vm.SearchText));
                }

                vm.ElectiveTypeList = (from p in tb
                                       orderby p.No
                                       select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ElectiveType.List vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var txtId = Request["txtId"].Split(',');
                var txtNo = Request["txtNo"].Split(',');
                var txtName = Request["txtName"].Split(',');
                var txtRemark = Request["txtRemark"].Split(',');

                var list = (from p in db.Table<Entity.tbElectiveType>()
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
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加选课模式");
                            tb.IsDeleted = true;
                        }
                    }
                    else
                    {
                        //输入内容不为空，判断是否存在id并执行对应的操作
                        if (string.IsNullOrEmpty(txtId[i]) == false)
                        {
                            //如果有id的，执行更新操作
                            var tb = list.Where(d => d.Id == txtId[i].ConvertToInt()).FirstOrDefault();
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改选课模式");
                            tb.No = txtNo[i].ConvertToInt();
                            tb.ElectiveTypeName = txtName[i];
                            tb.Remark = txtRemark[i];
                        }
                        else
                        {
                            //没有id的，执行插入操作

                            var tb = new Entity.tbElectiveType();
                            tb.No = txtNo[i].ConvertToInt();
                            tb.ElectiveTypeName = txtName[i];
                            tb.Remark = txtRemark[i];
                            db.Set<Entity.tbElectiveType>().Add(tb);
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加选课模式");
                        }
                    }
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveType>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.ElectiveTypeName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}