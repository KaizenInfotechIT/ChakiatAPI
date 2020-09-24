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

        public static List<Club> GetClubList( ClubDayInput search)
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

                if (ClubDetailList.Count >0 )
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
    }
}