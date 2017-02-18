using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace XkSystem.Areas.Dict.Controllers
{
    public class BloodController : ApiController
    {
        [HttpGet]
        public List<Entity.tbDictBlood> List(string searchText = "")
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Table<Dict.Entity.tbDictBlood>();
                if (string.IsNullOrEmpty(searchText) == false)
                {
                    tb = tb.Where(d => d.BloodName.Contains(searchText));
                }

                return tb.ToList();
            }
        }

        [HttpPost]
        public bool Delete([FromBody]string idList)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var ids = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt());
                var tb = (from p in db.Table<Dict.Entity.tbDictBlood>()
                          where ids.Contains(p.Id)
                          select p).ToList();
                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了血型");
                }

                return true;
            }
        }

        [HttpGet]
        public Models.Blood.Edit Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Blood.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Dict.Entity.tbDictBlood>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.tbDictBlood = tb;
                    }
                }

                return vm;
            }
        }

        [HttpPost]
        public bool Edit([FromBody]Models.Blood.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Dict.Entity.tbDictBlood>().Where(d => d.BloodName == vm.tbDictBlood.BloodName && d.Id != vm.tbDictBlood.Id).Any())
                    {
                        error.AddError("该血型已存在!");
                    }
                    else
                    {
                        if (vm.tbDictBlood.Id == 0)
                        {
                            var tb = new Dict.Entity.tbDictBlood();
                            tb.No = vm.tbDictBlood.No == null ? db.Table<Dict.Entity.tbDictBlood>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.tbDictBlood.No;
                            tb.BloodName = vm.tbDictBlood.BloodName;
                            db.Set<Dict.Entity.tbDictBlood>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了血型");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Dict.Entity.tbDictBlood>()
                                      where p.Id == vm.tbDictBlood.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.tbDictBlood.No == null ? db.Table<Dict.Entity.tbDictBlood>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.tbDictBlood.No;
                                tb.BloodName = vm.tbDictBlood.BloodName;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了血型");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                return true;
            }
        }

        [HttpGet]
        public List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictBlood>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.BloodName,
                              Value = p.Id.ToString()
                          }).ToList();

                return tb;
            }
        }
    }
}
