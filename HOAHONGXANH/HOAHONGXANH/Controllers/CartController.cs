using HOAHONGXANH.Helpers;
using HOAHONGXANH.Models;
using HOAHONGXANH.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

using Microsoft.AspNetCore.Authorization;

namespace HOAHONGXANH.Controllers
{
    // Controller quản lý Giỏ hàng và Thanh toán
    public class CartController : Controller
    {
        private readonly ProductDAO _productDAO;
        private readonly OrderDAO _orderDAO;
        private readonly CustomerDAO _customerDAO;
        private const string CartSessionKey = "CartSession"; // Tên key lưu session giỏ hàng

        public CartController(ProductDAO productDAO, OrderDAO orderDAO, CustomerDAO customerDAO)
        {
            _productDAO = productDAO;
            _orderDAO = orderDAO;
            _customerDAO = customerDAO;
        }

        // Trang giỏ hàng: Hiển thị danh sách sản phẩm đã chọn
        public IActionResult Index()
        {
            var cart = GetCartFromSession();
            return View(cart);
        }

        // Thêm sản phẩm vào giỏ hàng (Form submit)
        public IActionResult AddToCart(int id, string color, string size, int quantity = 1)
        {
            var product = _productDAO.GetById(id);
            if (product == null) return NotFound();

            var cart = GetCartFromSession();
            // Kiểm tra xem sản phẩm với cùng kích thước và màu sắc đã có trong giỏ chưa
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id && c.Size == size && c.Color == color);

            //if (cartItem != null)
            //{
            //    // Nếu có rồi thì tăng số lượng
            //    cartItem.Quantity += quantity;
            //}
            //else
            //{
                // Nếu chưa thì thêm mới vào giỏ
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Image = product.Image ?? "",
                    Quantity = quantity,
                    Size = size ?? "",
                    Color = color ?? ""
                });
            //}

            SaveCartToSession(cart); // Lưu lại vào Session
            return RedirectToAction("Index"); 
        }

        // Mua ngay: Thêm vào giỏ và chuyển ngay đến trang thanh toán
        public IActionResult BuyNow(int id, string color, string size, int quantity = 1)
        {
            var product = _productDAO.GetById(id);
            if (product == null) return NotFound();

            var cart = GetCartFromSession();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id && c.Size == size && c.Color == color);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Image = product.Image ?? "",
                    Quantity = quantity,
                    Size = size ?? "",
                    Color = color ?? ""
                });
            }

            SaveCartToSession(cart);
            return RedirectToAction("Checkout"); // Chuyển hướng đến CheckOut
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(c => c.ProductId == id);
            if (item != null)
            {
                cart.Remove(item);
                SaveCartToSession(cart);
            }
            return RedirectToAction("Index");
        }
        
        // Trang Thanh toán (GET) - Yêu cầu đăng nhập
        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
             var cart = GetCartFromSession();
             if (cart.Count == 0) return RedirectToAction("Index"); // Giỏ hàng rỗng thì quay lại
             ViewBag.Total = cart.Sum(c => c.Total);
             ViewBag.Cart = cart; 
             return View();
        }

        // Xử lý Thanh toán (POST)
        [Authorize]
        [HttpPost]
        public IActionResult Checkout(string shippingAddress, string paymentMethod)
        {
             var cart = GetCartFromSession();
             if (cart.Count == 0) return RedirectToAction("Index");
             
             // Kiểm tra địa chỉ
             if(string.IsNullOrEmpty(shippingAddress))
             {
                 ViewBag.Error = "Vui lòng nhập địa chỉ giao hàng";
                 ViewBag.Total = cart.Sum(item => item.Total);
                 ViewBag.Cart = cart;
                 return View();
             }

             // Lấy ID người dùng hiện tại từ Cookie
             var userIdClaim = User.FindFirst("Id");
             if(userIdClaim == null) return RedirectToAction("Login", "Account");
             int userId = int.Parse(userIdClaim.Value);
             
             // Kiểm tra lại người dùng trong DB
             var customer = _customerDAO.GetById(userId);
             if (customer == null)
             {
                 return RedirectToAction("Logout", "Account");
             }

             // Tạo đơn hàng mới
             var order = new HOAHONGXANH.Models.Order
             {
                 CustomerId = userId,
                 TotalAmount = cart.Sum(item => item.Total),
                 ShippingAddress = shippingAddress,
                 PaymentMethod = paymentMethod ?? "COD"
             };

             // Lưu đơn hàng và chi tiết đơn hàng vào DB
             int orderId = _orderDAO.CreateOrder(order, cart);
             
             // Xóa giỏ hàng sau khi đặt thành công
             HttpContext.Session.Remove(CartSessionKey);
             
             // Nếu chọn chuyển khoản ngân hàng
             if (paymentMethod == "Banking")
             {
                 return RedirectToAction("BankingInfo", new { orderId = orderId, amount = order.TotalAmount });
             }
             
             // Chuyển đến trang thông báo thành công
             return View("OrderSuccess", orderId);
        }

        // Trang thông báo đặt hàng thành công
        [HttpGet]
        public IActionResult OrderSuccess(int orderId)
        {
            return View(orderId);
        }

        // Trang thông tin chuyển khoản
        [Authorize]
        [HttpGet]
        public IActionResult BankingInfo(int orderId, decimal amount)
        {
            ViewBag.OrderId = orderId;
            ViewBag.Amount = amount;
            return View();
        }
        
        // Cập nhật số lượng sản phẩm trong giỏ (AJAX hoặc Form)
        [HttpPost]
        public IActionResult UpdateCart(int id, int quantity)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(c => c.ProductId == id);
            if (item != null)
            {
                if(quantity > 0)
                {
                    item.Quantity = quantity;
                }
                else 
                {
                    cart.Remove(item); // Nếu số lượng <= 0 thì xóa luôn
                }
                SaveCartToSession(cart);
            }
            return RedirectToAction("Index");
        }

        // Hàm helper: Lấy giỏ hàng từ Session
        private List<CartItem> GetCartFromSession()
        {
            var session = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(session))
            {
                return new List<CartItem>();
            }
            // Deserialize từ chuỗi JSON thành List<CartItem>
            return JsonSerializer.Deserialize<List<CartItem>>(session) ?? new List<CartItem>();
        }

        // Thêm vào giỏ hàng bằng AJAX (trả về JSON, không reload trang)
        [HttpPost]
        public IActionResult AddToCartJson(int id, string color, string size, int quantity = 1)
        {
            try
            {
                var product = _productDAO.GetById(id);
                if (product == null) return Json(new { success = false, message = "Product not found" });

                var cart = GetCartFromSession();
                var cartItem = cart.FirstOrDefault(c => c.ProductId == id && c.Size == size && c.Color == color);

                //if (cartItem != null)
                //{
                //    cartItem.Quantity += quantity;
                //}
                //else
                //{
                    cart.Add(new CartItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Price = product.Price,
                        Image = product.Image ?? "",
                        Quantity = quantity,
                        Size = size ?? "",
                        Color = color ?? ""
                    });
                //}

                SaveCartToSession(cart);

                return Json(new { 
                    success = true, 
                    cartCount = cart.Count,
                    totalAmount = cart.Sum(c => c.Total),
                    item = new {
                        name = product.Name,
                        image = product.Image,
                        price = product.Price,
                        color = color,
                        size = size
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Lấy mini cart (cho popup giỏ hàng)
        [HttpGet]
        public IActionResult GetMiniCart()
        {
            var cart = GetCartFromSession();
            return PartialView("_MiniCart", cart); 
        }

        // Hàm helper: Lưu giỏ hàng vào Session
        private void SaveCartToSession(List<CartItem> cart)
        {
            var session = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, session);
        }
    }
}
