using HOAHONGXANH.Helpers;
using HOAHONGXANH.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HOAHONGXANH.Services
{
    // DAO xử lý dữ liệu Khách hàng
    public class CustomerDAO
    {
        private readonly DatabaseHelper _dbHelper;

        public CustomerDAO(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // Kiểm tra đăng nhập Khách hàng
        public Customer? CheckLogin(string username, string password)
        {
            string query = "SELECT * FROM Customers WHERE Username = @Username AND Password = @Password";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", password) 
            };

            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                return GetFromRow(dt.Rows[0]);
            }
            return null;
        }

        public Customer? GetById(int id)
        {
            string query = "SELECT * FROM Customers WHERE Id = @Id";
            var parameters = new SqlParameter[] { new SqlParameter("@Id", id) };
            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                return GetFromRow(dt.Rows[0]);
            }
            return null;
        }

        // Helper: Chuyển DataRow thành Customer object
        private Customer GetFromRow(DataRow row)
        {
             return new Customer
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Username = row["Username"].ToString() ?? "",
                    FullName = row["FullName"].ToString() ?? "",
                    Email = row["Email"].ToString(),
                    Address = row["Address"].ToString(),
                    Phone = row["Phone"].ToString()
                };
        }

        // Đăng ký Khách hàng mới
        public bool Register(Customer customer)
        {
            // Kiểm tra xem Username đã tồn tại chưa
             string checkQuery = "SELECT COUNT(*) FROM Customers WHERE Username = @Username";
             var checkParam = new SqlParameter[] { new SqlParameter("@Username", customer.Username) };
             int count = Convert.ToInt32(_dbHelper.ExecuteScalar(checkQuery, checkParam));
             if (count > 0) return false;

            string query = @"INSERT INTO Customers (Username, Password, FullName, Email, Address, Phone) 
                             VALUES (@Username, @Password, @FullName, @Email, @Address, @Phone)";
            
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", customer.Username),
                new SqlParameter("@Password", customer.Password), // Lưu ý: Trong thực tế nên mã hóa mật khẩu
                new SqlParameter("@FullName", customer.FullName),
                new SqlParameter("@Email", (object?)customer.Email ?? DBNull.Value),
                new SqlParameter("@Address", (object?)customer.Address ?? DBNull.Value),
                new SqlParameter("@Phone", (object?)customer.Phone ?? DBNull.Value)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);
            return true;
        }
    }
}
