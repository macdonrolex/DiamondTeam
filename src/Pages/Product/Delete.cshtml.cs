using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using System.Linq;

namespace ContosoCrafts.WebSite.Pages.Product;

public class Delete : PageModel
{
    private readonly JsonFileProductService ProductService;

    public Delete(JsonFileProductService productService)
    {
        ProductService = productService;
    }
    
    // The data to show, bind to it for the post
    [BindProperty]
    public ProductModel Product { get; set; }
    public void OnGet(string id)
    {
        Product = ProductService.GetAllData().FirstOrDefault(m => m.Id.Equals(id));
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        ProductService.DeleteData(Product.Id);
        
        return RedirectToPage("/Product/Index");
    }
}