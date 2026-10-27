using NUnit.Framework;
using ContosoCrafts.WebSite.Pages.Product;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UnitTests.Pages.Product
{
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
            var pageModel = new PostCreateModel(productService);

            // Act
            pageModel.OnGet();
            
            // Reset
            // Not needed

            // Assert
            Assert.That(pageModel.Product, Is.Not.Null);
        }
        #endregion OnGet
        
        #region OnPost
        [Test]
        public void OnPost_InValid_All_Fields_Empty_Should_Return_Page()
        {
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var pageModel = new PostCreateModel(productService);
            
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
            
            // Reset
            // Not needed
            
            // Assert
            Assert.That(result, Is.TypeOf<PageResult>());
        }
        #endregion OnPost
    }
}