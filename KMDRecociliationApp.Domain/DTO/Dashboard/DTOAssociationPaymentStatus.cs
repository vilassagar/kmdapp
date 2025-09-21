using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO.Dashboard
{
    public class DTOAssociationPaymentStatus
    {
        public int? Id { get; set; }
        public string? AssociationName { get; set; }
        public decimal? CompletedPayment { get; set; } = 0;
        public decimal? InitiatedPayment { get; set; } = 0;
        public decimal? FailedPayment { get; set; } = 0;
        public decimal? RejectedPayment { get; set; } = 0;
        public decimal? TotalAmountPaid { get; set; } = 0;
        public PaymentStatus PaymentStatus { get; set; }
        public int AssociationId { get; set; }

    }

    public class DTOOfflinePaymentsStatus
    { 
        public string Status { get; set; }
        public int Count { get; set; }
    }
    public class DTOPaymentsModes
    {
        public string Mode { get; set; }
        public int Count { get; set; }
    }
    public class DTOUserCount
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
    public class DTOCampaignsCount
    {
        public DateTime date { get; set; }
        public int created { get; set; }
        public int executed { get; set; }
    }
}
