using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class clubMonthlyReport
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DbTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static List<ClubMonthlyReportList> GetMonthlyReportList(ClubMonthlyReport_Input Obj)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[4];

                param[0] = new MySqlParameter("?p_profileId", Obj.profileId);
                param[1] = new MySqlParameter("?p_groupId", Obj.groupId);
                param[2] = new MySqlParameter("?p_month", Obj.month);
                param[3] = new MySqlParameter("?p_type", Obj.type);

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
    }
}