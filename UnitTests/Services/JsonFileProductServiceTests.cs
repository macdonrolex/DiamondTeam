
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;

using Microsoft.AspNetCore.Hosting;

using Moq;

using NUnit.Framework;

namespace UnitTests.Services
{
    /// <summary>
    /// Tests JSON product reads, ratings, updates, creation, addition, and deletion.
    /// </summary>
    [TestFixture]
    public class JsonFileProductServiceTests
    {
        private string _webRootPath;
        private JsonFileProductService _service;

        /// <summary>
        /// Creates an isolated JSON data file for each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _webRootPath = Path.Combine(Path.GetTempPath(), "JsonFileProductServiceTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(_webRootPath, "data"));

            var environment = new Mock<IWebHostEnvironment>();
            environment.SetupGet(host => host.WebRootPath).Returns(_webRootPath);
            _service = new JsonFileProductService(environment.Object);

            WriteProducts(ProductWith("game-1", new[] { 3 }));
        }

        /// <summary>
        /// Removes the temporary data file after each test.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_webRootPath))
            {
                Directory.Delete(_webRootPath, recursive: true);
            }
        }

        #region AddRating

        /// <summary>
        /// Rejects a missing product identifier before reading the data file.
        /// </summary>
        [Test]
        public void AddRating_InValid_NullProductId_Should_ReturnFalse()
        {
            // Arrange
            const string productId = null;

            // Act
            var result = _service.AddRating(productId, 1);

            // Reset
            // This test does not change the data file; TearDown removes its isolated fixture.

            // Assert
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Rejects an empty product identifier before reading the data file.
        /// </summary>
        [Test]
        public void AddRating_InValid_EmptyProductId_Should_ReturnFalse()
        {
            // Arrange
            const string productId = "";

            // Act
            var result = _service.AddRating(productId, 1);

            // Reset
            // This test does not change the data file; TearDown removes its isolated fixture.

            // Assert
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Returns false when the identifier does not match a stored product.
        /// </summary>
        [Test]
        public void AddRating_InValid_UnknownProductId_Should_ReturnFalse()
        {
            // Arrange
            const string productId = "missing";

            // Act
            var result = _service.AddRating(productId, 1);

            // Reset
            // This test does not change the data file; TearDown removes its isolated fixture.

            // Assert
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Rejects ratings below the allowed range.
        /// </summary>
        [Test]
        public void AddRating_InValid_NegativeRating_Should_ReturnFalse()
        {
            // Arrange
            const int rating = -1;

            // Act
            var result = _service.AddRating("game-1", rating);

            // Reset
            // This test does not change the data file; TearDown removes its isolated fixture.

            // Assert
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Rejects ratings above the allowed range.
        /// </summary>
        [Test]
        public void AddRating_InValid_RatingAboveMaximum_Should_ReturnFalse()
        {
            // Arrange
            const int rating = 6;

            // Act
            var result = _service.AddRating("game-1", rating);

            // Reset
            // This test does not change the data file; TearDown removes its isolated fixture.

            // Assert
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Accepts the minimum rating and initializes a missing ratings array.
        /// </summary>
        [Test]
        public void AddRating_Valid_NullRatingsAndMinimumValue_Should_InitializeAndSaveRating()
        {
            // Arrange
            WriteProducts(ProductWith("game-1", ratings: null));

            // Act
            var result = _service.AddRating("game-1", 0);
            var savedRatings = _service.GetAllData().Single().Ratings;

            // Reset
            // TearDown removes the isolated data file after this assertion.

            // Assert
            Assert.That((result, savedRatings), Is.EqualTo((true, new[] { 0 })));
        }

        /// <summary>
        /// Accepts the maximum rating and appends it to existing ratings.
        /// </summary>
        [Test]
        public void AddRating_Valid_ExistingRatingsAndMaximumValue_Should_AppendAndSaveRating()
        {
            // Arrange
            WriteProducts(ProductWith("game-1", ratings: new[] { 3 }));

            // Act
            var result = _service.AddRating("game-1", 5);
            var savedRatings = _service.GetAllData().Single().Ratings;

            // Reset
            // TearDown removes the isolated data file after this assertion.

            // Assert
            Assert.That((result, savedRatings), Is.EqualTo((true, new[] { 3, 5 })));
        }

        #endregion AddRating
        #region Test Helpers
        /// <summary>
        /// Builds a product fixture with only the identifier and ratings needed by AddRating.
        /// </summary>
        /// <param name="id">Identifier to store.</param>
        /// <param name="ratings">Initial ratings or null to exercise initialization.</param>
        /// <returns>A product record ready for serialization.</returns>
        private ProductModel ProductWith(string id, int[] ratings)
        {
            return new ProductModel
            {
                Id = id,
                Ratings = ratings
            };
        }
        #region 

        /// <summary>
        /// Writes the supplied product records to this test's isolated JSON file.
        /// </summary>
        /// <param name="products">Records to write.</param>
        private void WriteProducts(params ProductModel[] products)
        {
            var filePath = Path.Combine(_webRootPath, "data", "products.json");
            var json = JsonSerializer.Serialize(products);
            File.WriteAllText(filePath, json);
        }
#endregion Test Helpers
        /// <summary>
        /// Confirms the service keeps the environment used to locate its data file.
        /// </summary>
        [Test]
        public void Constructor_Valid_EnvironmentProvided_Should_KeepEnvironment()
        {
            
            // Arrange
            var environment = new Mock<IWebHostEnvironment>();
            environment.SetupGet(host => host.WebRootPath).Returns(_webRootPath);

            // Act
            var result = new JsonFileProductService(environment.Object);

            // Reset
            // The temporary directory is removed by TearDown.

            // Assert
            Assert.That(result.WebHostEnvironment, Is.SameAs(environment.Object));
        }
        #region GetAllData


        /// <summary>
        /// Confirms product JSON is read even when its property names use different casing.
        /// </summary>
        [Test]
        public void GetAllData_Valid_Json_Should_ReadPropertiesCaseInsensitively()
        {
            // Arrange
            File.WriteAllText(
                Path.Combine(_webRootPath, "data", "products.json"),
                "[{\"id\":\"game-2\",\"title\":\"Sample\",\"ratings\":[4]}]");

            // Act
            var result = _service.GetAllData().Single();

            // Reset
            // TearDown removes the isolated JSON file.

            // Assert
            Assert.That(result.Id, Is.EqualTo("game-2"));
            Assert.That(result.Title, Is.EqualTo("Sample"));
            Assert.That(result.Ratings, Is.EqualTo(new[] { 4 }));
        }

        /// <summary>
        /// Confirms an empty JSON array produces an empty product collection.
        /// </summary>
        [Test]
        public void GetAllData_Valid_EmptyJsonArray_Should_ReturnNoProducts()
        {
            // Arrange
            WriteProducts();

            // Act
            var result = _service.GetAllData().Count();

            // Reset
            // TearDown removes the isolated JSON file.

            // Assert
            Assert.That(result, Is.Zero);
        }
   
        #endregion GetAllData
            
        #region UpdateData

            /// <summary>
        /// Confirms UpdateData trims and saves every editable field for a matching product.
        /// </summary>
        [Test]
        public void UpdateData_Valid_ExistingProduct_AllFields_Should_TrimAndPersist()
        {
            // Arrange
            var data = new ProductModel
            {
                Id = "game-1",
                Title = "  New title  ",
                Maker = "  Maker  ",
                Description = "  Description  ",
                Post = "  Post  ",
                Image = "  image.png  ",
                ShopLink = "  shop  ",
                RatingLink = "  rating  "
            };

            // Act
            var result = _service.UpdateData(data);
            var saved = _service.GetAllData().Single();

            // Reset
            // TearDown removes the isolated JSON file.

            // Assert
            Assert.That(
                (result.Title, result.Maker, result.Description, result.Post, result.Image, result.ShopLink, result.RatingLink,
                    saved.Title, saved.Maker, saved.Description, saved.Post, saved.Image, saved.ShopLink, saved.RatingLink),
                Is.EqualTo(("New title", "Maker", "Description", "Post", "image.png", "shop", "rating",
                    "New title", "Maker", "Description", "Post", "image.png", "shop", "rating")));
        }

        /// <summary>
        /// Confirms UpdateData returns null when the requested product is not stored.
        /// </summary>
        [Test]
        public void UpdateData_InValid_UnknownProductId_Should_ReturnNull()
        {
            // Arrange
            var data = ProductWith("missing", new int[0]);

            
            // Act
            var result = _service.UpdateData(data);

            // Reset
            // TearDown removes the isolated JSON file.

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion UpdateData
        #region CreateData

        /// <summary>
        /// Confirms CreateData generates and saves a product with its placeholder values.
        /// </summary>
        [Test]
        public void CreateData_Valid_ExistingProducts_Should_CreateAndPersistPlaceholderProduct()
        {
            // Arrange
            var originalCount = _service.GetAllData().Count();

            // Act
            var result = _service.CreateData();
            var saved = _service.GetAllData().Single(product => product.Id == result.Id);

            // Reset
            // TearDown removes the isolated JSON file.

            // Assert
            Assert.That(
                (!string.IsNullOrWhiteSpace(result.Id), result.Title, result.Description, result.Image,
                    _service.GetAllData().Count(), saved.Title),
                Is.EqualTo((true, "Enter Title", "Enter Description", "", originalCount + 1, "Enter Title")));
        }

        #endregion CreateData

        /// <summary>
        /// Confirms AddProduct replaces a supplied ID, initializes missing ratings, and saves the product.
        /// </summary>
        [Test]
        public void AddProduct_Valid_NullRatings_ProductInput_Should_InitializeRatings()
        {
            // Arrange
            var product = ProductWith("caller-id", null);

            // Act
            var result = _service.AddProduct(product);
            var saved = _service.GetAllData().Single(item => item.Id == result.Id);

            // Reset
            // TearDown removes the isolated JSON file.

            // Assert
            Assert.That(result.Ratings, Is.Empty);
        }

        /// <summary>
        /// Confirms AddProduct preserves ratings that are already supplied.
        /// </summary>
        [Test]
        public void AddProduct_Valid_ExistingRatings_ProductInput_Should_GenerateIdAndPreserveRatings()
        {
            // Arrange
            var product = ProductWith("caller-id", new[] { 2, 4 });

            // Act
            var result = _service.AddProduct(product);
            var saved = _service.GetAllData().Single(item => item.Id == result.Id);

            // Reset
            // TearDown removes the isolated JSON file.

            // Assert
            
            Assert.That(
                (Guid.TryParse(result.Id, out _), result.Id == "caller-id", result.Ratings, saved.Id, saved.Ratings),
                Is.EqualTo((true, false, new[] { 2, 4 }, result.Id, new[] { 2, 4 })));
        }

        /// <summary>
        /// Confirms DeleteData returns false when the identifier is not in the data file.
        /// </summary>
        [Test]
        public void DeleteData_InValid_UnknownProductId_Should_ReturnFalse()
        {
            // Arrange
            var id = "missing";

            // Act
            var result = _service.DeleteData(id);

            // Reset
            // TearDown removes the isolated JSON file.

            // Assert
            Assert.That(result, Is.False);
        }
        #endregion AddProduct
        #region DeleteData


        /// <summary>
        /// Confirms DeleteData removes a matching product and saves the remaining collection.
        /// </summary>
        [Test]
        public void DeleteData_Valid_ExistingProductId_Should_RemoveProductAndReturnTrue()
        {
            // Arrange
            var id = "game-1";

            // Act
            var result = _service.DeleteData(id);
            var remainingCount = _service.GetAllData().Count();

            // Reset
            // TearDown removes the isolated JSON file.

            // Assert
            Assert.That((result, remainingCount), Is.EqualTo((true, 0)));
        }

        #endregion DeleteData
    }
}
