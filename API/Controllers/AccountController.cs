using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using AutoMapper;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _tokenService = tokenService;
            _context = context;
            _mapper = mapper;
        }

        // Register
        [HttpPost("register")] //POST: api/account/register
       public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
       {
            // Vérifie si le nom d'utilisateur existe déjà dans la base de données
            if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registerDto);

            // Créer une instance de HMACSHA512 pour hacher le mot de passe
             using var hmac = new HMACSHA512();

            user.UserName = registerDto.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            // Ajoute l'utilisateur à la base de données
             _context.Users.Add(user);
             await _context.SaveChangesAsync();

            return new UserDto
           {
               Username = user.UserName,
               Token = _tokenService.CreateToken(user),
               KnownAs = user.KnownAs,
               Gender = user.Gender
           };
       }

         // Login
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // Rechercher un utilisateur 
            var user = await  _context.Users
                .Include(p=>p.Photos)
                .SingleOrDefaultAsync(x=>x.UserName == loginDto.UserName);

            // On verifie si l'utilisateur existe dans la  base de données 
            if(user == null) return Unauthorized("Le nom d'utilisateur est invalide !");

            // Créer une instance de HMACSHA512 avec le sel de l'utilisateur pour hacher le mot de passe de l'objet LoginDto
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // Compare les hachages pour vérifier si les mots de passe correspondent
            for(int i = 0 ; i < computedHash.Length; i++){
              
              if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Le mot passe est invalide !");

            }

            // Si les informations d'identification sont valides, retourne un objet UserDto avec le nom d'utilisateur et le jeton d'accès
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender 

            };
        }

        // Vérification de nom d'utilisateur 
        private async Task<bool> UserExists(string username){
            return await _context.Users.AnyAsync(x=>x.UserName == username.ToLower());
        }
    }  
}
