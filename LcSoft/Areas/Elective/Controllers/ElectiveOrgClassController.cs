using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Controllers
{
    public class ElectiveOrgClassController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ElectiveOrgClass.List();

                vm.IsPermitClass = db.Set<Entity.tbElectiveOrg>().Find(vm.ElectiveOrgId).IsPermitClass;
                vm.ElectiveOrgClassList = (from p in db.Table<Entity.tbElectiveClass>()
                                           join q in db.Table<Entity.tbElectiveOrgClass>().Where(d => d.tbElectiveOrg.Id == vm.ElectiveOrgId)
                                           on p.tbClass.Id equals q.tbClass.Id into temp
                                           from g in temp.DefaultIfEmpty()
                                           where p.tbElective.Id == vm.ElectiveId
                                           orderby p.tbClass.tbGrade.No, p.tbClass.tbGrade.GradeName, p.tbClass.No, p.tbClass.ClassName
                                           select new Dto.ElectiveOrgClass.List
                                           {
                                               Id = p.tbClass.Id,
                                               GradeName = p.tbClass.tbGrade.GradeName,
                                               ClassName = p.tbClass.ClassName,
                                               ClassTypeName = p.tbClass.tbClassType.ClassTypeName,
                                               IsChecked = vm.IsPermitClass == false ? true : g.tbClass != null,
                                               MaxLimit = g.tbClass != null ? g.MaxLimit : 999
                                           }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ElectiveOrgClass.List vm)
        {
            var error = new List<string>();
            using (var db = new XkSystem.Models.DbContext())
            {
                var ids = new List<int>();
                if (Request["CboxId"] != null)
                {
                    ids = Request["CboxId"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                }

                var tb = (from p in db.Table<Entity.tbElectiveOrg>()
                          where p.Id == vm.ElectiveOrgId
                          select p).FirstOrDefault();
                if (tb != null)
                {
                    tb.IsPermitClass = vm.IsPermitClass;
                 
                    var electiveOrgClass = (from p in db.Table<Entity.tbElectiveOrgClass>()
                                             .Include(d => d.tbClass)
                                            where p.tbElectiveOrg.Id == vm.ElectiveOrgId
                                            select p).ToList();
                    foreach (var a in electiveOrgClass.Where(d => ids.Contains(d.tbClass.Id) == false))
                    {
                        a.IsDeleted = true;
                    }


                    //获取当前默认学年Id

                    var yearId = Basis.Controllers.YearController.GetDefaultYearId(db);

                    //按班级获取已选课学员人数
                    var studentCount = (from p in db.Table<Entity.tbElectiveData>()
                              join c in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals c.tbStudent.Id
                              where p.tbElectiveOrg.Id == vm.ElectiveOrgId
                                  && c.tbClass.tbYear.Id==yearId
                              group c by c.tbClass.Id into result
                              select new
                              {
                                  ClassId = result.Key,
                                  StudentCount = result.Count()
                              }).ToList();
                    foreach (var a in electiveOrgClass.Where(d => ids.Contains(d.tbClass.Id)))
                    {
                        var maxLimit = Request["TxtMaxLimit" + a.tbClass.Id].ConvertToInt();

                        //修改限制班级最大人数时，判断已订课人数
                        var existsNum = studentCount.Where(p => p.ClassId == a.tbClass.Id).Select(p => p.StudentCount).FirstOrDefault();
                        if (maxLimit < existsNum)
                        {
                            error.Add($"{a.tbClass.ClassName}的最大人数:{maxLimit}不能小于当前班级已选课学员人数:{existsNum}！");
                            return Code.MvcHelper.Post(error);
                        }
                        else
                        {
                            a.MaxLimit = maxLimit;
                            a.RemainCount = maxLimit - existsNum;
                        }
                    }

                    //foreach (var a in electiveOrgClass.Where(d => ids.Contains(d.tbClass.Id)))
                    //{
                    //    var maxLimit = Request["TxtMaxLimit" + a.tbClass.Id].ConvertToInt();

                    //    //修改限制班级最大人数时，判断已订课人数
                    //    var existsNum = a.MaxLimit - a.RemainCount;
                    //    if (maxLimit < existsNum)
                    //    {
                    //        error.Add($"{a.tbClass.ClassName}的最大人数:{maxLimit}不能小于当前班级已选课学员人数:{existsNum}！");
                    //        return Code.MvcHelper.Post(error);
                    //    }
                    //    else
                    //    {
                    //        a.MaxLimit = maxLimit;
                    //        a.RemainCount = maxLimit - existsNum;
                    //    }
                    //}

                    var org = db.Set<Entity.tbElectiveOrg>().Find(vm.ElectiveOrgId);
                    var electiveClass = (from p in db.Table<Entity.tbElectiveClass>()
                                            .Include(d => d.tbClass)
                                         where p.tbElective.Id == vm.ElectiveId
                                         select p).ToList();

                    foreach (var a in ids.Where(d => electiveOrgClass.Select(o => o.tbClass.Id).Contains(d) == false))
                    {
                        var limit = new Entity.tbElectiveOrgClass();
                        limit.tbElectiveOrg = org;
                        limit.tbClass = electiveClass.Where(d => d.tbClass.Id == a).Select(d => d.tbClass).FirstOrDefault();
                        limit.MaxLimit = Request["TxtMaxLimit" + a].ConvertToInt();
                        limit.RemainCount = limit.MaxLimit;
                        db.Set<Entity.tbElectiveOrgClass>().Add(limit);
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了选课开班");
                    }
                }

                return Code.MvcHelper.Post();
            }
        }
    }
}