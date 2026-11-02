using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoCrafts.WebSite.Pages.Product
{
    /// <summary>
    /// Handles the form for creating a new product.
    /// </summary>
    public class CreateModel : PageModel
    {
        // Provides the operation that stores a submitted product.
        public readonly JsonFileProductService ProductService;

        /// <summary>
        /// Creates the product creation page model with its data service.
        /// </summary>
        /// <param name="productService">Service used to add the new product.</param>
        public CreateModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        /// <summary>
        /// Gets or sets the product fields bound from the creation form.
        /// </summary>
        [BindProperty]
        public ProductModel Product { get; set; } = new ProductModel();

        /// <summary>
        /// Handles a request to display the product creation form.
        /// </summary>
        public void OnGet()
        {
        }
        /// <summary>
        /// Validates and stores the submitted product, then returns to the listing.
        /// </summary>
        /// <returns>The form when validation fails; otherwise, a redirect to the listing.</returns>
        public IActionResult OnPost()
        {
            // Return the form with validation messages when model binding finds invalid data.
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            // Record creation time in UTC so it is independent of the server's local time zone.
            Product.DateAdded = System.DateTime.UtcNow;
            ProductService.AddProduct(Product);

            return RedirectToPage("/Product/Index");
        }
    }
}
