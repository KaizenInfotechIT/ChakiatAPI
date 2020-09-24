using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Text;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Net.Mail;

namespace TouchbaseInviteMember.App_Code
{
    public class JoinMember
    {
        public static DataSet SelfInsertToGroup(string name, string mobile, string email, string groupId, string country, int createBy)
        {
            MySqlParameter[] parameters = new MySqlParameter[7];
            parameters[0] = new MySqlParameter("?name", name);
            parameters[1] = new MySqlParameter("?mobile", mobile);
            parameters[2] = new MySqlParameter("?email", email);
            parameters[3] = new MySqlParameter("?groupId", groupId);
            parameters[4] = new MySqlParameter("?country", country);
            parameters[5] = new MySqlParameter("?createBy", createBy);
            //parameters[6] = new MySqlParameter("?profileID", 0);
            //parameters[6].Direction = ParameterDirection.InputOutput;
            string storeproc = "WebInsertMember";

            try
            {
                DataSet ds =MySqlHelper.ExecuteDataset(GlobalVars.strTBWSAppConn, CommandType.StoredProcedure, storeproc, parameters);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public static DataSet CountryCode(string country_id, string mobileNo, string groupID)
        {
            DataSet ds = new DataSet();

            string ssql = "select (SELECT country_code FROM country_master where country_master_id=" + country_id + ") as CountryCode,case when count((select IMEI_No from main_member_master where fk_country_id='" + country_id + "' and member_mobile='" + mobileNo + "')) > 0 then 'No' else 'Yes' end as 'sentSMS' ,case when (select SMSCount from group_master where pk_group_master_id='" + groupID + "') > 0 then 'Yes' else 'No' end as 'isSentSMS';";

            try
            {
                ds = MySqlHelper.ExecuteDataset(GlobalVars.strConn, CommandType.Text, ssql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        public static DataSet Fill_Combo_sql(string strQuery)
        {
            DataSet ds = new DataSet();
            try
            {
                ds = MySqlHelper.ExecuteDataset(GlobalVars.strConn, CommandType.Text, strQuery);
            }
            catch (Exception ex)
            {

            }

            return ds;
        }

        public static void FillCombo(DropDownList ddlList, DataSet ds, string strDataVal, string strDisplayVal, Boolean blIsBlank, string strInitialVal)
        {
            ddlList.Items.Clear();
            if (blIsBlank)
            {
                ddlList.DataSource = ds;
                ddlList.DataValueField = strDataVal;
                ddlList.DataTextField = strDisplayVal;
                ddlList.DataBind();
                ListItem InitialValue = new ListItem();
                InitialValue.Text = strInitialVal;
                InitialValue.Value = "0";
                ddlList.Items.Insert(0, InitialValue);
            }
            else
            {
                ddlList.DataSource = ds;
                ddlList.DataValueField = strDataVal;
                ddlList.DataTextField = strDisplayVal;
                ddlList.DataBind();
            }
        }

        public static string getCountryCode(string country)
        {
            MySqlParameter[] parameters = new MySqlParameter[1];
            parameters[0] = new MySqlParameter("?country", country);

            string storeproc = "SELECT country_code FROM country_master where country_master_id=@country";

            try
            {
                string countrycode = (string)MySqlHelper.ExecuteScalar(GlobalVars.strTBWSAppConn, CommandType.Text, storeproc, parameters);
                return countrycode;
            }
            catch
            {
                return "";
            }
        }

        public static void UpdateSMSCount(string GroupId)
        {
            string ssql = "update group_master set SMSCount=(SMSCount-1) where pk_group_master_id='" + GroupId + "'";

            try
            {
                MySqlHelper.ExecuteScalar(GlobalVars.strTBWSAppConn, CommandType.Text, ssql);
            }
            catch
            { }
        }

        public static string SendEmail(string FromMailID, string ToMailID, string MsgSubject, string MsgBody)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient();
                MailMessage message = new MailMessage();

                MailAddress fromAddress = new MailAddress(FromMailID);

                smtpClient.Host = ConfigurationManager.AppSettings["smtpserver"].ToString();
                smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"].ToString());
                smtpClient.EnableSsl = true;

                message.From = fromAddress;

                //MailAddress copy = new MailAddress("amod.m@kaizeninfotech.com");
                //message.CC.Add(copy);
                //MailAddress copy1 = new MailAddress("harita.koli@kaizeninfotech.com");
                //message.CC.Add(copy1);

                message.To.Add(ToMailID);
                message.Subject = MsgSubject;
                message.IsBodyHtml = true;
                message.Body = MsgBody;

                NetworkCredential basicAuthentication = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["frommail"].ToString(), ConfigurationManager.AppSettings["pass"].ToString());
                smtpClient.Credentials = basicAuthentication;

                try
                {
                    smtpClient.Send(message);
                }
                catch (Exception er)
                {
                    return "ServerError";
                }

                return string.Empty;

            }
            catch (SmtpFailedRecipientsException ex)
            {
                return "Error to send mail: " + ex.Message;
            }

        }
 
        public static bool SendSMS(string DomainName, string User, string Password, string SenderID, string PhoneNumber, string TextMessage)
        {
            string strValue = string.Empty;
            bool flag = false;

            try
            {
                StringBuilder sms = new StringBuilder();

                //sms.Append(DomainName);
                //sms.Append("?User=" + User);
                //sms.Append("&Password=" + Password);
                //sms.Append("&ReceiptRequested=Yes");
                //sms.Append("&sender=" + SenderID);
                //sms.Append("&PhoneNumber=" + PhoneNumber);
                //sms.Append("&Text=" + TextMessage);

                sms.Append(DomainName);
                sms.Append("?username=" + User + "&pass=" + Password);
                //sms.Append("&state=4");
                sms.Append("&dest_mobileno=" + PhoneNumber);
                sms.Append("&senderid=" + SenderID);
                sms.Append("&message=" + TextMessage);

                System.Net.WebRequest request = WebRequest.Create(sms.ToString());
                request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                if (responseFromServer.Contains("Ok"))
                {
                    strValue = responseFromServer;
                }
                response.Close();
                reader.Close();
                dataStream.Close();
                response.Close();

                //return strValue;
                flag = true;
            }
            catch (Exception ex)
            {
                strValue = "Error to send SMS: " + ex.ToString();
                //return strValue;
                flag = false;
            }
            return flag;
        }

        public static bool SendSMSInternational(string strMobile, string strAlertText, string CountryCode)
        {
            string strValue = string.Empty;
            bool flag = false;

            //ds.Clear();
            string dn = ConfigurationManager.AppSettings["dn_int"].ToString();
            string apikey = ConfigurationManager.AppSettings["apikey"].ToString();
            string sid = ConfigurationManager.AppSettings["sid_int"].ToString();

            string Number = CountryCode + strMobile;
            try
            {
                SendSMSInter(dn, apikey, sid, Number, strAlertText);
                flag = true;
            }
            catch //(Exception ex)
            {
                //strValue = "Error to send SMS: " + ex.ToString();
                //return strValue;
                flag = false;
            }
            //strValue = "SMS Send SuccessFully";
            //return strValue;
            return flag;
        }

        public static string SendSMSInter(string DomainName, string apikey, string SenderID, string PhoneNumber, string TextMessage)
        {
            string strValue = string.Empty;
            try
            {
                StringBuilder sms = new StringBuilder();
                //string txt = "Please enter one time password (OTP) " + TextMessage.Trim() + " to activate the mobile app you have just downloaded. It is valid only for 60 mins.";
                sms.Append(DomainName);
                sms.Append("?api_key=" + apikey);
                sms.Append("&method=sms");
                sms.Append("&sender=" + SenderID);
                sms.Append("&to=" + PhoneNumber);
                sms.Append("&message=" + TextMessage);

                WebRequest request = WebRequest.Create(sms.ToString());
                request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                if (responseFromServer.Contains("Ok"))
                {
                    strValue = responseFromServer;
                }
                response.Close();
                reader.Close();
                dataStream.Close();
                response.Close();

                return strValue;
            }
            catch (Exception ex)
            {
                strValue = "Error to send SMS: " + ex.ToString();
                return strValue;
            }
        }
    }
}