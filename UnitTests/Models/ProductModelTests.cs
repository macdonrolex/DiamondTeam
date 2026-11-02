using System.Text.Json;
using ContosoCrafts.WebSite.Models;
using NUnit.Framework;

namespace UnitTests.Models
{
    /// <summary>
    ///  Unit tests for ProductsModel.cs 
    /// </summary>
    [TestFixture]
    public class ProductModelTests
    {
        #region ToString
        /// <summary>
        /// Test whether ToString actually converts the entry to its JSON representation.
        /// </summary>
        [Test]
        public void ToString_Valid_Should_Convert_To_JSON()
        {
            // Arrange
            var productModel = new ProductModel();
            
            // Set up required attributes
            productModel.Id = "test_id";
            productModel.Maker = "test_maker";
            productModel.Title = "test_title";
            productModel.Description = "test_description";
            productModel.Post = "test_post";

            // Act
            var resultJson = productModel.ToString();
            // Deserialize the result back
            var result = JsonSerializer.Deserialize<ProductModel>(resultJson);

            // Reset
            // Not needed

            // Assert
            // Test whether successfully deserialize
            Assert.That(result, Is.Not.TypeOf<JsonException>());
            Assert.That(result.Id, Is.EqualTo("test_id"));
            Assert.That(result.Maker, Is.EqualTo("test_maker"));
            Assert.That(result.Title, Is.EqualTo("test_title"));
            Assert.That(result.Description, Is.EqualTo("test_description"));
            Assert.That(result.Post, Is.EqualTo("test_post"));
        }
        #endregion ToString
    }
}