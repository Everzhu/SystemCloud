using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XkSystem.Areas.Moral.Dto.MoralData;

namespace XkSystem.Areas.Moral.Models.MoralData
{
    public class OnceEdit
    {

        public int MoralId { get; set; } = HttpContext.Current.Request["MoralId"].ConvertToInt();

        public Dto.MoralData.OnceEdit MoralDataEdit { get; set; } = new Dto.MoralData.OnceEdit();

        public List<Student.Dto.Student.Info> StudentList = new List<Student.Dto.Student.Info>();

        public List<SelectListItem> MoralList { get; set; } = new List<SelectListItem>();

        public List<Dto.MoralClass.Info> MoralClassList { get; set; } = new List<Dto.MoralClass.Info>();

        public List<Dto.MoralGroup.Info> MoralGroupList { get; set; } = new List<Dto.MoralGroup.Info>();

        public List<Dto.MoralItem.List> MoralItemList { get; set; } = new List<Dto.MoralItem.List>();

        public List<Dto.MoralOption.List> MoralOptionList { get; set; } = new List<Dto.MoralOption.List>();

        public List<Dto.MoralData.OnceList> MoralDataList { get; internal set; } = new List<Dto.MoralData.OnceList>();

        public Code.EnumHelper.MoralType MoralType { get; set; } = Code.EnumHelper.MoralType.Many;

        public bool DataIsNull { get; set; }
    }
}