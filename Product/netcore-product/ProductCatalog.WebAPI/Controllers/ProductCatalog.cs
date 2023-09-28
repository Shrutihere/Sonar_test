using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.WebAPI.Models;
using ProductCatalog.WebAPI.Services;

namespace ProductCatalog.WebAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /*
        Post api/products: This endpoint allows adding a new product, asynchronously invoking _productService.AddProduct, 
        and returning an HTTP 201 Created status.
        */
        [HttpPost]
        public async Task<ActionResult> AddProduct([FromBody] Product product) {
            try {
                await _productService.AddProduct(product);
            }
            catch {
                return StatusCode(500);
            }
            return StatusCode(201);
        }
        
        /*
         GET api/products: When a client sends a GET request to this endpoint, it asynchronously retrieves a list of products using the _productService, 
        and then returns those products as a response with an HTTP status code indicating success (200 OK).
        */
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProductsList() {
            List<Product> list = new List<Product>();
            try {
                list = await _productService.GetAllProducts();
            }
            catch(Exception ex) {
                return BadRequest(null);
            }
            return Ok(list);
        }
        
        /*
        GET api/products/{id}: This endpoint that handles HTTP GET requests with a specific "id" parameter, retrieving a product by its unique identifier asynchronously. 
        If the product is found, it returns it with an HTTP 200 OK status; otherwise, it returns an HTTP 404 Not Found status.
        */
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Product>> GetProductById([FromRoute]int id) {
            var product = new Product();
            try {
                product = await _productService.GetProductById(id);
            }
            catch (Exception ex) {
                return StatusCode(404);
            }

            return Ok(product);
        }
        
        /*
        GET api/products/search: This endpoint accepts a "name" query parameter, asynchronously retrieves products with matching names 
        using _productService.GetProductsByName, and returns them in an HTTP response.
        */
        [HttpGet]
        [Route("search")]
        public async Task<ActionResult<List<Product>>> GetProductsByName([FromQuery]string name) {
            List<Product> list = new List<Product>();
            try {
                list = await _productService.GetProductsByName(name);
            }
            catch (Exception ex) {
                return BadRequest(null);
            }
            return list.Count == 0 ? StatusCode(400) : Ok(list);
        }
        
        /*
        GET api/products/total-count: This endpoint asynchronously retrieves the total count of products and returns it in an HTTP response,
        handling potential exceptions with a 500 Internal Server Error status.
        */
        [HttpGet]
        [Route("total-count")]
        public async Task<ActionResult<int>> GetTotalProductCount() {
            int totalCount = -1;
            try {
                totalCount = await _productService.GetTotalProductCount();
            }
            catch (Exception ex) {
                return StatusCode(500, null);
            }
            return Ok(totalCount);
        }
       
        /*
        PUT api/products/{id}: This endpoint handles HTTP PUT requests to update a product by its unique identifier. 
        It checks if the provided product ID matches the one in the request body and if the product exists; 
        if so, it updates the product's attributes and returns a 204 No Content status to indicate success.
        */
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> UpdateProduct([FromRoute]int id, [FromBody] Product updatedProduct) {
            try {
                Product product = await _productService.GetProductById(id);
                if (product != null) {
                    product.Name = updatedProduct.Name;
                    product.Description = updatedProduct.Description;
                    product.Price = updatedProduct.Price;
                    product.Category = updatedProduct.Category;
                    _productService.UpdateProduct(product);
                }
            }
            catch {
                return StatusCode(500);
            }
            return NoContent();
        }

    
       /*
       GET api/products/sort: This endpoint allows sorting products based on specified criteria (name, category, or price) and 
       order (ascending or descending), returning the sorted list in an HTTP response, with an option to specify the sorting order.
       */
       [HttpGet]
       [Route("sort")]
       public async Task<ActionResult<List<Product>>> GetSortedList([FromQuery]string criteria, [FromQuery]string order) {
           List<Product> list = new List<Product>();
           try {
               if(criteria == "name") {
                   list = await _productService.SortProductsByName(order);
               } else if (criteria == "category") {
                   list = await _productService.SortProductsByCategory(order);
               }
               else if (criteria == "price") {
                   list = await _productService.SortProductsByPrice(order);
               } else {
                   return BadRequest();
               }
           }
           catch (Exception ex) {
               return StatusCode(500);
           }

           return Ok(list);
       }
       
       /*
       GET api/products/category/{category}: This endpoint asynchronously retrieves products by a specified category, 
       handling potential exceptions and returning them in an HTTP response, with a 404 status if no products are found for the category.
       */
       [HttpGet]
       [Route("category/{category}")]
       public async Task<ActionResult<List<Product>>> GetProductsByCategory([FromRoute]string category) {
           List<Product> list = new List<Product>();
           try {
               list = await _productService.GetProductsByCategory(category);
           }
           catch (Exception ex) {
               return BadRequest(null);
           }
           return list.Count > 0 ? Ok(list) : StatusCode(404, null);
       }
       
       /*
       DELETE api/products/{id}: This endpoint handles HTTP DELETE requests to delete a product by its unique identifier,
       returning a 204 No Content status upon successful deletion or a 404 status if the product is not found.
       */
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteProduct([FromRoute]int id) {
            try {
                Product product = await _productService.GetProductById(id);
                if (product == null) throw new Exception();
                _productService.DeleteProduct(id);
            }
            catch (Exception ex) {
                return StatusCode(404);
            }
            return StatusCode(204);
        }

       
      /*
      DELETE api/products: This endpoint handles HTTP DELETE requests to delete all products, returning a 204 No Content status upon successful deletion.
      */
        [HttpDelete]
        public async Task<ActionResult> DeleteAllProducts() {
            try {
                _productService.DeleteAllProducts();
            }
            catch (Exception ex) {
                return StatusCode(500);
            }
            return StatusCode(204);
        }
    }
}
