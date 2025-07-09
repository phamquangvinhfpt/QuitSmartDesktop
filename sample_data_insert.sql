-- =============================================
-- SCRIPT THÊM DỮ LIỆU MẪU CHO QUITSMART APP
-- =============================================
USE QuitSmartDesktop
GO

-- Xóa dữ liệu cũ (nếu có)
DELETE FROM [AdminLogs];
DELETE FROM [UserBadges];
DELETE FROM [DailyLogs];
DELETE FROM [UserStatistics];
DELETE FROM [UserProfiles];
DELETE FROM [Users];
-- Không xóa Admins vì có thể đã có tài khoản admin
GO

-- =============================================
-- THÊM NGƯỜI DÙNG MẪU
-- =============================================

-- Khai báo các GUID cho users
DECLARE @User1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User3Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User4Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User5Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User6Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User7Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User8Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User9Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User10Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User11Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User12Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User13Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User14Id UNIQUEIDENTIFIER = NEWID();
DECLARE @User15Id UNIQUEIDENTIFIER = NEWID();

-- Thêm người dùng mẫu
INSERT INTO [Users] ([UserId], [Username], [Email], [PasswordHash], [FullName], [DateOfBirth], [Gender], [IsActive], [CreatedAt]) VALUES
(@User1Id, 'nguyenvana', 'nguyenvana@email.com', 'hashedpassword123', N'Nguyễn Văn A', '1990-05-15', 'Male', 1, DATEADD(DAY, -400, GETDATE())),
(@User2Id, 'tranthib', 'tranthib@email.com', 'hashedpassword123', N'Trần Thị B', '1988-03-22', 'Female', 1, DATEADD(DAY, -350, GETDATE())),
(@User3Id, 'lequangc', 'lequangc@email.com', 'hashedpassword123', N'Lê Quang C', '1992-12-10', 'Male', 1, DATEADD(DAY, -200, GETDATE())),
(@User4Id, 'phamthid', 'phamthid@email.com', 'hashedpassword123', N'Phạm Thị D', '1985-07-08', 'Female', 1, DATEADD(DAY, -150, GETDATE())),
(@User5Id, 'hoangvane', 'hoangvane@email.com', 'hashedpassword123', N'Hoàng Văn E', '1991-09-25', 'Male', 1, DATEADD(DAY, -120, GETDATE())),
(@User6Id, 'vothif', 'vothif@email.com', 'hashedpassword123', N'Võ Thị F', '1993-01-18', 'Female', 1, DATEADD(DAY, -100, GETDATE())),
(@User7Id, 'dangquang', 'dangquang@email.com', 'hashedpassword123', N'Đặng Quang G', '1987-11-30', 'Male', 1, DATEADD(DAY, -80, GETDATE())),
(@User8Id, 'buithih', 'buithih@email.com', 'hashedpassword123', N'Bùi Thị H', '1994-04-12', 'Female', 1, DATEADD(DAY, -60, GETDATE())),
(@User9Id, 'doanvani', 'doanvani@email.com', 'hashedpassword123', N'Đoàn Văn I', '1989-08-05', 'Male', 1, DATEADD(DAY, -45, GETDATE())),
(@User10Id, 'ngothij', 'ngothij@email.com', 'hashedpassword123', N'Ngô Thị J', '1995-06-20', 'Female', 1, DATEADD(DAY, -30, GETDATE())),
(@User11Id, 'trinhvank', 'trinhvank@email.com', 'hashedpassword123', N'Trịnh Văn K', '1986-02-14', 'Male', 1, DATEADD(DAY, -25, GETDATE())),
(@User12Id, 'dinhthil', 'dinhthil@email.com', 'hashedpassword123', N'Đinh Thị L', '1996-10-03', 'Female', 1, DATEADD(DAY, -20, GETDATE())),
(@User13Id, 'luvanm', 'luvanm@email.com', 'hashedpassword123', N'Lư Văn M', '1984-12-28', 'Male', 1, DATEADD(DAY, -15, GETDATE())),
(@User14Id, 'machin', 'machin@email.com', 'hashedpassword123', N'Mạc Thị N', '1997-03-16', 'Female', 1, DATEADD(DAY, -10, GETDATE())),
(@User15Id, 'caovano', 'caovano@email.com', 'hashedpassword123', N'Cao Văn O', '1983-09-07', 'Male', 1, DATEADD(DAY, -5, GETDATE()));

-- =============================================
-- THÊM PROFILE NGƯỜI DÙNG
-- =============================================

INSERT INTO [UserProfiles] ([UserId], [QuitStartDate], [QuitGoalDate], [CigarettesPerDay], [PricePerPack], [CigarettesPerPack], [SmokingYears], [QuitReason]) VALUES
(@User1Id, DATEADD(DAY, -400, GETDATE()), DATEADD(DAY, 365, GETDATE()), 20, 25000, 20, 10, N'Vì sức khỏe gia đình'),
(@User2Id, DATEADD(DAY, -350, GETDATE()), DATEADD(DAY, 365, GETDATE()), 15, 22000, 20, 8, N'Tiết kiệm tiền và sức khỏe'),
(@User3Id, DATEADD(DAY, -200, GETDATE()), DATEADD(DAY, 365, GETDATE()), 25, 30000, 20, 12, N'Chuẩn bị làm bố'),
(@User4Id, DATEADD(DAY, -150, GETDATE()), DATEADD(DAY, 365, GETDATE()), 10, 20000, 20, 6, N'Lời khuyên của bác sĩ'),
(@User5Id, DATEADD(DAY, -120, GETDATE()), DATEADD(DAY, 365, GETDATE()), 18, 24000, 20, 9, N'Muốn sống lâu hơn'),
(@User6Id, DATEADD(DAY, -100, GETDATE()), DATEADD(DAY, 365, GETDATE()), 12, 21000, 20, 5, N'Vì con cái'),
(@User7Id, DATEADD(DAY, -80, GETDATE()), DATEADD(DAY, 365, GETDATE()), 22, 28000, 20, 15, N'Giảm stress và lo âu'),
(@User8Id, DATEADD(DAY, -60, GETDATE()), DATEADD(DAY, 365, GETDATE()), 8, 18000, 20, 3, N'Cải thiện da và răng'),
(@User9Id, DATEADD(DAY, -45, GETDATE()), DATEADD(DAY, 365, GETDATE()), 16, 23000, 20, 7, N'Tăng cường thể lực'),
(@User10Id, DATEADD(DAY, -30, GETDATE()), DATEADD(DAY, 365, GETDATE()), 14, 22500, 20, 4, N'Làm gương cho bạn bè'),
(@User11Id, DATEADD(DAY, -25, GETDATE()), DATEADD(DAY, 365, GETDATE()), 30, 35000, 20, 18, N'Vợ yêu cầu cai'),
(@User12Id, DATEADD(DAY, -20, GETDATE()), DATEADD(DAY, 365, GETDATE()), 6, 16000, 20, 2, N'Học hỏi lối sống lành mạnh'),
(@User13Id, DATEADD(DAY, -15, GETDATE()), DATEADD(DAY, 365, GETDATE()), 24, 32000, 20, 20, N'Khám phá bản thân mới'),
(@User14Id, DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, 365, GETDATE()), 5, 15000, 20, 1, N'Gia đình không thích mùi thuốc'),
(@User15Id, DATEADD(DAY, -5, GETDATE()), DATEADD(DAY, 365, GETDATE()), 28, 40000, 20, 22, N'Tuổi tác và sức khỏe suy giảm');

-- =============================================
-- THÊM DAILY LOGS (Tạo lịch sử cai thuốc đa dạng)
-- =============================================

-- User 1: Cai được 380+ ngày (rất thành công)
DECLARE @i INT = 0;
WHILE @i < 380
BEGIN
    INSERT INTO [DailyLogs] ([UserId], [LogDate], [HasSmoked], [HealthStatus], [Notes])
    VALUES (@User1Id, DATEADD(DAY, -380 + @i, GETDATE()), 
           CASE WHEN @i % 50 = 0 AND @i > 0 THEN 1 ELSE 0 END, -- Thỉnh thoảng có lúc trượt
           CASE WHEN @i % 3 = 0 THEN 'Good' WHEN @i % 3 = 1 THEN 'Average' ELSE 'Poor' END,
           N'Ngày ' + CAST(@i + 1 AS NVARCHAR(10)));
    SET @i = @i + 1;
END;

-- User 2: Cai được 320+ ngày (khá thành công)
SET @i = 0;
WHILE @i < 320
BEGIN
    INSERT INTO [DailyLogs] ([UserId], [LogDate], [HasSmoked], [HealthStatus], [Notes])
    VALUES (@User2Id, DATEADD(DAY, -320 + @i, GETDATE()), 
           CASE WHEN @i % 30 = 0 AND @i > 0 THEN 1 ELSE 0 END, -- Thường xuyên trượt hơn
           CASE WHEN @i % 3 = 0 THEN 'Good' WHEN @i % 3 = 1 THEN 'Average' ELSE 'Poor' END,
           N'Cảm thấy tốt hơn');
    SET @i = @i + 1;
END;

-- User 3: Cai được 150+ ngày (trung bình)
SET @i = 0;
WHILE @i < 150
BEGIN
    INSERT INTO [DailyLogs] ([UserId], [LogDate], [HasSmoked], [HealthStatus], [Notes])
    VALUES (@User3Id, DATEADD(DAY, -150 + @i, GETDATE()), 
           CASE WHEN @i % 20 = 0 AND @i > 10 THEN 1 ELSE 0 END,
           CASE WHEN @i % 3 = 0 THEN 'Good' WHEN @i % 3 = 1 THEN 'Average' ELSE 'Poor' END,
           N'Đang cố gắng');
    SET @i = @i + 1;
END;

-- User 4: Cai được 100+ ngày
SET @i = 0;
WHILE @i < 100
BEGIN
    INSERT INTO [DailyLogs] ([UserId], [LogDate], [HasSmoked], [HealthStatus], [Notes])
    VALUES (@User4Id, DATEADD(DAY, -100 + @i, GETDATE()), 
           CASE WHEN @i % 15 = 0 AND @i > 5 THEN 1 ELSE 0 END,
           'Good', N'Tốt');
    SET @i = @i + 1;
END;

-- User 5: Cai được 80+ ngày
SET @i = 0;
WHILE @i < 80
BEGIN
    INSERT INTO [DailyLogs] ([UserId], [LogDate], [HasSmoked], [HealthStatus], [Notes])
    VALUES (@User5Id, DATEADD(DAY, -80 + @i, GETDATE()), 
           CASE WHEN @i % 12 = 0 AND @i > 3 THEN 1 ELSE 0 END,
           'Average', N'Bình thường');
    SET @i = @i + 1;
END;

-- User 6: Cai được 60+ ngày
SET @i = 0;
WHILE @i < 60
BEGIN
    INSERT INTO [DailyLogs] ([UserId], [LogDate], [HasSmoked], [HealthStatus], [Notes])
    VALUES (@User6Id, DATEADD(DAY, -60 + @i, GETDATE()), 
           CASE WHEN @i % 10 = 0 AND @i > 2 THEN 1 ELSE 0 END,
           'Good', N'Khá tốt');
    SET @i = @i + 1;
END;

-- User 7: Cai được 40+ ngày
SET @i = 0;
WHILE @i < 40
BEGIN
    INSERT INTO [DailyLogs] ([UserId], [LogDate], [HasSmoked], [HealthStatus], [Notes])
    VALUES (@User7Id, DATEADD(DAY, -40 + @i, GETDATE()), 
           CASE WHEN @i % 8 = 0 AND @i > 1 THEN 1 ELSE 0 END,
           'Average', N'Cố gắng');
    SET @i = @i + 1;
END;

-- User 8: Cai được 30+ ngày
SET @i = 0;
WHILE @i < 30
BEGIN
    INSERT INTO [DailyLogs] ([UserId], [LogDate], [HasSmoked], [HealthStatus], [Notes])
    VALUES (@User8Id, DATEADD(DAY, -30 + @i, GETDATE()), 
           CASE WHEN @i % 6 = 0 AND @i > 1 THEN 1 ELSE 0 END,
           'Poor', N'Khó khăn');
    SET @i = @i + 1;
END;

-- User 9: Cai được 20+ ngày
SET @i = 0;
WHILE @i < 20
BEGIN
    INSERT INTO [DailyLogs] ([UserId], [LogDate], [HasSmoked], [HealthStatus], [Notes])
    VALUES (@User9Id, DATEADD(DAY, -20 + @i, GETDATE()), 
           CASE WHEN @i % 5 = 0 AND @i > 0 THEN 1 ELSE 0 END,
           'Average', N'Vẫn còn khó');
    SET @i = @i + 1;
END;

-- User 10: Cai được 15+ ngày
SET @i = 0;
WHILE @i < 15
BEGIN
    INSERT INTO [DailyLogs] ([UserId], [LogDate], [HasSmoked], [HealthStatus], [Notes])
    VALUES (@User10Id, DATEADD(DAY, -15 + @i, GETDATE()), 
           CASE WHEN @i % 4 = 0 AND @i > 0 THEN 1 ELSE 0 END,
           'Good', N'Hy vọng');
    SET @i = @i + 1;
END;

-- User 11: Cai được 10+ ngày
SET @i = 0;
WHILE @i < 10
BEGIN
    INSERT INTO [DailyLogs] ([UserId], [LogDate], [HasSmoked], [HealthStatus], [Notes])
    VALUES (@User11Id, DATEADD(DAY, -10 + @i, GETDATE()), 
           CASE WHEN @i % 3 = 0 AND @i > 0 THEN 1 ELSE 0 END,
           'Poor', N'Mới bắt đầu');
    SET @i = @i + 1;
END;

-- User 12: Cai được 8+ ngày
SET @i = 0;
WHILE @i < 8
BEGIN
    INSERT INTO [DailyLogs] ([UserId], [LogDate], [HasSmoked], [HealthStatus], [Notes])
    VALUES (@User12Id, DATEADD(DAY, -8 + @i, GETDATE()), 0, 'Average', N'Mới bắt đầu hành trình');
    SET @i = @i + 1;
END;

-- User 13: Cai được 5+ ngày  
SET @i = 0;
WHILE @i < 5
BEGIN
    INSERT INTO [DailyLogs] ([UserId], [LogDate], [HasSmoked], [HealthStatus], [Notes])
    VALUES (@User13Id, DATEADD(DAY, -5 + @i, GETDATE()), 0, 'Good', N'Quyết tâm cao');
    SET @i = @i + 1;
END;

-- User 14: Cai được 3+ ngày
SET @i = 0;
WHILE @i < 3
BEGIN
    INSERT INTO [DailyLogs] ([UserId], [LogDate], [HasSmoked], [HealthStatus], [Notes])
    VALUES (@User14Id, DATEADD(DAY, -3 + @i, GETDATE()), 0, 'Poor', N'Rất khó');
    SET @i = @i + 1;
END;

-- User 15: Cai được 1+ ngày
INSERT INTO [DailyLogs] ([UserId], [LogDate], [HasSmoked], [HealthStatus], [Notes])
VALUES (@User15Id, DATEADD(DAY, -1, GETDATE()), 0, 'Average', N'Ngày đầu tiên');

-- =============================================
-- TÍNH TOÁN THỐNG KÊ CHO TẤT CẢ USER
-- =============================================

-- Gọi stored procedure để tính toán thống kê cho tất cả users
EXEC [CalculateUserStatistics] @User1Id;
EXEC [CalculateUserStatistics] @User2Id;
EXEC [CalculateUserStatistics] @User3Id;
EXEC [CalculateUserStatistics] @User4Id;
EXEC [CalculateUserStatistics] @User5Id;
EXEC [CalculateUserStatistics] @User6Id;
EXEC [CalculateUserStatistics] @User7Id;
EXEC [CalculateUserStatistics] @User8Id;
EXEC [CalculateUserStatistics] @User9Id;
EXEC [CalculateUserStatistics] @User10Id;
EXEC [CalculateUserStatistics] @User11Id;
EXEC [CalculateUserStatistics] @User12Id;
EXEC [CalculateUserStatistics] @User13Id;
EXEC [CalculateUserStatistics] @User14Id;
EXEC [CalculateUserStatistics] @User15Id;

-- Gọi stored procedure để trao badges
EXEC [CheckAndAwardBadges] @User1Id;
EXEC [CheckAndAwardBadges] @User2Id;
EXEC [CheckAndAwardBadges] @User3Id;
EXEC [CheckAndAwardBadges] @User4Id;
EXEC [CheckAndAwardBadges] @User5Id;
EXEC [CheckAndAwardBadges] @User6Id;
EXEC [CheckAndAwardBadges] @User7Id;
EXEC [CheckAndAwardBadges] @User8Id;
EXEC [CheckAndAwardBadges] @User9Id;
EXEC [CheckAndAwardBadges] @User10Id;
EXEC [CheckAndAwardBadges] @User11Id;
EXEC [CheckAndAwardBadges] @User12Id;
EXEC [CheckAndAwardBadges] @User13Id;
EXEC [CheckAndAwardBadges] @User14Id;
EXEC [CheckAndAwardBadges] @User15Id;

-- =============================================
-- KIỂM TRA KẾT QUẢ
-- =============================================

DECLARE @UserCount INT, @LogCount INT, @BadgeCount INT;

SELECT @UserCount = COUNT(*) FROM [Users];
SELECT @LogCount = COUNT(*) FROM [DailyLogs];
SELECT @BadgeCount = COUNT(*) FROM [UserBadges];

PRINT 'Đã thêm dữ liệu mẫu thành công!';
PRINT 'Tổng số users: ' + CAST(@UserCount AS NVARCHAR(10));
PRINT 'Tổng số daily logs: ' + CAST(@LogCount AS NVARCHAR(10));
PRINT 'Tổng số badges được trao: ' + CAST(@BadgeCount AS NVARCHAR(10));

-- Hiển thị thống kê tổng quan
SELECT 
    'Users >= 7 days' as Category,
    COUNT(*) as Count
FROM [UserStatistics] 
WHERE [TotalDaysQuit] >= 7
UNION ALL
SELECT 
    'Users >= 30 days' as Category,
    COUNT(*) as Count
FROM [UserStatistics] 
WHERE [TotalDaysQuit] >= 30
UNION ALL
SELECT 
    'Users >= 90 days' as Category,
    COUNT(*) as Count
FROM [UserStatistics] 
WHERE [TotalDaysQuit] >= 90
UNION ALL
SELECT 
    'Users >= 180 days' as Category,
    COUNT(*) as Count
FROM [UserStatistics] 
WHERE [TotalDaysQuit] >= 180
UNION ALL
SELECT 
    'Users >= 365 days' as Category,
    COUNT(*) as Count
FROM [UserStatistics] 
WHERE [TotalDaysQuit] >= 365;

-- Hiển thị top users
SELECT TOP 10
    u.[FullName],
    us.[TotalDaysQuit],
    us.[TotalMoneySaved],
    us.[CurrentStreak],
    us.[LongestStreak]
FROM [Users] u
INNER JOIN [UserStatistics] us ON u.[UserId] = us.[UserId]
ORDER BY us.[TotalDaysQuit] DESC;

GO 