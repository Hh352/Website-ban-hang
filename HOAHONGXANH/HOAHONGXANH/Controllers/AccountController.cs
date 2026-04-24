using HOAHONGXANH.Models;
using HOAHONGXANH.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HOAHONGXANH.Controllers
{
    // Controller quản lý tài khoản (Đăng nhập, Đăng ký, Hồ sơ)
    public class AccountController : Controller
    {
        private readonly CustomerDAO _customerDAO; // Tương tác với bảng Khách hàng
        private readonly OrderDAO _orderDAO;       // Tương tác với bảng Đơn hàng (để xem lịch sử mua)
        private readonly EmployeeDAO _employeeDAO; // Tương tác với bảng Nhân viên (để check đăng nhập Admin)

        public AccountController(CustomerDAO customerDAO, OrderDAO orderDAO, EmployeeDAO employeeDAO)
        {
            _customerDAO = customerDAO;
            _orderDAO = orderDAO;
            _employeeDAO = employeeDAO;
        }

        // Trang Đăng nhập (GET)
        [HttpGet]
        public IActionResult Login()
        {
            // Nếu đã đăng nhập rồi thì chuyển hướng
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Nếu là Admin/Staff -> Dashboard, Khách hàng -> Trang chủ
                if (User.IsInRole("Admin") || User.IsInRole("Staff")) return RedirectToAction("Dashboard", "Admin");
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // Xử lý Đăng nhập (POST)
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Kiểm tra nhập liệu
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }

            // 1. Thử đăng nhập với tư cách Quản trị viên / Nhân viên
            var employee = _employeeDAO.CheckLogin(username, password);
            if (employee != null)
            {
                var roleName = employee.Role == 0 ? "Admin" : "Staff";
                // Tạo Claims (Thông tin người dùng lưu trong Cookie)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, employee.Username),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim("FullName", employee.FullName),
                    new Claim("Id", employee.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                // Ghi Cookie xác thực
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Dashboard", "Admin");
            }

            // 2. Thử đăng nhập với tư cách Khách hàng
            var customer = _customerDAO.CheckLogin(username, password);
            if (customer != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, customer.Username),
                    new Claim(ClaimTypes.Role, "Customer"), // Role cố định là Customer
                    new Claim("FullName", customer.FullName),
                    new Claim("Id", customer.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Đăng nhập thất bại
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View();
            }
        }

        // Trang Đăng ký (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Xử lý Đăng ký (POST)
        [HttpPost]
        public IActionResult Register(Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Gọi DAO để thêm khách hàng mới
                bool result = _customerDAO.Register(customer);
                if (result)
                {
                    TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Error = "Tên đăng nhập đã tồn tại";
                }
            }
            return View(customer);
        }

        // Đăng xuất
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        
        // Trang Hồ sơ cá nhân (Yêu cầu đăng nhập là Customer)
        [Authorize(Roles = "Customer")]
        public IActionResult Profile()
        {
             var userIdClaim = User.FindFirst("Id");
             if (userIdClaim == null) return RedirectToAction("Login");
             int userId = int.Parse(userIdClaim.Value);

             // Lấy lịch sử mua hàng
             var orders = _orderDAO.GetOrdersByCustomerId(userId);
             ViewBag.Orders = orders;
             
             return View();
        }

        // Trang thông báo từ chối truy cập (khi không đủ quyền)
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
