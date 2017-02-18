using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ICSharpCode.SharpZipLib.Zip;

namespace XkSystem.Code
{
    public class ZipHelper
    {
        public static void CreateZip(string zipFileName, string sourceDirectory, bool recurse = true, string fileFilter = "")
        {
            FastZip fastZip = new FastZip();

            fastZip.CreateZip(zipFileName, sourceDirectory, recurse, fileFilter);
        }

        /// <summary>
        /// 快速解压
        /// </summary>
        /// <param name="zipFilePath">压缩文件路径</param>
        /// <param name="extractPath">解压路径</param>
        public static string ExtractZip(string zipFilePath, string extractPath)
        {
            try
            {
                FastZip zip = new FastZip();
                zip.ExtractZip(zipFilePath, extractPath, "");
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}