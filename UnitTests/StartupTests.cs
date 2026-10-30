using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

using NUnit.Framework;

namespace UnitTests.Pages.Startup
{
    /// <summary>
    /// Unit tests for Startup.cs
    /// </summary>
    public class StartupTests
    {
        #region TestSetup
        /// <summary>
        /// Runs before each test. There is nothing to prepare
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
        }
        
        /// <summary>
        /// Test version of the website Startup class. It inherits everything from the
        /// real Startup, and it gives the web host builder a type that it can create.
        /// </summary>
        public class Startup : ContosoCrafts.WebSite.Startup
        {
            public Startup(IConfiguration config) : base(config) { }
        }
        #endregion TestSetup

        #region ConfigureServices
        /// <summary>
        /// Test whether the web host builds with the default settings, which also
        /// runs the service registration of Startup.
        /// </summary>
        [Test]
        public void Startup_ConfigureServices_Valid_Default_Should_Pass()
        {
            // Arrange
            
            // Act
            // Build the web host with the default builder and the test Startup class
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
            
            // Reset
            
            // Assert
            // The host was built, so the services were registered without an error
            Assert.That(webHost, Is.Not.Null);
        }
        #endregion ConfigureServices

        #region Configure
        /// <summary>
        /// Test whether the web host builds with the default settings, so the
        /// request pipeline setup of Startup can be created.
        /// </summary>
        [Test]
        public void Startup_Configure_Valid_Default_Should_Pass()
        {
            // Arrange

            // Act
            // Build the web host with the default builder and the test Startup class
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
            
            // Reset
            
            // Assert
            // The host was built without an error
            Assert.That(webHost, Is.Not.Null);
        }

        #endregion Configure
    }
}