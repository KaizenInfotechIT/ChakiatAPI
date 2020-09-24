using System;
using System.Collections.Generic;
using System.Linq;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class Announcement
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static List<AnnounceList> GetAnnouncementBySearchText(string searchText)
        {
            try
            {
                var text = new MySqlParameter("?searchText", searchText);
                var Result = _DBTouchbase.ExecuteStoreQuery<AnnounceList>("CALL USPSearchAnnouncementList(?searchText)", text).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet GetAnnouncementList(AnnouncementSearch ann, string search)
        {
            try
            {
                //var memId = new MySqlParameter("?memberId", ann.memberProfileId);
                //var groupId = new MySqlParameter("?grpId", ann.groupId);
                //var searchText = new MySqlParameter("?searchText", search);
                //var type = new MySqlParameter("?type", ann.type);
                //var isAdmin = new MySqlParameter("?isAdmin", ann.isAdmin);
                //var Result = _DBTouchbase.ExecuteStoreQuery<AnnounceList>("CALL USPGetAnnouncementList(?memberId,?grpId,?searchText,?type,?isAdmin)", memId, groupId, searchText, type, isAdmin).ToList();

                //foreach (AnnounceList announ in Result)
                //{
                //    if (!string.IsNullOrEmpty(announ.announImg))
                //    {
                //        string announ_Image = announ.announImg.ToString();
                //        string path = ConfigurationManager.AppSettings["imgPath"] + "/Documents/announcement/Group" + ann.groupId + "/thumb/";
                //        announ.announImg = path + announ_Image;
                //    }
                //}

                MySqlParameter[] param = new MySqlParameter[6];
                param[0] = new MySqlParameter("?memberId", ann.memberProfileId);
                param[1] = new MySqlParameter("?grpId", ann.groupId);

                param[2] = new MySqlParameter("?moduleId", ann.moduleId);// Added by Nandu on 07/11/2016 Task--> Module replica 

                param[3] = new MySqlParameter("?searchText", search);
                param[4] = new MySqlParameter("?type", ann.type);
                param[5] = new MySqlParameter("?isAdmin", ann.isAdmin);

                DataSet Result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V5_USPGetAnnouncementList", param);

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<AnnounceList> GetAnnouncementDetails(AnnouncementDetail ann)
        {
            string repeatDateTime = "";
            try
            {
                MySqlParameter[] param = new MySqlParameter[3];
                param[0] = new MySqlParameter("@announID", ann.announID);
                param[1] = new MySqlParameter("@grpID", ann.grpID);
                param[2] = new MySqlParameter("@memberProfileID", ann.memberProfileID);

                DataSet Result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V6_USPGetAnnouncementDetails", param);
                DataTable dtAnnouncement = Result.Tables[0];
                DataTable dtRepeatAnnouncement = Result.Tables[1];
                List<AnnounceList>  Anndetail = new List<AnnounceList>();

                if(dtAnnouncement.Rows.Count>0)
                {
                    Anndetail = GlobalFuns.DataTableToList<AnnounceList>(dtAnnouncement);
                   
                    if (!string.IsNullOrEmpty(Anndetail[0].announImg))
                    {
                        string announ_Image = Anndetail[0].announImg.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/announcement/Group" + ann.grpID + "/";
                        Anndetail[0].announImg = path + announ_Image;
                    }

                     if (dtRepeatAnnouncement.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtRepeatAnnouncement.Rows.Count; i++)
                        {
                            repeatDateTime += dtRepeatAnnouncement.Rows[i]["annRepeatDate"].ToString() + ",";
                        }
                        repeatDateTime = repeatDateTime.TrimEnd(',');
                    }
                    Anndetail[0].repeatDateTime = repeatDateTime;

                }
                return Anndetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Imgname createAnnouncement(AddAnnouncement ann)
        {
            string subgrpIDs = "";
            try
            {
                if (ann.isSubGrpAdmin == "1" && ann.annType=="0")
                {
                   subgrpIDs= SubGroupDirectory.GetAdminSubGroupList(ann.grpID, ann.memID);
                }
                MySqlParameter[] param = new MySqlParameter[16];
                param[0] = new MySqlParameter("?annID", string.IsNullOrEmpty(ann.announID) ? "0" : ann.announID);
                param[1] = new MySqlParameter("?annType", ann.annType);
                param[2] = new MySqlParameter("?announTitle", ann.announTitle);
                param[3] = new MySqlParameter("?announceDEsc", string.IsNullOrEmpty(ann.announceDEsc) ? "" : ann.announceDEsc);

                param[4] = new MySqlParameter("?memID", ann.memID);
                param[5] = new MySqlParameter("?grpID", ann.grpID);
                param[6] = new MySqlParameter("?memprofileIDs", string.IsNullOrEmpty(ann.inputIDs) ? "" : ann.inputIDs);

                param[7] = new MySqlParameter("?moduleId", ann.moduleId); // Added by Nandu on 07/11/2016 Task--> Module replica 

                param[8] = new MySqlParameter("?announcementImg", string.IsNullOrEmpty(ann.announImg) ? "0" : ann.announImg);
                param[9] = new MySqlParameter("?sendSMSAll", string.IsNullOrEmpty(ann.sendSMSAll) ? "0" : ann.sendSMSAll);
                param[10] = new MySqlParameter("?sendSMSNonSmartPh", string.IsNullOrEmpty(ann.sendSMSNonSmartPh) ? "0" : ann.sendSMSNonSmartPh);

                param[11] = new MySqlParameter("?publishDate", ann.publishDate);
                param[12] = new MySqlParameter("?expiryDate", ann.expiryDate);
                param[13] = new MySqlParameter("?IsSubGrpAdmin", string.IsNullOrEmpty(ann.isSubGrpAdmin) ? "0" : ann.isSubGrpAdmin);
                param[14] = new MySqlParameter("?subgrpIDs", subgrpIDs);
                param[15] = new MySqlParameter("?annRepeatDate", string.IsNullOrEmpty(ann.AnnouncementRepeatDates) ? "" : ann.AnnouncementRepeatDates);
                var Result = _DBTouchbase.ExecuteStoreQuery<Imgname>("CALL V6_USPAddAnnouncement(?annID,?annType,?announTitle,?announceDEsc,?memID,?grpID,?memprofileIDs,?moduleId,?announcementImg,?publishDate,?expiryDate,?sendSMSAll,?sendSMSNonSmartPh,?IsSubGrpAdmin,?subgrpIDs,?annRepeatDate)",
                                                                        param).SingleOrDefault();
                return Result;
            }
            catch (Exception ex)
            {
                ManageExceptions.TraceException("Announcement/AddAnnouncement", "createAnnouncement()", Convert.ToString(ex.InnerException), Convert.ToString(ex.Message), Convert.ToString(ex.StackTrace));
                throw ex;
            }
            
        }
    }
}