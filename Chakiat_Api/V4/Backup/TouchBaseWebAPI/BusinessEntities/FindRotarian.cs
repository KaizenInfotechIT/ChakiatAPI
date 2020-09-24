using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class FindRotarian
    {
        //private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static List<RotarianResult> GetRotarian(FindRotarianInput search)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[4];
                param[0] = new MySqlParameter("?MemberName", string.IsNullOrEmpty(search.name) ? "" : search.name.Replace(" ", "%"));
                param[1] = new MySqlParameter("?classification", string.IsNullOrEmpty(search.classification) ? "" : search.classification.Replace(" ", "%"));
                param[2] = new MySqlParameter("?club", string.IsNullOrEmpty(search.club) ? "" : search.club.Replace(" ", "%"));
                param[3] = new MySqlParameter("?districtNo", string.IsNullOrEmpty(search.district_number) ? "0" : search.district_number.Replace(" ", "%"));
                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<RotarianResult>("CALL V1_USPFindRotarian(?MemberName,?classification,?club,?districtNo)", param).ToList();

                    foreach (RotarianResult Mem in Result)
                    {
                        if (!string.IsNullOrEmpty(Mem.pic) && Mem.pic != "profile_photo.png")
                        {
                            string ImageName = Mem.pic.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                            Mem.pic = path + ImageName;
                        }
                        else
                            Mem.pic = ConfigurationManager.AppSettings["imgPath"] + "images/profile_pic.png";
                    }
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static RotarianDetailsOutput GetRotarianDetails(string profileID)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[1];
                param[0] = new MySqlParameter("?profileID", profileID);
                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V2_1_USPGetRotarianDetails", param);
                List<RotarianDetailsOutput> rowMember = new List<RotarianDetailsOutput>();
                if (result != null)
                {
                    DataTable dtMember = result.Tables[0];
                    if (dtMember.Rows.Count > 0)
                    {
                        rowMember = GlobalFuns.DataTableToList<RotarianDetailsOutput>(dtMember);

                        if (!string.IsNullOrEmpty(rowMember[0].pic) && rowMember[0].pic != "profile_photo.png")
                        {
                            string ImageName = rowMember[0].pic.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                            rowMember[0].pic = path + ImageName;
                        }
                    }
                    if (result.Tables[1] != null)
                    {
                        if (result.Tables[1].Rows.Count > 0)
                        {
                            rowMember[0].BusinessAddress = result.Tables[1].Rows[0]["BusinessAddress"].ToString();
                            rowMember[0].city = result.Tables[1].Rows[0]["city"].ToString();
                            rowMember[0].state = result.Tables[1].Rows[0]["state"].ToString();
                            rowMember[0].country = result.Tables[1].Rows[0]["country"].ToString();
                            rowMember[0].pincode = result.Tables[1].Rows[0]["pincode"].ToString();
                            rowMember[0].Fax = result.Tables[1].Rows[0]["Fax"].ToString();
                            rowMember[0].phoneNo = result.Tables[1].Rows[0]["phoneNo"].ToString();
                        }
                        else
                        {
                            rowMember[0].BusinessAddress = "";
                            rowMember[0].city = "";
                            rowMember[0].state = "";
                            rowMember[0].country = "";
                            rowMember[0].pincode = "";
                            rowMember[0].Fax = "";
                            rowMember[0].phoneNo = "";
                        }
                    }
                }
                return rowMember[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}