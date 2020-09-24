using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;


namespace TouchBaseWebAPI.Controllers
{
    public class FindClubController : ApiController
    {
        [HttpPost]
        public object GetClubList(ClubDayInput searchCriteria)
        {
            dynamic TBGetClubResult;
            try
            {
                List<Club> Result = FindClub.GetClubList(searchCriteria);
                if (Result != null)
                {
                    TBGetClubResult = new { status = "0", message = "success", ClubResult = Result };
                }
                else
                {
                    TBGetClubResult = new { status = "0", message = "Record not found" };
                }

            }
            catch
            {
                TBGetClubResult = new { status = "1", message = "failed" };
            }
            return new { TBGetClubResult };
        }

        [HttpPost]
        public object GetClubsNearMe(NearMeInput searchCriteria)
        {
            dynamic TBGetClubResult;
            try
            {
                List<Club> Result = FindClub.GetClubsNearMe(searchCriteria);
                if (Result != null)
                {
                    TBGetClubResult = new { status = "0", message = "success", ClubResult = Result };
                }
                else
                {
                    TBGetClubResult = new { status = "0", message = "Record not found" };
                }

            }
            catch
            {
                TBGetClubResult = new { status = "1", message = "failed" };
            }
            return new { TBGetClubResult };
        }

        [HttpPost]
        public object GetClubDetails(GroupInfo grp)
        {
            dynamic TBGetClubDetailResult;
            try
            {
                ClubDetails Result = FindClub.GetClubDetails(grp.grpID);
                if (Result != null)
                {
                    TBGetClubDetailResult = new { status = "0", message = "success", ClubDetailResult = Result };
                }
                else
                {
                    TBGetClubDetailResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetClubDetailResult = new { status = "1", message = "failed" };
            }
            return new { TBGetClubDetailResult };
        }
    }
}
