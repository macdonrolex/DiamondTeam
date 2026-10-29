using System.Collections.Generic;
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
            
            // Store all necessary fields needed to be filled
            List<string> fields = new List<string>();
            fields.Add(Product.Title);
            fields.Add(Product.Maker);
            fields.Add(Product.Description);
            fields.Add(Product.Post);
            fields.Add(Product.Image);
            fields.Add(Product.RatingLink);
            fields.Add(Product.ShopLink);
            
            // Require at least one entered field so an empty form does not create a blank product.
            bool allEmpty = true;
            foreach (var field in fields)
            {
                // If one field has something
                if (!string.IsNullOrWhiteSpace(field))
                {
                    allEmpty = false;
                }
            }
            if (allEmpty)
            {
                ModelState.AddModelError(string.Empty, "Add at least one game post detail before submitting.");
                return Page();
            }

            // Record creation time in UTC so it is independent of the server's local time zone.
            Product.DateAdded = System.DateTime.UtcNow;
            ProductService.AddProduct(Product);

            return RedirectToPage("/Product/Index");
        }
    }
}
