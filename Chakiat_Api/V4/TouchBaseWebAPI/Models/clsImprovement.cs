using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class clsImprovement
    {
    }
   
    public class ImprovementSearch
    {
        public string memberProfileId { get; set; }
        public string groupId { get; set; }
        public string type { get; set; }
        public string isAdmin { get; set; }
        public string searchText { get; set; }
    }

    public class ImprovementList
    {
        public string improvementID { get; set; }
        public string improvementTitle { get; set; }
        public string improvementDesc { get; set; }
        public string improvementImg { get; set; }
        public string filterType { get; set; }

        public string createDateTime { get; set; }
        public string publishDateTime { get; set; }
        public string expiryDateTime { get; set; }

        public string createDate { get; set; }
        public string publishDate { get; set; }
        public string expiryDate { get; set; }

        public string isAdmin { get; set; }
        public string isRead { get; set; }

        public string type { get; set; }
        public string profileIds { get; set; }

        public string sendSMSNonSmartPh { get; set; }
        public string sendSMSAll { get; set; }
    }

    public class AddImprovement
    {
        public string improvementID { get; set; }
        public string imprType { get; set; }
        public string improvementTitle { get; set; }
        public string improvementDesc { get; set; }

        public string memID { get; set; }
        public string grpID { get; set; }
        public string inputIDs { get; set; }

        public string improvementImg { get; set; }
        public string sendSMSNonSmartPh { get; set; }
        public string sendSMSAll { get; set; }
        public string publishDate { get; set; } 
        public string expiryDate { get; set; }

    }

    public class ImprovementDetail
    {
        public string improvementID { get; set; }
        public string grpID { get; set; }
        public string memberProfileID { get; set; }
    }
}