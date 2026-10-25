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
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    public ProductsController(JsonFileProductService productService)
    {
        ProductService = productService;
    }

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