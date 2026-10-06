using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.RazorPages; 
using Microsoft.Extensions.Logging;

using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
<<<<<<< HEAD
// I love Seattle Winter
=======
// I love Seattle Summers
>>>>>>> c03f1c9 (Added Summers in Index.cshtml.cs for merge)
namespace ContosoCrafts.WebSite.Pages
{
    public class IndexModel : PageModel
    {
        //Hi Mike
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger,
            JsonFileProductService productService)
        {
            _logger = logger;
            ProductService = productService;
        }

        public JsonFileProductService ProductService { get; }
        public IEnumerable<ProductModel> Products { get; private set; }

        public void OnGet()
        {
            Products = ProductService.GetAllData();
        }
    }
}