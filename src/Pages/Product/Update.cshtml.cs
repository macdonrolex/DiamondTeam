using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using System.Linq;

namespace ContosoCrafts.WebSite.Pages.Product
{
	/// <summary>
	/// Loads a product for editing and saves its submitted changes.
	/// </summary>
	public class UpdateModel : PageModel
	{
		// Provides product lookup and update operations for this page.
		public JsonFileProductService ProductService;

		/// <summary>
		/// Creates the update page model with the product data service.
		/// </summary>
		/// <param name="productService">Service used to read and update products.</param>
		public UpdateModel(JsonFileProductService productService)
		{
			ProductService = productService;
		}

		/// <summary>
		/// Gets or sets the product displayed in the edit form and submitted for update.
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
		/// Validates the submitted product, saves its changes, and returns to the listing.
		/// </summary>
		/// <returns>The edit page when validation fails; otherwise, a redirect to the listing.</returns>
		public IActionResult OnPost()
		{
			// Preserve the submitted values so the user can correct validation errors.
			if (!ModelState.IsValid)
			{
				return Page();
			}

			ProductService.UpdateData(Product);

			return RedirectToPage("/Product/Index");
		}
	}
}
