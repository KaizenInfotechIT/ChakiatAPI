using System.Collections.Generic;
using System.Web.Http;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;
using System.Configuration;
using System.Data;

namespace TouchBaseWebAPI.Controllers
{
    public class SurveyController : ApiController
    {
        [HttpPost]
        public object AddEditSevey(Cls_Input Input)
        {
            dynamic AddSurvey;
            List<object> AddSurveyResult = new List<object>();

            try
            {
                int Result = servey.AddEditSevey(Input);

                if (Result > 0)
                {
                    AddSurvey = new { status = "0", message = "success" };
                }
                else
                {
                    AddSurvey = new { status = "1", message = "failed" };
                }
            }
            catch
            {
                AddSurvey = new { status = "1", message = "failed" };
            }

            return new { AddSurvey };
        }

        [System.Web.Http.HttpPost]
        public object Survey_List(Cls_Input Input)
        {
            dynamic TBSurveyListResult;
            List<object> data = new List<object>();

            try
            {
                List<Cls_Input> Result = servey.getSurvey_List(Input);

                //for (int i = 0; i < Result.Count; i++)
                //{
                //    data.Add(new { data = Result[i] });
                //}

                if (Result != null)
                {
                    TBSurveyListResult = new { status = "0", message = "success", Result };
                }
                else
                {
                    TBSurveyListResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBSurveyListResult = new { status = "1", message = "failed" };
            }

            return new { TBSurveyListResult };
        }

        [System.Web.Http.HttpPost]
        public object SurveyDetails(Cls_Input Input)
        {
            dynamic surveysteps;
            // List<object> data = new List<object>();

            try
            {
                List<Cls_Input> data = servey.SurveyDetails(Input);

                //for (int i = 0; i < Result.Count; i++)
                //{
                //    data.Add(new { data = Result[i] });
                //}

                if (data != null)
                {
                    surveysteps = new { status = "0", message = "success", data };
                }
                else
                {
                    surveysteps = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                surveysteps = new { status = "1", message = "failed" };
            }

            return new { surveysteps };
        }

        [HttpPost]
        public object DeleteSevey(Cls_Input Input)
        {
            dynamic DeleteSurvey;
            List<object> AddSurveyResult = new List<object>();

            try
            {
                int Result = servey.DeleteSevey(Input);

                if (Result > 0)
                {
                    DeleteSurvey = new { status = "0", message = "success" };
                }
                else
                {
                    DeleteSurvey = new { status = "1", message = "failed" };
                }
            }
            catch
            {
                DeleteSurvey = new { status = "1", message = "failed" };
            }

            return new { DeleteSurvey };
        }
    }
}
