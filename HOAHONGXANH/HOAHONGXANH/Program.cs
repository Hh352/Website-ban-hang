using HOAHONGXANH.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using HOAHONGXANH.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CẤU HÌNH DỊCH VỤ (DEPENDENCY INJECTION) ---
// Thêm các dịch vụ vào container để có thể sử dụng ở các nơi khác
builder.Services.AddControllersWithViews();

// Đăng ký các DAO (Data Access Object) để controller có thể gọi
builder.Services.AddScoped<ProductDAO>();
builder.Services.AddScoped<CustomerDAO>();
builder.Services.AddScoped<EmployeeDAO>();
builder.Services.AddScoped<CategoryDAO>();
builder.Services.AddScoped<OrderDAO>();

// Cấu hình Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton(new DatabaseHelper(connectionString)); // Dùng Singleton cho class hỗ trợ DB

// --- 2. CẤU HÌNH SESSION (GIỎ HÀNG) ---
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Giỏ hàng tồn tại 30 phút nếu không thao tác
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// --- 3. CẤU HÌNH ĐĂNG NHẬP (AUTHENTICATION) ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Chưa đăng nhập thì chuyển về đây
        options.AccessDeniedPath = "/Account/AccessDenied"; // Không đủ quyền thì chuyển về đây
    });

var app = builder.Build();

// --- 4. CẤU HÌNH PIPELINE XỬ LÝ REQUEST ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Cho phép load ảnh, css, js trong wwwroot

app.UseRouting();

// Thứ tự quan trọng: Session -> Authentication (Xác thực) -> Authorization (Phân quyền)
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run(); // Chạy ứng dụng
