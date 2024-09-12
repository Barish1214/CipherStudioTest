namespace CipherStudioTest
{
    public class FileUploadService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileUploadService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if (file == null)
                return null;

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/{folderName}/{uniqueFileName}";
        }
    }
}
