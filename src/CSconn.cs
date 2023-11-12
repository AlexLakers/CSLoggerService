using ClearScada.Client.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLogger.src


{
    //This class describes connection to ClearScada
    internal class CSconn:IDisposable
    {
        Connection conn = new Connection("CSLoggerr");
        public void Dispose()
        {
            conn.Dispose();
        }
        public void Connect()
        {

            conn.Connect("127.0.0.1");
            conn.LogOn("AlexLakers", "Lakers393");
        }
        public void Disconnect()
        {
            conn.Disconnect();
        }
        public Connection GetConn()
        {
            return conn;
        }
    }
}
