CREATE TABLE [dbo].[PetServicePrice](
	[PetServicePriceID] [bigint] IDENTITY(1,1) NOT NULL,
	[PetID] [bigint] NOT NULL,
	[ServiceID] [bigint] NOT NULL,
	[CustomPrice] [money] NULL, -- Override price for specific pet
	[Duration] [int] NULL, -- Override duration for specific pet
	[Notes] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL DEFAULT(1),
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateTime] [datetime] NOT NULL DEFAULT(GETDATE()),
	[ModifyUser] [nvarchar](20) NOT NULL,
	[ModifyTime] [datetime] NOT NULL DEFAULT(GETDATE()),
 CONSTRAINT [PK_PetServicePrice] PRIMARY KEY CLUSTERED 
(
	[PetServicePriceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [FK_PetServicePrice_Pet] FOREIGN KEY([PetID])
REFERENCES [dbo].[Pet] ([PetID]),
 CONSTRAINT [FK_PetServicePrice_Service] FOREIGN KEY([ServiceID])
REFERENCES [dbo].[Service] ([ServiceID])
) ON [PRIMARY]
GO

-- Create unique index to prevent duplicate pet-service combinations
CREATE UNIQUE NONCLUSTERED INDEX [IX_PetServicePrice_Pet_Service] ON [dbo].[PetServicePrice]
(
	[PetID] ASC,
	[ServiceID] ASC
)
WHERE ([IsActive]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO