using ClearScada.Client.Simple;
using ClearScada.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CSLogger.src
{
    /*
    And this is the main class and it associated with a service
    and contains the folowing functions:reading from ClearScada and writing to DB MSSQL.
    */
    internal class CSLoggerClass
    {
        public bool stop = true;
        XmlDocument xDoc;
        CSconn csConn = null;
        Sqlconn sqlConn = null;
        XmlElement xRoot = null;
        Logger logger = null;
        Station StTemp;
        List<Station> lstStTemp;
        public bool enabled;
        public CSLoggerClass(CSconn oCSConn, Sqlconn oSqlConn, Logger logger) { csConn = oCSConn; sqlConn = oSqlConn; this.logger = logger; }

        public List<Station> readDataFromXML(string sPath)
        {
            try
            {
                lstStTemp = new List<Station>();

                //Creating and linking the XML-document
                xDoc = new XmlDocument();
                xDoc.Load(sPath);
                xRoot = xDoc.DocumentElement;

                //And here we take the array of stations.
                XmlNodeList xlstStation = xRoot.SelectNodes("Station");
                logger.Debug("Считан перечень станций из XML-файла");

                //Iterating the array of stations.
                foreach (XmlNode xStation in xlstStation)
                {
                    StTemp = new Station();
                    StTemp.lstTags = new SortedList<string, string>();
                    StTemp.procName = xStation.Attributes["procName"].Value;

                    //And then we got the array of tag values for each station.
                    XmlNodeList xlstTag = xStation.SelectNodes("Tags//Tag");

                    /*We used iterator because we needed to iterate the values of all stations, which
                    we got before*/
                    foreach (XmlNode xNode in xlstTag)
                    {
                        string tagName = xNode.Attributes["tagName"].Value;
                        string tagSource = xNode.Attributes["tagSource"].Value;
                        StTemp.lstTags.Add(tagName, tagSource);
                    }
                    //We form list of stations for further work.
                    logger.Error(xStation.Name);
                    lstStTemp.Add(StTemp);
                }

            }
            catch (XmlException e)
            {
                logger.Error($"{e.Message}");
            }
            return lstStTemp;

        }

        /*If you called this function then the process of reading and writing
        //data will been started*/
        public void readingAndWriting()
        {
            object valueOfTag;
           
            if (!stop)
            {

                //Iterating a set of stations which were formed earlier.
                try
                {
                    foreach (Station station in lstStTemp)
                    {
                        //Forming the sql querry to MSSQL database
                        SqlCommand comm = new SqlCommand(station.procName, sqlConn.getConnection()) { CommandType = CommandType.StoredProcedure };
                        
                        //And now we can to itarate tags in the list each station.
                        foreach (string tagName in station.lstTags.Keys)
                        {

                            string tagSource = station.lstTags[tagName];

                            //Reading data from ClearScada database.
                            DBObject tag = csConn.GetConn().GetObject(tagSource);


                            valueOfTag = tag?.GetProperty("CurrentValue");

                            if (valueOfTag is null)
                            {
                                String errorNotFoundTag = $"В базе CS не найден объект {tagName} по пути {tagSource}.Цикл опроса прерван";
                                logger.Error(errorNotFoundTag);
                            }

                            comm.Parameters.Add($"@{tagName}", SqlDbType.Int).Value = valueOfTag;
                        }
                        //Calling the stored procedure from  instance of MSSQL for each Station.
                        comm.ExecuteNonQuery();
                    }



                }
                catch (ClearScadaException e)
                {
                    logger.Error(e.Message);
                    enabled = false;


                }
                catch (SqlException e)
                {
                    logger.Error(e.Message);
                    enabled = false;

                }




            }


        }
    }
}
