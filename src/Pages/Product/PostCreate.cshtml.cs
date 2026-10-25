using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoCrafts.WebSite.Pages.Product
{
    public class PostCreateModel : PageModel
    {
        private readonly JsonFileProductService _productService;

        public PostCreateModel(JsonFileProductService productService)

        {

            _productService = productService;

        }

        [BindProperty]

        public ProductModel Product { get; set; } = new ProductModel();
        public void OnGet()
        {
        }
        public IActionResult OnPost()

        {

            if (!ModelState.IsValid)

            {

                return Page();

            }

            Product.DateAdded = System.DateTime.UtcNow;
            _productService.AddProduct(Product);

            return RedirectToPage("/Product/Index");
        }


        }
    }
