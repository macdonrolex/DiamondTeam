using System.Linq;
using ContosoCrafts.WebSite.Controllers;
using NUnit.Framework;
using ContosoCrafts.WebSite.Pages.Product;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UnitTests.Controllers
{
    /// <summary>
    ///  Unit tests for ProductsController.cs 
    /// </summary>
    [TestFixture]
    public class ProductsControllerTests
    {
        #region Get
        /// <summary>
        /// Get should return all product data in the database
        /// </summary>
        [Test]
        public void Get_Valid_Should_Return_All_Products()
        {
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var productsController = new ProductsController(productService);
            
            // Get total entries count in database
            var dataCount = productService.GetAllData().ToList().Count;

            // Act
            var result = productsController.Get().ToList().Count;

            // Reset
            // Not needed

            // Assert
            Assert.That(result, Is.EqualTo(dataCount));
        }
        #endregion Get
    }
}