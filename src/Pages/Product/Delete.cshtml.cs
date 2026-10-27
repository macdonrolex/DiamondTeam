using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using System.Linq;

namespace ContosoCrafts.WebSite.Pages.Product;

/// <summary>
/// Loads a product for confirmation and handles its deletion.
/// </summary>
public class Delete : PageModel
{
    // Provides product lookup and deletion operations for this page.
    public readonly JsonFileProductService ProductService;

    /// <summary>
    /// Creates the delete page model with the product data service.
    /// </summary>
    /// <param name="productService">Service used to find and delete products.</param>
    public Delete(JsonFileProductService productService)
    {
        ProductService = productService;
    }
    
    /// <summary>
    /// Gets or sets the product displayed for confirmation and submitted for deletion.
    /// </summary>
    [BindProperty]
    public ProductModel Product { get; set; }

    /// <summary>
    /// Finds the product selected for deletion.
    /// </summary>
    /// <param name="id">Identifier of the product to load.</param>
    /// <returns>The details page when a product exists; otherwise, a redirect to the listing.</returns>
    public IActionResult OnGet(string id)
    {
        // A missing identifier cannot select a product, so return to the listing.
        if (string.IsNullOrEmpty(id))
        {
            return RedirectToPage("/Product/Index");
        }

        Product = ProductService.GetAllData().FirstOrDefault(x => x.Id == id);

        // Redirect when the supplied identifier does not match a stored product.
        if (Product == null)
        {
            return RedirectToPage("/Product/Index");
        }

        return Page();
    }

    /// <summary>
    /// Validates the submitted model, deletes the product, and returns to the listing.
    /// </summary>
    /// <returns>The current page when validation fails; otherwise, a redirect to the listing.</returns>
    public IActionResult OnPost()
    {
        // Prevent someone calling OnPost without calling OnGet first.
        if (Product == null)
        {
            return RedirectToPage("/Product/Index");
        }

        ProductService.DeleteData(Product.Id);
        return RedirectToPage("/Product/Index");
    }
}
