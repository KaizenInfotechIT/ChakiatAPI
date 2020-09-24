using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class clsServiceDirectory
    {
    }

    // Add member to Service Directory Parameters
    public class AddServiceDirectory
    {
        public string serviceId { get; set; }
        public string groupId { get; set; }
        public string moduleId { get; set; } // Added by Nandu on 30/09/2016 Task--> Module replica 

        public string categoryId { get; set; } // Added by Nandu on 20/03/2017 Task--> As per Req. Filter list as per Category 

        public string memberName { get; set; }
        public string description { get; set; }
        public string image { get; set; }

        public string countryCode1 { get; set; }
        public string mobileNo1 { get; set; }
        public string countryCode2 { get; set; }
        public string mobileNo2 { get; set; }
        public string paxNo { get; set; }
        public string email { get; set; }

        public string keywords { get; set; }

        public string address { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }

        public string createdBy { get; set; }

        public string addressCountry { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipcode { get; set; }
        
        public string updatedOn { get; set; }

        public string website { get; set; }// Added by Nandu on 01/03/2017 Task--> New parameter as per requirement
    }

    // Service Directory List Search Parameters
    public class ServiceDirectorySearch
    {
        public string groupId { get; set; }
        public string moduleId { get; set; } // Added by Nandu on 30/09/2016 Task--> Module replica 
        public string searchText { get; set; }
        public string updatedOn { get; set; }
    }

    public class ServiceDirector
    {
        public List<ServiceDirectoryList> newMembers { get; set; }
        public List<ServiceDirectoryList> updatedMembers { get; set; }
        public List<ServiceDirectoryList> deletedMembers { get; set; }
    }

    // Service Directory Detail member Parameters
    public class ServiceDirDetail
    {
        public string serviceDirId { get; set; }
        public string groupId { get; set; }
    }

    // Add member to Service Directory Parameters
    public class ServiceDirectoryList
    {
        public string serviceDirId { get; set; }
        public string groupId { get; set; }
        public string moduleId { get; set; } // Added by Nandu on 30/09/2016 Task--> Module replica 

        public string categoryId { get; set; } // Added by Nandu on 20/03/2017 Task--> As per Req. Filter list as per Category 

        public string memberName { get; set; }
        public string image { get; set; }
        public string contactNo { get; set; }
        public string isdeleted { get; set; }

        public string descriptn { get; set; }
        public string contactNo2 { get; set; }
        public string pax_no { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }

        public string countryId1 { get; set; }
        public string countryId2 { get; set; }

        public string countryCode1 { get; set; }
        public string countryCode2 { get; set; }

        public string keywords { get; set; }

        public string addressCountry { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipCode { get; set; }

        public string website { get; set; }// Added by Nandu on 01/03/2017 Task--> New parameter as per requirement
    }

    // Detail Member Parameters
    public class ServiceDirectoryDetail
    {
        public string serviceMemberName { get; set; }
        public string serviceDescription { get; set; }

        public string serviceImage { get; set; }
        public string serviceThumbimage { get; set; }

        public string serviceCountry1 { get; set; }
        public string serviceCountry2 { get; set; }

        public string serviceMobile1 { get; set; }
        public string serviceMobile2 { get; set; }
        public string servicePaxNo { get; set; }
        public string serviceEmail { get; set; }

        public string serviceKeywords { get; set; }
        public string serviceAddress { get; set; }
        public string serviceLatitude { get; set; }
        public string serviceLongitude { get; set; }
    }


    public class ServiceCategoryList
    {
        public string ID { get; set; }
        public string CategoryName { get; set; }
        public string TotalCount { get; set; }
    }


    #region Version 3 API's

    public class ClsSeviceCategoryData
    {
        public List<ServiceCategoryList> Category { get; set; }
        public List<ServiceDirectoryList> DirectoryData { get; set; }
    }

    #endregion
}