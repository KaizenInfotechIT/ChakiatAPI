using System.Web.Http;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;
using System.Configuration;
using System.Web;
using System;
using System.IO;
using System.Collections.Generic;

namespace TouchBaseWebAPI.Controllers
{
    public class ClubMonthlyReportController : ApiController
    {
        /// <summary>
        /// Author : Nandu
        /// created on : 10/10/2016
        /// task : Get Attendance Listing
        /// </summary>
        [System.Web.Http.HttpPost]
        public object GetMonthlyReportList(ClubMonthlyReport_Input Obj)
        {
            dynamic TBClubMonthlyReportListResult;
            List<object> ClubMonthlyReportListResult = new List<object>();

            try
            {
                List<ClubMonthlyReportList> Result = clubMonthlyReport.GetMonthlyReportList(Obj);

                //for (int i = 0; i < Result.Count; i++)
                //{
                //    ClubMonthlyReportListResult.Add(new { AttendanceResult = Result[i] });
                //}

                if (ClubMonthlyReportListResult != null)
                {
                    TBClubMonthlyReportListResult = new { status = "0", message = "success", Result };
                }
                else
                {
                    TBClubMonthlyReportListResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBClubMonthlyReportListResult = new { status = "1", message = "failed" };
            }

            return new { TBClubMonthlyReportListResult };
        }
    }
}
