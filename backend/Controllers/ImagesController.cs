using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        [HttpGet("{name}")]
        public IActionResult GetImage(string name)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Images", name);
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }
            var image = System.IO.File.OpenRead(path);
            return File(image, "image/jpg");
        }

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult UploadImage(IFormFile file)
        {

            try
            {

                // Checks whether or not the request contains a file and if this file is empty or not
                if (file == null || file.Length <= 0)
                {
                    return BadRequest("File is not specified");
                }

                // Checks that the file size does not exceed the allowed size
                if (!file.ContentType.Contains("image"))
                {
                    return BadRequest("Invalid file type");
                }
                if (file.Length > 0)
                {
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "Images");
                    var fullPath = Path.Combine(pathToSave, file.FileName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        var image = System.IO.File.OpenRead(fullPath);
                        if (image.Length != file.Length)
                        {
                            return BadRequest("File already exist");
                        }
                        return Ok();
                    }

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("")]
        public IActionResult GetImages()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            List<FileStreamResult> list = new List<FileStreamResult>();
            for (int i = 0; i < Directory.GetFiles(path).Length; i++)
            {
                var image = System.IO.File.OpenRead(Path.Combine(path, Directory.GetFiles(path)[i]));
                list.Add(File(image, "image/jpg"));
            }


            return (IActionResult)list;
        }
    }
}
