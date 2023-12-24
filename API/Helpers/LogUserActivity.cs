using System.Security.Claims;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
         //Mise à jour de la date de dernière activité de l'utilisateur
         public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
         {
             var resultContext = await next();
             if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
             var userId = resultContext.HttpContext.User.GetUserId();
             var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
             var user = await repo.GetUserByIdAsync(userId);
             user.LastActive = DateTime.Now;
             await repo.SaveAllAsync();
         }
        
    }
}