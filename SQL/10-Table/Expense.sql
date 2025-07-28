CREATE TABLE [dbo].[Expense](
	[ExpenseID] [bigint] IDENTITY(1,1) NOT NULL,
	[ExpenseType] [varchar](100) NOT NULL, -- Links to SystemCode
	[Amount] [money] NOT NULL,
	[ExpenseDate] [date] NOT NULL,
	[Description] [nvarchar](500) NULL,
	[Receipt] [nvarchar](255) NULL, -- Path to receipt image/document
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateTime] [datetime] NOT NULL DEFAULT(GETDATE()),
	[ModifyUser] [nvarchar](20) NOT NULL,
	[ModifyTime] [datetime] NOT NULL DEFAULT(GETDATE()),
 CONSTRAINT [PK_Expense] PRIMARY KEY CLUSTERED 
(
	[ExpenseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO