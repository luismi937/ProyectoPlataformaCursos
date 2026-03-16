using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ProyectoPlataformaCursos.Policies
{
    public class AntiguedadRequirement : AuthorizationHandler<AntiguedadRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AntiguedadRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == "FechaRegistro") == false)
            {
                context.Fail();
            }
            else
            {
                string data = context.User.Claims.FirstOrDefault(c => c.Type == "FechaRegistro").Value;
                if (DateTime.TryParse(data, out DateTime fechaRegistro))
                {
                    if (fechaRegistro < DateTime.Now.AddDays(1)) // Ejemplo: cualquier usuario registrado antes de mañana pasa
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}