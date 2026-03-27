using System.ComponentModel.DataAnnotations;

namespace RegisztracioTest.Dtos.UploadProfileImageDto
{
    public class UploadProfileImageDto
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
