using NUnit.Framework;
using ContosoCrafts.WebSite.Pages;


namespace UnitTests.Pages.About
{
    public class AboutTests
    {
        #region OnGet
        /// <summary>
        /// Just execute the OnGet
        /// </summary>
        [Test]
        public void OnGet_Valid_Should_Open_About_Page()
        {
            // Arrange
            var pageModel = new AboutModel();
            
            // Act
            pageModel.OnGet();

            // Reset
            // Not needed
            
            // Assert
            Assert.That(pageModel.ModelState.IsValid, Is.True);
        }
        #endregion OnGet
    }
}