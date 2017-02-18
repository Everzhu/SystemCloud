using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XkSystem.Areas.Student.Dto.Student;

namespace XkSystem.Code
{
    public class StudentComparer : IEqualityComparer<Areas.Student.Dto.Student.Info>
    {
        public bool Equals(Info x, Info y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
            {
                return false;
            }

            return x.Id == y.Id && x.StudentName == y.StudentName;
             
        }

        public int GetHashCode(Info obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;
            int hashStudentId = obj.Id.GetHashCode();
            int hashStudentNameCode = obj.StudentName.GetHashCode();
            return hashStudentId ^ hashStudentNameCode;
        }
    }


    public class MoralItemComparer : IEqualityComparer<Areas.Moral.Dto.MoralItem.Info>
    {
        public bool Equals(Areas.Moral.Dto.MoralItem.Info x, Areas.Moral.Dto.MoralItem.Info y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
            {
                return false;
            }

            return x.Id == y.Id && x.MoralItemName == y.MoralItemName;

        }

        public int GetHashCode(Areas.Moral.Dto.MoralItem.Info obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;
            int hashMoralId = obj.Id.GetHashCode();
            int hashMoralNameCode = obj.MoralItemName.GetHashCode();
            return hashMoralId ^ hashMoralNameCode;
        }
    }



}