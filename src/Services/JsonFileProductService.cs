using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using ContosoCrafts.WebSite.Models;

using Microsoft.AspNetCore.Hosting;

namespace ContosoCrafts.WebSite.Services
{
   /// <summary>
   /// Reads and writes product records stored in the site's JSON data file.
   /// </summary>
   public class JsonFileProductService
    {
        /// <summary>
        /// Creates the service with the web host environment used to locate the data file.
        /// </summary>
        /// <param name="webHostEnvironment">Web root path provider for the running application.</param>
        public JsonFileProductService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Gets the host environment used to resolve the web root directory.
        /// </summary>
        public IWebHostEnvironment WebHostEnvironment { get; }

        /// <summary>
        /// Gets the full path to the product JSON file under <c>wwwroot/data</c>.
        /// </summary>
        private string JsonFileName
        {
            get { return Path.Combine(WebHostEnvironment.WebRootPath, "data", "products.json"); }
        }

        /// <summary>
        /// Reads and deserializes all product records from the JSON file.
        /// </summary>
        /// <returns>The product records stored in the file.</returns>
        public IEnumerable<ProductModel> GetAllData()
        {
            // Open the JSON file for the duration of reading and deserializing its contents.
            using(var jsonFileReader = File.OpenText(JsonFileName))
            {
                return JsonSerializer.Deserialize<ProductModel[]>(jsonFileReader.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        // Accept JSON property names regardless of their letter casing.
                        PropertyNameCaseInsensitive = true
                    });
            }
        }

        /// <summary>
        /// Adds a rating to the selected product and saves the updated records.
        /// </summary>
        /// <param name="productId">Identifier of the product to rate.</param>
        /// <param name="rating">Rating value, which must be between 0 and 5.</param>
        /// <returns><see langword="true"/> when the rating is saved; otherwise, <see langword="false"/>.</returns>
        public bool AddRating(string productId, int rating)
        {
            // If the ProductID is invalid, return
            if (string.IsNullOrEmpty(productId))
            {
                return false;
            }

            // Load the records so the selected product can be updated in memory.
            var products = GetAllData();

            // Look up the product, if it does not exist, return
            var data = products.FirstOrDefault(x => x.Id.Equals(productId));
            if (data == null)
            {
                return false;
            }

            // Check Rating for boundaries, do not allow ratings below 0
            if (rating < 0)
            {
                return false;
            }

            // Check Rating for boundaries, do not allow ratings above 5
            if (rating > 5)
            {
                return false;
            }

            // Initialize the collection before adding its first rating.
            if(data.Ratings == null)
            {
                data.Ratings = new int[] { };
            }

            // Copy to a mutable list, append the new value, and store the updated array.
            var ratings = data.Ratings.ToList();
            ratings.Add(rating);
            data.Ratings = ratings.ToArray();

            // Save the data back to the data store
            SaveData(products);

            return true;
        }

        /// <summary>
        /// Copies editable fields from the supplied product into the stored record.
        /// </summary>
        /// <param name="data">Product values to apply to the matching stored record.</param>
        /// <returns>The updated stored product, or <see langword="null"/> when no matching ID exists.</returns>
        public ProductModel UpdateData(ProductModel data)
        {
            var products = GetAllData().ToList();
            var productData = products.FirstOrDefault(x => x.Id.Equals(data.Id));
            if (productData == null)
            {
                return null;
            }

            // Trim user-entered text before replacing the stored values.
            productData.Title = data.Title.Trim();
            productData.Maker = data.Maker.Trim();
            productData.Description = data.Description.Trim();
            productData.Post = data.Post.Trim();
            productData.Image = data.Image.Trim();
            productData.ShopLink = data.ShopLink.Trim();
            productData.RatingLink = data.RatingLink.Trim();

            SaveData(products);
            return productData;
        }

        /// <summary>
        /// Serializes the product records and replaces the contents of the JSON data file.
        /// </summary>
        /// <param name="products">Records to write to the data file.</param>
        private void SaveData(IEnumerable<ProductModel> products)
        {

            // Recreate the file so it contains the current product collection.
            using (var outputStream = File.Create(JsonFileName))
            {
                JsonSerializer.Serialize<IEnumerable<ProductModel>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    products
                );
            }
        }

        /// <summary>
        /// Creates a product with placeholder values and appends it to the data file.
        /// </summary>
        /// <returns>The newly created product, including its generated identifier.</returns>
        public ProductModel CreateData()
        {
            var data = new ProductModel()
            {
                Id = System.Guid.NewGuid().ToString(),
                Title = "Enter Title",
                Description = "Enter Description",
                Image = ""
            };
            // Add the new record to the existing collection before saving it.
            var dataSet = GetAllData();
            dataSet = dataSet.Append(data);

            SaveData(dataSet);

            return data;
        }
        /// <summary>
        /// Assigns a new identifier to a product, initializes its ratings, and saves it.
        /// </summary>
        /// <param name="product">Product values to add.</param>
        /// <returns>The saved product with its generated identifier.</returns>
        public ProductModel AddProduct(ProductModel product)
        {
            // Always generate the identifier in the service so each saved product is unique.
            product.Id = System.Guid.NewGuid().ToString();

            // Keep the ratings collection ready for pages that display or add ratings.
            if (product.Ratings == null)
            {
                product.Ratings = new int[0];
            }

            var products = GetAllData().ToList();
            products.Add(product);
            SaveData(products);

            return product;
        }

        /// <summary>
        /// Removes the product with the specified identifier from the data file.
        /// </summary>
        /// <param name="id">Identifier of the product to remove.</param>
        /// <returns><see langword="true"/> when a product is removed; otherwise, <see langword="false"/>.</returns>
        public bool DeleteData(string id)
        {
            var products = GetAllData();
            var productToDelete = products.FirstOrDefault(x => x.Id == id);

            if (productToDelete == null)
            {
                return false;
            }

            var updatedProducts = products.Where(x => x.Id != id);
            SaveData(updatedProducts);
            return true;
        }
        
    }
}
