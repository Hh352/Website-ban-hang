USE master
GO
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'WebsiteBanHang')
BEGIN
    CREATE DATABASE WebsiteBanHang
END
GO
USE WebsiteBanHang
GO


-- =============================================
-- DROP TABLES (Reverse Dependency Order)
-- =============================================
IF OBJECT_ID('dbo.CartDetails', 'U') IS NOT NULL DROP TABLE dbo.CartDetails;
IF OBJECT_ID('dbo.Carts', 'U') IS NOT NULL DROP TABLE dbo.Carts;
IF OBJECT_ID('dbo.OrderDetails', 'U') IS NOT NULL DROP TABLE dbo.OrderDetails;
IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID('dbo.Employees', 'U') IS NOT NULL DROP TABLE dbo.Employees;
IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL DROP TABLE dbo.Customers;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;

-- 1. Categories

CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255)
);

-- 2. Products

CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Image NVARCHAR(MAX), 
    Color NVARCHAR(50),
    Size NVARCHAR(20),
    Description NVARCHAR(MAX),
    CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
    Stock INT DEFAULT 100
);

-- 3. Customers

CREATE TABLE Customers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(100),
    Email NVARCHAR(100),
    Address NVARCHAR(200),
    Phone NVARCHAR(20)
);

-- 4. Employees

CREATE TABLE Employees (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(100),
    Role INT DEFAULT 1, -- 0: Admin, 1: Staff
    Email NVARCHAR(100)
);

-- 5. Orders

CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT FOREIGN KEY REFERENCES Customers(Id),
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2),
    Status INT DEFAULT 0,
    ShippingAddress NVARCHAR(200),
    PaymentMethod NVARCHAR(50) DEFAULT 'COD'
);

-- 6. OrderDetails
CREATE TABLE OrderDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT FOREIGN KEY REFERENCES Orders(Id),
    ProductId INT FOREIGN KEY REFERENCES Products(Id),
    Quantity INT,
    Price DECIMAL(18,2),
    Size NVARCHAR(20),
    Color NVARCHAR(50)
);

-- 7. Carts 
CREATE TABLE Carts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NULL,
    CookieIdentifier NVARCHAR(100) NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 8. CartDetails
CREATE TABLE CartDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CartId INT FOREIGN KEY REFERENCES Carts(Id),
    ProductId INT FOREIGN KEY REFERENCES Products(Id),
    Quantity INT,
    Size NVARCHAR(20),
    Color NVARCHAR(50)
);

-- SEED DATA 

-- 111: Categories (Detailed for Menu)
INSERT INTO Categories (Name, Description) VALUES 
-- NAM (1-12)
(N'Áo Polo Nam', N'Áo polo lịch lãm'), -- 1
(N'Áo Sơ Mi Nam', N'Áo sơ mi công sở, đi chơi'), -- 2
(N'Áo Thun Nam', N'Áo thun năng động'), -- 3
(N'Áo Len Nam', N'Áo len ấm áp'), -- 4
(N'Áo Hoodie Nam', N'Áo hoodie, áo nỉ cá tính'), -- 5
(N'Áo Khoác Nam', N'Áo khoác gió, da, bomber'), -- 6
(N'Quần Jeans Nam', N'Quần jeans bền đẹp'), -- 7
(N'Quần Tây Nam', N'Quần tây lịch sự'), -- 8
(N'Quần Kaki Nam', N'Quần kaki trẻ trung'), -- 9
(N'Quần Short Nam', N'Quần short thoải mái'), -- 10
(N'Đồ Thể Thao Nam', N'Trang phục tập luyện'), -- 11
(N'Đồ Lót Nam', N'Đồ mặc trong'), -- 12

-- NỮ (13-22)
(N'Áo Sơ Mi Nữ', N'Sơ mi nữ thanh lịch'), -- 13
(N'Áo Thun Nữ', N'Áo thun nữ hiện đại'), -- 14
(N'Áo Polo Nữ', N'Polo nữ năng động'), -- 15
(N'Áo Len Nữ', N'Len nữ dịu dàng'), -- 16
(N'Áo Khoác Nữ', N'Khoác nữ thời trang'), -- 17
(N'Quần Jeans Nữ', N'Jeans nữ tôn dáng'), -- 18
(N'Quần Âu Nữ', N'Quần âu công sở'), -- 19
(N'Quần Kaki Nữ', N'Kaki nữ tiện dụng'), -- 20
(N'Chân Váy', N'Chân váy các loại'), -- 21
(N'Đầm', N'Đầm dự tiệc, dạo phố'), -- 22

-- TRẺ EM (23-24)
(N'Áo Trẻ Em', N'Áo cho bé'), -- 23
(N'Quần Trẻ Em', N'Quần cho bé'); -- 24

-- 122: Products (Mapped to new Categories)
-- Áo Thun Nam (ID 3)
INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES
(N'Áo Thun Basic Trắng', 150000, N'/images/product_uniform.png', N'Trắng', N'L', N'Áo thun cotton 100% thấm hút mồ hôi', 3),
(N'Áo Thun Basic Đen', 150000, N'/images/product_uniform.png', N'Đen', N'M', N'Áo thun trơn đơn giản dễ phối đồ', 3),
(N'Áo Thun In Hình Hổ', 200000, N'/images/product_uniform.png', N'Đỏ', N'XL', N'Áo thun in hình cá tính', 3),
(N'Áo Thun Raglan', 180000, N'/images/product_uniform.png', N'Xám/Đen', N'M', N'Áo tay dài raglan', 3),
(N'Áo Thun Cổ Tim', 160000, N'/images/product_uniform.png', N'Xanh Lá', N'S', N'Thiết kế cổ tim trẻ trung', 3),
(N'Áo Thun Oversize', 220000, N'/images/product_uniform.png', N'Vàng', N'XXL', N'Form rộng thoải mái', 3),
(N'Áo Thun Sọc Ngang', 190000, N'/images/product_uniform.png', N'Trắng/Đen', N'L', N'Họa tiết sọc kinh điển', 3),
(N'Áo Thun Ba Lỗ', 120000, N'/images/product_uniform.png', N'Xám', N'L', N'Mát mẻ cho ngày hè', 3),
(N'Áo Thun Henley', 210000, N'/images/product_uniform.png', N'Nâu', N'M', N'Phong cách bụi bặm', 3);

-- Áo Polo Nam (ID 1)
INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES
(N'Áo Polo Basic Xanh', 250000, N'/images/product_uniform.png', N'Xanh Dương', N'L', N'Áo polo lịch sự công sở', 1),
(N'Áo Polo Sọc', 280000, N'/images/product_uniform.png', N'Trắng/Xanh', N'M', N'Polo sọc ngang', 1),
(N'Áo Polo Cao Cấp', 350000, N'/images/product_uniform.png', N'Đen', N'XL', N'Chất liệu cá sấu', 1);

-- Áo Sơ Mi Nam (ID 2)
INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES
(N'Sơ Mi Trắng Oxford', 350000, N'/images/product_uniform.png', N'Trắng', N'M', N'Sơ mi trắng kinh điển', 2),
(N'Sơ Mi Caro Flannel', 320000, N'/images/product_uniform.png', N'Đỏ/Đen', N'L', N'Ấm áp mùa thu', 2);

-- Áo Sơ Mi Nữ (ID 13)
INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES
(N'Sơ Mi Lụa', 350000, N'/images/product_uniform.png', N'Hồng', N'M', N'Chất liệu lụa mềm mại', 13),
(N'Sơ Mi Trắng Công Sở', 300000, N'/images/product_uniform.png', N'Trắng', N'S', N'Thanh lịch chuyên nghiệp', 13),
(N'Sơ Mi Họa Tiết Hoa', 320000, N'/images/product_uniform.png', N'Hoa Nhí', N'L', N'Nữ tính dịu dàng', 13),
(N'Sơ Mi Cổ Nơ', 340000, N'/images/product_uniform.png', N'Xanh Pastel', N'M', N'Điểm nhấn cổ nơ xinh xắn', 13),
(N'Sơ Mi Voan', 310000, N'/images/product_uniform.png', N'Kem', N'L', N'Mỏng nhẹ thoáng mát', 13),
(N'Sơ Mi Tay Phồng', 330000, N'/images/product_uniform.png', N'Trắng', N'M', N'Tay phồng che khuyết điểm', 13);

-- Quần Jeans Nam (ID 7)
INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES
(N'Jeans Skinny', 450000, N'/images/product_uniform.png', N'Xanh Đậm', N'30', N'Ôm sát tôn dáng', 7),
(N'Jeans Slim Fit', 480000, N'/images/product_uniform.png', N'Xanh Nhạt', N'32', N'Vừa vặn thoải mái', 7),
(N'Jeans Rách Gối', 500000, N'/images/product_uniform.png', N'Xám Khói', N'31', N'Phong cách đường phố', 7),
(N'Jeans Ống Đứng', 420000, N'/images/product_uniform.png', N'Đen', N'33', N'Cổ điển lịch sự', 7),
(N'Jeans Baggy', 460000, N'/images/product_uniform.png', N'Xanh', N'30', N'Rộng rãi thoải mái', 7);

-- Đầm (ID 22)
INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES
(N'Đầm Dự Tiệc Đen', 800000, N'/images/product_uniform.png', N'Đen', N'M', N'Sang trọng quý phái', 22),
(N'Váy Hoa Nhí Vintage', 450000, N'/images/product_uniform.png', N'Vàng', N'S', N'Phong cách vintage', 22),
(N'Đầm Maxi Đi Biển', 550000, N'/images/product_uniform.png', N'Xanh Biển', N'L', N'Dài thướt tha', 22),
(N'Đầm Bodycon', 400000, N'/images/product_uniform.png', N'Đỏ', N'XS', N'Tôn đường cong', 22),
(N'Đầm Yếm', 380000, N'/images/product_uniform.png', N'Demin', N'M', N'Trẻ trung tinh nghịch', 22);

-- Chân Váy (ID 21)
INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES
(N'Chân Váy Bút Chì', 300000, N'/images/product_uniform.png', N'Ghi', N'M', N'Công sở chuyên nghiệp', 21),
(N'Chân Váy Xếp Ly', 320000, N'/images/product_uniform.png', N'Hồng', N'S', N'Nữ tính nhẹ nhàng', 21);

-- Áo Khoác Nam (ID 6)
INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES
(N'Áo Khoác Da Biker', 1200000, N'/images/product_uniform.png', N'Đen', N'L', N'Da thật cao cấp', 6),
(N'Áo Khoác Bomber', 600000, N'/images/product_uniform.png', N'Xanh Rêu', N'XL', N'Bụi bặm', 6),
(N'Áo Gió Thể Thao', 400000, N'/images/product_uniform.png', N'Neon', N'M', N'Chống nước nhẹ', 6);

-- Áo Hoodie Nam (ID 5)
INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES
(N'Áo Hoodie Zip', 450000, N'/images/product_uniform.png', N'Xám', N'L', N'Thun nỉ ấm áp', 5),
(N'Áo Hoodie Basic', 400000, N'/images/product_uniform.png', N'Đen', N'XL', N'Hoodie trơn', 5);

-- Áo Len Nam (ID 4)
INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES
(N'Áo Cardigan Len', 350000, N'/images/product_uniform.png', N'Be', N'Free', N'Len mỏng nhẹ', 4);

-- Quần Short Nam (ID 10)
INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES
(N'Short Kaki Nam', 250000, N'/images/product_uniform.png', N'Kem', N'32', N'Thoải mái', 10),
(N'Short Jeans Nam Rách', 280000, N'/images/product_uniform.png', N'Xanh', N'S', N'Cá tính', 10),
(N'Short Nỉ Thể Thao', 200000, N'/images/product_uniform.png', N'Xám', N'L', N'Tập gym', 10),
(N'Short Cargo Túi Hộp', 350000, N'/images/product_uniform.png', N'Rêu', N'32', N'Tiện dụng', 10);

-- Đồ Thể Thao Nam (ID 11)
INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES
(N'Bộ Nỉ Thể Thao Nam', 600000, N'/images/product_uniform.png', N'Xám', N'XL', N'Mặc mùa đông', 11),
(N'Áo Bóng Đá', 180000, N'/images/product_uniform.png', N'Đỏ', N'L', N'Thoáng khí', 11);

-- Áo Trẻ Em (ID 23)
INSERT INTO Products (Name, Price, Image, Color, Size, Description, CategoryId) VALUES
(N'Áo Phao Trẻ Em', 469000, N'/images/product_uniform.png', N'Hồng Cam', N'2', N'Siêu nhẹ', 23),
(N'Áo Thun Trẻ Em', 150000, N'/images/product_uniform.png', N'Trắng', N'4', N'Cotton mềm', 23);

-- Customers
INSERT INTO Customers (Username, Password, FullName, Email) VALUES
('khachhang1', '123', N'Nguyễn Văn Khách', 'khach1@gmail.com'),
('khachhang2', '123', N'Trần Thị Mua', 'mua2@gmail.com'),
('hieuthuhai', '123', N'Trần Minh Hiếu', 'hieuthuhai@gmail.com');

-- Employees
INSERT INTO Employees (Username, Password, FullName, Role, Email) VALUES
('admin', 'admin', N'Quản Trị Viên', 0, 'admin@shop.com'),
('nhanvien1', '123', N'Lê Văn Nhân Viên', 1, 'nv1@shop.com');
