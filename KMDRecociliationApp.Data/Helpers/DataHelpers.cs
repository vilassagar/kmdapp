using KMDRecociliationApp.Domain.Common;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Helpers
{
    public class DataHelpers
    {
        public const string ASSOCIATIONMANDATEFILE = "AssociationMandateFiles";
        public const string ASSOCIATIONQRCODEFILE = "AssociationQRCodeFiles";
        public const string PRODUCTDOCUMENT = "ProductDocumentFiles";
        public const string CAMPAIGNTEMPLATE = "CampaignTemplate";
        public const string DISABILITYCERTIFICATE = "DisabilityCertificate";
        public const string CHEQUEPHOTO = "ChequePhoto";
        public const string REFUNDCHEQUEPHOTO = "RefundChequePhoto";

        public const string NEFTPAYMENTRECEIPT = "neftPaymentReceipt";
        public const string UPIPAYMENTRECEIPT = "upiPaymentReceipt";
        public const string REFUNDNEFTPAYMENTRECEIPT = "RefundneftPaymentReceipt";
        public const string REFUNDUPIPAYMENTRECEIPT = "RefundupiPaymentReceipt";

        public async Task<CommonFileModel> UploadFile(IFormFile file, string context, int Id)
        {
            CommonFileModel fileDetail = new CommonFileModel();
            try
            {
                // Ensure the upload directory exists
                string[] arr = { "Uploads", context, Id.ToString() };

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", context, Id.ToString());

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var filePath = Path.Combine(uploadPath, file.FileName);
                if (File.Exists(filePath))
                    File.Delete(filePath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                   await file.CopyToAsync(stream);
                }

                // Save file details to the object

                fileDetail.Name = file.FileName;
                fileDetail.Url = filePath;

            }
            catch (Exception ex)
            {
                Log.Error("error", ex.Message);
            }
            return fileDetail;
        }

    }
}
