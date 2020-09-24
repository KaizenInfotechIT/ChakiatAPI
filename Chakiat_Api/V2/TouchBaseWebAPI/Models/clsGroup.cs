using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchBaseWebAPI.Data;


namespace TouchBaseWebAPI.Models
{
    public class clsGroup
    {
    }

    public class ModulesList
    {
        public string moduleID { get; set; }
        public string moduleName { get; set; }
        public string moduleImage { get; set; }
        public string modulePriceRs { get; set; }
        public string modulePriceUS { get; set; }
        public string moduleInfo { get; set; }
    }

    public class ModuleMaster
    {
        public string moduleID { get; set; }
        public string replicaOfModule { get; set; }
    }

    public class MemberProfile
    {
        public string memberProfileId { get; set; }
        public string groupId { get; set; }

        public string type { get; set; }
        public string isAdmin { get; set; }

        public string searchText { get; set; }
        public string mycategory { get; set; }
        public string memberMainId { get; set; }
        public string modulelist { get; set; }
        public string updatedOn { get; set; }
    }

    //======================SUB GROUP Classes Start===========================//

    //Add SubGroup
    public class SubGroup
    {
        public string subGroupTitle { get; set; }
        public string memberProfileId { get; set; }
        public string subGroupID { get; set; }
        public string groupId { get; set; }
        public string subGroupType { get; set; }
        public string memberMainId { get; set; }
        public string parentID { get; set; }
        public string MemberIDs { get; set; }
    }


    public class SubGroupDtlSearch
    {
        public string groupId { get; set; }
        public string subgrpId { get; set; }
    }

    public class SubGroupList
    {
        public string subgrpId { get; set; }
        public string subgrpTitle { get; set; }
        public string noOfmem { get; set; }
    }

    public class SubGroupDetail
    {
        public string profileId { get; set; }
        public string memname { get; set; }
        public string mobile { get; set; }
    }

    public class ClubListInput
    {
        public string GroupId { get; set; }
        public string districtNumber { get; set; }
        public string search { get; set; }
    }

    //======================SUB GROUP Classes End===========================//

    public class GroupModulesList
    {
        public string groupModuleId { get; set; }
        public string groupId { get; set; }
        public string moduleId { get; set; }
        public string moduleName { get; set; }
        public string moduleOrderNo { get; set; }
        public string moduleStaticRef { get; set; }
        public string image { get; set; }
        public string masterProfileID { get; set; }
        public string isCustomized { get; set; }
    }

    public class GroupResult
    {
        public string grpId { get; set; }
        public string grpName { get; set; }
        public string grpImg { get; set; }
        public string grpProfileId { get; set; }
        public string myCategory { get; set; }
        public string isGrpAdmin { get; set; }
        public string IsSubGrpAdmin { get; set; }
        public string expiryDate { get; set; }
        // public string encryptGrpKey { get; set; }
    }

    public class CountryResult
    {
        public string countryId { get; set; }
        public string countryCode { get; set; }
        public string countryName { get; set; }
    }

    public class GroupCategoryResult
    {
        public string catId { get; set; }
        public string catName { get; set; }
    }

    public class clsgroup_master
    {
        public string grpId { get; set; }
        public string grpName { get; set; }
        public string grpImageID { get; set; }
        public string grpType { get; set; }
        public string grpCategory { get; set; }

        public string other { get; set; }

        public string addrss1 { get; set; }
        public string addrss2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string pincode { get; set; }
        public string country { get; set; }

        public string emailid { get; set; }
        public string mobile { get; set; }
        public string website { get; set; }

        public string userId { get; set; }
        public string userpwd { get; set; }
        public string grpImageName { get; set; }

    }

    //=======================================

    public class clsGlobalSearchGroup
    {
        public string memId { get; set; }
        public string otherMemId { get; set; }
    }

    public class clsGlobalSearch
    {
        public string grpId { get; set; }
        public string grpName { get; set; }
        public string grpImg { get; set; }
        public string grpProfileId { get; set; }
        public string isMygrp { get; set; }
    }

    /// <summary>
    /// Author:Pramod S
    /// Created Date:24-08-2016
    /// </summary>
    public class ModuleData
    {
        public int moduleID { get; set; }
        public string oldName { get; set; }
        public string newName { get; set; }
        public string imageName { get; set; }
    }

    /// <summary>
    /// Author:Pramod S
    /// Created Date:24-08-2016
    /// </summary>
    public class AddGroupModule
    {
        public string grpId { get; set; }
        public List<ModuleData> moduleIDs { get; set; }
        public string userID { get; set; }
        public string noOfmember { get; set; }
        public string memberCount { get; set; }
        public string Pwd { get; set; }
    }

    public class AddModuleResult
    {
        public string grpId { get; set; }
        public string grpName { get; set; }
        public string grpImage { get; set; }
        public string noOfMembers { get; set; }
        public string mobileno { get; set; }
    }
    //======================================

    #region Add Multiple Members to Group

    public class MemberResult
    {
        private DateTime dt = System.DateTime.Now;

        public string memberName { get; set; }
        public int memberId { get; set; }
        public string memberPhone { get; set; }
        public int GrpId { get; set; }

        public int createdBy { get; set; }
        public DateTime createdDate { get { return dt; } set { dt = System.DateTime.Now; } }
    }

    public class AddMemberREsult
    {
        public MemberResult MemberResult { get; set; }
    }

    public class TBCreateGroupResult
    {
        public string totalMember { get; set; }
        public string groupId { get; set; }
        public string mainmemberID { get; set; }
        public IEnumerable<AddMemberREsult> AddMemberREsult { get; set; }
    }

    public class GroupRootObject
    {
        public TBCreateGroupResult TBCreateGroupResult { get; set; }
    }

    #endregion

    public class GetGroupInfo
    {
        public string grpId { get; set; }
        public string grpNAme { get; set; }
        public string grpCat { get; set; }
        public string grpType { get; set; }
        public string grpImg { get; set; }
        public string grpAddress { get; set; }
        public string createdDateTime { get; set; }
        public string grpAdminProfileId { get; set; }
        public string grpAdmin { get; set; }
        public string grpEmail { get; set; }
        public string grpMobile { get; set; }
        public string cityCountry { get; set; }
    }

    public class DeleteByModuleName
    {
        public string typeID { get; set; }
        public string type { get; set; }
        public string profileID { get; set; }
    }

    public class DeleteImage
    {
        public string typeID { get; set; }
        public string grpID { get; set; }
        public string type { get; set; }
    }

    public class GroupInfo
    {
        public string grpID { get; set; }
        public string moduleID { get; set; }
        public string SearchText { get; set; }
    }


    public class getGroupInfo
    {
        public string grpId { get; set; }
        public string grpName { get; set; }
        public string grpImage { get; set; }
        public string grpType { get; set; }
        public string grpCategory { get; set; }
        public string grpCategoryName { get; set; }

        public string addrss1 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string pincode { get; set; }
        public string country { get; set; }
        public string countryName { get; set; }

        public string emailid { get; set; }
        public string mobile { get; set; }
        public string website { get; set; }
    }

    public class suggestFeature
    {
        public string title { get; set; }
        public string description { get; set; }
        public string profileID { get; set; }
        public string grpId { get; set; }
    }

    public class suggestFeatureResult
    {
        public string adminName { get; set; }
        public string emailID { get; set; }
        public string groupName { get; set; }
        public string suggestedBy { get; set; }
        public string grpId { get; set; }
        public string usercontact { get; set; }
        public string userEmail { get; set; }
    }

    public class EntityInfo
    {
        public string enname { get; set; }
        public string descptn { get; set; }
    }

    public class AdminInfo
    {
        public string adminId { get; set; }
        public string memberName { get; set; }
        public string memberMobile { get; set; }
        public string email { get; set; }
    }

    public class GroupListResult
    {
        public string grpId { get; set; }
        public string grpName { get; set; }
        public string grpImg { get; set; }
        public string grpProfileId { get; set; }
        public string myCategory { get; set; }
        public string isGrpAdmin { get; set; }
        //public string encryptGrpKey { get; set; }
        public List<GroupModulesList> ModuleList { get; set; }
    }

    public class GroupListSync
    {
        public List<GroupResult> NewGroupList { get; set; }
        public List<GroupResult> UpdatedGroupList { get; set; }
        public List<GroupResult> DeletedGroupList { get; set; }
    }

    public class ModuleListSync
    {
        public List<GroupModulesList> NewModuleList { get; set; }
        public List<GroupModulesList> UpdatedModuleList { get; set; }
        public List<GroupModulesList> DeletedModuleList { get; set; }
    }

    public class GroupModulesSyncResult
    {
        public GroupListSync GroupList { get; set; }
        public ModuleListSync ModuleList { get; set; }
    }

    public class ExternalLink
    {
        public string description { get; set; }
        public string link { get; set; }
        public string lableText { get; set; }
    }

    public class ClubHistory
    {
        public string clubName { get; set; }
        public string description { get; set; }
    }

    #region Rotary Library

    /// <summary>
    /// Rotary library output model class
    /// </summary>
    public class clsGetRotaryLibraryOutput
    {
        public string title { get; set; }
        public string description { get; set; }
    }

    #endregion

    #region Club Details

    /// <summary>
    /// Club Details output model class
    /// </summary>
    public class ClsGetClubDetailsOutput
    {
        public string ClubName { get; set; }
        public string MeetingPlace { get; set; }
        public string MeetingDay { get; set; }
        public string MeetingTime { get; set; }
        public string ClubID { get; set; }
        public string DistrictNumber { get; set; }
    }

    #endregion

    #region Feedback

    public class FeedbackInput
    {
        public string ProfileId { get; set; }
        public string Feedback { get; set; }
    }

    #endregion
}