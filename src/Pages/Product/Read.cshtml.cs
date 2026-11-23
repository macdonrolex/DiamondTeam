using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using System.Collections.Generic;
using System.Linq;

namespace ContosoCrafts.WebSite.Pages.Product;

/// <summary>
/// Loads one product for the product details page.
/// </summary>
public class ReadModel : PageModel
{

    // Provides access to the product records shown by this page.
    public readonly JsonFileProductService ProductService;

    /// <summary>
    /// Creates the details page model with the product data service.
    /// </summary>
    /// <param name="productService">Service used to find product records.</param>
    public ReadModel(JsonFileProductService productService)
    {
        ProductService = productService;
    }

    /// <summary>
    /// Gets the product selected for display, or null when no product was found.
    /// </summary>
    public ProductModel Product { get; private set; }

    /// <summary>
    /// Gets related products shown with the selected product.
    /// </summary>
    public List<ProductModel> OtherProducts { get; set; } = new List<ProductModel>();

    /// <summary>
    /// Finds the requested product and displays it, or redirects when it is missing.
    /// </summary>
    /// <param name="id">Identifier of the product to display.</param>
    /// <returns>The details page when a product exists; a redirect to the listing when no id is given; otherwise, Not Found.</returns>
    public IActionResult OnGet(string id)
    {

        // A missing identifier cannot select a product, so return to the listing.
        if (string.IsNullOrEmpty(id))
        {
            return RedirectToPage("/Product/Index");
        }

        var products = ProductService.GetAllData();

        Product = products.FirstOrDefault(x => x.Id == id);

        // Show the "page not found" card when the supplied identifier does not match a stored product.
        if (Product == null)

        {
            return NotFound();
        }

        OtherProducts = products
            .Where(x => (x.Id == Product.Id) == false)

            .Take(3)

            .ToList();

        return Page();
    }

}