using NUnit.Framework;
using ContosoCrafts.WebSite.Pages.Product;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UnitTests.Pages.Product
{
    [TestFixture]
    public class DeleteTests
    {
        #region OnGet
        /// <summary>
        /// Test whether OnGet actually retrieves the product(post) with given Id.
        /// </summary>
        [Test]
        public void OnGet_Valid_Should_Load_Product()
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
        #endregion OnGet
    }
}