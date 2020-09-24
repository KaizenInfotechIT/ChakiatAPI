using System;
using System.Collections.Generic;
using System.Linq;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;


namespace TouchBaseWebAPI.BusinessEntities
{
    public class ServiceDirectory
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static List<ServiceDirectoryList> GetServiceDirectoryList(ServiceDirectorySearch search)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[3];

                param[0] = new MySqlParameter("@grpId", search.groupId);
                param[1] = new MySqlParameter("@moduleId", search.moduleId); // Added by Nandu on 30/09/2016 Task--> Module replica 
                param[2] = new MySqlParameter("@updateOn", search.updatedOn);

                //var Result = _DBTouchbase.ExecuteStoreQuery<ServiceDirectoryList>("CALL V3_USPGetServiceDirectoryList(?grpId,?updateOn)", param).ToList();
                var Result = _DBTouchbase.ExecuteStoreQuery<ServiceDirectoryList>("CALL V6_USPGetServiceDirectoryList(?grpId,?moduleId,?updateOn)", param).ToList(); // Added by Nandu on 30/09/2016 Task--> Module replica 

                foreach (ServiceDirectoryList servDir in Result)
                {
                    if (!string.IsNullOrEmpty(servDir.image))
                    {
                        string profile_Image = servDir.image.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/servicedirectory/Group" + search.groupId + "/thumb/";
                        servDir.image = path + profile_Image;
                    }
                }

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        // For Offline data fetch
        public static ServiceDirector GetServiceDirectoryListSync(ServiceDirectorySearch search)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[3];
                param[0] = new MySqlParameter("@grpId", search.groupId);
                param[1] = new MySqlParameter("@moduleId", search.moduleId); // Added by Nandu on 30/09/2016 Task--> Module replica 
                param[2] = new MySqlParameter("@updateOn", search.updatedOn);                

                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V6_USPGetServiceDirectoryListSync", param); // Added by Nandu on 30/09/2016 Task--> Module replica 

                DataTable dtNewServiceMember = result.Tables[0];
                DataTable dtUpdatedServiceMember = result.Tables[1];
                DataTable dtDeletedServiceMember = result.Tables[2];

                List<ServiceDirectoryList> NewMemberList = new List<ServiceDirectoryList>();
                if (dtNewServiceMember.Rows.Count > 0)
                {
                    NewMemberList = GlobalFuns.DataTableToList<ServiceDirectoryList>(dtNewServiceMember);

                    foreach (ServiceDirectoryList sev in NewMemberList)
                    {
                        if (!string.IsNullOrEmpty(sev.image))
                        {
                            string ImageName = sev.image.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/servicedirectory/Group" + sev.groupId + "/thumb/";
                            sev.image = path + ImageName;
                        }
                    }
                }

                List<ServiceDirectoryList> updatedMemberList = new List<ServiceDirectoryList>();
                if (dtUpdatedServiceMember.Rows.Count > 0)
                {
                    updatedMemberList = GlobalFuns.DataTableToList<ServiceDirectoryList>(dtUpdatedServiceMember);

                    foreach (ServiceDirectoryList sev in updatedMemberList)
                    {
                        if (!string.IsNullOrEmpty(sev.image))
                        {
                            string ImageName = sev.image.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/servicedirectory/Group" + sev.groupId + "/thumb/";
                            sev.image = path + ImageName;
                        }
                    }
                }

                List<ServiceDirectoryList> deletedMemberList = new List<ServiceDirectoryList>();
                if (dtDeletedServiceMember.Rows.Count > 0)
                {
                    deletedMemberList = GlobalFuns.DataTableToList<ServiceDirectoryList>(dtDeletedServiceMember);
                }

                ServiceDirector serviceList = new ServiceDirector();

                serviceList.deletedMembers = deletedMemberList;
                serviceList.newMembers = NewMemberList;
                serviceList.updatedMembers = updatedMemberList;

                return serviceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ServiceDirectoryDetail> GetServiceDirDetails(ServiceDirDetail serv)
        {
            try
            {
                var serviceDirId = new MySqlParameter("?serviceDirId", serv.serviceDirId);

                var Result = _DBTouchbase.ExecuteStoreQuery<ServiceDirectoryDetail>("CALL V3_USPGetServiceDirectoryDetail(?serviceDirId)", serviceDirId).ToList();

                foreach (ServiceDirectoryDetail service in Result)
                {

                    if (!string.IsNullOrEmpty(service.serviceImage))
                    {
                        string profile_Image = service.serviceImage.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/servicedirectory/Group" + serv.groupId + "/";

                        service.serviceImage = path + profile_Image;//actual image URL
                        service.serviceThumbimage = path + "thumb/" + profile_Image;//thumb image URL
                    }
                    else
                    {
                        service.serviceThumbimage = "";
                    }
                }

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Imgname createService(AddServiceDirectory service)
        {
            try
            {
                var serviceId = new MySqlParameter("?p_service_id", service.serviceId);
                var groupId = new MySqlParameter("?p_group_id", service.groupId);
                var moduleId = new MySqlParameter("?p_module_id", service.moduleId); // Added by Nandu on 30/09/2016 Task--> Module replica 

                var memberName = new MySqlParameter("?p_member_name", string.IsNullOrEmpty(service.memberName) ? "" : service.memberName);
                var description = new MySqlParameter("?p_description", string.IsNullOrEmpty(service.description) ? "" : service.description);
                var image = new MySqlParameter("?p_image", string.IsNullOrEmpty(service.image) ? "0" : service.image);

                var countryCode1 = new MySqlParameter("?p_country_code1", string.IsNullOrEmpty(service.countryCode1) ? "" : service.countryCode1);
                var mobileNo1 = new MySqlParameter("?p_mobile_no1", string.IsNullOrEmpty(service.mobileNo1) ? "" : service.mobileNo1);
                var countryCode2 = new MySqlParameter("?p_country_code2", string.IsNullOrEmpty(service.countryCode2) ? "" : service.countryCode2);
                var mobileNo2 = new MySqlParameter("?p_mobile_no2", string.IsNullOrEmpty(service.mobileNo2) ? "" : service.mobileNo2);
                var paxNo = new MySqlParameter("?p_pax_no", string.IsNullOrEmpty(service.paxNo) ? "" : service.paxNo);
                var email = new MySqlParameter("?p_email", string.IsNullOrEmpty(service.email) ? "" : service.email);

                var keywords = new MySqlParameter("?p_keywords", string.IsNullOrEmpty(service.keywords) ? "" : service.keywords);

                var address = new MySqlParameter("?p_address", string.IsNullOrEmpty(service.address) ? "" : service.address);
                var latitude = new MySqlParameter("?p_latitude", string.IsNullOrEmpty(service.latitude) ? "" : service.latitude);
                var longitude = new MySqlParameter("?p_longitude", string.IsNullOrEmpty(service.longitude) ? "" : service.longitude);

                var createdBy = new MySqlParameter("?p_created", string.IsNullOrEmpty(service.createdBy) ? "" : service.createdBy);

                var addressCode = new MySqlParameter("?p_address_code", string.IsNullOrEmpty(service.addressCountry) ? "" : service.addressCountry);
                var city = new MySqlParameter("?p_city", string.IsNullOrEmpty(service.city) ? "" : service.city);
                var state = new MySqlParameter("?p_state", string.IsNullOrEmpty(service.state) ? "" : service.state);
                var zipcode = new MySqlParameter("?p_zipcode", string.IsNullOrEmpty(service.zipcode) ? "" : service.zipcode);

                var website = new MySqlParameter("?p_website", string.IsNullOrEmpty(service.website) ? "" : service.website);
                var category = new MySqlParameter("?p_category", string.IsNullOrEmpty(service.categoryId) ? "0" : service.categoryId);
                
                // Added by Nandu on 30/09/2016 Task--> Module replica 
                var Result = _DBTouchbase.ExecuteStoreQuery<Imgname>("CALL V7_USPAddServiceDirectory(?p_created, ?p_service_id, ?p_group_id, ?p_module_id, ?p_member_name, ?p_description, ?p_image, ?p_country_code1, ?p_mobile_no1, ?p_country_code2, ?p_mobile_no2, ?p_pax_no, ?p_email, ?p_keywords, ?p_address, ?p_latitude, ?p_longitude, ?p_address_code, ?p_city, ?p_state, ?p_zipcode, ?p_website, ?p_category )",
                                                                     createdBy, serviceId, groupId, moduleId, memberName, description, image, countryCode1, mobileNo1, countryCode2, mobileNo2, paxNo, email, keywords, address, latitude, longitude, addressCode, city, state, zipcode, website, category).SingleOrDefault();

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

        public static List<ServiceCategoryList> GetServiceDirCategoriesList(ServiceDirectorySearch search)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];

                param[0] = new MySqlParameter("@grpId", search.groupId);
                param[1] = new MySqlParameter("@moduleId", search.moduleId); // Added by Nandu Task--> Module replica

                var Result = _DBTouchbase.ExecuteStoreQuery<ServiceCategoryList>("CALL V7_GetServiceCategoriesList(?grpId,?moduleId)", param).ToList(); // Added by Nandu Task--> Module replica 
                
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}