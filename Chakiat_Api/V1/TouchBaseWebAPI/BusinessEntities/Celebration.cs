using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class Celebration
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static ClsMonthCalenderOutput GetMonthEventList(ClsMonthCalenderInput monthCal)
        {
            try
            {
                MySqlParameter[] parameterList = new MySqlParameter[4];
                parameterList[0] = new MySqlParameter("?GroupID", monthCal.groupId);
                parameterList[1] = new MySqlParameter("?ProfileID", monthCal.profileId);
                parameterList[2] = new MySqlParameter("?Curr_Date", monthCal.selectedDate);
                parameterList[3] = new MySqlParameter("?UpdatedOn", monthCal.updatedOn);

                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V6_USPGetEventByMonth_Calender", parameterList);
                DataTable dtNewEvents = result.Tables[0];
                DataTable dtUpdatedEvents = result.Tables[1];
                DataTable dtDeletedEvents = result.Tables[2];

                List<CalenderEventList> NewEventList = new List<CalenderEventList>();
                if (dtNewEvents.Rows.Count > 0)
                {
                    NewEventList = GlobalFuns.DataTableToList<CalenderEventList>(dtNewEvents);
                }
                List<CalenderEventList> UpdatedEventList = new List<CalenderEventList>();
                if (dtUpdatedEvents.Rows.Count > 0)
                {
                    UpdatedEventList = GlobalFuns.DataTableToList<CalenderEventList>(dtUpdatedEvents);
                }
                List<CalenderEventList> DeletedEventList = new List<CalenderEventList>();
                if (dtDeletedEvents.Rows.Count > 0)
                {
                    DeletedEventList = GlobalFuns.DataTableToList<CalenderEventList>(dtDeletedEvents);
                }
                ClsMonthCalenderOutput calender = new ClsMonthCalenderOutput();
                calender.newEvents = NewEventList;
                calender.updatedEvents = UpdatedEventList;
                calender.deletedEvents = DeletedEventList;

                return calender;
            }
            catch
            {
                throw;
            }
        }

        public static List<ClsTodayBirthday> GetTodaysBirthday(string groupID)
        {
            try
            {
             MySqlParameter[] parameterList = new MySqlParameter[1];
             parameterList[0] = new MySqlParameter("?grpID",groupID);
             var result = _DBTouchbase.ExecuteStoreQuery<ClsTodayBirthday>("Call V6_USPGetTodaysBirthday(?grpID)", parameterList).ToList();
             return result;
            }
            catch
            {
                throw;
            }
           
        }
    }
}