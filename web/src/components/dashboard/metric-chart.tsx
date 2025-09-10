'use client';

import { useState, useEffect } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { MetricValue, apiClient } from '@/lib/api-client';

interface MetricChartProps {
  restaurantId: number;
  metricName: string;
  title: string;
  color?: string;
  unit?: string;
}

interface ChartDataPoint {
  time: string;
  value: number;
  formattedTime: string;
}

export function MetricChart({ restaurantId, metricName, title, color = '#3B82F6', unit }: MetricChartProps) {
  const [data, setData] = useState<ChartDataPoint[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchMetricData = async () => {
      try {
        setIsLoading(true);
        setError(null);
        
        // Get metrics from the last 7 days
        const endDate = new Date();
        const startDate = new Date(endDate);
        startDate.setDate(startDate.getDate() - 7);

        const metrics = await apiClient.getRestaurantMetrics(restaurantId, {
          metricName,
          startDate: startDate.toISOString(),
          endDate: endDate.toISOString(),
          pageSize: 100,
        });

        // Process and sort the data
        const processedData = metrics
          .map((metric: MetricValue) => {
            const date = new Date(metric.timestamp);
            return {
              time: date.toISOString(),
              value: metric.value,
              formattedTime: date.toLocaleDateString('en-US', { 
                month: 'short', 
                day: 'numeric',
                hour: '2-digit',
                minute: '2-digit'
              }),
            };
          })
          .sort((a, b) => new Date(a.time).getTime() - new Date(b.time).getTime())
          .slice(-20); // Show last 20 data points for better visualization

        setData(processedData);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load metric data');
      } finally {
        setIsLoading(false);
      }
    };

    if (restaurantId) {
      fetchMetricData();
    }
  }, [restaurantId, metricName]);

  if (isLoading) {
    return (
      <div className="bg-white p-6 rounded-lg border shadow-sm">
        <div className="animate-pulse">
          <div className="h-4 bg-gray-200 rounded w-1/3 mb-4"></div>
          <div className="h-64 bg-gray-200 rounded"></div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-white p-6 rounded-lg border shadow-sm">
        <h3 className="text-lg font-medium text-gray-900 mb-2">{title}</h3>
        <div className="text-red-600 text-sm">Error: {error}</div>
      </div>
    );
  }

  if (data.length === 0) {
    return (
      <div className="bg-white p-6 rounded-lg border shadow-sm">
        <h3 className="text-lg font-medium text-gray-900 mb-2">{title}</h3>
        <div className="text-gray-500 text-sm">No data available for this metric</div>
      </div>
    );
  }

  return (
    <div className="bg-white p-6 rounded-lg border shadow-sm">
      <h3 className="text-lg font-medium text-gray-900 mb-4">{title}</h3>
      <div style={{ width: '100%', height: '300px' }}>
        <ResponsiveContainer>
          <LineChart data={data}>
            <CartesianGrid strokeDasharray="3 3" stroke="#E5E7EB" />
            <XAxis 
              dataKey="formattedTime" 
              stroke="#6B7280"
              fontSize={12}
              angle={-45}
              textAnchor="end"
              height={60}
            />
            <YAxis 
              stroke="#6B7280"
              fontSize={12}
            />
            <Tooltip 
              contentStyle={{
                backgroundColor: '#FFFFFF',
                border: '1px solid #E5E7EB',
                borderRadius: '6px',
                fontSize: '14px'
              }}
              formatter={(value) => [`${value}${unit ? ` ${unit}` : ''}`, title]}
              labelStyle={{ color: '#374151' }}
            />
            <Line 
              type="monotone" 
              dataKey="value" 
              stroke={color}
              strokeWidth={2}
              dot={{ fill: color, strokeWidth: 2, r: 4 }}
              activeDot={{ r: 6, stroke: color, strokeWidth: 2 }}
            />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
}