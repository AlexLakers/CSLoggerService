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
using CSLogger.src;

namespace CSLogger
{
    /*And this class extends the base class and here we can 
    to correct come params.*/
    public partial class CSLoggerService : ServiceBase
    {
        Logger loggerToFile;
        CSLoggerClass loggerToDB;
        CSconn csdb;
        Sqlconn sqldb;
       
        System.Threading.Timer timer;

        public CSLoggerService()
        {

            InitializeComponent();
            this.AutoLog = true;
            this.CanStop = true;
            this.CanPauseAndContinue = true;
           
        }
       public void StartLogger()
        {
            try
            {
                csdb.Connect();
                loggerToFile.Debug("The connection to DB of ClearSCADA is successfull");
                sqldb.OpenConn();
                loggerToFile.Debug("The connection to DB of MSSQL is successfull");

                //In the next row we can see path to file 'Stations.xml'
                List<Station> lstOfStations = loggerToDB.readDataFromXML(AppDomain.CurrentDomain.BaseDirectory+"\\cfg\\Stations.xml");  
                loggerToDB.stop = false ;

                //Creating a timer and setting  callback method(interval=60s)
                timer = new System.Threading.Timer(x => { loggerToDB.readingAndWriting(); }, null, 0, 60000);

                do { Thread.Sleep(1100); }
                while (!(loggerToDB.stop));
            }
            catch (Exception e)
            {
                loggerToFile.Error($"{e.Message} ({0})", "[Error!The service was stopped]");
            }
            finally
            {
                //Closing all connections.
                csdb.Disconnect();
                sqldb.CloseConn();
                timer.Dispose();
                loggerToFile.Debug("All connections were closed");
                

            }

        }
         //This method describes the behaviour when our service was stopped. 
        void StopLogger()
        {
            loggerToDB.stop = true;
            timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            timer.Dispose();
            //The additional time delay, it is necessary to stoping this service.
            Thread.Sleep(200);

          

        }

        //This method executing when the service starts.
        protected override void OnStart(string[] args)
        {
          

            loggerToFile = LogManager.GetCurrentClassLogger();
            csdb = new CSconn();
            sqldb = new Sqlconn();
            loggerToDB = new CSLoggerClass(csdb, sqldb, loggerToFile);
            
            /*
            And here creating a new thread for the main action as reading and writing data.
            We needed a new thread becouse the current thread processes commands from SCM.
            */
            Thread ThreadLogger = new Thread(() => StartLogger());
            ThreadLogger.Start();
        }
        //This method is executed when the service is stoped.
        protected override void OnStop()
        {
            
            StopLogger();   
        }
    }
    

    //This structure contains any detail for the station.
    struct Station
    {
        public string procName { get; set; }
        public SortedList<string, string> lstTags;


    }
    
}
    
    
        
    
    

