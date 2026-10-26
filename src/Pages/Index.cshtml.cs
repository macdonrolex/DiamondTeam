using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.RazorPages; 
using Microsoft.Extensions.Logging;

using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
namespace ContosoCrafts.WebSite.Pages
{
    /// <summary>
    /// Loads the product data displayed on the site's home page.
    /// </summary>
    public class IndexModel : PageModel
    {
        // Logger injected for the page model; retained for diagnostics as the page evolves.
        private readonly ILogger<IndexModel> _logger;

        /// <summary>
        /// Creates the home page model with its logging and product data dependencies.
        /// </summary>
        /// <param name="logger">Logger associated with this page model.</param>
        /// <param name="productService">Service that reads product data.</param>
        public IndexModel(ILogger<IndexModel> logger,
            JsonFileProductService productService)
        {
            _logger = logger;
            ProductService = productService;
        }

        /// <summary>
        /// Gets the service used to retrieve products.
        /// </summary>
        public JsonFileProductService ProductService { get; }

        /// <summary>
        /// Gets the products shown on the home page after the request is handled.
        /// </summary>
        public IEnumerable<ProductModel> Products { get; private set; }

        /// <summary>
        /// Loads the product collection when the home page receives a GET request.
        /// </summary>
        public void OnGet()
        {
            Products = ProductService.GetAllData();
        }
    }
}
