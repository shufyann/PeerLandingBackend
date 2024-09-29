using System.ComponentModel.DataAnnotations;

namespace DAL.DTO.Req
{
    public class ReqUpdateUserDto
    {
        
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(30, ErrorMessage = "Name cannot exceed 30 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [MaxLength(30, ErrorMessage = "Role cannot exceed 30 characters")]
        public string Role { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Balance must be positive")]
        public decimal? Balance { get; set; }
    }
}
