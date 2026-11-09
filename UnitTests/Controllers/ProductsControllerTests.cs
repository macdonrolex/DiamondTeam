using System.Linq;
using ContosoCrafts.WebSite.Controllers;
using ContosoCrafts.WebSite.Models;
using NUnit.Framework;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;

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

        #region Patch
        
        /// <summary>
        /// Patch should add a new rating and return OK result
        /// </summary>
        [Test]
        public void Patch_Valid_Should_Return_OK()
        {
            
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var productsController = new ProductsController(productService);
            var ratingRequestModel = new RatingRequestModel();
            
            // Ready to add a new rating to the first entry
            ratingRequestModel.ProductId = productService.GetAllData().ToList().First().Id;
            ratingRequestModel.Rating = 5;
            
            // Backup original database for resetting.
            var backupData = productService.GetAllData().First();

            // Act
            var result = productsController.Patch(ratingRequestModel);
            var resultModel = productService.GetAllData().First();
            var resultRating = resultModel.Ratings.Last();

            // Reset
            
            // Update with the original first data in database
            productService.UpdateData(backupData);

            // Assert
            Assert.That(result, Is.TypeOf<OkResult>());
            Assert.That(resultRating, Is.EqualTo(5));
        }
        
        #endregion Patch
        
    }
    
}