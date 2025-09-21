using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class AcknowledgementReceipt
    {
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string EMPIDPFNo { get; set; }
        public string AssociationName { get; set; }
        public string SpouseName { get; set; }
        public DateTime? SpouseDateOfBirth { get; set; }
        public string Child1Name { get; set; }
        public DateTime? Child1DateOfBirth { get; set; }
        public string Child2Name { get; set; }
        public DateTime? Child2DateOfBirth { get; set; }
        public DateTime? TransactionDate { get; set; }

        public bool DigitPaymentProtectionPurchased { get; set; }
        public decimal D_SumInsured { get; set; }
        public decimal D_PremiumRemitted { get; set; }
        public string D_TransactionId { get; set; }
        public DateTime? D_DateOfPayment { get; set; }

        public bool OutPatientDepartmentPolicyPurchased { get; set; }
        public decimal OPD_SumInsured { get; set; }
        public decimal OPD_PremiumRemitted { get; set; }
        public string OPD_TransactionId { get; set; }
        public DateTime? OPD_DateOfPayment { get; set; }
        public int? Spouse { get; set; }
        public int? Child1 { get; set; }
        public int? Child2 { get; set; }

    }

    // Define a class to represent a row in the table
    public class FamilyMember
    {
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string Relation { get; set; }

        public FamilyMember(string name, string dateOfBirth, string relation)
        {
            Name = name;
            DateOfBirth = dateOfBirth;
            Relation = relation;
        }
    }
}
