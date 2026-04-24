using HOAHONGXANH.Helpers;
using HOAHONGXANH.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HOAHONGXANH.Services
{
    // DAO xử lý dữ liệu Đơn hàng
    public class OrderDAO
    {
        private readonly DatabaseHelper _dbHelper;

        public OrderDAO(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // Tạo đơn hàng mới (Sử dụng Transaction để đảm bảo tính toàn vẹn dữ liệu)
        public int CreateOrder(Order order, List<CartItem> cartItems)
        {
            using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                // Bắt đầu 1 giao dịch (Transaction): Nếu lỗi thì rollback hết
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Thêm vào bảng Orders (Đơn hàng chính)
                        string orderQuery = @"INSERT INTO Orders (CustomerId, OrderDate, TotalAmount, Status, ShippingAddress, PaymentMethod) 
                                              OUTPUT INSERTED.Id 
                                              VALUES (@CustomerId, @OrderDate, @TotalAmount, @Status, @ShippingAddress, @PaymentMethod)";
                        
                        var orderCmd = new SqlCommand(orderQuery, conn, transaction);
                        orderCmd.Parameters.AddWithValue("@CustomerId", order.CustomerId);
                        orderCmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        orderCmd.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                        orderCmd.Parameters.AddWithValue("@Status", 0); // 0 = Chờ xử lý
                        orderCmd.Parameters.AddWithValue("@ShippingAddress", order.ShippingAddress);
                        orderCmd.Parameters.AddWithValue("@PaymentMethod", order.PaymentMethod);

                        int orderId = (int)orderCmd.ExecuteScalar(); // Lấy ID đơn hàng vừa tạo

                        // 2. Thêm vào bảng OrderDetails (Chi tiết đơn hàng - danh sách sản phẩm)
                        string detailQuery = @"INSERT INTO OrderDetails (OrderId, ProductId, Quantity, Price, Size, Color) 
                                               VALUES (@OrderId, @ProductId, @Quantity, @Price, @Size, @Color)";

                        foreach (var item in cartItems)
                        {
                            var detailCmd = new SqlCommand(detailQuery, conn, transaction);
                            detailCmd.Parameters.AddWithValue("@OrderId", orderId);
                            detailCmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                            detailCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                            detailCmd.Parameters.AddWithValue("@Price", item.Price);
                            detailCmd.Parameters.AddWithValue("@Size", item.Size ?? "");
                            detailCmd.Parameters.AddWithValue("@Color", item.Color ?? "");
                            detailCmd.ExecuteNonQuery();
                        }

                        // Hoàn tất giao dịch
                        transaction.Commit();
                        return orderId;
                    }
                    catch (Exception)
                    {
                        // Nếu có lỗi, hoàn tác mọi thay đổi
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Lấy danh sách tất cả đơn hàng (Admin)
        public List<Order> GetAllOrders()
        {
            var list = new List<Order>();
            string query = @"SELECT o.*, c.FullName as CustomerName 
                             FROM Orders o 
                             JOIN Customers c ON o.CustomerId = c.Id
                             ORDER BY o.OrderDate DESC";
            DataTable dt = _dbHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                int statusInt = Convert.ToInt32(row["Status"]);
                string statusStr = statusInt == 0 ? "Chờ xác nhận" : (statusInt == 1 ? "Đã xác nhận" : "Đã hủy");

                list.Add(new Order
                {
                    Id = Convert.ToInt32(row["Id"]),
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    OrderDate = Convert.ToDateTime(row["OrderDate"]),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    Status = statusStr,
                    ShippingAddress = row["ShippingAddress"].ToString() ?? "",
                    PaymentMethod = row.Table.Columns.Contains("PaymentMethod") ? row["PaymentMethod"].ToString() ?? "COD" : "COD",
                    CustomerName = row["CustomerName"].ToString() ?? ""
                });
            }
            return list;
        }
        
        // Lấy danh sách đơn hàng của một Khách hàng
        public List<Order> GetOrdersByCustomerId(int customerId)
        {
            var list = new List<Order>();
            string query = @"SELECT o.*, c.FullName as CustomerName 
                             FROM Orders o 
                             JOIN Customers c ON o.CustomerId = c.Id
                             WHERE o.CustomerId = @CustomerId
                             ORDER BY o.OrderDate DESC";
            var p = new SqlParameter[] { new SqlParameter("@CustomerId", customerId) };
            DataTable dt = _dbHelper.ExecuteQuery(query, p);
            foreach (DataRow row in dt.Rows)
            {
                int statusInt = Convert.ToInt32(row["Status"]);
                string statusStr = statusInt == 0 ? "Chờ xác nhận" : (statusInt == 1 ? "Đã xác nhận" : "Đã hủy");

                list.Add(new Order
                {
                    Id = Convert.ToInt32(row["Id"]),
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    OrderDate = Convert.ToDateTime(row["OrderDate"]),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    Status = statusStr,
                    ShippingAddress = row["ShippingAddress"].ToString() ?? "",
                    CustomerName = row["CustomerName"].ToString() ?? ""
                });
            }
            return list;
        }

        // Lấy chi tiết của một đơn hàng
        public List<OrderDetail> GetOrderDetails(int orderId)
        {
             var list = new List<OrderDetail>();
             string query = @"SELECT od.*, p.Name as ProductName, p.Image as ProductImage 
                              FROM OrderDetails od 
                              JOIN Products p ON od.ProductId = p.Id 
                              WHERE od.OrderId = @OrderId";
             var p = new SqlParameter[] { new SqlParameter("@OrderId", orderId) };
             DataTable dt = _dbHelper.ExecuteQuery(query, p);
              foreach (DataRow row in dt.Rows)
            {
                list.Add(new OrderDetail
                {
                    Id = Convert.ToInt32(row["Id"]),
                    OrderId = Convert.ToInt32(row["OrderId"]),
                    ProductId = Convert.ToInt32(row["ProductId"]),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    Price = Convert.ToDecimal(row["Price"]),
                    Size = row.Table.Columns.Contains("Size") ? row["Size"].ToString() ?? "" : "",
                    Color = row.Table.Columns.Contains("Color") ? row["Color"].ToString() ?? "" : "",
                    ProductName = row["ProductName"].ToString() ?? "",
                    ProductImage = row["ProductImage"].ToString() ?? ""
                });
            }
            return list;
        }

        // Cập nhật trạng thái đơn hàng (Duyệt/Hủy)
        public void UpdateStatus(int orderId, int status)
        {
            string query = "UPDATE Orders SET Status = @Status WHERE Id = @Id";
            var param = new SqlParameter[]
            {
                new SqlParameter("@Status", status),
                new SqlParameter("@Id", orderId)
            };
            _dbHelper.ExecuteNonQuery(query, param);
        }
    }
}
