using System.Diagnostics;
using HOAHONGXANH.Models;
using Microsoft.AspNetCore.Mvc;

namespace HOAHONGXANH.Controllers
{
    // Controller cho Trang chủ
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Services.ProductDAO _productDAO;
        private readonly Services.CategoryDAO _categoryDAO;

        public HomeController(ILogger<HomeController> logger, Services.ProductDAO productDAO, Services.CategoryDAO categoryDAO)
        {
            _logger = logger;
            _productDAO = productDAO;
            _categoryDAO = categoryDAO;
        }

        // Action hiển thị trang chủ
        public IActionResult Index()
        {
            var viewModel = new Models.ViewModels.HomeViewModel();
            var categories = _categoryDAO.GetAll();

            // Lấy danh sách sản phẩm nổi bật cho từng danh mục để hiển thị ra trang chủ
            foreach (var category in categories)
            {
                var topProducts = _productDAO.GetLatestByCategory(category.Id, 4); // Lấy 4 sản phẩm mới nhất mỗi loại
                if (topProducts.Any())
                {
                    viewModel.CategoryGroups.Add(new Models.ViewModels.CategoryGroup
                    {
                        Category = category,
                        Products = topProducts
                    });
                }
            }

            return View(viewModel);
        }
        
        // Action hiển thị trang chi tiết (Shortcut, thường dùng Products/Details hơn)
        public IActionResult Detail(int id)
        {
            var product = _productDAO.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
