using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Configuration;
using System.Text;
using System.Globalization;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class clubMonthlyReport
    {
        //private static TouchBaseWebAPI.Data.row_productionEntities _DbTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static List<ClubMonthlyReportList> GetMonthlyReportList(ClubMonthlyReport_Input Obj)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[5];

                param[0] = new MySqlParameter("?p_profileId", Obj.profileId);
                param[1] = new MySqlParameter("?p_groupId", Obj.groupId);
                param[2] = new MySqlParameter("?p_month", Obj.month);
                param[3] = new MySqlParameter("?p_type", Obj.type);
                param[4] = new MySqlParameter("?p_Fk_ZoneID", Obj.Fk_ZoneID);

                DataSet Result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "USP_API_Mobile_GetMonthlyReportList", param);

                //var Result = _DbTouchbase.ExecuteStoreQuery<ClubMonthlyReportList>("CALL USP_API_Mobile_GetMonthlyReportList(?p_profileId,?p_groupId,?p_month,?p_type)", param).ToList();

                DataTable dtResult = Result.Tables[0];
                List<ClubMonthlyReportList> EbulletinList = new List<ClubMonthlyReportList>();

                if (dtResult.Rows.Count > 0)
                {
                    EbulletinList = GlobalFuns.DataTableToList<ClubMonthlyReportList>(dtResult);

                    for (int i = 0; i < EbulletinList.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(EbulletinList[i].reportUrl))
                        {
                            string ebulletinlink = EbulletinList[i].reportUrl.ToString();
                            string path = System.Configuration.ConfigurationManager.AppSettings["imgPath"] + "Documents/Clubmonthly/Group" + EbulletinList[i].ClubId + "/";
                            EbulletinList[i].reportUrl = path + ebulletinlink;
                        }
                    }
                }

                return EbulletinList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int SendSMSAndMailToNonSubmitedReports(ClubMonthlyReport_Input Obj)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[3];

                param[0] = new MySqlParameter("?p_groupId", Obj.groupId);
                param[1] = new MySqlParameter("?p_month", Obj.month);
                param[2] = new MySqlParameter("?p_type", Obj.type);

                DataSet Result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "USP_API_SendSMSAndMailToNonSubmitedReports", param);

                
                
                int smsEmailflag = 0;

                string month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(Obj.month.Substring(0, 2)));
                string Year = Obj.month.Substring(3, 4);

                #region SMS sending
                if (Obj.type == "1")
                {
                    DataTable dtMobile = Result.Tables[0];
                    if (dtMobile.Rows.Count > 0)
                    {
                        int TotalMobileNumberCount = 0;
                        int CurrentSMScount = GetBalanceSMS(Obj.groupId.ToString());
                        string MobileNumberIndia = "";
                        int MobileNumberCount = 0;
                        foreach (DataRow row in dtMobile.Rows)
                        {
                            string[] values = row["member_mobile_no"].ToString().Split(',');
                            TotalMobileNumberCount = TotalMobileNumberCount + values.Length;
                        }
                        if (TotalMobileNumberCount < CurrentSMScount)
                        {
                            foreach (DataRow row in dtMobile.Rows)
                            {
                                string[] values = row["member_mobile_no"].ToString().Split(',');
                                if (row["country_code"].ToString() == "+91" && row["member_mobile_no"].ToString() != "")//for India
                                {
                                    MobileNumberIndia += row["member_mobile_no"].ToString() + ",";
                                    MobileNumberCount = MobileNumberCount + values.Length;
                                }
                                else//for International SMS
                                {
                                    if (row["member_mobile_no"].ToString() != "")//for India
                                    {
                                        if ((GlobalFuns.SendSMSInternational(row["member_mobile_no"].ToString(), "Dear Rtn," + Environment.NewLine + "The monthly report for your club for the month of " + month + " " + Year + " has not been submitted.", row["country_code"].ToString())) == "SMS Send SuccessFully")
                                        {
                                            UpdateInternationalSMSCount(Obj.groupId.ToString());
                                            smsEmailflag = 1;
                                        }
                                    }
                                }
                            }
                            if (MobileNumberIndia != "")
                            {
                                MobileNumberIndia = MobileNumberIndia.TrimEnd(',');
                                if ((GlobalFuns.SendSMSOnAdd(MobileNumberIndia + ",9763128181,9821130855", "Dear Club leader," + Environment.NewLine + "The monthly report of your club for " + month + " " + Year + " has not been submitted. Please submit it  through www.rosteronwheels.com" + Environment.NewLine + " District Governor.")) == true)
                                {
                                    UpdateAllSMSCount(Obj.groupId.ToString(), MobileNumberCount);
                                    smsEmailflag = 1;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region send email
                if (Obj.type == "2")
                {
                    DataTable dtEmail = Result.Tables[0];
                    if (dtEmail.Rows.Count > 0)
                    {
                        if (dtEmail.Rows.Count > 0)
                        {
                            string MailList = dtEmail.Rows[0]["member_email_id"].ToString();
                            if (MailList == "")
                            {
                                MailList += "mukesh.dhole@kaizeninfotech.com,naren@kaizeninfotech.com";//
                            }
                            else
                            {
                                MailList += ",mukesh.dhole@kaizeninfotech.com,naren@kaizeninfotech.com";//,naren@kaizeninfotech.com
                            }
                            
                            if (MailList != "")
                            {
                                MailList = MailList.TrimEnd(',');
                                GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), MailList.Trim().ToString(), "Club Monthly Report not Submitted", msgbody(month + " " + Year));
                            }
                        }
                    }
                }

                #endregion
                return smsEmailflag;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static string msgbody(string Month)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>");
            sb.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");
            sb.Append("<head>");
            sb.Append("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            sb.Append("<title>Mailer</title>");
            sb.Append("</head>");
            sb.Append("<body leftmargin='0' topmargin='0' marginwidth='0' marginheight='0'>");
            sb.Append("<table width='620px' cellspacing='0' align='center'  style='border:1px solid #ccc; border-collapse:collapse;'>");
            sb.Append("<tr width='620px' height='80px' style='display:none;'>");
            sb.Append("<td colspan='5' align='center' style='border-bottom: 2px solid #00a5ea;background-color:#fff'><a href='http://www.rosteronwheels.com/' target='_blank'><img src='http://kaizeninfotech.com/emailer/mbpt/p1.png'></a></td></tr><tr style=''>");
            sb.Append("<td width='20px'>&nbsp;</td><td colspan='3' style='padding-bottom: 15px;'>");
            sb.Append("<p style='font-size:16px; color:#414042; font-style:italic; font-family:Arial, Helvetica, sans-serif; line-height:30px;'><b>Dear Club leader</b><br /></p>");
            sb.Append("<p style='font-size:14px; color:#414042; font-style:italic; font-family:Arial, Helvetica, sans-serif; line-height:30px;'>The monthly report of  your club for the month of <b>" + Month + "</b> has not been submitted. ");
            sb.Append("Please visit <a href='http://rosteronwheels.com' style='color:#0072b3; font-weight:bold;'>www.rosteronwheels.com</a> using your username and password and click on Club monthly report icon to submit the report.</p>");
            sb.Append("<p style='font-size:14px; color:#414042; font-style:italic; font-family:Arial, Helvetica, sans-serif; line-height:30px;'>");
            sb.Append("If you have any query on the process, please call ROW helpline on 8424064369 / 8928755144 ( Monday to Friday) between 9:30 AM to 6:30 PM.</p>");
            sb.Append("<p style='font-size:14px; color:#414042; font-style:italic; font-family:Arial, Helvetica, sans-serif; line-height:30px;'>");
            sb.Append("If you are unable to get the support, please contact your Asst. Governor.");
            sb.Append("<p style='font-size:14px; color:#414042; font-style:italic; font-family:Arial, Helvetica, sans-serif; line-height:30px;'>");
            sb.Append("Please note that Club monthly Report should be submitted only through ROW , not by email or any physical format..  </p>");
            sb.Append("<p style='font-size:14px; color:#414042; font-style:italic; font-family:Arial, Helvetica, sans-serif; line-height:30px;'>");
            sb.Append("( Please ignore this email if the report is already submitted)</p>");
            sb.Append("<p style='font-size:14px; color:#414042; font-style:italic; font-family:Arial, Helvetica, sans-serif; line-height:30px;'>");
            sb.Append("Regards<br /><strong>District Governor.</strong></p></td><td width='20px'></td></tr>   ");
            sb.Append("<tr><td colspan='5' style='border-top: 2px solid #00a5ea;background-color:#fff'>");
            sb.Append("<p style='font-size:16px; color:#414042; text-align:center; font-style:italic; font-family:Arial, Helvetica, sans-serif; line-height:10px;'>This district is powered by<br /><img src='http://kaizeninfotech.com/emailer/mbpt/p1.png' height='50%' width='50%'>");
            sb.Append("</p></td></tr></table></body></html>");

            return sb.ToString();
        }
        public static int GetBalanceSMS(string GroupMasterID)
        {
            string strQuery = "select coalesce(SMSCount,0) as SMSCount from group_master where coalesce(isdeleted,0)=0 and pk_group_master_id=" + GroupMasterID;
            string SMScount = "";
            try
            {
                SMScount = MySqlHelper.ExecuteScalar(GlobalVar.strAppConn, CommandType.Text, strQuery).ToString();
            }
            catch
            {
            }
            return Convert.ToInt32(SMScount);
        }
        public static void UpdateInternationalSMSCount(string GroupId)
        {
            string ssql = "update group_master set SMSCount=(SMSCount-1) where pk_group_master_id='" + GroupId + "'";

            try
            {
                MySqlHelper.ExecuteScalar(GlobalVar.strAppConn, CommandType.Text, ssql);
            }
            catch
            { }
        }
        public static void UpdateAllSMSCount(string GroupId, int MobileNumberCount)
        {
            string ssql = "update group_master set SMSCount=(SMSCount-" + MobileNumberCount + ") where pk_group_master_id='" + GroupId + "'";

            try
            {
                MySqlHelper.ExecuteScalar(GlobalVar.strAppConn, CommandType.Text, ssql);
            }
            catch
            { }
        }
    }
}