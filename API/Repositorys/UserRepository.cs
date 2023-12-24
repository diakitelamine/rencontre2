
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repositorys
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {      _mapper = mapper;
               _context = context;
        }

        public async Task<MemberDto> GetMemberAsyn(string username)
        {
            return await _context.Users
                .Where(x=> x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        //Retourner une liste d'utilisateurs
        public async Task<PagedList<MemberDto>> GetMembersAsync( UserParams userParams)
        {
          
          var query =  _context.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u=>u.Gender == userParams.Gender);
             //Filtrer les utilisateurs par âge
            var minDob = DateOnly.FromDateTime(System.DateTime.Today.AddYears(-userParams.MaxAge-1));
            var maxDob = DateOnly.FromDateTime(System.DateTime.Today.AddYears(-userParams.MinAge));
            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u=>u.Created),
                _ => query.OrderByDescending(u=>u.LastActive)
            };


            return await PagedList<MemberDto>.CreateAsync(
                query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider), 
                userParams.PageNumber,
                userParams.PageSize);
                      
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
           return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string UserName)
        {
            return await _context.Users
                .Include(p =>p.Photos)
                .SingleOrDefaultAsync(x=>x.UserName == UserName);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
               .Include(p=>p.Photos)
               .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync()> 0;
        }

        public void Update(AppUser user)
        {
             _context.Entry(user).State = EntityState.Modified;
        }
    }
}