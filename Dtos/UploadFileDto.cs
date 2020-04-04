using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AmazonS3AspNet.Dtos
{
    public class UploadFileDto
    {
        [Display(Name = "Description")]
        [StringLength(50, MinimumLength = 0)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
    }
}
