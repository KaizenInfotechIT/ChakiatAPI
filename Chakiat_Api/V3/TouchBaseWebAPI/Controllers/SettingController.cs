using System.Collections.Generic;
using System.Web.Http;
using TouchBaseWebAPI.BusinessEntities;
using TouchBaseWebAPI.Models;
using System.Data;

namespace TouchBaseWebAPI.Controllers
{
    public class SettingController : ApiController
    {
        //================== update touchbase level memberwise notifications settings =============================//
        [System.Web.Http.HttpPost]
        public object TouchbaseSetting(GroupDetail grp)
        {
            dynamic TBSettingResult;

            try
            {
                int Result = Setting.updateTouchbaseSetting(grp);

                if (Result > 0)
                {
                    TBSettingResult = new { status = "0", message = "success" };
                }
                else
                {
                    TBSettingResult = new { status = "1", message = "Record not found" };
                }
            }
            catch
            {
                TBSettingResult = new { status = "1", message = "failed" };
            }

            return new { TBSettingResult };
        }

        //================== Get Touchbase Setting =============================//
        [System.Web.Http.HttpPost]
        public object GetTouchbaseSetting(MainMasterId mem)
        {
            dynamic TBSettingResult;
            List<object> AllTBSettingResults = new List<object>();

            try
            {
                List<SettingDetails> Result1 = Setting.getAllTouchbaseSetting(mem);

                for (int i = 0; i < Result1.Count; i++)
                {
                    AllTBSettingResults.Add(new { SettingResult = Result1[i] });
                }

                if (AllTBSettingResults != null)
                {
                    TBSettingResult = new { status = "0", message = "success", AllTBSettingResults };
                }
                else
                {
                    TBSettingResult = new { status = "0", message = "Record not found" };
                }
            }
            catch (System.Exception ex)
            {
                TBSettingResult = new { status = "1", message = "failed" ,ex.Message };
            }

            return new { TBSettingResult };
        }

        //================== update group level memberwise settings =============================//
        [System.Web.Http.HttpPost]
        public object GroupSetting(GrpSettingResult grp)
        {
            dynamic TBGroupSettingResult;

            try
            {
                int Result = Setting.updateGroupSetting(grp);

                if (Result > 0)
                {
                    TBGroupSettingResult = new { status = "0", message = "success" };
                }
                else
                {
                    TBGroupSettingResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGroupSettingResult = new { status = "1", message = "failed" };
            }

            return new { TBGroupSettingResult };
        }

        //================== Get Group Setting =============================//
        [System.Web.Http.HttpPost]
        public object GetGroupSetting(GetGroup grp)
        {
            dynamic TBGroupSettingResult;
            List<object> AllTBSettingResults = new List<object>();

            try
            {
                DataSet Result = Setting.getGroupSetting(grp);

                DataTable dtmain = Result.Tables[0];
                DataTable dt2 = Result.Tables[1];

                //string isMob, isEmail, isPersonal, isFamily, isBusiness;
                string isMobileSelf, isMobileOther, isEmailSelf, isEmailOther;

                //isMob = dt2.Rows[0]["isMob"].ToString();
                //isEmail = dt2.Rows[0]["isEmail"].ToString();
                //isPersonal = dt2.Rows[0]["isPersonal"].ToString();
                //isFamily = dt2.Rows[0]["isFamily"].ToString();
                //isBusiness = dt2.Rows[0]["isBusiness"].ToString();

                isMobileSelf = dt2.Rows[0]["show_mobile_self_club"].ToString();
                isMobileOther = dt2.Rows[0]["show_mobile_other_club"].ToString();
                isEmailSelf = dt2.Rows[0]["show_email_self_club"].ToString();
                isEmailOther = dt2.Rows[0]["show_email_other_club"].ToString();

                //Convert DataTable into List
                List<GRpSettingDetails> GroupSetting = new List<GRpSettingDetails>();
                if (dtmain.Rows.Count > 0)
                {
                    GroupSetting = GlobalFuns.DataTableToList<GRpSettingDetails>(dtmain);
                }

                //Add List into another List
                for (int i = 0; i < GroupSetting.Count; i++)
                {
                    AllTBSettingResults.Add(new { GRpSettingDetails = GroupSetting[i] });
                }

                if (AllTBSettingResults != null)
                {
                    //TBGroupSettingResult = new { status = "0", message = "success", isMob = isMob, isEmail = isEmail, isPersonal = isPersonal, isFamily = isFamily, isBusiness = isBusiness, GRpSettingResult = AllTBSettingResults };
                    TBGroupSettingResult = new
                    {
                        status = "0",
                        message = "success",

                        isMobileSelf = isMobileSelf,
                        isMobileOther = isMobileOther,
                        isEmailSelf = isEmailSelf,
                        isEmailOther = isEmailOther,

                        GRpSettingResult = AllTBSettingResults
                    };
                }
                else
                {
                    TBGroupSettingResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGroupSettingResult = new { status = "1", message = "failed" };
            }

            return new { TBGroupSettingResult };
        }

    }
}
