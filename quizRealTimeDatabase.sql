CREATE DATABASE RealtimeQuizDB;
GO

-- Sử dụng Database vừa tạo
USE RealtimeQuizDB;
GO

-- 🏠 Tạo bảng Room (Phòng chơi) với RoomCode tự động tăng
CREATE TABLE Room (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    RoomCode INT IDENTITY(100000,1) NOT NULL, -- 🔥 RoomCode tự động tăng
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- 👤 Tạo bảng Users (Người chơi)
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    Username NVARCHAR(100) UNIQUE NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- 🤝 Bảng UserRoom: Người chơi tham gia phòng nào
CREATE TABLE UserRoom (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoomId UNIQUEIDENTIFIER NOT NULL,
    JoinedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_UserRoom_User FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserRoom_Room FOREIGN KEY (RoomId) REFERENCES Room(Id) ON DELETE CASCADE
);
GO

-- ❓ Tạo bảng Question (Câu hỏi)
CREATE TABLE Question (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    Content NVARCHAR(500) NOT NULL
);
GO

-- ✅ Tạo bảng Answer (Đáp án)
CREATE TABLE Answer (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    QuestionId UNIQUEIDENTIFIER NOT NULL,
    Content NVARCHAR(255) NOT NULL,
    IsCorrect BIT NOT NULL,
    CONSTRAINT FK_Answer_Question FOREIGN KEY (QuestionId) REFERENCES Question(Id) ON DELETE CASCADE
);
GO

-- 📝 Lưu câu trả lời của người chơi
CREATE TABLE UserAnswer (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    QuestionId UNIQUEIDENTIFIER NOT NULL,
    AnswerId UNIQUEIDENTIFIER NULL,  -- NULL nếu hết thời gian không trả lời
    AnsweredAt DATETIME DEFAULT GETDATE(),
    IsTimeOut BIT DEFAULT 0,  -- 1 = Hết thời gian, 0 = Đã trả lời
    CONSTRAINT FK_UserAnswer_User FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserAnswer_Question FOREIGN KEY (QuestionId) REFERENCES Question(Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserAnswer_Answer FOREIGN KEY (AnswerId) REFERENCES Answer(Id) ON DELETE NO ACTION -- 🔧 Fix lỗi multiple cascade
);
GO

-- 🏆 Tạo bảng Score (Điểm số của người chơi)
CREATE TABLE Score (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    TotalPoints INT DEFAULT 0,
    CONSTRAINT FK_Score_User FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
GO

-- 📌 **Thêm dữ liệu mẫu**
INSERT INTO Room (Id, Name) VALUES 
    (NEWID(), 'Room 1'), 
    (NEWID(), 'Room 2');
GO

INSERT INTO Users (Id, Username) VALUES 
    (NEWID(), 'Alice'), 
    (NEWID(), 'Bob'), 
    (NEWID(), 'Charlie');
GO

INSERT INTO Question (Id, Content) VALUES 
    (NEWID(), 'What is the capital of France?'),
    (NEWID(), 'What is 2 + 2?');
GO

INSERT INTO Answer (Id, QuestionId, Content, IsCorrect) VALUES 
    (NEWID(), (SELECT Id FROM Question WHERE Content = 'What is the capital of France?'), 'Paris', 1),
    (NEWID(), (SELECT Id FROM Question WHERE Content = 'What is the capital of France?'), 'London', 0),
    (NEWID(), (SELECT Id FROM Question WHERE Content = 'What is 2 + 2?'), '4', 1),
    (NEWID(), (SELECT Id FROM Question WHERE Content = 'What is 2 + 2?'), '5', 0);
GO

-- 🔍 Kiểm tra dữ liệu
SELECT * FROM Room;
GO