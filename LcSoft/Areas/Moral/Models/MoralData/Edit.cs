using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralData
{
    public class Edit
    {

        public int MoralId { get; set; } = System.Web.HttpContext.Current.Request["MoralId"].ConvertToInt();

        public List<Student.Dto.Student.Info> StudentList { get; set; } = new List<Student.Dto.Student.Info>();

        public List<Basis.Dto.ClassGroup.Info> StudentGroupList { get; set; } = new List<Basis.Dto.ClassGroup.Info>();

        public List<Dto.MoralClass.Info> MoralClassList { get; set; } = new List<Dto.MoralClass.Info>();
        public List<SelectListItem> MoralClassListItem { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> MoralList { get; set; } = new List<SelectListItem>();

        public List<Dto.MoralGroup.Info> MoralGroupList { get; set; } = new List<Dto.MoralGroup.Info>();

        public List<Dto.MoralItem.List> MoralItemList { get; set; } = new List<Dto.MoralItem.List>();

        public List<Dto.MoralData.List> MoralDataList { get; internal set; } = new List<Dto.MoralData.List>();

        public List<SelectListItem> MoralItemKind { get; set; } = typeof(Code.EnumHelper.MoralItemKind).ToItemList();

        public int? KindId { get; set; } = HttpContext.Current.Request["KindId"].ConvertToIntWithNull();

        public Code.EnumHelper.MoralItemKind Kind { get; set; } = Code.EnumHelper.MoralItemKind.Class;

        public DateTime? MoralDate { get; set; } = HttpContext.Current.Request["MoralDate"].ConvertToDateTime();

        public int? ClassId{ get; set; } = HttpContext.Current.Request["ClassId"].ConvertToInt();

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public Code.EnumHelper.MoralType MoralType { get; set; } = Code.EnumHelper.MoralType.Many;

        public List<MoralPower.Info> MoralPowerClass { get; set; } = new List<MoralPower.Info>();

        public bool DataIsNull { get; set; } = false;

        public bool MoralItemIsNull { get; set; } = false;
    }
}