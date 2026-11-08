using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoCrafts.WebSite.Pages.Product
{
    /// <summary>
    /// Handles the form for creating a new product and saving its uploaded cover image.
    /// </summary>
    /// 
    public class CreateModel : PageModel
    {
        // Limit uploaded images to 10 MB.
        private const long MaximumImageSizeBytes = 10 * 1024 * 1024;

        // Only common image formats are accepted; SVG files are excluded for safety.
        private static readonly string[] AllowedImageExtensions =
            { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        // Provides the operation that stores a submitted product.
        public readonly JsonFileProductService ProductService;

        /// <summary>
        /// Creates the product creation page model with its data service.
        /// </summary>
        /// <param name="productService">Service used to add the new product.</param>
        public CreateModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        /// <summary>
        /// Gets or sets the product fields bound from the creation form.
        /// </summary>
        [BindProperty]
        public ProductModel Product { get; set; } = new ProductModel();


        /// <summary>
        /// Gets or sets the image file selected in the create form.
        /// </summary>
        [BindProperty]
        [Required(ErrorMessage = "Choose an image file to upload.")]
        public IFormFile ImageFile { get; set; }

        /// <summary>
        /// Handles a request to display the product creation form.
        /// </summary>
        public void OnGet()
        {
        }


        /// <summary>
        /// Validates the submitted fields, saves the image, and adds the post.
        /// </summary>
        /// <returns>The form when validation fails; otherwise, a redirect to the listing.</returns>
        [RequestSizeLimit(10 * 1024 * 1024 + 64 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 10 * 1024 * 1024 + 64 * 1024)]
        public async Task<IActionResult> OnPostAsync()
        {
            // The handler sets Product.Image itself later, so don't validate it here.
            ModelState.Remove("Product.Image");

            // Step 1: If the normal form fields have problems, show them right away.
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Step 2: Check the file size.
            bool fileIsEmpty = ImageFile.Length == 0;
            bool fileIsTooBig = !fileIsEmpty && ImageFile.Length > MaximumImageSizeBytes;
            bool fileSizeIsOk = !fileIsEmpty && !fileIsTooBig;

            if (fileIsEmpty)
            {
                ModelState.AddModelError(nameof(ImageFile), "The selected image file is empty.");
            }

            if (fileIsTooBig)
            {
                ModelState.AddModelError(nameof(ImageFile), "Choose an image smaller than 10 MB.");
            }

            // Step 3: Check the file extension (.jpg, .png, etc.).
            string extension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();

            bool extensionIsAllowed = false;
            foreach (string allowed in AllowedImageExtensions)
            {
                if (string.Equals(allowed, extension, StringComparison.OrdinalIgnoreCase))
                {
                    extensionIsAllowed = true;
                }
            }

            if (!extensionIsAllowed)
            {
                ModelState.AddModelError(nameof(ImageFile), "Choose a JPG, PNG, GIF, or WEBP image.");
            }

            // Step 4: Only look inside the file if the extension and size are both fine.
            if (extensionIsAllowed && fileSizeIsOk)
            {
                bool fileIsRealImage = await HasValidImageSignatureAsync(ImageFile, extension);

                if (!fileIsRealImage)
                {
                    ModelState.AddModelError(nameof(ImageFile), "This file is not a valid supported image.");
                }
            }

            // Step 5: If any image check failed, show the errors.
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Step 6: Make a random file name so users can't pick their own path.
            string imagesFolder = Path.Combine(
                ProductService.WebHostEnvironment.WebRootPath,
                "data",
                "images");
            string fileName = Guid.NewGuid().ToString("N") + extension;
            string fullImagePath = Path.Combine(imagesFolder, fileName);

            // Step 7: Save the image and the product.
            bool savedOk = await SaveImageAndProductAsync(imagesFolder, fullImagePath, fileName);

            if (!savedOk)
            {
                // Saving failed, so clean up any half-saved image.
                DeleteFileIfItExists(fullImagePath);

                ModelState.AddModelError(string.Empty, "We couldn't save the image or post. Please try again.");
                return Page();
            }

            // Step 8: Everything worked.
            TempData["SuccessMessage"] = "Your post and image were uploaded.";
            return RedirectToPage("/Product/Index");
        }

        // Returns true if the image and product were saved, false if saving failed.
  
      
        private async Task<bool> SaveImageAndProductAsync(string imagesFolder, string fullImagePath, string fileName)
        {
            // Stop if no image was uploaded
            if (ImageFile == null)
            {
                return false;
            }

            // Stop if a file with this name already exists
            if (System.IO.File.Exists(fullImagePath))
            {
                return false;
            }

            // Make sure the images folder exists
            Directory.CreateDirectory(imagesFolder);

            // Save the image
            using (var imageStream = new FileStream(fullImagePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(imageStream);
            }

            // Save the product
            Product.Image = "/data/images/" + fileName;
            Product.DateAdded = DateTime.UtcNow;
            ProductService.AddProduct(Product);

            return true;
        }


        private static void DeleteFileIfItExists(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        /// <summary>
        /// Checks the file signature so a renamed non-image file is rejected.
        /// </summary>
        private static async Task<bool> HasValidImageSignatureAsync(IFormFile file, string extension)
        {
            var header = new byte[12];
            using var stream = file.OpenReadStream();
            var bytesRead = await stream.ReadAsync(header, 0, header.Length);

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return bytesRead >= 3 &&
                           header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;

                case ".png":
                    return bytesRead >= 8 &&
                           header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E &&
                           header[3] == 0x47 && header[4] == 0x0D && header[5] == 0x0A &&
                           header[6] == 0x1A && header[7] == 0x0A;

                case ".gif":
                    return bytesRead >= 6 &&
                           header[0] == 'G' && header[1] == 'I' && header[2] == 'F' &&
                           header[3] == '8' && (header[4] == '7' || header[4] == '9') &&
                           header[5] == 'a';

                case ".webp":
                    return bytesRead >= 12 &&
                           header[0] == 'R' && header[1] == 'I' && header[2] == 'F' &&
                           header[3] == 'F' && header[8] == 'W' && header[9] == 'E' &&
                           header[10] == 'B' && header[11] == 'P';

                default:
                    return false;
            }
        }
    }
}
