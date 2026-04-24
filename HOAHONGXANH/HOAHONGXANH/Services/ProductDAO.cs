using HOAHONGXANH.Helpers;
using HOAHONGXANH.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace HOAHONGXANH.Services
{
    // Lớp DAO (Data Access Object) xử lý truy vấn dữ liệu Sản phẩm
    public class ProductDAO
    {
        private readonly DatabaseHelper _dbHelper;

        public ProductDAO(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // Lấy tất cả sản phẩm
        public List<Product> GetAll()
        {
            var products = new List<Product>();
            // Query kết nối bảng Products và Categories để lấy tên danh mục
            string query = @"SELECT p.*, c.Name as CategoryName 
                             FROM Products p 
                             LEFT JOIN Categories c ON p.CategoryId = c.Id";
            
            var dt = _dbHelper.ExecuteQuery(query);

            // Chuyển đổi từng dòng dữ liệu (DataRow) thành object Product
            foreach (DataRow row in dt.Rows)
            {
                products.Add(new Product
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString() ?? "",
                    Price = Convert.ToDecimal(row["Price"]),
                    Image = row["Image"].ToString(),
                    Color = row["Color"].ToString(),
                    Size = row["Size"].ToString(),
                    Description = row["Description"].ToString(),
                    CategoryId = Convert.ToInt32(row["CategoryId"]),
                    Stock = Convert.ToInt32(row["Stock"]),
                    CategoryName = row["CategoryName"].ToString()
                });
            }

            return products;
        }

        // Lấy sản phẩm theo ID (Dùng cho trang Chi tiết hoặc khi thêm vào giỏ)
        public Product? GetById(int id)
        {
            string query = @"SELECT p.*, c.Name as CategoryName 
                             FROM Products p 
                             LEFT JOIN Categories c ON p.CategoryId = c.Id
                             WHERE p.Id = @Id";
            
            // Sử dụng tham số để tránh SQL Injection
            var param = new SqlParameter[] {
                new SqlParameter("@Id", id)
            };

            var dt = _dbHelper.ExecuteQuery(query, param);
            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            return new Product
            {
                Id = Convert.ToInt32(row["Id"]),
                Name = row["Name"].ToString() ?? "",
                Price = Convert.ToDecimal(row["Price"]),
                Image = row["Image"].ToString(),
                Color = row["Color"].ToString(),
                Size = row["Size"].ToString(),
                Description = row["Description"].ToString(),
                CategoryId = Convert.ToInt32(row["CategoryId"]),
                Stock = Convert.ToInt32(row["Stock"]),
                CategoryName = row["CategoryName"].ToString()
            };
        }

        // Lấy danh sách sản phẩm theo Danh mục
        public List<Product> GetByCategoryId(int categoryId)
        {
            var products = new List<Product>();
            string query = @"SELECT p.*, c.Name as CategoryName 
                             FROM Products p 
                             LEFT JOIN Categories c ON p.CategoryId = c.Id
                             WHERE p.CategoryId = @CategoryId";

            var param = new SqlParameter[] {
                new SqlParameter("@CategoryId", categoryId)
            };

            var dt = _dbHelper.ExecuteQuery(query, param);

            foreach (DataRow row in dt.Rows)
            {
                products.Add(new Product
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString() ?? "",
                    Price = Convert.ToDecimal(row["Price"]),
                    Image = row["Image"].ToString(),
                    Color = row["Color"].ToString(),
                    Size = row["Size"].ToString(),
                    Description = row["Description"].ToString(),
                    CategoryId = Convert.ToInt32(row["CategoryId"]),
                    Stock = Convert.ToInt32(row["Stock"]),
                    CategoryName = row["CategoryName"].ToString()
                });
            }
            return products;
        }
        
        // Tìm kiếm sản phẩm theo tên hoặc mô tả
        public List<Product> Search(string keyword)
        {
             var products = new List<Product>();
            string query = @"SELECT p.*, c.Name as CategoryName 
                             FROM Products p 
                             LEFT JOIN Categories c ON p.CategoryId = c.Id
                             WHERE p.Name LIKE @Keyword OR p.Description LIKE @Keyword";
            
             // Thêm dấu % để tìm kiếm gần đúng (LIKE)
             var param = new SqlParameter[] {
                new SqlParameter("@Keyword", "%" + keyword + "%")
            };

            var dt = _dbHelper.ExecuteQuery(query, param);

             foreach (DataRow row in dt.Rows)
            {
                products.Add(new Product
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString() ?? "",
                    Price = Convert.ToDecimal(row["Price"]),
                    Image = row["Image"].ToString(),
                    Color = row["Color"].ToString(),
                    Size = row["Size"].ToString(),
                    Description = row["Description"].ToString(),
                    CategoryId = Convert.ToInt32(row["CategoryId"]),
                    Stock = Convert.ToInt32(row["Stock"]),
                    CategoryName = row["CategoryName"].ToString()
                });
            }
            return products;
        }

        // Thêm sản phẩm mới (INSERT)
        public void Insert(Product p)
        {
            string query = @"INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId, Stock) 
                             VALUES (@Name, @Price, @Image, @Color, @Size, @Description, @CategoryId, @Stock)";
             var param = new SqlParameter[] {
                new SqlParameter("@Name", p.Name),
                new SqlParameter("@Price", p.Price),
                new SqlParameter("@Image", (object?)p.Image ?? DBNull.Value), // Xử lý null
                new SqlParameter("@Color", (object?)p.Color ?? DBNull.Value),
                new SqlParameter("@Size", (object?)p.Size ?? DBNull.Value),
                new SqlParameter("@Description", (object?)p.Description ?? DBNull.Value),
                new SqlParameter("@CategoryId", p.CategoryId),
                new SqlParameter("@Stock", p.Stock)
            };
            _dbHelper.ExecuteNonQuery(query, param);
        }

        // Cập nhật sản phẩm (UPDATE)
        public void Update(Product p)
        {
             string query = @"UPDATE Products SET Name=@Name, Price=@Price, Image=@Image, Color=@Color, 
                              Size=@Size, Description=@Description, CategoryId=@CategoryId, Stock=@Stock WHERE Id=@Id";
             var param = new SqlParameter[] {
                new SqlParameter("@Name", p.Name),
                new SqlParameter("@Price", p.Price),
                new SqlParameter("@Image", (object?)p.Image ?? DBNull.Value),
                new SqlParameter("@Color", (object?)p.Color ?? DBNull.Value),
                new SqlParameter("@Size", (object?)p.Size ?? DBNull.Value),
                new SqlParameter("@Description", (object?)p.Description ?? DBNull.Value),
                new SqlParameter("@CategoryId", p.CategoryId),
                new SqlParameter("@Stock", p.Stock),
                new SqlParameter("@Id", p.Id)
            };
            _dbHelper.ExecuteNonQuery(query, param);
        }

        // Xóa sản phẩm (DELETE)
        public void Delete(int id)
        {
            string query = "DELETE FROM Products WHERE Id = @Id";
            var param = new SqlParameter[] { new SqlParameter("@Id", id) };
            _dbHelper.ExecuteNonQuery(query, param);
        }

        // Lấy danh sách sản phẩm mới nhất (Dùng cho trang chủ)
        public List<Product> GetLatest(int count)
        {
            var products = new List<Product>();
            string query = $@"SELECT TOP {count} p.*, c.Name as CategoryName 
                             FROM Products p 
                             LEFT JOIN Categories c ON p.CategoryId = c.Id
                             ORDER BY p.Id DESC"; 
            
            // Lưu ý: TOP {count} là nối chuỗi trực tiếp, nhưng count là số int nên an toàn.

            var dt = _dbHelper.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                products.Add(new Product
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString() ?? "",
                    Price = Convert.ToDecimal(row["Price"]),
                    Image = row["Image"].ToString(),
                    Color = row["Color"].ToString(),
                    Size = row["Size"].ToString(),
                    Description = row["Description"].ToString(),
                    CategoryId = Convert.ToInt32(row["CategoryId"]),
                    Stock = Convert.ToInt32(row["Stock"]),
                    CategoryName = row["CategoryName"].ToString()
                });
            }

            return products;
        }

        // Lấy sản phẩm mới nhất theo từng danh mục
        public List<Product> GetLatestByCategory(int categoryId, int count)
        {
            var products = new List<Product>();
            string query = $@"SELECT TOP {count} p.*, c.Name as CategoryName 
                             FROM Products p 
                             LEFT JOIN Categories c ON p.CategoryId = c.Id
                             WHERE p.CategoryId = @CategoryId
                             ORDER BY p.Id DESC"; 

            var param = new SqlParameter[] {
                new SqlParameter("@CategoryId", categoryId)
            };

            var dt = _dbHelper.ExecuteQuery(query, param);

            foreach (DataRow row in dt.Rows)
            {
                products.Add(new Product
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString() ?? "",
                    Price = Convert.ToDecimal(row["Price"]),
                    Image = row["Image"].ToString(),
                    Color = row["Color"].ToString(),
                    Size = row["Size"].ToString(),
                    Description = row["Description"].ToString(),
                    CategoryId = Convert.ToInt32(row["CategoryId"]),
                    Stock = Convert.ToInt32(row["Stock"]),
                    CategoryName = row["CategoryName"].ToString()
                });
            }

            return products;
        }
        
        // Helper: Lấy tên danh mục
        public string GetCategoryName(int categoryId)
        {
            string query = "SELECT Name FROM Categories WHERE Id = @Id";
            var param = new SqlParameter[] { new SqlParameter("@Id", categoryId) };
            
            var dt = _dbHelper.ExecuteQuery(query, param);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Name"].ToString() ?? "Sản phẩm";
            }
            return "Sản phẩm";
        }
    }
}
