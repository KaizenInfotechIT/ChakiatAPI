using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class FamilyMemberDetail
    {
        public string familyMemberId { get; set; }
        public string memberName { get; set; }
        public string relationship { get; set; }
        public string dOB { get; set; }
        public string emailID { get; set; }
        public string anniversary { get; set; }
        public string contactNo { get; set; }
        public string particulars { get; set; }
        public string bloodGroup { get; set; }
    }

    public class UpdateFamilyDetail
    {
        public string familyMemberId { get; set; }
        public string memberName { get; set; }
        public string relationship { get; set; }
        public string dOB { get; set; }
        public string anniversary { get; set; }
        public string emailID { get; set; }
        public string contactNo { get; set; }
        public string particulars { get; set; }
        public string bloodGroup { get; set; }
        public string profileID { get; set; }
    }

    public class FamilyMemberDetails
    {
        public string isVisible { get; set; }
        // public List<FamilyMemberDetail> familyMemberDetail { get; set; }
        public List<FamilyMemberDetail> familyMemberDetail { get; set; }
    }

    public class MemberDetail
    {
        public string masterID { get; set; }
        public string grpID { get; set; }
        public string profileID { get; set; }
        public string isAdmin { get; set; }
        public string memberName { get; set; }
        public string memberEmail { get; set; }
        public string memberMobile { get; set; }
        public string memberCountry { get; set; }
        public string profilePic { get; set; }
        public string isPersonalDetVisible { get; set; }
        public string isBusinDetVisible { get; set; }
        public string isFamilDetailVisible { get; set; }
        public List<object> familyMemberDetails { get; set; }
        public List<object> addressDetails { get; set; }
        public List<object> personalMemberDetails { get; set; }
        public List<object> BusinessMemberDetails { get; set; }
    }

    public class AddressResult
    {
        public string addressID { get; set; }
        public string addressType { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string pincode { get; set; }
        public string phoneNo { get; set; }
        public string fax { get; set; }
        public string profileID { get; set; }
    }

    public class AddressResultInput
    {
        public string addressID { get; set; }
        public string addressType { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string pincode { get; set; }
        public string phoneNo { get; set; }
        public string fax { get; set; }
        public string profileID { get; set; }
        public string groupID { get; set; }
    }

    public class AddressList
    {
        public string isResidanceAddrVisible { get; set; }
        public string isBusinessAddrVisible { get; set; }
        //public List<AddressResult> addressResult { get; set; }
        public List<AddressResult> addressResult { get; set; }
    }

    public class MemberPro
    {
        public string memberProfileId { get; set; }
        public string groupId { get; set; }
        public string date { get; set; }
    }

    public class CelebrationList
    {
        public string bdayID { get; set; }
        public string bdayName { get; set; }
        public string bdayDate { get; set; }
        public string userPic { get; set; }
        public string isAdmin { get; set; }
        public string celebrationType { get; set; }
        public string userMobile { get; set; }
        public string userEmail { get; set; }
    }

    public class CelebrationListResult
    {
        public CelebrationList CelebrationList { get; set; }
    }

    public class UserLogin
    {
        public string masterUID { get; set; }
        public string memberName { get; set; }
        public string profilePicPath { get; set; }
        public string memberEmailId { get; set; }
        public string mobileNo { get; set; }
        public string imeiNo { get; set; }
        public string deviceToken { get; set; }
        public string deviceName { get; set; }
        public string countryCode { get; set; }
        public string versionNo { get; set; }
        public string loginType { get; set; }//new parameter Added by Nandu on 06-04-2017 (0 - member,1 - family member)
    }

    public class MemberLogin
    {
        public string masterUID { get; set; }
        public string imeiNo { get; set; }
        public string updatedOn { get; set; }
        public string loginType { get; set; }
        public string mobileNo { get; set; }
        public string countryCode { get; set; }
    }

    public class MemberSearch
    {
        public string masterUID { get; set; }
        public string grpID { get; set; }
        public string searchText { get; set; }
        public string updatedOn { get; set; }
        public string page { get; set; }
        public string isFirst { get; set; }
        //Modified By Rupali on 17 nov 2016
        public string isSubGrpAdmin { get; set; }
        public string profileId { get; set; }
    }

    public class MemberListResult
    {
        public string masterUID { get; set; }
        public string grpID { get; set; }
        public string profileID { get; set; }
        public string groupName { get; set; }
        public string memberName { get; set; }
        public string pic { get; set; }
        public string membermobile { get; set; }

        //New output parameter starts -- added by Nandu on 21/11/2016 task -> offline data DOB, DOA and serach for designation

        public string designation { get; set; }
        public string businessName { get; set; }

        public string DateOfBirthDisplay { get; set; }
        public string DateOfBirth { get; set; }
        public string DateOfAnnDisplay { get; set; }
        public string DateOfAnn { get; set; }

        //New output parameter ends --

        public string isDeleted { get; set; }
        public string grpCount { get; set; }
    }

    public class MemberProfileUpdate
    {
        public string ProfileId { get; set; }
        public string memberMobile { get; set; }
        public string memberName { get; set; }
        public string memberEmailid { get; set; }
        public string ProfilePicPath { get; set; }
        public string ImageId { get; set; }
    }

    public class AddMember
    {
        public string mobile { get; set; }
        public string userName { get; set; }
        public string totalMember { get; set; }
        public string groupId { get; set; }
        public string masterID { get; set; }
        public string countryId { get; set; }
        public string memberEmail { get; set; }
    }

    public class AddMultipleMembers
    {
        public List<AddMember> newmembers { get; set; }
    }

    public class MemberListDetail
    {
        public string memberID { get; set; }
        public string grpID { get; set; }
        public string grpuserID { get; set; }
        public string isAdmin { get; set; }
        public string profilePic { get; set; }
        public string isPersonalDetVisible { get; set; }
        public string isBusinDetVisible { get; set; }
        public string isFamilDetailVisible { get; set; }
        public List<object> personalMemberDetails { get; set; }
        public List<object> familyMemberDetails { get; set; }
        public List<object> businessMemberDetails { get; set; }
        public List<object> addressDetails { get; set; }
    }


    # region Dynamic Fields

    public class PersonalMemberDetil
    {
        public string fieldID { get; set; }
        public string uniquekey { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string colType { get; set; }
        public string isEditable { get; set; }
        public string isVisible { get; set; }
    }

    public class BusinessMemberDetail
    {
        public string fieldID { get; set; }
        public string uniquekey { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string colType { get; set; }
        public string isEditable { get; set; }
        public string isVisible { get; set; }
    }

    public class MemberDetailsDynamicField
    {
        public string masterID { get; set; }
        public string grpID { get; set; }
        public string profileID { get; set; }
        public string isAdmin { get; set; }
        public string memberName { get; set; }
        public string memberEmail { get; set; }
        public string memberMobile { get; set; }
        public string memberCountry { get; set; }
        public string profilePic { get; set; }
        public string familyPic { get; set; }
        public string Branch_Id { get; set; }
        public string Branch_Name { get; set; }
        public string isPersonalDetVisible { get; set; }
        public string member_date_of_birth { get; set; }
        public string member_date_of_wedding { get; set; }
     
        public List<PersonalMemberDetil> personalMemberDetails { get; set; }
        public List<BusinessMemberDetail> businessMemberDetails { get; set; }
        public FamilyMemberDetails familyMemberDetails { get; set; }
        public AddressList addressDetails { get; set; }
    }

    public class MemberDetails
    {
        //public MemberDetail MemberDetail { get; set; }
        public List<MemberDetailsDynamicField> MemberDetail { get; set; }
    }

    public class MemberListSyncResult
    {
        public List<MemberDetailsDynamicField> NewMemberList { get; set; }
        public List<MemberDetailsDynamicField> UpdatedMemberList { get; set; }
        public string DeletedMemberList { get; set; }
    }

    public class masterlistResult
    {
        public List<Branchlist> Branchlist { get; set; }
        public List<Designationlist> Designationlist { get; set; }
        public List<Entitylist> Entitylist { get; set; }
        public List<Functional_Role> Functional_Rolelist { get; set; }
        public List<Department> Departmentlist { get; set; }
        public List<Country> Countrylist { get; set; }
    }

    public class Branchlist
    {
        public string pk_Branch_Id { get; set; }
        public string Branch_Name { get; set; }        
    }

    public class Designationlist
    {
        public string pk_designation_id { get; set; }
        public string Designation_Name { get; set; }
    }
    public class Entitylist
    {
        public string pk_EntityName_Id { get; set; }
        public string Entity_Name { get; set; }
    }
    public class Functional_Role
    {
        public string pk_functionalrole_id { get; set; }
        public string functionalrole_Name { get; set; }
    }
    public class Department
    {
        public string pk_department_id { get; set; }
        public string department_Name { get; set; }
    }
    public class Country
    {
        public string country_master_id { get; set; }
        public string country_master_name { get; set; }
    }

    public class SearchFilterInput
    {
        public string groupId { get; set; }
    }

    public class SearchFilters
    {
        public string filterID { get; set; }
        public string fieldID { get; set; }
        public string dbColumnName { get; set; }
        public string displayName { get; set; }
        public string ColType { get; set; }
        public string fieldType { get; set; }
        public string value { get; set; }
    }

    public class AdvanceSearchInput
    {
        public string groupID { get; set; }
        public List<SearchFilters> GroupFilters { get; set; }
    }

    public class DeleteFolderInput
    {
        public string folderPath { get; set; }
    }

    #endregion

    # region commented classes
    //public class AddMembers
    //{
    //    public List<AddMember> AddMember { get; set; }
    //}
    //  public class Add
    //public class PersonalMemberDetails
    //{
    //    public PersonalMemberDetil PersonalMemberDetil { get; set; }
    //}
    //public class BusinessMemberDetails
    //{
    //    public BusinessMemberDetail BuisnessMemberDetil { get; set; }
    //}
    //public class TBGroupResult
    //{
    //    public List<CelebrationList> CelebrationList { get; set; }
    //}
    # endregion

    public class UpdatePersonalDetails
    {
        public string key { get; set; }
        public string profileID { get; set; }
    }

    public class MemberDirectory
    {
        public List<MemberListResult> newMembers { get; set; }
        public List<MemberListResult> updatedMembers { get; set; }
        public List<MemberListResult> deletedMembers { get; set; }
    }

    public class GrpNotificationCount
    {
        public string groupId { get; set; }
        public string totalCount { get; set; }
        public string groupCategory { get; set; }
        public List<ModNotificationCount> ModCount { get; set; }
    }

    public class ModNotificationCount
    {
        public string moduleId { get; set; }
        public string count { get; set; }
    }

    public class BODListResult
    {
        public string masterUID { get; set; }
        public string grpID { get; set; }
        public string profileID { get; set; }
        public string memberName { get; set; }
        public string membermobile { get; set; }
        public string MemberDesignation { get; set; }
        public string pic { get; set; }
    }

    public class DistrictCommitteeResult
    {
        public string masterUID { get; set; }
        public string grpID { get; set; }
        public string profileID { get; set; }
        public string memberName { get; set; }
        public string membermobile { get; set; }
        public string MemberDesignation { get; set; }
        public string clubName { get; set; }
        public string pic { get; set; }
    }

    public class BODInput
    {
        public string grpId { get; set; }
        public string searchText { get; set; }
    }

    /// <summary>
    /// Created by : Nandu
    /// Created On : 24-04-2017
    /// </summary>
    public class RegisterMail
    {
        public string MobileNo { get; set; }
        //public string IsRotarian { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        //public string Club { get; set; }
        //public string Feedback { get; set; }
    }

    /// <summary>
    /// Created on : 02-05-2017
    /// Created By : Nandu
    /// Task : Model classes for profile update in one click
    /// </summary>
    /// 

    #region

    public class UpdatePersonalMemberDetails
    {
        public string memberName { get; set; }
        public string memberEmail { get; set; }
        public string secondaryMobileNo { get; set; }
        public string memberDOB { get; set; }
        public string memberDOA { get; set; }
        public string profilePic { get; set; }
        public string bloodGroup { get; set; }

        public string classification { get; set; }
        public string keywords { get; set; }
        public string rotaryId { get; set; }
        public string clubDesignation { get; set; }
        public string districtDesignation { get; set; }
        public string donarRecognition { get; set; }
    }

    public class UpdateBusinessMemberDetail
    {
        public string fieldID { get; set; }
        public string uniquekey { get; set; }
        public string value { get; set; }
    }

    public class UpdateFamilyMemberDetail
    {
        public string familyMemberId { get; set; }
        public string memberName { get; set; }
        public string relationship { get; set; }
        public string emailID { get; set; }
        public string memberDOB { get; set; }
        public string memberDOA { get; set; }
        public string countryID { get; set; }
        public string contactNo { get; set; }
        public string particulars { get; set; }
        public string bloodGroup { get; set; }
    }

    public class UpdateAddressDetail
    {
        public string addressID { get; set; }
        public string addressType { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string pincode { get; set; }
        public string phoneNo { get; set; }
        public string fax { get; set; }
    }

    public class UpdateDynamicField
    {
        public string fieldID { get; set; }
        public string uniquekey { get; set; }
        public string value { get; set; }
    }

    public class RootObject
    {
        public string profileID { get; set; }
        public string grpID { get; set; }
        public string updatedOn { get; set; }

        //public string memberName { get; set; }
        //public string memberEmail { get; set; }
        //public string memberMobile { get; set; }
        //public string memberCountry { get; set; }
        //public string profilePic { get; set; }
        public UpdatePersonalMemberDetails personalMemberDetails { get; set; }
        public List<UpdateBusinessMemberDetail> businessMemberDetails { get; set; }
        public List<UpdateFamilyMemberDetail> familyMemberDetail { get; set; }
        public string deletedFamilyMemberIds { get; set; }
        public List<UpdateAddressDetail> addressDetails { get; set; }
        public List<UpdateDynamicField> dynamicFields { get; set; }
    }

    #endregion

    #region
    /// <summary>
    /// Added by Madhavi Patil(23 Feb 2018)
    /// </summary>
    public class DistrictMemListInput
    {
        public string masterUID { get; set; }
        public string grpID { get; set; }
        public string searchText { get; set; }
        public string pageNo { get; set; }
        public string recordCount { get; set; }
        public string classification { get; set; }
        
    }
    public class DistrictMemListResult
    {
        public string masterUID { get; set; }
        public string grpID { get; set; }
        public string profileID { get; set; }
        public string memberName { get; set; }
        public string pic { get; set; }
        public string membermobile { get; set; }
        public string club_name { get; set; }
    }
    public class classificationOutput
    {
        public string RowNo { get; set; }
        public string classification { get; set; }
    }

    
    #endregion


    public class GrpPartResult
    {
        public int grpId { get; set; }
        public string grpName { get; set; }
    }

    public class grpPartResults
    {
        public GrpPartResult GrpPartResult { get; set; }
    }

    public class WelcomeResult
    {
        public string status { get; set; }
        public string message { get; set; }
        public string Name { get; set; }
        public List<grpPartResults> grpPartResults { get; set; }
    }

    public class RootObject1
    {
        public WelcomeResult WelcomeResult { get; set; }
    }
}
