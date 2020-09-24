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
    }

    /// <summary>
    /// Gallery Listing Output
    /// </summary>
    public class ClsAlbumListOutput
    {
        public List<ClsAlbumList> newAlbums { get; set; }
        public List<ClsAlbumList> updatedAlbums { get; set; }
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
    }

}