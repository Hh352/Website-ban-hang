using HOAHONGXANH.Helpers;
using HOAHONGXANH.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HOAHONGXANH.Services
{
    // DAO xử lý dữ liệu Nhân viên (Admin/Staff)
    public class EmployeeDAO
    {
        private readonly DatabaseHelper _dbHelper;

        public EmployeeDAO(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // Kiểm tra đăng nhập Nhân viên
        public Employee? CheckLogin(string username, string password)
        {
            string query = "SELECT * FROM Employees WHERE Username = @Username AND Password = @Password";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", password)
            };

            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new Employee
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Username = row["Username"].ToString() ?? "",
                    // Password = row["Password"].ToString(), // Không trả về password vì lý do bảo mật
                    FullName = row["FullName"].ToString() ?? "",
                    Role = Convert.ToInt32(row["Role"]),
                    Email = row["Email"].ToString()
                };
            }
            return null;
        }
        
        // --- CÁC HÀM CRUD (Thêm/Sửa/Xóa) ---

        // Lấy tất cả nhân viên
        public List<Employee> GetAll()
        {
            var list = new List<Employee>();
            string query = "SELECT * FROM Employees";
            DataTable dt = _dbHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Employee
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Username = row["Username"].ToString() ?? "",
                    FullName = row["FullName"].ToString() ?? "",
                    Role = Convert.ToInt32(row["Role"]),
                    Email = row["Email"].ToString()
                });
            }
            return list;
        }

        // Thêm nhân viên
        public void Add(Employee emp)
        {
            string query = @"INSERT INTO Employees (Username, Password, FullName, Role, Email) 
                             VALUES (@Username, @Password, @FullName, @Role, @Email)";
            var p = new SqlParameter[] {
                new SqlParameter("@Username", emp.Username),
                new SqlParameter("@Password", emp.Password), 
                new SqlParameter("@FullName", emp.FullName),
                new SqlParameter("@Role", emp.Role),
                new SqlParameter("@Email", (object?)emp.Email ?? DBNull.Value)
            };
            _dbHelper.ExecuteNonQuery(query, p);
        }
        
        // Cập nhật nhân viên
         public void Update(Employee emp)
        {
            string query = @"UPDATE Employees SET FullName=@FullName, Role=@Role, Email=@Email WHERE Id=@Id";
             var p = new SqlParameter[] {
                new SqlParameter("@FullName", emp.FullName),
                new SqlParameter("@Role", emp.Role),
                new SqlParameter("@Email", (object?)emp.Email ?? DBNull.Value),
                new SqlParameter("@Id", emp.Id)
            };
            _dbHelper.ExecuteNonQuery(query, p);
        }
        
        // Xóa nhân viên
        public void Delete(int id)
        {
             string query = "DELETE FROM Employees WHERE Id=@Id";
             var p = new SqlParameter[] { new SqlParameter("@Id", id) };
             _dbHelper.ExecuteNonQuery(query, p);
        }
        
        public Employee? GetById(int id)
        {
             string query = "SELECT * FROM Employees WHERE Id = @Id";
             var p = new SqlParameter[] { new SqlParameter("@Id", id) };
             DataTable dt = _dbHelper.ExecuteQuery(query, p);
             if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new Employee
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Username = row["Username"].ToString() ?? "",
                    FullName = row["FullName"].ToString() ?? "",
                    Role = Convert.ToInt32(row["Role"]),
                    Email = row["Email"].ToString(),
                    Password = row["Password"].ToString() ?? "" 
                };
            }
            return null;
        }
    }
}
