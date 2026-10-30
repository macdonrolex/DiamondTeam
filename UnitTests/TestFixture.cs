using System;
using System.IO;

using NUnit.Framework;

namespace UnitTests
{
    /// <summary>
    /// Shared setup for all the unit tests. It runs once, before the first test starts
    /// and after the last test finishes, and it gives the tests their own copy of the
    /// website data files.
    /// </summary>
    [SetUpFixture]
    public class TestFixture
    {
        // Path to the Web Root
        public static string DataWebRootPath = "./wwwroot";

        // Path to the data folder for the content
        public static string DataContentRootPath = "./data/";
        
        /// <summary>
        /// Runs once before any test. It replaces the test wwwroot folder with a fresh
        /// copy of the data files of the website, so every test run starts from the same data.
        /// </summary>
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            // Run this code once when the test harness starts up.

            // This will copy over the latest version of the database files

            // Resolve the wwwroot path dynamically based on the test assembly location
            // rather than hard-coding the framework version (net10.0, net11.0, etc.)
            var testAssemblyPath = AppContext.BaseDirectory;
            var DataWebPath = Path.GetFullPath(Path.Combine(testAssemblyPath, "../../../../src/wwwroot/data"));
            var DataUTDirectory = "wwwroot";
            var DataUTPath = DataUTDirectory + "/data";

            // Delete the Detination folder
            if (Directory.Exists(DataUTDirectory))
            {
                Directory.Delete(DataUTDirectory, true);
            }
            
            // Make the directory
            Directory.CreateDirectory(DataUTPath);

            // Copy over all data files
            var filePaths = Directory.GetFiles(DataWebPath);
            foreach (var filename in filePaths)
            {
                string OriginalFilePathName = filename.ToString();
                var newFilePathName = OriginalFilePathName.Replace(DataWebPath, DataUTPath);

                File.Copy(OriginalFilePathName, newFilePathName);
            }
        }
        
        /// <summary>
        /// Runs once after all the tests finish. There is nothing to clean up, because
        /// the test data folder is deleted and created again at the next test run.
        /// </summary>
        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
        }
    }
}