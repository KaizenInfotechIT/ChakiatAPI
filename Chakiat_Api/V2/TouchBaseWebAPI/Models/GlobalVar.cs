using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class GlobalVar
    {
        public static string strAppConn
        {
            get { return System.Configuration.ConfigurationManager.ConnectionStrings["TBWSConnectionString"].ToString(); }
        }

        public static string chatAppConn
        {
            get { return System.Configuration.ConfigurationManager.ConnectionStrings["ChatConnectionString"].ToString(); }
        }

    }
}