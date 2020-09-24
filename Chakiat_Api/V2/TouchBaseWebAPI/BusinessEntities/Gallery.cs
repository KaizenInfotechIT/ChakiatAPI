using System;
using System.Collections.Generic;
using System.Linq;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class Gallery
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static ClsAlbumListOutput GetGalleryList(ClsAlbumListInput gallery)
        {
            try
            {
                MySqlParameter[] parameterList = new MySqlParameter[4];

                parameterList[0] = new MySqlParameter("?ProfileId", gallery.profileId);
                parameterList[1] = new MySqlParameter("?GroupId", gallery.groupId);
                parameterList[2] = new MySqlParameter("?UpdatedOn", gallery.updatedOn);
                parameterList[3] = new MySqlParameter("?ModuleID", gallery.moduleId);
                // parameterList[3] = new MySqlParameter("?UpdatedOn", gallery.updatedOn);

                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V6_USPGalleryList", parameterList);

                DataTable dtNewAlbums = result.Tables[0];
                DataTable dtUpdatedAlbums = result.Tables[1];
                DataTable dtDeletedAlbums = result.Tables[2];

                List<ClsAlbumList> NewMemberList = new List<ClsAlbumList>();
                if (dtNewAlbums.Rows.Count > 0)
                {
                    NewMemberList = GlobalFuns.DataTableToList<ClsAlbumList>(dtNewAlbums);

                    foreach (ClsAlbumList g in NewMemberList)
                    {
                        if (!string.IsNullOrEmpty(g.image))
                        {
                            string ImageName = g.image.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/gallery/Group" + g.groupId + "/";
                            g.image = path + ImageName;
                        }
                    }
                }

                List<ClsAlbumList> updatedMemberList = new List<ClsAlbumList>();
                if (dtUpdatedAlbums.Rows.Count > 0)
                {
                    updatedMemberList = GlobalFuns.DataTableToList<ClsAlbumList>(dtUpdatedAlbums);

                    foreach (ClsAlbumList g in updatedMemberList)
                    {
                        if (!string.IsNullOrEmpty(g.image))
                        {
                            string ImageName = g.image.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/gallery/Group" + g.groupId + "/";
                            g.image = path + ImageName;
                        }
                    }
                }

                ClsAlbumListOutput galleryList = new ClsAlbumListOutput();

                galleryList.deletedAlbums = dtDeletedAlbums.Rows[0]["GalleryId"].ToString();
                galleryList.newAlbums = NewMemberList;
                galleryList.updatedAlbums = updatedMemberList;

                return galleryList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Imgname createAlbum(ClsAddUpdateAlbumInput album)
        {
            string subgrpIDs = "";
            try
            {
                if (album.isSubGrpAdmin == "1" && album.type == "0")
                {
                    subgrpIDs = SubGroupDirectory.GetAdminSubGroupList(album.groupId, album.createdBy);
                }
                MySqlParameter[] ParameterList = new MySqlParameter[12];
                ParameterList[0] = new MySqlParameter("?p_gallery_id", album.albumId);
                ParameterList[1] = new MySqlParameter("?p_group_id", album.groupId);
                ParameterList[2] = new MySqlParameter("?p_gallery_type", album.type);

                ParameterList[3] = new MySqlParameter("?p_memprofileIDs", album.memberIds);

                ParameterList[4] = new MySqlParameter("?p_album_title", album.albumTitle);
                ParameterList[5] = new MySqlParameter("?p_album_description", string.IsNullOrEmpty(album.albumDescription) ? "" : album.albumDescription);
                ParameterList[6] = new MySqlParameter("?p_image", string.IsNullOrEmpty(album.albumImage) ? "0" : album.albumImage);

                ParameterList[7] = new MySqlParameter("?p_createdby", album.createdBy);
                ParameterList[8] = new MySqlParameter("?IsSubgrpAdmin", string.IsNullOrEmpty(album.isSubGrpAdmin) ? "0" : album.isSubGrpAdmin);
                ParameterList[9] = new MySqlParameter("?subGrpIDs", subgrpIDs);

                ParameterList[10] = new MySqlParameter("?moduleID", album.moduleId); // Added for Gallery Replica
                ParameterList[11] = new MySqlParameter("?shareType", album.shareType); // Added for Gallery shareType

                var Result = _DBTouchbase.ExecuteStoreQuery<Imgname>("CALL V6_USPAddEditGallery(?p_gallery_id, ?p_group_id, ?p_gallery_type, ?p_memprofileIDs, ?p_album_title, ?p_album_description, ?p_image, ?p_createdby, ?IsSubgrpAdmin, ?subGrpIDs, ?moduleID, ?shareType)", ParameterList).SingleOrDefault();

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ClsPhotoListOutput GetAlbumPhotoList(ClsPhotoListInput album)
        {
            try
            {
                MySqlParameter[] parameterList = new MySqlParameter[3];

                parameterList[0] = new MySqlParameter("?AlbumId", album.albumId);
                parameterList[1] = new MySqlParameter("?GroupId", album.groupId);
                parameterList[2] = new MySqlParameter("?UpdatedOn", album.updatedOn);

                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V4_USPAlbumList", parameterList);

                DataTable dtNewPhoto = result.Tables[0];
                DataTable dtUpdatedPhoto = result.Tables[1];
                DataTable dtDeletedPhotoId = result.Tables[2];

                List<ClsPhotoList> NewPhotoList = new List<ClsPhotoList>();
                if (dtNewPhoto.Rows.Count > 0)
                {
                    NewPhotoList = GlobalFuns.DataTableToList<ClsPhotoList>(dtNewPhoto);

                    foreach (ClsPhotoList g in NewPhotoList)
                    {
                        if (!string.IsNullOrEmpty(g.url))
                        {
                            string ImageName = g.url.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/gallery/Group" + album.groupId + "/Album" + album.albumId + "/";
                            g.url = path + ImageName;
                        }
                    }
                }

                List<ClsPhotoList> updatedPhotoList = new List<ClsPhotoList>();
                if (dtUpdatedPhoto.Rows.Count > 0)
                {
                    updatedPhotoList = GlobalFuns.DataTableToList<ClsPhotoList>(dtUpdatedPhoto);

                    foreach (ClsPhotoList g in updatedPhotoList)
                    {
                        if (!string.IsNullOrEmpty(g.url))
                        {
                            string ImageName = g.url.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/gallery/Group" + album.groupId + "/Album" + album.albumId + "/";
                            g.url = path + ImageName;
                        }
                    }
                }

                ClsPhotoListOutput photoList = new ClsPhotoListOutput();

                photoList.deletedPhotos = dtDeletedPhotoId.Rows[0]["deletedPhotos"].ToString();
                photoList.newPhotos = NewPhotoList;
                photoList.updatedPhotos = updatedPhotoList;

                return photoList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int AddUpdateAlbumPhoto(string photoId, string imageName, string desc, string albumId, string createdBy)
        {
            int result = 0;

            MySqlParameter[] parameterList = new MySqlParameter[5];

            parameterList[0] = new MySqlParameter("?PhotoId", photoId);
            parameterList[1] = new MySqlParameter("?ImageName", imageName);
            parameterList[2] = new MySqlParameter("?Description", string.IsNullOrEmpty(desc) ? "" : desc);
            parameterList[3] = new MySqlParameter("?AlbumId", albumId);
            parameterList[4] = new MySqlParameter("?CreatedBy", createdBy);

            try
            {
                result = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.StoredProcedure, "V4_USPAddAlbumPhoto", parameterList);
            }
            catch
            {
                throw;
            }

            return result;
        }

        public static int DelteAlbumPhoto(ClsDeletePhotoInput photo)
        {
            int result = 0;

            MySqlParameter[] parameterList = new MySqlParameter[3];

            parameterList[0] = new MySqlParameter("?PhotoId", photo.photoId);
            parameterList[1] = new MySqlParameter("?AlbumId", photo.albumId);
            parameterList[2] = new MySqlParameter("?DeletedBy", photo.deletedBy);

            try
            {
                result = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.StoredProcedure, "V4_USPDeleteAlbumPhoto", parameterList);
            }
            catch
            {
                throw;
            }

            return result;
        }

        public static List<ClsGetAlbumDetailsOutput> GetAlbumDetails(ClsGetAlbumDetailsInput album)
        {
            try
            {
                MySqlParameter[] parameterList = new MySqlParameter[1];

                parameterList[0] = new MySqlParameter("?AlbumId", album.albumId);
                //parameterList[1] = new MySqlParameter("?GroupId", album.grpId);
                //parameterList[2] = new MySqlParameter("?ProfileId", album.memberProfileId);

                var Result = _DBTouchbase.ExecuteStoreQuery<ClsGetAlbumDetailsOutput>("CALL V6_USPGetAlbumDetails(?AlbumId)", parameterList).ToList();

                foreach (ClsGetAlbumDetailsOutput objAlbum in Result)
                {
                    if (!string.IsNullOrEmpty(objAlbum.albumImage))
                    {
                        string announ_Image = objAlbum.albumImage.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/gallery/Group" + objAlbum.groupId + "/";
                        objAlbum.albumImage = path + announ_Image;
                    }
                }
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}