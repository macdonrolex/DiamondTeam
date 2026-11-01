using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;

namespace ContosoCrafts.WebSite.Pages.Product
{
    /// <summary>
    /// Loads all products for the product listing page.
    /// </summary>
    public class IndexModel : PageModel
    {
        /// <summary>
        /// Creates the page model with the product data service.
        /// </summary>
        /// <param name="productService">Service that reads product records.</param>
        public IndexModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        /// <summary>
        /// Gets the service used to read product records.
        /// </summary>
        public JsonFileProductService ProductService { get; }

        /// <summary>
        /// Gets the products loaded for the listing page.
        /// </summary>
        public IEnumerable<ProductModel> Products { get; private set; }

        // Maximum number of posts shown on one page.
        public const int PageSize = 4;

        // The page currently selected by the visitor.
        public int CurrentPage { get; private set; } = 1;

        // Total number of pages needed to show all posts.
        public int TotalPages
        {
            get
            {
                //  No posts are loaded, so there is only one(the returned 1) empty page
                if (Products == null)
                {
                    return 1;
                }

                return Math.Max(1, (int)Math.Ceiling(Products.Count() / (double)PageSize));
            }
        }

        // The four posts to render for the current page.
        public IEnumerable<ProductModel> PagedProducts
        {
            get
            {
                // No posts are loaded, so there is nothing to show
                if (Products == null)
                {
                    return Enumerable.Empty<ProductModel>();
                }
                
                // Skip the posts of the previous pages, then take one page of posts
                return Products.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
            }
        }

        /// <summary>
        /// Loads all products when the listing page receives a GET request.
        /// </summary>
        public void OnGet(int pageNumber = 1)
        {
            // Keep the complete collection available, then calculate the requested page.
            Products = ProductService.GetAllData().ToList();
            CurrentPage = Math.Max(1, Math.Min(pageNumber, TotalPages));
        }
    }
}
