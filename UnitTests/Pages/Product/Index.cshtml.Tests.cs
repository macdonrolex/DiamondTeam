using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ContosoCrafts.WebSite.Pages.Product;
using ContosoCrafts.WebSite.Services;

namespace UnitTests.Pages.Product.Index;

/// <summary>
///  Unit tests for Index.cshtml.cs and Index.cshtml under Pages/Product
/// </summary>
[TestFixture]
public class IndexTests
{
    #region TestSetup

    public static IUrlHelperFactory urlHelperFactory;
    public static DefaultHttpContext httpContextDefault;
    public static IWebHostEnvironment webHostEnvironment;
    public static ModelStateDictionary modelState;
    public static ActionContext actionContext;
    public static EmptyModelMetadataProvider modelMetadataProvider;
    public static ViewDataDictionary viewData;
    public static TempDataDictionary tempData;
    public static PageContext pageContext;
    public static IndexModel pageModel;

    /// <summary>
    /// Creates the fake web objects and the Index page model before each test,
    /// so every test starts with a clean page model that reads the real data file.
    /// </summary>
    [SetUp]
    public void TestInitialize()
    {
        httpContextDefault = new DefaultHttpContext()
        {
            //RequestServices = serviceProviderMock.Object,
        };

        modelState = new ModelStateDictionary();

        actionContext = new ActionContext(httpContextDefault, httpContextDefault.GetRouteData(),
            new PageActionDescriptor(), modelState);

        modelMetadataProvider = new EmptyModelMetadataProvider();
        viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
        tempData = new TempDataDictionary(httpContextDefault, Mock.Of<ITempDataProvider>());

        pageContext = new PageContext(actionContext)
        {
            ViewData = viewData,
        };

        var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        mockWebHostEnvironment.Setup(m => m.EnvironmentName).Returns("Hosting:UnitTestEnvironment");

        // Resolve the wwwroot path dynamically based on the test assembly location
        // rather than hard-coding the framework version (net10.0, net11.0, etc.)
        var testAssemblyPath = AppContext.BaseDirectory;
        var wwwrootPath = Path.GetFullPath(Path.Combine(testAssemblyPath, "../../../../src/wwwroot"));
        mockWebHostEnvironment.Setup(m => m.WebRootPath).Returns(wwwrootPath);
        mockWebHostEnvironment.Setup(m => m.ContentRootPath).Returns("./data/");

        var MockLoggerDirect = Mock.Of<ILogger<IndexModel>>();
        JsonFileProductService productService;

        productService = new JsonFileProductService(mockWebHostEnvironment.Object);

        pageModel = new IndexModel(productService)
        {
        };
    }

    #endregion TestSetup

    #region OnGet

    /// <summary>
    /// If we cannot load the products(posts), display only one page.
    /// </summary>
    [Test]
    public void OnGet_InValid_Product_Null_Should_Get_1_Page()
    {
        // Arrange

        // Act
        pageModel.OnGet();
        
        // Assume we cannot load the posts
        pageModel.Products = null;

        var result = pageModel.TotalPages;

        // Reset
        
        // Not needed

        // Assert
        Assert.That(result, Is.EqualTo(1));
    }

    /// <summary>
    /// If we cannot load the products(posts), get no products
    /// </summary>
    [Test]
    public void OnGet_InValid_Product_Null_Should_Get_Empty_Products_Enum()
    {
        // Arrange

        // Act
        pageModel.OnGet();
        
        // Assume we cannot load the posts
        pageModel.Products = null;

        var result = pageModel.PagedProducts;

        // Reset
        
        // Not needed

        // Assert
        Assert.That(result, Is.Empty);
    }

    /// <summary>
    /// Each page should display only [current page size] posts
    /// </summary>
    [Test]
    public void OnGet_Valid_Should_Get_PageSize_Products_In_Each_Page()
    {
        // Arrange

        // Act
        pageModel.OnGet();
        var result = pageModel.PagedProducts.Count();

        // Reset
        
        // Not needed

        // Assert
        Assert.That(result, Is.EqualTo(4));
    }

    /// <summary>
    /// Test whether the index page can retrieve  all the products
    /// </summary>
    [Test]
    public void OnGet_Valid_Should_Return_Products()
    {
        // Arrange
        // Get total entries count in database
        var dataCount = pageModel.ProductService.GetAllData().ToList().Count;

        // Act
        pageModel.OnGet();

        // Reset
        
        // Not needed

        // Assert
        Assert.That(pageModel.ModelState.IsValid, Is.True);
        Assert.That(pageModel.Products.ToList().Count, Is.EqualTo(dataCount));
    }

    #endregion OnGet
    
}