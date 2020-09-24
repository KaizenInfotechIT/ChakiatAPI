using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class clsTicket
    {
    }

    public class SendTicket
    {
        public string groupId { get; set; }
        public string memberProfileId { get; set; }
        public string smsText { get; set; }
        public string sendDateTime { get; set; }

        public string createBy { get; set; }
    }

    public class GetTicket
    {
        public string groupId { get; set; }
        public string memberProfileId { get; set; } 
        public string pageNo { get; set; }
    }

    public class GetTicketList
    {
        public string memberProfileId { get; set; }
        public string SMStext { get; set; }
        public string smsTime { get; set; }
    }
}