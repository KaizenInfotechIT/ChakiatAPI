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

        public static Imgname createAlbum_New(ClsAddUpdateAlbumInput album)
        {
            string subgrpIDs = "";
            try
            {
                if (album.isSubGrpAdmin == "1" && album.type == "0")
                {
                    subgrpIDs = SubGroupDirectory.GetAdminSubGroupList(album.groupId, album.createdBy);
                }
                MySqlParameter[] ParameterList = new MySqlParameter[26];
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


                ParameterList[12] = new MySqlParameter("?categoryId", album.categoryId);
                ParameterList[13] = new MySqlParameter("?dateofproect", album.dateofproject);
                ParameterList[14] = new MySqlParameter("?costofproject", album.costofproject);
                ParameterList[15] = new MySqlParameter("?beneficiary", album.beneficiary);
                ParameterList[16] = new MySqlParameter("?manhourspent", album.manhourspent);
                ParameterList[17] = new MySqlParameter("?manhourspenttype", album.manhourspenttype);
                ParameterList[18] = new MySqlParameter("?p_NumberofRotarian", album.NumberofRotarian);
                ParameterList[19] = new MySqlParameter("?p_OtherCategorytext", album.OtherCategorytext);
                ParameterList[20] = new MySqlParameter("?costofprojecttype", album.costofprojecttype);

                ParameterList[21] = new MySqlParameter("?Attendance", album.Attendance);
                ParameterList[22] = new MySqlParameter("?AttendancePer", album.AttendancePer);
                ParameterList[23] = new MySqlParameter("?MeetingType", album.MeetingType);
                ParameterList[24] = new MySqlParameter("?AgendaDocID", album.AgendaDocID);
                ParameterList[25] = new MySqlParameter("?MOMDocID", album.MOMDocID);

                var Result = _DBTouchbase.ExecuteStoreQuery<Imgname>("CALL V7_USPAddEditGallery(?p_gallery_id, ?p_group_id, ?p_gallery_type, ?p_memprofileIDs, ?p_album_title, ?p_album_description, ?p_image, ?p_createdby, ?IsSubgrpAdmin, ?subGrpIDs, ?moduleID, ?shareType,?categoryId,?dateofproect,?costofproject,?beneficiary,?manhourspent,?manhourspenttype,?p_NumberofRotarian,?p_OtherCategorytext,?costofprojecttype,?Attendance,?AttendancePer,?MeetingType,?AgendaDocID,?MOMDocID)", ParameterList).SingleOrDefault();

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


        public static ClsGetShowcaseDetailsOutPut GetShowcaseDetails(GetShowcaseDetails clschowcase)
        {
            try
            {
                string sqlProc;

                sqlProc = "V1_USPGetShowcaseDetails";

                 MySqlParameter[] parameterList = new MySqlParameter[1];
                 parameterList[0] = new MySqlParameter("?DistrictID", clschowcase.DistrictID);
                 //parameterList[1] = new MySqlParameter("?ProfileID", monthCal.profileId);
                 //parameterList[2] = new MySqlParameter("?Curr_Date", monthCal.selectedDate);
                 //parameterList[3] = new MySqlParameter("?UpdatedOn", monthCal.updatedOn);

                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, sqlProc, parameterList);

                

                ClsGetShowcaseDetailsOutPut showcase = new ClsGetShowcaseDetailsOutPut();
                if (clschowcase.DistrictID == "0")
                {

                    //Inserted One row for ALL as per told rupesh
                    DataTable dt_Category = new DataTable();
                    if (result.Tables[0].Rows.Count > 0)
                    {
                        DataRow toInsert_Category = result.Tables[0].NewRow();
                        result.Tables[0].Rows.Add("0", "All", "");
                        DataView dv_Category = result.Tables[0].DefaultView;
                        dv_Category.Sort = "ID ASC";
                        dt_Category = dv_Category.ToTable();
                        DataRow toInsert_Category1 = dt_Category.NewRow();
                        dt_Category.Rows.Add("-1", "Others", "");
                    }

                    //Inserted One row for ALL as per told rupesh
                    DataTable dt_Club = new DataTable();
                    if (result.Tables[1].Rows.Count > 0)
                    {
                        DataRow toInsert_Club = result.Tables[1].NewRow();
                        result.Tables[1].Rows.Add("0", "All", "");
                        DataView dv_Club = result.Tables[1].DefaultView;
                        dv_Club.Sort = "ID ASC";
                        dt_Club = dv_Club.ToTable();
                    }

                    //Inserted One row for ALL as per told rupesh
                    DataTable dt_District = new DataTable();
                    if (result.Tables[2].Rows.Count > 0)
                    {
                        DataRow toInsert_District = result.Tables[2].NewRow();
                        result.Tables[2].Rows.Add("0", "All", "");
                        DataView dv_District = result.Tables[2].DefaultView;
                        dv_District.Sort = "ID ASC";
                        dt_District = dv_District.ToTable();
                    }

                    DataTable dtCategory = dt_Category;
                    DataTable dtClub = dt_Club;
                    DataTable dtDistrict = dt_District;

                    List<GetShowcaseDetails> CategoryList = new List<GetShowcaseDetails>();
                    if (dtCategory.Rows.Count > 0)
                    {
                        CategoryList = GlobalFuns.DataTableToList<GetShowcaseDetails>(dtCategory);
                    }
                    List<GetShowcaseDetails> ClubList = new List<GetShowcaseDetails>();
                    if (dtClub.Rows.Count > 0)
                    {
                        ClubList = GlobalFuns.DataTableToList<GetShowcaseDetails>(dtClub);
                    }
                    List<GetShowcaseDetails> DistrictList = new List<GetShowcaseDetails>();
                    if (dtDistrict.Rows.Count > 0)
                    {
                        DistrictList = GlobalFuns.DataTableToList<GetShowcaseDetails>(dtDistrict);
                    }

                    showcase.District = DistrictList;
                    showcase.Club = ClubList;
                    showcase.Categories = CategoryList;
                }
                else
                {

                    //Inserted One row for ALL as per told rupesh
                    DataTable dt_Club = new DataTable();
                    if (result.Tables[0].Rows.Count > 0)
                    {
                        DataRow toInsert_Club = result.Tables[0].NewRow();
                        result.Tables[0].Rows.Add("0", "All", "");
                        DataView dv_Club = result.Tables[0].DefaultView;
                        dv_Club.Sort = "ID ASC";
                        dt_Club = dv_Club.ToTable();
                    }

                    //DataTable dtClub = result.Tables[0];

                    List<GetShowcaseDetails> CategoryList = new List<GetShowcaseDetails>();

                    List<GetShowcaseDetails> ClubList = new List<GetShowcaseDetails>();
                    if (dt_Club.Rows.Count > 0)
                    {
                        ClubList = GlobalFuns.DataTableToList<GetShowcaseDetails>(dt_Club);
                    }

                    List<GetShowcaseDetails> DistrictList = new List<GetShowcaseDetails>();


                    showcase.District = DistrictList;
                    showcase.Club = ClubList;
                    showcase.Categories = CategoryList;
                }
                return showcase;
            }
            catch
            {
                throw;
            }
        }


        //public static ClsGetShowcaseDetailsOutPut GetShowcaseDetails(GetShowcaseDetails clschowcase)
        //{
        //    try
        //    {
        //        string sqlProc;

        //        sqlProc = "V1_USPGetShowcaseDetails";

        //        MySqlParameter[] parameterList = new MySqlParameter[0];
        //        parameterList[0] = new MySqlParameter("?DistrictID", clschowcase.DistrictID);
        //        //parameterList[1] = new MySqlParameter("?ProfileID", monthCal.profileId);
        //        //parameterList[2] = new MySqlParameter("?Curr_Date", monthCal.selectedDate);
        //        //parameterList[3] = new MySqlParameter("?UpdatedOn", monthCal.updatedOn);

        //        DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, sqlProc, parameterList);
        //        DataTable dtClub = result.Tables[0];              

               
        //        List<GetShowcaseDetails> ClubList = new List<GetShowcaseDetails>();
        //        if (dtClub.Rows.Count > 0)
        //        {
        //            ClubList = GlobalFuns.DataTableToList<GetShowcaseDetails>(dtClub);
        //        }
               
        //        ClsGetShowcaseDetailsOutPut showcase = new ClsGetShowcaseDetailsOutPut();
        //        showcase.Club = ClubList;
        //        return showcase;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}


        public static ClsAlbumListOutput GetGalleryList_New(ClsAlbumListInput gallery)
        {
            try
            {
                MySqlParameter[] parameterList = new MySqlParameter[9];

                parameterList[0] = new MySqlParameter("?grpId", gallery.groupId);
                parameterList[1] = new MySqlParameter("?district_id", gallery.district_id);
                parameterList[2] = new MySqlParameter("?category_id", gallery.category_id);
                parameterList[3] = new MySqlParameter("?year", gallery.year);
                parameterList[4] = new MySqlParameter("?club_id", gallery.club_id);
                parameterList[5] = new MySqlParameter("?SharType", gallery.SharType);
                parameterList[6] = new MySqlParameter("?ProfileId", gallery.profileId);
                parameterList[7] = new MySqlParameter("?ModuleID", gallery.moduleId);
                parameterList[8] = new MySqlParameter("?ClubRotaryType", gallery.ClubRotaryType);

                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V7_USPGalleryList", parameterList);

                DataTable dtNewAlbums = result.Tables[0];
                //DataTable dtTotals = result.Tables[0];
                //DataTable dtDeletedAlbums = result.Tables[2];
              
                List<ClsAlbumList> NewMemberList = new List<ClsAlbumList>();
                //List<ClsTotalOutput> TotalList = new List<ClsTotalOutput>();
                if (dtNewAlbums.Rows.Count > 0)
                {
                    NewMemberList = GlobalFuns.DataTableToList<ClsAlbumList>(dtNewAlbums);
                    //TotalList = GlobalFuns.DataTableToList<ClsTotalOutput>(dtTotals);

                    foreach (ClsAlbumList g in NewMemberList)
                    {
                        if (!string.IsNullOrEmpty(g.image))
                        {
                            string ImageName = g.image.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/gallery/Group" + g.groupId + "/";
                            g.image = path + ImageName;
                        }
                        if (!string.IsNullOrEmpty(g.AgendaDocID))
                        {
                            string ImageName = g.AgendaDocID.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/gallery/Group" + g.groupId + "/";
                            g.AgendaDocID = path + ImageName;
                        }
                        if (!string.IsNullOrEmpty(g.MOMDocID))
                        {
                            string ImageName = g.MOMDocID.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/gallery/Group" + g.groupId + "/";
                            g.MOMDocID = path + ImageName;
                        }
                    }
                }
              
              
                ClsAlbumListOutput galleryList = new ClsAlbumListOutput();             
                galleryList.newAlbums = NewMemberList;
                //galleryList.TotalList = TotalList;

                return galleryList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ClsPhotoList> GetAlbumPhotoList_New(ClsPhotoListInput album)
        {
            try
            {
                MySqlParameter[] parameterList = new MySqlParameter[1];

                parameterList[0] = new MySqlParameter("?AlbumId", album.albumId);

               
                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V4_USPAlbumList_New", parameterList);

                DataTable dtNewPhoto = result.Tables[0];
               
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
                ClsPhotoListOutput photoList = new ClsPhotoListOutput();
                photoList.newPhotos = NewPhotoList;

                return photoList.newPhotos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ClsGetAlbumDetailsOutput> GetAlbumDetails_New(ClsGetAlbumDetailsInput album)
        {
            try
            {
                MySqlParameter[] parameterList = new MySqlParameter[1];

                parameterList[0] = new MySqlParameter("?AlbumId", album.albumId);
                
                var Result = _DBTouchbase.ExecuteStoreQuery<ClsGetAlbumDetailsOutput>("CALL V6_USPGetAlbumDetails_New(?AlbumId)", parameterList).ToList();

                foreach (ClsGetAlbumDetailsOutput objAlbum in Result)
                {
                    if (!string.IsNullOrEmpty(objAlbum.albumImage))
                    {
                        string announ_Image = objAlbum.albumImage.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/gallery/Group" + objAlbum.groupId + "/";
                        objAlbum.albumImage = path + announ_Image;
                    }
                    if (!string.IsNullOrEmpty(objAlbum.AgendaDoc))
                    {
                        string ImageName = objAlbum.AgendaDoc.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/gallery/Group" + objAlbum.groupId + "/";
                        objAlbum.AgendaDoc = path + ImageName;
                    }
                    if (!string.IsNullOrEmpty(objAlbum.MOMDoc))
                    {
                        string ImageName = objAlbum.MOMDoc.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/gallery/Group" + objAlbum.groupId + "/";
                        objAlbum.MOMDoc = path + ImageName;
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