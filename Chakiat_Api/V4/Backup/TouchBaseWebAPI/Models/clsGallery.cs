using System.Collections.Generic;

namespace TouchBaseWebAPI.Models
{
    #region Album Add Update and Listing

    /// <summary>
    /// Create/Update Album input Json parameters
    /// </summary>
    public class ClsAddUpdateAlbumInput
    {
        public string albumId { get; set; }
        public string groupId { get; set; }

        public string moduleId { get; set; } // Added by Rupali on 3/2/17 Task- Gallery replica

        public string type { get; set; }
        public string memberIds { get; set; }
        public string albumTitle { get; set; }
        public string albumDescription { get; set; }
        public string albumImage { get; set; }
        public string createdBy { get; set; }
        public string isSubGrpAdmin { get; set; }

        public string shareType { get; set; } // Added by Nandu on 7/4/17 Task- Within the Club-0/public-1 mode - By default Within the Club-0

        public int categoryId { get; set; }
        public string dateofproject { get; set; }
        public decimal costofproject { get; set; }
        public string costofprojecttype { get; set; }
        public string beneficiary { get; set; }
        public int manhourspent { get; set; }
        public string manhourspenttype { get; set; }
        public string NumberofRotarian { get; set; }
        public string OtherCategorytext { get; set; }

        public string Attendance { get; set; }
        public string AttendancePer { get; set; }
        public string MeetingType { get; set; }
        public string AgendaDocID { get; set; }
        public string MOMDocID { get; set; }
    }

    /// <summary>
    /// Gallery List Input parameters
    /// </summary>
    public class ClsAlbumListInput
    {
        public string profileId { get; set; }
        public string groupId { get; set; }
        public string moduleId { get; set; }
        public string updatedOn { get; set; }


        public string district_id { get; set; }
        public string club_id { get; set; }
        public string category_id { get; set; }
        public string year { get; set; }
        public string SharType { get; set; }
        public string ClubRotaryType { get; set; }
        public string searchText { get; set; }//Nitesh Tiwari 27.03.2019 12.30 pm
    }

    /// <summary>
    /// Gallery Listing properties
    /// </summary>
    public class ClsAlbumList
    {
        public string albumId { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string groupId { get; set; }
        public string moduleId { get; set; }
        public string isAdmin { get; set; }

        public string clubname { get; set; }
        public string project_date { get; set; }
        public string project_cost { get; set; }
        public string beneficiary { get; set; }
        public string working_hour { get; set; }
        public string working_hour_type { get; set; }
        public string cost_of_project_type { get; set; }
        public string NumberOfRotarian { get; set; }
        public string sharetype { get; set; }

        public string Attendance { get; set; }
        public string AttendancePer { get; set; }
        public string MeetingType { get; set; }
        public string AgendaDocID { get; set; }
        public string MOMDocID { get; set; }
    }

    /// <summary>
    /// Gallery Listing Output
    /// </summary>
    public class ClsTotalOutput
    {
        public string TOTAL_PROJECTS { get; set; }
        public string COST_OF_PROJECT { get; set; }
        public string BENEFICIARY { get; set; }
        public string MEN_HOURS_SPENT { get; set; }
    }


    /// <summary>
    /// Gallery Listing Output
    /// </summary>
    public class ClsAlbumListOutput
    {
        public List<ClsAlbumList> newAlbums { get; set; }
        public List<ClsAlbumList> updatedAlbums { get; set; }
        public List<ClsTotalOutput> TotalList { get; set; }
        public string deletedAlbums { get; set; }
    }

    #endregion

    #region Album Photos Listing

    /// <summary>
    /// Album Listing Input parameters
    /// </summary>
    public class ClsPhotoListInput
    {
        public string albumId { get; set; }
        public string groupId { get; set; }
        public string updatedOn { get; set; }
    }

    /// <summary>
    /// Photos Listing properties
    /// </summary>
    public class ClsPhotoList
    {
        public string photoId { get; set; }
        public string url { get; set; }
        public string description { get; set; }
    }

    /// <summary>
    /// Photos Listing Output
    /// </summary>
    public class ClsPhotoListOutput
    {
        public List<ClsPhotoList> newPhotos { get; set; }
        public List<ClsPhotoList> updatedPhotos { get; set; }
        public string deletedPhotos { get; set; }
    }

    #endregion

    #region

    /// <summary>
    /// 
    /// </summary>
    public class ClsDeletePhotoInput
    {
        public string photoId { get; set; }
        public string albumId { get; set; }
        public string deletedBy { get; set; }
    }

    #endregion

    public class ClsGetAlbumDetailsInput
    {
        public string albumId { get; set; }
        public string grpId { get; set; }
        public string memberProfileId { get; set; }
    }



    /// <summary>
    /// Get album details
    /// </summary>
    public class ClsGetAlbumDetailsOutput
    {
        public string albumId { get; set; }
        public string groupId { get; set; }
        public string type { get; set; }
        public string memberIds { get; set; }
        public string albumTitle { get; set; }
        public string albumDescription { get; set; }
        public string albumImage { get; set; }
        public string shareType { get; set; } // Added by Nandu on 7/4/17 Task- Within the Club-0/public-1 mode - By default Within the Club-0

        public string clubname { get; set; }
        public string project_date { get; set; }
        public string project_cost { get; set; }
        public string beneficiary { get; set; }
        public string working_hour { get; set; }
        public string working_hour_type { get; set; }
        public string cost_of_project_type { get; set; }
        public string NumberOfRotarian { get; set; }

        public string albumCategoryID { get; set; }
        public string albumCategoryText { get; set; }
        public string othercategorytext { get; set; }

        public string Attendance { get; set; }
        public string AttendancePer { get; set; }
        public string MeetingType { get; set; }
        public string AgendaDocID { get; set; }
        public string MOMDocID { get; set; }
        public string AgendaDoc { get; set; }
        public string MOMDoc { get; set; }



    }

    // <summary>
    /// Fill all dropdowns
    /// </summary>
    public class GetShowcaseDetails
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string DistrictID { get; set; }

    }

    public class ClsGetShowcaseDetailsOutPut
    {
        public List<GetShowcaseDetails> Categories { get; set; }
        public List<GetShowcaseDetails> District { get; set; }
        public List<GetShowcaseDetails> Club { get; set; }

    }

    public class cls_NotificationInput
    {
        public string GroupID { get; set; }
        public string Type { get; set; }
    }
    public class cls_NotificationOutput
    {
        public List<cls_NotificationDetails> GalleryDetails { get; set; }
    }
    public class cls_NotificationDetails
    {
        public string GalleryID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string GalleryDate { get; set; }
        public string groupName { get; set; }
        public string GalleryType { get; set; }
        public string member_name { get; set; }
    }
}