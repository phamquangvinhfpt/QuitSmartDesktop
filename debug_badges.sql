-- Script debug badges để kiểm tra tại sao view hiển thị 0
USE QuitSmartDesktop
GO

PRINT '=== DEBUG BADGES DATA ===';

-- 1. Kiểm tra badge definitions
PRINT '1. Badge Definitions:';
SELECT [Name], [BadgeType], [RequiredValue], [IsActive] 
FROM [BadgeDefinitions] 
WHERE [IsActive] = 1
ORDER BY [BadgeType], [RequiredValue];

-- 2. Kiểm tra user statistics  
PRINT '2. Top 5 User Statistics:';
SELECT TOP 5
    u.[FullName],
    us.[TotalDaysQuit],
    us.[TotalMoneySaved],
    us.[CurrentStreak],
    us.[LongestStreak]
FROM [Users] u
INNER JOIN [UserStatistics] us ON u.[UserId] = us.[UserId]
WHERE u.[IsActive] = 1
ORDER BY us.[TotalDaysQuit] DESC;

-- 3. Kiểm tra user badges hiện có
PRINT '3. Current User Badges:';
SELECT 
    u.[FullName],
    bd.[Name] as BadgeName,
    bd.[BadgeType],
    bd.[RequiredValue],
    ub.[EarnedAt]
FROM [UserBadges] ub
INNER JOIN [Users] u ON ub.[UserId] = u.[UserId]
INNER JOIN [BadgeDefinitions] bd ON ub.[BadgeId] = bd.[BadgeId]
WHERE u.[IsActive] = 1
ORDER BY u.[FullName], bd.[BadgeType], bd.[RequiredValue];

-- 4. Kiểm tra view UserOverview
PRINT '4. UserOverview View Data:';
SELECT TOP 5
    [FullName],
    [TotalDaysQuit],
    [TotalMoneySaved], 
    [TotalBadges]
FROM [UserOverview]
ORDER BY [TotalDaysQuit] DESC;

-- 5. Test manual badge calculation
PRINT '5. Manual Badge Calculation:';
SELECT 
    u.[FullName],
    us.[TotalDaysQuit],
    us.[TotalMoneySaved],
    us.[CurrentStreak],
    (SELECT COUNT(*) FROM [UserBadges] ub WHERE ub.[UserId] = u.[UserId]) as ManualBadgeCount,
    -- Days badges should have
    (SELECT COUNT(*) FROM [BadgeDefinitions] bd 
     WHERE bd.[BadgeType] = 'Days' AND bd.[IsActive] = 1 
     AND bd.[RequiredValue] <= ISNULL(us.[TotalDaysQuit], 0)) as ShouldHaveDaysBadges,
    -- Money badges should have  
    (SELECT COUNT(*) FROM [BadgeDefinitions] bd 
     WHERE bd.[BadgeType] = 'Money' AND bd.[IsActive] = 1 
     AND bd.[RequiredValue] <= ISNULL(us.[TotalMoneySaved], 0)) as ShouldHaveMoneyBadges,
    -- Streak badges should have
    (SELECT COUNT(*) FROM [BadgeDefinitions] bd 
     WHERE bd.[BadgeType] = 'Streak' AND bd.[IsActive] = 1 
     AND bd.[RequiredValue] <= ISNULL(us.[CurrentStreak], 0)) as ShouldHaveStreakBadges
FROM [Users] u
LEFT JOIN [UserStatistics] us ON u.[UserId] = us.[UserId]
WHERE u.[IsActive] = 1
ORDER BY us.[TotalDaysQuit] DESC;

-- 6. Kiểm tra stored procedure có hoạt động không
PRINT '6. Testing Stored Procedure with specific user:';
DECLARE @TestUserId UNIQUEIDENTIFIER;
SELECT TOP 1 @TestUserId = [UserId] FROM [Users] WHERE [IsActive] = 1;

PRINT 'Testing with UserId: ' + CAST(@TestUserId AS NVARCHAR(50));

-- Gọi stored procedures
EXEC [CalculateUserStatistics] @TestUserId;
EXEC [CheckAndAwardBadges] @TestUserId;

-- Kiểm tra kết quả
SELECT 
    u.[FullName],
    us.[TotalDaysQuit],
    us.[TotalMoneySaved],
    (SELECT COUNT(*) FROM [UserBadges] ub WHERE ub.[UserId] = @TestUserId) as BadgeCountAfterSP
FROM [Users] u
LEFT JOIN [UserStatistics] us ON u.[UserId] = us.[UserId]
WHERE u.[UserId] = @TestUserId;

-- 7. Refresh view if needed
PRINT '7. Refreshing view data...';

-- Force refresh statistics cho tất cả users
DECLARE @UserId UNIQUEIDENTIFIER;
DECLARE user_cursor CURSOR FOR 
    SELECT [UserId] FROM [Users] WHERE [IsActive] = 1;

OPEN user_cursor;
FETCH NEXT FROM user_cursor INTO @UserId;

WHILE @@FETCH_STATUS = 0
BEGIN
    EXEC [CalculateUserStatistics] @UserId;
    EXEC [CheckAndAwardBadges] @UserId;
    FETCH NEXT FROM user_cursor INTO @UserId;
END;

CLOSE user_cursor;
DEALLOCATE user_cursor;

-- 8. Final check
PRINT '8. Final Badge Count Check:';
SELECT TOP 10
    u.[FullName],
    us.[TotalDaysQuit],
    us.[TotalMoneySaved],
    (SELECT COUNT(*) FROM [UserBadges] ub WHERE ub.[UserId] = u.[UserId]) as FinalBadgeCount,
    -- List badge names
    (SELECT STRING_AGG(bd.[Name], ', ') 
     FROM [UserBadges] ub 
     INNER JOIN [BadgeDefinitions] bd ON ub.[BadgeId] = bd.[BadgeId]
     WHERE ub.[UserId] = u.[UserId]) as BadgeNames
FROM [Users] u
LEFT JOIN [UserStatistics] us ON u.[UserId] = us.[UserId]
WHERE u.[IsActive] = 1
ORDER BY us.[TotalDaysQuit] DESC;

PRINT '=== COMPLETED DEBUG ===';
GO 