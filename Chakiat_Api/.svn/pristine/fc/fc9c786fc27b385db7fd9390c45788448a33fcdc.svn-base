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
    public class LeaderBoard
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DbTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static DataSet getLeaderBoardDetails(LeaderBoard_Input Obj)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[3];

                param[0] = new MySqlParameter("?p_GroupID", Obj.GroupID);
                param[1] = new MySqlParameter("?p_RowYear", Obj.RowYear);
                param[2] = new MySqlParameter("?p_ProfileID", Obj.ProfileID);

                DataSet Result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "API_GetLeaderBoarClubDetails", param);
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}