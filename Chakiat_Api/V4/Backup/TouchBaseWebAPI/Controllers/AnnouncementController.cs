using System.Collections.Generic;
using System.Web.Http;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;
using System.Configuration;
using System.Data;

namespace TouchBaseWebAPI.Controllers
{
    public class AnnouncementController : ApiController
    {
        [System.Web.Http.HttpPost]
        public object GetAnnouncementBySearchText(MemberProfile mem)
        {
            dynamic TBAnnounceSearchResult;
            List<object> AnnounListResult = new List<object>();

            try
            {
                List<AnnounceList> Result = Announcement.GetAnnouncementBySearchText(mem.searchText);

                for (int i = 0; i < Result.Count; i++)
                {
                    AnnounListResult.Add(new { AnnounceList = Result[i] });
                }

                if (Result != null && Result.Count != 0)
                {
                    TBAnnounceSearchResult = new { status = "0", message = "success", AnnounListResult = AnnounListResult };
                }
                else
                {
                    TBAnnounceSearchResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAnnounceSearchResult = new { status = "1", message = "failed" };
            }

            return new { TBAnnounceSearchResult };
        }

        [System.Web.Http.HttpPost]
        public object GetAnnouncementList(AnnouncementSearch ann)
        {
            dynamic TBAnnounceListResult;
            List<object> AnnounListResult = new List<object>();
            DataSet Result = new DataSet();

            try
            {
                string search = "";

                if (ann.searchText == null)
                { search = ""; }
                else
                { search = ann.searchText; }


                Result = Announcement.GetAnnouncementList(ann, search);
                DataTable dt = Result.Tables[0];
                DataTable dt1 = Result.Tables[1];

                List<AnnounceList> res = new List<AnnounceList>();

                if (dt.Rows.Count > 0)
                {
                    res = GlobalFuns.DataTableToList<AnnounceList>(dt);

                    if (!string.IsNullOrEmpty(res[0].announImg))
                    {
                        string announ_Image = res[0].announImg.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/announcement/Group" + ann.groupId + "/thumb/";
                        res[0].announImg = path + announ_Image;
                    }
                }

                for (int i = 0; i < res.Count; i++)
                {
                    AnnounListResult.Add(new { AnnounceList = res[i] });
                }

                if (res != null && res.Count != 0)
                {
                    TBAnnounceListResult = new { status = "0", message = "success", smscount = dt1.Rows[0]["SMSCount"].ToString(), AnnounListResult = AnnounListResult };
                }
                else
                {
                    TBAnnounceListResult = new { status = "1", message = "Record not found", smscount = dt1.Rows[0]["SMSCount"].ToString(), AnnounListResult = AnnounListResult };
                }
            }
            catch
            {
                TBAnnounceListResult = new { status = "1", message = "failed", smscount = 0 };
            }

            return new { TBAnnounceListResult };
        }

        [System.Web.Http.HttpPost]
        public object GetAnnouncementDetails(AnnouncementDetail ann)
        {
            dynamic TBAnnounceListResult;
            List<object> AnnounListResult = new List<object>();

            try
            {
                List<AnnounceList> Result = Announcement.GetAnnouncementDetails(ann);

                for (int i = 0; i < Result.Count; i++)
                {
                    AnnounListResult.Add(new { AnnounceList = Result[i] });
                }

                if (Result != null && Result.Count != 0)
                {
                    TBAnnounceListResult = new { status = "0", message = "success", AnnounListResult = AnnounListResult };
                }
                else
                {
                    TBAnnounceListResult = new { status = "1", message = "Record not found", AnnounListResult = AnnounListResult };
                }
            }
            catch
            {
                TBAnnounceListResult = new { status = "1", message = "failed" };
            }

            return new { TBAnnounceListResult };
        }

        [System.Web.Http.HttpPost]
        public object AddAnnouncement(AddAnnouncement addann)
        {
            dynamic TBAddAnnouncementResult;
            int str = -1;
            try
            {
                Imgname Result = Announcement.createAnnouncement(addann);
                if (!string.IsNullOrEmpty(Result.imgName))
                {
                    str = GlobalFuns.UploadImage(addann.grpID, Result.imgName, "announcement");
                }
                else
                    str = 0;

                if (Result != null)
                {

                    if (str == 0)
                    {
                        TBAddAnnouncementResult = new { status = "0", message = "success" };

                        if (addann.announID != "0")
                        {
                            //string url = ConfigurationManager.AppSettings["imgPath"] + "php/EditAnnouncement.php?AnnID=" + addann.announID;
                            //GroupMaster.Send(url);
                        }

                    }
                    else
                        TBAddAnnouncementResult = new { status = "1", message = "failed" };
                }
                else
                {
                    TBAddAnnouncementResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAddAnnouncementResult = new { status = "1", message = "failed" };
            }

            return new { TBAddAnnouncementResult };
        }
    }
}
