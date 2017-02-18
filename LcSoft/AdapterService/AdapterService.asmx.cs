using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace XkSystem.AdapterService
{
    /// <summary>
    /// AdapterService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.Web.Services.Protocols.SoapDocumentService(RoutingStyle = SoapServiceRoutingStyle.RequestElement)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class AdapterService : System.Web.Services.WebService, IAdapterServicePortBinding
    {
        //private static log4net.ILog logger = log4net.LogManager.GetLogger("AdapterService");

        //private decimal code = 0;

        [WebMethod]
        public bool test()
        {
            //logger.Info("调用成功:" + DateTime.Now);
            return true;
        }

        /// <summary>
        /// 操作枚举
        /// </summary>
        private enum OptionType
        {
            Add = 0,
            Delete = 1,
            Update = 2
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="tableFlag">表名</param>
        /// <param name="recordList">字段集合</param>
        /// <returns></returns>
        [WebMethod]
        public bool addObject(string tableFlag, dataObject[] recordList)
        {
            try
            {
                //匹配用户
                if ("xb_teacher".Equals(tableFlag) || "xb_student".Equals(tableFlag))
                {
                    return this.OptionUser(OptionType.Add, tableFlag, string.Empty, recordList);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="tableFlag">表名</param>
        /// <param name="synValue">用户编号</param>
        /// <param name="recordList">字段集合</param>
        /// <returns></returns>
        [WebMethod]
        public bool updateObject(string tableFlag, string synValue, dataObject[] recordList)
        {
            try
            {
                //匹配用户
                if ("xb_teacher".Equals(tableFlag) || "xb_student".Equals(tableFlag))
                {
                    return this.OptionUser(OptionType.Update, tableFlag, synValue, recordList);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="tableFlag">表名</param>
        /// <param name="synValue">用户Code</param>
        /// <param name="recordList">表字段数组</param>
        /// <returns></returns>
        [WebMethod]
        public bool deleteObject(string tableFlag, string synValue, dataObject[] recordList)
        {
            try
            {
                //匹配用户
                if ("xb_teacher".Equals(tableFlag) || "xb_student".Equals(tableFlag))
                {
                    return this.OptionUser(OptionType.Delete, tableFlag, string.Empty, recordList);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        [WebMethod]
        public bool pickPermission()
        {
            throw new NotImplementedException();
        }

        [WebMethod]
        public bool restartComponent(string serviceName)
        {
            throw new NotImplementedException();
        }

        [WebMethod]
        public bool startComponent(string serviceName)
        {
            throw new NotImplementedException();
        }

        [WebMethod]
        public bool stopComponent(string serviceName)
        {
            throw new NotImplementedException();
        }

        [WebMethod]
        public string detectComponent()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 操作
        /// </summary>
        /// <param name="type">操作类型</param>
        /// <param name="tableFlag">表名</param>
        /// <param name="synValue">用户编号</param>
        /// <param name="recordList">字段集合</param>
        /// <returns></returns>
        private bool OptionUser(OptionType type, string tableFlag, string synValue, dataObject[] recordList)
        {
            var tb = new Areas.Sys.Entity.tbSysUser();

            string userType = string.Empty;

            using (var db = new XkSystem.Models.DbContext())
            {
                try
                {
                    //匹配用户
                    tb = (from p in db.TableRoot<Areas.Sys.Entity.tbSysUser>()
                          where p.IsDeleted == false && p.UserCode == synValue
                          select p).FirstOrDefault();

                    //对象赋值
                    if (type == OptionType.Add || type == OptionType.Update)
                    {
                        if (type == OptionType.Update && tb == null)
                        {
                            return true;
                        }

                        if (type == OptionType.Add && tb != null)
                        {
                            return true;
                        }
                        if (tb == null)
                        {
                            tb = new Areas.Sys.Entity.tbSysUser();

                            //tb = new XkSystem.Areas.Sys.Entity.tbSysUser();
                            if (Code.Common.TenantId != 0)
                            {
                                tb.tbTenant = db.Set<Areas.Admin.Entity.tbTenant>().Find(Code.Common.TenantId);
                            }
                            else
                            {
                                var tenant = db.Set<Areas.Admin.Entity.tbTenant>().Where(d => d.IsDeleted == false && d.IsDefault == true).FirstOrDefault();
                                tb.tbTenant = tenant;
                            }
                        }

                        //遍历接口数据
                        foreach (var record in recordList)
                        {
                            if ("CODE".Equals(record.code.ToUpper()))
                            {
                                tb.UserCode = string.IsNullOrEmpty(record.value) ? string.Empty : record.value;
                                tb.Password = Code.Common.DESEnCode(record.value);
                            }
                            else if ("CARD_NO".Equals(record.code.ToUpper()))
                            {
                                tb.IdentityNumber = string.IsNullOrEmpty(record.value) ? string.Empty : record.value;
                            }
                            else if ("NAME".Equals(record.code.ToUpper()))
                            {
                                tb.UserName = string.IsNullOrEmpty(record.value) ? string.Empty : record.value;
                            }
                            else if ("PHONE".Equals(record.code.ToUpper()))
                            {
                                tb.Mobile = string.IsNullOrEmpty(record.value) ? string.Empty : record.value;
                            }
                            else if ("EMAIL".Equals(record.code.ToUpper()))
                            {
                                tb.Email = string.IsNullOrEmpty(record.value) ? string.Empty : record.value;
                            }
                            else if ("SCHOOL_ID".Equals(record.code.ToUpper()))
                            {
                                var tenantName = string.IsNullOrEmpty(record.value) ? string.Empty : record.value;
                                tb.tbTenant = (from p in db.TableRoot<Areas.Admin.Entity.tbTenant>()
                                               where p.TenantName == tenantName
                                               select p).FirstOrDefault();
                            }
                        }

                        if ("xb_teacher".Equals(tableFlag))
                        {
                            tb.UserType = Code.EnumHelper.SysUserType.Teacher;
                        }
                        else if ("xb_student".Equals(tableFlag))
                        {
                            tb.UserType = Code.EnumHelper.SysUserType.Student;
                        }
                        else
                        {
                            tb.UserType = Code.EnumHelper.SysUserType.Other;
                        }

                        if (type == OptionType.Add)
                        {
                            //tb.IdentityNumber = string.Empty;
                            tb.IsDisable = false;
                            tb.IsLock = false;
                            //tb.Mobile = string.Empty;
                            tb.Qq = string.Empty;

                            db.Set<XkSystem.Areas.Sys.Entity.tbSysUser>().Add(tb);

                            if ("xb_teacher".Equals(tableFlag))
                            {
                                var Ttb = new Areas.Teacher.Entity.tbTeacher();
                                Ttb.TeacherCode = tb.UserCode;
                                Ttb.TeacherName = tb.UserName;
                                Ttb.tbSysUser = tb;
                                Ttb.IsDeleted = false;
                                Ttb.tbTenant = tb.tbTenant;
                                db.Set<XkSystem.Areas.Teacher.Entity.tbTeacher>().Add(Ttb);
                            }
                            else if ("xb_student".Equals(tableFlag))
                            {
                                var Stb = new Areas.Student.Entity.tbStudent();
                                Stb.StudentCode = tb.UserCode;
                                Stb.StudentName = tb.UserName;
                                Stb.tbSysUser = tb;
                                Stb.IsDeleted = false;
                                Stb.tbTenant = tb.tbTenant;
                                db.Set<XkSystem.Areas.Student.Entity.tbStudent>().Add(Stb);
                            }
                        }
                    }
                    else if (type == OptionType.Delete)
                    {
                        if (tb != null)
                        {
                            tb.IsDeleted = true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    db.SaveChanges();
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
    }
}
