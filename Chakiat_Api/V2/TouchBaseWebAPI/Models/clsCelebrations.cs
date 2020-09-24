using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class clsCelebrations
    {
      
    }

    public class ClsMonthCalenderInput
    {
        public string profileId { get; set; }
        public string groupId { get; set; }
        public string selectedDate { get; set; }//Format:"yyyy-mm-dd"
        public string updatedOn { get; set; }//Format:"yyyy-mm-dd hh:mm"
        public string groupCategory { get; set; }
    }

    public class ClsMonthCalenderOutput
    {
        public List<CalenderEventList> newEvents { get; set; }
        public List<CalenderEventList> updatedEvents { get; set; }
        public List<CalenderEventList> deletedEvents { get; set; }
    }

    public class CalenderEventList
    {
        public string uniqueID { get; set; }
        public string groupID { get; set; }
        public string eventDate { get; set; }
        public string type { get; set; } // BD-BirthDay ,Ann- anniversary, Evt- Event
        public string typeID { get; set; } 
        public string title { get; set; }
        public string memberFamilyID { get; set; }
    }

    public class ClsTodayBirthday
    {
        public string profileId { get; set; }
        public string groupID { get; set; }
        public string memberName { get; set; }
        public string memberMobile { get; set; }
        public string relation { get; set; }
        public string msg { get; set; }
    }
}