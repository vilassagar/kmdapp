using KMDRecociliationApp.Data.Helpers;
using KMDRecociliationApp.Domain.Entities;
using Microsoft.Extensions.Logging;
using SqlHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using KMDRecociliationApp.Domain.ReportParamModels;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using KMDRecociliationApp.Domain.DTO.InsurerData;
namespace KMDRecociliationApp.Data.Repositories
{
    public class ReportsRepository : MainHeaderRepo<Reports>
    {
        private readonly IConfiguration _configuration;
        private IMsSqlHelper msSqlHelper;
        ApplicationDbContext context = null;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;


        public ReportsRepository(ILoggerFactory logger, ApplicationDbContext appContext
            , IConfiguration configuration) : base(appContext)
        {
            context = appContext;
            _logger = logger.CreateLogger("AssociationRepository");
            _configuration = configuration;
            msSqlHelper = new MsSqlHelper(_configuration.GetConnectionString("constr"));
        }
        public DataTable GetInsuranceCompanyReport(InsuranceCompanyReportModel insuranceCompanyReport)
        {

            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(insuranceCompanyReport.AssociationId.CreateSqlParameter("@AssociationID"));
            parameters.Add(insuranceCompanyReport.OrganisationId.CreateSqlParameter("@OrganisationId"));
            parameters.Add(insuranceCompanyReport.StartDate.CreateSqlParameter("@StartDate"));
            parameters.Add(insuranceCompanyReport.EndDate.CreateSqlParameter("@EndDate"));
            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                 DBConstant.USPINSURANCECOMPANYREPORT, parameters);


            if (result != null && result.Tables.Count > 0)
            { return result.Tables[0]; }
            else
                return new DataTable();
        }

        public DataTable GetDownloadAcknowledgement(int policyId)
        {

            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(policyId.CreateSqlParameter("@PolicyId"));
           
            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                 DBConstant.GETPENSIONERRECEIPTDETAILS, parameters);


            if (result != null && result.Tables.Count > 0)
            { return result.Tables[0]; }
            else
                return new DataTable();
        }

        public DataTable GetAssociationWisePaymentDetails(AssociationWisePaymentDetailsReportParamModel associationWisePaymentDetailsReportParamModel)
        {
            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(associationWisePaymentDetailsReportParamModel.AssociationId.CreateSqlParameter("@AssociationID"));
            parameters.Add(associationWisePaymentDetailsReportParamModel.StartDate.CreateSqlParameter("@StartDate"));
            parameters.Add(associationWisePaymentDetailsReportParamModel.EndDate.CreateSqlParameter("@EndDate"));
            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                 DBConstant.USPINSURANCECOMPANYREPORT, parameters);


            if (result != null && result.Tables.Count > 0)
            { return result.Tables[0]; }
            else
                return new DataTable();
        }

        public DataTable GetCompletedForms(CompletedFormsParamModel completedFormsParamModel)
        {
            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(completedFormsParamModel.AssociationId.CreateSqlParameter("@AssociationID"));
            //parameters.Add(completedFormsParamModel.ReportDate.CreateSqlParameter("@ReportDate"));
            //parameters.Add(completedFormsParamModel.StartDate.CreateSqlParameter("@StartDate"));
            //parameters.Add(completedFormsParamModel.EndDate.CreateSqlParameter("@EndDate"));
            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                 DBConstant.USPCOMPLETEDFORMS, parameters);


            if (result != null && result.Tables.Count > 0)
            { return result.Tables[0]; }
            else
                return new DataTable();
        }

        public DataTable GetIncompleteTransactions(IncompleteTransactionParamModel incompleteTransactionParamModel)
        {
            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(incompleteTransactionParamModel.AssociationId.CreateSqlParameter("@AssociationID"));
            parameters.Add(incompleteTransactionParamModel.ReportDate.CreateSqlParameter("@ReportDate"));
            //parameters.Add(incompleteTransactionParamModel.StartDate.CreateSqlParameter("@StartDate"));
            //parameters.Add(incompleteTransactionParamModel.EndDate.CreateSqlParameter("@EndDate"));
            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                 DBConstant.USPINCOMPLETETRANSACTIONS, parameters);


            if (result != null && result.Tables.Count > 0)
            { return result.Tables[0]; }
            else
                return new DataTable();
        }

        public DataTable GetBouncedPayments(GetBouncedPaymentsReportParamModel getBouncedPaymentsReportParamModel)
        {
            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(getBouncedPaymentsReportParamModel.AssociationId.CreateSqlParameter("@AssociationID"));
            parameters.Add(getBouncedPaymentsReportParamModel.StartDate.CreateSqlParameter("@StartDate"));
            parameters.Add(getBouncedPaymentsReportParamModel.EndDate.CreateSqlParameter("@EndDate"));
            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                 DBConstant.USPBOUNCEDPAYMENTS, parameters);


            if (result != null && result.Tables.Count > 0)
            { return result.Tables[0]; }
            else
                return new DataTable();
        }

        public DataTable GetReconcileedOnlinePayments(GetReconciledOnlinePaymentsParamModel getReconciledOnlinePaymentsParamModel)
        {
            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(getReconciledOnlinePaymentsParamModel.AssociationId.CreateSqlParameter("@AssociationID"));
            parameters.Add(getReconciledOnlinePaymentsParamModel.StartDate.CreateSqlParameter("@StartDate"));
            parameters.Add(getReconciledOnlinePaymentsParamModel.EndDate.CreateSqlParameter("@EndDate"));
            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                 DBConstant.USPRECONCILEEDONLINEPAYMENTS, parameters);


            if (result != null && result.Tables.Count > 0)
            { return result.Tables[0]; }
            else
                return new DataTable();
        }

        public DataTable GetOfflinePayments(GetOfflinePaymentsParamModel getOfflinePaymentsParamModel)
        {
            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(getOfflinePaymentsParamModel.AssociationId.CreateSqlParameter("@AssociationID"));
            parameters.Add(getOfflinePaymentsParamModel.StartDate.CreateSqlParameter("@StartDate"));
            parameters.Add(getOfflinePaymentsParamModel.EndDate.CreateSqlParameter("@EndDate"));
            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                 DBConstant.USPOFFLINEPAYMENTS, parameters);


            if (result != null && result.Tables.Count > 0)
            { return result.Tables[0]; }
            else
                return new DataTable();
        }

        public DataTable GetCorrectionReport(GetCorrectionReportParamModel getCorrectionReportParamModel)
        {
            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(getCorrectionReportParamModel.AssociationId.CreateSqlParameter("@AssociationID"));
            parameters.Add(getCorrectionReportParamModel.StartDate.CreateSqlParameter("@StartDate"));
            parameters.Add(getCorrectionReportParamModel.EndDate.CreateSqlParameter("@EndDate"));
            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                 DBConstant.USPCORRECTIONREPORT, parameters);


            if (result != null && result.Tables.Count > 0)
            { return result.Tables[0]; }
            else
                return new DataTable();
        }

        public DataTable GetDailyCountAssociationWise(GetDailyCountAssociationWiseReportParamModel getDailyCountAssociationWiseReportParamModel)
        {
            IList<SqlParameter> parameters = new List<SqlParameter>();
            //parameters.Add(reportDate.CreateSqlParameter("@reportDate"));
            parameters.Add(getDailyCountAssociationWiseReportParamModel.StartDate.CreateSqlParameter("@StartDate"));
            parameters.Add(getDailyCountAssociationWiseReportParamModel.EndDate.CreateSqlParameter("@EndDate"));
            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                 DBConstant.USPDAILYCOUNTASSOCIATIONWISE, parameters);


            if (result != null && result.Tables.Count > 0)
            { return result.Tables[0]; }
            else
                return new DataTable();
        }
        public DataTable GetExtractPensionerPaymentDetails(
            int paymentType,int paymentStatus, int campaignId,int associationId
            )
        {
            IList<SqlParameter> parameters = new List<SqlParameter>();
            if (paymentType > 0)
            {
                parameters.Add(paymentType.CreateSqlParameter("@paymentType"));
            }
            else
            {
                int? paymentTypen1 = null;
                parameters.Add(paymentTypen1.CreateSqlParameter("@paymentType"));
            }
            if (campaignId > 0)
            {
                parameters.Add(campaignId.CreateSqlParameter("@campaignId"));
            }
            else
            {
                int? campaignId1 = null;
                parameters.Add(campaignId1.CreateSqlParameter("@campaignId"));
            }
            if (associationId > 0)
            {
                parameters.Add(associationId.CreateSqlParameter("@associationId"));
            }
            else
            {
                int? associationId1 = null;
                parameters.Add(associationId1.CreateSqlParameter("@associationId"));
            }
            if (paymentStatus != 99)
            {
                parameters.Add(paymentStatus.CreateSqlParameter("@paymentStatus"));
            }
            else
            {
                int? paymentStatus1 = null;
                parameters.Add(paymentStatus1.CreateSqlParameter("@paymentStatus"));
            }
            
            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                 DBConstant.GETEXTRACTPENSIONERPAYMENTDETAILS, parameters);


            if (result != null && result.Tables.Count > 0)
            { return result.Tables[0]; }
            else
                return new DataTable();
        }
        private int getAssociationID(int id)
        {
            var userdetails = context.Applicationuser.AsNoTracking()
                        .Where(x => x.Id == id).FirstOrDefault();
            if (userdetails != null
                && userdetails.AssociationId != null)
                return userdetails.AssociationId.Value;
            else
                return 0;
        }
        public DataTable GetAssociationExtract(int id
        //DateTime reportDate,DateTime EndDate
        )
        {
            var Associationid = getAssociationID(id);
            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(Associationid.CreateSqlParameter("@id"));
            // parameters.Add(reportDate.CreateSqlParameter("@startdate"));
            //parameters.Add(reportDate.CreateSqlParameter("@enddate"));
            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                 DBConstant.GetAssociationExtractPaymentDetails, parameters);


            if (result != null && result.Tables.Count > 0)
            { return result.Tables[0]; }
            else
                return new DataTable();
        }

        public DataTable GetRefundReports(GetRefundReportsParamModel getRefundReportsParamModel)
        {
            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(getRefundReportsParamModel.AssociationId.CreateSqlParameter("@AssociationID"));
            parameters.Add(getRefundReportsParamModel.OrganisationId.CreateSqlParameter("@OrganisationId"));
            parameters.Add(getRefundReportsParamModel.StartDate.CreateSqlParameter("@StartDate"));
            parameters.Add(getRefundReportsParamModel.EndDate.CreateSqlParameter("@EndDate"));
            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                 DBConstant.USPREFUNDREPORTS, parameters);


            if (result != null && result.Tables.Count > 0)
            { return result.Tables[0]; }
            else
                return new DataTable();
        }
        public DataTable GetApplicantData(ApplicantFilterDto filterDto)
        {
            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(filterDto.Search.CreateSqlParameter("@Search"));
              var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                 DBConstant.GETAPPLICANTDATA, parameters);


            if (result != null && result.Tables.Count > 0)
            { return result.Tables[0]; }
            else
                return new DataTable();
        }
    }
}