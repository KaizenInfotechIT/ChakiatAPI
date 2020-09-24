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
    public class PastPresidentsController : ApiController
    {
        [System.Web.Http.HttpPost]
        public object getPastPresidentsList(clsPastPresidentsInput inputParam)
        {
            dynamic TBPastPresidentListResult;

            try
            {
                clsPastPresidentsOutput result = PastPresidents.GetPastPresidentsList(inputParam);

                if (result != null)
                {
                    TBPastPresidentListResult = new { status = "0", message = "success", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), TBPastPresidentList = result };
                }
                else
                {
                    TBPastPresidentListResult = new { status = "0", message = "success" };
                }
            }
            catch
            {
                TBPastPresidentListResult = new { status = "1", message = "failed" };
            }

            return new { TBPastPresidentListResult };
        }
    }
}
