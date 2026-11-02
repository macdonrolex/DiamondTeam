using System.Linq;
using AngleSharp.Common;
using NUnit.Framework;
using ContosoCrafts.WebSite.Pages.Product;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UnitTests.Pages.Product
{
    /// <summary>
    ///  Unit tests for Create.cshtml.cs and Create.cshtml
    /// </summary>
    [TestFixture]
    public class CreateTests
    {
        #region OnGet
        /// <summary>
        /// After calling OnGet, it shall initialize a new Product object
        /// </summary>
        [Test]
        public void OnGet_Valid_Product_Should_Not_Be_Null()
        {
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var pageModel = new CreateModel(productService);

            // Act
            pageModel.OnGet();
            
            // Reset
            // Not needed

            // Assert
            Assert.That(pageModel.Product, Is.Not.Null);
        }
        #endregion OnGet
        
        #region OnPost
        /// <summary>
        /// Invalid model should return the page.
        /// </summary>
        [Test]
        public void OnPost_InValid_Model_Invalid_Should_Return_Page()
        {
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var pageModel = new CreateModel(productService);
            
            // Force an invalid error state
            pageModel.ModelState.AddModelError("bogus", "bogus error");

            // Act
            var result = pageModel.OnPost();
            
            // Reset
            // Not needed

            // Assert
            // The result is the page itself, not a redirect
            Assert.That(result, Is.TypeOf<PageResult>());
        }
        
        /// <summary>
        /// If the user didn't fill in any content, return the same page with error message.
        /// </summary>
        [Test]
        public void OnPost_InValid_All_Fields_Empty_Should_Return_Page_With_Errors()
        {
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var pageModel = new CreateModel(productService);
            
            // Set all fields to empty
            pageModel.Product.Title = "";
            pageModel.Product.Maker = "";
            pageModel.Product.Description = "";
            pageModel.Product.Post = "";
            pageModel.Product.Image = "";
            pageModel.Product.RatingLink = "";
            pageModel.Product.ShopLink = "";
            
            // Act
            var result = pageModel.OnPost();
            // Get the error message from the errors collection of string.Empty
            var resultMessage = pageModel.ModelState[string.Empty].Errors[0].ErrorMessage;
            
            // Reset
            // Not needed
            
            // Assert
            Assert.That(result, Is.TypeOf<PageResult>());
            Assert.That(pageModel.ModelState.IsValid, Is.False);
            Assert.That(resultMessage, Is.EqualTo("Add at least one game post detail before submitting."));
        }
        
        /// <summary>
        /// After creating, return to Index page
        /// </summary>
        [Test]
        public void OnPost_Valid_Should_Return_Index_Page()
        {
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var pageModel = new CreateModel(productService);
            
            // Get the first data, and modify it to a new data
            pageModel.Product = productService.GetAllData().First();
            pageModel.Product.Title = "test_title";
            
            // Act
            var result = pageModel.OnPost() as RedirectToPageResult;
            
            // The data Id used to delete in the reset stage
            var dataId = pageModel.Product.Id;
            
            // Reset
            // Delete the data created in Act
            productService.DeleteData(dataId);
            
            // Assert
            // If not RedirectToPageResult type, it will be null
            Assert.That(result, Is.Not.Null);
            // The redirected page is what we want
            Assert.That(result.PageName, Is.EqualTo("/Product/Index"));
        }
        #endregion OnPost
    }
}