using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Configuration;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class DistrictCommittee
    {
        //private static TouchBaseWebAPI.Data.row_productionEntities _DbTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static districtCommittee getDistrictCommitteeList(districtCommitteeInput Obj)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[3];

                param[0] = new MySqlParameter("@p_groupId", Obj.groupID);
                param[1] = new MySqlParameter("@p_searchText", Obj.searchText);
                param[2] = new MySqlParameter("@p_Yearfilter", Obj.Yearfilter);

                DataSet Result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "API_districtCommitteeList", param);

                DataTable dt_WithoutCatlist = Result.Tables[0];
                DataTable dt_withcatlist = Result.Tables[1];
                DataTable dt_Yearlist = Result.Tables[2];

                List<districtCommitteeWithoutCatList> WithoutCategoryList = new List<districtCommitteeWithoutCatList>();
                List<districtCommitteeWithCatList> WithDirectoryList = new List<districtCommitteeWithCatList>();
                List<Yearlist> Yearlist = new List<Yearlist>();

                if (dt_WithoutCatlist.Rows.Count > 0)
                {
                    foreach (DataRow item in dt_WithoutCatlist.Rows)
                    {
                        if (item["img"].ToString() != "")
                        {
                            if (item["fk_Member_profileID"].ToString() == "0")
                            {
                                string profile_Image = item["img"].ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/DistrictCommittee/Group" + Obj.groupID + "/thumb/";
                                item["img"] = path + profile_Image;
                            }
                            else
                            {
                                string profile_Image = item["img"].ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                                item["img"] = path + profile_Image;
                            }
                        }
                        else
                        {
                            item["img"] = "";
                        }
                    }

                    WithoutCategoryList = GlobalFuns.DataTableToList<districtCommitteeWithoutCatList>(dt_WithoutCatlist);

                    
                }

                if (dt_withcatlist.Rows.Count > 0)
                {
                    WithDirectoryList = GlobalFuns.DataTableToList<districtCommitteeWithCatList>(dt_withcatlist);
                }

                if (dt_Yearlist.Rows.Count > 0)
                {
                    Yearlist = GlobalFuns.DataTableToList<Yearlist>(dt_Yearlist);
                }

                districtCommittee obj = new districtCommittee();
                obj.districtCommitteeWithoutCatList = WithoutCategoryList;
                obj.districtCommitteeWithCatList = WithDirectoryList;
                obj.Yearlist = Yearlist;
                obj.CurrentYear = Result.Tables[3].Rows[0]["CurrentYear"].ToString();

                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static districtCommittee getDistrictCommitteeSearchList(districtCommitteeInput Obj)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[3];

                param[0] = new MySqlParameter("@p_groupId", Obj.groupID);
                param[1] = new MySqlParameter("@p_searchText", Obj.searchText);
                param[2] = new MySqlParameter("@p_Yearfilter", Obj.Yearfilter);
                DataSet Result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "API_districtCommitteeSearchList", param);

                DataTable dt_WithoutCatlist = Result.Tables[0];
              
                List<districtCommitteeWithoutCatList> WithoutCategoryList = new List<districtCommitteeWithoutCatList>();
               
                if (dt_WithoutCatlist.Rows.Count > 0)
                {
                    foreach (DataRow item in dt_WithoutCatlist.Rows)
                    {
                        if (item["img"].ToString() != "")
                        {
                            if (item["fk_Member_profileID"].ToString() == "0")
                            {
                                string profile_Image = item["img"].ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/DistrictCommittee/Group" + Obj.groupID + "/thumb/";
                                item["img"] = path + profile_Image;
                            }
                            else
                            {
                                string profile_Image = item["img"].ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                                item["img"] = path + profile_Image;
                            }
                        }
                        else
                        {
                            item["img"] = "";
                        }
                    }
                    WithoutCategoryList = GlobalFuns.DataTableToList<districtCommitteeWithoutCatList>(dt_WithoutCatlist);
                }

                districtCommittee obj = new districtCommittee();
                obj.districtCommitteeWithoutCatList = WithoutCategoryList;
                
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static districtCommittee getDistrictCommitteeDetails(districtCommitteeInput Obj)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];

                param[0] = new MySqlParameter("@p_DistrictCommitteID", Obj.DistrictCommitteID);
                param[1] = new MySqlParameter("@p_groupID", Obj.groupID);

                DataSet Result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "API_districtCommitteeDetails", param);

                DataTable dt_WithoutCatlist = Result.Tables[0];

                List<districtCommitteeWithoutCatList> WithoutCategoryList = new List<districtCommitteeWithoutCatList>();

                if (dt_WithoutCatlist.Rows.Count > 0)
                {
                    foreach (DataRow item in dt_WithoutCatlist.Rows)
                    {
                        if (item["img"].ToString() != "")
                        {
                            if (item["fk_Member_profileID"].ToString() == "0")
                            {
                                string profile_Image = item["img"].ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/DistrictCommittee/Group" + Obj.groupID + "/thumb/";
                                item["img"] = path + profile_Image;
                            }
                            else
                            {
                                string profile_Image = item["img"].ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                                item["img"] = path + profile_Image;
                            }
                        }
                        else
                        {
                            item["img"] = "";
                        }
                    }

                    WithoutCategoryList = GlobalFuns.DataTableToList<districtCommitteeWithoutCatList>(dt_WithoutCatlist);
                }

                districtCommittee obj = new districtCommittee();
                obj.districtCommitteeWithoutCatList = WithoutCategoryList;

                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}