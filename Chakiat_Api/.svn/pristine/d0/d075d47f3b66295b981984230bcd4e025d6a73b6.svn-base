using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class FindClub
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        const double converter = 1.6;

        public static List<Club> GetClubList(ClubDayInput search)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[5];
                param[0] = new MySqlParameter("?clubName", string.IsNullOrEmpty(search.keyword) ? "" : search.keyword.Replace(" ", "%"));
                param[1] = new MySqlParameter("?countryId", string.IsNullOrEmpty(search.country) ? "0" : search.country);
                param[2] = new MySqlParameter("?meetingDay", string.IsNullOrEmpty(search.meetingDay) ? "" : search.meetingDay.Replace(" ", "%"));
                param[3] = new MySqlParameter("?districtNo", string.IsNullOrEmpty(search.district) ? "0" : search.district);
                param[4] = new MySqlParameter("?state", string.IsNullOrEmpty(search.stateProvinceCity) ? "" : search.stateProvinceCity.Replace(" ", "%"));
                var Result = _DBTouchbase.ExecuteStoreQuery<Club>(" CALL V1_USPFindClubOnMeetingDay(?clubName,?countryId,?meetingDay,?districtNo,?state)", param).ToList();

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ClubDetails GetClubDetails(string clubId)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[1];
                param[0] = new MySqlParameter("?clubId", clubId);
                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V1_USPClubDetails", param);

                DataTable dtclub = result.Tables[0];
                DataTable dtpresident = result.Tables[1];
                DataTable dtsecretary = result.Tables[2];
                DataTable dtGovernor = result.Tables[3];

                List<ClubDetails> ClubDetailList = new List<ClubDetails>();
                if (dtclub.Rows.Count > 0)
                {
                    ClubDetailList = GlobalFuns.DataTableToList<ClubDetails>(dtclub);
                }

                List<MemberDetl> presidentList = new List<MemberDetl>();
                if (dtpresident.Rows.Count > 0)
                {
                    presidentList = GlobalFuns.DataTableToList<MemberDetl>(dtpresident);
                }

                List<MemberDetl> secretaryList = new List<MemberDetl>();
                if (dtpresident.Rows.Count > 0)
                {
                    secretaryList = GlobalFuns.DataTableToList<MemberDetl>(dtsecretary);
                }

                List<MemberDetl> Governor = new List<MemberDetl>();
                if (dtpresident.Rows.Count > 0)
                {
                    Governor = GlobalFuns.DataTableToList<MemberDetl>(dtGovernor);
                }

                if (ClubDetailList.Count > 0)
                {
                    ClubDetailList[0].president = presidentList;
                    ClubDetailList[0].secretary = secretaryList;
                    ClubDetailList[0].districtGovernor = Governor;
                }
                return ClubDetailList[0];
            }
            catch
            {
                throw;
            }

        }

        public static List<Club> GetClubsNearMe(NearMeInput club)
        {
            try
            {
                if (club.distanceUnit == "Miles" && !string.IsNullOrEmpty(club.distance))
                {
                    club.distance = (Convert.ToDouble(club.distance) * converter).ToString();
                }

                MySqlParameter[] param = new MySqlParameter[5];
                param[0] = new MySqlParameter("?dist", string.IsNullOrEmpty(club.distance) ? "0" : club.distance);
                param[1] = new MySqlParameter("?currentLat", string.IsNullOrEmpty(club.currentLat) ? "0" : club.currentLat);
                param[2] = new MySqlParameter("?currentLong", string.IsNullOrEmpty(club.currentLong) ? "0" : club.currentLong);
                param[3] = new MySqlParameter("?meetingDay", string.IsNullOrEmpty(club.meetingDay) ? "" : club.meetingDay.Replace(" ", "%"));
                param[4] = new MySqlParameter("?meetingTime", string.IsNullOrEmpty(club.meetingTime) ? "0" : club.meetingTime);

                var Result = _DBTouchbase.ExecuteStoreQuery<Club>("CALL V1_GetClubNearMe(?dist,?meetingDay,?meetingTime,?currentLat,?currentLong)", param).ToList();

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Created By : Nandkishor K
        /// Created Date : 24/07/2017
        /// Reason : Listing of Public Albums
        /// </summary>
        public static List<ClsAlbumList> GetPublicAlbumList(GroupInfo input)
        {
            try
            {
                MySqlParameter[] parameterList = new MySqlParameter[1];

                parameterList[0] = new MySqlParameter("?grpID", input.grpID);

                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "USPPublicAlbumList", parameterList);
                DataTable dtNewAlbums = result.Tables[0];

                List<ClsAlbumList> NewMemberList = new List<ClsAlbumList>();
                if (dtNewAlbums.Rows.Count > 0)
                {
                    NewMemberList = GlobalFuns.DataTableToList<ClsAlbumList>(dtNewAlbums);

                    foreach (ClsAlbumList g in NewMemberList)
                    {
                        if (!string.IsNullOrEmpty(g.image))
                        {
                            string ImageName = g.image.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/gallery/Group" + g.groupId + "/";
                            g.image = path + ImageName;
                        }
                    }
                }

                return NewMemberList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Created By : Nandkishor K
        /// Created Date : 24/07/2017
        /// Reason : Listing of Events(Only having Event Type - All)
        /// </summary>
        public static List<EventList1> GetPublicEventsList(GroupInfo input)
        {
            try
            {
                MySqlParameter[] parameterList = new MySqlParameter[1];
                parameterList[0] = new MySqlParameter("?grpID", input.grpID);

                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V3_USPPublicEventList", parameterList);
                DataTable dtEventList = result.Tables[0];

                List<EventList1> PublicEventList = new List<EventList1>();
                if (dtEventList.Rows.Count > 0)
                {
                    PublicEventList = GlobalFuns.DataTableToList<EventList1>(dtEventList);

                    foreach (EventList1 g in PublicEventList)
                    {
                        if (!string.IsNullOrEmpty(g.eventImg))
                        {
                            string event_Image = g.eventImg.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Event/Group" + input.grpID + "/thumb/";
                            g.eventImg = path + event_Image;
                        }
                    }
                }

                return PublicEventList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Created By : Nandkishor K
        /// Created Date : 24/07/2017
        /// Reason : Listing of Newsletters(Only having Newsletters Type - All)
        /// </summary>
        public static List<EbulletinList> GetPublicNewsletterList(GroupInfo input)
        {
            try
            {
                MySqlParameter[] parameterList = new MySqlParameter[1];
                parameterList[0] = new MySqlParameter("?grpID", input.grpID);

                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V2_USPPublicEbulletinList", parameterList);
                DataTable dtNewsletterList = result.Tables[0];

                List<EbulletinList> PublicNewsletterList = new List<EbulletinList>();
                if (dtNewsletterList.Rows.Count > 0)
                {
                    PublicNewsletterList = GlobalFuns.DataTableToList<EbulletinList>(dtNewsletterList);

                    foreach (EbulletinList g in PublicNewsletterList)
                    {
                        if (!string.IsNullOrEmpty(g.ebulletinlink) && g.ebulletinType != "Link")
                        {
                            string ebulletinlink = g.ebulletinlink.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/ebulletin/Group" + input.grpID + "/";
                            g.ebulletinlink = path + ebulletinlink;
                        }
                    }
                }

                return PublicNewsletterList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<MemberList> GetClubsMemberList(GroupInfo input)
        {
            try
            {
                MySqlParameter[] parameterList = new MySqlParameter[2];
                parameterList[0] = new MySqlParameter("?groupID", input.grpID);
                parameterList[1] = new MySqlParameter("?SearchText", input.SearchText);
                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V7_USPGetClubMemberList", parameterList);
                DataTable dtMemberList = result.Tables[0];

                List<MemberList> memberList = new List<MemberList>();

                if (dtMemberList.Rows.Count > 0)
                {
                    memberList = GlobalFuns.DataTableToList<MemberList>(dtMemberList);

                    foreach (MemberList mem in memberList)
                    {
                        if (!string.IsNullOrEmpty(mem.pic))
                        {
                            string ImageName = mem.pic.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                            mem.pic = path + ImageName;
                        }
                    }
                }
                return memberList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void GetCommunicationCount(GroupInfo input, out int EventsCount, out int NewslettersCount)
        {
            try
            {
                DataSet ds = new DataSet();

                MySqlParameter[] parameters = new MySqlParameter[3];

                parameters[0] = new MySqlParameter("?groupID", input.grpID);

                parameters[1] = new MySqlParameter("?eventsCount", 0);
                parameters[2] = new MySqlParameter("?newslettersCount", 0);

                parameters[1].Direction = ParameterDirection.InputOutput;
                parameters[2].Direction = ParameterDirection.InputOutput;

                ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V2_USPCommunicationCount", parameters);

                EventsCount = Convert.ToInt32(parameters[1].Value);
                NewslettersCount = Convert.ToInt32(parameters[2].Value);
            }
            catch
            {
                throw;
            }
        }
    }
}