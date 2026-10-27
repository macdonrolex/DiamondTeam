using NUnit.Framework;
using ContosoCrafts.WebSite.Pages.Product;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests.Pages.Product
{
    [TestFixture]
    public class UpdateTests
    {
        #region OnGet
        /// <summary>
        /// Test whether OnGet actually redirect to the Index page while the given Id is null
        /// </summary>
        [Test]
        public void OnGet_InValid_Id_Null_Should_Return_RedirectToPage_Product_Index()
        {
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var pageModel = new UpdateModel(productService);

            // Act
            var result = pageModel.OnGet(null) as RedirectToPageResult;
            
            // Reset
            // Not needed

            // Assert
            // If not RedirectToPageResult type, it will be null
            Assert.That(result, Is.Not.Null);
            // The redirected page is what we want
            Assert.That(result.PageName, Is.EqualTo("/Product/Index"));
        }
        
        /// <summary>
        /// Test whether OnGet actually redirect to the Index page while the given Id is empty.
        /// </summary>
        [Test]
        public void OnGet_InValid_Id_Empty_Should_Return_RedirectToPage_Product_Index()
        {
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var pageModel = new UpdateModel(productService);

            // Act
            var result = pageModel.OnGet("") as RedirectToPageResult;
            
            // Reset
            // Not needed

            // Assert
            // If not RedirectToPageResult type, it will be null
            Assert.That(result, Is.Not.Null);
            // The redirected page is what we want
            Assert.That(result.PageName, Is.EqualTo("/Product/Index"));
        }
        #endregion OnGet
    }
}