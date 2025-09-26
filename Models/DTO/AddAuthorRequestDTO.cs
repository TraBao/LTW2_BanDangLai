using System.ComponentModel.DataAnnotations;

namespace WebAPI_simple.Models.DTO
{
    public class AddAuthorRequestDTO
    {
        [Required(ErrorMessage = "Tên tác giả không được để trống")]
        [MinLength(3, ErrorMessage = "Tên tác giả phải có ít nhất 3 ký tự")]
        public string FullName { get; set; }
    }
}