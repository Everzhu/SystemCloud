using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Survey.Controllers
{
    public class SurveyCourseController : Controller
    {
        public ActionResult List()
        {
            var vm = new Models.SurveyCourse.List();

            return PartialView(vm);
        }

        /// <summary>
        /// 修改或新增评价科目列表
        /// </summary>
        /// <param name="db">数据库上下文对象</param>
        /// <param name="ids">菜单ID集合</param>
        /// <param name="surveyGroup">评价分组数据</param>
        /// <param name="surveyId">评教Id</param>
        /// <returns></returns>
        [NonAction]
        public dynamic SaveSubjectNode(XkSystem.Models.DbContext db, string ids, Entity.tbSurveyGroup surveyGroup, int surveyId)
        {
            //查出整个评价Id的所有组的科目列表
            var tb = (from p in db.Table<Entity.tbSurveyCourse>()
                      .Include(d => d.tbSurveyGroup)
                      .Include(d => d.tbCourse)
                      where p.tbSurveyGroup.tbSurvey.Id == surveyId
                      && p.tbCourse.IsDeleted == false
                      && p.tbSurveyGroup.IsDeleted == false
                      && p.tbSurveyGroup.tbSurvey.IsDeleted == false
                      select p).ToList();

            //-1为虚构的根节点，要去掉
            var idArr = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(d => d.ConvertToInt() != -1).Select(d => d.ConvertToInt());

            //取交集，同一SurveyId中的多个分组Subject不能重复，且判断GroupId不能是当前要修改的Id
            bool flag = tb.Where(d => d.tbSurveyGroup.Id != surveyGroup.Id).Select(d => d.tbCourse.Id).Intersect(idArr).Count() > 0;

            if (flag)
                return new { Status = decimal.Zero, Message = "同一评价下不同分组课程不能重复！" };

            //修改当前分组的老数据为删除状态
            foreach (var a in tb.Where(d => d.tbSurveyGroup.Id == surveyGroup.Id))
            {
                a.IsDeleted = true;
            }
            var addtbSurveyCourseList = new List<Entity.tbSurveyCourse>();
            foreach (var id in idArr)
            {
                var temp = new Entity.tbSurveyCourse();
                temp.tbSurveyGroup = surveyGroup;
                temp.tbCourse = db.Set<Course.Entity.tbCourse>().Find(id);
                addtbSurveyCourseList.Add(temp);
            }
            db.Set<Entity.tbSurveyCourse>().AddRange(addtbSurveyCourseList);
            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加评价课程");
            return new { Status = decimal.One, Message = "操作成功！" };
        }

        /// <summary>
        /// 获取评价科目Tree菜单数据
        /// </summary>
        /// <param name="surveyGroupId">评价分组Id</param>
        /// <returns></returns>
        public ActionResult GetSurveySubjectTree(int surveyGroupId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                //查询所有科目
                var courses = (from p in db.Table<Course.Entity.tbCourse>()
                               select p).ToList();

                //查询该用户拥有的科目列表
                var tb = from p in db.Table<Entity.tbSurveyCourse>()
                         where p.tbCourse.IsDeleted == false
                            && p.tbCourse.tbSubject.IsDeleted == false
                            && p.tbSurveyGroup.Id == surveyGroupId
                         select p;

                var result = new List<Code.TreeHelper>() { new Code.TreeHelper() { Id = -1, name = "课程列表", open = true, children = new List<Code.TreeHelper>() } };

                foreach (var course in courses)
                {
                    var temp = tb.Where(t => t.tbCourse.Id == course.Id).FirstOrDefault();

                    result[0].children.Add(new Code.TreeHelper()
                    {
                        Id = course.Id,
                        name = course.CourseName,
                        open = true,
                        isChecked = temp != null
                    });
                }

                return Json(result);
            }
        }


        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int groupId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (groupId != 0)
                {
                    var tb = (from p in db.Table<Entity.tbSurveyCourse>()
                              where p.tbCourse.IsDeleted == false
                                && p.tbCourse.tbSubject.IsDeleted == false
                                && p.tbSurveyGroup.Id == groupId
                              orderby p.tbCourse.tbSubject.No, p.tbCourse.No
                              select new System.Web.Mvc.SelectListItem
                              {
                                  Text = p.tbCourse.CourseName,
                                  Value = p.tbCourse.Id.ToString()
                              }).ToList();
                    return tb;
                }
                else
                {
                    return new List<SelectListItem>();
                }
            }
        }
    }
}