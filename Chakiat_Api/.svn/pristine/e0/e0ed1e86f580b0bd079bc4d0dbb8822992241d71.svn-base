using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class clsAttendance
    {
    }

    public class clsGetAttendanceList
    {
        public string groupProfileID { get; set; }
        public string moduleID { get; set; }
        public string month { get; set; }
        public string year { get; set; }
    }

    public class AttendanceList
    {
        public string idno { get; set; }
        public string name { get; set; }
        public string attendence { get; set; }
    }

    public class GetAttendanceListNew_Input
    {
        public string groupProfileID { get; set; }
        public string groupID { get; set; }
        public string searchText { get; set; }
        public string type { get; set; }
    }

    public class GetAttendanceListNew
    {
        public string AttendanceID { get; set; }
        public string AttendanceName { get; set; }
        public string AttendanceDate { get; set; }
        public string Attendancetime { get; set; }
    }

    public class GetAttendanceEventsListNew
    {
        public string eventID { get; set; }
        public string eventTitle { get; set; }
        public string eventDesc { get; set; }
        public string eventDateTime { get; set; }
    }

    public class GetAttendanceDetails_Input
    {
        public string AttendanceID { get; set; }
        public int type { get; set; }
        public int createdBy { get; set; }
    }

    public class GetAttendanceDetails
    {
        public string AttendanceID { get; set; }
        public string AttendanceName { get; set; }
        public string AttendanceDate { get; set; }
        public string Attendancetime { get; set; }
        public string AttendanceDesc { get; set; }

        public int MemberCount { get; set; }
        public int AnnsCount { get; set; }
        public int AnnetsCount { get; set; }
        public int VisitorsCount { get; set; }
        public int RotarianCount { get; set; }
        public int DistrictDelegatesCount { get; set; }
    }

    public class RootObjectAttendance
    {
        public AttendanceAddEdit_Input AttendanceAddEdit_Input { get; set; }
        public List<AttendanceMembers> AttendanceMembers { get; set; }
        public List<AttendanceAnns> AttendanceAnns { get; set; }
        public List<AttendanceAnnets> AttendanceAnnets { get; set; }
        public List<AttendanceVisitors> AttendanceVisitors { get; set; }
        public List<AttendanceRotarians> AttendanceRotarians { get; set; }
        public List<AttendanceDistrictDelegate> AttendanceDistrictDelegate { get; set; }
    }

    public class AttendanceAddEdit_Input
    {
        public string AttendanceID { get; set; }
        public string AttendanceName { get; set; }
        public string AttendanceDesc { get; set; }
        public DateTime AttendanceDate { get; set; }

        public int fk_group_id { get; set; }
        public int fk_module_id { get; set; }
        public int created_by { get; set; }
        public int modification_by { get; set; }
        public int deleted_by { get; set; }
        public int FK_eventID { get; set; }

        public int MemberCount { get; set; }
        public int AnnsCount { get; set; }
        public int AnnetsCount { get; set; }
        public int VisitorsCount { get; set; }
        public int RotarianCount { get; set; }
        public int DistrictDelegatesCount { get; set; }
    }

    //Member added edit properties
    public class AttendanceMembers
    {
        public List<newMembers> newMembers { get; set; }
        public List<deleteMembers> deletedMembers { get; set; }
    }
    public class newMembers
    {
        public int FK_MemberID { get; set; }
        public string MemberName { get; set; }
        public string Designation { get; set; }
        public string image { get; set; }
    }
    public class deleteMembers
    {
        public int FK_MemberID { get; set; }
        public string MemberName { get; set; }
        public string Designation { get; set; }
        public string image { get; set; }
    }

    //Anns added edit properties
    public class AttendanceAnns
    {
        public List<newAnns> newAnns { get; set; }
        public List<UpdateAnns> UpdateAnns { get; set; }
        public List<deletedAnns> deletedAnns { get; set; }
    }
    public class newAnns
    {
        public string PK_AttendanceAnnsID { get; set; }
        public string Fk_AttendanceID { get; set; }
        public string AnnsName { get; set; }
    }
    public class UpdateAnns
    {
        public string PK_AttendanceAnnsID { get; set; }
        public string Fk_AttendanceID { get; set; }
        public string AnnsName { get; set; }
    }
    public class deletedAnns
    {
        public string PK_AttendanceAnnsID { get; set; }
        public string Fk_AttendanceID { get; set; }
        public string AnnsName { get; set; }
    }

    //Annets added edit properties
    public class AttendanceAnnets
    {
        public List<newAnnets> newAnnets { get; set; }
        public List<UpdateAnnets> UpdateAnnets { get; set; }
        public List<deletedAnnets> deletedAnnets { get; set; }
    }

    public class newAnnets
    {
        public string PK_AttendanceAnnetsID { get; set; }
        public string Fk_AttendanceID { get; set; }
        public string AnnetsName { get; set; }
    }
    public class UpdateAnnets
    {
        public string PK_AttendanceAnnetsID { get; set; }
        public string Fk_AttendanceID { get; set; }
        public string AnnetsName { get; set; }
    }
    public class deletedAnnets
    {
        public string PK_AttendanceAnnetsID { get; set; }
        public string Fk_AttendanceID { get; set; }
        public string AnnetsName { get; set; }
    }

    //Visitors added edit properties
    public class AttendanceVisitors
    {
        public List<newVisitors> newVisitors { get; set; }
        public List<UpdateVisitors> UpdateVisitors { get; set; }
        public List<deletedVisitors> deletedVisitors { get; set; }
    }

    public class newVisitors
    {
        public string PK_AttendanceVisitorID { get; set; }
        public string FK_AttendanceID { get; set; }
        public string VisitorsName { get; set; }
        public string Rotarian_whohas_Brought { get; set; }
    }
    public class UpdateVisitors
    {
        public string PK_AttendanceVisitorID { get; set; }
        public string FK_AttendanceID { get; set; }
        public string VisitorsName { get; set; }
        public string Rotarian_whohas_Brought { get; set; }
    }
    public class deletedVisitors
    {
        public string PK_AttendanceVisitorID { get; set; }
        public string FK_AttendanceID { get; set; }
        public string VisitorsName { get; set; }
        public string Rotarian_whohas_Brought { get; set; }
    }

    //Rotarians added edit properties
    public class AttendanceRotarians
    {
        public List<newRotarians> newRotarians { get; set; }
        public List<UpdateRotarians> UpdateRotarians { get; set; }
        public List<deletedRotarians> deletedRotarians { get; set; }        
    }
    public class newRotarians
    {
        public string PK_AttendanceRotarianID { get; set; }
        public string FK_AttendanceID { get; set; }
        public string RotarianID { get; set; }
        public string RotarianName { get; set; }
        public string ClubName { get; set; }
    }
    public class UpdateRotarians
    {
        public string PK_AttendanceRotarianID { get; set; }
        public string FK_AttendanceID { get; set; }
        public string RotarianID { get; set; }
        public string RotarianName { get; set; }
        public string ClubName { get; set; }
    }
    public class deletedRotarians
    {
        public string PK_AttendanceRotarianID { get; set; }
        public string FK_AttendanceID { get; set; }
        public string RotarianID { get; set; }
        public string RotarianName { get; set; }
        public string ClubName { get; set; }
    }
    //Delegate added edit properties
    public class AttendanceDistrictDelegate
    {
        public List<newDistrictDelegate> newDistrictDelegate { get; set; }
        public List<UpdateDistrictDelegate> UpdateDistrictDelegate { get; set; }
        public List<deletedDistrictDelegate> deletedDistrictDelegate { get; set; }
        
    }
    public class newDistrictDelegate
    {
        public string PK_AttendanceDelegateID { get; set; }
        public string FK_AttendanceID { get; set; }
        public string RotarianName { get; set; }
        public string DistrictDesignation { get; set; }
        public string ClubName { get; set; }
    }
    public class UpdateDistrictDelegate
    {
        public string PK_AttendanceDelegateID { get; set; }
        public string FK_AttendanceID { get; set; }
        public string RotarianName { get; set; }
        public string DistrictDesignation { get; set; }
        public string ClubName { get; set; }
    }
    public class deletedDistrictDelegate
    {
        public string PK_AttendanceDelegateID { get; set; }
        public string FK_AttendanceID { get; set; }
        public string RotarianName { get; set; }
        public string DistrictDesignation { get; set; }
        public string ClubName { get; set; }
    }

    //public class AttendanceMemberDetails
    //{
    //    public string MemberCount { get; set; }
    //    public List<AttendanceMembers> AttendanceMembers { get; set; }
    //}

    public class GetAttendanceRotarian_Input
    {
        public string RotarianID { get; set; }
        public string RotarianName { get; set; }
    }

    public class GetAttendanceRotarianDetailsbyID
    {
        public string Rotarian_Name { get; set; }
        public string Rotarian_ID { get; set; }
        public string Club_Name { get; set; }
    }

    public class GetAttendanceDelegateDetailsByRotarianName
    {
        public string Designation { get; set; }
        public string RotarianName { get; set; }
        public string clubName { get; set; }
    }
}