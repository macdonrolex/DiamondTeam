using System.Linq;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using ContosoCrafts.WebSite.Pages;

namespace UnitTests.Pages.Index
{
    /// <summary>
    ///  Unit tests for Index.cshtml.cs and Index.cshtml under Pages
    /// </summary>
    public class IndexTests
    {
        #region TestSetup

        public static IndexModel pageModel;
        
        /// <summary>
        /// Test setup for IndexTests
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            var MockLoggerDirect = Mock.Of<ILogger<IndexModel>>();

            pageModel = new IndexModel(MockLoggerDirect, TestHelper.ProductService)
            {
            };
        }

        #endregion TestSetup

        #region OnGet
        /// <summary>
        /// Test whether OnGet loads the game posts into the Products property
        /// of the Index page (home page).
        /// </summary>
        [Test]
        public void OnGet_Valid_Should_Return_Products()
        {
            // Arrange

            // Act
            pageModel.OnGet();
            
            // Reset

            // Assert
            Assert.That(pageModel.ModelState.IsValid, Is.True);
            Assert.That(pageModel.Products.ToList().Any(), Is.True);
        }
        #endregion OnGet
    }
}