CREATE DATABASE SaleManagement2025

USE SaleManagement2025

CREATE TABLE [User]
(
	userId char(7) primary key,
	fullName nvarchar(30) not null,
	userName varchar(20) not null unique,
	hashedPassword varchar(100) not null,
	avatarUrl varchar(100),
	avatarId varchar(25),
	phone char(25) not null,
	email varchar(30) not null,
	groupId char(7),
	createdAt datetime default getdate(),
	updateAt datetime
)

EXEC sp_RENAME '[User].updateAt', 'updatedAt', 'COLUMN';


CREATE TABLE [Group]
(
	groupId char(7) primary key,
	groupName nvarchar(30) not null unique,
	createdAt datetime default getdate(),
	updatedAt datetime
)

CREATE TABLE Permission
(
	methodId char(7) not null,
	groupId char(7) not null,
	primary key (methodId, groupId),
	createdAt datetime default getdate(),
	updatedAt datetime
)

CREATE TABLE TotalDrink 
(
	totalDrinkId char(7) primary key,
	[month] int not null,
	[year] int not null,
	totalQuantity int,
	createdAt datetime default getdate(),
	updatedAt datetime
)

CREATE TABLE TotalFood 
(
	totalFoodId char(7) primary key,
	[month] int not null, 
	[year] int not null,
	totalQuantity int,
	createdAt datetime default getdate(),
	updatedAt datetime
)


CREATE TABLE TotalDrinkDetail
(
	ttDrinkDetailId char(7) primary key,
	quantity int, 
	[month] int not null,
	[year] int not null, 
	[percentage] float,
	createdAt datetime default getdate(),
	updatedAt datetime
)

CREATE TABLE TotalFoodDetail
(
	ttFoodDetailId char(7) primary key,
	quantity int, 
	[month] int not null, 
	[year] int not null,
	[percentage] float,
	createdAt datetime default getdate(),
	updatedAt datetime
)

CREATE TABLE Method
(
	methodId char(7) primary key,
	methodName nvarchar(35) not null unique,
	screenName nvarchar(35) not null unique,
	createdAt datetime default getdate(),
	updatedAt datetime
)

CREATE TABLE [Table]
(
	tableId char(7) primary key,
	tableName nvarchar(5) not null unique,
	[location] nvarchar(100) not null,
	seatCount int not null,
	tableStatus nvarchar(30) not null,
	createdAt datetime default getdate(), 
	updatedAt datetime
)

CREATE TABLE TableReservation
(
	tableReservationId char(7) primary key,
	customerId char(7) not null,
	tableId char(7) not null,
	employeeId char(7) not null,
	arrivalTime datetime not null,
	holdUntil datetime not null,
	guestCount int not null,
	specialRequest nvarchar(30),
	createdAt datetime default getdate(),
	updatedAt datetime
)

CREATE TABLE [Order]
(
	orderId char(7) primary key,
	orderStatus nvarchar(25) not null,
	serveStatus nvarchar(25) not null,
	customerId char(7) not null,
	employeeId char(7) not null,
	tableId char(7) not null,
	deliveryLocation nvarchar(40),
	createdAt datetime default getdate(), 
	updatedAt datetime
)

CREATE TABLE OrderDetail 
(
	orderId char(7),
	menuItemId char(7) not null,
	quantity int not null,
	currentPrice money not null,
	createdAt datetime default getdate(), 
	updatedAt datetime,
	primary key(orderId, menuItemId)
)

CREATE TABLE Menu
(
	menuId char(7) primary key,
	menuName nvarchar(30) not null,
	createdAt datetime default getdate(), 
	updatedAt datetime
)

CREATE TABLE MenuDetail 
(
	menuItemId char(7),
	menuId char(7) not null,
	createdAt datetime default getdate(), 
	updatedAt datetime,
	primary key (menuItemId, menuId)
)

CREATE TABLE Revenue
(
	revenueId char(7) primary key,
	[month] int not null,
	[year] int not null,
	totalAmount money,
	createdAt datetime default getdate(), 
	updatedAt datetime
)

CREATE TABLE MenuItem 
(
	menuItemId char(7) primary key,
	menuItemName nvarchar(30) not null,
	unitPrice money not null,
	imageUrl varchar(100),
	size varchar(7),
	specialInfo nvarchar(75),
	[description] nvarchar(15) not null,
	ttDrinkDetailId char(7),
	ttFoodDetailId char(7),
	createdAt datetime default getdate(), 
	updatedAt datetime
)

CREATE TABLE MenuItemDetail
(
	menuItemId char(7),
	ingredientId char(7) not null,
	quantity int not null,
	createdAt datetime default getdate(), 
	updatedAt datetime,
	primary key (menuItemId, ingredientId)
)

CREATE TABLE Employee
(
	employeeId char(7) primary key,
	userId char(7) not null,
	fullName nvarchar(30) not null,
	position varchar(20) not null,
	createdAt datetime default getdate(), 
	updatedAt datetime
)

CREATE TABLE Attendance 
(
	attendanceId char(7) primary key,
	employeeId char(7) not null,
	workDate datetime not null,
	checkIn time not null,
	checkOut time not null,
	totalHours decimal(5,2) not null,
	note nvarchar(50),
	createdAt datetime default getdate(), 
	updatedAt datetime
)

CREATE TABLE PayRoll
(
	payRollId char(7) primary key,
	employeeId char(7) not null,
	[month] int not null,
	[year] int not null,
	totalHoursInMonth decimal(5,2) not null,
	salaryPerHour money not null,
	totalSalary money not null,
	[status] nvarchar(15) not null,
	createdAt datetime default getdate(), 
	updatedAt datetime
)

CREATE TABLE Customer 
(
	customerId char(7) primary key,
	fullName nvarchar(30) not null,
	phone varchar(25) not null unique,
	email varchar(30) not null unique,
	[location] nvarchar(100),
	createdAt datetime default getdate(), 
	updatedAt datetime
)

CREATE TABLE Invoice 
(
	invoiceId char(7) primary key,
	orderId char(7) not null, 
	paymentMethod nvarchar(20) not null,
	totalAmount money not null,
	createdAt datetime default getdate(), 
	updatedAt datetime
)

CREATE TABLE Ingredient 
(
	ingredientId char(7) primary key,
	ingredientName nvarchar(30) not null,
	unit nvarchar(10) not null,
	quantity int not null,
	minQuantity int not null,  
	createdAt datetime default getdate(), 
	updatedAt datetime
)

CREATE TABLE IngredientSupplier
(
	ingredientId char(7),
	supplierId char(7) not null,
	unitPrice money not null,
	note nvarchar(20),
	createdAt datetime default getdate(), 
	updatedAt datetime,
	primary key(ingredientId, supplierId)
)

CREATE TABLE Supplier
(
  supplierId char(7) primary key,
  supplierName nvarchar(30) not null,
  phone varchar(25) not null unique,
  email  varchar(30) not null unique,
  [address] nvarchar(50), 
  createdAt datetime default getdate(),
  updatedAt datetime
)

CREATE TABLE ImportOrder
(
  importOrderId char(7) primary key,
  importDate datetime not null,
  employeeId char(7) not null,
  totalAmount money not null,
  createdAt datetime default getdate(),
  updatedAt datetime
)

CREATE TABLE ImportOrderDetail
(
  importOrderId char(7) not null,
  ingredientId char(7) not null,
  quantity int not null,
  supplierId char(7) not null,
  createdAt datetime default getdate(),
  updatedAt datetime,
  primary key(importOrderId, ingredientId, supplierId)
)

CREATE TABLE ExportOrder
(
  exportOrderId char(7) primary key,
  exportDate datetime not null,
  employeeId char(7) not null,
  createdAt datetime default getdate(),
  updatedAt datetime
)

CREATE TABLE ExportOrderDetail
(
  exportOrderId char(7) not null,
  ingredientId char(7) not null,
  quantity int not null,
  note nvarchar(50),
  createdAt datetime default getdate(),
  updatedAt datetime,
  primary key (exportOrderId, ingredientId)
)

CREATE TABLE Feedback 
(
  feedbackId char(7) primary key,
  customerId char(7) not null,
  orderId char(7) not null,
  comment nvarchar(400) ,
  rating int, 
  createdAt datetime default getdate(),
  updatedAt datetime
)

ALTER TABLE TotalDrink ADD CHECK([month] between 1 and 12)
ALTER TABLE TotalDrink ADD CHECK([year] > 0)
ALTER TABLE TotalDrink ADD CHECK(totalQuantity >= 0)
ALTER TABLE TotalFood ADD CHECK([month] between 1 and 12)
ALTER TABLE TotalFood ADD CHECK([year] > 0)
ALTER TABLE TotalFood ADD CHECK(totalQuantity >= 0)
ALTER TABLE TotalDrinkDetail ADD CHECK([month] between 1 and 12)
ALTER TABLE TotalDrinkDetail ADD CHECK([year] > 0)
ALTER TABLE TotalDrinkDetail ADD CHECK(quantity >= 0)
ALTER TABLE TotalDrinkDetail ADD CHECK([percentage] between 0 and 100)
ALTER TABLE TotalFoodDetail ADD CHECK([month] between 1 and 12)
ALTER TABLE TotalFoodDetail ADD CHECK([year] > 0)
ALTER TABLE TotalFoodDetail ADD CHECK(quantity >= 0)
ALTER TABLE TotalFoodDetail ADD CHECK([percentage] between 0 and 100)
ALTER TABLE [Table] ADD CHECK(seatCount > 0)
ALTER TABLE [Table] ADD CHECK(tableStatus IN ('Đã có khách', 'Còn trống', 'Đang chọn'))
ALTER TABLE TableReservation ADD CHECK(guestCount >= 0)
ALTER TABLE [Order] ADD CHECK(serveStatus IN ('Chờ đặt bàn', 'Đang chế biến', 'Sẵn sàng', 'Đã phục vụ'))
ALTER TABLE [Order] ADD CHECK(orderStatus IN ('Ăn tại bàn', 'Mang đi', 'Đã hủy'))
ALTER TABLE OrderDetail ADD CHECK(quantity >= 0)
ALTER TABLE OrderDetail ADD CHECK(currentPrice > 0)
ALTER TABLE Revenue ADD CHECK([month] between 1 and 12)
ALTER TABLE Revenue ADD CHECK([year] > 0)
ALTER TABLE Revenue ADD CHECK(totalAmount >= 0)
ALTER TABLE MenuItem ADD CHECK(unitPrice >= 0)
ALTER TABLE MenuItem ADD CHECK([description] IN ('Đồ ăn', 'Nước uống'))
ALTER TABLE MenuItemDetail ADD CHECK(quantity >= 0)
ALTER TABLE Attendance ADD CHECK(totalHours >= 0) 
ALTER TABLE PayRoll ADD CHECK([month] between 1 and 12)
ALTER TABLE PayRoll ADD CHECK([year] > 0)
ALTER TABLE PayRoll ADD CHECK(totalHoursInMonth >= 0)
ALTER TABLE PayRoll ADD CHECK(salaryPerHour > 0)
ALTER TABLE PayRoll ADD CHECK(totalSalary >= 0)
ALTER TABLE PayRoll ADD CHECK([status] IN ('Đã trả lương', 'Chưa trả lương'))
ALTER TABLE Invoice ADD CHECK(paymentMethod IN ('Chuyển khoản', 'Tiền mặt'))
ALTER TABLE Invoice ADD CHECK(totalAmount >= 0)
ALTER TABLE Ingredient ADD CHECK(quantity >= 0)
ALTER TABLE Ingredient ADD CHECK(minQuantity >= 0)
ALTER TABLE IngredientSupplier ADD CHECK(unitPrice > 0)
ALTER TABLE ImportOrder ADD CHECK(totalAmount >= 0)
ALTER TABLE ImportOrderDetail ADD CHECK(quantity >= 0)
ALTER TABLE ExportOrderDetail ADD CHECK(quantity >= 0)
ALTER TABLE Feedback ADD CHECK(rating between 1 and 5)

-- Users → Group
ALTER TABLE [User]
ADD CONSTRAINT FK_User_Group
FOREIGN KEY (groupId) REFERENCES [Group](groupId);

-- Permission → Method
ALTER TABLE Permission
ADD CONSTRAINT FK_Permission_Method
FOREIGN KEY (methodId) REFERENCES Method(methodId);

-- Permission → Group
ALTER TABLE Permission
ADD CONSTRAINT FK_Permission_Group
FOREIGN KEY (groupId) REFERENCES [Group](groupId);

-- TableReservation → Customer
ALTER TABLE TableReservation
ADD CONSTRAINT FK_TableReservation_Customer
FOREIGN KEY (customerId) REFERENCES Customer(customerId);

-- TableReservation → Table
ALTER TABLE TableReservation
ADD CONSTRAINT FK_TableReservation_Table
FOREIGN KEY (tableId) REFERENCES [Table](tableId);

-- TableReservation → Employee
ALTER TABLE TableReservation
ADD CONSTRAINT FK_TableReservation_Employee
FOREIGN KEY (employeeId) REFERENCES Employee(employeeId);

-- [Order] → Customer
ALTER TABLE [Order]
ADD CONSTRAINT FK_Order_Customer
FOREIGN KEY (customerId) REFERENCES Customer(customerId);

-- [Order] → Employee
ALTER TABLE [Order]
ADD CONSTRAINT FK_Order_Employee
FOREIGN KEY (employeeId) REFERENCES Employee(employeeId);

-- [Order] → Table
ALTER TABLE [Order]
ADD CONSTRAINT FK_Order_Table
FOREIGN KEY (tableId) REFERENCES [Table](tableId);

-- OrderDetail → [Order]
ALTER TABLE OrderDetail
ADD CONSTRAINT FK_OrderDetail_Order
FOREIGN KEY (orderId) REFERENCES [Order](orderId);

-- OrderDetail → MenuItem
ALTER TABLE OrderDetail
ADD CONSTRAINT FK_OrderDetail_MenuItem
FOREIGN KEY (menuItemId) REFERENCES MenuItem(menuItemId);

-- MenuDetail → Menu
ALTER TABLE MenuDetail
ADD CONSTRAINT FK_MenuDetail_Menu
FOREIGN KEY (menuId) REFERENCES Menu(menuId);

-- MenuDetail → MenuItem
ALTER TABLE MenuDetail
ADD CONSTRAINT FK_MenuDetail_MenuItem
FOREIGN KEY (menuItemId) REFERENCES MenuItem(menuItemId);

-- MenuItem → TotalDrinkDetail
ALTER TABLE MenuItem
ADD CONSTRAINT FK_MenuItem_TotalDrinkDetail
FOREIGN KEY (ttDrinkDetailId) REFERENCES TotalDrinkDetail(ttDrinkDetailId);

-- MenuItem → TotalFoodDetail
ALTER TABLE MenuItem
ADD CONSTRAINT FK_MenuItem_TotalFoodDetail
FOREIGN KEY (ttFoodDetailId) REFERENCES TotalFoodDetail(ttFoodDetailId);

-- MenuItemDetail → MenuItem
ALTER TABLE MenuItemDetail
ADD CONSTRAINT FK_MenuItemDetail_MenuItem
FOREIGN KEY (menuItemId) REFERENCES MenuItem(menuItemId);

-- MenuItemDetail → Ingredient
ALTER TABLE MenuItemDetail
ADD CONSTRAINT FK_MenuItemDetail_Ingredient
FOREIGN KEY (ingredientId) REFERENCES Ingredient(ingredientId);

-- Employee → User
ALTER TABLE Employee
ADD CONSTRAINT FK_Employee_User
FOREIGN KEY (userId) REFERENCES [User](userId);

-- Attendance → Employee
ALTER TABLE Attendance
ADD CONSTRAINT FK_Attendance_Employee
FOREIGN KEY (employeeId) REFERENCES Employee(employeeId);

-- PayRoll → Employee
ALTER TABLE PayRoll
ADD CONSTRAINT FK_PayRoll_Employee
FOREIGN KEY (employeeId) REFERENCES Employee(employeeId);

-- Invoice → [Order]
ALTER TABLE Invoice
ADD CONSTRAINT FK_Invoice_Order
FOREIGN KEY (orderId) REFERENCES [Order](orderId);

-- IngredientSupplier → Ingredient
ALTER TABLE IngredientSupplier
ADD CONSTRAINT FK_IngredientSupplier_Ingredient
FOREIGN KEY (ingredientId) REFERENCES Ingredient(ingredientId);

-- IngredientSupplier → Supplier
ALTER TABLE IngredientSupplier
ADD CONSTRAINT FK_IngredientSupplier_Supplier
FOREIGN KEY (supplierId) REFERENCES Supplier(supplierId);

-- ImportOrder → Employee
ALTER TABLE ImportOrder
ADD CONSTRAINT FK_ImportOrder_Employee
FOREIGN KEY (employeeId) REFERENCES Employee(employeeId);

-- ImportOrderDetail → ImportOrder
ALTER TABLE ImportOrderDetail
ADD CONSTRAINT FK_ImportOrderDetail_ImportOrder
FOREIGN KEY (importOrderId) REFERENCES ImportOrder(importOrderId);

-- ImportOrderDetail → Ingredient
ALTER TABLE ImportOrderDetail
ADD CONSTRAINT FK_ImportOrderDetail_Ingredient
FOREIGN KEY (ingredientId) REFERENCES Ingredient(ingredientId);

-- ImportOrderDetail → Supplier
ALTER TABLE ImportOrderDetail
ADD CONSTRAINT FK_ImportOrderDetail_Supplier
FOREIGN KEY (supplierId) REFERENCES Supplier(supplierId);

-- ExportOrder → Employee
ALTER TABLE ExportOrder
ADD CONSTRAINT FK_ExportOrder_Employee
FOREIGN KEY (employeeId) REFERENCES Employee(employeeId);

-- ExportOrderDetail → ExportOrder
ALTER TABLE ExportOrderDetail
ADD CONSTRAINT FK_ExportOrderDetail_ExportOrder
FOREIGN KEY (exportOrderId) REFERENCES ExportOrder(exportOrderId);

-- ExportOrderDetail → Ingredient
ALTER TABLE ExportOrderDetail
ADD CONSTRAINT FK_ExportOrderDetail_Ingredient
FOREIGN KEY (ingredientId) REFERENCES Ingredient(ingredientId);

-- Feedback → Customer
ALTER TABLE Feedback
ADD CONSTRAINT FK_Feedback_Customer
FOREIGN KEY (customerId) REFERENCES Customer(customerId);

-- Feedback → [Order]
ALTER TABLE Feedback
ADD CONSTRAINT FK_Feedback_Order
FOREIGN KEY (orderId) REFERENCES [Order](orderId);

ALTER DATABASE SaleManagement2025 SET AUTO_CLOSE OFF;
ALTER DATABASE SaleManagement2025 SET AUTO_SHRINK OFF;


INSERT INTO [User]
VALUES
(
 'AD00001',
 N'Hoàng Tiến Đạt',
 'admin1',
 '240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9',
 null,
 null,
 '0789221342',
 'TienDatFoundIn@gmail.com',
 null,
 getdate(),
 getdate()
);

UPDATE [User]
SET hashedPassword = 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk='
WHERE userId = 'AD00001'

select * from [User]
