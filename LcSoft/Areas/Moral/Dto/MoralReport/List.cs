using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralReport
{
    public class List
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        

        public List<ClassDetail> ClassDataList { get; set; } = new List<ClassDetail>();

        public List<StudentDetail> StudentDataList { get; set; } = new List<StudentDetail>();

        public List<ClassGroupDetail> ClassGroupDataList { get; set; } = new List<ClassGroupDetail>();

    }

    public class DetailBase
    {
        public int ClassId { get; set; }
        public int MoralItemId { get; set; }
        public string MoralItemName { get; set; }
        public DateTime Date { get; set; }

        public int MoralDataId { get; set; }

        public Code.EnumHelper.MoralItemOperateType MoralItemOperateType { get; set; }

        public decimal Score { get; set; }

        public string Comment { get; set; }

        public string Reason { get; set; }
    }

    public class ClassDetail : DetailBase
    {   
     
    }
    public class StudentDetail:DetailBase
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
    }

    public class ClassGroupDetail : DetailBase
    {
        public int ClassGroupId { get; set; }
        public string ClassGroupName { get; set; }
    }

}