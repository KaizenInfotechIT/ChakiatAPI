using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TouchBaseWebAPI.Models;
using System.Data;
using TouchBaseWebAPI.BusinessEntities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace TouchBaseWebAPI.Controllers
{
    public class versionListController : ApiController
    {
        [System.Web.Http.HttpPost]
        public object GetVersionList(clsVersionInput Input)
        {
            dynamic TBVersionResult;
            try
            {
                List<clsVersionList> Result = version.getVesionList(Input);

                if (Result != null)
                {
                    TBVersionResult = new { status = "0", message = "success", Result };
                }
                else
                {
                    TBVersionResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBVersionResult = new { status = "1", message = "failed" };
            }

            return new { TBVersionResult };
        }
    }
}
