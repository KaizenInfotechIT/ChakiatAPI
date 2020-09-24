using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using MySql.Data.MySqlClient;
using System.Text;
using System.Globalization;
using System.Data;
using TouchBaseWebAPI.Models;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class ManageExceptions
    {
        #region Variable Declarition
        static string binPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        static string exceptionLogFilename = "ExceptionLog.txt";
        #endregion

        #region Function-->TraceException()
        /// <summary>
        /// Author: Pramod S
        /// Created Date: 03-08-2016
        /// Desp: Created to Log Exception
        /// </summary>
        public static int TraceException(string APIName, string currentFunctionName, string innerException, string message, string stackTrace)
        {
            int resultState = 0;
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[6];
                parameters[0] = new MySqlParameter("?p_ExceptionAutoID", "0");
                parameters[1] = new MySqlParameter("?p_excep_CurrentPageName", APIName);
                parameters[2] = new MySqlParameter("?p_excep_CurrentFunctionName", currentFunctionName);
                parameters[3] = new MySqlParameter("?p_excep_InnerException", innerException);
                parameters[4] = new MySqlParameter("?p_excep_Message", message);
                parameters[5] = new MySqlParameter("?p_excep_StackTrace", stackTrace);
                string storeproc = "USP_ManageException";

                resultState = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.StoredProcedure, storeproc, parameters);
            }
            catch
            {
                //TraceExceptionInFile(currentPageName, currentFunctionName, ex);
            }
            return resultState;
        }
        #endregion

        #region TraceTrack Function
        /// <summary>
        /// Author:Pramod S
        /// Created Date:03-08-2016
        /// </summary>
        public static void TraceExceptionInFile(string APIName, string currentFunctionName, Exception ex)
        {
            StringBuilder sbrTraceFileName = new StringBuilder();
            sbrTraceFileName.Append(binPath);
            sbrTraceFileName.Append(exceptionLogFilename);

            DateTime dtCurrentTime = DateTime.Now;
            StringBuilder sbrFileContent = new StringBuilder();
            sbrFileContent.Append("Date Format-->Year/Month/Day");
            sbrFileContent.Append(System.Environment.NewLine);
            sbrFileContent.Append(DateTime.Now.ToString("yyyy/MM/dd_hh:mm:ss.fff", CultureInfo.InvariantCulture));
            sbrFileContent.Append("_");
            sbrFileContent.Append("Page Name:-" + APIName);
            sbrFileContent.Append("_");
            sbrFileContent.Append("Function Name:-" + currentFunctionName);
            sbrFileContent.Append("_");
            sbrFileContent.Append("Message Name:-" + (Convert.ToString(ex.Message).Length > 0 ? Convert.ToString(ex.Message) : string.Empty));
            sbrFileContent.Append(System.Environment.NewLine);
            sbrFileContent.Append("InnerException Name:-" + (Convert.ToString(ex.InnerException).Length > 0 ? Convert.ToString(ex.InnerException) : string.Empty));
            sbrFileContent.Append(System.Environment.NewLine);
            sbrFileContent.Append("StackTrace Name:-" + (Convert.ToString(ex.StackTrace).Length > 0 ? Convert.ToString(ex.StackTrace) : string.Empty));
            sbrFileContent.Append(System.Environment.NewLine);

            File.AppendAllText(sbrTraceFileName.ToString(), sbrFileContent.ToString());
        }
        #endregion
    }
}