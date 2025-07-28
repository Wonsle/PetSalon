CREATE TABLE [dbo].[ReservationService](
	[ReservationServiceID] [bigint] IDENTITY(1,1) NOT NULL,
	[ReserveRecordID] [bigint] NOT NULL,
	[ServiceID] [bigint] NOT NULL,
	[ServicePrice] [money] NOT NULL,
	[Duration] [int] NOT NULL,
	[Notes] [nvarchar](500) NULL,
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateTime] [datetime] NOT NULL DEFAULT(GETDATE()),
	[ModifyUser] [nvarchar](20) NOT NULL,
	[ModifyTime] [datetime] NOT NULL DEFAULT(GETDATE()),
 CONSTRAINT [PK_ReservationService] PRIMARY KEY CLUSTERED 
(
	[ReservationServiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [FK_ReservationService_ReserveRecord] FOREIGN KEY([ReserveRecordID])
REFERENCES [dbo].[ReserveRecord] ([ReserveRecordID]),
 CONSTRAINT [FK_ReservationService_Service] FOREIGN KEY([ServiceID])
REFERENCES [dbo].[Service] ([ServiceID])
) ON [PRIMARY]
GO