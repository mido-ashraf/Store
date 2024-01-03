using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Store.Interfaces;
using Store.Models.Dto;
using Store.Models;

namespace Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _CategoryRepository;
        protected ApiResponse _response;

        public CategoryController(ICategoryRepository CategoryRepository, ApiResponse response)
        {
            _CategoryRepository = CategoryRepository;
            _response = response;
        }

        [HttpGet("GetAllCategories", Name = "GetAllCategories")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<ApiResponse>> GetCategories()
        {
            try
            {
                IEnumerable<Category> categoryList;

                categoryList = await _CategoryRepository.GetAll();

                _response.Result = categoryList;
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


        [HttpGet("GetCategory {id:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<ApiResponse>> GetCategory(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

            var category = await _CategoryRepository.Get(G => G.Id == id);

            if (category == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }
            _response.Result = category;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost("CreateCategory", Name = "CreateCategory")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]

        public async Task<ActionResult<ApiResponse>> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error");
                return BadRequest(_response);
            }
            if (createCategoryDto == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error No Category was given");
                return BadRequest(_response);
            }
            if (await _CategoryRepository.Get(u => u.Name.ToLower() == createCategoryDto.Name.ToLower()) != null)
            {
                //ModelState.AddModelError("CustomError", "Villa already Exists!");
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Category already exists");
                return BadRequest(_response);
            }
            Category category = new Category
            {
                Name = createCategoryDto.Name
            };

            await _CategoryRepository.Create(category);
            _response.Result = category;
            _response.StatusCode = HttpStatusCode.Created;

            return CreatedAtRoute("GetCategory", new { id = category.Id }, _response);
        }

        [HttpDelete("DeleteCategory {id:int}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> DeleteCategory(int id)
        {
            var category = await _CategoryRepository.Get(u => u.Id == id);
            if (category == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error this Category doesnt exists");
                return BadRequest(_response);
            }
            await _CategoryRepository.Delete(category);
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
        }

        [HttpPut("UpdateCategory {id:int}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            var b = await _CategoryRepository.DoesExist(V => V.Id == id);
            if (!b)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error this Category doesnt exists");
                return BadRequest(_response);
            }


            {
                if (categoryDto == null || id != categoryDto.Id)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Error this Category doesnt exists");
                    return BadRequest(_response);
                }

                Category category = new Category
                {
                    Id = categoryDto.Id,
                    Name = categoryDto.Name
                };

                await _CategoryRepository.UpdateAsync(category);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);

            }
        }

    }
}