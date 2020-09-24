using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class clsAttendance
    {
    }

    public class clsGetAttendanceList
    {
        public string groupProfileID { get; set; }
        public string moduleID { get; set; }
        public string month { get; set; }
        public string year { get; set; }
    }

    public class AttendanceList
    {
        public string idno { get; set; }
        public string name { get; set; }
        public string attendence { get; set; }
    }
}