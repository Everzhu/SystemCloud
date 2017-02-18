using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Code
{
    public class MvcHelper
    {
        public static JsonResult Post(List<string> error = null, string returnUrl = "", string message = "", bool isRefresh = true)
        {
            if (error != null)
            {
                error.RemoveAll(d => string.IsNullOrEmpty(d));
            }

            return new JsonResult()
            {
                Data = new
                {
                    Status = error != null ? (error.Count == decimal.Zero ? decimal.One : decimal.Zero) : decimal.One,
                    Message = error != null ? (error.Count == decimal.Zero ? message : string.Join("\r\n", error)) : message,
                    ReturnUrl = returnUrl ?? string.Empty,
                    IsRefresh = isRefresh
                },
                ContentEncoding = System.Text.Encoding.UTF8,
                ContentType = "text/plain"
            };
        }
    }
}

public static class MvcExtend
{
    /// <summary>
    /// 将枚举转换成SelectListItem，用户在前端显示
    /// </summary>
    /// <param name="enumType"></param>
    /// <returns></returns>
    public static List<System.Web.Mvc.SelectListItem> ToItemList(this Type enumType)
    {
        if (enumType.IsEnum)
        {
            var list = (from Enum p in Enum.GetValues(enumType)
                        orderby p.GetOrder()
                        select new System.Web.Mvc.SelectListItem()
                        {
                            Text = p.GetDescription(),
                            Value = Convert.ToInt32(p).ToString(),
                            Selected = (Enum.Equals(enumType, p) ? true : false)
                        }).ToList();

            return list;
        }
        else
        {
            return new List<System.Web.Mvc.SelectListItem>();
        }
    }

    public static void AddError(this List<string> errorList, string error)
    {
        if (string.IsNullOrEmpty(error) == false)
        {
            errorList.Add(error);
        }
    }

    public static IQueryable<TEntity> Table<TEntity>(this XkSystem.Models.DbContext db)
     where TEntity : XkSystem.Code.EntityHelper.EntityBase
    {
        return db.Set<TEntity>().Where(d => d.IsDeleted == false && d.tbTenant.Id == XkSystem.Code.Common.TenantId);
    }

    public static IQueryable<TEntity> Table<TEntity>(this XkSystem.Models.DbContext db, int TenantId)
     where TEntity : XkSystem.Code.EntityHelper.EntityBase
    {
        return db.Set<TEntity>().Where(d => d.IsDeleted == false && d.tbTenant.Id == TenantId);
    }

    public static IQueryable<TEntity> TableRoot<TEntity>(this XkSystem.Models.DbContext db)
    where TEntity : XkSystem.Code.EntityHelper.EntityRoot
    {
        return db.Set<TEntity>().Where(d => d.IsDeleted == false);
    }
}

public static class HtmlLabelExtensions
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString LabelForRequired<TModel, TValue>(this HtmlHelper<TModel> html, System.Linq.Expressions.Expression<Func<TModel, TValue>> expression, string labelText = "")
    {
        return LabelHelper(html,
            ModelMetadata.FromLambdaExpression(expression, html.ViewData),
            ExpressionHelper.GetExpressionText(expression), labelText);
    }

    private static MvcHtmlString LabelHelper(HtmlHelper html, ModelMetadata metadata, string htmlFieldName, string labelText)
    {
        if (string.IsNullOrEmpty(labelText))
        {
            labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
        }

        if (string.IsNullOrEmpty(labelText))
        {
            return MvcHtmlString.Empty;
        }

        bool isRequired = false;

        if (metadata.ContainerType != null)
        {
            //isRequired = metadata.ContainerType.GetProperty(metadata.PropertyName)
            //                .GetCustomAttributes(typeof(RequiredAttribute), false)
            //                .Length == 1;
            var req = metadata.ContainerType.GetProperty(metadata.PropertyName).GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute), false);
            if (req.Length > decimal.Zero && req[0].GetType().Name == "RequiredAttribute")
            {
                isRequired = true;
            }
        }

        TagBuilder tag = new TagBuilder("label");
        tag.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));

        if (isRequired)
            tag.Attributes.Add("class", "label-required");

        tag.SetInnerText(labelText);

        var output = tag.ToString(TagRenderMode.Normal);

        //return MvcHtmlString.Create(output + "：");
        return MvcHtmlString.Create(output);
    }
}

public static class HtmlExtensions
{
    public static System.Web.Mvc.MvcHtmlString SortHeader(string text, string value, string orderBy, string orderdesc)
    {
        var str = "<a href='#'";
        if (!string.IsNullOrEmpty(orderdesc))
        {
            str = str + " class='dropup'";
        }
        str = str + " onclick=\"OrderTo('" + value + "')\">";
        str = str + text;
        if (orderBy == value)
        {
            str = str + "<span class='caret'></span>";
        }
        str = str + "</a>";
        return new System.Web.Mvc.MvcHtmlString(str);
    }
}