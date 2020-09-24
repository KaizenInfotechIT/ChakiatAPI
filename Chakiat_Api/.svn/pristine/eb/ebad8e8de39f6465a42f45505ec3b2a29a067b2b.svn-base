using System;
using System.Collections.Generic;
using System.Linq;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class PastPresidents
    {
        //private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static clsPastPresidentsOutput GetPastPresidentsList(clsPastPresidentsInput inputParam)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[3];
                param[0] = new MySqlParameter("?GroupId", inputParam.GroupId);
                param[1] = new MySqlParameter("?SearchText", string.IsNullOrEmpty(inputParam.SearchText) ? "" : inputParam.SearchText.Replace(" ", "%").Trim());
                param[2] = new MySqlParameter("?updateOn", inputParam.updateOn);

                DataSet ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V1_USPGetPastPresidentsList", param);

                DataTable dtNewRecords = ds.Tables[0];
                DataTable dtUpdatedRecords = ds.Tables[1];
                string strDeletedRecords = ds.Tables[2].Rows[0]["PastPresidentId"].ToString();

                List<clsPastPresident> newRecords = new List<clsPastPresident>();
                newRecords = GlobalFuns.DataTableToList<clsPastPresident>(dtNewRecords);
                foreach (clsPastPresident objPP in newRecords)
                {
                    if (!string.IsNullOrEmpty(objPP.PhotoPath))
                    {
                        objPP.PhotoPath = ConfigurationManager.AppSettings["imgPath"] + "Documents/pastpresidents/Group" + inputParam.GroupId + "/" + objPP.PhotoPath;
                    }
                }

                List<clsPastPresident> updatedRecords = new List<clsPastPresident>();
                updatedRecords = GlobalFuns.DataTableToList<clsPastPresident>(dtUpdatedRecords);
                foreach (clsPastPresident objPP in updatedRecords)
                {
                    if (!string.IsNullOrEmpty(objPP.PhotoPath))
                    {
                        objPP.PhotoPath = ConfigurationManager.AppSettings["imgPath"] + "Documents/pastpresidents/Group" + inputParam.GroupId + "/" + objPP.PhotoPath;
                    }
                }

                clsPastPresidentsOutput obj = new clsPastPresidentsOutput();

                obj.newRecords = newRecords;
                obj.updatedRecords = updatedRecords;
                obj.deletedRecords = strDeletedRecords;

                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}