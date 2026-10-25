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

    /// <summary>
    /// Gets all the game posts.
    /// </summary>
    /// <returns>A collection with every game post in the data source</returns>
    // Handles HTTP GET requests sent to /Products
    [HttpGet]
    public IEnumerable<ProductModel> Get()
    {
        return ProductService.GetAllData();
    }

    /// <summary>
    /// Adds a rating to one game post.
    /// </summary>
    /// <param name="request">Request body with the id of the game post and the rating to add</param>
    /// <returns>An HTTP 200 (OK) response after the rating is saved</returns>
    // Handles HTTP PATCH requests sent to /Products (a partial update of a game post)
    [HttpPatch]
    public ActionResult Patch([FromBody] RatingRequestModel request)
    {
        ProductService.AddRating(request.ProductId, request.Rating);

        return Ok();
    }
    
    /// <summary>
    /// Data sent by the user when it rates a game post.
    /// </summary>
    public class RatingRequestModel
    {
        public string ProductId { get; set; }
        public int Rating { get; set; }
    }
}