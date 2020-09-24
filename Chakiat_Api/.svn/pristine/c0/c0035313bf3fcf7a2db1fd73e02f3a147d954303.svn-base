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
    public class AttendanceController : ApiController
    {
        /// <summary>
        /// Author : Nandu
        /// created on : 10/10/2016
        /// task : Get Attendance Listing
        /// </summary>
        [System.Web.Http.HttpPost]
        public object GetAttendanceList(clsGetAttendanceList attendance)
        {
            dynamic TBAttendanceListResult;
            List<object> AttendanceListResult = new List<object>();

            try
            {
                List<AttendanceList> Result = Attendance.getAttendanceList(attendance);

                for (int i = 0; i < Result.Count; i++)
                {
                    AttendanceListResult.Add(new { AttendanceResult = Result[i] });
                }

                if (AttendanceListResult != null)
                {
                    TBAttendanceListResult = new { status = "0", message = "success", AttendanceListResult };
                }
                else
                {
                    TBAttendanceListResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAttendanceListResult = new { status = "1", message = "failed" };
            }

            return new { TBAttendanceListResult };
        }

        [System.Web.Http.HttpPost]
        public object GetAttendanceEventsListNew(GetAttendanceListNew_Input Input)
        {
            dynamic TBAttendanceEventsListResult;
            List<object> AttendanceEventsListResult = new List<object>();

            try
            {
                List<GetAttendanceEventsListNew> Result = Attendance.getAttendanceEventsListNew(Input);

                for (int i = 0; i < Result.Count; i++)
                {
                    AttendanceEventsListResult.Add(new { AttendanceResult = Result[i] });
                }

                if (AttendanceEventsListResult != null)
                {
                    TBAttendanceEventsListResult = new { status = "0", message = "success", AttendanceEventsListResult };
                }
                else
                {
                    TBAttendanceEventsListResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAttendanceEventsListResult = new { status = "1", message = "failed" };
            }

            return new { TBAttendanceEventsListResult };
        }

        [System.Web.Http.HttpPost]
        public object GetAttendanceListNew(GetAttendanceListNew_Input Input)
        {
            dynamic TBAttendanceListResult;
            List<object> AttendanceListResult = new List<object>();
             
            try
            {
                List<GetAttendanceListNew> Result = Attendance.getAttendanceListNew(Input);

                for (int i = 0; i < Result.Count; i++)
                {
                    AttendanceListResult.Add(new { AttendanceResult = Result[i] });
                }

                if (AttendanceListResult != null)
                {
                    TBAttendanceListResult = new { status = "0", message = "success", AttendanceListResult };
                }
                else
                {
                    TBAttendanceListResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAttendanceListResult = new { status = "1", message = "failed" };
            }

            return new { TBAttendanceListResult };
        }

        [System.Web.Http.HttpPost]
        public object AttendanceAddEdit(JObject AttendanceInput)
        {
            dynamic TBAttendanceDetailsResult;
            
            try
            {
                string jsonData = JsonConvert.SerializeObject(AttendanceInput);
                RootObjectAttendance obj = JsonConvert.DeserializeObject<RootObjectAttendance>(jsonData);

                if (obj != null)
                {
                    int Result = Attendance.AttendanceAddEdit(obj.AttendanceAddEdit_Input);

                    if (Result != 0)
                    {
                        #region start members
                        if (obj.AttendanceMembers != null)
                        {
                            string ConnectionString = GlobalVar.strAppConn;
                            {
                                using (MySqlConnection mConnection = new MySqlConnection(ConnectionString))
                                {
                                    List<string> NewRows = new List<string>();
                                    StringBuilder NewMemberstr = new StringBuilder();
                                    StringBuilder DeleteMemberstr = new StringBuilder();
                                        if (obj.AttendanceMembers[0].newMembers.Count > 0)
                                        {
                                            NewMemberstr.Append("INSERT INTO tbl_attendancememberdetails (FK_AttendanceID, FK_MemberID) VALUES ");
                                            for (int i = 0; i < obj.AttendanceMembers[0].newMembers.Count; i++)
                                            {
                                                NewRows.Add(string.Format("('{0}','{1}')", Result, obj.AttendanceMembers[0].newMembers[i].FK_MemberID));
                                            }
                                        }
                                        if (obj.AttendanceMembers[0].deletedMembers.Count > 0)
                                        {
                                            for (int i = 0; i < obj.AttendanceMembers[0].deletedMembers.Count; i++)
                                            {
                                                DeleteMemberstr.Append("update tbl_attendancememberdetails set Isdeleted=1,deleted_date=now() where FK_AttendanceID=" + Convert.ToInt32(Result) + " and FK_MemberID=" + obj.AttendanceMembers[0].deletedMembers[i].FK_MemberID + ";");
                                            }
                                            
                                        }
                                        
                                        bool flag = false;
                                        if (NewRows.Count > 0)
                                        {
                                            NewMemberstr.Append(string.Join(",", NewRows));
                                            NewMemberstr.Append(";");
                                            flag = true;
                                        }
                                        if (DeleteMemberstr.Length > 0)
                                        {
                                            NewMemberstr.Append(DeleteMemberstr);
                                            flag = true;
                                        }

                                        if (flag == true)
                                        {
                                            //NewMemberstr.Append(string.Join(",", NewRows));
                                            //NewMemberstr.Append(";");
                                            //NewMemberstr.Append(DeleteMemberstr);
                                            mConnection.Open();

                                            using (MySqlCommand myCmd = new MySqlCommand(NewMemberstr.ToString(), mConnection))
                                            {
                                                myCmd.CommandType = CommandType.Text;
                                                myCmd.ExecuteNonQuery();
                                            }

                                            if (mConnection.State == ConnectionState.Open)
                                            {
                                                mConnection.Close();
                                            }
                                        }
                                }
                            }
                        }
                        #endregion

                        #region start anns
                        if (obj.AttendanceAnns != null)
                        {
                            string ConnectionString = GlobalVar.strAppConn;
                            {
                                using (MySqlConnection mConnection = new MySqlConnection(ConnectionString))
                                {
                                    List<string> NewRows = new List<string>();
                                    StringBuilder NewAnns = new StringBuilder();
                                    StringBuilder UpdatedAnns = new StringBuilder();
                                    StringBuilder DeleteAnns = new StringBuilder();
                                    if (obj.AttendanceAnns[0].newAnns.Count > 0)
                                    {
                                        NewAnns.Append("INSERT INTO attendanceannsdetails (FK_AttendanceID, AnnsName,created_date) VALUES ");
                                        for (int i = 0; i < obj.AttendanceAnns[0].newAnns.Count; i++)
                                        {
                                            NewRows.Add(string.Format("('{0}','{1}',{2})", Result, MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceAnns[0].newAnns[i].AnnsName),"now()"));
                                        }
                                    }
                                    if (obj.AttendanceAnns[0].UpdateAnns.Count > 0)
                                    {
                                        for (int i = 0; i < obj.AttendanceAnns[0].UpdateAnns.Count; i++)
                                        {
                                            UpdatedAnns.Append("update attendanceannsdetails set AnnsName='" + MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceAnns[0].UpdateAnns[i].AnnsName) + "',modified_date=now() where FK_AttendanceID=" + Convert.ToInt32(Result) + " and PK_AttendanceAnnsID=" + obj.AttendanceAnns[0].UpdateAnns[i].PK_AttendanceAnnsID + ";");
                                        }
                                    }
                                    if (obj.AttendanceAnns[0].deletedAnns.Count > 0)
                                    {
                                        for (int i = 0; i < obj.AttendanceAnns[0].deletedAnns.Count; i++)
                                        {
                                            DeleteAnns.Append("update attendanceannsdetails set Isdeleted=1,deleted_date=now() where FK_AttendanceID=" + Convert.ToInt32(Result) + " and PK_AttendanceAnnsID=" + obj.AttendanceAnns[0].deletedAnns[i].PK_AttendanceAnnsID + ";");
                                        }
                                    }

                                    bool flag = false;
                                    if (NewRows.Count > 0)
                                    {
                                        NewAnns.Append(string.Join(",", NewRows));
                                        NewAnns.Append(";");
                                        flag = true;
                                    }
                                    if (UpdatedAnns.Length > 0)
                                    {
                                        NewAnns.Append(UpdatedAnns);
                                        flag = true;
                                    }
                                    if (DeleteAnns.Length > 0)
                                    {
                                        NewAnns.Append(DeleteAnns);
                                        flag = true;
                                    }

                                    if (flag == true)
                                    {
                                        //NewAnns.Append(string.Join(",", NewRows));
                                        //NewAnns.Append(";");
                                        //NewAnns.Append(UpdatedAnns);
                                        //NewAnns.Append(DeleteAnns);

                                        mConnection.Open();
                                        using (MySqlCommand myCmd = new MySqlCommand(NewAnns.ToString(), mConnection))
                                        {
                                            myCmd.CommandType = CommandType.Text;
                                            myCmd.ExecuteNonQuery();
                                        }
                                        if (mConnection.State == ConnectionState.Open)
                                        {
                                            mConnection.Close();
                                        }
                                    }
                                    
                                }
                            }
                        }
                        #endregion

                        #region start Annets
                        {
                            if (obj.AttendanceAnnets != null)
                            {
                                string ConnectionString = GlobalVar.strAppConn;
                                {
                                    using (MySqlConnection mConnection = new MySqlConnection(ConnectionString))
                                    {
                                        List<string> NewRows = new List<string>();
                                        StringBuilder NewAnnets = new StringBuilder();
                                        StringBuilder UpdatedAnnets = new StringBuilder();
                                        StringBuilder DeleteAnnets = new StringBuilder();
                                        if (obj.AttendanceAnnets[0].newAnnets.Count > 0)
                                        {
                                            NewAnnets.Append("INSERT INTO tbl_attendanceannetsdetails (FK_AttendanceID, AnnetsName,created_date) VALUES ");
                                            for (int i = 0; i < obj.AttendanceAnnets[0].newAnnets.Count; i++)
                                            {
                                                NewRows.Add(string.Format("('{0}','{1}',{2})", Result, MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceAnnets[0].newAnnets[i].AnnetsName), "now()"));
                                            }
                                        }
                                        if (obj.AttendanceAnnets[0].UpdateAnnets.Count > 0)
                                        {
                                            for (int i = 0; i < obj.AttendanceAnnets[0].UpdateAnnets.Count; i++)
                                            {
                                                UpdatedAnnets.Append("update tbl_attendanceannetsdetails set AnnetsName='" + MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceAnnets[0].UpdateAnnets[i].AnnetsName) + "',modified_date=now() where FK_AttendanceID=" + Convert.ToInt32(Result) + " and PK_AttendanceAnnetsID=" + obj.AttendanceAnnets[0].UpdateAnnets[i].PK_AttendanceAnnetsID + ";");
                                            }
                                        }
                                        if (obj.AttendanceAnnets[0].deletedAnnets.Count > 0)
                                        {
                                            for (int i = 0; i < obj.AttendanceAnnets[0].deletedAnnets.Count; i++)
                                            {
                                                DeleteAnnets.Append("update tbl_attendanceannetsdetails set Isdeleted=1,deleted_date=now() where FK_AttendanceID=" + Convert.ToInt32(Result) + " and PK_AttendanceAnnetsID=" + obj.AttendanceAnnets[0].deletedAnnets[i].PK_AttendanceAnnetsID + ";");
                                            }
                                        }
                                        bool flag = false;
                                        if (NewRows.Count > 0)
                                        {
                                            NewAnnets.Append(string.Join(",", NewRows));
                                            NewAnnets.Append(";");
                                            flag = true;
                                        }
                                        if (UpdatedAnnets.Length > 0)
                                        {
                                            NewAnnets.Append(UpdatedAnnets);
                                            flag = true;
                                        }
                                        if (DeleteAnnets.Length > 0)
                                        {
                                            NewAnnets.Append(DeleteAnnets);
                                            flag = true;
                                        }

                                        if (flag == true)
                                        {
                                            //NewAnnets.Append(string.Join(",", NewRows));
                                            //NewAnnets.Append(";");
                                            //NewAnnets.Append(UpdatedAnnets);
                                            //NewAnnets.Append(DeleteAnnets);

                                            mConnection.Open();
                                            using (MySqlCommand myCmd = new MySqlCommand(NewAnnets.ToString(), mConnection))
                                            {
                                                myCmd.CommandType = CommandType.Text;
                                                myCmd.ExecuteNonQuery();
                                            }
                                            if (mConnection.State == ConnectionState.Open)
                                            {
                                                mConnection.Close();
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        #endregion

                        #region start Visitors
                        {
                            if (obj.AttendanceVisitors != null)
                            {
                                string ConnectionString = GlobalVar.strAppConn;
                                {
                                    using (MySqlConnection mConnection = new MySqlConnection(ConnectionString))
                                    {
                                        List<string> NewRows = new List<string>();
                                        StringBuilder NewVisitors = new StringBuilder();
                                        StringBuilder UpdatedVisitors = new StringBuilder();
                                        StringBuilder DeleteVisitors = new StringBuilder();
                                        if (obj.AttendanceVisitors[0].newVisitors.Count > 0)
                                        {
                                            NewVisitors.Append("INSERT INTO tbl_attendancevisitorsdetails (FK_AttendanceID, VisitorsName,Rotarian_whohas_Brought,created_date) VALUES ");
                                            for (int i = 0; i < obj.AttendanceVisitors[0].newVisitors.Count; i++)
                                            {
                                                NewRows.Add(string.Format("('{0}','{1}','{2}',{3})", Result, MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceVisitors[0].newVisitors[i].VisitorsName), MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceVisitors[0].newVisitors[i].Rotarian_whohas_Brought), "now()"));
                                            }
                                        }
                                        if (obj.AttendanceVisitors[0].UpdateVisitors.Count > 0)
                                        {
                                            for (int i = 0; i < obj.AttendanceVisitors[0].UpdateVisitors.Count; i++)
                                            {
                                                UpdatedVisitors.Append("update tbl_attendancevisitorsdetails set VisitorsName='" + MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceVisitors[0].UpdateVisitors[i].VisitorsName) + "',Rotarian_whohas_Brought='" + MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceVisitors[0].UpdateVisitors[i].Rotarian_whohas_Brought) + "',modified_date=now() where FK_AttendanceID=" + Convert.ToInt32(Result) + " and PK_AttendanceVisitorID=" + obj.AttendanceVisitors[0].UpdateVisitors[i].PK_AttendanceVisitorID + ";");
                                            }
                                        }
                                        if (obj.AttendanceVisitors[0].deletedVisitors.Count > 0)
                                        {
                                            for (int i = 0; i < obj.AttendanceVisitors[0].deletedVisitors.Count; i++)
                                            {
                                                DeleteVisitors.Append("update tbl_attendancevisitorsdetails set isdeleted=1,deleted_date=now() where FK_AttendanceID=" + Convert.ToInt32(Result) + " and PK_AttendanceVisitorID=" + obj.AttendanceVisitors[0].deletedVisitors[i].PK_AttendanceVisitorID + ";");
                                            }
                                        }
                                        bool flag = false;
                                        if (NewRows.Count > 0)
                                        {
                                            NewVisitors.Append(string.Join(",", NewRows));
                                            NewVisitors.Append(";");
                                            flag = true;
                                        }
                                        if (UpdatedVisitors.Length > 0)
                                        {
                                            NewVisitors.Append(UpdatedVisitors);
                                            flag = true;
                                        }
                                        if (DeleteVisitors.Length > 0)
                                        {
                                            NewVisitors.Append(DeleteVisitors);
                                            flag = true;
                                        }

                                        if (flag == true)
                                        {
                                            mConnection.Open();
                                            using (MySqlCommand myCmd = new MySqlCommand(NewVisitors.ToString(), mConnection))
                                            {
                                                myCmd.CommandType = CommandType.Text;
                                                myCmd.ExecuteNonQuery();
                                            }
                                            if (mConnection.State == ConnectionState.Open)
                                            {
                                                mConnection.Close();
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        #endregion

                        #region start Rotarians
                        {
                            if (obj.AttendanceRotarians != null)
                            {
                                string ConnectionString = GlobalVar.strAppConn;
                                {
                                    using (MySqlConnection mConnection = new MySqlConnection(ConnectionString))
                                    {
                                        List<string> NewRows = new List<string>();
                                        StringBuilder NewRotarian = new StringBuilder();
                                        StringBuilder UpdatedRotarian = new StringBuilder();
                                        StringBuilder DeleteRotarian = new StringBuilder();
                                        if (obj.AttendanceRotarians[0].newRotarians.Count > 0)
                                        {
                                            NewRotarian.Append("INSERT INTO tbl_attendancerotariansdetails (FK_AttendanceID, RotarianID,RotarianName,ClubName,created_date) VALUES ");
                                            for (int i = 0; i < obj.AttendanceRotarians[0].newRotarians.Count; i++)
                                            {
                                                NewRows.Add(string.Format("('{0}','{1}','{2}','{3}',{4})", Result, MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceRotarians[0].newRotarians[i].RotarianID), MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceRotarians[0].newRotarians[i].RotarianName),MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceRotarians[0].newRotarians[i].ClubName), "now()"));
                                            }
                                        }
                                        if (obj.AttendanceRotarians[0].UpdateRotarians.Count > 0)
                                        {
                                            for (int i = 0; i < obj.AttendanceRotarians[0].UpdateRotarians.Count; i++)
                                            {
                                                UpdatedRotarian.Append("update tbl_attendancerotariansdetails set RotarianID='" + MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceRotarians[0].UpdateRotarians[i].RotarianID) + "',RotarianName='" + MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceRotarians[0].UpdateRotarians[i].RotarianName) + "',ClubName='" + MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceRotarians[0].UpdateRotarians[i].ClubName) + "',modified_date=now() where FK_AttendanceID=" + Convert.ToInt32(Result) + " and PK_AttendanceRotarianID=" + obj.AttendanceRotarians[0].UpdateRotarians[i].PK_AttendanceRotarianID + ";");
                                            }
                                        }
                                        if (obj.AttendanceRotarians[0].deletedRotarians.Count > 0)
                                        {
                                            for (int i = 0; i < obj.AttendanceRotarians[0].deletedRotarians.Count; i++)
                                            {
                                                DeleteRotarian.Append("update tbl_attendancerotariansdetails set isdeleted=1,deleted_date=now() where FK_AttendanceID=" + Convert.ToInt32(Result) + " and PK_AttendanceRotarianID=" + obj.AttendanceRotarians[0].deletedRotarians[i].PK_AttendanceRotarianID + ";");
                                            }
                                        }
                                        bool flag = false;
                                        if (NewRows.Count > 0)
                                        {
                                            NewRotarian.Append(string.Join(",", NewRows));
                                            NewRotarian.Append(";");
                                            flag = true;
                                        }
                                        if (UpdatedRotarian.Length > 0)
                                        {
                                            NewRotarian.Append(UpdatedRotarian);
                                            flag = true;
                                        }
                                        if (DeleteRotarian.Length > 0)
                                        {
                                            NewRotarian.Append(DeleteRotarian);
                                            flag = true;
                                        }

                                        if (flag == true)
                                        {
                                            mConnection.Open();
                                            using (MySqlCommand myCmd = new MySqlCommand(NewRotarian.ToString(), mConnection))
                                            {
                                                myCmd.CommandType = CommandType.Text;
                                                myCmd.ExecuteNonQuery();
                                            }
                                            if (mConnection.State == ConnectionState.Open)
                                            {
                                                mConnection.Close();
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        #endregion

                        #region start delegate
                        {
                            if (obj.AttendanceDistrictDelegate != null)
                            {
                                string ConnectionString = GlobalVar.strAppConn;
                                {
                                    using (MySqlConnection mConnection = new MySqlConnection(ConnectionString))
                                    {
                                        List<string> NewRows = new List<string>();
                                        StringBuilder NewDistrictDelegate = new StringBuilder();
                                        StringBuilder UpdatedDistrictDelegate = new StringBuilder();
                                        StringBuilder DeleteDistrictDelegate = new StringBuilder();
                                        if (obj.AttendanceDistrictDelegate[0].newDistrictDelegate.Count > 0)
                                        {
                                            NewDistrictDelegate.Append("INSERT INTO tbl_attendancedistrictdelegatesdetails (FK_AttendanceID, RotarianName,DistrictDesignation,ClubName,created_date) VALUES ");
                                            for (int i = 0; i < obj.AttendanceDistrictDelegate[0].newDistrictDelegate.Count; i++)
                                            {
                                                NewRows.Add(string.Format("('{0}','{1}','{2}','{3}',{4})", Result, MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceDistrictDelegate[0].newDistrictDelegate[i].RotarianName), MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceDistrictDelegate[0].newDistrictDelegate[i].DistrictDesignation), MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceDistrictDelegate[0].newDistrictDelegate[i].ClubName), "now()"));
                                            }
                                        }
                                        if (obj.AttendanceDistrictDelegate[0].UpdateDistrictDelegate.Count > 0)
                                        {
                                            for (int i = 0; i < obj.AttendanceDistrictDelegate[0].UpdateDistrictDelegate.Count; i++)
                                            {
                                                UpdatedDistrictDelegate.Append("update tbl_attendancedistrictdelegatesdetails set RotarianName='" + MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceDistrictDelegate[0].UpdateDistrictDelegate[i].RotarianName) + "',DistrictDesignation='" + MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceDistrictDelegate[0].UpdateDistrictDelegate[i].DistrictDesignation) + "',ClubName='" + MySql.Data.MySqlClient.MySqlHelper.EscapeString(obj.AttendanceDistrictDelegate[0].UpdateDistrictDelegate[i].ClubName) + "',modified_date=now() where FK_AttendanceID=" + Convert.ToInt32(Result) + " and PK_AttendanceDelegateID=" + obj.AttendanceDistrictDelegate[0].UpdateDistrictDelegate[i].PK_AttendanceDelegateID + ";");
                                            }
                                        }
                                        if (obj.AttendanceDistrictDelegate[0].deletedDistrictDelegate.Count > 0)
                                        {
                                            for (int i = 0; i < obj.AttendanceDistrictDelegate[0].deletedDistrictDelegate.Count; i++)
                                            {
                                                DeleteDistrictDelegate.Append("update tbl_attendancedistrictdelegatesdetails set isdeleted=1,deleted_date=now() where FK_AttendanceID=" + Convert.ToInt32(Result) + " and PK_AttendanceDelegateID=" + obj.AttendanceDistrictDelegate[0].deletedDistrictDelegate[i].PK_AttendanceDelegateID + ";");
                                            }
                                        }
                                        bool flag = false;
                                        if (NewRows.Count > 0)
                                        {
                                            NewDistrictDelegate.Append(string.Join(",", NewRows));
                                            NewDistrictDelegate.Append(";");
                                            flag = true;
                                        }
                                        if (UpdatedDistrictDelegate.Length > 0)
                                        {
                                            NewDistrictDelegate.Append(UpdatedDistrictDelegate);
                                            flag = true;
                                        }
                                        if (DeleteDistrictDelegate.Length > 0)
                                        {
                                            NewDistrictDelegate.Append(DeleteDistrictDelegate);
                                            flag = true;
                                        }

                                        if (flag == true)
                                        {
                                            mConnection.Open();
                                            using (MySqlCommand myCmd = new MySqlCommand(NewDistrictDelegate.ToString(), mConnection))
                                            {
                                                myCmd.CommandType = CommandType.Text;
                                                myCmd.ExecuteNonQuery();
                                            }
                                            if (mConnection.State == ConnectionState.Open)
                                            {
                                                mConnection.Close();
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        #endregion

                        TBAttendanceDetailsResult = new { status = "0", message = "success", Result };
                    }
                    else
                    {
                        TBAttendanceDetailsResult = new { status = "1", message = "failed" };
                    }
                }
                else
                {
                    TBAttendanceDetailsResult = new { status = "0", message = "No Input Data" };
                }
            }
            catch
            {
                TBAttendanceDetailsResult = new { status = "1", message = "failed" };
            }

            return new { TBAttendanceDetailsResult };
        }

        [System.Web.Http.HttpPost]
        public object getAttendanceDetails(GetAttendanceDetails_Input Input)
        {
            dynamic TBAttendanceDetailsResult;
            List<object> AttendanceDetailsResult = new List<object>();

            try
            {
                List<GetAttendanceDetails> Result = Attendance.getAttendanceDetails(Input);

                for (int i = 0; i < Result.Count; i++)
                {
                    AttendanceDetailsResult.Add(new { AttendanceResult = Result[i] });
                }

                if (AttendanceDetailsResult != null)
                {
                    TBAttendanceDetailsResult = new { status = "0", message = "success", AttendanceDetailsResult };
                }
                else
                {
                    TBAttendanceDetailsResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAttendanceDetailsResult = new { status = "1", message = "failed" };
            }

            return new { TBAttendanceDetailsResult };
        }

        [System.Web.Http.HttpPost]
        public object getAttendanceMemberDetails(GetAttendanceDetails_Input Input)
        {
            dynamic TBAttendanceMemberDetailsResult;
            try
            {
                List<newMembers> AttendanceMemberResult = Attendance.getAttendanceMemberDetails(Input);

                if (AttendanceMemberResult.Count != 0)
                {
                    TBAttendanceMemberDetailsResult = new { status = "0", message = "success", AttendanceMemberResult };
                }
                else
                {
                    TBAttendanceMemberDetailsResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAttendanceMemberDetailsResult = new { status = "1", message = "failed" };
            }

            return new { TBAttendanceMemberDetailsResult };
        }

        [System.Web.Http.HttpPost]
        public object getAttendanceAnnsDetails(GetAttendanceDetails_Input Input)
        {
            dynamic TBAttendanceAnnsDetailsResult;
            try
            {
                List<newAnns> AttendanceAnnsResult = Attendance.getAttendanceAnnsDetails(Input);

                if (AttendanceAnnsResult.Count != 0)
                {
                    TBAttendanceAnnsDetailsResult = new { status = "0", message = "success", AttendanceAnnsResult };
                }
                else
                {
                    TBAttendanceAnnsDetailsResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAttendanceAnnsDetailsResult = new { status = "1", message = "failed" };
            }

            return new { TBAttendanceAnnsDetailsResult };
        }

        [System.Web.Http.HttpPost]
        public object getAttendanceAnnetsDetails(GetAttendanceDetails_Input Input)
        {
            dynamic TBAttendanceAnnetsDetailsResult;
            
            try
            {
                List<newAnnets> AttendanceAnnetsResult = Attendance.getAttendanceAnnetsDetails(Input);

                if (AttendanceAnnetsResult.Count != 0)
                {
                    TBAttendanceAnnetsDetailsResult = new { status = "0", message = "success", AttendanceAnnetsResult };
                }
                else
                {
                    TBAttendanceAnnetsDetailsResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAttendanceAnnetsDetailsResult = new { status = "1", message = "failed" };
            }

            return new { TBAttendanceAnnetsDetailsResult };
        }


        [System.Web.Http.HttpPost]
        public object getAttendanceVisitorsDetails(GetAttendanceDetails_Input Input)
        {
            dynamic TBAttendanceVisitorsDetailsResult;
            
            try
            {
                List<newVisitors> AttendanceVisitorsResult = Attendance.getAttendanceVisitorsDetails(Input);

                if (AttendanceVisitorsResult.Count != 0)
                {
                    TBAttendanceVisitorsDetailsResult = new { status = "0", message = "success", AttendanceVisitorsResult };
                }
                else
                {
                    TBAttendanceVisitorsDetailsResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAttendanceVisitorsDetailsResult = new { status = "1", message = "failed" };
            }

            return new { TBAttendanceVisitorsDetailsResult };
        }

        [System.Web.Http.HttpPost]
        public object getAttendanceRotariansDetails(GetAttendanceDetails_Input Input)
        {
            dynamic TBAttendanceRotariansDetailsResult;
            
            try
            {
                List<newRotarians> AttendanceRotariansResult = Attendance.getAttendanceRotariansDetails(Input);

                if (AttendanceRotariansResult.Count != 0)
                {
                    TBAttendanceRotariansDetailsResult = new { status = "0", message = "success", AttendanceRotariansResult };
                }
                else
                {
                    TBAttendanceRotariansDetailsResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAttendanceRotariansDetailsResult = new { status = "1", message = "failed" };
            }

            return new { TBAttendanceRotariansDetailsResult };
        }

        [System.Web.Http.HttpPost]
        public object getAttendanceDistrictDeleagateDetails(GetAttendanceDetails_Input Input)
        {
            dynamic TBAttendanceDistrictDeleagateDetailsResult;
            
            try
            {
                List<newDistrictDelegate> AttendanceDistrictDeleagateResult = Attendance.getAttendanceDistrictDeleagateDetails(Input);

                if (AttendanceDistrictDeleagateResult.Count != 0)
                {
                    TBAttendanceDistrictDeleagateDetailsResult = new { status = "0", message = "success", AttendanceDistrictDeleagateResult };
                }
                else
                {
                    TBAttendanceDistrictDeleagateDetailsResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAttendanceDistrictDeleagateDetailsResult = new { status = "1", message = "failed" };
            }

            return new { TBAttendanceDistrictDeleagateDetailsResult };
        }

        [HttpPost]
        public object GetrotarianDetailsbyRotarianID(GetAttendanceRotarian_Input Input)
        {
            dynamic TBGetRotarianResult;
            try
            {
                List<GetAttendanceRotarianDetailsbyID> Result = Attendance.GetrotarianDetailsbyRotarianID(Input);

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

        [HttpPost]
        public object GetAttendanceDistrinctDelegateDetailsByRotarianName(GetAttendanceRotarian_Input Input)
        {
            dynamic TBGetRotarianResult;
            try
            {
                List<GetAttendanceDelegateDetailsByRotarianName> Result = Attendance.GetAttendanceDistrinctDelegateDetailsByRotarianName(Input);

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

        [System.Web.Http.HttpPost]
        public object AttendanceDelete(GetAttendanceDetails_Input Input)
        {
            dynamic TBAttendanceDetailsResult=null;

            try
            {
                int Result = Attendance.getAttendanceDelete(Input);

                if (Result != 0)
                {
                    TBAttendanceDetailsResult = new { status = "0", message = "success"};
                }
                else
                {
                    TBAttendanceDetailsResult = new { status = "1", message = "failed" };
                }
            }
            catch
            {
                TBAttendanceDetailsResult = new { status = "1", message = "failed" };
            }

            return TBAttendanceDetailsResult;
        }
    }
}
