using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MySql.Data.MySqlClient;
using TouchBaseWebAPI.Models;
using System.Data;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class Setting
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DbTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static int updateTouchbaseSetting(GroupDetail grp)
        {
            try
            {
                var grpId = new MySqlParameter("?grpId", grp.GroupId);
                var updateValue = new MySqlParameter("?updateValue", grp.UpdatedValue);
                var mainMstId = new MySqlParameter("?mainMstId", grp.mainMasterId);

                var Result = _DbTouchbase.ExecuteStoreCommand("CALL USPtouchbase_rowSettings(?grpId,?updateValue,?mainMstId)", grpId, updateValue, mainMstId);

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<SettingDetails> getAllTouchbaseSetting(MainMasterId mem)
        {
            try
            {
                var mainMasterId = new MySqlParameter("?mainMasterId", mem.mainMasterId);
                var Result = _DbTouchbase.ExecuteStoreQuery<SettingDetails>("CALL USPGetTouchbase_rowSettings(?mainMasterId)", mainMasterId).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int updateGroupSetting(GrpSettingResult grp)
        {
            try
            {
                var GroupId = new MySqlParameter("?GrpId", grp.GroupId);
                var ModuleId = new MySqlParameter("?ModuleId", grp.ModuleId);
                var GroupProfileId = new MySqlParameter("?profileId", grp.GroupProfileId);

                var UpdatedValue = new MySqlParameter("?UpdatedValue", grp.UpdatedValue);

                var mobileSelf = new MySqlParameter("?showMobileSeflfClub", grp.showMobileSeflfClub);
                var mobileOutside = new MySqlParameter("?showMobileOutsideClub", grp.showMobileOutsideClub);
                var emailSelf = new MySqlParameter("?showEmailSeflfClub", grp.showEmailSeflfClub);
                var emailOutside = new MySqlParameter("?showEmailOutsideClub", grp.showEmailOutsideClub);

                var Result = _DbTouchbase.ExecuteStoreCommand("CALL USPGroupSettings(?GrpId,?ModuleId,?profileId,?UpdatedValue,?showMobileSeflfClub,?showMobileOutsideClub,?showEmailSeflfClub,?showEmailOutsideClub)",
                    GroupId, ModuleId, GroupProfileId, UpdatedValue, mobileSelf, mobileOutside, emailSelf, emailOutside);

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet getGroupSetting(GetGroup grp)
        {
            try
            {
                //var GroupId = new MySqlParameter("?grpId", grp.GroupId);
                //var GroupProfileId = new MySqlParameter("?profileId", grp.GroupProfileId);

                MySqlParameter[] parameters = new MySqlParameter[2];
                parameters[0] = new MySqlParameter("?grpId", grp.GroupId);
                parameters[1] = new MySqlParameter("?profileId", grp.GroupProfileId);

                string storeproc = "USPGetGroupSettings";
                DataSet ds = null;
                ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, storeproc, parameters);

                //var Result = _DbTouchbase.ExecuteStoreQuery<SettingDetails>("CALL USPGetTouchbaseSettings(?grpId,?profileId)", GroupId, GroupProfileId).ToList();

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}