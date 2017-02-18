using System.Web.Mvc;

namespace XkSystem.Areas.Disk
{
    public class DiskAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Disk";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Disk_default",
                "Disk/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}