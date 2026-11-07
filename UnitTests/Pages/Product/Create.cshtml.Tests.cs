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
    }
}