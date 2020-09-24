using System.Web.Http;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;
using System.Configuration;
using System.Web;
using System;
using System.IO;
using System.Collections.Generic;

namespace TouchBaseWebAPI.Controllers
{
    public class GalleryController : ApiController
    {
        /// <summary>
        /// Created By : Nandkishor K
        /// Created Date : 29/08/2016
        /// Reason : Listing of Albums
        /// </summary>
        /// <param name="service">Listing of gallery</param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public object GetAlbumsList(ClsAlbumListInput gallery)
        {
            dynamic TBAlbumsListResult;

            try
            {
                ClsAlbumListOutput Result = Gallery.GetGalleryList(gallery);

                if (Result != null)
                {
                    TBAlbumsListResult = new { status = "0", message = "success", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Result };
                }
                else
                {
                    TBAlbumsListResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAlbumsListResult = new { status = "1", message = "failed" };
            }

            return new { TBAlbumsListResult };
        }

        /// <summary>
        /// Created By : Nandkishor K
        /// Created Date : 26/08/2016
        /// Reason : Create Update New Albums
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public object AddUpdateAlbum(ClsAddUpdateAlbumInput album)
        {
            dynamic TBAddGalleryResult;
            int str = -1;

            try
            {
                Imgname Result = Gallery.createAlbum(album);

                if (!string.IsNullOrEmpty(Result.imgName))
                {
                    str = GlobalFuns.UploadImage(album.groupId, Result.imgName, "gallery");
                }
                else
                    str = 0;

                if (Result != null)
                {
                    if (str == 0)
                    {
                        TBAddGalleryResult = new { status = "0", message = "success" };
                    }
                    else
                        TBAddGalleryResult = new { status = "1", message = "failed" };
                }
                else
                {
                    TBAddGalleryResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAddGalleryResult = new { status = "1", message = "failed" };
            }

            return new { TBAddGalleryResult };
        }

        [System.Web.Http.HttpPost]
        public object GetAlbumDetails(ClsGetAlbumDetailsInput album)
        {
            dynamic TBAlbumDetailResult;
            List<object> AlbumDetailResult = new List<object>();

            try
            {
                List<ClsGetAlbumDetailsOutput> Result = Gallery.GetAlbumDetails(album);

                for (int i = 0; i < Result.Count; i++)
                {
                    AlbumDetailResult.Add(new { AlbumDetail = Result[i] });
                }

                if (Result != null && Result.Count != 0)
                {
                    TBAlbumDetailResult = new { status = "0", message = "success", AlbumDetailResult = AlbumDetailResult };
                }
                else
                {
                    TBAlbumDetailResult = new { status = "1", message = "Record not found", AlbumDetailResult = AlbumDetailResult };
                }
            }
            catch
            {
                TBAlbumDetailResult = new { status = "1", message = "failed" };
            }

            return new { TBAlbumDetailResult };
        }

        /// <summary>
        /// Created By : Nandkishor K
        /// Created Date : 29/08/2016
        /// Reason : Listing of Album Photos
        /// </summary>
        /// <param name="album"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public object GetAlbumPhotoList(ClsPhotoListInput album)
        {
            dynamic TBAlbumPhotoListResult;

            try
            {
                ClsPhotoListOutput Result = Gallery.GetAlbumPhotoList(album);

                if (Result != null)
                {
                    TBAlbumPhotoListResult = new { status = "0", message = "success", updatedOn = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), Result };
                }
                else
                {
                    TBAlbumPhotoListResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAlbumPhotoListResult = new { status = "1", message = "failed" };
            }

            return new { TBAlbumPhotoListResult };
        }

        /// <summary>
        /// Created By : Nandkishor K
        /// Created Date : 30/08/2016
        /// Modified Date : 29/11/2016
        /// Reason : Add/Update photos to albums
        /// </summary>
        [System.Web.Http.HttpPost]
        public object AddUpdateAlbumPhoto(string photoId, string desc, string albumId, string groupId, string createdBy)
        {
            dynamic LoadImageResult;

            try
            {
                var httpRequest = HttpContext.Current.Request;
                string FileName = string.Empty;

                if (httpRequest.Files.Count > 0)
                {
                    bool flag = false;
                    FileName = DateTime.Now.ToString("ddMMyyyyhhmmsstt") + ".png";

                    string Path = ConfigurationManager.AppSettings["imgPathSave"] + "Documents\\gallery\\Group" + groupId + "\\Album" + albumId;
                    string filePath = ConfigurationManager.AppSettings["imgPathSave"] + "Documents\\gallery\\Group" + groupId + "\\Album" + albumId + "\\" + FileName;

                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];

                        if (!Directory.Exists(Path))
                            Directory.CreateDirectory(Path);

                        postedFile.SaveAs(filePath);
                        flag = true;
                    }

                    if (flag)
                    {
                        Gallery.AddUpdateAlbumPhoto(photoId, FileName, desc, albumId, createdBy);
                        LoadImageResult = new { status = "0", message = "success" };
                    }
                    else
                    {
                        LoadImageResult = new { status = "1", message = "failed" };
                    }
                }
                else
                {
                    Gallery.AddUpdateAlbumPhoto(photoId, FileName, desc, albumId, createdBy);
                    LoadImageResult = new { status = "0", message = "success" };
                }
            }
            catch
            {
                LoadImageResult = new { status = "1", message = "failed" };
            }

            return new { LoadImageResult };
        }

        /// <summary>
        /// Created By : Nandkishor K
        /// Created Date : 31/08/2016
        /// Reason : Delete photos from Album
        /// </summary>
        /// <param name="deletePhoto"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public object DeleteAlbumPhoto(ClsDeletePhotoInput deletePhoto)
        {
            dynamic TBDelteAlbumPhoto;

            try
            {
                int result = Gallery.DelteAlbumPhoto(deletePhoto);

                if (result > 0)
                {
                    TBDelteAlbumPhoto = new { status = "0", message = "success" };
                }
                else
                {
                    TBDelteAlbumPhoto = new { status = "1", message = "failed" };
                }
            }
            catch
            {
                TBDelteAlbumPhoto = new { status = "1", message = "failed" };
            }

            return new { TBDelteAlbumPhoto };
        }     
    }
}
