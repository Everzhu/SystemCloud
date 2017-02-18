using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Study.Controllers
{
    public class StudyClassController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.StudyClass.List();
                vm.RoomList = Basis.Controllers.RoomController.SelectList(0, 0, true);
                var tbClass = from p in db.Table<Basis.Entity.tbClass>()
                              select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tbClass = tbClass.Where(d => d.ClassName.Contains(vm.SearchText));
                }

                var tbStudyClass = from p in db.Table<Study.Entity.tbStudyClass>()
                                   where p.tbStudy.Id == vm.StudyId
                                   && p.tbStudy.IsDeleted == false
                                   && p.tbClass.IsDeleted == false
                                   select p;

                vm.StudyClassList = (from p in tbClass
                                     orderby p.No
                                     select new Dto.StudyClass.List
                                     {
                                         Id = p.Id,
                                         GradeName = p.tbGrade.GradeName,
                                         ClassName = p.ClassName,
                                         StudyId = vm.StudyId,
                                         ClassId = p.Id,
                                         ClassTypeName = p.tbClassType.ClassTypeName,
                                         IsChecked = tbStudyClass.Where(d => d.tbClass.Id == p.Id).Count() > 0 ? true : false,
                                         RoomId = tbStudyClass.Where(d => d.tbClass.Id == p.Id).Select(d => d.tbRoom.Id).FirstOrDefault()
                                     }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.StudyClass.List vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var arrystr = new string[] { };
                var txtCboxId = Request["CboxId"] != null ? Request["CboxId"].Split(',') : arrystr;
                var txtClassId = Request["txtClassId"] != null ? Request["txtClassId"].Split(',') : arrystr;
                var txtRoomId = Request["txtRoomId"] != null ? Request["txtRoomId"].Split(',') : arrystr;

                #region 删除关系
                var tbStudyClassList = (from p in db.Table<Study.Entity.tbStudyClass>()
                                .Include(d => d.tbClass)
                                .Include(d => d.tbStudy)
                                        where p.tbStudy.Id == vm.StudyId
                                        && p.tbStudy.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        select p).ToList();

                foreach (var a in tbStudyClassList.Where(d => txtCboxId.Contains(d.tbClass.Id.ToString()) == false))
                {
                    a.IsDeleted = true;
                }

                var tbStudyStudentList = (from p in db.Table<Study.Entity.tbStudyClassStudent>()
                                          .Include(d => d.tbClass)
                                          .Include(d => d.tbStudy)
                                          where p.tbStudy.Id == vm.StudyId
                                          && p.tbClass.IsDeleted == false
                                          && p.tbStudy.IsDeleted == false
                                          && p.tbStudent.IsDeleted == false
                                          select p).ToList();

                foreach (var a in tbStudyStudentList.Where(d => txtCboxId.Contains(d.tbClass.Id.ToString()) == false))
                {
                    a.IsDeleted = true;
                }

                var tbStudyClassTeacherList = (from p in db.Table<Study.Entity.tbStudyClassTeacher>()
                                               .Include(d => d.tbClass)
                                               .Include(d => d.tbStudy)
                                               where p.tbStudy.Id == vm.StudyId
                                               && p.tbClass.IsDeleted == false
                                               && p.tbStudy.IsDeleted == false
                                               && p.tbTeacher.IsDeleted == false
                                               select p).ToList();

                foreach (var a in tbStudyClassTeacherList.Where(d => txtCboxId.Contains(d.tbClass.Id.ToString()) == false))
                {
                    a.IsDeleted = true;
                } 
                #endregion

                for (var i = 0; i < txtClassId.Count(); i++)
                {
                    if (txtCboxId.Where(d => d == txtClassId[i]).Count() > decimal.Zero)//勾选班级
                    {
                        if (tbStudyClassList.Where(d => d.tbClass.Id == txtClassId[i].ConvertToInt()).Count() > decimal.Zero)
                        {
                            var tf = tbStudyClassList.Where(d => d.tbClass.Id == txtClassId[i].ConvertToInt()).FirstOrDefault();
                            tf.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(txtRoomId[i].ConvertToInt());
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改晚自习班级");
                        }
                        else
                        {
                            //没有id的，执行插入操作
                            var tf = new Study.Entity.tbStudyClass();
                            tf.tbRoom = db.Set<Basis.Entity.tbRoom>().Find(txtRoomId[i].ConvertToInt());
                            tf.tbClass = db.Set<Basis.Entity.tbClass>().Find(txtClassId[i].ConvertToInt());
                            tf.tbStudy = db.Set<Study.Entity.tbStudy>().Find(vm.StudyId);
                            db.Set<Study.Entity.tbStudyClass>().Add(tf);
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加晚自习班级");

                            //默认插入本班学生
                            var classId = txtClassId[i].ConvertToInt();
                            var tbStudentIds = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                                where p.tbClass.Id == classId
                                                && p.tbClass.IsDeleted == false
                                                && p.tbStudent.IsDeleted == false
                                                select p.tbStudent.Id).Distinct().ToList();

                            foreach (var studentId in tbStudentIds)
                            {
                                var tfStudent = new Study.Entity.tbStudyClassStudent();
                                tfStudent.tbClass = db.Set<Basis.Entity.tbClass>().Find(classId);
                                tfStudent.tbStudy = db.Set<Study.Entity.tbStudy>().Find(vm.StudyId);
                                tfStudent.tbStudent = db.Set<Student.Entity.tbStudent>().Find(studentId);
                                db.Set<Study.Entity.tbStudyClassStudent>().Add(tfStudent);
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加晚自习班级学生");
                            }
                        }
                    }

                }
                db.SaveChanges();
            }
            return Code.MvcHelper.Post(null, Url.Action("List", "StudyClassTeacher", new { studyId = vm.StudyId }));
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int studyId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Study.Entity.tbStudyClass>()
                          where p.tbStudy.Id == studyId
                          && p.tbStudy.IsDeleted == false
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.tbClass.ClassName,
                              Value = p.tbClass.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}