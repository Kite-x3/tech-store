using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace TechStore.Application.Services
{
    public class ImageService
    {
        private readonly IHostEnvironment _environment;
        private readonly string _productImagesPath;

        public ImageService(IHostEnvironment environment)
        {
            _environment = environment;
            _productImagesPath = Path.Combine(_environment.ContentRootPath, "ProductImages");

            Directory.CreateDirectory(_productImagesPath);
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Image file is empty");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Invalid image format. Allowed: .jpg, .jpeg, .png");

            // файл не болше 5мб
            if (imageFile.Length > 5 * 1024 * 1024)
                throw new ArgumentException("Image size exceeds 5MB limit");

            // уникальное имя
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var fullPath = Path.Combine(_productImagesPath, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/images/{uniqueFileName}";
        }

        public Task DeleteImageAsync(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return Task.CompletedTask;

            var fileName = Path.GetFileName(imagePath);
            if (string.IsNullOrEmpty(fileName))
                return Task.CompletedTask;

            var fullPath = Path.Combine(_productImagesPath, fileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }
    }
}