using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFIService.security;

namespace MFIService.config
{
    class DatabaseConnection
    {
        private static String LOCAL_HOST = "";
        private static String LOCAL_DATABASE = "";
        private static String LOCAL_PORT = "";
        private static String LOCAL_USER = "";
        private static String LOCAL_PASSWORD = "";
        private static String REMOTE_HOST = "";
        private static String REMOTE_DATABASE = "";
        private static String REMOTE_PORT = "";
        private static String REMOTE_USER = "";
        private static String REMOTE_PASSWORD = "";
        private static String PASSWORDS_ARE_ENCRYPTED = "";
        private static String LOCAL_DATABASE_TECHNOLOGY = "";
        public static long SYNC_INTERVAL = 3600000;//60000=1 min
        public static String LOG_PATH = "C:\\MFI_Service_Log.txt";
        public static int SYNC_BATCH_SIZE = 50;

        public static void readConfigFile()
        {
            if (Properties.ConfigFile.Default.sync_interval > 0)
            {
                DatabaseConnection.SYNC_INTERVAL = Properties.ConfigFile.Default.sync_interval;
            }
            if (Properties.ConfigFile.Default.sync_batch_size > 0)
            {
                DatabaseConnection.SYNC_BATCH_SIZE = Properties.ConfigFile.Default.sync_batch_size;
            }
            if (Properties.ConfigFile.Default.log_path.Length > 0)
            {
                DatabaseConnection.LOG_PATH = Properties.ConfigFile.Default.log_path;
            }
            DatabaseConnection.LOCAL_DATABASE_TECHNOLOGY = Properties.ConfigFile.Default.local_database_technology;
            DatabaseConnection.PASSWORDS_ARE_ENCRYPTED = Properties.ConfigFile.Default.passwords_are_encrypted;
            DatabaseConnection.LOCAL_HOST = Properties.ConfigFile.Default.local_host;
            DatabaseConnection.LOCAL_DATABASE = Properties.ConfigFile.Default.local_database;
            DatabaseConnection.LOCAL_PORT = Properties.ConfigFile.Default.local_port;
            DatabaseConnection.LOCAL_USER = Properties.ConfigFile.Default.local_user;
            if (DatabaseConnection.PASSWORDS_ARE_ENCRYPTED == "1")
            {
                DatabaseConnection.LOCAL_PASSWORD = Security.Decrypt(Properties.ConfigFile.Default.local_password);
            }
            else
            {
                DatabaseConnection.LOCAL_PASSWORD = Properties.ConfigFile.Default.local_password;
            }
            DatabaseConnection.REMOTE_HOST = Properties.ConfigFile.Default.remote_host;
            DatabaseConnection.REMOTE_DATABASE = Properties.ConfigFile.Default.remote_database;
            DatabaseConnection.REMOTE_PORT = Properties.ConfigFile.Default.remote_port;
            DatabaseConnection.REMOTE_USER = Properties.ConfigFile.Default.remote_user;
            if (DatabaseConnection.PASSWORDS_ARE_ENCRYPTED == "1")
            {
                DatabaseConnection.REMOTE_PASSWORD = Security.Decrypt(Properties.ConfigFile.Default.remote_password);
            }
            else
            {
                DatabaseConnection.REMOTE_PASSWORD = Properties.ConfigFile.Default.remote_password;
            }
        }

        public static String getLocalConnectionString()
        {
            String LocalConnectionString = "";
            try
            {
                //MS SQL
                if (DatabaseConnection.LOCAL_DATABASE_TECHNOLOGY.Equals("MSSQL"))
                {
                    if (DatabaseConnection.LOCAL_PORT.Length > 0)
                    {
                        LocalConnectionString = @"Data Source=" + DatabaseConnection.LOCAL_HOST + "," + DatabaseConnection.LOCAL_PORT + ";Initial Catalog=" + DatabaseConnection.LOCAL_DATABASE + ";User ID=" + DatabaseConnection.LOCAL_USER + ";Password=" + DatabaseConnection.LOCAL_PASSWORD + "";
                    }
                    else
                    {
                        LocalConnectionString = @"Data Source=" + DatabaseConnection.LOCAL_HOST + ";Initial Catalog=" + DatabaseConnection.LOCAL_DATABASE + ";User ID=" + DatabaseConnection.LOCAL_USER + ";Password=" + DatabaseConnection.LOCAL_PASSWORD + "";
                    }
                }
                else
                {
                    LocalConnectionString = "";
                }
            }
            catch (ApplicationException e)
            {
            }
            return LocalConnectionString;
        }

        public static String getRemoteConnectionString()
        {
            String LocalConnectionString = "";
            try
            {
                //MS SQL
                if (DatabaseConnection.REMOTE_PORT.Length > 0)
                {
                    LocalConnectionString = @"Data Source=" + DatabaseConnection.REMOTE_HOST + "," + DatabaseConnection.REMOTE_PORT + ";Initial Catalog=" + DatabaseConnection.REMOTE_DATABASE + ";User ID=" + DatabaseConnection.REMOTE_USER + ";Password=" + DatabaseConnection.REMOTE_PASSWORD + "";
                }
                else
                {
                    LocalConnectionString = @"Data Source=" + DatabaseConnection.REMOTE_HOST + ";Initial Catalog=" + DatabaseConnection.REMOTE_DATABASE + ";User ID=" + DatabaseConnection.REMOTE_USER + ";Password=" + DatabaseConnection.REMOTE_PASSWORD + "";
                }
            }
            catch (ApplicationException e)
            {
            }
            return LocalConnectionString;
        }

        public static Boolean IsConnection(SqlConnection aConn)
        {
            try
            {
                aConn.Open();
                if (aConn.State == ConnectionState.Open)
                {
                    aConn.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SqlException se)
            {
                return false;
            }
        }

        public static Boolean IsConnectionRemote()
        {
            SqlConnection aConn = new SqlConnection(DatabaseConnection.getRemoteConnectionString());
            try
            {
                aConn.Open();
                if (aConn.State == ConnectionState.Open)
                {
                    aConn.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SqlException se)
            {
                return false;
            }
        }

        public static Boolean IsConnectionLocal()
        {
            SqlConnection aConn = new SqlConnection(DatabaseConnection.getLocalConnectionString());
            try
            {
                aConn.Open();
                if (aConn.State == ConnectionState.Open)
                {
                    aConn.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SqlException se)
            {
                return false;
            }
        }

    }
}
