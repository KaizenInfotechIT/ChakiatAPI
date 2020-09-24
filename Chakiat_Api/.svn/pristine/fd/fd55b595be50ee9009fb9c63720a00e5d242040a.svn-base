using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;

namespace TouchBaseWebAPI.Controllers
{
    public class FindRotarianController : ApiController
    {
        private TouchBaseWebAPI.Data.row_productionEntities _DbTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        [HttpPost]
        public object GetRotarianList(FindRotarianInput searchCriteria)
        {
            dynamic TBGetRotarianResult;
            try
            {
                List<RotarianResult> Result = FindRotarian.GetRotarian(searchCriteria);

                if (Result != null)
                {
                    TBGetRotarianResult = new { status = "0", message = "success", RotarianResult = Result };
                }
                else
                {
                    TBGetRotarianResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetRotarianResult = new { status = "1", message = "failed" };
            }

            return new { TBGetRotarianResult };
        }

        [HttpPost]
        public object GetrotarianDetails(MemberPro profile)
        {
            dynamic TBGetRotarianResult;
            try
            {
                RotarianDetailsOutput Result = FindRotarian.GetRotarianDetails(profile.memberProfileId);
                
                if (Result != null)
                {
                    TBGetRotarianResult = new { status = "0", message = "success", Result };
                }
                else
                {
                    TBGetRotarianResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetRotarianResult = new { status = "1", message = "failed" };
            }
            return new { TBGetRotarianResult };
        }
    }
}

