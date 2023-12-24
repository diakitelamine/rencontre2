using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {

        private readonly Cloudinary _cloudinary;

        // IOptions permet de récupérer les valeurs de CloudinarySettings
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                // ouvre un stream pour lire le fichier
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    // fichier à uploader
                    File = new FileDescription(file.FileName, stream),
                    // transformation de l'image
                    Transformation = new Transformation()
                        .Height(500)
                        .Width(500)
                        .Crop("fill")
                        .Gravity("face"),
                    // dossier de destination
                    Folder = "dating_app/"
                };

                // uploadResult contient les infos de l'image uploadée
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
           
            var deleteParams = new DeletionParams(publicId);

            var result = _cloudinary.DestroyAsync(deleteParams);

            return await result;
        }
    }
}