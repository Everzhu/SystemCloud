using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Code
{
    /// <summary>
    /// 本类所有和周相关的操作默认每周的第一天为星期一
    /// </summary>
    public class DateHelper
    {
        public static DateTime MinDate = DateTime.Parse("1900-01-01");

        /// <summary>
        /// 本月第一天
        /// </summary>
        public static DateTime MonthFirstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        /// <summary>
        /// 本月最后一天  23:59:59
        /// </summary>
        public static DateTime MonthLastDay = MonthFirstDay.AddMonths(1).AddSeconds(-1);

        private static string[] WeekName = new string[] {"周日","周一", "周二", "周三", "周四", "周五", "周六"};

        /// <summary>
        /// 获取制定日期是周几
        /// </summary>
        /// <param name="date"></param>
        /// <returns>周一</returns>
        public static string GetWeekName(DateTime date)
        {
            return WeekName[(int)date.DayOfWeek];
        }

        /// <summary>
        /// 根据年份及周数，获取该周的第一天
        /// </summary>
        /// <param name="year"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeByWeekNumOfYear(int year, int num)
        {
            DateTime day = new DateTime(year, 1, 1);
            return day.AddDays(7 * (num - 1) - ((int)day.DayOfWeek % 7 == 0 ? 0 : (int)day.DayOfWeek));
        }


        /// <summary>
        /// 根据年份获取总周数
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int GetWeekCountByYear(int year)
        {
            DateTime date = new DateTime(year, 1, 1);
            //得到该年的第一天是周几 
            int startWeekDay = Convert.ToInt32(date.AddYears(1).AddDays(-1).DayOfWeek);

            //总天数
            int countDay = date.AddYears(1).AddDays(-1).DayOfYear;

            var endWeekDay = date.AddDays(countDay - 1).DayOfWeek;

            //当年总周数
            var weekCount = countDay / 7 + (startWeekDay == 0 && endWeekDay == DayOfWeek.Saturday ? 0 :1);

            return weekCount;
        }


        /// <summary>
        /// 判断传入日期属于当年的第几周
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetWeekNumOfYearByDate(DateTime dt)
        {
            int days = dt.DayOfYear + (7 - (dt.DayOfWeek== DayOfWeek.Sunday?7:(int)dt.DayOfWeek));
            return days / 7 + (days % 7 == 0 ? 0 : 1);
        }

        /// <summary>
        /// 判断传入日期属于当年的第几周
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        //public static int WeekOfYear(DateTime currentDate)
        //{
        //    int firstdayofweek = Convert.ToInt32(Convert.ToDateTime(currentDate.Year.ToString() + "- " + "1-1 ").DayOfWeek);

        //    int days = currentDate.DayOfYear;
        //    int daysOutOneWeek = days - (7 - firstdayofweek);

        //    if (daysOutOneWeek <= 0)
        //    {
        //        return 1;
        //    }
        //    else
        //    {
        //        int weeks = daysOutOneWeek / 7;
        //        if (daysOutOneWeek % 7 != 0)
        //            weeks++;

        //        return weeks + 1;
        //    }
        //}


        ///<summary>
        /// 得到一年中的某周的起始日和截止日
        /// </summary>
        /// <param name="nYear">年</param>
        /// <param name="nNumWeek">周数</param>
        /// <param name="dtWeekStart">周始日期</param>
        /// <param name="dtWeekeEnd">周终日期</param>
        public static void GetWeekDate(int nYear, int nNumWeek, out DateTime dtWeekStart, out DateTime dtWeekeEnd)
        {
            DateTime date = new DateTime(nYear, 1, 1);
            date = date + new TimeSpan((nNumWeek - 1) * 7, 0, 0, 0);
            dtWeekStart = date.AddDays(-(date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek)+1);
            //dtWeekeEnd = date.AddDays((int)DayOfWeek.Saturday - (int)date.DayOfWeek );
            dtWeekeEnd = dtWeekStart.AddDays(6);
        }


        /// <summary>
        /// 根据年份获取该年所有周数及对应周的起始日期
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static List<System.Web.Mvc.SelectListItem> GetWeekDateList(int year)
        {
            var list = new List<System.Web.Mvc.SelectListItem>();

            var weekCount = GetWeekCountByYear(year);
            DateTime startDate;
            DateTime endDate;
            for (var i = 1; i <= weekCount; i++)
            {
                GetWeekDate(year, i, out startDate, out endDate);
                list.Add(new System.Web.Mvc.SelectListItem()
                {
                    Value = i.ToString(),
                    Text = $"第[{i}]周 {startDate.ToString(Code.Common.StringToDate)}至{endDate.ToString(Common.StringToDate)}",
                    Selected = DateTime.Now >= startDate && DateTime.Now <= endDate
                });
            }

            return list;
        }


    }
}
