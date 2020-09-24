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
    public class version
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DbTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static List<clsVersionList> getVesionList(clsVersionInput input)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[1];

                param[0] = new MySqlParameter("?p_type", input.Type);

                var Result = _DbTouchbase.ExecuteStoreQuery<clsVersionList>("CALL MobileAPI_VersionControllist(?p_type)", param).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}