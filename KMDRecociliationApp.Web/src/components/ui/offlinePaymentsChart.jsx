import React, { useMemo, useState, useEffect } from "react";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "./card";
import { ChartContainer, ChartTooltip, ChartTooltipContent } from "./chart";
import { Label, Pie, PieChart } from "recharts";
import { getOfflinePayments } from "@/services/dashboard";
import { userStore } from "@/lib/store";

const OfflinePaymentsChart = () => {
  const user = userStore((state) => state.user);

  const associationId =
    user?.userType?.name?.toLowerCase()?.trim() === "association"
      ? user?.userId
      : 0;

  const [offlinePayments, setOfflinePayments] = useState([]);

  const chartConfig = {
    NEFT: {
      label: "NEFT",
      color: "hsl(var(--chart-1))",
    },
    UPI: {
      label: "UPI",
      color: "hsl(var(--chart-2))",
    },
    Cheque: {
      label: "Cheque",
      color: "hsl(var(--chart-3))",
    },
  };

  useEffect(() => {
    const fetchPaymentsData = async () => {
      try {
        const response = await getOfflinePayments(associationId || 0);

        if (response.status === "success") {
          const dataWithFill = response.data.map((item) => ({
            ...item,
            fill:
              chartConfig[item.status]?.color || "hsl(var(--chart-default))",
          }));

          setOfflinePayments(dataWithFill);
        }
      } catch (error) {
       console.error("Error fetching offline payments data:", error);
      }
    };

    fetchPaymentsData();
  }, []);

  const totalOfflinePayments = useMemo(() => {
    return offlinePayments.reduce((acc, curr) => acc + curr.count, 0);
  }, [offlinePayments]);

  return (
    <Card className="flex flex-col h-full">
      <CardHeader className="items-center pb-0">
        <CardTitle> Payments</CardTitle>
      </CardHeader>
      {offlinePayments.length > 0 ? (
        <CardContent className="flex-1 pb-0">
          <ChartContainer
            config={chartConfig}
            className="mx-auto aspect-square max-h-[250px]"
          >
            <PieChart>
              <ChartTooltip
                cursor={false}
                content={<ChartTooltipContent hideLabel />}
              />
              <Pie
                data={offlinePayments}
                dataKey="count"
                nameKey="status"
                innerRadius={60}
                strokeWidth={5}
              >
                <Label
                  content={({ viewBox }) => {
                    if (viewBox && "cx" in viewBox && "cy" in viewBox) {
                      return (
                        <text
                          x={viewBox.cx}
                          y={viewBox.cy}
                          textAnchor="middle"
                          dominantBaseline="middle"
                        >
                          <tspan
                            x={viewBox.cx}
                            y={viewBox.cy}
                            className="fill-foreground text-3xl font-bold"
                          >
                            {totalOfflinePayments.toLocaleString()}
                          </tspan>
                          <tspan
                            x={viewBox.cx}
                            y={(viewBox.cy || 0) + 24}
                            className="fill-muted-foreground"
                          >
                            Total Payments
                          </tspan>
                        </text>
                      );
                    }
                  }}
                />
              </Pie>
            </PieChart>
          </ChartContainer>
        </CardContent>
      ) : (
        <CardContent>
          <ChartContainer
            config={chartConfig}
            className="mx-auto aspect-square max-h-[250px]"
          >
            <div className="justify-center items-center flex h-full text-xl text-gray-500">
              No data available
            </div>
          </ChartContainer>
        </CardContent>
      )}
    </Card>
  );
};

export default OfflinePaymentsChart;
