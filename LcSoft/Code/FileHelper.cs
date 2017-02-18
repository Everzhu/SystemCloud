using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace XkSystem.Code
{
    public class FileHelper
    {
        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                string[] fileList = Directory.GetFileSystemEntries(path);

                //foreach (string file in fileList)
                //{
                //    if (Directory.Exists(file))
                //    {
                //        DeleteDirectory(path + Path.GetFileName(file));
                //    }
                //    else
                //    {
                //        File.Delete(path + Path.GetFileName(file));
                //    }
                //}

                Directory.Delete(path, true);
            }
        }
    }
}