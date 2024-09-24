using System.ComponentModel.DataAnnotations;

namespace DAL.DTO.Req
{
    public class ReqUpdateUserDto
    {
        [Required(ErrorMessage = "ID is required")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(30, ErrorMessage = "Name cannot exceed 30 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(50, ErrorMessage = "Email cannot exceed 50 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [MaxLength(30, ErrorMessage = "Role cannot exceed 30 characters")]
        public string Role { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Balance must be positive")]
        public decimal? Balance { get; set; }
    }
}
