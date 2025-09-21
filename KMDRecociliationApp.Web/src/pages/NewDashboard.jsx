import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import React, { useEffect, useState } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  PieChart,
  Pie,
  Cell,
  ResponsiveContainer,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  Legend,
} from "recharts";
import { getDashboardData } from "@/services/dashboard";
import { getCampaignsList } from "@/services/campaigns";
import { Label } from "@/components/ui/label";
import { Combobox } from "@/components/ui/comboBox";
import { Button } from "@/components/ui/button";
import { userStore } from "@/lib/store";
import { useNavigate } from "react-router-dom";
import { getAssociationsByIdFilter } from "@/services/customerProfile";
import dashboardimage from "@/assets/dashboard_image.jpg";
const PAGE_NAME = "dashboard";

// Default empty data structure to prevent errors
const INITIAL_DATA = {
  Pensioner: [],
  PendingRejected: [],
  Initiated: [],
  Completed: [],
};

const NewDashboard = () => {
  const navigate = useNavigate();
  const [data, setData] = useState(INITIAL_DATA);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [campaigns, setCampaigns] = useState();
  const [campaign, setCampaign] = useState();
  const [associations, setAssociations] = useState([]);
  const [association, setAssociation] = useState();
  // Color palette
  const COLORS = ["#0088FE", "#00C49F", "#FFBB28", "#FF8042"];
  const user = userStore((state) => state.user);
  const dashboarduser =
    user?.userType?.name?.toLowerCase()?.trim() === "internaluser" ||
    user?.userType?.name?.toLowerCase()?.trim() === "association";
  const notdashboarduser =
    user?.userType?.name?.toLowerCase()?.trim() === "pensioner" ||
    user?.userType?.name?.toLowerCase()?.trim() === "community";
  const isProfileComplete = user?.isProfileComplete;
  const associationId =
    user?.userType?.name?.toLowerCase()?.trim() === "association"
      ? user?.associationId
      : 0;
  const datacollection =
    user?.userType?.name?.toLowerCase()?.trim() === "datacollection";
  console.log(datacollection);

  const getData = async (campaignId, associationId) => {
    try {
      setIsLoading(true);

      const response = await getDashboardData(
        campaignId || 0,
        associationId || associationId
      );
      if (response.status === "success" && response.data) {
        setData(response.data);
      } else {
        setError("Failed to fetch dashboard data");
      }
    } catch (error) {
      console.error("Error fetching dashboard data:", error);
      setError(error.message || "An error occurred while fetching data");
    } finally {
      setIsLoading(false);
    }
  };

  const getCampaigns = async (associationId) => {
    try {
      const response = await getCampaignsList(associationId, "all");
      if (response.status === "success" && response.data) {
        setCampaigns(response.data);

        if (response.data.length > 0) {
          setCampaign(response.data[0]);
          getData(response.data[0].id, associationId);
        }
      } else {
        setError("Failed to get campaigns");
      }
    } catch (error) {
      console.log("Error", error);
    }
  };

  const getAssociationsData = async (associationId) => {
    try {
      const response = await getAssociationsByIdFilter(associationId, "all");
      if (response.status === "success" && response.data) {
        setAssociations(response.data);
      } else {
        setError("Failed to get associations");
      }
    } catch (error) {
      console.log("Error", error);
    }
  };

  useEffect(() => {
    // Fetch associations data first
    const fetchInitialData = async () => {
      try {
        setIsLoading(true);

        // Fetch associations first
        const associationsResponse = await getAssociationsByIdFilter(
          associationId,
          "all"
        );
        if (
          associationsResponse.status === "success" &&
          associationsResponse.data
        ) {
          setAssociations(associationsResponse.data);

          // If associationId is not zero, find and select the specific association
          if (associationId !== 0) {
            const matchedAssociation = associationsResponse.data.find(
              (assoc) => assoc.id === associationId
            );

            if (matchedAssociation) {
              setAssociation(matchedAssociation);

              // Fetch campaigns for this specific association
              const campaignsResponse = await getCampaignsList(
                matchedAssociation.id,
                "all"
              );

              if (
                campaignsResponse.status === "success" &&
                campaignsResponse.data?.length
              ) {
                setCampaigns(campaignsResponse.data);

                // Set first campaign and fetch its data
                const firstCampaign = campaignsResponse.data[0];
                setCampaign(firstCampaign);

                // Fetch dashboard data
                await getData(firstCampaign.id, matchedAssociation.id);
              }
            }
          } else {
            // If associationId is zero, handle it differently (e.g., first association or no selection)
            if (associationsResponse.data.length > 0) {
              const firstAssociation =
                // associationsResponse.data[0];
                associationsResponse.data.find((assoc) => assoc.id === -1);
              setAssociation(firstAssociation);

              // Fetch campaigns for the first association
              const campaignsResponse = await getCampaignsList(
                firstAssociation.id,
                "all"
              );

              if (
                campaignsResponse.status === "success" &&
                campaignsResponse.data?.length
              ) {
                setCampaigns(campaignsResponse.data);

                // Set first campaign and fetch its data
                const firstCampaign = campaignsResponse.data[0];
                setCampaign(firstCampaign);

                // Fetch dashboard data
                await getData(firstCampaign.id, firstAssociation.id);
              }
            }
          }
        }
      } catch (error) {
        console.error("Error fetching initial data:", error);
        setError(error.message || "Failed to fetch initial data");
      } finally {
        setIsLoading(false);
      }
    };

    if (dashboarduser) {
      fetchInitialData();
    } else {
      setIsLoading(false);
    }
  }, [associationId]);

  const handleCellClick = (event) => {
    navigate(
      `/dashboardfilterdata?filter=${event.payload.name}&associationId=${associationId}&campaignId=${campaign.id}`
    );
  };

  const renderHollowPieChart = (dataKey) => {
    const chartData = data[dataKey] || [];

    // Calculate total count safely
    const totalCount = chartData.reduce(
      (sum, item) => sum + (item.count || 0),
      0
    );

    return (
      <>
        {dashboarduser ? (
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={chartData}
                dataKey="count"
                nameKey="name"
                cx="50%"
                cy="50%"
                innerRadius="40%"
                outerRadius="80%"
                fill="#8884d8"
                stroke="#fff"
                strokeWidth={2}
                onMouseDown={(event) => handleCellClick(event)}
                // label={({ name, percent }) =>
                //   `${name} (${isNaN(percent) ? 0 : (percent * 100).toFixed(0)}%)`
                // }
                labelLine={false}
              >
                {chartData.map((entry, index) => (
                  <Cell
                    key={`cell-${index}`}
                    fill={COLORS[index % COLORS.length]}
                  />
                ))}
              </Pie>
              <text
                x="50%"
                y="45%"
                textAnchor="middle"
                dominantBaseline="middle"
                className="text-2xl font-bold mb-10"
                fill="#333"
              >
                {totalCount}
              </text>
              <Tooltip />
              <Legend />
            </PieChart>
          </ResponsiveContainer>
        ) : (
          <div className="flex flex-col items-center justify-center min-h-screen gap-4">
            {!isProfileComplete && (
              <h5 className="text-red-600">Please Complete your profile</h5>
            )}
            <h1 className="text-4xl font-bold text-center">
              Welcome To KMD Enrollment Portal
            </h1>
          </div>
        )}
      </>
    );
  };

  // Render loading or error state
  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-full">
        <p>Loading dashboard...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex justify-center items-center h-full text-red-500">
        <p>Error: {error}</p>
      </div>
    );
  }
  const handleCampaignChange = (value) => {
    setCampaign(value);
  };
  const handleAssociationChange = async (value) => {
    setAssociation(value);
    try {
      const campaignsResponse = await getCampaignsList(value.id, "all");
      if (
        campaignsResponse.status === "success" &&
        campaignsResponse.data?.length
      ) {
        setCampaigns(campaignsResponse.data);

        // Set first campaign and fetch its data
        const firstCampaign = campaignsResponse.data[0];
        setCampaign(firstCampaign);
        await getData(firstCampaign.id, value.id);
      }
    } catch (error) {
      console.error("Error fetching campaigns:", error);
      setError("Failed to fetch campaigns");
    }
  };
  const handleSearch = () => {
    getData(campaign.id, association.id);
  };

  return (
    <>
      {dashboarduser ? (
        <div>
          <div className="flex flex-row items-center gap-2 w-full pl-5">
            <div className="flex flex-row items-center justify-between space-y-2">
              <Label htmlFor="association-name">
                <span className="mr-3">Association Name: </span>
              </Label>
              <Combobox
                options={associations}
                placeholder="Select association name"
                valueProperty="id"
                labelProperty="name"
                id="association-name"
                onChange={handleAssociationChange}
                value={association}
              />
            </div>
            <div className="flex items-center gap-1">
              <Label htmlFor="campaign-type" className="flex items-center">
                <span className="mr-1">Campaign Name</span>
                <span className="text-red-600">*</span>
              </Label>
            </div>
            <Combobox
              options={campaigns}
              placeholder="Select campaign type"
              valueProperty="id"
              labelProperty="name"
              onChange={handleCampaignChange}
              value={campaign}
              className="flex-grow"
            />
            <Button onClick={handleSearch}>Search</Button>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 p-4">
            <Card>
              <CardHeader>
                <CardTitle>Pensioners </CardTitle>
              </CardHeader>
              <CardContent>{renderHollowPieChart("pensioner")}</CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Payments Pending/Rejected </CardTitle>
              </CardHeader>
              <CardContent>
                {renderHollowPieChart("pendingRejected")}
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Payments Initiated </CardTitle>
              </CardHeader>
              <CardContent>{renderHollowPieChart("initiated")}</CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Payments Completed </CardTitle>
              </CardHeader>
              <CardContent>{renderHollowPieChart("completed")}</CardContent>
            </Card>
          </div>
        </div>
      ) : (
        <div className="flex flex-col items-center justify-center min-h-screen gap-4">
          {!isProfileComplete && (
            <div>
              <h5 className="text-red-600">
                Complete your profile now by{" "}
                <a
                  href="/profile"
                  className="text-blue-600 hover:underline inline-flex items-center"
                >
                  clicking here
                </a>
              </h5>
            </div>
          )}
          {datacollection ? (
            <div className="">
              <h1 className="text-4xl font-bold text-center">
                Welcome To Enrollment Portal
              </h1>
              <div className="flex justify-center sm:justify-start py-4">
                <img
                  src={dashboardimage}
                  alt="KMD logo"
                  className="h-auto w-auto"
                />
              </div>
            </div>
          ) : (
            <h1 className="text-4xl font-bold text-center">
              Welcome To KMD Enrollment Portal
            </h1>
          )}
        </div>
      )}
    </>
  );
};

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(NewDashboard))
);
