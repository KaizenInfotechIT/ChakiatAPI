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


        /// <summary>
        /// Created By : Nandkishor K
        /// Created Date : 24/07/2017
        /// Reason : Listing of Public Albums
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public object GetPublicAlbumsList(GroupInfo input)
        {
            dynamic TBPublicAlbumsList;

            try
            {
                List<ClsAlbumList> Result = FindClub.GetPublicAlbumList(input);

                if (Result != null)
                {
                    TBPublicAlbumsList = new { status = "0", message = "success", Result };
                }
                else
                {
                    TBPublicAlbumsList = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBPublicAlbumsList = new { status = "1", message = "failed" };
            }

            return new { TBPublicAlbumsList };
        }


        /// <summary>
        /// Created By : Nandkishor K
        /// Created Date : 24/07/2017
        /// Reason : Listing of Newsletters(Only having Newsletters Type - All)
        /// </summary>
        [System.Web.Http.HttpPost]
        public object GetPublicEventsList(GroupInfo input)
        {
            dynamic TBPublicEventList;

            try
            {
                List<EventList1> Result = FindClub.GetPublicEventsList(input);

                if (Result != null)
                {
                    TBPublicEventList = new { status = "0", message = "success", Result };
                }
                else
                {
                    TBPublicEventList = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBPublicEventList = new { status = "1", message = "failed" };
            }

            return new { TBPublicEventList };
        }


        /// <summary>
        /// Created By : Nandkishor K
        /// Created Date : 24/07/2017
        /// Reason : Listing of Newsletters(Only having Newsletters Type - All)
        /// </summary>
        [System.Web.Http.HttpPost]
        public object GetPublicNewsletterList(GroupInfo input)
        {
            dynamic TBPublicNewsletterList;

            try
            {
                List<EbulletinList> Result = FindClub.GetPublicNewsletterList(input);

                if (Result != null)
                {
                    TBPublicNewsletterList = new { status = "0", message = "success", Result };
                }
                else
                {
                    TBPublicNewsletterList = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBPublicNewsletterList = new { status = "1", message = "failed" };
            }

            return new { TBPublicNewsletterList };
        }

        [System.Web.Http.HttpPost]
        public object GetClubMembers(GroupInfo input)
        {
            dynamic TBMemberList;

            try
            {
                SubGrpDirectoryResult Result = FindClub.GetClubsMemberList(input);

                if (Result != null)
                {
                    TBMemberList = new { status = "0", message = "success", Result };
                }
                else
                {
                    TBMemberList = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBMemberList = new { status = "1", message = "failed" };
            }

            return new { TBMemberList };
        }


        /// <summary>
        /// Created By : Nandkishor K
        /// Created Date : 01/08/2017
        /// Reason : Get Count of published Events and Newsletters
        /// </summary>
        [System.Web.Http.HttpPost]
        public object GetCommunicationCount(GroupInfo input)
        {
            dynamic TBCountResult;

            try
            {
                int EventsCount = 0;
                int NewslettersCount = 0;

                FindClub.GetCommunicationCount(input, out EventsCount, out NewslettersCount);

                TBCountResult = new { status = "0", message = "success", EventCount = EventsCount, NewsletterCount = NewslettersCount, };
            }
            catch
            {
                TBCountResult = new { status = "1", message = "failed" };
            }

            return new { TBCountResult };
        }
    }
}
