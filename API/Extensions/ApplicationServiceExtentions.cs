using System;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Repositorys;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AppApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Ajout de DbContext dans la collection de services
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            // Ajout du service CORS dans la collection de services
            services.AddCors();

            // Ajout du service TokenService dans la collection de services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<LogUserActivity>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<LikesRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();


            // Retour de la collection de services IServiceCollection modifiée pour permettre la méthode de chaînage.
            return services;
        }
    }
}
