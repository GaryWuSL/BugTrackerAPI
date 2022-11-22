using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using BugTrackerAPI.Models;

namespace BugTrackerAPI.Services
{
    public interface ITicketService
    {
        List<TicketView> GetTicketViews(Ticket ticket);

        List<TicketSummaryByUser> GetTicketSummarybyUser();

        TicketView GetTicketView(int ticketid);

        ServiceResponse New(Ticket ticket);

        ServiceResponse Set(Ticket ticket);

        ServiceResponse Del(int ticketid);
    }

    public class TicketService : ITicketService
    {
        private const string _constr = "BugTrackerAppCon";
        private const string _dbname = "DBName";
        private const string _errortemplate = "Please fill in required field(s): {0}";
        private const string _strgetsql = @"SELECT tblTicket.TicketId, tblTicket.Title, tblTicket.FullDescription, tblTicket.Organization, Project, 
                    tblTicket.AppModule, tblTicket.AppVersion, ISNULL(tblPriority.PriorityName,'') AS PriorityName, 
                    ISNULL(tblStatus.StatusName,'') AS StatusName, tblTicket.ImageFilePath, 
                    ISNULL(tblUser.UserName,'') AS AssignUserName, tblTicket.PriorityId, tblTicket.StatusId, tblTicket.AssignUserId,
                    tblTicket.CreateUserId, tblTicket.UpdateUserId, UpdateUser.UserName AS UpdateUserName,
                    convert(varchar(10),tblTicket.CreateDate,103)+' '+convert(varchar(10),tblTicket.CreateDate,108) AS CreateDate,
                    convert(varchar(10),tblTicket.UpdateDate,103)+' '+convert(varchar(10),tblTicket.UpdateDate,108) AS UpdateDate
                    FROM dbo.tblTicket WITH (NOLOCK)
                    LEFT JOIN dbo.tblUser WITH (NOLOCK)
                    ON tblTicket.AssignUserId = tblUser.UserId
                    LEFT JOIN dbo.tblPriority WITH (NOLOCK)
                    ON tblTicket.PriorityId = tblPriority.PriorityId
                    LEFT JOIN dbo.tblStatus WITH (NOLOCK)
                    ON tblTicket.StatusId = tblStatus.StatusId
                    LEFT JOIN (SELECT * FROM tblUser WITH (NOLOCK)) UpdateUser
					ON tblTicket.UpdateUserId = UpdateUser.UserId";
        private const string _strinssql = @"INSERT INTO dbo.tblTicket 
                    (Title, FullDescription, Organization, Project, AppModule, 
                    AppVersion, ImageFilePath, PriorityId, StatusId, AssignUserId, 
                    CreateUserId, UpdateUserId, CreateDate, UpdateDate )
                    VALUES 
                    (@Title, @FullDescription, @Organization, @Project, @AppModule, 
                    @AppVersion, @ImageFilePath, @PriorityId, @StatusId, @AssignUserId, 
                    @CreateUserId, @UpdateUserId, GETDATE(), GETDATE());";
        private const string _strupdsql = @"UPDATE dbo.tblTicket
                    SET Title = @Title,
                    FullDescription = @FullDescription,  
                    Organization = @Organization, 
                    Project = @Project, 
                    AppModule = @AppModule, 
                    AppVersion = @AppVersion, 
                    ImageFilePath = @ImageFilePath,  
                    PriorityId = @PriorityId, 
                    StatusId = @StatusId, 
                    AssignUserId = @AssignUserId, 
                    UpdateUserId = @UpdateUserId,
                    UpdateDate = GETDATE()
                    WHERE TicketId = @TicketId;";
        private const string _strdelsql = @"DELETE dbo.tblTicket WHERE TicketId = @TicketId;";
        private const string _strsumsql = @"SELECT TicketSummary.*, tblUser.UserDescription, tblUser.PhotoFilePath
                    FROM tblUser  WITH (NOLOCK) INNER JOIN 
                    (Select tblTicket.AssignUserId, tblTicket.StatusId, tblUser.UserName AS AssignUserName, tblStatus.StatusName, Count(tblTicket.AssignUserId) AS TicketCount
                    FROM tblTicket WITH (NOLOCK)
                    INNER JOIN tblUser WITH (NOLOCK)
                    ON tblTicket.AssignUserId = tblUser.UserId
                    INNER JOIN tblStatus  WITH (NOLOCK)
                    ON tblTicket.StatusId = tblStatus.StatusId
                    GROUP BY tblTicket.AssignUserId, tblUser.UserName, tblTicket.StatusId, tblStatus.StatusName ) TicketSummary
                    ON tblUser.UserId = TicketSummary.AssignUserId
                    ORDER BY tblUser.UserName, TicketSummary.StatusId";
        private const string _errormessage = "System error occurs and please try again later;";
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public TicketService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _config = configuration;
            _env = env;
        }

        public List<TicketView> GetTicketViews(Ticket _ticket)
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            List<TicketView> _lstticketlist = new List<TicketView>();
            string _strsql = string.Empty;
            DataTable dtTicket = new DataTable();
            SqlDataReader sqlReader;

            if (_ticket.TicketId > 0)
            {
                _strsql = (string.IsNullOrWhiteSpace(_strsql) ? " WHERE " : " AND ") +
                    string.Format(" tblTicket.TicketId = {0};", _ticket.TicketId.ToString());
            }
            if (!(string.IsNullOrWhiteSpace(_strsql)))
            {
                _strsql = _strgetsql + _strsql;
            } else
            {
                _strsql = _strgetsql;
            }
            using (SqlConnection sqlCon = new SqlConnection(sqlDataSource))
            {
                sqlCon.Open();
                using (SqlCommand sqlCommand = new SqlCommand(_strsql, sqlCon))
                {
                    sqlReader = sqlCommand.ExecuteReader();
                    dtTicket.Load(sqlReader);
                    foreach (DataRow _drTicket in dtTicket.Rows)
                    {
                        _lstticketlist.Add(new TicketView()
                        {
                            TicketId = int.Parse(_drTicket["TicketId"].ToString()),
                            Title = _drTicket["Title"].ToString(),
                            FullDescription = _drTicket["FullDescription"].ToString(),
                            Organization = _drTicket["Organization"].ToString(),
                            Project = _drTicket["Project"].ToString(),
                            AppModule = _drTicket["AppModule"].ToString(),
                            AppVersion = _drTicket["AppVersion"].ToString(),
                            PriorityName = _drTicket["PriorityName"].ToString(),
                            StatusName = _drTicket["StatusName"].ToString(),
                            ImageFilePath = _drTicket["ImageFilePath"].ToString(),
                            AssignUserName = _drTicket["AssignUserName"].ToString(),
                            UpdateUserName = _drTicket["UpdateUserName"].ToString(),
                            PriorityId = int.Parse(_drTicket["PriorityId"].ToString()),
                            StatusId = int.Parse(_drTicket["StatusId"].ToString()),
                            AssignUserId = int.Parse(_drTicket["AssignUserId"].ToString()),
                            CreateUserId = int.Parse(_drTicket["CreateUserId"].ToString()),
                            UpdateUserId = int.Parse(_drTicket["UpdateUserId"].ToString()),
                            CreateDate = _drTicket["CreateDate"].ToString(),
                            UpdateDate = _drTicket["UpdateDate"].ToString()
                        });
                    }
                    sqlReader.Close();

                }
                sqlCon.Close();
            }
            return _lstticketlist;
        }

        public List<TicketSummaryByUser> GetTicketSummarybyUser()
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            List<TicketSummaryByUser> _lstticketsumbyuser = new List<TicketSummaryByUser>();
            string _strsql = string.Empty;
            DataTable dtTicket = new DataTable();
            SqlDataReader sqlReader;

            using (SqlConnection sqlCon = new SqlConnection(sqlDataSource))
            {
                sqlCon.Open();
                using (SqlCommand sqlCommand = new SqlCommand(_strsumsql, sqlCon))
                {
                    sqlReader = sqlCommand.ExecuteReader();
                    dtTicket.Load(sqlReader);
                    foreach (DataRow _drTicket in dtTicket.Rows)
                    {
                        _lstticketsumbyuser.Add(new TicketSummaryByUser()
                        {
                            StatusId = int.Parse(_drTicket["StatusId"].ToString()),
                            AssignUserName = _drTicket["AssignUserName"].ToString(),
                            StatusName = _drTicket["StatusName"].ToString(),
                            TicketCount = int.Parse(_drTicket["TicketCount"].ToString()),
                            UserDescription = _drTicket["UserDescription"].ToString(),
                            PhotoFilePath = _drTicket["PhotoFilePath"].ToString(),
                        });
                    }
                    sqlReader.Close();

                }
                sqlCon.Close();
            }
            return _lstticketsumbyuser;
        }

        public TicketView GetTicketView(int _ticketid)
        {
            List<TicketView> _ticketviews = new List<TicketView>();
            TicketView _ticketview = new TicketView();
            _ticketview.TicketId = _ticketid;
            _ticketviews = GetTicketViews(_ticketview);
            if (_ticketviews.Count >= 1)
            {
                _ticketview = _ticketviews[0];
            } else
            {
                _ticketview = new TicketView();
            }
            return _ticketview;
        }

        public ServiceResponse New(Ticket _ticket)
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            ServiceResponse _serviceresponse = new ServiceResponse();

            try
            {
                string _err = string.Empty;
                //..... Server side checking before any update
                if (string.IsNullOrWhiteSpace(_ticket.Title))
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Title";
                }
                if (_ticket.StatusId == 0)
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Status";
                }
                if (_ticket.PriorityId == 0)
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Priority";
                }
                if (_ticket.AssignUserId == 0)
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
                            sqlCommand.Parameters.AddWithValue("@Title", (string.IsNullOrWhiteSpace(_ticket.Title) ? string.Empty : _ticket.Title));
                            sqlCommand.Parameters.AddWithValue("@FullDescription", (string.IsNullOrWhiteSpace(_ticket.FullDescription) ? string.Empty : _ticket.FullDescription));
                            sqlCommand.Parameters.AddWithValue("@Organization", (string.IsNullOrWhiteSpace(_ticket.Organization) ? string.Empty : _ticket.Organization));
                            sqlCommand.Parameters.AddWithValue("@Project", (string.IsNullOrWhiteSpace(_ticket.Project) ? string.Empty : _ticket.Project));
                            sqlCommand.Parameters.AddWithValue("@AppModule", (string.IsNullOrWhiteSpace(_ticket.AppModule) ? string.Empty : _ticket.AppModule));
                            sqlCommand.Parameters.AddWithValue("@AppVersion", (string.IsNullOrWhiteSpace(_ticket.AppVersion) ? string.Empty : _ticket.AppVersion));
                            sqlCommand.Parameters.AddWithValue("@ImageFilePath", (string.IsNullOrWhiteSpace(_ticket.ImageFilePath) ? string.Empty : _ticket.ImageFilePath));
                            sqlCommand.Parameters.AddWithValue("@PriorityId", _ticket.PriorityId);
                            sqlCommand.Parameters.AddWithValue("@StatusId", _ticket.StatusId);
                            sqlCommand.Parameters.AddWithValue("@AssignUserId", _ticket.AssignUserId);
                            sqlCommand.Parameters.AddWithValue("@CreateUserId", _ticket.CreateUserId);
                            sqlCommand.Parameters.AddWithValue("@UpdateUserId", _ticket.UpdateUserId);
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

        public ServiceResponse Set(Ticket _ticket)
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            ServiceResponse _serviceresponse = new ServiceResponse();

            try
            {
                string _err = string.Empty;
                //..... Server side checking before any update
                if (string.IsNullOrWhiteSpace(_ticket.Title))
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Title";
                }
                if (_ticket.StatusId == 0)
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Status";
                }
                if (_ticket.PriorityId == 0)
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Priority";
                }
                if (_ticket.AssignUserId == 0)
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
                            sqlCommand.Parameters.AddWithValue("@Title", (string.IsNullOrWhiteSpace(_ticket.Title) ? string.Empty : _ticket.Title));
                            sqlCommand.Parameters.AddWithValue("@FullDescription", (string.IsNullOrWhiteSpace(_ticket.FullDescription) ? string.Empty : _ticket.FullDescription));
                            sqlCommand.Parameters.AddWithValue("@Organization", (string.IsNullOrWhiteSpace(_ticket.Organization) ? string.Empty : _ticket.Organization));
                            sqlCommand.Parameters.AddWithValue("@Project", (string.IsNullOrWhiteSpace(_ticket.Project) ? string.Empty : _ticket.Project));
                            sqlCommand.Parameters.AddWithValue("@AppModule", (string.IsNullOrWhiteSpace(_ticket.AppModule) ? string.Empty : _ticket.AppModule));
                            sqlCommand.Parameters.AddWithValue("@AppVersion", (string.IsNullOrWhiteSpace(_ticket.AppVersion) ? string.Empty : _ticket.AppVersion));
                            sqlCommand.Parameters.AddWithValue("@ImageFilePath", (string.IsNullOrWhiteSpace(_ticket.ImageFilePath) ? string.Empty : _ticket.ImageFilePath));
                            sqlCommand.Parameters.AddWithValue("@PriorityId", _ticket.PriorityId);
                            sqlCommand.Parameters.AddWithValue("@StatusId", _ticket.StatusId);
                            sqlCommand.Parameters.AddWithValue("@AssignUserId", _ticket.AssignUserId);
                            sqlCommand.Parameters.AddWithValue("@UpdateUserId", _ticket.UpdateUserId);
                            sqlCommand.Parameters.AddWithValue("@TicketId", _ticket.TicketId);
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

        public ServiceResponse Del(int _ticketid)
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            ServiceResponse _serviceresponse = new ServiceResponse();

            try
            {
                if (_ticketid > 0)
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
                            sqlCommand.Parameters.AddWithValue("@TicketId", _ticketid);
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
