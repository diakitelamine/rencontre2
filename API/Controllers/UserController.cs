using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//Définir le namespace pour la classe
namespace API.Controllers
{
    
     [Authorize]
    //Attribuer un contrôleur API à la classe UsersController
    public class UsersController : BaseApiController
    {
        //Définir un champ privé pour stocker l'objet DataContext
         
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        private readonly IPhotoService _photoService;

        //Créer un constructeur pour initialiser le champ _context
        public UsersController(IUserRepository userRepository, IMapper mapper,
         IPhotoService photoService)
        {
              _photoService = photoService;
              _mapper = mapper;
              _userRepository = userRepository;
            
        }

        //Définir une méthode qui gère une requête HTTP GET pour obtenir tous les utilisateurs
        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers( [FromQuery] UserParams userParams)
        {
            
          var currentUser = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            userParams.CurrentUsername = currentUser.UserName;

            if(string.IsNullOrEmpty(userParams.Gender)){
                userParams.Gender = currentUser.Gender == "male"? "femele" : "male";
            }
           var users = await _userRepository.GetMembersAsync(userParams);
           Response.AddPaginationHeader(users.CurrentPage, users.PageSize,
            users.TotalCount, users.TotalPages);
            return Ok(users);
        }
       
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser( string username)
        {
            //Utiliser la méthode ToListAsync pour obtenir tous les utilisateurs dans la base de données
            return   await  _userRepository.GetMemberAsyn(username);
            
        }

       //Mise a jour des données de l'utilisateur
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //Utiliser la méthode ToListAsync pour obtenir tous les utilisateurs dans la base de données
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if(user == null) return NotFound();
            _mapper.Map(memberUpdateDto, user);
            _userRepository.Update(user);
            if(await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");
            
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            //Utiliser la méthode ToListAsync pour obtenir tous les utilisateurs dans la base de données
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);

            if(result.Error != null) return BadRequest(result.Error.Message);

            //Création d'une photo 
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId

            };

            // On verifie si l'utilisateur n'a pas de photo princiapale 
            if(user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            if(await _userRepository.SaveAllAsync())
            {
                // Pour retourner une réponse 201 avec un header location
                return CreatedAtAction(nameof(GetUser), new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("Problem adding photo");
            
        }

        //Defintion de la photo princiapele
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if(photo.IsMain) return BadRequest("This is already your main photo");
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if(currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            if(await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to set main photo");
            
        }

        //Suppression d'une photo
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            //Utiliser la méthode ToListAsync pour obtenir tous les utilisateurs dans la base de données
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if(photo == null) return NotFound();
            if(photo.IsMain) return BadRequest("You cannot delete your main photo");
            if(photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if(await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Failed to delete the photo");
            
        }

   }
}
