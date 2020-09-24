using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchBaseWebAPI.Models;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class SubGroupDirectory
    {
        //private static TouchBaseWebAPI.Data.row_productionEntities _DbTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();
        public static string ChildSubGrp_Ids = "";
        public static string Admin_Ids = "";
        public static int parentID = -1;

        public static List<SubGroupList> GetSubGroupList(SubGroup sub)
        {
            try
            {
                var grpId = new MySqlParameter("?grpId", sub.groupId);
                var profileID = new MySqlParameter("?profileID", sub.memberProfileId);
                var subGroupId = new MySqlParameter("?subGroupId", string.IsNullOrEmpty(sub.subGroupID) ? "0" : sub.subGroupID);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<SubGroupList>("CALL V3_USPGetSubGroupList(?grpId,?profileID,?subGroupId)", grpId, profileID, subGroupId).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get SubGrp Directory
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>

        public static SubGrpDirectoryResult GetSubGroupDirectory(SubGrpDirectoryInput sub)
        {
            string subGrpIDs = "";
            SubGrpDirectoryResult directory = new SubGrpDirectoryResult();
            try
            {
                // Check if member is group Admin 


                MySqlParameter[] param = new MySqlParameter[3];
                param[0] = new MySqlParameter("?grpId", sub.groupId);
                param[1] = new MySqlParameter("?profileID", sub.profileId);
                param[2] = new MySqlParameter("?IsAdmin", "0");
                param[2].Direction = ParameterDirection.InputOutput;
                DataSet dsSubgrpIDs = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V4_SubGroupIsAdmin", param);

                if (dsSubgrpIDs != null)
                {
                    for (int i = 0; i < dsSubgrpIDs.Tables[0].Rows.Count; i++)
                    {
                        subGrpIDs += dsSubgrpIDs.Tables[0].Rows[i]["subGrpID"].ToString();
                        subGrpIDs += ",";
                    }
                    subGrpIDs = subGrpIDs.TrimEnd(',');

                    // Member Is subGroup Admin return subgrp List
                    if (Convert.ToInt32(param[2].Value.ToString()) > 0 && subGrpIDs != "")
                    {

                        if (sub.parentID == "0")
                        {
                            MySqlParameter[] param1 = new MySqlParameter[4];
                            param1[0] = new MySqlParameter("?grpId", sub.groupId);
                            param1[1] = new MySqlParameter("?profileID", sub.profileId);
                            param1[2] = new MySqlParameter("?subGrpIds", subGrpIDs);
                            param1[3] = new MySqlParameter("?parentID", sub.parentID);

                            using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                            {
                                context.Connection.Open();
                                var subgrpList = context.ExecuteStoreQuery<SubGrpListResult>("V4_USPGetSubGrpList(?grpId,?profileID,?subGrpIds,?parentID)", param1).ToList();
                                directory.subGrpList = subgrpList;
                            }
                        }

                        else
                        {
                            MySqlParameter[] param2 = new MySqlParameter[3];
                            param2[0] = new MySqlParameter("?grpId", sub.groupId);
                            param2[1] = new MySqlParameter("?profileID", sub.profileId);
                            param2[2] = new MySqlParameter("?parentID", sub.parentID);
                            DataSet dsSubgrpList = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V4_USPGetSubGrpDetails", param2);
                            if (dsSubgrpList != null)
                            {
                                if (dsSubgrpList.Tables[0].Rows.Count > 0)
                                {
                                    List<SubGrpListResult> subgrpList = GlobalFuns.DataTableToList<SubGrpListResult>(dsSubgrpList.Tables[0]);
                                    directory.subGrpList = subgrpList;
                                }
                                if (dsSubgrpList.Tables[1].Rows.Count > 0)
                                {
                                    List<MemberList> memList = GlobalFuns.DataTableToList<MemberList>(dsSubgrpList.Tables[1]);

                                    foreach (MemberList mem in memList)
                                    {
                                        if (!string.IsNullOrEmpty(mem.pic))
                                        {
                                            string ImageName = mem.pic.ToString();
                                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                                            mem.pic = path + ImageName;
                                        }
                                    }

                                    directory.memberList = memList;
                                }
                            }
                        }
                    }
                    //IF Member Is Not an Admin Then return Member Directory
                    else if (param[2].Value.ToString() == "0" && subGrpIDs != "")
                    {
                        MySqlParameter[] param3 = new MySqlParameter[3];
                        param3[0] = new MySqlParameter("?grpId", sub.groupId);
                        param3[1] = new MySqlParameter("?profileID", sub.profileId);
                        param3[2] = new MySqlParameter("?subGrpIds", subGrpIDs);

                        using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                        {
                            context.Connection.Open();
                            var memberList = context.ExecuteStoreQuery<MemberList>("CALL V4_USPGetSubGrpMemberList(?grpId,?profileID,?subGrpIds)", param3).ToList();
                            foreach (MemberList mem in memberList)
                            {
                                if (!string.IsNullOrEmpty(mem.pic))
                                {
                                    string ImageName = mem.pic.ToString();
                                    string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                                    mem.pic = path + ImageName;
                                }
                            }
                            directory.memberList = memberList;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            if (directory.memberList == null)
            {
                directory.memberList = new List<MemberList>();
            }
            if (directory.subGrpList == null)
            {
                directory.subGrpList = new List<SubGrpListResult>();
            }
            return directory;
        }

        public static void FindChildSubgroup(int parentSubGrpID)
        {
            DataSet ds = new DataSet();

            string ssql = "SELECT pk_subgroup_id as subID,fk_admin_profileID as Admin FROM subgroup_master where fk_subgrp_ParentID=" + parentSubGrpID;
            ds = MySqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["TBWSConnectionString"].ToString(), CommandType.Text, ssql);

            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //result += ds.Tables[0].Rows[i]["subID"].ToString();
                    // result += ",";
                    parentID = Convert.ToInt32(ds.Tables[0].Rows[i]["subID"].ToString());
                    ChildSubGrp_Ids += parentID;
                    ChildSubGrp_Ids += ",";
                    Admin_Ids = ds.Tables[0].Rows[i]["Admin"].ToString();
                    Admin_Ids += ",";
                    FindChildSubgroup(parentID);
                }
            }

        }

        public static List<MemberListResult> GetSubGroupMemberList(SubGrpDirectoryInput mem)
        {
            string subGrpIDs = "";
            List<MemberListResult> memberList = new List<MemberListResult>();
            try
            {
                // Check if member is subgroup Admin 
                MySqlParameter[] param = new MySqlParameter[3];
                param[0] = new MySqlParameter("?grpId", mem.groupId);
                param[1] = new MySqlParameter("?profileID", mem.profileId);
                param[2] = new MySqlParameter("?IsAdmin", "1");
                param[2].Direction = ParameterDirection.InputOutput;
                DataSet dsSubgrpIDs = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V4_SubGroupIsAdmin", param);

                if (dsSubgrpIDs != null)
                {
                    for (int i = 0; i < dsSubgrpIDs.Tables[0].Rows.Count; i++)
                    {
                        subGrpIDs += dsSubgrpIDs.Tables[0].Rows[i]["subGrpID"].ToString();
                        subGrpIDs += ",";
                    }
                    subGrpIDs = subGrpIDs.TrimEnd(',');
                }
                if (Convert.ToInt32(param[2].Value) > 0)
                {
                    MySqlParameter[] param1 = new MySqlParameter[3];
                    param1[0] = new MySqlParameter("?grpId", mem.groupId);
                    param1[1] = new MySqlParameter("?profileID", mem.profileId);
                    param1[2] = new MySqlParameter("?subGrpIds", subGrpIDs);

                    using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                    {
                        context.Connection.Open();
                        memberList = context.ExecuteStoreQuery<MemberListResult>("CALL V4_USPGetSubGrpAdminMemberList(?grpId,?profileID,?subGrpIds)", param1).ToList();
                        foreach (MemberListResult member in memberList)
                        {
                            if (!string.IsNullOrEmpty(member.pic))
                            {
                                string ImageName = member.pic.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                                member.pic = path + ImageName;
                            }
                        }
                    }
                }

            }
            catch
            {
                throw;
            }
            return memberList;
        }

        public static string GetAdminSubGroupList(string grpID, string profileID)
        {
            string subGrpIDs = "";
            try
            {
                string sql = "SELECT pk_subgroup_id as subGrpID From subgroup_master where fk_group_master_id=" + grpID + " and (fk_Admin_ProfileID =" + profileID + " or created_by=" + profileID + ")";
                DataSet dsSubgrpIDs = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.Text, sql);
                if (dsSubgrpIDs != null)
                {
                    for (int i = 0; i < dsSubgrpIDs.Tables[0].Rows.Count; i++)
                    {
                        subGrpIDs += dsSubgrpIDs.Tables[0].Rows[i]["subGrpID"].ToString();
                        subGrpIDs += ",";
                    }
                    subGrpIDs = subGrpIDs.TrimEnd(',');
                }
            }
            catch
            {
                throw;
            }
            return subGrpIDs;
        }
    }
}