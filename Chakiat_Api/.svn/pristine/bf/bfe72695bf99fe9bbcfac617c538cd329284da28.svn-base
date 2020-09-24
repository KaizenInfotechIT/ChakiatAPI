using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public static class GlobalVars
{
    public static string strConn
    {
        get
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["TBWSConnectionString"].ToString();
        }
    }

    public static string strTBWSAppConn
    {
        get
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["TBWSConnectionString"].ToString();//.Replace("TTTT", HttpContext.Current.Session["AppName"].ToString());
        }
    }

   

}
