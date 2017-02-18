using System.Web.Mvc;

namespace XkSystem.Areas.Perform
{
    public class PerformAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Perform";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Perform_default",
                "Perform/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}