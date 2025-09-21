using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace KMDRecociliationApp.Data.Repositories
{
    public class DigitReceiptRepository : MainHeaderRepo<PaymentHeader>
    {
        ApplicationDbContext _context;
        public DigitReceiptRepository(ApplicationDbContext context): base(context)
        {
            _context = context;
        }
        public void GeneratePdf()
        {
            string dest = "napster";
            PdfWriter writer = new PdfWriter(dest);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            document.Add(new Paragraph("Hello, PDF!"));
            document.Close();
        }

        public DTOPaymentReceipt GetPaymentReceiptdetails(int policyId)
        {
            DTOPaymentReceipt receipt = new DTOPaymentReceipt();

            var payment = _context.PolicyHeader.AsNoTracking()
                .Where(x => x.Id == policyId).FirstOrDefault();
            if (payment != null)
            {
                receipt.OrderNumber = policyId;
                receipt.TotalPremium = payment.TotalPremimum;
                receipt.AmountPaid = payment.TotalPaidPremimum;
                var user = _context.Applicationuser.AsNoTracking()
                    .Where(x => x.Id == payment.UserId).FirstOrDefault();
                if (user != null)
                {
                    receipt.Name = user.FullName;
                }
                receipt.PaymentReceiptDetails = new List<DTOPaymentReceiptDetails>();
                var paymentdetails = _context.PaymentDetails.AsNoTracking()
                 .Where(x => x.PolicyId == policyId).ToList();
                foreach (var item in paymentdetails)
                {
                    DTOPaymentReceiptDetails dTOPaymentReceipt = new DTOPaymentReceiptDetails();

                    dTOPaymentReceipt.PaymentDate = item.PaymentDate;
                    if (item.PaymentAcceptedDate != null)
                        dTOPaymentReceipt.PaymentAcceptedDate = item.PaymentAcceptedDate;
                    else
                        dTOPaymentReceipt.PaymentAcceptedDate = item.PaymentDate;
                    dTOPaymentReceipt.PaymentMode = item.PaymentType.ToString();
                    dTOPaymentReceipt.AmountPaid = item.AmountPaid;
                    receipt.PaymentReceiptDetails.Add(dTOPaymentReceipt);
                }

            }

            return receipt;
        }

    }
}
