# Tên dự án: HOAHONGXANH (Website Bán Hàng)

Website thương mại điện tử kinh doanh các mặt hàng thời trang (Nam, Nữ, Trẻ em) mang phong cách hiện đại. Dự án được xây dựng dựa trên kiến trúc **MVC** kết hợp **DAO Pattern**, mang lại cấu trúc code rõ ràng, dễ bảo trì, hiệu năng cao và cung cấp trải nghiệm mua sắm mượt mà cho khách hàng.

## Công nghệ sử dụng (Tech Stack)
- **Backend:** C#, ASP.NET Core MVC (.NET 8.0)
- **Database:** Microsoft SQL Server (Sử dụng ADO.NET/SqlClient trực tiếp)
- **Frontend:** HTML5, CSS3, JavaScript, jQuery
- **UI Framework:** Bootstrap 5, Bootstrap Icons
- **Kiến trúc & Design Pattern:** Model-View-Controller (MVC), Data Access Object (DAO), Dependency Injection (DI)

## Tính năng nổi bật (Key Features)
- **Giỏ hàng thông minh (Session Cart & AJAX):** Hệ thống giỏ hàng được quản lý bảo mật qua `Session` với thời gian chờ (IdleTimeout) được cấu hình tùy chỉnh. Hỗ trợ thêm sản phẩm, cập nhật số lượng và hiển thị Mini Cart mượt mà thông qua cơ chế AJAX (`AddToCartJson`) mà không cần tải lại toàn bộ trang.
- **Xác thực & Phân quyền (Role-based Auth):** Sử dụng `Cookie Authentication` tích hợp sẵn của .NET Core. Phân quyền truy cập tài nguyên cực kỳ chặt chẽ dựa trên 3 vai trò (Roles): `Admin` (Toàn quyền hệ thống), `Staff` (Quản lý hàng hóa, đơn hàng) và `Customer` (Thực hiện mua hàng).
- **Quy trình thanh toán (Checkout Flow):** Tự động liên kết giỏ hàng trong Session với định danh người dùng. Khi Checkout, hệ thống sẽ thực hiện giao dịch lưu đồng thời thông tin Đơn hàng chính (`Order`) và Chi tiết đơn hàng (`OrderDetail`), đồng thời xử lý logic chuyển hướng linh hoạt theo hình thức thanh toán (COD hoặc Banking).
- **Trang quản trị tập trung (Admin Dashboard):** Bảng điều khiển mạnh mẽ tính toán và thống kê tức thời dữ liệu doanh thu, số lượng đơn hàng, sản phẩm. Cung cấp nghiệp vụ quản trị hoàn chỉnh (CRUD) cho Danh mục, Sản phẩm, Quản lý Nhân sự, và quy trình Duyệt / Từ chối đơn hàng thay đổi trạng thái (`Status`).
- **Giao diện đa cấp (Mega Menu) & Tìm kiếm:** Tích hợp thanh điều hướng dạng Mega Menu phức tạp với các danh mục lồng nhau (Nested Menu) sử dụng cấu trúc Collapse của Bootstrap. Hỗ trợ bộ lọc tìm kiếm sản phẩm theo tên (keyword) hoặc mã danh mục (categoryId).

## Kiến trúc & Cơ sở dữ liệu (Architecture & Database)
- **Kiến trúc DAO (Data Access Object):** Ứng dụng không sử dụng ORM nặng nề mà tương tác với CSDL qua các lớp DAO chuyên biệt (`ProductDAO`, `OrderDAO`, `CustomerDAO`...). Mọi truy vấn SQL, thao tác Reader được đóng gói tại đây, giúp Controller cực kỳ mỏng nhẹ (Thin Controller) và chỉ tập trung vào điều hướng luồng dữ liệu.
- **Quản lý kết nối (DatabaseHelper):** Chuỗi kết nối được khai báo tập trung trong `appsettings.json` và tiêm (Inject) dưới dạng `Singleton` vào các service, tối ưu hóa quá trình khởi tạo và tái sử dụng kết nối đến SQL Server.

## Giao diện sản phẩm (Screenshots)
<img width="1896" height="968" alt="image" src="https://github.com/user-attachments/assets/a6c406fe-0cec-4d0c-a0c5-9ed44b87521b" />


<img width="1260" height="931" alt="image" src="https://github.com/user-attachments/assets/75353a16-c23e-4f4c-9815-c9afc90aec5e" />

## Hướng dẫn cài đặt (How to Run)
1. **Clone repository:** Tải toàn bộ mã nguồn dự án về máy tính cá nhân.
2. **Khởi tạo Database:** Mở file `Documents/DBScript.sql` và chạy toàn bộ lệnh bên trong Microsoft SQL Server Management Studio (SSMS) để tự động tạo cấu trúc bảng và dữ liệu mẫu cho cơ sở dữ liệu `WebsiteBanHang`.
3. **Cấu hình chuỗi kết nối:** Mở file `HOAHONGXANH/appsettings.json`, chỉnh sửa thuộc tính `DefaultConnection` cho khớp với môi trường của bạn (Thay đổi `Server=...` thành tên Server SQL của bạn).
4. **Khởi chạy ứng dụng:** Mở file `HOAHONGXANH.sln` bằng Visual Studio 2022. Đợi IDE tự động tải các gói NuGet (Microsoft.Data.SqlClient) và nhấn **F5** để bắt đầu trải nghiệm dự án.
