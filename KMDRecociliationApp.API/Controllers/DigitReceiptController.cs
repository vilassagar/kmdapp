using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using KMDRecociliationApp.Domain.DTO;
using Microsoft.EntityFrameworkCore;
using KMDRecociliationApp.Domain.Enum;
using KMDRecociliationApp.Domain.Entities;
using DocumentFormat.OpenXml.InkML;
using System.Data;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;


namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class DigitReceiptController : ApiBaseController
    {
        //private readonly TokenService _tokenService;
        private readonly ILogger<UserController> _logger;
        private readonly DigitReceiptRepository _digitReceiptRepository;
        private readonly ApplicationDbContext _context;
        private readonly ReportsRepository _reportsRepository;
        public DigitReceiptController(DigitReceiptRepository digitReceiptRepository,
           ApplicationDbContext context, ILogger<UserController> logger
            , ReportsRepository reportsRepository
            , RoleRepository roleRepository) : base(context)
        {
            // _tokenService = tokenService;
            _logger = logger;
            _digitReceiptRepository = digitReceiptRepository;
            _context = context;
            _reportsRepository = reportsRepository;
            //_kMDAPISecretKey = kmdapikey.Value;
            //_roleRepository = roleRepository;
        }

        [HttpGet("download")]
        public IActionResult DownloadPdf()
        {
            using (var memoryStream = new MemoryStream())
            {
                // Create a new PDF document
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);

                // Draw text on the PDF
                gfx.DrawString("Hello, PDF!", font, XBrushes.Black,
                    new XRect(0, 0, page.Width, page.Height),
                    XStringFormats.Center);
                XImage image = XImage.FromFile("Templates/RECEIPT.jpg");

                double xPosition = 20;
                double yPosition = 20;

                // Draw the image
                gfx.DrawImage(image, xPosition, yPosition, 550, 700);

                XFont font1 = new XFont("Arial", 11, XFontStyle.Regular);
                XBrush brush = XBrushes.Black;

                // Define text position over the image
                double textXPosition_ReceiptNumber = 300;  // 300 points from the left edge
                double textYPosition_ReceiptNumber = 165;  // 223 points from the top edge

                // Draw the text over the image
                gfx.DrawString("Number", font1, brush, new XPoint(textXPosition_ReceiptNumber, textYPosition_ReceiptNumber), XStringFormats.TopLeft);

                // Define text position over the image
                double textXPosition_ReceiptDate = 300;  // 300 points from the left edge
                double textYPosition_ReceiptDate = 185;  // 223 points from the top edge

                // Draw the text over the image
                gfx.DrawString("Date", font1, brush, new XPoint(textXPosition_ReceiptDate, textYPosition_ReceiptDate), XStringFormats.TopLeft);

                // Define text position over the image
                double textXPosition_DepositorCode = 300;  // 300 points from the left edge
                double textYPosition_DepositorCode = 205;  // 223 points from the top edge

                // Draw the text over the image
                gfx.DrawString("Code", font1, brush, new XPoint(textXPosition_DepositorCode, textYPosition_DepositorCode), XStringFormats.TopLeft);


                // Define text position over the image
                double textXPosition_DepositorName = 300;  // 300 points from the left edge
                double textYPosition_DepositorName = 225;  // 223 points from the top edge

                // Draw the text over the image
                gfx.DrawString("Name", font1, brush, new XPoint(textXPosition_DepositorName, textYPosition_DepositorName), XStringFormats.TopLeft);


                // Define text position over the image
                double textXPosition_DepositMode = 35;  // 300 points from the left edge
                double textYPosition_DepositMode = 275;  // 223 points from the top edge

                // Draw the text over the image
                gfx.DrawString("Online", font1, brush, new XPoint(textXPosition_DepositMode, textYPosition_DepositMode), XStringFormats.TopLeft);

                // Define text position over the image
                double textXPosition_InstrumentNumber = 140;  // 300 points from the left edge
                double textYPosition_InstrumentNumber = 275;  // 223 points from the top edge

                // Draw the text over the image
                gfx.DrawString("InstrumentNumber", font1, brush, new XPoint(textXPosition_InstrumentNumber, textYPosition_InstrumentNumber), XStringFormats.TopLeft);

                // Define text position over the image
                double textXPosition_InstrumentDate = 245;  // 300 points from the left edge
                double textYPosition_InstrumentDate = 275;  // 223 points from the top edge

                // Draw the text over the image
                gfx.DrawString("InstrumentDate", font1, brush, new XPoint(textXPosition_InstrumentDate, textYPosition_InstrumentDate), XStringFormats.TopLeft);

                // Define text position over the image
                double textXPosition_BankName = 350;  // 300 points from the left edge
                double textYPosition_BankName = 275;  // 223 points from the top edge

                // Draw the text over the image
                gfx.DrawString("BankName", font1, brush, new XPoint(textXPosition_BankName, textYPosition_BankName), XStringFormats.TopLeft);

                // Define text position over the image
                double textXPosition_Amount = 455;  // 300 points from the left edge
                double textYPosition_Amount = 275;  // 223 points from the top edge

                // Draw the text over the image
                gfx.DrawString("Amount", font1, brush, new XPoint(textXPosition_Amount, textYPosition_Amount), XStringFormats.TopLeft);

                // Save the PDF to the memory stream
                document.Save(memoryStream);

                // Convert MemoryStream to byte array
                byte[] pdfBytes = memoryStream.ToArray();

                // Return as a downloadable file
                return File(pdfBytes, "application/pdf", "GeneratedDocument.pdf");
            }
        }

        [HttpGet("downloadAcknowledgement")]
        public IActionResult DownloadAcknowledgement()
        {
            using (var memoryStream = new MemoryStream())
            {
                // Create a new PDF document
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                //XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);

                //Draw text on the PDF
                //gfx.DrawString("Hello, PDF!", font, XBrushes.Black,
                //    new XRect(0, 0, page.Width, page.Height),
                //    XStringFormats.Center);
                XImage image = XImage.FromFile("Templates/kmdHeader.jpg");

                double xPosition = ((page.Width - (page.Width - 10)) / 2) - .5;
                double yPosition = 5.5;

                // Draw the image
                gfx.DrawImage(image, xPosition, yPosition, page.Width - 10, 140);

                XFont font1 = new XFont("Arial", 10, XFontStyle.Regular);
                XBrush brush = XBrushes.Black;

                // Define text position over the image
                double textXPosition_ReceiptNumber = 496;  // 300 points from the left edge
                double textYPosition_ReceiptNumber = 156;  // 223 points from the top edge

                // Draw the text over the image
                gfx.DrawString("Date: 12-11-2024 ", font1, brush, new XPoint(textXPosition_ReceiptNumber, textYPosition_ReceiptNumber), XStringFormats.TopLeft);

                XFont font2 = new XFont("Arial", 10, XFontStyle.Bold);
                //XBrush brush = XBrushes.Black;

                double textXPosition_To = 35;  // X-coordinate
                double textYPosition_To = 168;  // Initial Y-coordinate
                double lineHeight = 11; // Adjust the line height as needed

                // Define the text to draw, line by line
                string[] lines = new string[]
                {
                    "To,",
                    "VENKATASUBRAMANIAN R",
                    "PF No.: 4801",
                    //"Width"+ page.Width+"",
                    //"Height"+ page.Height+"",
                };

                // Draw each line
                foreach (string line in lines)
                {
                    gfx.DrawString(line, font2, brush, new XPoint(textXPosition_To, textYPosition_To), XStringFormats.TopLeft);
                    textYPosition_To += lineHeight; // Move to the next line
                }
                XFont font3 = new XFont("Arial", 10, XFontStyle.Bold);
                //XBrush brush = XBrushes.Black;

                //double textXPosition_To = 38;  // X-coordinate
                //double textYPosition_To = 168;  // Initial Y-coordinate
                //double lineHeight = 10; // Adjust the line height as needed

                // Define the text to draw, line by line
                string[] _lines = new string[]
                {
                    "",
                    "Dear Sir / Madam,",
                    "",
                    "It gives us immense pleasure to welcome you and your family as a member of the Mediclaim Policy for Retirees for the",
                    "Year 2024-25.",
                    "",
                    "Policy shall be with Go Digit General Insurance Limited (Super Top Up Policy) and shall start from 1** November 2024.",
                    "",
                    "Based on the data and payment, we give below the details of the family members.",
                    ""
                };

                // Draw each line
                foreach (string line in _lines)
                {
                    gfx.DrawString(line, font1, brush, new XPoint(textXPosition_To, textYPosition_To), XStringFormats.TopLeft);
                    textYPosition_To += lineHeight; // Move to the next line
                }

                double tableXPosition = 35;  // Starting X-coordinate of the table
                double tableYPosition = 322; // Starting Y-coordinate of the table
                double rowHeight = 12;       // Height of each row
                double[] columnWidths = { 250, 130, 130 }; // Width of each column

                // Sample data for the table
                string[,] tableData = new string[,]
                {
                    { "Header 1", "Header 2", "Header 3" },
                    { "Row 1, Col 1", "Row 1, Col 2", "Row 1, Col 3" },
                    { "Row 2, Col 1", "Row 2, Col 2", "Row 2, Col 3" }
                };

                // Draw the table
                for (int row = 0; row < tableData.GetLength(0); row++)
                {
                    double currentX = tableXPosition;

                    // Draw each column in the row
                    for (int col = 0; col < tableData.GetLength(1); col++)
                    {
                        // Draw cell border
                        gfx.DrawRectangle(XPens.Black, currentX, tableYPosition, columnWidths[col], rowHeight);

                        // Draw cell content
                        gfx.DrawString(
                            tableData[row, col],
                            font1,
                            brush,
                            new XRect(currentX, tableYPosition, columnWidths[col], rowHeight),
                            XStringFormats.Center);

                        // Move to the next column
                        currentX += columnWidths[col];
                    }

                    // Move to the next row
                    tableYPosition += rowHeight;
                }

                // Save the PDF to the memory stream
                document.Save(memoryStream);

                // Convert MemoryStream to byte array
                byte[] pdfBytes = memoryStream.ToArray();

                // Return as a downloadable file
                return File(pdfBytes, "application/pdf", "GeneratedDocument.pdf");
            }
        }

        [HttpGet("downloadOPDAcknowledgement")]
        public IActionResult DownloadOPDAcknowledgement(int policyId)
        {
            var table = _reportsRepository.GetDownloadAcknowledgement(policyId);
            var _OPDA = new AcknowledgementReceipt();
            foreach (DataRow dataRow in table.Rows)
            {
                _OPDA.AssociationName = dataRow["Association Name"].ToString();
                _OPDA.Name = dataRow["Pensioner Name"].ToString();
                _OPDA.EMPIDPFNo = dataRow["PF No/Emp ID"].ToString();

                if (DateTime.TryParse(dataRow["Date of Birth"].ToString(), out DateTime result))
                    _OPDA.DateOfBirth = result;
                if (DateTime.TryParse(dataRow["Payment Date/Cheque Date"].ToString(), out DateTime result1))
                    _OPDA.TransactionDate = Convert.ToDateTime(result1);
                _OPDA.SpouseName = Convert.ToString(dataRow["Name of Spouse"]);

                if (DateTime.TryParse(dataRow["Spouse Date of Birth"].ToString(), out DateTime Spouseresult1))
                    //_OPDA.TransactionDate = Convert.ToDateTime(result1);
                    _OPDA.SpouseDateOfBirth = Convert.ToDateTime(Spouseresult1);
                _OPDA.Child1Name = Convert.ToString(dataRow["Name of Child1"]);

                if (DateTime.TryParse(dataRow["Child1 Date of Birth"].ToString(), out DateTime Child1result1))
                    _OPDA.Child1DateOfBirth = Convert.ToDateTime(Child1result1);

                _OPDA.Child2Name = Convert.ToString(dataRow["Name of Child2"]);

                if (DateTime.TryParse(dataRow["Child2 Date of Birth"].ToString(), out DateTime Child2result1))
                    _OPDA.Child2DateOfBirth = Convert.ToDateTime(Child2result1);




            }

            using (var memoryStream = new MemoryStream())
            {
                // Create a new PDF document
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                //XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);

                XImage image = XImage.FromFile("Templates/kmdHeader.jpg");

                double xPosition = ((page.Width - (page.Width - 10)) / 2) - .5;
                double yPosition = 5.5;

                // Draw the image
                gfx.DrawImage(image, xPosition, yPosition, page.Width - 10, 140);

                XFont font1 = new XFont("Arial", 10, XFontStyle.Regular);
                XFont font0 = new XFont("Arial", 9, XFontStyle.Regular);
                XBrush brush = XBrushes.Black;

                // Define text position over the image
                double textXPosition_ReceiptNumber = 496;  // 300 points from the left edge
                double textYPosition_ReceiptNumber = 156;  // 223 points from the top edge

                // Draw the text over the image
                gfx.DrawString($"Date: {_OPDA.TransactionDate?.ToString("dd-MM-yyyy") ?? ""}", font1, brush, new XPoint(textXPosition_ReceiptNumber, textYPosition_ReceiptNumber), XStringFormats.TopLeft);

                XFont font2 = new XFont("Arial", 10, XFontStyle.Bold);
                //XBrush brush = XBrushes.Black;

                double textXPosition_To = 35;  // X-coordinate
                double textYPosition_To = 168;  // Initial Y-coordinate
                double lineHeight = 11; // Adjust the line height as needed

                // Define the text to draw, line by line
                List<string> linesList =
                    [
                        "To,",
                        // Adding a new element
                        _OPDA.Name.ToString(),
                        "PF No.: " + _OPDA.EMPIDPFNo.ToString(),
                    ];

                // Convert back to array if needed
                string[] lines = linesList.ToArray();

                // Draw each line
                foreach (string line in lines)
                {
                    gfx.DrawString(line, font2, brush, new XPoint(textXPosition_To, textYPosition_To), XStringFormats.TopLeft);
                    textYPosition_To += lineHeight; // Move to the next line
                }
                XFont font3 = new XFont("Arial", 10, XFontStyle.Bold);

                // Define the text to draw, line by line
                List<string> _lines = new List<string>
                    {
                "",
                "Dear Sir / Madam,",
                "",
                "Greetings from K. M. Dastur Reinsurance Brokers Pvt Ltd.!",
                "",
                "You have purchased the following:" };
                _lines.Add($"");
                foreach (DataRow item in table.Rows)
                {
                    _lines.Add($"           {item["ProductName"].ToString()}");
                    _lines.Add($"");
                }
                _lines.Add("Your policy shall commence on 10th March 2025.");
                _lines.Add($"");
                _lines.Add("Based on the data and payment, we give below the details of the family members. ");
                _lines.Add($"");


                // Draw each line
                foreach (string line in _lines)
                {
                    gfx.DrawString(line, font1, brush, new XPoint(textXPosition_To, textYPosition_To), XStringFormats.TopLeft);
                    textYPosition_To += lineHeight; // Move to the next line
                }

                double tableXPosition = 35;  // Starting X-coordinate of the table
                double tableYPosition = 370; // Starting Y-coordinate of the table
                double rowHeight = 12;       // Height of each row
                double[] columnWidths = { 250, 130, 130 }; // Width of each column



                
                // Create a list of family members
                List<FamilyMember> familyMembers =
                [
                    // Add header row
                    new FamilyMember("Name", "Date of Birth", "Relation"),
                    // Add data rows
                    // Self
                    new FamilyMember(
                        _OPDA.Name ?? "",
                        _OPDA.DateOfBirth?.ToString("dd-MM-yyyy") ?? "",
                        "Self"
                    ),
                ];

                // Spouse

                if (!string.IsNullOrWhiteSpace(_OPDA.SpouseName))
                {
                        familyMembers.Add(new FamilyMember(
                        _OPDA.SpouseName ?? "",
                        _OPDA.SpouseDateOfBirth?.ToString("dd-MM-yyyy") ?? "",
                        string.IsNullOrEmpty(_OPDA.SpouseName) ? "" : "Spouse"
                    ));
                }
                if (!string.IsNullOrWhiteSpace(_OPDA.Child1Name))
                {
                    // Child 1
                    familyMembers.Add(new FamilyMember(
                    _OPDA.Child1Name ?? "",
                    _OPDA.Child1DateOfBirth?.ToString("dd-MM-yyyy") ?? "",
                    string.IsNullOrEmpty(_OPDA.Child1Name) ? "" : "Child1"
                ));
                }

                // Child 2
                if (!string.IsNullOrWhiteSpace(_OPDA.Child2Name))
                {
                    familyMembers.Add(new FamilyMember(
                        _OPDA.Child2Name ?? "",
                        _OPDA.Child2DateOfBirth?.ToString("dd-MM-yyyy") ?? "",
                        string.IsNullOrEmpty(_OPDA.Child2Name) ? "" : "Child2"
                    ));
                }
                // Draw each row
                foreach (var member in familyMembers)
                {
                    double currentX = tableXPosition;

                    // Draw first column (Name)
                    gfx.DrawRectangle(XPens.Black, currentX, tableYPosition, columnWidths[0], rowHeight);
                    gfx.DrawString(member.Name, font1, brush, new XRect(currentX, tableYPosition, columnWidths[0], rowHeight), XStringFormats.Center);
                    currentX += columnWidths[0];

                    // Draw second column (Date of Birth)
                    gfx.DrawRectangle(XPens.Black, currentX, tableYPosition, columnWidths[1], rowHeight);
                    gfx.DrawString(member.DateOfBirth, font1, brush, new XRect(currentX, tableYPosition, columnWidths[1], rowHeight), XStringFormats.Center);
                    currentX += columnWidths[1];

                    // Draw third column (Relation)
                    gfx.DrawRectangle(XPens.Black, currentX, tableYPosition, columnWidths[2], rowHeight);
                    gfx.DrawString(member.Relation, font1, brush, new XRect(currentX, tableYPosition, columnWidths[2], rowHeight), XStringFormats.Center);

                    // Move to the next row
                    tableYPosition += rowHeight;
                }
                string[] _line1 = new string[]
                    {
                "",
                "Name of the Association: "+_OPDA.AssociationName.ToUpper(),
                ""
                    };
                textYPosition_To = tableYPosition;
                // Draw each line
                foreach (string line in _line1)
                {
                    gfx.DrawString(line, font1, brush, new XPoint(textXPosition_To, textYPosition_To), XStringFormats.TopLeft);
                    textYPosition_To += lineHeight; // Move to the next line
                }

                double table2XPosition = 35;  // Starting X-coordinate of the table
                double table2YPosition = textYPosition_To; // Starting Y-coordinate of the table
                double rowHeight2 = 12;       // Height of each row
                double[] columnWidths2 = { 150, 75, 85, 160, 80 }; // Width of each column


                List<string[]> tableDataList =
                [
                    // Add header row
                    new string[] { "Policy Type", "Sum Insured", "Premium Remitted", "Transaction Id", "Date of Payment" },
                ];

                // Conditionally add "Digit Payment Protection" row
                if (table.Rows.Count >= 1)
                {
                    _OPDA.DigitPaymentProtectionPurchased = true;
                    DataRow dataRow = table.Rows[0];
                    string policytype = "-";
                    if (dataRow["ProductName"].ToString().Length < 30)
                        policytype = dataRow["ProductName"].ToString();
                    else
                    {
                        policytype = dataRow["ProductName"].ToString().Substring(0, 29);
                    }
                    string paymentdate= "";
                    if (DateTime.TryParse(dataRow["Payment Date/Cheque Date"].ToString(), out DateTime result))
                    {
                        paymentdate = (Convert.ToDateTime(result).ToString("dd-MM-yyyy") ?? "");
                    }

                    tableDataList.Add(new string[]
                            {
                    policytype,
                    _OPDA.DigitPaymentProtectionPurchased ?  dataRow["SumInsured"].ToString() : "",
                    _OPDA.DigitPaymentProtectionPurchased ? dataRow["TotalProductPremimum"].ToString() : "",
                    _OPDA.DigitPaymentProtectionPurchased ? dataRow["Transaction Number/Cheque Number"].ToString() : "",
                    _OPDA.DigitPaymentProtectionPurchased ? paymentdate : ""
                            });
                    
                }
                if (table.Rows.Count >= 2)
                {

                    _OPDA.OutPatientDepartmentPolicyPurchased = true;
                }
                // Conditionally add "Out Patient Department Policy" row
                if (_OPDA.OutPatientDepartmentPolicyPurchased)
                {
                    DataRow dataRow = table.Rows[1];
                    string policytype = "-";
                    if (dataRow["ProductName"].ToString().Length < 30)
                        policytype = dataRow["ProductName"].ToString();
                    else
                    {
                        policytype = dataRow["ProductName"].ToString().Substring(0, 29);
                    }
                    string paymentdate = "";
                    if (DateTime.TryParse(dataRow["Payment Date/Cheque Date"].ToString(), out DateTime result))
                    {
                        paymentdate= (Convert.ToDateTime(result).ToString("dd-MM-yyyy") ?? "") ;
                    }
                        tableDataList.Add(new string[]
                        {
                   policytype,
                    _OPDA.OutPatientDepartmentPolicyPurchased ? dataRow["SumInsured"].ToString() : "",
                    _OPDA.OutPatientDepartmentPolicyPurchased ? dataRow["TotalProductPremimum"].ToString() : "",
                    _OPDA.OutPatientDepartmentPolicyPurchased ? dataRow["Transaction Number/Cheque Number"].ToString() : "",
                    _OPDA.OutPatientDepartmentPolicyPurchased ? paymentdate : ""
                        });
                    
                }

                // Conditionally add "Total Premium Paid" row
                if (_OPDA.DigitPaymentProtectionPurchased || _OPDA.OutPatientDepartmentPolicyPurchased)
                {
                    tableDataList.Add(new string[]
                    {
                    "Total Premium Paid",
                    "",
                    (table.Rows[0]["Total Paid Premium"].ToString()),
                    "",
                    ""
                    });
                }

                // Convert to 2D array if needed
                string[,] tableData2 = new string[tableDataList.Count, 5];
                for (int i = 0; i < tableDataList.Count; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        tableData2[i, j] = tableDataList[i][j];
                    }
                }


                // Draw the table
                for (int row = 0; row < tableData2.GetLength(0); row++)
                {
                    double currentX = tableXPosition;

                    // Draw each column in the row
                    for (int col = 0; col < tableData2.GetLength(1); col++)
                    {
                        // Draw cell border
                        gfx.DrawRectangle(XPens.Black, currentX, table2YPosition, columnWidths2[col], rowHeight);

                        // Draw cell content
                        gfx.DrawString(tableData2[row, col], font1, brush, new XRect(currentX, table2YPosition, columnWidths2[col], rowHeight), XStringFormats.Center);

                        // Move to the next column
                        currentX += columnWidths2[col];
                    }

                    // Move to the next row
                    table2YPosition += rowHeight;
                }

                string[] _line2 = new string[]
                {
                "",
                "Kindly ensure that they are correct else you can edit the same in the website “https://kmd-enrolmentportal.com”",
                "",
                "We wish you excellent health and assure you of our support.",
                "",
                "Yours faithfully,"

                };
                textYPosition_To = table2YPosition;
                // Draw each line
                foreach (string line in _line2)
                {
                    gfx.DrawString(line, font0, brush, new XPoint(textXPosition_To, textYPosition_To), XStringFormats.TopLeft);
                    textYPosition_To += lineHeight; // Move to the next line
                }

                XImage image1 = XImage.FromFile("Templates/kmdSign.jpg");

                double xPosition1 = 35;
                double yPosition1 = textYPosition_To + 12;

                // Draw the image
                gfx.DrawImage(image1, xPosition1, yPosition1);
                XImage imageD;
                if (_OPDA.DigitPaymentProtectionPurchased == true)
                {
                    imageD = XImage.FromFile("Templates/checked.png");
                }
                else
                {
                    imageD = XImage.FromFile("Templates/unchecked.png");
                }
                double xPositionD = 45;
                double yPositionD = 278;

                // Draw the image
                gfx.DrawImage(imageD, xPositionD, yPositionD, 10, 10);

                XImage imageO;
                if (_OPDA.OutPatientDepartmentPolicyPurchased == true)
                {
                    imageO = XImage.FromFile("Templates/checked.png");
                }
                else
                {
                    imageO = XImage.FromFile("Templates/unchecked.png");
                }
                double xPositionO = 45;
                double yPositionO = 300;

                // Draw the image
                gfx.DrawImage(imageO, xPositionO, yPositionO, 10, 10);


                gfx.DrawString("Anahita Daver", font2, brush, new XPoint(xPosition1, yPosition1 + 86), XStringFormats.TopLeft);

                XImage imageF = XImage.FromFile("Templates/kmdFooter.jpg");

                double xPositionF = ((page.Width - (page.Width - 40)) / 2) - .5;
                double yPositionF = (page.Height - 32.5);

                // Draw the image
                gfx.DrawImage(imageF, xPositionF, yPositionF, (page.Width - 50), 20);

                // Save the PDF to the memory stream
                document.Save(memoryStream);

                // Convert MemoryStream to byte array
                byte[] pdfBytes = memoryStream.ToArray();

                // Return as a downloadable file
                return File(pdfBytes, "application/pdf", "GeneratedDocument.pdf");
            }






        }
    }
}



