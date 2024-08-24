using BusinessObject.IService;
using Firebase.Storage;
using Google.Cloud.Storage.V1;

namespace BusinessObject.Service;

public class FirebaseImageService : IFirebaseImageService
{
    private readonly FirebaseStorage _firebaseStorage;

    public FirebaseImageService(FirebaseStorage firebaseStorage)
    {
        _firebaseStorage = firebaseStorage;
    }

    public async Task<string> UploadImageFromUrl(string imageUrl)
    {
        try
        {
            // Download the image from the provided URL
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(imageUrl);
                response.EnsureSuccessStatusCode();

                // Read the image data into a byte array
                var imageData = await response.Content.ReadAsByteArrayAsync();

                // Create a unique filename for the image
                var fileName = Guid.NewGuid() + Path.GetExtension(imageUrl);

                // Upload the image to Firebase Storage
                var storageRef = _firebaseStorage.Child("your-storage-path").Child(fileName);

                // Convert byte array to a MemoryStream
                using (var stream = new MemoryStream(imageData))
                {
                    await storageRef.PutAsync(stream); // Use PutAsync with MemoryStream
                }

                // Return the public URL of the uploaded image
                return await storageRef.GetDownloadUrlAsync();
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately
            Console.WriteLine($"Error uploading image: {ex.Message}");
            return null;
        }
    }
}