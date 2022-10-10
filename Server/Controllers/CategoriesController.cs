using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers;

[Route("api/[controller]"), ApiController]
public class CategoriesController : ControllerBase
{
	private readonly AppDBContext _appDBContext;
	private readonly IWebHostEnvironment _webHostEnvironment;

	public CategoriesController(AppDBContext appDBContext, IWebHostEnvironment webHostEnvironment)
	{
		_appDBContext = appDBContext;
		_webHostEnvironment = webHostEnvironment;
	}

	#region CRUD Methods

	[HttpGet]
	public async Task<IActionResult> Get()
	{
		List<Category> categories = await _appDBContext.Categories.ToListAsync();

		return Ok(categories);
	}

	// website.com/api/categories/withposts
	[HttpGet("withposts")]
	public async Task<IActionResult> GetWithPosts()
	{
		List<Category> categories = await _appDBContext.Categories
			.Include(category => category.Posts)
			.ToListAsync();

		return Ok(categories);
	}

	// website.com/api/categories/id
	[HttpGet("{id}")]
	public async Task<IActionResult> Get(int id)
	{
		Category category = await GetCategoryByCategoryId(id, false);

		return Ok(category);
	}

    // website.com/api/categories/id
    [HttpGet("withposts/{id}")]
    public async Task<IActionResult> GetWithPosts(int id)
    {
        Category category = await GetCategoryByCategoryId(id, true);

        return Ok(category);
    }

	[HttpPost]
	public async Task<IActionResult> Create([FromBody]Category categoryToCreate)
	{
		try
		{
			if (categoryToCreate is null)
			{
				return BadRequest(ModelState);
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			await _appDBContext.Categories.AddAsync(categoryToCreate);

			bool changesPersistedToDatabase = await PersistChangesToDatabase();

			if (changesPersistedToDatabase)
			{
				return Created("Create", categoryToCreate);
			}
			else
			{
                return StatusCode(500, "Something went wrong on our side. Please contact the administrator.");
            }
		}
		catch (System.Exception e)
		{
			return StatusCode(500, $"Something went wrong on our side. Please contact the administrator. Error message: {e.Message}.");
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, [FromBody] Category updatedCategory)
	{
		try
		{
			if (id < 1 || updatedCategory is null || id != updatedCategory.CategoryId)
            {
                return BadRequest(ModelState);
            }

			bool exists = await _appDBContext.Categories.AnyAsync(category => category.CategoryId == id);

			if (!exists)
			{
				return NotFound();
			}

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _appDBContext.Categories.Update(updatedCategory);

            bool changesPersistedToDatabase = await PersistChangesToDatabase();

            if (changesPersistedToDatabase)
            {
				return NoContent();
            }
            else
            {
                return StatusCode(500, "Something went wrong on our side. Please contact the administrator.");
            }
        }
        catch (System.Exception e)
        {
            return StatusCode(500, $"Something went wrong on our side. Please contact the administrator. Error message: {e.Message}.");
        }
    }

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		try
		{
            if (id < 1)
            {
                return BadRequest(ModelState);
            }

            bool exists = await _appDBContext.Categories.AnyAsync(category => category.CategoryId == id);

            if (!exists)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

			Category categoryToDelete = await GetCategoryByCategoryId(id, false);

			if (categoryToDelete.ThumbnailImagePath != "uploads/placeholder.jpg")
			{
				string fileName = categoryToDelete.ThumbnailImagePath.Split('/').Last();

				System.IO.File.Delete($"{_webHostEnvironment.ContentRootPath}\\wwwroot\\uploads\\{fileName}");
			}

			_appDBContext.Categories.Remove(categoryToDelete);

            bool changesPersistedToDatabase = await PersistChangesToDatabase();

            if (changesPersistedToDatabase)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500, "Something went wrong on our side. Please contact the administrator.");
            }
        }
		catch (System.Exception e)
		{
            return StatusCode(500, $"Something went wrong on our side. Please contact the administrator. Error message: {e.Message}.");
        }
	}

    #endregion

    #region Utility Methods

    [NonAction]
	[ApiExplorerSettings(IgnoreApi = true)]
	private async Task<bool> PersistChangesToDatabase()
	{
		int amountofChanges = await _appDBContext.SaveChangesAsync();

		return amountofChanges > 0;
	}

	[NonAction]
	[ApiExplorerSettings(IgnoreApi = true)]
	private async Task<Category> GetCategoryByCategoryId(int categoryId, bool withPosts)
	{
		Category categoryToGet = null;

		if (withPosts)
		{
			categoryToGet = await _appDBContext.Categories
				.Include(category => category.Posts)
				.FirstAsync(category => category.CategoryId == categoryId);
		}
		else
		{
			categoryToGet = await _appDBContext.Categories
				.FirstAsync(category => category.CategoryId == categoryId);
		}

		return categoryToGet;
	}

	#endregion
}
