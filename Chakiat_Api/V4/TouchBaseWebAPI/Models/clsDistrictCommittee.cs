using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class clsDistrictCommittee
    {
    }
    public class districtCommitteeInput
    {
        public string groupID { get; set; }
        public string searchText { get; set; }
        public string DistrictCommitteID { get; set; }
        public string Yearfilter { get; set; }
    }

    public class districtCommitteeWithoutCatList
    {
        public string Fk_DistrictCommitteeID { get; set; }
        public string fk_Member_profileID { get; set; }
        public string name { get; set; }
        public string MobileNumber { get; set; }
        public string MailID { get; set; }
        public string DistrictDesignation { get; set; }
        public string ClubName { get; set; }
        public string MobileCodeID { get; set; }
        public string img { get; set; }
        public string type { get; set; }
        public string classification { get; set; }
        public string Keywords { get; set; }
        public string BusinessName { get; set; }
        public string Designation { get; set; }
        public string BusinessAddress { get; set; }
        public string RotaryID { get; set; }
        public string DonarReco { get; set; }
    }
    public class districtCommitteeWithCatList
    {
        public string Fk_DistrictCommitteeID { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }
    public class Yearlist
    {
        public string Yearvalue { get; set; }
    }

    public class districtCommittee
    {
        public List<districtCommitteeWithoutCatList> districtCommitteeWithoutCatList { get; set; }
        public List<districtCommitteeWithCatList> districtCommitteeWithCatList { get; set; }
        public List<Yearlist> Yearlist { get; set; }
        public string CurrentYear { get; set; }
    }
}