/*
付款紀錄
*/
CREATE TABLE [dbo].[PaymentRecord] (
    [PaymentRecordID] INT           IDENTITY (1, 1) NOT NULL,
    [PaymentCode]     INT           NULL,
    [PetID]           INT           NULL,
    [ReceivablePrice] MONEY         NULL,
    [ActualPrice]     MONEY         NULL,
    [CreateUser]      NVARCHAR (20) NOT NULL,
    [CreateTime]      DATETIME      DEFAULT (getdate()) NOT NULL,
    [ModifyUser]      NVARCHAR (20) NOT NULL,
    [ModifyTime]      DATETIME      DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([PaymentRecordID] ASC),
    CONSTRAINT [FK_PaymentRecord_Pet] FOREIGN KEY ([PetID]) REFERENCES [dbo].[Pet] ([PetID])
);


