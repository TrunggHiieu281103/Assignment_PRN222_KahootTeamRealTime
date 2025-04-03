-- Tạo cơ sở dữ liệu
CREATE DATABASE RealtimeQuizDB;
GO

USE RealtimeQuizDB;
GO

-- 🏠 Phòng chơi
CREATE TABLE Room (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    RoomCode INT IDENTITY(100000,1) NOT NULL UNIQUE, -- Mã phòng để người chơi tham gia
    CreatedAt DATETIME DEFAULT GETDATE(),
    isActive BIT NOT NULL DEFAULT 1 -- Cột mới thêm vào
);
GO

-- 👤 Người chơi
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    Username NVARCHAR(100) UNIQUE NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- 🤝 Người chơi tham gia phòng
CREATE TABLE UserRoom (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoomId UNIQUEIDENTIFIER NOT NULL,
    JoinedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_UserRoom_User FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserRoom_Room FOREIGN KEY (RoomId) REFERENCES Room(Id) ON DELETE CASCADE
);
GO

-- ❓ Câu hỏi
CREATE TABLE Question (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    Content NVARCHAR(500) NOT NULL
);
GO

-- ✅ Đáp án
CREATE TABLE Answer (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    QuestionId UNIQUEIDENTIFIER NOT NULL,
    Content NVARCHAR(255) NOT NULL,
    IsCorrect BIT NOT NULL,
    CONSTRAINT FK_Answer_Question FOREIGN KEY (QuestionId) REFERENCES Question(Id) ON DELETE CASCADE
);
GO

-- 📌 Gán câu hỏi cho từng phòng
CREATE TABLE RoomQuestion (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    RoomId UNIQUEIDENTIFIER NOT NULL,
    QuestionId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT FK_RoomQuestion_Room FOREIGN KEY (RoomId) REFERENCES Room(Id) ON DELETE CASCADE,
    CONSTRAINT FK_RoomQuestion_Question FOREIGN KEY (QuestionId) REFERENCES Question(Id) ON DELETE CASCADE
);
GO

-- 📝 Câu trả lời của người chơi trong từng phòng
CREATE TABLE UserAnswer (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoomId UNIQUEIDENTIFIER NOT NULL,
    QuestionId UNIQUEIDENTIFIER NOT NULL,
    AnswerId UNIQUEIDENTIFIER NULL,  -- NULL nếu hết thời gian không trả lời
    AnsweredAt DATETIME DEFAULT GETDATE(),
    IsTimeOut BIT DEFAULT 0,  -- 1 = Hết thời gian, 0 = Đã trả lời
    CONSTRAINT FK_UserAnswer_User FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserAnswer_Room FOREIGN KEY (RoomId) REFERENCES Room(Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserAnswer_Question FOREIGN KEY (QuestionId) REFERENCES Question(Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserAnswer_Answer FOREIGN KEY (AnswerId) REFERENCES Answer(Id) ON DELETE NO ACTION
);
GO

-- 🏆 Điểm số của người chơi trong từng phòng
CREATE TABLE Score (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoomId UNIQUEIDENTIFIER NOT NULL,
    TotalPoints INT DEFAULT 0, -- Tổng số câu trả lời đúng
    CONSTRAINT FK_Score_User FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Score_Room FOREIGN KEY (RoomId) REFERENCES Room(Id) ON DELETE CASCADE
);
GO

-- 🏠 Thêm phòng chơi
INSERT INTO Room (Id, Name) VALUES 
    (NEWID(), 'Room A'), 
    (NEWID(), 'Room B');
GO

-- 👤 Thêm người chơi
INSERT INTO Users (Id, Username) VALUES 
    (NEWID(), 'Alice'), 
    (NEWID(), 'Bob'), 
    (NEWID(), 'Charlie');
GO

-- ❓ Thêm câu hỏi
INSERT INTO Question (Id, Content) VALUES 
    (NEWID(), 'What is the capital of France?'),
    (NEWID(), 'What is 2 + 2?');
GO

-- ✅ Thêm đáp án
INSERT INTO Answer (Id, QuestionId, Content, IsCorrect)
SELECT NEWID(), Id, 'Paris', 1 FROM Question WHERE Content = 'What is the capital of France?'
UNION ALL
SELECT NEWID(), Id, 'London', 0 FROM Question WHERE Content = 'What is the capital of France?'
UNION ALL
SELECT NEWID(), Id, '4', 1 FROM Question WHERE Content = 'What is 2 + 2?'
UNION ALL
SELECT NEWID(), Id, '5', 0 FROM Question WHERE Content = 'What is 2 + 2?';
GO

-- 📌 Gán câu hỏi vào phòng A
INSERT INTO RoomQuestion (Id, RoomId, QuestionId)
SELECT NEWID(), (SELECT Id FROM Room WHERE Name = 'Room A'), Id FROM Question;
GO

-- 🤝 Người chơi tham gia phòng
INSERT INTO UserRoom (Id, UserId, RoomId)
SELECT NEWID(), (SELECT Id FROM Users WHERE Username = 'Alice'), (SELECT Id FROM Room WHERE Name = 'Room A')
UNION ALL
SELECT NEWID(), (SELECT Id FROM Users WHERE Username = 'Bob'), (SELECT Id FROM Room WHERE Name = 'Room A');
GO

-- Cách tính điểm
SELECT 
    ua.UserId, 
    ua.RoomId, 
    COUNT(a.Id) AS TotalPoints
FROM UserAnswer ua
JOIN Answer a ON ua.AnswerId = a.Id
WHERE a.IsCorrect = 1
GROUP BY ua.UserId, ua.RoomId;
GO

--  cập nhật vào bảng Score
UPDATE Score
SET TotalPoints = (
    SELECT COUNT(a.Id) 
    FROM UserAnswer ua
    JOIN Answer a ON ua.AnswerId = a.Id
    WHERE ua.UserId = Score.UserId AND ua.RoomId = Score.RoomId AND a.IsCorrect = 1
);
GO

-- Tạo bảng Role và Administrator
CREATE TABLE [RealtimeQuizDB].[dbo].[Role] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [RoleName] NVARCHAR(50) NOT NULL,
    [isActive] BIT NOT NULL DEFAULT 1
);

CREATE TABLE [RealtimeQuizDB].[dbo].[Administrator] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UserName] NVARCHAR(50) NOT NULL,
    [Password] NVARCHAR(256) NOT NULL,
    [Email] NVARCHAR(255) NULL,
    [FullName] NVARCHAR(255) NULL,
    [PhoneNumber] NVARCHAR(20) NULL,
    [DateOfBirth] DATE NULL,
    [IsActive] BIT DEFAULT 1,
    [RoleId] INT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Administrator_Role FOREIGN KEY ([RoleId]) REFERENCES [RealtimeQuizDB].[dbo].[Role]([Id])
);

-- Thêm dữ liệu Role
INSERT INTO Role (RoleName, IsActive) VALUES ('Admin', 1);
INSERT INTO Role (RoleName, IsActive) VALUES ('Manager', 1);

-- Thêm dữ liệu Administrator
INSERT INTO Administrator (UserName, Password, Email, FullName, PhoneNumber, DateOfBirth, IsActive, RoleId)
VALUES 
    ('admin', 'admin123', 'admin@example.com', 'Admin User', '0987654321', '1985-01-01', 1, 1),
    ('manager', 'manager123', 'manager@example.com', 'Manager User', '0976543210', '1990-05-15', 1, 2),
    ('mod1', 'mod123', 'mod1@example.com', 'Moderator One', '0965123456', '1992-07-20', 1, 2),
    ('mod2', 'mod456', 'mod2@example.com', 'Moderator Two', '0956789012', '1995-09-10', 1, 2);
GO
