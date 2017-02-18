using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Code
{
    public static class LinqSort
    {
        /// <summary>
        /// Linq动态排序
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="source">要排序的数据源</param>
        /// <param name="value">排序依据（加空格）排序方式</param>
        /// <returns>IOrderedQueryable</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string value)
        {
            string[] arr = value.Split(' ');
            string Name = arr[1].ToUpper() == "DESC" ? "OrderByDescending" : "OrderBy";
            return ApplyOrder<T>(source, arr[0], Name);
        }

        /// <summary>
        /// Linq动态排序再排序
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="source">要排序的数据源</param>
        /// <param name="value">排序依据（加空格）排序方式</param>
        /// <returns>IOrderedQueryable</returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string value)
        {
            string[] arr = value.Split(' ');
            string Name = arr[1].ToUpper() == "DESC" ? "ThenByDescending" : "ThenBy";
            return ApplyOrder<T>(source, arr[0], Name);
        }

        static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            var type = typeof(T);
            var arg = System.Linq.Expressions.Expression.Parameter(type, "a");
            var pi = type.GetProperty(property);
            if (pi == null)
            {
                //如果排序字段不存在，则不做排序返回（注释的那种是按默认第一种进行排序，感觉不是很好）
                //pi = type.GetProperties()[0];
                return (IOrderedQueryable<T>)source;
            }
            var expr = System.Linq.Expressions.Expression.Property(arg, pi);
            type = pi.PropertyType;
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            var lambda = System.Linq.Expressions.Expression.Lambda(delegateType, expr, arg);
            var result = typeof(Queryable).GetMethods().Single(
                a => a.Name == methodName
                    && a.IsGenericMethodDefinition
                    && a.GetGenericArguments().Length == 2
                    && a.GetParameters().Length == 2).MakeGenericMethod(typeof(T), type).Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }
    }
}