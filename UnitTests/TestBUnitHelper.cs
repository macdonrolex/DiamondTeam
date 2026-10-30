using Bunit;
using NUnit.Framework;

namespace UnitTests
{
    /// <summary>
    /// Test Context used by bUnit
    /// </summary>
    public abstract class BunitTestContext : Bunit.TestContext
    {
        /// <summary>
        /// The Setup sets the context
        /// </summary>
        [SetUp]
        public void Setup()
        {
            
        }

        /// <summary>
        /// When done displose removes it, to free up system resources
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            
            this.Dispose();
        }
    }
}