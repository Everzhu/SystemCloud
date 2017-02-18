using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Models
{
    public class ContactsModel
    {
        public List<Dto.ContactListDto> ContactList { get; set; } = new List<Dto.ContactListDto>();
    }
}