using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamLevelGroupController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamLevelGroup.List();

                var tb = from p in db.Table<Exam.Entity.tbExamLevelGroup>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ExamLevelGroupName.Contains(vm.SearchText));
                }

                vm.ExamLevelGroupList = (from p in tb
                                         orderby p.No
                                         select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamLevelGroup.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamLevelGroup>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除考试等级分组");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamLevelGroup.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamLevelGroup>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamLevelGroupEdit = tb;

                        vm.ExamLevelList = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                            where p.tbExamLevelGroup.Id == tb.Id
                                            select p).ToList();
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamLevelGroup.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                //等级组下的等级
                var arrystr = new string[] { };
                var txtId = Request["txtId"]!=null? Request["txtId"].Split(','): arrystr;
                var txtNo = Request["txtNo"]!=null? Request["txtNo"].Split(','): arrystr;
                var txtName = Request["txtName"]!=null?Request["txtName"].Split(','): arrystr;
                var txtValue = Request["txtValue"]!=null? Request["txtValue"].Split(','): arrystr;
                var txtRate = Request["txtRate"]!=null? Request["txtRate"].Split(','): arrystr;
                var txtMax = Request["txtMax"]!=null? Request["txtMax"].Split(','): arrystr;
                var txtMin = Request["txtMin"]!=null? Request["txtMin"].Split(','): arrystr;
                if (error.Count == decimal.Zero)
                {
                    if (vm.ExamLevelGroupEdit.Id == 0)
                    {
                        var tb = new Exam.Entity.tbExamLevelGroup();
                        tb.No = vm.ExamLevelGroupEdit.No == null ? db.Table<Exam.Entity.tbExamLevelGroup>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamLevelGroupEdit.No;
                        tb.ExamLevelGroupName = vm.ExamLevelGroupEdit.ExamLevelGroupName;
                        tb.IsGenerate = vm.ExamLevelGroupEdit.IsGenerate;
                        tb.IsTotal = vm.ExamLevelGroupEdit.IsTotal;
                        db.Set<Exam.Entity.tbExamLevelGroup>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考试等级分组");
                        }

                       #region 等级处理
                        var list = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                    where p.tbExamLevelGroup.Id == tb.Id
                                    select p).ToList();
                        foreach (var a in list.Where(d => txtId.Contains(d.Id.ToString()) == false))
                        {
                            a.IsDeleted = true;
                        }
                        for (var i = 0; i < txtId.Count(); i++)
                        {
                            if (string.IsNullOrEmpty(txtName[i]))
                            {
                                //输入内容为空,判断是否存在Id
                                if (string.IsNullOrEmpty(txtId[i]) == false)
                                {
                                    //如果是有id的，那就是数据库中记录的，应该做删除
                                    var tf = list.Where(d => d.Id == txtId[i].ConvertToInt()).FirstOrDefault();
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除考试等级");
                                    tb.IsDeleted = true;
                                }
                            }
                            else
                            {
                                //输入内容不为空，判断是否存在id并执行对应的操作
                                if (string.IsNullOrEmpty(txtId[i]) == false)
                                {
                                    //如果有id的，执行更新操作
                                    var tf = list.Where(d => d.Id == txtId[i].ConvertToInt()).FirstOrDefault();
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改考试等级");
                                    tf.No = txtNo[i].ConvertToInt();
                                    tf.ExamLevelName = txtName[i];
                                    tf.ExamLevelValue = txtValue[i].ConvertToDecimal();
                                    tf.Rate = txtRate[i].ConvertToDecimal();
                                    tf.MaxScore = txtMax[i].ConvertToDecimal();
                                    tf.MinScore = txtMin[i].ConvertToDecimal();
                                }
                                else
                                {
                                    //没有id的，执行插入操作
                                    var tf = new Exam.Entity.tbExamLevel();
                                    tf.tbExamLevelGroup = db.Set<Exam.Entity.tbExamLevelGroup>().Find(tb.Id);
                                    tf.No = txtNo[i].ConvertToInt();
                                    tf.ExamLevelName = txtName[i];
                                    tf.ExamLevelValue = txtValue[i].ConvertToDecimal();
                                    tf.Rate =txtRate[i].ConvertToDecimal();
                                    tf.MaxScore = txtMax[i].ConvertToDecimal();
                                    tf.MinScore = txtMin[i].ConvertToDecimal();
                                    db.Set<Exam.Entity.tbExamLevel>().Add(tf);
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考试等级");
                                }
                            }
                        }
                        db.SaveChanges();
                        #endregion
                    }
                    else
                    {
                        var tb = (from p in db.Table<Exam.Entity.tbExamLevelGroup>()
                                  where p.Id == vm.ExamLevelGroupEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.No = vm.ExamLevelGroupEdit.No == null ? db.Table<Exam.Entity.tbExamLevelGroup>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ExamLevelGroupEdit.No;
                            tb.ExamLevelGroupName = vm.ExamLevelGroupEdit.ExamLevelGroupName;
                            tb.IsGenerate = vm.ExamLevelGroupEdit.IsGenerate;
                            tb.IsTotal = vm.ExamLevelGroupEdit.IsTotal;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改考试等级分组");
                            }

                            #region 等级处理
                            var list = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                        where p.tbExamLevelGroup.Id == vm.ExamLevelGroupEdit.Id
                                        select p).ToList();
                            foreach (var a in list.Where(d => txtId.Contains(d.Id.ToString()) == false))
                            {
                                a.IsDeleted = true;
                            }

                            for (var i = 0; i < txtId.Count(); i++)
                            {
                                if (string.IsNullOrEmpty(txtName[i]))
                                {
                                    //输入内容为空,判断是否存在Id
                                    if (string.IsNullOrEmpty(txtId[i]) == false)
                                    {
                                        //如果是有id的，那就是数据库中记录的，应该做删除
                                        var tf = list.Where(d => d.Id == txtId[i].ConvertToInt()).FirstOrDefault();
                                        tb.IsDeleted = true;
                                    }
                                }
                                else
                                {
                                    //输入内容不为空，判断是否存在id并执行对应的操作
                                    if (string.IsNullOrEmpty(txtId[i]) == false)
                                    {
                                        //如果有id的，执行更新操作
                                        var tf = list.Where(d => d.Id == txtId[i].ConvertToInt()).FirstOrDefault();
                                        tf.No = txtNo[i].ConvertToInt();
                                        tf.ExamLevelName = txtName[i];
                                        tf.ExamLevelValue = txtValue[i].ConvertToDecimal();
                                        tf.Rate = txtRate[i].ConvertToDecimal();
                                        tf.MaxScore = txtMax[i].ConvertToDecimal();
                                        tf.MinScore = txtMin[i].ConvertToDecimal();
                                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改考试等级");
                                    }
                                    else
                                    {
                                        //没有id的，执行插入操作
                                        var tf = new Exam.Entity.tbExamLevel();
                                        tf.tbExamLevelGroup = db.Set<Exam.Entity.tbExamLevelGroup>().Find(vm.ExamLevelGroupEdit.Id);
                                        tf.No = txtNo[i].ConvertToInt();
                                        tf.ExamLevelName = txtName[i];
                                        tf.ExamLevelValue = txtValue[i].ConvertToDecimal();
                                        tf.Rate = txtRate[i].ConvertToDecimal();
                                        tf.MaxScore = txtMax[i].ConvertToDecimal();
                                        tf.MinScore = txtMin[i].ConvertToDecimal();
                                        db.Set<Exam.Entity.tbExamLevel>().Add(tf);
                                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考试等级");
                                    }
                                }
                            }
                            db.SaveChanges();
                            #endregion
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamLevelGroup>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.ExamLevelGroupName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetGenerate(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Exam.Entity.tbExamLevelGroup>().Find(id);
                if (tb != null)
                {
                    tb.IsGenerate = !tb.IsGenerate;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetTotal(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Exam.Entity.tbExamLevelGroup>().Find(id);
                if (tb != null)
                {
                    tb.IsTotal = !tb.IsTotal;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectTotalList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamLevelGroup>()
                          where p.IsTotal==true
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.ExamLevelGroupName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}