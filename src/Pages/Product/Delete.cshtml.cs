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
    /// <returns>The details page when a product exists; a redirect to the listing when no id is given; otherwise, Not Found.</returns>
    public IActionResult OnGet(string id)
    {

        // A missing identifier cannot select a product, so return to the listing.
        if (string.IsNullOrEmpty(id))
        {
            return RedirectToPage("/Product/Index");
        }

        Product = ProductService.GetAllData().FirstOrDefault(x => x.Id == id);

        // Show the "page not found" card when the supplied identifier does not match a stored product.
        if (Product == null)
        {
            return NotFound();
        }

        return Page();
    }

    /// <summary>
    /// Deletes the product(post) if it exists, and returns to the listing.
    /// </summary>
    /// <returns>A redirect to the listing after deleting; Not Found when the post was already deleted.</returns>
    public IActionResult OnPost()
    {

        // Prevent someone calling OnPost without calling OnGet first.
        if (Product == null)
        {
            return RedirectToPage("/Product/Index");
        }

        // The post was already deleted (for example in another tab), so show the Error page
        if (ProductService.DeleteData(Product.Id) == false)
        {
            return NotFound();
        }

        return RedirectToPage("/Product/Index");
    }

}