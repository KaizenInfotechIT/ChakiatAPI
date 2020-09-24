using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TouchbaseInviteMember.App_Code;
using System.Data;
using System.Configuration;

namespace TouchbaseInviteMember
{
    public partial class Register : System.Web.UI.Page
    {
        #region
        public string GroupId
        {
            get
            {
                object o = ViewState["GroupId"];
                if (o != null) return (string)o;
                return "0";
            }
            set { ViewState["GroupId"] = value; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //Session["LoginMainID"] = 1;
            try
            {
                if (!String.IsNullOrEmpty(Request.QueryString["Invite"].ToString()))
                {
                    if (!IsPostBack)
                    {
                        GroupId = Request.QueryString["Invite"].ToString();
                        fillcountry();
                        lblGroupName.Text = GetGroupName();
                    }
                }
            }
            catch
            {

            }
            finally
            {
                GC.Collect();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtMemberName.Text;
                string Mobile = txtMobileNo.Text;
                string Email = txtEmail.Text;
                string country = ddlCountry.SelectedValue;

                DataSet ds = JoinMember.SelfInsertToGroup(name, Mobile, Email, GroupId, country, 0);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string masterId = ds.Tables[0].Rows[0]["masterId"].ToString();
                    string isGrpAdmin = ds.Tables[0].Rows[0]["isGrpAdmin"].ToString();

                    // Send SMS if Member is not exists in group
                    if (ds.Tables[0].Rows[0]["memExists"].ToString() == "N")
                    {

                        DataSet dsCountry = new DataSet();
                        dsCountry = JoinMember.CountryCode(country, Mobile, GroupId);

                        if (dsCountry.Tables[0].Rows[0]["isSentSMS"].ToString() == "Yes")
                        {
                            if (dsCountry.Tables[0].Rows[0]["CountryCode"].ToString() == "+91")//for India
                            {
                                if ((JoinMember.SendSMS(ConfigurationManager.AppSettings["dnapp"].ToString(), ConfigurationManager.AppSettings["usrapp"].ToString(), ConfigurationManager.AppSettings["pwdapp"].ToString(), ConfigurationManager.AppSettings["sidapp"].ToString(), Mobile, ds.Tables[0].Rows[0]["member_name"].ToString() + " has invited you to " + lblGroupName.Text + " mobile app powered by TouchBasE . Download link : http://goo.gl/uK2UQQ") == true))
                                {
                                    JoinMember.UpdateSMSCount(GroupId);
                                }
                            }
                            else//for International SMS
                            {
                                if ((JoinMember.SendSMSInternational(Mobile, ds.Tables[0].Rows[0]["member_name"].ToString() + " has invited you to " + lblGroupName.Text + " mobile app powered by TouchBasE . Download link : http://goo.gl/uK2UQQ", dsCountry.Tables[0].Rows[0]["CountryCode"].ToString())) == true)
                                {
                                    JoinMember.UpdateSMSCount(GroupId);
                                }
                            }
                        }
                        //send Mail to user
                        if (Email != "")
                        {
                            JoinMember.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), Email.ToString(), Session["LoginProfileName"].ToString() + " has added you to Entity " + lblGroupName.Text + " on TouchBasE.", msgbody(ds.Tables[0].Rows[0]["member_name"].ToString(), lblGroupName.Text));
                            //GlobalFuns.SendElasticEmail(Email, Session["LoginProfileName"].ToString() + " has added you to Entity " + Session["GroupName"].ToString() + " on TouchBase", "", msgbody(Session["LoginProfileName"].ToString(), Session["GroupName"].ToString()), ConfigurationManager.AppSettings["frommail"].ToString(), "touchbase.in");
                        }

                    }

                    System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
                    if (((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).IsMobileDevice)
                    {
                        //lblmsg.Text = "Browser is a mobile device.";
                        if (HttpContext.Current.Request != null && HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"] != null)
                        {
                            var u = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"].ToString();

                            if (u.Contains("Android"))
                            {
                                // Response.Redirect("https://goo.gl/9YqIl0");
                                //Response.Redirect("http://webtest.touchbase.in:8070/JoinTouchbase.aspx?groupId=" + GroupId + "&masterUID=" + masterId + "&isAdmin=" + isGrpAdmin + "&groupName=" + lblGroupName.Text);
                                string url = ConfigurationManager.AppSettings["imgPath"].ToString() + "InviteMember/Join.aspx?groupId=" + GroupId + "&masterUID=" + masterId + "&isAdmin=" + isGrpAdmin + "&groupName=" + lblGroupName.Text;
                                Response.Redirect(url);
                            }
                            else if (u.Contains("iPhone"))
                            {
                                Response.Redirect("https://itunes.apple.com/in/app/touchbase-tb/id1104294041?mt=8");
                            }
                        }
                    }
                    else
                    {
                        lblmsg.Text = "Browser is not a mobile device.Please open this url on you mobile";
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected void fillcountry()
        {
            JoinMember.FillCombo(ddlCountry, JoinMember.Fill_Combo_sql("SELECT DISTINCT country_master_id,country_master_name FROM country_master WHERE country_master_name!=''"), "country_master_id", "country_master_name", true, "--Select--");
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtcode.Text = JoinMember.getCountryCode(ddlCountry.SelectedValue.ToString());
        }

        public string GetGroupName()
        {
            try
            {
                string sql = "Select group_name from group_master where pk_group_master_id=" + GroupId;
                string grpName = (string)MySqlHelper.ExecuteScalar(GlobalVars.strTBWSAppConn, CommandType.Text, sql);
                return grpName;
            }
            catch
            {
                return "";
            }
        }

        public static string msgbody(string adminName, string groupName)
        {
            return "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>" +
                   "<html xmlns='http://www.w3.org/1999/xhtml'>" +
                   "<head>" +
                   "<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />" +
                   "<title>Mail</title>" +
                   "</head>" +
                   "<body style=' padding:5px; margin:0px; font-family:Verdana, Geneva, sans-serif; line-height:22px; text-align:center;'>" +
                   "<div id='page'>" +
                   "<div class='header'>" +
                   "<div class='logo'><img src='http://webtest.touchbase.in:8070//Images/mail-logo.png' alt='TouchBasE' width='200' height='95' style='width:200px; height:95px; margin:0px auto;' /></div>" +
                   "</div>" +
                   "<div class='container'>" +
                   "<h2 style='color:#000; line-height:30px;'><span style='color:#1a4994;'>" + adminName + "</span> has invited you to <span style='color:#1a4994;'>" + groupName + "</span> mobile app powered by TouchBase.</h2>" +
                   "<div class='abt' style='font-size:12px;'>TouchBase is an app that makes communications<br /> Focused, Real-Time and Controlled</div><br/>" +
                //"<p style='font-size:12px;'><span style='color:#1a4994;font-weight:bold;'>" + adminName + "</span> added you</p>" +

                   " <div style='width:290px; margin:0px auto;'> " +
                   " <div style='float:left; margin:0px 5px'> " +
                   " <a href='https://itunes.apple.com/us/app/touchbase-tb/id1104294041?ls=1&mt=8' target='_blank'>  " +
                   " <img src='http://webtest.touchbase.in:8070/Images/apple_ios_store.png' /> " +
                   " </a> </div><div style='float:left; margin:0px 5px'> " +
                   " <a href='https://play.google.com/store/apps/details?id=kaizen.app.com.touchbase' target='_blank'>  " +
                   " <img src='http://webtest.touchbase.in:8070/Images/google_play_store.png' /></a></div>  " +
                   " <div style='clear:both;'></div></div> " +

                   "<div class='link' style='font-size:12px; padding:5px;'> " +
                   "<div align='center'><a href='#'><img src='http://webtest.touchbase.in:8070/images/1460475507_facebook_social_media_online.png' width='32' height='32' /></a> <a href='#'><img src='http://webtest.touchbase.in:8070/Images/1460475484_twitter_social_media_online.png' width='32' height='32' /></a> <a href='#'><img src='http://webtest.touchbase.in:8070//Images/1460475497_linked_in_social_media_online.png' width='32' height='32' /></a> <a href='#'><img src='http://webtest.touchbase.in:8070//Images/1460475491_youtube_social_media_online.png' width='32' height='32' /></a> </div></div></div></div>" +
                   "</body>" +
                   "</html>";
        }
    }
}