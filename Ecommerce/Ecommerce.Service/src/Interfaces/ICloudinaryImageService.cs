using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Service.src.Interfaces
{
    public interface ICloudinaryImageService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file);
        Task<DeletionResult> DeleteImageAsync(string publicId);
    }
}