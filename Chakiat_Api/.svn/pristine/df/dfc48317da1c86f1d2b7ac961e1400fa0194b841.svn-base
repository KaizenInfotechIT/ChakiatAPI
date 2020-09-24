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
    public class Attendance
    {

        //private static TouchBaseWebAPI.Data.row_productionEntities _DbTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static List<AttendanceList> getAttendanceList(clsGetAttendanceList attendance)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[4];

                param[0] = new MySqlParameter("?p_groupProfileID", attendance.groupProfileID);
                param[1] = new MySqlParameter("?p_attendanceMonth", attendance.month);
                param[2] = new MySqlParameter("?p_attendanceYear", attendance.year);
                param[3] = new MySqlParameter("?p_moduleId", attendance.moduleID);
                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<AttendanceList>("CALL USPGetAttendanceList(?p_groupProfileID,?p_attendanceMonth,?p_attendanceYear,?p_moduleId)", param).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Created By Mukesh A. Dhole
        public static List<GetAttendanceListNew> getAttendanceListNew(GetAttendanceListNew_Input Input)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];

                param[0] = new MySqlParameter("?p_groupProfileID", Input.groupProfileID);
                param[1] = new MySqlParameter("?p_groupID", Input.groupID);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<GetAttendanceListNew>("CALL USP_API_Mobile_getAttendanceListNew(?p_groupProfileID,?p_groupID)", param).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Created By Mukesh A. Dhole
        public static List<GetAttendanceEventsListNew> getAttendanceEventsListNew(GetAttendanceListNew_Input Input)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[3];

                param[0] = new MySqlParameter("?grpId", Input.groupID);
                param[1] = new MySqlParameter("?searchText", Input.searchText);
                param[2] = new MySqlParameter("?filterType", Input.type);
                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<GetAttendanceEventsListNew>("CALL WEBAttendanceGetEventList(?grpId,?searchText,?filterType)", param).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Created By Mukesh A. Dhole
        public static List<GetAttendanceDetails> getAttendanceDetails(GetAttendanceDetails_Input Input)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[1];

                param[0] = new MySqlParameter("?p_AttendanceID", Input.AttendanceID);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<GetAttendanceDetails>("CALL USP_API_Mobile_getAttendanceDetails(?p_AttendanceID)", param).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int getAttendanceDelete(GetAttendanceDetails_Input Input)
        {
            int result=0;
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];

                param[0] = new MySqlParameter("?p_AttendanceID", Input.AttendanceID);
                param[1] = new MySqlParameter("?p_CreatedBy", Input.createdBy);

                result = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.StoredProcedure, "USP_API_Mobile_AttendanceDelete", param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public static int AttendanceAddEdit(AttendanceAddEdit_Input obj)
        {
            int result = 0;

            

            MySqlParameter[] parameterList = new MySqlParameter[17];

            parameterList[0] = new MySqlParameter("?P_AttendanceID", obj.AttendanceID);
            parameterList[1] = new MySqlParameter("?P_AttendanceName", obj.AttendanceName);
            parameterList[2] = new MySqlParameter("?P_AttendanceDesc", obj.AttendanceDesc);
            parameterList[3] = new MySqlParameter("?P_AttendanceDate", obj.AttendanceDate);

            parameterList[4] = new MySqlParameter("?P_fk_group_id", obj.fk_group_id);
            parameterList[5] = new MySqlParameter("?P_fk_module_id", obj.fk_module_id);
            parameterList[6] = new MySqlParameter("?P_created_by", obj.created_by);
            parameterList[7] = new MySqlParameter("?P_modification_by", obj.modification_by);
            parameterList[8] = new MySqlParameter("?P_deleted_by", obj.deleted_by);
            parameterList[9] = new MySqlParameter("?P_FK_eventID", obj.FK_eventID);

            parameterList[10] = new MySqlParameter("?P_MemberCount", obj.MemberCount);
            parameterList[11] = new MySqlParameter("?P_AnnsCount", obj.AnnsCount);
            parameterList[12] = new MySqlParameter("?P_AnnetsCount", obj.AnnetsCount);
            parameterList[13] = new MySqlParameter("?P_VisitorsCount", obj.VisitorsCount);
            parameterList[14] = new MySqlParameter("?P_RotarianCount", obj.RotarianCount);
            parameterList[15] = new MySqlParameter("?P_DistrictDelegatesCount", obj.DistrictDelegatesCount);

            parameterList[16] = new MySqlParameter("?AttendanceID", DbType.Int32);
            parameterList[16].Direction = ParameterDirection.InputOutput;
            
            try
            {
                result = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.StoredProcedure, "USP_API_Mobile_AttendanceAddEdit", parameterList);
                result = Convert.ToInt32(parameterList[16].Value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        public static List<newMembers> getAttendanceMemberDetails(GetAttendanceDetails_Input Input)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];

                param[0] = new MySqlParameter("?p_AttendanceID", Input.AttendanceID);
                param[1] = new MySqlParameter("?p_Type", Input.type);
                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<newMembers>("CALL USP_API_Mobile_AttendanceTypeDetails(?p_AttendanceID,?p_Type)", param).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<newAnns> getAttendanceAnnsDetails(GetAttendanceDetails_Input Input)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];

                param[0] = new MySqlParameter("?p_AttendanceID", Input.AttendanceID);
                param[1] = new MySqlParameter("?p_Type", Input.type);
                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<newAnns>("CALL USP_API_Mobile_AttendanceTypeDetails(?p_AttendanceID,?p_Type)", param).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<newAnnets> getAttendanceAnnetsDetails(GetAttendanceDetails_Input Input)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];

                param[0] = new MySqlParameter("?p_AttendanceID", Input.AttendanceID);
                param[1] = new MySqlParameter("?p_Type", Input.type);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<newAnnets>("CALL USP_API_Mobile_AttendanceTypeDetails(?p_AttendanceID,?p_Type)", param).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<newVisitors> getAttendanceVisitorsDetails(GetAttendanceDetails_Input Input)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];

                param[0] = new MySqlParameter("?p_AttendanceID", Input.AttendanceID);
                param[1] = new MySqlParameter("?p_Type", Input.type);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<newVisitors>("CALL USP_API_Mobile_AttendanceTypeDetails(?p_AttendanceID,?p_Type)", param).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<newRotarians> getAttendanceRotariansDetails(GetAttendanceDetails_Input Input)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];

                param[0] = new MySqlParameter("?p_AttendanceID", Input.AttendanceID);
                param[1] = new MySqlParameter("?p_Type", Input.type);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<newRotarians>("CALL USP_API_Mobile_AttendanceTypeDetails(?p_AttendanceID,?p_Type)", param).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<newDistrictDelegate> getAttendanceDistrictDeleagateDetails(GetAttendanceDetails_Input Input)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];

                param[0] = new MySqlParameter("?p_AttendanceID", Input.AttendanceID);
                param[1] = new MySqlParameter("?p_Type", Input.type);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<newDistrictDelegate>("CALL USP_API_Mobile_AttendanceTypeDetails(?p_AttendanceID,?p_Type)", param).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<GetAttendanceRotarianDetailsbyID> GetrotarianDetailsbyRotarianID(GetAttendanceRotarian_Input Input)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[1];

                param[0] = new MySqlParameter("?p_RotarianID", Input.RotarianID);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<GetAttendanceRotarianDetailsbyID>("CALL USP_API_Mobile_GetrotarianDetailsbyRotarianID(?p_RotarianID)", param).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<GetAttendanceDelegateDetailsByRotarianName> GetAttendanceDistrinctDelegateDetailsByRotarianName(GetAttendanceRotarian_Input Input)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[1];

                param[0] = new MySqlParameter("?p_RotarianName", Input.RotarianName);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<GetAttendanceDelegateDetailsByRotarianName>("CALL USP_API_Mobile_GetAttendanceDistrinctByRotarianName(?p_RotarianName)", param).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}