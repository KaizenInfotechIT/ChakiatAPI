using System.Collections.Generic;
using System.Web.Http;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;

namespace TouchBaseWebAPI.Controllers
{
    /// <summary>
    /// Created By : Nandu
    /// Created On : 25-04-2017
    /// </summary>
    public class WebLinkController : ApiController
    {
        [System.Web.Http.HttpPost]
        public object AddUpdateWebLinks(clsAddWebLinkInput obj)
        {
            dynamic TBAddUpdateWeblinkResult;

            try
            {
                int result = WebLinks.addUpdateWebLink(obj);

                if (result > 0)
                {
                    TBAddUpdateWeblinkResult = new { status = "0", message = "success" };
                }
                else
                {
                    TBAddUpdateWeblinkResult = new { status = "1", message = "failed" };
                }
            }
            catch
            {
                TBAddUpdateWeblinkResult = new { status = "1", message = "failed" };
            }

            return new { TBAddUpdateWeblinkResult };
        }

        [HttpPost]
        public object GetWebLinksList(clsGetWebLinkInput searchCriteria)
        {
            dynamic TBGetWebLinkListResult;
            try
            {
                List<clsGetWebLinkOutput> Result = WebLinks.GetWebLinkList(searchCriteria);

                if (Result != null)
                {
                    TBGetWebLinkListResult = new { status = "0", message = "success", WebLinkListResult = Result };
                }
                else
                {
                    TBGetWebLinkListResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetWebLinkListResult = new { status = "1", message = "failed" };
            }

            return new { TBGetWebLinkListResult };
        }
    }
}
