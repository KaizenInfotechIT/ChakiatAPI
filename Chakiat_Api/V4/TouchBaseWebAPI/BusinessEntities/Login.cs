﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class Login
    {
        //private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static DataSet GetMembersList(UserLogin user)
        {
            DataSet ds = new DataSet();
            try
            {
                MySqlParameter[] param = new MySqlParameter[3];

                param[0] = new MySqlParameter("?mobileNo", user.mobileNo);
                param[1] = new MySqlParameter("?countryCode", user.countryCode);
                param[2] = new MySqlParameter("?loginType", user.loginType); // 0 Login as Member, 1 Login as Family member

                ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V1_USPGetActiveMemberList", param);

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet GetMembersPostOTP(UserLogin user)
        {
            DataSet ds = new DataSet();
            //int masterId = 0;

            try
            {
                MySqlParameter[] param = new MySqlParameter[7];

                param[0] = new MySqlParameter("?mobileNo", user.mobileNo);
                param[1] = new MySqlParameter("?countryCode", user.countryCode);
                param[2] = new MySqlParameter("?loginType", user.loginType); // 0 Login as Member, 1 Login as Family member

                param[3] = new MySqlParameter("?imeiNo", user.imeiNo);
                param[4] = new MySqlParameter("?deviceToken", user.deviceToken);
                param[5] = new MySqlParameter("?deviceName", user.deviceName);

                param[6] = new MySqlParameter("?versionNo", user.versionNo);

                ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V1_USPPostOTP", param);

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet Get_Welcomescreen_Data(UserLogin user)
        {
            DataSet ds = new DataSet();
            try
            {
                MySqlParameter[] param = new MySqlParameter[3];

                param[0] = new MySqlParameter("?p_masterUID", user.masterUID);
                param[1] = new MySqlParameter("?p_mobileno", user.mobileNo);
                param[2] = new MySqlParameter("?p_loginType", user.loginType); // 0 Login as Member, 1 Login as Family member

                ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V4_API_Get_Welcomescreen_Data", param);

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public static List<RootObject1> Get_Welcomescreen_Data(UserLogin user)
        //{
        //    try
        //    {
        //        var masterUID = new MySqlParameter("?p_masterUID", user.masterUID);
        //        var mobileNo = new MySqlParameter("?p_mobileno", user.mobileNo);
        //        var loginType = new MySqlParameter("?p_loginType", user.loginType);
        //        var Result = _DBTouchbase.ExecuteStoreQuery<RootObject1>("CALL V4_API_Get_Welcomescreen_Data(?p_masterUID,?p_mobileno,?p_loginType)", masterUID, mobileNo, loginType).ToList();
        //        return Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}