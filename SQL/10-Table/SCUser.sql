CREATE TABLE SCUser
(
    SCUserID BIGINT PRIMARY KEY IDENTITY(1,1),
    UserName NVARCHAR(20),
    PasswordHash VARCHAR(200),
    LastLogin DATETIME,
    [CreateUser]      NVARCHAR (20)  NOT NULL,
    [CreateTime]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifyUser]      NVARCHAR (20)  NOT NULL,
    [ModifyTime]      DATETIME       DEFAULT (getdate()) NOT NULL,
)