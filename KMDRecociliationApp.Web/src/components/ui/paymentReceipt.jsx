/* eslint-disable react/prop-types */
import React from "react";
import { Download } from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

// eslint-disable-next-line react/prop-types
const PaymentReceipt = ({ receiptData }) => {
  const handleDownload = async () => {
    try {
      const response = await fetch(
        `/api/receipt/download/${receiptData.receiptNo}`,
        {
          method: "GET",
        }
      );

      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = `Receipt_${receiptData.receiptNo}.pdf`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error("Error downloading receipt:", error);
    }
  };

  return (
    <Card className="max-w-2xl mx-auto bg-white">
      <CardHeader className="text-center border-b">
        <CardTitle className="text-2xl font-bold">
          Go Digit General Insurance
        </CardTitle>
        <p className="text-gray-600">Proposal Deposit Acknowledgement</p>
        <p className="text-sm text-gray-500">
          Your rupees have arrived, safe and sound.
        </p>
      </CardHeader>

      <CardContent className="p-6">
        <div className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <p className="text-gray-600">Receipt No.</p>
              <p className="font-medium">{receiptData.receiptNo}</p>
            </div>
            <div>
              <p className="text-gray-600">Receipt Date</p>
              <p className="font-medium">{receiptData.receiptDate}</p>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <p className="text-gray-600">Depositor Code</p>
              <p className="font-medium">{receiptData.depositorCode}</p>
            </div>
            <div>
              <p className="text-gray-600">Depositor Name</p>
              <p className="font-medium">{receiptData.depositorName}</p>
            </div>
          </div>

          <div className="mt-6">
            <h3 className="font-semibold mb-2">Payment Details</h3>
            <table className="w-full border-collapse">
              <thead>
                <tr className="bg-gray-50">
                  <th className="p-2 text-left border">Mode of Deposit</th>
                  <th className="p-2 text-left border">Instrument No.</th>
                  <th className="p-2 text-left border">Instrument Date</th>
                  <th className="p-2 text-left border">Amount (₹)</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td className="p-2 border">{receiptData?.modeOfDeposit}</td>
                  <td className="p-2 border">{receiptData?.instrumentNo}</td>
                  <td className="p-2 border">{receiptData?.instrumentDate}</td>
                  <td className="p-2 border">{receiptData?.amount}</td>
                </tr>
              </tbody>
            </table>
          </div>

          <button
            onClick={handleDownload}
            className="mt-6 flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
          >
            <Download size={18} />
            Download PDF
          </button>
        </div>
      </CardContent>
    </Card>
  );
};

export default PaymentReceipt;
