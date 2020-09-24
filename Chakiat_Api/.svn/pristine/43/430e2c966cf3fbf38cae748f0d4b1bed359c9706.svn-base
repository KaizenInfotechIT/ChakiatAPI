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
    public class CelebrationsController : ApiController
    {
        [System.Web.Http.HttpPost]
        public object GetMonthEventList(ClsMonthCalenderInput monthCal)
        {
            dynamic TBEventListResult;
            
            try
            {
                var Result = Celebration.GetMonthEventList(monthCal);

                if (Result != null)
                {
                    TBEventListResult = new { status = "0", message = "success",updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Result };
                }
                else
                {
                    TBEventListResult = new { status = "0", message = "Record not found", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
                }
            }
            catch
            {
                TBEventListResult = new { status = "1", message = "failed", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
            }

            return new { TBEventListResult };
        }

        [HttpPost]
        public object GetTodaysBirthday(ClsMonthCalenderInput monthCal)
        {
            dynamic TBMemberListResult;

            try
            {
                var Result = Celebration.GetTodaysBirthday(monthCal.groupId);

                if (Result != null)
                {
                    TBMemberListResult = new { status = "0", message = "success",  Result };
                }
                else
                {
                    TBMemberListResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBMemberListResult = new { status = "1", message = "failed" };
            }

            return new { TBMemberListResult };
        }
    }
}
