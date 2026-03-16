using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using ProyectoPlataformaCursos.Repositories.Interfaces;
using System.Security.Claims;

namespace ProyectoPlataformaCursos.Policies
{
    public class TieneCursosRequirement : AuthorizationHandler<TieneCursosRequirement>, IAuthorizationRequirement
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TieneCursosRequirement requirement)
        {
            if (context.User.IsInRole("ADMIN"))
            {
                context.Succeed(requirement);
                return;
            }

            if (context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier) == false)
            {
                context.Fail();
                return;
            }

            var httpContext = context.Resource switch
            {
                Microsoft.AspNetCore.Http.HttpContext currentHttpContext => currentHttpContext,
                AuthorizationFilterContext authorizationFilterContext => authorizationFilterContext.HttpContext,
                _ => null
            };

            if (httpContext == null)
            {
                context.Fail();
                return;
            }

            var repo = httpContext.RequestServices.GetService<ICursoRepository>();
            if (repo == null)
            {
                context.Fail();
                return;
            }

            string idString = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idString, out int idUsuario))
            {
                var cursos = await repo.GetByProfesorAsync(idUsuario);
                if (cursos != null && cursos.Any())
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            else
            {
                context.Fail();
            }
        }
    }
}