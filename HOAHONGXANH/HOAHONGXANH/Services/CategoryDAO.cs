using HOAHONGXANH.Helpers;
using HOAHONGXANH.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HOAHONGXANH.Services
{
    // DAO xử lý dữ liệu Danh mục
    public class CategoryDAO
    {
        private readonly DatabaseHelper _dbHelper;

        public CategoryDAO(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // Lấy danh sách tất cả danh mục
        public List<Category> GetAll()
        {
            var list = new List<Category>();
            string query = "SELECT * FROM Categories";
            DataTable dt = _dbHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Category
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString() ?? "",
                    Description = row["Description"].ToString()
                });
            }
            return list;
        }
    }
}
