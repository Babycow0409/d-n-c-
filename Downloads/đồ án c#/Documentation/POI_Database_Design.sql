-- =====================================================
-- HỆ THỐNG QUẢN LÝ POI (Point of Interest) 
-- Cho App Thuyết Minh Đa Ngôn Ngữ
-- =====================================================

-- Xóa các bảng cũ (nếu tồn tại)
IF OBJECT_ID('dbo.Logs', 'U') IS NOT NULL DROP TABLE dbo.Logs;
IF OBJECT_ID('dbo.POI_Audio', 'U') IS NOT NULL DROP TABLE dbo.POI_Audio;
IF OBJECT_ID('dbo.POI_Content', 'U') IS NOT NULL DROP TABLE dbo.POI_Content;
IF OBJECT_ID('dbo.POI', 'U') IS NOT NULL DROP TABLE dbo.POI;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.Languages', 'U') IS NOT NULL DROP TABLE dbo.Languages;

-- =====================================================
-- 1. BẢNG: Languages (Ngôn Ngữ)
-- =====================================================
-- Mô tả: Lưu danh sách các ngôn ngữ được hỗ trợ
--
CREATE TABLE Languages
(
    LanguageId INT PRIMARY KEY IDENTITY(1,1),
    
    -- Tên ngôn ngữ (Tiếng Việt, English, 中文, etc)
    LanguageName NVARCHAR(100) NOT NULL UNIQUE,
    
    -- Mã ISO (vi-VN, en-US, zh-CN, etc)
    LanguageCode VARCHAR(10) NOT NULL UNIQUE,
    
    -- Trạng thái kích hoạt
    IsActive BIT NOT NULL DEFAULT 1,
    
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

-- =====================================================
-- 2. BẢNG: Users (Người Dùng)
-- =====================================================
-- Mô tả: Quản lý tài khoản (Admin, Owner, User)
--
CREATE TABLE Users
(
    UserId INT PRIMARY KEY IDENTITY(1,1),
    
    -- Thông tin cơ bản
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(MAX) NOT NULL,
    FullName NVARCHAR(255),
    
    -- Vai trò (Admin, Owner, User, Viewer)
    [Role] VARCHAR(50) NOT NULL DEFAULT 'User',
        -- Admin: Quản lý toàn bộ hệ thống
        -- Owner: Sở hữu các POI cụ thể
        -- User: Người dùng thường
        -- Viewer: Chỉ xem
    
    -- Thông tin liên hệ
    PhoneNumber VARCHAR(20),
    Address NVARCHAR(500),
    
    -- Trạng thái
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    
    -- Thời gian
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME,
    LastLoginAt DATETIME,
    
    -- Index để tìm kiếm nhanh
    INDEX IX_Username (Username),
    INDEX IX_Email (Email),
    INDEX IX_Role ([Role])
);

-- =====================================================
-- 3. BẢNG: POI (Point of Interest - Điểm Quan Tâm)
-- =====================================================
-- Mô tả: Thông tin chính về các điểm thuyết minh
--
CREATE TABLE POI
(
    POIId INT PRIMARY KEY IDENTITY(1,1),
    
    -- Thông tin cơ bản
    POIName NVARCHAR(500) NOT NULL,
    Description NVARCHAR(1000),
    
    -- Vị trí địa lý
    Latitude DECIMAL(10, 8) NOT NULL,      -- -90 đến 90
    Longitude DECIMAL(11, 8) NOT NULL,     -- -180 đến 180
    
    -- Bán kính tác động (mặc định 100m)
    RadiusMeters INT NOT NULL DEFAULT 100,
    
    -- Độ ưu tiên (1-10, cao nhất là 10)
    Priority INT NOT NULL DEFAULT 5 CHECK (Priority BETWEEN 1 AND 10),
    
    -- Chủ sở hữu POI
    OwnerId INT NOT NULL,
    FOREIGN KEY (OwnerId) REFERENCES Users(UserId) ON DELETE RESTRICT,
    
    -- Trạng thái
    IsActive BIT NOT NULL DEFAULT 1,
    IsPublished BIT NOT NULL DEFAULT 0,
    
    -- Thời gian
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME,
    PublishedAt DATETIME,
    
    -- Index để tìm kiếm nhanh
    INDEX IX_POIName (POIName),
    INDEX IX_Coordinates (Latitude, Longitude),
    INDEX IX_OwnerId (OwnerId),
    INDEX IX_IsActive (IsActive),
    INDEX IX_Priority (Priority)
);

-- =====================================================
-- 4. BẢNG: POI_Content (Nội Dung POI - Đa Ngôn Ngữ)
-- =====================================================
-- Mô tả: Nội dung thuyết minh được dịch theo từng ngôn ngữ
--
CREATE TABLE POI_Content
(
    ContentId INT PRIMARY KEY IDENTITY(1,1),
    
    -- Quan hệ với POI
    POIId INT NOT NULL,
    FOREIGN KEY (POIId) REFERENCES POI(POIId) ON DELETE CASCADE,
    
    -- Ngôn ngữ
    LanguageId INT NOT NULL,
    FOREIGN KEY (LanguageId) REFERENCES Languages(LanguageId) ON DELETE RESTRICT,
    
    -- Nội dung thuyết minh
    Title NVARCHAR(500) NOT NULL,                      -- Tiêu đề
    Description NVARCHAR(MAX) NOT NULL,                -- Mô tả chi tiết
    ShortDescription NVARCHAR(500),                    -- Mô tả ngắn (preview)
    
    -- Thông tin dịch
    TranslatedBy NVARCHAR(255),                        -- Người dịch
    TranslationStatus VARCHAR(50) DEFAULT 'Draft',     -- Draft, Pending, Approved, Rejected
    
    -- Thời gian
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME,
    ApprovedAt DATETIME,
    
    -- Ràng buộc: Một POI chỉ có một nội dung trên một ngôn ngữ
    CONSTRAINT UC_POI_Language UNIQUE(POIId, LanguageId),
    
    -- Index
    INDEX IX_POIId (POIId),
    INDEX IX_LanguageId (LanguageId),
    INDEX IX_TranslationStatus (TranslationStatus)
);

-- =====================================================
-- 5. BẢNG: POI_Audio (Audio/Âm Thanh Thuyết Minh)
-- =====================================================
-- Mô tả: Lưu URL, metadata của file audio cho từng ngôn ngữ
--
CREATE TABLE POI_Audio
(
    AudioId INT PRIMARY KEY IDENTITY(1,1),
    
    -- Quan hệ với POI_Content
    ContentId INT NOT NULL,
    FOREIGN KEY (ContentId) REFERENCES POI_Content(ContentId) ON DELETE CASCADE,
    
    -- URL/Path của file audio
    AudioUrl VARCHAR(MAX) NOT NULL,                 -- https://storage.example.com/audio/poi_1_vi.mp3
    
    -- Thông tin audio
    DurationSeconds INT,                             -- Độ dài (giây)
    FileSizeKB INT,                                  -- Kích thước file (KB)
    FileFormat VARCHAR(20) NOT NULL DEFAULT 'mp3',  -- mp3, wav, aac, etc
    
    -- Chất lượng (128kbps, 192kbps, 320kbps)
    Bitrate INT,
    
    -- Người phát âm
    NarratorName NVARCHAR(255),
    NarratorId INT,
    FOREIGN KEY (NarratorId) REFERENCES Users(UserId) ON DELETE SET NULL,
    
    -- Trạng thái
    IsApproved BIT NOT NULL DEFAULT 0,
    IsPublished BIT NOT NULL DEFAULT 0,
    
    -- Thời gian
    UploadedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    ApprovedAt DATETIME,
    PublishedAt DATETIME,
    
    -- Index
    INDEX IX_ContentId (ContentId),
    INDEX IX_NarratorId (NarratorId),
    INDEX IX_IsPublished (IsPublished)
);

-- =====================================================
-- 6. BẢNG: Logs (Lịch Sử Nghe - Analytics)
-- =====================================================
-- Mô tả: Ghi nhật ký mỗi lần người dùng nghe audio/xem POI
--
CREATE TABLE Logs
(
    LogId BIGINT PRIMARY KEY IDENTITY(1,1),
    
    -- Người dùng
    UserId INT,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE SET NULL,
    
    -- POI được truy cập
    POIId INT NOT NULL,
    FOREIGN KEY (POIId) REFERENCES POI(POIId) ON DELETE CASCADE,
    
    -- Audio được nghe (nếu có)
    AudioId INT,
    FOREIGN KEY (AudioId) REFERENCES POI_Audio(AudioId) ON DELETE SET NULL,
    
    -- Ngôn ngữ được sử dụng
    LanguageId INT,
    FOREIGN KEY (LanguageId) REFERENCES Languages(LanguageId) ON DELETE SET NULL,
    
    -- Loại hành động
    ActionType VARCHAR(50) NOT NULL,
        -- view_poi: Xem chi tiết POI
        -- listen_audio: Nghe audio
        -- search: Tìm kiếm
        -- download: Tải về
    
    -- Thông tin thiết bị
    DeviceType VARCHAR(50),                 -- Mobile, Web, Desktop
    AppVersion VARCHAR(20),                 -- 1.0.0, 1.1.0, etc
    
    -- Thông tin vị trí (nếu có)
    UserLatitude DECIMAL(10, 8),
    UserLongitude DECIMAL(11, 8),
    DistanceMeters INT,                     -- Khoảng cách từ POI tới user
    
    -- Thời lượng tương tác (nếu nghe audio)
    DurationSeconds INT,                    -- Bao lâu nghe (giây)
    CompletionPercentage INT,               -- % hoàn thành (0-100)
    
    -- Thêm dữ liệu khác
    CustomData VARCHAR(MAX),                -- JSON data nếu cần
    
    -- Thời gian
    LoggedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    
    -- Index cho query nhanh
    INDEX IX_UserId (UserId),
    INDEX IX_POIId (POIId),
    INDEX IX_ActionType (ActionType),
    INDEX IX_LoggedAt (LoggedAt),
    INDEX IX_LoggedAt_POIId (LoggedAt, POIId)
);

-- =====================================================
-- INSERT DỮ LIỆU MẪU
-- =====================================================

-- Thêm ngôn ngữ
INSERT INTO Languages (LanguageName, LanguageCode) VALUES
('Tiếng Việt', 'vi-VN'),
('English', 'en-US'),
('中文', 'zh-CN'),
('日本語', 'ja-JP');

-- Thêm người dùng
INSERT INTO Users (Username, Email, PasswordHash, FullName, [Role]) VALUES
('admin', 'admin@example.com', '$2b$10$...', 'Administrator', 'Admin'),
('owner1', 'owner1@example.com', '$2b$10$...', 'Trần Văn A', 'Owner'),
('user1', 'user1@example.com', '$2b$10$...', 'Nguyễn Thị B', 'User');

-- Thêm POI (Điểm quan tâm)
INSERT INTO POI (POIName, Description, Latitude, Longitude, RadiusMeters, Priority, OwnerId) VALUES
('Nhà Thờ Lớn Hà Nội', 'Nhà thờ Chính tòa khu vực Hà Nội', 21.0285, 105.8542, 150, 10, 2),
('Lăng Chủ Tịch Hồ Chí Minh', 'Quảng trường Ba Đình', 21.0519, 105.8482, 200, 10, 2),
('Thạc Bạch Tower', 'Tháp nổi tiếng Hà Nội', 21.0654, 105.8165, 100, 8, 2);

-- Thêm nội dung POI (Đa ngôn ngữ)
INSERT INTO POI_Content (POIId, LanguageId, Title, Description, TranslatedBy, TranslationStatus) VALUES
(1, 1, 'Nhà Thờ Lớn', 'Nhà thờ Chính tòa được xây dựng từ năm 1886...', 'User1', 'Approved'),
(1, 2, 'Hanoi Cathedral', 'The cathedral was built from 1886...', 'User2', 'Approved'),
(2, 1, 'Lăng Chủ Tịch', 'Lăng được xây dựng từ 1973...', 'User1', 'Approved'),
(2, 2, 'Ho Chi Minh Mausoleum', 'The mausoleum was built in 1973...', 'User2', 'Approved');

-- Thêm audio
INSERT INTO POI_Audio (ContentId, AudioUrl, DurationSeconds, FileSizeKB, FileFormat, Bitrate, NarratorName, NarratorId, IsPublished) VALUES
(1, 'https://storage.example.com/audio/cathedral_vi.mp3', 120, 2048, 'mp3', 192, 'Phương Anh', 2, 1),
(2, 'https://storage.example.com/audio/cathedral_en.mp3', 115, 1950, 'mp3', 192, 'John Smith', 3, 1),
(3, 'https://storage.example.com/audio/mausoleum_vi.mp3', 180, 3072, 'mp3', 192, 'Phương Anh', 2, 1),
(4, 'https://storage.example.com/audio/mausoleum_en.mp3', 175, 2980, 'mp3', 192, 'John Smith', 3, 1);

-- Thêm logs (lịch sử nghe)
INSERT INTO Logs (UserId, POIId, AudioId, LanguageId, ActionType, DeviceType, AppVersion, DurationSeconds, CompletionPercentage) VALUES
(3, 1, 1, 1, 'listen_audio', 'Mobile', '1.0.0', 120, 100),
(3, 1, 1, 1, 'view_poi', 'Mobile', '1.0.0', NULL, NULL),
(3, 2, 3, 1, 'listen_audio', 'Mobile', '1.0.0', 180, 85),
(NULL, 1, 2, 2, 'listen_audio', 'Web', '1.0.0', 115, 100);

-- =====================================================
-- VIEW HỮU DỤNG
-- =====================================================

-- View: POI kèm thông tin chủ sở hữu
CREATE VIEW vw_POI_Detail AS
SELECT 
    p.POIId,
    p.POIName,
    p.Description,
    p.Latitude,
    p.Longitude,
    p.RadiusMeters,
    p.Priority,
    u.Username AS OwnerName,
    u.FullName AS OwnerFullName,
    p.IsActive,
    p.CreatedAt
FROM POI p
INNER JOIN Users u ON p.OwnerId = u.UserId;

-- View: POI kèm nội dung (tất cả ngôn ngữ)
CREATE VIEW vw_POI_Content_Detail AS
SELECT 
    p.POIId,
    p.POIName,
    pc.ContentId,
    pc.Title,
    pc.Description,
    l.LanguageName,
    l.LanguageCode,
    pc.TranslationStatus,
    pc.CreatedAt
FROM POI p
INNER JOIN POI_Content pc ON p.POIId = pc.POIId
INNER JOIN Languages l ON pc.LanguageId = l.LanguageId;

-- View: POI kèm audio
CREATE VIEW vw_POI_Audio_Detail AS
SELECT 
    p.POIId,
    p.POIName,
    pa.AudioId,
    pa.AudioUrl,
    pa.DurationSeconds,
    pa.FileFormat,
    l.LanguageName,
    u.FullName AS NarratorName,
    pa.IsPublished,
    pa.UploadedAt
FROM POI p
INNER JOIN POI_Content pc ON p.POIId = pc.POIId
INNER JOIN POI_Audio pa ON pc.ContentId = pa.ContentId
INNER JOIN Languages l ON pc.LanguageId = l.LanguageId
LEFT JOIN Users u ON pa.NarratorId = u.UserId;

-- =====================================================
-- STORED PROCEDURE: Thống kê lưu lượng
-- =====================================================

CREATE PROCEDURE sp_GetPOI_Analytics
    @POIId INT,
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SET @StartDate = ISNULL(@StartDate, DATEADD(MONTH, -1, GETUTCDATE()));
    SET @EndDate = ISNULL(@EndDate, GETUTCDATE());
    
    SELECT 
        p.POIId,
        p.POIName,
        COUNT(*) AS TotalViews,
        COUNT(DISTINCT l.UserId) AS UniqueUsers,
        COUNT(CASE WHEN l.ActionType = 'listen_audio' THEN 1 END) AS AudioPlays,
        AVG(CAST(l.CompletionPercentage AS FLOAT)) AS AvgCompletion,
        CAST(COUNT(*) AS FLOAT) * 100 / (SELECT COUNT(*) FROM Logs) AS ViewPercentage
    FROM POI p
    LEFT JOIN Logs l ON p.POIId = l.POIId
    WHERE p.POIId = @POIId
        AND l.LoggedAt BETWEEN @StartDate AND @EndDate
    GROUP BY p.POIId, p.POIName;
END;

-- =====================================================
-- STORED PROCEDURE: Lấy POI gần người dùng
-- =====================================================

CREATE PROCEDURE sp_GetNearby_POIs
    @UserLatitude DECIMAL(10,8),
    @UserLongitude DECIMAL(11,8),
    @MaxDistanceMeters INT = 500,
    @LanguageId INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP 20
        p.POIId,
        p.POIName,
        p.Latitude,
        p.Longitude,
        p.RadiusMeters,
        p.Priority,
        pc.Title,
        pc.Description,
        -- Tính khoảng cách (công thức Haversine)
        CAST(
            6371000 * ACOS(
                COS(RADIANS(90 - @UserLatitude)) * COS(RADIANS(90 - p.Latitude)) +
                SIN(RADIANS(90 - @UserLatitude)) * SIN(RADIANS(90 - p.Latitude)) * 
                COS(RADIANS(@UserLongitude - p.Longitude))
            ) AS INT
        ) AS DistanceMeters
    FROM POI p
    LEFT JOIN POI_Content pc ON p.POIId = pc.POIId AND pc.LanguageId = @LanguageId
    WHERE p.IsActive = 1 AND p.IsPublished = 1
    ORDER BY DistanceMeters ASC;
END;

PRINT '✅ Database POI đã được tạo thành công!';
