using System.ComponentModel.DataAnnotations;

namespace HOAHONGXANH.Models
{
    // Model đại diện cho Sản phẩm
    public class Product
    {
        public int Id { get; set; } // Mã sản phẩm

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        public string Name { get; set; } = string.Empty; // Tên sản phẩm

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; } // Giá bán
        
        public string? Image { get; set; } // Đường dẫn ảnh
        public string? Color { get; set; } // Màu sắc
        public string? Size { get; set; }  // Kích thước

        public string? Description { get; set; } // Mô tả chi tiết

        public int CategoryId { get; set; } // Mã danh mục (Khóa ngoại)
        public int Stock { get; set; } = 0; // Số lượng tồn kho

        // Thuộc tính bổ sung (không có trong bảng Products, lấy từ bảng Categories khi join)
        public string? CategoryName { get; set; } 
    }
}
