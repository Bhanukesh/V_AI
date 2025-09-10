'use client';

import { useState, useEffect } from 'react';
import { 
  DollarSign, 
  Clock, 
  Users, 
  CheckCircle, 
  Star,
  TrendingUp,
  Calendar 
} from 'lucide-react';
import { KpiCard } from '@/components/dashboard/kpi-card';
import { RestaurantSelector } from '@/components/dashboard/restaurant-selector';
import { MetricChart } from '@/components/dashboard/metric-chart';
import { RestaurantKpis, apiClient } from '@/lib/api-client';

export default function DashboardPage() {
  const [selectedRestaurantId, setSelectedRestaurantId] = useState<number | null>(null);
  const [kpiData, setKpiData] = useState<RestaurantKpis | null>(null);
  const [selectedPeriod, setSelectedPeriod] = useState('last_30d');
  const [isLoadingKpis, setIsLoadingKpis] = useState(false);

  useEffect(() => {
    if (selectedRestaurantId) {
      fetchKpiData();
    }
  }, [selectedRestaurantId, selectedPeriod]);

  const fetchKpiData = async () => {
    if (!selectedRestaurantId) return;
    
    try {
      setIsLoadingKpis(true);
      const data = await apiClient.getRestaurantKpis(selectedRestaurantId, selectedPeriod);
      setKpiData(data);
    } catch (error) {
      console.error('Failed to fetch KPI data:', error);
    } finally {
      setIsLoadingKpis(false);
    }
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(amount);
  };

  const formatPercentage = (value: number) => {
    return `${(value * 100).toFixed(1)}%`;
  };

  const formatTime = (minutes: number) => {
    return `${minutes.toFixed(1)} min`;
  };

  const formatRating = (rating: number) => {
    return `${rating.toFixed(1)}/5`;
  };

  const periodOptions = [
    { value: 'last_7d', label: 'Last 7 days' },
    { value: 'last_30d', label: 'Last 30 days' },
    { value: 'last_90d', label: 'Last 90 days' },
  ];

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-8">
          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">Restaurant Dashboard</h1>
              <p className="text-gray-600">Monitor performance metrics and KPIs</p>
            </div>
            <div className="flex flex-col md:flex-row gap-4">
              <select
                value={selectedPeriod}
                onChange={(e) => setSelectedPeriod(e.target.value)}
                className="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              >
                {periodOptions.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
              <RestaurantSelector
                selectedRestaurantId={selectedRestaurantId}
                onRestaurantChange={setSelectedRestaurantId}
              />
            </div>
          </div>
        </div>

        {selectedRestaurantId ? (
          <>
            {/* KPI Cards */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
              <KpiCard
                title="Total Revenue"
                value={kpiData ? formatCurrency(kpiData.totalRevenue) : isLoadingKpis ? "Loading..." : "N/A"}
                icon={DollarSign}
                description={kpiData ? `${kpiData.totalTransactions} transactions` : undefined}
              />
              <KpiCard
                title="Avg Prep Time"
                value={kpiData ? formatTime(kpiData.prepTimeAverage) : isLoadingKpis ? "Loading..." : "N/A"}
                icon={Clock}
                changeType={kpiData && kpiData.prepTimeAverage > 15 ? 'negative' : 'positive'}
                description="Kitchen efficiency"
              />
              <KpiCard
                title="Order Accuracy"
                value={kpiData ? formatPercentage(kpiData.orderAccuracyAverage) : isLoadingKpis ? "Loading..." : "N/A"}
                icon={CheckCircle}
                changeType={kpiData && kpiData.orderAccuracyAverage > 0.9 ? 'positive' : 'negative'}
                description="Correct orders"
              />
              <KpiCard
                title="Customer Satisfaction"
                value={kpiData ? formatRating(kpiData.customerSatisfactionAverage) : isLoadingKpis ? "Loading..." : "N/A"}
                icon={Star}
                changeType={kpiData && kpiData.customerSatisfactionAverage > 4.0 ? 'positive' : 'negative'}
                description="Average rating"
              />
            </div>

            {/* Additional KPI Row */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
              <KpiCard
                title="Daily Average Revenue"
                value={kpiData ? formatCurrency(kpiData.averageRevenue) : isLoadingKpis ? "Loading..." : "N/A"}
                icon={TrendingUp}
                description="Per day average"
              />
              <KpiCard
                title="Table Turnover"
                value={kpiData ? formatTime(kpiData.tableTurnoverAverage) : isLoadingKpis ? "Loading..." : "N/A"}
                icon={Users}
                description="Average dining time"
              />
              <KpiCard
                title="Reporting Period"
                value={kpiData ? new Date(kpiData.periodStart).toLocaleDateString() : isLoadingKpis ? "Loading..." : "N/A"}
                icon={Calendar}
                description={kpiData ? `to ${new Date(kpiData.periodEnd).toLocaleDateString()}` : undefined}
              />
            </div>

            {/* Charts */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <MetricChart
                restaurantId={selectedRestaurantId}
                metricName="prep_time"
                title="Kitchen Prep Time"
                color="#EF4444"
                unit="min"
              />
              <MetricChart
                restaurantId={selectedRestaurantId}
                metricName="customer_satisfaction"
                title="Customer Satisfaction"
                color="#10B981"
                unit="/5"
              />
              <MetricChart
                restaurantId={selectedRestaurantId}
                metricName="order_accuracy"
                title="Order Accuracy"
                color="#3B82F6"
                unit="%"
              />
              <MetricChart
                restaurantId={selectedRestaurantId}
                metricName="table_turnover"
                title="Table Turnover Time"
                color="#8B5CF6"
                unit="min"
              />
            </div>
          </>
        ) : (
          <div className="flex items-center justify-center h-64">
            <div className="text-center">
              <div className="text-gray-500 mb-4">
                <Users className="h-16 w-16 mx-auto" />
              </div>
              <h3 className="text-lg font-medium text-gray-900 mb-2">Select a Restaurant</h3>
              <p className="text-gray-600">Choose a restaurant to view performance metrics and analytics.</p>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}