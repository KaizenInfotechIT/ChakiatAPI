using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class Ebulletin
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static List<EbulletinList> GetEbulletinBySearchText(string searchText)
        {
            try
            {
                var text = new MySqlParameter("?searchText", searchText);
                var Result = _DBTouchbase.ExecuteStoreQuery<EbulletinList>("CALL USPSearchEbulletinList(?searchText)", text).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet GetEbulletinList(EbulletinSearch ebull, string searchtext)
        {
            try
            {
                //var memId = new MySqlParameter("?memberId", ebull.memberProfileId);
                //var groupId = new MySqlParameter("?grpId", ebull.groupId);
                //var searchText = new MySqlParameter("?searchText", searchtext);
                //var type = new MySqlParameter("?type", ebull.type);
                //var isAdmin = new MySqlParameter("?isAdmin", ebull.isAdmin);

                //var Result = _DBTouchbase.ExecuteStoreQuery<EbulletinList>("CALL USPGetEbulletinList(?memberId,?grpId,?searchText,?type,?isAdmin)", memId, groupId, searchText, type, isAdmin).ToList();

                //foreach (EbulletinList ebulltn in Result)
                //{
                //    if (ebulltn.ebulletinType == "File")
                //    {
                //        string ebulletinlink = ebulltn.ebulletinlink.ToString();
                //        string path = ConfigurationManager.AppSettings["imgPath"] + "/Documents/ebulletin/Group" + ebull.groupId + "/";
                //        ebulltn.ebulletinlink = path + ebulletinlink;
                //    }
                //}
                //return Result;
                
                MySqlParameter[] param = new MySqlParameter[5];
                param[0] = new MySqlParameter("?memberId", ebull.memberProfileId);
                param[1] = new MySqlParameter("?grpId", ebull.groupId);
                param[2] = new MySqlParameter("?searchText", searchtext);
                param[3] = new MySqlParameter("?type", ebull.type);
                param[4] = new MySqlParameter("?isAdmin", ebull.isAdmin);

                DataSet Result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V2_USPGetEbulletinList", param);

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int GetEbulletinDetails(EbulletinDetails ebull)
        {
            try
            {
                var EbullID = new MySqlParameter("?EbullID", ebull.ebulletinID);
                var memberProfileID = new MySqlParameter("?memberProfileID", ebull.memberProfileID);

                var Result = _DBTouchbase.ExecuteStoreCommand("CALL USPGetEbulletinDetails(?EbullID,?memberProfileID)", EbullID, memberProfileID);
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string createEbulletin(AddEbulletin ebulletin)
        {
            string subgrpIDs = "";
            try
            {
                if (ebulletin.isSubGrpAdmin == "1" && ebulletin.ebulletinType=="0")
                {
                    subgrpIDs = SubGroupDirectory.GetAdminSubGroupList(ebulletin.grpID, ebulletin.memID);
                }

                MySqlParameter[] param = new MySqlParameter[14];
                param[0] = new MySqlParameter("?bulletinID", string.IsNullOrEmpty(ebulletin.ebulletinID) ? "0" : ebulletin.ebulletinID);

                param[1] = new MySqlParameter("?bulletinType", string.IsNullOrEmpty(ebulletin.ebulletinType) ? "0" : ebulletin.ebulletinType);
                param[2] = new MySqlParameter("?ebulletinTitle", ebulletin.ebulletinTitle);
                param[3] = new MySqlParameter("?ebulletinlink", string.IsNullOrEmpty(ebulletin.ebulletinlink) ? "" : ebulletin.ebulletinlink);
                param[4] = new MySqlParameter("?ebulletinfile", string.IsNullOrEmpty(ebulletin.ebulletinfileid) ? "" : ebulletin.ebulletinfileid);

                param[5] = new MySqlParameter("?memID", ebulletin.memID);
                param[6] = new MySqlParameter("?grpID", ebulletin.grpID);
                param[7] = new MySqlParameter("?memprofileIDs", string.IsNullOrEmpty(ebulletin.inputIDs) ? "" : ebulletin.inputIDs);

                param[8] = new MySqlParameter("?publishDate", ebulletin.publishDate);
                param[9] = new MySqlParameter("?expiryDate", ebulletin.expiryDate);

                param[10] = new MySqlParameter("?sendSMSAll", string.IsNullOrEmpty(ebulletin.sendSMSAll) ? "0" : ebulletin.sendSMSAll);
                param[11] = new MySqlParameter("?sendSMSNonSmartPh", string.IsNullOrEmpty(ebulletin.sendSMSNonSmartPh) ? "0" : ebulletin.sendSMSNonSmartPh);

                param[12] = new MySqlParameter("?IsSubGrpAdmin", string.IsNullOrEmpty(ebulletin.isSubGrpAdmin) ? "0" : ebulletin.isSubGrpAdmin);
                param[13] = new MySqlParameter("?subgrpIDs", subgrpIDs);

                var Result = _DBTouchbase.ExecuteStoreQuery<string>("CALL V5_USPAddEbulletin(?bulletinID,?bulletinType,?ebulletinTitle,?ebulletinlink,?ebulletinfile,?memID,?grpID,?memprofileIDs,?publishDate,?expiryDate,?sendSMSAll,?sendSMSNonSmartPh,?IsSubGrpAdmin,?subgrpIDs)", param).SingleOrDefault();

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
    }
}