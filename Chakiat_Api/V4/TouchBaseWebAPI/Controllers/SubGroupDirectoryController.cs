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
    public class SubGroupDirectoryController : ApiController
    {
        //============================= Get Sub Group directory ========================================================
        [System.Web.Http.HttpPost]
        public object GetSubGrpDirectoryList(SubGrpDirectoryInput member)
        {
            dynamic TBSubDirectoryResult;
            try
            {
                var result = SubGroupDirectory.GetSubGroupDirectory(member);

                if (result == null)
                {
                    TBSubDirectoryResult = new { status = "0", message = "Record Not Found" };
                }
                else
                {
                    TBSubDirectoryResult = new { status = "0", message = "success", result };
                }
            }
            catch
            {
                TBSubDirectoryResult = new { status = "1", message = "Failed" };
            }
            return TBSubDirectoryResult;
        }
  
        [System.Web.Http.HttpPost]
        public object GetSubGroupMemberList(SubGrpDirectoryInput member)
        {
            dynamic TBSubGrpMemberResult;
            try
            {
                var result = SubGroupDirectory.GetSubGroupMemberList(member);

                if (result == null)
                {
                    TBSubGrpMemberResult = new { status = "0", message = "Record Not Found" };
                }
                else
                {
                    TBSubGrpMemberResult = new { status = "0", message = "success", result };
                }
            }
            catch
            {
                TBSubGrpMemberResult = new { status = "1", message = "Failed" };
            }
            return TBSubGrpMemberResult;
        }
    }
}
