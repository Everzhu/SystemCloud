using System;
using System.Linq;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Controllers
{
    public class MoralReportController : Controller
    {
        // GET: Moral/MoralReport
        public ActionResult List()
        {
            var vm = new Models.MoralReport.List();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.MoralList = MoralController.SelectList();

                if (vm.MoralId == 0 && vm.MoralList.Count() > 0)
                {
                    vm.MoralId = vm.MoralList.Select(t => t.Value).FirstOrDefault().ConvertToInt();
                }

                var moral = db.Set<Entity.tbMoral>().Find(vm.MoralId);
                if (moral == null)
                {
                    var endDate = DateTime.Now.Date;
                    moral = (from p in db.Table<Entity.tbMoral>() where endDate <= p.ToDate && DateTime.Now >= p.FromDate select p).FirstOrDefault();
                }
                if (moral == null)
                {
                    vm.MoralIsNull = true;
                    return View(vm);
                }

                if ((!vm.FromDate.HasValue || !vm.ToDate.HasValue) || vm.ToDate <= vm.FromDate)
                {
                    vm.FromDate = moral.FromDate;
                    vm.ToDate = moral.ToDate;
                }

                SetVmData(db, vm);
            }
            return View(vm);
        }

        private void SetVmData(XkSystem.Models.DbContext db, Models.MoralReport.List vm)
        {
            var moralClassList = (from p in db.Table<Entity.tbMoralClass>() where p.tbMoral.Id == vm.MoralId select p);

            var classIds = moralClassList.Select(p => p.tbClass.Id);

            var moralStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>() where classIds.Contains(p.tbClass.Id) select p.tbStudent);

            var moralClassGroupList = (from p in db.Table<Basis.Entity.tbClassGroup>() where classIds.Contains(p.tbClass.Id) select p);

            var moralData = (from p in db.Table<Entity.tbMoralData>()
                             join cs in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals cs.tbStudent.Id into result
                             from s in result.DefaultIfEmpty()
                             where
                                p.CheckStatus == Code.EnumHelper.CheckStatus.Success &&
                                p.tbMoralItem.tbMoralGroup.tbMoral.Id == vm.MoralId &&
                                p.MoralDate >= vm.FromDate && p.MoralDate <= vm.ToDate
                             select new
                             {
                                 tbMoralData = p,
                                 tbClassStudent = s
                             });

            vm.DataList = moralClassList.Select(p => new Dto.MoralReport.List()
            {   
                ClassId = p.tbClass.Id,
                ClassName = p.tbClass.tbGrade.GradeName+p.tbClass.ClassName
            }).ToList();

            vm.DataList.ForEach(p =>
            {
                p.ClassDataList = moralData.Where(c => c.tbMoralData.tbClass != null && c.tbMoralData.tbClass.Id == p.ClassId).Select(c => new Dto.MoralReport.ClassDetail()
                {
                    MoralDataId=c.tbMoralData.Id,
                    ClassId = p.ClassId,
                    MoralItemId = c.tbMoralData.tbMoralItem.Id,
                    MoralItemName = c.tbMoralData.tbMoralItem.MoralItemName,
                    MoralItemOperateType = c.tbMoralData.tbMoralItem.MoralItemOperateType,
                    Comment = c.tbMoralData.Comment,
                    Score = c.tbMoralData.DataText.HasValue ? c.tbMoralData.DataText.Value : 0,
                    Date = c.tbMoralData.MoralDate,
                    Reason=c.tbMoralData.tbMoralDataReason.Reason
                }).OrderBy(c => c.Date).ThenBy(c => c.ClassId).ToList();

                p.StudentDataList = moralData.Where(c => c.tbMoralData.tbStudent != null && c.tbClassStudent.tbClass.Id == p.ClassId).Select(c => new Dto.MoralReport.StudentDetail()
                {
                    MoralDataId=c.tbMoralData.Id,
                    StudentId = c.tbMoralData.tbStudent.Id,
                    StudentName = c.tbMoralData.tbStudent.StudentName,
                    ClassId = c.tbClassStudent.tbClass.Id,
                    MoralItemId = c.tbMoralData.tbMoralItem.Id,
                    MoralItemName = c.tbMoralData.tbMoralItem.MoralItemName,
                    MoralItemOperateType = c.tbMoralData.tbMoralItem.MoralItemOperateType,
                    Comment = c.tbMoralData.Comment,
                    Score = c.tbMoralData.DataText.HasValue ? c.tbMoralData.DataText.Value : 0,
                    Date = c.tbMoralData.MoralDate,
                    Reason=c.tbMoralData.tbMoralDataReason.Reason
                }).OrderBy(c => c.Date).ThenBy(c => c.StudentId).ToList();

                p.ClassGroupDataList = moralData.Where(c => c.tbMoralData.tbClassGroup != null && c.tbMoralData.tbClassGroup.tbClass.Id == p.ClassId).Select(c => new Dto.MoralReport.ClassGroupDetail()
                {
                    MoralDataId=c.tbMoralData.Id,
                    ClassGroupId = c.tbMoralData.tbClassGroup.Id,
                    ClassGroupName = c.tbMoralData.tbClassGroup.ClassGroupName,
                    ClassId = c.tbMoralData.tbClassGroup.tbClass.Id,
                    MoralItemId = c.tbMoralData.tbMoralItem.Id,
                    MoralItemName = c.tbMoralData.tbMoralItem.MoralItemName,
                    MoralItemOperateType = c.tbMoralData.tbMoralItem.MoralItemOperateType,
                    Comment = c.tbMoralData.Comment,
                    Score = c.tbMoralData.DataText.HasValue ? c.tbMoralData.DataText.Value : 0,
                    Date = c.tbMoralData.MoralDate,
                    Reason = c.tbMoralData.tbMoralDataReason.Reason
                }).OrderBy(c => c.Date).ThenBy(c => c.ClassGroupId).ToList();
            });

            //移除不包含任何数据的班级
            vm.DataList.RemoveAll(p =>
                (p.ClassDataList == null || !p.ClassDataList.Any()) &&
                (p.StudentDataList == null || !p.StudentDataList.Any()) &&
                (p.ClassGroupDataList == null || !p.ClassGroupDataList.Any())
            );

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.MoralReport.List vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("List", new
            {
                MoralId = vm.MoralId,
                FromDate = vm.FromDate,
                ToDate = vm.ToDate,
                ClassId = vm.ClassId
            }));
        }


        [HttpPost]
        public JsonResult UpdateCommit()
        {
            var id = Request["Id"].ConvertToInt();
            var comment = Request["Comment"];

            using (var db = new XkSystem.Models.DbContext())
            {
                var tbMoralData = db.Set<Entity.tbMoralData>().Find(id);
                if (tbMoralData != null)
                {
                    tbMoralData.Comment = comment;
                    tbMoralData.UpdateTime = DateTime.Now;
                }
                db.SaveChanges();
            }

                return Json(null);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportToWord(Models.MoralReport.List vm)
        {
            return Code.MvcHelper.Post(returnUrl: Url.Action("ExportToWord", new
            {
                MoralId = vm.MoralId,
                FromDate = vm.FromDate,
                ToDate = vm.ToDate
            }));
        }

        public ActionResult ExportToWord()
        {
            var vm = new Models.MoralReport.List();
            string file = System.IO.Path.GetTempFileName();
            if (string.IsNullOrEmpty(file) == false)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    var moral = db.Set<Entity.tbMoral>().Find(vm.MoralId);
                    if ((!vm.FromDate.HasValue || !vm.ToDate.HasValue) || vm.ToDate <= vm.FromDate)
                    {
                        vm.FromDate = moral.FromDate;
                        vm.ToDate = moral.ToDate;
                    }

                    SetVmData(db, vm);

                    using (var doc = Novacode.DocX.Create(file, Novacode.DocumentTypes.Document))
                    {
                        var pTitle = doc.InsertParagraph();
                        pTitle.AppendLine("德育报告").Bold().FontSize(20).Alignment = Novacode.Alignment.center;

                        var pSubTitle = doc.InsertParagraph();

                        pSubTitle.AppendLine($"德育：{moral.MoralName}").Bold().FontSize(14).Alignment = Novacode.Alignment.left;
                        pSubTitle.AppendLine($"报告日期：{vm.FromDate.Value.ToString(Code.Common.StringToDate)}至{vm.ToDate.Value.ToString(Code.Common.StringToDate)}").Bold().FontSize(14).Alignment = Novacode.Alignment.left;

                        foreach (var data in vm.DataList)
                        {
                            Novacode.Paragraph p1 = doc.InsertParagraph();
                            p1.AppendLine(data.ClassName).Bold().FontSize(14);
                            var table = doc.AddTable(data.ClassDataList.Count + data.ClassGroupDataList.Count + data.StudentDataList.Count + 1, 6);
                            table.Design = Novacode.TableDesign.TableGrid;
                            table.Alignment = Novacode.Alignment.center;

                            var rows = table.Rows;
                            var rowHeader = rows[0];
                            rowHeader.Cells[0].Paragraphs[0].Append("班级、小组、学生");
                            rowHeader.Cells[1].Paragraphs[0].Append("德育项目");
                            rowHeader.Cells[2].Paragraphs[0].Append("分数");
                            rowHeader.Cells[3].Paragraphs[0].Append("日期");
                            rowHeader.Cells[4].Paragraphs[0].Append("评分原因");
                            rowHeader.Cells[5].Paragraphs[0].Append("评语");

                            var index = 1;
                            foreach (var dItem in data.ClassDataList)
                            {
                                var row = rows[index];
                                row.Cells[0].Paragraphs[0].Append(data.ClassName);
                                row.Cells[1].Paragraphs[0].Append(dItem.MoralItemName);
                                row.Cells[2].Paragraphs[0].Append(dItem.Score.ToString());
                                row.Cells[3].Paragraphs[0].Append(dItem.Date.ToString(Code.Common.StringToDate));
                                row.Cells[4].Paragraphs[0].Append(dItem.Reason);
                                row.Cells[5].Paragraphs[0].Append(dItem.Comment);
                                index++;
                            }

                            foreach (var dItem in data.ClassGroupDataList)
                            {
                                var row = rows[index];
                                row.Cells[0].Paragraphs[0].Append(dItem.ClassGroupName);
                                row.Cells[1].Paragraphs[0].Append(dItem.MoralItemName);
                                row.Cells[2].Paragraphs[0].Append(dItem.Score.ToString());
                                row.Cells[3].Paragraphs[0].Append(dItem.Date.ToString(Code.Common.StringToDate));
                                row.Cells[4].Paragraphs[0].Append(dItem.Reason);
                                row.Cells[5].Paragraphs[0].Append(dItem.Comment);
                                index++;
                            }

                            foreach (var dItem in data.StudentDataList)
                            {
                                var row = rows[index];
                                row.Cells[0].Paragraphs[0].Append(dItem.StudentName);
                                row.Cells[1].Paragraphs[0].Append(dItem.MoralItemName);
                                row.Cells[2].Paragraphs[0].Append(dItem.Score.ToString());
                                row.Cells[3].Paragraphs[0].Append(dItem.Date.ToString(Code.Common.StringToDate));
                                row.Cells[4].Paragraphs[0].Append(dItem.Reason);
                                row.Cells[5].Paragraphs[0].Append(dItem.Comment);
                                index++;
                            }
                            p1.InsertTableAfterSelf(table);
                        }
                        doc.Save();
                    }
                }


            }
            return File(file, Code.Common.DownloadType, Code.Common.ExportByWord);

        }
    }
}