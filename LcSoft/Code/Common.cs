using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace XkSystem.Code
{
    public class Common
    {
        public const string FormatToDate = "{0:yyyy-MM-dd}";
        public const string FormatToDateTime = "{0:yyyy-MM-dd HH:mm:ss}";
        public const string FormatToDateMinuteTime = "{0:yyyy-MM-dd HH:mm}";
        public const string FormatToYear = "{0:yyyy}";
        public const string FormatToYearMonth = "{0:yyyy-MM}";
        public const string FormatToInt = "{0:f0}";

        public const string StringToDate = "yyyy-MM-dd";
        public const string StringToDateTime = "yyyy-MM-dd HH:mm:ss";
        public const string StringToDateMinuteTime = "yyyy-MM-dd HH:mm";
        public const string StringToTime = "HH:mm:ss";
        public const string StringToYear = "yyyy";
        public const string StringToYearMonth = "yyyy-MM";
        public const string StringToInt = "f0";

        public const string RegIntAndDecimal = "^[0-9]+([.]{1}[0-9]+){0,1}$";       //正整数&浮点数
        /// <summary>
        /// 正整数校验(>0)
        /// </summary>
        public const string RegPositiveInt = @"^[0-9]*[1-9][0-9]*$";       //正整数校验不包括0
        /// <summary>
        /// 正整数校验(>=0)
        /// </summary>
        public const string RegPositiveIntZero = @"^[1-9]\d*|0$";       //正整数包括0
        /// <summary>
        /// 手机格式不正确
        /// </summary>
        public const string RegMobil = @"(13\d|14[57]|15[^4,\D]|17[678]|18\d)\d{8}|170[059]\d{7}";

        /// <summary>
        /// 座机验证
        /// </summary>
        public const string RegTel = @"\d{3}-\d{8}|\d{4}-\d{7,8}|\d{7,8}";

        /// <summary>
        /// 邮箱格式不正确
        /// </summary>
        public const string RegEmail = @"\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}";       //邮箱验证
        /// <summary>
        /// 只允许输入中英文字符、数字和_或-，特殊字符都不能包含
        /// </summary>
        public const string RegUserCode = @"[A-Za-z0-9\u4E00-\u9FA5_-]+$";       //用户账号
        /// <summary>
        /// 只允许输入中英文字符、数字、空格和_或-，特殊字符都不能包含
        /// </summary>
        public const string RegUserName = @"[A-Za-z0-9\s\u4E00-\u9FA5_-]+$";       //用户姓名
        /// <summary>
        /// 身份证号格式不正确
        /// </summary>
        public const string RegIdentityNumber = @"^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$|^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}([0-9]|X)$";       //身份证号验证
        /// <summary>
        /// QQ格式不正确
        /// </summary>
        public const string RegQQNumber = @"^\d{5,10}$";       //QQ验证        
        /// <summary>
        /// 数字和英文
        /// </summary>
        public const string RegEnglishName = @"^[A-Za-z0-9]+$";       //数字和英文校验
        /// <summary>
        /// 数字、英文、空格（学生姓名拼音中需要包含空格）
        /// </summary>
        public const string RegEnglishName2 = @"^[A-Za-z0-9\s]+$";       //数字、英文、空格校验
        /// <summary>
        /// 邮政编码
        /// </summary>
        public const string RegPostalCode = @"^[1-9][0-9]{5}$";//邮政编码验证

        public static DateTime DateMonthFirst = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        public static DateTime DateMonthLast = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddSeconds(-1);

        public const string DownloadType = "application/octet-stream";

        public static string Host
        {
            get
            {
                if (HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "Host"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return DESDeCode(HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "Host"].Value);
                }
            }
            set
            {
                HttpContext.Current.Response.Cookies[XkSystem.Code.Common.AppName + "Host"].Value = DESEnCode(value);
            }
        }

        public static string AppName
        {
            get
            {
                return AppDomain.CurrentDomain.SetupInformation.ApplicationName;
            }
        }

        public static int TenantId
        {
            get
            {
                if (System.Web.HttpContext.Current != null)
                {
                    if (System.Web.HttpContext.Current.Request.Cookies[AppName + "TenantId"] == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return System.Web.HttpContext.Current.Request.Cookies[AppName + "TenantId"].Value.ConvertToInt();
                    }
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                System.Web.HttpContext.Current.Response.Cookies[AppName + "TenantId"].Value = value.ToString();
            }
        }

        public static string Program
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["Program"] != null)
                {
                    return System.Configuration.ConfigurationManager.AppSettings["Program"];
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public static string IndexArea
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["IndexArea"] != null)
                {
                    return System.Configuration.ConfigurationManager.AppSettings["IndexArea"];
                }
                else
                {
                    return "Sys";
                }
            }
        }

        public static string IndexController
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["IndexController"] != null)
                {
                    return System.Configuration.ConfigurationManager.AppSettings["IndexController"];
                }
                else
                {
                    return "SysIndex";
                }
            }
        }

        public static string IndexAction
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["IndexAction"] != null)
                {
                    return System.Configuration.ConfigurationManager.AppSettings["IndexAction"];
                }
                else
                {
                    return "Login";
                }
            }
        }

        public static int ProgramId
        {
            get
            {
                if (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ProgramId"]) == false)
                {
                    return System.Configuration.ConfigurationManager.AppSettings["ProgramId"].ConvertToInt();
                }
                else
                {

                    if (HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "ProgramId"] == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "ProgramId"].Value.ConvertToInt();
                    }
                }
            }
            set
            {
                HttpContext.Current.Response.Cookies[XkSystem.Code.Common.AppName + "ProgramId"].Value = value.ToString();
            }
        }

        public static string ProgramName
        {
            get
            {
                if (HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "ProgramName"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return DESDeCode(HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "ProgramName"].Value);
                }
            }
            set
            {
                HttpContext.Current.Response.Cookies[XkSystem.Code.Common.AppName + "ProgramName"].Value = DESEnCode(value);
            }
        }   

        public static int UserId
        {
            get
            {
                if (HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "UserId"] == null)
                {
                    return 0;
                }
                else
                {
                    return HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "UserId"].Value.ConvertToInt();
                }
            }
            set
            {
                HttpContext.Current.Response.Cookies[XkSystem.Code.Common.AppName + "UserId"].Value = value.ToString();
            }
        }

        public static string UserName
        {
            get
            {
                if (HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "UserName"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return DESDeCode(HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "UserName"].Value);
                }
            }
            set
            {
                HttpContext.Current.Response.Cookies[XkSystem.Code.Common.AppName + "UserName"].Value = DESEnCode(value);
            }
        }

        public static XkSystem.Code.EnumHelper.SysUserType UserType
        {
            get
            {
                if (HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "UserType"] == null)
                {
                    return XkSystem.Code.EnumHelper.SysUserType.Other;
                }
                else
                {
                    //((Areas.Code.EnumHelper.SysUserType)Code.EnumHelper.TryParse(typeof(Code.EnumHelper.SysUserType), HttpContext.Current.Request.Cookies[AppName + "UserType"].Value
                    XkSystem.Code.EnumHelper.SysUserType userType;
                    Enum.TryParse(HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "UserType"].Value, out userType);
                    return userType;
                }
            }
            set
            {
                HttpContext.Current.Response.Cookies[XkSystem.Code.Common.AppName + "UserType"].Value = value.ToString();
            }
        }

        public static bool IsJavaApp
        {
            get
            {
                if (HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "IsJavaApp"] == null)
                {
                    return false;
                }
                else
                {
                    return bool.Parse(HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "IsJavaApp"].Value);
                }
            }
            set
            {
                HttpContext.Current.Response.Cookies[XkSystem.Code.Common.AppName + "IsJavaApp"].Value = value.ToString();
            }
        }

        public static bool IsRepairMananger
        {
            get
            {
                if (HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "IsRepairMananger"] == null)
                {
                    return false;
                }
                else
                {
                    return bool.Parse(HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "IsRepairMananger"].Value);
                }
            }
            set
            {
                HttpContext.Current.Response.Cookies[XkSystem.Code.Common.AppName + "IsRepairMananger"].Value = value.ToString();
            }
        }


        public static bool IsMoralMananger
        {
            get
            {
                if (HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "IsMoralMananger"] == null)
                {
                    return false;
                }
                else
                {
                    return bool.Parse(HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "IsMoralMananger"].Value);
                }
            }
            set
            {
                HttpContext.Current.Response.Cookies[XkSystem.Code.Common.AppName + "IsMoralMananger"].Value = value.ToString();
            }
        }


        public static bool IsProcessUser
        {
            get
            {
                if (HttpContext.Current.Request.Cookies["IsProcessUser"] == null)
                {
                    return false;
                }
                else
                {
                    return bool.Parse(HttpContext.Current.Request.Cookies["IsProcessUser"].Value);
                }
            }
            set
            {
                HttpContext.Current.Response.Cookies["IsProcessUser"].Value = value.ToString();
            }
        }

        public static string AppTitle
        {
            get
            {
                if (HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "AppTitle"] == null)
                {
                    if (HttpContext.Current.Cache["AppTitle"] != null)
                    {
                        return HttpContext.Current.Cache["AppTitle"].ToString();
                    }
                    else
                    {
                        return "龙创软件";
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "AppTitle"].Value))
                    {
                        return "龙创软件";
                    }
                    else
                    {
                        return DESDeCode(HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "AppTitle"].Value);
                    }
                }
            }
            set
            {
                HttpContext.Current.Response.Cookies[XkSystem.Code.Common.AppName + "AppTitle"].Value = DESEnCode(value);
            }
        }

        public static bool IsWide
        {
            get
            {
                if (HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "IsWide"] == null)
                {
                    return false;
                }
                else
                {
                    return bool.Parse(HttpContext.Current.Request.Cookies[XkSystem.Code.Common.AppName + "IsWide"].Value);
                }
            }
            set
            {
                HttpContext.Current.Response.Cookies[XkSystem.Code.Common.AppName + "IsWide"].Value = value.ToString();
            }
        }

        public static bool IsApp
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["IsApp"] != null)
                {
                    if (string.Compare(System.Configuration.ConfigurationManager.AppSettings["IsApp"].ToString(), "true", true) == decimal.Zero)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public static string ExportByExcel
        {
            get
            {
                return System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            }
        }

        public static string ExportByWord
        {
            get
            {
                return System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".docx";
            }
        }

        public static bool IsLog
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["IsLog"] != null)
                {
                    if (string.Compare(System.Configuration.ConfigurationManager.AppSettings["IsLog"].ToString(), "true", true) == decimal.Zero)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// IIS站点名称
        /// </summary>
        public static string FolderName
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["FolderName"] != null)
                {
                    return System.Configuration.ConfigurationManager.AppSettings["FolderName"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public static string AssetDomain
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["AssetDomain"] != null)
                {
                    return System.Configuration.ConfigurationManager.AppSettings["AssetDomain"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public static string PayDomain
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["PayDomain"] != null)
                {
                    return System.Configuration.ConfigurationManager.AppSettings["PayDomain"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public static string OpenDomain
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["OpenDomain"] != null)
                {
                    return System.Configuration.ConfigurationManager.AppSettings["OpenDomain"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public static bool IsStartCas
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["IsStartCas"] != null)
                {
                    if (string.Compare(System.Configuration.ConfigurationManager.AppSettings["IsStartCas"].ToString(), "true", true) == decimal.Zero)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public static string LoginUrl
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["LoginUrl"] != null)
                {
                    return System.Configuration.ConfigurationManager.AppSettings["LoginUrl"] + "?service=" + System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 单点使用
        /// </summary>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string RedirectCas(string url, string msg = "")
        {
            var val = string.Empty;
            if (string.IsNullOrEmpty(msg) == false)
            {
                val = "<script>alert('" + msg.Replace("'", "\"").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n") + "');";
            }
            string ServerUrl = System.Configuration.ConfigurationManager.AppSettings.Get("ServerUrl");
            val = val + "window.location.href='" + ServerUrl + url + "';</script>";
            return val;
        }

        public static string Redirect(string url, string msg = "")
        {
            var val = "<script>";
            if (string.IsNullOrEmpty(msg) == false)
            {
                val = val + "alert('" + msg.Replace("'", "\"").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n") + "');";
            }
            val = val + "window.location.replace('" + url + "');</script>";
            return val;
        }

        public static string CreateCheckCode()
        {
            var codeList = new List<string>() { "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            var tb = (from p in codeList
                      orderby Guid.NewGuid()
                      select p).Take(4);
            return string.Join("", tb.ToArray());
        }

        public static byte[] CreateCheckGraphic(string checkCode)
        {
            var image = new System.Drawing.Bitmap(100, 34);
            var graphics = System.Drawing.Graphics.FromImage(image);
            var random = new Random();
            graphics.Clear(System.Drawing.Color.White);
            var font = new System.Drawing.Font("Arial", 16, (System.Drawing.FontStyle.Bold));
            var fontColor = System.Drawing.ColorTranslator.FromHtml("#337ab7");
            var brush = new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), fontColor, fontColor, 1.2f, true);
            graphics.DrawString(checkCode, font, brush, 12, 5);
            var stream = new System.IO.MemoryStream();
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }

        /// <summary>
        /// 生成机器码
        /// </summary>
        /// <returns></returns>
        public static string getMachineCode()
        {
            //Code.Common.DESEnCode(Code.JsonHelper.ToJsonString<Admin.Dto.Config.Cdkey>(new Admin.Dto.Config.Cdkey() { LicenseTo = "龙创软件", MachineCode = "a", Deadline = new DateTime(2099,01,01).ToString(Code.Common.StringToDate), Power = "ddd" }))

            return AppDomain.CurrentDomain.SetupInformation.ApplicationName;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="pToEncrypt"></param>
        /// <returns></returns>
        public static string DESEnCode(string pToEncrypt)
        {
            try
            {
                var sKey = "Lc.8Soft";//必须为8位
                var des = new System.Security.Cryptography.DESCryptoServiceProvider();
                var inputByteArray = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(pToEncrypt);
                des.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = System.Text.ASCIIEncoding.ASCII.GetBytes(sKey);
                var ms = new System.IO.MemoryStream();
                var cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                var ret = new System.Text.StringBuilder();
                foreach (byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                return ret.ToString();
            }
            catch
            {
                return pToEncrypt;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <returns></returns>
        public static string DESDeCode(string pToDecrypt)
        {
            try
            {
                var sKey = "Lc.8Soft";//必须为8位
                var des = new System.Security.Cryptography.DESCryptoServiceProvider();
                var inputByteArray = new byte[pToDecrypt.Length / 2];
                for (var x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                des.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = System.Text.ASCIIEncoding.ASCII.GetBytes(sKey);
                var ms = new System.IO.MemoryStream();
                var cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                var ret = new System.Text.StringBuilder();
                return System.Text.Encoding.UTF8.GetString(ms.ToArray());
            }
            catch
            {
                return pToDecrypt;
            }
        }

        public static string CreateMD5Hash(string input)
        {
            //SQLMD5：substring(sys.fn_sqlvarbasetostr(HashBytes('MD5','123456')),3,32)
            // Use input string to calculate MD5 hash
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
                // To force the hex string to lower-case letters instead of
                // upper-case, use he following line instead:
                // sb.Append(hashBytes[i].ToString("x2")); 
            }
            return sb.ToString();
        }

        public static string GetSwcSH1(string value)
        {
            var algorithm = System.Security.Cryptography.SHA1.Create();
            byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            string sh1 = "";
            for (int i = 0; i < data.Length; i++)
            {
                sh1 += data[i].ToString("x2").ToUpperInvariant();
            }
            return sh1;
        }

        public static string GetDayOfWeek(DateTime dt)
        {
            switch (dt.DayOfWeek.ToString("D"))
            {
                case "0":
                    return "星期日 ";
                case "1":
                    return "星期一 ";
                case "2":
                    return "星期二 ";
                case "3":
                    return "星期三 ";
                case "4":
                    return "星期四 ";
                case "5":
                    return "星期五 ";
                case "6":
                    return "星期六 ";
                default:
                    return "未知";
            }
        }

        //这段代码用户请求真实IP(用HTTP_X_FORWARDED_FOR获取外网IP)
        public static string GetIp()
        {
            var request = HttpContext.Current.Request;
            if (request != null)
            {
                //HTTP_X_FORWARDED_FOR
                string ipAddress = request.ServerVariables["x-forwarded-for"];
                if (!IsEffectiveIP(ipAddress))
                {
                    ipAddress = request.ServerVariables["Proxy-Client-IP"];
                }
                if (!IsEffectiveIP(ipAddress))
                {
                    ipAddress = request.ServerVariables["WL-Proxy-Client-IP"];
                }
                if (!IsEffectiveIP(ipAddress))
                {
                    ipAddress = request.ServerVariables["Remote_Addr"];
                    if (ipAddress.Equals("127.0.0.1") || ipAddress.Equals("::1"))
                    {
                        // 根据网卡取本机配置的IP
                        System.Net.IPAddress[] AddressList = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList;
                        foreach (System.Net.IPAddress _IPAddress in AddressList)
                        {
                            if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                            {
                                ipAddress = _IPAddress.ToString();
                                break;
                            }
                        }
                    }
                }
                // 对于通过多个代理的情况，第一个IP为客户端真实IP,多个IP按照','分割
                if (ipAddress != null && ipAddress.Length > 15)
                {
                    if (ipAddress.IndexOf(",") > 0)
                    {
                        ipAddress = ipAddress.Substring(0, ipAddress.IndexOf(","));
                    }
                }

                return ipAddress;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 是否有效IP地址
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <returns>bool</returns>
        private static bool IsEffectiveIP(string ipAddress)
        {
            return !(string.IsNullOrEmpty(ipAddress) || "unknown".Equals(ipAddress, StringComparison.OrdinalIgnoreCase));
        }

        public static string ResolveUrl(string relativeUrl)
        {
            if (relativeUrl == null) throw new ArgumentNullException("relativeUrl");

            if (relativeUrl.Length == 0 || relativeUrl[0] == '/' ||
                relativeUrl[0] == '\\')
                return relativeUrl;

            int idxOfScheme =
              relativeUrl.IndexOf(@"://", StringComparison.Ordinal);
            if (idxOfScheme != -1)
            {
                int idxOfQM = relativeUrl.IndexOf('?');
                if (idxOfQM == -1 || idxOfQM > idxOfScheme) return relativeUrl;
            }

            var sbUrl = new System.Text.StringBuilder();
            sbUrl.Append(System.Web.HttpRuntime.AppDomainAppVirtualPath);
            if (sbUrl.Length == 0 || sbUrl[sbUrl.Length - 1] != '/') sbUrl.Append('/');

            // found question mark already? query string, do not touch!
            bool foundQM = false;
            bool foundSlash; // the latest char was a slash?
            if (relativeUrl.Length > 1
                && relativeUrl[0] == '~'
                && (relativeUrl[1] == '/' || relativeUrl[1] == '\\'))
            {
                relativeUrl = relativeUrl.Substring(2);
                foundSlash = true;
            }
            else foundSlash = false;
            foreach (char c in relativeUrl)
            {
                if (!foundQM)
                {
                    if (c == '?') foundQM = true;
                    else
                    {
                        if (c == '/' || c == '\\')
                        {
                            if (foundSlash) continue;
                            else
                            {
                                sbUrl.Append('/');
                                foundSlash = true;
                                continue;
                            }
                        }
                        else if (foundSlash) foundSlash = false;
                    }
                }
                sbUrl.Append(c);
            }

            return sbUrl.ToString();
        }

        //年份转换为大写汉字
        public static string numtoUpper(int num)
        {
            return "零壹贰叁肆伍陆柒捌玖"[num].ToString();
        }

        //月份转换大写汉字
        public static string monthtoUpper(int month)
        {
            if (month < 10)
            {
                return numtoUpper(month);
            }
            else
            {
                if (month == 10) { return "壹拾"; }

                else
                {
                    return "壹拾" + numtoUpper(month - 10);
                }
            }
        }

        //日期转化为大写汉字
        public static string daytoUpper(int day)
        {
            if (day < 20)
            {
                return monthtoUpper(day);
            }
            else
            {
                String str = day.ToString();
                if (str[1] == '0')
                {
                    return numtoUpper(Convert.ToInt16(str[0].ToString())) + "拾";
                }
                else
                {
                    return numtoUpper(Convert.ToInt16(str[0].ToString())) + "拾"
                        + numtoUpper(Convert.ToInt16(str[1].ToString()));
                }
            }
        }

        //日转化为大写
        public static string daytoNewUpper(int day)
        {
            if (day < 20)
            {
                return monthNewtoUpper(day);
            }
            else
            {
                String str = day.ToString();
                if (str[1] == '0')
                {
                    return numtoNewUpper(Convert.ToInt16(str[0].ToString())) + "十";
                }
                else
                {
                    return numtoNewUpper(Convert.ToInt16(str[0].ToString())) + "十"
                        + numtoNewUpper(Convert.ToInt16(str[1].ToString()));
                }
            }
        }
        /// <summary>
        /// //把数字转换为大写
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string numtoNewUpper(int num)
        {
            String str = num.ToString();
            string rstr = "";
            int n;
            for (int i = 0; i < str.Length; i++)
            {
                n = Convert.ToInt16(str[i].ToString());//char转数字,转换为字符串，再转数字
                switch (n)
                {
                    case 0: rstr = rstr + "零"; break;
                    case 1: rstr = rstr + "一"; break;
                    case 2: rstr = rstr + "二"; break;
                    case 3: rstr = rstr + "三"; break;
                    case 4: rstr = rstr + "四"; break;
                    case 5: rstr = rstr + "五"; break;
                    case 6: rstr = rstr + "六"; break;
                    case 7: rstr = rstr + "七"; break;
                    case 8: rstr = rstr + "八"; break;
                    default: rstr = rstr + "九"; break;
                }
            }
            return rstr;
        }
        //月转化为大写
        public static string monthNewtoUpper(int month)
        {
            if (month < 10)
            {
                return numtoNewUpper(month);
            }
            else
                if (month == 10) { return "十"; }

            else
            {
                return "十" + numtoNewUpper(month - 10);
            }
        }

        //日期转换为大写
        public static string dateToUpper(System.DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;
            return numtoNewUpper(year) + "年" + monthNewtoUpper(month) + "月" + daytoNewUpper(day) + "日";
        }

        //日期转换为大写年月
        public static string dateToUpperYM(System.DateTime? date)
        {
            if (date == null)
            {
                return string.Empty;
            }
            else
            {
                int year = ((DateTime)date).Year;
                int month = ((DateTime)date).Month;
                return numtoNewUpper(year) + "年" + monthNewtoUpper(month) + "月";
            }
        }

        /// <summary>
        /// string[]作为列名生成DataTable
        /// </summary>
        public static System.Data.DataTable ArrayToDataTable(string[] arr)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            for (int i = 0; i < arr.Length; i++)
            {
                dt.Columns.Add(arr[i]);
            }
            return dt;
        }

        public static FileType GetFileType(string fileName)
        {
            FileType ft = 0;
            var extensionName = fileName.Split('.').Last().ToLower();

            var excelList = new List<string>() { "xls", "xlsx", "xlsm" };
            var wordList = new List<string>() { "doc", "docx" };
            var imageList = new List<string>() { "jpg", "jpeg", "png", "bmp" };
            var zipList = new List<string>() { "zip" };

            if (excelList.Contains(extensionName))
            {
                ft = FileType.Excel;
            }
            else if (wordList.Contains(extensionName))
            {
                ft = FileType.Word;
            }
            else if (imageList.Contains(extensionName))
            {
                ft = FileType.Image;
            }
            else if (zipList.Contains(extensionName))
            {
                ft = FileType.Zip;
            }
            else
            {
                ft = FileType.Other;
            }

            return ft;
        }

       

        /// <summary>
        /// 检测终端
        /// </summary>
        public static bool IsMobile
        {
            get
            {
                if (Code.Common.IsApp == false)
                {
                    return false;
                }

                string strUserAgent = HttpContext.Current.Request.UserAgent.ToString().ToLower();
                bool flag = false;
                if (strUserAgent != null)
                {
                    if (HttpContext.Current.Request.Browser.IsMobileDevice == true || strUserAgent.Contains("iphone") ||
                        strUserAgent.Contains("blackberry") || strUserAgent.Contains("mobile") ||
                        strUserAgent.Contains("windows ce") || strUserAgent.Contains("opera mini") ||
                        strUserAgent.Contains("windows ce") || strUserAgent.Contains("opera mini") ||
                        strUserAgent.Contains("palm"))
                    {
                        flag = true;
                    }
                }

                return flag;
            }
        }


        /// <summary>
        /// 保存上传文件，并返回文件名
        /// </summary>
        /// <param name="file"></param>
        /// <param name="savePath"></param>
        /// <param name="isRename">是否重命名文件标题</param>
        /// <returns></returns>
        public static Tuple<string, string> SaveFile(HttpPostedFileBase file, string savePath,bool isRename)
        {
            var _filePath = HttpContext.Current.Server.MapPath(savePath);
            if (!System.IO.Directory.Exists(_filePath))
            {
                System.IO.Directory.CreateDirectory(_filePath);
            }
            var fileTitle = System.IO.Path.GetFileName(file.FileName);
            var fileName = Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);
            file.SaveAs(_filePath + fileName);
            return new Tuple<string, string>(fileTitle, fileName);
        }


        /// <summary>
        /// 保存文件并返回文件名
        /// </summary>
        /// <param name="file"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public static string SaveFile(HttpPostedFileBase file, string savePath)
        {
            var _filePath = HttpContext.Current.Server.MapPath(savePath);
            if (!System.IO.Directory.Exists(_filePath))
            {
                System.IO.Directory.CreateDirectory(_filePath);
            }
            var fileName = Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);
            file.SaveAs(_filePath + fileName);
            return fileName;
        }

        /// <summary>
        /// 遍历获取文件加path下的所有文件，并返回文件名集合
        /// </summary>
        public static List<string[]> GetFileNames(System.IO.DirectoryInfo dir)
        {
            List<string[]> list = new List<string[]>();
            var dd = dir.GetDirectories();
            if (dir.Attributes == System.IO.FileAttributes.Directory)
            {
                foreach (var v in dir.GetFiles())
                {
                    list.Add(new string[] { v.Name, v.FullName });
                }
                foreach (var v in dir.GetDirectories())
                {
                    list.AddRange(GetFileNames(v));
                }
            }
            else if (dir.Attributes == System.IO.FileAttributes.Normal)
            {
                list.Add(new string[] { dir.Name, dir.FullName });
            }
            return list;
        }

        #region 对象转JSON字符串
        public static string ToJSONString(object obj)
        {
            if (obj == null) return "[]";
            //var js = new System.Web.Script.Serialization.JavaScriptSerializer();
            //return js.Serialize(obj);
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);

        }

        #endregion

    }

    public enum FileType
    {
        Excel,
        Word,
        Image,
        Zip,
        Other
    }
}
/// <summary>
/// 汉字转拼音或转拼音首字母
/// </summary>
public class ChineseToSpell
{
    private static int[] pyValue = new int[]
    {
  -20319,-20317,-20304,-20295,-20292,-20283,-20265,-20257,-20242,-20230,-20051,-20036,
  -20032,-20026,-20002,-19990,-19986,-19982,-19976,-19805,-19784,-19775,-19774,-19763,
  -19756,-19751,-19746,-19741,-19739,-19728,-19725,-19715,-19540,-19531,-19525,-19515,
  -19500,-19484,-19479,-19467,-19289,-19288,-19281,-19275,-19270,-19263,-19261,-19249,
  -19243,-19242,-19238,-19235,-19227,-19224,-19218,-19212,-19038,-19023,-19018,-19006,
  -19003,-18996,-18977,-18961,-18952,-18783,-18774,-18773,-18763,-18756,-18741,-18735,
  -18731,-18722,-18710,-18697,-18696,-18526,-18518,-18501,-18490,-18478,-18463,-18448,
  -18447,-18446,-18239,-18237,-18231,-18220,-18211,-18201,-18184,-18183, -18181,-18012,
  -17997,-17988,-17970,-17964,-17961,-17950,-17947,-17931,-17928,-17922,-17759,-17752,
  -17733,-17730,-17721,-17703,-17701,-17697,-17692,-17683,-17676,-17496,-17487,-17482,
  -17468,-17454,-17433,-17427,-17417,-17202,-17185,-16983,-16970,-16942,-16915,-16733,
  -16708,-16706,-16689,-16664,-16657,-16647,-16474,-16470,-16465,-16459,-16452,-16448,
  -16433,-16429,-16427,-16423,-16419,-16412,-16407,-16403,-16401,-16393,-16220,-16216,
  -16212,-16205,-16202,-16187,-16180,-16171,-16169,-16158,-16155,-15959,-15958,-15944,
  -15933,-15920,-15915,-15903,-15889,-15878,-15707,-15701,-15681,-15667,-15661,-15659,
  -15652,-15640,-15631,-15625,-15454,-15448,-15436,-15435,-15419,-15416,-15408,-15394,
  -15385,-15377,-15375,-15369,-15363,-15362,-15183,-15180,-15165,-15158,-15153,-15150,
  -15149,-15144,-15143,-15141,-15140,-15139,-15128,-15121,-15119,-15117,-15110,-15109,
  -14941,-14937,-14933,-14930,-14929,-14928,-14926,-14922,-14921,-14914,-14908,-14902,
  -14894,-14889,-14882,-14873,-14871,-14857,-14678,-14674,-14670,-14668,-14663,-14654,
  -14645,-14630,-14594,-14429,-14407,-14399,-14384,-14379,-14368,-14355,-14353,-14345,
  -14170,-14159,-14151,-14149,-14145,-14140,-14137,-14135,-14125,-14123,-14122,-14112,
  -14109,-14099,-14097,-14094,-14092,-14090,-14087,-14083,-13917,-13914,-13910,-13907,
  -13906,-13905,-13896,-13894,-13878,-13870,-13859,-13847,-13831,-13658,-13611,-13601,
  -13406,-13404,-13400,-13398,-13395,-13391,-13387,-13383,-13367,-13359,-13356,-13343,
  -13340,-13329,-13326,-13318,-13147,-13138,-13120,-13107,-13096,-13095,-13091,-13076,
  -13068,-13063,-13060,-12888,-12875,-12871,-12860,-12858,-12852,-12849,-12838,-12831,
  -12829,-12812,-12802,-12607,-12597,-12594,-12585,-12556,-12359,-12346,-12320,-12300,
  -12120,-12099,-12089,-12074,-12067,-12058,-12039,-11867,-11861,-11847,-11831,-11798,
  -11781,-11604,-11589,-11536,-11358,-11340,-11339,-11324,-11303,-11097,-11077,-11067,
  -11055,-11052,-11045,-11041,-11038,-11024,-11020,-11019,-11018,-11014,-10838,-10832,
  -10815,-10800,-10790,-10780,-10764,-10587,-10544,-10533,-10519,-10331,-10329,-10328,
  -10322,-10315,-10309,-10307,-10296,-10281,-10274,-10270,-10262,-10260,-10256,-10254
    };
    private static string[] pyName = new string[]
    {
  "A","Ai","An","Ang","Ao","Ba","Bai","Ban","Bang","Bao","Bei","Ben",
  "Beng","Bi","Bian","Biao","Bie","Bin","Bing","Bo","Bu","Ba","Cai","Can",
  "Cang","Cao","Ce","Ceng","Cha","Chai","Chan","Chang","Chao","Che","Chen","Cheng",
  "Chi","Chong","Chou","Chu","Chuai","Chuan","Chuang","Chui","Chun","Chuo","Ci","Cong",
  "Cou","Cu","Cuan","Cui","Cun","Cuo","Da","Dai","Dan","Dang","Dao","De",
  "Deng","Di","Dian","Diao","Die","Ding","Diu","Dong","Dou","Du","Duan","Dui",
  "Dun","Duo","E","En","Er","Fa","Fan","Fang","Fei","Fen","Feng","Fo",
  "Fou","Fu","Ga","Gai","Gan","Gang","Gao","Ge","Gei","Gen","Geng","Gong",
  "Gou","Gu","Gua","Guai","Guan","Guang","Gui","Gun","Guo","Ha","Hai","Han",
  "Hang","Hao","He","Hei","Hen","Heng","Hong","Hou","Hu","Hua","Huai","Huan",
  "Huang","Hui","Hun","Huo","Ji","Jia","Jian","Jiang","Jiao","Jie","Jin","Jing",
  "Jiong","Jiu","Ju","Juan","Jue","Jun","Ka","Kai","Kan","Kang","Kao","Ke",
  "Ken","Keng","Kong","Kou","Ku","Kua","Kuai","Kuan","Kuang","Kui","Kun","Kuo",
  "La","Lai","Lan","Lang","Lao","Le","Lei","Leng","Li","Lia","Lian","Liang",
  "Liao","Lie","Lin","Ling","Liu","Long","Lou","Lu","Lv","Luan","Lue","Lun",
  "Luo","Ma","Mai","Man","Mang","Mao","Me","Mei","Men","Meng","Mi","Mian",
  "Miao","Mie","Min","Ming","Miu","Mo","Mou","Mu","Na","Nai","Nan","Nang",
  "Nao","Ne","Nei","Nen","Neng","Ni","Nian","Niang","Niao","Nie","Nin","Ning",
  "Niu","Nong","Nu","Nv","Nuan","Nue","Nuo","O","Ou","Pa","Pai","Pan",
  "Pang","Pao","Pei","Pen","Peng","Pi","Pian","Piao","Pie","Pin","Ping","Po",
  "Pu","Qi","Qia","Qian","Qiang","Qiao","Qie","Qin","Qing","Qiong","Qiu","Qu",
  "Quan","Que","Qun","Ran","Rang","Rao","Re","Ren","Reng","Ri","Rong","Rou",
  "Ru","Ruan","Rui","Run","Ruo","Sa","Sai","San","Sang","Sao","Se","Sen",
  "Seng","Sha","Shai","Shan","Shang","Shao","She","Shen","Sheng","Shi","Shou","Shu",
  "Shua","Shuai","Shuan","Shuang","Shui","Shun","Shuo","Si","Song","Sou","Su","Suan",
  "Sui","Sun","Suo","Ta","Tai","Tan","Tang","Tao","Te","Teng","Ti","Tian",
  "Tiao","Tie","Ting","Tong","Tou","Tu","Tuan","Tui","Tun","Tuo","Wa","Wai",
  "Wan","Wang","Wei","Wen","Weng","Wo","Wu","Xi","Xia","Xian","Xiang","Xiao",
  "Xie","Xin","Xing","Xiong","Xiu","Xu","Xuan","Xue","Xun","Ya","Yan","Yang",
  "Yao","Ye","Yi","Yin","Ying","Yo","Yong","You","Yu","Yuan","Yue","Yun",
  "Za", "Zai","Zan","Zang","Zao","Ze","Zei","Zen","Zeng","Zha","Zhai","Zhan",
  "Zhang","Zhao","Zhe","Zhen","Zheng","Zhi","Zhong","Zhou","Zhu","Zhua","Zhuai","Zhuan",
  "Zhuang","Zhui","Zhun","Zhuo","Zi","Zong","Zou","Zu","Zuan","Zui","Zun","Zuo"
    };
    /// <summary>
    /// 把汉字转换成拼音(全拼)
    /// </summary>
    /// <param name="hzString">汉字字符串</param>
    /// <returns>转换后的拼音(全拼)字符串</returns>
    public static string CharacterConvertString(string hzString)
    {
        // 匹配中文字符
        Regex regex = new Regex("^[\u4e00-\u9fa5]$");
        byte[] array = new byte[2];
        string pyString = "";
        int chrAsc = 0;
        int i1 = 0;
        int i2 = 0;
        char[] noWChar = hzString.ToCharArray();
        for (int j = 0; j < noWChar.Length; j++)
        {
            // 中文字符
            if (regex.IsMatch(noWChar[j].ToString()))
            {
                array = System.Text.Encoding.Default.GetBytes(noWChar[j].ToString());
                i1 = (short)(array[0]);
                i2 = (short)(array[1]);
                chrAsc = i1 * 256 + i2 - 65536;
                if (chrAsc > 0 && chrAsc < 160)
                {
                    pyString += noWChar[j];
                }
                else
                {
                    // 修正部分文字
                    if (chrAsc == -9254) // 修正“圳”字
                    {
                        pyString += "Zhen";
                    }
                    else
                    {
                        for (int i = (pyValue.Length - 1); i >= 0; i--)
                        {
                            if (pyValue[i] <= chrAsc)
                            {
                                pyString += pyName[i];
                                break;
                            }
                        }
                    }
                }
            }
            // 非中文字符
            else
            {
                pyString += noWChar[j].ToString();
            }
        }
        return pyString;
    }
    /// <summary>
    /// 只转换每个汉字首字母（大写）
    /// </summary>
    /// <param name="strText"></param>
    /// <returns></returns>
    public static string GetChineseSpell(string strText)
    {
        int len = strText.Length;
        string myStr = "";
        for (int i = 0; i < len; i++)
        {
            myStr += getSpell(strText.Substring(i, 1));
        }
        return myStr;
    }
    /// <summary>
    /// 获得第一个汉字的首字母（大写）；
    /// </summary>
    /// <param name="cnChar"></param>
    /// <returns></returns>
    public static string getSpell(string cnChar)
    {
        byte[] arrCN = Encoding.Default.GetBytes(cnChar);
        if (arrCN.Length > 1)
        {
            int area = (short)arrCN[0];
            int pos = (short)arrCN[1];
            int code = (area << 8) + pos;
            int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
            for (int i = 0; i < 26; i++)
            {
                int max = 55290;
                if (i != 25) max = areacode[i + 1];
                if (areacode[i] <= code && code < max)
                {
                    return Encoding.Default.GetString(new byte[] { (byte)(65 + i) });
                }
            }
            return "*";
        }
        else return cnChar;
    }





}
