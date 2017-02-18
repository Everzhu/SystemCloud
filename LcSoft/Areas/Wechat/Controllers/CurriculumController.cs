using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Wechat.Controllers
{
    public class CurriculumController : Controller
    {
        public ActionResult CurriculumIndex()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var teacherId = db.Table<Teacher.Entity.tbTeacher>().Where(m => m.tbSysUser.Id == Code.Common.UserId).Select(m => m.Id).FirstOrDefault();
                var yearId = db.Table<Basis.Entity.tbYear>().Where(m => m.IsDefault == true).Select(m => m.Id).FirstOrDefault();

                var vm = new Course.Models.Schedule.ClassAll();
                vm.WeekList = Basis.Controllers.WeekController.SelectList();
                vm.PeriodList = Basis.Controllers.PeriodController.SelectScheduleList();
                vm.OrgScheduleList = Course.Controllers.OrgScheduleController.GetTeacherAll(teacherId, yearId).ToList();

                return View(vm);
            }
        }
    }
}