﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TouchBaseWebAPI.Data;
using TouchBaseWebAPI.BusinessEntities;
using TouchBaseWebAPI.Models;
using System.Data;
using System.Configuration;
using System.Web;
using System.IO;
using System.Text;
//using jabber.client;
//using jabber;

namespace TouchBaseWebAPI.Controllers
{
    public class GroupController : ApiController
    {
        private TouchBaseWebAPI.Data.row_productionEntities _DbTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        # region Group
        //================== Add Group =============================//
        [System.Web.Http.HttpPost]
        public object CreateGroup(clsgroup_master group)
        {
            dynamic CreateGRpResult;
            int str = -1;
            try
            {
                group.userpwd = GlobalFuns.CreateRandomPassword(8);
                group.userpwd = GlobalFuns.EncryptPassward(group.userpwd);

                #region Added By Nandu on 21/09/2016 --Task--> disable create entity functionalty from app for while

                if (group.grpId != "0")
                {
                    group = GroupMaster.CreateGroup(group);

                    if (group != null)
                    {
                        if (group.grpImageID != "0")
                        {
                            str = GlobalFuns.UploadImage(group.grpImageName, Convert.ToInt32(group.grpId), "Group");
                        }
                        else
                            str = 0;
                    }

                    if (Convert.ToInt32(group.grpId) > 0 && str == 0)
                    {
                        CreateGRpResult = new { status = "0", message = "success", grdId = group.grpId.ToString() };

                        if (group.grpId != "0")
                        {
                            string url = ConfigurationManager.AppSettings["imgPath"] + "php/EditGroup.php?GrpID=" + group.grpId;
                            GroupMaster.Send(url);
                        }
                    }
                    else
                    {
                        CreateGRpResult = new { status = "1", message = "failed", grdId = "0" };
                    }
                }
                else
                {
                    GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), ConfigurationManager.AppSettings["frommail"].ToString(), "New Entity Created on TouchBase on " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), groupmail(group.grpName, group.grpCategory, group.country, group.mobile, group.emailid, group.addrss1, group.userId));
                    CreateGRpResult = new { status = "1", message = "failed", grdId = "0" };
                }

                #endregion

            }
            catch
            {
                CreateGRpResult = new { status = "1", message = "failed", grdId = "0" };
            }

            return new { CreateGRpResult };
        }

        //================== Get all Groups List =============================//
        [System.Web.Http.HttpPost]
        public object GetAllGroupsList(MemberLogin mem)
        {
            dynamic TBGroupResult;
            List<object> AllGroupListResults = new List<object>();
            if (string.IsNullOrEmpty(mem.imeiNo))
            {
                mem.imeiNo = "0";
            }
            try
            {
                List<GroupListResult> Result1 = GroupMaster.getAllGroupLists(mem.masterUID, mem.imeiNo, mem.updatedOn);
                var Version = GroupMaster.GetCurrentVersion(mem.masterUID);


                if (Result1 == null)
                {
                    TBGroupResult = new { status = "2", message = "Your session has been expired." };
                }
                else if (Result1.Count == 0)
                {
                    TBGroupResult = new { status = "0", message = "Record not found" };
                }
                else
                {
                    for (int i = 0; i < Result1.Count; i++)
                    {

                        AllGroupListResults.Add(new { GroupResult = Result1[i] });
                    }

                    #region
                    //List<object> PersonalGroupListResults = new List<object>();
                    //List<object> SocialGroupListResults = new List<object>();
                    //List<object> BusinessGroupListResults = new List<object>();

                    //List<GroupResult> Result2 = GroupMaster.getPersonalGroupLists(mem.masterUID);
                    //for (int i = 0; i < Result2.Count; i++)
                    //{
                    //    PersonalGroupListResults.Add(new { GroupResult = Result2[i] });
                    //}

                    //List<GroupResult> Result3 = GroupMaster.getSocialGroupLists(mem.masterUID);
                    //for (int i = 0; i < Result3.Count; i++)
                    //{
                    //    SocialGroupListResults.Add(new { GroupResult = Result3[i] });
                    //}

                    //List<GroupResult> Result4 = GroupMaster.getBusinessGroupLists(mem.masterUID);
                    //for (int i = 0; i < Result4.Count; i++)
                    //{
                    //    BusinessGroupListResults.Add(new { GroupResult = Result4[i] });
                    //}

                    //, PersonalGroupListResults, SocialGroupListResults, BusinessGroupListResults 
                    #endregion

                    if (AllGroupListResults != null)
                    {
                        TBGroupResult = new { status = "0", version = Version, curDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), message = "success", AllGroupListResults };
                    }
                    else
                    {
                        TBGroupResult = new { status = "0", message = "Record not found" };
                    }
                }
            }
            catch
            {
                TBGroupResult = new { status = "1", message = "failed" };
            }

            return new { TBGroupResult };
        }

        //================== Remove Self from Group =============================//
        [System.Web.Http.HttpPost]
        public object RemoveSelfFromGroup(MemberProfile memPro)
        {
            dynamic TBRemoveSelfResult;

            try
            {
                var Result = GroupMaster.removeSelfFromGroup(memPro);

                if (Result != null)
                {
                    //#region Unregister user from Chat

                    ////Delete user from chat database
                    //string mobile = GroupMaster.getMobilNo(memPro.memberProfileId, memPro.groupId);
                    //string username = memPro.groupId + "_" + mobile;

                    ////GroupMaster.AddRemoveMember(mobile, memPro.groupId, 2);//2 is for delete mode  

                    ////string host = "version.touchbase.in";
                    ////string cmd = "ejabberdctl unregister " + '"' + username + '"' + " " + '"' + host + '"' + "";
                    ////int i = GlobalFuns.ExecuteCommandSync(cmd, 2);

                    //GlobalFuns.ExecuteCommandChat(username, 1);

                    //#endregion

                    TBRemoveSelfResult = new { status = "0", message = "success" };
                }
                else
                {
                    TBRemoveSelfResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBRemoveSelfResult = new { status = "1", message = "failed" };
            }

            return new { TBRemoveSelfResult };
        }

        //================== Group Info for Edit Group =============================//
        [System.Web.Http.HttpPost]
        public object GetGroupInfo(GroupInfo grp)
        {
            dynamic TBGetGroupResult;
            List<object> GetGroupInfo = new List<object>();

            try
            {
                List<getGroupInfo> Result = GroupMaster.getGroupDetail(grp);

                for (int i = 0; i < Result.Count; i++)
                {
                    GetGroupInfo.Add(new { GetGroupInfo = Result[i] });
                }

                if (Result != null)
                {
                    TBGetGroupResult = new { status = "0", message = "success", getGroupDetailResult = GetGroupInfo };
                }
                else
                {
                    TBGetGroupResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetGroupResult = new { status = "1", message = "failed" };
            }

            return new { TBGetGroupResult };
        }

        //================== Delete Entity =============================//
        [System.Web.Http.HttpPost]
        public object DeleteEntity(MemberProfile memPro)
        {
            dynamic TBDeleteEntityResult;

            try
            {
                var Result = GroupMaster.deleteEntity(memPro.memberProfileId, memPro.groupId);

                if (Result.ToString() != "-1")
                {
                    TBDeleteEntityResult = new { status = "0", message = "success" };
                }
                else
                {
                    TBDeleteEntityResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBDeleteEntityResult = new { status = "1", message = "failed" };
            }

            return new { TBDeleteEntityResult };
        }

        //================== Group Detail =============================//
        [System.Web.Http.HttpPost]
        public object GetGroupDetail(MemberProfile mem)
        {
            dynamic TBGetGroupResult;
            List<object> GetGroupInfo = new List<object>();

            try
            {
                List<GetGroupInfo> Result = GroupMaster.getGroupInfo(mem);

                for (int i = 0; i < Result.Count; i++)
                {
                    GetGroupInfo.Add(new { GetGroupInfo = Result[i] });
                }

                if (Result != null)
                {
                    TBGetGroupResult = new { status = "0", message = "success", getGroupDetailResult = GetGroupInfo };
                }
                else
                {
                    TBGetGroupResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetGroupResult = new { status = "1", message = "failed" };
            }

            return new { TBGetGroupResult };
        }


        //================== Version 2 Entity Info =============================//
        [System.Web.Http.HttpPost]
        public object GetEntityInfo(GroupInfo group)
        {
            dynamic TBEntityInfoResult;
            List<object> EntityInfoResult = new List<object>();
            List<object> AdminInfoResult = new List<object>();

            DataSet Result = new DataSet();

            try
            {
                Result = GroupMaster.GetEntityInfo(group);

                DataTable dt = Result.Tables[0];
                DataTable dt1 = Result.Tables[1];
                DataTable dt2 = Result.Tables[2];

                List<EntityInfo> res = new List<EntityInfo>();
                List<AdminInfo> adm = new List<AdminInfo>();

                if (dt.Rows.Count > 0)
                {
                    res = GlobalFuns.DataTableToList<EntityInfo>(dt);
                }

                // Admin Info
                if (dt2.Rows.Count > 0)
                {
                    adm = GlobalFuns.DataTableToList<AdminInfo>(dt2);
                }

                for (int i = 0; i < res.Count; i++)
                {
                    EntityInfoResult.Add(new { EntityInfo = res[i] });
                }

                for (int i = 0; i < adm.Count; i++)
                {
                    AdminInfoResult.Add(new { AdminInfo = adm[i] });
                }

                string path = "";

                if (!string.IsNullOrEmpty(dt1.Rows[0]["group_image"].ToString()))
                {
                    path = ConfigurationManager.AppSettings["imgPath"] + "Documents/Group/";
                    path += dt1.Rows[0]["group_image"].ToString();
                }

                TBEntityInfoResult = new { status = "0", message = "success", groupName = dt1.Rows[0]["group_name"].ToString(), groupImg = path, contactNo = dt1.Rows[0]["contact_no"].ToString(), address = dt1.Rows[0]["address1"].ToString(), email = dt1.Rows[0]["group_email"].ToString(), EntityInfoResult = EntityInfoResult, AdminInfoResult = AdminInfoResult };
            }
            catch
            {
                TBEntityInfoResult = new { status = "1", message = "failed", smscount = 0 };
            }

            return new { TBEntityInfoResult };
        }

        [System.Web.Http.HttpPost]
        public object GetClubHistory(GroupInfo group)
        {
            dynamic TBClubHistoryResult;
            try
            {
                ClubHistory clubHistory = GroupMaster.GetClubHistory(group);

                if (clubHistory != null)
                {
                    TBClubHistoryResult = new { status = "0", message = "success", curDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), clubHistory, };
                }
                else
                {
                    TBClubHistoryResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBClubHistoryResult = new { status = "1", message = "failed", };
            }
            return new { TBClubHistoryResult };
        }

        # endregion

        #region Club Details

        //================== Get Club Info =============================//
        /// <summary>
        /// Created by: Nandu
        /// Created on: 23-05-2017
        /// Task: get Club Info
        /// </summary>
        [System.Web.Http.HttpPost]
        public object GetClubDetails(GroupInfo grp)
        {
            dynamic TBGetGroupResult;

            try
            {
                ClsGetClubDetailsOutput Result = GroupMaster.getClubDetail(grp);

                if (Result != null)
                {
                    TBGetGroupResult = new { status = "0", message = "success", getGroupDetailResult = Result };
                }
                else
                {
                    TBGetGroupResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetGroupResult = new { status = "1", message = "failed" };
            }

            return new { TBGetGroupResult };
        }

        #endregion

        # region SyncAPI

        //================== Sync all Groups List for offline data =============================//
        [System.Web.Http.HttpPost]
        public object GetAllGroupListSync(MemberLogin mem)
        {
            dynamic TBGroupResult;
            try
            {
                mem.loginType = string.IsNullOrEmpty(mem.loginType) ? "0" : mem.loginType;
                string sql;
                
                if (mem.loginType == "0")
                {
                    sql = "Select m.IMEI_No FROM main_member_master m where m.pk_main_member_master_id=" + mem.masterUID;
                }
                else
                {
                    sql = " Select  IMEI_No FROM member_family_master  join member_master_profile "+
                          " On member_family_master.fk_main_member_master_id=member_master_profile.pk_member_master_profile_id "+
                          " where member_contact_no= "+ mem.mobileNo +" and member_family_master.fk_country_id="+ mem.countryCode +
                          " and member_master_profile.fk_main_member_master_id=" + mem.masterUID;
                }

                string result = Convert.ToString(MySqlHelper.ExecuteScalar(GlobalVar.strAppConn, CommandType.Text, sql));

                if (result == mem.imeiNo)
                {
                    GroupModulesSyncResult Result = GroupMaster.GetAllGroupListsSync(mem.masterUID, mem.updatedOn);
                    if (Result == null)
                    {
                        TBGroupResult = new { status = "0", message = "Record not found", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
                    }
                    else
                    {
                        TBGroupResult = new { status = "0", message = "Success", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Result };
                    }
                }
                else
                {
                    TBGroupResult = new { status = "2", message = "Your session has been expired." };
                }
            }
            catch
            {
                TBGroupResult = new { status = "1", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), message = "failed" };
            }
            return TBGroupResult;
        }

        #endregion

        #region Module

        //================== Get modules of Particular Group =============================//
        [System.Web.Http.HttpPost]
        public object GetGroupModulesList(MemberProfile mem)
        {
            dynamic TBGetGroupModuleResult;
            List<object> GroupListResult = new List<object>();

            try
            {
                List<GroupModulesList> Result = GroupMaster.getGroupModuleLists(mem.groupId, mem.memberProfileId);

                for (int i = 0; i < Result.Count; i++)
                {
                    GroupListResult.Add(new { GroupList = Result[i] });
                }

                if (Result != null)
                {
                    TBGetGroupModuleResult = new { status = "0", message = "success", GroupListResult = GroupListResult };
                }
                else
                {
                    TBGetGroupModuleResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetGroupModuleResult = new { status = "1", message = "failed" };
            }

            return new { TBGetGroupModuleResult };
        }


        [System.Web.Http.HttpPost]
        public object AddSelectedModule(AddGroupModule module)
        {
            dynamic TBAddModuleResult;
            string imagePath = "";
            // List<> ModulesLsitResult 
            string[] ModulesLsitResult = new string[0];
            try
            {
                AddModuleResult result = GroupMaster.AddSelectedModule(module);
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(result.grpImage))
                    {
                        imagePath = ConfigurationManager.AppSettings["imgPath"] + "Documents/Group/" + result.grpImage;
                    }

                    #region add user to chat database

                    //if (module.moduleIDs.Contains("11")) // If user select CHAT module then only register user in chat database
                    //{
                    //    //GroupMaster.AddRemoveMember(members.mobile, members.groupId, 1);//1 is for insert mode                                      

                    //    string username = module.grpId + "_" + result.mobileno;

                    //    //string host = "version.touchbase.in";
                    //    ////ejabberdctl register "testuser" "localhost" "passw0rd"  ----------- command format
                    //    //string cmd = "ejabberdctl register " + '"' + username + '"' + " " + '"' + host + '"' + " " + '"' + username + '"' + "";
                    //    //int i = GlobalFuns.ExecuteCommandSync(cmd, 1);

                    //    GlobalFuns.ExecuteCommandChat(username, 0);
                    //}

                    #endregion

                    senMailSMSToAdmin(module.grpId.ToString(), result.mobileno);
                    TBAddModuleResult = new { status = "0", message = "success", grpID = result.grpId.ToString(), grpname = result.grpName, grpImg = imagePath, trialMsg = "You have Successfully Activated a FULL FEATURED 30 day FREE trial.Please Note after 30 day period, " + result.grpName + " would be automatically Deactivated.For uninterrupted use: SUBSCRIBE NOW", ModulesLsitResult = ModulesLsitResult };
                }
                else
                {
                    TBAddModuleResult = new { status = "1", message = "failed", ModulesLsitResult = ModulesLsitResult };
                }
            }
            catch
            {
                TBAddModuleResult = new { status = "1", message = "failed", ModulesLsitResult = ModulesLsitResult };
            }

            return new { TBAddModuleResult };
        }

        public object GetReplicaInfo(string LastModuleID)
        {
            dynamic ModuleResult;
            try
            {
                List<ModuleMaster> Result = GroupMaster.GetReplicaInfo(LastModuleID);
                if (Result != null)
                {
                    ModuleResult = new { status = "0", message = "success", ModuleList = Result };
                }
                else
                {
                    ModuleResult = new { status = "0", message = "Record not found" };
                }
            }
            catch (Exception ex)
            {
                ModuleResult = new { status = "1", message = ex.Message };
            }
            return new { ModuleResult };
        }

        //====================== Get all modules list =============================//
        [System.Web.Http.HttpPost]
        public object GetModulesList()
        {
            //dynamic TBGetModuleResult;

            //try
            //{
            //    var Result = GroupMaster.getModuleLists();

            //    if (Result != null)
            //    {
            //        TBGetModuleResult = new { status = "0", message = "success", EventsListResult = Result };
            //    }
            //    else
            //    {
            //        TBGetModuleResult = new { status = "0", message = "Record not found" };
            //    }                
            //}
            //catch
            //{
            //    TBGetModuleResult = new { status = "1", message = "failed" };
            //}

            //return new { TBGetModuleResult };

            dynamic TBGetGroupModuleResult;
            List<object> GroupListResult = new List<object>();

            try
            {
                List<ModulesList> Result = GroupMaster.getModuleLists();

                foreach (var item in Result)
                {

                }


                for (int i = 0; i < Result.Count; i++)
                {
                    GroupListResult.Add(new { GroupList = Result[i] });
                }

                if (Result != null)
                {
                    TBGetGroupModuleResult = new { status = "0", message = "success", GroupListResult = GroupListResult };
                }
                else
                {
                    TBGetGroupModuleResult = new { status = "0", message = "Record not found" };
                }
            }
            catch (Exception ex)
            {
                TBGetGroupModuleResult = new { status = "1", message = "failed", error = ex.Message };
            }

            return new { TBGetGroupModuleResult };
        }


        //Service Related To Module Dashboard
        [System.Web.Http.HttpPost]
        public object UpdateModuleDashboard(MemberProfile mempro)
        {
            dynamic TBUpdateGroupModulesResult;
            List<object> GroupListResult = new List<object>();

            try
            {
                var Result = GroupMaster.UpdateModuleDashboard(mempro.memberProfileId, mempro.modulelist);

                if (Result != null)
                {
                    TBUpdateGroupModulesResult = new { status = "0", message = "success", };
                }
                else
                {
                    TBUpdateGroupModulesResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBUpdateGroupModulesResult = new { status = "1", message = "failed" };
            }

            return new { TBUpdateGroupModulesResult };
        }

        //================== Common web service for Delete =============================//
        [HttpPost]
        public object DeleteByModuleName(DeleteByModuleName mod)
        {
            dynamic DeleteResult;
            try
            {
                if (mod.typeID == "0" || string.IsNullOrEmpty(mod.type))
                {
                    throw new InvalidOperationException();
                }

                var result = GroupMaster.DeletebyModuleName(mod);

                if (result >= 0)
                {                    
                    #region Commented by Nandu on 29/06/2017

                    //DataSet ds = GroupMaster.GetGroupDtls(mod.typeID, mod.profileID);
                    ////string countryCode = GroupMaster.CountryCode(ds.Tables[0].Rows[0]["fk_country_ID"].ToString(),);

                    //if (mod.type == "Member" && mod.typeID != mod.profileID)
                    //{
                    //    //send SMS to user  
                    //    DataSet dsCountry = new DataSet();
                    //    dsCountry = GroupMaster.CountryCode(ds.Tables[0].Rows[0]["fk_country_ID"].ToString(), ds.Tables[0].Rows[0]["member_mobile_no"].ToString(), ds.Tables[0].Rows[0]["grpID"].ToString());

                    //    //if (dsCountry.Tables[0].Rows[0]["sentSMS"].ToString() == "Yes")
                    //    //{ }

                    //    if (dsCountry.Tables[0].Rows[0]["isSentSMS"].ToString() == "Yes")
                    //    {
                    //        if (dsCountry.Tables[0].Rows[0]["CountryCode"].ToString() == "+91")//for India
                    //        {
                    //            if ((GlobalFuns.SendSMSOnAdd(ds.Tables[0].Rows[0]["member_mobile_no"].ToString(), "You Have been removed from " + ds.Tables[0].Rows[0]["group_name"].ToString() + " by " + ds.Tables[0].Rows[0]["admin"].ToString() + ".")) == true)
                    //            {
                    //                GroupMaster.UpdateSMSCount(ds.Tables[0].Rows[0]["grpID"].ToString());
                    //            }
                    //        }
                    //        else//for International SMS
                    //        {
                    //            if ((GlobalFuns.SendSMSInterOnAdd(ds.Tables[0].Rows[0]["member_mobile_no"].ToString(), "You Have been removed from " + ds.Tables[0].Rows[0]["group_name"].ToString() + " by " + ds.Tables[0].Rows[0]["admin"].ToString() + ".", dsCountry.Tables[0].Rows[0]["CountryCode"].ToString())) == true)
                    //            {
                    //                GroupMaster.UpdateSMSCount(ds.Tables[0].Rows[0]["grpID"].ToString());
                    //            }
                    //        }
                    //    }

                    //    if (ds.Tables[0].Rows[0]["member_email_id"].ToString() != "")
                    //    {
                    //        GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), ds.Tables[0].Rows[0]["member_email_id"].ToString(), "You have been removed from Entity " + ds.Tables[0].Rows[0]["group_name"].ToString() + " on Touchbase.", msgbody(ds.Tables[0].Rows[0]["group_name"].ToString()));
                    //        //GlobalFuns.SendElasticEmail(ds.Tables[0].Rows[0]["member_email_id"].ToString(), "You have been removed from " + ds.Tables[0].Rows[0]["group_name"].ToString() + " on Touchbase.", "", msgbody(ds.Tables[0].Rows[0]["group_name"].ToString()), ConfigurationManager.AppSettings["frommail"].ToString(), "kaizeninfotech.com");
                    //    }
                    //    var MID = mod.typeID;
                    //    var grpID = ds.Tables[0].Rows[0]["grpID"].ToString();

                    //    string url = ConfigurationManager.AppSettings["imgPath"] + "php/RemoveMember.php?MID=" + MID + " &GrpID=" + grpID;
                    //    GroupMaster.Send(url);
                    //}

                    #endregion                    

                    DeleteResult = new { status = "0", message = "success" };
                }
                else
                {
                    DeleteResult = new { status = "1", message = "failed" };
                }
            }
            catch
            {
                DeleteResult = new { status = "1", message = "failed" };
            }
            return new { DeleteResult };
        }

        [HttpPost]
        public object GetExternalLink(GroupInfo grp)
        {
            dynamic TBGetLinkResult;
            try
            {
                ExternalLink link = GroupMaster.GetExternalLink(grp);

                if (link != null)
                {
                    TBGetLinkResult = new { status = "0", message = "success", link };
                }
                else
                {
                    TBGetLinkResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetLinkResult = new { status = "1", message = "failed", };
            }
            return new { TBGetLinkResult };
        }

        #endregion

        #region Touchbase User

        //================== Update Member Group Category(personal,Social,Business etc) =============================//
        /// <summary>
        /// Updates Group Category. then calls GetAllGroupListSync
        /// it gives reponse of GetAllGroupListSync WebAPI
        /// Modified by Rupali at 02/08/2016 for offline Mode
        /// </summary>
        /// <param name="memPro"></param>
        /// <returns></returns>

        [System.Web.Http.HttpPost]
        public object UpdateMemberGroupCategory(MemberProfile memPro)
        {
            dynamic TBGroupResult;
            List<object> AllGroupListResults = new List<object>();

            try
            {
                List<GroupResult> ResultCat = GroupMaster.getUpdatedGroupLists(memPro.memberProfileId, memPro.mycategory, memPro.memberMainId);
                for (int i = 0; i < ResultCat.Count; i++)
                {
                    AllGroupListResults.Add(new { GroupResult = ResultCat[i] });
                }

                if (AllGroupListResults != null)
                {
                    GroupModulesSyncResult Result = GroupMaster.GetAllGroupListsSync(memPro.memberMainId, memPro.updatedOn);
                    //TBGroupResult = new { status = "0", message = "success", AllGroupListResults };
                    if (Result == null)
                    {
                        TBGroupResult = new { status = "0", message = "Record not found", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
                    }
                    else
                    {
                        TBGroupResult = new { status = "0", message = "Success", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Result };
                    }

                }
                else
                {
                    TBGroupResult = new { status = "0", message = "Record not found", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
                }
            }
            catch
            {
                TBGroupResult = new { status = "1", message = "failed", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
            }

            return TBGroupResult;
        }

        //================== Remove Member Group Category(personal,Social,Business etc) =============================//
        /// <summary>
        ///    /// <summary>
        /// Remove Group Category. then calls GetAllGroupListSync
        /// It gives reponse of GetAllGroupListSync WebAPI 
        /// Modified by Rupali at 02/08/2016 for offline Mode
        /// </summary>
        /// <param name="memPro"></param>
        /// <returns></returns>
        /// </summary>
        /// <param name="memPro"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public object RemoveGroupCategory(MemberProfile memPro)
        {
            dynamic TBGroupResult;
            List<object> AllGroupListResults = new List<object>();

            try
            {
                List<GroupResult> Resultgrp = GroupMaster.getRemoveGroupCategory(memPro.memberProfileId, memPro.memberMainId);
                for (int i = 0; i < Resultgrp.Count; i++)
                {
                    AllGroupListResults.Add(new { GroupResult = Resultgrp[i] });
                }

                if (AllGroupListResults != null)
                {
                    //TBGroupResult = new { status = "0", message = "success", AllGroupListResults };
                    GroupModulesSyncResult Result = GroupMaster.GetAllGroupListsSync(memPro.memberMainId, memPro.updatedOn);

                    if (Result == null)
                    {
                        TBGroupResult = new { status = "0", message = "Record not found", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
                    }
                    else
                    {
                        TBGroupResult = new { status = "0", message = "Success", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Result };
                    }

                }
                else
                {
                    TBGroupResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGroupResult = new { status = "1", message = "failed" };
            }

            return TBGroupResult;
        }

        #endregion

        # region Email

        private string groupmail(string grpName, string grpCategory, string country, string mobile, string emailId, string address, string userId)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table width='50%' border='0' cellpadding='8' cellspacing='0' style='font-family:Verdana, Geneva, sans-serif; font-size:12px; border:1px solid #ccc;'><tr>");
            sb.Append("<td width='210' style='border-bottom:1px solid #ccc;border-right:1px solid #ccc;'><strong>Entity Name</strong></td>");
            sb.Append("<td width='334' style='border-bottom:1px solid #ccc;'>" + grpName + "</td></tr><tr>");
            sb.Append("<td width='210' style='border-bottom:1px solid #ccc;border-right:1px solid #ccc;'><strong>Group Category</strong></td>");
            sb.Append("<td width='334' style='border-bottom:1px solid #ccc;'>" + grpCategory + "</td></tr><tr>");
            sb.Append("<td width='210' style='border-bottom:1px solid #ccc;border-right:1px solid #ccc;'><strong>Country</strong></td>");
            sb.Append("<td width='334' style='border-bottom:1px solid #ccc;'>" + country + "</td></tr><tr>");
            sb.Append("<td width='210' style='border-bottom:1px solid #ccc;border-right:1px solid #ccc;'><strong>Mobile No</strong></td>");
            sb.Append("<td width='334' style='border-bottom:1px solid #ccc;'>" + mobile + "</td></tr><tr>");
            sb.Append("<td width='210' style='border-bottom:1px solid #ccc;border-right:1px solid #ccc;'><strong>Email Id</strong></td>");
            sb.Append("<td width='334' style='border-bottom:1px solid #ccc;'>" + emailId + "</td></tr><tr>");
            sb.Append("<td width='210' style='border-bottom:1px solid #ccc;border-right:1px solid #ccc;'><strong>Address</strong></td>");
            sb.Append("<td width='334' style='border-bottom:1px solid #ccc;'>" + address + "</td></tr><tr>");
            sb.Append("<td width='210' style='border-bottom:1px solid #ccc;border-right:1px solid #ccc;'><strong>Entity Created By</strong></td>");
            sb.Append("<td width='334' style='border-bottom:1px solid #ccc;'>" + userId + "</td></tr></table>");

            return sb.ToString();
        }

        [System.Web.Http.HttpPost]
        public object GetEmail(GroupInfo grp)
        {
            dynamic GrpEmailResult;
            string email = GroupMaster.GetGrpEmail(grp);

            if (email != null)
            {
                GrpEmailResult = new { status = "0", message = "success", email };
            }
            else
            {
                GrpEmailResult = new { status = "0", message = "Record not found" };
            }
            return new { GrpEmailResult };
        }

        [NonAction]
        private void senMailSMSToAdmin(string groupId, string mobilNo)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = GroupMaster.GetPwdDetails(groupId, mobilNo);

                if (ds.Tables.Count > 0)
                {
                    //send mail to admin with admin panel credentials of admin panel
                    string Password = GlobalFuns.DecryptPassward(ds.Tables[0].Rows[0]["pwd"].ToString());//decrypt password

                    GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), ds.Tables[0].Rows[0]["email"].ToString(), "[TouchBase] Entity " + ds.Tables[0].Rows[0]["group_name"].ToString() + " Successfully Created", msgbody(ds.Tables[0].Rows[0]["mobile_no"].ToString(), Password, ds.Tables[0].Rows[0]["group_name"].ToString(), ds.Tables[0].Rows[0]["createdate"].ToString(), groupId));
                }
            }
            catch
            {
            }
        }

        [NonAction]
        private string msgbody(string mobile, string password, string entityname, string createon, string groupId)
        {
            return "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>" +
                   "<html xmlns='http://www.w3.org/1999/xhtml'>" +
                   "<head>" +
                   " <meta http-equiv='Content-Type' content='text/html; charset=utf-8' />" +
                   " <title>Untitled Document</title>" +
                   " </head>" +
                   " <body style='padding:5px; margin:0px; font-family:Verdana, Geneva, sans-serif; line-height:22px; text-align:center'>" +
                   " <div id='page'>" +
                   " <div class='header'>" +
                   " <div class='logo'><img src='http://web.touchbase.in/Images/mail-logo.png' width='200' height='95' style='margin:0px auto;' /></div>" +
                   " </div>" +
                   " <div class='container'>" +
                   " <h2 style='color:#000; line-height:30px; font-size:16px; margin-bottom:0px;'>​Entity <span style='color:#1a4994;'>" + entityname + "</span> successfully created on <span style='font-size:12px; color:#000;'>" + createon + "</span></h2>" +

                   " <div class='link' style='font-size:12px; padding:5px;'><strong>Web Access :</strong> <a href='http://web.touchbase.in/'>http://web.touchbase.in/</a><br />" +
                   " <span style='width:200px;'><strong>User ID</strong></span> : <span>" + mobile + "</span><br />" +
                   " <span style='width:200px;'><strong>Password</strong> : " + password + "</span><br/><br/>" +

                   " <strong>To invite users to your entity share the link :</strong> " +
                   " <a href='http://web.touchbase.in/Redirector.aspx?Invite=" + groupId + "' target='_blank'>http://web.touchbase.in/Redirector.aspx?Invite=" + groupId + "</a><br /><br/> " +

                   " <div align='center'><a href='#'><img src='http://web.touchbase.in/images/1460475507_facebook_social_media_online.png' width='32' height='32' /></a>" +
                   " <a href='#'><img src='http://web.touchbase.in/Images/1460475484_twitter_social_media_online.png' width='32' height='32' /></a>" +
                   " <a href='#'><img src='http://web.touchbase.in/Images/1460475497_linked_in_social_media_online.png' width='32' height='32' /></a>" +
                   " <a href='#'><img src='http://web.touchbase.in/Images/1460475491_youtube_social_media_online.png' width='32' height='32' /></a>" +
                   " </div>" +
                   " </div>" +
                   " </div>" +
                   " <div class='footer'></div>" +
                   " </div>" +
                   " </body>" +
                   " </html>";
        }


        [NonAction]
        private string msgbody(string adminName, string groupName)
        {
            return "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>" +
                   "<html xmlns='http://www.w3.org/1999/xhtml'>" +
                   "<head>" +
                   "<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />" +
                   "<title>Mail</title>" +
                   "</head>" +
                   "<body style=' padding:5px; margin:0px; font-family:Verdana, Geneva, sans-serif; line-height:22px; text-align:center;'>" +
                   "<div id='page'>" +
                   "<div class='header'>" +
                   "<div class='logo'><img src='http://web.touchbase.in/Images/mail-logo.png' alt='TouchBasE' width='200' height='95' style='width:200px; height:95px; margin:0px auto;' /></div>" +
                   "</div>" +
                   "<div class='container'>" +
                   "<h2 style='color:#000; line-height:30px;'><span style='color:#1a4994;'>" + adminName + "</span> has invited you to <span style='color:#1a4994;'>" + groupName + "</span> mobile app powered by TouchBase.</h2>" +
                   "<div class='abt' style='font-size:12px;'>TouchBase is an app that makes communications<br /> Focused, Real-Time and Controlled</div><br/>" +
                //"<p style='font-size:12px;'><span style='color:#1a4994;font-weight:bold;'>" + adminName + "</span> added you</p>" +

                   " <div style='width:290px; margin:0px auto;'> " +
                   " <div style='float:left; margin:0px 5px'> " +
                   " <a href='https://itunes.apple.com/us/app/touchbase-tb/id1104294041?ls=1&mt=8' target='_blank'>  " +
                   " <img src='http://web.touchbase.in/Images/apple_ios_store.png' /> " +
                   " </a> </div><div style='float:left; margin:0px 5px'> " +
                   " <a href='https://play.google.com/store/apps/details?id=kaizen.app.com.touchbase' target='_blank'>  " +
                   " <img src='http://web.touchbase.in/Images/google_play_store.png' /></a></div>  " +
                   " <div style='clear:both;'></div></div> " +

                   "<div class='link' style='font-size:12px; padding:5px;'> " +
                   "<div align='center'><a href='#'><img src='http://web.touchbase.in/images/1460475507_facebook_social_media_online.png' width='32' height='32' /></a> <a href='#'><img src='http://web.touchbase.in/Images/1460475484_twitter_social_media_online.png' width='32' height='32' /></a> <a href='#'><img src='http://web.touchbase.in/Images/1460475497_linked_in_social_media_online.png' width='32' height='32' /></a> <a href='#'><img src='http://web.touchbase.in/Images/1460475491_youtube_social_media_online.png' width='32' height='32' /></a> </div></div></div></div>" +
                   "</body>" +
                   "</html>";
        }

        //================== Mailbody of  =============================//
        [NonAction]
        private string msgbody(string groupName)
        {
            return "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>" +
                   "<html xmlns='http://www.w3.org/1999/xhtml'>" +
                   "<head>" +
                   "<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />" +
                   "<title>Mail</title>" +
                   "</head>" +
                   "<body style=' padding:5px; margin:0px; font-family:Verdana, Geneva, sans-serif; line-height:22px; text-align:center;'>" +
                   "<div id='page'>" +
                   "<div class='header'>" +
                   "<div class='logo'><img src='http://web.touchbase.in/Images/mail-logo.png' width='200' height='95' style='width:200px; height:95px; margin:0px auto;' /></div>" +
                   "</div>" +
                   "<div class='container'>" +
                   "<h2 style=' color:#000; line-height:30px;'>You have been removed from Entity <span style='color:#1a4994;'>" + groupName + "</span> on TOUCHBasE.<br/> Please contact the admin for more info.</h2>" +
                //"<div class='abt' style='font-size:12px;'>TouchBase is an app that makes communications<br /> Focused, Real-Time and Controlled</div>" +
                //"<p style='font-size:12px;'><strong>" + adminName + "</strong> added you</p>" +
                //"<div style='width:150px; margin:0px auto; background:#1a4994; padding:5px;text-align:center!important;'><a href='#' style='color:#fff; text-decoration:none;'>Download Now</a></div>" +
                //   "<div class='link' style='font-size:12px; padding:5px;'> " +
                //   "<div align='center'><a href='#'><img src='http://web.touchbase.in/images/1460475507_facebook_social_media_online.png' width='32' height='32' /></a> <a href='#'><img src='http://web.touchbase.in/Images/1460475484_twitter_social_media_online.png' width='32' height='32' /></a> <a href='#'><img src='http://web.touchbase.in/Images/1460475497_linked_in_social_media_online.png' width='32' height='32' /></a> <a href='#'><img src='http://web.touchbase.in/Images/1460475491_youtube_social_media_online.png' width='32' height='32' /></a> </div></div>" +
                   "</div></div>" +
                   "</body>" +
                   "</html>";
        }


        #endregion

        # region Notification

        [System.Web.Http.HttpPost]
        public object GetNotificationCount(MemberLogin mem)
        {
            dynamic TBGroupResult;
            try
            {
                List<GrpNotificationCount> GrpCountList = GroupMaster.GetNotificationCount(mem.masterUID);
                if (GrpCountList != null)
                {
                    TBGroupResult = new { status = "0", message = "Success", GrpCountList };
                }
                else
                {
                    TBGroupResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGroupResult = new { status = "1", message = "failed" };

            }
            return new { TBGroupResult };
        }


        #endregion

        # region SubGroup


        //================== Add SubGroup =============================//
        [System.Web.Http.HttpPost]
        public object CreateSubGroup(SubGroup subGroup)
        {
            dynamic TBGetSubGroupListResult;

            try
            {
                int Result = GroupMaster.createSubGroup(subGroup);

                if (Result > 0)
                {
                    TBGetSubGroupListResult = new { status = "0", message = "success" };
                }
                else
                {
                    TBGetSubGroupListResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetSubGroupListResult = new { status = "1", message = "failed" };
            }

            return new { TBGetSubGroupListResult };
        }


        //================== Get SubGroup List=============================//
        [System.Web.Http.HttpPost]
        public object GetSubGroupList(SubGroup sub)
        {
            dynamic TBGetSubGroupListResult;
            List<object> SubGroupResult = new List<object>();
            List<SubGroupList> Result = new List<SubGroupList>();
            try
            {
                if (!string.IsNullOrEmpty(sub.memberProfileId) || !string.IsNullOrEmpty(sub.memberMainId))
                {
                    Result = GroupMaster.GetSubGroupDirectory(sub);
                }
                else
                {
                    Result = GroupMaster.getSubGroupList(sub);
                }
                for (int i = 0; i < Result.Count; i++)
                {
                    SubGroupResult.Add(new { Subgroup = Result[i] });
                }

                if (Result != null)
                {
                    TBGetSubGroupListResult = new { status = "0", message = "success", SubGroupResult = SubGroupResult };
                }
                else
                {
                    TBGetSubGroupListResult = new { status = "0", message = "Record not found", SubGroupResult = SubGroupResult };
                }
            }
            catch
            {
                TBGetSubGroupListResult = new { status = "1", message = "failed" };
            }

            return new { TBGetSubGroupListResult };
        }


        //================== Get SubGroup Detail=============================//
        [System.Web.Http.HttpPost]
        public object GetSubGroupDetail(SubGroupDtlSearch sub)
        {
            dynamic TBGetSubGroupDetailListResult;
            List<object> MemberInsubGrpResult = new List<object>();

            try
            {
                List<SubGroupDetail> Result = GroupMaster.getSubGroupDetails(sub);

                for (int i = 0; i < Result.Count; i++)
                {
                    MemberInsubGrpResult.Add(new { SubgrpMemberDetail = Result[i] });
                }

                if (Result != null)
                {
                    TBGetSubGroupDetailListResult = new { status = "0", message = "success", SubGroupResult = MemberInsubGrpResult };
                }
                else
                {
                    TBGetSubGroupDetailListResult = new { status = "0", message = "Record not found", SubGroupResult = MemberInsubGrpResult };
                }
            }
            catch
            {
                TBGetSubGroupDetailListResult = new { status = "1", message = "failed" };
            }

            return new { TBGetSubGroupDetailListResult };
        }


        # endregion

        # region Group Member
        //================== Add member to particular group =============================//
        [System.Web.Http.HttpPost]
        public object AddMemberToGroup(AddMember members)
        {
            dynamic TBAddMemberGroupResult;
            string[] AddMemberREsult = new string[0];
            DataSet Result = new DataSet();

            try
            {
                Result = GroupMaster.AddMemberToGroup(members);

                if (Result.Tables[0].Rows.Count > 0)
                {
                    //#region Register user for Chat
                    //// Register user if Group has Chat module
                    //if (Result.Tables[0].Rows[0]["chat"].ToString() == "Y")
                    //{
                    //    //GroupMaster.AddRemoveMember(members.mobile, members.groupId, 1);//1 is for insert mode                                      

                    //    string username = members.groupId + "_" + members.mobile;

                    //    //string host = "version.touchbase.in";
                    //    ////ejabberdctl register "testuser" "localhost" "passw0rd"  ----------- command format
                    //    //string cmd = "ejabberdctl register " + '"' + username + '"' + " " + '"' + host + '"' + " " + '"' + username + '"' + "";
                    //    //int i = GlobalFuns.ExecuteCommandSync(cmd, 1);

                    //    GlobalFuns.ExecuteCommandChat(username, 0);
                    //}

                    //#endregion

                    TBAddMemberGroupResult = new { status = "0", message = "success", totalMember = Result.Tables[0].Rows[0]["TotalMembers"].ToString(), AddMemberResult = AddMemberREsult };
                }
                else
                {
                    TBAddMemberGroupResult = new { status = "0", message = "Record not found", totalMember = Result.Tables[0].Rows[0]["TotalMembers"].ToString() };
                }

            }
            catch (Exception ex)
            {
                TBAddMemberGroupResult = new { status = "1", message = "failed", error = ex.Message.ToString() };
            }

            //if (Result.Tables[0].Rows[0]["chkAdded"].ToString() == "1")
            //{
            //string countryCode = GroupMaster.CountryCode(members.countryId);
            ////send SMS to National/International user
            //if (countryCode == "+91")//for India
            //    GlobalFuns.SendSMSOnAdd(members.mobile, Result.Tables[0].Rows[0]["member_name"].ToString() + " has invited you to " + Result.Tables[0].Rows[0]["group_name"].ToString() + " mobile app powered by TouchBasE . Download link : http://goo.gl/uK2UQQ");
            //else//for International SMS
            //    GlobalFuns.SendSMSInterOnAdd(members.mobile, Result.Tables[0].Rows[0]["member_name"].ToString() + " has invited you to " + Result.Tables[0].Rows[0]["group_name"].ToString() + " mobile app powered by TouchBasE . Download link : http://goo.gl/uK2UQQ", countryCode);


            //send SMS to user

            //DataSet dsCountry = new DataSet();
            //dsCountry = GroupMaster.CountryCode(members.countryId, members.mobile, members.groupId);

            //if (dsCountry.Tables[0].Rows[0]["sentSMS"].ToString() == "Yes")
            //{
            //}

            //if (dsCountry.Tables[0].Rows[0]["isSentSMS"].ToString() == "Yes")
            //{
            //    if (dsCountry.Tables[0].Rows[0]["CountryCode"].ToString() == "+91")//for India
            //    {
            //        if ((GlobalFuns.SendSMSOnAdd(members.mobile, Result.Tables[0].Rows[0]["member_name"].ToString() + " has invited you to " + Result.Tables[0].Rows[0]["group_name"].ToString() + " mobile app powered by TouchBasE . Download link : http://goo.gl/uK2UQQ")) == true)
            //        {
            //            GroupMaster.UpdateSMSCount(members.groupId);
            //        }
            //    }
            //    else//for International SMS
            //    {
            //        if ((GlobalFuns.SendSMSInterOnAdd(members.mobile, Result.Tables[0].Rows[0]["member_name"].ToString() + " has invited you to " + Result.Tables[0].Rows[0]["group_name"].ToString() + " mobile app powered by TouchBasE . Download link : http://goo.gl/uK2UQQ", dsCountry.Tables[0].Rows[0]["CountryCode"].ToString())) == true)
            //        {
            //            GroupMaster.UpdateSMSCount(members.groupId);
            //        }
            //    }
            //}

            ////send Email to user
            //if (!string.IsNullOrEmpty(members.memberEmail))
            //{
            //    GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), members.memberEmail.ToString(), Result.Tables[0].Rows[0]["member_name"].ToString() + " has added you into Entity " + Result.Tables[0].Rows[0]["group_name"].ToString() + " on Touchbase.", msgbody(Result.Tables[0].Rows[0]["member_name"].ToString(), Result.Tables[0].Rows[0]["group_name"].ToString()));
            //    //GlobalFuns.SendElasticEmail(members.memberEmail.ToString(), Result.Tables[0].Rows[0]["member_name"].ToString() + " has added you into " + Result.Tables[0].Rows[0]["group_name"].ToString() + " on Touchbase.", "", msgbody(Result.Tables[0].Rows[0]["member_name"].ToString()), ConfigurationManager.AppSettings["frommail"].ToString(), "kaizeninfotech.com");
            //}
            //var MID = Result.Tables[1].Rows[0]["memID"].ToString();
            //var grpID = Result.Tables[0].Rows[0]["grpID"].ToString();
            //string url = ConfigurationManager.AppSettings["imgPath"] + "php/AddMember.php?MID=" + MID + " &GrpID=" + grpID;
            //GroupMaster.Send(url);
            //}


            return new { TBAddMemberGroupResult };
        }


        /// <summary>
        /// Add multiple Members to corresponding group from mobile Phone book
        /// Author : Nandkishor K
        /// Created on : 09/08/2016
        /// </summary>
        /// <returns></returns>
        /// 

        //================== Add multiple Members to corresponding group =============================//
        [System.Web.Http.HttpPost]
        public object AddMultipleMemberToGroup(AddMultipleMembers AddMember)
        {
            dynamic TBAddMultipleMemberGroupResult;
            string[] AddMemberREsult = new string[0];
            DataSet Result = new DataSet();

            try
            {
                foreach (AddMember mem in AddMember.newmembers)
                {
                    mem.countryId = GroupMaster.getCountryId(string.IsNullOrEmpty(mem.countryId) ? "" : mem.countryId);

                    Result = GroupMaster.AddMemberToGroup(mem);

                    if (Result.Tables[0].Rows.Count > 0)
                    {
                        #region Register user for Chat

                        //// Register user if Group has Chat module
                        //if (Result.Tables[0].Rows[0]["chat"].ToString() == "Y")
                        //{
                        //    //GroupMaster.AddRemoveMember(members.mobile, members.groupId, 1);//1 is for insert mode                                      

                        //    string username = mem.groupId + "_" + mem.mobile;

                        //    //string host = "version.touchbase.in";
                        //    ////ejabberdctl register "testuser" "localhost" "passw0rd"  ----------- command format
                        //    //string cmd = "ejabberdctl register " + '"' + username + '"' + " " + '"' + host + '"' + " " + '"' + username + '"' + "";
                        //    //int i = GlobalFuns.ExecuteCommandSync(cmd, 1);

                        //    GlobalFuns.ExecuteCommandChat(username, 0);
                        //}

                        #endregion
                        #region Commented by Nandu on 16-03-2017 Task -> Stop sending notifications to user. req by -> Anirudh

                        //if (Result.Tables[0].Rows[0]["chkAdded"].ToString() == "1")
                        //{
                        //    DataSet dsCountry = new DataSet();
                        //    dsCountry = GroupMaster.CountryCode(mem.countryId, mem.mobile, mem.groupId);

                        ////send SMS to user
                        //if (dsCountry.Tables[0].Rows[0]["isSentSMS"].ToString() == "Yes")
                        //{
                        //    if (mem.countryId == "1")//for India
                        //    {
                        //        if ((GlobalFuns.SendSMSOnAdd(mem.mobile, Result.Tables[0].Rows[0]["member_name"].ToString() + " has invited you to " + Result.Tables[0].Rows[0]["group_name"].ToString() + " mobile app powered by TouchBasE . Download link : http://goo.gl/uK2UQQ")) == true)
                        //        {
                        //            GroupMaster.UpdateSMSCount(mem.groupId);
                        //        }
                        //    }
                        //    else//for International SMS
                        //    {
                        //        if ((GlobalFuns.SendSMSInterOnAdd(mem.mobile, Result.Tables[0].Rows[0]["member_name"].ToString() + " has invited you to " + Result.Tables[0].Rows[0]["group_name"].ToString() + " mobile app powered by TouchBasE . Download link : http://goo.gl/uK2UQQ", dsCountry.Tables[0].Rows[0]["CountryCode"].ToString())) == true)
                        //        {
                        //            GroupMaster.UpdateSMSCount(mem.groupId);
                        //        }
                        //    }
                        //}

                        ////send Email to user
                        //if (!string.IsNullOrEmpty(mem.memberEmail))
                        //{
                        //    GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), mem.memberEmail.ToString(), Result.Tables[0].Rows[0]["member_name"].ToString() + " has added you into Entity " + Result.Tables[0].Rows[0]["group_name"].ToString() + " on Touchbase.", msgbody(Result.Tables[0].Rows[0]["member_name"].ToString(), Result.Tables[0].Rows[0]["group_name"].ToString()));
                        //    //GlobalFuns.SendElasticEmail(members.memberEmail.ToString(), Result.Tables[0].Rows[0]["member_name"].ToString() + " has added you into " + Result.Tables[0].Rows[0]["group_name"].ToString() + " on Touchbase.", "", msgbody(Result.Tables[0].Rows[0]["member_name"].ToString()), ConfigurationManager.AppSettings["frommail"].ToString(), "kaizeninfotech.com");
                        //}
                        //var MID = Result.Tables[1].Rows[0]["memID"].ToString();
                        //var grpID = Result.Tables[0].Rows[0]["grpID"].ToString();
                        //string url = ConfigurationManager.AppSettings["imgPath"] + "php/AddMember.php?MID=" + MID + " &GrpID=" + grpID;
                        //GroupMaster.Send(url);
                        // }
                        # endregion
                    }
                }

                TBAddMultipleMemberGroupResult = new { status = "0", message = "success" };
            }
            catch (Exception ex)
            {
                TBAddMultipleMemberGroupResult = new { status = "1", message = "failed", error = ex.Message.ToString() };
            }

            #region Commented Sent SMS and Mail

            //if (Result.Tables[0].Rows[0]["chkAdded"].ToString() == "1")
            //{
            //    foreach (AddMember item in members)
            //    {
            //        //send SMS to user

            //        DataSet dsCountry = new DataSet();
            //        dsCountry = GroupMaster.CountryCode(item.countryId, item.mobile, item.groupId);

            //        if (dsCountry.Tables[0].Rows[0]["isSentSMS"].ToString() == "Yes")
            //        {
            //            if (dsCountry.Tables[0].Rows[0]["CountryCode"].ToString() == "+91")//for India
            //            {
            //                if ((GlobalFuns.SendSMSOnAdd(item.mobile, Result.Tables[0].Rows[0]["member_name"].ToString() + " has invited you to " + Result.Tables[0].Rows[0]["group_name"].ToString() + " mobile app powered by TouchBasE . Download link : http://goo.gl/uK2UQQ")) == true)
            //                {
            //                    GroupMaster.UpdateSMSCount(item.groupId);
            //                }
            //            }
            //            else//for International SMS
            //            {
            //                if ((GlobalFuns.SendSMSInterOnAdd(item.mobile, Result.Tables[0].Rows[0]["member_name"].ToString() + " has invited you to " + Result.Tables[0].Rows[0]["group_name"].ToString() + " mobile app powered by TouchBasE . Download link : http://goo.gl/uK2UQQ", dsCountry.Tables[0].Rows[0]["CountryCode"].ToString())) == true)
            //                {
            //                    GroupMaster.UpdateSMSCount(item.groupId);
            //                }
            //            }
            //        }

            //        //send Email to user
            //        if (!string.IsNullOrEmpty(item.memberEmail))
            //        {
            //            GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), item.memberEmail.ToString(), Result.Tables[0].Rows[0]["member_name"].ToString() + " has added you into Entity " + Result.Tables[0].Rows[0]["group_name"].ToString() + " on Touchbase.", msgbody(Result.Tables[0].Rows[0]["member_name"].ToString(), Result.Tables[0].Rows[0]["group_name"].ToString()));
            //            //GlobalFuns.SendElasticEmail(members.memberEmail.ToString(), Result.Tables[0].Rows[0]["member_name"].ToString() + " has added you into " + Result.Tables[0].Rows[0]["group_name"].ToString() + " on Touchbase.", "", msgbody(Result.Tables[0].Rows[0]["member_name"].ToString()), ConfigurationManager.AppSettings["frommail"].ToString(), "kaizeninfotech.com");
            //        }
            //    }

            //    var MID = Result.Tables[1].Rows[0]["memID"].ToString();
            //    var grpID = Result.Tables[0].Rows[0]["grpID"].ToString();
            //    string url = ConfigurationManager.AppSettings["imgPath"] + "php/AddMember.php?MID=" + MID + " &GrpID=" + grpID;
            //    GroupMaster.Send(url);
            //}

            #endregion

            return new { TBAddMultipleMemberGroupResult };
        }


        //================== Remove Member from Group =============================//
        [System.Web.Http.HttpPost]
        public object DeleteMember(MemberProfile memPro)
        {
            dynamic TBDeleteMemberResult;

            try
            {
                var Result = GroupMaster.deleteMember(memPro.memberProfileId, memPro.groupId);

                if (Result != null)
                {
                    //#region Unregister user from Chat

                    ////Delete user from chat database
                    //string mobile = GroupMaster.getMobilNo(memPro.memberProfileId, memPro.groupId);
                    //string username = memPro.groupId + "_" + mobile;

                    ////GroupMaster.AddRemoveMember(mobile, memPro.groupId, 2);//2 is for delete mode  
                    ////string host = "version.touchbase.in";
                    ////string cmd = "ejabberdctl unregister " + '"' + username + '"' + " " + '"' + host + '"' + "";
                    ////int i = GlobalFuns.ExecuteCommandSync(cmd, 2);

                    //GlobalFuns.ExecuteCommandChat(username, 1);

                    //#endregion

                    TBDeleteMemberResult = new { status = "0", message = "success", totalMember = Result };
                }
                else
                {
                    TBDeleteMemberResult = new { status = "0", message = "Record not found" };
                }

                //string url = ConfigurationManager.AppSettings["imgPath"] + "php/RemoveMember.php?MID=" + memPro.memberProfileId + " &GrpID=" + memPro.groupId;
                //GroupMaster.Send(url);
            }
            catch
            {
                TBDeleteMemberResult = new { status = "1", message = "failed" };
            }

            return new { TBDeleteMemberResult };
        }


        //================== Global search profile =============================//
        [System.Web.Http.HttpPost]
        public object GlobalSearchGroup(clsGlobalSearchGroup sub)
        {
            dynamic TBGlobalSearchGroupResult;
            List<object> AllGlobalGroupListResults = new List<object>();

            try
            {
                DataSet Result = GroupMaster.globalSearchGroup(sub);

                DataTable dtmain = Result.Tables[0];
                DataTable dtsearch = Result.Tables[1];

                string member_name, member_mobile, ProfilePic_Path;

                member_name = dtmain.Rows[0]["member_name"].ToString();
                member_mobile = dtmain.Rows[0]["member_mobile"].ToString();
                ProfilePic_Path = dtmain.Rows[0]["ProfilePic_Path"].ToString();



                //Convert DataTable into List
                List<clsGlobalSearch> globalSearch = new List<clsGlobalSearch>();
                if (dtsearch.Rows.Count > 0)
                {
                    globalSearch = GlobalFuns.DataTableToList<clsGlobalSearch>(dtsearch);
                }


                //Add List into another List
                for (int i = 0; i < globalSearch.Count; i++)
                {
                    AllGlobalGroupListResults.Add(new { GlobalGroupResult = globalSearch[i] });
                }

                if (Result != null)
                {
                    TBGlobalSearchGroupResult = new { status = "0", message = "success", membername = member_name, membermobile = member_mobile, profilepicpath = ProfilePic_Path, AllGlobalGroupListResults = AllGlobalGroupListResults };
                }
                else
                {
                    TBGlobalSearchGroupResult = new { status = "0", message = "Record not found", AllGlobalGroupListResults = AllGlobalGroupListResults };
                }
            }
            catch
            {
                TBGlobalSearchGroupResult = new { status = "1", message = "failed" };
            }

            return new { TBGlobalSearchGroupResult };
        }


        # endregion

        # region Common APIs
        //================== get all Countries and Categories =============================//
        [System.Web.Http.HttpPost]
        public object GetAllCountriesAndCategories()
        {
            dynamic TBCountryResult;

            List<object> CountryLists = new List<object>();
            List<object> CategoryLists = new List<object>();

            try
            {
                List<CountryResult> Result1 = GroupMaster.getAllCountriesLists();
                for (int i = 0; i < Result1.Count; i++)
                {
                    CountryLists.Add(new { GrpCountryList = Result1[i] });
                }

                List<GroupCategoryResult> Result2 = GroupMaster.getAllGroupCategory();
                for (int i = 0; i < Result2.Count; i++)
                {
                    CategoryLists.Add(new { GrpCatList = Result2[i] });
                }

                #region
                //List<object> PersonalGroupListResults = new List<object>();
                //List<object> SocialGroupListResults = new List<object>();
                //List<object> BusinessGroupListResults = new List<object>();

                //List<GroupResult> Result2 = GroupMaster.getPersonalGroupLists(mem.masterUID);
                //for (int i = 0; i < Result2.Count; i++)
                //{
                //    PersonalGroupListResults.Add(new { GroupResult = Result2[i] });
                //}

                //List<GroupResult> Result3 = GroupMaster.getSocialGroupLists(mem.masterUID);
                //for (int i = 0; i < Result3.Count; i++)
                //{
                //    SocialGroupListResults.Add(new { GroupResult = Result3[i] });
                //}

                //List<GroupResult> Result4 = GroupMaster.getBusinessGroupLists(mem.masterUID);
                //for (int i = 0; i < Result4.Count; i++)
                //{
                //    BusinessGroupListResults.Add(new { GroupResult = Result4[i] });
                //}

                //, PersonalGroupListResults, SocialGroupListResults, BusinessGroupListResults 
                #endregion

                if (CountryLists != null)
                {
                    TBCountryResult = new { status = "0", message = "success", CountryLists, CategoryLists };
                }
                else
                {
                    TBCountryResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBCountryResult = new { status = "1", message = "failed" };
            }

            return new { TBCountryResult };
        }

        //================== Common web service for Delete Image=============================//
        [HttpPost]
        public object DeleteImage(DeleteImage mod)
        {
            dynamic DeleteResult;
            string result = "";
            try
            {
                if (mod.typeID == "0" || string.IsNullOrEmpty(mod.type))
                {
                    throw new InvalidOperationException();
                }

                result = GroupMaster.DeletebyImage(mod);
                DeleteResult = new { status = "0", message = "success" };
            }
            catch (Exception ex)
            {
                DeleteResult = new { status = "1", message = "failed", error = ex.Message.ToString() };
            }
            //if (!string.IsNullOrEmpty(result))
            //{
            //    if (mod.type == "Group")
            //    {
            //        string path = HttpContext.Current.Server.MapPath("~/Documents/Group/" + result.ToString());
            //        if (File.Exists(path))
            //            File.Delete(path);
            //    }
            //    else if (mod.type == "Member")
            //    {
            //        string path = HttpContext.Current.Server.MapPath("~/Documents/directory/Group" + mod.grpID + "/" + result.ToString());
            //        if (File.Exists(path))
            //            File.Delete(path);
            //    }
            //    else
            //    {
            //        string path = HttpContext.Current.Server.MapPath("~/Documents/" + mod.type + "/Group" + mod.grpID + "/" + result.ToString());
            //        if (File.Exists(path))
            //            File.Delete(path);
            //    }
            //}
            return new { DeleteResult };
        }

        #endregion

        # region Suggest Feature
        [HttpPost]
        public object SuggestFeature(suggestFeature suggestion)
        {
            dynamic SuggestFeatureResult;
            try
            {
                var result = GroupMaster.AddSuggestedFeature(suggestion);
                if (result != null)
                {
                    SuggestFeatureResult = new { status = "0", message = "success" };
                    foreach (suggestFeatureResult s in result)
                    {
                        if (s.emailID != "")
                        {
                            string msgSubject = "New Feature suggestion for TouchBase";
                            string msgbody = "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>" +
                                               "<html xmlns='http://www.w3.org/1999/xhtml'>" +
                                               "<head>" +
                                               "<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />" +
                                               "<title>Mail</title>" +
                                               "</head>" +
                                               "<body style=' padding:5px; margin:0px; font-family:Verdana, Geneva, sans-serif; line-height:22px; text-align:center;'>" +
                                               "<div id='page'>" +
                                               "<div class='header'>" +
                                               "<div class='logo'><img src='http://web.touchbase.in/Images/mail-logo.png' width='200' height='95' style='width:200px; height:95px; margin:0px auto;' /></div>" +
                                               "</div>" +
                                               "<div class='container'>" +
                                               "<h2 style=' color:#1a4994; line-height:30px;'>" + s.suggestedBy + "<span style='color:#000;'> has sent a suggestion.</span></h2><br><span style='font-weight:bold; font-size:14px;'>Contact Number :</span><span style=' color:#1a4994;font-size:14px;'>" + s.usercontact + "<br>" + s.userEmail + "<br>" + suggestion.title + "<br>" + suggestion.description + "<br>"
                                                + "</div></div>" +
                                               "</body>" +
                                               "</html>";
                            GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), s.emailID.ToString(), msgSubject, msgbody);
                            //GlobalFuns.SendElasticEmail(members.memberEmail.ToString(), Result.Tables[0].Rows[0]["member_name"].ToString() + " has added you into " + Result.Tables[0].Rows[0]["group_name"].ToString() + " on Touchbase.", "", msgbody(Result.Tables[0].Rows[0]["member_name"].ToString()), ConfigurationManager.AppSettings["frommail"].ToString(), "kaizeninfotech.com");
                        }
                    }
                }
                else
                {
                    SuggestFeatureResult = new { status = "1", message = "failed" };
                }
            }
            catch
            {
                SuggestFeatureResult = new { status = "1", message = "failed" };
            }

            return new { SuggestFeatureResult };
        }

        #endregion

        #region Rotary Library Module Data

        /// <summary>
        /// Created By: Nandu
        /// Created On: 16-05-2017
        /// Task: Get rotary library data 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetRotaryLibraryData()
        {
            dynamic TBGetRotaryLibraryResult;
            try
            {
                List<clsGetRotaryLibraryOutput> Result = GroupMaster.GetRotaryLibraryList();

                if (Result != null)
                {
                    TBGetRotaryLibraryResult = new { status = "0", message = "success", RotaryLibraryListResult = Result };
                }
                else
                {
                    TBGetRotaryLibraryResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetRotaryLibraryResult = new { status = "1", message = "failed" };
            }

            return new { TBGetRotaryLibraryResult };
        }

        #endregion

        #region Feedback To Team ROW

        /// <summary>
        /// Created by : Nandu
        /// Created on : 23/05/2017
        /// Task : Registration mail
        /// </summary>
        [HttpPost]
        public object Feedback(FeedbackInput obj)
        {
            dynamic FeedbackResult;
            try
            {
                DataSet ds = GroupMaster.getClubFeedbackDetail(obj);

                if (ds != null)
                {
                    if (ds.Tables != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[0]["Email"])))
                            {
                                if ((GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), "row.techsupport@kaizeninfotech.com, securitycode@kaizeninfotech.com", "Feedback for Roster On Wheels", mailBody(Convert.ToString(ds.Tables[0].Rows[0]["Name"]), Convert.ToString(ds.Tables[0].Rows[0]["MobileNo"]), Convert.ToString(ds.Tables[0].Rows[0]["Email"]), Convert.ToString(ds.Tables[0].Rows[0]["ClubName"]), Convert.ToString(ds.Tables[0].Rows[0]["Dictrict"]), obj.Feedback)) == ""))
                                {
                                    GlobalFuns.SendEmail(ConfigurationManager.AppSettings["frommail"].ToString(), Convert.ToString(ds.Tables[0].Rows[0]["Email"]), "Thank you for Feedback", userMailBody(Convert.ToString(ds.Tables[0].Rows[0]["Name"])));
                                    FeedbackResult = new { status = "0", message = "success" };
                                }
                                else
                                {
                                    FeedbackResult = new { status = "0", message = "Record not found, feedback not sent" };
                                }
                            }
                            else
                            {
                                FeedbackResult = new { status = "0", message = "Email id does not exists" };
                            }
                        }
                        else
                        {
                            FeedbackResult = new { status = "0", message = "Record not found, feedback not sent" };
                        }
                    }
                    else
                    {
                        FeedbackResult = new { status = "0", message = "Record not found, feedback not sent" };
                    }
                }
                else
                {
                    FeedbackResult = new { status = "0", message = "Record not found, feedback not sent" };
                }
            }
            catch
            {
                FeedbackResult = new { status = "1", message = "failed" };
            }

            return new { FeedbackResult };
        }

        /// <summary>
        /// User defined functions
        /// Created by : Nandu
        /// Created on : 23/05/2017
        /// Task : Admin mail body
        /// </summary>
        [NonAction]
        private string mailBody(string name, string mobileNo, string email, string clubName, string district, string feedback)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>");
            sb.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");
            sb.Append("<head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            sb.Append("<title>Roster on Wheels</title></head>");
            sb.Append("<body><table width='600' border='0' cellpadding='10' style='background-color:#fbf9f9; border:1px solid #dbdbdb; font-family:Roboto;font-size:13px; color:#585453;'>");

            sb.Append("<tr><td colspan='2' style='border-bottom:2px solid #ffcb5b;background-color:#FFF;';>");
            sb.Append("<img src='http://webtest.rosteronwheels.com/images/logo-header.png'/></td></tr>");

            sb.Append("<tr><td width='191' style='padding-top: 25px; color:#01a0e2;'>Name: </td>");
            sb.Append("<td width='361' style='padding-top: 25px;'>");
            sb.Append(name);
            sb.Append("</td></tr>");

            sb.Append("<tr><td width='191' style='color:#01a0e2;'>Moile number: </td>");
            sb.Append("<td width='361'>");
            sb.Append(mobileNo);
            sb.Append("</td></tr>");

            sb.Append("<tr><td width='191' style='color:#01a0e2;'>Email: </td>");
            sb.Append("<td width='361'>");
            sb.Append(email);
            sb.Append("</td></tr>");

            sb.Append("<tr><td width='191' style='color:#01a0e2;'>Club name: </td>");
            sb.Append("<td width='361'>");
            sb.Append(clubName);
            sb.Append("</td></tr>");

            sb.Append("<tr><td width='191' style='color:#01a0e2;'>District number: </td>");
            sb.Append("<td width='361'>");
            sb.Append(district);
            sb.Append("</td></tr>");

            sb.Append("<tr><td width='191' style='color:#01a0e2;'>Feedback: </td>");
            sb.Append("<td>");
            sb.Append(feedback);
            sb.Append("</td></tr>");

            //sb.Append("<tr><td colspan='2' style='border-top: 1px solid #e0e0e0'><p>Regards,<br /><b>Team Roster On Wheels</b><br /><br/><b>Email</b> : support@rosteronwheels.com<br/>");
            //sb.Append("<b>Phone No.</b> : +91 22 41516999 ( Mon-Fri- 9:30 AM to 7:00 PM (IST))<br /><br/>Roster On Wheels is developed and marketed by Kaizen Infotech Solutions PVT. LTD.<br /><br />");
            //sb.Append("<b>INDIA:</b> 1st Floor, Building A, Gala Industrial Estate,<br />Deen Dayal Upadhyay Marg,<br />Mulund West, Mumbai 400080.<br /><br />");
            //sb.Append("<b>USA:</b> 4048, sharon Park Lane,<br />Cincinnati, Ohio 4524</p></td></tr> <tr><td><img src='http://webtest.rosteronwheels.com/images/logo-kaizen.png' width='141' height='86' style='float:left;' /></td><td></td></tr>");

            sb.Append("</table></body>");
            sb.Append("</html>");

            return sb.ToString();
        }

        /// <summary>
        /// Created by : Nandu
        /// Created on : 23/05/2017
        /// Task : Customer mail body 
        /// </summary>
        /// <returns></returns>
        [NonAction]
        private string userMailBody(string Name)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>");
            sb.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");
            sb.Append("<head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            sb.Append("<title>Roster on Wheels</title></head>");
            sb.Append("<body>");
            sb.Append("<table width='600' border='0' cellpadding='10' style='background-color:#fbf9f9; border:1px solid #dbdbdb; font-family:Roboto;font-size:13px; color:#585453;'>");
            sb.Append("<tr><td colspan='2' style='border-bottom:2px solid #ffcb5b;background-color:#FFF;';><img src='http://webtest.rosteronwheels.com/images/logo-header.png'/></td></tr>");

            sb.Append("<tr><td style='padding-top: 25px; color:#01a0e2;'><strong>Dear Rtn ");
            sb.Append(Name);
            sb.Append("</strong></td></tr>");
            sb.Append("<tr>");
            sb.Append("<td style='border-bottom: 1px solid #e0e0e0; padding-top: 12px;padding-bottom: 63px;'>We acknowledge the receipt of your Feedback/Query. Response to your Feedback/Query is of prime importance to us.");
            sb.Append("<br />We shall revert to you at the earliest.</td>");
            sb.Append("</tr>");
            sb.Append("<tr><td><strong>Thanking You,<br />Tech Support Team<br />Roster On Wheels<br />Kaizen InfoTech Solutions Pvt Ltd<br />Email: row.techsupport@kaizeninfotech.com<br />Phone: +91 22 41516999</strong></td></tr>");
            sb.Append("<tr><td><img src='http://webtest.rosteronwheels.com/images/logo-kaizen.png' width='141' height='86' style='float:left;' /></td><td></td></tr>");
            sb.Append("</table>");
            sb.Append("</body></html>");

            return sb.ToString();
        }

        #endregion

    }

}
