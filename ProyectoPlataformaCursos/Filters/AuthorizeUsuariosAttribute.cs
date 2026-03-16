using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ProyectoPlataformaCursos.Filters
{
    public class AuthorizeUsuariosAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                try
                {
                    string controller = context.RouteData.Values["controller"]?.ToString();
                    string action = context.RouteData.Values["action"]?.ToString();
                    var ruta = context.RouteData.Values["id"];

                    ITempDataProvider provider = context.HttpContext.RequestServices.GetService<ITempDataProvider>();
                    if (provider != null)
                    {
                        var tempData = provider.LoadTempData(context.HttpContext);
                        tempData["controller"] = controller;
                        tempData["action"] = action;

                        if (ruta != null)
                        {
                            tempData["id"] = ruta.ToString();
                        }
                        else
                        {
                            tempData.Remove("id");
                        }

                        provider.SaveTempData(context.HttpContext, tempData);
                    }
                }
                catch
                {
                }

                context.Result = GetRoute("Account", "Login");
            }
        }

        private RedirectToRouteResult GetRoute(string controller, string action)
        {
            RouteValueDictionary ruta = new RouteValueDictionary(new
            {
                controller = controller,
                action = action
            });
            return new RedirectToRouteResult(ruta);
        }
    }
}