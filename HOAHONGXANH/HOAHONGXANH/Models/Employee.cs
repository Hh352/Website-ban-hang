using System.ComponentModel.DataAnnotations;

namespace HOAHONGXANH.Models
{
    // Model đại diện cho Nhân viên / Quản trị viên
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; } = string.Empty;

        // Role: 0 = Admin (Toàn quyền), 1 = Staff (Nhân viên - Bị giới hạn quyền)
        public int Role { get; set; } 

        [EmailAddress]
        public string? Email { get; set; }
    }
}
