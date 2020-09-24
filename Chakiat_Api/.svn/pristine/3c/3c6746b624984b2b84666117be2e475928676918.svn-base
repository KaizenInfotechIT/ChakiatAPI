using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class Improvement
    {
        //private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static DataSet GetImprovementList(ImprovementSearch imp, string search)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[5];
                param[0] = new MySqlParameter("?memberId", imp.memberProfileId);
                param[1] = new MySqlParameter("?grpId", imp.groupId);
                param[2] = new MySqlParameter("?searchText", search);
                param[3] = new MySqlParameter("?type", imp.type);
                param[4] = new MySqlParameter("?isAdmin", imp.isAdmin);

                DataSet Result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V4_USPGetImprovementList", param);

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ImprovementList> GetImprovementDetails(ImprovementDetail imp)
        {
            try
            {
                var imprID = new MySqlParameter("?imprID", imp.improvementID);
                var grpID = new MySqlParameter("?grpID", imp.grpID);
                var memberProfileID = new MySqlParameter("?memberProfileID", imp.memberProfileID);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<ImprovementList>("CALL V4_USPGetImprovementDetails(?imprID,?grpID,?memberProfileID)", imprID, grpID, memberProfileID).ToList();

                    foreach (ImprovementList improvement in Result)
                    {
                        if (!string.IsNullOrEmpty(improvement.improvementImg))
                        {
                            string announ_Image = improvement.improvementImg.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Improvement/Group" + imp.grpID + "/";
                            improvement.improvementImg = path + announ_Image;
                        }
                    }
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Imgname createImprovement(AddImprovement imp)
        {
            try
            {
                if (string.IsNullOrEmpty(imp.improvementID))
                    imp.improvementID = "0";

                if (string.IsNullOrEmpty(imp.inputIDs))
                    imp.inputIDs = "";

                var imprID = new MySqlParameter("?imprID", imp.improvementID);
                var imprType = new MySqlParameter("?imprType", imp.imprType);
                var imprTitle = new MySqlParameter("?imprTitle", imp.improvementTitle);
                var imprDesc = new MySqlParameter("?imprDesc", string.IsNullOrEmpty(imp.improvementDesc) ? "" : imp.improvementDesc);

                var memID = new MySqlParameter("?memID", imp.memID);
                var grpID = new MySqlParameter("?grpID", imp.grpID);
                var memprofileIDs = new MySqlParameter("?memprofileIDs", imp.inputIDs);
                var imprImg = new MySqlParameter("?ImprovementImg", string.IsNullOrEmpty(imp.improvementImg) ? "0" : imp.improvementImg);
                var sendSMSAll = new MySqlParameter("?sendSMSAll", string.IsNullOrEmpty(imp.sendSMSAll) ? "0" : imp.sendSMSAll);
                var sendSMSNonSmartPh = new MySqlParameter("?sendSMSNonSmartPh", string.IsNullOrEmpty(imp.sendSMSNonSmartPh) ? "0" : imp.sendSMSNonSmartPh);
                var publishDate = new MySqlParameter("?publishDate", imp.publishDate);
                var expiryDate = new MySqlParameter("?expiryDate", imp.expiryDate);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<Imgname>("CALL V4_USPAddImprovement(?imprID,?imprType,?imprTitle,?imprDesc,?memID,?grpID,?memprofileIDs,?ImprovementImg,?publishDate,?expiryDate,?sendSMSAll,?sendSMSNonSmartPh)",
                                                                             imprID, imprType, imprTitle, imprDesc, memID, grpID, memprofileIDs, imprImg, publishDate, expiryDate, sendSMSAll, sendSMSNonSmartPh).SingleOrDefault();
                    //var Result = 1;
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}