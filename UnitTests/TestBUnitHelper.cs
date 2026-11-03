using Bunit;
using NUnit.Framework;

namespace UnitTests
{
    /// <summary>
    /// Test Context used by bUnit
    ///
    /// [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    /// A new instance of the test class is created for each test removing the need for the wrapper.
    /// see https://bunit.dev/docs/getting-started/writing-tests.html?tabs=nunit
    /// </summary>
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
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