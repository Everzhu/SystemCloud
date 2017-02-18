using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Code
{
    public class PageHelper
    {
        public string PageOrderBy
        {
            get;
            set;
        } = HttpContext.Current.Request["PageOrderBy"];

        public string PageOrderDesc
        {
            get;
            set;
        } = HttpContext.Current.Request["PageOrderDesc"];

        public int PageIndex
        {
            get;
            set;
        } = HttpContext.Current.Request["PageIndex"] == null ? 1 : HttpContext.Current.Request["PageIndex"].ConvertToInt();

        public int PageSize 
        {
            get;
            set;
        } = HttpContext.Current.Request["PageSize"] == null ? 10 : HttpContext.Current.Request["PageSize"].ConvertToInt();

        public int PageCount
        {
            get;
            set;
        } = 0;

        public int TotalCount
        {
            get;
            set;
        } = 0;

        public string PageName
        {
            get;
            set;
        } = "Page";
    }
}

public static class PageExtend
{
    public static List<TEntity> ToPageList<TEntity>(this IQueryable<TEntity> tb, XkSystem.Code.PageHelper page)
        where TEntity : class
    {
        page.TotalCount = tb.Count();

        return tb.Skip(page.PageSize * (page.PageIndex - 1)).Take(page.PageSize).ToList();
    }

    public static List<TEntity> ToPageList<TEntity>(this IEnumerable<TEntity> tb, XkSystem.Code.PageHelper page)
        where TEntity : class
    {
        page.TotalCount = tb.Count();

        return tb.Skip(page.PageSize * (page.PageIndex - 1)).Take(page.PageSize).ToList();
    }
}