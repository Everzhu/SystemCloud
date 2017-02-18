using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityPhoto
{
    public class List
    {
        public List<Entity.tbQualityPhoto> QualityPhotoList { get; set; } = new List<Entity.tbQualityPhoto>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}