using KMDRecociliationApp.Data.Helpers;
using KMDRecociliationApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SqlHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Repositories
{
    public class BulkReconcilationRepository
    {

        ApplicationDbContext context = null;
        private readonly ILogger _logger;

        public BulkReconcilationRepository(ILoggerFactory logger
            , ApplicationDbContext appContext, IConfiguration configuration)
        {
            context = appContext;
            _logger = logger.CreateLogger("BulkReconcilationRepository");

        }

        public async Task<List<string>> BulkReconcilationCheque(DataSet ds, int loggedinUserId,int campaignId)
        {
            List<string> message = new List<string>();

            // Remove blank rows and validate required fields
            DataTable chequeDataTable = ds.Tables[0].DropBlankRows();
            message = validateRequiredFields(chequeDataTable);

            if (message.Count > 0)
                return message;

            // Remove the first row if needed
            chequeDataTable = RemoveFirstRow(ds.Tables[0]);

            foreach (DataRow row in chequeDataTable.Rows)
            {

                var obj = await (from a in context.Applicationuser.AsNoTracking()
                                 join p in context.PolicyHeader.AsNoTracking() on a.Id equals p.Id
                                 join p1 in context.PaymentDetails.AsNoTracking() on p.Id equals p1.PolicyId
                                 join c in context.PaymentModeCheque.AsNoTracking() on p1.Id equals c.PaymentDetailId
                                 where c.ChequeNumber == row["ChequeNumber"].ToString() &&
                                   c.Amount == Convert.ToDecimal(row["Amount"].ToString()) &&
                                   a.MobileNumber == row["Mobile Number"].ToString() &&
                                   p.PaymentStatus == Domain.Enum.PaymentStatus.Initiated
                                   &&p.CampaignId== campaignId
                                 select new
                                 {
                                     policy = p,
                                     Payment = p1,
                                     cheque = c
                                 }
                        ).FirstOrDefaultAsync();

                if (obj != null && obj.policy != null && obj.Payment != null && obj.cheque != null)
                {

                    var amount= obj.cheque.Amount;
                    var policy = context.PolicyHeader.AsNoTracking()
                        .Where(x => x.Id == obj.policy.Id).FirstOrDefault();
                    if (policy != null)
                    {
                        policy.UpdatedAt = DateTime.Now;
                        policy.UpdatedBy = loggedinUserId;
                        policy.PaymentStatus = Domain.Enum.PaymentStatus.Completed;
                        policy.TotalPaidPremimum = amount;
                        policy.Comment = "Bulk Reconcilation Cheque ";

                        context.PolicyHeader.Add(policy);
                        await context.SaveChangesAsync();

                    }
                    var payment = context.PaymentDetails.AsNoTracking()
                        .Where(x => x.Id == obj.Payment.Id).FirstOrDefault();
                    if (payment != null)
                    {
                        payment.UpdatedAt = DateTime.Now;
                        payment.UpdatedBy = loggedinUserId;
                        payment.PaymentStatus = Domain.Enum.PaymentStatus.Completed;
                        payment.PaymentAcceptedDate = DateTime.Now;
                        payment.Comment = "Bulk Reconcilation Cheque " ;
                        payment.AmountPaid=amount;
                        payment.IsAccepted = true;

                        context.PaymentDetails.Add(payment);
                        await context.SaveChangesAsync();

                    }
                }

            }


            // Optionally, add a success message
            message.Add("The Application Users have been successfully created.");

            return message;
        }

        public async Task<List<string>> BulkReconcilationNEFT(DataSet ds, int loggedinUserId, int campaignId)
        {
            List<string> message = new List<string>();

            // Remove blank rows and validate required fields
            DataTable neftDataTable = ds.Tables[0].DropBlankRows();
            message = validateRequiredFields(neftDataTable);

            if (message.Count > 0)
                return message;

            // Remove the first row if needed
            neftDataTable = RemoveFirstRow(ds.Tables[0]);

            foreach (DataRow row in neftDataTable.Rows)
            {

                var obj = await (from a in context.Applicationuser.AsNoTracking()
                                 join p in context.PolicyHeader.AsNoTracking() on a.Id equals p.Id
                                 join p1 in context.PaymentDetails.AsNoTracking() on p.Id equals p1.PolicyId
                                 join c in context.PaymentModeNEFT.AsNoTracking() on p1.Id equals c.PaymentDetailId
                                 where c.TransactionId == row["Transaction Number"].ToString() &&
                                   c.Amount == Convert.ToDecimal(row["Amount"].ToString()) &&
                                   a.MobileNumber == row["Mobile Number"].ToString() &&
                                   p.PaymentStatus==Domain.Enum.PaymentStatus.Initiated
                                   && p.CampaignId==campaignId
                                 select new
                                 {
                                     policy = p,
                                     Payment = p1,
                                     cheque = c
                                 }
                        ).FirstOrDefaultAsync();

                if (obj != null && obj.policy != null && obj.Payment != null && obj.cheque != null)
                {

                    var amount = obj.cheque.Amount;
                    var policy = context.PolicyHeader.AsNoTracking()
                        .Where(x => x.Id == obj.policy.Id).FirstOrDefault();
                    if (policy != null)
                    {
                        policy.UpdatedAt = DateTime.Now;
                        policy.UpdatedBy = loggedinUserId;
                        policy.PaymentStatus = Domain.Enum.PaymentStatus.Completed;
                        policy.TotalPaidPremimum = amount;
                        policy.Comment = "Bulk Reconcilation NEFT";
                        context.PolicyHeader.Add(policy);
                        await context.SaveChangesAsync();

                    }
                    var payment = context.PaymentDetails.AsNoTracking()
                        .Where(x => x.Id == obj.Payment.Id).FirstOrDefault();
                    if (payment != null)
                    {
                        payment.UpdatedAt = DateTime.Now;
                        payment.UpdatedBy = loggedinUserId;
                        payment.PaymentStatus = Domain.Enum.PaymentStatus.Completed;
                        payment.PaymentAcceptedDate = DateTime.Now;
                        payment.Comment = "Bulk Reconcilation NEFT ";
                        payment.AmountPaid = amount;
                        payment.IsAccepted = true;

                        context.PaymentDetails.Add(payment);
                        await context.SaveChangesAsync();

                    }
                }

            }


            // Optionally, add a success message
            message.Add("The Application Users have been successfully created.");

            return message;
        }
        private DataTable RemoveFirstRow(DataTable dataTable)
        {
            /* var cols = dataTable.Columns.Cast<DataColumn>().ToArray()*/
            DataTable dest = dataTable.Clone();
            //DataTable dest = DropBlankRows( dataTable);
            foreach (DataColumn dataColumn in dest.Columns)
            {
                dataColumn.DataType = typeof(string);
            }
            //dest = DropBlankRows(dest);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {

                if (i != 0)
                {
                    DataRow drNew = dest.NewRow();
                    drNew.ItemArray = dataTable.Rows[i].ItemArray;
                    dest.Rows.Add(drNew);
                }
            }
            return dest;
        }
        private List<string> validateRequiredFields(DataTable dt)
        {
            List<string> message = new List<string>();

            for (int j = 0; j < dt.Columns.Count; j++)
            {
                List<string> s = new List<string>();
                string Col = string.Empty;
                if (dt.Rows.Count > 1)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Col = dt.Rows[0][j].ToString();
                        s.Add(dt.Rows[i][j].ToString());
                    }
                    if (Col.Trim() == "Mandatory")
                    {
                        var finlst = s.Select(p => p.ToString()).Where(x => string.IsNullOrWhiteSpace(x)).ToList();

                        if (finlst.Count > 0)
                            message.Add($" {dt.Columns[j].ColumnName} is Required.");
                    }

                }
                else
                {
                    message.Add($" {dt.Columns[j].ColumnName} is Required.");
                }
            }
            return message;
        }
    }
}
