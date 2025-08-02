using API.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> config)
    {
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string folder)
    {
        var publicId = Path.GetFileNameWithoutExtension(fileName); // ✅ Remove extension from file name
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            PublicId = $"{folder}/{publicId}",  // ✅ Full path with recognizable name
            Overwrite = true, // Optional: overwrite if same name exists
            Folder = folder   // Not strictly needed if PublicId has folder path
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.SecureUrl.ToString();
    }

    public async Task<bool> DeleteImageAsync(string fileUrl)
    {
        try
        {
            // Extract public_id from the file URL
            var uri = new Uri(fileUrl);
            var segments = uri.AbsolutePath.Split('/');
            var fileName = Path.GetFileNameWithoutExtension(segments[^1]); // last segment
            var folder = segments[^2]; // assuming structure: /folder/file.jpg

            var publicId = $"{folder}/{fileName}";

            var deletionParams = new DeletionParams(publicId);
            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

            return deletionResult.Result == "ok";
        }
        catch
        {
            return false;
        }
    }
}
