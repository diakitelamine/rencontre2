using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;

        // On injecte IConfiguration pour obtenir la clé de jeton de configuration
        public TokenService(IConfiguration config)
        {
            // On crée une nouvelle clé de sécurité symétrique en utilisant la clé de jeton de configuration
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        // Méthode pour créer un jeton JWT pour un utilisateur donné
        public string CreateToken(AppUser user)
        {
            // On crée une liste de revendications qui seront incluses dans le jeton JWT
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            // On crée des informations d'identification en utilisant notre clé de sécurité symétrique
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            // On configure le descripteur de jeton JWT avec des informations d'identification et des revendications
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            // On crée un gestionnaire de jeton JWT
            var tokenHandler = new JwtSecurityTokenHandler();

            // On crée le jeton JWT à partir du descripteur de jeton
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // On retourne le jeton JWT sous forme de chaîne
            return tokenHandler.WriteToken(token);
        }
    }
}
