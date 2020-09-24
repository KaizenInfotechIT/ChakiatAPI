using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class DocumentSafe
    {
        private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static DataSet GetDocumentList(GetDocument doc)
        {
            try
            {
                //var groupId = new MySqlParameter("?groupId", doc.grpID);
                //var memId = new MySqlParameter("?memberProfileId", doc.memberProfileID);

                //var Result = _DBTouchbase.ExecuteStoreQuery<DocumentList>("CALL USPGetDocumentList(?groupId,?memberProfileId)", groupId, memId).ToList();

                //foreach (DocumentList document in Result)
                //{
                //    if (!string.IsNullOrEmpty(document.docURL))
                //    {
                //        string docURL = document.docURL.ToString();
                //        string path = ConfigurationManager.AppSettings["imgPath"] + "/Documents/documentsafe/Group" + doc.grpID + "/";
                //        document.docURL = path + docURL;
                //    }
                //}

                MySqlParameter[] param = new MySqlParameter[5];
                param[0] = new MySqlParameter("?groupId", doc.grpID);
                param[1] = new MySqlParameter("?memberProfileId", doc.memberProfileID);

                param[2] = new MySqlParameter("?searchText", string.IsNullOrEmpty(doc.searchText) ? "" : doc.searchText);
                param[3] = new MySqlParameter("?type", doc.type);
                param[4] = new MySqlParameter("?isAdmin", doc.isAdmin);

                DataSet Result = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.StoredProcedure, "V7_USPGetDocumentList", param);

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static AddDocResult createDocument(AddDocument doc)
        {
            string subgrpIDs="";
            try
            {
                if (doc.isSubGrpAdmin == "1" && doc.docType == "0")
                {
                    subgrpIDs = SubGroupDirectory.GetAdminSubGroupList(doc.grpID, doc.memID);
                }

                MySqlParameter[] param = new MySqlParameter[12];
                param[0] = new MySqlParameter("?docID", string.IsNullOrEmpty(doc.docID) ? "0" : doc.docID);
                param[1] = new MySqlParameter("?documentType", doc.docType);
                param[2] = new MySqlParameter("?documentTitle", doc.docTitle);

                param[3] = new MySqlParameter("?docAccessType", doc.docAccessType);

                param[4] = new MySqlParameter("?memID", doc.memID);
                param[5] = new MySqlParameter("?grpID", doc.grpID);
                param[6] = new MySqlParameter("?memprofileIDs", string.IsNullOrEmpty(doc.inputIDs) ? "" : doc.inputIDs);

                param[7] = new MySqlParameter("?documentFileId", string.IsNullOrEmpty(doc.documentFileId) ? "0" : doc.documentFileId);
                param[8] = new MySqlParameter("?isSubGrpAdmin",string.IsNullOrEmpty(doc.isSubGrpAdmin)?"0":doc.isSubGrpAdmin);
                param[9] = new MySqlParameter("?subgrpIDs", subgrpIDs);

                param[10] = new MySqlParameter("?publishDate", doc.publishDate);
                param[11] = new MySqlParameter("?expiryDate", doc.expiryDate);

                var Result = _DBTouchbase.ExecuteStoreQuery<AddDocResult>("CALL V6_USPAddDocument(?docID,?documentType,?documentTitle,?docAccessType,?memID,?grpID,?memprofileIDs,?documentFileId,?isSubGrpAdmin,?subgrpIDs,?publishDate,?expiryDate)", param).SingleOrDefault();
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int UpdateDocumentDetails(UpdateReadFlag doc)
        {
            try
            {
                var docID = new MySqlParameter("?docID", doc.docID);
                var memberProfileID = new MySqlParameter("?memberProfileID", doc.memberProfileID);

                var Result = _DBTouchbase.ExecuteStoreCommand("CALL USPUpdateDocumentDetails(?docID,?memberProfileID)", docID, memberProfileID);
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}