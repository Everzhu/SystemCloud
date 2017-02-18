using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveClassController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveClass.List();
                vm.ElectiveName = db.Table<Entity.tbElective>().FirstOrDefault(d => d.Id == vm.ElectiveId).ElectiveName;
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ElectiveClass.List vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var classList = new List<int>();
                if (string.IsNullOrEmpty(vm.ClassIds) == false)
                {
                    classList = vm.ClassIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                    classList.RemoveAll(d => d == 0);
                }

                var electiveClassList = (from p in db.Table<Entity.tbElectiveClass>()
                                            .Include(d => d.tbClass)
                                         where p.tbElective.Id == vm.ElectiveId
                                         select p).ToList();
                foreach (var a in electiveClassList.Where(d => classList.Contains(d.tbClass.Id) == false))
                {
                    a.IsDeleted = true;
                }

                foreach (var a in classList.Where(d => electiveClassList.Select(q => q.tbClass.Id).Contains(d) == false))
                {
                    var tb = new Entity.tbElectiveClass();
                    tb.tbElective = db.Set<Entity.tbElective>().Find(vm.ElectiveId);
                    tb.tbClass = db.Set<Basis.Entity.tbClass>().Find(a);
                    db.Set<Entity.tbElectiveClass>().Add(tb);
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("设置了选课的参选班级!");
                }

                if (Request["Step"] != null)
                {
                    return Code.MvcHelper.Post(null, Url.Action("List", "ElectiveSubject", new { ElectiveId = vm.ElectiveId }));
                }
                else
                {
                    return Code.MvcHelper.Post(null, Url.Action("List", "Elective"));
                }                
            }
        }

        public ActionResult GetElectiveClassTree(int electiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var gradeList = Areas.Basis.Controllers.GradeController.SelectList();
                var electiveClassList = SelectList(electiveId);
                var result = new List<Code.TreeHelper>();
                foreach (var grade in gradeList)
                {
                    var node = new Code.TreeHelper();
                    node.name = grade.Text;
                    node.Id = 0;
                    node.open = true;
                    node.isChecked = false;
                    node.children = GetElectiveClassTreeSub(grade.Value.ConvertToInt(), electiveClassList);
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

        private List<Code.TreeHelper> GetElectiveClassTreeSub(int gradeId, List<Dto.ElectiveClass.Info> electiveClassList)
        {
            var classList = Areas.Basis.Controllers.ClassController.SelectList(0, gradeId);
            var result = new List<Code.TreeHelper>();
            foreach (var cla in classList)
            {
                var node = new Code.TreeHelper();
                node.name = cla.Text;
                node.Id = cla.Value.ConvertToInt();
                node.open = true;
                node.isChecked = electiveClassList.Where(d => d.ClassId == node.Id).FirstOrDefault() == null ? false : true;
                node.children = null;
                result.Add(node);
            }

            return result;
        }

        [NonAction]
        public static List<Dto.ElectiveClass.Info> SelectList(int electiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveClass>()
                          where p.tbElective.Id == electiveId
                          orderby p.tbClass.tbGrade.No, p.tbClass.No
                          select new Dto.ElectiveClass.Info
                          {
                              ClassId = p.tbClass.Id,
                              ClassName = p.tbClass.ClassName + "[" + p.tbClass.tbClassType.ClassTypeName + "]",
                              GradeId = p.tbClass.tbGrade.Id
                          }).ToList();
                return tb;
            }
        }
    }
}