using AutoMapper;

namespace Server.Controllers;

[Route("api/[controller]"), ApiController]
public class PostsController : ControllerBase
{
	private readonly AppDBContext _appDBContext;
	private readonly IWebHostEnvironment _webHostEnvironment;
	private readonly IMapper _mapper;

	public PostsController(AppDBContext appDBContext, IWebHostEnvironment webHostEnvironment, IMapper mapper)
	{
		_appDBContext = appDBContext;
		_webHostEnvironment = webHostEnvironment;
		_mapper = mapper;
	}

	#region CRUD Methods

	[HttpGet]
	public async Task<IActionResult> Get()
	{
		List<Post> posts = await _appDBContext.Posts
			.Include(post => post.Category)
			.ToListAsync();

		return Ok(posts);
	}

	// website.com/api/posts/id
	[HttpGet("{id}")]
	public async Task<IActionResult> Get(int id)
	{
		Post post = await GetPostByPostId(id);

		return Ok(post);
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody]PostDTO postToCreateDTO)
	{
		try
		{
			if (postToCreateDTO is null)
			{
				return BadRequest(ModelState);
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			Post postToCreate = _mapper.Map<Post>(postToCreateDTO);

			if (postToCreate.Published)
			{
                // American DateTime
                postToCreate.PublishDate = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm");
			}

			await _appDBContext.Posts.AddAsync(postToCreate);

			bool changesPersistedToDatabase = await PersistChangesToDatabase();

			if (changesPersistedToDatabase)
			{
				return Created("Create", postToCreateDTO);
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
	public async Task<IActionResult> Update(int id, [FromBody] PostDTO updatedPostDTO)
	{
		try
		{
			if (id < 1 || updatedPostDTO is null || id != updatedPostDTO.PostId)
            {
                return BadRequest(ModelState);
            }

			Post? oldPost = await _appDBContext.Posts.FindAsync(id);

			if (oldPost is null)
				return NotFound();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

			Post updatedPost = _mapper.Map<Post>(updatedPostDTO);

			if (updatedPost.Published)
			{
				if (oldPost.Published)
				{
					updatedPost.PublishDate = oldPost.PublishDate;
				}
                else
				{
                    updatedPost.PublishDate = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm");
                }
            }
			else
			{
				updatedPost.PublishDate = string.Empty;
			}

			// Detach old post from EF else it can't be updated
			_appDBContext.Entry(oldPost).State = EntityState.Detached;

            _appDBContext.Posts.Update(updatedPost);

            bool changesPersistedToDatabase = await PersistChangesToDatabase();

            if (changesPersistedToDatabase)
            {
				return Created("Created", updatedPost);
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

            bool exists = await _appDBContext.Posts.AnyAsync(post => post.PostId == id);

            if (!exists)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

			Post postToDelete = await GetPostByPostId(id);

			if (postToDelete.ThumbnailImagePath != "uploads/placeholder.jpg")
			{
				string fileName = postToDelete.ThumbnailImagePath.Split('/').Last();

				System.IO.File.Delete($"{_webHostEnvironment.ContentRootPath}\\wwwroot\\uploads\\{fileName}");
			}

			_appDBContext.Posts.Remove(postToDelete);

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
	private async Task<Post> GetPostByPostId(int postId)
	{
		Post postToGet = await _appDBContext.Posts
			.Include(post => post.Category)
			.FirstAsync(post => post.PostId == postId);

		return postToGet;
	}

	#endregion
}
