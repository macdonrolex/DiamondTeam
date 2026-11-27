using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using ContosoCrafts.WebSite.Components;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace UnitTests.Components;

/// <summary>
/// Unit tests for ImageUpload.razor
/// </summary>
public class ImageUploadTests : BunitTestContext
{
    #region LoadFile

    /// <summary>
    /// If the uploaded file is too big, keep the default file.
    /// Use async Task here to pause the test untile LoadFile finishes
    /// </summary>
    [Test]
    public async Task LoadFile_InValid_File_Greater_Than_10MB_Should_Keep_Default_Path()
    {

        // Arrange
        var data = new ImageUpload();
        
        // The mock data file 20MB (not actual size, just used to trigger the if condition)
        var mockFile = new Mock<IBrowserFile>();
        mockFile.SetupGet(f => f.Name).Returns("big.png");
        mockFile.SetupGet(f => f.Size).Returns(2 * 10 * 1024 * 1024);

        // Create the file list for triggering the event
        var fileList = new List<IBrowserFile>();
        fileList.Add(mockFile.Object);
        
        // The event that InputFile raises when a file is chosen
        var dataEvent = new InputFileChangeEventArgs(fileList);
        
        // Act
        await data.LoadFile(dataEvent);
        
        // Reset

        // Not needed

        // Assert
        
        // File path is default
        Assert.That(data.FilePath, Is.EqualTo("/images/default-post.svg"));
    } 
    
    /// </summary>
    /// If the uploaded file is too big, The error message should display on the page
    /// Use async Task here to pause the test untile LoadFile finishes
    /// </summary>
    [Test]
    public async Task LoadFile_InValid_File_Greater_Than_10MB_Should_Display_Error_Message()
    {

        Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
        
        // Render the upload fields
        var page = Render<ImageUpload>();
        
        // The mock data file 20MB (not actual size, just used to trigger the if condition)
        var mockFile = new Mock<IBrowserFile>();
        mockFile.SetupGet(f => f.Name).Returns("image.png");
        mockFile.SetupGet(f => f.Size).Returns(20 * 1024 * 1024);

        // Create the file list for triggering the event
        var fileList = new List<IBrowserFile>();
        fileList.Add(mockFile.Object);
        
        // The event that InputFile raises when a file is chosen
        var dataEvent = new InputFileChangeEventArgs(fileList);
        
        // Save markup before uploading.
        var preMarkup = page.Markup;
        
        // Act
        
        // Call LoadFile on the rendered component, through its renderer, as Blazor does when the user chooses a file.
        // InvokeAsync runs the call on the component's own dispatcher, and await waits until the upload finishes.
        await page.InvokeAsync(() => page.Instance.LoadFile(dataEvent));
        
        // Rerender the page for displaying the error message
        page.Render();
        
        // Save markup to see if the error message is displayed
        var postMarkup = page.Markup;
        
        // Reset
        
        // Not needed

        // Assert
        
        // The error message is set
        Assert.That(page.Instance.ErrorMessage, Is.EqualTo("The image must be smaller than 10 MB"));
        Assert.That(preMarkup.Contains(page.Instance.ErrorMessage), Is.EqualTo(false));
        Assert.That(postMarkup.Contains(page.Instance.ErrorMessage), Is.EqualTo(true));
    }
    
    /// </summary>
    /// If the uploaded file is not an image, The error message should display on the page
    /// Use async Task here to pause the test untile LoadFile finishes
    /// </summary>
    [Test]
    public async Task LoadFile_InValid_File_Not_Image_Should_Display_Error_Message()
    {

        Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
        
        // Render the upload fields
        var page = Render<ImageUpload>();
        
        // The mock video file
        var mockFile = new Mock<IBrowserFile>();
        mockFile.SetupGet(f => f.Name).Returns("video.mp4");

        // Create the file list for triggering the event
        var fileList = new List<IBrowserFile>();
        fileList.Add(mockFile.Object);
        
        // The event that InputFile raises when a file is chosen
        var dataEvent = new InputFileChangeEventArgs(fileList);
        
        // Save markup before uploading.
        var preMarkup = page.Markup;
        
        // Act
        
        // Call LoadFile on the rendered component, through its renderer, as Blazor does when the user chooses a file.
        // InvokeAsync runs the call on the component's own dispatcher, and await waits until the upload finishes.
        await page.InvokeAsync(() => page.Instance.LoadFile(dataEvent));
        
        // Rerender the page for displaying the error message
        page.Render();
        
        // Save markup to see if the error message is displayed
        var postMarkup = page.Markup;
        
        // Reset
        
        // Not needed

        // Assert
        
        // The error message is set
        Assert.That(page.Instance.ErrorMessage, Is.EqualTo("The uploaded file is not accepted. We accept .jpg, .jpeg, .png, .gif, and .webp"));
        
        // Before uploading a not image file, no error message
        Assert.That(preMarkup.Contains(page.Instance.ErrorMessage), Is.EqualTo(false));
        
        // After uploading a not image file, show error message
        Assert.That(postMarkup.Contains(page.Instance.ErrorMessage), Is.EqualTo(true));
    }

    /// <summary>
    /// Test the situation that we uploaded a normal image. 
    /// Use async Task here to pause the test untile LoadFile finishes
    /// </summary>
    [Test]
    public async Task LoadFile_Valid_Normal_File_Should_Set_File_Path()
    {

        // Arrange
        
        Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
        
        // Render the upload fields, starting with an existing image (like the Update page does)
        var page = Render<ImageUpload>(parameters => parameters
            .Add(p => p.InitialFilePath, "/data/images/old.png"));
        
        // The mock data file 5MB
        var mockFile = new Mock<IBrowserFile>();
        mockFile.SetupGet(f => f.Name).Returns("image.png");
        mockFile.SetupGet(f => f.Size).Returns(5 * 1024 * 1024);
        
        // Bytes of a tiny fake PNG file: the 8-byte PNG signature, then a few extra bytes
        var fileBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x00 };

        // When the component opens the file, give it a stream that reads these bytes
        mockFile.Setup(f => f.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(() => new MemoryStream(fileBytes));

        // Create the file list for triggering the event
        var fileList = new List<IBrowserFile>();
        fileList.Add(mockFile.Object);
        
        // The event that InputFile raises when a file is chosen
        var dataEvent = new InputFileChangeEventArgs(fileList);
        
        // Act
        
        // Call LoadFile on the rendered component, through its renderer, as Blazor does when the user chooses a file.
        // InvokeAsync runs the call on the component's own dispatcher, and await waits until the upload finishes.
        await page.InvokeAsync(() => page.Instance.LoadFile(dataEvent));
        
        // Reset
        
        // Disk path of the image that the upload just saved in the test copy
        var savedImagePath = Path.Combine(TestFixture.DataWebRootPath, "data", "images", page.Instance.TrustedFileName);

        // Delete the file we just created
        File.Delete(savedImagePath);

        // Assert
        
        // The name should be different
        Assert.That(page.Instance.TrustedFileName, Is.Not.EqualTo(mockFile.Object.Name));
        
        // File path is set to /data/images/ + generated filename
        Assert.That(page.Instance.FilePath, Is.EqualTo("/data/images/" + page.Instance.TrustedFileName));
        
        // The error message is set to empty
        Assert.That(page.Instance.ErrorMessage, Is.EqualTo(""));
    }

    #endregion LoadFile
    
}