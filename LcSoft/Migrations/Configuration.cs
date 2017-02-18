namespace XkSystem.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<XkSystem.Models.DbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "XkSystem";
        }

        protected override void Seed(XkSystem.Models.DbContext db)
        {
            #region tbProgram
            var programList = new List<Areas.Admin.Entity.tbProgram>()
            {
                new Areas.Admin.Entity.tbProgram()
                {
                    No = 6,
                    ProgramName = "教务管理",
                    ProgramCode = XkSystem.Code.EnumHelper.ProgramCode.EAS,
                    ProgramTitle = "教务管理",
                    IsWide = true,
                    BgIcon = "glyphicon glyphicon-blackboard",
                    BgColor = "#05bd94",
                    Remark = "新生注册资料填写、学籍管理、选课、课表查看、成绩学分。",
                },
                new Areas.Admin.Entity.tbProgram()
                {
                    No = 1,
                    ProgramName = "云桌面",
                    ProgramCode = XkSystem.Code.EnumHelper.ProgramCode.Desktop,
                    ProgramTitle = "云桌面",
                    IsWide = true,
                    BgIcon = "glyphicon glyphicon-modal-window",
                    BgColor = "#3e95ed",
                    Remark = "个性化桌面、消息中心、常用功能聚合中心。",
                },
                new Areas.Admin.Entity.tbProgram()
                {
                    No = 2,
                    ProgramName = "基础数据库平台",
                    ProgramCode = XkSystem.Code.EnumHelper.ProgramCode.Base,
                    ProgramTitle = "基础数据库平台",
                    IsWide = true,
                    BgIcon = "glyphicon glyphicon-oil",
                    BgColor = "#fd971f",
                    Remark = "消息中心、接口中心、基础模块管理。",
                },
                //new Areas.Admin.Entity.tbProgram()
                //{
                //    No = 3,
                //    ProgramName = "招生管理",
                //    ProgramCode = XkSystem.Code.EnumHelper.ProgramCode.Admit,
                //    ProgramTitle = "招生管理",
                //    IsWide = true,
                //    BgIcon = "glyphicon glyphicon-user",
                //    BgColor = "#14b2cf",
                //    Remark = "招生设置、报名管理、报名资料审核、统计查询。",
                //},
                new Areas.Admin.Entity.tbProgram()
                {
                    No = 4,
                    ProgramName = "门户网站管理",
                    ProgramCode = XkSystem.Code.EnumHelper.ProgramCode.Portal,
                    ProgramTitle = "门户网站管理",
                    IsWide = true,
                    BgIcon = "glyphicon glyphicon-home",
                    BgColor = "#e86586",
                    Remark = "栏目分级、信息发布、支持多校区及网站信息推送。",
                },
                new Areas.Admin.Entity.tbProgram()
                {
                    No = 5,
                    ProgramName = "协同办公",
                    ProgramCode = XkSystem.Code.EnumHelper.ProgramCode.OA,
                    ProgramTitle = "协同办公",
                    IsWide = true,
                    BgIcon = "glyphicon glyphicon-pencil",
                    BgColor = "#c15ed7",
                    Remark = "公文流程、流程管理、PC端与手机端同步。",
                },
                new Areas.Admin.Entity.tbProgram()
                {
                    No = 7,
                    ProgramName = "排课管理",
                    ProgramCode = XkSystem.Code.EnumHelper.ProgramCode.Timetable,
                    ProgramTitle = "排课管理",
                    IsWide = true,
                    BgIcon = "glyphicon glyphicon-retweet",
                    BgColor = "#f5c405",
                    Remark = "多种参数设置自动排课，支持分层走班，排课结果可手工调整。",
                },
                new Areas.Admin.Entity.tbProgram()
                {
                    No = 8,
                    ProgramName = "教学质量分析",
                    ProgramCode = XkSystem.Code.EnumHelper.ProgramCode.ExamAnalyze,
                    ProgramTitle = "教学质量分析",
                    IsWide = true,
                    BgIcon = "glyphicon glyphicon-stats",
                    BgColor = "#78c82a",
                    Remark = "成绩统计，按班级、学生进行横向、纵向对比，多种图表展示。",
                },
                new Areas.Admin.Entity.tbProgram()
                {
                    No = 9,
                    ProgramName = "德育管理",
                    ProgramCode = XkSystem.Code.EnumHelper.ProgramCode.Moral,
                    ProgramTitle = "德育管理",
                    IsWide = true,
                    BgIcon = "glyphicon glyphicon-edit",
                    BgColor = "#14b2cf",
                    Remark = "可针对班级、小组、个人进行德育考核登记，汇总生成考核结果。",
                },
                new Areas.Admin.Entity.tbProgram()
                {
                    No = 10,
                    ProgramName = "综合素养评价",
                    ProgramCode = XkSystem.Code.EnumHelper.ProgramCode.Quality,
                    ProgramTitle = "综合素养评价",
                    IsWide = true,
                    BgIcon = "glyphicon glyphicon-globe",
                    BgColor = "#e86586",
                    Remark = "学生成长记录，多纬度评价管理。",
                },
                new Areas.Admin.Entity.tbProgram()
                {
                    No = 12,
                    ProgramName = "公开课管理",
                    ProgramCode = XkSystem.Code.EnumHelper.ProgramCode.Open,
                    ProgramTitle = "公开课管理",
                    IsWide = true,
                    BgIcon = "glyphicon glyphicon-eye-open",
                    BgColor = "#fd971f",
                    Remark = "公开课发布、预约、签到、评分评语及开课听课统计。",
                },
                new Areas.Admin.Entity.tbProgram()
                {
                    No = 14,
                    ProgramName = "资产管理",
                    ProgramCode = XkSystem.Code.EnumHelper.ProgramCode.Asset,
                    ProgramTitle = "资产管理",
                    IsWide = true,
                    BgIcon = "glyphicon glyphicon-transfer",
                    BgColor = "#78c82a",
                    Remark = "资产申请、领用、报废、维修，资产状态跟踪。",
                },
                new Areas.Admin.Entity.tbProgram()
                {
                    No = 15,
                    ProgramName = "资源管理",
                    ProgramCode = XkSystem.Code.EnumHelper.ProgramCode.Ware,
                    ProgramTitle = "资源管理",
                    IsWide = true,
                    BgIcon = "glyphicon glyphicon-duplicate",
                    BgColor = "#c15ed7",
                    Remark = "资源上传下载、教学资源管理、资源使用分析。",
                },
                new Areas.Admin.Entity.tbProgram()
                {
                    No = 16,
                    ProgramName = "移动智慧校园",
                    ProgramCode = XkSystem.Code.EnumHelper.ProgramCode.App,
                    ProgramTitle = "移动智慧校园",
                    IsWide = true,
                    BgIcon = "glyphicon glyphicon-phone",
                    BgColor = "#05bd94",
                    Remark = "消息公告、移动办公、移动教务、家校沟通。",
                }
            };
            var tbProgramList = db.Set<Areas.Admin.Entity.tbProgram>().ToList();
            foreach (var a in programList)
            {
                if (tbProgramList.Where(d => d.ProgramName == a.ProgramName && d.ProgramCode == a.ProgramCode).Any())
                {
                    foreach (var program in tbProgramList.Where(d => d.ProgramName == a.ProgramName))
                    {
                        program.No = a.No;
                        program.ProgramTitle = a.ProgramTitle;
                        program.IsWide = a.IsWide;
                        //program.Startup = a.Startup;
                        program.BgIcon = a.BgIcon;
                        program.BgColor = a.BgColor;
                        program.Remark = a.Remark;
                    }
                }
                else
                {
                    db.Set<Areas.Admin.Entity.tbProgram>().Add(a);
                }
            }

            #endregion

            #region tbTenant

            var tenant = new Areas.Admin.Entity.tbTenant();
            if (db.Set<Areas.Admin.Entity.tbTenant>().Where(d => d.IsDeleted == false).Count() == decimal.Zero)
            {
                tenant = new Areas.Admin.Entity.tbTenant()
                {
                    No = 0,
                    TenantName = "深圳龙创软件",
                    Title = "龙创软件",
                    IsDefault = true,
                    Host = "XkSystem",
                    Power = "*",
                    IsVip = 9,
                    Deadine = new DateTime(2099, 12, 30),
                };

                db.Set<Areas.Admin.Entity.tbTenant>().Add(tenant);

                db.SaveChanges();
            }
            else
            {
                tenant = db.Set<Areas.Admin.Entity.tbTenant>().Where(d => d.IsDeleted == false).OrderBy(d => d.No).FirstOrDefault();
            }

            #endregion

            db.SaveChanges();

            #region tbTenantProgram

            if (db.TableRoot<Areas.Admin.Entity.tbTenantProgram>().Count() == 0)
            {
                db.Set<Areas.Admin.Entity.tbTenantProgram>().AddOrUpdate(d => d.Id,
                    new Areas.Admin.Entity.tbTenantProgram()
                    {
                        No = 1,
                        tbTenant = tenant,
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().OrderBy(d => d.No).FirstOrDefault()
                    });
            }

            #endregion

            Seed(db, tenant);
        }

        public static void Seed(XkSystem.Models.DbContext db, Areas.Admin.Entity.tbTenant tenant, string adminLoginCode = "", string adminPassword = "")
        {
            #region tbSysMenu
            if (db.Set<Areas.Sys.Entity.tbSysMenu>().Count() == 0)
            {

                #region 一级菜单

                var sysMenuList1 = new List<Areas.Sys.Entity.tbSysMenu>() {
                    #region
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 1,
                    MenuName = "基本资料",
                    Icon="glyphicon glyphicon-book",
                    Remark="可设置学校、学年信息，教室、教师管理，年级、科研组长等操作。",
                    IsShortcut=true,
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = string.Empty,
                    tbMenuParent = null,
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 2,
                    MenuName = "班级学生",
                    Remark="班级设置、学生信息查询、学生荣誉、学生异动查询。",
                    Icon ="glyphicon glyphicon-user",
                    IsShortcut=true,
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = string.Empty,
                    tbMenuParent = null,
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 3,
                    MenuName = "教学管理",
                    Remark="教学班设置、课程模块设置、课表设置功能。",
                    Icon ="glyphicon glyphicon-th-large",
                    IsShortcut=true,
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = string.Empty,
                    tbMenuParent = null,
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 4,
                    MenuName = "选课管理",
                    IsShortcut=true,
                    Remark="选课设置、选课查看等功能，老师可以查看我的学生列表。",
                    Icon ="glyphicon glyphicon-th-large",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = string.Empty,
                    tbMenuParent = null,
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 5,
                    MenuName = "学习表现",
                    IsShortcut=true,
                    Remark="学生评价、考勤表现、学期评语等功能。",
                    Icon ="glyphicon glyphicon-tags",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = string.Empty,
                    tbMenuParent = null,
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 6,
                    MenuName = "考试成绩",
                    IsShortcut=true,
                    Remark="提供考生设置、成绩录入查询、成绩分析统计功能。",
                    Icon ="glyphicon glyphicon-stats",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = string.Empty,
                    tbMenuParent = null,
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 7,
                    MenuName = "教学评价",
                    IsShortcut=true,
                    Remark="教学评价设置和评价功能，以及查看评价结果。",
                    Icon ="glyphicon glyphicon-thumbs-up",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = string.Empty,
                    tbMenuParent = null,
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 8,
                    MenuName = "住宿管理",
                    IsDeleted=true,
                    IsShortcut=true,
                    Remark="宿舍设置、宿舍申请审批、宿舍分配等。",
                    Icon ="glyphicon glyphicon-th-large",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                    MenuUrl = string.Empty,
                    tbMenuParent = null,
                },
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant=tenant,
                //    No=11,
                //    MenuName="晚自习管理",
                //    IsShortcut=true,
                //    Remark="晚自习申请、审批、表现，统计等功能。",
                //    Icon ="glyphicon glyphicon-th-large",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                //    MenuUrl=string.Empty,
                //    tbMenuParent=null,
                //},
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant=tenant,
                //    No=13,
                //    MenuName="招生管理",
                //    IsShortcut=true,
                //    Remark="招生管理设置，发布面试结果，学生消息。",
                //    Icon ="glyphicon glyphicon-th-large",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //    MenuUrl=string.Empty,
                //    tbMenuParent=null,
                //},
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant=tenant,
                //    No=14,
                //    MenuName="在线考试",
                //    IsShortcut=true,
                //    Remark="在线考试设置、考试以及考试结果的一系列图表。",
                //    Icon ="glyphicon glyphicon-list-alt",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //    MenuUrl=string.Empty,
                //    tbMenuParent=null,
                //},
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant=tenant,
                    No=15,
                    MenuName="预约管理",
                    IsShortcut=true,
                    Remark="预约设置、预约申请、审批、审查，陈列预约统计结果。",
                    Icon ="glyphicon glyphicon-th-large",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                    MenuUrl=string.Empty,
                    tbMenuParent=null,
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No =16,
                    MenuName = "投票系统",
                    IsShortcut=true,
                    Remark="投票系统设置、发布，开始投票并显示投票统计结果。",
                    Icon ="glyphicon glyphicon-tag",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                    MenuUrl = string.Empty,
                    tbMenuParent = null,
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No =17,
                    MenuName = "综合素养",
                    IsShortcut=true,
                    Remark="班主任、教师、家长评语，学生、班主任素质报告单、班级、年级汇总。",
                    Icon ="glyphicon glyphicon-leaf",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = string.Empty,
                    tbMenuParent = null,
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No =18,
                    MenuName = "听课管理",
                    IsShortcut=true,
                    Remark="听课设置、申请、审批，听课结果查看。",
                    Icon ="glyphicon glyphicon-th-large",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                    MenuUrl = string.Empty,
                    tbMenuParent = null,
                }
                #endregion
                };
                var tbSysMenuList1 = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
                foreach (var v in sysMenuList1)
                {
                    if (tbSysMenuList1.Where(d => d.MenuName == v.MenuName && d.tbProgram.ProgramCode == v.tbProgram.ProgramCode).Any())
                    {
                        //更新
                    }
                    else
                    {
                        db.Set<Areas.Sys.Entity.tbSysMenu>().Add(v);
                    }
                }

                #endregion

                db.SaveChanges();

                #region 二级菜单

                var sysMenuList2 = new List<Areas.Sys.Entity.tbSysMenu>() {
                #region

                #region 基本资料
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 1,
                    MenuName = "学校信息",
                    IsShortcut=true,
                    Remark="设置并陈列显示学校的详细信息。",
                    Icon ="glyphicon glyphicon-list",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Basis/School/Edit",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "基本资料" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 2,
                    MenuName = "学年设置",
                    IsShortcut=true,
                    Remark="设置管理当前学年、学期、学段。",
                    Icon ="glyphicon glyphicon-star",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Basis/Year/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "基本资料" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 3,
                    MenuName = "教室管理",
                    IsShortcut=true,
                    Remark="设置学校教室，并提供批量导入功能。",
                    Icon ="glyphicon glyphicon-th-large",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Basis/Room/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "基本资料" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 4,
                    MenuName = "教师管理",
                    IsShortcut=true,
                    Remark="设置学校教师，并提供批量导入功能。",
                    Icon ="glyphicon glyphicon-th-large",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Teacher/Teacher/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "基本资料" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 5,
                    MenuName = "年级组长",
                    IsShortcut=true,
                    Remark="设置学校的年级组长。",
                    Icon ="glyphicon glyphicon-user",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Teacher/TeacherGrade/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "基本资料" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 6,
                    MenuName = "科研组长",
                    IsShortcut=true,
                    Remark="设置学校的科研组长。",
                    Icon ="glyphicon glyphicon-user",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Teacher/TeacherSubject/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "基本资料" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 9,
                    MenuName = "校历设置",
                    IsShortcut=true,
                    Remark="设置学校校历。",
                    Icon ="glyphicon glyphicon-calendar",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Basis/Calendar/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "基本资料" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                #endregion

                #region 班级学生
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 1,
                    MenuName = "学生管理",
                    IsShortcut=true,
                    Remark="学生列表显示，添加学生资料、学生获奖、优生管理、批量导入。",
                    Icon ="glyphicon glyphicon-th-large",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Student/Student/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "班级学生" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 2,
                    MenuName = "学生资料",
                    IsShortcut=true,
                    Remark="显示详细的学生资料，并提供修改。",
                    Icon ="glyphicon glyphicon-user",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Student/Student/ViewList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "班级学生" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 3,
                    MenuName = "资料填写",
                    IsShortcut=true,
                    Remark="学生填写自己资料页面。",
                    Icon ="glyphicon glyphicon-edit",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Student/Student/EditStudent",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "班级学生" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 4,
                    MenuName = "班级管理",
                    IsShortcut=true,
                    Remark="提供班级管理、设置，并提供批量导入和自动分班。",
                    Icon ="glyphicon glyphicon-th-large",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Basis/Class/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "班级学生" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 5,
                    MenuName = "学生名单",
                    IsShortcut=true,
                    Remark="列表显示学生名单、学生详细信息。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Basis/ClassStudent/ListByTeacher",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "班级学生" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 6,
                    MenuName = "学生异动",
                    IsShortcut=true,
                    Remark="学生异动列表、学生异动设置、异动录入。",
                    Icon ="glyphicon glyphicon-share",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Student/StudentChange/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "班级学生" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 7,
                    MenuName = "学生荣誉",
                    IsShortcut=true,
                    Remark="学生荣誉列表、学生荣誉设置、荣誉录入。",
                    Icon ="glyphicon glyphicon-thumbs-up",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Student/StudentHonor/HonorList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "班级学生" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 8,
                    MenuName = "学生教学班",
                    IsShortcut=true,
                    Remark="班主任查询、管理学生所在教学班，以便班主任对学生的教学进行系统管理。",
                    Icon ="glyphicon glyphicon-thumbs-up",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Basis/Class/OrgStudentByClassList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "班级学生" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                #endregion

                #region 教学管理
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 1,
                    MenuName = "课程模块",
                    IsShortcut=true,
                    Remark="课程管理设置、课程录入、课程查询。",
                    Icon ="glyphicon glyphicon-th-large",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Course/Course/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 2,
                    MenuName = "课程信息",
                    Remark="课程的详细信息，课程管理设置。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Course/Course/InfoList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 3,
                    MenuName = "教学班",
                    IsShortcut=true,
                    Remark="教学班设置、教学班学生导入、教学班批量导入。",
                    Icon ="glyphicon glyphicon-book",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Course/Org/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 4,
                    MenuName = "课表管理",
                    Remark="课程表设置管理、课程表添加修改。",
                    Icon ="glyphicon glyphicon-gift",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Course/Schedule/Set",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 5,
                    MenuName = "课表查看",
                    IsShortcut=true,
                    Remark="根据学生、教师、年级组长、科组长角色查看课表。",
                    Icon ="glyphicon glyphicon-folder-open",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Course/Schedule/ClassAll",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 6,
                    MenuName = "我的课表",
                    IsShortcut=true,
                    Remark="根据学生、教师、年级组长、科组长角色查看课表。",
                    Icon ="glyphicon glyphicon-folder-open",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Course/Schedule/My",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                #endregion

                #region 选课管理
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 1,
                    MenuName = "选课设置",
                    Remark="选课开放设置。",
                    Icon ="glyphicon glyphicon-cog",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Elective/Elective/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "选课管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 2,
                    MenuName = "学生选课",
                    IsShortcut=true,
                    Remark="学生根据选课开放设置进行选课。",
                    Icon ="glyphicon glyphicon-tags",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Elective/ElectiveInput/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "选课管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 3,
                    MenuName = "选课调整",
                    Remark="查看并调整选课结果。",
                    Icon ="glyphicon glyphicon-edit",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Elective/ElectiveChange/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "选课管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 4,
                    MenuName = "选课统计",
                    IsShortcut=true,
                    Remark="对选课结果进行统计，生成统计报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Elective/ElectiveReport/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "选课管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 5,
                    MenuName = "我的学生",
                    IsShortcut=true,
                    Remark="教师可查看选择教师课程的学生名单。",
                    Icon ="glyphicon glyphicon-user",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Elective/ElectiveReport/Teacher",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "选课管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                #endregion
                
                #region 学习表现
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 1,
                    MenuName = "评价设置",
                    Remark="评价设置。",
                    Icon ="glyphicon glyphicon-cog",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Perform/Perform/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "学习表现" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 2,
                    MenuName = "学生评价",
                    IsShortcut=true,
                    Remark="学生进行评价。",
                    Icon ="glyphicon glyphicon-thumbs-up",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Perform/PerformData/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "学习表现" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 2,
                    MenuName = "日常表现",
                    IsShortcut=true,
                    Remark="学生日常表现。",
                    Icon ="glyphicon glyphicon-thumbs-up",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Perform/PerformDataDay/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "学习表现" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 3,
                    MenuName = "评价调整",
                    Remark="评价结果进行调整，评价结果调优。",
                    Icon ="glyphicon glyphicon-edit",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Perform/PerformChange/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "学习表现" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 4,
                    MenuName = "评价查看",
                    IsShortcut=true,
                    Remark="对评价结果进行处理，生成各种形式的图表。",
                    Icon ="glyphicon glyphicon-align-justify",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Perform/PerformData/PerformDataAll",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "学习表现" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 5,
                    MenuName = "课堂考勤",
                    IsShortcut=true,
                    Remark="课堂考勤设置，数据录入，并提供报表查询。",
                    Icon ="glyphicon glyphicon-check",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Attendance/Attendance/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "学习表现" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 6,
                    MenuName = "学期评语",
                    IsShortcut=true,
                    Remark="对学生本学期的评语，对下学期的期望。",
                    Icon ="glyphicon glyphicon-font",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Perform/PerformComment/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "学习表现" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 7,
                    MenuName = "考勤统计",
                    IsShortcut=true,
                    Remark="对时间段内的考勤数据进行统计分析，生成各考勤状态的报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Attendance/Attendance/AttendanceAll",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "学习表现" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 8,
                    MenuName = "评语统计",
                    IsShortcut=true,
                    Remark="对评语进行统计分析，生成各种报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Perform/PerformComment/ReportAll",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "学习表现" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 9,
                    MenuName = "我的考勤",
                    IsShortcut=true,
                    Remark="我的考勤记录，查询自己的出勤率。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Attendance/Attendance/My",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "学习表现" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                #endregion

                #region 考试成绩
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 1,
                    MenuName = "考试设置",
                    Remark="提供考试详细信息设置，发布考试功能。",
                    Icon ="glyphicon glyphicon-cog",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Exam/Exam/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "考试成绩" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 2,
                    MenuName = "成绩录入",
                    IsShortcut=true,
                    Remark="录入考试成绩，并提供修改调优功能。",
                    Icon ="glyphicon glyphicon-edit",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Exam/ExamMark/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "考试成绩" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 3,
                    MenuName = "成绩调整",
                    IsShortcut=true,
                    Remark="对已有成绩数据进行调整、调优。",
                    Icon ="glyphicon glyphicon-check",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Exam/ExamChange/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "考试成绩" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 4,
                    MenuName = "成绩报表",
                    IsShortcut=true,
                    Remark="根据已有成绩数据生成各种报表图表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Exam/ExamReport/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "考试成绩" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 5,
                    MenuName = "成绩汇总",
                    IsShortcut=true,
                    Remark="根据已有成绩数据进行汇总分析，生成各种图表。",
                    Icon ="glyphicon glyphicon-list",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Exam/ExamReport/ReportOrgTeacher",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "考试成绩" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 6,
                    MenuName = "分数段设置",
                    Remark="设置多个分数段，有助于成绩分析。",
                    Icon ="glyphicon glyphicon-cog",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.ExamAnalyze).FirstOrDefault(),
                    MenuUrl = "Exam/ExamSegmentMark/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "考试成绩" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.ExamAnalyze).FirstOrDefault(),
                },
                 new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 7,
                    MenuName = "横向分析",
                    IsShortcut=true,
                    Remark="对考试成绩进行横向分析。",
                     Icon ="glyphicon glyphicon-indent-left",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.ExamAnalyze).FirstOrDefault(),
                    MenuUrl = "Exam/ExamAnalyze/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "考试成绩" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.ExamAnalyze).FirstOrDefault(),
                },
                 new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 8,
                    MenuName = "纵向分析",
                    IsShortcut=true,
                    Remark="对考试成绩进行纵向分析。",
                     Icon ="glyphicon glyphicon-indent-right",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.ExamAnalyze).FirstOrDefault(),
                    MenuUrl = "Exam/ExamPortrait/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "考试成绩" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.ExamAnalyze).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 9,
                    MenuName = "成绩查询",
                    IsShortcut=true,
                    Remark="根据各种条件查询筛选成绩数据，呈现表格。",
                    Icon ="glyphicon glyphicon-list",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.ExamAnalyze).FirstOrDefault(),
                    MenuUrl = "Exam/ExamReport/SearchMarlList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "考试成绩" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 10,
                    MenuName = "查总成绩",
                    IsShortcut=true,
                    Remark="根据各种条件查询筛选查总成绩数据，呈现表格。",
                    Icon ="glyphicon glyphicon-sort-by-attributes-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.ExamAnalyze).FirstOrDefault(),
                    MenuUrl = "Exam/ExamReport/StudentTotalList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "考试成绩" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 11,
                    MenuName = "成绩分析",
                    Remark="对考试成绩进行分析，获取分析结果。",
                    Icon ="glyphicon glyphicon-refresh",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.ExamAnalyze).FirstOrDefault(),
                    MenuUrl = "Exam/ExamAnalyzeLw/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "考试成绩" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.ExamAnalyze).FirstOrDefault(),
                },
                #endregion

                #region 教学评价
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 1,
                    MenuName = "评教设置",
                    Remark="对评教信息进行设置管理，并设置评教参数。",
                    Icon ="glyphicon glyphicon-cog",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/Survey/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 2,
                    MenuName = "评价录入",
                    IsShortcut=true,
                    Remark="录入评教内容，并提供查询功能。",
                    Icon ="glyphicon glyphicon-edit",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyData/Input",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 3,
                    MenuName = "评价统计",
                    IsShortcut=true,
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/Subject",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 4,
                    MenuName = "班级科目统计",
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/SubjectOrg",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 5,
                    MenuName = "任课教师统计",
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/TeacherOrg",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 6,
                    MenuName = "年级班级统计",
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/GradeOrg",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 7,
                    MenuName = "年级均分汇总",
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/GradeClass",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 8,
                    MenuName = "年级科目统计",
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/GradeSubject",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 9,
                    MenuName = "年级教师分数",
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/GradeTeacher",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 10,
                    MenuName = "年级教师排名",
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/GradeTeacherRanking",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 11,
                    MenuName = "课程教师统计",
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    IsShortcut=true,
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/SubjectList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 12,
                    MenuName = "课程满意统计",
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    IsShortcut=true,
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/SubjectTotalList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 13,
                    MenuName = "我的评价统计",
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    IsShortcut=true,
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/SubjectTeacherList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },                
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 14,
                    MenuName = "班主任未评统计",
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    IsShortcut=true,
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/UnClassTeacherList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 15,
                    MenuName = "班主任满意统计",
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    IsShortcut=true,
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/ClassTeacherList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 16,
                    MenuName = "任课教师统计通用",
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/SubjectCourseList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 17,
                    MenuName = "任课教师统计纵向",
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/SubjectCourseListHor",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 18,
                    MenuName = "任课教师未评统计",
                    Remark="对已有评教信息进行条件查询统计，生成报表。",
                    Icon ="glyphicon glyphicon-list-alt",
                    IsShortcut=true,
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    MenuUrl = "Survey/SurveyReport/UnSubjectList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "教学评价" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                #endregion

                #region 资产管理
                new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 1,
                     MenuName = "资产列表",
                    IsShortcut=true,
                    Remark="对已有资产数据进行条件查询，生成数据表格。",
                    Icon ="glyphicon glyphicon-list",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Asset).FirstOrDefault(),
                     MenuUrl = "Asset/Asset/List",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "资产管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 2,
                    MenuName = "资产报修",
                    IsShortcut=true,
                    Remark="资产报修通道，对资产进行报修。",
                    Icon ="glyphicon glyphicon-wrench",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Asset).FirstOrDefault(),
                    MenuUrl = "Asset/AssetRepair/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "资产管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 3,
                    MenuName = "资产维护",
                    IsShortcut=true,
                    Remark="提交资产维护信息，对响应资产进行维护。",
                    Icon ="glyphicon glyphicon-cutlery",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Asset).FirstOrDefault(),
                    MenuUrl = "Asset/AssetService/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "资产管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 4,
                    MenuName = "资产申请",
                    IsShortcut=true,
                    Remark="提交资产申请，申请某资产的应用权限。",
                    Icon ="glyphicon glyphicon-export",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Asset).FirstOrDefault(),
                    MenuUrl = "Asset/AssetApply/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "资产管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 5,
                    MenuName = "资产审批",
                    IsShortcut=true,
                    Remark="资产管理员对资产申请进行审批。",
                    Icon ="glyphicon glyphicon-floppy-saved",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Asset).FirstOrDefault(),
                    MenuUrl = "Asset/AssetApply/ApproveList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "资产管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 6,
                    MenuName = "资产领用",
                    IsShortcut=true,
                    Remark="资产管理员根据审批结果发放资产给申请人。",
                    Icon ="glyphicon glyphicon-saved",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Asset).FirstOrDefault(),
                    MenuUrl = "Asset/AssetOut/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "资产管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 7,
                    MenuName = "我的资产",
                    IsShortcut=true,
                    Remark="资产领用者查看自己领用的资产详细信息。",
                    Icon ="glyphicon glyphicon-shopping-cart",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Asset).FirstOrDefault(),
                    MenuUrl = "Asset/AssetOut/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "资产管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 8,
                    MenuName = "消耗品入库",
                    IsShortcut=true,
                    Remark="采购员采购消耗品并存入库存。",
                    Icon ="glyphicon glyphicon-import",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Asset).FirstOrDefault(),
                    MenuUrl = "Asset/AssetIn/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "资产管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                },
                #endregion

                #region 德育管理
                 new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 1,
                     MenuName = "德育设置",
                    Remark="对德育系统进行设置，并设置详细参数。",
                     Icon ="glyphicon glyphicon-cog",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/Moral/List",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                 new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 3,
                     MenuName = "德育录入",
                    IsShortcut=true,
                    Remark="录入德育数据。",
                     Icon ="glyphicon glyphicon-edit",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/MoralData/Edit",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                 new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 4,
                     MenuName = "德育审核",
                    IsShortcut=true,
                    Remark="对已有德育数据进行审核。",
                     Icon ="glyphicon glyphicon-list-alt",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/MoralData/Check",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                 new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 5,
                     MenuName = "德育统计",
                    IsShortcut=true,
                    Remark="对已有德育数据进行统计分析，生成各种报表。",
                     Icon ="glyphicon glyphicon-list-alt",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/MoralStat/List",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                 new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 6,
                     MenuName = "班主任评语",
                    IsShortcut=true,
                    Remark="班主任每一个月录入学生评语。",
                     Icon ="glyphicon glyphicon-edit",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/MoralComment/MoralClassList",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                 new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 7,
                     MenuName = "我的心得",
                    IsShortcut=true,
                    Remark="输入学生自己每周的心得。",
                     Icon ="glyphicon glyphicon-edit",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/MoralComment/MoralStudentList",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                  new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 8,
                     MenuName = "学生心得",
                    IsShortcut=true,
                    Remark="查看学生每周输入的心得。",
                     Icon ="glyphicon glyphicon-edit",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/MoralComment/MoralTeacherList",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                   new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 9,
                     MenuName = "家长评语",
                    IsShortcut=true,
                    Remark="输入自己孩子的评语。",
                     Icon ="glyphicon glyphicon-edit",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/MoralComment/MoralFamilyList",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                   new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 10,
                     MenuName = "孩子家中情况",
                    IsShortcut=true,
                    Remark="输入孩子每天在家里的情况。",
                     Icon ="glyphicon glyphicon-edit",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/MoralComment/MoralHappeningList",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                    new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 11,
                     MenuName = "家长意见与建议",
                    IsShortcut=true,
                    Remark="输入家长对学校的意见与建议。",
                     Icon ="glyphicon glyphicon-edit",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/MoralComment/MoralSuggestList",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                    new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 12,
                     MenuName = "学生家长评语",
                    IsShortcut=true,
                    Remark="查看学生家长对孩子输入的评语。",
                     Icon ="glyphicon glyphicon-edit",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/MoralComment/FamilyTeacherList",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                    new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 13,
                     MenuName = "学生家中情况",
                    IsShortcut=true,
                    Remark="查看学生家长对孩子输入的家中情况。",
                     Icon ="glyphicon glyphicon-edit",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/MoralComment/HappeningTeacherList",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                    new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 14,
                     MenuName = "学生家长意见与建议",
                    IsShortcut=true,
                    Remark="查看学生家长输入的意见与建议。",
                     Icon ="glyphicon glyphicon-edit",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/MoralComment/SuggestTeacherList",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                 new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 15,
                     MenuName = "每月之星",
                     IsShortcut=true,
                     Remark="查看及设置班级每月之星。",
                     Icon ="glyphicon glyphicon-edit",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/MoralStat/Star",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                 new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 16,
                     MenuName = "流动红旗",
                     IsShortcut=true,
                     Remark="查看及设置班级流动红旗。",
                     Icon ="glyphicon glyphicon-flag",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/MoralRedFlag/List",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                 new Areas.Sys.Entity.tbSysMenu()
                 {
                     tbTenant = tenant,
                     No = 17,
                     MenuName = "德育报告",
                     IsShortcut=true,
                     Remark="查看及导出班级德育详细报告。",
                     Icon ="glyphicon glyphicon-list",
                     tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                     MenuUrl = "Moral/MoralReport/List",
                     tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "德育管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                 },
                 #endregion

                #region 综合素养
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 1,
                    MenuName = "素养设置",
                    Remark="综合素养设置，并配置其详细参数。",
                    Icon ="glyphicon glyphicon-cog",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = "Quality/Quality/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 2,
                    MenuName = "学生评价录入",
                    IsShortcut=true,
                    Remark="对学生评价进行录入存档。",
                    Icon ="glyphicon glyphicon-edit",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = "Quality/QualityData/Input",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 3,
                    MenuName = "班主任评价录入",
                    IsShortcut=true,
                    Remark="对班主任评价进行录入存档。",
                    Icon ="glyphicon glyphicon-edit",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = "Quality/QualityData/ClassInput",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 4,
                    MenuName = "任课教师评价录入",
                    IsShortcut=true,
                    Remark="对任课老师评价进行录入存档。",
                    Icon ="glyphicon glyphicon-edit",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = "Quality/QualityData/OrgInput",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 5,
                    MenuName = "家长评价录入",
                    IsShortcut=true,
                    Remark="对家长评价进行录入存档。",
                    Icon ="glyphicon glyphicon-edit",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = "Quality/QualityData/FamilyInput",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 6,
                    MenuName = "我的评语",
                    IsShortcut=true,
                    Remark="用户查阅其他用户对自己的评语，自检。",
                    Icon ="glyphicon glyphicon-bookmark",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = "Quality/QualitySelf/Input",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 7,
                    MenuName = "学生评语记录",
                    IsShortcut=true,
                    Remark="查询学生评语的记录，推测学生的成长记录。",
                    Icon ="glyphicon glyphicon-align-left",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = "Quality/QualitySelf/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 8,
                    MenuName = "班主任评语",
                    IsShortcut=true,
                    Remark="查看班主任评语。",
                    Icon ="glyphicon glyphicon-align-left",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = "Quality/QualityComment/ClassList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                      new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 9,
                    MenuName = "任课教师评语",
                    IsShortcut=true,
                    Remark="查看任课教师评语。",
                    Icon ="glyphicon glyphicon-align-left",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl =  "Quality/QualityComment/OrgList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 10,
                    MenuName = "家长评语",
                    IsShortcut=true,
                    Remark="查看家长评语。",
                    Icon ="glyphicon glyphicon-align-left",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = "Quality/QualityComment/FamilyList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 11,
                    MenuName = "学生素质报告单",
                    IsShortcut=true,
                    Remark="根据已有学生素养数据生成学生素质报告单。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = "Quality/QualityReport/StudentReport",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 12,
                    MenuName = "班级汇总",
                    IsShortcut=true,
                    Remark="班级信息汇总，生成报表。",
                    Icon ="glyphicon glyphicon-list",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = "Quality/QualityReport/ClassReport",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 13,
                    MenuName = "年级汇总",
                    IsShortcut=true,
                    Remark="年级信息汇总，生成报表。",
                    Icon ="glyphicon glyphicon-list",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = "Quality/QualityReport/GradeReport",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 14,
                    MenuName = "班主任素质报告单",
                    IsShortcut=true,
                    Remark="根据学生对班主任的评价生成班主任素质报告单。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = "Quality/QualityReport/ClassList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 15,
                    MenuName = "孩子评语",
                    IsShortcut=true,
                    Remark="查看孩子的自评、学期期待、学期总结信息",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = "Quality/QualityComment/ChildComment",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 16,
                    MenuName = "孩子综合素质报告单",
                    IsShortcut=true,
                    Remark="根据家长对家长和教师的评价生成综合素质报告单。",
                    Icon ="glyphicon glyphicon-list-alt",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    MenuUrl = "Quality/QualityReport/ChildReport",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "综合素养" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                },
                #endregion

                #region 宿舍管理
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 1,
                    MenuName = "宿舍管理",
                    IsDeleted=true,
                    IsShortcut=true,
                    Remark="对宿舍进行管理、添加修改，管理宿舍的参数设置，并显示宿舍信息。",
                    Icon ="glyphicon glyphicon-th-large",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                    MenuUrl = "Dorm/Dorm/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "住宿管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 1,
                    MenuName = "教管管理",
                    IsDeleted=true,
                    IsShortcut=true,
                    Remark="对宿舍教管进行设置、管理，并显示详细信息。",
                    Icon ="glyphicon glyphicon-th-large",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                    MenuUrl = "Dorm/DormTeacher/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "住宿管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 3,
                    MenuName = "申请审批",
                    IsDeleted=true,
                    IsShortcut=true,
                    Remark="学生申请宿舍，管理员进行审批申请。",
                    Icon ="glyphicon glyphicon-saved",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                    MenuUrl = "Dorm/Dorm/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "住宿管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 4,
                    MenuName = "住宿安排",
                    IsDeleted=true,
                    IsShortcut=true,
                    Remark="管理员审批通过后，安排学生入住相应宿舍。",
                    Icon ="glyphicon glyphicon-home",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                    MenuUrl = "Dorm/DormStudent/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "住宿管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 5,
                    MenuName = "住宿表现",
                    IsDeleted=true,
                    IsShortcut=true,
                    Remark="管理员对学生的住宿表现进行设置、查看。",
                    Icon ="glyphicon glyphicon-tags",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                    MenuUrl = "Dorm/DormData/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "住宿管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                },
                #endregion

                #region 晚自习管理
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant = tenant,
                //    No = 1,
                //    MenuName = "晚自习设置",
                //    Remark="管理晚自习设置，并管理晚自习参数设置。",
                //    Icon ="glyphicon glyphicon-cog",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                //    MenuUrl = "Study/Study/List",
                //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "晚自习管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                //},
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant = tenant,
                //    No = 2,
                //    MenuName = "晚自习申请",
                //    IsShortcut=true,
                //    Remark="学生查看已发布的晚自习，并申请晚自习。",
                //    Icon ="glyphicon glyphicon-edit",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                //    MenuUrl = "Study/StudyApply/List",
                //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "晚自习管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                //},
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant = tenant,
                //    No = 3,
                //    MenuName = "晚自习审批",
                //    IsShortcut=true,
                //    Remark="管理员查看晚自习申请，并进行审批。",
                //    Icon ="glyphicon glyphicon-check",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                //    MenuUrl = "Study/StudyApply/CheckList",
                //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "晚自习管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                //},
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant = tenant,
                //    No = 4,
                //    MenuName = "晚自习表现",
                //    IsShortcut=true,
                //    Remark="添加修改学生的晚自习表现，并提供条件查询功能。",
                //    Icon ="glyphicon glyphicon-tags",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                //    MenuUrl = "Study/StudyData/List",
                //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "晚自习管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                //},
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant = tenant,
                //    No = 5,
                //    MenuName = "晚自习统计",
                //    IsShortcut=true,
                //    Remark="对已有晚自习数据进行统计分析，并生成各种报表。",
                //    Icon ="glyphicon glyphicon-list",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                //    MenuUrl = "Study/StudyReport/List",
                //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "晚自习管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                //},
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant = tenant,
                //    No = 6,
                //    MenuName = "晚自习参数",
                //    Remark="设置晚自习参数。",
                //    Icon ="glyphicon glyphicon-tasks",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                //    MenuUrl = "Study/StudyOption/List",
                //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "晚自习管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                //},
	            #endregion

                #region 在线考试
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant = tenant,
                //    No = 1,
                //    MenuName = "测试设置",
                //    Remark="发布在线考试，并对在线考试的参数进行设置。",
                //    Icon ="glyphicon glyphicon-cog",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //    MenuUrl = "Test/Test/List",
                //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "在线考试" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //},
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant = tenant,
                //    No = 2,
                //    MenuName = "在线测试",
                //    IsShortcut=true,
                //    Remark="用户进行在线测试。",
                //    Icon ="glyphicon glyphicon-edit",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //    MenuUrl = "Test/TestData/TestList",
                //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "在线考试" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //},
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant = tenant,
                //    No = 3,
                //    MenuName = "成绩统计",
                //    IsShortcut=true,
                //    Remark="系统对在线测试的结果进行统计分析，并生成各种报表。",
                //    Icon ="glyphicon glyphicon-list",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //    MenuUrl = "Test/TestResult/List",
                //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "在线考试" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //},
                #endregion

                #region 招生管理
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant = tenant,
                //    No = 1,
                //    MenuName = "招生设置",
                //    Remark="发布招生，并对招生的参数进行设置。",
                //    Icon ="glyphicon glyphicon-cog",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //    MenuUrl = "Admit/Admit/List",
                //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "招生管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //},
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant = tenant,
                //    No = 2,
                //    MenuName = "招生活动",
                //    IsShortcut=true,
                //    Remark="根据已发布的招生添加招生活动，并发布招生活动。",
                //    Icon ="glyphicon glyphicon-tags",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //    MenuUrl = "Admit/AdmitEvent/List",
                //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "招生管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //},

                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant = tenant,
                //    No = 3,
                //    MenuName = "招生官设置",
                //    Remark="对招生官进行认派和管理，并提供列表查询功能和招生官参数设置功能。",
                //    Icon ="glyphicon glyphicon-cog",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //    MenuUrl = "Admit/AdmitTeacher/List",
                //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "招生管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //},
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant = tenant,
                //    No = 4,
                //    MenuName = "新生列表",
                //    IsShortcut=true,
                //    Remark="导入或添加新生信息，并提供条件查询新生列表功能和导出功能。",
                //    Icon ="glyphicon glyphicon-list",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //    MenuUrl = "Admit/AdmitStudent/List",
                //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "招生管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //},
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant = tenant,
                //    No = 5,
                //    MenuName = "发布面试结果",
                //    IsShortcut=true,
                //    Remark="面试后，发布本次的面试结果，并提供查询功能。",
                //    Icon ="glyphicon glyphicon-align-left",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //    MenuUrl = "Admit/AdmitVerifyStudent/List",
                //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "招生管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //},
                //new Areas.Sys.Entity.tbSysMenu()
                //{
                //    tbTenant = tenant,
                //    No = 6,
                //    MenuName = "我的消息",
                //    IsShortcut=true,
                //    Remark="面试后，面试结果发送的面试学生的消息中心，学生查阅。",
                //    Icon ="glyphicon glyphicon-comment",
                //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //    MenuUrl = "Admit/AdmitMessage/PrivateMessageList",
                //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "招生管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                //},
                #endregion

                #region 投票系统
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 1,
                    MenuName = "投票设置",
                    Remark="发布投票，并管理投票参数。",
                    Icon ="glyphicon glyphicon-cog",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                    MenuUrl = "Vote/Vote/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "投票系统" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 2,
                    MenuName = "投票录入",
                    IsShortcut=true,
                    Remark="录入投票数据，并提供条件查询功能。",
                    Icon ="glyphicon glyphicon-edit",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                    MenuUrl = "Vote/VoteData/Input",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "投票系统" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 3,
                    MenuName = "投票统计",
                    IsShortcut=true,
                    Remark="根据已有投票数据，进行统计分析，并生成各种图表报表。",
                    Icon ="glyphicon glyphicon-list",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                    MenuUrl = "Vote/VoteData/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "投票系统" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                },
                #endregion

                #region 预约管理

                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 1,
                    MenuName = "预约设置",
                    Remark="发布预约，并管理预约详细参数设置。",
                    Icon ="glyphicon glyphicon-cog",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                    MenuUrl = "Reserve/Reserve/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "预约管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 2,
                    MenuName = "预约申请",
                    IsShortcut=true,
                    Remark="申请已发布的预约，并提供申请信息查询列表。",
                    Icon ="glyphicon glyphicon-edit",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                    MenuUrl = "Reserve/ReserveData/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "预约管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 3,
                    MenuName = "预约审批",
                    IsShortcut=true,
                    Remark="对预约进行审批，查询预约审批。",
                    Icon ="glyphicon glyphicon-check",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                    MenuUrl = "Reserve/ReserveData/ApprovalList",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "预约管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 4,
                    MenuName = "预约统计",
                    IsShortcut=true,
                    Remark="对已有预约数据进行统计，并生成各种报表。",
                    Icon ="glyphicon glyphicon-list",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                    MenuUrl = "Reserve/ReserveReport/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "预约管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                },

                #endregion

                #region 听课管理

                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 6,
                    MenuName = "我的开课",
                    IsShortcut=true,
                    Remark="教师查看、设置自己的开课数据。",
                    Icon ="glyphicon glyphicon-th-list",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                    MenuUrl = "Open/Open/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "听课管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 7,
                    MenuName = "我的听课",
                    IsShortcut=true,
                    Remark="学生查看并设置自己的听课数据。",
                    Icon ="glyphicon glyphicon-headphones",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                    MenuUrl = "Open/OpenListen/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "听课管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 8,
                    MenuName = "开课审批",
                    IsShortcut=true,
                    Remark="提供开课功能、对申请信息进行审批功能。",
                    Icon ="glyphicon glyphicon-check",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                    MenuUrl = "Open/OpenManagement/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "听课管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 9,
                    MenuName = "评分参数",
                    Remark="设置评分参数。",
                    Icon ="glyphicon glyphicon-tasks",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                    MenuUrl = "Open/OpenScore/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "听课管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 10,
                    MenuName = "听课预约",
                    IsShortcut=true,
                    Remark="查询听课预约数据，并申请听课预约。",
                    Icon ="glyphicon glyphicon-edit",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                    MenuUrl = "Open/OpenReserve/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "听课管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 11,
                    MenuName = "听课统计",
                    IsShortcut=true,
                    Remark="对已有听课数据进行统计分析，并生成各种报表。",
                    Icon ="glyphicon glyphicon-list",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                    MenuUrl = "Open/OpenReport/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "听课管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 12,
                    MenuName = "问卷管理",
                    IsShortcut=true,
                    Remark="管理问卷，并提供条件查询功能。",
                    Icon ="glyphicon glyphicon-th-large",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                    MenuUrl = "Open/OpenSurveySolution/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "听课管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 13,
                    MenuName = "问卷录入",
                    IsShortcut=true,
                    Remark="对问卷调查进行录入，并提供条件查询功能。",
                    Icon ="glyphicon glyphicon-edit",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                    MenuUrl = "Open/OpenSurveyData/Input",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "听课管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                },
                new Areas.Sys.Entity.tbSysMenu()
                {
                    tbTenant = tenant,
                    No = 14,
                    MenuName = "问卷统计",
                    IsShortcut=true,
                    Remark="根据已有问卷数据，进行统计分析，并生成报表。",
                    Icon ="glyphicon glyphicon-list",
                    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                    MenuUrl = "Open/OpenSurveyReport/List",
                    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "听课管理" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                },
                #endregion

                #endregion
                };
                var tbSysMenuList2 = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
                foreach (var v in sysMenuList2)
                {
                    if (tbSysMenuList2.Where(d => d.MenuName == v.MenuName && d.tbProgram.ProgramCode == v.tbProgram.ProgramCode).Any())
                    {
                        /*
                        foreach (var menu in tbSysMenuList2.Where(d => d.MenuName == v.MenuName && d.tbProgram.ProgramCode == v.tbProgram.ProgramCode))
                        {
                            menu.No = v.No;
                            menu.tbProgram = v.tbProgram;
                            menu.MenuUrl = v.MenuUrl;
                            menu.tbMenuParent = v.tbMenuParent;
                        }
                        */
                    }
                    else
                    {
                        db.Set<Areas.Sys.Entity.tbSysMenu>().Add(v);
                    }
                }

                #endregion

                db.SaveChanges();

                #region 系统设置(每个程序模块添加系统设置菜单)

                #region 一级菜单

                var settingMenuList = new List<Areas.Sys.Entity.tbSysMenu>() {
                    #region
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 99,
                        MenuName = "系统设置",
                        Remark="提供菜单管理、用户管理、权限角色、操作日志、基础配置、数据字典管理功能。",
                        Icon ="glyphicon glyphicon-cog",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                        MenuUrl = string.Empty,
                        tbMenuParent = null,
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 99,
                        MenuName = "系统设置",
                        Remark="提供菜单管理、用户管理、权限角色、操作日志、基础配置、数据字典管理功能。",
                        Icon ="glyphicon glyphicon-cog",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Desktop).FirstOrDefault(),
                        MenuUrl = string.Empty,
                        tbMenuParent = null,
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 99,
                         MenuName = "系统设置",
                        Remark="提供菜单管理、用户管理、权限角色、操作日志、基础配置、数据字典管理功能。",
                        Icon ="glyphicon glyphicon-cog",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Base).FirstOrDefault(),
                        MenuUrl = string.Empty,
                        tbMenuParent = null,
                    },
                    //new Areas.Sys.Entity.tbSysMenu()
                    //{
                    //    tbTenant = tenant,
                    //    No = 99,
                    //    MenuName = "系统设置",
                    //    Remark="提供菜单管理、用户管理、权限角色、操作日志、基础配置、数据字典管理功能。",
                    //    Icon ="glyphicon glyphicon-cog",
                    //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                    //    MenuUrl = string.Empty,
                    //    tbMenuParent = null,
                    //},
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 99,
                        MenuName = "系统设置",
                        Remark="提供菜单管理、用户管理、权限角色、操作日志、基础配置、数据字典管理功能。",
                        Icon ="glyphicon glyphicon-cog",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Portal).FirstOrDefault(),
                        MenuUrl = string.Empty,
                        tbMenuParent = null,
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 99,
                        MenuName = "系统设置",
                        Remark="提供菜单管理、用户管理、权限角色、操作日志、基础配置、数据字典管理功能。",
                        Icon ="glyphicon glyphicon-cog",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                        MenuUrl = string.Empty,
                        tbMenuParent = null,
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 99,
                        MenuName = "系统设置",
                        Remark="提供菜单管理、用户管理、权限角色、操作日志、基础配置、数据字典管理功能。",
                        Icon ="glyphicon glyphicon-cog",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Timetable).FirstOrDefault(),
                        MenuUrl = string.Empty,
                        tbMenuParent = null,
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 99,
                        MenuName = "系统设置",
                        Remark="提供菜单管理、用户管理、权限角色、操作日志、基础配置、数据字典管理功能。",
                        Icon ="glyphicon glyphicon-cog",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.ExamAnalyze).FirstOrDefault(),
                        MenuUrl = string.Empty,
                        tbMenuParent = null,
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 99,
                        MenuName = "系统设置",
                        Remark="提供菜单管理、用户管理、权限角色、操作日志、基础配置、数据字典管理功能。",
                        Icon ="glyphicon glyphicon-cog",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                        MenuUrl = string.Empty,
                        tbMenuParent = null,
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 99,
                        MenuName = "系统设置",
                        Remark="提供菜单管理、用户管理、权限角色、操作日志、基础配置、数据字典管理功能。",
                        Icon ="glyphicon glyphicon-cog",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                        MenuUrl = string.Empty,
                        tbMenuParent = null,
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 99,
                        MenuName = "系统设置",
                        Remark="提供菜单管理、用户管理、权限角色、操作日志、基础配置、数据字典管理功能。",
                        Icon ="glyphicon glyphicon-cog",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                        MenuUrl = string.Empty,
                        tbMenuParent = null,
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 99,
                        MenuName = "系统设置",
                        Remark="提供菜单管理、用户管理、权限角色、操作日志、基础配置、数据字典管理功能。",
                        Icon ="glyphicon glyphicon-cog",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Asset).FirstOrDefault(),
                        MenuUrl = string.Empty,
                        tbMenuParent = null,
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 99,
                        MenuName = "系统设置",
                        Remark="提供菜单管理、用户管理、权限角色、操作日志、基础配置、数据字典管理功能。",
                        Icon ="glyphicon glyphicon-cog",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Ware).FirstOrDefault(),
                        MenuUrl = string.Empty,
                        tbMenuParent = null,
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 99,
                        MenuName = "系统设置",
                        Remark="提供菜单管理、用户管理、权限角色、操作日志、基础配置、数据字典管理功能。",
                        Icon ="glyphicon glyphicon-cog",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.App).FirstOrDefault(),
                        MenuUrl = string.Empty,
                        tbMenuParent = null,
                    }
                    #endregion
                };
                var tbSettingMenuList = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
                foreach (var v in settingMenuList)
                {
                    if (tbSettingMenuList.Where(d => d.MenuName == v.MenuName && d.tbProgram.ProgramCode == v.tbProgram.ProgramCode).Any())
                    {
                        //更新
                    }
                    else
                    {
                        db.Set<Areas.Sys.Entity.tbSysMenu>().Add(v);
                    }
                }

                #endregion

                db.SaveChanges();

                #region 二级菜单

                var settingSonMenuList = new List<Areas.Sys.Entity.tbSysMenu>() {
                    #region
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 1,
                        MenuName = "菜单管理",
                        Remark="管理菜单，修改菜单显示状态、是否首页显示快捷菜单。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                        MenuUrl = "Sys/SysMenu/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 2,
                        MenuName = "用户管理",
                        Remark="管理系统用户，提供重置密码、解锁账号、启用禁用账号等功能。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                        MenuUrl = "Sys/SysUser/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 3,
                        MenuName = "权限角色",
                        Remark="管理系统权限角色，并提供按角色、按菜单、按用户权限授权功能。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                        MenuUrl = "Sys/SysRole/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 4,
                        MenuName = "操作日志",
                        Remark="管理系统的操作日志，提供导出功能、清空日志功能。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                        MenuUrl = "Sys/SysUserLog/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 5,
                        MenuName = "基础配置",
                        Remark="管理系统的年级、星期、节次等基础配置。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                        MenuUrl = "Basis/Grade/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 6,
                        MenuName = "数据字典",
                        Remark="提供性别、血型、健康状况、家庭关系、民族、政治面貌、区域、婚姻状态、户口类型、港澳台侨字典的管理功能。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                        MenuUrl = "Dict/DictSex/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 1,
                        MenuName = "审批权限角色",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.App).FirstOrDefault(),
                        MenuUrl = "Wechat/WeApprover/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.EAS).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 1,
                        MenuName = "菜单管理",
                        Remark="管理菜单，修改菜单显示状态、是否首页显示快捷菜单。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Desktop).FirstOrDefault(),
                        MenuUrl = "Sys/SysMenu/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Desktop).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 1,
                        MenuName = "菜单管理",
                        Remark="管理菜单，修改菜单显示状态、是否首页显示快捷菜单。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Base).FirstOrDefault(),
                        MenuUrl = "Sys/SysMenu/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Base).FirstOrDefault(),
                    },
                    //new Areas.Sys.Entity.tbSysMenu()
                    //{
                    //    tbTenant = tenant,
                    //    No = 1,
                    //    MenuName = "菜单管理",
                    //    Remark="管理菜单，修改菜单显示状态、是否首页显示快捷菜单。",
                    //    Icon ="glyphicon glyphicon-th-large",
                    //    tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                    //    MenuUrl = "Sys/SysMenu/List",
                    //    tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Admit).FirstOrDefault(),
                    //},
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 1,
                        MenuName = "菜单管理",
                        Remark="管理菜单，修改菜单显示状态、是否首页显示快捷菜单。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Portal).FirstOrDefault(),
                        MenuUrl = "Sys/SysMenu/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Portal).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 1,
                        MenuName = "菜单管理",
                        Remark="管理菜单，修改菜单显示状态、是否首页显示快捷菜单。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                        MenuUrl = "Sys/SysMenu/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.OA).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 1,
                        MenuName = "菜单管理",
                        Remark="管理菜单，修改菜单显示状态、是否首页显示快捷菜单。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Timetable).FirstOrDefault(),
                        MenuUrl = "Sys/SysMenu/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Timetable).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 1,
                        MenuName = "菜单管理",
                        Remark="管理菜单，修改菜单显示状态、是否首页显示快捷菜单。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.ExamAnalyze).FirstOrDefault(),
                        MenuUrl = "Sys/SysMenu/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.ExamAnalyze).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 1,
                        MenuName = "菜单管理",
                        Remark="管理菜单，修改菜单显示状态、是否首页显示快捷菜单。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                        MenuUrl = "Sys/SysMenu/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Moral).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 1,
                        MenuName = "菜单管理",
                        Remark="管理菜单，修改菜单显示状态、是否首页显示快捷菜单。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                        MenuUrl = "Sys/SysMenu/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Quality).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 1,
                        MenuName = "菜单管理",
                        Remark="管理菜单，修改菜单显示状态、是否首页显示快捷菜单。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                        MenuUrl = "Sys/SysMenu/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 2,
                        MenuName = "权限角色",
                        Remark="管理系统权限角色，并提供按角色、按菜单、按用户权限授权功能。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                        MenuUrl = "Sys/SysRole/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Open).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 1,
                        MenuName = "菜单管理",
                        Remark="管理菜单，修改菜单显示状态、是否首页显示快捷菜单。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Asset).FirstOrDefault(),
                        MenuUrl = "Sys/SysMenu/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Asset).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 1,
                        MenuName = "菜单管理",
                        Remark="管理菜单，修改菜单显示状态、是否首页显示快捷菜单。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Ware).FirstOrDefault(),
                        MenuUrl = "Sys/SysMenu/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.Ware).FirstOrDefault(),
                    },
                    new Areas.Sys.Entity.tbSysMenu()
                    {
                        tbTenant = tenant,
                        No = 1,
                        MenuName = "菜单管理",
                        Remark="管理菜单，修改菜单显示状态、是否首页显示快捷菜单。",
                        Icon ="glyphicon glyphicon-th-large",
                        tbProgram = db.Set<Areas.Admin.Entity.tbProgram>().Where(d => d.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.App).FirstOrDefault(),
                        MenuUrl = "Sys/SysMenu/List",
                        tbMenuParent = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id && d.MenuName == "系统设置" && d.tbProgram.ProgramCode == XkSystem.Code.EnumHelper.ProgramCode.App).FirstOrDefault(),
                    }
                #endregion
                };

                var tbSettingSonMenuList = db.Set<Areas.Sys.Entity.tbSysMenu>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
                foreach (var v in settingSonMenuList)
                {
                    if (tbSettingSonMenuList.Where(d => d.MenuName == v.MenuName && d.tbProgram.ProgramCode == v.tbProgram.ProgramCode).Any())
                    {
                        /*
                        foreach (var menu in tbSysMenuList2.Where(d => d.MenuName == v.MenuName && d.tbProgram.ProgramCode == v.tbProgram.ProgramCode))
                        {
                            menu.No = v.No;
                            menu.tbProgram = v.tbProgram;
                            menu.MenuUrl = v.MenuUrl;
                            menu.tbMenuParent = v.tbMenuParent;
                        }
                        */
                    }
                    else
                    {
                        db.Set<Areas.Sys.Entity.tbSysMenu>().Add(v);
                    }
                }

                #endregion

                db.SaveChanges();

                #endregion
            }

            #endregion

            #region tbDictSex

            var sexList = new List<Areas.Dict.Entity.tbDictSex>()
            {
                new Areas.Dict.Entity.tbDictSex() { tbTenant = tenant, No = 1, SexName = "男", },
                new Areas.Dict.Entity.tbDictSex() { tbTenant = tenant, No = 2, SexName = "女" }
            };
            var tbSexList = db.Set<Areas.Dict.Entity.tbDictSex>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var a in sexList)
            {
                if (tbSexList.Where(d => d.SexName == a.SexName).Any())
                {
                    //更新
                }
                else
                {
                    db.Set<Areas.Dict.Entity.tbDictSex>().Add(a);
                }
            }

            #endregion

            #region tbDictDegrees

            var dictDegreesList = new List<Areas.Dict.Entity.tbDictDegree>()
            {
                new Areas.Dict.Entity.tbDictDegree() { tbTenant = tenant, No = 1, DegreeName = "学士" },
                new Areas.Dict.Entity.tbDictDegree() { tbTenant = tenant, No = 2, DegreeName = "硕士" },
                new Areas.Dict.Entity.tbDictDegree() { tbTenant = tenant, No = 3, DegreeName = "博士" },
                new Areas.Dict.Entity.tbDictDegree() { tbTenant = tenant, No = 9, DegreeName = "其他" }
            };
            var tbDictDegreeList = db.Set<Areas.Dict.Entity.tbDictDegree>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in dictDegreesList)
            {
                if (tbDictDegreeList.Where(d => d.DegreeName == v.DegreeName).Any())
                {
                    //更新
                }
                else
                {
                    db.Set<Areas.Dict.Entity.tbDictDegree>().Add(v);
                }
            }

            #endregion

            #region tbDictEducations
            var dictDegreesList1 = dictDegreesList.Union(tbDictDegreeList);
            var dictEducationsList = new List<Areas.Dict.Entity.tbDictEducation>()
            {
                #region
                new Areas.Dict.Entity.tbDictEducation()
                {
                    tbTenant = tenant,
                    No = 1,
                    EducationName = "小学及以下",
                    tbDictDegree = dictDegreesList1.Where(d=>d.DegreeName == "其他").FirstOrDefault()
                },
                new Areas.Dict.Entity.tbDictEducation()
                {
                    tbTenant = tenant,
                    No = 2,
                    EducationName = "初中",
                    tbDictDegree = dictDegreesList1.Where(d=>d.DegreeName == "其他").FirstOrDefault()
                },
                new Areas.Dict.Entity.tbDictEducation()
                {
                    tbTenant = tenant,
                    No = 3,
                    EducationName = "高中",
                    tbDictDegree = dictDegreesList1.Where(d=>d.DegreeName == "其他").FirstOrDefault()
                },
                new Areas.Dict.Entity.tbDictEducation()
                {
                    tbTenant = tenant,
                    No = 4,
                    EducationName = "中专",
                    tbDictDegree = dictDegreesList1.Where(d=>d.DegreeName == "其他").FirstOrDefault()
                },
                new Areas.Dict.Entity.tbDictEducation()
                {
                    tbTenant = tenant,
                    No = 5,
                    EducationName = "大专",
                    tbDictDegree = dictDegreesList1.Where(d=>d.DegreeName == "其他").FirstOrDefault()
                },
                new Areas.Dict.Entity.tbDictEducation()
                {
                    tbTenant = tenant,
                    No = 6,
                    EducationName = "本科",
                    tbDictDegree = dictDegreesList1.Where(d=>d.DegreeName == "学士").FirstOrDefault()
                },
                new Areas.Dict.Entity.tbDictEducation()
                {
                    tbTenant = tenant,
                    No = 7,
                    EducationName = "研究生",
                    tbDictDegree = dictDegreesList1.Where(d=>d.DegreeName == "硕士").FirstOrDefault()
                },
                new Areas.Dict.Entity.tbDictEducation()
                {
                    tbTenant = tenant,
                    No = 8,
                    EducationName = "博士",
                    tbDictDegree = dictDegreesList1.Where(d=>d.DegreeName == "博士").FirstOrDefault()
                },
                new Areas.Dict.Entity.tbDictEducation()
                {
                    tbTenant = tenant,
                    No = 9,
                    EducationName = "院士及以上",
                    tbDictDegree = dictDegreesList1.Where(d=>d.DegreeName == "其他").FirstOrDefault()
                }
                #endregion
            };
            var tbDictEducationList = db.Set<Areas.Dict.Entity.tbDictEducation>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in dictEducationsList)
            {
                if (tbDictEducationList.Where(d => d.EducationName == v.EducationName).Any())
                {
                    //更新
                }
                else
                {
                    db.Set<Areas.Dict.Entity.tbDictEducation>().Add(v);
                }
            }

            #endregion

            #region tbDictOverseas

            var overseasList = new List<Areas.Dict.Entity.tbDictOverseas>()
            {
                new Areas.Dict.Entity.tbDictOverseas() { tbTenant = tenant, No = 1, DictOverseasName = "香港同胞", },
                new Areas.Dict.Entity.tbDictOverseas() { tbTenant = tenant, No = 2, DictOverseasName = "香港同胞亲属", },
                new Areas.Dict.Entity.tbDictOverseas() { tbTenant = tenant, No = 3, DictOverseasName = "澳门同胞", },
                new Areas.Dict.Entity.tbDictOverseas() { tbTenant = tenant, No = 4, DictOverseasName = "澳门同胞亲属", },
                new Areas.Dict.Entity.tbDictOverseas() { tbTenant = tenant, No = 5, DictOverseasName = "台湾同胞", },
                new Areas.Dict.Entity.tbDictOverseas() { tbTenant = tenant, No = 6, DictOverseasName = "台湾同胞亲属", },
                new Areas.Dict.Entity.tbDictOverseas() { tbTenant = tenant, No = 11, DictOverseasName = "华侨", },
                new Areas.Dict.Entity.tbDictOverseas() { tbTenant = tenant, No = 12, DictOverseasName = "侨眷", },
                new Areas.Dict.Entity.tbDictOverseas() { tbTenant = tenant, No = 13, DictOverseasName = "归侨", },
                new Areas.Dict.Entity.tbDictOverseas() { tbTenant = tenant, No = 14, DictOverseasName = "归侨子女", },
                new Areas.Dict.Entity.tbDictOverseas() { tbTenant = tenant, No = 21, DictOverseasName = "归国留学人员", },
                new Areas.Dict.Entity.tbDictOverseas() { tbTenant = tenant, No = 31, DictOverseasName = "非华裔中国人", },
                new Areas.Dict.Entity.tbDictOverseas() { tbTenant = tenant, No = 41, DictOverseasName = "外籍华裔人", },
                new Areas.Dict.Entity.tbDictOverseas() { tbTenant = tenant, No = 51, DictOverseasName = "外国人", },
                new Areas.Dict.Entity.tbDictOverseas() { tbTenant = tenant, No = 99, DictOverseasName = "其他", }
            };
            var tbOverseasList = db.Set<Areas.Dict.Entity.tbDictOverseas>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var a in overseasList)
            {
                if (tbOverseasList.Where(d => d.DictOverseasName == a.DictOverseasName).Any())
                {
                    //更新
                }
                else
                {
                    db.Set<Areas.Dict.Entity.tbDictOverseas>().Add(a);
                }
            }

            #endregion

            #region tbDictMarriage

            var marriageList = new List<Areas.Dict.Entity.tbDictMarriage>()
            {
                new Areas.Dict.Entity.tbDictMarriage() { tbTenant = tenant, No = 10, MarriageName = "未婚", },
                new Areas.Dict.Entity.tbDictMarriage() { tbTenant = tenant, No = 20, MarriageName = "已婚" },
                new Areas.Dict.Entity.tbDictMarriage() { tbTenant = tenant, No = 90, MarriageName = "其他" }
            };
            var tbMarriageList = db.Set<Areas.Dict.Entity.tbDictMarriage>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var a in marriageList)
            {
                if (tbMarriageList.Where(d => d.MarriageName == a.MarriageName).Any())
                {
                    //更新
                }
                else
                {
                    db.Set<Areas.Dict.Entity.tbDictMarriage>().Add(a);
                }
            }

            #endregion

            #region tbPeriodType

            var periodTypeList = new List<Areas.Basis.Entity.tbPeriodType>()
            {
                new Areas.Basis.Entity.tbPeriodType() { tbTenant = tenant, No = 1, PeriodTypeName = "上午", Color = "#5bc0de" },
                new Areas.Basis.Entity.tbPeriodType() { tbTenant = tenant, No = 2, PeriodTypeName = "中午", Color = "#777777" },
                new Areas.Basis.Entity.tbPeriodType() { tbTenant = tenant, No = 3, PeriodTypeName = "下午", Color = "#f0ad4e" },
                new Areas.Basis.Entity.tbPeriodType() { tbTenant = tenant, No = 4, PeriodTypeName = "晚上", Color = "#777777" },
            };
            var tbPeriodTypeList = db.Set<Areas.Basis.Entity.tbPeriodType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in periodTypeList)
            {
                if (tbPeriodTypeList.Where(d => d.PeriodTypeName == v.PeriodTypeName).Any())
                { }
                else
                {
                    db.Set<Areas.Basis.Entity.tbPeriodType>().Add(v);
                }
            }

            #endregion

            #region tbTeacherType

            var staffList = new List<Areas.Teacher.Entity.tbTeacherType>()
            {
                new Areas.Teacher.Entity.tbTeacherType() { tbTenant = tenant, No = 10, TeacherTypeName = "教学类", },
                new Areas.Teacher.Entity.tbTeacherType() { tbTenant = tenant, No = 20, TeacherTypeName = "教辅类" },
                new Areas.Teacher.Entity.tbTeacherType() { tbTenant = tenant, No = 30, TeacherTypeName = "科研类", },
                new Areas.Teacher.Entity.tbTeacherType() { tbTenant = tenant, No = 40, TeacherTypeName = "行政类" },
                new Areas.Teacher.Entity.tbTeacherType() { tbTenant = tenant, No = 50, TeacherTypeName = "工人类", },
                new Areas.Teacher.Entity.tbTeacherType() { tbTenant = tenant, No = 60, TeacherTypeName = "校办企业类" },
                new Areas.Teacher.Entity.tbTeacherType() { tbTenant = tenant, No = 70, TeacherTypeName = "附设机构类", },
                new Areas.Teacher.Entity.tbTeacherType() { tbTenant = tenant, No = 99, TeacherTypeName = "其它类" }
            };
            var tbStaffList = db.Set<Areas.Teacher.Entity.tbTeacherType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var a in staffList)
            {
                if (tbStaffList.Where(d => d.TeacherTypeName == a.TeacherTypeName).Any())
                {
                    //更新
                }
                else
                {
                    db.Set<Areas.Teacher.Entity.tbTeacherType>().Add(a);
                }
            }

            #endregion

            #region tbDictPaperSize

            var dictPaperSizeList = new List<Areas.Dict.Entity.tbDictPaperSize>() {
                new Areas.Dict.Entity.tbDictPaperSize() { tbTenant = tenant, No = 1, PaperSizeName = "A4（纵向）", Height = 0, Width = 800, },
                new Areas.Dict.Entity.tbDictPaperSize() { tbTenant = tenant, No = 2, PaperSizeName = "A4（纵向）[窄边]", Height = 0, Width = 800, },
                new Areas.Dict.Entity.tbDictPaperSize() { tbTenant = tenant, No = 2, PaperSizeName = "A4（横向）", Height = 0, Width = 1000, },
                new Areas.Dict.Entity.tbDictPaperSize() { tbTenant = tenant, No = 3, PaperSizeName = "A4（横向）[窄边]", Height = 0, Width = 1000, },
                new Areas.Dict.Entity.tbDictPaperSize() { tbTenant = tenant, No = 4, PaperSizeName = "A3", Height = 0, Width = 1350, }
            };
            var tbDictPaperSizeList = db.Set<Areas.Dict.Entity.tbDictPaperSize>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in dictPaperSizeList)
            {
                if (tbDictPaperSizeList.Where(d => d.PaperSizeName == v.PaperSizeName).Any())
                { }
                else
                {
                    db.Set<Areas.Dict.Entity.tbDictPaperSize>().Add(v);
                }
            }

            #endregion

            #region tbDictBlood

            var dictBloodList = new List<Areas.Dict.Entity.tbDictBlood>() {
                new Areas.Dict.Entity.tbDictBlood() { tbTenant = tenant, No = 1, BloodName = "A型", },
                new Areas.Dict.Entity.tbDictBlood() { tbTenant = tenant, No = 2, BloodName = "B型", },
                new Areas.Dict.Entity.tbDictBlood() { tbTenant = tenant, No = 3, BloodName = "AB型", },
                new Areas.Dict.Entity.tbDictBlood() { tbTenant = tenant, No = 4, BloodName = "O型", },
                new Areas.Dict.Entity.tbDictBlood() { tbTenant = tenant, No = 5, BloodName = "其他", }
            };
            var tbDictBloodList = db.Set<Areas.Dict.Entity.tbDictBlood>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in dictBloodList)
            {
                if (tbDictBloodList.Where(d => d.BloodName == v.BloodName).Any())
                { }
                else
                {
                    db.Set<Areas.Dict.Entity.tbDictBlood>().Add(v);
                }
            }

            #endregion

            #region tbDictHealth

            var dictHealthList = new List<Areas.Dict.Entity.tbDictHealth>() {
                 new Areas.Dict.Entity.tbDictHealth() { tbTenant = tenant, No = 1, HealthName = "健康", },
                 new Areas.Dict.Entity.tbDictHealth() { tbTenant = tenant, No = 2, HealthName = "一般", },
                 new Areas.Dict.Entity.tbDictHealth() { tbTenant = tenant, No = 3, HealthName = "有慢性病", },
                 new Areas.Dict.Entity.tbDictHealth() { tbTenant = tenant, No = 4, HealthName = "残疾", },
                 new Areas.Dict.Entity.tbDictHealth() { tbTenant = tenant, No = 5, HealthName = "其他", }
            };
            var tbDictHealthList = db.Set<Areas.Dict.Entity.tbDictHealth>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in dictHealthList)
            {
                if (tbDictHealthList.Where(d => d.HealthName == v.HealthName).Any())
                { }
                else
                {
                    db.Set<Areas.Dict.Entity.tbDictHealth>().Add(v);
                }
            }

            #endregion

            #region tbDictKinship

            var dictKinshipList = new List<Areas.Dict.Entity.tbDictKinship>() {
                new Areas.Dict.Entity.tbDictKinship() { tbTenant = tenant, No = 1, KinshipName = "父亲", },
                new Areas.Dict.Entity.tbDictKinship() { tbTenant = tenant, No = 2, KinshipName = "母亲", },
                new Areas.Dict.Entity.tbDictKinship() { tbTenant = tenant, No = 3, KinshipName = "其他", }
            };
            var tbDictKinshipList = db.Set<Areas.Dict.Entity.tbDictKinship>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in dictKinshipList)
            {
                if (tbDictKinshipList.Where(d => d.KinshipName == v.KinshipName).Any())
                { }
                else
                {
                    db.Set<Areas.Dict.Entity.tbDictKinship>().Add(v);
                }
            }

            #endregion

            #region tbDictMarriage

            var dictMarriage = new List<Areas.Dict.Entity.tbDictMarriage>()
            {
                new Areas.Dict.Entity.tbDictMarriage() { tbTenant = tenant, No = 1, MarriageName = "未婚" },
                new Areas.Dict.Entity.tbDictMarriage() { tbTenant = tenant, No = 2, MarriageName = "已婚" },
                new Areas.Dict.Entity.tbDictMarriage() { tbTenant = tenant, No = 9, MarriageName = "其他" }
            };
            var dictMarriageList = db.Set<Areas.Dict.Entity.tbDictMarriage>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in dictMarriageList)
            {
                if (dictMarriage.Where(d => d.MarriageName == v.MarriageName).Any())
                { }
                else
                {
                    db.Set<Areas.Dict.Entity.tbDictMarriage>().Add(v);
                }
            }

            #endregion

            #region tbDictNation

            var dictNationList = new List<Areas.Dict.Entity.tbDictNation>() {
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 1, NationName = "汉族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 2, NationName = "蒙古族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 3, NationName = "回族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 4, NationName = "藏族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 5, NationName = "维吾尔族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 6, NationName = "苗族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 7, NationName = "彝族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 8, NationName = "壮族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 9, NationName = "布依族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 10, NationName = "朝鲜族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 11, NationName = "满族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 12, NationName = "侗族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 13, NationName = "瑶族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 14, NationName = "白族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 15, NationName = "土家族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 16, NationName = "哈尼族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 17, NationName = "哈萨克族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 18, NationName = "傣族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 19, NationName = "黎族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 20, NationName = "傈僳族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 21, NationName = "佤族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 22, NationName = "畲族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 23, NationName = "高山族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 24, NationName = "拉祜族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 25, NationName = "水族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 26, NationName = "东乡族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 27, NationName = "纳西族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 28, NationName = "景颇族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 29, NationName = "柯尔克孜族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 30, NationName = "土族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 31, NationName = "达斡尔族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 32, NationName = "仫佬族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 33, NationName = "羌族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 34, NationName = "布朗族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 35, NationName = "撒拉族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 36, NationName = "毛南族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 37, NationName = "仡佬族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 38, NationName = "锡伯族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 39, NationName = "阿昌族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 40, NationName = "普米族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 41, NationName = "塔吉克族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 42, NationName = "怒族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 43, NationName = "乌孜别克族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 44, NationName = "俄罗斯族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 45, NationName = "鄂温克族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 46, NationName = "德昂族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 47, NationName = "保安族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 48, NationName = "裕固族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 49, NationName = "京族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 50, NationName = "塔塔尔族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 51, NationName = "独龙族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 52, NationName = "鄂伦春族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 53, NationName = "赫哲族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 54, NationName = "门巴族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 55, NationName = "珞巴族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 56, NationName = "基诺族", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 57, NationName = "其他", },
                new Areas.Dict.Entity.tbDictNation() { tbTenant = tenant, No = 58, NationName = "外国血统", }
            };
            var tbDictNationList = db.Set<Areas.Dict.Entity.tbDictNation>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in dictNationList)
            {
                if (tbDictNationList.Where(d => d.NationName == v.NationName).Any())
                { }
                else
                {
                    db.Set<Areas.Dict.Entity.tbDictNation>().Add(v);
                }
            }

            #endregion

            #region tbDictParty

            var dictPartyList = new List<Areas.Dict.Entity.tbDictParty>() {
                new Areas.Dict.Entity.tbDictParty() { tbTenant = tenant, No = 1, PartyName = "群众", },
                new Areas.Dict.Entity.tbDictParty() { tbTenant = tenant, No = 2, PartyName = "中共党员", },
                new Areas.Dict.Entity.tbDictParty() { tbTenant = tenant, No = 3, PartyName = "中共预备党员", },
                new Areas.Dict.Entity.tbDictParty() { tbTenant = tenant, No = 4, PartyName = "共青团员", },
                new Areas.Dict.Entity.tbDictParty() { tbTenant = tenant, No = 5, PartyName = "民革会员", },
                new Areas.Dict.Entity.tbDictParty() { tbTenant = tenant, No = 6, PartyName = "民盟盟员", },
                new Areas.Dict.Entity.tbDictParty() { tbTenant = tenant, No = 7, PartyName = "民建会员", },
                new Areas.Dict.Entity.tbDictParty() { tbTenant = tenant, No = 8, PartyName = "民进会员", },
                new Areas.Dict.Entity.tbDictParty() { tbTenant = tenant, No = 9, PartyName = "农工党党员", },
                new Areas.Dict.Entity.tbDictParty() { tbTenant = tenant, No = 10, PartyName = "致公党党员", },
                new Areas.Dict.Entity.tbDictParty() { tbTenant = tenant, No = 11, PartyName = "九三学社社员", },
                new Areas.Dict.Entity.tbDictParty() { tbTenant = tenant, No = 12, PartyName = "台盟盟员", },
                new Areas.Dict.Entity.tbDictParty() { tbTenant = tenant, No = 13, PartyName = "无党派人士", }
            };
            var tbDictPartyList = db.Set<Areas.Dict.Entity.tbDictParty>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in dictPartyList)
            {
                if (tbDictPartyList.Where(d => d.PartyName == v.PartyName).Any())
                { }
                else
                {
                    db.Set<Areas.Dict.Entity.tbDictParty>().Add(v);
                }
            }

            #endregion

            #region tbDictRegion

            //var dictRegionList = new List<Areas.Dict.Entity.tbDictRegion>() {
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 1, RegionName = "北京市", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 2, RegionName = "天津市", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 3, RegionName = "河北省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 4, RegionName = "山西省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 5, RegionName = "内蒙古自治区", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 6, RegionName = "辽宁省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 7, RegionName = "吉林省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 8, RegionName = "黑龙江省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 9, RegionName = "上海市", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 10, RegionName = "江苏省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 11, RegionName = "浙江省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 12, RegionName = "安徽省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 13, RegionName = "福建省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 14, RegionName = "江西省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 15, RegionName = "山东省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 16, RegionName = "河南省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 17, RegionName = "湖北省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 18, RegionName = "湖南省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 19, RegionName = "广东省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 20, RegionName = "广西壮族自治区", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 21, RegionName = "海南省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 22, RegionName = "重庆市", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 23, RegionName = "四川省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 24, RegionName = "贵州省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 25, RegionName = "云南省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 26, RegionName = "西藏自治区", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 27, RegionName = "陕西省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 28, RegionName = "甘肃省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 29, RegionName = "青海省", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 30, RegionName = "宁夏回族自治区", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 31, RegionName = "新疆维吾尔自治区", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 32, RegionName = "香港特别行政区", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 33, RegionName = "澳门特别行政区", },
            //    new Areas.Dict.Entity.tbDictRegion() { tbTenant = tenant, No = 34, RegionName = "台湾省", }
            //};
            //var tbDictRegionList = db.Set<Areas.Dict.Entity.tbDictRegion>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            //foreach (var v in dictRegionList)
            //{
            //    if (tbDictRegionList.Where(d => d.RegionName == v.RegionName).Any())
            //    { }
            //    else
            //    {
            //        db.Set<Areas.Dict.Entity.tbDictRegion>().Add(v);
            //    }
            //}

            #endregion

            #region tbClassType

            var classTypeList = new List<Areas.Basis.Entity.tbClassType>() {
                new Areas.Basis.Entity.tbClassType() { No = 1, tbTenant = tenant, ClassTypeName = "普通班", },
                new Areas.Basis.Entity.tbClassType() { No = 2, tbTenant = tenant, ClassTypeName = "重点班", }
            };
            var tbClassTypeList = db.Set<Areas.Basis.Entity.tbClassType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in classTypeList)
            {
                if (tbClassTypeList.Where(d => d.ClassTypeName == v.ClassTypeName).Any())
                { }
                else
                {
                    db.Set<Areas.Basis.Entity.tbClassType>().Add(v);
                }
            }

            #endregion

            #region tbBuild

            var BuildList = new List<Areas.Basis.Entity.tbBuild>() {
                 new Areas.Basis.Entity.tbBuild() { No = 1, tbTenant = tenant, BuildName = "教学楼" }
            };
            var tbBuildList = db.Set<Areas.Basis.Entity.tbBuild>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in BuildList)
            {
                if (tbBuildList.Where(d => d.BuildName == v.BuildName).Any())
                { }
                else
                {
                    db.Set<Areas.Basis.Entity.tbBuild>().Add(v);
                }
            }

            #endregion

            #region tbRoomType

            var roomTypeList = new List<Areas.Basis.Entity.tbRoomType>() {
                new Areas.Basis.Entity.tbRoomType() { No = 1, tbTenant = tenant, RoomTypeName = "普通教室" },
                new Areas.Basis.Entity.tbRoomType() { No = 2, tbTenant = tenant, RoomTypeName = "多媒体教室" },
                new Areas.Basis.Entity.tbRoomType() { No = 3, tbTenant = tenant, RoomTypeName = "语音室" },
                new Areas.Basis.Entity.tbRoomType() { No = 4, tbTenant = tenant, RoomTypeName = "实验室" },
                new Areas.Basis.Entity.tbRoomType() { No = 5, tbTenant = tenant, RoomTypeName = "计算机房" },
                new Areas.Basis.Entity.tbRoomType() { No = 6, tbTenant = tenant, RoomTypeName = "专用教室" },
                new Areas.Basis.Entity.tbRoomType() { No = 9, tbTenant = tenant, RoomTypeName = "其他" }
            };
            var tbRoomTypeList = db.Set<Areas.Basis.Entity.tbRoomType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in roomTypeList)
            {
                if (tbRoomTypeList.Where(d => d.RoomTypeName == v.RoomTypeName).Any())
                { }
                else
                {
                    db.Set<Areas.Basis.Entity.tbRoomType>().Add(v);
                }
            }

            #endregion

            #region tbCourseType

            var courseTypeList = new List<Areas.Course.Entity.tbCourseType>() {
                new Areas.Course.Entity.tbCourseType() { tbTenant = tenant, No = 1, CourseTypeName = "必修" },
                new Areas.Course.Entity.tbCourseType() { tbTenant = tenant, No = 2, CourseTypeName = "选修I" },
                new Areas.Course.Entity.tbCourseType() { tbTenant = tenant, No = 3, CourseTypeName = "选修II" },
                new Areas.Course.Entity.tbCourseType() { tbTenant = tenant, No = 4, CourseTypeName = "校本" },
                new Areas.Course.Entity.tbCourseType() { tbTenant = tenant, No = 5, CourseTypeName = "其他" }
            };
            var tbCourseTypeList = db.Set<Areas.Course.Entity.tbCourseType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in courseTypeList)
            {
                if (tbCourseTypeList.Where(d => d.CourseTypeName == v.CourseTypeName).Any())
                { }
                else
                {
                    db.Set<Areas.Course.Entity.tbCourseType>().Add(v);
                }
            }

            #endregion

            #region tbElectiveType

            var electiveTypeList = new List<Areas.Elective.Entity.tbElectiveType>() {
                new Areas.Elective.Entity.tbElectiveType() { tbTenant = tenant, No = 1, ElectiveTypeName = "列表选课", ElectiveTypeCode = XkSystem.Code.EnumHelper.ElectiveType.List, Remark = "列表显示进行选课" },
                new Areas.Elective.Entity.tbElectiveType() { tbTenant = tenant, No = 2, ElectiveTypeName = "课表选课", ElectiveTypeCode = XkSystem.Code.EnumHelper.ElectiveType.WeekPeriod, Remark = "按课表进行选课" }
            };
            var tbElectiveTypeList = db.Set<Areas.Elective.Entity.tbElectiveType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in electiveTypeList)
            {
                if (tbElectiveTypeList.Where(d => d.ElectiveTypeName == v.ElectiveTypeName).Any())
                { }
                else
                {
                    db.Set<Areas.Elective.Entity.tbElectiveType>().Add(v);
                }
            }

            #endregion

            #region tbWeek

            var weekList = new List<Areas.Basis.Entity.tbWeek>() {
                new Areas.Basis.Entity.tbWeek() { No = 1, tbTenant = tenant, WeekName = "星期一", WeekCode = 1 },
                new Areas.Basis.Entity.tbWeek() { No = 2, tbTenant = tenant, WeekName = "星期二", WeekCode = 2 },
                new Areas.Basis.Entity.tbWeek() { No = 3, tbTenant = tenant, WeekName = "星期三", WeekCode = 3 },
                new Areas.Basis.Entity.tbWeek() { No = 4, tbTenant = tenant, WeekName = "星期四", WeekCode = 4 },
                new Areas.Basis.Entity.tbWeek() { No = 5, tbTenant = tenant, WeekName = "星期五", WeekCode = 5 },
                new Areas.Basis.Entity.tbWeek() { No = 6, tbTenant = tenant, WeekName = "星期六", WeekCode = 6 },
                new Areas.Basis.Entity.tbWeek() { No = 7, tbTenant = tenant, WeekName = "星期日", WeekCode = 0 }
            };
            var tbWeekList = db.Set<Areas.Basis.Entity.tbWeek>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var a in weekList)
            {
                if (tbWeekList.Where(d => d.No == a.No).Any())
                {
                    foreach (var week in tbWeekList.Where(d => d.No == a.No))
                    {
                        week.No = a.No;
                        week.WeekName = a.WeekName;
                        week.WeekCode = a.WeekCode;
                    }
                }
                else
                {
                    db.Set<Areas.Basis.Entity.tbWeek>().Add(a);
                }
            }

            #endregion

            #region tbGradeType

            var gradeTypeList = new List<Areas.Basis.Entity.tbGradeType>() {
                new Areas.Basis.Entity.tbGradeType() { tbTenant = tenant, No = 2, GradeTypeName = "小学" },
                new Areas.Basis.Entity.tbGradeType() { tbTenant = tenant, No = 3, GradeTypeName = "初中" },
                new Areas.Basis.Entity.tbGradeType() { tbTenant = tenant, No = 4, GradeTypeName = "高中" }
            };
            var tbGradeTypeList = db.Set<Areas.Basis.Entity.tbGradeType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in gradeTypeList)
            {
                if (tbGradeTypeList.Where(d => d.GradeTypeName == v.GradeTypeName).Any())
                { }
                else
                {
                    db.Set<Areas.Basis.Entity.tbGradeType>().Add(v);
                }
            }

            #endregion

            db.SaveChanges();

            #region tbGrade

            var gradeList = new List<Areas.Basis.Entity.tbGrade>() {
                new Areas.Basis.Entity.tbGrade() { tbTenant = tenant, No = 11, GradeName = "一年级", tbGradeType = db.Table<Areas.Basis.Entity.tbGradeType>().Where(d=>d.GradeTypeName == "小学").FirstOrDefault() },
                new Areas.Basis.Entity.tbGrade() { tbTenant = tenant, No = 12, GradeName = "二年级", tbGradeType = db.Table<Areas.Basis.Entity.tbGradeType>().Where(d=>d.GradeTypeName == "小学").FirstOrDefault() },
                new Areas.Basis.Entity.tbGrade() { tbTenant = tenant, No = 13, GradeName = "三年级", tbGradeType = db.Table<Areas.Basis.Entity.tbGradeType>().Where(d=>d.GradeTypeName == "小学").FirstOrDefault() },
                new Areas.Basis.Entity.tbGrade() { tbTenant = tenant, No = 14, GradeName = "四年级", tbGradeType = db.Table<Areas.Basis.Entity.tbGradeType>().Where(d=>d.GradeTypeName == "小学").FirstOrDefault() },
                new Areas.Basis.Entity.tbGrade() { tbTenant = tenant, No = 15, GradeName = "五年级", tbGradeType = db.Table<Areas.Basis.Entity.tbGradeType>().Where(d=>d.GradeTypeName == "小学").FirstOrDefault() },
                new Areas.Basis.Entity.tbGrade() { tbTenant = tenant, No = 16, GradeName = "六年级", tbGradeType = db.Table<Areas.Basis.Entity.tbGradeType>().Where(d=>d.GradeTypeName == "小学").FirstOrDefault() },
                new Areas.Basis.Entity.tbGrade() { tbTenant = tenant, No = 21, GradeName = "初一年级", tbGradeType = db.Table<Areas.Basis.Entity.tbGradeType>().Where(d=>d.GradeTypeName == "初中").FirstOrDefault() },
                new Areas.Basis.Entity.tbGrade() { tbTenant = tenant, No = 22, GradeName = "初二年级", tbGradeType = db.Table<Areas.Basis.Entity.tbGradeType>().Where(d=>d.GradeTypeName == "初中").FirstOrDefault() },
                new Areas.Basis.Entity.tbGrade() { tbTenant = tenant, No = 23, GradeName = "初三年级", tbGradeType = db.Table<Areas.Basis.Entity.tbGradeType>().Where(d=>d.GradeTypeName == "初中").FirstOrDefault() },
                new Areas.Basis.Entity.tbGrade() { tbTenant = tenant, No = 31, GradeName = "高一年级", tbGradeType = db.Table<Areas.Basis.Entity.tbGradeType>().Where(d=>d.GradeTypeName == "高中").FirstOrDefault() },
                new Areas.Basis.Entity.tbGrade() { tbTenant = tenant, No = 32, GradeName = "高二年级", tbGradeType = db.Table<Areas.Basis.Entity.tbGradeType>().Where(d=>d.GradeTypeName == "高中").FirstOrDefault() },
                new Areas.Basis.Entity.tbGrade() { tbTenant = tenant, No = 33, GradeName = "高三年级", tbGradeType = db.Table<Areas.Basis.Entity.tbGradeType>().Where(d=>d.GradeTypeName == "高中").FirstOrDefault() }
            };
            var tbGradeList = db.Set<Areas.Basis.Entity.tbGrade>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in gradeList)
            {
                if (tbGradeList.Where(d => d.GradeName == v.GradeName).Any())
                { }
                else
                {
                    db.Set<Areas.Basis.Entity.tbGrade>().Add(v);
                }
            }

            #endregion

            #region tbSysRole

            var sysRoleList = new List<Areas.Sys.Entity.tbSysRole>() {
                new Areas.Sys.Entity.tbSysRole() { tbTenant = tenant, RoleName = "自定义", RoleCode = XkSystem.Code.EnumHelper.SysRoleCode.Other },
                new Areas.Sys.Entity.tbSysRole() { tbTenant = tenant, RoleName = "管理员", RoleCode = XkSystem.Code.EnumHelper.SysRoleCode.Administrator },
                new Areas.Sys.Entity.tbSysRole() { tbTenant = tenant, RoleName = "教师", RoleCode = XkSystem.Code.EnumHelper.SysRoleCode.Teacher },
                new Areas.Sys.Entity.tbSysRole() { tbTenant = tenant, RoleName = "年级组长", RoleCode = XkSystem.Code.EnumHelper.SysRoleCode.GradeTeacher },
                new Areas.Sys.Entity.tbSysRole() { tbTenant = tenant, RoleName = "科组长", RoleCode = XkSystem.Code.EnumHelper.SysRoleCode.SubjectTeacher },
                new Areas.Sys.Entity.tbSysRole() { tbTenant = tenant, RoleName = "班主任", RoleCode = XkSystem.Code.EnumHelper.SysRoleCode.ClassTeacher },
                new Areas.Sys.Entity.tbSysRole() { tbTenant = tenant, RoleName = "任课教师", RoleCode = XkSystem.Code.EnumHelper.SysRoleCode.OrgTeacher },
                new Areas.Sys.Entity.tbSysRole() { tbTenant = tenant, RoleName = "学生", RoleCode = XkSystem.Code.EnumHelper.SysRoleCode.Student },
                new Areas.Sys.Entity.tbSysRole() { tbTenant = tenant, RoleName = "家长", RoleCode = XkSystem.Code.EnumHelper.SysRoleCode.Family },
                new Areas.Sys.Entity.tbSysRole() { tbTenant = tenant, RoleName = "报修受理人", RoleCode = XkSystem.Code.EnumHelper.SysRoleCode.Repair },
                new Areas.Sys.Entity.tbSysRole() { tbTenant = tenant, RoleName = "报修管理人", RoleCode = XkSystem.Code.EnumHelper.SysRoleCode.RepairManagner }
            };
            var tbSysRoleList = db.Set<Areas.Sys.Entity.tbSysRole>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in sysRoleList)
            {
                if (tbSysRoleList.Where(d => d.RoleName == v.RoleName).Any())
                { }
                else
                {
                    db.Set<Areas.Sys.Entity.tbSysRole>().Add(v);
                }
            }

            #endregion

            #region tbExamType

            var examTypeList = new List<Areas.Exam.Entity.tbExamType>() {
                new Areas.Exam.Entity.tbExamType() { tbTenant = tenant, No = 1, ExamTypeName = "期中" },
                new Areas.Exam.Entity.tbExamType() { tbTenant = tenant, No = 2, ExamTypeName = "期末" },
                new Areas.Exam.Entity.tbExamType() { tbTenant = tenant, No = 3, ExamTypeName = "补考" },
                new Areas.Exam.Entity.tbExamType() { tbTenant = tenant, No = 4, ExamTypeName = "会考" },
                new Areas.Exam.Entity.tbExamType() { tbTenant = tenant, No = 5, ExamTypeName = "测验" },
                new Areas.Exam.Entity.tbExamType() { tbTenant = tenant, No = 6, ExamTypeName = "其他" }
            };
            var tbExamTypeList = db.Set<Areas.Exam.Entity.tbExamType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in examTypeList)
            {
                if (tbExamTypeList.Where(d => d.ExamTypeName == v.ExamTypeName).Any())
                { }
                else
                {
                    db.Set<Areas.Exam.Entity.tbExamType>().Add(v);
                }
            }

            #endregion

            #region tbExamLevelGroup
            var levelGroup = new Areas.Exam.Entity.tbExamLevelGroup() { tbTenant = tenant, No = 1, ExamLevelGroupName = "默认" };
            var levelGroupList = new List<Areas.Exam.Entity.tbExamLevelGroup>() {
                levelGroup
            };
            var tbLevelGroupList = db.Set<Areas.Exam.Entity.tbExamLevelGroup>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in levelGroupList)
            {
                if (tbLevelGroupList.Where(d => d.ExamLevelGroupName == v.ExamLevelGroupName).Any())
                { }
                else
                {
                    db.Set<Areas.Exam.Entity.tbExamLevelGroup>().Add(v);
                }
            }

            #endregion

            #region tbExamLevel
            var examLevelList = new List<Areas.Exam.Entity.tbExamLevel>() {
                new Areas.Exam.Entity.tbExamLevel() { tbTenant = tenant, No = 1, ExamLevelName = "A+", MaxScore = 100, MinScore = 90, tbExamLevelGroup = levelGroup },
                new Areas.Exam.Entity.tbExamLevel() { tbTenant = tenant, No = 2, ExamLevelName = "A", MaxScore = 89.99M, MinScore = 80, tbExamLevelGroup = levelGroup },
                new Areas.Exam.Entity.tbExamLevel() { tbTenant = tenant, No = 3, ExamLevelName = "B", MaxScore = 79.99M, MinScore = 70, tbExamLevelGroup = levelGroup },
                new Areas.Exam.Entity.tbExamLevel() { tbTenant = tenant, No = 4, ExamLevelName = "C", MaxScore = 69.99M, MinScore = 60, tbExamLevelGroup = levelGroup },
                new Areas.Exam.Entity.tbExamLevel() { tbTenant = tenant, No = 5, ExamLevelName = "D", MaxScore = 59.99M, MinScore = 0, tbExamLevelGroup = levelGroup }
            };
            var tbExamLevelList = db.Set<Areas.Exam.Entity.tbExamLevel>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in examLevelList)
            {
                if (tbExamLevelList.Where(d => d.ExamLevelName == v.ExamLevelName).Any())
                { }
                else
                {
                    db.Set<Areas.Exam.Entity.tbExamLevel>().Add(v);
                }
            }

            #endregion

            #region tbExamStatus

            var examStatusList = new List<Areas.Exam.Entity.tbExamStatus>() {
                new Areas.Exam.Entity.tbExamStatus() { No = 1, tbTenant = tenant, ExamStatusName = "正常", },
                new Areas.Exam.Entity.tbExamStatus() { No = 2, tbTenant = tenant, ExamStatusName = "缺考", },
                new Areas.Exam.Entity.tbExamStatus() { No = 3, tbTenant = tenant, ExamStatusName = "作弊", }
            };
            var tbExamStatusList = db.Set<Areas.Exam.Entity.tbExamStatus>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in examStatusList)
            {
                if (tbExamStatusList.Where(d => d.ExamStatusName == v.ExamStatusName).Any())
                { }
                else
                {
                    db.Set<Areas.Exam.Entity.tbExamStatus>().Add(v);
                }
            }

            #endregion

            #region tbStudentType

            var studentTypeList = new List<Areas.Student.Entity.tbStudentType>() {
                new Areas.Student.Entity.tbStudentType() { tbTenant = tenant, No = 1, StudentTypeName = "统招生", },
                new Areas.Student.Entity.tbStudentType() { tbTenant = tenant, No = 2, StudentTypeName = "择校生", },
                new Areas.Student.Entity.tbStudentType() { tbTenant = tenant, No = 3, StudentTypeName = "插班生", },
                new Areas.Student.Entity.tbStudentType() { tbTenant = tenant, No = 4, StudentTypeName = "借读生", },
                new Areas.Student.Entity.tbStudentType() { tbTenant = tenant, No = 5, StudentTypeName = "转学生", }
            };
            var tbStudentTypeList = db.Set<Areas.Student.Entity.tbStudentType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in studentTypeList)
            {
                if (tbStudentTypeList.Where(d => d.StudentTypeName == v.StudentTypeName).Any())
                { }
                else
                {
                    db.Set<Areas.Student.Entity.tbStudentType>().Add(v);
                }
            }

            #endregion

            #region tbStudentSession

            var studentSessionList = new List<Areas.Student.Entity.tbStudentSession>() {
                new Areas.Student.Entity.tbStudentSession() { tbTenant = tenant, No = 2010, StudentSessionName = "2010级" },
                new Areas.Student.Entity.tbStudentSession() { tbTenant = tenant, No = 2011, StudentSessionName = "2011级" },
                new Areas.Student.Entity.tbStudentSession() { tbTenant = tenant, No = 2012, StudentSessionName = "2012级" },
                new Areas.Student.Entity.tbStudentSession() { tbTenant = tenant, No = 2013, StudentSessionName = "2013级" },
                new Areas.Student.Entity.tbStudentSession() { tbTenant = tenant, No = 2014, StudentSessionName = "2014级" },
                new Areas.Student.Entity.tbStudentSession() { tbTenant = tenant, No = 2015, StudentSessionName = "2015级" },
                new Areas.Student.Entity.tbStudentSession() { tbTenant = tenant, No = 2016, StudentSessionName = "2016级" }
            };
            var tbStudentSessionList = db.Set<Areas.Student.Entity.tbStudentSession>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in studentSessionList)
            {
                if (tbStudentSessionList.Where(d => d.StudentSessionName == v.StudentSessionName).Any())
                { }
                else
                {
                    db.Set<Areas.Student.Entity.tbStudentSession>().Add(v);
                }
            }

            #endregion

            #region tbStudentStudyType

            var studentStudyTypeList = new List<Areas.Student.Entity.tbStudentStudyType>() {
                new Areas.Student.Entity.tbStudentStudyType() { tbTenant = tenant, No = 1, StudyTypeName = "寄宿", },
                new Areas.Student.Entity.tbStudentStudyType() { tbTenant = tenant, No = 2, StudyTypeName = "走读", }
            };
            var tbStudentStudyTypeList = db.Set<Areas.Student.Entity.tbStudentStudyType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in studentStudyTypeList)
            {
                if (tbStudentStudyTypeList.Where(d => d.StudyTypeName == v.StudyTypeName).Any())
                { }
                else
                {
                    db.Set<Areas.Student.Entity.tbStudentStudyType>().Add(v);
                }
            }

            #endregion

            #region tbStudentChnageType

            var studentChangeTypeList = new List<Areas.Student.Entity.tbStudentChangeType>() {
                new Areas.Student.Entity.tbStudentChangeType() { tbTenant = tenant, No = 1, StudentChangeTypeName = "休学", StudentChangeType=Code.EnumHelper.StudentChangeType.OutSchool },
                new Areas.Student.Entity.tbStudentChangeType() { tbTenant = tenant, No = 2, StudentChangeTypeName = "转学", StudentChangeType=Code.EnumHelper.StudentChangeType.OutSchool },
                new Areas.Student.Entity.tbStudentChangeType() { tbTenant = tenant, No = 3, StudentChangeTypeName = "开除", StudentChangeType=Code.EnumHelper.StudentChangeType.OutSchool },
                new Areas.Student.Entity.tbStudentChangeType() { tbTenant = tenant, No = 4, StudentChangeTypeName = "退学", StudentChangeType=Code.EnumHelper.StudentChangeType.OutSchool },
                new Areas.Student.Entity.tbStudentChangeType() { tbTenant = tenant, No = 5, StudentChangeTypeName = "出国", StudentChangeType=Code.EnumHelper.StudentChangeType.OutSchool },
                new Areas.Student.Entity.tbStudentChangeType() { tbTenant = tenant, No = 6, StudentChangeTypeName = "调班", StudentChangeType=Code.EnumHelper.StudentChangeType.InSchool },
                new Areas.Student.Entity.tbStudentChangeType() { tbTenant = tenant, No = 7, StudentChangeTypeName = "转入", StudentChangeType=Code.EnumHelper.StudentChangeType.InSchool },
                new Areas.Student.Entity.tbStudentChangeType() { tbTenant = tenant, No = 8, StudentChangeTypeName = "复学", StudentChangeType=Code.EnumHelper.StudentChangeType.InSchool },
                new Areas.Student.Entity.tbStudentChangeType() { tbTenant = tenant, No = 9, StudentChangeTypeName = "其他", StudentChangeType=Code.EnumHelper.StudentChangeType.OutSchool }
            };
            var tbStudentChangeTypeList = db.Set<Areas.Student.Entity.tbStudentChangeType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in studentChangeTypeList)
            {
                if (tbStudentChangeTypeList.Where(d => d.StudentChangeTypeName == v.StudentChangeTypeName).Any())
                { }
                else
                {
                    db.Set<Areas.Student.Entity.tbStudentChangeType>().Add(v);
                }
            }

            #endregion

            #region tbCourseDomain

            var courseDomainList = new List<Areas.Course.Entity.tbCourseDomain>() {
                new Areas.Course.Entity.tbCourseDomain() { tbTenant = tenant, No = 11, CourseDomainName = "语言与文学" },
                new Areas.Course.Entity.tbCourseDomain() { tbTenant = tenant, No = 12, CourseDomainName = "数学" },
                new Areas.Course.Entity.tbCourseDomain() { tbTenant = tenant, No = 13, CourseDomainName = "人文与社会" },
                new Areas.Course.Entity.tbCourseDomain() { tbTenant = tenant, No = 14, CourseDomainName = "科学" },
                new Areas.Course.Entity.tbCourseDomain() { tbTenant = tenant, No = 15, CourseDomainName = "技术" },
                new Areas.Course.Entity.tbCourseDomain() { tbTenant = tenant, No = 16, CourseDomainName = "艺术" },
                new Areas.Course.Entity.tbCourseDomain() { tbTenant = tenant, No = 21, CourseDomainName = "体育与健康" },
                new Areas.Course.Entity.tbCourseDomain() { tbTenant = tenant, No = 22, CourseDomainName = "综合实践活动" },
            };
            var tbCourseDomainList = db.Set<Areas.Course.Entity.tbCourseDomain>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in courseDomainList)
            {
                if (courseDomainList.Where(d => d.CourseDomainName == v.CourseDomainName).Any())
                { }
                else
                {
                    db.Set<Areas.Course.Entity.tbCourseDomain>().Add(v);
                }
            }

            #endregion

            #region tbSubject

            var subjectList = new List<Areas.Course.Entity.tbSubject>() {
                new Areas.Course.Entity.tbSubject() { No = 1, tbTenant = tenant, SubjectName = "语文", SubjectNameEn = "Chinese" },
                new Areas.Course.Entity.tbSubject() { No = 2, tbTenant = tenant, SubjectName = "外语", SubjectNameEn = "English" },
                new Areas.Course.Entity.tbSubject() { No = 3, tbTenant = tenant, SubjectName = "数学", SubjectNameEn = "Maths" },
                new Areas.Course.Entity.tbSubject() { No = 4, tbTenant = tenant, SubjectName = "思想政治", SubjectNameEn = "Politics" },
                new Areas.Course.Entity.tbSubject() { No = 5, tbTenant = tenant, SubjectName = "历史", SubjectNameEn = "History" },
                new Areas.Course.Entity.tbSubject() { No = 6, tbTenant = tenant, SubjectName = "地理", SubjectNameEn = "Geography" },
                new Areas.Course.Entity.tbSubject() { No = 6, tbTenant = tenant, SubjectName = "物理", SubjectNameEn = "Physics" },
                new Areas.Course.Entity.tbSubject() { No = 7, tbTenant = tenant, SubjectName = "化学", SubjectNameEn = "Chemistry" },
                new Areas.Course.Entity.tbSubject() { No = 8, tbTenant = tenant, SubjectName = "生物", SubjectNameEn = "Biology" },
                new Areas.Course.Entity.tbSubject() { No = 9, tbTenant = tenant, SubjectName = "信息技术", SubjectNameEn = "Technology" },
                new Areas.Course.Entity.tbSubject() { No = 10, tbTenant = tenant, SubjectName = "通用技术", SubjectNameEn = "Technology" },
                new Areas.Course.Entity.tbSubject() { No = 11, tbTenant = tenant, SubjectName = "体育与健康", SubjectNameEn = "P.E." },
                new Areas.Course.Entity.tbSubject() { No = 12, tbTenant = tenant, SubjectName = "研究性学习活动", SubjectNameEn = "Research" },
                new Areas.Course.Entity.tbSubject() { No = 13, tbTenant = tenant, SubjectName = "社区服务", SubjectNameEn = "Community Service" },
                new Areas.Course.Entity.tbSubject() { No = 14, tbTenant = tenant, SubjectName = "实践活动", SubjectNameEn = "Social Practice" },
                new Areas.Course.Entity.tbSubject() { No = 16, tbTenant = tenant, SubjectName = "艺术", SubjectNameEn = "Arts" },
                new Areas.Course.Entity.tbSubject() { No = 17, tbTenant = tenant, SubjectName = "其他", SubjectNameEn = "Other" }
            };
            var tbSubjectList = db.Set<Areas.Course.Entity.tbSubject>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in subjectList)
            {
                if (tbSubjectList.Where(d => d.SubjectName == v.SubjectName).Any())
                { }
                else
                {
                    db.Set<Areas.Course.Entity.tbSubject>().Add(v);
                }
            }

            #endregion

            #region tbYear

            var yearList = new List<Areas.Basis.Entity.tbYear>();
            for (var i = 2015; i < 2030; i++)
            {
                var year = new Areas.Basis.Entity.tbYear()
                {
                    No = i * 100,
                    tbTenant = tenant,
                    YearName = i + "-" + (i + 1) + "学年",
                    tbYearParent = null,
                    YearType = Code.EnumHelper.YearType.Year,
                    IsDisable = i > 2020 ? true : false,
                };
                var term1 = new Areas.Basis.Entity.tbYear()
                {
                    No = i * 100 + 10,
                    tbTenant = tenant,
                    YearName = i + "-" + (i + 1) + "学年上学期",
                    tbYearParent = year,
                    YearType = Code.EnumHelper.YearType.Term,
                    IsDisable = i > 2020 ? true : false,
                };
                var term2 = new Areas.Basis.Entity.tbYear()
                {
                    No = i * 100 + 20,
                    tbTenant = tenant,
                    YearName = i + "-" + (i + 1) + "学年下学期",
                    tbYearParent = year,
                    YearType = Code.EnumHelper.YearType.Term,
                    IsDisable = i > 2020 ? true : false,
                };
                var section1 = new Areas.Basis.Entity.tbYear()
                {
                    No = i * 100 + 11,
                    tbTenant = tenant,
                    YearName = i + "-" + (i + 1) + "学年上学期期中",
                    tbYearParent = term1,
                    YearType = Code.EnumHelper.YearType.Section,
                    IsDisable = i > 2020 ? true : false,
                    IsDefault = (i == DateTime.Now.Year) && DateTime.Now.Month < 5
                };
                var section2 = new Areas.Basis.Entity.tbYear()
                {
                    No = i * 100 + 12,
                    tbTenant = tenant,
                    YearName = i + "-" + (i + 1) + "学年上学期期末",
                    tbYearParent = term1,
                    YearType = Code.EnumHelper.YearType.Section,
                    IsDisable = i > 2020 ? true : false,
                    IsDefault = (i == DateTime.Now.Year) && (DateTime.Now.Month >= 5 && DateTime.Now.Month <= 7)
                };
                var section3 = new Areas.Basis.Entity.tbYear()
                {
                    No = i * 100 + 23,
                    tbTenant = tenant,
                    YearName = i + "-" + (i + 1) + "学年下学期期中",
                    tbYearParent = term2,
                    YearType = Code.EnumHelper.YearType.Section,
                    IsDisable = i > 2020 ? true : false,
                    IsDefault = (i == DateTime.Now.Year) && (DateTime.Now.Month >= 9 && DateTime.Now.Month < 11)
                };
                var section4 = new Areas.Basis.Entity.tbYear()
                {
                    No = i * 100 + 24,
                    tbTenant = tenant,
                    YearName = i + "-" + (i + 1) + "学年下学期期末",
                    tbYearParent = term2,
                    YearType = Code.EnumHelper.YearType.Section,
                    IsDisable = i > 2020 ? true : false,
                    IsDefault = (i == DateTime.Now.Year) && DateTime.Now.Month >= 11
                };
                yearList.Add(year);
                yearList.Add(term1);
                yearList.Add(term2);
                yearList.Add(section1);
                yearList.Add(section2);
                yearList.Add(section3);
                yearList.Add(section4);
            }
            var tbYearList = db.Set<Areas.Basis.Entity.tbYear>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in yearList)
            {
                if (tbYearList.Where(d => d.YearName == v.YearName).Any())
                { }
                else
                {
                    db.Set<Areas.Basis.Entity.tbYear>().Add(v);
                }
            }

            #endregion

            #region tbAttendanceType

            var AttendanceTypeList = new List<Areas.Attendance.Entity.tbAttendanceType>() {
                new Areas.Attendance.Entity.tbAttendanceType() { tbTenant = tenant, No = 1, AttendanceTypeName = "正常", AttendanceValue = 0 },
                new Areas.Attendance.Entity.tbAttendanceType() { tbTenant = tenant, No = 2, AttendanceTypeName = "迟到", AttendanceValue = 1 },
                new Areas.Attendance.Entity.tbAttendanceType() { tbTenant = tenant, No = 3, AttendanceTypeName = "早退", AttendanceValue = 2 },
                new Areas.Attendance.Entity.tbAttendanceType() { tbTenant = tenant, No = 4, AttendanceTypeName = "缺席", AttendanceValue = 3 },
                new Areas.Attendance.Entity.tbAttendanceType() { tbTenant = tenant, No = 5, AttendanceTypeName = "病假", AttendanceValue = 4 },
                new Areas.Attendance.Entity.tbAttendanceType() { tbTenant = tenant, No = 5, AttendanceTypeName = "事假", AttendanceValue = 5 }
            };
            var tbAttendanceTypeList = db.Set<Areas.Attendance.Entity.tbAttendanceType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in AttendanceTypeList)
            {
                if (tbAttendanceTypeList.Where(d => d.AttendanceTypeName == v.AttendanceTypeName).Any())
                { }
                else
                {
                    db.Set<Areas.Attendance.Entity.tbAttendanceType>().Add(v);
                }
            }

            #endregion

            #region tbTeacherDept

            var teacherDeptList = new List<Areas.Teacher.Entity.tbTeacherDept>() {
                new Areas.Teacher.Entity.tbTeacherDept() { tbTenant = tenant, No = 0, TeacherDeptName = "教师部门", tbTeacherDeptParent = null }
            };
            var tbTeacherDeptList = db.Set<Areas.Teacher.Entity.tbTeacherDept>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in teacherDeptList)
            {
                if (tbTeacherDeptList.Where(d => d.TeacherDeptName == v.TeacherDeptName).Any())
                { }
                else
                {
                    db.Set<Areas.Teacher.Entity.tbTeacherDept>().Add(v);
                }
            }

            #endregion

            #region tbStudentHonorLevel

            var studentHonorLevelList = new List<Areas.Student.Entity.tbStudentHonorLevel>() {
                new Areas.Student.Entity.tbStudentHonorLevel() { No=1,tbTenant=tenant,StudentHonorLevelName="一等奖"},
                new Areas.Student.Entity.tbStudentHonorLevel() { No=2,tbTenant=tenant,StudentHonorLevelName="二等奖"},
                new Areas.Student.Entity.tbStudentHonorLevel() { No=3,tbTenant=tenant,StudentHonorLevelName="三等奖"}
            };
            var tbStudentHonorLevelList = db.Set<Areas.Student.Entity.tbStudentHonorLevel>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in studentHonorLevelList)
            {
                if (tbStudentHonorLevelList.Where(d => d.StudentHonorLevelName == v.StudentHonorLevelName).Any())
                { }
                else
                {
                    db.Set<Areas.Student.Entity.tbStudentHonorLevel>().Add(v);
                }
            }

            #endregion

            #region tbStudentHonorType

            var studentHonorTypeList = new List<Areas.Student.Entity.tbStudentHonorType>() {
                new Areas.Student.Entity.tbStudentHonorType() {No=1,tbTenant=tenant,StudentHonorTypeName="国家级" },
                new Areas.Student.Entity.tbStudentHonorType() {No=2,tbTenant=tenant,StudentHonorTypeName="省级" },
                new Areas.Student.Entity.tbStudentHonorType() {No=3,tbTenant=tenant,StudentHonorTypeName="市县级" }
            };
            var tbStudentHonorTypeList = db.Set<Areas.Student.Entity.tbStudentHonorType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in studentHonorTypeList)
            {
                if (tbStudentHonorTypeList.Where(d => d.StudentHonorTypeName == v.StudentHonorTypeName).Any())
                { }
                else
                {
                    db.Set<Areas.Student.Entity.tbStudentHonorType>().Add(v);
                }
            }

            #endregion
            

            db.SaveChanges();

            #region tbPeriod

            var periodList = new List<Areas.Basis.Entity.tbPeriod>() {
                new Areas.Basis.Entity.tbPeriod() { No = 1, tbTenant = tenant, PeriodName = "1", tbPeriodType = db.Set<Areas.Basis.Entity.tbPeriodType>().Where(d=>d.PeriodTypeName == "上午").FirstOrDefault() },
                new Areas.Basis.Entity.tbPeriod() { No = 2, tbTenant = tenant, PeriodName = "2", tbPeriodType = db.Set<Areas.Basis.Entity.tbPeriodType>().Where(d=>d.PeriodTypeName == "上午").FirstOrDefault() },
                new Areas.Basis.Entity.tbPeriod() { No = 3, tbTenant = tenant, PeriodName = "3", tbPeriodType = db.Set<Areas.Basis.Entity.tbPeriodType>().Where(d=>d.PeriodTypeName == "上午").FirstOrDefault() },
                new Areas.Basis.Entity.tbPeriod() { No = 4, tbTenant = tenant, PeriodName = "4", tbPeriodType = db.Set<Areas.Basis.Entity.tbPeriodType>().Where(d=>d.PeriodTypeName == "上午").FirstOrDefault() },
                new Areas.Basis.Entity.tbPeriod() { No = 5, tbTenant = tenant, PeriodName = "5", tbPeriodType = db.Set<Areas.Basis.Entity.tbPeriodType>().Where(d=>d.PeriodTypeName == "上午").FirstOrDefault() },
                new Areas.Basis.Entity.tbPeriod() { No = 6, tbTenant = tenant, PeriodName = "午", tbPeriodType = db.Set<Areas.Basis.Entity.tbPeriodType>().Where(d=>d.PeriodTypeName == "中午").FirstOrDefault() },
                new Areas.Basis.Entity.tbPeriod() { No = 7, tbTenant = tenant, PeriodName = "6", tbPeriodType = db.Set<Areas.Basis.Entity.tbPeriodType>().Where(d=>d.PeriodTypeName == "下午").FirstOrDefault() },
                new Areas.Basis.Entity.tbPeriod() { No = 8, tbTenant = tenant, PeriodName = "7", tbPeriodType = db.Set<Areas.Basis.Entity.tbPeriodType>().Where(d=>d.PeriodTypeName == "下午").FirstOrDefault() },
                new Areas.Basis.Entity.tbPeriod() { No = 9, tbTenant = tenant, PeriodName = "8", tbPeriodType = db.Set<Areas.Basis.Entity.tbPeriodType>().Where(d=>d.PeriodTypeName == "下午").FirstOrDefault() }
            };
            var tbPeriodList = db.Set<Areas.Basis.Entity.tbPeriod>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in periodList)
            {
                if (tbPeriodList.Where(d => d.No == v.No).Any())
                { }
                else
                {
                    db.Set<Areas.Basis.Entity.tbPeriod>().Add(v);
                }
            }

            #endregion

            #region tbSysUser

            var admin = new Areas.Sys.Entity.tbSysUser()
            {
                tbTenant = tenant,
                UserCode = string.IsNullOrEmpty(adminLoginCode) ? "administrator" : adminLoginCode,
                UserName = "管理员",
                UserType = Code.EnumHelper.SysUserType.Administrator,
                Password = Code.Common.DESEnCode(string.IsNullOrEmpty(adminPassword) ? "admin&123456" : adminPassword),
                PasswordMd5 = Code.Common.CreateMD5Hash(string.IsNullOrEmpty(adminPassword) ? "admin&123456" : adminPassword)
            };

            if (db.Set<Areas.Sys.Entity.tbSysUser>().Where(d => d.tbTenant.Id == tenant.Id && d.UserCode == admin.UserCode).Any() == false)
            {
                db.Set<Areas.Sys.Entity.tbSysUser>().Add(admin);
            }

            #endregion
             
            #region tbTeacher

            var teacher = new Areas.Teacher.Entity.tbTeacher()
            {
                tbTenant = tenant,
                TeacherCode = "administrator",
                TeacherName = "管理员",
                tbSysUser = admin
            };

            if (db.Set<Areas.Teacher.Entity.tbTeacher>().Where(d => d.tbTenant.Id == tenant.Id && d.TeacherCode == admin.UserCode).Any() == false)
            {
                db.Set<Areas.Teacher.Entity.tbTeacher>().Add(teacher);
            }

            #endregion

            #region tbStudent

            //var student = new Areas.Student.Entity.tbStudent()
            //{
            //    tbTenant = tenant,
            //    StudentCode = "administrator",
            //    StudentName = "管理员",
            //    tbSysUser = admin
            //};

            //if (db.Set<Areas.Student.Entity.tbStudent>().Where(d => d.tbTenant.Id == tenant.Id && d.StudentCode == admin.UserCode).Any() == false)
            //{
            //    db.Set<Areas.Student.Entity.tbStudent>().Add(student);
            //}

            #endregion

            db.SaveChanges();

            #region tbDiskType
            var diskTypePublic = new Areas.Disk.Entity.tbDiskType() { tbTenant = tenant, No = 1, DiskType = XkSystem.Code.EnumHelper.DiskType.Public, DiskTypeName = "公开文件夹" };
            var diskTypeList = new List<Areas.Disk.Entity.tbDiskType>() {
                diskTypePublic,
                new Areas.Disk.Entity.tbDiskType() { tbTenant = tenant, No = 2, DiskType = XkSystem.Code.EnumHelper.DiskType.Private, DiskTypeName = "个人文件夹"}
            };
            var tbDiskTypeList = db.Set<Areas.Disk.Entity.tbDiskType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in diskTypeList)
            {
                if (tbDiskTypeList.Where(d => d.DiskTypeName == v.DiskTypeName).Any())
                { }
                else
                {
                    db.Set<Areas.Disk.Entity.tbDiskType>().Add(v);
                }
            }

            #endregion

            #region tbDiskFolder

            var tbDiskFolderList = db.Set<Areas.Disk.Entity.tbDiskFolder>().Where(d => d.tbTenant.Id == tenant.Id).ToList();

            var rootFolder = new Areas.Disk.Entity.tbDiskFolder();
            rootFolder.DiskFolderName = "公开文件夹";
            rootFolder.No = 1;
            rootFolder.DiskPermit = XkSystem.Code.EnumHelper.DiskPermit.Public;
            rootFolder.tbDiskType = diskTypePublic;
            rootFolder.tbSysUser = admin;
            rootFolder.tbTenant = tenant;

            if (tbDiskFolderList.Where(d => d.DiskFolderName == rootFolder.DiskFolderName).Any())
            {
                rootFolder = tbDiskFolderList.Where(d => d.DiskFolderName == rootFolder.DiskFolderName).FirstOrDefault();
            }
            else
            {
                db.Set<Areas.Disk.Entity.tbDiskFolder>().Add(rootFolder);
            }

            var diskFolderList = new List<Areas.Disk.Entity.tbDiskFolder>() {
                new Areas.Disk.Entity.tbDiskFolder() {
                    DiskFolderName = "学校共享文件夹",
                    tbTenant = tenant,
                    No = 2,
                    DiskPermit = XkSystem.Code.EnumHelper.DiskPermit.Public,
                    tbDiskFolderParent = rootFolder,
                    tbSysUser = admin,
                    tbDiskType = diskTypePublic,
                },
                new Areas.Disk.Entity.tbDiskFolder() {
                    DiskFolderName = "教师共享文件夹",
                    tbTenant = tenant,
                    No = 3,
                    DiskPermit = XkSystem.Code.EnumHelper.DiskPermit.Public,
                    tbDiskFolderParent = rootFolder,
                    tbSysUser = admin,
                    tbDiskType = diskTypePublic,
                }
            };
            foreach (var v in diskFolderList)
            {
                if (tbDiskFolderList.Where(d => d.DiskFolderName == v.DiskFolderName).Any())
                { }
                else
                {
                    db.Set<Areas.Disk.Entity.tbDiskFolder>().Add(v);
                }
            }

            #endregion

            db.SaveChanges();

            #region tbWeOAFlowType
            var oAFlowTypeList = new List<Areas.Wechat.Entity.tbWeOAFlowType>() {
                new Areas.Wechat.Entity.tbWeOAFlowType() { tbTenant = tenant, No = 1,Code = "OF",FlowTypeName="办公收发文审批" },
                new Areas.Wechat.Entity.tbWeOAFlowType() { tbTenant = tenant, No = 1,Code = "CAR",FlowTypeName="用车申请" },
                new Areas.Wechat.Entity.tbWeOAFlowType() { tbTenant = tenant, No = 1,Code = "LEAVE",FlowTypeName="请假申请" }
            };
            var tbSysFlowTypeList = db.Set<Areas.Wechat.Entity.tbWeOAFlowType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in oAFlowTypeList)
            {
                if (tbSysFlowTypeList.Where(d => d.Code == v.Code).Any())
                { }
                else
                {
                    db.Set<Areas.Wechat.Entity.tbWeOAFlowType>().Add(v);
                }
            }
            #endregion

            db.SaveChanges();

            #region tbWeOAFlowNode
            var tOFlowTypeList = db.Set<Areas.Wechat.Entity.tbWeOAFlowType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            var tbOFObj = tOFlowTypeList.Where(m => m.Code == "OF").FirstOrDefault();
            var tbCarObj = tOFlowTypeList.Where(m => m.Code == "CAR").FirstOrDefault();
            var tbLeaveObj = tOFlowTypeList.Where(m => m.Code == "LEAVE").FirstOrDefault();
            var oAFlowTemplateList = new List<Areas.Wechat.Entity.tbWeOAFlowNode>() {
                //办公发文流程图数据
                new Areas.Wechat.Entity.tbWeOAFlowNode() { tbTenant = tenant, No = 1, FlowApprovalNode = "办公室主任",FlowStep=1,tbSysOAFlowType= tbOFObj},
                new Areas.Wechat.Entity.tbWeOAFlowNode() { tbTenant = tenant, No = 2, FlowApprovalNode = "公文处理人",FlowStep=2,tbSysOAFlowType= tbOFObj},
                new Areas.Wechat.Entity.tbWeOAFlowNode() { tbTenant = tenant, No = 3, FlowApprovalNode = "副校长",FlowStep=3,tbSysOAFlowType= tbOFObj},
                new Areas.Wechat.Entity.tbWeOAFlowNode() { tbTenant = tenant, No = 4, FlowApprovalNode = "部门负责人",FlowStep=4,tbSysOAFlowType= tbOFObj,FlowComplete=true},
                //用车申请流程图数据
                new Areas.Wechat.Entity.tbWeOAFlowNode() { tbTenant = tenant, No = 1, FlowApprovalNode = "用车申请人",FlowStep=1,tbSysOAFlowType= tbCarObj},
                new Areas.Wechat.Entity.tbWeOAFlowNode() { tbTenant = tenant, No = 2, FlowApprovalNode = "主管领导",FlowStep=2,tbSysOAFlowType= tbCarObj},
                new Areas.Wechat.Entity.tbWeOAFlowNode() { tbTenant = tenant, No = 3, FlowApprovalNode = "车队领导",FlowStep=3,tbSysOAFlowType= tbCarObj},
                new Areas.Wechat.Entity.tbWeOAFlowNode() { tbTenant = tenant, No = 4, FlowApprovalNode = "司机",FlowStep=4,tbSysOAFlowType= tbCarObj,FlowComplete=true},
                //请假流程图数据
                new Areas.Wechat.Entity.tbWeOAFlowNode() { tbTenant = tenant, No = 1, FlowApprovalNode = "教师",FlowStep=1,tbSysOAFlowType= tbLeaveObj,ConditionalFormula=""},
                new Areas.Wechat.Entity.tbWeOAFlowNode() { tbTenant = tenant, No = 2, FlowApprovalNode = "部门负责人",FlowStep=2,tbSysOAFlowType= tbLeaveObj,FlowComplete=true, ConditionalFormula="day=0.5"},
                new Areas.Wechat.Entity.tbWeOAFlowNode() { tbTenant = tenant, No = 2, FlowApprovalNode = "年级负责人",FlowStep=2,tbSysOAFlowType= tbLeaveObj,FlowComplete=true,ConditionalFormula="day=0.5"},
                new Areas.Wechat.Entity.tbWeOAFlowNode() { tbTenant = tenant, No = 2, FlowApprovalNode = "教学处",FlowStep=3,tbSysOAFlowType= tbLeaveObj,FlowComplete=false,ConditionalFormula="day<=2&day>0.5"},
                new Areas.Wechat.Entity.tbWeOAFlowNode() { tbTenant = tenant, No = 2, FlowApprovalNode = "主管教学副校长",FlowStep=4,tbSysOAFlowType= tbLeaveObj,FlowComplete=true,ConditionalFormula="day<=2&day>0.5"},
                new Areas.Wechat.Entity.tbWeOAFlowNode() { tbTenant = tenant, No = 2, FlowApprovalNode = "校长",FlowStep=5,tbSysOAFlowType= tbLeaveObj,FlowComplete=true,ConditionalFormula="day>=3"},

            };
            var tbOAFlowTemplateList = db.Set<Areas.Wechat.Entity.tbWeOAFlowNode>().Where(d => d.tbTenant.Id == tenant.Id).Include(d => d.tbSysOAFlowType).ToList();
            foreach (var v in oAFlowTemplateList)
            {
                if (tbOAFlowTemplateList.Where(d => d.FlowApprovalNode == v.FlowApprovalNode && d.tbSysOAFlowType.Code == v.tbSysOAFlowType.Code).Any())
                { }
                else
                {
                    db.Set<Areas.Wechat.Entity.tbWeOAFlowNode>().Add(v);
                }
            }
            #endregion

            db.SaveChanges();

            #region tbWeOALeave
            var oALeaveList = new List<Areas.Wechat.Entity.tbWeOALeaveType>() {
                new Areas.Wechat.Entity.tbWeOALeaveType() { tbTenant = tenant, No = 1,LeaveTypeName="事假"},
                new Areas.Wechat.Entity.tbWeOALeaveType() { tbTenant = tenant, No = 2,LeaveTypeName="病假"},
                new Areas.Wechat.Entity.tbWeOALeaveType() { tbTenant = tenant, No = 3,LeaveTypeName="婚嫁"},
                new Areas.Wechat.Entity.tbWeOALeaveType() { tbTenant = tenant, No = 4,LeaveTypeName="丧假"},
                new Areas.Wechat.Entity.tbWeOALeaveType() { tbTenant = tenant, No = 5,LeaveTypeName="公假"},
                new Areas.Wechat.Entity.tbWeOALeaveType() { tbTenant = tenant, No = 6,LeaveTypeName="工伤"},
                new Areas.Wechat.Entity.tbWeOALeaveType() { tbTenant = tenant, No = 7,LeaveTypeName="产假"},
                new Areas.Wechat.Entity.tbWeOALeaveType() { tbTenant = tenant, No = 8,LeaveTypeName="护理假"},
                new Areas.Wechat.Entity.tbWeOALeaveType() { tbTenant = tenant, No = 9,LeaveTypeName="其他"}
            };
            var tbWeOALeaveTypeList = db.Set<Areas.Wechat.Entity.tbWeOALeaveType>().Where(d => d.tbTenant.Id == tenant.Id).ToList();
            foreach (var v in oALeaveList)
            {
                if (tbWeOALeaveTypeList.Where(d => d.LeaveTypeName == v.LeaveTypeName).Any())
                { }
                else
                {
                    db.Set<Areas.Wechat.Entity.tbWeOALeaveType>().Add(v);
                }
            }
            #endregion

            db.SaveChanges();
        }
    }
}
