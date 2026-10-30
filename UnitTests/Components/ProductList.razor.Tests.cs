using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Bunit;
using NUnit.Framework;

using ContosoCrafts.WebSite.Components;
using ContosoCrafts.WebSite.Services;

namespace UnitTests.Components
{
    /// <summary>
    /// Unit tests for ProductList.razor
    /// </summary>
    public class ProductListTests : BunitTestContext
    {
        #region TestSetup

        [SetUp]
        public void TestInitialize()
        {
        }

        #endregion TestSetup
        /// <summary>
        /// Test whether the home page can retrieve data from database
        /// </summary>
        [Test]
        public void ProductList_Valid_Default_Should_Return_Content()
        {
            // Arrange
            Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);

            // Act
            var page = Render<ProductList>();

            // Get the Cards retrned
            var result = page.Markup;

            // Assert
            Assert.That(result.Contains("Minecraft"), Is.True);
        }
    }
}