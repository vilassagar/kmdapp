import React, { useEffect, useState } from "react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "./card";
import { ChartContainer } from "./chart";
import { Bar, BarChart, LabelList, XAxis, YAxis } from "recharts";
import { userStore } from "@/lib/store";
import { getPaymentModes } from "@/services/dashboard";

const PaymentsModeGraph = () => {
  const user = userStore((state) => state.user);

  const associationId =
    user?.userType?.name?.toLowerCase()?.trim() === "association"
      ? user?.userId
      : 0;

  const [totalPayments, setTotalPayments] = useState([]);

  useEffect(() => {
    const fetchPaymentsData = async () => {
      try {
        const response = await getPaymentModes(associationId || 0);
        if (response.status === "success") {
          const data = response.data;
          setTotalPayments(data);
        }
      } catch (error) {
        console.error("Error fetching payments data:", error);
      }
    };

    fetchPaymentsData();
  }, [associationId]);

  const onlinePayment = totalPayments.find(
    (item) => item.mode.toLowerCase() === "online"
  ) || { count: 0 };
  const offlinePayment = totalPayments.find(
    (item) => item.mode.toLowerCase() === "offline"
  ) || { count: 0 };

  const totalPaymentsCount = onlinePayment.count + offlinePayment.count;

  const onlineSteps =
    totalPaymentsCount > 0
      ? (onlinePayment.count / totalPaymentsCount) * 100
      : 0;
  const offlineSteps =
    totalPaymentsCount > 0
      ? (offlinePayment.count / totalPaymentsCount) * 100
      : 0;

  return (
    <Card className="flex flex-col h-full" x-chunk="charts-01-chunk-2">
      <CardHeader>
        <CardTitle>Payment Modes</CardTitle>
        <CardDescription>
          Payments made by users in different modes.
        </CardDescription>
      </CardHeader>

      {totalPayments.length > 0 ? (
        <CardContent className="grid gap-4">
          <div className="grid auto-rows-min gap-2">
            <div className="flex items-baseline gap-1 text-2xl font-bold tabular-nums leading-none">
              {onlinePayment.count}
            </div>
            <ChartContainer
              config={{
                steps: {
                  label: "Steps",
                  color: "hsl(var(--chart-1))",
                },
              }}
              className="aspect-auto h-[32px] w-full"
            >
              <BarChart
                layout="vertical"
                margin={{
                  left: 0,
                  top: 0,
                  right: 0,
                  bottom: 0,
                }}
                data={[{ mode: "Online", steps: onlineSteps }]}
              >
                <Bar
                  dataKey="steps"
                  fill="var(--color-steps)"
                  radius={4}
                  barSize={32}
                >
                  <LabelList
                    position="insideLeft"
                    dataKey="mode"
                    offset={8}
                    fontSize={12}
                    fill="hsl(var(--chart-3))"
                  />
                </Bar>
                <YAxis dataKey="mode" type="category" hide />
                <XAxis type="number" domain={[0, 100]} hide />
              </BarChart>
            </ChartContainer>
          </div>
          <div className="grid auto-rows-min gap-2">
            <div className="flex items-baseline gap-1 text-2xl font-bold tabular-nums leading-none">
              {offlinePayment.count}
            </div>
            <ChartContainer
              config={{
                steps: {
                  label: "Steps",
                  color: "hsl(var(--chart-4))",
                },
              }}
              className="aspect-auto h-[32px] w-full"
            >
              <BarChart
                layout="vertical"
                margin={{
                  left: 0,
                  top: 0,
                  right: 0,
                  bottom: 0,
                }}
                data={[{ mode: "Offline", steps: offlineSteps }]}
              >
                <Bar
                  dataKey="steps"
                  fill="var(--color-steps)"
                  radius={4}
                  barSize={32}
                >
                  <LabelList
                    position="insideLeft"
                    dataKey="mode"
                    offset={8}
                    fontSize={12}
                    fill="hsl(var(--chart-2))"
                  />
                </Bar>
                <YAxis dataKey="mode" type="category" hide />
                <XAxis type="number" domain={[0, 100]} hide />
              </BarChart>
            </ChartContainer>
          </div>
        </CardContent>
      ) : (
        <CardContent className="h-48 flex justify-center items-center text-xl">
          <h1 className="text-gray-500">No data available for this chart</h1>
        </CardContent>
      )}
    </Card>
  );
};

export default PaymentsModeGraph;
