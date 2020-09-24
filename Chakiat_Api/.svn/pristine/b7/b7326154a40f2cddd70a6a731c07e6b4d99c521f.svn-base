using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.Data;
using System.Data;
using System.Configuration;
using System.Runtime;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class EventMaster
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DbTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static List<EventList> GetEventList(string memberID, string grpID, string SearchText, string type, string Admin, out string Grp_SMSCount)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[6];
                param[0] = new MySqlParameter("?memberId", memberID);
                param[1] = new MySqlParameter("?grpId", grpID);
                param[2] = new MySqlParameter("?searchText", SearchText);
                param[3] = new MySqlParameter("?filterType", type);
                param[4] = new MySqlParameter("?Admin", Admin);
                param[5] = new MySqlParameter("?SMS_Count", 0);
                param[5].Direction = ParameterDirection.InputOutput;
                DataSet Result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V4_USPEventSearchBYText", param);

                DataTable dt = Result.Tables[0];
                List<EventList> res = new List<EventList>();
                if (dt.Rows.Count > 0)
                {
                    res = GlobalFuns.DataTableToList<EventList>(dt);
                    foreach (EventList eve in res)
                    {
                        if (!string.IsNullOrEmpty(eve.eventImg))
                        {
                            string event_Image = eve.eventImg.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Event/Group" + grpID + "/thumb/";
                            eve.eventImg = path + event_Image;
                        }
                    }

                }
                Grp_SMSCount = param[5].Value.ToString();
                return res;
            }
            catch (Exception ex)
            {
                ManageExceptions.TraceException("Event/GetEventList", "GetEventList()", Convert.ToString(ex.InnerException), Convert.ToString(ex.Message), Convert.ToString(ex.StackTrace));
                throw ex;
            }
        }

        public static List<EventsDetail> GetEventDetails(string grpID, string eventID, string groupProfileID)
        {
            string repeatDateTime = "";
            try
            {
              
                MySqlParameter[] param = new MySqlParameter[3];
                param[0] = new MySqlParameter("@grpID", MySqlDbType.Int16);
                param[1] = new MySqlParameter("@eventID", MySqlDbType.Int16);
                param[2] = new MySqlParameter("@groupProfileID", MySqlDbType.Int16);
                param[0].Value = grpID;
                param[1].Value = eventID;
                param[2].Value = groupProfileID;
                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V4_USPGetEventListDetails", param);
                DataTable dtEvent = result.Tables[0];
                DataTable dtRepeatEvent = result.Tables[1];
                DataTable dtQuestions = result.Tables[2];
                List<EventsDetail> Eventdetail = new List<EventsDetail>();
                if (dtEvent.Rows.Count > 0)
                {
                    Eventdetail = GlobalFuns.DataTableToList<EventsDetail>(dtEvent);
                    if (!string.IsNullOrEmpty(Eventdetail[0].eventImg))
                    {
                        string event_Image = Eventdetail[0].eventImg.ToString();
                        // string path = HttpContext.Current.Server.MapPath("~/Documents/Events/Group" + grpID + "/thumb/");
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Event/Group" + grpID + "/";
                        Eventdetail[0].eventImg = path + event_Image;
                    }
                    if (dtRepeatEvent.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtRepeatEvent.Rows.Count; i++)
                        {
                            repeatDateTime += dtRepeatEvent.Rows[i]["eventDate"].ToString() + ",";
                        }
                        repeatDateTime = repeatDateTime.TrimEnd(',');
                    }
                    Eventdetail[0].repeatDateTime = repeatDateTime;

                    //Eventdetail[0].repeatEventResult = new List<object>();
                    //Eventdetail[0].questionArray = new List<object>();
                    //if (dtRepeatEvent.Rows.Count > 0)
                    //{
                    //    List<RepeatEvent> RepeatEvent = GlobalFuns.DataTableToList<RepeatEvent>(dtRepeatEvent);
                    //    for (int i = 0; i < RepeatEvent.Count; i++)
                    //    {
                    //        Eventdetail[0].repeatEventResult.Add(new { RepeatEvent = (object)RepeatEvent[i] });
                    //    }
                    //}
                    if (dtQuestions.Rows.Count == 1)
                    {
                        Eventdetail[0].questionId = dtQuestions.Rows[0]["questionId"].ToString();
                        Eventdetail[0].questionText = dtQuestions.Rows[0]["questionText"].ToString();
                        Eventdetail[0].questionType = dtQuestions.Rows[0]["questionType"].ToString();
                        Eventdetail[0].option1 = dtQuestions.Rows[0]["option1"].ToString();
                        Eventdetail[0].option2 = dtQuestions.Rows[0]["option2"].ToString();
                    }
                    else
                    {
                        Eventdetail[0].questionId = "";
                        Eventdetail[0].questionText = "";
                        Eventdetail[0].questionType = "";
                        Eventdetail[0].option1 = "";
                        Eventdetail[0].option2 = "";
                    }
                }
                return Eventdetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<EventsDetail> GetEventBySearchText(string text)
        {
            try
            {
                var searchText = new MySqlParameter("?Search_Text", text);
                var Result = _DbTouchbase.ExecuteStoreQuery<EventsDetail>("CALL USPEventSearchBYText(?Search_Text)", searchText).ToList();

                return Result;
            }
            catch (Exception ex)
            {
                ManageExceptions.TraceException("Event/GetEventBySearchText", "GetEventBySearchText()", Convert.ToString(ex.InnerException), Convert.ToString(ex.Message), Convert.ToString(ex.StackTrace));
                throw ex;
            }
        }

        public static EventJoinResult AnsweringEvent(AnsweringEvent obj_Responce)
        {
            try
            {
                dynamic EventJoinResult = null;
                MySqlParameter[] param = new MySqlParameter[5];

                param[0] = new MySqlParameter("?profileID", Convert.ToInt32(obj_Responce.profileID));
                param[1] = new MySqlParameter("?eventId", Convert.ToInt32(obj_Responce.eventId));
                param[2] = new MySqlParameter("?joiningStatus", obj_Responce.joiningStatus);
                param[3] = new MySqlParameter("?questionId", string.IsNullOrEmpty(obj_Responce.questionId) ? "0" : obj_Responce.questionId);
                param[4] = new MySqlParameter("?answer", string.IsNullOrEmpty(obj_Responce.answerByme) ? "" : obj_Responce.answerByme);
 
                EventJoinResult = _DbTouchbase.ExecuteStoreQuery<EventJoinResult>("CALL V4_USPAnsweringEvent(?profileID,?eventId,?joiningStatus,?questionId,?answer)", param).ToList();

                return EventJoinResult[0];
            }
            catch (Exception ex)
            {
                ManageExceptions.TraceException("Event/AnsweringEvent", "AnsweringEvent()", Convert.ToString(ex.InnerException), Convert.ToString(ex.Message), Convert.ToString(ex.StackTrace));
                throw ex;
            }
        }

        # region Commented Code
        //public static int AddEvent(AddEventResult obj_Input)
        //{
        //    string ssql;
        //    try
        //    {
        //        //#################################### Insert row in event Master ###########################################
        //        event_master obj_event = new event_master();
        //        obj_event.questionEnable =Convert.ToInt16 (obj_Input.questionEnable);
        //        obj_event.event_title = obj_Input.evntTitle;
        //        obj_event.event_type = obj_Input.eventType;
        //        obj_event.event_img = obj_Input.eventPic;
        //        obj_event.event_desc = obj_Input.evntDesc;
        //        obj_event.event_venue = obj_Input.eventVenue;
        //        obj_event.event_time = obj_Input.evntTime;
        //        obj_event.notification_start_time = obj_Input.publishTime;
        //        obj_event.expiry_Time = obj_Input.expiryTime;
        //        obj_event.publish_Time = obj_Input.publishTime;
        //        obj_event.fk_group_master_id = string.IsNullOrEmpty(obj_Input.grpID) ? 0 : Convert.ToInt32(obj_Input.grpID);
        //        obj_event.event_date = string.IsNullOrEmpty(obj_Input.evntDate) ? (DateTime?)null : GlobalFuns.GetFormattedDate(obj_Input.evntDate);
        //        obj_event.notification_start_date = string.IsNullOrEmpty(obj_Input.notifyDate) ? (DateTime?)null : GlobalFuns.GetFormattedDate(obj_Input.notifyDate);
        //        obj_event.expiry_Date = string.IsNullOrEmpty(obj_Input.notifyDate) ? (DateTime?)null : GlobalFuns.GetFormattedDate(obj_Input.expiryDate);
        //        obj_event.publish_Date = string.IsNullOrEmpty(obj_Input.publishDate) ? (DateTime?)null : GlobalFuns.GetFormattedDate(obj_Input.publishDate);
        //        obj_event.created_by = string.IsNullOrEmpty(obj_Input.userID) ? 0 : Convert.ToInt32(obj_Input.userID);

        //        _DbTouchbase.event_master.AddObject(obj_event);
        //        _DbTouchbase.SaveChanges();

        //        //Select event master ID of currently Inserted Row
        //        var eventID = (from e in _DbTouchbase.event_master
        //                       select e.pk_event_master_id).Max();

        //        //############################# Insert into event member details #####################################

        //        if (obj_Input.eventType == "SubGroup")
        //        {
        //            char[] delimiterChars = { ',' };
        //            string[] SubgroupID = obj_Input.membersIDs.Split(delimiterChars);
        //            foreach (string ID in SubgroupID)
        //            {
        //                int subgrpId = Convert.ToInt32(ID);

        //            }
        //        }

        //        else if (obj_Input.eventType == "Members" && !string.IsNullOrEmpty(obj_Input.membersIDs))
        //        {
        //            char[] delimiterChars = { ',' };
        //            string[] memberID = obj_Input.membersIDs.Split(delimiterChars);

        //            foreach (string s in memberID)
        //            {
        //                int memID = Convert.ToInt32(s);
        //                event_details obj_member = new event_details();
        //                obj_member.fk_event_master_id = eventID;
        //                obj_member.fk_member_master_id = memID;
        //                obj_member.creation_date = System.DateTime.Now;
        //                obj_member.created_by = string.IsNullOrEmpty(obj_Input.userID) ? 0 : Convert.ToInt32(obj_Input.userID);
        //                _DbTouchbase.event_details.AddObject(obj_member);
        //                _DbTouchbase.SaveChanges();
        //            }
        //        }
        //        else if (obj_Input.eventType == "All")
        //        {
        //            //ssql = "Insert Into event_master_details (fk_event_master_id,fk_member_master_id,creation_date,created_by) " +
        //            //       " (select " + eventID + " as ,pk_member_master_profile_id as fk_member_master_id,CURDATE()as creation_date," +
        //            //       "  " + obj_Input.userID + " as created_by from member_master_profile where fk_group_master_id=" + fk_event_master_idobj_Input.grpID + ") ";
        //            //_DbTouchbase.ExecuteStoreCommand(@ssql);
        //        }

        //        //#############################  Insert Into event_Repeat_Date ########################################
        //        int event_repeat_Rows = obj_Input.repeateventResult.Count;
        //        for (int i = 0; i < event_repeat_Rows; i++)
        //        {
        //            event_repeatdate obj_repeatDate = new event_repeatdate();
        //            obj_repeatDate.fk_event_master_id = Convert.ToInt32(eventID);
        //            obj_repeatDate.repeat_time = obj_Input.repeateventResult[i].eventTime;
        //            obj_repeatDate.created_by = Convert.ToInt32(obj_Input.userID);
        //            obj_repeatDate.creation_date = System.DateTime.Now;
        //            if (!string.IsNullOrEmpty(obj_Input.repeateventResult[i].eventDate))
        //            {
        //                obj_repeatDate.Repeat_date = GlobalFuns.GetFormattedDate(obj_Input.repeateventResult[i].eventDate);
        //            }

        //            _DbTouchbase.event_repeatdate.AddObject(obj_repeatDate);
        //            _DbTouchbase.SaveChanges();
        //        }
        //        return eventID;
        //    }
        //    catch
        //    {
        //        return -1;
        //    }
        //}
        #endregion

        public static Imgname AddEvent(AddEventResult obj_Input)
        {
            string subGrpIDs = "";

            try
            {
                if (obj_Input.isSubGrpAdmin == "1")
                {
                    subGrpIDs = SubGroupDirectory.GetAdminSubGroupList(obj_Input.grpID, obj_Input.userID);
                }
                MySqlParameter[] param = new MySqlParameter[26];
                param[0] = new MySqlParameter("?eventID", string.IsNullOrEmpty(obj_Input.eventID) ? "0" : obj_Input.eventID);
                param[1] = new MySqlParameter("?questionEnable", string.IsNullOrEmpty(obj_Input.questionEnable) ? "0" : obj_Input.questionEnable);
                param[2] = new MySqlParameter("?eventType", obj_Input.eventType);
                param[3] = new MySqlParameter("?membersIDs", obj_Input.membersIDs);
                param[4] = new MySqlParameter("?eventImageID", string.IsNullOrEmpty(obj_Input.eventImageID) ? "0" : obj_Input.eventImageID);
                param[5] = new MySqlParameter("?evntTitle", obj_Input.evntTitle);
                param[6] = new MySqlParameter("?evntDesc", obj_Input.evntDesc);
                param[7] = new MySqlParameter("?eventVenue", obj_Input.eventVenue);
                param[8] = new MySqlParameter("?venueLat", obj_Input.venueLat);
                param[9] = new MySqlParameter("?venueLong", obj_Input.venueLong);
                param[10] = new MySqlParameter("?evntDate", obj_Input.evntDate);
                param[11] = new MySqlParameter("?publishDate", obj_Input.publishDate);
                param[12] = new MySqlParameter("?expiryDate", obj_Input.expiryDate);               
                param[13] = new MySqlParameter("?sendSMSAll", obj_Input.sendSMSAll);
                param[14] = new MySqlParameter("?rsvpEnable", obj_Input.rsvpEnable);
                param[15] = new MySqlParameter("?sendSMSNonSmartPh", obj_Input.sendSMSNonSmartPh);
                param[16] = new MySqlParameter("?userID", obj_Input.userID);
                param[17] = new MySqlParameter("?grpID", obj_Input.grpID);
                param[18] = new MySqlParameter("?questionID", string.IsNullOrEmpty(obj_Input.questionId) ? "" : obj_Input.questionId);
                param[19] = new MySqlParameter("?questionText", string.IsNullOrEmpty(obj_Input.questionText) ? "" : obj_Input.questionText);
                param[20] = new MySqlParameter("?questionType", string.IsNullOrEmpty(obj_Input.questionType) ? "" : obj_Input.questionType);
                param[21] = new MySqlParameter("?option1", string.IsNullOrEmpty(obj_Input.option1) ? "" : obj_Input.option1);
                param[22] = new MySqlParameter("?option2", string.IsNullOrEmpty(obj_Input.option2) ? "" : obj_Input.option2);
                param[23] = new MySqlParameter("?rsvpRepeatDate", string.IsNullOrEmpty(obj_Input.repeatDateTime) ? "" : obj_Input.repeatDateTime);
                param[24] = new MySqlParameter("?IsSubGrpAdmin", string.IsNullOrEmpty(obj_Input.isSubGrpAdmin) ? "0" : obj_Input.isSubGrpAdmin);
                param[25] = new MySqlParameter("?subgrpIDs", subGrpIDs);

                var Result = _DbTouchbase.ExecuteStoreQuery<Imgname>
                    ("CALL V6_USPAddEvent(?eventID,?questionEnable,?eventType,?membersIDs,?eventImageID,?evntTitle,?evntDesc,?eventVenue,?venueLat,?venueLong,?evntDate,?publishDate,?expiryDate,?userID,?grpID,?sendSMSNonSmartPh,?sendSMSAll,?rsvpEnable,?rsvpRepeatDate,?questionId,?questionText,?questionType,?option1,?option2,?IsSubGrpAdmin,?subgrpIDs)",
                                        param).SingleOrDefault();
                return Result;
            }
            catch (Exception ex)
            {
                ManageExceptions.TraceException("Event/AddEvent", "AddEvent()", Convert.ToString(ex.InnerException), Convert.ToString(ex.Message), Convert.ToString(ex.StackTrace));
                throw;
            }

        }

        /// <summary>
        /// Created By : Nandu
        /// Returns List of Group Created Event List, Update Event List and Deleted Event List 
        /// created On 25/07/2016
        /// </summary>

        public static EventListNew GetEventListNew(string memberID, string grpID, out string Grp_SMSCount, string updatedOn)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[4];
                param[0] = new MySqlParameter("?memberId", memberID);
                param[1] = new MySqlParameter("?grpId", grpID);
                param[2] = new MySqlParameter("?SMS_Count", 0);
                param[3] = new MySqlParameter("?updatedOn", updatedOn);

                param[2].Direction = ParameterDirection.InputOutput;

                DataSet Result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V3_USPGetEventList", param);

                DataTable dt = Result.Tables[0];
                DataTable dt2 = Result.Tables[1];
                DataTable dt3 = Result.Tables[2];

                EventListNew res = new EventListNew();

                List<EventList1> createdList = GlobalFuns.DataTableToList<EventList1>(dt);

                if (dt.Rows.Count > 0)
                {
                    foreach (EventList1 eve in createdList)
                    {
                        if (!string.IsNullOrEmpty(eve.eventImg))
                        {
                            string event_Image = eve.eventImg.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Event/Group" + grpID + "/thumb/";
                            eve.eventImg = path + event_Image;
                        }
                    }
                }

                List<EventList1> updatedList = GlobalFuns.DataTableToList<EventList1>(dt2);

                if (dt2.Rows.Count > 0)
                {
                    foreach (EventList1 eve in updatedList)
                    {
                        if (!string.IsNullOrEmpty(eve.eventImg))
                        {
                            string event_Image = eve.eventImg.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Event/Group" + grpID + "/thumb/";
                            eve.eventImg = path + event_Image;
                        }
                    }
                }

                List<EventList1> deletedList = GlobalFuns.DataTableToList<EventList1>(dt3);

                if (dt3.Rows.Count > 0)
                {
                    foreach (EventList1 eve in deletedList)
                    {
                        if (!string.IsNullOrEmpty(eve.eventImg))
                        {
                            string event_Image = eve.eventImg.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Event/Group" + grpID + "/thumb/";
                            eve.eventImg = path + event_Image;
                        }
                    }
                }

                Grp_SMSCount = param[2].Value.ToString();

                res.CreatedEventList = createdList;
                res.UpdatedEventList = updatedList;
                res.DeletedEventList = deletedList;

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}