using XkSystem.Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Top.Api;
using Top.Api.Request;

namespace XkSystem.Areas.Sms.Controllers
{
    public class SmsSendController : Controller
    {
        /// <summary>
        /// 待发短信
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetSmsToList(SmsSendRequestModel model)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SmsTo.List();
                var tb = (from p in db.TableRoot<Entity.tbSmsTo>()
                          where (p.Status == decimal.Zero || p.Status == -decimal.One)
                          && p.tbTenant.Id == model.TenantId
                          && p.tbTenant.IsDeleted == false
                          && p.tbSms.PlanDate.Day == DateTime.Now.Day
                          select p);

                var tbSmsToList = (from p in tb
                                   join m in db.TableRoot<Entity.tbSms>() on p.tbSms.Id equals m.Id into n
                                   from k in n.DefaultIfEmpty()
                                   orderby p.SendDate
                                   select new Dto.SmsTo.List
                                   {
                                       Id = p.Id,
                                       Mobile = p.Mobile,
                                       No = p.No,
                                       Remark = p.Remark,
                                       Retry = p.Retry,
                                       SmsId = p.tbSms.Id,
                                       SmsTitle = k == null ? "" : k.SmsTitle,
                                       Status = p.Status,
                                       SendDate = p.SendDate,
                                       SysUserName = p.tbSysUser.UserName
                                   }).Take(50).ToList();

                return Json(tbSmsToList);
            }
        }

        /// <summary>
        /// 更改状态
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SetSmsStatus(SmsSendRequestModel model)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                if (model.SmsResponseList != null && model.SmsResponseList.Count > decimal.Zero)
                {
                    var ids = model.SmsResponseList.Select(d => d.SmsId).ToList();
                    var tbSmsToList = (from p in db.TableRoot<Entity.tbSmsTo>()
                                       where ids.Contains(p.Id) && p.tbTenant.Id == model.TenantId
                                       select p).ToList();

                    foreach (var sms in tbSmsToList)
                    {
                        var smsStatus = model.SmsResponseList.Where(d => d.SmsId == sms.Id).FirstOrDefault();
                        if (smsStatus == null)
                        {
                            continue;
                        }
                        else
                        {
                            if (smsStatus.IsSuccess)
                            {
                                sms.Status = decimal.One;
                                sms.SendDate = string.IsNullOrEmpty(smsStatus.SendDate) ? DateTime.Now : smsStatus.SendDate.ConvertToDateTime();
                                sms.Retry = sms.Retry + 1;
                            }
                            else
                            {
                                sms.Status = -decimal.One;
                                sms.SendDate = string.IsNullOrEmpty(smsStatus.SendDate) ? DateTime.Now : smsStatus.SendDate.ConvertToDateTime();
                                sms.Retry = sms.Retry + 1;
                                sms.Remark = smsStatus.errorMsg;
                            }
                        }
                    }
                    db.SaveChanges();
                }
                return Json(new { IsSuccess = true });
            }
        }

        /// <summary>
        /// 更改状态
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public static void SetSmsStatusModel(List<SmsResponseModel> model)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var ids = model.Select(d => d.SmsId).ToList();
                var tbSmsToList = (from p in db.TableRoot<Entity.tbSmsTo>()
                                   join t in db.Set<Admin.Entity.tbTenant>() on p.tbTenant.Id equals t.Id
                                   where ids.Contains(p.Id) && p.IsDeleted == false && t.IsDefault
                                   select p).ToList();

                foreach (var sms in tbSmsToList)
                {
                    var smsStatus = model.Where(d => d.SmsId == sms.Id).FirstOrDefault();
                    if (smsStatus == null)
                    {
                        continue;
                    }
                    else
                    {
                        if (smsStatus.IsSuccess)
                        {
                            sms.Status = decimal.One;
                            sms.SendDate = string.IsNullOrEmpty(smsStatus.SendDate) ? DateTime.Now : smsStatus.SendDate.ConvertToDateTime();
                            sms.Retry = sms.Retry + 1;
                        }
                        else
                        {
                            sms.Status = -decimal.One;
                            sms.SendDate = string.IsNullOrEmpty(smsStatus.SendDate) ? DateTime.Now : smsStatus.SendDate.ConvertToDateTime();
                            sms.Retry = sms.Retry + 1;
                            sms.Remark = smsStatus.errorMsg;
                        }
                    }
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 待发短信
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public static List<Dto.SmsTo.List> GetSmsToSmsList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.TableRoot<Entity.tbSmsTo>()
                          join t in db.Set<Admin.Entity.tbTenant>() on p.tbTenant.Id equals t.Id
                          where (p.Status == decimal.Zero || p.Status == -decimal.One) && p.IsDeleted == false && t.IsDefault
                          && p.tbTenant.IsDeleted == false
                          && p.tbSms.PlanDate.Day == DateTime.Now.Day
                          && p.Retry <= 10
                          select p);

                var tbSmsToList = (from p in tb
                                   join m in db.TableRoot<Entity.tbSms>() on p.tbSms.Id equals m.Id into n
                                   from k in n.DefaultIfEmpty()
                                   orderby p.SendDate
                                   select new Dto.SmsTo.List
                                   {
                                       Id = p.Id,
                                       Mobile = p.Mobile,
                                       No = p.No,
                                       Remark = p.Remark,
                                       Retry = p.Retry,
                                       SmsId = p.tbSms.Id,
                                       SmsTitle = k == null ? "" : k.SmsTitle,
                                       Status = p.Status,
                                       SendDate = p.SendDate,
                                       SysUserName = p.tbSysUser.UserName
                                   }).Take(50).ToList();
                return tbSmsToList;
            }
        }

        /// <summary>
        /// 待发短信
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public static List<Dto.SmsTo.List> GetSmsToSmsOne(int smsId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.TableRoot<Entity.tbSmsTo>()
                          join t in db.Set<Admin.Entity.tbTenant>() on p.tbTenant.Id equals t.Id
                          where p.IsDeleted == false && t.IsDefault
                          && p.tbTenant.IsDeleted == false
                          && p.tbSms.PlanDate.Day == DateTime.Now.Day
                          where p.tbSms.Id == smsId
                          && p.Retry <= 10
                          select p);

                var tbSmsToList = (from p in tb
                                   join m in db.TableRoot<Entity.tbSms>() on p.tbSms.Id equals m.Id into n
                                   from k in n.DefaultIfEmpty()
                                   orderby p.SendDate
                                   select new Dto.SmsTo.List
                                   {
                                       Id = p.Id,
                                       Mobile = p.Mobile,
                                       No = p.No,
                                       Remark = p.Remark,
                                       Retry = p.Retry,
                                       SmsId = p.tbSms.Id,
                                       SmsTitle = k == null ? "" : k.SmsTitle,
                                       SmsPIN = k == null ? "" : k.SmsPIN,
                                       Status = p.Status,
                                       SendDate = p.SendDate,
                                       SysUserName = p.tbSysUser.UserName
                                   }).Take(50).ToList();
                return tbSmsToList;
            }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        public static void SmsSend()
        {
            var smsConfigInfo = SmsConfigController.SelectDefaultInfo();
            if (smsConfigInfo == null)
            {
                return;
            }
            else
            {
                var SmsList = GetSmsToSmsList();
                if (SmsList.Count <= decimal.Zero)
                {
                    return;
                }
                var SmsResponseList = new List<SmsResponseModel>();
                if (smsConfigInfo.SmsServerType == Code.EnumHelper.SmsServerType.KaiWei)
                {
                    //开维短信
                    #region 深圳开维
                    var service = new KVSMSService.SMSServiceClient();
                    var rand = service.getRandom();
                    var KvSmsUC = smsConfigInfo.SmsAccount;
                    var KvSmsPW = smsConfigInfo.SmsPassword;
                    foreach (var sms in SmsList)
                    {
                        var time = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string msg = Convert.ToBase64String(Encoding.UTF8.GetBytes(sms.SmsTitle));
                        var sendstatus = service.sendMsgs(KvSmsUC, rand + GetMD5(KvSmsPW) + GetMD5(KvSmsPW), "1", rand, sms.Mobile, msg, time, "");
                        var smsResponse = new SmsResponseModel();
                        if (sendstatus == "1")
                        {
                            //发送成功
                            smsResponse.SmsId = sms.Id;
                            smsResponse.IsSuccess = true;
                            smsResponse.SendDate = DateTime.Now.ToString();
                            SmsResponseList.Add(smsResponse);
                        }
                        else
                        {
                            #region 错误消息
                            switch (sendstatus)
                            {
                                case "-4":
                                    smsResponse.errorMsg = "开维短信：用户名或密码错误!";
                                    break;
                                case "-5":
                                    smsResponse.errorMsg = "开维短信：服务商还没有充值!";
                                    break;
                                case "-6":
                                    smsResponse.errorMsg = "开维短信：短信的余额不足!";
                                    break;
                                case "-12":
                                    smsResponse.errorMsg = "开维短信：系统超时!";
                                    break;
                                case "-225":
                                    smsResponse.errorMsg = "开维短信：传入参数不合法，数据格式不符!!";
                                    break;
                                default:
                                    smsResponse.errorMsg = "开维短信：发送失败!";
                                    break;
                            }
                            #endregion
                            //发送失败
                            smsResponse.SmsId = sms.Id;
                            smsResponse.IsSuccess = false;
                            smsResponse.SendDate = DateTime.Now.ToString();
                            SmsResponseList.Add(smsResponse);
                        }
                        Thread.Sleep(50);//停一下
                    }
                    #endregion
                }
                else if (smsConfigInfo.SmsServerType == Code.EnumHelper.SmsServerType.Aali)
                {
                    //阿里大于
                    #region 阿里大于
                    var TopSdkUrl = smsConfigInfo.SmsUrl;
                    var TopSdkAppkey = smsConfigInfo.SmsAccount;
                    var TopSdkSecret = smsConfigInfo.SmsPassword;
                    var TopSdkSmsType = "normal";
                    var TopSdkSmsFreeSignName = smsConfigInfo.SmsFreeSignName;
                    var TopSdkSmsTemplateCode = smsConfigInfo.SmsTemplateCode;
                    ITopClient client = new DefaultTopClient(TopSdkUrl, TopSdkAppkey, TopSdkSecret);
                    foreach (var sms in SmsList)
                    {
                        var req = new AlibabaAliqinFcSmsNumSendRequest();
                        req.Extend = sms.Id.ToString();
                        req.SmsType = TopSdkSmsType;
                        req.SmsFreeSignName = TopSdkSmsFreeSignName;
                        var smsParam = new SmsParam();
                        smsParam.SmsTitle = sms.SmsTitle == null ? "" : sms.SmsTitle;
                        req.SmsParam = Newtonsoft.Json.JsonConvert.SerializeObject(smsParam);
                        req.RecNum = sms.Mobile;
                        req.SmsTemplateCode = TopSdkSmsTemplateCode;
                        var rsp = client.Execute(req);
                        //返回结果
                        var smsResponse = new SmsResponseModel();
                        smsResponse.SmsId = sms.Id;
                        smsResponse.IsSuccess = !rsp.IsError;
                        smsResponse.SendDate = DateTime.Now.ToString();
                        smsResponse.errorMsg = rsp.SubErrMsg + "";
                        SmsResponseList.Add(smsResponse);
                        Thread.Sleep(50);//停一下
                    }
                    #endregion
                }
                else if (smsConfigInfo.SmsServerType == Code.EnumHelper.SmsServerType.None)
                {
                    //其它短信
                }
                if (SmsResponseList.Count > decimal.Zero)
                {
                    SetSmsStatusModel(SmsResponseList);
                }
            }
        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }

            return byte2String;
        }

        /// <summary>
        /// 发送验证码
        /// </summary>
        public static bool SmsSendCheckCode(int smsId = 0)
        {
            var smsConfigInfo = SmsConfigController.SelectDefaultInfo();
            if (smsConfigInfo == null)
            {
                return false;
            }
            else
            {
                var SmsList = GetSmsToSmsOne(smsId);
                if (SmsList.Count <= decimal.Zero)
                {
                    return false;
                }
                var SmsResponseList = new List<SmsResponseModel>();
                if (smsConfigInfo.SmsServerType == Code.EnumHelper.SmsServerType.KaiWei)
                {
                    //开维短信
                    #region 深圳开维
                    var service = new KVSMSService.SMSServiceClient();
                    var rand = service.getRandom();
                    var KvSmsUC = smsConfigInfo.SmsAccount;
                    var KvSmsPW = smsConfigInfo.SmsPassword;
                    foreach (var sms in SmsList)
                    {
                        var time = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string msg = Convert.ToBase64String(Encoding.UTF8.GetBytes(sms.SmsTitle));
                        var sendstatus = service.sendMsgs(KvSmsUC, rand + GetMD5(KvSmsPW) + GetMD5(KvSmsPW), "1", rand, sms.Mobile, msg, time, "");
                        var smsResponse = new SmsResponseModel();
                        if (sendstatus == "1")
                        {
                            //发送成功
                            smsResponse.SmsId = sms.Id;
                            smsResponse.IsSuccess = true;
                            smsResponse.SendDate = DateTime.Now.ToString();
                            SmsResponseList.Add(smsResponse);
                        }
                        else
                        {
                            #region 错误消息
                            switch (sendstatus)
                            {
                                case "-4":
                                    smsResponse.errorMsg = "开维短信：用户名或密码错误!";
                                    break;
                                case "-5":
                                    smsResponse.errorMsg = "开维短信：服务商还没有充值!";
                                    break;
                                case "-6":
                                    smsResponse.errorMsg = "开维短信：短信的余额不足!";
                                    break;
                                case "-12":
                                    smsResponse.errorMsg = "开维短信：系统超时!";
                                    break;
                                case "-225":
                                    smsResponse.errorMsg = "开维短信：传入参数不合法，数据格式不符!!";
                                    break;
                                default:
                                    smsResponse.errorMsg = "开维短信：发送失败!";
                                    break;
                            }
                            #endregion
                            //发送失败
                            smsResponse.SmsId = sms.Id;
                            smsResponse.IsSuccess = false;
                            smsResponse.SendDate = DateTime.Now.ToString();
                            SmsResponseList.Add(smsResponse);
                        }
                        Thread.Sleep(50);//停一下
                    }
                    #endregion
                }
                else if (smsConfigInfo.SmsServerType == Code.EnumHelper.SmsServerType.Aali)
                {
                    //阿里大于
                    #region 阿里大于
                    var TopSdkUrl = smsConfigInfo.SmsUrl;
                    var TopSdkAppkey = smsConfigInfo.SmsAccount;
                    var TopSdkSecret = smsConfigInfo.SmsPassword;
                    var TopSdkSmsType = "normal";
                    var TopSdkSmsFreeSignName = smsConfigInfo.SmsFreeSignName;
                    var TopSdkSmsTemplateCode = smsConfigInfo.SmsTemplateCode;
                    ITopClient client = new DefaultTopClient(TopSdkUrl, TopSdkAppkey, TopSdkSecret);
                    foreach (var sms in SmsList)
                    {
                        var req = new AlibabaAliqinFcSmsNumSendRequest();
                        req.Extend = sms.Id.ToString();
                        req.SmsType = TopSdkSmsType;
                        req.SmsFreeSignName = TopSdkSmsFreeSignName;
                        var smsParam = new SmsParam();
                        smsParam.SmsTitle = sms.SmsPIN == null ? "" : sms.SmsPIN;
                        req.SmsParam = Newtonsoft.Json.JsonConvert.SerializeObject(smsParam);
                        req.RecNum = sms.Mobile;
                        req.SmsTemplateCode = TopSdkSmsTemplateCode;
                        var rsp = client.Execute(req);
                        //返回结果
                        var smsResponse = new SmsResponseModel();
                        smsResponse.SmsId = sms.Id;
                        smsResponse.IsSuccess = !rsp.IsError;
                        smsResponse.SendDate = DateTime.Now.ToString();
                        smsResponse.errorMsg = rsp.SubErrMsg + "";
                        SmsResponseList.Add(smsResponse);
                        Thread.Sleep(50);//停一下
                    }
                    #endregion
                }
                else if (smsConfigInfo.SmsServerType == Code.EnumHelper.SmsServerType.None)
                {
                    //其它短信
                }
                if (SmsResponseList.Count > decimal.Zero)
                {
                    SetSmsStatusModel(SmsResponseList);
                    return true;
                }
                return false;
            }
        }
    }


    [DataContract]
    public class SmsSendRequestModel
    {
        [DataMember]
        public int TenantId { get; set; }

        [DataMember]
        public List<SmsResponse> SmsResponseList { get; set; }
    }

    [DataContract]
    public class SmsResponse
    {
        /// <summary>
        /// 短信Id
        /// </summary>
        [DataMember]
        public int SmsId { get; set; }

        /// <summary>
        /// 接收人员
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        [DataMember]
        public string errorMsg { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        [DataMember]
        public string SendDate { get; set; }
    }

    public class Sms
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        [Display(Name = "发送时间")]
        public DateTime SendDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        public decimal Status { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        [Display(Name = "重试次数")]
        public int Retry { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "备注信息")]
        public string Remark { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "所属短信")]
        public int SmsId { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "短信内容")]
        public string SmsTitle { get; set; }

        /// <summary>
        /// 接收人员
        /// </summary>
        [Display(Name = "接收人员")]
        public string SysUserName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public DateTime FromDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public DateTime ToDate { get; set; } = DateTime.Now;
    }

    public class SmsResponseModel
    {
        /// <summary>
        /// 短信Id
        /// </summary>
        [Display(Name = "短信Id")]
        public int SmsId { get; set; }

        /// <summary>
        /// 接收人员
        /// </summary>
        [Display(Name = "是否成功")]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        [Display(Name = "错误消息")]
        public string errorMsg { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        [Display(Name = "发送时间")]
        public string SendDate { get; set; }
    }

    public class SmsParam
    {
        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "短信内容")]
        public string SmsTitle { get; set; }
    }
}