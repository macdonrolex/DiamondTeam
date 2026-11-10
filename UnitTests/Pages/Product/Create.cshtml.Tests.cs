using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Pages.Product;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;

namespace UnitTests.Pages.Product;

/// <summary>
/// Tests the Create page's image upload validation and save behavior.
/// </summary>
[TestFixture]
public class CreateTests
{
    // Test data root keeps each upload away from the project's shared JSON data.
    private string webRootPath;

    // Page model under test for the current test case.
    private CreateModel pageModel;

    // Product service used by the page model.
    private JsonFileProductService productService;

    // Content stream assigned to the uploaded form file.
    private MemoryStream imageStream;

    #region TestSetup

    /// <summary>
    /// Creates an isolated data folder and page model before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // Create a unique web root so tests cannot modify project data.
        webRootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        // Create the data folder used by the JSON product service.
        string dataFolder = Path.Combine(webRootPath, "data");

        Directory.CreateDirectory(dataFolder);

        // Start each test with an empty product collection.
        File.WriteAllText(Path.Combine(dataFolder, "products.json"), "[]");

        // Point the service at the isolated test web root.
        var environment = new Mock<IWebHostEnvironment>();

        environment.Setup(item => item.WebRootPath).Returns(webRootPath);
        productService = new JsonFileProductService(environment.Object);

        // Create the page model with a fresh valid product.
        pageModel = new CreateModel(productService)
        {
            Product = new ProductModel
            {
                Maker = "Example author",
                Title = "Example Game",
                Description = "A sample description for an upload test.",
                Post = "A sample post body for testing image uploads.",
                Genre = GenreEnum.Action,
                ReleaseYear = 2024,
                ShopLink = "https://example.com/shop",
                RatingLink = "https://example.com/reviews"
            }
        };

        // Supply page context so Page() and TempData work in handler tests.
        var httpContext = new DefaultHttpContext();

        var modelState = new ModelStateDictionary();

        var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);

        var metadataProvider = new EmptyModelMetadataProvider();

        var viewData = new ViewDataDictionary(metadataProvider, modelState);

        var pageContext = new PageContext(actionContext)
        {
            HttpContext = httpContext,
            ViewData = viewData
        };

        pageModel.PageContext = pageContext;
        pageModel.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
    }

    /// <summary>
    /// Releases the temporary image stream and isolated test data folder.
    /// </summary>
    [TearDown]
    public void TearDown()
    {

        // Reset

        // Release temporary test data so the next test starts clean.
        // Close the image stream when a test created one.
        imageStream?.Dispose();

        // Remove the isolated data after the test completes.
        if (Directory.Exists(webRootPath))
        {
            Directory.Delete(webRootPath, true);
        }
    }

    #endregion TestSetup

    #region OnGet

    /// <summary>
    /// Confirms that opening the Create page has a product ready for form binding.
    /// </summary>
    [Test]
    public void OnGet_Valid_Product_Should_Not_Be_Null()
    {

        // Arrange

        // Use the page model created by SetUp.

        // Act

        pageModel.OnGet();
        
        // Reset

        // Assert

        Assert.That(pageModel.Product, Is.Not.Null);
    }

    #endregion OnGet

    #region OnPostAsync

    /// <summary>
    /// Confirms that invalid form data returns the Create page before reading the file.
    /// </summary>
    [Test]
    public async Task OnPostAsync_Invalid_Model_State_Should_Return_Page()
    {

        // Arrange

        // Simulate a field validation error and no selected image.
        pageModel.ModelState.AddModelError("Product.Title", "Enter a title.");

        // Act

        IActionResult result = await pageModel.OnPostAsync();
        
        // Reset

        // Assert

        Assert.That(result, Is.TypeOf<PageResult>());

        Assert.That(productService.GetAllData(), Is.Empty);
    }

    /// <summary>
    /// Confirms that a valid PNG is saved and its path is stored with the product.
    /// </summary>
    [Test]
    public async Task OnPostAsync_Valid_Png_Should_Save_Image_And_Redirect_To_Product_Index()
    {

        // Arrange

        // Minimal PNG header is enough for the handler's signature validation.
        byte[] pngBytes = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x01 };

        imageStream = new MemoryStream(pngBytes);

        pageModel.ImageFile = new FormFile(imageStream, 0, pngBytes.Length, "ImageFile", "cover.png");

        // Act

        IActionResult result = await pageModel.OnPostAsync();
        
        // Reset

        // Assert

        Assert.That(result, Is.TypeOf<RedirectToPageResult>());

        Assert.That(((RedirectToPageResult)result).PageName, Is.EqualTo("/Product/Index"));

        // Read the saved record from the isolated JSON product data.
        ProductModel savedProduct = productService.GetAllData().Single();

        Assert.That(savedProduct.Image, Does.StartWith("/data/images/"));

        // Convert the saved web path to a local file path under the test web root.
        string savedImagePath = Path.Combine(
            webRootPath,
            savedProduct.Image.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        Assert.That(File.Exists(savedImagePath), Is.True);

        Assert.That(pageModel.TempData["SuccessMessage"], Is.EqualTo("Your post and image were uploaded."));
    }

    /// <summary>
    /// Confirms that a failed save removes a partial image and returns the form with an error.
    /// </summary>
    [Test]
    public async Task OnPostAsync_Invalid_Save_Result_Should_Remove_Partial_Image_And_Return_Page()
    {

        // Arrange

        // Use a test page model that simulates a save failing after writing partial data.
        var failingPageModel = new FailedImageSaveCreateModel(productService)
        {
            Product = pageModel.Product,
            PageContext = pageModel.PageContext,
            TempData = pageModel.TempData
        };

        // Provide a supported PNG so the request reaches the save operation.
        byte[] imageBytes = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        imageStream = new MemoryStream(imageBytes);

        failingPageModel.ImageFile = new FormFile(
            imageStream,
            0,
            imageBytes.Length,
            "ImageFile",
            "cover.png");

        // Act

        IActionResult result = await failingPageModel.OnPostAsync();
        
        // Reset

        // Assert

        Assert.That(result, Is.TypeOf<PageResult>());

        Assert.That(failingPageModel.ModelState[string.Empty].Errors, Is.Not.Empty);

        Assert.That(Directory.GetFiles(Path.Combine(webRootPath, "data", "images")), Is.Empty);

        Assert.That(productService.GetAllData(), Is.Empty);
    }

    /// <summary>
    /// Confirms that every supported image extension passes the Create handler.
    /// </summary>
    [Test]
    public async Task OnPostAsync_Valid_Supported_Formats_Should_Save_And_Redirect()
    {

        // Arrange

        // Provide the minimal signatures accepted for each supported format.
        (string Extension, byte[] Header)[] supportedImages =
        {
            (".jpg", new byte[] { 0xFF, 0xD8, 0xFF }),
            (".jpeg", new byte[] { 0xFF, 0xD8, 0xFF }),
            (".png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            (".gif", new byte[] { (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'7', (byte)'a' }),
            (".webp", new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F', 0, 0, 0, 0, (byte)'W', (byte)'E', (byte)'B', (byte)'P' })
        };

        // Act

        foreach ((string extension, byte[] header) in supportedImages)
        {
            // Dispose the previous stream before preparing the next upload.
            imageStream?.Dispose();

            imageStream = new MemoryStream(header);

            pageModel.ImageFile = new FormFile(
                imageStream,
                0,
                header.Length,
                "ImageFile",
                "cover" + extension);

            IActionResult result = await pageModel.OnPostAsync();

            // Assert each valid format redirects to the product list.
            Assert.That(
                result,
                Is.TypeOf<RedirectToPageResult>(),
                $"The {extension} upload should be accepted.");
        }
        
        // Reset

        // Assert

        Assert.That(productService.GetAllData().Count(), Is.EqualTo(supportedImages.Length));
    }

    /// <summary>
    /// Confirms that an unsupported file extension is rejected without saving a product.
    /// </summary>
    [Test]
    public async Task OnPostAsync_Invalid_Extension_Should_Return_Page_With_Image_Error()
    {

        // Arrange

        // A text file should be rejected before any image bytes are stored.
        byte[] fileBytes = { 0x01, 0x02, 0x03 };

        imageStream = new MemoryStream(fileBytes);

        pageModel.ImageFile = new FormFile(imageStream, 0, fileBytes.Length, "ImageFile", "cover.txt");

        // Act

        IActionResult result = await pageModel.OnPostAsync();
        
        // Reset

        // Assert

        Assert.That(result, Is.TypeOf<PageResult>());

        Assert.That(pageModel.ModelState[nameof(pageModel.ImageFile)].Errors, Is.Not.Empty);

        Assert.That(productService.GetAllData(), Is.Empty);

        Assert.That(Directory.Exists(Path.Combine(webRootPath, "data", "images")), Is.False);
    }

    /// <summary>
    /// Confirms that image bytes with a supported extension but invalid signature are rejected.
    /// </summary>
    [Test]
    public async Task OnPostAsync_Invalid_Image_Signature_Should_Return_Page()
    {

        // Arrange

        // A PNG extension does not make non-PNG content a valid image.
        byte[] fileBytes = { 0x01, 0x02, 0x03, 0x04 };

        imageStream = new MemoryStream(fileBytes);

        pageModel.ImageFile = new FormFile(imageStream, 0, fileBytes.Length, "ImageFile", "cover.png");

        // Act

        IActionResult result = await pageModel.OnPostAsync();
        
        // Reset

        // Assert

        Assert.That(result, Is.TypeOf<PageResult>());

        Assert.That(pageModel.ModelState[nameof(pageModel.ImageFile)].Errors, Is.Not.Empty);

        Assert.That(productService.GetAllData(), Is.Empty);

        Assert.That(Directory.Exists(Path.Combine(webRootPath, "data", "images")), Is.False);
    }

    /// <summary>
    /// Confirms that an image larger than 10 MB is rejected without saving a product.
    /// </summary>
    [Test]
    public async Task OnPostAsync_Invalid_Over_Size_File_Should_Return_Page()
    {

        // Arrange

        // Build a file one byte larger than the Create page's 10 MB limit.
        byte[] fileBytes = new byte[10 * 1024 * 1024 + 1];

        imageStream = new MemoryStream(fileBytes);

        pageModel.ImageFile = new FormFile(imageStream, 0, fileBytes.Length, "ImageFile", "cover.png");

        // Act

        IActionResult result = await pageModel.OnPostAsync();
        
        // Reset

        // Assert

        Assert.That(result, Is.TypeOf<PageResult>());

        Assert.That(pageModel.ModelState[nameof(pageModel.ImageFile)].Errors, Is.Not.Empty);

        Assert.That(productService.GetAllData(), Is.Empty);
    }

    /// <summary>
    /// Confirms that an empty uploaded file is rejected without saving a product.
    /// </summary>
    [Test]
    public async Task OnPostAsync_Invalid_Empty_File_Should_Return_Page_With_Image_Error()
    {

        // Arrange

        // A zero-byte file should fail the upload validation.
        imageStream = new MemoryStream();

        pageModel.ImageFile = new FormFile(imageStream, 0, 0, "ImageFile", "cover.png");

        // Act

        IActionResult result = await pageModel.OnPostAsync();
        
        // Reset

        // Assert

        Assert.That(result, Is.TypeOf<PageResult>());

        Assert.That(pageModel.ModelState[nameof(pageModel.ImageFile)].Errors, Is.Not.Empty);

        Assert.That(productService.GetAllData(), Is.Empty);
    }

    #endregion OnPostAsync

    #region SaveImageAndProductAsync

    /// <summary>
    /// Confirms that the public save method writes the image and its product record.
    /// </summary>
    [Test]
    public async Task SaveImageAndProductAsync_Valid_Image_Should_Save_Image_And_Product()
    {

        // Arrange

        // Prepare a small image stream for the save helper.
        byte[] imageBytes = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        imageStream = new MemoryStream(imageBytes);

        pageModel.ImageFile = new FormFile(imageStream, 0, imageBytes.Length, "ImageFile", "cover.png");

        // Create the file paths used by the save helper.
        string imagesFolder = Path.Combine(webRootPath, "data", "images");

        string fileName = "test-cover.png";

        string fullImagePath = Path.Combine(imagesFolder, fileName);

        // Act

        bool saved = await pageModel.SaveImageAndProductAsync(imagesFolder, fullImagePath, fileName);
        
        // Reset

        // Assert

        Assert.That(saved, Is.True);

        Assert.That(File.Exists(fullImagePath), Is.True);

        Assert.That(pageModel.Product.Image, Is.EqualTo("/data/images/test-cover.png"));

        Assert.That(productService.GetAllData().Single().Title, Is.EqualTo("Example Game"));
    }

    /// <summary>
    /// Confirms that the public save method rejects a missing image file.
    /// </summary>
    [Test]
    public async Task SaveImageAndProductAsync_Invalid_Missing_Image_Should_Return_False()
    {

        // Arrange

        // Ensure the page model has no uploaded file.
        pageModel.ImageFile = null;

        string imagesFolder = Path.Combine(webRootPath, "data", "images");

        string fullImagePath = Path.Combine(imagesFolder, "missing.png");

        // Act

        bool saved = await pageModel.SaveImageAndProductAsync(imagesFolder, fullImagePath, "missing.png");
        
        // Reset

        // Assert

        Assert.That(saved, Is.False);

        Assert.That(productService.GetAllData(), Is.Empty);
    }

    /// <summary>
    /// Confirms that the public save method does not overwrite an existing file.
    /// </summary>
    [Test]
    public async Task SaveImageAndProductAsync_Invalid_Existing_Destination_Should_Return_False()
    {

        // Arrange

        // Provide an image file and create a destination that already exists.
        byte[] imageBytes = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        imageStream = new MemoryStream(imageBytes);

        pageModel.ImageFile = new FormFile(imageStream, 0, imageBytes.Length, "ImageFile", "cover.png");

        string imagesFolder = Path.Combine(webRootPath, "data", "images");

        Directory.CreateDirectory(imagesFolder);

        string fullImagePath = Path.Combine(imagesFolder, "existing.png");

        File.WriteAllText(fullImagePath, "keep this file");

        // Act

        bool saved = await pageModel.SaveImageAndProductAsync(imagesFolder, fullImagePath, "existing.png");
        
        // Reset

        // Assert

        Assert.That(saved, Is.False);

        Assert.That(File.ReadAllText(fullImagePath), Is.EqualTo("keep this file"));

        Assert.That(productService.GetAllData(), Is.Empty);
    }

    #endregion SaveImageAndProductAsync

    #region DeleteFileIfItExists

    /// <summary>
    /// Confirms that the public cleanup method removes a file that exists.
    /// </summary>
    [Test]
    public void DeleteFileIfItExists_Valid_Existing_File_Should_Remove_File()
    {

        // Arrange

        // Create a file that the cleanup helper can remove.
        string filePath = Path.Combine(webRootPath, "temporary-image.png");

        File.WriteAllText(filePath, "temporary image");

        // Act

        CreateModel.DeleteFileIfItExists(filePath);
        
        // Reset

        // Assert

        Assert.That(File.Exists(filePath), Is.False);
    }

    /// <summary>
    /// Confirms that the public cleanup method safely ignores a missing file.
    /// </summary>
    [Test]
    public void DeleteFileIfItExists_Invalid_Missing_File_Should_Not_Throw()
    {

        // Arrange

        // Use a path that has not been created.
        string filePath = Path.Combine(webRootPath, "missing-image.png");

        // Act

        CreateModel.DeleteFileIfItExists(filePath);
        
        // Reset

        // Assert

        Assert.That(File.Exists(filePath), Is.False);
    }

    #endregion DeleteFileIfItExists

    #region HasValidImageSignatureAsync

    /// <summary>
    /// Confirms that the public signature checker accepts a valid PNG header.
    /// </summary>
    [Test]
    public async Task HasValidImageSignatureAsync_Valid_Png_Should_Return_True()
    {

        // Arrange

        // Use the PNG signature bytes expected by the upload validator.
        byte[] imageBytes = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        imageStream = new MemoryStream(imageBytes);

        IFormFile imageFile = new FormFile(imageStream, 0, imageBytes.Length, "ImageFile", "cover.png");

        // Act

        bool isValid = await CreateModel.HasValidImageSignatureAsync(imageFile, ".png");
        
        // Reset

        // Assert

        Assert.That(isValid, Is.True);
    }

    /// <summary>
    /// Confirms that the public signature checker accepts a valid JPG header.
    /// </summary>
    [Test]
    public async Task HasValidImageSignatureAsync_Valid_Jpg_Should_Return_True()
    {

        // Arrange

        // JPEG files start with the three-byte JPG signature.
        byte[] imageBytes = { 0xFF, 0xD8, 0xFF };

        imageStream = new MemoryStream(imageBytes);

        IFormFile imageFile = new FormFile(imageStream, 0, imageBytes.Length, "ImageFile", "cover.jpg");

        // Act

        bool isValid = await CreateModel.HasValidImageSignatureAsync(imageFile, ".jpg");
        
        // Reset

        // Assert

        Assert.That(isValid, Is.True);
    }

    /// <summary>
    /// Confirms that the public signature checker accepts a valid JPEG header.
    /// </summary>
    [Test]
    public async Task HasValidImageSignatureAsync_Valid_Jpeg_Should_Return_True()
    {

        // Arrange

        // The JPEG extension uses the same signature as JPG.
        byte[] imageBytes = { 0xFF, 0xD8, 0xFF };

        imageStream = new MemoryStream(imageBytes);

        IFormFile imageFile = new FormFile(imageStream, 0, imageBytes.Length, "ImageFile", "cover.jpeg");

        // Act

        bool isValid = await CreateModel.HasValidImageSignatureAsync(imageFile, ".jpeg");
        
        // Reset

        // Assert

        Assert.That(isValid, Is.True);
    }

    /// <summary>
    /// Confirms that the public signature checker accepts a valid GIF header.
    /// </summary>
    [Test]
    public async Task HasValidImageSignatureAsync_Valid_Gif_Should_Return_True()
    {

        // Arrange

        // GIF images begin with the six ASCII bytes GIF87a or GIF89a.
        byte[] imageBytes = { (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'9', (byte)'a' };

        imageStream = new MemoryStream(imageBytes);

        IFormFile imageFile = new FormFile(imageStream, 0, imageBytes.Length, "ImageFile", "cover.gif");

        // Act

        bool isValid = await CreateModel.HasValidImageSignatureAsync(imageFile, ".gif");
        
        // Reset

        // Assert

        Assert.That(isValid, Is.True);
    }

    /// <summary>
    /// Confirms that both accepted GIF signatures are recognized.
    /// </summary>
    [Test]
    public async Task HasValidImageSignatureAsync_Valid_Gif87a_Should_Return_True()
    {

        // Arrange

        // GIF87a is the second valid header accepted by the signature check.
        byte[] imageBytes = { (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'7', (byte)'a' };

        imageStream = new MemoryStream(imageBytes);

        IFormFile imageFile = new FormFile(imageStream, 0, imageBytes.Length, "ImageFile", "cover.gif");

        // Act

        bool isValid = await CreateModel.HasValidImageSignatureAsync(imageFile, ".gif");
        
        // Reset

        // Assert

        Assert.That(isValid, Is.True);
    }

    /// <summary>
    /// Confirms that short headers are rejected for each supported image format.
    /// </summary>
    [Test]
    public async Task HasValidImageSignatureAsync_Invalid_Short_Headers_Should_Return_False()
    {

        // Arrange

        // Use one byte less than the minimum header length for each format.
        (string Extension, byte[] Header)[] shortHeaders =
        {
            (".jpg", new byte[] { 0xFF, 0xD8 }),
            (".jpeg", new byte[] { 0xFF, 0xD8 }),
            (".png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A }),
            (".gif", new byte[] { (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'9' }),
            (".webp", new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F', 0, 0, 0, 0, (byte)'W', (byte)'E', (byte)'B' })
        };

        // Act

        foreach ((string extension, byte[] header) in shortHeaders)
        {
            // Replace the stream for the next short-header case.
            imageStream?.Dispose();

            imageStream = new MemoryStream(header);

            IFormFile imageFile = new FormFile(
                imageStream,
                0,
                header.Length,
                "ImageFile",
                "cover" + extension);

            bool isValid = await CreateModel.HasValidImageSignatureAsync(imageFile, extension);
            
            // Reset

            // Assert

            Assert.That(isValid, Is.False, $"The short {extension} header should be rejected.");
        }
        
    }

    /// <summary>
    /// Confirms that changing any checked signature byte makes a supported image invalid.
    /// </summary>
    [Test]
    public async Task HasValidImageSignatureAsync_Invalid_Changed_Signature_Byte_Should_Return_False()
    {

        // Arrange

        // List the bytes that the validator checks for each image format.
        (string Extension, byte[] Header, int[] CheckedPositions)[] images =
        {
            (".jpg", new byte[] { 0xFF, 0xD8, 0xFF }, new[] { 0, 1, 2 }),
            (".jpeg", new byte[] { 0xFF, 0xD8, 0xFF }, new[] { 0, 1, 2 }),
            (".png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, new[] { 0, 1, 2, 3, 4, 5, 6, 7 }),
            (".gif", new byte[] { (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'9', (byte)'a' }, new[] { 0, 1, 2, 3, 4, 5 }),
            (".webp", new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F', 0, 0, 0, 0, (byte)'W', (byte)'E', (byte)'B', (byte)'P' }, new[] { 0, 1, 2, 3, 8, 9, 10, 11 })
        };

        // Act

        foreach ((string extension, byte[] header, int[] checkedPositions) in images)
        {
            foreach (int position in checkedPositions)
            {
                // Change one signature byte while keeping the rest of the header intact.
                byte[] changedHeader = (byte[])header.Clone();

                changedHeader[position] ^= 0xFF;

                imageStream?.Dispose();

                imageStream = new MemoryStream(changedHeader);

                IFormFile imageFile = new FormFile(
                    imageStream,
                    0,
                    changedHeader.Length,
                    "ImageFile",
                    "cover" + extension);

                bool isValid = await CreateModel.HasValidImageSignatureAsync(imageFile, extension);
                
                // Reset

                // Assert

                Assert.That(
                    isValid,
                    Is.False,
                    $"The {extension} header should fail when byte {position} changes.");
            }
        }
    }

    /// <summary>
    /// Confirms that the public signature checker accepts a valid WEBP header.
    /// </summary>
    [Test]
    public async Task HasValidImageSignatureAsync_Valid_Webp_Should_Return_True()
    {

        // Arrange

        // WEBP images have RIFF at the start and WEBP at byte eight.
        byte[] imageBytes =
        {
            (byte)'R', (byte)'I', (byte)'F', (byte)'F',
            0x00, 0x00, 0x00, 0x00,
            (byte)'W', (byte)'E', (byte)'B', (byte)'P'
        };

        imageStream = new MemoryStream(imageBytes);

        IFormFile imageFile = new FormFile(imageStream, 0, imageBytes.Length, "ImageFile", "cover.webp");

        // Act

        bool isValid = await CreateModel.HasValidImageSignatureAsync(imageFile, ".webp");
        
        // Reset

        // Assert

        Assert.That(isValid, Is.True);
    }

    /// <summary>
    /// Confirms that the public signature checker rejects a mismatched image signature.
    /// </summary>
    [Test]
    public async Task HasValidImageSignatureAsync_Invalid_Png_Should_Return_False()
    {

        // Arrange

        // Use bytes that do not match the PNG signature.
        byte[] imageBytes = { 0x01, 0x02, 0x03, 0x04 };

        imageStream = new MemoryStream(imageBytes);

        IFormFile imageFile = new FormFile(imageStream, 0, imageBytes.Length, "ImageFile", "cover.png");

        // Act

        bool isValid = await CreateModel.HasValidImageSignatureAsync(imageFile, ".png");
        
        // Reset

        // Assert

        Assert.That(isValid, Is.False);
    }

    /// <summary>
    /// Confirms that the public signature checker rejects an unsupported extension.
    /// </summary>
    [Test]
    public async Task HasValidImageSignatureAsync_Invalid_Unsupported_Extension_Should_Return_False()
    {

        // Arrange

        // Provide content even though SVG is intentionally unsupported.
        byte[] imageBytes = { 0x01, 0x02, 0x03, 0x04 };

        imageStream = new MemoryStream(imageBytes);

        IFormFile imageFile = new FormFile(imageStream, 0, imageBytes.Length, "ImageFile", "cover.svg");

        // Act

        bool isValid = await CreateModel.HasValidImageSignatureAsync(imageFile, ".svg");
        
        // Reset

        // Assert

        Assert.That(isValid, Is.False);
    }

    #endregion HasValidImageSignatureAsync

    #region TestHelpers

    /// <summary>
    /// Simulates a partial image write followed by a failed product save.
    /// </summary>
    private sealed class FailedImageSaveCreateModel : CreateModel
    {
        /// <summary>
        /// Creates the test page model with the product service.
        /// </summary>
        /// <param name="productService">Service used by the Create page.</param>
        public FailedImageSaveCreateModel(JsonFileProductService productService)
            : base(productService)
        {
        }

        /// <summary>
        /// Writes partial data and reports a failure so handler cleanup can be tested.
        /// </summary>
        /// <param name="imagesFolder">Image destination folder.</param>
        /// <param name="fullImagePath">Full image destination path.</param>
        /// <param name="fileName">Generated image file name.</param>
        /// <returns>A task that reports a failed save.</returns>
        public override Task<bool> SaveImageAndProductAsync(
            string imagesFolder,
            string fullImagePath,
            string fileName)
        {
            // Create the image folder and leave a partial file for the handler to remove.
            Directory.CreateDirectory(imagesFolder);

            System.IO.File.WriteAllText(fullImagePath, "partial image");

            return Task.FromResult(false);
        }
        
    }
    
    #endregion TestHelpers
    
}
