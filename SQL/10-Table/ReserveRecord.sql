/*
預約紀錄
*/
CREATE TABLE [dbo].[ReserveRecord] (
    [ReserveRecordID] BIGINT            IDENTITY (1, 1) NOT NULL,
    [PetID]           BIGINT            NOT NULL,
    [SubscriptionID]  BIGINT            NULL,
    [ReserverDate ]   DATE           NOT NULL,
    [ReserverTime ]   TIME (7)       NOT NULL,
    [Memo]            NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [CreateUser]      NVARCHAR (20)  NOT NULL,
    [CreateTime]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [ModifyUser]      NVARCHAR (20)  NOT NULL,
    [ModifyTime]      DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([ReserveRecordID] ASC),
    CONSTRAINT [FK_ReserveRecord_Pet] FOREIGN KEY ([SubscriptionID]) REFERENCES [dbo].[Subscription] ([SubscriptionID])
);




