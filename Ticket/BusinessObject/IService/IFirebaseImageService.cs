namespace BusinessObject.IService;

public interface IFirebaseImageService
{
    Task<string> UploadImageFromUrl(string imageUrl);
}