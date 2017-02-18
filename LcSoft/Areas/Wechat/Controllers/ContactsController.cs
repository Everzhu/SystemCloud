using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Wechat.Controllers
{
    public class ContactsController : Controller
    {
        /// <summary>
        /// 验证微信接口
        /// </summary>
        /// <param name="echostr"></param>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        [AllowAnonymous]
        public void GetWeChatAccessToken(string echostr, string signature, string timestamp, string nonce)
        {
            Code.LogHelper.Info("echostr:" + echostr + "；signature:" + signature + "；timestamp:" + timestamp + "；nonce:" + nonce);
            //如果是微信接入验证
            if (Code.WeChatHelper.IsWeChatSwitchingIn(echostr, signature, timestamp, nonce, Code.WeChatConfig.TOKEN))
            {
                //输出echostr
                HttpContext.Response.Write(echostr);
            }
            else
            {
                HttpContext.Response.Write("");
            }

            HttpContext.Response.End();
        }

        // GET: Wechat/Contacts
        public ActionResult ContactsSelect()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                Models.ContactsModel vm = GetTeacherList(db);

                return View(vm);
            }
        }

        public ActionResult ContactsSelectForOffice()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                Models.ContactsModel vm = GetTeacherList(db);

                return View(vm);
            }
        }

        // GET: Wechat/Contacts
        public ActionResult ContactsIndex()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                Models.ContactsModel vm = GetTeacherList(db);

                return View(vm);
            }
        }

        private static Models.ContactsModel GetTeacherList(XkSystem.Models.DbContext db)
        {
            var vm = new Models.ContactsModel();
            vm.ContactList = (from p in db.TableRoot<Teacher.Entity.tbTeacher>()
                              select new Dto.ContactListDto
                              {
                                  Id = p.Id,
                                  TeacherCode = p.TeacherCode,
                                  TeacherName = p.TeacherName,
                                  Mobile = p.tbSysUser.Mobile,
                                  UserId = p.tbSysUser.Id
                              }).ToList().OrderBy(m => m.Group).ToList();
            foreach (var item in vm.ContactList)
            {
                var sysuser = Areas.Sys.Controllers.SysUserController.Info(item.UserId);
                if (!String.IsNullOrWhiteSpace(sysuser.Photo))
                {
                    item.HeadImg = "/Files/UserPhoto/" + sysuser.Photo;
                }
            }

            return vm;
        }
    }
}