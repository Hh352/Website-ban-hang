using HOAHONGXANH.Models;
using Microsoft.AspNetCore.Mvc;

namespace HOAHONGXANH.Controllers
{
    // Controller quản lý việc hiển thị sản phẩm cho khách hàng
    public class ProductsController : Controller
    {
        private readonly Services.ProductDAO _productDAO;

        public ProductsController(Services.ProductDAO productDAO)
        {
            _productDAO = productDAO;
        }

        // Trang danh sách sản phẩm (có thể lọc theo danh mục hoặc từ khóa tìm kiếm)
        public IActionResult Index(string? keyword, int? categoryId)
        {
            List<Product> products;

            if (categoryId.HasValue)
            {
                // Nếu có CategoryId -> Lấy sản phẩm theo danh mục
                products = _productDAO.GetByCategoryId(categoryId.Value);
                ViewBag.CategoryTitle = _productDAO.GetCategoryName(categoryId.Value).ToUpper();
            }
            else if (!string.IsNullOrEmpty(keyword))
            {
                // Nếu có Keyword -> Tìm kiếm sản phẩm
                products = _productDAO.Search(keyword);
                ViewData["Keyword"] = keyword; // Lưu lại từ khóa để hiển thị trong ô tìm kiếm
                ViewBag.CategoryTitle = $"KẾT QUẢ TÌM KIẾM: \"{keyword.ToUpper()}\"";
            }
            else
            {
                // Mặc định -> Lấy tất cả sản phẩm
                products = _productDAO.GetAll();
                ViewBag.CategoryTitle = "TẤT CẢ SẢN PHẨM";
            }

            return View(products);
        }

        // Trang chi tiết sản phẩm
        public IActionResult Details(int id)
        {
            var product = _productDAO.GetById(id);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}
