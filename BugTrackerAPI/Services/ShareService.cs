using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using BugTrackerAPI.Models;

namespace BugTrackerAPI.Services
{
    public interface IShareService
    {
        List<Status> GetStatusList();

        List<Priority> GetPriorityList();

        FileStream GetFile(string filename);

        string UploadFile(IFormFile postedfile);
    }

    public class ShareService : IShareService
    {
        private const string _constr = "BugTrackerAppCon";
        private const string _dbname = "DBName";
        private const string _errormessage = "System error occurs and please try again later";
        private const string _strgetstatussql = @"SELECT StatusId, StatusName FROM dbo.tblStatus ORDER BY StatusId";
        private const string _strgetprioritysql = @"SELECT PriorityId, PriorityName FROM dbo.tblPriority ORDER BY PriorityId";
        private const string _filenameDefault = "anonymous.jpg";

        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public ShareService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _config = configuration;
            _env = env;
        }


        public List<Status> GetStatusList()
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            List<Status> _lststatus = new List<Status>();
            DataTable dtStatus = new DataTable();
            SqlDataReader sqlReader;

            using (SqlConnection sqlCon = new SqlConnection(sqlDataSource))
            {
                sqlCon.Open();
                using (SqlCommand sqlCommand = new SqlCommand(_strgetstatussql, sqlCon))
                {
                    sqlReader = sqlCommand.ExecuteReader();
                    dtStatus.Load(sqlReader);
                    foreach (DataRow _drStatus in dtStatus.Rows)
                    {
                        _lststatus.Add(new Status()
                        {
                            StatusId = int.Parse(_drStatus["StatusId"].ToString()),
                            StatusName = _drStatus["StatusName"].ToString(),
                        });
                    }
                    sqlReader.Close();
                }
                sqlCon.Close();
            }
            return _lststatus;
        }

        public List<Priority> GetPriorityList()
        {
            string dbname = _config[_dbname];
            string sqlDataSource = string.Format(_config.GetConnectionString(_constr), dbname);
            List<Priority> _listpriority = new List<Priority>();
            DataTable dtPriority = new DataTable();
            
            SqlDataReader sqlReader;

            using (SqlConnection sqlCon = new SqlConnection(sqlDataSource))
            {
                sqlCon.Open();
                using (SqlCommand sqlCommand = new SqlCommand(_strgetprioritysql, sqlCon))
                {
                    sqlReader = sqlCommand.ExecuteReader();
                    dtPriority.Load(sqlReader);
                    foreach (DataRow _drPriority in dtPriority.Rows)
                    {
                        _listpriority.Add(new Priority()
                        {
                            PriorityId = int.Parse(_drPriority["PriorityId"].ToString()),
                            PriorityName = _drPriority["PriorityName"].ToString(),
                        });
                    }
                    sqlReader.Close();
                }
                sqlCon.Close();
            }
            return _listpriority;
        }

        public FileStream GetFile(string _filename)
        {
            FileStream imageFile;
            var physicalPath = _env.ContentRootPath + _config["UploadPath"] + _filename;

            if (!(System.IO.File.Exists(physicalPath)))
            {
                physicalPath = _env.ContentRootPath + _config["UploadPath"] + _filenameDefault;
            }
            imageFile = System.IO.File.OpenRead(physicalPath);
            return imageFile;
        }

        public string UploadFile(IFormFile _postedfile)
        {
            const string fileNameDefault = "yyyyMMddHHmmssfff";
            const string fileExtensionDefault = ".jpg";
            string filename = string.Empty;

            try
            {
                // Rename the orginal file name into timestamp to avoid duplicate file name exists.
                filename = DateTime.Now.ToString(fileNameDefault) + fileExtensionDefault;
                var physicalPath = _env.ContentRootPath + _config["UploadPath"] + filename;

                while (System.IO.File.Exists(physicalPath))
                {
                    filename = DateTime.Now.ToString(fileNameDefault) + fileExtensionDefault;
                    physicalPath = _env.ContentRootPath + _config["UploadPath"] + filename;
                }

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    _postedfile.CopyTo(stream);
                }
            }
            catch (Exception)
            {
                filename = string.Empty;
            }
            return filename;
        }
    }
}
