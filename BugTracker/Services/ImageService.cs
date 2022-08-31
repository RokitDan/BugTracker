using BugTracker.Services.Interfaces;

namespace BugTracker.Services
{
    public class ImageService : IImageService
    {
        private readonly string _defaultProjectImageSrc = "/img/DefaultProjectImage.png";
        private readonly string _defaultCompanyImageSrc = "/img/DefaultCompanyImage.jpg";
        private readonly string _defaultBTUserImageSrc = "/img/DefaultBTUserImage.jpg";

        //C:\Users\dlees\OneDrive\Documents\codeCF\vsRepos\BlogApp\BlogApp\wwwroot\img\DefaultContactImage.png

        //TO DO: Blog Customizations

        public string ConvertByteArrayToFile(byte[] fileData, string extension, int imageType)
        {
            if (fileData == null || fileData.Length == 0)
            {
                switch (imageType)
                {

                    case 1: return _defaultProjectImageSrc; //Project Image
                    case 2: return _defaultCompanyImageSrc; //Company Image
                    case 3: return _defaultBTUserImageSrc; //BTUser Image
                }
            }
            try
            {
                string imageBase64Data = Convert.ToBase64String(fileData);
                return string.Format($"data:{extension};base64,{imageBase64Data}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();

                return byteFile;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
