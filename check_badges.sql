-- Script kiểm tra và sửa lỗi badges
USE QuitSmartDesktop
GO

-- Kiểm tra dữ liệu hiện tại
PRINT '=== KIỂM TRA DỮ LIỆU BADGES ===';

SELECT 'Badge Definitions' as TableName, COUNT(*) as Count FROM [BadgeDefinitions];
SELECT 'User Badges' as TableName, COUNT(*) as Count FROM [UserBadges];
SELECT 'User Statistics' as TableName, COUNT(*) as Count FROM [UserStatistics];

PRINT '=== CHI TIẾT BADGE DEFINITIONS ===';
SELECT [Name], [BadgeType], [RequiredValue], [IsActive] 
FROM [BadgeDefinitions] 
ORDER BY [BadgeType], [RequiredValue];

PRINT '=== CHI TIẾT USER STATISTICS ===';
SELECT TOP 5 
    u.[FullName],
    us.[TotalDaysQuit],
    us.[TotalMoneySaved],
    us.[CurrentStreak],
    (SELECT COUNT(*) FROM [UserBadges] ub WHERE ub.[UserId] = u.[UserId]) as BadgeCount
FROM [Users] u
LEFT JOIN [UserStatistics] us ON u.[UserId] = us.[UserId]
ORDER BY us.[TotalDaysQuit] DESC;

PRINT '=== CHẠY LẠI STORED PROCEDURE CHO TẤT CẢ USERS ===';

-- Tạo cursor để chạy lại CheckAndAwardBadges cho tất cả users
DECLARE @UserId UNIQUEIDENTIFIER;
DECLARE user_cursor CURSOR FOR 
    SELECT [UserId] FROM [Users] WHERE [IsActive] = 1;

OPEN user_cursor;
FETCH NEXT FROM user_cursor INTO @UserId;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Tính lại statistics trước
    EXEC [CalculateUserStatistics] @UserId;
    
    -- Sau đó trao badges
    EXEC [CheckAndAwardBadges] @UserId;
    
    FETCH NEXT FROM user_cursor INTO @UserId;
END;

CLOSE user_cursor;
DEALLOCATE user_cursor;

PRINT '=== KẾT QUẢ SAU KHI CHẠY LẠI ===';

-- Kiểm tra lại kết quả
SELECT TOP 10
    u.[FullName],
    us.[TotalDaysQuit],
    us.[TotalMoneySaved],
    (SELECT COUNT(*) FROM [UserBadges] ub WHERE ub.[UserId] = u.[UserId]) as BadgeCount,
    (SELECT STRING_AGG(bd.[Name], ', ') 
     FROM [UserBadges] ub 
     INNER JOIN [BadgeDefinitions] bd ON ub.[BadgeId] = bd.[BadgeId]
     WHERE ub.[UserId] = u.[UserId]) as BadgeNames
FROM [Users] u
LEFT JOIN [UserStatistics] us ON u.[UserId] = us.[UserId]
WHERE u.[IsActive] = 1
ORDER BY us.[TotalDaysQuit] DESC;

PRINT '=== HOÀN THÀNH ===';
GO 