import { Bar, BarChart, XAxis } from "recharts";

import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
} from "@/components/ui/chart";
const chartData = [
  { date: "2024-07-15", created: 450, executed: 300 },
  { date: "2024-07-16", created: 380, executed: 100 },
  { date: "2024-07-17", created: 520, executed: 120 },
  { date: "2024-07-18", created: 640, executed: 550 },
  { date: "2024-07-19", created: 600, executed: 350 },
  { date: "2024-07-20", created: 480, executed: 400 },
];

const chartConfig = {
  running: {
    label: "Completed",
    color: "hsl(var(--chart-1))",
  },
  swimming: {
    label: "Executed",
    color: "hsl(var(--chart-2))",
  },
};

export function CampaignGraph() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Campaigns</CardTitle>
        <CardDescription>Track the progress in campaigns.</CardDescription>
      </CardHeader>
      <CardContent>
        <ChartContainer config={chartConfig}>
          <BarChart accessibilityLayer data={chartData}>
            <XAxis
              dataKey="campaignName"
              tickLine={false}
              tickMargin={10}
              axisLine={false}
            />
            <Bar
              dataKey="created"
              stackId="a"
              fill="var(--color-running)"
              radius={[0, 0, 4, 4]}
            />
            <Bar
              dataKey="executed"
              stackId="a"
              fill="var(--color-swimming)"
              radius={[4, 4, 0, 0]}
            />
            <ChartTooltip
              content={<ChartTooltipContent />}
              cursor={false}
              defaultIndex={1}
            />
          </BarChart>
        </ChartContainer>
      </CardContent>
    </Card>
  );
}
