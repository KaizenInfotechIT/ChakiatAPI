using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchBaseWebAPI.Models;
using MySql.Data.MySqlClient;

namespace TouchBaseWebAPI.BusinessEntities
{
    public class Ticket
    {
        //private static TouchBaseWebAPI.Data.row_productionEntities _DBTouchbase = new TouchBaseWebAPI.Data.row_productionEntities();

        public static int createTicket(SendTicket tkt)
        {
            try
            {
                var grpID = new MySqlParameter("?groupId", tkt.groupId);
                var profileId = new MySqlParameter("?memberProfileId", tkt.memberProfileId);
                var smsText = new MySqlParameter("?smsText", tkt.smsText);
                var createBy = new MySqlParameter("?createBy", tkt.createBy);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreCommand("CALL V2_USPAddTicket(?groupId,?memberProfileId,?smsText,?createBy)",
                                                                             grpID, profileId, smsText, createBy);
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

        public static List<GetTicketList> getTicket(GetTicket tkt)
        {
            try
            {
                var grpID = new MySqlParameter("?groupId", tkt.groupId);
                var memberProfileId = new MySqlParameter("?memberProfileId", tkt.memberProfileId);

                using (TouchBaseWebAPI.Data.row_productionEntities context = new TouchBaseWebAPI.Data.row_productionEntities())
                {
                    context.Connection.Open();
                    var Result = context.ExecuteStoreQuery<GetTicketList>("CALL V2_USPGetTicket(?groupId,?memberProfileId)", grpID, memberProfileId).ToList();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

    }
}