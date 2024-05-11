using CloudinaryDotNet.Actions;

namespace Taxi_App;

public interface IPhotoService
{
    Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
}
