using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Code
{
    public class StringHelper
    {
        public static string FormatValue(string str)
        {
            str = str.Trim();
            str = str.Replace("（", "(");
            str = str.Replace("）", ")");
            str = str.Replace("　", " ");
            return str;
        }

        ///   <summary>   
        ///    去除HTML标记   
        ///   </summary>   
        ///   <param    name="NoHTML">包括HTML的源码   </param>   
        ///   <returns>已经去除后的文字</returns>   
        public static string NoHTML(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"-->", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"<!--.*", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(amp|#38);", "&", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(lt|#60);", "<", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(gt|#62);", ">", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&#(\d+);", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();

            return Htmlstring;
        }

        public static string ToHtml(string s, bool nofollow = false)
        {
            s = HttpUtility.HtmlEncode(s);
            string[] paragraphs = s.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
            var sb = new System.Text.StringBuilder();
            foreach (string par in paragraphs)
            {
                sb.AppendLine("<p>");
                string p = par.Replace(Environment.NewLine, "<br />\r\n");
                if (nofollow)
                {
                    p = System.Text.RegularExpressions.Regex.Replace(p, @"\[\[(.+)\]\[(.+)\]\]", "<a href=\"$2\" rel=\"nofollow\">$1</a>");
                    p = System.Text.RegularExpressions.Regex.Replace(p, @"\[\[(.+)\]\]", "<a href=\"$1\" rel=\"nofollow\">$1</a>");
                }
                else
                {
                    p = System.Text.RegularExpressions.Regex.Replace(p, @"\[\[(.+)\]\[(.+)\]\]", "<a href=\"$2\">$1</a>");
                    p = System.Text.RegularExpressions.Regex.Replace(p, @"\[\[(.+)\]\]", "<a href=\"$1\">$1</a>");
                    sb.AppendLine(p);
                }
                sb.AppendLine("</p>");
            }

            return sb.ToString();
        }

        public static string HtmlToText(string strContent)
        {
            strContent = strContent.Replace("&amp", "&");
            strContent = strContent.Replace("''", "'");
            strContent = strContent.Replace("&lt", "<");
            strContent = strContent.Replace("&gt", ">");
            strContent = strContent.Replace("&lt", "chr(60)");
            strContent = strContent.Replace("&gt", "chr(37)");
            strContent = strContent.Replace("&quot", "\"");
            strContent = strContent.Replace(";", ";");
            strContent = strContent.Replace("<br/>", "\n");
            strContent = strContent.Replace("&nbsp;", " ");
            return strContent;
        }

        public static string TextToHtml(string strContent)
        {
            strContent = strContent.Replace("&", "&amp");
            strContent = strContent.Replace("'", "''");
            strContent = strContent.Replace("<", "&lt");
            strContent = strContent.Replace(">", "&gt");
            strContent = strContent.Replace("chr(60)", "&lt");
            strContent = strContent.Replace("chr(37)", "&gt");
            strContent = strContent.Replace("\"", "&quot");
            strContent = strContent.Replace(";", ";");
            strContent = strContent.Replace("\r\n", "<br/>");
            strContent = strContent.Replace("\n", "<br/>");
            strContent = strContent.Replace("  ", "&nbsp;&nbsp;");
            return strContent;
        }

        /// 转全角的函数(SBC case)
        ///
        ///任意字符串
        ///全角字符串
        ///
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///
        public static String ToSBC(String input)
        {
            // 半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new String(c);
        }

        /**/
        // /
        // / 转半角的函数(DBC case)
        // /
        // /任意字符串
        // /半角字符串
        // /
        // /全角空格为12288，半角空格为32
        // /其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        // /
        public static String ToDBC(String input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new String(c);
        }

        /// <summary>
        /// 剪切指定长度的字符串，并去掉HTML标记
        /// </summary>
        /// <param name="strr">要剪切字符串的Object形式</param>
        /// <param name="len">长度（中文长度为2)</param>
        /// <returns></returns>
        public static string CutStringX(object strr, int len)
        {
            string str = strr.ToString();
            string strRet = "";
            char CH;
            int nLen = str.Length;
            int nCutLen = 0;
            int nPos = 0;
            int bLeft = 0;
            int nChinese = 0;
            while (nPos < nLen && nCutLen < len)
            {
                CH = str[nPos];
                nPos++;

                if (CH == '<')
                {
                    bLeft++;
                    continue;
                }
                if (CH == '>')
                {
                    bLeft--;
                    continue;
                }
                if (nCutLen == 0 && CH.ToString() == " " && CH.ToString() == "\n")
                {
                    continue;
                }
                if (bLeft == 0)
                {
                    //是否为中文
                    if (IsChinese(CH))
                    {
                        nCutLen += 2;
                        nChinese++;
                    }
                    else
                    {
                        nCutLen += 1;
                    }
                    strRet += CH;
                }
            }
            strRet = strRet.Replace(" ", " ");
            if (nPos < nLen)
            {
                strRet += "";
                // strRet += "...";
            }
            return strRet;
        }


        //是否为中文
        public static bool IsChinese(char ch)
        {
            var en = new System.Text.ASCIIEncoding();
            byte[] b = en.GetBytes(ch.ToString());
            return (b[0] == 63);
        }

        public static string FormatFileSize(long? fileLength)
        {
            string strFileSize = String.Empty;

            if (fileLength != null)
            {
                if (fileLength < 1024)
                {
                    strFileSize = Convert.ToInt64(fileLength) + " Byte";
                }
                else if (fileLength >= 1024 && fileLength < 1048576)
                {
                    strFileSize = Math.Round((Convert.ToInt64(fileLength) / 1024.00), 2).ToString() + " KB";
                }
                else if (fileLength >= 1048576 && fileLength < 1073741824)
                {
                    strFileSize = Math.Round((Convert.ToInt64(fileLength) / 1024.00 / 1024.00), 2).ToString() + " MB";
                }
                else if (fileLength >= 1073741824)
                {
                    strFileSize = Math.Round((Convert.ToInt64(fileLength) / 1024.00 / 1024.00 / 1024.00), 2).ToString() + " GB";
                }
            }

            return strFileSize;
        }
    }
}