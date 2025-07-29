/*
付款紀錄
*/
CREATE TABLE [dbo].[PaymentRecord] (
    [PaymentRecordID] BIGINT           IDENTITY (1, 1) NOT NULL,
    [PaymentCode]     INT           NULL,
    [PetID]           BIGINT           NULL,
    [ReceivablePrice] MONEY         NULL,
    [ActualPrice]     MONEY         NULL,
    [CreateUser]      NVARCHAR (20) NOT NULL,
    [CreateTime]      DATETIME      DEFAULT (getdate()) NOT NULL,
    [ModifyUser]      NVARCHAR (20) NOT NULL,
    [ModifyTime]      DATETIME      DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([PaymentRecordID] ASC),
    CONSTRAINT [FK_PaymentRecord_Pet] FOREIGN KEY ([PetID]) REFERENCES [dbo].[Pet] ([PetID])
);

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款記錄管理 - 存放客戶付款記錄，包含應收金額、實收金額等財務資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款記錄唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'PaymentRecordID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付款代碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'PaymentCode';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物ID，關聯至Pet表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'PetID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'應收金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'ReceivablePrice';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'實收金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'ActualPrice';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'CreateUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'CreateTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'ModifyUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PaymentRecord', @level2type = N'COLUMN', @level2name = N'ModifyTime';

