using System.IO;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImageUploadController : ControllerBase
{
	private readonly IWebHostEnvironment _webHostEnvironment;

	public ImageUploadController(IWebHostEnvironment webHostEnvironment)
	{
		_webHostEnvironment = webHostEnvironment;
	}

	[HttpPost]
	public async Task<IActionResult> Post([FromBody] UploadedImage uploadedImage)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (uploadedImage.OldImagePath != string.Empty)
			{
				if (uploadedImage.OldImagePath != "uploads/placeholder.jpg")
				{
					string oldUploadedImageFileName = uploadedImage.OldImagePath.Split('/').Last();

					System.IO.File.Delete($"{_webHostEnvironment.ContentRootPath}\\wwwroot\\uploads\\{oldUploadedImageFileName}");
				}
			}

			string guid = Guid.NewGuid().ToString();
			string imageFileName = guid + uploadedImage.NewImageFileExtension;

			string fullImageFileSystemPath = $"{_webHostEnvironment.ContentRootPath}\\wwwroot\\uploads\\{imageFileName}";

			FileStream fileStream = System.IO.File.Create(fullImageFileSystemPath);
			byte[] imageContentByteArray = Convert.FromBase64String(uploadedImage.NewImageBase64Content);

			await fileStream.WriteAsync(imageContentByteArray, 0, imageContentByteArray.Length);

			fileStream.Close();

			string relativeFilePathWithoutTrailingSlashes = $"uploads/{imageFileName}";

			return Created("Create", relativeFilePathWithoutTrailingSlashes);
		}
		catch (Exception e)
		{
			return StatusCode(500, $"Something went wrong on our side. Please contact the administrator. Error Message: {e.Message}");
		}
	}
}
