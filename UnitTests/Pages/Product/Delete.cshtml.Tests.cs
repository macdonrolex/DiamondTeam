using System.IO;
using System.Linq;
using NUnit.Framework;
using ContosoCrafts.WebSite.Pages.Product;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UnitTests.Pages.Product
{
    /// <summary>
    ///  Unit tests for Delete.cshtml.cs and Delete.cshtml
    /// </summary>
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
        /// Test whether OnGet actually redirect to the Index page while the given Id is empty.
        /// </summary>
        [Test]
        public void OnGet_InValid_Id_Empty_Should_Return_RedirectToPage_Product_Index()
        {
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var pageModel = new Delete(productService);

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
        
        /// <summary>
        /// Test whether OnGet actually redirect to the Index page while the given Id doesn't belong to any product(post).
        /// </summary>
        [Test]
        public void OnGet_InValid_Product_Id_Not_Exist_Should_Return_RedirectToPage_Product_Index()
        {
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var pageModel = new Delete(productService);

            // Act
            var result = pageModel.OnGet("Non_Exist_Id") as RedirectToPageResult;
            
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
            // Not needed

            // Assert
            // The result is the page itself, not a redirect
            Assert.That(result, Is.TypeOf<PageResult>());
        }
        #endregion OnGet
        
        #region OnPost
        /// <summary>
        /// Test whether OnPost redirects to the index page while Product(post) is null
        /// </summary>
        [Test]
        public void OnPost_InValid_Product_Null_Should_Return_RedirectToPage_Product_Index()
        {
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var pageModel = new Delete(productService);
            
            pageModel.Product = null;
            
            // Act
            var result = pageModel.OnPost() as RedirectToPageResult;
            
            // Reset
            // Not needed
            
            // Assert
            // If not RedirectToPageResult type, it will be null
            Assert.That(result, Is.Not.Null);
            // The redirected page is what we want
            Assert.That(result.PageName, Is.EqualTo("/Product/Index"));
        }
        
        /// <summary>
        /// Test whether OnPost deletes the product(post) if it exists.
        /// </summary>
        [Test]
        public void OnPost_Valid_Product_Exist_Should_Delete_Product()
        {
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var pageModel = new Delete(productService);
            
            // Put data into product
            pageModel.Product = productService.GetAllData().FirstOrDefault(x => x.Id == "jenlooper-light");
            
            // Backup original database for resetting.
            var path = Path.Combine(TestFixture.DataWebRootPath, "data", "products.json");
            var backupDatabase = File.ReadAllText(path);
            
            // Act
            pageModel.OnPost();
            
            // Try to get the product with the same Id
            var resultPageModel = productService.GetAllData().FirstOrDefault(x => x.Id == "jenlooper-light");
            
            // Reset
            // Write the original texts in the database back to modified database
            File.WriteAllText(path, backupDatabase);
            
            // Assert
            // Cannot get the product with the Id
            Assert.That(resultPageModel, Is.Null);
        }
        
        /// <summary>
        /// Test whether OnPost redirects to the index page after deleting a product(post)
        /// </summary>
        [Test]
        public void OnPost_Valid_Product_Exist_Should_Return_RedirectToPage_Product_Index()
        {
            // Arrange
            var webHostEnvironment = TestHelper.MockWebHostEnvironment.Object;
            var productService = new JsonFileProductService(webHostEnvironment);
            var pageModel = new Delete(productService);
            
            // Put data into product
            pageModel.Product = productService.GetAllData().FirstOrDefault(x => x.Id == "jenlooper-light");
            
            // Backup original database for resetting.
            var path = Path.Combine(TestFixture.DataWebRootPath, "data", "products.json");
            var backupDatabase = File.ReadAllText(path);
            
            // Act
            var result = pageModel.OnPost() as RedirectToPageResult;
            
            // Reset
            // Write the original texts in the database back to modified database
            File.WriteAllText(path, backupDatabase);
            
            // Assert
            // If not RedirectToPageResult type, it will be null
            Assert.That(result, Is.Not.Null);
            // The redirected page is what we want
            Assert.That(result.PageName, Is.EqualTo("/Product/Index"));
        }
        #endregion OnPost
    }
}