using System;
using System.Collections.Generic;
using System.Web.Http;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;

namespace TouchBaseWebAPI.Controllers
{
    public class ServiceDirectoryController : ApiController
    {
        //API TO Get Entity wise Service Directory List
        [System.Web.Http.HttpPost]
        public object GetServiceDirectoryList(ServiceDirectorySearch service)
        {
            dynamic TBServiceDirectoryResult;
            List<object> ServiceDirectoryResult = new List<object>();

            try
            {
                List<ServiceDirectoryList> Result = ServiceDirectory.GetServiceDirectoryList(service);
                for (int i = 0; i < Result.Count; i++)
                {
                    ServiceDirectoryResult.Add(new { ServiceDirResult = Result[i] });
                }

                if (ServiceDirectoryResult != null)
                {
                    TBServiceDirectoryResult = new { status = "0", message = "success", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), ServiceDirectoryResult };
                }
                else
                {
                    TBServiceDirectoryResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBServiceDirectoryResult = new { status = "1", message = "failed" };
            }

            return new { TBServiceDirectoryResult };
        }


        //API TO Get Entity wise particular Service Directory Detail
        [System.Web.Http.HttpPost]
        public object GetServiceDirectoryDetails(ServiceDirDetail serv)
        {
            dynamic TBServiceDirectoryListResult;
            List<object> ServiceDirectoryListResult = new List<object>();

            try
            {
                //string a = HttpContext.Current.Request.HttpMethod;   
                List<ServiceDirectoryDetail> Result = ServiceDirectory.GetServiceDirDetails(serv);
                for (int i = 0; i < Result.Count; i++)
                {
                    ServiceDirectoryListResult.Add(new { ServiceDirList = Result[i] });
                }

                if (Result != null && Result.Count != 0)
                {
                    TBServiceDirectoryListResult = new { status = "0", message = "success", ServiceDirListResult = ServiceDirectoryListResult };
                }
                else
                {
                    TBServiceDirectoryListResult = new { status = "1", message = "Record not found", ServiceDirListResult = ServiceDirectoryListResult };
                }
            }
            catch
            {
                TBServiceDirectoryListResult = new { status = "1", message = "failed" };
            }

            return new { TBServiceDirectoryListResult };
        }


        //API TO Add New Member in Service Directory
        [System.Web.Http.HttpPost]
        public object AddServiceDirectory(AddServiceDirectory addservice)
        {
            dynamic TBAddServiceResult;
            int str = -1;

            try
            {
                Imgname Result = ServiceDirectory.createService(addservice);

                if (!string.IsNullOrEmpty(Result.imgName))
                {
                    str = GlobalFuns.UploadImage(addservice.groupId, Result.imgName, "servicedirectory");
                }
                else
                    str = 0;

                //if (Result != null)
                //{
                if (str == 0)
                {
                    //TBAddServiceResult = new { status = "0", message = "success" };
                    ServiceDirectorySearch serv = new ServiceDirectorySearch();
                    serv.groupId = addservice.groupId;
                    serv.moduleId = addservice.moduleId;

                    DateTime dt = DateTime.Now;
                    if (addservice.updatedOn != null)
                    {
                        dt = Convert.ToDateTime(addservice.updatedOn);
                        dt = dt.AddSeconds(-1);
                    }

                    serv.updatedOn = dt.ToString("yyyy/MM/dd HH:mm:ss");

                    ServiceDirector res = ServiceDirectory.GetServiceDirectoryListSync(serv);

                    if (res != null)
                    {
                        TBAddServiceResult = new { status = "0", message = "success", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), res };
                    }
                    else
                    {
                        TBAddServiceResult = new { status = "1", message = "failed", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
                    }
                }
                else
                    TBAddServiceResult = new { status = "1", message = "failed" };
                //}
                //else
                //{
                //    TBAddServiceResult = new { status = "0", message = "Record not found" };
                //}
            }
            catch
            {
                TBAddServiceResult = new { status = "1", message = "failed" };
            }

            return new { TBAddServiceResult };
        }


        [System.Web.Http.HttpPost]
        public object GetServiceDirectoryCategories(ServiceDirectorySearch service)
        {
            //dynamic TBServiceCategoriesResult;
            List<object> ServiceCategoriesResult = new List<object>();

            try
            {
                List<ServiceCategoryList> Result = ServiceDirectory.GetServiceDirCategoriesList(service);
                for (int i = 0; i < Result.Count; i++)
                {
                    ServiceCategoriesResult.Add(Result[i]);
                }

                if (ServiceCategoriesResult != null)
                {
                   return new { status = "0", message = "success", ServiceCategoriesResult };
                }
                else
                {
                    return new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                return new { status = "1", message = "failed" };
            }
        }
    }
}
