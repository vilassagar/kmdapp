/* eslint-disable react/prop-types */
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

export const ErrorDisplay = ({ error }) => (
  <Card className="p-6">
    <div className="text-red-500 text-center">
      <p className="text-lg font-semibold">Error loading data</p>
      <p className="mt-2">{error}</p>
    </div>
  </Card>
);
