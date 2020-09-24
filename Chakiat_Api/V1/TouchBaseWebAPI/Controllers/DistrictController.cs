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
                string zipFilePath="";
                MemberListSyncResult MemberDetail = new MemberListSyncResult();

                if (Convert.ToDateTime(member.updatedOn).Date.ToString("dd/MM/yyyy") == "01/01/1970")
                {
                    zipFilePath = District.GetZipFilePath(member.updatedOn, member.grpID);
                }

                if(Convert.ToDateTime(member.updatedOn).Date.ToString("dd/MM/yyyy") != "01/01/1970" ||  string.IsNullOrEmpty(zipFilePath))
                {
                    MemberDetail = District.GetMemberListSync(member.updatedOn, member.grpID, out zipFilePath);
                }

                if (!string.IsNullOrEmpty(zipFilePath))
                {
                    MemberDirectorResult = new { status = "0", message = "success", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, MemberDetail };
                }
                else if (MemberDetail.NewMemberList.Count == 0 && MemberDetail.UpdatedMemberList.Count == 0 && string.IsNullOrEmpty(MemberDetail.DeletedMemberList))
                {
                    MemberDirectorResult = new { status = "2", message = "No New Updates", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, MemberDetail };
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

    }
}
