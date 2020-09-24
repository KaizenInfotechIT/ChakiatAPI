using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class clsPastPresidents
    {
    }

    public class clsPastPresidentsInput
    {
        public string GroupId { get; set; }
        public string SearchText { get; set; }
        public string updateOn { get; set; }
    }

    public class clsPastPresidentsOutput
    {
        public List<clsPastPresident> newRecords { get; set; }
        public List<clsPastPresident> updatedRecords { get; set; }
        public string deletedRecords { get; set; }
    }

    public class clsPastPresident
    {
        public string PastPresidentId { get; set; }
        public string MemberName { get; set; }
        public string PhotoPath { get; set; }
        public string TenureYear { get; set; }
    }
}