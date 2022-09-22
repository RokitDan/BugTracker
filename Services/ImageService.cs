using BugTracker.Services.Interfaces;

namespace BugTracker.Services
{
    //image service will also take care of other types of file attachments. Antonio has named his BTFileService

    public class ImageService : IImageService
    {
        private readonly string _defaultProjectImageSrc = "/img/DefaultProjectImage.png";
        private readonly string _defaultCompanyImageSrc = "/img/DefaultCompanyImage.png";
        private readonly string _defaultBTUserImageSrc = "/img/DefaultBTUserImage.png";
        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        //C:\Users\dlees\OneDrive\Documents\codeCF\vsRepos\BlogApp\BlogApp\wwwroot\img\DefaultContactImage.png

        //TO DO: Blog Customizations

        public string ConvertByteArrayToFile(byte[] fileData, string extension, int imageType)
        {
            //if (fileData == null || fileData.Length == 0 && imageType != null)

            if (fileData == null || fileData.Length == 0 && imageType != null)
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
                string imageBase64Data = Convert.ToBase64String(fileData!);
                return string.Format($"data:{extension};base64,{imageBase64Data}");
            }
            catch (Exception)
            {
                throw;
            }
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

        public string GetFileIcon(string file)
        {
            string ext = Path.GetExtension(file).Replace(".", "");
            return $"/img/contenttype/{ext}.png";
        }

        public string FormatFileSize(long bytes)
        {
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }
    }
}