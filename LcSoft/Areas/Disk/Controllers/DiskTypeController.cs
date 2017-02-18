using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Disk.Controllers
{
    public class DiskTypeController : Controller
    {
        [NonAction]
        public static List<Dto.DiskType.Info> SelectInfoList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskType>()
                          select new Dto.DiskType.Info
                          {
                              Id = t.Id,
                              DiskTypeName = t.DiskTypeName,
                              DiskType = t.DiskType
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static Dto.DiskType.Info SelectInfo(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskType>()
                          where t.Id == id
                          select new Dto.DiskType.Info
                          {
                              Id = t.Id,
                              DiskTypeName = t.DiskTypeName,
                              DiskType = t.DiskType
                          }).FirstOrDefault();
                return tb;
            }
        }

        [NonAction]
        public static Dto.DiskType.Info SelectByEnumDiskType(Code.EnumHelper.DiskType diskType)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Disk.Entity.tbDiskType>()
                          where t.DiskType == diskType
                          select new Dto.DiskType.Info
                          {
                              Id = t.Id,
                              DiskTypeName = t.DiskTypeName,
                              DiskType = t.DiskType
                          }).FirstOrDefault();
                return tb;
            }
        }
    }
}