using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class cls_LeaderBoard
    {
    }

    public class LeaderBoard_Input
    {
        public int GroupID { get; set; }
        public string RowYear { get; set; }
        public int ProfileID { get; set; }
    }
    public class GetLeaderBoardDetails
    {
        public int TotalProjects { get; set; }
        public int ProjectCost { get; set; }
        public int BeneficiaryCount { get; set; }
        public int ManHoursCount { get; set; }
        public int RotariansCount { get; set; }
        public int MembersCount { get; set; }
        public int TRFCount { get; set; }

        //public LeaderBoard_Input LeaderBoard_Input { get; set; }
        public List<LeaderBoard_clubList> LeaderBoard_clubList { get; set; }
    }
    public class LeaderBoard_clubList
    {
        public int Srno { get; set; }
        public int clubId { get; set; }
        public string clubName { get; set; }
        public string Points { get; set; }
    }
}