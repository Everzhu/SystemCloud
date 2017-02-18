using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Dto
{
    public class ContactListDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 教师编码
        /// </summary>
        public string TeacherCode { get; set; }
        /// <summary>
        /// 教师名称
        /// </summary>
        public string TeacherName { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImg { get; set; } = "/Content/Images/a5.jpg";

        public int UserId { get; set; }
        /// <summary>
        /// 拼音首字母
        /// </summary>
        public char Group
        {
            get
            {
                return ChineseToSpell.CharacterConvertString(TeacherName)[0];
            }
        }
    }
}