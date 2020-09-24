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
    public class servey
    {
        //private static TouchBaseWebAPI.Data.row_productionEntities _DbTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static int AddEditSevey(Cls_Input obj)
        {
            int result = 0;

            MySqlParameter[] parameterList = new MySqlParameter[13];

            parameterList[0] = new MySqlParameter("?P_Pk_ServeyID", obj.Pk_ServeyID);
            parameterList[1] = new MySqlParameter("?P_Fk_GroupID", obj.groupId);
            parameterList[2] = new MySqlParameter("?P_publicServeyID", obj.publicsurveyid);
            parameterList[3] = new MySqlParameter("?P_datecreated", obj.datecreated);
            parameterList[4] = new MySqlParameter("?P_dateupdated", obj.dateupdated);
            parameterList[5] = new MySqlParameter("?P_ExpiredDate", obj.expirydate);
            parameterList[6] = new MySqlParameter("?P_Name", obj.name);
            parameterList[7] = new MySqlParameter("?P_Status", obj.status);
            parameterList[8] = new MySqlParameter("?P_ResultCount", obj.resultcount);
            parameterList[9] = new MySqlParameter("?P_surveysteps", obj.surveysteps);
            parameterList[10] = new MySqlParameter("?P_surveylink", obj.surveylink);
            parameterList[11] = new MySqlParameter("?P_profileId", obj.profileId);

            parameterList[12] = new MySqlParameter("?P_Fk_ServeyID", DbType.Int32);
            parameterList[12].Direction = ParameterDirection.InputOutput;

            try
            {
                result = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.StoredProcedure, "USP_API_Mobile_AddEditSevey", parameterList);
                result = Convert.ToInt32(parameterList[12].Value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        public static List<Cls_Input> getSurvey_List(Cls_Input Obj)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];

                param[0] = new MySqlParameter("?p_groupId", Obj.groupId);
                param[1] = new MySqlParameter("?p_profileId", Obj.profileId);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<Cls_Input>("CALL USPGetSurvey_List(?p_groupId,?p_profileId)", param).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Cls_Input> SurveyDetails(Cls_Input Obj)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[3];

                param[0] = new MySqlParameter("?p_groupId", Obj.groupId);
                param[1] = new MySqlParameter("?p_profileId", Obj.profileId);
                param[2] = new MySqlParameter("?p_Pk_ServeyID", Obj.Pk_ServeyID);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<Cls_Input>("CALL USPGetSurveyDetails(?p_groupId,?p_profileId,?p_Pk_ServeyID)", param).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int DeleteSevey(Cls_Input obj)
        {
            int result = 0;

            MySqlParameter[] parameterList = new MySqlParameter[3];

            parameterList[0] = new MySqlParameter("?P_groupId", obj.groupId);
            parameterList[1] = new MySqlParameter("?P_profileId", obj.profileId);
            parameterList[2] = new MySqlParameter("?P_Pk_ServeyID", obj.Pk_ServeyID);

            try
            {
                result = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.StoredProcedure, "USP_API_DeleteSevey", parameterList);
            }
            catch
            {
                throw;
            }

            return result;
        }
    }
}