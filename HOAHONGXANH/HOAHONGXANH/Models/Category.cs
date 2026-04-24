namespace HOAHONGXANH.Models
{
    // Model đại diện cho Danh mục sản phẩm (Ví dụ: Nam, Nữ, Trẻ em...)
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
