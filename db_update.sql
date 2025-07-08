-- Update BadgeDefinitions with icon names instead of emojis
-- This avoids encoding issues with emojis in SQL Server

UPDATE [BadgeDefinitions] SET [IconPath] = 'first-day' WHERE [Name] = 'First Day';
UPDATE [BadgeDefinitions] SET [IconPath] = '3-days' WHERE [Name] = '3 Days Strong';
UPDATE [BadgeDefinitions] SET [IconPath] = 'one-week' WHERE [Name] = 'One Week';
UPDATE [BadgeDefinitions] SET [IconPath] = 'two-weeks' WHERE [Name] = 'Two Weeks';
UPDATE [BadgeDefinitions] SET [IconPath] = 'one-month' WHERE [Name] = 'One Month';
UPDATE [BadgeDefinitions] SET [IconPath] = 'three-months' WHERE [Name] = 'Three Months';
UPDATE [BadgeDefinitions] SET [IconPath] = 'half-year' WHERE [Name] = 'Half Year';
UPDATE [BadgeDefinitions] SET [IconPath] = 'one-year' WHERE [Name] = 'One Year';
UPDATE [BadgeDefinitions] SET [IconPath] = 'saver-100k' WHERE [Name] = 'Saver 100K';
UPDATE [BadgeDefinitions] SET [IconPath] = 'saver-500k' WHERE [Name] = 'Saver 500K';
UPDATE [BadgeDefinitions] SET [IconPath] = 'saver-1m' WHERE [Name] = 'Saver 1M';
UPDATE [BadgeDefinitions] SET [IconPath] = 'saver-5m' WHERE [Name] = 'Saver 5M';
UPDATE [BadgeDefinitions] SET [IconPath] = 'streak-5' WHERE [Name] = 'Streak 5';
UPDATE [BadgeDefinitions] SET [IconPath] = 'streak-10' WHERE [Name] = 'Streak 10';
UPDATE [BadgeDefinitions] SET [IconPath] = 'streak-30' WHERE [Name] = 'Streak 30';

-- Check if the updates worked
SELECT [Name], [IconPath], [Description] FROM [BadgeDefinitions] ORDER BY [Name]; 