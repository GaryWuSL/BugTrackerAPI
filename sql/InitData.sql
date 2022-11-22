USE [master]
GO

DROP DATABASE IF EXISTS [BugTrackerDB];

/****** Object:  Database [BugTrackerDB]    Script Date: 11/21/2022 8:49:50 PM ******/
CREATE DATABASE [BugTrackerDB]
 CONTAINMENT = NONE
-- ON  PRIMARY 
--( NAME = N'BugTrackerDB', FILENAME = N'D:\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\BugTrackerDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 --LOG ON 
-- ( NAME = N'BugTrackerDB_log', FILENAME = N'D:\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\BugTrackerDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO

USE [BugTrackerDB]
GO

/****** Object:  Table [dbo].[tblUser]    Script Date: 11/21/2022 8:32:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblUser](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
	[UserDescription] [varchar](100) NULL,
	[EmailAddress] [varchar](100) NULL,
	[PhotoFilePath] [varchar](500) NULL,
 CONSTRAINT [PK_tblUser] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
GO

INSERT INTO [dbo].[tblUser] ([UserName],[Password],[UserDescription],[EmailAddress],[PhotoFilePath])
VALUES ('Admin',N'Admin!123',N'System Admin',N'admin@bugtracker.co.uk','http://localhost:12964/api/Attachment?filename=1.jpg');

INSERT INTO [dbo].[tblUser] ([UserName],[Password],[UserDescription],[EmailAddress],[PhotoFilePath])
VALUES ('RebeccaPat',N'Aa123456',N'Data Entry user',N'rebecca.pat@bugtracker.co.uk','http://localhost:12964/api/Attachment?filename=2.jpg');

INSERT INTO [dbo].[tblUser] ([UserName],[Password],[UserDescription],[EmailAddress],[PhotoFilePath])
VALUES ('DanArmstrong',N'Cc888999',N'Database Administrator',N'dan.armstrong@bugtracker.co.uk','http://localhost:12964/api/Attachment?filename=3.jpg');				

INSERT INTO [dbo].[tblUser] ([UserName],[Password],[UserDescription],[EmailAddress],[PhotoFilePath])
VALUES ('EdwardJones',N'Cc888999',N'Finance user',N'edward.jones@bugtracker.co.uk','http://localhost:12964/api/Attachment?filename=4.jpg');				

INSERT INTO [dbo].[tblUser] ([UserName],[Password],[UserDescription],[EmailAddress],[PhotoFilePath])
VALUES ('GiilianNewey',N'Aa000999',N'System Testing Officer',N'giilian.neweyg@bugtracker.co.uk','http://localhost:12964/api/Attachment?filename=5.jpg');				

INSERT INTO [dbo].[tblUser] ([UserName],[Password],[UserDescription],[EmailAddress],[PhotoFilePath])
VALUES ('AndrewBowell',N'Pp123456',N'System Support',N'andrew.bowell@bugtracker.co.uk','http://localhost:12964/api/Attachment?filename=6.jpg');				

/****** Object:  Table [dbo].[tblStatus]    Script Date: 11/21/2022 8:33:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblStatus](
	[StatusId] [int] IDENTITY(1,1) NOT NULL,
	[StatusName] [varchar](50) NULL,
 CONSTRAINT [PK_tblStatus] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[tblStatus] ([StatusName]) VALUES ('New');
INSERT INTO [dbo].[tblStatus] ([StatusName]) VALUES ('Assigned');
INSERT INTO [dbo].[tblStatus] ([StatusName]) VALUES ('In Progress');
INSERT INTO [dbo].[tblStatus] ([StatusName]) VALUES ('Cancelled');
INSERT INTO [dbo].[tblStatus] ([StatusName]) VALUES ('Closed');
INSERT INTO [dbo].[tblStatus] ([StatusName]) VALUES ('Reopen');

/****** Object:  Table [dbo].[tblPriority]    Script Date: 11/21/2022 8:34:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblPriority](
	[PriorityId] [int] IDENTITY(1,1) NOT NULL,
	[PriorityName] [varchar](50) NULL,
 CONSTRAINT [PK_tblPriority] PRIMARY KEY CLUSTERED 
(
	[PriorityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[tblPriority] ([PriorityName]) VALUES ('Low');
INSERT INTO [dbo].[tblPriority] ([PriorityName]) VALUES ('Medium');
INSERT INTO [dbo].[tblPriority] ([PriorityName]) VALUES ('High');

/****** Object:  Table [dbo].[tblTicket]    Script Date: 11/21/2022 8:34:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblTicket](
	[TicketId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](50) NULL,
	[FullDescription] [varchar](max) NULL,
	[Organization] [varchar](200) NULL,
	[Project] [varchar](100) NULL,
	[AppModule] [varchar](100) NULL,
	[AppVersion] [varchar](50) NULL,
	[ImageFilePath] [varchar](500) NULL,
	[PriorityId] [int] NULL,
	[StatusId] [int] NULL,
	[AssignUserId] [int] NULL,
	[CreateUserId] [int] NULL,
	[UpdateUserId] [int] NULL,
	[CreateDate] [datetime] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_tblTicket] PRIMARY KEY CLUSTERED 
(
	[TicketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

INSERT INTO [dbo].[tblTicket]
([Title],[FullDescription],[Organization],[Project],[AppModule],[AppVersion],[ImageFilePath],
[PriorityId],[StatusId],[AssignUserId],[CreateUserId],[UpdateUserId],[CreateDate],[UpdateDate])
VALUES ('Blue screen comes out','SYSTEM_SERVICE_EXCEPTION Blue Screen in Windows','Bug Tracker Co., Ltd','Desktop O/S','Desktop Operating System','Windows 10','http://localhost:12964/api/Attachment?filename=error1.jpg',2,1,5,1,5,'2022-11-16 12:16:36.683','2022-11-18 18:01:49.923');

INSERT INTO [dbo].[tblTicket]
([Title],[FullDescription],[Organization],[Project],[AppModule],[AppVersion],[ImageFilePath],
[PriorityId],[StatusId],[AssignUserId],[CreateUserId],[UpdateUserId],[CreateDate],[UpdateDate])
VALUES ('Access Denied','Open user folder in Drive X:','Bug Tracker Co., Ltd','File Manager','Desktop Operating System','Windows 11','http://localhost:12964/api/Attachment?filename=error2.jpg',2,3,6,1,5,'2022-11-16 12:31:34.527','2022-11-21 18:54:01.560');

INSERT INTO [dbo].[tblTicket]
([Title],[FullDescription],[Organization],[Project],[AppModule],[AppVersion],[ImageFilePath],
[PriorityId],[StatusId],[AssignUserId],[CreateUserId],[UpdateUserId],[CreateDate],[UpdateDate])
VALUES ('Invalid checking','System improperly checking for the optional fields','Bug Tracker Co., Ltd','Online Store','Customer Registration','2.3','http://localhost:12964/api/Attachment?filename=error3.jpg',1,3,2,1,5,'2022-11-16 16:32:30.640','2022-11-17 20:26:15.963');

INSERT INTO [dbo].[tblTicket]
([Title],[FullDescription],[Organization],[Project],[AppModule],[AppVersion],[ImageFilePath],
[PriorityId],[StatusId],[AssignUserId],[CreateUserId],[UpdateUserId],[CreateDate],[UpdateDate])
VALUES ('File size error','Upload submission file size error. Ssytem only allows file size smaller than 2MB','Bug Tracker Co., Ltd','Membership Application','License file upload','1.4','http://localhost:12964/api/Attachment?filename=error4.jpg',2,2,2,1,5,'2022-11-16 18:23:51.700','2022-11-17 20:20:15.940');

INSERT INTO [dbo].[tblTicket]
([Title],[FullDescription],[Organization],[Project],[AppModule],[AppVersion],[ImageFilePath],
[PriorityId],[StatusId],[AssignUserId],[CreateUserId],[UpdateUserId],[CreateDate],[UpdateDate])
VALUES ('Error when export to PDF file','Popup error when export report to PDF file','Bug Tracker Co., Ltd','Order Processing','Export order to PDF file','3.1','http://localhost:12964/api/Attachment?filename=error5.jpg',1,3,6,1,5,'2022-11-17 19:05:09.773','2022-11-17 20:22:01.267')

INSERT INTO [dbo].[tblTicket]
([Title],[FullDescription],[Organization],[Project],[AppModule],[AppVersion],[ImageFilePath],
[PriorityId],[StatusId],[AssignUserId],[CreateUserId],[UpdateUserId],[CreateDate],[UpdateDate])
VALUES ('Database connection error','Perform stress test and return database errors please check the attached screen shot.','Bug Tracker Co., Ltd','CRM System','Customer Profile','3.5','http://localhost:12964/api/Attachment?filename=error6.jpg',1,2,3,1,6,'2022-11-18 21:31:59.457','2022-11-18 23:00:34.227');
GO

/****** Object:  Table [dbo].[tblFollowup]    Script Date: 11/21/2022 8:44:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblFollowup](
	[FollowupId] [int] IDENTITY(1,1) NOT NULL,
	[TicketId] [int] NULL,
	[Title] [varchar](50) NULL,
	[FullDescription] [varchar](max) NULL,
	[PriorityId] [int] NULL,
	[StatusId] [int] NULL,
	[AssignUserId] [int] NULL,
	[CreateUserId] [int] NULL,
	[UpdateUserId] [int] NULL,
	[CreateDate] [datetime] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_tblFollowup] PRIMARY KEY CLUSTERED 
(
	[FollowupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

INSERT INTO [dbo].[tblFollowup]
([TicketId],[Title],[FullDescription],[PriorityId],[StatusId],[AssignUserId],[CreateUserId],[UpdateUserId],[CreateDate],[UpdateDate])
VALUES (6,'RE:Database connection error','Database error and deadlock occurs Reassign to DBA to perform detail checking Please send email to user once completed',2,3,5,5,5,'2022-11-18 21:57:46.903','2022-11-18 22:49:24.863')

INSERT INTO [dbo].[tblFollowup]
([TicketId],[Title],[FullDescription],[PriorityId],[StatusId],[AssignUserId],[CreateUserId],[UpdateUserId],[CreateDate],[UpdateDate])
VALUES (6,'RE:Database connection error','Check database log',1,2,3,6,6,'2022-11-18 23:00:34.227','2022-11-18 23:00:34.227')







