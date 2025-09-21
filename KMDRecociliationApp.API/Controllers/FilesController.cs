using KMDRecociliationApp.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class FilesController : ControllerBase
    {

        [HttpGet("download")]
        [AllowAnonymous]
        public IActionResult DownloadFile(int id,string url,string name)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest();

            var filePath = Path.Combine(url);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            FileInfo fileInfo = new FileInfo(name);

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                // stream.CopyToAsync(memory);
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(fileInfo.Extension), name);

            //if (System.IO.File.Exists(filePath))
            //{
            //    return File(System.IO.File.OpenRead(filePath), GetContentType(filePath), Path.GetFileName(filePath));
            //    //return File(System.IO.File.OpenRead(filePath), GetContentType(filePath), Path.GetFileName(filePath));
            //}
            //var memory = new MemoryStream();
            //using (var stream = new FileStream(filePath, FileMode.Open))
            //{
            //    await stream.CopyToAsync(memory);
            //}
            //memory.Position = 0;

            //return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
            //return NotFound();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }
            var path = "D:\\Code\\KMD\\KMDRecociliationApp\\KMDRecociliationApp.API\\Uploads\\ProductDocumentFiles\\1003";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), path, file.FileName);

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { filePath });
        }

        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>
            {
                { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".csv", "text/csv" }
            };

            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }
    }
}


