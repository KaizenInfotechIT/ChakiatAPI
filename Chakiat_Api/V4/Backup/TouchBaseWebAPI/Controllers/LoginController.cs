using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Configuration;
using System.Text;
using TouchBaseWebAPI.Data;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;

using System.Threading.Tasks;
using System.Data;

namespace TouchBaseServices.Controllers
{
    public class LoginController : ApiController
    {
        private TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        
        [HttpPost]
        [System.Web.Mvc.OutputCache(Duration = 10)]
        public object UserLogin(UserLogin user)
        {
            dynamic LoginResult;
            try
            {
               

                string strOTP = GlobalFuns.CreateRandomPassword(4);
                DataSet ds = new DataSet();

                ds = Login.GetMembersList(user);

                if (ds != null)
                {
                    if (ds.Tables[0] != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            bool flag = SendSMSAndEmailToUser(user, strOTP, ds.Tables[0].Rows[0]["emailId"].ToString(), ds.Tables[0].Rows[0]["memberName"].ToString(), ds.Tables[0].Rows[0]["groupName"].ToString());

                            if (flag == true)
                                LoginResult = new { status = "0", message = "success", otp = strOTP };
                            else
                                LoginResult = new { status = "1", message = "failed", otp = 0 };
                        }
                        else
                        {
                            LoginResult = new { status = "0", message = "Member not registered", otp = 0 };
                        }
                    }
                    else
                    {
                        LoginResult = new { status = "0", message = "Member not registered", otp = 0 };
                    }
                }
                else
                {
                    LoginResult = new { status = "0", message = "Member not registered", otp = 0 };
                }
            }
            catch(Exception ex)
            {
                LoginResult = new { status = "1", message = "failed", otp = 0, error = ex.Message };
            }
            return new { LoginResult };
        }

        #region UserDefinedFunctions

        [NonAction]
        private bool SendSMSAndEmailToUser(UserLogin user, string OTP, string emailId, string memberName, string clubName)
        {
            bool flag = false;

            try
            {
                //send OTP to mobile through SMS
                if (user.countryCode == "1")//for India
                {
                    //Added by Nandu on 07/12/2016 Task --> Run Parellel functionality of OTP as well as mail send to user
                    Task task1 = Task.Factory.StartNew(() => sendNationalOTP(user.mobileNo, OTP));
                    Task task2 = null;

                    if (emailId != null)
                    {
                        string email = string.IsNullOrEmpty(emailId) ? "securitycode@kaizeninfotech.com,milan.haldankar@kaizeninfotech.com" : "securitycode@kaizeninfotech.com,milan.haldankar@kaizeninfotech.com," + emailId.ToString().Trim();
                        task2 = Task.Factory.StartNew(() => sendMail(email, OTP, memberName, user.mobileNo, clubName));
                    }
                    else
                        task2 = Task.Factory.StartNew(() => sendMail("securitycode@kaizeninfotech.com,milan.haldankar@kaizeninfotech.com", OTP, memberName, user.mobileNo, clubName));

                    Task.WaitAll(task1);
                }
                else
                {
                    int conCode=int.Parse(user.countryCode);
                    var countryCode = (from e in _DBTouchbase.country_master
                                       where e.country_master_id == conCode
                                       select e.country_code).FirstOrDefault();

                    Task task1 = Task.Factory.StartNew(() => sendInternationalNationalOTP(user.mobileNo, OTP, countryCode.ToString()));

                    Task task2 = null;
                    if (emailId != null)
                    {
                        string email = string.IsNullOrEmpty(emailId) ? "securitycode@kaizeninfotech.com" : "securitycode@kaizeninfotech.com," + emailId.ToString().Trim();
                        task2 = Task.Factory.StartNew(() => sendMail(email, OTP, memberName, user.mobileNo, clubName));
                    }
                    else
                        task2 = Task.Factory.StartNew(() => sendMail("securitycode@kaizeninfotech.com", OTP, memberName, user.mobileNo, clubName));

                    Task.WaitAll(task1);
                }

                flag = true;
            }
            catch
            {
            }

            return flag;
        }

        /// <summary>
        /// Send National OTP to User
        /// Created on : 07/12/2016
        /// Author : Nandu
        /// </summary>
        [NonAction]
        string sendNationalOTP(string mobile, string otp)
        {
            //Smsjust Provider
            //GlobalFuns.SendSMSOnAdd(mobile, otp + " is your one time password(OTP). Please enter the OTP to proceed. Thank you," + System.Environment.NewLine + "Team ROW");
            GlobalFuns.SendSMSOnAdd(mobile, @"<%23> " + otp + " is your one time password(OTP). Please enter your OTP to proceed." + System.Environment.NewLine + "Thank you, Team chakiat" + System.Environment.NewLine + "G2sqeNpT3wC");
            //MVaayoo Provider
            //GlobalFuns.SendAlertSMS(mobile, otp + " is your one time password(OTP). Please enter the OTP to proceed. Thank you, Team ROW");
            
            return "true";
        }

        /// <summary>
        /// Send International OTP to User
        /// Created on : 07/12/2016
        /// Author : Nandu
        /// </summary>
        [NonAction]
        string sendInternationalNationalOTP(string mobile, string otp, string countyCode)
        {
            GlobalFuns.SendSMSInternational(mobile, otp + " is your one time password(OTP). Please enter the OTP to proceed. Thank you," + System.Environment.NewLine + "Team chakiat", countyCode);
            return "true";
        }

        /// <summary>
        /// Send Mail to user
        /// Created on : 07/12/2016
        /// Author : Nandu
        /// </summary>
        [NonAction]
        string sendMail(string email, string otp, string memberName, string mobile, string clubName )
        {
            GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), email, "Chakiat- OTP for mobile application", mailbody(memberName, mobile, otp, clubName));
            return "true";
        }

        [NonAction]
        private string mailbody(string memberName, string mobile, string OTP, string clubName)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table width='100%' border='0'>");

            sb.Append("<tr><td>Dear ");
            sb.Append(memberName);
            sb.Append("(");
            sb.Append(mobile);
            sb.Append(")</td></tr>");
            sb.Append("<tr><td>The security code for accessing on your mobile is: ");
            sb.Append(OTP);
            sb.Append("<br /><br />Please type this code when prompted while installing the app.</td></tr>");
            sb.Append("<tr><td><p>Thank you<br /><br />Regards,<br /><br />Team Chakiat<br /><br /></p></td> </tr>");
            sb.Append("<tr><td>&nbsp;</td> </tr></table>");

            return sb.ToString();
        }

        /// <summary>
        /// User defined functions
        /// Created by : Nandu
        /// Created on : 05/05/2017
        /// Task : Admin mail body
        /// </summary>
        [NonAction]
        private string mailBody(string mobileNo,  string name, string emailId )
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>");
            sb.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");
            sb.Append("<head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            sb.Append("<title>Roster on wheels</title></head>");
            sb.Append("<body><table width='600' border='0' cellpadding='5'>");

            sb.Append("<tr><td width='103'>Mobile number: </td>");
            sb.Append("<td width='471'>");
            sb.Append(mobileNo);
            sb.Append("</td></tr>");

            //sb.Append("<tr><td width='103'>Is Rotarian: </td>");
            //sb.Append("<td width='471'>");
            //sb.Append(isRotarian);
            //sb.Append("</td></tr>");

            sb.Append("<tr><td width='103'>Name: </td>");
            sb.Append("<td width='471'>");
            sb.Append(name);
            sb.Append("</td></tr>");

            sb.Append("<tr><td>Email Id: </td>");
            sb.Append("<td>");
            sb.Append(emailId);
            sb.Append("</td></tr>");

            //sb.Append("<tr><td>Club Name: </td>");
            //sb.Append("<td>");
            //sb.Append(club);
            //sb.Append("</td></tr>");

            //sb.Append("<tr><td>Feedback: </td>");
            //sb.Append("<td>");
            //sb.Append(feedback);
            //sb.Append("</td></tr>");

            sb.Append("</table></body>");
            sb.Append("</html>");

            return sb.ToString();
        }

        /// <summary>
        /// Created by : Nandu
        /// Created on : 05/05/2017
        /// Task : Customer mail body 
        /// </summary>
        /// <returns></returns>
        [NonAction]
        private string customerMailBody()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>");
            sb.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");
            sb.Append("<head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            sb.Append("<title></title></head>");
            sb.Append("<body>");
            sb.Append("<table width='100%' border='0' cellpadding='5' cellspacing='5' style='font-family: Roboto; font-size:14px;'>");
            sb.Append("<tr><td><strong>Dear Sir/Ma'am</strong></td></tr>");
            sb.Append("<tr>");
            sb.Append("<td>Thank you for Enquiring. A team member will contact you as soon as possible with a detailed explanation of the product that fits your");
            sb.Append(" business need.<br /> <br />Thanks again for your Enquiry.</td>");
            sb.Append("</tr>");
            sb.Append("<tr><td><strong>Sincerely,<br />Team Chakiat</strong></td></tr>");
            sb.Append("</table>");
            sb.Append("</body></html>");

            return sb.ToString();
        }

        #endregion

        [HttpPost]

        public object PostOTP(UserLogin user)
        {
            dynamic LoginResult;
            try
            {
                DataSet ds_data = Login.GetMembersPostOTP(user);

                if (ds_data.Tables[0].Rows.Count > 0)
                {
                    LoginResult = new { status = "0", message = "success", isexists = "true", masterUID = ds_data.Tables[0].Rows[0]["masterUID"].ToString(), membername = ds_data.Tables[0].Rows[0]["member_name"].ToString() };
                    //MemberMaster.UpdateDeviceToken(user.deviceToken, masterId.ToString());
                }
                else
                {
                    LoginResult = new { status = "0", message = "success", isexists = "false" };
                }

            }
            catch
            {
                LoginResult = new { status = "1", message = "failed" };
            }
            return new { LoginResult };
        }

        [HttpPost]
        public object GetWelcomeScreen(UserLogin user)
        {
            string message; string Status;
            int uId = Convert.ToInt32(user.masterUID);
            var memberName = "";
            //var grpPartResults = "";
            try
            {
                List<object> grpPartResults = new List<object>();

                DataSet ds_Data= Login.Get_Welcomescreen_Data(user);
                List<GrpPartResult> GrpPartResult = null;
                if (ds_Data.Tables[0].Rows.Count > 0)
                {
                    memberName = ds_Data.Tables[0].Rows[0]["member_name"].ToString();
                }
                if (ds_Data.Tables[1].Rows.Count > 0)
                {
                    GrpPartResult = GlobalFuns.DataTableToList<GrpPartResult>(ds_Data.Tables[1]);
                }

                for (int i = 0; i < GrpPartResult.Count; i++)
                {
                    grpPartResults.Add(new { GrpPartResult = GrpPartResult[i] });
                }

              
                if (grpPartResults.Count > 0)
                {
                    message = "Success"; Status = "0";
                    var WelcomeResult = new { status = Status, message = message, Name = memberName, grpPartResults };
                    return new
                    {
                        WelcomeResult
                    };
                }
                else
                {
                    var WelcomeResult = new { status = "1", message = "User does not belong to any group", grpPartResults };
                    return new
                    {
                        WelcomeResult
                    };
                }
            }
            catch
            {
                var WelcomeResult = new { status = "1", message = "An error occurred, please try again or contact the administrator" };
                return new
                {
                    WelcomeResult
                };
            }
        }

        [HttpPost]
        public object GetMemberDetails(UserLogin user)
        {
            dynamic MemberListDetailResult;
            List<object> MemberListResult = new List<object>();

            try
            {
                List<UserLogin> res = MemberMaster.GetMemberDetails(user.masterUID);

                for (int i = 0; i < res.Count; i++)
                {
                    MemberListResult.Add(new { MemberDetail = res[i] });
                }

                if (res != null && res.Count != 0)
                {
                    MemberListDetailResult = new { status = "0", message = "sucess", MemberDetails = MemberListResult };
                }
                else
                {
                    MemberListDetailResult = new { status = "0", message = "User Not Found" };
                }
            }
            catch
            {
                MemberListDetailResult = new { status = "1", message = "An error occured. Please contact Administrator" };
            }
            return new
            {
                MemberListDetailResult
            };
        }

        [NonAction]
        protected int UpdateDeviceToken(UserLogin user)
        {
            try
            {
                var result = (from e in _DBTouchbase.member_master_profile
                              where e.pk_member_master_profile_id == Convert.ToInt32(user.masterUID)
                              select e).ToList();

                foreach (var x in result)
                {
                    x.app_device_token_id = user.deviceToken;
                    x.device_name = user.deviceName;
                }
                _DBTouchbase.SaveChanges();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Created by : Nandu
        /// Created on : 05/05/2017
        /// Task : Registration mail
        /// </summary>
        [HttpPost]
        public object Registration(RegisterMail obj)
        {
            dynamic RegistrationResult;
            try
            {
                if ((GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), "row.techsupport@kaizeninfotech.com, securitycode@kaizeninfotech.com", "Enquiry from Chakiat", mailBody(obj.MobileNo,  obj.Name, obj.Email)) == ""))
                {
                    GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), Convert.ToString(obj.Email), "Thank you for Enquiry", customerMailBody());
                    RegistrationResult = new { status = "0", message = "success" };
                }
                else
                {
                    RegistrationResult = new { status = "1", message = "failed" };
                }
            }
            catch
            {
                RegistrationResult = new { status = "1", message = "failed" };
            }

            return new { RegistrationResult };
        }
    }
}
