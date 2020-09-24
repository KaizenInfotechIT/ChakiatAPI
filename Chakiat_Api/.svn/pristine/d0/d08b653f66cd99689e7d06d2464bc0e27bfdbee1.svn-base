using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;

namespace TouchBaseWebAPI.Controllers
{
    public class OfflineDataController : ApiController
    {
        [HttpPost]
        public object GetDirectoryListSync(MemberSearch member)
        {
            dynamic MemberDirectorResult;
            List<object> MemberListResult = new List<object>();
          
            if (string.IsNullOrEmpty(member.masterUID))
            {
                member.masterUID = "0";
            }
           
            try
            {
                MemberDirectory Result = MemberMaster.GetDirectoryListSync(Convert.ToInt32(member.masterUID), Convert.ToInt32(member.grpID), member.updatedOn);
                MemberDirectorResult = new { status = "0", message = "success",curDate=System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Result };
            }
            catch
            {
                MemberDirectorResult = new { status = "1", message = "An error occured. Please contact Administrator",curDate=System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
            }
            return new
            {
                MemberDirectorResult
            };
        }

        [HttpPost]
        public object GetServiceDirectoryListSync(ServiceDirectorySearch serv)
        {
            dynamic TBServiceDirectoryResult;
            List<object> ServiceDirectoryResult = new List<object>();

            try
            {
                ServiceDirector Result = ServiceDirectory.GetServiceDirectoryListSync(serv);

                TBServiceDirectoryResult = new { status = "0", message = "success", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Result };
            }
            catch
            {
                TBServiceDirectoryResult = new { status = "1", message = "An error occured. Please contact Administrator" };
            }
            return new
            {
                TBServiceDirectoryResult
            };
        }
    }
}
