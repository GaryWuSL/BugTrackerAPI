using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using BugTrackerAPI.Models;

namespace BugTrackerAPI.Services
{
    public interface IFollowupService
    {
        List<FollowupView> GetFollowupViews(int TicketId);

        ServiceResponse New(Followup followup);

        ServiceResponse Set(Followup followup);

        ServiceResponse Del(int followupid);
    }

    public class FollowupService : IFollowupService
    {
        private const string _constr = "BugTrackerAppCon";
        private const string _dbname = "DBName";
        private const string _errortemplate = "Please fill in required field(s): {0}";
        private const string _strgetsql = @"SELECT TicketId, FollowupId, Title, FullDescription, 
                    tblFollowup.PriorityId, tblFollowup.StatusId, AssignUserId, CreateUserId, UpdateUserId,
                    convert(varchar(10),CreateDate,103)+' '+convert(varchar(10),CreateDate,108) AS CreateDate,
                    convert(varchar(10),UpdateDate,103)+' '+convert(varchar(10),UpdateDate,108) AS UpdateDate
                    FROM dbo.tblFollowup WITH (NOLOCK) WHERE tblFollowup.FollowupId = @FollowupId";
        private const string _strgetviewsql = @"SELECT tblFollowup.FollowupId, tblFollowup.TicketId, tblFollowup.Title, tblFollowup.FullDescription, 
                    ISNULL(tblPriority.PriorityName,'') AS PriorityName, ISNULL(tblStatus.StatusName,'') AS StatusName, 
                    ISNULL(tblUser.UserName,'') AS AssignUserName, tblFollowup.PriorityId, tblFollowup.StatusId, 
                    tblFollowup.AssignUserId, tblFollowup.CreateUserId, tblFollowup.UpdateUserId, UpdateUser.UserName AS UpdateUserName,
                    convert(varchar(10),tblFollowup.CreateDate,103)+' '+convert(varchar(10),tblFollowup.CreateDate,108) AS CreateDate,
                    convert(varchar(10),tblFollowup.UpdateDate,103)+' '+convert(varchar(10),tblFollowup.UpdateDate,108) AS UpdateDate
                    FROM dbo.tblFollowup WITH (NOLOCK)
                    LEFT JOIN dbo.tblUser WITH (NOLOCK)
                    ON tblFollowup.AssignUserId = tblUser.UserId
                    LEFT JOIN dbo.tblPriority WITH (NOLOCK)
                    ON tblFollowup.PriorityId = tblPriority.PriorityId
                    LEFT JOIN dbo.tblStatus WITH (NOLOCK)
                    ON tblFollowup.StatusId = tblStatus.StatusId
                    LEFT JOIN (SELECT * FROM tblUser WITH (NOLOCK)) UpdateUser
					ON tblFollowup.UpdateUserId = UpdateUser.UserId
                    WHERE tblFollowup.TicketId = @TicketId
                    ORDER BY tblFollowup.UpdateDate DESC";
        private const string _strinssql = @"
                    BEGIN TRY
                        BEGIN TRANSACTION

                        INSERT INTO dbo.tblFollowup
                        (TicketId, Title, FullDescription, PriorityId, StatusId, AssignUserId, 
                        CreateUserId, UpdateUserId, CreateDate, UpdateDate )
                        VALUES 
                        (@TicketId, @Title, @FullDescription, @PriorityId, @StatusId, @AssignUserId, 
                        @CreateUserId, @UpdateUserId, GETDATE(), GETDATE());

	                    UPDATE tblTicket
	                    SET PriorityId = @PriorityId, 
	                    StatusId = @StatusId, 
	                    AssignUserId = @AssignUserId, 
	                    UpdateUserId = @UpdateUserId,
	                    UpdateDate = GETDATE()
	                    WHERE TicketId = @TicketId;

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
        private const string _strupdsql = @"
                    BEGIN TRY
                        BEGIN TRANSACTION
                            UPDATE dbo.tblFollowup
                            SET Title = @Title,
                            FullDescription = @FullDescription,  
                            PriorityId = @PriorityId, 
                            StatusId = @StatusId, 
                            AssignUserId = @AssignUserId, 
                            UpdateUserId = @UpdateUserId,
                            UpdateDate = GETDATE()
                            WHERE FollowupId = @FollowupId;

	                        DECLARE @TicketId INT

	                        SELECT @TicketId = TicketId 
	                        FROM dbo.tblFollowup WITH (NOLOCK)
	                        WHERE FollowupId = @FollowupId;

	                        UPDATE dbo.tblTicket
	                        SET PriorityId = @PriorityId, 
	                        StatusId = @StatusId, 
	                        AssignUserId = @AssignUserId, 
	                        UpdateUserId = @UpdateUserId,
	                        UpdateDate = GETDATE()
	                        FROM dbo.tblTicket
	                        WHERE TicketId = @TicketId;
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
        private const string _strdelsql = @"DELETE dbo.tblFollowup WHERE FollowupId = @FollowupId";
        private const string _errormessage = "System error occurs and please try again later";

        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public FollowupService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _config = configuration;
            _env = env;
        }
        public List<FollowupView> GetFollowupViews(int _ticketid)
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            List<FollowupView> _lstfollowupview = new List<FollowupView>();
            DataTable dtFollowup = new DataTable();
            SqlDataReader sqlReader;

            using (SqlConnection sqlCon = new SqlConnection(sqlDataSource))
            {
                sqlCon.Open();
                using (SqlCommand sqlCommand = new SqlCommand(_strgetviewsql, sqlCon))
                {
                    sqlCommand.Parameters.AddWithValue("@TicketId", _ticketid);
                    sqlReader = sqlCommand.ExecuteReader();
                    dtFollowup.Load(sqlReader);
                    foreach (DataRow _drFollowup in dtFollowup.Rows)
                    {
                        _lstfollowupview.Add(new FollowupView()
                        {
                            TicketId = int.Parse(_drFollowup["TicketId"].ToString()),
                            FollowupId = int.Parse(_drFollowup["FollowupId"].ToString()),
                            Title = _drFollowup["Title"].ToString(),
                            FullDescription = _drFollowup["FullDescription"].ToString(),
                            PriorityName = _drFollowup["PriorityName"].ToString(),
                            StatusName = _drFollowup["StatusName"].ToString(),
                            AssignUserName = _drFollowup["AssignUserName"].ToString(),
                            UpdateUserName = _drFollowup["UpdateUserName"].ToString(),
                            PriorityId = int.Parse(_drFollowup["PriorityId"].ToString()),
                            StatusId = int.Parse(_drFollowup["StatusId"].ToString()),
                            AssignUserId = int.Parse(_drFollowup["AssignUserId"].ToString()),
                            CreateUserId = int.Parse(_drFollowup["CreateUserId"].ToString()),
                            UpdateUserId = int.Parse(_drFollowup["UpdateUserId"].ToString()),
                            CreateDate = _drFollowup["CreateDate"].ToString(),
                            UpdateDate = _drFollowup["UpdateDate"].ToString()
                        });
                    }
                    sqlReader.Close();

                }
                sqlCon.Close();
            }
            return _lstfollowupview;
        }

        public ServiceResponse New(Followup _followup)
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            ServiceResponse _serviceresponse = new ServiceResponse();

            try
            {
                string _err = string.Empty;
                //..... Server side checking before any update
                if (string.IsNullOrWhiteSpace(_followup.Title))
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Title";
                }
                if (string.IsNullOrWhiteSpace(_followup.FullDescription))
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Follow Up Details";
                }
                if (_followup.StatusId == 0)
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Status";
                }
                if (_followup.PriorityId == 0)
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Priority";
                }
                if (_followup.AssignUserId == 0)
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Assign To";
                }
                if (_err.Length > 0)
                {
                    _serviceresponse.Success = false;
                    _serviceresponse.Error = string.Format(_errortemplate, _err);
                }
                else
                {
                    using (SqlConnection sqlCon = new SqlConnection(sqlDataSource))
                    {
                        sqlCon.Open();
                        using (SqlCommand sqlCommand = new SqlCommand(_strinssql, sqlCon))
                        {
                            sqlCommand.Parameters.AddWithValue("@TicketId", _followup.TicketId);
                            sqlCommand.Parameters.AddWithValue("@Title", (string.IsNullOrWhiteSpace(_followup.Title) ? string.Empty : _followup.Title));
                            sqlCommand.Parameters.AddWithValue("@FullDescription", (string.IsNullOrWhiteSpace(_followup.FullDescription) ? string.Empty : _followup.FullDescription));
                            sqlCommand.Parameters.AddWithValue("@PriorityId", _followup.PriorityId);
                            sqlCommand.Parameters.AddWithValue("@StatusId", _followup.StatusId);
                            sqlCommand.Parameters.AddWithValue("@AssignUserId", _followup.AssignUserId);
                            sqlCommand.Parameters.AddWithValue("@CreateUserId", _followup.CreateUserId);
                            sqlCommand.Parameters.AddWithValue("@UpdateUserId", _followup.UpdateUserId);
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
                        sqlCon.Close();
                    }
                }
            }
            catch (Exception _ex)
            {
                _serviceresponse.RecordCount = 0;
                _serviceresponse.Success = false;
                _serviceresponse.Error = _ex.Message.ToString();
            }
            return _serviceresponse;
        }

        public ServiceResponse Set(Followup _followup)
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            ServiceResponse _serviceresponse = new ServiceResponse();

            try
            {
                string _err = string.Empty;
                //..... Server side checking before any update
                if (string.IsNullOrWhiteSpace(_followup.Title))
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Title";
                }
                if (string.IsNullOrWhiteSpace(_followup.FullDescription))
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Follow Up Details";
                }
                if (_followup.StatusId == 0)
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Status";
                }
                if (_followup.PriorityId == 0)
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Priority";
                }
                if (_followup.AssignUserId == 0)
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Assign To";
                }
                if (_err.Length > 0)
                {
                    _serviceresponse.Success = false;
                    _serviceresponse.Error = string.Format(_errortemplate, _err);
                }
                else
                {
                    using (SqlConnection sqlCon = new SqlConnection(sqlDataSource))
                    {
                        sqlCon.Open();
                        using (SqlCommand sqlCommand = new SqlCommand(_strupdsql, sqlCon))
                        {
                            sqlCommand.Parameters.AddWithValue("@Title", (string.IsNullOrWhiteSpace(_followup.Title) ? string.Empty : _followup.Title));
                            sqlCommand.Parameters.AddWithValue("@FullDescription", (string.IsNullOrWhiteSpace(_followup.FullDescription) ? string.Empty : _followup.FullDescription));
                            sqlCommand.Parameters.AddWithValue("@PriorityId", _followup.PriorityId);
                            sqlCommand.Parameters.AddWithValue("@StatusId", _followup.StatusId);
                            sqlCommand.Parameters.AddWithValue("@AssignUserId", _followup.AssignUserId);
                            sqlCommand.Parameters.AddWithValue("@UpdateUserId", _followup.UpdateUserId);
                            sqlCommand.Parameters.AddWithValue("@FollowupId", _followup.FollowupId);
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
                        sqlCon.Close();
                    }
                }
            }
            catch (Exception _ex)
            {
                _serviceresponse.RecordCount = 0;
                _serviceresponse.Success = false;
                _serviceresponse.Error = _ex.Message.ToString();
            }
            return _serviceresponse;
        }

        public ServiceResponse Del(int _followupid)
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            ServiceResponse _serviceresponse = new ServiceResponse();

            try
            {
                if (_followupid > 0)
                {
                    _serviceresponse.Success = false;
                    _serviceresponse.Error = _errormessage;
                }
                else
                {
                    using (SqlConnection sqlCon = new SqlConnection(sqlDataSource))
                    {
                        sqlCon.Open();
                        using (SqlCommand sqlCommand = new SqlCommand(_strdelsql, sqlCon))
                        {
                            sqlCommand.Parameters.AddWithValue("@FollowupId", _followupid);
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
                        sqlCon.Close();
                    }
                }
            }
            catch (Exception _ex)
            {
                _serviceresponse.RecordCount = 0;
                _serviceresponse.Success = false;
                _serviceresponse.Error = _ex.Message.ToString();
            }
            return _serviceresponse;
        }

    }
}