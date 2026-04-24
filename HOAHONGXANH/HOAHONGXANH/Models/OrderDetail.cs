namespace HOAHONGXANH.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; } = "";
        public string Color { get; set; } = "";
        
        // Navigation
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
    }
}
