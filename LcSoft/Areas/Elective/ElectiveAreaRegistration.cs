using System.Web.Mvc;

namespace XkSystem.Areas.Elective
{
    public class ElectiveAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Elective";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Elective_default",
                "Elective/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}