﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.Data;
using System.Data;
using System.Configuration;
using System.Net;
using System.Threading;
using System.Text;
using System.Dynamic;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class GroupMaster
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DbTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        # region Group Operations

        public static clsgroup_master CreateGroup(clsgroup_master group)
        {
            try
            {

                MySqlParameter[] parameters = new MySqlParameter[16];
                parameters[0] = new MySqlParameter("@group_master_id", group.grpId);
                parameters[1] = new MySqlParameter("@group_name", group.grpName);
                parameters[2] = new MySqlParameter("@group_type", string.IsNullOrEmpty(group.grpType) ? "" : group.grpType);
                parameters[3] = new MySqlParameter("@group_category", string.IsNullOrEmpty(group.grpCategory) ? "" : group.grpCategory);
                parameters[4] = new MySqlParameter("@address1", string.IsNullOrEmpty(group.addrss1) ? "" : group.addrss1);
                parameters[5] = new MySqlParameter("@address2", string.IsNullOrEmpty(group.addrss2) ? "" : group.addrss2);
                parameters[6] = new MySqlParameter("@city", string.IsNullOrEmpty(group.city) ? "" : group.city);
                parameters[7] = new MySqlParameter("@state", string.IsNullOrEmpty(group.state) ? "" : group.state);
                parameters[8] = new MySqlParameter("@pincode", string.IsNullOrEmpty(group.pincode) ? "" : group.pincode);
                parameters[9] = new MySqlParameter("@country", string.IsNullOrEmpty(group.country) ? "" : group.country);
                parameters[10] = new MySqlParameter("@userID", group.userId);
                parameters[11] = new MySqlParameter("@grpImageID", string.IsNullOrEmpty(group.grpImageID) ? "0" : group.grpImageID);

                parameters[12] = new MySqlParameter("@mobileno", group.mobile);
                parameters[13] = new MySqlParameter("@emailid", group.emailid);
                parameters[14] = new MySqlParameter("@website", string.IsNullOrEmpty(group.website) ? "" : group.website);

                parameters[15] = new MySqlParameter("@other", string.IsNullOrEmpty(group.other) ? "" : group.other);

                //parameters[11] = new MySqlParameter("@created_by", group.userId);
                string storeproc = "V1_USP_CreateGroup";
                DataSet ds = null;
                ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, storeproc, parameters);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    group.grpId = ds.Tables[0].Rows[0]["groupID"].ToString();
                    group.emailid = ds.Tables[0].Rows[0]["member_emailid"].ToString();
                    group.mobile = ds.Tables[0].Rows[0]["member_mobile"].ToString();
                    //group.userpwd = ds.Tables[0].Rows[0]["pwd"].ToString();
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    group.grpImageName = ds.Tables[1].Rows[0]["grpImageName"].ToString();
                }

                return group;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

        public static List<GroupListResult> getAllGroupLists(string memberId, string IMEINO, string updatedOn)
        {
            try
            {

                List<GroupListResult> grpList = new List<GroupListResult>();
                MySqlParameter[] parameters = new MySqlParameter[3];
                parameters[0] = new MySqlParameter("?memberId", memberId);
                parameters[1] = new MySqlParameter("?imei_no", IMEINO);
                parameters[2] = new MySqlParameter("?updatedOn", updatedOn);

                //string storeproc = "V4_USPGetAllGroupListNew";
                string storeproc = "V7_USPGetAllGroupListNew";

                DataSet ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, storeproc, parameters);

                if (ds.Tables[0].Rows.Count == 0)
                {
                    return grpList;
                }
                if (ds.Tables[0].Rows[0]["grpId"].ToString() == "0")
                {
                    grpList = null;
                }
                else
                {

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataTable dtGrpModule = ds.Tables[1].Clone();

                        //fetch modules for each group
                        for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
                        {
                            if (ds.Tables[0].Rows[i]["grpId"].ToString() == ds.Tables[1].Rows[j]["groupId"].ToString())
                            {
                                if (ds.Tables[0].Rows[i]["isGrpAdmin"].ToString() == "No" && ds.Tables[1].Rows[j]["moduleId"].ToString() == "5")
                                {
                                    // Do nothing;
                                    // skip subgroup mobule for normal member
                                }
                                else
                                {
                                    //Add all modules for admin
                                    DataRow dr = ds.Tables[1].Rows[j];
                                    dtGrpModule.ImportRow(dr);
                                }
                            }
                        }

                        //Create nested json of group and module list and update image path

                        List<GroupModulesList> moduleList = GlobalFuns.DataTableToList<GroupModulesList>(dtGrpModule);
                        foreach (GroupModulesList module in moduleList)
                        {
                            if (!string.IsNullOrEmpty(module.image))
                            {
                                string Image = module.image.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Images/";
                                module.image = path + Image;
                            }
                            else
                            {
                                string Image = module.image.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/noimg.png";
                                module.image = path + Image;
                            }
                        }

                        GroupListResult group = new GroupListResult();
                        group.grpId = ds.Tables[0].Rows[i]["grpId"].ToString();
                        group.grpImg = ds.Tables[0].Rows[i]["grpImg"].ToString();
                        group.grpName = ds.Tables[0].Rows[i]["grpName"].ToString();
                        group.grpProfileId = ds.Tables[0].Rows[i]["grpProfileId"].ToString();
                        group.isGrpAdmin = ds.Tables[0].Rows[i]["isGrpAdmin"].ToString();
                        group.myCategory = ds.Tables[0].Rows[i]["myCategory"].ToString();
                        group.ModuleList = moduleList;
                        grpList.Add(group);
                    }
                    foreach (GroupListResult grp in grpList)
                    {
                        if (!string.IsNullOrEmpty(grp.grpImg.ToString()))
                        {
                            string imageName = grp.grpImg.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Group/";
                            grp.grpImg = path + imageName;
                        }
                    }
                }
                return grpList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static GroupModulesSyncResult GetAllGroupListsSync(string memberId, string updatedOn)
        {
            try
            {
                DateTime dt = DateTime.Now;
                if (updatedOn != null)
                {
                    dt = Convert.ToDateTime(updatedOn);
                    dt = dt.AddSeconds(-1);
                }
                GroupModulesSyncResult grpModuleResult = new GroupModulesSyncResult();
                GroupListSync grpList = new GroupListSync();
                ModuleListSync moduleList = new ModuleListSync();

                MySqlParameter[] parameters = new MySqlParameter[2];
                parameters[0] = new MySqlParameter("?masterID", memberId);
                parameters[1] = new MySqlParameter("?updatedOn", dt);


                //=================================== for default date return single arrays (new records) ==================================================
                if (updatedOn == "1970-01-01 00:00:00" || updatedOn == "1970-01-01 12:00:00")
                {
                    string storeproc = "V6_USPGetAllGroupLisSync";
                    DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, storeproc, parameters);
                    DataTable dtnewGroups = result.Tables[0];

                    // List of New Groups
                    List<GroupResult> NewGroupList = new List<GroupResult>();
                    List<GroupResult> DeletedGroupList = new List<GroupResult>();
                    List<GroupResult> UpdatedGroupList = new List<GroupResult>();
                    if (dtnewGroups.Rows.Count > 0)
                    {
                        NewGroupList = GlobalFuns.DataTableToList<GroupResult>(dtnewGroups);
                        foreach (GroupResult grp in NewGroupList)
                        {
                            if (!string.IsNullOrEmpty(grp.grpImg.ToString()))
                            {
                                string imageName = grp.grpImg.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Group/";
                                grp.grpImg = path + imageName;
                            }
                            if (grp.IsSubGrpAdmin == "Yes" && grp.isGrpAdmin == "No")
                            {
                                grp.isGrpAdmin = "Partial";
                            }
                        }
                    }
                    grpList.NewGroupList = NewGroupList;
                    grpList.UpdatedGroupList = UpdatedGroupList;
                    grpList.DeletedGroupList = DeletedGroupList;


                    //============================= create modules List =====================================================
                    DataTable dtnewModules = result.Tables[1];

                    List<GroupModulesList> NewModuleList = new List<GroupModulesList>();
                    List<GroupModulesList> UpdatedModuleList = new List<GroupModulesList>();
                    List<GroupModulesList> DeletedModuleList = new List<GroupModulesList>();

                    if (dtnewModules.Rows.Count > 0)
                    {
                        NewModuleList = GlobalFuns.DataTableToList<GroupModulesList>(dtnewModules);
                        foreach (GroupModulesList module in NewModuleList)
                        {
                            if (!string.IsNullOrEmpty(module.image))
                            {
                                string Image = module.image.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Images/";
                                module.image = path + Image;
                            }
                            else
                            {
                                string Image = module.image.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/noimg.png";
                                module.image = path + Image;
                            }
                        }
                    }
                    moduleList.NewModuleList = NewModuleList;
                    moduleList.UpdatedModuleList = UpdatedModuleList;
                    moduleList.DeletedModuleList = DeletedModuleList;
                    grpModuleResult.GroupList = grpList;
                    grpModuleResult.ModuleList = moduleList;

                }
                else
                {

                    //=================================== for any other date return all three arrays (new/updated/ deleted records) ==================================================

                    string storeproc = "V5_USPGetAllGroupLisSync";
                    DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, storeproc, parameters);

                    //============================= create Group List =====================================================
                    DataTable dtnewGroups = result.Tables[0];
                    DataTable dtUpdatedGroups = result.Tables[1];
                    DataTable dtDeletedGroups = result.Tables[2];

                    // List of New Groups
                    List<GroupResult> NewGroupList = new List<GroupResult>();

                    if (dtnewGroups.Rows.Count > 0)
                    {
                        NewGroupList = GlobalFuns.DataTableToList<GroupResult>(dtnewGroups);
                        foreach (GroupResult grp in NewGroupList)
                        {
                            if (!string.IsNullOrEmpty(grp.grpImg.ToString()))
                            {
                                string imageName = grp.grpImg.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Group/";
                                grp.grpImg = path + imageName;
                            }
                            if (grp.IsSubGrpAdmin == "Yes" && grp.isGrpAdmin == "No")
                            {
                                grp.isGrpAdmin = "Partial";
                            }
                        }
                    }

                    // List of Updated Groups
                    List<GroupResult> UpdatedGroupList = new List<GroupResult>();
                    if (dtUpdatedGroups.Rows.Count > 0)
                    {
                        UpdatedGroupList = GlobalFuns.DataTableToList<GroupResult>(dtUpdatedGroups);
                        foreach (GroupResult grp in UpdatedGroupList)
                        {

                            if (!string.IsNullOrEmpty(grp.grpImg.ToString()))
                            {
                                string imageName = grp.grpImg.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Group/";
                                grp.grpImg = path + imageName;
                            }
                            if (grp.IsSubGrpAdmin == "Yes" && grp.isGrpAdmin == "No")
                            {
                                grp.isGrpAdmin = "Partial";
                            }
                        }
                    }


                    // List of Deleted Groups
                    List<GroupResult> DeletedGroupList = new List<GroupResult>();
                    if (dtDeletedGroups.Rows.Count > 0)
                    {
                        DeletedGroupList = GlobalFuns.DataTableToList<GroupResult>(dtDeletedGroups);
                        foreach (GroupResult grp in DeletedGroupList)
                        {
                            if (!string.IsNullOrEmpty(grp.grpImg.ToString()))
                            {
                                string imageName = grp.grpImg.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Group/";
                                grp.grpImg = path + imageName;
                            }
                            if (grp.IsSubGrpAdmin == "Yes")
                            {
                                grp.isGrpAdmin = "Partial";
                            }
                        }
                    }

                    grpList.NewGroupList = NewGroupList;
                    grpList.UpdatedGroupList = UpdatedGroupList;
                    grpList.DeletedGroupList = DeletedGroupList;

                    //============================= create modules List =====================================================
                    DataTable dtnewModules = result.Tables[3];
                    DataTable dtUpdatedModules = result.Tables[4];
                    DataTable dtDeletedModules = result.Tables[5];


                    List<GroupModulesList> NewModuleList = new List<GroupModulesList>();
                    if (dtnewModules.Rows.Count > 0)
                    {
                        NewModuleList = GlobalFuns.DataTableToList<GroupModulesList>(dtnewModules);
                        foreach (GroupModulesList module in NewModuleList)
                        {
                            if (!string.IsNullOrEmpty(module.image))
                            {
                                string Image = module.image.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Images/";
                                module.image = path + Image;
                            }
                            else
                            {
                                string Image = module.image.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/noimg.png";
                                module.image = path + Image;
                            }
                        }
                    }


                    List<GroupModulesList> UpdatedModuleList = new List<GroupModulesList>();
                    if (dtUpdatedModules.Rows.Count > 0)
                    {
                        UpdatedModuleList = GlobalFuns.DataTableToList<GroupModulesList>(dtUpdatedModules);
                        foreach (GroupModulesList module in UpdatedModuleList)
                        {
                            if (!string.IsNullOrEmpty(module.image))
                            {
                                string Image = module.image.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Images/";
                                module.image = path + Image;
                            }
                            else
                            {
                                string Image = module.image.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/noimg.png";
                                module.image = path + Image;
                            }
                        }
                    }


                    List<GroupModulesList> DeletedModuleList = new List<GroupModulesList>();
                    if (dtDeletedModules.Rows.Count > 0)
                    {
                        DeletedModuleList = GlobalFuns.DataTableToList<GroupModulesList>(dtDeletedModules);
                        foreach (GroupModulesList module in DeletedModuleList)
                        {
                            if (!string.IsNullOrEmpty(module.image))
                            {
                                string Image = module.image.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Images/";
                                module.image = path + Image;
                            }
                            else
                            {
                                string Image = module.image.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/noimg.png";
                                module.image = path + Image;
                            }
                        }
                    }
                    moduleList.NewModuleList = NewModuleList;
                    moduleList.UpdatedModuleList = UpdatedModuleList;
                    moduleList.DeletedModuleList = DeletedModuleList;
                    grpModuleResult.GroupList = grpList;
                    grpModuleResult.ModuleList = moduleList;
                }
                return grpModuleResult;

            }
            catch (Exception)
            {
                throw;
            }
        }

        //public static object getModuleLists()
        //{
        //    try
        //    {
        //        var Result = _DbTouchbase.ExecuteStoreQuery<ModulesList>("CALL USPGetModuleList()").ToList();

        //        return Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public static string GetGrpEmail(GroupInfo grp)
        {
            string email;
            try
            {
                string query = string.Empty;

                //Added by Nandu on 02-02-2017 Task --> feedback replica
                query = "SELECT email_id FROM feedback_master where fk_module_id='" + grp.moduleID + "' and fk_group_id='" + grp.grpID + "'";
                email = Convert.ToString(MySqlHelper.ExecuteScalar(GlobalVar.strAppConn, CommandType.Text, query));

                if (email == "")
                {
                    query = "Select group_email from group_master where pk_group_master_id=" + grp.grpID;
                    email = Convert.ToString(MySqlHelper.ExecuteScalar(GlobalVar.strAppConn, CommandType.Text, query));
                }
            }
            catch
            {
                throw;
            }
            return email;
        }

        public static List<GroupResult> getUpdatedGroupLists(string memberProfileId, string mycategory, string memberMasterId)
        {
            try
            {
                var memberProId = new MySqlParameter("?memberProId", memberProfileId);
                var grpcategory = new MySqlParameter("?grpcategory", mycategory);
                var mainMasterId = new MySqlParameter("?mainMasterId", memberMasterId);

                var Result = _DbTouchbase.ExecuteStoreQuery<GroupResult>("CALL USPUpdatedGroupList(?memberProId,?grpcategory,?mainMasterId)", memberProId, grpcategory, mainMasterId).ToList();

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<GroupResult> getRemoveGroupCategory(string memberId, string mainmemberid)
        {
            try
            {
                var MemberId = new MySqlParameter("?memberId", memberId);
                var MainMemberId = new MySqlParameter("?mainmemberid", mainmemberid);
                var Result = _DbTouchbase.ExecuteStoreQuery<GroupResult>("CALL USPDeleteGroupCategory(?memberId,?mainmemberId)", MemberId, MainMemberId).ToList();

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet GetGroupDtls(string memDeleteId, string profileId)
        {
            DataSet ds = new DataSet();
            string strQuery = "SELECT member_mobile_no,fk_country_ID,member_email_id,group_name,pk_group_master_id as grpID,(select member_name from member_master_profile where user_role='Admin' and fk_group_master_id=pk_group_master_id and pk_member_master_profile_id=" + profileId + ") as admin FROM group_master join member_master_profile on group_master.pk_group_master_id=member_master_profile.fk_group_master_id where pk_member_master_profile_id=" + memDeleteId;
            try
            {
                ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.Text, strQuery);
            }
            catch
            { }
            return ds;
        }

        public static List<GetGroupInfo> getGroupInfo(MemberProfile mem)
        {
            try
            {
                var memberMainId = new MySqlParameter("?memberMainId", mem.memberMainId);
                var groupId = new MySqlParameter("?groupId", mem.groupId);

                var Result = _DbTouchbase.ExecuteStoreQuery<GetGroupInfo>("CALL V1_USPGetGroupInfo(?memberMainId,?groupId)", memberMainId, groupId).ToList();
                foreach (GetGroupInfo grp in Result)
                {
                    if (!string.IsNullOrEmpty(grp.grpImg))
                    {
                        string ImageName = grp.grpImg.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Group/";
                        grp.grpImg = path + ImageName;
                    }
                }

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object deleteEntity(string memberId, string groupId)
        {
            try
            {
                var GrpId = new MySqlParameter("?grpId", groupId);
                var profileID = new MySqlParameter("?profileID", memberId);

                string Result = _DbTouchbase.ExecuteStoreQuery<string>("CALL V2_USPDeleteGroup(?grpId,?profileID)", GrpId, profileID).SingleOrDefault();

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet GetEntityInfo(GroupInfo group)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];
                param[0] = new MySqlParameter("?grpId", group.grpID);
                param[1] = new MySqlParameter("?moduleID", group.moduleID);

                DataSet Result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V4_USPEntityInfo", param);

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<getGroupInfo> getGroupDetail(GroupInfo grp)
        {
            try
            {
                var grpId = new MySqlParameter("?groupId", grp.grpID);

                var Result = _DbTouchbase.ExecuteStoreQuery<getGroupInfo>("CALL V1_USPGetGroupDetail(?groupId)", grpId).ToList();

                foreach (getGroupInfo g in Result)
                {
                    if (!string.IsNullOrEmpty(g.grpImage))
                    {
                        string grp_Image = g.grpImage.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Group/";
                        g.grpImage = path + grp_Image;
                    }
                }

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<GroupCategoryResult> getAllGroupCategory()
        {
            try
            {
                var Result = _DbTouchbase.ExecuteStoreQuery<GroupCategoryResult>("CALL V1_USPGetAllGroupCategory()").ToList();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ClubHistory GetClubHistory(GroupInfo grp)
        {
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[1];
                parameters[0] = new MySqlParameter("?grpId", grp.grpID);

                var result = _DbTouchbase.ExecuteStoreQuery<ClubHistory>("CALL USPGetClubHistory(?grpId)", parameters).SingleOrDefault();
                return result;
            }
            catch
            {
                throw;
            }
        }

        # endregion

        # region Modules Operation

        public static List<ModulesList> getModuleLists()
        {
            try
            {
                List<ModulesList> Result = _DbTouchbase.ExecuteStoreQuery<ModulesList>("CALL V1_USPGetModuleList()").ToList();

                foreach (ModulesList m in Result)
                {
                    if (!string.IsNullOrEmpty(m.moduleImage))
                    {
                        string moduleImage = m.moduleImage.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "images/";
                        m.moduleImage = path + moduleImage;
                    }
                }

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ModuleMaster> GetReplicaInfo(string LastModuleID)
        {
            try
            {
                var ModuleID = new MySqlParameter("?ModuleID", LastModuleID);
                List<ModuleMaster> Result = _DbTouchbase.ExecuteStoreQuery<ModuleMaster>("CALL V5_USPGetAllModules(?ModuleID)", ModuleID).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<GroupModulesList> getGroupModuleLists(string groupId, string memberProfileId)
        {
            try
            {
                var GrpId = new MySqlParameter("?groupID", groupId);
                var masterProfileID = new MySqlParameter("?masterProfileID", memberProfileId);
                var Result = _DbTouchbase.ExecuteStoreQuery<GroupModulesList>("CALL V1_USPGetGroupModuleList(?groupID,?masterProfileID)", GrpId, masterProfileID).ToList();

                foreach (GroupModulesList module in Result)
                {
                    if (!string.IsNullOrEmpty(module.image))
                    {
                        string Image = module.image.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Images/";
                        module.image = path + Image;
                    }
                    else
                    {
                        string Image = module.image.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/noimg.png";
                        module.image = path + Image;
                    }
                }

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //_DbTouchbase.Dispose();
            }
        }

        public static object UpdateModuleDashboard(string memberProfileId, string modulelist)
        {
            try
            {
                string spinpparam = "";
                string[] modulearr = modulelist.Split(',');
                for (int i = 0; i < modulearr.Length; i++)
                {
                    if (spinpparam != "")
                    {
                        spinpparam = spinpparam + ",";
                    }
                    spinpparam = spinpparam + "(" + memberProfileId + "," + modulearr[i] + ")";
                }

                var Result = "";

                //var varmemberProfileId = new MySqlParameter("?memberProfileId", memberProfileId);
                //var varmodulelist = new MySqlParameter("?modulelist", spinpparam);
                //var Result = _DbTouchbase.ExecuteStoreQuery<GroupModulesList>("CALL UPD_member_dashboard(?memberProfileId,?modulelist)", varmemberProfileId, varmodulelist).ToList();

                ////MySqlParameter[] parameters = new MySqlParameter[2];
                ////parameters[0] = new MySqlParameter("@memberProfileId", memberProfileId);
                ////parameters[1] = new MySqlParameter("@modulelist", spinpparam);
                ////string storeproc = "UPD_member_dashboard";
                ////DataSet ds = null;
                ////ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, storeproc, parameters);


                return Result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

        public static int DeletebyModuleName(DeleteByModuleName mod)
        {
            try
            {
                var typeID = new MySqlParameter("?typeID", string.IsNullOrEmpty(mod.typeID) ? "0" : mod.typeID);
                var type = new MySqlParameter("?type", string.IsNullOrEmpty(mod.type) ? " " : mod.type);
                var profileID = new MySqlParameter("?profileID", string.IsNullOrEmpty(mod.profileID) ? "0" : mod.profileID);
                int Result = _DbTouchbase.ExecuteStoreCommand("CALL USP_DeleteModuleName(?typeID,?type,?profileID)", typeID, type, profileID);
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }

        /// <summary>        
        /// Modified By:Pramod S
        /// Modified date: 26-08-2016
        /// Reason: Modified for Module label renaming purpose
        /// </summary>
        /// <param name="groupModule">Parsed JSON Data</param>
        /// <returns></returns>
        public static AddModuleResult AddSelectedModule(AddGroupModule groupModule)
        {
            try
            {
                MySqlParameter[] dataParameterList = new MySqlParameter[5];
                dataParameterList[0] = new MySqlParameter("?grpId", groupModule.grpId);

                //Format Used ModuleID:Old Module Name:New Module Name
                StringBuilder sbrModuleNames = new StringBuilder();

                ///Add default SubGroup
                sbrModuleNames.Append("5");
                sbrModuleNames.Append(":");
                sbrModuleNames.Append("SubGroup");
                sbrModuleNames.Append(":");
                sbrModuleNames.Append("SubGroup");

                sbrModuleNames.Append(",");

                ///Add default GroupInfo
                sbrModuleNames.Append("10");
                sbrModuleNames.Append(":");
                sbrModuleNames.Append("GroupInfo");
                sbrModuleNames.Append(":");
                sbrModuleNames.Append("GroupInfo");

                if (groupModule.moduleIDs != null)
                {
                    if (groupModule.moduleIDs.Count > 0)
                    {
                        int totalRecords = groupModule.moduleIDs.Count;
                        for (int counter = 0; counter < totalRecords; counter++)
                        {
                            sbrModuleNames.Append(",");

                            sbrModuleNames.Append(groupModule.moduleIDs[counter].moduleID);
                            sbrModuleNames.Append(":");
                            sbrModuleNames.Append(groupModule.moduleIDs[counter].oldName);
                            sbrModuleNames.Append(":");
                            sbrModuleNames.Append(groupModule.moduleIDs[counter].newName);
                        }
                    }
                }
                dataParameterList[1] = new MySqlParameter("?moduleIDs", sbrModuleNames.ToString());
                dataParameterList[2] = new MySqlParameter("?userID", groupModule.userID);
                dataParameterList[3] = new MySqlParameter("?memberCount", string.IsNullOrEmpty(groupModule.noOfmember) ? "0" : groupModule.noOfmember);
                dataParameterList[4] = new MySqlParameter("?Pwd", string.IsNullOrEmpty(groupModule.Pwd) ? "" : groupModule.Pwd);

                AddModuleResult Result = _DbTouchbase.ExecuteStoreQuery<AddModuleResult>("CALL V4_USPAddModules(?grpId,?moduleIDs,?userID,?memberCount,?Pwd)", dataParameterList).SingleOrDefault();
                //AddModuleResult Result = _DbTouchbase.ExecuteStoreQuery<AddModuleResult>("CALL V2_USPAddModules(?grpId,?moduleIDs,?userID,?memberCount,?Pwd)", grpId, moduleIDs, userID, memberCount, userpwd).SingleOrDefault();

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //_DbTouchbase.Dispose();
            }
        }

        public static ExternalLink GetExternalLink(GroupInfo grp)
        {
            try
            {
                MySqlParameter[] dataParameterList = new MySqlParameter[2];
                dataParameterList[0] = new MySqlParameter("?grpId", grp.grpID);
                dataParameterList[1] = new MySqlParameter("?moduleID", grp.moduleID);
                var result = _DbTouchbase.ExecuteStoreQuery<ExternalLink>("CALL V5_USPGetExternalLink(?grpId,?moduleID)", dataParameterList).SingleOrDefault();
                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                //  _DbTouchbase.Dispose();
            }

        }

        # endregion

        # region SubGroup Operation

        public static int createSubGroup(SubGroup subGroup)
        {
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[9];
                parameters[0] = new MySqlParameter("?title", subGroup.subGroupTitle);
                parameters[1] = new MySqlParameter("?createdBy", subGroup.memberProfileId);
                parameters[2] = new MySqlParameter("?groupId", subGroup.groupId);
                parameters[3] = new MySqlParameter("?memberProfileId", subGroup.MemberIDs);
                parameters[4] = new MySqlParameter("?intSubGroupMasterID", "0");
                parameters[4].Direction = ParameterDirection.InputOutput;
                parameters[5] = new MySqlParameter("?parentsubgrpID", subGroup.parentID);
                parameters[6] = new MySqlParameter("?DeleteMemIDs", "");
                parameters[7] = new MySqlParameter("?childSubGrpID", "");
                parameters[8] = new MySqlParameter("?AdminID", subGroup.memberProfileId);

                // var Result = _DbTouchbase.ExecuteStoreCommand("CALL USPAddSubGroup(?title,?groupId,?createdBy,?memberProfileId)", Title, GroupId, CreateBy, MemberProfileIds);
                //var Result = _DbTouchbase.ExecuteStoreCommand
                //    ("CALL V3_USPAddSubGroup(?title,?groupId,?createdBy,?memberProfileId,?parentsubgrpID,?DeleteMemIDs,?childSubGrpID,?AdminID,?intSubGroupMasterID)",
                //                               Title, GroupId, CreateBy,MemberProfileIds,parentID, deleteMem, childsubgrpId, admin,subgrpMasterID);
                string storeproc = "V3_USPAddSubGroup";
                var Result = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.StoredProcedure, storeproc, parameters);
                int intSubGroupMasterID = Convert.ToInt32(parameters[4].Value);

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

        public static List<SubGroupList> GetSubGroupDirectory(SubGroup sub)
        {
            string subGrpIDs = "";
            //SubGrpDirectoryResult directory = new SubGrpDirectoryResult();
            List<SubGroupList> subgrpList = new List<SubGroupList>();
            try
            {
                MySqlParameter[] param = new MySqlParameter[3];
                param[0] = new MySqlParameter("?grpId", sub.groupId);
                param[1] = new MySqlParameter("?profileID", sub.memberProfileId);
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
                    if (param[2].Value.ToString() == "1" && subGrpIDs != "")
                    {

                        if (sub.parentID == "0")
                        {
                            MySqlParameter[] param1 = new MySqlParameter[4];
                            param1[0] = new MySqlParameter("?grpId", sub.groupId);
                            param1[1] = new MySqlParameter("?profileID", sub.memberProfileId);
                            param1[2] = new MySqlParameter("?subGrpIds", subGrpIDs);
                            param1[3] = new MySqlParameter("?parentID", sub.parentID);
                            subgrpList = _DbTouchbase.ExecuteStoreQuery<SubGroupList>("V4_USPGetSubGrpList(?grpId,?profileID,?subGrpIds,?parentID)", param1).ToList();
                            // directory.subGrpList = subgrpList;

                        }

                        else
                        {
                            MySqlParameter[] param2 = new MySqlParameter[3];
                            param2[0] = new MySqlParameter("?grpId", sub.groupId);
                            param2[1] = new MySqlParameter("?profileID", sub.memberProfileId);
                            param2[2] = new MySqlParameter("?parentID", string.IsNullOrEmpty(sub.parentID) ? "0" : sub.parentID);
                            DataSet dsSubgrpList = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V4_USPGetSubGrpDetails", param2);
                            if (dsSubgrpList != null)
                            {
                                if (dsSubgrpList.Tables[0].Rows.Count > 0)
                                {
                                    subgrpList = GlobalFuns.DataTableToList<SubGroupList>(dsSubgrpList.Tables[0]);
                                    //directory.subGrpList = subgrpList;
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

                                    //   directory.memberList = memList;
                                }
                            }
                        }
                    }
                    //IF Member Is Not an Admin Then return Member Directory
                    //else if (param[2].Value.ToString() == "0" && subGrpIDs != "")
                    //{
                    //    MySqlParameter[] param3 = new MySqlParameter[3];
                    //    param3[0] = new MySqlParameter("?grpId", sub.groupId);
                    //    param3[1] = new MySqlParameter("?profileID", sub.memberProfileId);
                    //    param3[2] = new MySqlParameter("?subGrpIds", subGrpIDs);
                    //    var memberList = _DbTouchbase.ExecuteStoreQuery<MemberList>("CALL V4_USPGetSubGrpMemberList(?grpId,?profileID,?subGrpIds)", param3).ToList();
                    //    foreach (MemberList mem in memberList)
                    //    {
                    //        if (!string.IsNullOrEmpty(mem.pic))
                    //        {
                    //            string ImageName = mem.pic.ToString();
                    //            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/Group" + mem.grpID + "/";
                    //            mem.pic = path + ImageName;
                    //        }
                    //    }
                    //   // directory.memberList = memberList;
                    //}
                }
            }
            catch
            {
                throw;
            }

            //directory.subGrpList = new List<SubGroupList>();
            return subgrpList;
        }

        public static List<SubGroupDetail> getSubGroupDetails(SubGroupDtlSearch sub)
        {
            try
            {
                var grpId = new MySqlParameter("?grpId", sub.groupId);
                var subGrpId = new MySqlParameter("?subGrpId", sub.subgrpId);

                var Result = _DbTouchbase.ExecuteStoreQuery<SubGroupDetail>("CALL USPGetSubGroupDetailList(?grpId,?subGrpId)", grpId, subGrpId).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        # endregion

        # region Member Operation

        public static object deleteMember(string memberId, string groupId)
        {
            try
            {
                var MemID = new MySqlParameter("?memberId", memberId);
                var GrpId = new MySqlParameter("?grpId", groupId);

                var Result = _DbTouchbase.ExecuteStoreCommand("CALL USPDeleteGroupMember(?memberId,?grpId)", MemID, GrpId);

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet AddMemberToGroup(AddMember data)
        {
            //dynamic Result = 0;
            DataSet ds = new DataSet();

            try
            {
                //foreach (AddMember obj in data)
                //{
                //}
                if (Convert.ToInt32(data.groupId) > 0)
                {
                    //var membername = new MySqlParameter("?memberName", string.IsNullOrEmpty(data.userName) ? "" : data.userName);
                    //var membermobile = new MySqlParameter("?memberMobile", data.mobile);
                    //var groupMasterId = new MySqlParameter("?groupMasterId", data.groupId);
                    //var countryId = new MySqlParameter("?countryId", data.countryId);
                    //var memberEmail = new MySqlParameter("?memberEmail", data.memberEmail);
                    //var createdBy = new MySqlParameter("?createdBy", Convert.ToInt32(data.masterID));  
                    //Result = _DbTouchbase.ExecuteStoreQuery<int>("CALL USPAddMemberToGroup(?memberName,?memberMobile,?groupMasterId,?countryId,?memberEmail,?createdBy)", membername, membermobile, groupMasterId, countryId, memberEmail, createdBy).SingleOrDefault();

                    MySqlParameter[] parameters = new MySqlParameter[6];
                    parameters[0] = new MySqlParameter("?memberName", string.IsNullOrEmpty(data.userName) ? "" : data.userName);
                    parameters[1] = new MySqlParameter("?memberMobile", data.mobile);
                    parameters[2] = new MySqlParameter("?groupMasterId", data.groupId);
                    parameters[3] = new MySqlParameter("?countryId", data.countryId);
                    parameters[4] = new MySqlParameter("?memberEmail", string.IsNullOrEmpty(data.memberEmail) ? "" : data.memberEmail);
                    parameters[5] = new MySqlParameter("?createdBy", Convert.ToInt32(data.masterID));

                    string storeproc = "V1_USPAddMemberToGroup";
                    ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, storeproc, parameters);

                }
                else
                    throw new NoNullAllowedException();


                //return Convert.ToInt32(Result);
                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static void AddRemoveMember(string mobileNo, string groupId, int mode)
        {
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[3];
                parameters[0] = new MySqlParameter("?mobileNo", mobileNo);
                parameters[1] = new MySqlParameter("?groupId", groupId);
                parameters[2] = new MySqlParameter("?mode", mode);

                int i = MySqlHelper.ExecuteNonQuery(GlobalVar.chatAppConn, CommandType.StoredProcedure, "V3_AddUser", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet globalSearchGroup(clsGlobalSearchGroup sub)
        {
            try
            {
                MySqlParameter[] parameters = new MySqlParameter[2];
                parameters[0] = new MySqlParameter("?memID", sub.memId);
                parameters[1] = new MySqlParameter("?otherMemID", sub.otherMemId);

                string storeproc = "V1_USPGlobalSearchProfile";
                DataSet ds = null;
                ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, storeproc, parameters);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["ProfilePic_Path"].ToString()))
                    {
                        string imageName = ds.Tables[0].Rows[i]["ProfilePic_Path"].ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                        ds.Tables[0].Rows[i]["ProfilePic_Path"] = path + imageName;
                    }
                }

                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ds.Tables[1].Rows[i]["grpImg"].ToString()))
                    {
                        string imageName = ds.Tables[1].Rows[i]["grpImg"].ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Group/";
                        ds.Tables[1].Rows[i]["grpImg"] = path + imageName;
                    }
                }

                return ds;



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string DeletebyImage(DeleteImage mod)
        {
            try
            {
                var typeID = new MySqlParameter("?typeID", string.IsNullOrEmpty(mod.typeID) ? "0" : mod.typeID);
                var type = new MySqlParameter("?type", string.IsNullOrEmpty(mod.type) ? " " : mod.type);

                var Result = _DbTouchbase.ExecuteStoreQuery<string>("CALL V1_USP_DeleteImage(?typeID,?type)", typeID, type).SingleOrDefault();
                return Result.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string getMobilNo(string profileId, string groupId)
        {
            MySqlParameter[] parameters = new MySqlParameter[2];
            parameters[0] = new MySqlParameter("?profileId", profileId);
            parameters[1] = new MySqlParameter("?groupId", groupId);

            string storeproc = "SELECT member_mobile_no FROM member_master_profile where pk_member_master_profile_id=@profileId and fk_group_master_id=@groupId;";

            try
            {
                string MobileNo = (string)MySqlHelper.ExecuteScalar(GlobalVar.strAppConn, CommandType.Text, storeproc, parameters);
                return MobileNo;
            }
            catch
            { return ""; }
        }

        public static object removeSelfFromGroup(MemberProfile mem)
        {
            try
            {
                var MemID = new MySqlParameter("?memberId", mem.memberProfileId);
                var GrpId = new MySqlParameter("?grpId", mem.groupId);

                var Result = _DbTouchbase.ExecuteStoreCommand("CALL USPRemoveSelfFromGroup(?memberId,?grpId)", MemID, GrpId);

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet GetPwdDetails(string grpId, string mobileNo)
        {
            DataSet ds = new DataSet();

            MySqlParameter[] parameters = new MySqlParameter[2];
            parameters[0] = new MySqlParameter("?grpId", grpId);
            parameters[1] = new MySqlParameter("?mobileNo", mobileNo);

            string storeproc = "WEBPWDDetails";

            try
            {
                ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, storeproc, parameters);
            }
            catch
            {
            }
            return ds;
        }

        # endregion

        # region Notification

        public static void UpdateSMSCount(string GroupId)
        {
            string ssql = "update group_master set SMSCount=(SMSCount-1) where pk_group_master_id='" + GroupId + "'";

            try
            {
                MySqlHelper.ExecuteScalar(GlobalVar.strAppConn, CommandType.Text, ssql);
            }
            catch
            { }
        }

        public static void Send(string url)
        {
            //perform an async get on the url
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                ThreadPool.QueueUserWorkItem(
                    o =>
                    {
                        var response = request.GetResponse();
                        response.Close();
                    });
            }
            catch
            {
                //Errors.ErrorSignal.SignalError(ex);
            }
        }

        public static List<GrpNotificationCount> GetNotificationCount(string masterID)
        {
            List<GrpNotificationCount> grpNotList = new List<GrpNotificationCount>();

            try
            {
                MySqlParameter[] parameters = new MySqlParameter[1];
                parameters[0] = new MySqlParameter("?masterId", masterID);
                string storeproc = "V5_GetNotificationCount";
                DataSet ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, storeproc, parameters);

                if (ds != null)
                {
                    DataTable dtEvents = ds.Tables[1];
                    DataTable dtAnn = ds.Tables[2];
                    DataTable dtEbull = ds.Tables[3];
                    DataTable dtDocs = ds.Tables[4];
                    // DataTable dtImpr = ds.Tables[5];
                    if (ds.Tables[0].Rows.Count > 0) //loop through all groups
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            GrpNotificationCount grp = new GrpNotificationCount();
                            List<ModNotificationCount> modList = new List<ModNotificationCount>();
                            dynamic group = new ExpandoObject();
                            //group.id2 = 1;

                            grp.groupId = ds.Tables[0].Rows[i]["grpID"].ToString();
                            grp.groupCategory = ds.Tables[0].Rows[i]["grpCategory"].ToString();
                            int totalCount = 0;

                            //================================== Default Modules =========================================
                            //Event
                            for (int eve = 0; eve < dtEvents.Rows.Count; eve++)
                            {
                                if (dtEvents.Rows[eve]["grpID"].ToString() == ds.Tables[0].Rows[i]["grpID"].ToString())
                                {
                                    ModNotificationCount mod = new ModNotificationCount();
                                    mod.moduleId = dtEvents.Rows[eve]["moduleId"].ToString();
                                    mod.count = dtEvents.Rows[eve]["EveCount"].ToString();
                                    totalCount += Convert.ToInt32(mod.count);
                                    modList.Add(mod);
                                }
                            }

                            //Announcement
                            for (int ann = 0; ann < dtAnn.Rows.Count; ann++)
                            {
                                if (dtAnn.Rows[ann]["grpID"].ToString() == ds.Tables[0].Rows[i]["grpID"].ToString())
                                {
                                    ModNotificationCount mod = new ModNotificationCount();
                                    mod.moduleId = dtAnn.Rows[ann]["moduleId"].ToString();
                                    mod.count = dtAnn.Rows[ann]["AnnCount"].ToString();
                                    totalCount += Convert.ToInt32(mod.count);
                                    modList.Add(mod);
                                }
                            }

                            //Ebulletin
                            for (int news = 0; news < dtEbull.Rows.Count; news++)
                            {
                                if (dtEbull.Rows[news]["grpID"].ToString() == ds.Tables[0].Rows[i]["grpID"].ToString())
                                {
                                    ModNotificationCount mod = new ModNotificationCount();
                                    mod.moduleId = dtEbull.Rows[news]["moduleId"].ToString();
                                    mod.count = dtEbull.Rows[news]["ECount"].ToString();
                                    totalCount += Convert.ToInt32(mod.count);
                                    modList.Add(mod);
                                }
                            }

                            // Documents
                            for (int doc = 0; doc < dtDocs.Rows.Count; doc++)
                            {
                                if (dtDocs.Rows[doc]["grpID"].ToString() == ds.Tables[0].Rows[i]["grpID"].ToString())
                                {
                                    ModNotificationCount mod = new ModNotificationCount();
                                    mod.moduleId = dtDocs.Rows[doc]["moduleId"].ToString();
                                    mod.count = dtDocs.Rows[doc]["DocCount"].ToString();
                                    totalCount += Convert.ToInt32(mod.count);
                                    modList.Add(mod);
                                }
                            }

                            // Improvement
                            //for (int imp = 0; imp < dtImpr.Rows.Count; imp++)
                            //{
                            //    if (dtImpr.Rows[imp]["grpID"].ToString() == ds.Tables[0].Rows[i]["grpID"].ToString())
                            //    {
                            //        ModNotificationCount mod = new ModNotificationCount();
                            //        mod.moduleId = dtImpr.Rows[imp]["moduleId"].ToString();
                            //        mod.count = dtImpr.Rows[imp]["ImpCount"].ToString();
                            //        totalCount += Convert.ToInt32(mod.count);
                            //        modList.Add(mod);
                            //    }
                            //}

                            grp.ModCount = modList;
                            grp.totalCount = totalCount.ToString();
                            grpNotList.Add(grp);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return grpNotList;
        }

        #endregion

        # region Get App Version

        public static string GetCurrentVersion(string memberId)
        {
            try
            {
                string sql;
                sql = "select Device_name from main_member_master where pk_main_member_master_id=" + memberId;
                var result = MySqlHelper.ExecuteScalar(GlobalVar.strAppConn, CommandType.Text, sql);
                if (result != null)
                {
                    if (result.ToString() == "Android")
                    {
                        sql = "select Android from touchbase_versionmaster order by pk_Vid desc  limit 1";
                    }
                    else if (result.ToString() == "iOS")
                    {
                        sql = "select IOS from touchbase_versionmaster order by pk_Vid desc  limit 1";
                    }
                    var version = MySqlHelper.ExecuteScalar(GlobalVar.strAppConn, CommandType.Text, sql);
                    if (version != null)
                    {
                        return version.ToString();
                    }
                    else
                    {
                        return "0";
                    }
                }
                else
                    return "0";
            }
            catch
            {
                return "0";
            }
        }

        public static string GetCurrentVersionFamily(string memberId,string mobileNo)
        {
            try
            {
              string sql;

              sql = "select member_family_master.Device_name from member_family_master join member_master_profile on  member_family_master.fk_main_member_master_id=member_master_profile.pk_member_master_profile_id"+
                    " where member_master_profile.fk_main_member_master_id="+ memberId +" and member_family_master.member_contact_no=" + mobileNo;

                var result = MySqlHelper.ExecuteScalar(GlobalVar.strAppConn, CommandType.Text, sql);
                if (result != null)
                {
                    if (result.ToString() == "Android")
                    {
                        sql = "select Android from touchbase_versionmaster order by pk_Vid desc  limit 1";
                    }
                    else if (result.ToString() == "iOS")
                    {
                        sql = "select IOS from touchbase_versionmaster INTO ios order by pk_Vid desc  limit 1";
                    }
                    var version = MySqlHelper.ExecuteScalar(GlobalVar.strAppConn, CommandType.Text, sql);
                    if (version != null)
                    {
                        return version.ToString();
                    }
                    else
                    {
                        return "0";
                    }
                }
                else
                    return "0";

            }
            catch
            {
                return "0";
            }
        }
        # endregion

        # region suggession

        public static List<suggestFeatureResult> AddSuggestedFeature(suggestFeature suggestion)
        {
            try
            {
                var Title = new MySqlParameter("?title", string.IsNullOrEmpty(suggestion.title) ? "" : suggestion.title);
                var Description = new MySqlParameter("?Description", string.IsNullOrEmpty(suggestion.description) ? " " : suggestion.description);
                var GrpID = new MySqlParameter("?grpID", string.IsNullOrEmpty(suggestion.grpId) ? " " : suggestion.grpId);
                var ProfileID = new MySqlParameter("?profileID", string.IsNullOrEmpty(suggestion.profileID) ? " " : suggestion.profileID);

                var Result = _DbTouchbase.ExecuteStoreQuery<suggestFeatureResult>("CALL V1_USPAddSuggestFeature(?title,?Description,?grpID,?profileID)", Title, Description, GrpID, ProfileID).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        # endregion

        # region Country

        public static List<CountryResult> getAllCountriesLists()
        {
            try
            {
                var Result = _DbTouchbase.ExecuteStoreQuery<CountryResult>("CALL USPGetAllCountryList()").ToList();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To get Country Id from given Country Code
        /// Author : Nandlishor K
        /// created on : 09/08/2016
        /// </summary>
        public static string getCountryId(string CountryCode)
        {
            string ssql = "";
            ssql = "select country_master.country_master_id from country_master where country_code=" + CountryCode;
            try
            {
                return Convert.ToString(MySqlHelper.ExecuteScalar(GlobalVar.strAppConn, CommandType.Text, ssql));
            }
            catch
            {
                return "";
            }
            finally { }
        }

        public static DataSet CountryCode(string country_id, string mobileNo, string groupId)
        {
            DataSet ds = new DataSet();

            //string ssql="SELECT country_code FROM country_master where country_master_id=" + country_id ;
            string ssql = "select (SELECT country_code FROM country_master where country_master_id=" + country_id + ") as CountryCode,case when count((select IMEI_No from main_member_master where fk_country_id='" + country_id + "' and member_mobile='" + mobileNo + "')) > 0 then 'No' else 'Yes' end as 'sentSMS', case when (select SMSCount from group_master where pk_group_master_id='" + groupId + "') > 0 then 'Yes' else 'No' end as 'isSentSMS';";

            try
            {
                ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.Text, ssql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        # endregion

        #region Club Details
        /// <summary>
        /// Created by: Nandu
        /// Created on: 23-05-2017
        /// Task: get Club Info
        /// </summary>
        public static ClsGetClubDetailsOutput getClubDetail(GroupInfo grp)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];
                param[0] = new MySqlParameter("?grpId", grp.grpID);

                var Result = _DbTouchbase.ExecuteStoreQuery<ClsGetClubDetailsOutput>("SELECT group_name as ClubName, coalesce(club_id,'') as ClubID, coalesce(address1,'') as MeetingPlace, coalesce(club_meeting_day,'') as MeetingDay,coalesce(club_meeting_from_time,'') as MeetingTime,district_master.district_number as DistrictNumber FROM group_master join district_master on group_master.fk_district_master_id = district_master.pk_district_master_id where group_master.pk_group_master_id= {0}", grp.grpID).FirstOrDefault();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Rotary Library

        /// <summary>
        /// Created By: Nandu
        /// Created On: 16-05-2017
        /// Task: Get rotary library data 
        /// </summary>
        /// <returns></returns>
        public static List<clsGetRotaryLibraryOutput> GetRotaryLibraryList()
        {
            try
            {
                var Result = _DbTouchbase.ExecuteStoreQuery<clsGetRotaryLibraryOutput>("CALL V7_USPGetRotaryLibraryData()").ToList();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Feedback

        public static DataSet getClubFeedbackDetail(FeedbackInput obj)
        {
            DataSet ds = new DataSet();

            try
            {
                MySqlParameter[] param = new MySqlParameter[1];
                param[0] = new MySqlParameter("?profileId", obj.ProfileId);

                string query = "SELECT member_name as Name, member_mobile_no as MobileNo, member_email_id as Email, group_name as ClubName, district_master.district_number as Dictrict FROM member_master_profile join group_master on group_master.pk_group_master_id = member_master_profile.fk_group_master_id join district_master on group_master.fk_district_master_id = district_master.pk_district_master_id where pk_member_master_profile_id = @profileId;";

                ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.Text, query, param);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        #endregion

        #region Commented

        //public static List<GroupResult> getBusinessGroupLists(string memberId)
        //{
        //    try
        //    {
        //        var MemberId = new MySqlParameter("?memberId", memberId);
        //        var Result = _DbTouchbase.ExecuteStoreQuery<GroupResult>("CALL USPGetBusinessGroupList(?memberId)", MemberId).ToList();

        //        return Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public static List<GroupResult> getPersonalGroupLists(string memberId)
        //{
        //    try
        //    {
        //        var MemberId = new MySqlParameter("?memberId", memberId);
        //        var Result = _DbTouchbase.ExecuteStoreQuery<GroupResult>("CALL USPGetPersonalGroupList(?memberId)", MemberId).ToList();

        //        return Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public static List<GroupResult> getSocialGroupLists(string memberId)
        //{
        //    try
        //    {
        //        var MemberId = new MySqlParameter("?memberId", memberId);
        //        var Result = _DbTouchbase.ExecuteStoreQuery<GroupResult>("CALL USPGetSocialGroupList(?memberId)", MemberId).ToList();

        //        return Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public static List<SubGroupList> getSubGroupList(SubGroup sub)
        {
            try
            {
                var grpId = new MySqlParameter("?grpId", sub.groupId);
                //var profileID = new MySqlParameter("?profileID", sub.memberProfileId);
                //var subGroupId = new MySqlParameter("?subGroupId", string.IsNullOrEmpty(sub.subGroupID) ? "0" : sub.subGroupID);
                //var Result = _DbTouchbase.ExecuteStoreQuery<SubGroupList>("CALL V3_USPGetSubGroupList(?grpId,?profileID,?subGroupId)", grpId, profileID, subGroupId).ToList();
                var Result = _DbTouchbase.ExecuteStoreQuery<SubGroupList>("CALL USPGetSubGroupList(?grpId)", grpId).ToList();

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}