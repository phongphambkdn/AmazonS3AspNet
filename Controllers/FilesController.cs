using AmazonS3AspNet.Dtos;
using AmazonS3AspNet.Repositories;
using AmazonS3AspNet.Storages;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace AmazonS3AspNet.Controllers
{
    [Route("files")]
    public class FilesController : Controller
    {
        private readonly IFileRepository _repository;
        private readonly IAmazonS3StorageManager _storageManager;

        public FilesController(
            IFileRepository repository,
            IAmazonS3StorageManager storageManager)
        {
            _repository = repository;
            _storageManager = storageManager;
        }

        [HttpGet("media")]
        public IActionResult GetMedia(Guid id)
        {
            try
            {
                var fileInDb = _repository.Get(id);
                if (fileInDb == null)
                    return NotFound();

                var url = _storageManager.GetCannedSignedURL(fileInDb.FileLocation);
                if (string.IsNullOrEmpty(url))
                    return NotFound();

                return Ok(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return BadRequest();
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadFileDto model)
        {
            Guid? id = null;

            try
            {
                var fileEntry = new Models.File
                {
                    Description = model.Description,
                    Size = model.FormFile.Length,
                    UploadedTime = DateTime.Now,
                    FileName = model.FormFile.FileName,
                };

                id = _repository.Add(fileEntry);

                using (var stream = new MemoryStream())
                {
                    await model.FormFile.CopyToAsync(stream);
                    _storageManager.Create(fileEntry, stream);
                }

                _repository.Update(fileEntry);

                return Ok(fileEntry.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                if (id.HasValue)
                {
                    _repository.Delete(id.Value);
                }

                return BadRequest();
            }
        }

        [HttpPost("download")]
        [Produces("application/octet-stream", Type = typeof(FileResult))]
        public IActionResult Download(Guid id)
        {
            var fileEntry = _repository.Get(id);
            if (fileEntry == null)
                return NotFound();

            var content = _storageManager.Read(fileEntry);
            return File(content, MediaTypeNames.Application.Octet, WebUtility.HtmlEncode(fileEntry.FileName));
        }

        [HttpPost("delete")]
        public IActionResult Delete(Guid id)
        {
            var fileEntry = _repository.Get(id);
            if (fileEntry == null)
                return NotFound();

            _repository.Delete(id);
            _storageManager.Delete(fileEntry);

            return Ok();
        }
    }
}