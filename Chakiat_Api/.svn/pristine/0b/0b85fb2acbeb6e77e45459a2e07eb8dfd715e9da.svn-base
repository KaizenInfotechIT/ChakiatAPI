using System.Web.Http;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;
using System.Configuration;
using System.Web;
using System;
using System.IO;
using System.Collections.Generic;

namespace TouchBaseWebAPI.Controllers
{
    public class InterviewTestController : ApiController
    {
        [HttpPost]
        public object EmpAddEdit(Emp_Inputs Emp)
        {
            dynamic EmpAddEditResult;
            
            int flag = 0;

            try
            {
                flag = InterviewTest.EmpAddEdit(Emp);

                if (flag > 0)
                {
                    EmpAddEditResult = new { status = "0", message = "success" };
                }
                else
                {
                    EmpAddEditResult = new { status = "1", message = "failed" };
                }
            }
            catch
            {
                EmpAddEditResult = new { status = "1", message = "failed" };
            }

            return new { EmpAddEditResult };
        }

        [HttpPost]
        public object EmpDelete(Emp_Inputs Emp)
        {
            dynamic EmpDeleteResult;

            int flag = 0;

            try
            {
                flag = InterviewTest.EmpDelete(Emp);

                if (flag > 0)
                {
                    EmpDeleteResult = new { status = "0", message = "success" };
                }
                else
                {
                    EmpDeleteResult = new { status = "1", message = "failed" };
                }
            }
            catch
            {
                EmpDeleteResult = new { status = "1", message = "failed" };
            }

            return new { EmpDeleteResult };
        }

        [HttpPost]
        public object EmpSearch(Emp_Inputs Emp)
        {
            dynamic TBEmpListResult;
            List<object> EMPListResult = new List<object>();

            try
            {
                List<Emp_Inputs> Result = InterviewTest.EmpSearch(Emp);

                //for (int i = 0; i < Result.Count; i++)
                //{
                //    EMPListResult.Add(new { EMPResult = Result[i] });
                //}

                if (Result != null)
                {
                    TBEmpListResult = new { status = "0", message = "success", Result };
                }
                else
                {
                    TBEmpListResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBEmpListResult = new { status = "1", message = "failed" };
            }

            return new { TBEmpListResult };
        }
    }
}
