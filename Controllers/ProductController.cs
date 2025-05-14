using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Practice.Models;
using Practice.Models.Dtos;
using Practice.Services;

namespace Practice.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? name, int page = 1, int pageSize = 10)
        {
            if (pageSize > 100)
                return BadRequest(ApiResponse<string>.Fail("Page size limit exceeded."));

            var products = await _productService.GetProductsAsync(name, page, pageSize);
            return Ok(ApiResponse<IEnumerable<Product>>.Successful(products));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid input."));

            var result = await _productService.CreateProductAsync(request);
            return Created("", ApiResponse<Product>.Successful(result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            return deleted
                ? NoContent()
                : NotFound(ApiResponse<string>.Fail("Product not found."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return product is not null
                ? Ok(ApiResponse<Product>.Successful(product))
                : NotFound(ApiResponse<string>.Fail("Product not found"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ProductCreateRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid input"));

            var updated = await _productService.UpdateProductAsync(id, request);
            return updated is not null
                ? Ok(ApiResponse<Product>.Successful(updated))
                : NotFound(ApiResponse<string>.Fail("Product not found"));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(string id, [FromBody] Dictionary<string, object> patchData)
        {
            if (patchData == null || !patchData.Any())
                return BadRequest(ApiResponse<string>.Fail("No patch data provided"));

            var patched = await _productService.PatchProductAsync(id, patchData);
            return patched is not null
                ? Ok(ApiResponse<Product>.Successful(patched))
                : NotFound(ApiResponse<string>.Fail("Product not found"));
        }

    }
}
