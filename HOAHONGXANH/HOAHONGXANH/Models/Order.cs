namespace HOAHONGXANH.Models
{
    // Model đại diện cho Đơn hàng
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; } // Mã khách hàng đặt đơn này
        public DateTime OrderDate { get; set; } // Ngày đặt hàng
        public decimal TotalAmount { get; set; } // Tổng tiền
        
        // Trạng thái đơn: Pending (Chờ), Approved (Duyệt), Rejected (Hủy)
        public string Status { get; set; } = "Pending"; 
        
        public string ShippingAddress { get; set; } = string.Empty; // Địa chỉ giao hàng
        
        // Phương thức thanh toán: COD (Tiền mặt), Banking (Chuyển khoản)
        public string PaymentMethod { get; set; } = "COD"; 
        
        // Thuộc tính hiển thị tên khách hàng (để hiển thị trên trang Admin)
        public string CustomerName { get; set; } = string.Empty;
    }
}
