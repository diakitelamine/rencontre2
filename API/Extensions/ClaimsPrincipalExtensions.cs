using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // Récupère le username du token
        public static string GetUsername(this ClaimsPrincipal user)
        {
            // Récupère le username du token
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        
        public static int GetUserId(this ClaimsPrincipal user)
        {
            // Récupère le username du token
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

    }
}