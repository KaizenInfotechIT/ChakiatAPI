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
    public class DistrictCommitteeController : ApiController
    {
        [System.Web.Http.HttpPost]
        public object districtCommitteeList(districtCommitteeInput attendance)
        {
            dynamic TBDistrictCommitteeResult;

            try
            {
                districtCommittee Result = DistrictCommittee.getDistrictCommitteeList(attendance);

                if (Result != null)
                {
                    TBDistrictCommitteeResult = new { status = "0", message = "success", Result };
                }
                else
                {
                    TBDistrictCommitteeResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBDistrictCommitteeResult = new { status = "1", message = "failed" };
            }

            return new { TBDistrictCommitteeResult };
        }

        [System.Web.Http.HttpPost]
        public object districtCommitteeDetails(districtCommitteeInput attendance)
        {
            dynamic TBDistrictCommitteeDetailsResult;

            try
            {
                districtCommittee Result = DistrictCommittee.getDistrictCommitteeDetails(attendance);

                if (Result != null)
                {
                    TBDistrictCommitteeDetailsResult = new { status = "0", message = "success", Result };
                }
                else
                {
                    TBDistrictCommitteeDetailsResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBDistrictCommitteeDetailsResult = new { status = "1", message = "failed" };
            }

            return new { TBDistrictCommitteeDetailsResult };
        }


        [System.Web.Http.HttpPost]
        public object districtCommitteeSearchList(districtCommitteeInput attendance)
        {
            dynamic TBDistrictCommitteeResult;

            try
            {
                districtCommittee Result = DistrictCommittee.getDistrictCommitteeSearchList(attendance);

                if (Result != null)
                {
                    TBDistrictCommitteeResult = new { status = "0", message = "success", Result };
                }
                else
                {
                    TBDistrictCommitteeResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBDistrictCommitteeResult = new { status = "1", message = "failed" };
            }

            return new { TBDistrictCommitteeResult };
        }
    }
}
