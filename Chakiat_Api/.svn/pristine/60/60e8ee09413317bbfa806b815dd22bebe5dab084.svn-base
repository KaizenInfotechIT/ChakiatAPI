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

namespace TouchBaseWebAPI.Controllers
{
    public class EventController : ApiController
    {
        /// <summary>
        /// Returns List of Group Events for user
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns Event details by Event ID
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Search Event by text
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Answering Event Yes/No/ Maybe
        /// </summary>
        /// <param name="Event"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Add new Event and events repeate Date
        /// </summary>
        /// <param name="eventResult"></param>
        /// <returns></returns>
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
       
        /// <summary>
        /// Created By : Nandkishor K
        /// Created Date : 31/08/2016
        /// Reason : Returns List of Group Events for user Offline mode
        /// </summary>
        /// <param name="deletePhoto"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Returns Event details by Event ID for version 3
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Add new Event and events repeate Date for version 3(Madhavi Patil on 16 March 2018)
        /// </summary>
        /// <param name="eventResult"></param>
        /// <returns></returns>
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



        ///// <summary>
        ///// Returns Event details by Event ID(28-03-2018)
        ///// </summary>
        ///// <param name="event"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public object GetEventDetails_Club(eventdtl eve)
        //{
        //    dynamic EventsListDetailResult;
        //    List<object> EventsDetailResult = new List<object>();

        //    try
        //    {
        //        var Result = EventMaster.GetEventDetails(eve.grpId, eve.eventID, eve.groupProfileID);

        //        for (int i = 0; i < Result.Count; i++)
        //        {
        //            EventsDetailResult.Add(new { EventsDetail = Result[i] });
        //        }

        //        if (Result != null)
        //        {
        //            EventsListDetailResult = new { status = "0", message = "success", EventsDetailResult };
        //        }
        //        else
        //        {
        //            EventsListDetailResult = new { status = "0", message = "Record not found" };
        //        }
        //    }
        //    catch
        //    {
        //        EventsListDetailResult = new { status = "1", message = "failed" };
        //    }
        //    return new { EventsListDetailResult };
        //}
    }
}
