using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class clsEbulletin
    {
    }

    public class EbulletinSearch
    {
        public string memberProfileId { get; set; }
        public string groupId { get; set; }
        public string type { get; set; }
        public string isAdmin { get; set; }
        public string searchText { get; set; }
    }

    public class EbulletinList
    {
        public string ebulletinID { get; set; }
        public string  ebulletinTitle { get; set; }
        public string ebulletinlink { get; set; }
        public string ebulletinType { get; set; }

        public string filterType { get; set; }
        //public string ebulletinDate { get; set; }
        //public string ebulletinTime { get; set; }

        public string createDateTime { get; set; }
        public string publishDateTime { get; set; }
        public string expiryDateTime { get; set; } 

        public string isAdmin { get; set; }
        public string isRead { get; set; }
    }

    public class AddEbulletin
    {
        public string ebulletinID { get; set; }

        public string ebulletinType { get; set; }
        public string ebulletinTitle { get; set; }
        public string ebulletinlink { get; set; }
        public string ebulletinfileid { get; set; }

        public string memID { get; set; }
        public string grpID { get; set; }
        public string inputIDs { get; set; }

        public string sendSMSNonSmartPh { get; set; }
        public string sendSMSAll { get; set; }

        public string publishDate { get; set; }
     
        public string expiryDate { get; set; }

        public string isSubGrpAdmin { get; set; }
    }

    public class EbulletinDetails
    {
        public string ebulletinID { get; set; }
        public string memberProfileID { get; set; }
    }

    #region Version 3 API's

    public class InputGetYearWiseEbull
    {
        public string memberProfileId { get; set; }
        public string groupId { get; set; }
        public string fromYear { get; set; }
        public string toYear { get; set; }
    }

    #endregion

}