namespace HOAHONGXANH.Models.ViewModels
{
    // ViewModel này dùng để chứa tất cả dữ liệu cần thiết cho Trang chủ
    public class HomeViewModel
    {
        // Chứa danh sách các nhóm sản phẩm theo từng danh mục (Ví dụ: Nhóm Nam, Nhóm Nữ...)
        public List<CategoryGroup> CategoryGroups { get; set; } = new List<CategoryGroup>();
    }

    public class CategoryGroup
    {
        public Category Category { get; set; } // Thông tin danh mục
        public List<Product> Products { get; set; } // 4 sản phẩm mới nhất của danh mục đó
    }
}
