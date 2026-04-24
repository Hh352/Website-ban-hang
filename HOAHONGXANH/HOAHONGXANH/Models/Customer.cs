using System.ComponentModel.DataAnnotations;

namespace HOAHONGXANH.Models
{
    // Model đại diện cho Khách hàng
    public class Customer
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; } // Email (có thể null)
        
        public string? Address { get; set; } // Địa chỉ
        public string? Phone { get; set; }   // Số điện thoại
    }
}
