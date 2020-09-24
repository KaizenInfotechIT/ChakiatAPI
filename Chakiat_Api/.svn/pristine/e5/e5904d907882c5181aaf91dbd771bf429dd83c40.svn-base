using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Configuration;
using TouchBaseWebAPI.Data;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;
using System.Web;
using System.IO;
using System.Drawing.Imaging;
using System.Text;

namespace TouchBaseWebAPI.Controllers
{
    public class DistrictController : ApiController
    {
        [HttpPost]
        public object GetMemberWithDynamicFields(MemberPro member)
        {

            dynamic MemberListDetailResult;
            try
            {
                MemberDetailsDynamicField result = District.GetMemberDtlWithDynamicFeild(member.memberProfileId, member.groupId);
                if (result != null)
                {
                    MemberListDetailResult = new { status = "0", message = "sucess", MemberDetails = result };
                }
                else
                {
                    MemberListDetailResult = new { status = "1", message = "User Not Found" };
                }
            }
            catch
            {
                MemberListDetailResult = new { status = "1", message = "An error occured. Please contact Administrator" };
            }
            return new
            {
                MemberListDetailResult
            };
        }

        [HttpPost]
        public object GetDistrictMemberListSync(MemberSearch member)
        {
            dynamic MemberDirectorResult;
            try
            {
                string zipFilePath = "";
                DateTime currentDate = DateTime.Now;
                MemberListSyncResult MemberDetail = new MemberListSyncResult();

                if (Convert.ToDateTime(member.updatedOn).Date.ToString("dd/MM/yyyy") == "01/01/1970")
                {
                    zipFilePath = District.GetZipFilePath(member.updatedOn, member.grpID, out currentDate);
                }

                if (Convert.ToDateTime(member.updatedOn).Date.ToString("dd/MM/yyyy") != "01/01/1970" || string.IsNullOrEmpty(zipFilePath))
                {
                    MemberDetail = District.GetMemberListSync(member.updatedOn, member.grpID, out zipFilePath);
                }

                if (!string.IsNullOrEmpty(zipFilePath))
                {
                    MemberDirectorResult = new { status = "0", message = "success", curDate = currentDate.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, MemberDetail };
                }
                else if (MemberDetail.NewMemberList.Count == 0 && MemberDetail.UpdatedMemberList.Count == 0 && string.IsNullOrEmpty(MemberDetail.DeletedMemberList))
                {
                    MemberDirectorResult = new { status = "2", message = "No New Updates", curDate = currentDate, zipFilePath, MemberDetail };
                }
                else
                {
                    MemberDirectorResult = new { status = "0", message = "success", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, MemberDetail };
                }
            }
            catch (Exception ex)
            {
                MemberDirectorResult = new { status = "1", error = ex.ToString(), message = "An error occured. Please contact Administrator", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
            }
            return MemberDirectorResult;
        }

        [HttpPost]
        public object GetDistrictMemberList(DistrictMemListInput obj)
        {
           dynamic MemberDirectoryResult;
           int totalPages=0;
           try
           {
               List<DistrictMemListResult> Result = District.GetDistrictMemberList(obj, out totalPages);
               if (Result != null)
               {
                   MemberDirectoryResult = new { status = "0", message = "success", resultCount = Result.Count.ToString(), TotalPages = totalPages.ToString(), currentPage = obj.pageNo.ToString(), Result };
               }
               else
               {
                   MemberDirectoryResult = new { status = "1", message = "falied" };
               }
           }
           catch (Exception ex)
           {
               MemberDirectoryResult = new { status = "1", message = "falied" , ex.Message };
           }
           return MemberDirectoryResult;
        }

        [HttpPost]
        public object GetClubs(ClubListInput cls)
        {
            dynamic ClubListResult;
            try
            {
                List<Club> result = District.GetDistrictClubs(cls.GroupId, cls.search);
                if (result != null)
                {
                    ClubListResult = new { status = "0", message = "sucess", Clubs = result };
                }
                else
                {
                    ClubListResult = new { status = "1", message = "Clubs not found" };
                }
            }
            catch
            {
                ClubListResult = new { status = "1", message = "An error occured. Please contact Administrator" };
            }
            return new
            {
                ClubListResult
            };
        }

        [HttpPost]
        public object GetDistrictCommittee(BODInput list)
        {
            dynamic TBGetBODResult;
            try
            {
                List<DistrictCommitteeResult> Result = District.GetDistrictCommittee(list.grpId, list.searchText);


                if (Result != null)
                {
                    TBGetBODResult = new { status = "0", message = "success", DistrictCommitteeResult = Result };
                }
                else
                {
                    TBGetBODResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetBODResult = new { status = "1", message = "failed" };
            }

            return new { TBGetBODResult };
        }

        [HttpPost]
        public object GetDistrictSubGroupList(SubGroup sub)
        {
            dynamic TBGetDistSubGroupListResult;
            List<SubGroupList> Result = new List<SubGroupList>();
            try
            {
                Result = District.GetDistrictSubGroup(sub.groupId, sub.subGroupType);
                if (Result != null)
                {
                    TBGetDistSubGroupListResult = new { status = "0", message = "success", SubGroupResult = Result };
                }
                else
                {
                    TBGetDistSubGroupListResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetDistSubGroupListResult = new { status = "1", message = "failed" };
            }

            return new { TBGetDistSubGroupListResult };
        }

        [HttpPost]
        public object GetClassificationList(DistrictMemListInput obj)
        {
            dynamic ClassificationListResult;
            List<classificationOutput> Result = new List<classificationOutput>();
            int totalPages = 0;
            try
            {
                Result = District.GetClassificationList(obj,out totalPages);
                if (Result != null)
                {
                    ClassificationListResult = new { status = "0", message = "success", resultCount = Result.Count.ToString(), TotalPages = totalPages.ToString(), currentPage = obj.pageNo.ToString(), Result };
                }
                else
                {
                    ClassificationListResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                ClassificationListResult = new { status = "1", message = "failed" };
            }
            return new { ClassificationListResult };
        }

        [HttpPost]
        public object GetMemberByClassification(DistrictMemListInput cls)
        {
            dynamic MemberListResult;
            List<DistrictMemListResult> Result = new List<DistrictMemListResult>();
          
            try
            {
                Result = District.GetMemberByClassification(cls.classification,cls.grpID);
                if (Result != null)
                {
                    MemberListResult = new { status = "0", message = "success",  Result };
                }
                else
                {
                    MemberListResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                MemberListResult = new { status = "1", message = "failed" };
            }
            return new { MemberListResult };
        }
    }
}
