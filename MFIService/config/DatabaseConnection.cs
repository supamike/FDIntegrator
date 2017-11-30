using System;
using System.Collections.Generic;
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
        private static String PASSWORDS_ARE_ENCRYPTED="";
        private static String LOCAL_DATABASE_TECHNOLOGY="";

        public static void readConfigFile()
        {
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
                    LocalConnectionString = @"Data Source=" + DatabaseConnection.LOCAL_HOST + "," + DatabaseConnection.LOCAL_PORT + ";Initial Catalog=" + DatabaseConnection.LOCAL_DATABASE + ";User ID=" + DatabaseConnection.LOCAL_USER + ";Password=" + DatabaseConnection.LOCAL_PASSWORD + "";
                    //LocalConnectionString = "Data Source=" + DatabaseConnection.LOCAL_HOST + "," + DatabaseConnection.LOCAL_PORT + ";Initial Catalog=" + DatabaseConnection.LOCAL_DATABASE + ";User ID=" + DatabaseConnection.LOCAL_USER + ";Password=" + DatabaseConnection.LOCAL_PASSWORD + "";
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
                LocalConnectionString = @"Data Source=" + DatabaseConnection.REMOTE_HOST + "," + DatabaseConnection.REMOTE_PORT + ";Initial Catalog=" + DatabaseConnection.REMOTE_DATABASE + ";User ID=" + DatabaseConnection.REMOTE_USER + ";Password=" + DatabaseConnection.REMOTE_PASSWORD + "";
                //LocalConnectionString = "Data Source=" + DatabaseConnection.REMOTE_HOST + "," + DatabaseConnection.REMOTE_PORT + ";Initial Catalog=" + DatabaseConnection.REMOTE_DATABASE + ";User ID=" + DatabaseConnection.REMOTE_USER + ";Password=" + DatabaseConnection.REMOTE_PASSWORD + "";
            }
            catch (ApplicationException e)
            {
            }
            return LocalConnectionString;
        }

    }

}
