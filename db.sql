USE QuitSmartDesktop
GO

-- =============================================
-- SECTION 1: USERS TABLE
-- =============================================

-- Main users table (Members)
CREATE TABLE [Users] (
    [UserId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Username] NVARCHAR(50) NOT NULL UNIQUE,
    [Email] NVARCHAR(255) NOT NULL UNIQUE,
    [PasswordHash] NVARCHAR(255) NOT NULL,
    [FullName] NVARCHAR(100) NOT NULL,
    [DateOfBirth] DATE,
    [Gender] NVARCHAR(10) CHECK ([Gender] IN ('Male', 'Female', 'Other')),
    [IsActive] BIT DEFAULT 1,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE()
)
GO

-- User smoking profile and goals
CREATE TABLE [UserProfiles] (
    [ProfileId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [QuitStartDate] DATE NOT NULL,                    -- Ngày bắt đầu cai thuốc
    [QuitGoalDate] DATE,                             -- Ngày mục tiêu cai thuốc hoàn toàn
    [CigarettesPerDay] INT NOT NULL,                 -- Số điếu mỗi ngày
    [PricePerPack] DECIMAL(10,2) NOT NULL,           -- Giá 1 gói thuốc
    [CigarettesPerPack] INT DEFAULT 20,              -- Số điếu/gói
    [SmokingYears] INT,                              -- Số năm đã hút
    [QuitReason] NVARCHAR(500),                      -- Lý do muốn cai thuốc
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    
    FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE
)
GO

-- =============================================
-- SECTION 2: DAILY TRACKING
-- =============================================

-- Daily quit tracking logs
CREATE TABLE [DailyLogs] (
    [LogId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [LogDate] DATE NOT NULL,
    [HasSmoked] BIT DEFAULT 0,                       -- Có hút thuốc không? (0=Không, 1=Có)
    [HealthStatus] NVARCHAR(20) CHECK ([HealthStatus] IN ('Good', 'Average', 'Poor')), -- Sức khỏe hôm nay
    [Notes] NVARCHAR(255),                           -- Ghi chú tự do (max 255 ký tự)
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    
    FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    UNIQUE ([UserId], [LogDate])
)
GO

-- User statistics summary
CREATE TABLE [UserStatistics] (
    [StatId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [TotalDaysQuit] INT DEFAULT 0,                   -- Tổng số ngày không hút
    [TotalMoneySaved] DECIMAL(10,2) DEFAULT 0,       -- Tổng số tiền đã tiết kiệm
    [CurrentStreak] INT DEFAULT 0,                   -- Chuỗi ngày hiện tại không hút
    [LongestStreak] INT DEFAULT 0,                   -- Chuỗi ngày dài nhất không hút
    [LastCalculatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    
    FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    UNIQUE ([UserId])
)
GO

-- =============================================
-- SECTION 3: ACHIEVEMENTS SYSTEM
-- =============================================

-- Badge definitions
CREATE TABLE [BadgeDefinitions] (
    [BadgeId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Name] NVARCHAR(100) NOT NULL,                   -- Tên huy hiệu
    [Description] NVARCHAR(500),                     -- Mô tả huy hiệu
    [IconPath] NVARCHAR(500),                        -- Đường dẫn icon local
    [BadgeType] NVARCHAR(50) NOT NULL,               -- Loại: 'Days', 'Money', 'Streak'
    [RequiredValue] DECIMAL(10,2) NOT NULL,          -- Giá trị yêu cầu để đạt huy hiệu
    [IsActive] BIT DEFAULT 1,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
)
GO

-- User earned badges
CREATE TABLE [UserBadges] (
    [UserBadgeId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [BadgeId] UNIQUEIDENTIFIER NOT NULL,
    [EarnedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [IsNotified] BIT DEFAULT 0,                      -- Đã thông báo cho user chưa
    
    FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    FOREIGN KEY ([BadgeId]) REFERENCES [BadgeDefinitions]([BadgeId]) ON DELETE CASCADE,
    UNIQUE ([UserId], [BadgeId])
)
GO

-- =============================================
-- SECTION 4: MOTIVATIONAL CONTENT
-- =============================================

-- Motivational messages (pre-loaded data)
CREATE TABLE [MotivationalMessages] (
    [MessageId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Title] NVARCHAR(200),
    [Content] NVARCHAR(1000) NOT NULL,               -- Nội dung thông điệp
    [MessageType] NVARCHAR(50) NOT NULL,             -- 'Daily', 'Achievement', 'Encouragement'
    [IsActive] BIT DEFAULT 1,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
)
GO

-- Health information content (static info about smoking effects)
CREATE TABLE [HealthInfo] (
    [InfoId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Title] NVARCHAR(200) NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,                -- Nội dung thông tin sức khỏe
    [InfoType] NVARCHAR(50) NOT NULL,                -- 'SmokeEffects', 'QuitBenefits', 'Tips'
    [IsActive] BIT DEFAULT 1,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
)
GO

-- =============================================
-- SECTION 5: ADMIN SYSTEM
-- =============================================

-- Admin users table
CREATE TABLE [Admins] (
    [AdminId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Username] NVARCHAR(50) NOT NULL UNIQUE,
    [Email] NVARCHAR(255) NOT NULL UNIQUE,
    [PasswordHash] NVARCHAR(255) NOT NULL,
    [FullName] NVARCHAR(100) NOT NULL,
    [IsActive] BIT DEFAULT 1,
    [LastLoginAt] DATETIME2,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE()
)
GO

-- Admin activity logs
CREATE TABLE [AdminLogs] (
    [LogId] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [AdminId] UNIQUEIDENTIFIER NOT NULL,
    [Action] NVARCHAR(100) NOT NULL,                 -- 'ViewUser', 'DeleteUser', 'EditUser', etc.
    [TargetUserId] UNIQUEIDENTIFIER,                 -- User được tác động
    [Details] NVARCHAR(500),                         -- Chi tiết hành động
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    
    FOREIGN KEY ([AdminId]) REFERENCES [Admins]([AdminId]) ON DELETE CASCADE
)
GO

-- =============================================
-- SECTION 6: CREATE INDEXES
-- =============================================

-- Indexes for Users table
CREATE INDEX IX_Users_Username ON [Users]([Username])
CREATE INDEX IX_Users_Email ON [Users]([Email])
CREATE INDEX IX_Users_IsActive ON [Users]([IsActive])

-- Indexes for DailyLogs table
CREATE INDEX IX_DailyLogs_UserId_LogDate ON [DailyLogs]([UserId], [LogDate])
CREATE INDEX IX_DailyLogs_LogDate ON [DailyLogs]([LogDate])
CREATE INDEX IX_DailyLogs_HasSmoked ON [DailyLogs]([HasSmoked])

-- Indexes for UserBadges table
CREATE INDEX IX_UserBadges_UserId ON [UserBadges]([UserId])
CREATE INDEX IX_UserBadges_EarnedAt ON [UserBadges]([EarnedAt])

-- Indexes for AdminLogs table
CREATE INDEX IX_AdminLogs_AdminId_CreatedAt ON [AdminLogs]([AdminId], [CreatedAt])
CREATE INDEX IX_AdminLogs_Action ON [AdminLogs]([Action])
GO

-- =============================================
-- SECTION 7: CREATE VIEWS
-- =============================================

-- User overview for admin dashboard
CREATE VIEW [UserOverview] AS
SELECT 
    u.[UserId],
    u.[Username],
    u.[Email],
    u.[FullName],
    u.[Gender],
    u.[IsActive],
    u.[CreatedAt],
    up.[QuitStartDate],
    up.[CigarettesPerDay],
    up.[PricePerPack],
    up.[QuitReason],
    us.[TotalDaysQuit],
    us.[TotalMoneySaved],
    us.[CurrentStreak],
    us.[LongestStreak],
    (SELECT COUNT(*) FROM [UserBadges] ub WHERE ub.[UserId] = u.[UserId]) AS TotalBadges
FROM [Users] u
LEFT JOIN [UserProfiles] up ON u.[UserId] = up.[UserId]
LEFT JOIN [UserStatistics] us ON u.[UserId] = us.[UserId]
WHERE u.[IsActive] = 1
GO

-- Health tracking overview
CREATE VIEW [HealthTrackingOverview] AS
SELECT 
    u.[UserId],
    u.[FullName],
    dl.[LogDate],
    dl.[HasSmoked],
    dl.[HealthStatus],
    dl.[Notes],
    us.[CurrentStreak],
    us.[TotalDaysQuit]
FROM [DailyLogs] dl
INNER JOIN [Users] u ON dl.[UserId] = u.[UserId]
LEFT JOIN [UserStatistics] us ON u.[UserId] = us.[UserId]
WHERE u.[IsActive] = 1
GO

-- Badge collection view
CREATE VIEW [UserBadgeCollection] AS
SELECT 
    u.[UserId],
    u.[FullName],
    bd.[Name] AS BadgeName,
    bd.[Description] AS BadgeDescription,
    bd.[IconPath],
    bd.[BadgeType],
    ub.[EarnedAt]
FROM [UserBadges] ub
INNER JOIN [Users] u ON ub.[UserId] = u.[UserId]
INNER JOIN [BadgeDefinitions] bd ON ub.[BadgeId] = bd.[BadgeId]
WHERE u.[IsActive] = 1 AND bd.[IsActive] = 1
GO

-- =============================================
-- SECTION 8: STORED PROCEDURES
-- =============================================

-- Calculate user statistics
CREATE PROCEDURE [CalculateUserStatistics]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @QuitStartDate DATE;
    DECLARE @CigarettesPerDay INT;
    DECLARE @PricePerPack DECIMAL(10,2);
    DECLARE @CigarettesPerPack INT;
    
    -- Get smoking profile info
    SELECT 
        @QuitStartDate = [QuitStartDate],
        @CigarettesPerDay = [CigarettesPerDay],
        @PricePerPack = [PricePerPack],
        @CigarettesPerPack = [CigarettesPerPack]
    FROM [UserProfiles]
    WHERE [UserId] = @UserId;
    
    IF @QuitStartDate IS NULL
        RETURN;
    
    DECLARE @TotalDaysQuit INT;
    DECLARE @TotalMoneySaved DECIMAL(10,2);
    DECLARE @CurrentStreak INT;
    DECLARE @LongestStreak INT;
    
    -- Calculate total days quit (days without smoking)
    SELECT @TotalDaysQuit = COUNT(*)
    FROM [DailyLogs]
    WHERE [UserId] = @UserId AND [HasSmoked] = 0;
    
    -- Calculate current streak
    WITH RecentLogs AS (
        SELECT [LogDate], [HasSmoked],
               ROW_NUMBER() OVER (ORDER BY [LogDate] DESC) as rn
        FROM [DailyLogs]
        WHERE [UserId] = @UserId
    )
    SELECT @CurrentStreak = COUNT(*)
    FROM RecentLogs
    WHERE rn <= (
        SELECT ISNULL(MIN(rn) - 1, 999999)
        FROM RecentLogs
        WHERE [HasSmoked] = 1
    );
    
    -- Calculate longest streak
    WITH StreakGroups AS (
        SELECT [LogDate], [HasSmoked],
               ROW_NUMBER() OVER (ORDER BY [LogDate]) - 
               ROW_NUMBER() OVER (PARTITION BY CASE WHEN [HasSmoked] = 0 THEN 1 ELSE 0 END ORDER BY [LogDate]) as grp
        FROM [DailyLogs]
        WHERE [UserId] = @UserId
    )
    SELECT @LongestStreak = ISNULL(MAX(streak_length), 0)
    FROM (
        SELECT COUNT(*) as streak_length
        FROM StreakGroups
        WHERE [HasSmoked] = 0
        GROUP BY grp
    ) streaks;
    
    -- Calculate money saved
    DECLARE @TotalCigarettesAvoided INT = @TotalDaysQuit * @CigarettesPerDay;
    SET @TotalMoneySaved = (@TotalCigarettesAvoided * @PricePerPack) / @CigarettesPerPack;
    
    -- Update or insert statistics
    MERGE [UserStatistics] AS target
    USING (SELECT @UserId as UserId) AS source
    ON target.[UserId] = source.UserId
    WHEN MATCHED THEN
        UPDATE SET
            [TotalDaysQuit] = @TotalDaysQuit,
            [TotalMoneySaved] = @TotalMoneySaved,
            [CurrentStreak] = @CurrentStreak,
            [LongestStreak] = @LongestStreak,
            [LastCalculatedAt] = GETUTCDATE(),
            [UpdatedAt] = GETUTCDATE()
    WHEN NOT MATCHED THEN
        INSERT ([UserId], [TotalDaysQuit], [TotalMoneySaved], [CurrentStreak], [LongestStreak], [LastCalculatedAt])
        VALUES (@UserId, @TotalDaysQuit, @TotalMoneySaved, @CurrentStreak, @LongestStreak, GETUTCDATE());
END
GO

-- Check and award badges
CREATE PROCEDURE [CheckAndAwardBadges]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get user statistics
    DECLARE @TotalDaysQuit INT, @MoneySaved DECIMAL(10,2), @CurrentStreak INT;
    
    SELECT 
        @TotalDaysQuit = [TotalDaysQuit],
        @MoneySaved = [TotalMoneySaved],
        @CurrentStreak = [CurrentStreak]
    FROM [UserStatistics]
    WHERE [UserId] = @UserId;
    
    -- Award badges based on days quit
    INSERT INTO [UserBadges] ([UserId], [BadgeId], [EarnedAt])
    SELECT @UserId, bd.[BadgeId], GETUTCDATE()
    FROM [BadgeDefinitions] bd
    WHERE bd.[BadgeType] = 'Days'
        AND bd.[IsActive] = 1
        AND bd.[RequiredValue] <= @TotalDaysQuit
        AND NOT EXISTS (
            SELECT 1 FROM [UserBadges] ub 
            WHERE ub.[UserId] = @UserId AND ub.[BadgeId] = bd.[BadgeId]
        );
    
    -- Award badges based on money saved
    INSERT INTO [UserBadges] ([UserId], [BadgeId], [EarnedAt])
    SELECT @UserId, bd.[BadgeId], GETUTCDATE()
    FROM [BadgeDefinitions] bd
    WHERE bd.[BadgeType] = 'Money'
        AND bd.[IsActive] = 1
        AND bd.[RequiredValue] <= @MoneySaved
        AND NOT EXISTS (
            SELECT 1 FROM [UserBadges] ub 
            WHERE ub.[UserId] = @UserId AND ub.[BadgeId] = bd.[BadgeId]
        );
    
    -- Award badges based on streak
    INSERT INTO [UserBadges] ([UserId], [BadgeId], [EarnedAt])
    SELECT @UserId, bd.[BadgeId], GETUTCDATE()
    FROM [BadgeDefinitions] bd
    WHERE bd.[BadgeType] = 'Streak'
        AND bd.[IsActive] = 1
        AND bd.[RequiredValue] <= @CurrentStreak
        AND NOT EXISTS (
            SELECT 1 FROM [UserBadges] ub 
            WHERE ub.[UserId] = @UserId AND ub.[BadgeId] = bd.[BadgeId]
        );
END
GO

-- =============================================
-- SECTION 9: SAMPLE DATA
-- =============================================

-- Insert sample badge definitions
INSERT INTO [BadgeDefinitions] ([Name], [Description], [IconPath], [BadgeType], [RequiredValue]) VALUES
('First Day', N'Ngày đầu tiên không hút thuốc', 'badges/first-day.png', 'Days', 1),
('3 Days Strong', N'3 ngày không hút thuốc', 'badges/3-days.png', 'Days', 3),
('One Week', N'1 tuần không hút thuốc', 'badges/one-week.png', 'Days', 7),
('Two Weeks', N'2 tuần không hút thuốc', 'badges/two-weeks.png', 'Days', 14),
('One Month', N'1 tháng không hút thuốc', 'badges/one-month.png', 'Days', 30),
('Three Months', N'3 tháng không hút thuốc', 'badges/three-months.png', 'Days', 90),
('Half Year', N'6 tháng không hút thuốc', 'badges/half-year.png', 'Days', 180),
('One Year', N'1 năm không hút thuốc', 'badges/one-year.png', 'Days', 365),
('Saver 100K', N'Tiết kiệm được 100,000 đồng', 'badges/saver-100k.png', 'Money', 100000),
('Saver 500K', N'Tiết kiệm được 500,000 đồng', 'badges/saver-500k.png', 'Money', 500000),
('Saver 1M', N'Tiết kiệm được 1,000,000 đồng', 'badges/saver-1m.png', 'Money', 1000000),
('Saver 5M', N'Tiết kiệm được 5,000,000 đồng', 'badges/saver-5m.png', 'Money', 5000000),
('Streak 5', N'Chuỗi 5 ngày liên tiếp không hút', 'badges/streak-5.png', 'Streak', 5),
('Streak 10', N'Chuỗi 10 ngày liên tiếp không hút', 'badges/streak-10.png', 'Streak', 10),
('Streak 30', N'Chuỗi 30 ngày liên tiếp không hút', 'badges/streak-30.png', 'Streak', 30);
GO

-- Insert sample motivational messages
INSERT INTO [MotivationalMessages] ([Title], [Content], [MessageType]) VALUES
(N'Chào buổi sáng!', N'Một ngày mới đã bắt đầu! Hãy tiếp tục hành trình cai thuốc của bạn.', 'Daily'),
(N'Bạn làm được!', N'Mỗi ngày không hút thuốc là một chiến thắng lớn. Hãy tự hào về bản thân!', 'Encouragement'),
(N'Sức khỏe đang cải thiện', N'Cơ thể bạn đang phục hồi mỗi ngày. Hãy tiếp tục!', 'Daily'),
(N'Tiết kiệm thành công', N'Bạn đã tiết kiệm được một khoản tiền đáng kể rồi!', 'Achievement'),
(N'Hãy suy nghĩ tích cực', N'Khi cảm thấy muốn hút thuốc, hãy nghĩ về lý do bạn bắt đầu cai.', 'Encouragement'),
(N'Gia đình bạn tự hào', N'Những người thân yêu đang ủng hộ quyết định tuyệt vời này của bạn.', 'Encouragement'),
(N'Sức khỏe là tài sản', N'Mỗi ngày không hút thuốc là một khoản đầu tư cho sức khỏe tương lai.', 'Daily'),
(N'Bạn mạnh mẽ hơn cơn thèm', N'Cơn thèm thuốc chỉ kéo dài vài phút, nhưng lợi ích cai thuốc kéo dài cả đời.', 'Encouragement'),
(N'Thành tích đáng tự hào', N'Hãy nhìn lại những gì bạn đã đạt được. Bạn thật tuyệt vời!', 'Achievement'),
(N'Hướng tới tương lai', N'Mỗi ngày không hút thuốc đưa bạn gần hơn với phiên bản tốt nhất của chính mình.', 'Daily');
GO

-- Insert sample health information
INSERT INTO [HealthInfo] ([Title], [Content], [InfoType]) VALUES
(N'Tác hại của thuốc lá', N'Thuốc lá chứa hơn 7000 hóa chất độc hại, trong đó có hơn 70 chất gây ung thư. Hút thuốc lá làm tăng nguy cơ mắc bệnh tim mạch, ung thư phổi, đột quỵ và nhiều bệnh nghiêm trọng khác.', 'SmokeEffects'),
(N'Lợi ích khi cai thuốc lá', N'Sau 20 phút: Nhịp tim và huyết áp giảm. Sau 12 giờ: Lượng CO trong máu trở về bình thường. Sau 1 năm: Nguy cơ bệnh tim giảm 50%.', 'QuitBenefits'),
(N'Cách vượt qua cơn thèm', N'Khi cảm thấy thèm thuốc: Uống nước, thở sâu, đi bộ, gọi điện cho bạn bè, hoặc làm những việc yêu thích khác để phân tán sự chú ý.', 'Tips'),
(N'Thay đổi tích cực trong cơ thể', N'Cơ thể bắt đầu phục hồi ngay khi bạn ngừng hút thuốc. Hệ tuần hoàn cải thiện, phổi làm sạch bản thân, và khả năng ngửi, nếm tăng lên.', 'QuitBenefits'),
(N'Hỗ trợ từ gia đình', N'Chia sẻ quyết định cai thuốc với gia đình và bạn bè. Sự ủng hộ từ những người thân yêu là động lực mạnh mẽ giúp bạn thành công.', 'Tips');
GO

-- Insert default admin account
INSERT INTO [Admins] ([Username], [Email], [PasswordHash], [FullName]) VALUES
('admin', 'admin@quitsmart.com', 'AQAAAAEAACcQAAAAEKxKfW1z2Q1YJ2QQ1zCQ2W3X4Y5Z6A7B8C9D0E1F2G3H4I5J6K7L8M9N0O1P2Q3R4S5T', N'Administrator');
GO

-- =============================================
-- SECTION 10: CREATE TRIGGERS
-- =============================================

-- Trigger to update UpdatedAt field when Users table is modified
CREATE TRIGGER [TR_Users_UpdatedAt]
ON [Users]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [Users]
    SET [UpdatedAt] = GETUTCDATE()
    FROM [Users] u
    INNER JOIN inserted i ON u.[UserId] = i.[UserId];
END
GO

-- Trigger to update UpdatedAt field when UserProfiles table is modified
CREATE TRIGGER [TR_UserProfiles_UpdatedAt]
ON [UserProfiles]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [UserProfiles]
    SET [UpdatedAt] = GETUTCDATE()
    FROM [UserProfiles] up
    INNER JOIN inserted i ON up.[ProfileId] = i.[ProfileId];
END
GO

-- Trigger to update UpdatedAt field when DailyLogs table is modified
CREATE TRIGGER [TR_DailyLogs_UpdatedAt]
ON [DailyLogs]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [DailyLogs]
    SET [UpdatedAt] = GETUTCDATE()
    FROM [DailyLogs] dl
    INNER JOIN inserted i ON dl.[LogId] = i.[LogId];
END
GO

-- Trigger to automatically calculate statistics and check badges when daily log is added/updated
CREATE TRIGGER [TR_DailyLogs_CalculateStats]
ON [DailyLogs]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId UNIQUEIDENTIFIER;
    
    -- Get affected user(s)
    SELECT DISTINCT @UserId = [UserId] FROM inserted;
    
    -- Calculate statistics
    EXEC [CalculateUserStatistics] @UserId;
    
    -- Check and award badges
    EXEC [CheckAndAwardBadges] @UserId;
END
GO

PRINT 'QuitSmart Desktop Database created successfully!'
PRINT 'Database includes:'
PRINT '- User management for Members and Admins'
PRINT '- Daily quit tracking system'
PRINT '- Achievement/Badge system'
PRINT '- Motivational content system'
PRINT '- Administrative functions'
PRINT '- Sample data for testing'
GO