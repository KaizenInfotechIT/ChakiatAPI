using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class clsSetting
    {
    }

    public class GroupDetail
    {
        public string GroupId { get; set; }
        public string UpdatedValue { get; set; }
        public string mainMasterId { get; set; }
    }

    public class GetGroup
    {
        public string GroupId { get; set; }
        public string GroupProfileId { get; set; }
    }

    public class GrpSettingResult
    {
        public string GroupId { get; set; }
        public string ModuleId { get; set; }
        public string GroupProfileId { get; set; }

        public string UpdatedValue { get; set; }

        public string isGet { get; set; }

        public string showMobileSeflfClub { get; set; }
        public string showMobileOutsideClub { get; set; }
        public string showEmailSeflfClub { get; set; }
        public string showEmailOutsideClub { get; set; }
    }

    public class SettingDetails
    {
        public string grpId { get; set; }
        public string grpVal { get; set; }
        public string grpName { get; set; }
    }

    public class MainMasterId
    {
        public string mainMasterId { get; set; }
    }

    public class GRpSettingDetails
    {
        public string moduleId { get; set; }
        public string modVal { get; set; }
        public string modName { get; set; }
    }
}