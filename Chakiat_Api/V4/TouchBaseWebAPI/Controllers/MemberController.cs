using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Configuration;
using TouchBaseWebAPI.Data;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;
using System.Web;
using System.IO;
using System.Drawing.Imaging;
using System.Text;


namespace TouchBaseWebAPI.Controllers
{
    public class MemberController : ApiController
    {
        private TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        [HttpPost]
        public object GetDirectoryList(MemberSearch member)
        {
            dynamic TBMemberResult;
            dynamic MemberListResults;
            List<object> MemberListResult = new List<object>();
            int pagesize = 10, pageno = 1, total;

            if (!string.IsNullOrEmpty(member.page))
            {
                pageno = Convert.ToInt32(member.page);
            }
            if (string.IsNullOrEmpty(member.searchText))
            {
                member.searchText = "";
            }
            if (string.IsNullOrEmpty(member.grpID))
            {
                member.grpID = "0";
            }
            if (string.IsNullOrEmpty(member.masterUID))
            {
                member.masterUID = "0";
            }
            if (string.IsNullOrEmpty(member.isSubGrpAdmin))
            {
                member.isSubGrpAdmin = "0";
            }
            int skippageno = pageno - 1;
            try
            {
                List<MemberListResult> Result = MemberMaster.GetDirectoryList(Convert.ToInt32(member.masterUID), member.grpID, member.searchText, member.updatedOn, Convert.ToInt32(member.isSubGrpAdmin), member.profileId);
                for (int i = 0; i < Result.Count; i++)
                {
                    MemberListResult.Add(new { MemberListResult = Result[i] });
                }

                if (Result.Count > 0)
                {
                    var totalPages = 1;
                    if (string.IsNullOrEmpty(member.page))
                    {
                        total = Result.Count;

                        MemberListResults = MemberListResult.ToList();
                    }
                    else
                    {
                        total = Result.Count;
                        totalPages = (int)Math.Ceiling((double)total / pagesize);
                        MemberListResults = MemberListResult.Skip(pagesize * skippageno).Take(pagesize).ToList();
                    }
                    TBMemberResult = new { status = "0", message = "success", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), resultCount = total.ToString(), TotalPages = totalPages.ToString(), currentPage = pageno.ToString(), MemberListResults };
                }
                else
                {
                    TBMemberResult = new { status = "1", message = "User Not Found", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
                }
            }
            catch
            {
                TBMemberResult = new { status = "1", message = "An error occured. Please contact Administrator", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
            }
            return new
            {
                TBMemberResult
            };
        }

        [HttpPost]
        public object GetMemberListSyncIOS(MemberSearch member)
        {
            dynamic MemberDirectorResult;
            try
            {
                string zipFilePath, FileName;
                MemberListSyncResult memberListSyncResult = MemberMaster.GetMemberListSyncIOS(member.updatedOn, member.grpID, out zipFilePath, out FileName);
                FileName = "Profile" + FileName + ".zip";
                if (!string.IsNullOrEmpty(zipFilePath))
                {
                    MemberDirectorResult = new { status = "0", message = "success", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, FileName, memberListSyncResult };
                }
                else if (memberListSyncResult.NewMemberList.Count == 0 && memberListSyncResult.UpdatedMemberList.Count == 0 && string.IsNullOrEmpty(memberListSyncResult.DeletedMemberList))
                {
                    MemberDirectorResult = new { status = "2", message = "No New Updates", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, FileName, memberListSyncResult };
                }
                else
                {
                    MemberDirectorResult = new { status = "0", message = "success", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, FileName, memberListSyncResult };
                }
            }
            catch (Exception ex)
            {
                MemberDirectorResult = new { status = "1", error = ex.ToString(), message = "An error occured. Please contact Administrator", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
            }
            return MemberDirectorResult;
        }

        #region comment

        //[System.Web.Http.HttpPost]
        //public object GetDirectoryList(MemberSearch member)
        //{
        //    int pagesize = 5, pageno;
        //    if (member.page == "")
        //    {
        //        pageno = 1;
        //    }
        //    else
        //    {
        //        pageno = Convert.ToInt32(member.page);
        //    }
        //    int skippageno = pageno - 1;
        //    dynamic MemberListFinal = null;
        //    int total = 0;
        //    dynamic MemberListResults = null;
        //    dynamic TBGroupResult;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(member.masterUID) && string.IsNullOrEmpty(member.grpID) && !string.IsNullOrEmpty(member.searchText))
        //        {
        //            //####################################  Search BY Search text only  ################################################################################
        //            MemberListFinal = (from e in _DBTouchbase.member_master_profile
        //                               join country in _DBTouchbase.country_master on e.fk_buss_country_master_id equals country.country_master_id
        //                               join grp in _DBTouchbase.group_master on e.fk_group_master_id equals grp.pk_group_master_id
        //                               orderby e.member_name
        //                               where (e.member_name.Contains(@member.searchText) || e.member_mobile_no.Contains(@member.searchText))
        //                               select new
        //                               {
        //                                   MemberListDetails = new
        //                                   {
        //                                       masterUID = e.fk_main_member_master_id,
        //                                       grpID = '"' + e.fk_group_master_id + '"',
        //                                       grpProfileID = '"' + e.pk_member_master_profile_id + '"',
        //                                       group_name = grp.group_name,
        //                                       pic = e.member_profile_photo_path ?? string.Empty,
        //                                       membername = e.member_name ?? string.Empty,
        //                                       membermobile = ("+" + country.country_code + e.member_mobile_no) ?? string.Empty
        //                                   }
        //                               }).Distinct();
        //            if (MemberListFinal.Count > 0)
        //            {
        //                total = MemberListFinal.ToList().Count;
        //                var totalPages = (int)Math.Ceiling((double)total / pagesize);
        //                MemberListResults = MemberListFinal.Skip(pagesize * skippageno).Take(pagesize);
        //                TBGroupResult = new { status = "0", message = "success", resultCount = total.ToString(), TotalPages = totalPages.ToString(), currentPage = pageno.ToString(), MemberListResults };

        //            }
        //            else
        //            {
        //                TBGroupResult = new { status = "1", message = "User Not Found" };

        //            }
        //        }
        //        else if (string.IsNullOrEmpty(member.masterUID) && !string.IsNullOrEmpty(member.grpID) && string.IsNullOrEmpty(member.searchText))
        //        {
        //            //############################### Search By GroupID alone #################################################################################

        //            int GroupID = Convert.ToInt32(member.grpID);
        //            MemberListFinal = (from e in _DBTouchbase.member_master_profile
        //                                   join country in _DBTouchbase.country_master on e.fk_buss_country_master_id equals country.country_master_id
        //                                   join grp in _DBTouchbase.group_master on e.fk_group_master_id equals grp.pk_group_master_id
        //                                   orderby e.fk_group_master_id
        //                                   where e.fk_group_master_id == GroupID
        //                                   select new
        //                                   {
        //                                       MemberListResult = new
        //                                       {
        //                                           masterUID = e.fk_main_member_master_id,
        //                                           grpID = e.fk_group_master_id,
        //                                           grpProfileID = e.pk_member_master_profile_id,
        //                                           group_name = grp.group_name,
        //                                           pic = e.member_profile_photo_path ?? string.Empty,
        //                                           membername = e.member_name ?? string.Empty,
        //                                           membermobile = ("+" + country.country_code + e.member_mobile_no) ?? string.Empty
        //                                       }
        //                                   }).Distinct().ToList();
        //            if (MemberListFinal.Count > 0)
        //            {
        //                total = MemberListFinal.ToList().Count;
        //                var totalPages = (int)Math.Ceiling((double)total / pagesize);
        //                MemberListResults = MemberListFinal.Skip(pagesize * skippageno).Take(pagesize);
        //                TBGroupResult = new { status = "0", message = "success", resultCount = total.ToString(), TotalPages = totalPages.ToString(), currentPage = pageno.ToString(), MemberListResults };

        //            }
        //            else
        //            {
        //                TBGroupResult = new { status = "1", message = "User Not Found" };

        //            }

        //        }
        //        else if (string.IsNullOrEmpty(member.masterUID) && !string.IsNullOrEmpty(member.grpID) && !string.IsNullOrEmpty(member.searchText))
        //        {
        //            //============================================ Search By GroupID and SearchText ================================================================

        //            int GroupID = Convert.ToInt32(member.grpID);
        //            MemberListFinal = (from e in _DBTouchbase.member_master_profile
        //                                   join country in _DBTouchbase.country_master on e.fk_buss_country_master_id equals country.country_master_id
        //                                   join grp in _DBTouchbase.group_master on e.fk_group_master_id equals grp.pk_group_master_id
        //                                   orderby e.fk_group_master_id
        //                                   where e.fk_group_master_id == GroupID && (e.member_name.Contains(@member.searchText) || e.member_mobile_no.Contains(@member.searchText))
        //                                   select new
        //                                   {
        //                                       MemberListResult = new
        //                                       {
        //                                           masterUID = e.fk_main_member_master_id,
        //                                           grpID = e.fk_group_master_id,
        //                                           grpProfileID = e.pk_member_master_profile_id,
        //                                           group_name = grp.group_name,
        //                                           pic = e.member_profile_photo_path ?? string.Empty,
        //                                           membername = e.member_name ?? string.Empty,
        //                                           membermobile = ("+" + country.country_code + e.member_mobile_no) ?? string.Empty
        //                                       }
        //                                   }).Distinct().ToList();
        //            if (MemberListFinal.Count > 0)
        //            {
        //                total = MemberListFinal.ToList().Count;
        //                var totalPages = (int)Math.Ceiling((double)total / pagesize);
        //                MemberListResults = MemberListFinal.Skip(pagesize * skippageno).Take(pagesize);
        //                TBGroupResult = new { status = "0", message = "success", resultCount = total.ToString(), TotalPages = totalPages.ToString(), currentPage = pageno.ToString(), MemberListResults };

        //            }
        //            else
        //            {
        //                TBGroupResult = new { status = "1", message = "User Not Found" };

        //            }
        //        }
        //        else if (!string.IsNullOrEmpty(member.masterUID) && !string.IsNullOrEmpty(member.grpID) && !string.IsNullOrEmpty(member.searchText))
        //        {
        //            //===================================Search By UID and GroupID and SearchText=====================================================================
        //            int UserID = Convert.ToInt32(member.masterUID);
        //            int GroupID = Convert.ToInt32(member.grpID);
        //             MemberListFinal = (from e in _DBTouchbase.member_master_profile.AsEnumerable()
        //                                   join country in _DBTouchbase.country_master.AsEnumerable() on e.fk_buss_country_master_id equals country.country_master_id
        //                                   join grp in _DBTouchbase.group_master.AsEnumerable() on e.fk_group_master_id equals grp.pk_group_master_id
        //                                   orderby e.fk_group_master_id
        //                                   where e.fk_group_master_id == GroupID && (e.member_name.Contains(@member.searchText) || e.member_mobile_no.Contains(@member.searchText))
        //                                   && e.fk_main_member_master_id == UserID
        //                                   select new
        //                                   {
        //                                       MemberListResult = new
        //                                       {
        //                                           masterUID = e.fk_main_member_master_id,
        //                                           grpID = e.fk_group_master_id,
        //                                           grpProfileID = e.pk_member_master_profile_id,
        //                                           group_name = grp.group_name,
        //                                           pic = e.member_profile_photo_path ?? string.Empty,
        //                                           membername = e.member_name ?? string.Empty,
        //                                           membermobile = ("+" + country.country_code + e.member_mobile_no) ?? string.Empty
        //                                       }
        //                                   }).Distinct().ToList();
        //             if (MemberListFinal.Count > 0)
        //             {
        //                 total = MemberListFinal.ToList().Count;
        //                 var totalPages = (int)Math.Ceiling((double)total / pagesize);
        //                 MemberListResults = MemberListFinal.Skip(pagesize * skippageno).Take(pagesize);
        //                 TBGroupResult = new { status = "0", message = "success", resultCount = total.ToString(), TotalPages = totalPages.ToString(), currentPage = pageno.ToString(), MemberListResults };

        //             }
        //             else
        //             {
        //                 TBGroupResult = new { status = "1", message = "User Not Found" };

        //             }
        //        }
        //        else if (!string.IsNullOrEmpty(member.masterUID) && string.IsNullOrEmpty(member.grpID) && string.IsNullOrEmpty(member.searchText))
        //        {
        //            //===================================Search by UID alone ==========================================================================
        //            int UID = Convert.ToInt32(member.masterUID);
        //             MemberListFinal = (
        //                                    from e in _DBTouchbase.member_master_profile
        //                                    join grp in _DBTouchbase.group_master on e.fk_group_master_id equals grp.pk_group_master_id
        //                                    join country in _DBTouchbase.country_master on e.fk_buss_country_master_id equals country.country_master_id
        //                                    orderby e.fk_group_master_id
        //                                    where
        //                                    (
        //                                        from m in _DBTouchbase.member_master_profile
        //                                        join master in _DBTouchbase.main_member_master
        //                                        on m.fk_main_member_master_id equals master.pk_main_member_master_id
        //                                        where m.fk_main_member_master_id == UID
        //                                        select m.fk_group_master_id
        //                                    ).Contains(e.fk_group_master_id)

        //                                    select new
        //                                    {
        //                                        MemberListResult = new
        //                                        {
        //                                            masterUID =e.fk_main_member_master_id,
        //                                            grpID = e.fk_group_master_id,
        //                                            grpProfileID = e.pk_member_master_profile_id,
        //                                            group_name = grp.group_name,
        //                                            pic = e.member_profile_photo_path ?? string.Empty,
        //                                            membername = e.member_name ?? string.Empty,
        //                                            membermobile = ("+" + country.country_code + e.member_mobile_no) ?? string.Empty
        //                                        }
        //                                    }).Distinct().ToList();
        //             if (MemberListFinal.Count > 0)
        //             {
        //                 total = MemberListFinal.ToList().Count;
        //                 var totalPages = (int)Math.Ceiling((double)total / pagesize);
        //                 MemberListResults = MemberListFinal.Skip(pagesize * skippageno).Take(pagesize);
        //                 TBGroupResult = new { status = "0", message = "success", resultCount = total.ToString(), TotalPages = totalPages.ToString(), currentPage = pageno.ToString(), MemberListResults };

        //             }
        //             else
        //             {
        //                 TBGroupResult = new { status = "1", message = "User Not Found" };

        //             }
        //        }
        //        else if (!string.IsNullOrEmpty(member.masterUID) && !string.IsNullOrEmpty(member.grpID) && string.IsNullOrEmpty(member.searchText))
        //        {
        //            //===================================Search By UID and GroupID=====================================================================
        //            int UserID = Convert.ToInt32(member.masterUID);
        //            int GroupId = Convert.ToInt32(member.grpID);
        //            MemberListFinal = (
        //                                   from e in _DBTouchbase.member_master_profile
        //                                   join grp in _DBTouchbase.group_master on e.fk_group_master_id equals grp.pk_group_master_id
        //                                   join country in _DBTouchbase.country_master on e.fk_buss_country_master_id equals country.country_master_id
        //                                   orderby e.fk_group_master_id
        //                                   where e.fk_group_master_id == GroupId && e.fk_main_member_master_id == UserID
        //                                   select new
        //                                   {
        //                                       MemberListResult = new
        //                                       {
        //                                           masterUID = e.fk_main_member_master_id,
        //                                           grpID = e.fk_group_master_id,
        //                                           grpProfileID = e.pk_member_master_profile_id,
        //                                           group_name = grp.group_name,
        //                                           pic = e.member_profile_photo_path ?? string.Empty,
        //                                           membername = e.member_name ?? string.Empty,
        //                                           membermobile = ("+" + country.country_code + e.member_mobile_no) ?? string.Empty
        //                                       }
        //                                   }).Distinct().ToList();
        //            if (MemberListFinal.Count > 0)
        //            {
        //                total = MemberListFinal.ToList().Count;
        //                var totalPages = (int)Math.Ceiling((double)total / pagesize);
        //                MemberListResults = MemberListFinal.Skip(pagesize * skippageno).Take(pagesize);
        //                TBGroupResult = new { status = "0", message = "success", resultCount = total.ToString(), TotalPages = totalPages.ToString(), currentPage = pageno.ToString(), MemberListResults };

        //            }
        //            else
        //            {
        //                TBGroupResult = new { status = "1", message = "User Not Found" };

        //            }
        //        }
        //        else
        //        {
        //            TBGroupResult = new { status = "1", message = "Please Enter Search Text" };

        //        }
        //        if (MemberListFinal.Count > 0)
        //        {
        //            total = MemberListFinal.Count;
        //            foreach (var item in MemberListFinal)
        //            {

        //            }

        //            var totalPages = (int)Math.Ceiling((double)total / pagesize);
        //            MemberListResults = MemberListFinal.Skip(pagesize * skippageno).Take(pagesize);
        //            TBGroupResult = new { status = "0", message = "success", resultCount = total.ToString(), TotalPages = totalPages.ToString(), currentPage = pageno.ToString(), MemberListResults };

        //        }
        //        else
        //        {
        //            TBGroupResult = new { status = "1", message = "User Not Found" };

        //        }

        //    }

        //    catch
        //    {
        //        TBGroupResult = new { status = "1", message = "An error occured. Please contact Administrator" };

        //    }
        //    return new
        //    {
        //        TBGroupResult
        //    };
        //}


        //[System.Web.Http.HttpGet]
        //public object GetDirectoryDetails(string grpuserID, string grpID)
        //{
        //    try
        //    {
        //        int GroupUserID = Convert.ToInt32(grpuserID);
        //        int GroupID = Convert.ToInt32(grpID);
        //        var MemberDetails = (from e in _DBTouchbase.member_master_profile
        //                             join res_con in _DBTouchbase.single_field_master on e.fk_res_country_id equals res_con.pk_single_field_master_id
        //                             join res_st in _DBTouchbase.single_field_master on e.fk_res_state_ID equals res_st.pk_single_field_master_id
        //                             join res_city in _DBTouchbase.single_field_master on e.fk_res_city_id equals res_city.pk_single_field_master_id
        //                             join bus_con in _DBTouchbase.single_field_master on e.fk_buss_country_master_id equals bus_con.pk_single_field_master_id
        //                             join bus_st in _DBTouchbase.single_field_master on e.fk_buss_state_master_id equals bus_st.pk_single_field_master_id
        //                             join bus_city in _DBTouchbase.single_field_master on e.fk_buss_city_master_id equals bus_city.pk_single_field_master_id
        //                             join grp in _DBTouchbase.group_master on e.fk_group_master_id equals grp.pk_group_master_id
        //                             where e.fk_group_master_id == GroupID && e.pk_member_master_profile_id == GroupUserID
        //                             select new
        //                             {
        //                                 MemberDetail = new
        //                                 {
        //                                     memberID = e.fk_main_member_master_id,
        //                                     grpID = e.fk_group_master_id,
        //                                     grpuserID = e.pk_member_master_profile_id,
        //                                     isAdmin = (e.user_role == "Admin" ? 0 : 1),
        //                                     isDeleted = e.isdeleted,
        //                                     membername = e.member_name ?? string.Empty,
        //                                     memberCountry = res_con.english ?? string.Empty,
        //                                     bloodgrp = e.blood_Group ?? string.Empty,
        //                                     membermobile = e.member_mobile_no ?? string.Empty,
        //                                     memberemail = e.member_buss_email ?? string.Empty,
        //                                     secondrymobile = e.member_phone_no ?? string.Empty,
        //                                     alternateEmail = e.member_email_id ?? string.Empty,
        //                                     BOD = e.member_date_of_birth.Value.ToShortDateString(),
        //                                     anniversary = e.member_date_of_wedding,
        //                                     profilePic = e.member_profile_photo_path ?? string.Empty,
        //                                     residentAddres = e.member_residence_address ?? string.Empty,
        //                                     residentState = res_st.english ?? string.Empty,
        //                                     residentCity = res_city.english ?? string.Empty,
        //                                     residentPincode = e.res_pincode ?? string.Empty,
        //                                     residentPhone = e.res_phone_no ?? string.Empty,
        //                                     businessAddres = e.member_business_address ?? string.Empty,
        //                                     businessCountry = bus_con.english ?? string.Empty,
        //                                     businessState = bus_st.english ?? string.Empty,
        //                                     businessCity = bus_city.english ?? string.Empty,
        //                                     businessPincode = e.res_pincode ?? string.Empty,
        //                                     businessfax = e.member_fax_no ?? string.Empty,
        //                                     businessphone = e.member_buss_phone_no ?? string.Empty,
        //                                     businessEmail = e.member_buss_email ?? string.Empty,
        //                                     isPersonalDetVisible = "",
        //                                     isBusinDetVisible = "",
        //                                     isFamilDetailVisible = "",
        //                                     familyMemberDetails = (from d in _DBTouchbase.member_family_master
        //                                                            where d.fk_member_master_id == e.fk_main_member_master_id
        //                                                            select new
        //                                                            {
        //                                                                familyMemberDetail = new
        //                                                                {
        //                                                                    familymobile = d.member_contact_no ?? string.Empty,
        //                                                                    familyname = d.member_name ?? string.Empty,
        //                                                                    familyrelation = d.member_relationship,
        //                                                                    familyParticulars = d.particulars ?? string.Empty,
        //                                                                    familyBOD = d.member_date_of_birth,
        //                                                                    familyAnniversary = d.member_date_of_wedding
        //                                                                }
        //                                                            })

        //                                 }
        //                             }).SingleOrDefault();
        //        if (MemberDetails != null)
        //        {
        //            var MemberListDEtailResult = new { status = "0", message = "sucess", MemberDetails };
        //            return new
        //            {
        //                MemberListDEtailResult
        //            };
        //        }
        //        else
        //        {
        //            var MemberListDEtailResult = new { status = "1", message = "User Not Found" };
        //            return new
        //            {
        //                MemberListDEtailResult
        //            };
        //        }
        //    }
        //    catch
        //    {
        //        var MemberListDEtailResult = new { status = "1", message = "An error occured. Please contact Administrator" };
        //        return new
        //        {
        //            MemberListDEtailResult
        //        };
        //    }
        //}

        #endregion
        [HttpPost]
        public object UpdateProfile(MemberProfileUpdate member)
        {
            dynamic UserResult;
            int str;
            int UID = Convert.ToInt32(member.ProfileId);
            try
            {
                string result = MemberMaster.updateMemberDetails(member);

                if (!string.IsNullOrEmpty(result))
                {
                    str = GlobalFuns.UploadImage(result, UID, "MemberProfile");
                }
                else
                    str = 0;
                if (str == 0)
                    UserResult = new { status = "0", message = "success" };
                else
                    UserResult = new { status = "1", message = "failed" };
            }
            catch
            {
                UserResult = new { status = "1", message = "failed" };
            }

            return new { UserResult };
        }

        [HttpPost]
        public object GetMember(MemberPro member)
        {
            List<object> MemberListDetails = new List<object>();
            dynamic MemberListDetailResult;
            try
            {

                List<MemberDetail> result = MemberMaster.GetMember(member.memberProfileId, member.groupId);
                for (int i = 0; i < result.Count; i++)
                {
                    MemberListDetails.Add(new { MemberListDetail = result[i] });
                }

                if (MemberListDetails != null)
                {
                    MemberListDetailResult = new { status = "0", message = "sucess", MemberDetails = MemberListDetails };
                }
                else
                {
                    MemberListDetailResult = new { status = "1", message = "User Not Found" };
                }
            }
            catch
            {
                MemberListDetailResult = new { status = "1", message = "An error occured. Please contact Administrator" };
            }
            return new
            {
                MemberListDetailResult
            };
        }

        [HttpPost]
        public object GetMemberListSync(MemberSearch member)
        {
            dynamic MemberDirectorResult;
            try
            {
                string zipFilePath;
                MemberListSyncResult MemberDetail = new MemberListSyncResult();
                if (member.grpID == "31072" & Convert.ToDateTime(member.updatedOn).Date.ToString("dd/MM/yyyy") == "01/01/1970")
                {
                    zipFilePath = "http://www.rosteronwheels.com/TempDocuments/DirectoryData/Profile10062017054630PM.zip";
                }
                else
                {
                    MemberDetail = MemberMaster.GetMemberListSync(member.updatedOn, member.profileId, member.grpID, out zipFilePath);
                }
                

                if (!string.IsNullOrEmpty(zipFilePath))
                {
                    MemberDirectorResult = new { status = "0", message = "success", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, MemberDetail };
                }
                else if (MemberDetail.NewMemberList.Count == 0 && MemberDetail.UpdatedMemberList.Count == 0 && string.IsNullOrEmpty(MemberDetail.DeletedMemberList))
                {
                    MemberDirectorResult = new { status = "2", message = "No New Updates", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, MemberDetail };
                }
                else
                {
                    MemberDirectorResult = new { status = "0", message = "success", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, MemberDetail };
                }
            }
            catch (Exception ex)
            {
                MemberDirectorResult = new { status = "1", error = ex.ToString(), message = "An error occured. Please contact Administrator", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
            }
            return MemberDirectorResult;
        }

        [HttpPost]
        public object GetMemberListSync_test(MemberSearch member)
        {
            dynamic MemberDirectorResult;
            try
            {
                string zipFilePath;
                MemberListSyncResult MemberDetail = new MemberListSyncResult();
                if (member.grpID == "31072" & Convert.ToDateTime(member.updatedOn).Date.ToString("dd/MM/yyyy") == "01/01/1970")
                {
                    zipFilePath = "http://www.rosteronwheels.com/TempDocuments/DirectoryData/Profile10062017054630PM.zip";
                }
                else
                {
                    MemberDetail = MemberMaster.GetMemberListSync_Test(member.updatedOn, member.grpID, out zipFilePath);
                }


                if (!string.IsNullOrEmpty(zipFilePath))
                {
                    MemberDirectorResult = new { status = "0", message = "success", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, MemberDetail };
                }
                else if (MemberDetail.NewMemberList.Count == 0 && MemberDetail.UpdatedMemberList.Count == 0 && string.IsNullOrEmpty(MemberDetail.DeletedMemberList))
                {
                    MemberDirectorResult = new { status = "2", message = "No New Updates", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, MemberDetail };
                }
                else
                {
                    MemberDirectorResult = new { status = "0", message = "success", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, MemberDetail };
                }
            }
            catch (Exception ex)
            {
                MemberDirectorResult = new { status = "1", error = ex.ToString(), message = "An error occured. Please contact Administrator", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
            }
            return MemberDirectorResult;
        }


        /// <summary>
        /// Created On : 02-05-2017
        /// Created By : Nandu
        /// task : New API -- Update Profile on single click 
        /// </summary>
        [HttpPost]
        public object UpdateProfileDetails(RootObject Input)
        {
            dynamic MemberDirectorResult;
            int result = MemberMaster.UpdateAllProfileDetails(Input);

            if (result > 0)
            {
                string profileID = Input.profileID;
                 MemberMaster.AddMemberToDistrict(profileID,profileID);

                DateTime dt = DateTime.Now;
                if (Input.updatedOn != null)
                {
                    dt = Convert.ToDateTime(Input.updatedOn);
                    dt = dt.AddSeconds(-2);
                }

                // Forward to GetMemberListSync method to get updated records
                string zipFilePath = string.Empty;
                MemberListSyncResult MemberDetail = MemberMaster.GetMemberListSync(dt.ToString("yyyy/MM/dd HH:mm:ss"),null, Input.grpID, out zipFilePath);

                if (!string.IsNullOrEmpty(zipFilePath))
                {
                    MemberDirectorResult = new { status = "0", message = "success", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, MemberDetail };
                }
                else if (MemberDetail.NewMemberList.Count == 0 && MemberDetail.UpdatedMemberList.Count == 0 && string.IsNullOrEmpty(MemberDetail.DeletedMemberList))
                {
                    MemberDirectorResult = new { status = "2", message = "No New Updates", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, MemberDetail };
                }
                else
                {
                    MemberDirectorResult = new { status = "0", message = "success", curDate = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), zipFilePath, MemberDetail };
                }
            }
            else
            {
                MemberDirectorResult = new { status = "1", message = "failed" };
            }
            return MemberDirectorResult;
        }

        [HttpPost]
        public object DeleteFolder(DeleteFolderInput folder)
        {
            dynamic DeleteFolderResult;
            try
            {
                int result;
                if (folder.folderPath == "Profile10062017054630PM.zip")
                {
                    result = 1;
                }
                else{
                result= MemberMaster.DeleteZipFolder(folder.folderPath);
                }
                if (result == 1)
                {
                    DeleteFolderResult = new { status = "0", message = "success" };
                }
                else
                {
                    DeleteFolderResult = new { status = "1", message = "falied" };
                }
            }
            catch
            {
                DeleteFolderResult = new { status = "1", message = "falied" };
            }
            return DeleteFolderResult;
        }

        [HttpPost]
        public object GetMemberWithDynamicFields(MemberPro member)
        {

            dynamic MemberListDetailResult;
            try
            {


                MemberDetailsDynamicField result = MemberMaster.GetMemberDtlWithDynamicFeild(member.memberProfileId, member.groupId);
                if (result != null)
                {
                    MemberListDetailResult = new { status = "0", message = "sucess", MemberDetails = result };
                }
                else
                {
                    MemberListDetailResult = new { status = "1", message = "User Not Found" };
                }
            }
            catch
            {
                MemberListDetailResult = new { status = "1", message = "An error occured. Please contact Administrator" };
            }
            return new
            {
                MemberListDetailResult
            };
        }

        [HttpPost]
        public object GetBdayAnnList(member_master_profile mem)
        {
            dynamic TBGetBdayAnnResult;

            List<object> CelebrationListResult = new List<object>();

            try
            {
                List<CelebrationList> Result = MemberMaster.getBdayAnnList(mem.pk_member_master_profile_id.ToString(), mem.fk_group_master_id.ToString(), String.Format("{0:yyyy/MM/dd}", mem.member_date_of_birth));

                for (int i = 0; i < Result.Count; i++)
                {
                    CelebrationListResult.Add(new { CelebrationList = Result[i] });
                }

                if (Result != null)
                {
                    TBGetBdayAnnResult = new { status = "0", message = "success", CelebrationListResult = CelebrationListResult };
                }
                else
                {
                    TBGetBdayAnnResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetBdayAnnResult = new { status = "1", message = "failed" };
            }

            return new { TBGetBdayAnnResult };
        }

        //[HttpPost]
        //public object GetDirectoryDetail(MemberPro mem)
        //{
        //    dynamic MemberListDetailResult;
        //    List<object> MemberDetails = new List<object>();

        //    try
        //    {
        //        var Result = MemberMaster.GetDirectoryDetail(mem);

        //        for (int i = 0; i < Result.Count; i++)
        //        {
        //            //EventsDetailResult.Add(new { EventsDetail = Result[i] });
        //        }

        //        if (Result != null)
        //        {
        //            MemberListDetailResult = new { status = "0", message = "success", MemberDetails };
        //        }
        //        else
        //        {
        //            MemberListDetailResult = new { status = "0", message = "Record not found" };
        //        }
        //    }
        //    catch
        //    {
        //        MemberListDetailResult = new { status = "1", message = "failed" };
        //    }
        //    return new { MemberListDetailResult };
        //}
        public object UploadProfilePhoto(string ProfileID, string GrpID, string Type)
        {
            dynamic UploadImageResult;
            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    bool flag = false;
                    string FileName = DateTime.Now.ToString("ddMMyyyyhhmmsstt") + ".png";

                    string Path = ConfigurationManager.AppSettings["imgPathSave"] + "Documents\\directory";
                    string filePath = string.Empty;

                    if (Type == "profile")
                    {
                        filePath = Path + "\\" + FileName;
                    }
                    else
                    {
                        filePath = Path + "\\" + FileName;
                    }

                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];

                        if (!Directory.Exists(Path))
                            Directory.CreateDirectory(Path);

                        postedFile.SaveAs(filePath);
                        flag = true;
                    }

                    if (Type == "profile")//create thumb for profile pic only
                    {
                        //Create Thumbnail
                        System.Drawing.Image img1 = System.Drawing.Image.FromFile(filePath);
                        using (var newImage = GlobalFuns.ScaleImage(img1, 250, 250))
                        {
                            string path1 = Path + "\\thumb\\";
                            if (!Directory.Exists(path1))
                                Directory.CreateDirectory(path1);
                            newImage.Save(path1 + "\\" + FileName, ImageFormat.Png);
                        }
                    }

                    if (flag)
                    {
                        var Result = MemberMaster.EditPhoto(FileName, ProfileID, Type);
                        if (Result != null)
                        {
                            UploadImageResult = new { status = "0", message = "success", Imagepath = ConfigurationManager.AppSettings["imgPath"] + "Documents//directory" + "//" + FileName };
                        }
                        else
                        {
                            UploadImageResult = new { status = "1", message = "failed" };
                        }
                    }
                    else
                    {
                        UploadImageResult = new { status = "1", message = "failed" };
                    }
                }
                else
                {
                    UploadImageResult = new { status = "0", message = "No file selected" };
                }
            }
            catch
            {
                UploadImageResult = new { status = "1", message = "failed" };
            }
            return new
            {
                UploadImageResult
            };
        }

        [HttpPost]
        public object UpdateProfilePersonalDetails(UpdatePersonalDetails Input)
        {
            dynamic UserResult;
            string strPostData = Input.key;
            int result = MemberMaster.UpdatePersonalDetails(Input.key, Input.profileID);
            if (result == 1)
            {
                UserResult = new { status = "0", message = "success" };
            }
            else
            {
                UserResult = new { status = "1", message = "failed" };
            }
            return new { UserResult };
        }

        [HttpPost]
        public object UpdatePersonalDynamicDtls(UpdatePersonalDetails Input)
        {
            dynamic UserResult;
            string strPostData = Input.key;
            int result = MemberMaster.UpdatePersonalAndDynamicDtls(Input.key, Input.profileID);
            if (result == 1)
            {
                UserResult = new { status = "0", message = "success" };
            }
            else
            {
                UserResult = new { status = "1", message = "failed" };
            }
            return new { UserResult };
        }

        //[HttpPost]
        //public object UpdateProfileBusinessDetails(UpdatePersonalDetails Input)
        //{
        //    dynamic UserResult;
        //    string strPostData = Input.key;
        //    int result = MemberMaster.UpdateBusinessDetails(Input.key, Input.profileID);
        //    if (result == 1)
        //    {
        //        UserResult = new { status = "0", message = "success" };
        //    }
        //    else
        //    {
        //        UserResult = new { status = "1", message = "failed" };
        //    }
        //    return new { UserResult };
        //}

        [HttpPost]
        public object UpdateAddressDetails(AddressResultInput Input)
        {
            dynamic UpdateAddressResult;
            int result = MemberMaster.UpdateAddressDetails(Input);
            if (result == 1)
            {
                UpdateAddressResult = new { status = "0", message = "success" };
            }
            else
            {
                UpdateAddressResult = new { status = "1", message = "failed" };
            }
            return new { UpdateAddressResult };
        }

        [HttpPost]
        public object UpdateFamilyDetails(UpdateFamilyDetail Input)
        {
            dynamic UpdateFamilyResult;
            int result = MemberMaster.UpdateFamilyDetails(Input);
            if (result == 1)
            {
                UpdateFamilyResult = new { status = "0", message = "success" };
            }
            else
            {
                UpdateFamilyResult = new { status = "1", message = "failed" };
            }
            return new { UpdateFamilyResult };
        }

        /// <summary>
        /// Demo API To get 10000 members in one go
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object ReadMemberFile()
        {
            dynamic result;
            string path = ConfigurationManager.AppSettings["imgPathSave"] + "\\Documents\\sampledata.txt";
            //"D:\\Data\\Rupali\\V3\\TouchBaseWebAPI\\Documents\\sampledata.txt";
            //ConfigurationManager.AppSettings["imgPath"] + "/Documents/sampledata.txt";
            StringBuilder strbuild = new StringBuilder();
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    string[] readText = File.ReadAllLines(path);

                    foreach (string s in readText)
                    {
                        strbuild.Append(s);
                        strbuild.AppendLine();
                    }
                }
                result = strbuild.ToString();
            }

            catch (FileNotFoundException ex)
            {

            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return new { strbuild };
        }

        [HttpPost]
        public object GetAdvanceSearchFilters(SearchFilterInput Input)
        {
            dynamic SearchFilterResult;
            var result = MemberMaster.GetSearchFilters(Input.groupId);
            if (result != null)
            {
                SearchFilterResult = new { status = "0", message = "success", GroupFilters = result };
            }
            else
            {
                SearchFilterResult = new { status = "1", message = "failed" };
            }
            return new { SearchFilterResult };
        }

        [HttpPost]
        public object AdvanceSearch(AdvanceSearchInput searchFilter)
        {
            dynamic SearchResult;
            var result = MemberMaster.AdvanceSearch(searchFilter);

            if (result != null)
            {
                SearchResult = new { status = "0", message = "success", result };
            }
            else
            {
                SearchResult = new { status = "1", message = "failed" };
            }
            return new { SearchResult };
        }

        # region Board Of Directors

        [HttpPost]
        public object GetBODList(BODInput list)
        {
            dynamic TBGetBODResult;
            try
            {
                List<BODListResult> Result = MemberMaster.GetBODList(list.grpId, list.searchText);


                if (Result != null)
                {
                    TBGetBODResult = new { status = "0", message = "success", BODResult = Result };
                }
                else
                {
                    TBGetBODResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetBODResult = new { status = "1", message = "failed" };
            }

            return new { TBGetBODResult };
        }


        # endregion

    }
}

