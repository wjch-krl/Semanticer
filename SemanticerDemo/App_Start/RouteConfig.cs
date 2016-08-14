using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SemanticerDemo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Semantic",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Semantic", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
