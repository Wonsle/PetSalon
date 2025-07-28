CREATE TABLE [dbo].[ReservationAddon](
	[ReservationAddonID] [bigint] IDENTITY(1,1) NOT NULL,
	[ReserveRecordID] [bigint] NOT NULL,
	[ServiceAddonID] [bigint] NOT NULL,
	[AddonPrice] [money] NOT NULL,
	[Notes] [nvarchar](500) NULL,
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateTime] [datetime] NOT NULL DEFAULT(GETDATE()),
	[ModifyUser] [nvarchar](20) NOT NULL,
	[ModifyTime] [datetime] NOT NULL DEFAULT(GETDATE()),
 CONSTRAINT [PK_ReservationAddon] PRIMARY KEY CLUSTERED 
(
	[ReservationAddonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [FK_ReservationAddon_ReserveRecord] FOREIGN KEY([ReserveRecordID])
REFERENCES [dbo].[ReserveRecord] ([ReserveRecordID]),
 CONSTRAINT [FK_ReservationAddon_ServiceAddon] FOREIGN KEY([ServiceAddonID])
REFERENCES [dbo].[ServiceAddon] ([ServiceAddonID])
) ON [PRIMARY]
GO