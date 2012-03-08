USE [UnitTests]
GO
/****** Object:  Table [dbo].[TestRun]    Script Date: 08/04/2008 09:59:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestRun](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[date] [datetime] NOT NULL CONSTRAINT [DF_TestRun_date]  DEFAULT (getdate()),
	[name] [nvarchar](100) NULL,
 CONSTRAINT [PK_TestRun] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TestRunData]    Script Date: 08/04/2008 09:59:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestRunData](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[run_id] [int] NOT NULL,
	[cs_id] [int] NULL,
	[name] [nvarchar](100) NOT NULL,
	[starttime] [datetime] NOT NULL,
	[stoptime] [datetime] NULL,
	[text] [nvarchar](max) NULL,
	[ticks] [bigint] NULL,
 CONSTRAINT [PK_TestRunData] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TestLimits]    Script Date: 08/04/2008 09:59:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TestLimits](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[test_name] [nvarchar](100) NOT NULL,
	[ticks] [bigint] NOT NULL,
	[type] [varchar](5) NOT NULL,
 CONSTRAINT [PK_TestLimits] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ConnectionStrings]    Script Date: 08/04/2008 09:59:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConnectionStrings](
	[ID] [int] NOT NULL,
	[ConnectionString] [nvarchar](500) NULL,
	[Type] [nvarchar](50) NOT NULL,
	[IsValid] [bit] NOT NULL,
 CONSTRAINT [PK_ConnectionStrings] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[InitTestRun]    Script Date: 08/04/2008 09:59:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Alexander Aberer
-- Create date: 2008-08-01
-- Description:	Creates a new test run entry and returns the id.
-- =============================================
CREATE PROCEDURE [dbo].[InitTestRun] 
	-- Add the parameters for the stored procedure here
	@testname nvarchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO [TestRun]
           ([date]
           ,[name])
     VALUES
           (GETDATE()
           ,@testname)

    RETURN SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[StartStopWatch]    Script Date: 08/04/2008 09:59:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Alexander Aberer
-- Create date: 2008-08-01
-- Description:	Create a start entry for a test.
-- =============================================
CREATE PROCEDURE [dbo].[StartStopWatch] 
	-- Add the parameters for the stored procedure here
	@runid int, 
	@csid int, 
	@testname nvarchar(100), 
	@time datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [UnitTests].[dbo].[TestRunData]
           ([run_id]
           ,[cs_id]
           ,[name]
           ,[starttime]
           ,[text])
     VALUES
           (@runid
           ,@csid
           ,@testname
           ,@time
           ,NULL)
    RETURN SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[StopStopWatch]    Script Date: 08/04/2008 09:59:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Alexander Aberer
-- Create date: 2008-08-01
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[StopStopWatch] 
	-- Add the parameters for the stored procedure here
	@runid int, 
	@csid int, 
	@testname nvarchar(100), 
	@time datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @id int

    -- Insert statements for procedure here
    SELECT TOP 1 @id = [id] FROM [TestRunData]
      WHERE [run_id] = @runid AND [cs_id] = @csid AND [name] = @testname
	UPDATE [TestRunData]
	  SET [stoptime] = @time
	  WHERE [id] = @id

    RETURN @id
END
GO
/****** Object:  StoredProcedure [dbo].[CheckLimits]    Script Date: 08/04/2008 09:59:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Alexander Aberer
-- Create date: 2008-08-01
-- Description:	Checks the time limits for a test.
-- =============================================
CREATE PROCEDURE [dbo].[CheckLimits] 
	-- Add the parameters for the stored procedure here
	@runid int, 
	@csid int, 
	@testname nvarchar(100), 
	@ticks bigint 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE [TestRunData]
	   SET [ticks] = @ticks
	 WHERE [run_id] = @runid AND [cs_id] = @csid AND [name] = @testname

	SELECT [ticks]
      FROM [TestLimits]
        WHERE [test_name] = @testname AND (
            ([ticks] > @ticks AND LOWER([type]) = 'lower')
            OR ([ticks] < @ticks AND LOWER([type]) = 'upper')
          )
END
GO
/****** Object:  StoredProcedure [dbo].[CleanupTests]    Script Date: 08/04/2008 09:59:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Alexander Aberer
-- Create date: 2008-08-01
-- Description:	Cleanup unsuccessfull test logs.
-- =============================================
CREATE PROCEDURE [dbo].[CleanupTests] 
	@runid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM [TestRunData]
      WHERE [stoptime] IS NULL AND [run_id] = @runid
END
GO
/****** Object:  Check [CK_TestLimits_Type]    Script Date: 08/04/2008 09:59:24 ******/
ALTER TABLE [dbo].[TestLimits]  WITH CHECK ADD  CONSTRAINT [CK_TestLimits_Type] CHECK  ((lower([type])='upper' OR lower([type])='lower'))
GO
ALTER TABLE [dbo].[TestLimits] CHECK CONSTRAINT [CK_TestLimits_Type]
GO
