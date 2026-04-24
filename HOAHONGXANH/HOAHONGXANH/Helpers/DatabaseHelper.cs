using Microsoft.Data.SqlClient;
using System.Data;

namespace HOAHONGXANH.Helpers
{
    // Lớp hỗ trợ kết nối và thực thi câu lệnh SQL (Dùng chung cho cả dự án)
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Tạo kết nối mới (Cần đóng sau khi dùng)
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Thực thi câu lệnh SELECT (Trả về bảng dữ liệu DataTable)
        public DataTable ExecuteQuery(string query, SqlParameter[]? parameters = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    
                    var dt = new DataTable();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt); // Đổ dữ liệu từ Server vào bảng tạm
                    }
                    return dt;
                }
            }
        }

        // Thực thi câu lệnh INSERT, UPDATE, DELETE (Không trả về dữ liệu, chỉ thực hiện)
        public int ExecuteNonQuery(string query, SqlParameter[]? parameters = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery(); // Trả về số dòng bị ảnh hưởng
                }
            }
        }

        // Thực thi câu lệnh lấy 1 giá trị duy nhất (Ví dụ: SELECT COUNT(*), SELECT ID vừa tạo)
        public object? ExecuteScalar(string query, SqlParameter[]? parameters = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}
