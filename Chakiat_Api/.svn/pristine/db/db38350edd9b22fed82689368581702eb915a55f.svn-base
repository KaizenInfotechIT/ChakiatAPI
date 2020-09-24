using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TouchBaseWebAPI.Models;
using TouchBaseWebAPI.BusinessEntities;

namespace TouchBaseWebAPI.Controllers
{
    public class TicketingController : ApiController
    {
        [System.Web.Http.HttpPost]
        public object AddTicket(SendTicket addtkt)
        {
            dynamic TBAddTicketResult;
            try
            {
                int Result = Ticket.createTicket(addtkt);

                if (Result > 0)
                {
                    TBAddTicketResult = new { status = "0", message = "success" };
                }
                else
                {
                    TBAddTicketResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBAddTicketResult = new { status = "1", message = "failed" };
            }

            return new { TBAddTicketResult };
        }

        [System.Web.Http.HttpPost]
        public object GetTicketList(GetTicket gettkt)
        {
            dynamic TBGetTicketResult;
            dynamic GetTicketResults;

            List<object> GetTicketResult = new List<object>();
            int pagesize = 3, pageno = 1, total;

            int skippageno = pageno - 1;

            try
            {
                List<GetTicketList> Result = Ticket.getTicket(gettkt);

                for (int i = 0; i < Result.Count; i++)
                {
                    GetTicketResult.Add(new { TicketResult = Result[i] });
                }

                if (Result.Count > 0)
                {
                    var totalPages = 1;
                    if (string.IsNullOrEmpty(gettkt.pageNo))
                    {
                        total = Result.Count;
                        GetTicketResults = GetTicketResult.ToList();
                    }
                    else
                    {
                        total = Result.Count;
                        totalPages = (int)Math.Ceiling((double)total / pagesize);

                        GetTicketResults = GetTicketResult.Skip(0).Take(pagesize * Convert.ToInt32(gettkt.pageNo)).ToList();
                    }

                    TBGetTicketResult = new { status = "0", message = "success", resultCount = total.ToString(), TotalPages = totalPages.ToString(), currentPage = pageno.ToString(), GetTicketResults };
                }
                else
                {
                    TBGetTicketResult = new { status = "0", message = "Record not found" };
                }
            }
            catch
            {
                TBGetTicketResult = new { status = "1", message = "failed" };
            }

            return new { TBGetTicketResult };
        }
    }
}
