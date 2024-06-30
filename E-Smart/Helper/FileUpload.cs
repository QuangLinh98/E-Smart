namespace E_Smart.Helper
{
    public class FileUpload
    {
        static readonly string baseFolder = "Upload";

        public static async Task<string> SaveImage(string subFolder, IFormFile formFile)
        {
            string imageName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", baseFolder, subFolder);

            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            var exactPath = Path.Combine(imagePath, imageName);
            using (var fileStream = new FileStream(exactPath, FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
            }

            return Path.Combine(baseFolder, subFolder, imageName);
        }

        public static void DeleteImage(string imagePath)
        {
            var exactPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath);
            if (File.Exists(exactPath))
            {
                File.Delete(exactPath);
            }
        }
    }
}
