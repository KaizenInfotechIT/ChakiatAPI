using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class EventInfo
    {
    }

    public class EventList
    {
        public string eventID { get; set; }
        public string eventImg { get; set; }
        public string eventTitle { get; set; }
        public string eventDateTime { get; set; }
        public string goingCount { get; set; }
        public string maybeCount { get; set; }
        public string notgoingCount { get; set; }
        public string venue { get; set; }
        public string myResponse { get; set; }
        public string filterType { get; set; }
        public string grpID { get; set; }
        public string grpAdminId { get; set; }
        public string isRead { get; set; }
        public string venueLat { get; set; }
        public string venueLon { get; set; }
    }

    public class EventsDetail
    {
        public string eventID { get; set; }
        public string eventImg { get; set; }
        public string eventTitle { get; set; }
        public string eventType { get; set; }
        public string inputIds { get; set; }
        public string pubDate { get; set; }
        public string expiryDate { get; set; }
        public string eventDate { get; set; }
        public string eventDesc { get; set; }
        public string eventDateTime { get; set; }
        public string goingCount { get; set; }
        public string maybeCount { get; set; }
        public string notgoingCount { get; set; }
        public string venue { get; set; }
        public string myResponse { get; set; }
        public string filterType { get; set; }
        public string grpID { get; set; }
        public string grpAdminId { get; set; }
        public string totalCount { get; set; }
        public string venueLat { get; set; }
        public string venueLon { get; set; }
        public string sendSMSNonSmartPh { get; set; }
        public string sendSMSAll { get; set; }
        public string isQuesEnable { get; set; }
        public string rsvpEnable { get; set; }
        public string repeatDateTime { get; set; }
        public string questionId { get; set; }
        public string questionType { get; set; }
        public string questionText { get; set; }
        public string option1 { get; set; }
        public string option2 { get; set; }
    }

    public class EventsDetailResult
    {
        public EventsDetail EventsDetail { get; set; }
    }

    public class eventdtl
    {
        public string groupProfileID { get; set; }
        public string grpId { get; set; }
        public string eventID { get; set; }
        public string SearchText { get; set; }
        public string type { get; set; }
        public string admin { get; set; }
        public string page { get; set; }
        public string updatedOn { get; set; }
    }

    public class EventJoinResult
    {
        public string goingCount { get; set; }
        public string maybeCount { get; set; }
        public string notgoingCount { get; set; }
        public string myResponse { get; set; }
    }

    public class RepeatEvent
    {
        public string eventDate { get; set; }
        public string eventTime { get; set; }
    }

    public class RepeatEventResult
    {
        public RepeatEvent repeatEvent { get; set; }
    }

    public class AnsweringEvent
    {
        public string grpID { get; set; }
        public string profileID { get; set; }
        public string eventId { get; set; }
        public string joiningStatus { get; set; }
        public string questionId { get; set; }
        public string answerByme { get; set; }
    }

    public class AddEventResult
    {
        public string eventID { get; set; }
        public string questionEnable { get; set; }
        public string eventType { get; set; }
        public string membersIDs { get; set; }
        public string eventImageID { get; set; }
        public string evntTitle { get; set; }
        public string evntDesc { get; set; }
        public string eventVenue { get; set; }
        public string venueLat { get; set; }
        public string venueLong { get; set; }
        public string evntDate { get; set; }
        public string sendSMSAll { get; set; }
        public string sendSMSNonSmartPh { get; set; }
        public string publishDate { get; set; }
        public string expiryDate { get; set; }
        public string notifyDate { get; set; }
        public string userID { get; set; }
        public string grpID { get; set; }
        public string repeatDateTime { get; set; }
        public string rsvpEnable { get; set; }
        public string questionId { get; set; }
        public string questionType { get; set; }
        public string questionText { get; set; }
        public string option1 { get; set; }
        public string option2 { get; set; }
        public string isSubGrpAdmin { get; set; }
    }

    public class QuestionList
    {
        public string questionId { get; set; }
        public string questionType { get; set; }
        public string questionText { get; set; }
        public string option1 { get; set; }
        public string option2 { get; set; }
    }

    public class QuestionArray
    {
        public QuestionList QuestionList { get; set; }
    }

    /// <summary>
    /// Written by Nandu
    /// created on 25/07/2016
    /// </summary>
    public class EventListNew
    {
        public List<EventList1> CreatedEventList { get; set; }
        public List<EventList1> UpdatedEventList { get; set; }
        public List<EventList1> DeletedEventList { get; set; }
    }

    public class EventList1
    {
        public string eventID { get; set; }
        public string eventImg { get; set; }
        public string eventTitle { get; set; }
        public string eventDesc { get; set; }
        public string eventDateTime { get; set; }
        public string goingCount { get; set; }
        public string maybeCount { get; set; }
        public string notgoingCount { get; set; }
        public string venue { get; set; }
        public string myResponse { get; set; }
        public string filterType { get; set; }
        public string grpID { get; set; }
        public string grpAdminId { get; set; }
        public string isRead { get; set; }
        public string venueLat { get; set; }
        public string venueLon { get; set; }
    }    
}