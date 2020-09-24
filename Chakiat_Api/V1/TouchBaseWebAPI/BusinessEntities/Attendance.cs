using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class Attendance
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DbTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static List<AttendanceList> getAttendanceList(clsGetAttendanceList attendance)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[4];

                param[0] = new MySqlParameter("?p_groupProfileID", attendance.groupProfileID);
                param[1] = new MySqlParameter("?p_attendanceMonth", attendance.month);
                param[2] = new MySqlParameter("?p_attendanceYear", attendance.year);
                param[3] = new MySqlParameter("?p_moduleId", attendance.moduleID);

                var Result = _DbTouchbase.ExecuteStoreQuery<AttendanceList>("CALL USPGetAttendanceList(?p_groupProfileID,?p_attendanceMonth,?p_attendanceYear,?p_moduleId)", param).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}