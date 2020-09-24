using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class Celebration
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static ClsMonthCalenderOutput GetMonthEventList(ClsMonthCalenderInput monthCal)
        {
            try
            {
                string sqlProc;
                if (monthCal.groupCategory == "2")
                {
                    sqlProc = "V7_USPGetDistrictEventByMonth_Calender";
                }
                else
                {
                    sqlProc = "V7_USPGetEventByMonth_Calender";
                }
                MySqlParameter[] parameterList = new MySqlParameter[4];
                parameterList[0] = new MySqlParameter("?GroupID", monthCal.groupId);
                parameterList[1] = new MySqlParameter("?ProfileID", monthCal.profileId);
                parameterList[2] = new MySqlParameter("?Curr_Date", monthCal.selectedDate);
                parameterList[3] = new MySqlParameter("?UpdatedOn", monthCal.updatedOn);

                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, sqlProc, parameterList);
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

        /// <summary>
        /// Created By : Nandkishor K
        /// Created Date : 24/07/2017
        /// Reason : Listing of Events(Only having Event Type - All)
        /// </summary>
        public static EventList1 GetEventMinDetails(string eventID)
        {
            try
            {
                MySqlParameter[] parameterList = new MySqlParameter[1];
                parameterList[0] = new MySqlParameter("?eventID", eventID);

                EventList1 evt = _DBTouchbase.ExecuteStoreQuery<EventList1>("Call V4_USPEventDetails(?eventID)", parameterList).SingleOrDefault();
                if (evt != null)
                {
                    if (!string.IsNullOrEmpty(evt.eventImg))
                    {
                        string event_Image = evt.eventImg.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Event/Group" + evt.grpID + "/thumb/";
                        evt.eventImg = path + event_Image;
                    }
                }
                return evt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}