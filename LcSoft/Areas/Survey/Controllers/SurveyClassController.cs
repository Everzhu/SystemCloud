using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Survey.Controllers
{
    public class SurveyClassController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SurveyClass.List();
                vm.SurveyName = db.Table<Entity.tbSurvey>().FirstOrDefault(d => d.Id == vm.SurveyId).SurveyName;
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.SurveyClass.List vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var classList = new List<int>();
                if (string.IsNullOrEmpty(vm.ClassIds) == false)
                {
                    classList = vm.ClassIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                    classList.RemoveAll(d => d == 0);
                }

                var surveyClassList = (from p in db.Table<Entity.tbSurveyClass>()
                                            .Include(d => d.tbClass)
                                       where p.tbSurvey.Id == vm.SurveyId
                                       select p).ToList();
                foreach (var a in surveyClassList.Where(d => classList.Contains(d.tbClass.Id) == false))
                {
                    a.IsDeleted = true;
                }

                var list = new List<Entity.tbSurveyClass>();
                foreach (var a in classList.Where(d => surveyClassList.Select(q => q.tbClass.Id).Contains(d) == false))
                {
                    var tb = new Entity.tbSurveyClass();
                    tb.tbSurvey = db.Set<Entity.tbSurvey>().Find(vm.SurveyId);
                    tb.tbClass = db.Set<Basis.Entity.tbClass>().Find(a);
                    list.Add(tb);
                }

                db.Set<Entity.tbSurveyClass>().AddRange(list);

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加参评班级");
                }

                if (Request["Step"] != null)
                {
                    return Code.MvcHelper.Post(null, Url.Action("List", "SurveyGroup", new { surveyId = vm.SurveyId }));
                }
                else
                {
                    return Code.MvcHelper.Post(null, Url.Action("List", "Survey", new { searchText = vm.SearchText }));
                }
            }
        }

        public ActionResult GetSurveyClassTree(int surveyId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var survey = (from p in db.Table<Entity.tbSurvey>()
                              where p.Id == surveyId
                              select new
                              {
                                  YearId = p.tbYear.Id
                              }).FirstOrDefault();
                var gradeList = Areas.Basis.Controllers.GradeController.SelectList();
                var surveyClassList = Areas.Survey.Controllers.SurveyClassController.SelectList(surveyId);
                var result = new List<Code.TreeHelper>();
                foreach (var grade in gradeList)
                {
                    var node = new Code.TreeHelper();
                    node.name = grade.Text;
                    node.Id = grade.Value.ConvertToInt();
                    node.open = true;
                    node.isChecked = false;
                    node.children = GetSurveyClassTreeSub(survey.YearId, grade.Value.ConvertToInt(), surveyClassList);
                    result.Add(node);
                }

                var treeList = new List<Code.TreeHelper>();
                var root = new Code.TreeHelper();
                root.name = "全部";
                root.Id = 0;
                root.open = true;
                root.isChecked = false;
                root.children = result;
                treeList.Add(root);

                return Json(treeList, JsonRequestBehavior.AllowGet);
            }
        }

        private List<Code.TreeHelper> GetSurveyClassTreeSub(int yearId, int gradeId, List<System.Web.Mvc.SelectListItem> surveyClassList)
        {
            var classList = Areas.Basis.Controllers.ClassController.SelectListByYearType(Code.EnumHelper.YearType.Section, yearId, gradeId);
            var result = new List<Code.TreeHelper>();
            foreach (var cla in classList)
            {
                var node = new Code.TreeHelper();
                node.name = cla.Text;
                node.Id = cla.Value.ConvertToInt();
                node.open = true;
                node.isChecked = surveyClassList.Where(d => d.Value == cla.Value).FirstOrDefault() == null ? false : true;
                node.children = null;
                result.Add(node);
            }

            return result;
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int surveyId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbSurveyClass>()
                          where p.tbSurvey.Id == surveyId
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.tbClass.ClassName,
                              Value = p.tbClass.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<Areas.Basis.Dto.Class.Info> SelectClassInfoListByTeacher(int surveyId, int teacherId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var surveyClassIds = (from p in db.Table<Entity.tbSurveyClass>()
                                      where p.tbClass.IsDeleted == false
                                        && p.tbSurvey.Id == surveyId
                                      select p.tbClass.Id).ToList();

                var tb = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                          where p.tbClass.IsDeleted == false 
                            && p.tbTeacher.Id == teacherId 
                            && surveyClassIds.Contains(p.tbClass.Id)
                          select new Areas.Basis.Dto.Class.Info
                          {
                              Id = p.tbClass.Id,
                              ClassName = p.tbClass.ClassName
                          }).ToList();
                return tb;
            }
        }

    }
}