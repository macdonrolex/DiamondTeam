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
            
            // Reset
            // Not needed

            // Assert
            Assert.That(result.Contains("Minecraft"), Is.True);
        }

        #region SelectProduct
        /// <summary>
        ///  Open the popup, and see if the content is what we want.
        /// </summary>
        [Test]
        public void SelectProduct_Valid_ID_jenlooper_Should_Return_Content()
        {
            // Arrange
            Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
            var id = "MoreInfoButton_jenlooper-cactus";

            var page = Render<ProductList>();
            
            // Find the Buttons (more info)
            var buttonList = page.FindAll<ProductList>("Button");
            
            // Find the one that matches the ID looking for and click it
            var button = buttonList.First((m => m.OuterHtml.Contains(id)));
            
            // Act
            button.Click();
            
            // Get the markup to use for the assert
            var pageMarkup = page.Markup;

            // Reset
            // Not Needed

            // Assert
            Assert.That(
                pageMarkup.Contains(
                    "Minecraft is a sandbox video game developed and published by the Swedish company Mojang Studios."),
                Is.True);
        }
        #endregion SelectProduct

        #region SubmitRating

        
        #endregion SubmitRating
    }
}