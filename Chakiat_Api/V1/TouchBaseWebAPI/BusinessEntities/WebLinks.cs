using System;
using System.Collections.Generic;
using System.Linq;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace TouchBaseWebAPI.BusinessEntities
{
    /// <summary>
    /// Created By : Nandu
    /// Created On : 25-04-2017
    /// </summary>
    public class WebLinks
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static int addUpdateWebLink(clsAddWebLinkInput obj)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[5];

                param[0] = new MySqlParameter("?weblinkId", obj.WeblinkId);
                param[1] = new MySqlParameter("?groupId", obj.GroupId);
                param[2] = new MySqlParameter("?createdBy", obj.CreateBy);

                param[3] = new MySqlParameter("?title", obj.Title);
                param[4] = new MySqlParameter("?description", string.IsNullOrEmpty(obj.Description) ? "" : obj.Description);
                param[5] = new MySqlParameter("?url", string.IsNullOrEmpty(obj.LinkUrl) ? "" : obj.LinkUrl);

                MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.StoredProcedure, "WEBAddUpdateWebLink", param);

                int result = Convert.ToInt32(param[0].Value);

                return result;
            }
            catch (Exception ex)
            {
                ManageExceptions.TraceException("WebLinks/addUpdateWebLink", "addUpdateWebLink()", Convert.ToString(ex.InnerException), Convert.ToString(ex.Message), Convert.ToString(ex.StackTrace));
                throw ex;
            }
        }

        public static List<clsGetWebLinkOutput> GetWebLinkList(clsGetWebLinkInput input)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];

                param[0] = new MySqlParameter("?strSearch", string.IsNullOrEmpty(input.SearchText) ? "" : input.SearchText.Replace(' ', '%').Trim());
                param[1] = new MySqlParameter("?strGroupID", input.GroupId);

                var Result = _DBTouchbase.ExecuteStoreQuery<clsGetWebLinkOutput>("CALL WEBGetWebLinkList(?strSearch, ?strGroupID)", param).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}