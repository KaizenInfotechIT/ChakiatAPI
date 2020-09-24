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

        #region Old Working method Commented by Nandu on 06-04-207

        //[HttpPost]
        //public object UserLogin(UserLogin user)
        //{
        //    dynamic LoginResult;
        //    try
        //    {
        //        //Authentication
        //        //IEnumerable<string> headerValues = Request.Headers.GetValues("authtoken");
        //        //string id = headerValues.FirstOrDefault().ToString();

        //        //if (id == "123")
        //        //{ LoginResult = new { status = "0", message = "Success" }; }
        //        //else
        //        //{ LoginResult = new { status = "0", message = "access denied" }; }

        //        string strOTP = GlobalFuns.CreateRandomPassword(4);

        //        var country = int.Parse(user.countryCode);
        //        var c = (from e in _DBTouchbase.main_member_master
        //                 where e.member_mobile == user.mobileNo && e.fk_country_id == country
        //                 orderby e.pk_main_member_master_id
        //                 select e).FirstOrDefault();

        //        //send OTP to mobile through SMS
        //        if (user.countryCode == "1")//for India
        //        {
        //            //Commented by Nandu 08/07/2016 - due to change sender provider
        //            //GlobalFuns.SendAlertSMS(user.mobileNo, strOTP);

        //            //GlobalFuns.SendSMSOnAdd(user.mobileNo, strOTP + " is your one time password(OTP). Please enter the OTP to proceed. Thank you," + System.Environment.NewLine + "Team TouchBasE");
        //            //Added By Nandu on 06-12-2016 Task --> Mail OTP to us as well as user                     
        //            //GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), string.IsNullOrEmpty(c.member_emailid) ? "securitycode@kaizeninfotech.com" : "securitycode@kaizeninfotech.com," + c.member_emailid.ToString(), "TouchBasE - OTP for mobile application", mailbody(strOTP));

        //            //Added by Nandu on 07/12/2016 Task --> Run Parellel functionality of OTP as well as mail send to user
        //            Task task1 = Task.Factory.StartNew(() => sendNationalOTP(user.mobileNo, strOTP));

        //            Task task2 = null;
        //            if (c != null)
        //            {
        //                string email = string.IsNullOrEmpty(c.member_emailid) ? "securitycode@kaizeninfotech.com" : "securitycode@kaizeninfotech.com," + c.member_emailid.ToString().Trim();

        //                //task2 = Task.Factory.StartNew(() => sendMail(string.IsNullOrEmpty(Convert.ToString(c.member_emailid).Trim()) ? "securitycode@kaizeninfotech.com" : "securitycode@kaizeninfotech.com," + c.member_emailid.ToString().Trim(), strOTP, user.mobileNo));
        //                task2 = Task.Factory.StartNew(() => sendMail(email, strOTP, user.mobileNo));
        //            }
        //            else
        //                task2 = Task.Factory.StartNew(() => sendMail("securitycode@kaizeninfotech.com", strOTP, user.mobileNo));

        //            Task.WaitAll(task1);

        //            //string result1, result2;
        //            //Parallel.Invoke(() => result1 = sendOTP(user.mobileNo, strOTP),
        //            //() => result2 = sendMail(string.IsNullOrEmpty(c.member_emailid) ? "securitycode@kaizeninfotech.com" : "securitycode@kaizeninfotech.com," + c.member_emailid.ToString(), strOTP));

        //        }
        //        else
        //        {
        //            var countryCode = (from e in _DBTouchbase.country_master
        //                               where e.country_master_id == country
        //                               select e.country_code).FirstOrDefault();

        //            Task task1 = Task.Factory.StartNew(() => sendInternationalNationalOTP(user.mobileNo, strOTP, countryCode.ToString()));

        //            Task task2 = null;
        //            if (c != null)
        //            {
        //                string email = string.IsNullOrEmpty(c.member_emailid) ? "securitycode@kaizeninfotech.com" : "securitycode@kaizeninfotech.com," + c.member_emailid.ToString().Trim();
        //                task2 = Task.Factory.StartNew(() => sendMail(email, strOTP, user.mobileNo));

        //                //task2 = Task.Factory.StartNew(() => sendMail(string.IsNullOrEmpty(Convert.ToString(c.member_emailid).Trim()) ? "securitycode@kaizeninfotech.com" : "securitycode@kaizeninfotech.com," + c.member_emailid.ToString().Trim(), strOTP, user.mobileNo));
        //            }
        //            else
        //                task2 = Task.Factory.StartNew(() => sendMail("securitycode@kaizeninfotech.com", strOTP, user.mobileNo));

        //            Task.WaitAll(task1);
        //        }

        //        LoginResult = new { status = "0", message = "success", otp = strOTP };
        //    }
        //    catch
        //    {
        //        LoginResult = new { status = "1", message = "failed", otp = 0 };
        //    }
        //    return new
        //    {
        //        LoginResult
        //    };
        //}

        #endregion

        [HttpPost]
        [System.Web.Mvc.OutputCache(Duration = 10)]
        public object UserLogin(UserLogin user)
        {
            dynamic LoginResult;
            try
            {
                #region

                //Authentication
                //IEnumerable<string> headerValues = Request.Headers.GetValues("authtoken");
                //string id = headerValues.FirstOrDefault().ToString();

                //if (id == "123")
                //{ LoginResult = new { status = "0", message = "Success" }; }
                //else
                //{ LoginResult = new { status = "0", message = "access denied" }; }

                #endregion

                string strOTP = GlobalFuns.CreateRandomPassword(4);
                DataSet ds = new DataSet();

                ds = Login.GetMembersList(user);

                if (ds != null)
                {
                    if (ds.Tables[0] != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            bool flag = SendSMSAndEmailToUser(user, strOTP, ds.Tables[0].Rows[0]["emailId"].ToString(), ds.Tables[0].Rows[0]["memberName"].ToString(), ds.Tables[0].Rows[0]["groupName"].ToString(), ds.Tables[0].Rows[0]["districtNumber"].ToString());

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
            catch
            {
                LoginResult = new { status = "1", message = "failed", otp = 0 };
            }
            return new { LoginResult };
        }

        #region UserDefinedFunctions

        [NonAction]
        private bool SendSMSAndEmailToUser(UserLogin user, string OTP, string emailId, string memberName, string clubName, string district)
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
                        string email = string.IsNullOrEmpty(emailId) ? "securitycode@kaizeninfotech.com" : "securitycode@kaizeninfotech.com," + emailId.ToString().Trim();
                        task2 = Task.Factory.StartNew(() => sendMail(email, OTP, memberName, user.mobileNo, clubName, district));
                    }
                    else
                        task2 = Task.Factory.StartNew(() => sendMail("securitycode@kaizeninfotech.com", OTP, memberName, user.mobileNo, clubName, district));

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
                        task2 = Task.Factory.StartNew(() => sendMail(email, OTP, memberName, user.mobileNo, clubName, district));
                    }
                    else
                        task2 = Task.Factory.StartNew(() => sendMail("securitycode@kaizeninfotech.com", OTP, memberName, user.mobileNo, clubName, district));

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

            //MVaayoo Provider
            GlobalFuns.SendAlertSMS(mobile, otp + " is your one time password(OTP). Please enter the OTP to proceed. Thank you, Team ROW");
            
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
            GlobalFuns.SendSMSInternational(mobile, otp + " is your one time password(OTP). Please enter the OTP to proceed. Thank you," + System.Environment.NewLine + "Team ROW", countyCode);
            return "true";
        }

        /// <summary>
        /// Send Mail to user
        /// Created on : 07/12/2016
        /// Author : Nandu
        /// </summary>
        [NonAction]
        string sendMail(string email, string otp, string memberName, string mobile, string clubName, string district)
        {
            GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), email, "Roster On Wheels - OTP for mobile application", mailbody(memberName, mobile, otp, clubName, district));
            return "true";
        }

        [NonAction]
        private string mailbody(string memberName, string mobile, string OTP, string clubName, string district)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table width='100%' border='0'>");

            sb.Append("<tr><td>Dear ");
            sb.Append(memberName);
            sb.Append("(");
            sb.Append(mobile);
            sb.Append(")</td></tr>");

            sb.Append("<tr><td><br /><br />The security code for accessing Roster on wheels on your mobile is: ");
            sb.Append(OTP);
            sb.Append("<br /><br />Please type this code when prompted while installing the app.</td></tr>");

            sb.Append("<tr><td><br /><br />Club Name : ");
            sb.Append(clubName);
            sb.Append("[District :");
            sb.Append(district);
            sb.Append("]</td></tr>");

            sb.Append("<tr><td><br /><br /><br /><br /><p>Thank you<br /><br />Regards,<br /><br />Team ROW<br /><br />Help line number: +91 22 41516989 / +91 9004404397</p></td> </tr>");
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
        private string mailBody(string mobileNo, string isRotarian, string name, string emailId, string club, string feedback)
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

            sb.Append("<tr><td width='103'>Is Rotarian: </td>");
            sb.Append("<td width='471'>");
            sb.Append(isRotarian);
            sb.Append("</td></tr>");

            sb.Append("<tr><td width='103'>Name: </td>");
            sb.Append("<td width='471'>");
            sb.Append(name);
            sb.Append("</td></tr>");

            sb.Append("<tr><td>Email Id: </td>");
            sb.Append("<td>");
            sb.Append(emailId);
            sb.Append("</td></tr>");

            sb.Append("<tr><td>Club Name: </td>");
            sb.Append("<td>");
            sb.Append(club);
            sb.Append("</td></tr>");

            sb.Append("<tr><td>Feedback: </td>");
            sb.Append("<td>");
            sb.Append(feedback);
            sb.Append("</td></tr>");

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
            sb.Append("<tr><td><strong>Sincerely,<br />Team Roster On Wheels</strong></td></tr>");
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
                int masterId = Login.GetMembersPostOTP(user);

                if (masterId > 0)
                {
                    LoginResult = new { status = "0", message = "success", isexists = "true", masterUID = masterId.ToString() };
                    //MemberMaster.UpdateDeviceToken(user.deviceToken, masterId.ToString());
                }
                else
                {
                    LoginResult = new { status = "0", message = "success", isexists = "false" };
                }

                #region Old Working method Commented by Nandu on 08-04-207

                //var country = int.Parse(user.countryCode);
                //if (user.loginType == "0")  // 0 Login as Member
                //{
                //    var UID = (from e in _DBTouchbase.main_member_master
                //               where e.member_mobile == user.mobileNo && e.fk_country_id == country
                //               orderby e.pk_main_member_master_id
                //               select e.pk_main_member_master_id
                //                   ).FirstOrDefault();

                //    if (UID > 0)
                //    {
                //        var res = (from c in _DBTouchbase.main_member_master
                //                   where c.member_mobile == user.mobileNo && c.fk_country_id == country
                //                   select c).First();

                //        res.IMEI_No = user.imeiNo;
                //        res.DeviceToken = user.deviceToken;
                //        res.Device_name = user.deviceName;
                //        res.versionNo = user.versionNo;
                //        res.modification_by = Convert.ToInt32(user.masterUID);

                //        _DBTouchbase.SaveChanges();
                //        _DBTouchbase.Connection.Close();

                //        LoginResult = new { status = "0", message = "success", isexists = "true", masterUID = UID.ToString() };

                //        MemberMaster.UpdateDeviceToken(user.deviceToken, UID.ToString());
                //    }
                //    else
                //    {
                //        LoginResult = new { status = "0", message = "success", isexists = "true", masterUID = UID.ToString() };
                //    }
                //}
                //else // 1 Login as Family member
                //{
                //    var UID = (from e in _DBTouchbase.member_family_master
                //               where e.member_contact_no == user.mobileNo && e.fk_country_id == country
                //               orderby e.pk_member_family_master_id
                //               select e.fk_main_member_master_id
                //                   ).FirstOrDefault();

                //    if (UID > 0)
                //    {
                //        var res = (from c in _DBTouchbase.member_family_master
                //                   where c.member_contact_no == user.mobileNo && c.fk_country_id == country
                //                   select c).First();

                //        //res.IMEI_No = user.imeiNo;
                //        //res.DeviceToken = user.deviceToken;
                //        //res.Device_name = user.deviceName;
                //        //res.versionNo = user.versionNo;
                //        //res.modification_by = Convert.ToInt32(user.masterUID);

                //        _DBTouchbase.SaveChanges();
                //        _DBTouchbase.Connection.Close();

                //        LoginResult = new { status = "0", message = "success", isexists = "true", masterUID = UID.ToString() };

                //        MemberMaster.UpdateDeviceToken(user.deviceToken, UID.ToString());
                //    }
                //    else
                //    {
                //        LoginResult = new { status = "0", message = "success", isexists = "true", masterUID = UID.ToString() };
                //    }
                //}

                #endregion
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
            try
            {
                if (user.loginType == "1")
                {
                     memberName = (from member in _DBTouchbase.member_family_master
                                   where member.member_contact_no == user.mobileNo && member.isdeleted == false
                                      select member.member_name).Distinct().SingleOrDefault();
                }
                else
                {
                     memberName = (from member in _DBTouchbase.main_member_master
                                      where member.pk_main_member_master_id == uId
                                      select member.member_name).Distinct().SingleOrDefault();
                }

                var grpPartResults = (from member in _DBTouchbase.main_member_master
                                      join group_profile in _DBTouchbase.member_master_profile
                                      on member.pk_main_member_master_id equals group_profile.fk_main_member_master_id
                                      join grp in _DBTouchbase.group_master
                                      on group_profile.fk_group_master_id equals grp.pk_group_master_id
                                      orderby group_profile.fk_group_master_id
                                      where member.pk_main_member_master_id == uId && grp.isdeleted == false
                                      && group_profile.isdeleted == false
                                      select new
                                      {
                                          GrpPartResult = new
                                          {
                                              grpId = group_profile.fk_group_master_id,
                                              grpName = grp.group_name ?? string.Empty
                                          }
                                      }).Distinct()
                                   .ToList();

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
                if ((GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), "row.techsupport@kaizeninfotech.com, securitycode@kaizeninfotech.com", "Enquiry from Roster On Wheels", mailBody(obj.MobileNo, obj.IsRotarian, obj.Name, obj.Email, obj.Club, obj.Feedback)) == ""))
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
