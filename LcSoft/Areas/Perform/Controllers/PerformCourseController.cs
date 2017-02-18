using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Perform.Controllers
{
    public class PerformCourseController : Controller
    {
        public ActionResult List()
        {
            var vm = new Models.PerformCourse.List();
            return PartialView(vm);
        }

        /// <summary>
        /// 修改或新增评价科目列表
        /// </summary>
        /// <param name="db">数据库上下文对象</param>
        /// <param name="ids">菜单ID集合</param>
        /// <param name="performGroup">评价分组数据</param>
        /// <param name="performId">评价Id</param>
        /// <returns></returns>
        [NonAction]
        public dynamic SaveCourseNode(XkSystem.Models.DbContext db, string ids, Perform.Entity.tbPerformGroup performGroup, int performId)
        {
            //查出整个评价Id的所有组的科目列表
            var tb = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                      .Include(d => d.tbPerformGroup)
                      .Include(d => d.tbCourse)
                      where p.tbPerformGroup.tbPerform.Id == performId && p.tbPerformGroup.IsDeleted == false
                      select p).ToList();

            //-1为虚构的根节点，要去掉
            var idArr = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(d => d.ConvertToInt() != -1).Select(d => d.ConvertToInt());

            //取交集，同一PerformId中的多个分组Subject不能重复，且判断GroupId不能是当前要修改的Id
            bool flag = tb.Where(d => d.tbPerformGroup.Id != performGroup.Id && d.tbPerformGroup.IsDeleted == false).Select(d => d.tbCourse.Id).Intersect(idArr).Count() > 0;

            if (flag)
                return new { Status = decimal.Zero, Message = "同一评价下不同分组科目不能重复！" };

            //修改当前分组的老数据为删除状态
            foreach (var a in tb.Where(d => d.tbPerformGroup.Id == performGroup.Id))
            {
                a.IsDeleted = true;
            }

            foreach (var id in idArr)
            {
                var temp = new Perform.Entity.tbPerformCourse();
                temp.tbPerformGroup = performGroup;
                temp.tbCourse = db.Set<Course.Entity.tbCourse>().Find(id);
                db.Set<Perform.Entity.tbPerformCourse>().Add(temp);
            }

            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价科目");
            return new { Status = decimal.One, Message = "操作成功！" };
        }

        /// <summary>
        /// 获取评价科目Tree菜单数据
        /// </summary>
        /// <param name="performGroupId">评价分组Id</param>
        /// <returns></returns>
        public ActionResult GetPerformCourseTree(int performGroupId, int performId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var year = (from p in db.Table<Perform.Entity.tbPerform>()
                            where p.Id == performId
                            select new Dto.Perform.Edit
                            {
                                Id = p.Id,
                                YearId = p.tbYear.Id,
                            }).ToList().FirstOrDefault();

                var tbCourseIds = (from p in db.Table<Course.Entity.tbOrg>()
                                   where p.tbYear.Id == year.YearId
                                   select p.tbCourse.Id).Distinct().ToList();

                var tbSubjectIds = (from p in db.Table<Course.Entity.tbCourse>()
                                    where tbCourseIds.Contains(p.Id)
                                    select p.tbSubject.Id).Distinct().ToList();

                var tbCourseList = (from p in db.Table<Course.Entity.tbCourse>()
                                    where tbCourseIds.Contains(p.Id)
                                    select p).Distinct().ToList();

                //查询所有科目
                var subjects = (from p in db.Table<Course.Entity.tbSubject>()
                                where tbSubjectIds.Contains(p.Id)
                                select p).ToList();

                //查询该用户拥有的科目列表
                var tbPerformCourseListOur = (from p in db.Table<Perform.Entity.tbPerformCourse>()
                                                .Include(d => d.tbCourse)
                                              where p.tbPerformGroup.Id == performGroupId 
                                                && p.tbPerformGroup.IsDeleted == false
                                              select p).ToList();

                List<Code.TreeHelper> result = new List<Code.TreeHelper>() {
                    new Code.TreeHelper() {
                        Id=-1,
                        name="科目列表",
                        open=true,
                        children=new List<Code.TreeHelper>()
                    }
                };
                result = GetCourseTree(result, subjects, tbCourseList, tbPerformCourseListOur);
                return Json(result);
            }
        }

        public List<Code.TreeHelper> GetCourseTree(List<Code.TreeHelper> result, List<Course.Entity.tbSubject> tbSubject, List<Course.Entity.tbCourse> tbCourse, List<Perform.Entity.tbPerformCourse> tbPerformCourseListOur)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                foreach (var subject in tbSubject)
                {
                    var node = new Code.TreeHelper();
                    node.name = subject.SubjectName;
                    node.Id = subject.Id;
                    node.open = true;
                    node.isChecked = false;
                    node.chkDisabled = false;
                    var courseList = (from p in tbCourse.Where(d => d.tbSubject.Id == subject.Id)
                                      select p).ToList();
                    node.children = GetCourseTreeSub(courseList, tbPerformCourseListOur);
                    result[0].children.Add(node);
                }
                return result;
            }
        }

        private List<Code.TreeHelper> GetCourseTreeSub(List<Course.Entity.tbCourse> courseList, List<Perform.Entity.tbPerformCourse> tbPerformCourseListOur)
        {
            var result = new List<Code.TreeHelper>();
            foreach (var course in courseList)
            {
                var node = new Code.TreeHelper();
                node.name = course.CourseName;
                node.Id = course.Id;
                node.open = true;
                node.isChecked = tbPerformCourseListOur.Where(d => d.tbCourse.Id == course.Id).FirstOrDefault() == null ? false : true;
                node.children = null;
                result.Add(node);
            }
            return result;
        }
    }
}