CREATE DATABASE SaleManagement20251_12

USE SaleManagement20251_12

CREATE TABLE [User]
(
	userId char(7) primary key,
	fullName nvarchar(30) null,
	userName varchar(20) not null unique,
	hashedPassword varchar(100) not null,
	avatarUrl varchar(100),
	phone char(25) not null,
	email varchar(30) not null,
	groupId char(7) not null,
  employeeId char(7),
	createdAt datetime default getdate(),
	updatedAt datetime
)
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

CREATE TABLE ImportMoneyPerMonth
(
	importMoneyPerMonthId char(7) primary key not null,
	[month] int not null,
	[year] int not null,
	totalAmount money,
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
  menuItemId char(7) not null,
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
  menuItemId char(7) not null,
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
	tableId char(7),
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
    [type] nvarchar(30) not null,
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
	fullName nvarchar(30) not null,
    dateOfBirth date not null,
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
	[address] nvarchar(100),
	createdAt datetime default getdate(), 
	updatedAt datetime
)
CREATE TABLE Invoice 
(
	invoiceId char(7) primary key,
	orderId char(7) not null, 
	paymentMethod nvarchar(20) not null,
	totalAmount money not null,
    invoiceStatus nvarchar(30) not null,
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
  supplierId char(7) not null,
  createdAt datetime default getdate(),
  updatedAt datetime
)

CREATE TABLE ImportOrderDetail
(
  importOrderId char(7) not null,
  ingredientId char(7) not null,
  unitPrice money not null,
  quantity int not null,
  createdAt datetime default getdate(),
  updatedAt datetime,
  primary key(importOrderId, ingredientId)
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
ALTER TABLE [Table] ADD CHECK(tableStatus IN (N'Đã có khách', N'Còn trống', N'Đang chọn'))
ALTER TABLE TableReservation ADD CHECK(guestCount >= 0)
ALTER TABLE [Order] ADD CHECK(serveStatus IN (N'Chờ đặt bàn', N'Đang chế biến', N'Sẵn sàng', N'Đã phục vụ'))
ALTER TABLE [Order] ADD CHECK(orderStatus IN (N'Ăn tại bàn', N'Mang đi', N'Đã hủy'))
ALTER TABLE OrderDetail ADD CHECK(quantity >= 0)
ALTER TABLE OrderDetail ADD CHECK(currentPrice > 0)
ALTER TABLE Revenue ADD CHECK([month] between 1 and 12)
ALTER TABLE Revenue ADD CHECK([year] > 0)
ALTER TABLE Revenue ADD CHECK(totalAmount >= 0)
ALTER TABLE MenuItem ADD CHECK(unitPrice >= 0)
ALTER TABLE MenuItem ADD CHECK([description] IN (N'Đồ ăn', N'Nước uống'))
ALTER TABLE ImportMoneyPerMonth ADD CHECK(totalAmount >= 0)
ALTER TABLE ImportMoneyPerMonth ADD CHECK([month] between 1 and 12)
ALTER TABLE ImportMoneyPerMonth ADD CHECK([year] > 0)
ALTER TABLE MenuItemDetail ADD CHECK(quantity >= 0)
ALTER TABLE Attendance ADD CHECK(totalHours >= 0) 
ALTER TABLE PayRoll ADD CHECK([month] between 1 and 12)
ALTER TABLE PayRoll ADD CHECK([year] > 0)
ALTER TABLE PayRoll ADD CHECK(totalHoursInMonth >= 0)
ALTER TABLE PayRoll ADD CHECK(salaryPerHour > 0)
ALTER TABLE PayRoll ADD CHECK(totalSalary >= 0)
ALTER TABLE PayRoll ADD CHECK([status] IN (N'Đã trả lương', N'Chưa trả lương'))
ALTER TABLE Invoice ADD CHECK(paymentMethod IN (N'Chuyển khoản', N'Tiền mặt'))
ALTER TABLE Invoice ADD CHECK(totalAmount >= 0)
ALTER TABLE Invoice ADD CHECK(invoiceStatus IN(N'Đã thanh toán', N'Chưa thanh toán',N'Đã hủy'))
ALTER TABLE Ingredient ADD CHECK(quantity >= 0)
ALTER TABLE Ingredient ADD CHECK(minQuantity >= 0)
ALTER TABLE IngredientSupplier ADD CHECK(unitPrice > 0)
ALTER TABLE ImportOrder ADD CHECK(totalAmount >= 0)
ALTER TABLE ImportOrderDetail ADD CHECK(unitPrice >= 0)
ALTER TABLE ImportOrderDetail ADD CHECK(quantity >= 0)
ALTER TABLE ExportOrderDetail ADD CHECK(quantity >= 0)
ALTER TABLE Feedback ADD CHECK(rating between 1 and 5)
-- Users → Group
ALTER TABLE [User]
ADD CONSTRAINT FK_User_Group
FOREIGN KEY (groupId) REFERENCES [Group](groupId);

-- User -> Employee
ALTER TABLE [User]
ADD CONSTRAINT FK_Employee_User
FOREIGN KEY (employeeId) REFERENCES Employee(employeeId)


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

-- TotalDrinkDetail → MenuItem
ALTER TABLE TotalDrinkDetail
ADD CONSTRAINT FK_TotalDrinkDetail_MenuItem
FOREIGN KEY (menuItemId) REFERENCES MenuItem(menuItemId);

-- TotalFoodDetail → MenuItem
ALTER TABLE TotalFoodDetail
ADD CONSTRAINT FK_TotalFoodDetail_MenuItem
FOREIGN KEY (menuItemId) REFERENCES MenuItem(menuItemId);

-- MenuItemDetail → MenuItem
ALTER TABLE MenuItemDetail
ADD CONSTRAINT FK_MenuItemDetail_MenuItem
FOREIGN KEY (menuItemId) REFERENCES MenuItem(menuItemId);

-- MenuItemDetail → Ingredient
ALTER TABLE MenuItemDetail
ADD CONSTRAINT FK_MenuItemDetail_Ingredient
FOREIGN KEY (ingredientId) REFERENCES Ingredient(ingredientId);

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

-- ImportOrder → Supplier
ALTER TABLE ImportOrder
ADD CONSTRAINT FK_ImportOrder_Supplier
FOREIGN KEY (supplierId) REFERENCES Supplier(supplierId);

-- ImportOrderDetail → ImportOrder
ALTER TABLE ImportOrderDetail
ADD CONSTRAINT FK_ImportOrderDetail_ImportOrder
FOREIGN KEY (importOrderId) REFERENCES ImportOrder(importOrderId);

-- ImportOrderDetail → Ingredient
ALTER TABLE ImportOrderDetail
ADD CONSTRAINT FK_ImportOrderDetail_Ingredient
FOREIGN KEY (ingredientId) REFERENCES Ingredient(ingredientId);

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

--Tiến hành chạy lại database và tạo mới tất cả nhé
--Thêm thông tin admin
INSERT INTO [Group]
VALUES ('GR00001', 'Admin', getdate(), getdate())

INSERT INTO [User] (userId, fullName, userName, hashedPassword, avatarUrl, phone, email, employeeId,
groupId, createdAt, updatedAt)
VALUES
(
 'AD00001',
 N'Hoàng Tiến Đạt',
 'admin1',
 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=',
 null,
 '0789221342',
 'TienDatFoundIn@gmail.com',
 null,
 'GR00001',
 getdate(),
 getdate()
);


--UPDATE [User]
--SET hashedPassword = 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk='
--WHERE userId = 'AD00001'



--TẠM THỜI ĐỂ ĐÂY NHƯNG MÀ GROUPID TRONG ĐÂY KO NÊN NULL PHẢI LUÔN NOT NULL, VÌ CHƯA CÓ DỮ
-- LIỆU NÊN ĐỂ TẠM, NHƯNG MÀ VẪN CẦN PHẢI KIỂM TRA TRONG PROCEDURE TRƯỚC KHI INSERT LÀ NÓ KO ĐƯỢC 
--PHÉP NULL NHÉ.


--GENERATE ID AUTOMATICALLY
CREATE PROCEDURE sp_GenerateId
    @prefix     CHAR(2),        -- 2 ký tự đầu ID
    @tableName  SYSNAME,        -- tên bảng cần tạo ID
    @idColumn   SYSNAME,        -- tên cột ID trong bảng
    @idLength   INT,            -- tổng chiều dài ID (7)
    @newId      CHAR(7) OUTPUT
AS
BEGIN
    DECLARE @sql NVARCHAR(MAX);
    DECLARE @maxId VARCHAR(20);
    DECLARE @numberPart INT;
    DECLARE @numberLength INT = @idLength - LEN(@prefix);  -- = 5

    -- Lấy MAX(ID) trong bảng theo prefix
    SET @sql = N'
        SELECT @maxId_OUT = MAX(' + QUOTENAME(@idColumn) + N')
        FROM ' + QUOTENAME(@tableName) + N'
        WHERE ' + QUOTENAME(@idColumn) + N' LIKE ''' + @prefix + '%'';';

    EXEC sp_executesql
        @sql,
        N'@maxId_OUT VARCHAR(20) OUTPUT',
        @maxId_OUT = @maxId OUTPUT;
    -- Nếu chưa có ID -> sinh cái đầu tiên
    IF @maxId IS NULL
    BEGIN
        SET @numberPart = 1;
    END
    ELSE
    BEGIN
        SET @numberPart = CAST(SUBSTRING(@maxId, LEN(@prefix)+1, @numberLength) AS INT) + 1;
    END
    -- Tạo ID cuối cùng: prefix + số dạng 00001
    SET @newId = @prefix + RIGHT(REPLICATE('0', @numberLength) + CAST(@numberPart AS VARCHAR), @numberLength);
END;
GO


--PROCEDURE INSERT DATA
--Thêm user vào trong db(đã chạy)
CREATE PROCEDURE sp_InsertUser
    @UserName       varchar(20),
    @HashedPassword varchar(100),
    @AvatarUrl      varchar(100) = NULL,
    @Phone          varchar(25)  = NULL,
    @Email          varchar(30)  = NULL,
    @GroupId        char(7),
    @EmployeeId     char(7) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @UserID char(7);

    BEGIN TRY
        BEGIN TRAN;

        -- Kiểm tra username trùng
        IF EXISTS (SELECT 1 FROM [User] WHERE userName = @UserName)
        BEGIN
            RAISERROR(N'Username đã tồn tại', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        -- Kiểm tra GroupId
        IF NOT EXISTS (SELECT 1 FROM [Group] WHERE groupId = @GroupId)
        BEGIN
            RAISERROR(N'Nhóm người dùng không tồn tại', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        -- Generate ID
        EXEC sp_GenerateId 
            @prefix    = 'US',
            @tableName = 'User',
            @idColumn  = 'userId',
            @idLength  = 7,
            @newId     = @UserID OUTPUT;

        -- Insert User
        INSERT INTO [User](
            userId, fullName, userName, hashedPassword, avatarUrl, phone, email,
            groupId, createdAt, updatedAt, employeeId
        )
        VALUES (
            @UserID, NULL, @UserName, @HashedPassword, @AvatarUrl, @Phone, @Email,
            @GroupId, GETDATE(), GETDATE(), @EmployeeId
        );

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
GO

--Thêm nhân viên vào db (chạy rồi)
CREATE PROCEDURE sp_InsertEmployee
    @FullName   NVARCHAR(30),
    @Position   VARCHAR(20),
	@DateOfBirth DATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @EmpId CHAR(7);

    -- Tạo ID cho Employee
    EXEC sp_GenerateId 
        @prefix    = 'EM',        
        @tableName = 'Employee', 
        @idColumn  = 'employeeId',
        @idLength  = 7,
        @newId     = @EmpId OUTPUT;

    INSERT INTO Employee(
        employeeId, fullName,dateOfBirth, position, createdAt, updatedAt
    )
    VALUES(
        @EmpId, @FullName, @DateOfBirth, @Position, GETDATE(), GETDATE()
    );
END;
GO

--Thêm MenuItem vào hệ thống (đã chạy)
CREATE PROCEDURE sp_InsertMenuItem
    @MenuItemName      NVARCHAR(30),
    @UnitPrice         MONEY,
    @ImageUrl          VARCHAR(100),
    @Size              VARCHAR(7),
    @SpecialInfo       NVARCHAR(75),
    @Type              NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MenuItemId CHAR(7);
    EXEC sp_GenerateId
        @prefix    = 'MI',
        @tableName = 'MenuItem',
        @idColumn  = 'menuItemId',
        @idLength  = 7,
        @newId     = @MenuItemId OUTPUT;

    DECLARE @Description NVARCHAR(15)
    IF(@Type <> N'Nước uống')
        BEGIN
           SET @Description = N'Đồ ăn'
        END
    ELSE 
        BEGIN
           SET @Description = N'Nước uống'
        END

    INSERT INTO MenuItem(
        menuItemId, menuItemName, unitPrice, imageUrl, size, specialInfo, [description], [type],
        createdAt, updatedAt
    )
    VALUES (
        @MenuItemId, @MenuItemName, @UnitPrice, @ImageUrl, @Size, @SpecialInfo, @Description, @Type,
        GETDATE(), GETDATE()
    );
END;
GO

--Thêm menu vào hệ thống(đã chạy)
CREATE PROCEDURE sp_InsertMenu
    @MenuName NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MenuId CHAR(7);
    EXEC sp_GenerateId
        @prefix    = 'MN',
        @tableName = 'Menu',
        @idColumn  = 'menuId',
        @idLength  = 7,
        @newId     = @MenuId OUTPUT;

    INSERT INTO Menu (menuId, menuName, createdAt, updatedAt)
    VALUES (
        @MenuId,
        @MenuName,
        GETDATE(),
        GETDATE()
    );
END;
GO

--Thêm món vào menu trong hệ thống (đã thêm)
CREATE PROCEDURE sp_InsertMenuDetail
    @MenuId      CHAR(7),
    @MenuItemId  CHAR(7)
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra menu có tồn tại
    IF NOT EXISTS (SELECT 1 FROM Menu WHERE menuId = @MenuId)
    BEGIN
        RAISERROR(N'MenuId không tồn tại.', 16, 1);
        RETURN;
    END

    -- Kiểm tra menuItem có tồn tại
    IF NOT EXISTS (SELECT 1 FROM MenuItem WHERE menuItemId = @MenuItemId)
    BEGIN
        RAISERROR(N'MenuItemId không tồn tại.', 16, 1);
        RETURN;
    END

    -- Kiểm tra trùng khóa chính (MenuId + MenuItemId)
    IF EXISTS (SELECT 1 FROM MenuDetail WHERE menuId = @MenuId AND menuItemId = @MenuItemId)
    BEGIN
        RAISERROR(N'Món này đã tồn tại trong Menu.', 16, 1);
        RETURN;
    END

    INSERT INTO MenuDetail(menuId, menuItemId, createdAt, updatedAt)
    VALUES (
        @MenuId, @MenuItemId,
        GETDATE(), GETDATE()
    );
END;
GO

--Thêm customer vào hệ thống(đã thêm)
CREATE PROCEDURE sp_InsertCustomer
	@FullName nvarchar(30),
	@Phone varchar(25),
	@Email varchar(30),
	@Location nvarchar(100)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CustomerId CHAR(7);
    EXEC sp_GenerateId
        @prefix    = 'CU',
        @tableName = 'Customer',
        @idColumn  = 'customerId',
        @idLength  = 7,
        @newId     = @CustomerId OUTPUT;

	INSERT INTO Customer(customerId, fullName, phone, email, [address], createdAt, updatedAt)
	VALUES (@CustomerId, @FullName, @Phone, @Email, @Location, getdate(), getdate());
END;
GO

--Thêm đơn đặt bàn vào hệ thống (đã thêm)
CREATE PROCEDURE sp_InsertTableReservation
    @ArrivalTime       DATETIME,
    @HoldUntil         DATETIME,
    @GuestCount        INT,
    @CustomerId        CHAR(7),
    @EmployeeId        CHAR(7),
    @TableId           CHAR(7),     
    @SpecialRequest  NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        -- Kiểm tra Customer
        IF NOT EXISTS (SELECT 1 FROM Customer WHERE customerId = @CustomerId)
        BEGIN
            RAISERROR(N'Khách hàng không tồn tại trong hệ thống', 16, 1);
            ROLLBACK;
            RETURN;
        END

        -- Kiểm tra Employee
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeId = @EmployeeId)
        BEGIN
            RAISERROR(N'Nhân viên không tồn tại trong hệ thống', 16, 1);
            ROLLBACK;
            RETURN;
        END

        -- Kiểm tra table
        IF NOT EXISTS (SELECT 1 FROM [Table] WHERE tableId = @TableId)
        BEGIN
            RAISERROR(N'Bàn không tồn tại trong hệ thống', 16, 1);
            ROLLBACK;
            RETURN;
        END

        -- Sinh TableReservationId
        DECLARE @TableReservationId CHAR(7);
        EXEC sp_GenerateId
            @prefix    = 'TR',
            @tableName = 'TableReservation',
            @idColumn  = 'tableReservationId',
            @idLength  = 7,
            @newId     = @TableReservationId OUTPUT;

        INSERT INTO TableReservation(
            tableReservationId, customerId, tableId, employeeId, arrivalTime, holdUntil, guestCount,
            specialRequest, createdAt, updatedAt)
        VALUES (
            @TableReservationId, @CustomerId, @TableId, @EmployeeId, @ArrivalTime, @HoldUntil, 
            @GuestCount,@SpecialRequest, GETDATE(), GETDATE()
        );

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END;    
GO

--Thêm nhà cung cấp vào hệ thống(đã thêm)
CREATE PROCEDURE sp_InsertSupplier
    @SupplierName NVARCHAR(30),
    @Phone        VARCHAR(25),
    @Email        VARCHAR(30),
    @Address      NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @SupplierId CHAR(7);
        EXEC sp_GenerateId
            @prefix='SP',
            @tableName='Supplier',
            @idColumn='supplierId',
            @idLength=7,
            @newId=@SupplierId OUTPUT;

        INSERT INTO Supplier(supplierId, supplierName, phone, email, [address], createdAt, updatedAt)
        VALUES(@SupplierId, @SupplierName, @Phone, @Email, @Address, GETDATE(), GETDATE());

        COMMIT;
        SELECT @SupplierId AS supplierId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH
END;
GO

--Thêm nguyên liệu cho quán vào hệ thống (đã thêm)
CREATE PROCEDURE sp_InsertIngredient
    @IngredientName NVARCHAR(30),
    @Unit           NVARCHAR(10),
    @Quantity       INT = 0,
    @MinQuantity    INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        DECLARE @IngredientId CHAR(7);
        EXEC sp_GenerateId
            @prefix='IG',
            @tableName='Ingredient',
            @idColumn='ingredientId',
            @idLength=7,
            @newId=@IngredientId OUTPUT;

        INSERT INTO Ingredient(ingredientId, ingredientName, unit, quantity, minQuantity, createdAt, updatedAt)
        VALUES(@IngredientId, @IngredientName, @Unit, @Quantity, @MinQuantity, GETDATE(), GETDATE());

        COMMIT;
        SELECT @IngredientId AS ingredientId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH
END;
GO

--Thêm chi tiết nguyên liệu thuộc từng nhà cung cấp(đã thêm)
CREATE PROCEDURE sp_InsertIngredientSupplier
    @IngredientId CHAR(7),
    @SupplierId   CHAR(7),
    @UnitPrice    MONEY,
    @Note         NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM Ingredient WHERE ingredientId=@IngredientId)
        BEGIN
            RAISERROR(N'Nguyên liệu không tồn tại', 16, 1);
            ROLLBACK; RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM Supplier WHERE supplierId=@SupplierId)
        BEGIN
            RAISERROR(N'Nhà cung cấp không tồn tại', 16, 1);
            ROLLBACK; RETURN;
        END

        INSERT INTO IngredientSupplier(
            ingredientId, supplierId, unitPrice, note, createdAt, updatedAt
        )
        VALUES(@IngredientId, @SupplierId, @UnitPrice, @Note, GETDATE(), GETDATE());

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT>0 ROLLBACK;
        THROW;
    END CATCH
END;
GO

--Thêm đơn nhập cho hệ thống(Đã thêm)
CREATE PROCEDURE sp_InsertImportOrder
    @EmployeeId CHAR(7),
    @SupplierId CHAR(7)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeId = @EmployeeId)
    BEGIN
        RAISERROR(N'Nhân viên không tồn tại.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM Supplier WHERE supplierId = @SupplierId)
    BEGIN
        RAISERROR(N'Nhà cung cấp không tồn tại.', 16, 1);
        RETURN;
    END

    DECLARE @ImportOrderId CHAR(7);
    EXEC sp_GenerateId
        @prefix    = 'IO',
        @tableName = 'ImportOrder',
        @idColumn  = 'importOrderId',
        @idLength  = 7,
        @newId     = @ImportOrderId OUTPUT;

    INSERT INTO ImportOrder(importOrderId, importDate, employeeId, supplierId, totalAmount, createdAt, updatedAt)
    VALUES (@ImportOrderId, GETDATE(), @EmployeeId, @SupplierId, 0, GETDATE(), GETDATE());

    SELECT @ImportOrderId AS NewImportOrderId;
END;
GO

--Thêm đơn nhập chi tiết cho hệ thống (đã thêm)
CREATE PROCEDURE sp_InsertImportOrderDetail
    @ImportOrderId CHAR(7),
    @IngredientId  CHAR(7),
    @Quantity      INT,
    @UnitPrice     MONEY
AS
BEGIN
    SET NOCOUNT ON; 
    BEGIN TRY
        BEGIN TRAN;

        -- Kiểm tra đơn nhập
        IF NOT EXISTS (SELECT 1 FROM ImportOrder WHERE importOrderId = @ImportOrderId)
        BEGIN
            RAISERROR(N'Đơn nhập không tồn tại.', 16, 1);
            ROLLBACK;
            RETURN;
        END

        -- Kiểm tra nguyên liệu
        IF NOT EXISTS (SELECT 1 FROM Ingredient WHERE ingredientId = @IngredientId)
        BEGIN
            RAISERROR(N'Nguyên liệu không tồn tại.', 16, 1);
            ROLLBACK;
            RETURN;
        END

        -- 1. Thêm chi tiết đơn nhập
        INSERT INTO ImportOrderDetail(importOrderId, ingredientId, quantity, unitPrice, createdAt, updatedAt)
        VALUES (@ImportOrderId, @IngredientId, @Quantity, @UnitPrice, GETDATE(), GETDATE());

        -- 2. Cập nhật tồn kho nguyên liệu
        UPDATE Ingredient
        SET quantity = quantity + @Quantity,
            updatedAt = GETDATE()
        WHERE ingredientId = @IngredientId;

        -- 3. Cập nhật lại tổng tiền đơn nhập
        UPDATE ImportOrder
        SET totalAmount = (
            SELECT SUM(quantity * unitPrice)
            FROM ImportOrderDetail
            WHERE importOrderId = @ImportOrderId
        ),
        updatedAt = GETDATE()
        WHERE importOrderId = @ImportOrderId;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END;
GO

--TẠO ĐƠN HÀNG TRƯỚC TIÊN (đã thêm)
CREATE PROCEDURE sp_InsertOrder
    @OrderStatus NVARCHAR(25),
    @ServeStatus nvarchar(25),
    @CustomerId char(7),
    @TableId char(7),
    @DeliveryLocation nvarchar(40),
    @EmployeeId char(7)
AS
BEGIN
    SET NOCOUNT ON
    --Ktra khách hàng
    IF NOT EXISTS(SELECT 1 FROM Customer WHERE customerId = @CustomerId)
    BEGIN
        RAISERROR(N'Khách hàng không tồn tại', 16, 1)
        RETURN
    END
    
    --Ktra nhân viên
    IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeId = @EmployeeId)
    BEGIN 
        RAISERROR(N'Nhân viên không tồn tại',16, 1)
        RETURN
    END

    DECLARE @OrderId char(7)
    EXEC sp_GenerateId
        @prefix = 'OR',
        @tableName = 'Order',
        @idColumn = 'orderId',
        @idLength = 7,
        @newId = @OrderId OUTPUT

    INSERT INTO [Order](orderId, orderStatus, serveStatus, customerId, tableId, deliveryLocation, 
        employeeId, createdAt, updatedAt)
    VALUES (@OrderId, @OrderStatus, @ServeStatus, @CustomerId, @TableId, @DeliveryLocation,
        @EmployeeId, getdate(), getdate())
END
GO

--THÊM MÓN VÀO HÓA ĐƠN (đã thêm)
CREATE PROCEDURE sp_InsertMenuItemIntoOrderDetail
    @OrderId char(7),
    @MenuItemId char(7),
    @Quantity int,
    @CurrentPrice money
AS 
BEGIN
    SET NOCOUNT ON
    --Ktra orderId
    IF NOT EXISTS(SELECT 1 FROM [Order] WHERE orderId = @OrderId)
    BEGIN
     RAISERROR (N'Đơn hàng không tồn tại', 16, 1)
     RETURN
    END

    --Ktra menuItemId
    IF NOT EXISTS(SELECT 1 FROM MenuItem WHERE menuItemId = @MenuItemId)
    BEGIN 
        RAISERROR (N'Món ăn không tồn tại', 16, 1)
        RETURN
    END

    INSERT INTO OrderDetail(orderId, menuItemId, quantity, currentPrice, createdAt, updatedAt)
    VALUES (@OrderId, @MenuItemId, @Quantity, @CurrentPrice, getdate(), getdate())
END
GO

--THÊM HÓA ĐƠN VÀO HỆ THỐNG (đã thêm)
CREATE PROCEDURE sp_InsertInvoice
    @OrderId char(7),
    @InvoiceStatus nvarchar(30),
    @PayMentMethod nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON
    --Ktra orderId
    IF NOT EXISTS (SELECT 1 FROM [Order] WHERE orderId = @OrderId)
    BEGIN 
        RAISERROR(N'Đơn hàng không tồn tại', 16, 1)
        RETURN
    END
    
    --Set invoiceId
    DECLARE @InvoiceId char(7)
    EXEC sp_GenerateId
        @prefix = 'IV',
        @tableName = 'Invoice',
        @idColumn = 'invoiceId',
        @idLength = 7,
        @newId = @InvoiceId OUTPUT
    
    --set totalAmount
    DECLARE @TotalAmount money
    SELECT @TotalAmount = ISNULL(SUM(quantity * currentPrice), 0)
    FROM OrderDetail
    WHERE orderId = @OrderId

    INSERT INTO Invoice(invoiceId, orderId, invoiceStatus, paymentMethod, totalAmount, createdAt, updatedAt)
    VALUES(@InvoiceId, @OrderId, @InvoiceStatus, @PayMentMethod, @TotalAmount, getdate(), getdate())
END
GO

-- THÊM ĐƠN XUẤT
CREATE PROCEDURE sp_ExportOrder
    @EmployeeId char(7),
    @ExportDate datetime
AS
BEGIN 
    SET NOCOUNT ON;

    -- Kiểm tra employeeId
    IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeId = @EmployeeId)
    BEGIN
        RAISERROR(N'Không tồn tại nhân viên', 16, 1);
        RETURN;
    END;

    DECLARE @ExportOrderId char(7);

    EXEC sp_GenerateId
        @prefix = 'EX',
        @tableName = 'ExportOrder',
        @idColumn = 'exportOrderId',
        @idLength = 7,
        @newId = @ExportOrderId OUTPUT;

    INSERT INTO ExportOrder(employeeId, exportOrderId, exportDate, createdAt, updatedAt)
    VALUES (@EmployeeId, @ExportOrderId, @ExportDate, GETDATE(), GETDATE());

    SELECT @ExportOrderId AS ExportOrderId;
END;
GO


-- THÊM CHI TIẾT ĐƠN XUẤT
CREATE PROCEDURE sp_ExportOrderDetail
    @ExportOrderId char(7),
    @Quantity int,
    @Note nvarchar(50),
    @IngredientId char(7)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        -- Kiểm tra ExportOrder
        IF NOT EXISTS (SELECT 1 FROM ExportOrder WHERE exportOrderId = @ExportOrderId)
        BEGIN
            RAISERROR(N'Đơn xuất hàng không tồn tại', 16, 1);
            RETURN;
        END;

        -- Kiểm tra Ingredient
        IF NOT EXISTS (SELECT 1 FROM Ingredient WHERE ingredientId = @IngredientId)
        BEGIN
            RAISERROR(N'Nguyên liệu không tồn tại', 16, 1);
            RETURN;
        END;

        -- Kiểm tra đủ hàng
        IF (SELECT quantity FROM Ingredient WHERE ingredientId = @IngredientId) < @Quantity
        BEGIN
            RAISERROR(N'Số lượng trong kho không đủ để xuất', 16, 1);
            RETURN;
        END;

        -- Insert chi tiết xuất hàng
        INSERT INTO ExportOrderDetail(exportOrderId, quantity, note, createdAt, updatedAt, ingredientId)
        VALUES (@ExportOrderId, @Quantity, @Note, GETDATE(), GETDATE(), @IngredientId);

        -- Trừ kho
        UPDATE Ingredient
        SET quantity = quantity - @Quantity,
            updatedAt = GETDATE()
        WHERE ingredientId = @IngredientId;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH;
END;
GO

-------------------
--Thêm vào 2 thuộc tính còn thiếu cho Employee là email và phone
ALTER TABLE Employee
ADD email varchar(30) not null;

ALTER TABLE Employee
ADD phone varchar(20) not null;


-----------------------
--Sửa lại kiểu dữ liệu của tableName và thêm dữ liệu cho bàn.
ALTER TABLE [Table] ALTER COLUMN tableName nvarchar(20)
INSERT INTO [Table] VALUES('TB00001', 'Bàn 1', '1', 5, N'Còn trống', getdate(), getdate()),
                          ('TB00002', 'Bàn 2', '2', 5, N'Còn trống', getdate(), getdate()),
                          ('TB00003', 'Bàn 3', '3', 6, N'Còn trống', getdate(), getdate()),
                          ('TB00004', 'Bàn 4', '4', 7, N'Còn trống', getdate(), getdate()),
                          ('TB00005', 'Bàn 5', '5', 4, N'Còn trống', getdate(), getdate()), 
                          ('TB00006', 'Bàn 6', '6', 10, N'Còn trống', getdate(), getdate()),
                          ('TB00007', 'Bàn 7', '7', 7, N'Còn trống', getdate(), getdate()), 
                          ('TB00008', 'Bàn 8', '8', 6, N'Còn trống', getdate(), getdate()),
                          ('TB00009', 'Bàn 9', '9', 5, N'Còn trống', getdate(), getdate()),
                          ('TB00010', 'Bàn 10', '10', 6, N'Còn trống', getdate(), getdate());