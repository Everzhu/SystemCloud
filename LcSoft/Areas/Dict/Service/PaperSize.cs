using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Service
{
    public class PaperSize
    {
        public static Models.PaperSize.List List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PaperSize.List();
                var tb = from p in db.Table<Dict.Entity.tbDictPaperSize>()
                         select p;
                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.PaperSizeName.Contains(vm.SearchText));
                }

                vm.PaperSizeList = (from p in tb
                                    orderby p.No
                                    select p).ToList();
                return vm;
            }
        }

        public static string Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictPaperSize>()
                          where ids.Contains(p.Id)
                          select p).ToList();
                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                db.SaveChanges();

                return string.Empty;
            }
        }

        public static Models.PaperSize.Edit Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PaperSize.Edit();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Dict.Entity.tbDictPaperSize>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.PaperSizeEdit = tb;
                    }
                }

                return vm;
            }
        }

        public static string Insert(Models.PaperSize.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = new Dict.Entity.tbDictPaperSize();
                tb.No = vm.PaperSizeEdit.No == null ? db.Table<Dict.Entity.tbDictPaperSize>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.PaperSizeEdit.No;
                tb.PaperSizeName = vm.PaperSizeEdit.PaperSizeName;
                tb.Height = vm.PaperSizeEdit.Height;
                tb.Width = vm.PaperSizeEdit.Width;
                db.Set<Dict.Entity.tbDictPaperSize>().Add(tb);
                db.SaveChanges();

                return string.Empty;
            }
        }

        public static string Update(Models.PaperSize.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictPaperSize>()
                          where p.Id == vm.PaperSizeEdit.Id
                          select p).FirstOrDefault();
                if (tb != null)
                {
                    tb.No = vm.PaperSizeEdit.No == null ? db.Table<Dict.Entity.tbDictPaperSize>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.PaperSizeEdit.No;
                    tb.PaperSizeName = vm.PaperSizeEdit.PaperSizeName;
                    tb.Height = vm.PaperSizeEdit.Height;
                    tb.Width = vm.PaperSizeEdit.Width;
                    db.SaveChanges();

                    return string.Empty;
                }
                else
                {
                    return Resources.LocalizedText.MsgNotFound;
                }
            }
        }

        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictPaperSize>()
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.PaperSizeName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}