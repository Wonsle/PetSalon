CREATE TABLE [dbo].[Service](
	[ServiceID] [bigint] IDENTITY(1,1) NOT NULL,
	[ServiceName] [nvarchar](100) NOT NULL,
	[ServiceType] [varchar](100) NOT NULL, -- Links to SystemCode
	[BasePrice] [money] NOT NULL,
	[Duration] [int] NOT NULL, -- Duration in minutes
	[Description] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL DEFAULT(1),
	[Sort] [int] NOT NULL DEFAULT(0),
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateTime] [datetime] NOT NULL DEFAULT(GETDATE()),
	[ModifyUser] [nvarchar](20) NOT NULL,
	[ModifyTime] [datetime] NOT NULL DEFAULT(GETDATE()),
 CONSTRAINT [PK_Service] PRIMARY KEY CLUSTERED 
(
	[ServiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO