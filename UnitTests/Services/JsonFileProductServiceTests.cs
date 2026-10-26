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
    /// Verifies AddRating input guards, rating boundaries, and persisted updates.
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

        /// <summary>
        /// Rejects a missing product identifier before reading the data file.
        /// </summary>
        [Test]
        public void AddRating_NullProductId_Should_ReturnFalse()
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
        public void AddRating_EmptyProductId_Should_ReturnFalse()
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
        public void AddRating_UnknownProductId_Should_ReturnFalse()
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
        public void AddRating_NegativeRating_Should_ReturnFalse()
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
        public void AddRating_RatingAboveMaximum_Should_ReturnFalse()
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
        public void AddRating_NullRatingsAndMinimumValue_Should_InitializeAndSaveRating()
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
        public void AddRating_ExistingRatingsAndMaximumValue_Should_AppendAndSaveRating()
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

        /// <summary>
        /// Writes the supplied product records to this test's isolated JSON file.
        /// </summary>
        /// <param name="products">Records to write.</param>
        private void WriteProducts(params ProductModel[] products)
        {
            var json = JsonSerializer.Serialize(products);
            File.WriteAllText(Path.Combine(_webRootPath, "data", "products.json"), json);
        }
    }
}
