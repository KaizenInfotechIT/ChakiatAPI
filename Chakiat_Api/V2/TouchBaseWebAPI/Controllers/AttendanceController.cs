using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TouchBaseWebAPI.Models;
using System.Data;
using TouchBaseWebAPI.BusinessEntities;

namespace TouchBaseWebAPI.Controllers
{
    public class AttendanceController : ApiController
    {
        /// <summary>
        /// Author : Nandu
        /// created on : 10/10/2016
        /// task : Get Attendance Listing
        /// </summary>
        [System.Web.Http.HttpPost]
        public object GetAttendanceList(clsGetAttendanceList attendance)
        {
            dynamic TBAttendanceListResult;
            List<object> AttendanceListResult = new List<object>();

            try
            {
                List<AttendanceList> Result = Attendance.getAttendanceList(attendance);

                for (int i = 0; i < Result.Count; i++)
                {
                    AttendanceListResult.Add(new { AttendanceResult = Result[i] });
                }

                if (AttendanceListResult != null)
                {
                    TBAttendanceListResult = new { status = "0", message = "success", AttendanceListResult };
                }
                else
                {
                    TBAttendanceListResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAttendanceListResult = new { status = "1", message = "failed" };
            }

            return new { TBAttendanceListResult };
        }
    }
}
