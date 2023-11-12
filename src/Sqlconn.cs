using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceProcess;
using System.Threading;
using NLog;
using System.Data.SqlClient;
using System.Configuration;
using System.Dynamic;
using System.Xml;
using ClearScada.Client.Simple;
using ClearScada.Client;

namespace CSLogger.src
{
    internal class Sqlconn: IDisposable
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString);
        public void Dispose()
        {
            conn.Dispose();
        }
         internal SqlConnection getConnection()
        {
            return conn;
        }
        internal void OpenConn()
        {
            if (conn.State == System.Data.ConnectionState.Closed) conn.Open();
        }
        internal void CloseConn()
        {
            if (conn.State == System.Data.ConnectionState.Open) conn.Close();
        }
    }
}
