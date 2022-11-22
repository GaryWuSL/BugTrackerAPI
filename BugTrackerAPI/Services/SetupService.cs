using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using BugTrackerAPI.Models;

namespace BugTrackerAPI.Services
{
    public interface ISetupService
    {
		ServiceResponse CreateDB();

		ServiceResponse ImportData();

		string DBName();
	}

    public class SetupService : ISetupService
    {
        private const string _constr = "BugTrackerAppCon";
		private const string _dbname = "DBName";
		private const string _conmasstr = "BugTrackerMasCon";
		private const string _strcdbsql = @"DROP DATABASE IF EXISTS [{0}];
                    CREATE DATABASE [{1}] CONTAINMENT = NONE WITH CATALOG_COLLATION = DATABASE_DEFAULT;
					SELECT name, database_id, create_date FROM sys.databases WHERE name = '{2}';";
        private const string _strimpsql = @"
			BEGIN TRY
				BEGIN TRANSACTION
					SET ANSI_NULLS ON;
					SET QUOTED_IDENTIFIER ON;

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

					CREATE TABLE [dbo].[tblStatus](
						[StatusId] [int] IDENTITY(1,1) NOT NULL,
						[StatusName] [varchar](50) NULL,
					 CONSTRAINT [PK_tblStatus] PRIMARY KEY CLUSTERED 
					(
						[StatusId] ASC
					)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
					) ON [PRIMARY];

					INSERT INTO [dbo].[tblStatus] ([StatusName]) VALUES ('New');
					INSERT INTO [dbo].[tblStatus] ([StatusName]) VALUES ('Assigned');
					INSERT INTO [dbo].[tblStatus] ([StatusName]) VALUES ('In Progress');
					INSERT INTO [dbo].[tblStatus] ([StatusName]) VALUES ('Cancelled');
					INSERT INTO [dbo].[tblStatus] ([StatusName]) VALUES ('Closed');
					INSERT INTO [dbo].[tblStatus] ([StatusName]) VALUES ('Reopen');

					CREATE TABLE [dbo].[tblPriority](
						[PriorityId] [int] IDENTITY(1,1) NOT NULL,
						[PriorityName] [varchar](50) NULL,
					 CONSTRAINT [PK_tblPriority] PRIMARY KEY CLUSTERED 
					(
						[PriorityId] ASC
					)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
					) ON [PRIMARY];

					INSERT INTO [dbo].[tblPriority] ([PriorityName]) VALUES ('Low');
					INSERT INTO [dbo].[tblPriority] ([PriorityName]) VALUES ('Medium');
					INSERT INTO [dbo].[tblPriority] ([PriorityName]) VALUES ('High');

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
					) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

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
					) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

					INSERT INTO [dbo].[tblFollowup]
					([TicketId],[Title],[FullDescription],[PriorityId],[StatusId],[AssignUserId],[CreateUserId],[UpdateUserId],[CreateDate],[UpdateDate])
					VALUES (6,'RE:Database connection error','Database error and deadlock occurs Reassign to DBA to perform detail checking Please send email to user once completed',2,3,5,5,5,'2022-11-18 21:57:46.903','2022-11-18 22:49:24.863');

					INSERT INTO [dbo].[tblFollowup]
					([TicketId],[Title],[FullDescription],[PriorityId],[StatusId],[AssignUserId],[CreateUserId],[UpdateUserId],[CreateDate],[UpdateDate])
					VALUES (6,'RE:Database connection error','Check database log',1,2,3,6,6,'2022-11-18 23:00:34.227','2022-11-18 23:00:34.227');

				COMMIT TRANSACTION
			END TRY
			BEGIN CATCH
				DECLARE 
					@ErrorMessage NVARCHAR(4000),
					@ErrorSeverity INT,
					@ErrorState INT;
				SELECT 
					@ErrorMessage = ERROR_MESSAGE(),
					@ErrorSeverity = ERROR_SEVERITY(),
					@ErrorState = ERROR_STATE();
				RAISERROR (
					@ErrorMessage,
					@ErrorSeverity,
					@ErrorState    
					);
				ROLLBACK TRANSACTION
			END CATCH";
        private const string _errormessage = "System error occurs and please try again later";
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public SetupService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _config = configuration;
            _env = env;
        }

        public ServiceResponse CreateDB()
        {
			string dbname = _config[_dbname];
			string sqlDataSource = _config.GetConnectionString(_conmasstr);
			ServiceResponse _serviceresponse = new ServiceResponse();

            using (SqlConnection sqlCon = new SqlConnection(sqlDataSource))
            {
                sqlCon.Open();
                using (SqlCommand sqlCommand = new SqlCommand(string.Format(_strcdbsql,dbname,dbname,dbname), sqlCon))
                {
					try
					{
						sqlCommand.ExecuteNonQuery();
						_serviceresponse.Success = true;
						_serviceresponse.Error = string.Empty;
					} 
					catch (Exception _ex)
					{
						_serviceresponse.Success = false;
						_serviceresponse.Error = _ex.Message;
						_serviceresponse.RecordCount = 0;
					}
                }
                sqlCon.Close();
            }

            return _serviceresponse;
        }

		public ServiceResponse ImportData()
		{
			string dbname = _config[_dbname];
			string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
			ServiceResponse _serviceresponse = new ServiceResponse();

			using (SqlConnection sqlCon = new SqlConnection(sqlDataSource))
			{
				sqlCon.Open();
				using (SqlCommand sqlCommand = new SqlCommand(_strimpsql, sqlCon))
				{
					try
					{
						_serviceresponse.RecordCount = sqlCommand.ExecuteNonQuery();
						if (_serviceresponse.RecordCount > 0)
						{
							_serviceresponse.Success = true;
							_serviceresponse.Error = string.Empty;
						}
						else
						{
							_serviceresponse.Success = false;
							_serviceresponse.Error = _errormessage;
						}
					}
					catch (Exception _ex)
					{
						_serviceresponse.RecordCount = 0;
						_serviceresponse.Success = false;
						_serviceresponse.Error = _ex.InnerException.Message;
					}
				}
				sqlCon.Close();
			}

			return _serviceresponse;
		}

		public string DBName()
        {
			return _config[_dbname];
		}
	}


}
