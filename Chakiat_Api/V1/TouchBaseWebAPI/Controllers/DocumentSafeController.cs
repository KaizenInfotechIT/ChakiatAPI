using System.Collections.Generic;
using System.Web.Http;
using TouchBaseWebAPI.BusinessEntities;
using TouchBaseWebAPI.Models;
using System.Configuration;
using System.Data;

namespace TouchBaseWebAPI.Controllers
{
    public class DocumentSafeController : ApiController
    {
        [System.Web.Http.HttpPost]
        public object GetDocumentList(GetDocument doc)
        {
            dynamic TBDocumentistResult;
            List<object> DocumentListResult = new List<object>();
            DataSet Result = new DataSet();

            try
            {
                Result = DocumentSafe.GetDocumentList(doc);
                DataTable dt = Result.Tables[0];
                DataTable dt1 = Result.Tables[1];

                List<DocumentList> res = new List<DocumentList>();

                if (dt.Rows.Count > 0)
                {
                    res = GlobalFuns.DataTableToList<DocumentList>(dt);

                    for (int i = 0; i < res.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(res[i].docURL))
                        {
                            string docURL = res[i].docURL.ToString();
                            string path = ConfigurationManager.AppSettings["imgPath"] + "Documents/documentsafe/Group" + doc.grpID + "/";
                            res[i].docURL = path + docURL;
                        }
                    }
                }


                for (int i = 0; i < res.Count; i++)
                {
                    DocumentListResult.Add(new { DocumentList = res[i] });
                }

                if (res != null && res.Count != 0)
                {
                    TBDocumentistResult = new { status = "0", message = "success", smscount = dt1.Rows[0]["SMSCount"].ToString(), DocumentLsitResult = DocumentListResult };
                }
                else
                {
                    TBDocumentistResult = new { status = "1", message = "Record not found", smscount = dt1.Rows[0]["SMSCount"].ToString(), DocumentLsitResult = DocumentListResult };
                }
            }
            catch
            {
                TBDocumentistResult = new { status = "1", message = "failed", smscount = 0 };
            }

            return new { TBDocumentistResult };
        }

        [System.Web.Http.HttpPost]
        public object AddDocument(AddDocument adddoc)
        {
            dynamic TBAddDocumentResult;
            int str = -1;
            try
            {
                AddDocResult Result = DocumentSafe.createDocument(adddoc);

                if (!string.IsNullOrEmpty(Result.ImgName))
                {
                    str = GlobalFuns.uploadDocs(adddoc.grpID, Result.ImgName, "documentsafe");
                }
                else
                    str = 0;

                if (Result != null)
                {
                    if (str == 0)
                    {
                        TBAddDocumentResult = new { status = "0", message = "success" };
                        
                        //== Commented by Nandu on 01-03-2017 Task-> Cron set on server(notifiy on publish date)

                        //string url = ConfigurationManager.AppSettings["imgPath"] + "php/AddDocument.php?DocID=" + Result.docID;
                        //GroupMaster.Send(url);
                    }
                    else
                        TBAddDocumentResult = new { status = "1", message = "failed" };
                }
                else
                {
                    TBAddDocumentResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAddDocumentResult = new { status = "1", message = "failed" };
            }

            return new { TBAddDocumentResult };
        }

        [System.Web.Http.HttpPost]
        public object UpdateDocumentIsRead(UpdateReadFlag doc)
        {
            dynamic TBDocumentUpdateResult;
            try
            {
                var Result = DocumentSafe.UpdateDocumentDetails(doc);

                if (Result > 0)
                {
                    TBDocumentUpdateResult = new { status = "0", message = "success"};
                }
                else
                {
                    TBDocumentUpdateResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBDocumentUpdateResult = new { status = "1", message = "failed" };
            }

            return new { TBDocumentUpdateResult };
        }
    }
}
