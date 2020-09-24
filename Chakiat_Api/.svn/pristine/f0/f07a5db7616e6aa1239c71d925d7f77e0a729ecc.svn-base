using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;
using System.Configuration;
using System.Data;

namespace TouchBaseWebAPI.Controllers
{
    public class EbulletinController : ApiController
    {
        [System.Web.Http.HttpPost]
        public object GetEbulletinBySearchText(MemberProfile mem)
        {
            dynamic TBEbulletinSearchResult;
            List<object> EbulletinListResult = new List<object>();

            try
            {
                List<EbulletinList> Result = Ebulletin.GetEbulletinBySearchText(mem.searchText);

                for (int i = 0; i < Result.Count; i++)
                {
                    EbulletinListResult.Add(new { EbulletinList = Result[i] });
                }

                if (Result != null && Result.Count != 0)
                {
                    TBEbulletinSearchResult = new { status = "0", message = "success", AnnounListResult = EbulletinListResult };
                }
                else
                {
                    TBEbulletinSearchResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBEbulletinSearchResult = new { status = "1", message = "failed" };
            }

            return new { TBEbulletinSearchResult };
        }

        [System.Web.Http.HttpPost]
        public object GetEbulletinList(EbulletinSearch ebull)
        {
            dynamic TBEbulletinListResult;
            List<object> EbulletinListResult = new List<object>();
            DataSet Result = new DataSet();

            try
            {
                string search = "";

                if (ebull.searchText == null)
                {
                    search = "";
                }
                else
                {
                    search = ebull.searchText;
                }

                Result = Ebulletin.GetEbulletinList(ebull, search);

                DataTable dt = Result.Tables[0];
                DataTable dt1 = Result.Tables[1];

                List<EbulletinList> res = new List<EbulletinList>();

                if (dt.Rows.Count > 0)
                {
                    res = GlobalFuns.DataTableToList<EbulletinList>(dt);
                    for (int i = 0; i < res.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(res[i].ebulletinlink) && res[i].ebulletinType != "Link")
                        {
                            string ebulletinlink = res[i].ebulletinlink.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/ebulletin/Group" + ebull.groupId + "/";
                            res[i].ebulletinlink = path + ebulletinlink;
                        }
                    }
                }

                for (int i = 0; i < res.Count; i++)
                {
                    EbulletinListResult.Add(new { EbulletinList = res[i] });
                }

                if (res != null && res.Count != 0)
                {
                    TBEbulletinListResult = new { status = "0", message = "success", smscount = dt1.Rows[0]["SMSCount"].ToString(), EbulletinListResult = EbulletinListResult };
                }
                else
                {
                    TBEbulletinListResult = new { status = "0", message = "Record not found", smscount = dt1.Rows[0]["SMSCount"].ToString(), EbulletinListResult = EbulletinListResult };
                }
            }
            catch
            {
                TBEbulletinListResult = new { status = "1", message = "failed", smscount = 0 };
            }

            return new { TBEbulletinListResult };
        }

        [System.Web.Http.HttpPost]
        public object GetEbulletinDetails(EbulletinDetails ebull)
        {
            dynamic TBEbulletinListResult;
            List<object> EbulletinDetailResult = new List<object>();

            try
            {
                var Result = Ebulletin.GetEbulletinDetails(ebull);

                if (Result > 0)
                {
                    TBEbulletinListResult = new { status = "0", message = "success", EbulletinDetailResult = EbulletinDetailResult };
                }
                else
                {
                    TBEbulletinListResult = new { status = "0", message = "Record not found", EbulletinDetailResult = EbulletinDetailResult };
                }
            }
            catch
            {
                TBEbulletinListResult = new { status = "1", message = "failed" };
            }

            return new { TBEbulletinListResult };
        }

        [System.Web.Http.HttpPost]
        public object AddEbulletin(AddEbulletin addebulltn)
        {
            dynamic TBAddEbulletinResult;
            int str = 0;

            try
            {
                string Result = Ebulletin.createEbulletin(addebulltn);

                if (!string.IsNullOrEmpty(Result))
                {
                    str = GlobalFuns.uploadDocs(addebulltn.grpID, Result, "ebulletin");
                }

                if (str == 0)
                {
                    TBAddEbulletinResult = new { status = "0", message = "success" };

                    if (addebulltn.ebulletinID != "0")
                    {
                        string url = ConfigurationManager.AppSettings["imgPath"] + "php/EditEbulletin.php?newsID=" + addebulltn.ebulletinID;
                        GroupMaster.Send(url);
                    }
                }
                else
                    TBAddEbulletinResult = new { status = "1", message = "failed" };
            }
            catch
            {
                TBAddEbulletinResult = new { status = "1", message = "failed" };
            }
            return new { TBAddEbulletinResult };
        }
    }
}
