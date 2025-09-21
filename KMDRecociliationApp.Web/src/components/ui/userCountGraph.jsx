import React, { useMemo, useState, useEffect } from "react";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "./card";
import { ChartContainer, ChartTooltip, ChartTooltipContent } from "./chart";
import { Label, Pie, PieChart } from "recharts";
import { getUserCount } from "@/services/dashboard";
import { userStore } from "@/lib/store";

const UserCountGraph = () => {
  const user = userStore((state) => state.user);

  const associationId =
    user?.userType?.name?.toLowerCase()?.trim() === "association"
      ? user?.userId
      : 0;

  const [userCount, setUserCount] = useState([]);
  const [totalCount, setTotalCount] = useState();
  const chartConfig = {
    Pending: {
      label: "Pending",
      color: "hsl(var(--chart-1))",
    },
    Rejected: {
      label: "Rejected",
      color: "hsl(var(--chart-2))",
    },
    Completed: {
      label: "Completed",
      color: "hsl(var(--chart-3))",
    },
    Initiated: {
      label: "Initiated",
      color: "hsl(var(--chart-4))",
    },
    Failed: {
      label: "Failed",
      color: "hsl(var(--chart-5))",
    },
  };

  useEffect(() => {
    const fetchUsersData = async () => {
      try {
        const response = await getUserCount(associationId || 0);

        if (response.status === "success") {
          const totalUserObject = response.data?.find(
            (x) => x.name === "TotalUser"
          );
          setTotalCount(totalUserObject?.count);
          const dataWithFill = response.data.map((item) => ({
            ...item,
            fill: chartConfig[item.name]?.color || "hsl(var(--chart-default))",
          }));

          const updatedArray = dataWithFill.filter(
            (x) => x.name !== "TotalUser"
          );
          setUserCount(updatedArray);
        }
      } catch (error) {
        console.error("Error fetching offline payments data:", error);
      }
    };

    fetchUsersData();
  }, []);

  // const totalUserCount = useMemo(() => {
  //   return totalCount;
  // }, [userCount]);

  return (
    <Card className="flex flex-col h-full">
      <CardHeader className="pb-0 text-center">
        <CardTitle className="text-base">Pensioner Registration</CardTitle>
      </CardHeader>
      {userCount.length > 0 ? (
        <CardContent className="flex-1 pb-2 pt-2">
          <ChartContainer
            config={chartConfig}
            className="mx-auto aspect-square max-h-[200px]"
          >
            <PieChart>
              <ChartTooltip
                cursor={false}
                content={<ChartTooltipContent hideLabel />}
              />
              <Pie
                data={userCount}
                dataKey="count"
                nameKey="name"
                innerRadius={50}
                strokeWidth={4}
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
                            className="fill-foreground text-2xl font-bold"
                          >
                            {totalCount?.toLocaleString()}
                          </tspan>
                          <tspan
                            x={viewBox.cx}
                            y={(viewBox.cy || 0) + 20}
                            className="fill-muted-foreground text-sm"
                          >
                            Total Users
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
        <CardContent className="flex items-center justify-center h-[200px]">
          <div className="text-center text-gray-500">No data available</div>
        </CardContent>
      )}
    </Card>
  );
};

export default UserCountGraph;
