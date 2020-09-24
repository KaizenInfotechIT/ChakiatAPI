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
    public class LeaderBoardController : ApiController
    {
        [System.Web.Http.HttpPost]
        public object GetLeaderBoardDetails(LeaderBoard_Input Obj)
        {
            dynamic TBLeaderBoardResult;
            List<object> LeaderBoardResult = new List<object>();

            try
            {
                DataSet ds_details = LeaderBoard.getLeaderBoardDetails(Obj);

                List<LeaderBoard_clubList> Result = GlobalFuns.DataTableToList<LeaderBoard_clubList>(ds_details.Tables[1]);

                string TotalProjectsstr = "0";
                string ProjectCoststr = "0";
                string BeneficiaryCountstr = "0";
                string ManHoursCountstr = "0";
                string RotariansCountstr = "0";
                string MembersCountstr = "0";
                string TRFCountstr = "0";

                if (ds_details.Tables[0].Rows.Count > 0)
                {
                    string[] commandArgs = ds_details.Tables[0].Rows[0]["ClubsDetails"].ToString().Split(new char[] { '|' });

                    TotalProjectsstr = commandArgs[0].ToString();
                    ProjectCoststr = commandArgs[1].ToString();
                    BeneficiaryCountstr = commandArgs[2].ToString();
                    ManHoursCountstr = commandArgs[3].ToString();
                    RotariansCountstr = commandArgs[4].ToString();
                    MembersCountstr = commandArgs[5].ToString();
                    TRFCountstr = commandArgs[6].ToString();
                }

                for (int i = 0; i < Result.Count; i++)
                {
                    LeaderBoardResult.Add(new { LeaderBoardResult = Result[i] });
                }

                if (LeaderBoardResult != null)
                {
                    TBLeaderBoardResult = new { status = "0", message = "success", TotalProjects = TotalProjectsstr, ProjectCost = ProjectCoststr, BeneficiaryCount = BeneficiaryCountstr, ManHoursCount = ManHoursCountstr, RotariansCount = RotariansCountstr, MembersCount = MembersCountstr, TRFCount = TRFCountstr, LeaderBoardResult };
                }
                else
                {
                    TBLeaderBoardResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBLeaderBoardResult = new { status = "1", message = "failed" };
            }

            return new { TBLeaderBoardResult };
        }
    }
}
