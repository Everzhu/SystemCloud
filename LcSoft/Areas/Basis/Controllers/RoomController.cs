using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class RoomController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Room.List();
                vm.BuildList = BuildController.SelectList();
                vm.RoomTypeList = RoomTypeController.SelectList();

                var tb = from p in db.Table<Basis.Entity.tbRoom>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.RoomName.Contains(vm.SearchText));
                }

                if (vm.BuildId != 0)
                {
                    tb = tb.Where(d => d.tbBuild.Id == vm.BuildId);
                }

                if (vm.RoomTypeId != 0)
                {
                    tb = tb.Where(d => d.tbRoomType.Id == vm.RoomTypeId);
                }

                vm.RoomList = (from p in tb
                               orderby  p.RoomName,p.No
                               select new Dto.Room.List
                               {
                                   Id = p.Id,
                                   No = p.No,
                                   RoomName = p.RoomName,
                                   BuildName = p.tbBuild.BuildName,
                                   RoomTypeName = p.tbRoomType.RoomTypeName,
                                   MaxUser = p.MaxUser
                               }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Room.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                searchText = vm.SearchText,
                BuildId = vm.BuildId,
                roomTypeId = vm.RoomTypeId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbRoom>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了教室");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Room.Edit();
                vm.BuildList = BuildController.SelectList();
                vm.RoomTypeList = RoomTypeController.SelectList();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbRoom>()
                              where p.Id == id
                              select new Dto.Room.Edit
                              {
                                  Id = p.Id,
                                  No = p.No,
                                  AttendanceMachine = p.AttendanceMachine,
                                  RoomName = p.RoomName,
                                  BuildId = p.tbBuild.Id,
                                  RoomTypeId = p.tbRoomType.Id,
                                  MaxUser = p.MaxUser
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.RoomEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Room.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Basis.Entity.tbRoom>().Where(d=>d.RoomName == vm.RoomEdit.RoomName && d.Id != vm.RoomEdit.Id).Any())
                    {
                        error.AddError("该教室已存在!");
                    }
                    else
                    {
                        if (vm.RoomEdit.Id == 0)
                        {
                            var tb = new Basis.Entity.tbRoom();
                            tb.No = vm.RoomEdit.No == null ? db.Table<Basis.Entity.tbRoom>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.RoomEdit.No;
                            tb.RoomName = vm.RoomEdit.RoomName;
                            tb.tbBuild = db.Set<Basis.Entity.tbBuild>().Find(vm.RoomEdit.BuildId);
                            tb.tbRoomType = db.Set<Basis.Entity.tbRoomType>().Find(vm.RoomEdit.RoomTypeId);
                            tb.MaxUser = vm.RoomEdit.MaxUser;
                            tb.AttendanceMachine = vm.RoomEdit.AttendanceMachine;
                            db.Set<Basis.Entity.tbRoom>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了教室");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Basis.Entity.tbRoom>()
                                      where p.Id == vm.RoomEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.RoomEdit.No == null ? db.Table<Basis.Entity.tbRoom>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.RoomEdit.No;
                                tb.RoomName = vm.RoomEdit.RoomName;
                                tb.tbBuild = db.Set<Basis.Entity.tbBuild>().Find(vm.RoomEdit.BuildId);
                                tb.tbRoomType = db.Set<Basis.Entity.tbRoomType>().Find(vm.RoomEdit.RoomTypeId);
                                tb.MaxUser = vm.RoomEdit.MaxUser;
                                tb.AttendanceMachine = vm.RoomEdit.AttendanceMachine;

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了教室");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int id = 0, int buildId = 0, bool isShowGroup = false)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = from p in db.Table<Basis.Entity.tbRoom>()
                         select p;

                var list = (from p in tb
                            orderby p.RoomName
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = ((isShowGroup && p.tbBuild != null) ? ("[" + p.tbBuild.BuildName + "]") : string.Empty) + p.RoomName,
                                Value = p.Id.ToString(),
                                Selected = p.Id == id
                            }).ToList();
                return list;
            }
        }

        public ActionResult Export()
        {
            var vm = new Models.Room.Export();
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var tb = db.Table<Basis.Entity.tbRoom>();
                if (vm.BuildId != 0)
                {
                    tb = tb.Where(d => d.tbBuild.Id == vm.BuildId);
                }
                if (vm.RoomTypeId != 0)
                {
                    tb = tb.Where(d => d.tbRoomType.Id == vm.RoomTypeId);
                }
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.RoomName.Contains(vm.SearchText));
                }

                var roomList = (from p in tb
                                select new Dto.Room.Export
                                {
                                    No = p.No,
                                    RoomName = p.RoomName,
                                    MaxUser = p.MaxUser,
                                    BuildName = p.tbBuild.BuildName,
                                    RoomTypeName = p.tbRoomType.RoomTypeName
                                }).ToList();
                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("排序"),
                        new System.Data.DataColumn("教室"),
                        new System.Data.DataColumn("容纳人数"),
                        new System.Data.DataColumn("教学楼"),
                        new System.Data.DataColumn("教室类型")
                    });
                foreach (var a in roomList)
                {
                    var dr = dt.NewRow();
                    dr["排序"] = a.No;
                    dr["教室"] = a.RoomName;
                    dr["容纳人数"] = a.MaxUser;
                    dr["教学楼"] = a.BuildName;
                    dr["教室类型"] = a.RoomTypeName;
                    dt.Rows.Add(dr);
                }

                Code.NpoiHelper.DataTableToExcel(file, dt);

                if (string.IsNullOrEmpty(file) == false)
                {
                    return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
                }
                else
                {
                    return View();
                }
            }
        }

        public ActionResult Import()
        {
            var vm = new Models.Room.Import();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.Room.Import vm)
        {
            vm.ImportList = new List<Dto.Room.Import>();
            if (ModelState.IsValid)
            {
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);

                using (var db = new XkSystem.Models.DbContext())
                {
                    if (Code.Common.GetFileType(file.FileName) != Code.FileType.Excel)
                    {
                        ModelState.AddModelError("", "上传的文件不是正确的EXCLE文件!");
                        return View(vm);
                    }

                    var dt = Code.NpoiHelper.ExcelToDataTable(fileSave, System.IO.Path.GetExtension(file.FileName), string.Empty);
                    if (dt == null)
                    {
                        ModelState.AddModelError("", "无法读取上传的文件，请检查文件格式是否正确!");
                        return View(vm);
                    }

                    var tbList = new List<string>() { "房间号", "教室", "容纳人数", "教学楼", "教室类型","考勤机" };

                    var Text = string.Empty;
                    foreach (var a in tbList)
                    {
                        if (dt.Columns.Contains(a.ToString()) == false)
                        {
                            Text += a + ",";
                        }
                    }

                    if (string.IsNullOrEmpty(Text) == false)
                    {
                        ModelState.AddModelError("", "上传的EXCEL内容与预期不一致，缺少对应的字段：" + Text);
                        return View(vm);
                    }

                    //将DataTable转为List
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        var dto = new Dto.Room.Import();
                        dto.No = dr["房间号"].ConvertToString();
                        dto.RoomName = dr["教室"].ConvertToString();
                        dto.MaxUser = dr["容纳人数"].ConvertToString();
                        dto.BuildName = dr["教学楼"].ConvertToString();
                        dto.RoomTypeName = dr["教室类型"].ConvertToString();
                        dto.AttendanceMachine = dr["考勤机"].ConvertToString();
                        vm.ImportList.Add(dto);
                    }

                    vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.RoomName + d.MaxUser));

                    if (vm.ImportList.Count == 0)
                    {
                        ModelState.AddModelError("", "未读取到任何有效数据!");
                        return View(vm);
                    }

                    var roomList = (from p in db.Table<Basis.Entity.tbRoom>()
                                    select p).ToList();
                    var BuildList = db.Table<Basis.Entity.tbBuild>().ToList();
                    var roomTypeList = db.Table<Basis.Entity.tbRoomType>().ToList();

                    #region 验证数据格式是否正确
                    foreach (var item in vm.ImportList)
                    {
                        if (!string.IsNullOrEmpty(item.BuildName)
                            && BuildList.Where(d => d.BuildName == item.BuildName).Count() == 0)
                        {
                            item.Error += "教学楼不存在；";
                        }

                        if (!string.IsNullOrEmpty(item.RoomTypeName)
                            && roomTypeList.Where(d => d.RoomTypeName == item.RoomTypeName).Count() == 0)
                        {
                            item.Error += "教室类型不存在；";
                        }

                        if (string.IsNullOrEmpty(item.RoomName))
                        {
                            item.Error = item.Error + "教室不能为空!";
                        }

                        if (vm.ImportList.Where(d => d.RoomName == item.RoomName).Count() > 1)
                        {
                            item.Error = item.Error + "该条数据重复!";
                        }

                        if (string.IsNullOrEmpty(item.No) == false)
                        {
                            int no = 0;
                            if (int.TryParse(item.No, out no) == false && no >= 0)
                            {
                                item.Error = item.Error + "房间号必须是正整数!";
                            }
                        }

                        if (string.IsNullOrEmpty(item.MaxUser) == false)
                        {
                            int maxUser = 0;
                            if (int.TryParse(item.MaxUser, out maxUser) == false && maxUser >= 0)
                            {
                                item.Error = item.Error + "容纳人数必须是正整数!";
                            }
                        }

                        if (vm.IsUpdate == false && roomList.Where(d => d.RoomName == item.RoomName).Count() > 0)
                        {
                            item.Error = item.Error + "系统中已存在该记录!";
                        }
                    }

                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    #region 数据导入，判断关键字，存在的数据做更新，不存在的做新增
                    List<Basis.Entity.tbRoom> listRoom = new List<Basis.Entity.tbRoom>();
                    foreach (var item in vm.ImportList)
                    {
                        if (roomList.Where(d => d.RoomName == item.RoomName).Count() > 0)
                        {
                            if (vm.IsUpdate)
                            {
                                if (roomList.Where(d => d.RoomName == item.RoomName).Count() > 1)
                                {
                                    item.Error = item.Error + "系统中该教室数据存在重复，无法确认需要更新的记录!";
                                }
                                else
                                {
                                    var tb = roomList.Where(d => d.RoomName == item.RoomName).FirstOrDefault();
                                    tb.No = item.No.ConvertToInt();
                                    tb.RoomName = item.RoomName;
                                    tb.MaxUser = item.MaxUser.ConvertToInt();
                                    tb.AttendanceMachine = item.AttendanceMachine;

                                    if (!string.IsNullOrEmpty(item.RoomTypeName))
                                    {
                                        tb.tbRoomType = roomTypeList.Where(d => d.RoomTypeName == item.RoomTypeName).FirstOrDefault();
                                    }

                                    if (!string.IsNullOrEmpty(item.BuildName))
                                    {
                                        tb.tbBuild = BuildList.Where(d => d.BuildName == item.BuildName).FirstOrDefault();
                                    }
                                }
                            }
                            else
                            {
                                item.Error += "教室已经存在；";
                            }
                        }
                        else
                        {
                            var tb = new Basis.Entity.tbRoom();
                            tb.No = item.No.ConvertToInt();
                            tb.RoomName = item.RoomName;
                            tb.MaxUser = item.MaxUser.ConvertToInt();
                            tb.AttendanceMachine = item.AttendanceMachine;
                            if (!string.IsNullOrEmpty(item.RoomTypeName))
                            {
                                tb.tbRoomType = roomTypeList.Where(d => d.RoomTypeName == item.RoomTypeName).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(item.BuildName))
                            {
                                tb.tbBuild = BuildList.Where(d => d.BuildName == item.BuildName).FirstOrDefault();
                            }

                            listRoom.Add(tb);
                        }
                    }

                    if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        return View(vm);
                    }
                    #endregion

                    db.Set<Basis.Entity.tbRoom>().AddRange(listRoom);

                    if (db.SaveChanges() > 0)
                    {
                        vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入教室信息");
                        vm.Status = true;
                    }
                }
            }

            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Basis/Views/Room/RoomTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        public ActionResult SelectRoom()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Room.SelectRoom();
                vm.BuildList = BuildController.SelectList();
                vm.RoomTypeList = RoomTypeController.SelectList();

                var tb = from p in db.Table<Basis.Entity.tbRoom>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.RoomName.Contains(vm.SearchText));
                }

                if (vm.BuildId != 0)
                {
                    tb = tb.Where(d => d.tbBuild.Id == vm.BuildId);
                }

                if (vm.RoomTypeId != 0)
                {
                    tb = tb.Where(d => d.tbRoomType.Id == vm.RoomTypeId);
                }

                vm.SelectRoomList = (from p in tb
                                     orderby p.tbBuild.No, p.tbBuild.BuildName, p.RoomName
                                     select new Dto.Room.SelectRoom
                                     {
                                         Id = p.Id,
                                         No = p.No,
                                         RoomName = p.RoomName,
                                         BuildName = p.tbBuild.BuildName,
                                         RoomTypeName = p.tbRoomType.RoomTypeName,
                                         MaxUser = p.MaxUser
                                     }).ToPageList(vm.Page);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectRoom(Models.Room.SelectRoom vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SelectRoom", new
            {
                searchText = vm.SearchText,
                BuildId = vm.BuildId,
                roomTypeId = vm.RoomTypeId,
                pageIndex = vm.Page.PageIndex,
                pageSize = vm.Page.PageSize
            }));
        }
    }
}