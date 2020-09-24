using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class clsClubMonthlyReport
    {
    }
    public class ClubMonthlyReport_Input
    {
        public string profileId { get; set; }
        public string groupId { get; set; }
        public string month { get; set; }
        public string type { get; set; }
        public string Fk_ZoneID { get; set; }
    }
    public class ClubMonthlyReportList
    {
        public string clubName { get; set; }
        public string ClubId { get; set; }
        public string ReportDate { get; set; }
        public string Reporttime { get; set; }
        public string clubAG { get; set; }
        public string reportUrl { get; set; }
        public string ClubId1 { get; set; }
        public string SendToDistrictDate { get; set; }
        public string SendToDistrictTime { get; set; }
    }

    public class RootObjectTest
    {
        public List<MobileList> Mobilelist = new List<MobileList>();
        public List<Emaillist> Emaillist = new List<Emaillist>();
    }
    public class MobileList
    {
        public string country_code { get; set; }
        public string member_mobile_no { get; set; }
    }
    public class Emaillist
    {
        public string member_email_id { get; set; }
    }
}