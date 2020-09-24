using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using TouchBaseWebAPI.Models;
using System.Data;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;


namespace TouchBaseWebAPI.BusinessEntities
{
    public class District
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();
        //private static MemoryStream ms;
        private static ZipOutputStream zos;
        private static string strBaseDir;

        public static MemberDetailsDynamicField GetMemberDtlWithDynamicFeild(string grpuserID, string grpID)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];
                param[0] = new MySqlParameter("@profileId", grpuserID);
                param[1] = new MySqlParameter("@GrpID", grpID);

                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V6_USPGetMemberDetailsDistrict", param);
                DataTable dtMember = result.Tables[0];
                DataTable dtAddress = result.Tables[3];
                //DataTable dtFamilyMember = result.Tables[4];
                DataTable dtSettings = result.Tables[4];
                DataTable dtDynamicFields = result.Tables[5];

                List<MemberDetailsDynamicField> Memberdtls = new List<MemberDetailsDynamicField>();
                if (dtMember.Rows.Count > 0)
                {
                    Memberdtls = GlobalFuns.DataTableToList<MemberDetailsDynamicField>(dtMember);

                    foreach (MemberDetailsDynamicField mem in Memberdtls)
                    {
                        if (!string.IsNullOrEmpty(mem.profilePic))
                        {
                            string ImageName = mem.profilePic.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                            mem.profilePic = path + ImageName;
                        }
                        if (!string.IsNullOrEmpty(mem.familyPic))
                        {
                            string ImageName = mem.familyPic.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                            mem.familyPic = path + ImageName;
                        }
                    }
                    //=============================== Get Personal Fields ===========================================
                    DataTable dtPersonal = new DataTable();
                    dtPersonal.Columns.Add("fieldID");
                    dtPersonal.Columns.Add("Key");
                    dtPersonal.Columns.Add("Value");
                    dtPersonal.Columns.Add("ColType");
                    dtPersonal.Columns.Add("Uniquekey");
                    dtPersonal.Columns.Add("isEditable");
                    dtPersonal.Columns.Add("isVisible");
                    for (int i = 0; i < result.Tables[1].Rows.Count; i++)
                    {
                        for (int j = 0; j < result.Tables[1].Columns.Count; j++)
                        {

                            string colName = result.Tables[1].Columns[j].ColumnName;
                            string isVisible = (from e in dtSettings.AsEnumerable()
                                                where e.Field<string>("DBColName") == colName
                                                && e.Field<string>("fieldType") == "P"
                                                select e.Field<string>("isVisible")).SingleOrDefault();


                            DataRow dr = dtPersonal.NewRow();
                            dr["fieldID"] = "0";
                            dr["Uniquekey"] = colName;
                            dr["isEditable"] = "1";
                            if (colName == "isBOD")
                                dr["isVisible"] = "n";
                            else
                                dr["isVisible"] = "y";
                            string key = (from e in dtSettings.AsEnumerable()
                                          where e.Field<string>("DBColName") == colName
                                          && e.Field<string>("fieldType") == "P"
                                          select e.Field<string>("DisplayName")).SingleOrDefault();
                            string colType = (from e in dtSettings.AsEnumerable()
                                              where e.Field<string>("DBColName") == colName
                                              && e.Field<string>("fieldType") == "P"
                                              select e.Field<string>("ColType")).SingleOrDefault();

                            dr["key"] = key;
                            dr["colType"] = colType;
                            dr["Value"] = result.Tables[1].Rows[i][j].ToString();
                            dtPersonal.Rows.Add(dr);
                        }
                    }

                    //=============================== Get Buisness Fields ===========================================
                    DataTable dtBuisness = new DataTable();
                    dtBuisness.Columns.Add("fieldID");
                    dtBuisness.Columns.Add("Uniquekey");
                    dtBuisness.Columns.Add("Value");
                    dtBuisness.Columns.Add("ColType");
                    dtBuisness.Columns.Add("key");
                    dtBuisness.Columns.Add("isEditable");
                    dtBuisness.Columns.Add("isVisible");
                    for (int i = 0; i < result.Tables[2].Rows.Count; i++)
                    {
                        for (int j = 0; j < result.Tables[2].Columns.Count; j++)
                        {
                            string colName = result.Tables[2].Columns[j].ColumnName;

                            DataRow dr = dtBuisness.NewRow();
                            string isVisible = (from e in dtSettings.AsEnumerable()
                                                where e.Field<string>("DBColName") == colName
                                                && e.Field<string>("fieldType") == "B"
                                                select e.Field<string>("isVisible")).SingleOrDefault();

                            if (isVisible == "y")
                            {
                                dr["fieldID"] = "0";
                                dr["Uniquekey"] = colName;
                                dr["isEditable"] = "1";
                                dr["isVisible"] = isVisible;
                                string query = (from lookup in dtSettings.AsEnumerable()
                                                where lookup.Field<string>("DBColName") == colName
                                                && lookup.Field<string>("fieldType") == "B"
                                                select lookup.Field<string>("DisplayName")).SingleOrDefault();
                                string colType = (from e in dtSettings.AsEnumerable()
                                                  where e.Field<string>("DBColName") == colName
                                                  && e.Field<string>("fieldType") == "B"
                                                  select e.Field<string>("ColType")).SingleOrDefault();
                                dr["key"] = query;
                                dr["colType"] = colType;
                                dr["Value"] = result.Tables[2].Rows[i][j].ToString();
                                dtBuisness.Rows.Add(dr);
                            }
                        }
                    }

                    //=============================== Get Dynamic Fields ===========================================
                    if (dtDynamicFields != null)
                    {
                        if (dtDynamicFields.Rows.Count > 0)
                        {
                            for (int i = 0; i < dtDynamicFields.Rows.Count; i++)
                            {
                                if (dtDynamicFields.Rows[i]["FieldCategory"].ToString() == "P")
                                {
                                    DataRow dr = dtPersonal.NewRow();
                                    dr["fieldID"] = dtDynamicFields.Rows[i]["pk_fieldID"].ToString();
                                    dr["Uniquekey"] = dtDynamicFields.Rows[i]["DisplayName"].ToString();
                                    dr["key"] = dtDynamicFields.Rows[i]["DisplayName"].ToString();
                                    dr["colType"] = dtDynamicFields.Rows[i]["fieldType"].ToString();
                                    dr["Value"] = dtDynamicFields.Rows[i]["FieldValue"].ToString();
                                    dr["isEditable"] = dtDynamicFields.Rows[i]["IsEditableByMember"].ToString();
                                    dtPersonal.Rows.Add(dr);
                                }
                                else
                                {
                                    DataRow dr = dtBuisness.NewRow();
                                    dr["fieldID"] = dtDynamicFields.Rows[i]["pk_fieldID"].ToString();
                                    dr["Uniquekey"] = dtDynamicFields.Rows[i]["DisplayName"].ToString();
                                    dr["key"] = dtDynamicFields.Rows[i]["DisplayName"].ToString();
                                    dr["colType"] = dtDynamicFields.Rows[i]["fieldType"].ToString();
                                    dr["Value"] = dtDynamicFields.Rows[i]["FieldValue"].ToString();
                                    dr["isEditable"] = dtDynamicFields.Rows[i]["IsEditableByMember"].ToString();
                                    dr["isVisible"] = "y";
                                    dtBuisness.Rows.Add(dr);
                                }
                            }
                        }
                    }

                    Memberdtls[0].personalMemberDetails = new List<PersonalMemberDetil>(0);
                    if (dtPersonal.Rows.Count > 0)
                    {
                        List<PersonalMemberDetil> personalMemberDetail = GlobalFuns.DataTableToList<PersonalMemberDetil>(dtPersonal);
                        Memberdtls[0].personalMemberDetails = personalMemberDetail;
                    }

                    Memberdtls[0].businessMemberDetails = new List<BusinessMemberDetail>(0);
                    if (dtBuisness.Rows.Count > 0)
                    {
                        List<BusinessMemberDetail> BusinessMemberDetail = GlobalFuns.DataTableToList<BusinessMemberDetail>(dtBuisness);
                        Memberdtls[0].businessMemberDetails = BusinessMemberDetail;
                    }


                    //=============================== Get Address Fields ===========================================
                    AddressList memberAddress = new AddressList();
                    List<AddressResult> addressList = new List<AddressResult>(0);
                    memberAddress.isResidanceAddrVisible = "n";
                    //memberAddress.isResidanceAddrVisible = (from e in dtSettings.AsEnumerable()
                    //                                        where e.Field<string>("DBColName") == "Residance_Address"
                    //                                        && e.Field<string>("fieldType") == "P"
                    //                                        select e.Field<string>("isVisible")).SingleOrDefault();

                    memberAddress.isBusinessAddrVisible = (from e in dtSettings.AsEnumerable()
                                                           where e.Field<string>("DBColName") == "Business_Address"
                                                           && e.Field<string>("fieldType") == "B"
                                                           select e.Field<string>("isVisible")).SingleOrDefault();
                    if (dtAddress.Rows.Count > 0)
                    {
                        addressList = GlobalFuns.DataTableToList<AddressResult>(dtAddress);
                    }
                    memberAddress.addressResult = addressList;
                    Memberdtls[0].addressDetails = memberAddress;

                    //=============================== Get Family Fields ===========================================
                    FamilyMemberDetails familyDtl = new FamilyMemberDetails();
                    List<FamilyMemberDetail> FamilyMemberlist = new List<FamilyMemberDetail>(0);
                    familyDtl.isVisible = "n";
                    familyDtl.familyMemberDetail = FamilyMemberlist;
                    Memberdtls[0].familyMemberDetails = familyDtl;
                   
                    //familyDtl.isVisible = (from e in dtSettings.AsEnumerable()
                    //                       where e.Field<string>("DBColName") == "member_name"
                    //                       && e.Field<string>("fieldType") == "F"
                    //                       select e.Field<string>("isVisible")).SingleOrDefault();
                    //if (familyDtl.isVisible == "y")
                    //{
                    //    FamilyMemberlist = GlobalFuns.DataTableToList<FamilyMemberDetail>(dtFamilyMember);
                    //}
                    //familyDtl.familyMemberDetail = FamilyMemberlist;
                    //Memberdtls[0].familyMemberDetails = familyDtl;
                }
                return Memberdtls[0];
            }
            catch
            {
                throw;
            }
        }
  
        public static MemberListSyncResult GetMemberListSync(string updatedOn, string grpID, out string filepath)
        {
            bool isDataFound = false;
            string targetPath = "";
            try
            {
                filepath = "";
                MySqlParameter[] param = new MySqlParameter[2];
                param[0] = new MySqlParameter("@groupID", grpID);
                param[1] = new MySqlParameter("@updatedOn", updatedOn);
                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V7_USPGetDirectoryListSync", param);

                DataTable dtnewMember = result.Tables[0];
                DataTable dtUpdatedMember = result.Tables[1];
                DataTable dtDeletedMember = result.Tables[2];

                int dataCount = 10;// Used for paging. DataCount is page size
                List<MemberDetailsDynamicField> memberList;
                int Filecounter = 1;
                int totalrows = dtnewMember.Rows.Count;
                string FileName = "Dist-"+ grpID + DateTime.Now.ToString("ddMMyyyyhhmmsstt");

                MemberListSyncResult memListSyncResult = new MemberListSyncResult();

                //======================= If total records less than 20 Return Json directly =================================================
                int totalRowCount;
                if (Convert.ToDateTime(updatedOn).Date.ToString("dd/MM/yyyy") == "01/01/1970")
                    totalRowCount = dtnewMember.Rows.Count;
                else
                    totalRowCount = dtnewMember.Rows.Count + dtUpdatedMember.Rows.Count;

                if (totalRowCount <= 20)
                {
                    //---------------Create array of new members -------------------------------
                    memberList = new List<MemberDetailsDynamicField>();
                    if (dtnewMember.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtnewMember.Rows.Count; i++)
                        {
                            MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeild(dtnewMember.Rows[i]["profileID"].ToString(), grpID);
                            memberList.Add(memberdtl);
                        }
                    }
                    memListSyncResult.NewMemberList = memberList;

                    //---------------Create array of Updated members -------------------------------
                    memberList = new List<MemberDetailsDynamicField>();
                    if (dtUpdatedMember.Rows.Count > 0 && Convert.ToDateTime(updatedOn).Date.ToString("dd/MM/yyyy") != "01/01/1970")
                    {
                        for (int i = 0; i < dtUpdatedMember.Rows.Count; i++)
                        {
                            MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeild(dtUpdatedMember.Rows[i]["profileID"].ToString(), grpID);
                            memberList.Add(memberdtl);
                        }
                    }
                    memListSyncResult.UpdatedMemberList = memberList;

                    //-----------------Deleted member --------------------------------------------------
                    string deletedProfiles = "";
                    if (dtDeletedMember != null && Convert.ToDateTime(updatedOn).Date.ToString("dd/MM/yyyy") != "01/01/1970")
                    {
                        if (!string.IsNullOrEmpty(dtDeletedMember.Rows[0]["profileID"].ToString()))
                        {
                            deletedProfiles = dtDeletedMember.Rows[0]["profileID"].ToString();
                        }
                    }
                    memListSyncResult.DeletedMemberList = deletedProfiles;
                }

                //======================= If Records Greater than 20 Return Zip File =================================================
                else
                {
                    if (dtnewMember.Rows.Count > 0)
                    {
                        memberList = new List<MemberDetailsDynamicField>();
                        for (int i = 0; i < totalrows; i++)
                        {
                            if (i < dataCount) // add object to page
                            {
                                try
                                {
                                    MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeild(dtnewMember.Rows[i]["profileID"].ToString(), grpID);
                                    memberList.Add(memberdtl);
                                    MemberDetails memberDtl = new MemberDetails();
                                    memberDtl.MemberDetail = memberList;

                                    // For each page create a seperate json file
                                    if (i == dataCount - 1 || i == totalrows - 1)
                                    {
                                        // create a file
                                        string json = JsonConvert.SerializeObject(memberDtl);
                                        //write string to file

                                        string Path = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments\\DirectoryData\\Profile" + FileName;

                                        if (!Directory.Exists(Path))
                                            Directory.CreateDirectory(Path);

                                        if (!Directory.Exists(Path + "/NewMembers"))
                                            Directory.CreateDirectory(Path + "/NewMembers");

                                        System.IO.File.WriteAllText(Path + "/NewMembers/New" + Filecounter + ".json", json);
                                        Filecounter++;
                                    }
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                // reset member list for next page
                                memberList = new List<MemberDetailsDynamicField>();
                                dataCount = dataCount + 10;
                                i--;
                            }
                        }
                    }

                    if (dtUpdatedMember.Rows.Count > 0 && Convert.ToDateTime(updatedOn).Date.ToString("dd/MM/yyyy") != "01/01/1970")
                    {
                        dataCount = 10; // Used for paging. dataCount is page size
                        memberList = new List<MemberDetailsDynamicField>();
                        Filecounter = 1;
                        totalrows = dtUpdatedMember.Rows.Count;
                        for (int i = 0; i < totalrows; i++)
                        {
                            if (i < dataCount)
                            {
                                try
                                {
                                    MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeild(dtUpdatedMember.Rows[i]["profileID"].ToString(), grpID);
                                    memberList.Add(memberdtl);
                                    MemberDetails memberDtl = new MemberDetails();
                                    memberDtl.MemberDetail = memberList;

                                    // For each page create a seperate json file
                                    if (i == dataCount - 1 || i == totalrows - 1)
                                    {
                                        // create a file
                                        string json = JsonConvert.SerializeObject(memberDtl);
                                        //write string to file

                                        string Path = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments\\DirectoryData\\Profile" + FileName;

                                        if (!Directory.Exists(Path))
                                            Directory.CreateDirectory(Path);

                                        if (!Directory.Exists(Path + "/UpdatedMembers"))
                                            Directory.CreateDirectory(Path + "/UpdatedMembers");

                                        System.IO.File.WriteAllText(Path + "/UpdatedMembers/Update" + Filecounter + ".json", json);
                                        Filecounter++;
                                    }
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                // reset member list for next page
                                memberList = new List<MemberDetailsDynamicField>();
                                dataCount = dataCount + 10;
                                i--;
                            }
                        }
                    }

                    if (dtDeletedMember != null && Convert.ToDateTime(updatedOn).Date.ToString("dd/MM/yyyy") != "01/01/1970")
                    {
                        if (!string.IsNullOrEmpty(dtDeletedMember.Rows[0]["profileID"].ToString()))
                        {
                            string deletedProfiles = dtDeletedMember.Rows[0]["profileID"].ToString();
                            string Path = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments\\DirectoryData\\Profile" + FileName;

                            if (!Directory.Exists(Path))
                                Directory.CreateDirectory(Path);

                            if (!Directory.Exists(Path + "/DeletedMembers"))
                                Directory.CreateDirectory(Path + "/DeletedMembers");

                            System.IO.File.WriteAllText(Path + "/DeletedMembers/Delete" + Filecounter + ".txt", deletedProfiles);
                        }
                    }

                    string zipFolderPath = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments\\DirectoryData\\Profile" + FileName;
                    if (Directory.Exists(zipFolderPath))
                    {
                        targetPath = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments\\DirectoryData\\Profile" + FileName + ".zip";
                        zos = new ZipOutputStream(File.Create(targetPath));
                        zos.UseZip64 = UseZip64.Off;
                        strBaseDir = zipFolderPath + "\\";
                        AddZipEntry(strBaseDir);
                        zos.Finish();
                        zos.Close();
                        isDataFound = true;
                    }
                    if (isDataFound)
                        filepath = ConfigurationManager.AppSettings["imgPath"] + "TempDocuments/DirectoryData/Profile" + FileName + ".zip";
                }
                return memListSyncResult;
            }
            catch
            {
                throw;
            }
        }

        protected static void AddZipEntry(string PathStr)
        {
            DirectoryInfo di = new DirectoryInfo(PathStr);
            foreach (DirectoryInfo item in di.GetDirectories())
            {
                AddZipEntry(item.FullName);
            }
            foreach (FileInfo item in di.GetFiles())
            {
                FileStream fs = File.OpenRead(item.FullName);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                string strEntryName = item.FullName.Replace(strBaseDir, "");
                ZipEntry entry = new ZipEntry(strEntryName);
                zos.PutNextEntry(entry);
                zos.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
        }

        public static string GetZipFilePath(string updatedOn, string grpID,out DateTime creationDate)
        {
            string filepath = "";
            try
            {
                string sql = "SELECT ZipfilePath as path ,Creation_date FROM row_production.district_directory where fk_group_master_id=" + grpID;
                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.Text, sql);
                if (result != null)
                {
                    if (result.Tables[0].Rows.Count > 0 )
                    {
                        filepath = ConfigurationManager.AppSettings["imgPath"] + "TempDocuments/DirectoryData/" + result.Tables[0].Rows[0]["path"].ToString();
                        creationDate =Convert.ToDateTime(result.Tables[0].Rows[0]["Creation_date"].ToString());
                    }
                    else
                    {
                        creationDate = System.DateTime.Now;
                    }
                }
                else
                {
                    creationDate = System.DateTime.Now;
                }
            }
            catch
            {
                throw;
            }
            return filepath;
        }

        public static List<Club> GetDistrictClubs(string groupId, string searchStr)
        {
            try
            {
                var GrpId = new MySqlParameter("?groupId", groupId);
                var search = new MySqlParameter("?search", searchStr);
                List<Club> Result = _DBTouchbase.ExecuteStoreQuery<Club>("CALL V1_USPGetDistrictClubs(?groupId,?search)", GrpId,search).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<DistrictCommitteeResult> GetDistrictCommittee(string grpId, string searchText)
        {
            List<DistrictCommitteeResult> result = new List<DistrictCommitteeResult>();
            MySqlParameter[] parameters = new MySqlParameter[2];
            parameters[0] = new MySqlParameter("?groupId", grpId);
            parameters[1] = new MySqlParameter("?searchText", string.IsNullOrEmpty(searchText) ? "" : searchText.Replace(' ', '%'));
            try
            {
                result = _DBTouchbase.ExecuteStoreQuery<DistrictCommitteeResult>("CALL V1_USPGetDistrictCommittee(?groupId,?searchText)", parameters).ToList();
                foreach (DistrictCommitteeResult mem in result)
                {
                    if (!string.IsNullOrEmpty(mem.pic))
                    {
                        string ImageName = mem.pic.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                        mem.pic = path + ImageName;
                    }
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        public static List<SubGroupList> GetDistrictSubGroup(string GrpID, string subgrpType)
        {
            try
            {
                var grpId = new MySqlParameter("?grpId", GrpID);
                var subGrpType = new MySqlParameter("?subgrpType", subgrpType);
                List<SubGroupList> Result = _DBTouchbase.ExecuteStoreQuery<SubGroupList>("CALL V6_USPDistGetSubGroupList(?grpId,?subGrpType)", grpId, subGrpType).ToList();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
     
        }

        /// <summary>
        /// Added By Madhavi Patil(23 March 2018)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public static List<DistrictMemListResult> GetDistrictMemberList(DistrictMemListInput obj, out int totalPages)
        {
            List<DistrictMemListResult> memberList = new List<DistrictMemListResult>();
            try
            {
                if (!string.IsNullOrEmpty(obj.searchText))
                {
                    obj.searchText = obj.searchText.Replace(" ", "%");
                }
                MySqlParameter[] param = new MySqlParameter[6];
                param[0] = new MySqlParameter("?masterUID", obj.masterUID);
                param[1] = new MySqlParameter("?groupID", obj.grpID);
                param[2] = new MySqlParameter("?SearchText", string.IsNullOrEmpty(obj.searchText) ? "" : obj.searchText);
                param[3] = new MySqlParameter("?PageNo", string.IsNullOrEmpty(obj.pageNo) ? "1" : obj.pageNo);
                param[4] = new MySqlParameter("?NoOfRecord", string.IsNullOrEmpty(obj.recordCount) ? "100" : obj.recordCount);
                param[5] = new MySqlParameter("?TotalCount", DbType.Int32);
                param[5].Direction = ParameterDirection.InputOutput;
                DataSet dsMember = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V7_USPGetDistrictDirectoryList", param);
                totalPages = Convert.ToInt32(param[5].Value);
                if (dsMember != null)
                {
                    if (dsMember.Tables[0].Rows.Count > 0)
                    {
                        memberList = GlobalFuns.DataTableToList<DistrictMemListResult>(dsMember.Tables[0]);

                        foreach (DistrictMemListResult mem in memberList)
                        {
                            if (!string.IsNullOrEmpty(mem.pic))
                            {
                                string ImageName = mem.pic.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                                mem.pic = path + ImageName;
                            }
                        }
                    }
                }

            }
            catch
            {
                throw;
            }
            return memberList;
        }


        public static List<classificationOutput> GetClassificationList(DistrictMemListInput obj, out int totalPages)
        {
            List<classificationOutput> Result = new List<classificationOutput>();
            try
            {
                MySqlParameter[] param = new MySqlParameter[5];
                param[0] = new MySqlParameter("?grpID", obj.grpID);
                param[1] = new MySqlParameter("?PageNo", string.IsNullOrEmpty(obj.pageNo) ? "1" : obj.pageNo);
                param[2] = new MySqlParameter("?NoOfRecord", string.IsNullOrEmpty(obj.recordCount) ? "100" : obj.recordCount);
                param[3] = new MySqlParameter("?TotalCount", DbType.Int32);
                param[4] = new MySqlParameter("?SearchText", string.IsNullOrEmpty(obj.searchText) ? "" : obj.searchText);
                param[3].Direction = ParameterDirection.Output;
                DataSet dsClassification = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V7GetDistrictClassifications", param);
                totalPages = Convert.ToInt32(param[3].Value);
                if (dsClassification != null)
                {
                    if (dsClassification.Tables[0].Rows.Count > 0)
                    {
                        Result = GlobalFuns.DataTableToList<classificationOutput>(dsClassification.Tables[0]);
                    }
                }
            }
            catch
            {
                throw;
            }
            return Result;
        }


        public static List<classificationOutput> GetClassificationList_New(DistrictMemListInput obj, out int totalPages)
        {
            List<classificationOutput> Result = new List<classificationOutput>();
            try
            {
                MySqlParameter[] param = new MySqlParameter[5];
                param[0] = new MySqlParameter("?grpID", obj.grpID);
                param[1] = new MySqlParameter("?PageNo", string.IsNullOrEmpty(obj.pageNo) ? "1" : obj.pageNo);
                param[2] = new MySqlParameter("?NoOfRecord", string.IsNullOrEmpty(obj.recordCount) ? "100" : obj.recordCount);
                param[3] = new MySqlParameter("?TotalCount", DbType.Int32);
                param[4] = new MySqlParameter("?SearchText", string.IsNullOrEmpty(obj.searchText) ? "" : obj.searchText);
                param[3].Direction = ParameterDirection.Output;
                DataSet dsClassification = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V8GetDistrictClassifications", param);
                totalPages = Convert.ToInt32(param[3].Value);
                if (dsClassification != null)
                {
                    if (dsClassification.Tables[0].Rows.Count > 0)
                    {
                        Result = GlobalFuns.DataTableToList<classificationOutput>(dsClassification.Tables[0]);
                    }
                }
            }
            catch
            {
                throw;
            }
            return Result;
        }

        public static List<DistrictMemListResult> GetMemberByClassification(string classification, string groupID)
        {
            List<DistrictMemListResult> memberList = new List<DistrictMemListResult>();
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];
                param[0] = new MySqlParameter("?classification", classification.Replace(" ", "%"));
                param[1] = new MySqlParameter("?groupID", groupID);
                DataSet dsMember = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V7_USPGetMemberByClassification", param);

                if (dsMember != null)
                {
                    if (dsMember.Tables[0].Rows.Count > 0)
                    {
                        memberList = GlobalFuns.DataTableToList<DistrictMemListResult>(dsMember.Tables[0]);

                        foreach (DistrictMemListResult mem in memberList)
                        {
                            if (!string.IsNullOrEmpty(mem.pic))
                            {
                                string ImageName = mem.pic.ToString();
                                string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                                mem.pic = path + ImageName;
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return memberList;
        }
    }
}