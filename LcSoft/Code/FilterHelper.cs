using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Code
{
    public class FilterHelper
    {
        //[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        //public class SsoAttribute : ActionFilterAttribute
        //{
        //}

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        public class KeygenAttribute : ActionFilterAttribute
        {
        }
    }
}