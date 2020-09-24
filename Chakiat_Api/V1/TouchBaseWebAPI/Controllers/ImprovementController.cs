using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;
using System.Data;
using System.Configuration;

namespace TouchBaseWebAPI.Controllers
{
    public class ImprovementController : ApiController
    {
        [System.Web.Http.HttpPost]
        public object GetImprovementList(ImprovementSearch impr)
        {
            dynamic ImprovementListResult;
            List<object> ImprListResult = new List<object>();
            DataSet Result = new DataSet();

            try
            {
                string search = "";

                if (impr.searchText == null)
                { search = ""; }
                else
                { search = impr.searchText; }

                Result = Improvement.GetImprovementList(impr, search);
                DataTable dt = Result.Tables[0];
                DataTable dt1 = Result.Tables[1];

                List<ImprovementList> res = new List<ImprovementList>();

                if (dt.Rows.Count > 0)
                {
                    res = GlobalFuns.DataTableToList<ImprovementList>(dt);

                    if (!string.IsNullOrEmpty(res[0].improvementImg))
                    {
                        string announ_Image = res[0].improvementImg.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Improvement/Group" + impr.groupId + "/thumb/";
                        res[0].improvementImg = path + announ_Image;
                    }
                }

                for (int i = 0; i < res.Count; i++)
                {
                    ImprListResult.Add(new { ImprovementList = res[i] });
                }

                if (res != null && res.Count != 0)
                {
                    ImprovementListResult = new { status = "0", message = "success", smscount = dt1.Rows[0]["SMSCount"].ToString(), ImprListResult = ImprListResult };
                }
                else
                {
                    ImprovementListResult = new { status = "1", message = "Record not found", smscount = dt1.Rows[0]["SMSCount"].ToString(), ImprListResult = ImprListResult };
                }
            }
            catch
            {
                ImprovementListResult = new { status = "1", message = "failed", smscount = 0 };
            }

            return new { TBImprovementListResult = ImprovementListResult };
        }

        [System.Web.Http.HttpPost]
        public object GetImprovementDetails(ImprovementDetail impr)
        {
            dynamic TBImprovementListResult;
            List<object> ImprListResult = new List<object>();

            try
            {
                List<ImprovementList> Result = Improvement.GetImprovementDetails(impr);
                   
                for (int i = 0; i < Result.Count; i++)
                {
                    ImprListResult.Add(new { ImprovementList = Result[i] });
                }

                if (Result != null && Result.Count != 0)
                {
                    TBImprovementListResult = new { status = "0", message = "success", ImprovementListResult = ImprListResult };
                }
                else
                {
                    TBImprovementListResult = new { status = "1", message = "Record not found", ImprovementListResult = ImprListResult };
                }
            }
            catch
            {
                TBImprovementListResult = new { status = "1", message = "failed" };
            }

            return new { TBImprovementListResult = TBImprovementListResult };
        }

        [System.Web.Http.HttpPost]
        public object AddImprovement(AddImprovement addImpr)
        {
            dynamic TBAddImprovementResult;
            int str = -1;
            try
            {
                Imgname Result = Improvement.createImprovement(addImpr);
                if (!string.IsNullOrEmpty(Result.imgName))
                {
                    str = GlobalFuns.UploadImage(addImpr.grpID, Result.imgName, "Improvement");
                }
                else
                    str = 0;

                if (Result != null)
                {

                    if (str == 0)
                    {
                        TBAddImprovementResult = new { status = "0", message = "success" };

                        if (addImpr.improvementID != "0")
                        {
                            string url = ConfigurationManager.AppSettings["imgPath"] + "php/EditImprovement.php?AnnID=" + addImpr.improvementID;
                          //  GroupMaster.Send(url);
                        }

                    }
                    else
                        TBAddImprovementResult = new { status = "1", message = "failed" };
                }
                else
                {
                    TBAddImprovementResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAddImprovementResult = new { status = "1", message = "failed" };
            }

            return new { TBAddImprovementResult };
        }
    }
}
