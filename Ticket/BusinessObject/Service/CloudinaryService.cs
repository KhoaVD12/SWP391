using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace BusinessObject.Service;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null;

        // Create the upload params
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, file.OpenReadStream()),
            // Optionally, you can set the folder, transformation, etc.
        };

        // Upload the image
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        // Return the secure URL of the uploaded image
        return uploadResult.SecureUrl?.ToString();
    }
}