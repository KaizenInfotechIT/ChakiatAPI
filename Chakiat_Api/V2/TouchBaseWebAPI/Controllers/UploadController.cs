using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Http;
using TouchBaseWebAPI.BusinessEntities;

namespace TouchBaseWebAPI.Controllers
{
    public class UploadController : ApiController
    {
        [System.Web.Http.HttpPost]
        public object UploadImage(string module)
        {
            dynamic LoadImageResult;
            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    bool flag = false;
                    string FileName = "ROW_" + DateTime.Now.ToString("ddMMyyyyhhmmsstt") + ".png";

                    //string Path = HttpContext.Current.Server.MapPath("~/TempDocuments/");
                    //string filePath = HttpContext.Current.Server.MapPath("~/TempDocuments/" + FileName);

                    string Path = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments";
                    string filePath = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments\\" + FileName;

                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];

                        if (!Directory.Exists(Path))
                            Directory.CreateDirectory(Path);

                        postedFile.SaveAs(filePath);
                        flag = true;
                    }
                    if (flag)
                    {
                        var Result = MemberMaster.UploadFile(FileName, module, ".png");
                        LoadImageResult = new { status = "0", message = "success", ImageID = Result.ToString() };
                    }
                    else
                    {
                        LoadImageResult = new { status = "1", message = "failed" };
                    }
                }
                else
                {
                    LoadImageResult = new { status = "1", message = "failed" };
                }
            }
            catch
            {
                LoadImageResult = new { status = "1", message = "failed" };
            }
            return new
            {
                LoadImageResult
            };
        }

        [System.Web.Http.HttpPost]
        public object UploadAllDocs(string module)
        {
            dynamic LoadImageResult;
            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    bool flag = false;
                    string final = "";
                    string filetype = "";

                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];

                        string docs = postedFile.FileName.ToString();
                        //docs = postedFile.FileName.Replace(" ", "_").ToString();
                        //docs = docs.Substring(0, docs.LastIndexOf("."));

                        docs = "ROW_" + DateTime.Now.ToString("ddMMyyyyhhmmsstt");
                        final = docs + postedFile.FileName.Substring(postedFile.FileName.LastIndexOf("."));

                        filetype = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf("."));

                        //string Path = HttpContext.Current.Server.MapPath("~/TempDocuments/");
                        //string filePath = HttpContext.Current.Server.MapPath("~/TempDocuments/" + final);

                        string Path = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments";
                        string filePath = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments\\" + final;

                        if (!Directory.Exists(Path))
                            Directory.CreateDirectory(Path);

                        postedFile.SaveAs(filePath);

                        flag = true;
                    }
                    if (flag)
                    {
                        //var Result = 0;
                        var Result = MemberMaster.UploadFile(final, module, filetype);
                        LoadImageResult = new { status = "0", message = "success", ImageID = Result.ToString() };
                    }
                    else
                    {
                        LoadImageResult = new { status = "1", message = "failed" };
                    }
                }
                else
                {
                    LoadImageResult = new { status = "1", message = "failed" };
                }
            }
            catch
            {
                LoadImageResult = new { status = "1", message = "failed" };
            }
            return new
            {
                LoadImageResult
            };
        }
    }
}
