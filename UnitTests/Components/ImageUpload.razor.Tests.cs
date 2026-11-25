using System.Collections.Generic;
using System.Threading.Tasks;
using ContosoCrafts.WebSite.Components;
using Microsoft.AspNetCore.Components.Forms;
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

    #endregion LoadFile
    
}