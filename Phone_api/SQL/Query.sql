INSERT INTO Brands (Id, Name, Created, LastModified, ModerationStatus)
VALUES 
('0d703727-f962-4b91-87a7-afc7ee5c0174', N'Samsung', GETDATE(), GETDATE(), 1),
('d2a1e9d1-2c22-41f0-9c1b-1e6a024674c1', N'Apple', GETDATE(), GETDATE(), 1),
('b91c594e-caf5-4c1d-91f6-cc365ef2a650', N'Xiaomi', GETDATE(), GETDATE(), 1),
('c4720a96-ec2d-45f7-ae4d-38135cd6f9f7', N'Oppo', GETDATE(), GETDATE(), 1),
('ff4f6fd7-2216-46c0-8137-64d287b2c3e5', N'Nokia', GETDATE(), GETDATE(), 1);

DECLARE @Samsung UNIQUEIDENTIFIER = '0d703727-f962-4b91-87a7-afc7ee5c0174';
DECLARE @Apple UNIQUEIDENTIFIER   = 'd2a1e9d1-2c22-41f0-9c1b-1e6a024674c1';
DECLARE @Xiaomi UNIQUEIDENTIFIER  = 'b91c594e-caf5-4c1d-91f6-cc365ef2a650';
DECLARE @Oppo UNIQUEIDENTIFIER    = 'c4720a96-ec2d-45f7-ae4d-38135cd6f9f7';
DECLARE @Nokia UNIQUEIDENTIFIER   = 'ff4f6fd7-2216-46c0-8137-64d287b2c3e5';

INSERT INTO Phones (Id, Model, Price, Stock, BrandId, Created, LastModified, ModerationStatus)
VALUES 
-- Samsung
(NEWID(), 'Galaxy S23', 799.99, 50, @Samsung, GETDATE(), GETDATE(), 1),
(NEWID(), 'Galaxy A54', 399.99, 80, @Samsung, GETDATE(), GETDATE(), 1),
(NEWID(), 'Galaxy Z Flip 4', 999.99, 30, @Samsung, GETDATE(), GETDATE(), 1),
(NEWID(), 'Galaxy M14', 249.99, 60, @Samsung, GETDATE(), GETDATE(), 1),
(NEWID(), 'Galaxy Note 20', 899.99, 15, @Samsung, GETDATE(), GETDATE(), 1),
(NEWID(), 'Galaxy S21 FE', 499.99, 40, @Samsung, GETDATE(), GETDATE(), 1),

-- Apple
(NEWID(), 'iPhone 14 Pro', 1199.99, 20, @Apple, GETDATE(), GETDATE(), 1),
(NEWID(), 'iPhone 13', 899.99, 30, @Apple, GETDATE(), GETDATE(), 1),
(NEWID(), 'iPhone SE 2022', 429.99, 70, @Apple, GETDATE(), GETDATE(), 1),
(NEWID(), 'iPhone 12 Mini', 699.99, 25, @Apple, GETDATE(), GETDATE(), 1),
(NEWID(), 'iPhone 15 Pro Max', 1399.99, 10, @Apple, GETDATE(), GETDATE(), 1),
(NEWID(), 'iPhone XR', 499.99, 60, @Apple, GETDATE(), GETDATE(), 1),

-- Xiaomi
(NEWID(), 'Redmi Note 12', 299.99, 100, @Xiaomi, GETDATE(), GETDATE(), 1),
(NEWID(), 'Redmi 12C', 149.99, 80, @Xiaomi, GETDATE(), GETDATE(), 1),
(NEWID(), 'Xiaomi 13 Pro', 999.99, 25, @Xiaomi, GETDATE(), GETDATE(), 1),
(NEWID(), 'POCO X5 Pro', 349.99, 45, @Xiaomi, GETDATE(), GETDATE(), 1),
(NEWID(), 'Xiaomi Mi 11', 649.99, 35, @Xiaomi, GETDATE(), GETDATE(), 1),
(NEWID(), 'Redmi K60', 429.99, 60, @Xiaomi, GETDATE(), GETDATE(), 1),

-- Oppo
(NEWID(), 'Oppo Reno8', 449.99, 60, @Oppo, GETDATE(), GETDATE(), 1),
(NEWID(), 'Oppo Find X5', 849.99, 20, @Oppo, GETDATE(), GETDATE(), 1),
(NEWID(), 'Oppo A96', 299.99, 50, @Oppo, GETDATE(), GETDATE(), 1),
(NEWID(), 'Oppo A17', 199.99, 75, @Oppo, GETDATE(), GETDATE(), 1),
(NEWID(), 'Oppo Reno10', 549.99, 15, @Oppo, GETDATE(), GETDATE(), 1),
(NEWID(), 'Oppo F21 Pro', 389.99, 45, @Oppo, GETDATE(), GETDATE(), 1),

-- Nokia
(NEWID(), 'Nokia G21', 199.99, 40, @Nokia, GETDATE(), GETDATE(), 1),
(NEWID(), 'Nokia C32', 129.99, 60, @Nokia, GETDATE(), GETDATE(), 1),
(NEWID(), 'Nokia X30', 349.99, 25, @Nokia, GETDATE(), GETDATE(), 1),
(NEWID(), 'Nokia 5.4', 179.99, 50, @Nokia, GETDATE(), GETDATE(), 1),
(NEWID(), 'Nokia 7.2', 249.99, 30, @Nokia, GETDATE(), GETDATE(), 1),
(NEWID(), 'Nokia 8.3', 399.99, 20, @Nokia, GETDATE(), GETDATE(), 1);

GO
CREATE OR ALTER PROCEDURE AddPhone
    @Model NVARCHAR(100),
    @Price DECIMAL(18,2),
    @Stock INT,
    @BrandId UNIQUEIDENTIFIER = NULL,
    @Result BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Khởi tạo kết quả mặc định là 0 (false)
    SET @Result = 0;

    BEGIN TRY
        -- Validate Model
        IF @Model IS NULL OR LTRIM(RTRIM(@Model)) = ''
        BEGIN
            RETURN;
        END;

        -- Validate Price
        IF @Price <= 0
        BEGIN
            RETURN;
        END;

        -- Validate Stock
        IF @Stock < 0
        BEGIN
            RETURN;
        END;

        -- Tạo biến ModerationStatus mặc định là 0 (Rejected)
        DECLARE @ModerationStatus INT = 1;

        -- Thêm Phone mới
        INSERT INTO Phones (Model, Price, Stock, Created, LastModified, ModerationStatus, BrandId)
        VALUES (
            @Model,
            @Price,
            @Stock,
            GETUTCDATE(), -- Created (UTC)
            GETUTCDATE(), -- LastModified (UTC)
            @ModerationStatus,
            @BrandId
        );

        -- Nếu thêm thành công, đặt kết quả là 1 (true)
        SET @Result = 1;
    END TRY
    BEGIN CATCH
        -- Nếu có lỗi (ví dụ: lỗi constraint, lỗi hệ thống), giữ @Result = 0
        SET @Result = 0;
    END CATCH;
END;