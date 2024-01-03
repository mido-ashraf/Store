using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Interfaces;
using Store.Models;
using Store.Models.Dto;
using System.Linq.Expressions;

namespace Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        protected ApiResponse _response;

        public ProductController(IProductRepository productRepository, ApiResponse response, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _response = response;
            _categoryRepository = categoryRepository;
        }
        [HttpGet("GetAllProducts", Name = "GetAllProducts")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(Duration = 30)]
        public async Task<ActionResult<ApiResponse>> GetProducts()
        {
            try
            {
                IEnumerable<Product> productList;

                productList = await _productRepository.GetAll();
                _response.Result = productList;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("GetAllProductsByCategory", Name = "GetAllProductsByCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<ApiResponse>> GetProductsByCategory(string categoryName)
        {
            try
            {
                IEnumerable<Product> productList;

                Expression<Func<Product, bool>> filter = p => p.Category.Name == categoryName;

                productList = await _productRepository.GetAll(filter, includeProperties: "Category");

                _response.Result = productList;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("GetProduct {id:int}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<ApiResponse>> GetProduct(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

            var product = await _productRepository.Get(G => G.Id == id);

            if (product == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }
            _response.Result = product;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost("CreateProduct", Name = "CreateProduct")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ApiResponse>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error");
                return BadRequest(_response);
            }
            if (createProductDto == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error No Product was given");
                return BadRequest(_response);
            }
            if (await _productRepository.Get(u => u.Name.ToLower() == createProductDto.Name.ToLower()) != null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Product already exists");
                return BadRequest(_response);
            }

            var category = await _categoryRepository.Get(c=> c.Name.ToLower() == createProductDto.CategoryName.ToLower());
            if (category == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Category already exists");
                return BadRequest(_response);
            }
            Product product = new Product
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Category = category,
                CategoryId = category.Id,
            };


            await _productRepository.Create(product);
            _response.Result =product.Name;
            _response.StatusCode = HttpStatusCode.Created;

            return CreatedAtRoute("GetProduct", new { id = product.Id }, _response);
        }

        [HttpDelete("DeleteProduct {id:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> DeleteProduct(int id)
        {
            var product = await _productRepository.Get(u => u.Id == id);
            if (product == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error this Product doesnt exists");
                return BadRequest(_response);
            }
            await _productRepository.Delete(product);
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
        }

        [HttpPut("UpdateProduct {id:int}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto productDto)
        {
            var b = await _productRepository.DoesExist(V => V.Id == id);
            if (!b)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error this Product doesnt exists");
                return BadRequest(_response);
            }


            {
                if (productDto == null || id != productDto.Id)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Error this Product doesnt exists");
                    return BadRequest(_response);
                }

                var category = await _categoryRepository.Get(c => c.Name.ToLower() == productDto.CategoryName.ToLower());
                if (category == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Category already exists");
                    return BadRequest(_response);
                }

                Product product = new Product
                {
                    Id = productDto.Id,
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Category = category,
                    CategoryId = category.Id,
                };
                product.Category = category;
                product.CategoryId = category.Id;

                await _productRepository.UpdateAsync(product);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
        }
    }
}