using System;
using System.Collections.Generic;
using System.Linq;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class InterviewTest
    {
        //private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static int EmpAddEdit(Emp_Inputs Emp)
        {
            try
            {
                int result = 0;

                MySqlParameter[] ParameterList = new MySqlParameter[5];
                ParameterList[0] = new MySqlParameter("?p_FK_EmpID", Emp.FK_EmpID);
                ParameterList[1] = new MySqlParameter("?p_EmpName", Emp.EmpName);
                ParameterList[2] = new MySqlParameter("?p_EmpCode", Emp.EmpCode);
                ParameterList[3] = new MySqlParameter("?p_EmpDesignation", Emp.EmpDesignation);
                ParameterList[4] = new MySqlParameter("?EMPID", DbType.Int32);
                ParameterList[4].Direction = ParameterDirection.InputOutput;

                result = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.StoredProcedure, "V7_EmpAddEdit", ParameterList);
                result = Convert.ToInt32(ParameterList[4].Value);

                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static int EmpDelete(Emp_Inputs Emp)
        {
            try
            {
                int result = 0;

                MySqlParameter[] ParameterList = new MySqlParameter[1];
                ParameterList[0] = new MySqlParameter("?p_FK_EmpID", Emp.FK_EmpID);

                result = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.StoredProcedure, "V7_EmpDelete", ParameterList);

                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Emp_Inputs> EmpSearch(Emp_Inputs Emp)
        {
            try
            {
                MySqlParameter[] ParameterList = new MySqlParameter[2];
                ParameterList[0] = new MySqlParameter("?p_FK_EmpID", Emp.FK_EmpID);
                ParameterList[1] = new MySqlParameter("?p_Emp_txtSearch", Emp.Emp_txtSearch);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<Emp_Inputs>("CALL V7_EmpSearch(?p_FK_EmpID,?p_Emp_txtSearch)", ParameterList).ToList();

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