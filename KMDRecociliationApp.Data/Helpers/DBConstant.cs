using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Helpers
{
    public class DBConstant
    {
        public const string USPBULKUPLOADAPPLICATIONUSERS = "[dbo].[USPBulkUploadApplicationUsers]";
        public const string USPBULKUPLOADASSOCIATION = "[dbo].[USPBulkUploadAssociation]";
        public const string USPBULKUPLOADPRODUCT = "[dbo].[USPBulkUploadProduct]";
        public const string USPBULKUPLOADBASEPRODUCT = "";

        public const string USPINSURANCECOMPANYREPORT = "[dbo].[USPInsuranceCompanyReport]";
        public const string USPASSOCIATIONWISEPAYMENTDETAILS = "[dbo].[USPGetAccountWisePaymentReport]";
        public const string USPCOMPLETEDFORMS = "[dbo].[USPGetCompletedFormsReport]";
        public const string USPINCOMPLETETRANSACTIONS = "[dbo].[USPGetIncompleteTransactionReport]";
        public const string USPBOUNCEDPAYMENTS = "[dbo].[USPGetBouncedPaymentsReport]";
        public const string USPRECONCILEEDONLINEPAYMENTS = "[dbo].[USPGetReconciledOnlinePaymentsReport]";
        public const string USPOFFLINEPAYMENTS = "[dbo].[USPOfflinePaymentsReport]";
        public const string USPCORRECTIONREPORT = "[dbo].[USPGetCorrectionReport]";
        public const string USPDAILYCOUNTASSOCIATIONWISE = "[dbo].[USPDailyCountAssociationWise]";
        public const string USPREFUNDREPORTS = "[dbo].[USPGetRefundReport]";

        public const string GETALLAPPLICATIONUSERMETADATA = "[dbo].[USPGetAllApplicationUserMetadata]";
        public const string GETALLMETADATALIST = "[dbo].[USPGetAllTemplateAndMetadata]";

        public const string GETALLASSOCIATIONMETADATA = "[dbo].[USPGetAllAssociationMetadata]";

        public const string GETEXTRACTPENSIONERPAYMENTDETAILS = "[dbo].[GetExtractPensionerPaymentDetails]";
        public const string GetAssociationExtractPaymentDetails = "[dbo].[GetAssociationExtractPaymentDetails]";

        public const string USPGETDASHBOARDDATA  = "[dbo].[USPGetDashBoardData]";

        public const string USPGETDASHBOARDDATADETAILS = "[dbo].[USPGetDashBoardDataDetails]";
        public const string GETPENSIONERRECEIPTDETAILS = "[dbo].[GetPensionerReceiptDetails]";
        public const string GETAPPLICANTDATA = "[dbo].[GetApplicantData]";
    }
}
