using NUnit.Framework;
using ContosoCrafts.WebSite.Pages.Product;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UnitTests.Pages.Product
{
    [TestFixture]
    public class DeleteTests
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
            var pageModel = new Delete(productService);

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
        /// Test whether OnGet actually retrieves the product(post) with given Id.
        /// </summary>
        [Test]
        public void OnGet_Valid_Id_Exist_Should_Load_Product()
        {
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var pageModel = new Delete(productService);

            // Act
            pageModel.OnGet("jenlooper-light"); 
            
            // Reset
            // Not needed

            // Assert
            // The post was actually loaded into the page
            Assert.That(pageModel.Product, Is.Not.Null);
            Assert.That(pageModel.Product.Id, Is.EqualTo("jenlooper-light"));
        }
        
        /// <summary>
        /// Test whether OnGet returns the page when the product(post) with the given id exists.
        /// </summary>
        [Test]
        public void OnGet_Valid_Id_Exist_Should_Return_Page()
        {
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var pageModel = new Delete(productService);

            // Act
            var result = pageModel.OnGet("jenlooper-light"); 
            
            // Reset
            // No needed

            // Assert
            // The result is the page itself, not a redirect
            Assert.That(result, Is.TypeOf<PageResult>());
        }
        #endregion OnGet
    }
}