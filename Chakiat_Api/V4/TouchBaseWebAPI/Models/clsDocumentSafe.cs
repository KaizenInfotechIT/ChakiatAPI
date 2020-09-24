using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    public class clsDocumentSafe
    {
    }

    public class GetDocument
    {
        public string grpID { get; set; }
        public string memberProfileID { get; set; }

        public string type { get; set; }
        public string isAdmin { get; set; }
        public string searchText { get; set; }
    }

    public class DocumentList
    {
        public string docID { get; set; }
        public string docTitle { get; set; }
        public string docType { get; set; }

        public string docAccessType { get; set; }// Added by Nandu on 08/11/2016 Task :-> Access type to user(0-View / 1-Download) return 0 or 1 value
        public string isRead { get; set; }// Added by Nandu on 08/11/2016

        public string docURL { get; set; }
        public string createDateTime { get; set; }

        public string filterType { get; set; }
    }

    public class AddDocument
    {
        public string docID { get; set; }
        public string docType { get; set; }//All/Subgroup/Member

        public string docAccessType { get; set; }// Added by Nandu on 08/11/2016 Task :-> Access type to user(0 - View/ 1 - Download) 

        public string publishDate { get; set; }
        public string expiryDate { get; set; }

        public string docTitle { get; set; }
        public string memID { get; set; }//Created By
        public string grpID { get; set; }
        public string inputIDs { get; set; }
        public string documentFileId { get; set; }// ImageId
        public string isSubGrpAdmin { get; set; }
    }

    public class AddDocResult
    {
        public string docID { get; set; }
        public string ImgName { get; set; }
    }

    public class UpdateReadFlag
    {
        public string docID { get; set; }
        public string memberProfileID { get; set; }
    }
}