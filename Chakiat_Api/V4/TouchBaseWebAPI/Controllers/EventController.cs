using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.Data;
using TouchBaseWebAPI.BusinessEntities;
using System.Web;
using System.Configuration;
using System.Data;

namespace TouchBaseWebAPI.Controllers
{
    public class EventController : ApiController
    {
        [HttpPost]
        public object GetEventList(eventdtl eve)
        {
            dynamic EventListDetailResult;
            string grp_SMSCount;
            List<object> EventsListResult = new List<object>();
            int pagesize = 10, pageno = 1, total;
            if (string.IsNullOrEmpty(eve.SearchText))
            {
                eve.SearchText = "";
            }
            if (!string.IsNullOrEmpty(eve.page))
            {
                pageno = Convert.ToInt32(eve.page);
            }
            int skippageno = pageno - 1;
            try
            {
                List<EventList> Result = EventMaster.GetEventList(eve.groupProfileID, eve.grpId, eve.SearchText, eve.type, eve.admin, out grp_SMSCount);
                for (int i = 0; i < Result.Count; i++)
                {
                    EventsListResult.Add(new { EventList = Result[i] });
                }

                if (Result.Count > 0)
                {
                    var totalPages = 1;
                    if (string.IsNullOrEmpty(eve.page))
                    {
                        total = Result.Count;
                        EventListDetailResult = EventsListResult.ToList();
                    }
                    else
                    {
                        total = Result.Count;
                        totalPages = (int)Math.Ceiling((double)total / pagesize);
                        EventsListResult = EventsListResult.Skip(pagesize * skippageno).Take(pagesize).ToList();
                    }

                    EventListDetailResult = new { status = "0", message = "success", SMSCount = grp_SMSCount, resultCount = total.ToString(), TotalPages = totalPages.ToString(), currentPage = pageno.ToString(), EventsListResult = EventsListResult };
                }
                else
                {
                    EventListDetailResult = new { status = "1", SMSCount = grp_SMSCount, message = "Record not found" };
                }
            }
            catch
            {
                EventListDetailResult = new { status = "1", message = "failed", SMSCount = 0 };
            }

            return new { EventListDetailResult };
        }

        [HttpPost]
        public object GetEventDetails(eventdtl eve)
        {
            dynamic EventsListDetailResult;
            List<object> EventsDetailResult = new List<object>();

            try
            {
                var Result = EventMaster.GetEventDetails(eve.grpId, eve.eventID, eve.groupProfileID);

                for (int i = 0; i < Result.Count; i++)
                {
                    EventsDetailResult.Add(new { EventsDetail = Result[i] });
                }

                if (Result != null)
                {
                    EventsListDetailResult = new { status = "0", message = "success", EventsDetailResult };
                }
                else
                {
                    EventsListDetailResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                EventsListDetailResult = new { status = "1", message = "failed" };
            }
            return new { EventsListDetailResult };
        }

        [HttpGet]
        public object GetEventBySearchText(string searchText)
        {
            dynamic TBGroupResult;
            List<object> EventsListResult = new List<object>();
            if (string.IsNullOrEmpty(searchText))
            {
                searchText = "";
            }
            try
            {
                List<EventsDetail> Result = EventMaster.GetEventBySearchText(searchText);
                for (int i = 0; i < Result.Count; i++)
                {
                    EventsListResult.Add(new { EventList = Result[i] });
                }
                if (Result != null)
                {
                    TBGroupResult = new { status = "0", message = "success", EventsListResult = EventsListResult };
                }
                else
                {
                    TBGroupResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGroupResult = new { status = "1", message = "failed" };
            }
            return new { TBGroupResult };
        }

        [HttpPost]
        public object AnsweringEvent(AnsweringEvent obj_Responce)
        {
            dynamic EventJoinResult;
            try
            {

                EventJoinResult EventResult = EventMaster.AnsweringEvent(obj_Responce);
                if (EventResult.goingCount != null)
                {
                    EventJoinResult = new { status = "0", message = "success", goingCount = EventResult.goingCount, maybeCount = EventResult.maybeCount, notgoingCount = EventResult.notgoingCount, myResponse = EventResult.myResponse };
                }
                else
                {
                    EventJoinResult = new { status = "1", message = "failed" };
                }
            }
            catch
            {
                EventJoinResult = new { status = "1", message = "failed" };
            }
            return new { EventJoinResult };
        }

        [HttpPost]
        public object AddEvent(AddEventResult eventResult)
        {
            dynamic AddEventResult;
            int str;
            try
            {
                Imgname result = EventMaster.AddEvent(eventResult);
                if (!string.IsNullOrEmpty(result.imgName))
                {
                    str = GlobalFuns.UploadImage(eventResult.grpID, result.imgName, "Event");
                }
                else
                    str = 0;
                if (result != null)
                {

                    if (str == 0)
                    {
                        AddEventResult = new { status = "0", message = "success" };
                        if (eventResult.eventID != "0")
                        {
                            //string url = ConfigurationManager.AppSettings["imgPath"] + "php/EditEvent.php?EventID=" + eventResult.eventID;
                            //GroupMaster.Send(url);
                        }
                    }
                    else
                        AddEventResult = new { status = "1", message = "failed" };
                }
                else
                {
                    AddEventResult = new { status = "1", message = "failed" };
                }
            }
            catch
            {
                AddEventResult = new { status = "1", message = "failed" };
            }
            return new { AddEventResult };
        }
       
        [HttpPost]
        public object GetEventListNew(eventdtl eve)
        {
            dynamic EventListDetailResult;
            string grp_SMSCount;
            List<object> EventsListResult = new List<object>();

            try
            {
                EventListNew Result = EventMaster.GetEventListNew(eve.groupProfileID, eve.grpId, out grp_SMSCount, eve.updatedOn);

                //for (int i = 0; i < Result.Count; i++)
                //{
                //    EventsListResult.Add(new { EventList = Result[i] });
                //}

                if (Result != null)
                {
                    EventListDetailResult = new { status = "0", message = "success", smsCount = grp_SMSCount, updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), EventsListResult = Result };
                }
                else
                {
                    EventListDetailResult = new { status = "1", smsCount = grp_SMSCount, updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), message = "Record not found" };
                }
            }
            catch
            {
                EventListDetailResult = new { status = "1", message = "failed", smsCount = 0, updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), };
            }

            return new { EventListDetailResult };
        }

        [HttpPost]
        public object GetEventDetails_New(eventdtl eve)
        {
            dynamic EventsListDetailResult;
            List<object> EventsDetailResult = new List<object>();

            try
            {
                var Result = EventMaster.GetEventDetails_New( eve.eventID, eve.groupProfileID);

                for (int i = 0; i < Result.Count; i++)
                {
                    EventsDetailResult.Add(new { EventsDetail = Result[i] });
                }

                if (Result != null)
                {
                    EventsListDetailResult = new { status = "0", message = "success", EventsDetailResult };
                }
                else
                {
                    EventsListDetailResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                EventsListDetailResult = new { status = "1", message = "failed" };
            }
            return new { EventsListDetailResult };
        }

        [HttpPost]
        public object AddEvent_New(AddEventResult eventResult)
        {
            dynamic AddEventResult;
            int str;
            try
            {
                Imgname result = EventMaster.AddEvent_New(eventResult);
                if (!string.IsNullOrEmpty(result.imgName))
                {
                    str = GlobalFuns.UploadImage(eventResult.grpID, result.imgName, "Event");
                }
                else
                    str = 0;
                if (result != null)
                {

                    if (str == 0)
                    {
                        AddEventResult = new { status = "0", message = "success" };
                        if (eventResult.eventID != "0")
                        {
                            //string url = ConfigurationManager.AppSettings["imgPath"] + "php/EditEvent.php?EventID=" + eventResult.eventID;
                            //GroupMaster.Send(url);
                        }
                    }
                    else
                        AddEventResult = new { status = "1", message = "failed", Exception = "1" };
                }
                else
                {
                    AddEventResult = new { status = "1", message = "failed", Exception = "2" };
                }
            }
            catch(Exception e)
            {
                AddEventResult = new { status = "1", message = "failed", Exception = e };
            }
            return new { AddEventResult };
        }

        [System.Web.Http.HttpPost]
        public object Getsmscountdetails(smscountInputs sms)
        {
            dynamic TBsmscountResult="";
            try
            {
                int smsbalance = 0;
                int totalmembercount = 0;
                int Repeatcount = 0;
                int totalsms = 0;
                string msg = "";
                
                DataSet ds_GetCount = EventMaster.Getsmscountdetails(sms);

                if (ds_GetCount.Tables[1].Rows.Count > 0 && ds_GetCount.Tables[1].Rows[0]["p_MemCount"].ToString() != "0")
                {
                    //check sms balance
                    if (ds_GetCount.Tables[0].Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(ds_GetCount.Tables[0].Rows[0]["SMSCount"].ToString()))
                        {
                            smsbalance = Convert.ToInt32(ds_GetCount.Tables[0].Rows[0]["SMSCount"]);
                        }
                        else
                        {
                            smsbalance = 0;
                        }
                    }
                    else
                    {
                        smsbalance = 0;
                    }
                    //check member count
                    if (ds_GetCount.Tables[1].Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(ds_GetCount.Tables[1].Rows[0]["p_MemCount"].ToString()))
                        {
                            totalmembercount = Convert.ToInt32(ds_GetCount.Tables[1].Rows[0]["p_MemCount"]);
                        }
                        else
                        {
                            totalmembercount = 0;
                        }
                    }
                    else
                    {
                        totalmembercount = 0;
                    }

                    if (Convert.ToInt32(sms.p_Remindercount) > 0)
                    {
                        Repeatcount = Convert.ToInt32(sms.p_Remindercount);
                    }
                    else
                    {
                        Repeatcount = 0;
                    }

                    int msgcount = 0;
                    int TextLenght = Convert.ToInt32(sms.p_DescriptionCount);
                    if ((TextLenght % 160) > 0)
                    {
                        msgcount = (TextLenght / 160) + 1;
                    }
                    else
                    {
                        msgcount = TextLenght / 160;
                    }

                    //if (Repeatcount > 0)
                    //{
                    //    totalsms = ((totalmembercount * msgcount)) + ((Repeatcount * msgcount) * totalmembercount);
                    //}
                    //else
                    //{
                    //    totalsms = totalmembercount * msgcount;
                    //}

                    if (Convert.ToInt32(sms.p_PublishDateFlag) == 1)
                    {
                        if (Repeatcount > 0)
                        {
                            totalsms = ((totalmembercount * msgcount)) + ((Repeatcount * msgcount) * totalmembercount);
                        }
                        else
                        {
                            totalsms = totalmembercount * msgcount;
                        }
                    }
                    else
                    {
                        if (Repeatcount > 0)
                        {
                            totalsms = ((Repeatcount * msgcount) * totalmembercount);
                        }
                        else
                        {
                            totalsms = 0;
                        }
                    }

                    if (smsbalance < totalsms)
                    {
                        msg = "Description length : " + TextLenght + " characters\nNo. of messages per user : " + msgcount.ToString() + "\nTotal No of Members selected : " + totalmembercount + "\nNo of Reminder : " + Repeatcount + "\nTotal no of messages : " + totalsms + "\nSMS Balance : " + smsbalance + ".\nYour SMS Balance is low to send sms.";
                        TBsmscountResult = new { status = "0", message = "success", smsResult = msg, smsperuser = msgcount };
                    }
                    else
                    {
                        TBsmscountResult = new { status = "0", message = "success", smsResult = msg, smsperuser = msgcount };
                    }
                }
                else
                {
                    TBsmscountResult = new { status = "1", message = "Record not found", smsResult = msg, smsperuser = 0 };
                }
            }
            catch
            {
                TBsmscountResult = new { status = "1", message = "failed", smscount = 0 };
            }

            return new { TBsmscountResult };
        }

    }
}
