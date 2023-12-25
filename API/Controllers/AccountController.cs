using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
      
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        private readonly UserManager<AppUser> _userManager;



        public AccountController(UserManager<AppUser> user, ITokenService tokenService, IMapper mapper)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _userManager = user;
        }

        // Register
        [HttpPost("register")] //POST: api/account/register
       public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
       {
            // Vérifie si le nom d'utilisateur existe déjà dans la base de données
            if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.UserName.ToLower();
            
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if(!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if(!roleResult.Succeeded) return BadRequest(result.Errors);


            return new UserDto
           {
               Username = user.UserName,
               Token = await  _tokenService.CreateToken(user),
               KnownAs = user.KnownAs,
               Gender = user.Gender
           };
       }

         // Login
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // Rechercher un utilisateur 
            var user = await  _userManager.Users
                .Include(p=>p.Photos)
                .SingleOrDefaultAsync(x=>x.UserName == loginDto.UserName);

            if(user == null) return Unauthorized("Le nom d'utilisateur est invalide !");

            // Vérifier le mot de passe
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if(!result) return Unauthorized("Le mot de passe est invalide !");

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender 

            };
        }

        // Vérification de nom d'utilisateur 
        private async Task<bool> UserExists(string username){
            return await _userManager.Users.AnyAsync(x=>x.UserName == username.ToLower());
        }
    }  
}
