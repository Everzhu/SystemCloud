using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class RoomTypeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.RoomType.List();
                var tb = from p in db.Table<Basis.Entity.tbRoomType>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.RoomTypeName.Contains(vm.SearchText));
                }

                vm.RoomTypeList = (from p in tb
                                   orderby p.No, p.RoomTypeName
                                   select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.RoomType.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbRoomType>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了教学楼");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.RoomType.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbRoomType>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.RoomTypeEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.RoomType.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Basis.Entity.tbRoomType>().Where(d=>d.RoomTypeName == vm.RoomTypeEdit.RoomTypeName && d.Id != vm.RoomTypeEdit.Id).Any())
                    {
                        error.AddError("该教学楼已存在!");
                    }
                    else
                    {
                        if (vm.RoomTypeEdit.Id == 0)
                        {
                            var tb = new Basis.Entity.tbRoomType();
                            tb.No = vm.RoomTypeEdit.No == null ? db.Table<Basis.Entity.tbRoomType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.RoomTypeEdit.No;
                            tb.RoomTypeName = vm.RoomTypeEdit.RoomTypeName;
                            db.Set<Basis.Entity.tbRoomType>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了教学楼");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Basis.Entity.tbRoomType>()
                                      where p.Id == vm.RoomTypeEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.RoomTypeEdit.No == null ? db.Table<Basis.Entity.tbRoomType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.RoomTypeEdit.No;
                                tb.RoomTypeName = vm.RoomTypeEdit.RoomTypeName;

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了教学楼");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var list = (from p in db.Table<Basis.Entity.tbRoomType>()
                            orderby p.No, p.RoomTypeName
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.RoomTypeName,
                                Value = p.Id.ToString()
                            }).ToList();
                return list;
            }
        }
    }
}