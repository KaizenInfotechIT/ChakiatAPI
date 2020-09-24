using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class clsSubgrpDirectory
    {

    }

    public class SubGrpDirectoryResult
    {
        public List<SubGrpListResult> subGrpList { get; set; }
        public List<MemberList> memberList { get; set; }
        public List<DesignationList> classification { get; set; }
    }

    public class DesignationList
    {
        public string classification { get; set; }
    }
    public class MemberList
    {
        public string masterUID { get; set; }
        public string grpID { get; set; }
        public string profileID { get; set; }
        public string memberName { get; set; }
        public string membermobile { get; set; }
        public string pic { get; set; }
        public string distDesignation { get; set; }
        public string classification { get; set; }
    }

    public class SubGrpListResult
    {
        public string subgrpId { get; set; }
        public string subgrpTitle { get; set; }
        public string noOfmem { get; set; }
        public string hasSubgroup { get; set; }
    }

    public class SubGrpDirectoryInput
    {

        public string profileId { get; set; }
        public string groupId { get; set; }
        public string parentID { get; set; }
        public string memberMainId { get; set; }
    }
}