/*
訂閱(包月)
*/
CREATE TABLE [dbo].[Subscription] (
    [SubscriptionID]   INT           IDENTITY (1, 1) NOT NULL,
    [PetID]            INT           NOT NULL,
    [SubscriptionDate] DATE          NOT NULL,
    [StartDate]        DATE          NOT NULL,
    [EndDate]          DATE          NOT NULL,
    [CreateUser]       NVARCHAR (20) NOT NULL,
    [CreateTime]       DATETIME      DEFAULT (getdate()) NOT NULL,
    [ModifyUser]       NVARCHAR (20) NOT NULL,
    [ModifyTime]       DATETIME      DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([SubscriptionID] ASC),
    CONSTRAINT [FK_Subscription_Pet] FOREIGN KEY ([PetID]) REFERENCES [dbo].[Pet] ([PetID])
);


