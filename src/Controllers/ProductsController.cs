using System.Collections.Generic;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContosoCrafts.WebSite.Controllers;

/// <summary>
/// API controller for game posts. It lets users read all the game posts and add a rating to a game post.
/// </summary>
// Enables automatic API behavior
[ApiController]
// Attribute routing for REST APIs. See https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-10.0
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    /// <summary>
    /// Creates the controller with the service that reads and saves the game posts.
    /// </summary>
    /// <param name="productService">Service that gives access to the game post data</param>
    public ProductsController(JsonFileProductService productService)
    {
        ProductService = productService;
    }

    // Service used to read and update the game post data (read-only after construction)
    public JsonFileProductService ProductService { get; }

    [HttpGet]
    public IEnumerable<ProductModel> Get()
    {
        return ProductService.GetAllData();
    }

    [HttpPatch]
    public ActionResult Patch([FromBody] RatingRequestModel request)
    {
        ProductService.AddRating(request.ProductId, request.Rating);

        return Ok();
    }

    public class RatingRequestModel
    {
        public string ProductId { get; set; }
        public int Rating { get; set; }
    }
}