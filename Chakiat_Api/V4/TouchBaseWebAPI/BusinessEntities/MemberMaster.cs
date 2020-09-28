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
    public class MemberMaster
    {
        //private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();
        //private static MemoryStream ms;
        private static ZipOutputStream zos;
        private static string strBaseDir;

        public static List<MemberDetail> GetMember(string grpuserID, string grpID)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];
                param[0] = new MySqlParameter("@GrpUserId", grpuserID);
                param[1] = new MySqlParameter("@GrpID", grpID);
                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "USPGetMemberDetails", param);

                DataTable dtMember = result.Tables[0];
                DataTable dtAddress = result.Tables[3];
                DataTable dtFamilyMember = result.Tables[4];
                List<MemberDetail> MemberDetail = new List<MemberDetail>();

                if (dtMember.Rows.Count > 0)
                {
                    MemberDetail = GlobalFuns.DataTableToList<MemberDetail>(dtMember);

                    foreach (MemberDetail mem in MemberDetail)
                    {
                        if (!string.IsNullOrEmpty(mem.profilePic))
                        {
                            string ImageName = mem.profilePic.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                            mem.profilePic = path + ImageName;
                        }
                    }

                    DataTable dtPersonal = new DataTable();
                    dtPersonal.Columns.Add("Key");
                    dtPersonal.Columns.Add("Value");
                    dtPersonal.Columns.Add("ColType");
                    dtPersonal.Columns.Add("Uniquekey");
                    for (int i = 0; i < result.Tables[1].Rows.Count; i++)
                    {
                        for (int j = 0; j < result.Tables[1].Columns.Count; j++)
                        {
                            DataRow dr = dtPersonal.NewRow();
                            string colName = result.Tables[1].Columns[j].ColumnName;
                            dr["Uniquekey"] = colName;
                            DataTable dt = result.Tables[5];
                            string key = (from e in dt.AsEnumerable()
                                          where e.Field<string>("DBColumnName") == colName
                                          select e.Field<string>("DisplayName")).SingleOrDefault();
                            string colType = (from e in dt.AsEnumerable()
                                              where e.Field<string>("DBColumnName") == colName
                                              select e.Field<string>("ColType")).SingleOrDefault();
                            dr["key"] = key;
                            dr["colType"] = colType;
                            dr["Value"] = result.Tables[1].Rows[i][j].ToString();
                            dtPersonal.Rows.Add(dr);

                        }
                    }

                    DataTable dtBuisness = new DataTable();
                    dtBuisness.Columns.Add("Uniquekey");
                    dtBuisness.Columns.Add("Value");
                    dtBuisness.Columns.Add("ColType");
                    dtBuisness.Columns.Add("key");
                    for (int i = 0; i < result.Tables[2].Rows.Count; i++)
                    {
                        for (int j = 0; j < result.Tables[2].Columns.Count; j++)
                        {
                            DataRow dr = dtBuisness.NewRow();
                            string colName = result.Tables[2].Columns[j].ColumnName;
                            dr["Uniquekey"] = colName;
                            string query = (from lookup in result.Tables[5].AsEnumerable()
                                            where lookup.Field<string>("DBColumnName") == colName
                                            select lookup.Field<string>("DisplayName")).SingleOrDefault();
                            string colType = (from e in result.Tables[5].AsEnumerable()
                                              where e.Field<string>("DBColumnName") == colName
                                              select e.Field<string>("ColType")).SingleOrDefault();
                            dr["key"] = query;
                            dr["colType"] = colType;
                            dr["Value"] = result.Tables[2].Rows[i][j].ToString();
                            dtBuisness.Rows.Add(dr);

                        }
                    }

                    MemberDetail[0].familyMemberDetails = new List<object>();
                    MemberDetail[0].addressDetails = new List<object>();
                    MemberDetail[0].personalMemberDetails = new List<object>();
                    MemberDetail[0].BusinessMemberDetails = new List<object>();
                    if (dtAddress.Rows.Count > 0)
                    {
                        List<AddressResult> AddressList = GlobalFuns.DataTableToList<AddressResult>(dtAddress);
                        for (int i = 0; i < AddressList.Count; i++)
                        {
                            MemberDetail[0].addressDetails.Add(new { Address = (object)AddressList[i] });
                        }
                    }

                    if (dtFamilyMember.Rows.Count > 0)
                    {
                        List<FamilyMemberDetail> FamilyMemberDetil = GlobalFuns.DataTableToList<FamilyMemberDetail>(dtFamilyMember);
                        for (int i = 0; i < FamilyMemberDetil.Count; i++)
                        {
                            MemberDetail[0].familyMemberDetails.Add(new { FamilyMemberDetil = (object)FamilyMemberDetil[i] });
                        }
                    }

                    if (dtPersonal.Rows.Count > 0)
                    {
                        List<PersonalMemberDetil> personalMemberDetail = GlobalFuns.DataTableToList<PersonalMemberDetil>(dtPersonal);
                        for (int i = 0; i < personalMemberDetail.Count; i++)
                        {
                            MemberDetail[0].personalMemberDetails.Add(new { PersonalMemberDetil = (object)personalMemberDetail[i] });
                        }
                    }
                    if (dtBuisness.Rows.Count > 0)
                    {
                        List<BusinessMemberDetail> BusinessMemberDetail = GlobalFuns.DataTableToList<BusinessMemberDetail>(dtBuisness);
                        for (int i = 0; i < BusinessMemberDetail.Count; i++)
                        {
                            MemberDetail[0].BusinessMemberDetails.Add(new { BusinessMemberDetail = (object)BusinessMemberDetail[i] });
                        }
                    }
                }

                return MemberDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        # region dynamic Fields -- Added by Rupali 15 feb 2017

        public static MemberListSyncResult GetMemberListSyncIOS(string updatedOn, string grpID, out string filepath, out string FileName)
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

                //int dataCount = 10;// Used for paging. DataCount is page size
                List<MemberDetailsDynamicField> memberList;
                //int Filecounter = 1;
                int totalrows = dtnewMember.Rows.Count;
                FileName = DateTime.Now.ToString("ddMMyyyyhhmmsstt");

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
                        MemberDetails memberDtl = new MemberDetails();
                        for (int i = 0; i < totalrows; i++)
                        {

                            try
                            {
                                MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeild(dtnewMember.Rows[i]["profileID"].ToString(), grpID);
                                memberList.Add(memberdtl);
                                memberDtl.MemberDetail = memberList;
                            }
                            catch
                            {
                            }
                        }
                        string json = JsonConvert.SerializeObject(memberDtl);
                        //write string to file

                        string Path = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments\\DirectoryData\\Profile" + FileName;

                        if (!Directory.Exists(Path))
                            Directory.CreateDirectory(Path);

                        if (!Directory.Exists(Path + "/NewMembers"))
                            Directory.CreateDirectory(Path + "/NewMembers");

                        System.IO.File.WriteAllText(Path + "/NewMembers/New" + ".json", json);

                    }
                    if (dtUpdatedMember.Rows.Count > 0 && Convert.ToDateTime(updatedOn).Date.ToString("dd/MM/yyyy") != "01/01/1970")
                    {

                        memberList = new List<MemberDetailsDynamicField>();
                        MemberDetails memberDtl = new MemberDetails();
                        totalrows = dtUpdatedMember.Rows.Count;
                        for (int i = 0; i < totalrows; i++)
                        {
                            try
                            {
                                MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeild(dtUpdatedMember.Rows[i]["profileID"].ToString(), grpID);
                                memberList.Add(memberdtl);
                                memberDtl.MemberDetail = memberList;
                            }
                            catch
                            {
                            }
                        }
                        // create a file
                        string json = JsonConvert.SerializeObject(memberDtl);
                        //write string to file

                        string Path = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments\\DirectoryData\\Profile" + FileName;

                        if (!Directory.Exists(Path))
                            Directory.CreateDirectory(Path);

                        if (!Directory.Exists(Path + "/UpdatedMembers"))
                            Directory.CreateDirectory(Path + "/UpdatedMembers");

                        System.IO.File.WriteAllText(Path + "/UpdatedMembers/Update" + ".json", json);

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

                            System.IO.File.WriteAllText(Path + "/DeletedMembers/Delete" + ".txt", deletedProfiles);
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

        public static MemberDetailsDynamicField GetMemberDtlWithDynamicFeild(string grpuserID, string grpID)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];
                param[0] = new MySqlParameter("@profileId", grpuserID);
                param[1] = new MySqlParameter("@GrpID", grpID);

                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V6_USPGetMemberDetails", param);
                DataTable dtMember = result.Tables[0];
                DataTable dtAddress = result.Tables[3];
                DataTable dtFamilyMember = result.Tables[4];
                DataTable dtSettings = result.Tables[5];
                DataTable dtDynamicFields = result.Tables[6];

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
                    memberAddress.isResidanceAddrVisible = (from e in dtSettings.AsEnumerable()
                                                            where e.Field<string>("DBColName") == "Residance_Address"
                                                            && e.Field<string>("fieldType") == "P"
                                                            select e.Field<string>("isVisible")).SingleOrDefault();

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
                    familyDtl.isVisible = (from e in dtSettings.AsEnumerable()
                                           where e.Field<string>("DBColName") == "member_name"
                                           && e.Field<string>("fieldType") == "F"
                                           select e.Field<string>("isVisible")).SingleOrDefault();
                    if (familyDtl.isVisible == "y")
                    {
                        FamilyMemberlist = GlobalFuns.DataTableToList<FamilyMemberDetail>(dtFamilyMember);
                    }
                    familyDtl.familyMemberDetail = FamilyMemberlist;
                    Memberdtls[0].familyMemberDetails = familyDtl;
                }
                return Memberdtls[0];
            }
            catch
            {
                throw;
            }
        }

        public static MemberDetailsDynamicField GetMemberDtlWithDynamicFeild_Test(string grpuserID, string grpID)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];
                param[0] = new MySqlParameter("@profileId", grpuserID);
                param[1] = new MySqlParameter("@GrpID", grpID);

                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V6_USPGetMemberDetails_Copy", param);
                DataTable dtMember = result.Tables[0];
                DataTable dtAddress = result.Tables[3];
                DataTable dtFamilyMember = result.Tables[4];
                DataTable dtSettings = result.Tables[5];
                DataTable dtDynamicFields = result.Tables[6];

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
                    memberAddress.isResidanceAddrVisible = (from e in dtSettings.AsEnumerable()
                                                            where e.Field<string>("DBColName") == "Residance_Address"
                                                            && e.Field<string>("fieldType") == "P"
                                                            select e.Field<string>("isVisible")).SingleOrDefault();

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
                    familyDtl.isVisible = (from e in dtSettings.AsEnumerable()
                                           where e.Field<string>("DBColName") == "member_name"
                                           && e.Field<string>("fieldType") == "F"
                                           select e.Field<string>("isVisible")).SingleOrDefault();
                    if (familyDtl.isVisible == "y")
                    {
                        FamilyMemberlist = GlobalFuns.DataTableToList<FamilyMemberDetail>(dtFamilyMember);
                    }
                    familyDtl.familyMemberDetail = FamilyMemberlist;
                    Memberdtls[0].familyMemberDetails = familyDtl;
                }
                return Memberdtls[0];
            }
            catch
            {
                throw;
            }
        }

        public static MemberDetailsDynamicField GetMemberDtlWithDynamicFeildByDatatables(string grpuserID, DataTable dt_MemberDetails, DataTable dt_FilterNewPersonal, DataTable dt_FilterNewBuisness, DataTable dt_Address, DataTable dt_FamilyDetailsm, DataTable dt_Setting, DataTable dt_Dynamicfields)
        {
            try
            {
                //MySqlParameter[] param = new MySqlParameter[2];
                //param[0] = new MySqlParameter("@profileId", grpuserID);
                //param[1] = new MySqlParameter("@GrpID", grpID);

                //DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V6_USPGetMemberDetails_Copy", param);

                //DataTable dtMember = result.Tables[0];
                //DataTable dtAddress = result.Tables[3];
                //DataTable dtFamilyMember = result.Tables[4];
                //DataTable dtSettings = result.Tables[5];
                //DataTable dtDynamicFields = result.Tables[6];

                DataTable dtMember = dt_MemberDetails;
                DataTable dtAddress = dt_Address;
                DataTable dtFamilyMember = dt_FamilyDetailsm;
                DataTable dtSettings = dt_Setting;
                DataTable dtDynamicFields = dt_Dynamicfields;

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

                    for (int i = 0; i < dt_FilterNewPersonal.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt_FilterNewPersonal.Columns.Count; j++)
                        {

                            string colName = dt_FilterNewPersonal.Columns[j].ColumnName;
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
                            dr["Value"] = dt_FilterNewPersonal.Rows[i][j].ToString();
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
                    for (int i = 0; i < dt_FilterNewBuisness.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt_FilterNewBuisness.Columns.Count; j++)
                        {
                            string colName = dt_FilterNewBuisness.Columns[j].ColumnName;

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
                                dr["Value"] = dt_FilterNewBuisness.Rows[i][j].ToString();
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
                    memberAddress.isResidanceAddrVisible = (from e in dtSettings.AsEnumerable()
                                                            where e.Field<string>("DBColName") == "Residance_Address"
                                                            && e.Field<string>("fieldType") == "P"
                                                            select e.Field<string>("isVisible")).SingleOrDefault();

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
                    familyDtl.isVisible = (from e in dtSettings.AsEnumerable()
                                           where e.Field<string>("DBColName") == "member_name"
                                           && e.Field<string>("fieldType") == "F"
                                           select e.Field<string>("isVisible")).SingleOrDefault();
                    if (familyDtl.isVisible == "y")
                    {
                        FamilyMemberlist = GlobalFuns.DataTableToList<FamilyMemberDetail>(dtFamilyMember);
                    }
                    familyDtl.familyMemberDetail = FamilyMemberlist;
                    Memberdtls[0].familyMemberDetails = familyDtl;
                }
                return Memberdtls[0];
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        private static string getLastVisitedDatatime(string profileID)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];

                param[0] = new MySqlParameter("?profileID", profileID);

                string sqlQuery = "select IFNULL(Last_Visited_On, '1970-01-01 00:00:00') as Last_Visited_On from main_member_master where pk_main_member_master_id =@profileID; ";

                string result = Convert.ToString(MySqlHelper.ExecuteScalar(GlobalVar.strAppConn, CommandType.Text, sqlQuery.ToString(), param));

                result = Convert.ToDateTime(result).ToString("yyyy/MM/dd HH:mm:ss").Replace("-","/");

                return result;
            }
            catch (Exception)
            {
                return "1970/01/01 00:00:00";
            }
        }

        public static MemberListSyncResult GetMemberListSync(string updatedOn,string grpID, out string filepath)
        {
            #region this is new code written by Mukesh Dhole on 15 jun 2018
            bool isDataFound = false;
            string targetPath = "";
            DataSet result;
            try
            {

                //Code By Nikhil //To fetch last Visited datetime .             
                //updatedOn = getLastVisitedDatatime(profileId);

                updatedOn = Convert.ToDateTime(updatedOn).ToString("yyyy/MM/dd HH:mm:ss").Replace("-", "/");

                //DataSet result;
                CallForMemberDetails(updatedOn, grpID, out filepath, out result);
                
                #region Here get all member details
                //get distinct New member id
                DataView vwnewMember = new DataView(result.Tables[0]);
                DataTable dtnewMember = vwnewMember.ToTable(true, "profileID");

                //get distinct New member details 
                DataView vw_newMemberDetails = new DataView(result.Tables[1]);
                DataTable dt_newMemberDetails = vw_newMemberDetails.ToTable(true, "profileID", "masterID", "grpID", "Salutation", "memberName", "memberEmail", "memberMobile", "memberCountry", "profilePic", "isAdmin", "isPersonalDetVisible", "Branch_Id", "Branch_Name");

                //get distinct New member personal details
                DataView vw_newPersonal = new DataView(result.Tables[2]);
                DataTable dt_newPersonal = vw_newPersonal.ToTable(true, "profileID", "Salutation", "member_name", "member_email_id", "memberMobile", "memberCountry", "member_date_of_birth", "member_date_of_wedding", "Employee_Code_One", "First_Level_of_Reporting_Name", "Employee_Code_2", "Second_LevelofReportingName", "UHID", "Official_Landline_No", "Second_Official_Landline_No", "Date_of_Joining", "Fk_Dept_ID", "Fk_FunctionalRole_Id", "fk_EntityName_id", "fk_designation_id", "department_Name", "functionalrole_Name", "Entity_Name", "Designation_Name", "Employee_Code", "Branch_Id", "Branch_Name");

                //get distinct New member Buisness details
                DataView vw_newBuisness = new DataView(result.Tables[3]);
                DataTable dt_newBuisness = vw_newBuisness.ToTable(true, "profileID", "member_buss_email", "businessPosition", "BusinessName");

                //get distinct New member address details
                DataView vw_newAddress = new DataView(result.Tables[4]);
                DataTable dt_newAddress = vw_newAddress.ToTable(true, "addressID", "profileID", "fk_group_master_id", "Address", "city", "state", "country", "pincode", "phoneNo", "fax", "addressType");

                //get distinct New member Family details
                DataView vw_newFamilyMember = new DataView(result.Tables[5]);
                DataTable dt_newFamilyMember = vw_newFamilyMember.ToTable(true, "profileID", "familyMemberId", "fk_main_member_master_id", "emailID", "memberName", "Relationship", "contactNo", "DOB", "anniversary", "BloodGroup", "particulars");

                //get distinct New member setting details
                DataView vw_newSettings = new DataView(result.Tables[6]);
                DataTable dt_newSettings = vw_newSettings.ToTable(true, "fieldID", "DisplayName", "DBColName", "fieldType", "isVisible", "colType");

                //get distinct New member Dynamic field details
                DataView vw_newDynamicFields;
                DataTable dt_newDynamicFields;
                DataColumnCollection columns = result.Tables[7].Columns;
                if (columns.Contains("profileID"))
                {

                    vw_newDynamicFields = new DataView(result.Tables[7]);
                    dt_newDynamicFields = vw_newDynamicFields.ToTable(true, "profileID", "pk_fieldID", "groupID", "DisplayName", "FieldValue", "FieldCategory", "PkId", "FieldType", "IsEditableByMember", "OrderNo");
                }
                else
                {
                    vw_newDynamicFields = new DataView(result.Tables[7]);
                    dt_newDynamicFields = vw_newDynamicFields.ToTable(true, "pk_fieldID", "groupID", "DisplayName", "FieldValue", "FieldCategory", "PkId", "FieldType", "IsEditableByMember", "OrderNo");
                }
                #endregion

                #region Here get all Updated member details
                DataView vwUpdateMember = new DataView(result.Tables[8]);
                DataTable dtUpdatedMember = vwUpdateMember.ToTable(true, "profileID");

                //get distinct Updated member details 
                DataView vw_UpdateMemberDetails = new DataView(result.Tables[9]);
                DataTable dt_UpdateMemberDetails = vw_UpdateMemberDetails.ToTable(true, "profileID", "masterID", "grpID", "Salutation", "memberName", "memberEmail", "memberMobile", "memberCountry", "profilePic", "isAdmin", "isPersonalDetVisible", "Branch_Id", "Branch_Name");

                //get distinct Updated member personal details
                DataView vw_UpdatePersonal = new DataView(result.Tables[10]);
                DataTable dt_UpdatePersonal = vw_UpdatePersonal.ToTable(true, "profileID", "Salutation", "member_name", "memberMobile", "member_email_id", "member_date_of_birth", "member_date_of_wedding", "Employee_Code_One", "First_Level_of_Reporting_Name", "Employee_Code_2", "Second_LevelofReportingName", "UHID", "Official_Landline_No", "Second_Official_Landline_No", "Date_of_Joining", "Fk_Dept_ID", "Fk_FunctionalRole_Id", "fk_EntityName_id", "fk_designation_id", "department_Name", "functionalrole_Name", "Entity_Name", "Designation_Name", "Employee_Code", "Branch_Id", "Branch_Name");

                //get distinct Updated member Buisness details
                DataView vw_UpdateBuisness = new DataView(result.Tables[11]);
                DataTable dt_UpdateBuisness = vw_UpdateBuisness.ToTable(true, "profileID", "member_buss_email", "businessPosition", "BusinessName");

                //get distinct Updated member address details
                DataView vw_UpdateAddress = new DataView(result.Tables[12]);
                DataTable dt_UpdateAddress = vw_UpdateAddress.ToTable(true, "addressID", "profileID", "fk_group_master_id", "Address", "city", "state", "country", "pincode", "phoneNo", "fax", "addressType");

                //get distinct Updated member Family details
                DataView vw_UpdateFamilyMember = new DataView(result.Tables[13]);
                DataTable dt_UpdateFamilyMember = vw_UpdateFamilyMember.ToTable(true, "profileID", "familyMemberId", "fk_main_member_master_id", "emailID", "memberName", "Relationship", "contactNo", "DOB", "anniversary", "BloodGroup", "particulars");

                //get distinct Updated member setting details
                DataView vw_UpdateSettings = new DataView(result.Tables[14]);
                DataTable dt_UpdateSettings = vw_UpdateSettings.ToTable(true, "fieldID", "DisplayName", "DBColName", "fieldType", "isVisible", "colType");

                //get distinct Updated member Dynamic field details
                DataView vw_UpdateDynamicFields;
                DataTable dt_UpdateDynamicFields;
                DataColumnCollection columns1 = result.Tables[15].Columns;
                if (columns1.Contains("profileID"))
                {
                    vw_UpdateDynamicFields = new DataView(result.Tables[15]);
                    dt_UpdateDynamicFields = vw_UpdateDynamicFields.ToTable(true, "profileID", "pk_fieldID", "groupID", "DisplayName", "FieldValue", "FieldCategory", "PkId", "FieldType", "IsEditableByMember", "OrderNo");
                }
                else
                {
                    vw_UpdateDynamicFields = new DataView(result.Tables[15]);
                    dt_UpdateDynamicFields = vw_UpdateDynamicFields.ToTable(true, "pk_fieldID", "groupID", "DisplayName", "FieldValue", "FieldCategory", "PkId", "FieldType", "IsEditableByMember", "OrderNo");
                }
                #endregion

                #region Here get all deleted member ids

                DataTable dtDeletedMember = result.Tables[16];

                #endregion

                Int32 dataCount = 10;// Used for paging. DataCount is page size
                List<MemberDetailsDynamicField> memberList;
                Int32 Filecounter = 1;
                Int32 totalrows = dtnewMember.Rows.Count;
 
                string FileName = DateTime.Now.ToString("ddMMyyyyhhmmsstt");

                MemberListSyncResult memListSyncResult = new MemberListSyncResult();

                #region
                //======================= If total records less than 20 Return Json directly =================================================
                Int32 totalRowCount;
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
                            DataView dv_FilterNewMember = dt_newMemberDetails.DefaultView;
                            dv_FilterNewMember.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterNewMember = dv_FilterNewMember.ToTable();

                            DataView dv_FilterNewPersonal = dt_newPersonal.DefaultView;
                            dv_FilterNewPersonal.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterNewPersonal = dv_FilterNewPersonal.ToTable();

                            DataView dv_FilterNewBuisness = dt_newBuisness.DefaultView;
                            dv_FilterNewBuisness.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterNewBuisness = dv_FilterNewBuisness.ToTable();

                            DataView dv_FilterNewAddress = dt_newAddress.DefaultView;
                            dv_FilterNewAddress.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterNewAddress = dv_FilterNewAddress.ToTable();

                            DataView dv_FilterNewFamilyMember = dt_newFamilyMember.DefaultView;
                            dv_FilterNewFamilyMember.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterNewFamilyMember = dv_FilterNewFamilyMember.ToTable();

                            //DataView dv_FilterNewSettings = dt_newSettings.DefaultView;
                            //dv_FilterNewSettings.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                            //DataTable dt_FilterNewSettings = dv_FilterNewSettings.ToTable();

                            DataView dv_FilterNewDynamicFields;
                            DataTable dt_FilterNewDynamicFields;
                            DataColumnCollection columns2 = dt_newDynamicFields.Columns;
                            if (columns2.Contains("profileID"))
                            {
                                dv_FilterNewDynamicFields = dt_newDynamicFields.DefaultView;
                                dv_FilterNewDynamicFields.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                dt_FilterNewDynamicFields = dv_FilterNewDynamicFields.ToTable();
                            }
                            else
                            {
                                dv_FilterNewDynamicFields = dt_newDynamicFields.DefaultView;
                                dt_FilterNewDynamicFields = dv_FilterNewDynamicFields.ToTable();
                            }

                            MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeildByDatatables(dtnewMember.Rows[i]["profileID"].ToString(), dt_FilterNewMember, dt_FilterNewPersonal, dt_FilterNewBuisness, dt_FilterNewAddress, dt_FilterNewFamilyMember, dt_newSettings, dt_FilterNewDynamicFields);
                            memberList.Add(memberdtl);

                            // MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeildByDatatables(dtnewMember.Rows[i]["profileID"].ToString(), dt_FilterNewMember, dt_FilterNewPersonal, dt_FilterNewBuisness, dt_FilterNewAddress, dt_FilterNewFamilyMember, dt_newSettings, dt_FilterNewDynamicFields);
                            //  memberList.Add(memberdtl);
                        }
                    }
                    memListSyncResult.NewMemberList = memberList;

                    //---------------Create array of Updated members -------------------------------
                    memberList = new List<MemberDetailsDynamicField>();
                    if (dtUpdatedMember.Rows.Count > 0 && Convert.ToDateTime(updatedOn).Date.ToString("dd/MM/yyyy") != "01/01/1970")
                    {
                        for (int i = 0; i < dtUpdatedMember.Rows.Count; i++)
                        {
                            DataView dv_FilterUpdateMember = dt_UpdateMemberDetails.DefaultView;
                            dv_FilterUpdateMember.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterUpdateMember = dv_FilterUpdateMember.ToTable();

                            DataView dv_FilterUpdatePersonal = dt_UpdatePersonal.DefaultView;
                            dv_FilterUpdatePersonal.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterUpdatePersonal = dv_FilterUpdatePersonal.ToTable();

                            DataView dv_FilterUpdateBuisness = dt_UpdateBuisness.DefaultView;
                            dv_FilterUpdateBuisness.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterUpdateBuisness = dv_FilterUpdateBuisness.ToTable();

                            DataView dv_FilterUpdateAddress = dt_UpdateAddress.DefaultView;
                            dv_FilterUpdateAddress.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterUpdateAddress = dv_FilterUpdateAddress.ToTable();

                            DataView dv_FilterUpdateFamilyMember = dt_UpdateFamilyMember.DefaultView;
                            dv_FilterUpdateFamilyMember.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterUpdateFamilyMember = dv_FilterUpdateFamilyMember.ToTable();

                            //DataView dv_FilterUpdateSettings = dt_UpdateSettings.DefaultView;
                            //dv_FilterUpdateSettings.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                            //DataTable dt_FilterUpdateSettings = dv_FilterUpdateSettings.ToTable();

                            DataView dv_FilterUpdateDynamicFields;
                            DataTable dt_FilterUpdateDynamicFields;
                            DataColumnCollection columns3 = dt_UpdateDynamicFields.Columns;
                            if (columns3.Contains("profileID"))
                            {
                                dv_FilterUpdateDynamicFields = dt_UpdateDynamicFields.DefaultView;
                                dv_FilterUpdateDynamicFields.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                dt_FilterUpdateDynamicFields = dv_FilterUpdateDynamicFields.ToTable();
                            }
                            else
                            {
                                dv_FilterUpdateDynamicFields = dt_UpdateDynamicFields.DefaultView;
                                dt_FilterUpdateDynamicFields = dv_FilterUpdateDynamicFields.ToTable();
                            }
                            MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeildByDatatables(dtUpdatedMember.Rows[i]["profileID"].ToString(), dt_FilterUpdateMember, dt_FilterUpdatePersonal, dt_FilterUpdateBuisness, dt_FilterUpdateAddress, dt_FilterUpdateFamilyMember, dt_UpdateSettings, dt_FilterUpdateDynamicFields);
                            memberList.Add(memberdtl);

                            //MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeild_Test(dtUpdatedMember.Rows[i]["profileID"].ToString(), grpID);
                            //memberList.Add(memberdtl);
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

                                    DataView dv_FilterNewMember = dt_newMemberDetails.DefaultView;
                                    dv_FilterNewMember.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterNewMember = dv_FilterNewMember.ToTable();

                                    DataView dv_FilterNewPersonal = dt_newPersonal.DefaultView;
                                    dv_FilterNewPersonal.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterNewPersonal = dv_FilterNewPersonal.ToTable();

                                    DataView dv_FilterNewBuisness = dt_newBuisness.DefaultView;
                                    dv_FilterNewBuisness.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterNewBuisness = dv_FilterNewBuisness.ToTable();

                                    DataView dv_FilterNewAddress = dt_newAddress.DefaultView;
                                    dv_FilterNewAddress.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterNewAddress = dv_FilterNewAddress.ToTable();

                                    DataView dv_FilterNewFamilyMember = dt_newFamilyMember.DefaultView;
                                    dv_FilterNewFamilyMember.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterNewFamilyMember = dv_FilterNewFamilyMember.ToTable();

                                    //DataView dv_FilterNewSettings = dt_newSettings.DefaultView;
                                    //dv_FilterNewSettings.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                    //DataTable dt_FilterNewSettings = dv_FilterNewSettings.ToTable();

                                    DataView dv_FilterNewDynamicFields;
                                    DataTable dt_FilterNewDynamicFields;
                                    DataColumnCollection columns3 = dt_newDynamicFields.Columns;
                                    if (columns3.Contains("profileID"))
                                    {
                                        dv_FilterNewDynamicFields = dt_newDynamicFields.DefaultView;
                                        dv_FilterNewDynamicFields.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                        dt_FilterNewDynamicFields = dv_FilterNewDynamicFields.ToTable();
                                    }
                                    else
                                    {
                                        dv_FilterNewDynamicFields = dt_newDynamicFields.DefaultView;
                                        dt_FilterNewDynamicFields = dv_FilterNewDynamicFields.ToTable();
                                    }

                                    MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeildByDatatables(dtnewMember.Rows[i]["profileID"].ToString(), dt_FilterNewMember, dt_FilterNewPersonal, dt_FilterNewBuisness, dt_FilterNewAddress, dt_FilterNewFamilyMember, dt_newSettings, dt_FilterNewDynamicFields);
                                    memberList.Add(memberdtl);

                                    //MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeild_Test(dtnewMember.Rows[i]["profileID"].ToString(), grpID);
                                    //memberList.Add(memberdtl);

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

                                    DataView dv_FilterUpdateMember = dt_UpdateMemberDetails.DefaultView;
                                    dv_FilterUpdateMember.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterUpdateMember = dv_FilterUpdateMember.ToTable();

                                    DataView dv_FilterUpdatePersonal = dt_UpdatePersonal.DefaultView;
                                    dv_FilterUpdatePersonal.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterUpdatePersonal = dv_FilterUpdatePersonal.ToTable();

                                    DataView dv_FilterUpdateBuisness = dt_UpdateBuisness.DefaultView;
                                    dv_FilterUpdateBuisness.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterUpdateBuisness = dv_FilterUpdateBuisness.ToTable();

                                    DataView dv_FilterUpdateAddress = dt_UpdateAddress.DefaultView;
                                    dv_FilterUpdateAddress.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterUpdateAddress = dv_FilterUpdateAddress.ToTable();

                                    DataView dv_FilterUpdateFamilyMember = dt_UpdateFamilyMember.DefaultView;
                                    dv_FilterUpdateFamilyMember.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterUpdateFamilyMember = dv_FilterUpdateFamilyMember.ToTable();

                                    //DataView dv_FilterUpdateSettings = dt_UpdateSettings.DefaultView;
                                    //dv_FilterUpdateSettings.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                    //DataTable dt_FilterUpdateSettings = dv_FilterUpdateSettings.ToTable();

                                    DataView dv_FilterUpdateDynamicFields;
                                    DataTable dt_FilterUpdateDynamicFields;
                                    DataColumnCollection columns3 = dt_UpdateDynamicFields.Columns;
                                    if (columns3.Contains("profileID"))
                                    {
                                        dv_FilterUpdateDynamicFields = dt_UpdateDynamicFields.DefaultView;
                                        dv_FilterUpdateDynamicFields.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                        dt_FilterUpdateDynamicFields = dv_FilterUpdateDynamicFields.ToTable();
                                    }
                                    else
                                    {
                                        dv_FilterUpdateDynamicFields = dt_UpdateDynamicFields.DefaultView;
                                        dt_FilterUpdateDynamicFields = dv_FilterUpdateDynamicFields.ToTable();
                                    }
                                    MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeildByDatatables(dtnewMember.Rows[i]["profileID"].ToString(), dt_FilterUpdateMember, dt_FilterUpdatePersonal, dt_FilterUpdateBuisness, dt_FilterUpdateAddress, dt_FilterUpdateFamilyMember, dt_UpdateSettings, dt_FilterUpdateDynamicFields);
                                    memberList.Add(memberdtl);

                                    //MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeild_Test(dtUpdatedMember.Rows[i]["profileID"].ToString(), grpID);
                                    //memberList.Add(memberdtl);
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
                #endregion
                return memListSyncResult;
            }
            catch(Exception ex)
            {
                throw;
            }
            #endregion
        }

        public static MemberListSyncResult GetMemberListSync_Test(string updatedOn, string grpID, out string filepath)
        {
            bool isDataFound = false;
            string targetPath = "";
            DataSet result;
            try
            {
                //DataSet result;
                CallForMemberDetails(updatedOn, grpID, out filepath, out result);

                //DataTable dtnewMember = result.Tables[0];

                #region Here get all member details
                            //get distinct New member id
                            DataView vwnewMember = new DataView(result.Tables[0]);
                            DataTable dtnewMember = vwnewMember.ToTable(true, "profileID");

                            //get distinct New member details 
                            DataView vw_newMemberDetails = new DataView(result.Tables[1]);
                            DataTable dt_newMemberDetails = vw_newMemberDetails.ToTable(true, "profileID", "masterID", "grpID", "memberName", "memberEmail", "memberMobile", "memberCountry", "profilePic", "isAdmin", "isPersonalDetVisible", "Branch_Id", "Branch_Name");

                            //get distinct New member personal details
                            DataView vw_newPersonal = new DataView(result.Tables[2]);
                            DataTable dt_newPersonal = vw_newPersonal.ToTable(true, "profileID", "member_name", "member_mobile_no", "secondry_mobile_no", "member_email_id", "member_date_of_birth", "member_date_of_wedding", "blood_Group", "member_master_designation", "rotary_donar_recognation", "designation", "isBOD", "dg_master_designation", "member_rotary_id", "Keywords");

                            //get distinct New member Buisness details
                            DataView vw_newBuisness = new DataView(result.Tables[3]);
                            DataTable dt_newBuisness = vw_newBuisness.ToTable(true, "profileID", "member_buss_email", "businessPosition", "BusinessName");

                            //get distinct New member address details
                            DataView vw_newAddress = new DataView(result.Tables[4]);
                            DataTable dt_newAddress = vw_newAddress.ToTable(true, "addressID", "profileID", "fk_group_master_id", "Address", "city", "state", "country", "pincode", "phoneNo", "fax", "addressType");

                            //get distinct New member Family details
                            DataView vw_newFamilyMember = new DataView(result.Tables[5]);
                            DataTable dt_newFamilyMember = vw_newFamilyMember.ToTable(true, "profileID", "familyMemberId", "fk_main_member_master_id", "emailID", "memberName", "Relationship", "contactNo", "DOB", "anniversary", "BloodGroup", "particulars");

                            //get distinct New member setting details
                            DataView vw_newSettings = new DataView(result.Tables[6]);
                            DataTable dt_newSettings = vw_newSettings.ToTable(true,"fieldID", "DisplayName", "DBColName", "fieldType", "isVisible", "colType");

                            //get distinct New member Dynamic field details
                            DataView vw_newDynamicFields;
                            DataTable dt_newDynamicFields;
                            DataColumnCollection columns = result.Tables[7].Columns;
                            if (columns.Contains("profileID"))
                            {
                                
                                 vw_newDynamicFields = new DataView(result.Tables[7]);
                                 dt_newDynamicFields = vw_newDynamicFields.ToTable(true, "profileID", "pk_fieldID", "groupID", "DisplayName", "FieldValue", "FieldCategory", "PkId", "FieldType", "IsEditableByMember", "OrderNo");
                            }
                            else
                            {
                                vw_newDynamicFields = new DataView(result.Tables[7]);
                                dt_newDynamicFields = vw_newDynamicFields.ToTable(true, "pk_fieldID", "groupID", "DisplayName", "FieldValue", "FieldCategory", "PkId", "FieldType", "IsEditableByMember", "OrderNo");
                            }
                #endregion

                //DataTable dtUpdatedMember = result.Tables[1];

                #region Here get all Updated member details
                            DataView vwUpdateMember = new DataView(result.Tables[8]);
                            DataTable dtUpdatedMember = vwUpdateMember.ToTable(true, "profileID");

                            //get distinct Updated member details 
                            DataView vw_UpdateMemberDetails = new DataView(result.Tables[9]);
                            DataTable dt_UpdateMemberDetails = vw_UpdateMemberDetails.ToTable(true, "profileID", "masterID", "grpID", "memberName", "memberEmail", "memberMobile", "memberCountry", "profilePic", "familyPic", "isAdmin", "isPersonalDetVisible", "Branch_Id", "isBusinDetVisible", "isFamilDetailVisible", "Branch_Name");

                            //get distinct Updated member personal details
                            DataView vw_UpdatePersonal = new DataView(result.Tables[10]);
                            DataTable dt_UpdatePersonal = vw_UpdatePersonal.ToTable(true, "profileID", "member_name", "member_mobile_no", "secondry_mobile_no", "member_email_id", "member_date_of_birth", "member_date_of_wedding", "blood_Group", "member_master_designation", "rotary_donar_recognation", "designation", "isBOD", "dg_master_designation", "member_rotary_id", "Keywords");

                            //get distinct Updated member Buisness details
                            DataView vw_UpdateBuisness = new DataView(result.Tables[11]);
                            DataTable dt_UpdateBuisness = vw_UpdateBuisness.ToTable(true, "profileID", "member_buss_email", "businessPosition", "BusinessName");

                            //get distinct Updated member address details
                            DataView vw_UpdateAddress = new DataView(result.Tables[12]);
                            DataTable dt_UpdateAddress = vw_UpdateAddress.ToTable(true, "addressID", "profileID", "fk_group_master_id", "Address", "city", "state", "country", "pincode", "phoneNo", "fax", "addressType");

                            //get distinct Updated member Family details
                            DataView vw_UpdateFamilyMember = new DataView(result.Tables[13]);
                            DataTable dt_UpdateFamilyMember = vw_UpdateFamilyMember.ToTable(true, "profileID", "familyMemberId", "fk_main_member_master_id", "emailID", "memberName", "Relationship", "contactNo", "DOB", "anniversary", "BloodGroup", "particulars");

                            //get distinct Updated member setting details
                            DataView vw_UpdateSettings = new DataView(result.Tables[14]);
                            DataTable dt_UpdateSettings = vw_UpdateSettings.ToTable(true,"fieldID", "DisplayName", "DBColName", "fieldType", "isVisible", "colType");

                            //get distinct Updated member Dynamic field details
                            DataView vw_UpdateDynamicFields;
                            DataTable dt_UpdateDynamicFields;
                            DataColumnCollection columns1 = result.Tables[15].Columns;
                            if (columns1.Contains("profileID"))
                            {
                                vw_UpdateDynamicFields = new DataView(result.Tables[15]);
                                dt_UpdateDynamicFields = vw_UpdateDynamicFields.ToTable(true, "profileID", "pk_fieldID", "groupID", "DisplayName", "FieldValue", "FieldCategory", "PkId", "FieldType", "IsEditableByMember", "OrderNo");
                            }
                            else
                            {
                                vw_UpdateDynamicFields = new DataView(result.Tables[15]);
                                dt_UpdateDynamicFields = vw_UpdateDynamicFields.ToTable(true, "pk_fieldID", "groupID", "DisplayName", "FieldValue", "FieldCategory", "PkId", "FieldType", "IsEditableByMember", "OrderNo");
                            }
                #endregion

                #region Here get all deleted member ids

                            DataTable dtDeletedMember = result.Tables[16];

                #endregion
                
                Int32 dataCount = 10;// Used for paging. DataCount is page size
                List<MemberDetailsDynamicField> memberList;
                Int32 Filecounter = 1;
                Int32 totalrows = dtnewMember.Rows.Count;
                string FileName = DateTime.Now.ToString("ddMMyyyyhhmmsstt");

                MemberListSyncResult memListSyncResult = new MemberListSyncResult();

                #region
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
                            DataView dv_FilterNewMember = dt_newMemberDetails.DefaultView;
                            dv_FilterNewMember.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterNewMember = dv_FilterNewMember.ToTable();

                            DataView dv_FilterNewPersonal = dt_newPersonal.DefaultView;
                            dv_FilterNewPersonal.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterNewPersonal = dv_FilterNewPersonal.ToTable();

                            DataView dv_FilterNewBuisness = dt_newBuisness.DefaultView;
                            dv_FilterNewBuisness.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterNewBuisness = dv_FilterNewBuisness.ToTable();

                            DataView dv_FilterNewAddress = dt_newAddress.DefaultView;
                            dv_FilterNewAddress.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterNewAddress = dv_FilterNewAddress.ToTable();

                            DataView dv_FilterNewFamilyMember = dt_newFamilyMember.DefaultView;
                            dv_FilterNewFamilyMember.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterNewFamilyMember = dv_FilterNewFamilyMember.ToTable();

                            //DataView dv_FilterNewSettings = dt_newSettings.DefaultView;
                            //dv_FilterNewSettings.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                            //DataTable dt_FilterNewSettings = dv_FilterNewSettings.ToTable();

                            DataView dv_FilterNewDynamicFields;
                            DataTable dt_FilterNewDynamicFields;
                            DataColumnCollection columns2 = dt_newDynamicFields.Columns;
                            if (columns2.Contains("profileID"))
                            {
                                dv_FilterNewDynamicFields = dt_newDynamicFields.DefaultView;
                                dv_FilterNewDynamicFields.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                dt_FilterNewDynamicFields = dv_FilterNewDynamicFields.ToTable();
                            }
                            else
                            {
                                dv_FilterNewDynamicFields = dt_newDynamicFields.DefaultView;
                                dt_FilterNewDynamicFields = dv_FilterNewDynamicFields.ToTable();
                            }

                            MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeildByDatatables(dtnewMember.Rows[i]["profileID"].ToString(), dt_FilterNewMember, dt_FilterNewPersonal, dt_FilterNewBuisness, dt_FilterNewAddress, dt_FilterNewFamilyMember, dt_newSettings, dt_FilterNewDynamicFields);
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
                            DataView dv_FilterUpdateMember = dt_UpdateMemberDetails.DefaultView;
                            dv_FilterUpdateMember.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterUpdateMember = dv_FilterUpdateMember.ToTable();

                            DataView dv_FilterUpdatePersonal = dt_UpdatePersonal.DefaultView;
                            dv_FilterUpdatePersonal.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterUpdatePersonal = dv_FilterUpdatePersonal.ToTable();

                            DataView dv_FilterUpdateBuisness = dt_UpdateBuisness.DefaultView;
                            dv_FilterUpdateBuisness.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterUpdateBuisness = dv_FilterUpdateBuisness.ToTable();

                            DataView dv_FilterUpdateAddress = dt_UpdateAddress.DefaultView;
                            dv_FilterUpdateAddress.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterUpdateAddress = dv_FilterUpdateAddress.ToTable();

                            DataView dv_FilterUpdateFamilyMember = dt_UpdateFamilyMember.DefaultView;
                            dv_FilterUpdateFamilyMember.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                            DataTable dt_FilterUpdateFamilyMember = dv_FilterUpdateFamilyMember.ToTable();

                            //DataView dv_FilterUpdateSettings = dt_UpdateSettings.DefaultView;
                            //dv_FilterUpdateSettings.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                            //DataTable dt_FilterUpdateSettings = dv_FilterUpdateSettings.ToTable();

                            DataView dv_FilterUpdateDynamicFields;
                            DataTable dt_FilterUpdateDynamicFields;
                            DataColumnCollection columns3 = dt_UpdateDynamicFields.Columns;
                            if (columns3.Contains("profileID"))
                            {
                                dv_FilterUpdateDynamicFields = dt_UpdateDynamicFields.DefaultView;
                                dv_FilterUpdateDynamicFields.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                dt_FilterUpdateDynamicFields = dv_FilterUpdateDynamicFields.ToTable();
                            }
                            else
                            {
                                dv_FilterUpdateDynamicFields = dt_UpdateDynamicFields.DefaultView;
                                dt_FilterUpdateDynamicFields = dv_FilterUpdateDynamicFields.ToTable();
                            }
                            MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeildByDatatables(dtnewMember.Rows[i]["profileID"].ToString(), dt_FilterUpdateMember, dt_FilterUpdatePersonal, dt_FilterUpdateBuisness, dt_FilterUpdateAddress, dt_FilterUpdateFamilyMember, dt_UpdateSettings, dt_FilterUpdateDynamicFields);
                            memberList.Add(memberdtl);

                            //MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeild_Test(dtUpdatedMember.Rows[i]["profileID"].ToString(), grpID);
                            //memberList.Add(memberdtl);
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

                                    DataView dv_FilterNewMember = dt_newMemberDetails.DefaultView;
                                    dv_FilterNewMember.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterNewMember = dv_FilterNewMember.ToTable();

                                    DataView dv_FilterNewPersonal = dt_newPersonal.DefaultView;
                                    dv_FilterNewPersonal.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterNewPersonal = dv_FilterNewPersonal.ToTable();

                                    DataView dv_FilterNewBuisness = dt_newBuisness.DefaultView;
                                    dv_FilterNewBuisness.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterNewBuisness = dv_FilterNewBuisness.ToTable();

                                    DataView dv_FilterNewAddress = dt_newAddress.DefaultView;
                                    dv_FilterNewAddress.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterNewAddress = dv_FilterNewAddress.ToTable();

                                    DataView dv_FilterNewFamilyMember = dt_newFamilyMember.DefaultView;
                                    dv_FilterNewFamilyMember.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterNewFamilyMember = dv_FilterNewFamilyMember.ToTable();

                                    //DataView dv_FilterNewSettings = dt_newSettings.DefaultView;
                                    //dv_FilterNewSettings.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                    //DataTable dt_FilterNewSettings = dv_FilterNewSettings.ToTable();

                                    DataView dv_FilterNewDynamicFields;
                                    DataTable dt_FilterNewDynamicFields;
                                    DataColumnCollection columns3 = dt_newDynamicFields.Columns;
                                    if (columns3.Contains("profileID"))
                                    {
                                        dv_FilterNewDynamicFields = dt_newDynamicFields.DefaultView;
                                        dv_FilterNewDynamicFields.RowFilter = " profileID = " + dtnewMember.Rows[i]["profileID"].ToString();
                                        dt_FilterNewDynamicFields = dv_FilterNewDynamicFields.ToTable();
                                    }
                                    else
                                    {
                                        dv_FilterNewDynamicFields = dt_newDynamicFields.DefaultView;
                                        dt_FilterNewDynamicFields = dv_FilterNewDynamicFields.ToTable();
                                    }

                                    MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeildByDatatables(dtnewMember.Rows[i]["profileID"].ToString(), dt_FilterNewMember, dt_FilterNewPersonal, dt_FilterNewBuisness, dt_FilterNewAddress, dt_FilterNewFamilyMember, dt_newSettings, dt_FilterNewDynamicFields);
                                    memberList.Add(memberdtl);

                                    //MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeild_Test(dtnewMember.Rows[i]["profileID"].ToString(), grpID);
                                    //memberList.Add(memberdtl);

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

                                    DataView dv_FilterUpdateMember = dt_UpdateMemberDetails.DefaultView;
                                    dv_FilterUpdateMember.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterUpdateMember = dv_FilterUpdateMember.ToTable();

                                    DataView dv_FilterUpdatePersonal = dt_UpdatePersonal.DefaultView;
                                    dv_FilterUpdatePersonal.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterUpdatePersonal = dv_FilterUpdatePersonal.ToTable();

                                    DataView dv_FilterUpdateBuisness = dt_UpdateBuisness.DefaultView;
                                    dv_FilterUpdateBuisness.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterUpdateBuisness = dv_FilterUpdateBuisness.ToTable();

                                    DataView dv_FilterUpdateAddress = dt_UpdateAddress.DefaultView;
                                    dv_FilterUpdateAddress.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterUpdateAddress = dv_FilterUpdateAddress.ToTable();

                                    DataView dv_FilterUpdateFamilyMember = dt_UpdateFamilyMember.DefaultView;
                                    dv_FilterUpdateFamilyMember.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                    DataTable dt_FilterUpdateFamilyMember = dv_FilterUpdateFamilyMember.ToTable();

                                    //DataView dv_FilterUpdateSettings = dt_UpdateSettings.DefaultView;
                                    //dv_FilterUpdateSettings.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                    //DataTable dt_FilterUpdateSettings = dv_FilterUpdateSettings.ToTable();

                                    DataView dv_FilterUpdateDynamicFields;
                                    DataTable dt_FilterUpdateDynamicFields;
                                    DataColumnCollection columns3 = dt_UpdateDynamicFields.Columns;
                                    if (columns3.Contains("profileID"))
                                    {
                                        dv_FilterUpdateDynamicFields = dt_UpdateDynamicFields.DefaultView;
                                        dv_FilterUpdateDynamicFields.RowFilter = " profileID = " + dtUpdatedMember.Rows[i]["profileID"].ToString();
                                        dt_FilterUpdateDynamicFields = dv_FilterUpdateDynamicFields.ToTable();
                                    }
                                    else
                                    {
                                        dv_FilterUpdateDynamicFields = dt_UpdateDynamicFields.DefaultView;
                                        dt_FilterUpdateDynamicFields = dv_FilterUpdateDynamicFields.ToTable();
                                    }
                                    MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeildByDatatables(dtnewMember.Rows[i]["profileID"].ToString(), dt_FilterUpdateMember, dt_FilterUpdatePersonal, dt_FilterUpdateBuisness, dt_FilterUpdateAddress, dt_FilterUpdateFamilyMember, dt_UpdateSettings, dt_FilterUpdateDynamicFields);
                                    memberList.Add(memberdtl);

                                    //MemberDetailsDynamicField memberdtl = GetMemberDtlWithDynamicFeild_Test(dtUpdatedMember.Rows[i]["profileID"].ToString(), grpID);
                                    //memberList.Add(memberdtl);
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
                #endregion
                return memListSyncResult;
            }
            catch
            {
                throw;
            }
        }

        private static void CallForMemberDetails(string updatedOn, string grpID, out string filepath, out DataSet result)
        {
            filepath = "";
            MySqlParameter[] param = new MySqlParameter[2];
            param[0] = new MySqlParameter("@GrpID", grpID);
            param[1] = new MySqlParameter("@updatedOn", updatedOn);
            int i = 0;
            try
            {
                result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V6_USPGetMemberDetails_ALLTest", param);
            }
            catch
            {
                if (i < 3)
                {
                    i += 1;
                    result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V6_USPGetMemberDetails_ALLTest", param);
                }
                else
                {
                    result = new DataSet();
                    // TODO : set exception message for no internet connection 
                }
            }
        }

        public static int UpdatePersonalAndDynamicDtls(string Input, string profileID)
        {
            try
            {
                List<PersonalMemberDetil> fieldList = JsonConvert.DeserializeObject<List<PersonalMemberDetil>>(Input);
                StringBuilder strQuery = new StringBuilder();//Set all queries in string builder and execute at once

                foreach (PersonalMemberDetil field in fieldList)
                {
                    if (field.fieldID == "0") // Update static field
                    {
                        // Set Default values
                        switch (field.uniquekey)
                        {
                            case "member_date_of_birth":
                                if (field.value == "")
                                {
                                    field.value = "1753-01-01 00:00:00";
                                }
                                break;
                            case "member_date_of_wedding":
                                if (field.value == "")
                                {
                                    field.value = "1753-01-01 00:00:00";
                                }
                                break;

                            default:
                                break;
                        }
                        if (field.uniquekey == "Address")
                        {
                            strQuery.Append("Update member_address_detail Set ");
                            strQuery.Append(field.uniquekey);
                            strQuery.Append("='");
                            strQuery.Append(field.value);
                            strQuery.Append("',Address_Type='Residence',modification_date=now() where fk_member_profile_id=");
                            strQuery.Append(profileID);
                            strQuery.Append(";");
                        }
                        else if (field.uniquekey != "member_mobile_no")
                        {
                            strQuery.Append("Update member_master_profile Set ");
                            strQuery.Append(field.uniquekey);
                            strQuery.Append("='");
                            strQuery.Append(field.value);
                            strQuery.Append("' ,modification_date=now() where pk_member_master_profile_id=");
                            strQuery.Append(profileID);
                            strQuery.Append(";");
                            // MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.Text, strQuery);
                        }
                    }
                    else // Update dynamic fields
                    {
                        MySqlParameter[] param = new MySqlParameter[4];
                        param[0] = new MySqlParameter("@profileID", profileID);
                        param[1] = new MySqlParameter("@fieldID", field.fieldID);
                        param[2] = new MySqlParameter("@Fkey", field.key);
                        param[3] = new MySqlParameter("@Fvalue", field.value);
                        MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.StoredProcedure, "V6_USPUpdateDynamicFields", param);
                    }
                }
                MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.Text, strQuery.ToString());

                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public static int DeleteZipFolder(string folderName)
        {
            try
            {
                string zipFilePath = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments\\DirectoryData\\" + folderName;

                //delete zip folder
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                }

                //delete Actual folder
                string directoryPath = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments\\DirectoryData\\" + folderName.Split('.')[0].ToString();
                DeleteDirectory(directoryPath);

                return 1;
            }
            catch
            {
                return -1;
            }
        }

        private static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                //Delete all files from the Directory
                foreach (string file in Directory.GetFiles(path))
                {
                    File.Delete(file);
                }
                //Delete all child Directories
                foreach (string directory in Directory.GetDirectories(path))
                {
                    DeleteDirectory(directory);
                }
                //Delete a Directory
                Directory.Delete(path);
            }
        }

        /// <summary>
        /// Created by : Nandu
        /// Created on : 02-05-2017
        /// Task : Update Member Profile on Single Click
        /// </summary>
        public static int UpdateAllProfileDetails(RootObject input)
        {
            try
            {

                //Update Personal Details
                StringBuilder sbPersonal = new StringBuilder();
                if (input.personalMemberDetails != null)
                {
                    input.personalMemberDetails.memberDOB = string.IsNullOrEmpty(input.personalMemberDetails.memberDOB) ? "1753/01/01" : input.personalMemberDetails.memberDOB;
                    input.personalMemberDetails.memberDOA = string.IsNullOrEmpty(input.personalMemberDetails.memberDOA) ? "1753/01/01" : input.personalMemberDetails.memberDOA;

                    sbPersonal.Append("Update member_master_profile set member_name='");
                    sbPersonal.Append(input.personalMemberDetails.memberName);
                    sbPersonal.Append("', member_email_id='");
                    sbPersonal.Append(input.personalMemberDetails.memberEmail);
                    sbPersonal.Append("', secondry_mobile_no='");
                    sbPersonal.Append(input.personalMemberDetails.secondaryMobileNo);
                    sbPersonal.Append("', member_date_of_birth='");
                    sbPersonal.Append(input.personalMemberDetails.memberDOB);
                    sbPersonal.Append("', member_date_of_wedding='");
                    sbPersonal.Append(input.personalMemberDetails.memberDOA);
                    //sbPersonal.Append("', member_profile_photo_path='");
                    //sbPersonal.Append(input.personalMemberDetails.profilePic);
                    sbPersonal.Append("', blood_Group='");
                    sbPersonal.Append(input.personalMemberDetails.bloodGroup);

                    sbPersonal.Append("', designation='");
                    sbPersonal.Append(input.personalMemberDetails.classification);
                    sbPersonal.Append("', Keywords='");
                    sbPersonal.Append(input.personalMemberDetails.keywords);
                    sbPersonal.Append("', member_rotary_id='");
                    sbPersonal.Append(input.personalMemberDetails.rotaryId);
                    sbPersonal.Append("', member_master_designation='");
                    sbPersonal.Append(input.personalMemberDetails.clubDesignation);
                    sbPersonal.Append("', dg_master_designation='");
                    sbPersonal.Append(input.personalMemberDetails.districtDesignation);
                    sbPersonal.Append("', rotary_donar_recognation='");
                    sbPersonal.Append(input.personalMemberDetails.donarRecognition);

                    sbPersonal.Append("', modification_date = now(), ");
                    sbPersonal.Append(" modification_by = '");
                    sbPersonal.Append(input.profileID);

                    sbPersonal.Append("' where pk_member_master_profile_id = '");
                    sbPersonal.Append(input.profileID);
                    sbPersonal.Append("';");
                }

                //Update Business Details
                StringBuilder sbuss = new StringBuilder();
                if (input.businessMemberDetails != null)
                {
                    if (input.businessMemberDetails.Count > 0)
                    {
                        sbuss.Append("Update member_master_profile set ");

                        int count = input.businessMemberDetails.Count;

                        foreach (UpdateBusinessMemberDetail objBusiness in input.businessMemberDetails)
                        {
                            sbuss.Append(objBusiness.uniquekey);
                            sbuss.Append(" = '");
                            sbuss.Append(objBusiness.value);
                            sbuss.Append("',");
                        }

                        //char[] charsToTrim = { ',' };
                        //string str = sbuss.ToString().Trim().TrimEnd(charsToTrim);
                        //sbuss = new StringBuilder();
                        //sbuss.Append(str);

                        sbuss.Append(" modification_date = now(), modification_by = '");
                        sbuss.Append(input.profileID);
                        sbuss.Append("' where pk_member_master_profile_id = '");
                        sbuss.Append(input.profileID);
                        sbuss.Append("';");
                    }
                }


                //Update Address Details
                StringBuilder sbAddress = new StringBuilder();
                if (input.addressDetails != null)
                {
                    if (input.addressDetails.Count > 0)
                    {
                        foreach (UpdateAddressDetail objAddress in input.addressDetails)
                        {
                            //if (!string.IsNullOrEmpty(objAddress.address) && Convert.ToInt32(objAddress.country) != 0)
                            //{
                                //If addressId = 0 then Add mode - insert record in DB as new record
                                if (objAddress.addressID == "0")
                                {
                                    sbAddress.Append("INSERT INTO member_address_detail (fk_member_profile_id, Address, fk_city_id, fk_state_id, fk_country_id, pincode, phone_no, fax, Address_Type, created_on, created_By) VALUES ('");

                                    sbAddress.Append(input.profileID);
                                    sbAddress.Append("','");
                                    sbAddress.Append(objAddress.address);
                                    sbAddress.Append("','");
                                    sbAddress.Append(objAddress.city);
                                    sbAddress.Append("','");
                                    sbAddress.Append(objAddress.state);
                                    sbAddress.Append("','");
                                    sbAddress.Append(objAddress.country);
                                    sbAddress.Append("','");
                                    sbAddress.Append(objAddress.pincode);
                                    sbAddress.Append("','");
                                    sbAddress.Append(objAddress.phoneNo);
                                    sbAddress.Append("','");
                                    sbAddress.Append(objAddress.fax);
                                    sbAddress.Append("','");
                                    sbAddress.Append(objAddress.addressType);
                                    sbAddress.Append("', now(), '");
                                    sbAddress.Append(input.profileID);
                                    sbAddress.Append("');");
                                }
                                else//If addressId != 0 then Update mode - Update record in DB with given addressID
                                {
                                    sbAddress.Append("UPDATE member_address_detail SET fk_member_profile_id = '");
                                    sbAddress.Append(input.profileID);
                                    sbAddress.Append("', Address='");
                                    sbAddress.Append(objAddress.address);
                                    sbAddress.Append("', fk_city_id='");
                                    sbAddress.Append(objAddress.city);
                                    sbAddress.Append("', fk_state_id='");
                                    sbAddress.Append(objAddress.state);
                                    sbAddress.Append("', fk_country_id='");
                                    sbAddress.Append(objAddress.country);
                                    sbAddress.Append("', pincode='");
                                    sbAddress.Append(objAddress.pincode);
                                    sbAddress.Append("', phone_no='");
                                    sbAddress.Append(objAddress.phoneNo);
                                    sbAddress.Append("', fax='");
                                    sbAddress.Append(objAddress.fax);
                                    sbAddress.Append("', Address_Type='");
                                    sbAddress.Append(objAddress.addressType);

                                    sbAddress.Append("', modified_on = now(),");
                                    sbAddress.Append(" modified_by = '");
                                    sbAddress.Append(input.profileID);
                                    sbAddress.Append("' where pk_address_id='");
                                    sbAddress.Append(objAddress.addressID);
                                    sbAddress.Append("';");
                                }
                            //}
                        }
                    }
                }



                //Update Family Members Details
                StringBuilder sbFamily = new StringBuilder();
                if (input.familyMemberDetail != null)
                {
                    if (input.familyMemberDetail.Count > 0)
                    {
                        foreach (UpdateFamilyMemberDetail objFamily in input.familyMemberDetail)
                        {
                            objFamily.memberDOB = string.IsNullOrEmpty(objFamily.memberDOB) ? "1753/01/01" : objFamily.memberDOB;
                            objFamily.memberDOA = string.IsNullOrEmpty(objFamily.memberDOA) ? "1753/01/01" : objFamily.memberDOA;

                            if (objFamily.familyMemberId == "0")
                            {
                                sbFamily.Append("INSERT INTO member_family_master (fk_main_member_master_id, member_name, member_relationship, fk_country_id, member_contact_no, member_date_of_birth ,member_date_of_wedding, member_blood_group, EmailId, particulars, creation_date, created_by) VALUES (");
                                sbFamily.Append("'");
                                sbFamily.Append(input.profileID);
                                sbFamily.Append("','");
                                sbFamily.Append(objFamily.memberName);
                                sbFamily.Append("','");
                                sbFamily.Append(objFamily.relationship);
                                sbFamily.Append("','");

                                sbFamily.Append(string.IsNullOrEmpty(objFamily.countryID) ? "0" : objFamily.countryID);
                                sbFamily.Append("','");

                                sbFamily.Append(objFamily.contactNo);
                                sbFamily.Append("','");
                                sbFamily.Append(objFamily.memberDOB);
                                sbFamily.Append("','");
                                sbFamily.Append(objFamily.memberDOA);
                                sbFamily.Append("','");
                                sbFamily.Append(objFamily.bloodGroup);
                                sbFamily.Append("','");
                                sbFamily.Append(objFamily.emailID);
                                sbFamily.Append("','");
                                sbFamily.Append(objFamily.particulars);
                                sbFamily.Append("',");
                                sbFamily.Append(" now(), '");
                                sbFamily.Append(input.profileID);
                                sbFamily.Append("');");
                            }
                            else
                            {
                                sbFamily.Append("UPDATE member_family_master SET fk_main_member_master_id ='");
                                sbFamily.Append(input.profileID);
                                sbFamily.Append("', member_name='");
                                sbFamily.Append(objFamily.memberName);
                                sbFamily.Append("', member_relationship='");
                                sbFamily.Append(objFamily.relationship);

                                sbFamily.Append("', fk_country_id='");
                                sbFamily.Append(string.IsNullOrEmpty(objFamily.countryID) ? "0" : objFamily.countryID);

                                sbFamily.Append("', member_contact_no='");
                                sbFamily.Append(objFamily.contactNo);
                                sbFamily.Append("', member_date_of_birth='");
                                sbFamily.Append(objFamily.memberDOB);
                                sbFamily.Append("', member_date_of_wedding='");
                                sbFamily.Append(objFamily.memberDOA);
                                sbFamily.Append("', member_blood_group='");
                                sbFamily.Append(objFamily.bloodGroup);
                                sbFamily.Append("', EmailId='");
                                sbFamily.Append(objFamily.emailID);
                                sbFamily.Append("', particulars='");
                                sbFamily.Append(objFamily.particulars);
                                sbFamily.Append("', modification_date = now(),");
                                sbFamily.Append("modification_by=");
                                sbFamily.Append(input.profileID);

                                sbFamily.Append(" where pk_member_family_master_id='");
                                sbFamily.Append(objFamily.familyMemberId);
                                sbFamily.Append("';");
                            }
                        }
                    }
                }

                //Delete Family Members Details
                StringBuilder sbQuery = new StringBuilder();
                if (input.deletedFamilyMemberIds != string.Empty || input.deletedFamilyMemberIds != "")
                {
                    sbQuery.Append("UPDATE member_family_master SET isdeleted = '1', ");
                    sbQuery.Append("deletion_date = now(), ");
                    sbQuery.Append("deleted_by = ");
                    sbQuery.Append(input.profileID);

                    sbQuery.Append(" where pk_member_family_master_id in (");
                    sbQuery.Append(input.deletedFamilyMemberIds);
                    sbQuery.Append(");");
                }


                //Update Dynamic fields Details
                StringBuilder sbDynamic = new StringBuilder();
                if (input.dynamicFields != null)
                {
                    if (input.dynamicFields.Count > 0)
                    {
                        foreach (UpdateDynamicField objDynamic in input.dynamicFields)
                        {
                            if (getRowsCount(input.profileID, objDynamic.fieldID) > 0)
                            {
                                sbDynamic.Append("UPDATE member_profile_dynamic_fields SET ");
                                //sbDynamic.Append(objDynamic.uniquekey);
                                sbDynamic.Append("fieldValue= '");
                                sbDynamic.Append(objDynamic.value);
                                sbDynamic.Append("' where modification_date = now() and fk_MemberProfileID = '");
                                sbDynamic.Append(input.profileID);
                                sbDynamic.Append("' and fk_DynamicFieldID = '");
                                sbDynamic.Append(objDynamic.fieldID);
                                sbDynamic.Append("' and modified_by = '");
                                sbDynamic.Append(input.profileID);
                                sbDynamic.Append("';");
                            }
                            else
                            {
                                sbDynamic.Append("INSERT INTO member_profile_dynamic_fields (fk_MemberProfileID, fk_DynamicFieldID, fieldValue, creation_date, created_by) VALUES ('");
                                sbDynamic.Append(input.profileID);
                                sbDynamic.Append("','");
                                sbDynamic.Append(objDynamic.fieldID);
                                sbDynamic.Append("','");
                                sbDynamic.Append(objDynamic.value);
                                sbDynamic.Append("', now(), '");
                                sbDynamic.Append(input.profileID);
                                sbDynamic.Append("');");
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(sbPersonal.ToString()))
                {
                    //Excecute update Personal data query 
                    int result1 = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.Text, sbPersonal.ToString());
                }

                if (!string.IsNullOrEmpty(sbuss.ToString()))
                {
                    //Excecute update Business data query
                    int result2 = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.Text, sbuss.ToString());
                }

                //Excecute update Address data query
                if (!string.IsNullOrEmpty(sbAddress.ToString()))
                {
                    int result3 = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.Text, sbAddress.ToString());
                }

                if (!string.IsNullOrEmpty(sbFamily.ToString()))
                {
                    //Excecute update Family data query
                    int result4 = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.Text, sbFamily.ToString());
                }

                //Excecute delete family data query
                if (input.deletedFamilyMemberIds != string.Empty || input.deletedFamilyMemberIds != "")
                {
                    int result5 = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.Text, sbQuery.ToString());
                }

                if (!string.IsNullOrEmpty(sbDynamic.ToString()))
                {
                    //Excecute dynamic data query
                    int result6 = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.Text, sbDynamic.ToString());
                }

                // Forward to GetMemberListSync method to get updated records

                //string zipFilePath = string.Empty;
                //MemberListSyncResult memberListSyncResult = MemberMaster.GetMemberListSync(input.updatedOn, input.grpID, out zipFilePath);
                //return memberListSyncResult;

                return 1;
            }
            catch
            {
                return 0;
                //return null;
            }
        }

        /// <summary>
        /// Created By : 
        /// Created On : 
        /// Task : UDF for Checking member dynamic field entry available in Database
        /// </summary>
        /// <param name="profileID"></param>
        /// <param name="fieldID"></param>
        /// <returns></returns>
        private static int getRowsCount(string profileID, string fieldID)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];

                param[0] = new MySqlParameter("?profileID", profileID);
                param[1] = new MySqlParameter("?fieldID", fieldID);

                string sqlQuery = "SELECT count(pk_fieldValueID) FROM member_profile_dynamic_fields where fk_MemberProfileID=@profileID and fk_DynamicFieldID=@fieldID;";

                int result = Convert.ToInt32(MySqlHelper.ExecuteScalar(GlobalVar.strAppConn, CommandType.Text, sqlQuery.ToString(), param));

                return result;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        # endregion

        public static List<UserLogin> GetMemberDetails(string masterUID)
        {
            try
            {
                var masterID = new MySqlParameter("?masterUID", masterUID);
                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<UserLogin>("CALL USPMembersDetails(?masterUID)", masterID).ToList();
                    foreach (UserLogin usr in Result)
                    {
                        if (!string.IsNullOrEmpty(usr.profilePicPath))
                        {
                            string ImageName = usr.profilePicPath.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/MemberProfile/";
                            usr.profilePicPath = path + ImageName;
                        }
                    }
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<CelebrationList> getBdayAnnList(string memberId, string groupId, string date)
        {
            try
            {
                var MemID = new MySqlParameter("?memberId", memberId);
                var GrpId = new MySqlParameter("?grpId", groupId);
                var date1 = new MySqlParameter("?date", date);
                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<CelebrationList>("CALL USPGetBdayAnnList(?memberId,?grpId,?date)", MemID, GrpId, date1).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<MemberListResult> GetDirectoryList(int masterUID, string grpID, string searchText, string updatedOn, int isSubGrpAdmin, string profileID)
        {
            try
            {
                List<MemberListResult> Result = new List<MemberListResult>();
                var master_ID = new MySqlParameter("?masterUID", masterUID);
                var groupID = new MySqlParameter("?groupID", string.IsNullOrEmpty(grpID) ? "" : grpID);
                var SearchText = new MySqlParameter("?SearchText", string.IsNullOrEmpty(searchText) ? "" : searchText.Replace(" ", "%"));
                var UpdatedOn = new MySqlParameter("?updatedOn", string.IsNullOrEmpty(updatedOn) ? "1970/01/01 00:00:00" : updatedOn);

                //Modified by Rupali on Nov 17 2016
                //Same API will get called at Add Event/Announcement/Ebulletin/Gallery modules 
                //Modified response for subgrp Admin

                if (isSubGrpAdmin == 0)
                {
                    // Member is Group Admin
                    using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                    {
                        context.Connection.Open();
                        Result = context.ExecuteStoreQuery<MemberListResult>("CALL V3_USPGetDirectoryList(?masterUID,?groupID,?SearchText,?updatedOn)", master_ID, groupID, SearchText, UpdatedOn).ToList();
                    }
                }
                else
                {
                    //Member is Sub group Admin
                    SubGrpDirectoryInput mem = new SubGrpDirectoryInput();
                    mem.groupId = grpID;
                    mem.profileId = profileID;
                    Result = SubGroupDirectory.GetSubGroupMemberList(mem);
                }
                foreach (MemberListResult mem in Result)
                {
                    if (!string.IsNullOrEmpty(mem.pic))
                    {
                        string ImageName = mem.pic.ToString();
                        string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                        mem.pic = path + ImageName;
                    }
                }

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static MemberDirectory GetDirectoryListSync(int masterUID, int grpID, string updatedOn)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[4];
                param[0] = new MySqlParameter("@masterUID", masterUID);
                param[1] = new MySqlParameter("@groupID", grpID);
                param[3] = new MySqlParameter("@updatedOn", updatedOn);
                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V3_USPGetDirectoryListSync", param);

                DataTable dtnewMember = result.Tables[0];
                DataTable dtUpdatedMember = result.Tables[1];
                DataTable dtDeletedMember = result.Tables[2];

                List<MemberListResult> NewMemberList = new List<MemberListResult>();
                if (dtnewMember.Rows.Count > 0)
                {
                    NewMemberList = GlobalFuns.DataTableToList<MemberListResult>(dtnewMember);

                    foreach (MemberListResult mem in NewMemberList)
                    {
                        if (!string.IsNullOrEmpty(mem.pic))
                        {
                            string ImageName = mem.pic.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                            mem.pic = path + ImageName;
                        }
                    }
                }

                List<MemberListResult> updatedMemberList = new List<MemberListResult>();
                if (dtUpdatedMember.Rows.Count > 0)
                {
                    updatedMemberList = GlobalFuns.DataTableToList<MemberListResult>(dtUpdatedMember);

                    foreach (MemberListResult mem in updatedMemberList)
                    {
                        if (!string.IsNullOrEmpty(mem.pic))
                        {
                            string ImageName = mem.pic.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                            mem.pic = path + ImageName;
                        }
                    }
                }
                List<MemberListResult> deletedMemberList = new List<MemberListResult>();
                if (dtDeletedMember.Rows.Count > 0)
                {
                    deletedMemberList = GlobalFuns.DataTableToList<MemberListResult>(dtDeletedMember);
                }
                MemberDirectory memberList = new MemberDirectory();
                memberList.deletedMembers = deletedMemberList;
                memberList.newMembers = NewMemberList;
                memberList.updatedMembers = updatedMemberList;
                return memberList;

            }
            catch
            {
                throw;
            }
        }

        public static object UploadFile(string FileName, string ModuleName, string filetype)
        {
            try
            {
                var fileName = new MySqlParameter("?fileName", FileName);
                var moduleName = new MySqlParameter("?moduleName", ModuleName);
                var file = new MySqlParameter("?filetype", filetype);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<int>("CALL USPAdd_Image(?fileName,?moduleName,?filetype)", fileName, moduleName, file).SingleOrDefault();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static MemberListDetail GetDirectoryDetail(MemberPro mem)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[2];
                param[0] = new MySqlParameter("?memberProfileId", mem.memberProfileId);
                param[1] = new MySqlParameter("?groupId", mem.groupId);

                DataSet result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "USPGetDirectoryDetail", param);

                DataTable dtMemberDtl = result.Tables[0];
                DataTable dtPersonalDtl = result.Tables[1];
                DataTable dtFamilyDtl = result.Tables[2];
                DataTable dtBusinessDtl = result.Tables[3];
                DataTable dtAddressDtl = result.Tables[4];

                MemberListDetail memberDtl = new MemberListDetail();

                //if()
                //{

                //}
                return memberDtl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object EditPhoto(string FileName, string ProfileID, string Type)
        {
            try
            {
                MySqlParameter[] param = new MySqlParameter[3];
                param[0] = new MySqlParameter("?fileName", FileName);
                param[1] = new MySqlParameter("?profileID", ProfileID);
                param[2] = new MySqlParameter("?type", Type);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<int>("CALL USP_UploadFoto(?fileName,?profileID,?type)", param).SingleOrDefault();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string updateMemberDetails(MemberProfileUpdate member)
        {
            try
            {
                MemberPro mem = new MemberPro();

                var memberId = new MySqlParameter("?memberId", member.ProfileId);
                var mem_name = new MySqlParameter("?mem_name", member.memberName);
                //var mem_mobile = new MySqlParameter("?mem_mobile", member_mobile);
                var mem_email = new MySqlParameter("?mem_email", member.memberEmailid);
                var Image_ID = new MySqlParameter("?image_ID", string.IsNullOrEmpty(member.ImageId) ? "0" : member.ImageId);
                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var result = context.ExecuteStoreQuery<string>("CALL USPUpdateMemberDetails(?memberId,?mem_name,?mem_email,?image_ID)", memberId, mem_name, mem_email, Image_ID).SingleOrDefault();

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int AddMemberToDistrict(string profileId, string createdBy)
        {
            MySqlParameter[] param = new MySqlParameter[2];
            param[0] = new MySqlParameter("?profileID", profileId);
            param[1] = new MySqlParameter("?createdBy", createdBy);

            //var Result = _DBTouchbase.ExecuteStoreQuery<int>("CALL V7_USPAddUpdateDistrictProfile(?profileID,?createdBy)", param).SingleOrDefault();
            int Result = MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.StoredProcedure, "V7_USPAddUpdateDistrictProfile",param);
            return Result;
        }

        public static int UpdatePersonalDetails(string Input, string profileID)
        {
            try
            {
                List<PersonalMemberDetil> list = JsonConvert.DeserializeObject<List<PersonalMemberDetil>>(Input);
                StringBuilder strQuery = new StringBuilder();//Set all queries and execute at once

                foreach (PersonalMemberDetil member in list)
                {
                    // Set Default values
                    switch (member.uniquekey)
                    {
                        case "member_date_of_birth":
                            if (member.value == "")
                            {
                                member.value = "1753-01-01 00:00:00";
                            }
                            break;
                        case "member_date_of_wedding":
                            if (member.value == "")
                            {
                                member.value = "1753-01-01 00:00:00";
                            }
                            break;
                        case "member_phone_no":
                            if (member.value == "")
                            {
                                member.value = "";
                            }
                            break;
                        case "designation":
                            if (member.value == "")
                            {
                                member.value = "";
                            }
                            break;
                        case "member_buss_email":
                            if (member.value == "")
                            {
                                member.value = "";
                            }
                            break;
                        case "member_email_id":
                            if (member.value == "")
                            {
                                member.value = "";
                            }
                            break;
                        case "BusinessName":
                            if (member.value == "")
                            {
                                member.value = "";
                            }
                            break;
                        default:
                            break;
                    }

                    if (member.uniquekey == "Address")
                    {
                        strQuery.Append("Update member_address_detail Set ");
                        strQuery.Append(member.uniquekey);
                        strQuery.Append("='");
                        strQuery.Append(member.value);
                        strQuery.Append("',Address_Type='Residence',modification_date=now() where fk_member_profile_id=");
                        strQuery.Append(profileID);
                        strQuery.Append(";");
                    }
                    else if (member.uniquekey != "member_mobile_no")
                    {
                        strQuery.Append("Update member_master_profile Set ");
                        strQuery.Append(member.uniquekey);
                        strQuery.Append("='");
                        strQuery.Append(member.value);
                        strQuery.Append("' ,modification_date=now() where pk_member_master_profile_id=");
                        strQuery.Append(profileID);
                        strQuery.Append(";");
                        // MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.Text, strQuery);
                    }
                }
                MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.Text, strQuery.ToString());
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public static int UpdateBusinessDetails(string Input, string profileID)
        {
            try
            {
                List<BusinessMemberDetail> list = JsonConvert.DeserializeObject<List<BusinessMemberDetail>>(Input);

                foreach (BusinessMemberDetail member in list)
                {
                    if (member.uniquekey == "Address")
                    {
                        string strQuery = "Update member_address_detail Set " + member.uniquekey + "='" + member.value + "',Address_Type='Buisness' where fk_member_profile_id=" + profileID;
                        MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.Text, strQuery);
                    }
                    else
                    {
                        string strQuery = "Update member_master_profile Set " + member.uniquekey + "='" + member.value + "' where pk_member_master_profile_id=" + profileID;
                        MySqlHelper.ExecuteNonQuery(GlobalVar.strAppConn, CommandType.Text, strQuery);
                    }
                }
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public static int UpdateAddressDetails(AddressResultInput add)
        {
            try
            {
                var addressID = new MySqlParameter("?addressID", add.addressID);
                var addressType = new MySqlParameter("?addressType", add.addressType);
                var address = new MySqlParameter("?address", add.address);
                var city = new MySqlParameter("?city", add.city);
                var state = new MySqlParameter("?state", add.state);
                var country = new MySqlParameter("?country", add.country);
                var pincode = new MySqlParameter("?pincode", add.pincode);
                var phoneNo = new MySqlParameter("?phoneNo", add.phoneNo);
                var fax = new MySqlParameter("?fax", add.fax);
                var userID = new MySqlParameter("?userID", add.profileID);
                var groupID = new MySqlParameter("?groupID", add.groupID);
                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<int>("CALL USPUpdateAddressDetails(?addressID,?addressType,?address,?city,?state,?country,?pincode,?phoneNo,?fax,?userID,?groupID)", addressID, addressType, address, city, state, country, pincode, phoneNo, fax, userID, groupID).SingleOrDefault();

                    return Result;
                }
            }
            catch
            {
                return -1;
            }
        }

        public static int UpdateFamilyDetails(UpdateFamilyDetail mem)
        {
            try
            {
                var familyMemberId = new MySqlParameter("?familyMemberId", mem.familyMemberId);
                var memberName = new MySqlParameter("?memberName", mem.memberName);
                var relationship = new MySqlParameter("?relationship", mem.relationship);
                var dOB = new MySqlParameter("?dOB", mem.dOB);
                var anniversary = new MySqlParameter("?anniversary", mem.anniversary);
                var contactNo = new MySqlParameter("?contactNo", mem.contactNo);
                var particulars = new MySqlParameter("?particulars", mem.particulars);
                var bloodGroup = new MySqlParameter("?bloodGroup", mem.bloodGroup);
                var profile_ID = new MySqlParameter("?profileID", mem.profileID);
                var emailID = new MySqlParameter("?emailID", mem.emailID);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<int>("CALL USPUpdateFamilyDetails(?familyMemberId,?memberName,?relationship,?dOB,?anniversary,?contactNo,?particulars,?bloodGroup,?emailID,?profileID)",
                       familyMemberId, memberName, relationship, dOB, anniversary, contactNo, particulars, bloodGroup, emailID, profile_ID).SingleOrDefault();
                    return Result;
                }
            }
            catch
            {
                return -1;
            }
        }

        public static int UpdateDeviceToken(string deviceToken, string MemID)
        {
            try
            {
                var DeviceToken = new MySqlParameter("?dToken", deviceToken);
                var UserID = new MySqlParameter("?memID", MemID);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<int>("CALL USPUpdateDeviceToken(?dToken,?memID)", DeviceToken, UserID).SingleOrDefault();
                    return Result;
                }
            }
            catch
            {
                return -1;
            }
        }

        public static List<SearchFilters> GetSearchFilters(string grpID)
        {
            try
            {
                var groupID = new MySqlParameter("?grpID", grpID);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<SearchFilters>("CALL V6_USPGetSearchFilters(?grpID)", groupID).ToList();
                    return Result;
                }
            }
            catch
            {
                throw;
            }

        }

        public static List<MemberListResult> AdvanceSearch(AdvanceSearchInput input)
        {
            try
            {
                StringBuilder squery = new StringBuilder();
                squery.Append("select pk_member_master_profile_id as profileID,member_master_profile.fk_group_master_id as grpID,");
                squery.Append(" fk_main_member_master_id as masterUID,CONCAT(country_code ,' ', member_mobile_no) as membermobile,");
                squery.Append(" COALESCE(g.group_name,'')as groupName,");
                squery.Append(" COALESCE(member_profile_photo_path,'') as pic,");
                squery.Append(" COALESCE(member_name,'') as memberName,");
                squery.Append(" case when member_date_of_birth='1753/01/01' then '' when coalesce(member_date_of_birth ,'0000/00/00')='0000/00/00' then ''");
                squery.Append(" else DATE_FORMAT(coalesce(member_date_of_birth,''),'%d %b') end as DateOfBirthDisplay,");

                squery.Append(" case when member_date_of_birth='1753/01/01' then DATE_FORMAT(member_date_of_birth,'%Y%m%d')  when coalesce(member_date_of_birth ,'0000/00/00')='0000/00/00' then DATE_FORMAT(member_date_of_birth,'%Y%m%d') ");
                squery.Append(" else DATE_FORMAT(member_date_of_birth,'%Y%m%d') end as DateOfBirth,");

                squery.Append(" case when member_date_of_wedding='1753/01/01' then '' when coalesce(member_date_of_wedding ,'0000/00/00')='0000/00/00' then ''");
                squery.Append(" else DATE_FORMAT(coalesce(member_date_of_wedding,''),'%d %b') end as DateOfAnnDisplay,");

                squery.Append(" case when member_date_of_wedding='1753/01/01' then DATE_FORMAT(member_date_of_wedding,'%Y%m%d') when coalesce(member_date_of_wedding ,'0000/00/00')='0000/00/00' then DATE_FORMAT(member_date_of_wedding,'%Y%m%d') ");
                squery.Append(" else DATE_FORMAT(member_date_of_wedding,'%Y%m%d') end as DateOfAnn");

                squery.Append(" from member_master_profile");
                squery.Append(" join country_master c on fk_country_ID=country_master_id");
                squery.Append(" join group_master g on fk_group_master_id=pk_group_master_id");
                squery.Append(" join group_dynamic_fields on member_master_profile.fk_group_master_id=group_dynamic_fields.fk_group_master_id");
                squery.Append(" left join member_profile_dynamic_fields on fk_MemberProfileID=member_master_profile.pk_member_master_profile_id");
                squery.Append(" LEFT JOIN member_address_detail on fk_member_profile_id=pk_member_master_profile_id");
                squery.Append(" where member_master_profile.fk_group_master_id=");
                squery.Append(input.groupID);
                for (int i = 0; i < input.GroupFilters.Count; i++)
                {
                    if (input.GroupFilters[i].fieldType == "s")
                    {
                        switch (input.GroupFilters[i].dbColumnName)
                        {
                            case "member_name":
                                if (input.GroupFilters[i].dbColumnName != "")
                                {
                                    squery.Append(" and member_master_profile.member_name like '%");
                                    squery.Append(input.GroupFilters[i].value);
                                    squery.Append("%'");
                                }
                                break;

                            case "member_mobile_no":
                                if (input.GroupFilters[i].dbColumnName != "")
                                {
                                    squery.Append(" and ( member_master_profile.member_mobile_no like '%");
                                    squery.Append(input.GroupFilters[i].value);
                                    squery.Append("%'");
                                    squery.Append(" or member_master_profile.member_phone_no like '%");
                                    squery.Append(input.GroupFilters[i].value);
                                    squery.Append("%' )");
                                }
                                break;

                            case "member_date_of_birth":
                                if (input.GroupFilters[i].dbColumnName != "")
                                {
                                    squery.Append(" and member_master_profile.member_date_of_birth='");
                                    squery.Append(input.GroupFilters[i].value);
                                    squery.Append("'");
                                }
                                break;

                            case "member_date_of_wedding":
                                if (input.GroupFilters[i].dbColumnName != "")
                                {
                                    squery.Append(" and member_master_profile.member_date_of_wedding='");
                                    squery.Append(input.GroupFilters[i].value);
                                    squery.Append("'");
                                }
                                break;

                            case "designation":
                                if (input.GroupFilters[i].dbColumnName != "")
                                {
                                    squery.Append(" and member_master_profile.designation like '%");
                                    squery.Append(input.GroupFilters[i].value);
                                    squery.Append("%'");
                                }
                                break;

                            case "member_buss_email":
                                if (input.GroupFilters[i].dbColumnName != "")
                                {
                                    squery.Append(" and member_master_profile.member_buss_email like '%");
                                    squery.Append(input.GroupFilters[i].value);
                                    squery.Append("%'");
                                }
                                break;

                            case "member_email_id":
                                if (input.GroupFilters[i].dbColumnName != "")
                                {
                                    squery.Append(" and member_master_profile.member_email_id like '%");
                                    squery.Append(input.GroupFilters[i].value);
                                    squery.Append("%'");
                                }
                                break;

                            case "BusinessName":
                                if (input.GroupFilters[i].dbColumnName != "")
                                {
                                    squery.Append(" and member_master_profile.BusinessName like '%");
                                    squery.Append(input.GroupFilters[i].value);
                                    squery.Append("%'");
                                }
                                break;

                            case "Address":
                                if (input.GroupFilters[i].dbColumnName != "")
                                {
                                    squery.Append(" and (member_address_detail.Address like '%");
                                    squery.Append(input.GroupFilters[i].value);
                                    squery.Append("%'");
                                    squery.Append(" or member_address_detail.fk_city_id like '%");
                                    squery.Append(input.GroupFilters[i].value);
                                    squery.Append("%'");
                                    squery.Append(" or member_address_detail.fk_state_id like '%");
                                    squery.Append(input.GroupFilters[i].value);
                                    squery.Append("%' )");
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else if (input.GroupFilters[i].fieldType == "d")
                    {
                        squery.Append(" group_dynamic_fields.Field_Title='");
                        squery.Append(input.GroupFilters[i].dbColumnName);
                        squery.Append("'");
                        squery.Append(" and member_profile_dynamic_fields.fieldValue like '%");
                        squery.Append(input.GroupFilters[i].value);
                        squery.Append("%'");
                    }
                }
                squery.Append(" group by member_master_profile.fk_main_member_master_id");

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<MemberListResult>(squery.ToString(), string.Empty).ToList();
                    foreach (MemberListResult mem in Result)
                    {
                        if (!string.IsNullOrEmpty(mem.pic))
                        {
                            string ImageName = mem.pic.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/directory/";
                            mem.pic = path + ImageName;
                        }
                    }

                    return Result;
                }
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

        public static List<BODListResult> GetBODList(string grpId, string searchText)
        {
            List<BODListResult> result = new List<BODListResult>();
            MySqlParameter[] parameters = new MySqlParameter[2];
            parameters[0] = new MySqlParameter("?groupId", grpId);
            parameters[1] = new MySqlParameter("?searchText", string.IsNullOrEmpty(searchText) ? "" : searchText.Replace(' ', '%'));
            try
            {
                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    result = context.ExecuteStoreQuery<BODListResult>("CALL V1_USPGetBODList(?groupId,?searchText)", parameters).ToList();
                    foreach (BODListResult mem in result)
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
            catch
            {
                throw;
            }
            return result;
        }


    }
}