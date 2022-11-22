using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using BugTrackerAPI.Models;

namespace BugTrackerAPI.Services
{
    public interface IUserService
    {
        User Get(string username, string password);

        List<User> GetUsers(User user);

        ServiceResponse New(User user);

        ServiceResponse Set(User user);

        ServiceResponse Del(int userid);
    }

    public class UserService : IUserService
    {
        private const string _constr = "BugTrackerAppCon";
        private const string _dbname = "DBName";
        private const string _errortemplate = "Please fill in required field(s): {0}";
        private const string _strgetsql = @"SELECT UserId, UserName, Password, UserDescription, EmailAddress, PhotoFilePath
                    FROM dbo.tblUser";
        private const string _strgetbyusersql = @"SELECT UserId, UserName, Password, UserDescription, EmailAddress, PhotoFilePath
                    FROM dbo.tblUser WHERE (UserName = @UserName OR EmailAddress = @EmailAddress) AND Password = @Password";
        private const string _strinssql = @"BEGIN
                   IF NOT EXISTS (SELECT * FROM dbo.tblUser  
                   WHERE UserName = @UserName
                   OR EmailAddress = @EmailAddress)
                   BEGIN
                       INSERT INTO dbo.tblUser 
                       (UserName, Password, UserDescription, EmailAddress, PhotoFilePath)
                       VALUES 
                       (@UserName, @Password, @UserDescription, @EmailAddress, @PhotoFilePath)
                   END
                END;";
        private const string _strupdsql = @"BEGIN
                   IF NOT EXISTS (SELECT * FROM dbo.tblUser  
                   WHERE (UserName = @UserName
                   OR EmailAddress = @EmailAddress)
                   AND UserId <> @UserId)
                   BEGIN
                    UPDATE dbo.tblUser
                    SET UserName = @UserName,
                    Password = @Password,  
                    UserDescription = @UserDescription, 
                    EmailAddress = @EmailAddress, 
                    PhotoFilePath = @PhotoFilePath
                    WHERE UserId = @UserId
                   END
                END";
        private const string _strdelsql = @"DELETE dbo.tblUser WHERE UserId = @UserId";
        private const string _errormessage = "Duplicate user name or email address and please try again";
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public UserService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _config = configuration;
            _env = env;
        }

        public User Get(string _username, string _password)
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            SqlDataReader sqlReader;
            User _user = new User();

            //..... Server side checking before retrieve records
            if ((!(string.IsNullOrWhiteSpace(_username))) && (!(string.IsNullOrWhiteSpace(_password))))
            {
                using (SqlConnection sqlCon = new SqlConnection(sqlDataSource))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(_strgetbyusersql, sqlCon))
                    {
                        sqlCommand.Parameters.AddWithValue("@UserName", _username);
                        sqlCommand.Parameters.AddWithValue("@EmailAddress", _username);
                        sqlCommand.Parameters.AddWithValue("@Password", _password);
                        sqlReader = sqlCommand.ExecuteReader();
                        // dtUser.Load(sqlReader);
                        if (sqlReader.HasRows)
                        {
                            if ((sqlReader.Read()) && (!(sqlReader == null)))
                            {
                                _user.UserId = (sqlReader["UserId"] != null ? Int32.Parse(sqlReader["UserId"].ToString()) : 0);
                                _user.UserName = sqlReader["UserName"].ToString();
                                _user.UserDescription = sqlReader["UserDescription"].ToString();
                                _user.EmailAddress = sqlReader["EmailAddress"].ToString();
                                _user.PhotoFilePath = sqlReader["PhotoFilePath"].ToString();
                            }
                        }
                        sqlReader.Close();
                    }
                    sqlCon.Close();
                }
            }
            return _user;
        }

        public List<User> GetUsers(User _user)
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            List<User> _lstuser = new List<User>();
            DataTable dtUser = new DataTable();
            SqlDataReader sqlReader;

            using (SqlConnection sqlCon = new SqlConnection(sqlDataSource))
            {
                sqlCon.Open();
                using (SqlCommand sqlCommand = new SqlCommand(_strgetsql, sqlCon))
                {
                    sqlReader = sqlCommand.ExecuteReader();
                    dtUser.Load(sqlReader);
                    foreach (DataRow _drUser in dtUser.Rows)
                    {
                        _lstuser.Add(new User()
                        {
                            UserId = int.Parse(_drUser["UserId"].ToString()),
                            UserName = _drUser["UserName"].ToString(),
                            UserDescription = _drUser["UserDescription"].ToString(),
                            Password = _drUser["Password"].ToString(),
                            ConfirmPassword = _drUser["Password"].ToString(),
                            EmailAddress = _drUser["EmailAddress"].ToString(),
                            PhotoFilePath = _drUser["PhotoFilePath"].ToString()
                        });
                    }
                    sqlReader.Close();

                }
                sqlCon.Close();
            }
            return _lstuser;
        }

        public ServiceResponse New(User _user)
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            ServiceResponse _serviceresponse = new ServiceResponse();

            try
            {
                string _err = string.Empty;
                //..... Server side checking before any update
                if (string.IsNullOrWhiteSpace(_user.UserName))
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "User Name";
                }
                if (string.IsNullOrWhiteSpace(_user.Password))
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Password";
                }
                if (string.IsNullOrWhiteSpace(_user.EmailAddress))
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "EmailAddress";
                }
                if (_err.Length > 0)
                {
                    _serviceresponse.Error = string.Format(_errortemplate, _err);
                }
                if (_user.Password != _user.ConfirmPassword)
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? " AND " : string.Empty) + "Passwords not matched";
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
                            sqlCommand.Parameters.AddWithValue("@UserName", _user.UserName);
                            sqlCommand.Parameters.AddWithValue("@Password", _user.Password);
                            sqlCommand.Parameters.AddWithValue("@UserDescription",
                            (string.IsNullOrWhiteSpace(_user.UserDescription) ? string.Empty : _user.UserDescription));
                            sqlCommand.Parameters.AddWithValue("@EmailAddress", _user.EmailAddress);
                            sqlCommand.Parameters.AddWithValue("@PhotoFilePath",
                                (string.IsNullOrWhiteSpace(_user.PhotoFilePath) ? string.Empty : _user.PhotoFilePath));
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
            } catch (Exception _ex)
            {
                _serviceresponse.RecordCount = 0;
                _serviceresponse.Success = false;
                _serviceresponse.Error = _ex.Message.ToString();
            }
            return _serviceresponse;
        }

        public ServiceResponse Set(User _user)
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            ServiceResponse _serviceresponse = new ServiceResponse();

            try
            {
                string _err = string.Empty;
                //..... Server side checking before any update
                if (string.IsNullOrWhiteSpace(_user.UserName))
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "User Name";
                }
                if (string.IsNullOrWhiteSpace(_user.Password))
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "Password";
                }
                if (string.IsNullOrWhiteSpace(_user.EmailAddress))
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? "," : string.Empty) + "EmailAddress";
                }
                if (_err.Length > 0)
                {
                    _serviceresponse.Error = string.Format(_errortemplate, _err);
                }
                if (_user.Password != _user.ConfirmPassword)
                {
                    _err += (!(string.IsNullOrWhiteSpace(_err)) ? " AND " : string.Empty) + "Passwords not matched";
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
                            sqlCommand.Parameters.AddWithValue("@UserName", _user.UserName);
                            sqlCommand.Parameters.AddWithValue("@Password", _user.Password);
                            sqlCommand.Parameters.AddWithValue("@UserDescription", _user.UserDescription);
                            sqlCommand.Parameters.AddWithValue("@EmailAddress", _user.EmailAddress);
                            sqlCommand.Parameters.AddWithValue("@PhotoFilePath", _user.PhotoFilePath);
                            sqlCommand.Parameters.AddWithValue("@UserId", _user.UserId);
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

        public ServiceResponse Del(int _userid)
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            ServiceResponse _serviceresponse = new ServiceResponse();

            try
            {
                if (_userid > 0)
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
                            sqlCommand.Parameters.AddWithValue("@UserId", _userid);
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
