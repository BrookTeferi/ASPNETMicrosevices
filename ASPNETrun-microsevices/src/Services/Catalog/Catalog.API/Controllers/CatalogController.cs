using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepositories _repositories;
        private readonly ILogger<CatalogController> _logger;
        public CatalogController(IProductRepositories repositories, ILogger<CatalogController> logger)
        {
            _logger= logger ?? throw new ArgumentNullException(nameof(logger));
            _repositories= repositories ?? throw new ArgumentNullException(nameof(repositories));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _repositories.GetProducts();
            return Ok(products);
        }



        [HttpGet("{id:length(24)}" , Name = "GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]

        public async Task<ActionResult<Product>> GetProductById(string Id)
        {
            var product = await _repositories.GetProduct(Id);
            if(product == null)
            {
                _logger.LogError($"Product with id: {Id}, NotFound");
                return NotFound();
            }
            return Ok(product);
        }

        [Route("[action]/{category}", Name ="GetProductByCatagory")]
        [HttpGet]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCatagory (string Category)
        {
            var products=await _repositories.GetProductByCategory(Category);
            return Ok(products);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _repositories.CreateProduct(product);

            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            return Ok(await _repositories.UpdateProduct(product));
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            return Ok(await _repositories.DeleteProduct(id));
        }
    }
}
