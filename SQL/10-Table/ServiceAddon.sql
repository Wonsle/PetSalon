CREATE TABLE [dbo].[ServiceAddon](
	[ServiceAddonID] [bigint] IDENTITY(1,1) NOT NULL,
	[AddonName] [nvarchar](100) NOT NULL,
	[AddonType] [varchar](100) NOT NULL, -- Links to SystemCode for addon types
	[AddonPrice] [money] NOT NULL,
	[Description] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL DEFAULT(1),
	[Sort] [int] NOT NULL DEFAULT(0),
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateTime] [datetime] NOT NULL DEFAULT(GETDATE()),
	[ModifyUser] [nvarchar](20) NOT NULL,
	[ModifyTime] [datetime] NOT NULL DEFAULT(GETDATE()),
 CONSTRAINT [PK_ServiceAddon] PRIMARY KEY CLUSTERED 
(
	[ServiceAddonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO