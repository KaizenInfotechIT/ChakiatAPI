using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class clsVersion
    {
    }

    public class clsVersionInput
    {
        public string Type { get; set; }
    }

    public class clsVersionList
    {
        public string pk_VID { get; set; }
        public string versionNumber { get; set; }
        public string DotNetVersion { get; set; }
        public string Description { get; set; }
        public string Creation_date { get; set; }
        public string URL { get; set; }
        public string type { get; set; }
    }
}