using HOAHONGXANH.Models;
using HOAHONGXANH.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HOAHONGXANH.Controllers
{
    // Controller quản lý toàn bộ chức năng dành cho Admin và Staff (Nhân viên)
    public class AdminController : Controller
    {
        // Khai báo các class DAO (Data Access Object) để tương tác với cơ sở dữ liệu
        private readonly EmployeeDAO _employeeDAO; // Quản lý nhân viên
        private readonly ProductDAO _productDAO;   // Quản lý sản phẩm
        private readonly CategoryDAO _categoryDAO; // Quản lý danh mục
        private readonly OrderDAO _orderDAO;       // Quản lý đơn hàng

        // Constructor nhận các DAO thông qua Dependency Injection (DI)
        public AdminController(EmployeeDAO employeeDAO, ProductDAO productDAO, CategoryDAO categoryDAO, OrderDAO orderDAO)
        {
            _employeeDAO = employeeDAO;
            _productDAO = productDAO;
            _categoryDAO = categoryDAO;
            _orderDAO = orderDAO;
        }

        // Chức năng Đăng xuất khỏi hệ thống quản trị
        public async Task<IActionResult> Logout()
        {
            // Xóa cookie xác thực
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("Login", "Account");
        }

        // Trang Dashboard (Bảng điều khiển) chính - Yêu cầu quyền Admin hoặc Staff
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Dashboard()
        {
            // Lấy dữ liệu thống kê
            var orders = _orderDAO.GetAllOrders();
            var products = _productDAO.GetAll();
            var customers = 3; // Số lượng khách hàng (Giả định, cần thêm CustomerDAO.Count sau này)
            
            // Đẩy dữ liệu ra View để hiển thị
            ViewBag.TotalOrders = orders.Count;
            ViewBag.TotalProducts = products.Count;
            ViewBag.TotalRevenue = orders.Sum(o => o.TotalAmount); // Tổng doanh thu
            ViewBag.TotalCustomers = customers;

            return View();
        }

        // --- QUẢN LÝ ĐƠN HÀNG ---

        // Hiển thị danh sách tất cả đơn hàng
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult OrderManagement()
        {
            var orders = _orderDAO.GetAllOrders();
            return View(orders);
        }

        // Xem chi tiết một đơn hàng cụ thể
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult OrderDetail(int id)
        {
            var details = _orderDAO.GetOrderDetails(id);
            ViewBag.OrderId = id;
            return View(details);
        }

        // Duyệt đơn hàng (Chuyển trạng thái thành 1: Approved)
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult ApproveOrder(int id)
        {
            _orderDAO.UpdateStatus(id, 1);
            return RedirectToAction("OrderManagement");
        }

        // Từ chối đơn hàng (Chuyển trạng thái thành 2: Rejected)
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult RejectOrder(int id)
        {
            _orderDAO.UpdateStatus(id, 2); 
            return RedirectToAction("OrderManagement");
        }
        
        // --- QUẢN LÝ SẢN PHẨM ---

        // Hiển thị danh sách sản phẩm
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult ProductManagement()
        {
            var products = _productDAO.GetAll();
            return View(products);
        }

        // Trang thêm mới sản phẩm (GET) - Hiển thị form
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public IActionResult CreateProduct()
        {
            // Lấy danh sách danh mục để hiển thị trong dropdown
            ViewBag.Categories = _categoryDAO.GetAll();
            return View();
        }

        // Xử lý thêm mới sản phẩm (POST) - Nhận dữ liệu từ form
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public IActionResult CreateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                 // Gán ảnh mặc định nếu người dùng không nhập link ảnh
                if(string.IsNullOrEmpty(product.Image)) product.Image = "https://via.placeholder.com/300";
                
                // Lưu vào database
                _productDAO.Insert(product);
                return RedirectToAction("ProductManagement");
            }
            // Nếu dữ liệu lỗi, hiển thị lại form với thông báo lỗi
            ViewBag.Categories = _categoryDAO.GetAll();
            return View(product);
        }

        // Trang sửa sản phẩm (GET)
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var product = _productDAO.GetById(id);
            if (product == null) return NotFound();
            
            ViewBag.Categories = _categoryDAO.GetAll();
            return View(product);
        }

        // Xử lý cập nhật sản phẩm (POST)
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
             if (ModelState.IsValid)
            {
                _productDAO.Update(product);
                return RedirectToAction("ProductManagement");
            }
            ViewBag.Categories = _categoryDAO.GetAll();
            return View(product);
        }

        // Xóa sản phẩm
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult DeleteProduct(int id)
        {
            _productDAO.Delete(id);
            return RedirectToAction("ProductManagement");
        }

        // --- QUẢN LÝ NHÂN VIÊN (Chỉ Admin mới được truy cập) ---

        [Authorize(Roles = "Admin")]
        public IActionResult EmployeeManagement()
        {
            if (!User.IsInRole("Admin")) return Forbid(); // Chặn nếu không phải Admin
            var employees = _employeeDAO.GetAll();
            return View(employees);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateEmployee()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateEmployee(Employee emp)
        {
            if (ModelState.IsValid)
            {
                // Thêm nhân viên mới
                _employeeDAO.Add(emp);
                return RedirectToAction("EmployeeManagement");
            }
            return View(emp);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditEmployee(int id)
        {
            var emp = _employeeDAO.GetById(id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditEmployee(Employee emp)
        {
             if (ModelState.IsValid)
            {
                _employeeDAO.Update(emp);
                return RedirectToAction("EmployeeManagement");
            }
            return View(emp);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteEmployee(int id)
        {
            _employeeDAO.Delete(id);
            return RedirectToAction("EmployeeManagement");
        }
    }
}
