using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TouchbaseInviteMember
{
    public partial class JoinTouchbase : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LoginMainID"] = 1;
            var u = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"].ToString();
            if (u.Contains("Android"))
            {
                 Response.Redirect("https://goo.gl/9YqIl0");
               
            }
            else if (u.Contains("iPhone"))
            {
                Response.Redirect("https://itunes.apple.com/in/app/touchbase-tb/id1104294041?mt=8");
            }
        }
    }
}