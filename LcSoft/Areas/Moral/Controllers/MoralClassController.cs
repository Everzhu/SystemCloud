using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Controllers
{
    public class MoralClassController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.MoralClass.List();
                vm.MoralName = db.Table<Moral.Entity.tbMoral>().FirstOrDefault(d => d.Id == vm.MoralId).MoralName;
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.MoralClass.List vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var classList = new List<int>();
                if (string.IsNullOrEmpty(vm.ClassIds) == false)
                {
                    classList = vm.ClassIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                    classList.RemoveAll(d => d == 0);
                }

                var MoralClassList = (from p in db.Table<Moral.Entity.tbMoralClass>()
                                            .Include(d => d.tbClass)
                                         where p.tbMoral.Id == vm.MoralId
                                         select p).ToList();
                foreach (var a in MoralClassList.Where(d => classList.Contains(d.tbClass.Id) == false))
                {
                    a.IsDeleted = true;
                }

                foreach (var a in classList.Where(d => MoralClassList.Select(q => q.tbClass.Id).Contains(d) == false))
                {
                    var tb = new Moral.Entity.tbMoralClass();
                    tb.tbMoral = db.Set<Moral.Entity.tbMoral>().Find(vm.MoralId);
                    tb.tbClass = db.Set<Basis.Entity.tbClass>().Find(a);
                    db.Set<Moral.Entity.tbMoralClass>().Add(tb);
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("设置了德育的参评班级!");
                }

                if (Request["Step"] != null)
                {
                    return Code.MvcHelper.Post(null, Url.Action("List", "MoralGroup", new { MoralId = vm.MoralId }));
                }
                else
                {
                    return Code.MvcHelper.Post(null, Url.Action("List", "Moral"));
                }
            }
        }

        public ActionResult GetMoralClassTree(int moralId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var gradeList = Areas.Basis.Controllers.GradeController.SelectList();
                var MoralClassList = Areas.Moral.Controllers.MoralClassController.SelectList(moralId);

                //学段Id
                var sectionId = (from p in db.Table<Moral.Entity.tbMoral>() where p.Id == moralId select p.tbYear.Id).FirstOrDefault();

                var result = new List<Code.TreeHelper>();
                foreach (var grade in gradeList)
                {
                    var node = new Code.TreeHelper();
                    node.name = grade.Text;
                    node.Id = 0;
                    node.open = true;
                    node.isChecked = false;
                    node.children = GetMoralClassTreeSub(sectionId,grade.Value.ConvertToInt(), MoralClassList);
                    result.Add(node);
                }

                var treeList = new List<Code.TreeHelper>();
                var root = new Code.TreeHelper();
                root.name = "全部班级";
                root.Id = 0;
                root.open = true;
                root.isChecked = false;
                root.children = result;
                treeList.Add(root);

                return Json(treeList, JsonRequestBehavior.AllowGet);
            }
        }

        private List<Code.TreeHelper> GetMoralClassTreeSub(int sectionId,int gradeId, List<Dto.MoralClass.Info> MoralClassList)
        {

            var classList = Areas.Basis.Controllers.ClassController.SelectListByYearType(Code.EnumHelper.YearType.Section,sectionId, gradeId);
            var result = new List<Code.TreeHelper>();
            foreach (var cla in classList)
            {
                var node = new Code.TreeHelper();
                node.name = cla.Text;
                node.Id = cla.Value.ConvertToInt();
                node.open = true;
                node.isChecked = MoralClassList.Where(d => d.ClassId == node.Id).FirstOrDefault() == null ? false : true;
                node.children = null;
                result.Add(node);
            }

            return result;
        }

        [NonAction]
        public static List<Dto.MoralClass.Info> SelectList(int MoralId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Moral.Entity.tbMoralClass>()
                          where p.tbMoral.Id == MoralId
                          orderby p.tbClass.tbGrade.No, p.tbClass.No
                          select new Dto.MoralClass.Info
                          {
                              ClassId = p.tbClass.Id,
                              ClassName = p.tbClass.ClassName + "[" + p.tbClass.tbClassType.ClassTypeName + "]",
                              GradeId = p.tbClass.tbGrade.Id
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<SelectListItem> SelectItemList(int MoralId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Moral.Entity.tbMoralClass>()
                          where p.tbMoral.Id == MoralId
                          orderby p.tbClass.tbGrade.No, p.tbClass.No
                          select new SelectListItem
                          {
                              Value = p.tbClass.Id.ToString(),
                              Text = p.tbClass.ClassName
                          }).ToList();
                return tb;
            }
        }

    }
}