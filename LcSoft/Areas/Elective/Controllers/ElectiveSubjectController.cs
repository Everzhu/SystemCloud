using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveSubjectController : Controller
    {
        // GET: Elective/ElectiveSubject
        public ActionResult List()
        {
            var vm = new Models.ElectiveSubject.List();
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbSubject>()
                          join e in db.Table< Entity.tbElectiveSubject>()  on new { p.Id, ElectiveId = vm.ElectiveId } equals new { e.tbSubject.Id, ElectiveId = e.tbElective.Id } into  subject
                          from s in subject.DefaultIfEmpty()
                          select new {p,s});
                if (!string.IsNullOrWhiteSpace(vm.SearchText))
                {
                    tb = tb.Where(p => p.s.tbSubject.SubjectName.Contains(vm.SearchText) || p.s.tbSubject.SubjectNameEn.Contains(vm.SearchText));
                }
                vm.ElectiveSubjectList=(from p in tb
                    select new Dto.ElectiveSubject.List()
                {
                    Id=p.p.Id,
                    SubjectId=p.p.Id,
                    SubjectName=p.p.SubjectName,
                    IsOpen=p.s!=null
                }).ToList();

                vm.ElectiveName = db.Table<Entity.tbElective>().FirstOrDefault(d => d.Id == vm.ElectiveId).ElectiveName;
                

            }
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ElectiveSubject.List vm)
        {
            return Code.MvcHelper.Post(returnUrl:Url.Action("List",new { SearchText=vm.SearchText,ElectiveId=vm.ElectiveId}));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetAll(int electiveId, string type)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tbElectiveSubjects = (from p in db.Table<Entity.tbElectiveSubject>().Include(p=>p.tbSubject) where p.tbElective.Id == electiveId select p).ToList();

                if (type.Equals("close"))
                {
                    if (tbElectiveSubjects != null && tbElectiveSubjects.Count > 0)
                    {
                        foreach (var tb in tbElectiveSubjects)
                        {
                            tb.IsDeleted = !tb.IsDeleted;
                            tb.UpdateTime = DateTime.Now;
                        }
                    }
                }
                else
                {
                    var tbSubjects = (from p in db.Table<Course.Entity.tbSubject>() select p).ToList();
                    tbSubjects.RemoveAll(p => tbElectiveSubjects.Exists(c => c.tbSubject.Id == p.Id));
                    foreach (var tb in tbSubjects)
                    {

                    }

                    if (tbSubjects != null && tbSubjects.Count > 0)
                    {
                        var tbElective = db.Set<Entity.tbElective>().Find(electiveId);
                        db.Set<Entity.tbElectiveSubject>().AddRange(tbSubjects.Select(p => new Entity.tbElectiveSubject()
                        {
                            tbElective=tbElective,
                            tbSubject = p
                        }));
                    }
                }
                db.SaveChanges();  
            }
            return Code.MvcHelper.Post(message:"操作成功!");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetStatus(int subjectId, int electiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbElectiveSubject>() where p.tbElective.Id == electiveId && p.tbSubject.Id == subjectId select p).FirstOrDefault();
                if (tb != null)
                {
                    tb.IsDeleted = !tb.IsDeleted;
                    tb.UpdateTime = DateTime.Now;
                }
                else
                {
                    db.Set<Entity.tbElectiveSubject>().Add(new Entity.tbElectiveSubject()
                    {
                        tbElective=db.Set<Entity.tbElective>().Find(electiveId),
                        tbSubject=db.Set<Course.Entity.tbSubject>().Find(subjectId)
                    });
                }
                db.SaveChanges();
                return Code.MvcHelper.Post();
            }
        }


        [NonAction]
        public static List<SelectListItem> SelectSubjectList(int electiveId=0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {

                var tb = (from p in db.Table<Entity.tbElectiveSubject>() select p);
                if (electiveId > 0)
                {
                    tb = tb.Where(p => p.tbElective.Id == electiveId);
                }
                return (from p in tb select new SelectListItem()
                {
                    Text = p.tbSubject.SubjectName,
                    Value = p.tbSubject.Id.ToString()
                }).ToList();
            }
        }


        [HttpPost]
        public JsonResult GetSubjectList(int electiveId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb=(from p in db.Table<Entity.tbElectiveSubject>()
                        where p.tbElective.Id == electiveId
                        select new SelectListItem()
                        {
                            Text = p.tbSubject.SubjectName,
                            Value = p.tbSubject.Id.ToString()
                        }).ToList();
                return Json(tb);
            }
        }

    }
}