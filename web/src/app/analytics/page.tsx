'use client';

import { useState, useEffect } from 'react';
import Link from 'next/link';
import { ArrowLeft, TrendingUp, BarChart3, AlertCircle } from 'lucide-react';
import { RestaurantSelector } from '@/components/dashboard/restaurant-selector';
import { Button } from '@/components/ui/button';
import { apiClient, CorrelationAnalysis, AnalyticsData } from '@/lib/api-client';

export default function AnalyticsPage() {
  const [selectedRestaurantId, setSelectedRestaurantId] = useState<number | null>(null);
  const [correlationData, setCorrelationData] = useState<CorrelationAnalysis | null>(null);
  const [analyticsData, setAnalyticsData] = useState<AnalyticsData | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (selectedRestaurantId) {
      fetchAnalyticsData();
    }
  }, [selectedRestaurantId]);

  const fetchAnalyticsData = async () => {
    if (!selectedRestaurantId) return;

    try {
      setLoading(true);
      setError(null);

      // Fetch both correlation analysis and basic analytics data
      const [correlation, analytics] = await Promise.all([
        apiClient.getCorrelationAnalysis(selectedRestaurantId),
        apiClient.getAnalyticsData(selectedRestaurantId, 90)
      ]);

      setCorrelationData(correlation);
      setAnalyticsData(analytics);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load analytics data');
      console.error('Analytics data fetch error:', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-8">
          <div className="flex items-center gap-4 mb-4">
            <Link href="/dashboard">
              <Button variant="outline" size="sm">
                <ArrowLeft className="h-4 w-4 mr-2" />
                Back to Dashboard
              </Button>
            </Link>
          </div>
          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">Analytics & Correlations</h1>
              <p className="text-gray-600">Advanced statistical analysis and metric correlations</p>
            </div>
            <RestaurantSelector
              selectedRestaurantId={selectedRestaurantId}
              onRestaurantChange={setSelectedRestaurantId}
            />
          </div>
        </div>

        {selectedRestaurantId ? (
          <div className="space-y-8">
            {/* Analytics Data Summary */}
            {analyticsData && (
              <div className="bg-white p-6 rounded-lg border shadow-sm">
                <div className="flex items-center gap-3 mb-4">
                  <BarChart3 className="h-5 w-5 text-blue-600" />
                  <h2 className="text-lg font-semibold text-gray-900">Data Overview</h2>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                  <div className="text-center">
                    <div className="text-2xl font-bold text-blue-600">{analyticsData.revenueDataPoints}</div>
                    <div className="text-sm text-gray-600">Revenue Points</div>
                  </div>
                  <div className="text-center">
                    <div className="text-2xl font-bold text-green-600">{analyticsData.metricDataPoints}</div>
                    <div className="text-sm text-gray-600">Metric Points</div>
                  </div>
                  <div className="text-center">
                    <div className="text-2xl font-bold text-purple-600">{analyticsData.availableMetrics.length}</div>
                    <div className="text-sm text-gray-600">Metrics</div>
                  </div>
                  <div className="text-center">
                    <div className={`text-2xl font-bold ${analyticsData.dataQuality === 'Good' ? 'text-green-600' : 'text-orange-600'}`}>
                      {analyticsData.dataQuality}
                    </div>
                    <div className="text-sm text-gray-600">Data Quality</div>
                  </div>
                </div>
              </div>
            )}

            {/* Correlation Analysis */}
            <div className="bg-white rounded-lg border shadow-sm">
              <div className="p-6 border-b">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <TrendingUp className="h-5 w-5 text-blue-600" />
                    <h2 className="text-lg font-semibold text-gray-900">Revenue Correlation Analysis</h2>
                  </div>
                  <Button 
                    onClick={fetchAnalyticsData} 
                    disabled={loading}
                    variant="outline"
                    size="sm"
                  >
                    {loading ? 'Analyzing...' : 'Refresh Analysis'}
                  </Button>
                </div>
                <p className="text-gray-600 mt-1">
                  Statistical correlation between operational metrics and revenue performance
                </p>
              </div>

              <div className="p-6">
                {loading && (
                  <div className="text-center py-8">
                    <div className="animate-spin h-8 w-8 border-2 border-blue-600 border-t-transparent rounded-full mx-auto mb-4"></div>
                    <p className="text-gray-600">Analyzing correlations...</p>
                  </div>
                )}

                {error && (
                  <div className="bg-red-50 border border-red-200 rounded-lg p-4">
                    <div className="flex items-center gap-2">
                      <AlertCircle className="h-5 w-5 text-red-600" />
                      <p className="text-red-800 font-medium">Analysis Error</p>
                    </div>
                    <p className="text-red-700 mt-1">{error}</p>
                  </div>
                )}

                {correlationData && correlationData.correlations.length > 0 && (
                  <div className="space-y-4">
                    <div className="grid gap-4">
                      {correlationData.correlations.map((correlation, index) => (
                        <div key={index} className="border rounded-lg p-4 hover:bg-gray-50 transition-colors">
                          <div className="flex items-center justify-between mb-2">
                            <div className="flex items-center gap-3">
                              <div className="text-lg font-semibold text-gray-900">
                                {correlation.displayName}
                              </div>
                              <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                                correlation.isSignificant 
                                  ? 'bg-green-100 text-green-800'
                                  : 'bg-gray-100 text-gray-600'
                              }`}>
                                {correlation.isSignificant ? 'Significant' : 'Not Significant'}
                              </span>
                            </div>
                            <div className="text-right">
                              <div className={`text-2xl font-bold ${
                                correlation.coefficient > 0 ? 'text-green-600' : 'text-red-600'
                              }`}>
                                {correlation.coefficient > 0 ? '+' : ''}{correlation.coefficient.toFixed(3)}
                              </div>
                              <div className="text-sm text-gray-600">{correlation.strength}</div>
                            </div>
                          </div>
                          <div className="text-gray-700 mb-2">
                            {correlation.interpretation}
                          </div>
                          <div className="flex items-center gap-4 text-sm text-gray-500">
                            <span>Direction: {correlation.direction}</span>
                            <span>p-value: {correlation.pValue.toFixed(4)}</span>
                          </div>
                        </div>
                      ))}
                    </div>

                    <div className="mt-6 p-4 bg-blue-50 rounded-lg">
                      <h4 className="font-medium text-blue-900 mb-2">Analysis Summary</h4>
                      <p className="text-blue-800 text-sm">
                        Analysis performed on {new Date(correlationData.analysisDate).toLocaleDateString()} 
                        using {correlationData.correlationType} correlation method.
                        {correlationData.correlations.filter(c => c.isSignificant).length} out of {correlationData.correlations.length} correlations are statistically significant (p &lt; 0.05).
                      </p>
                    </div>
                  </div>
                )}

                {correlationData && correlationData.correlations.length === 0 && (
                  <div className="text-center py-8">
                    <TrendingUp className="h-12 w-12 text-gray-400 mx-auto mb-4" />
                    <h3 className="text-lg font-medium text-gray-900 mb-2">No Correlations Found</h3>
                    <p className="text-gray-600">
                      Insufficient data to perform correlation analysis. Try again with more historical data.
                    </p>
                  </div>
                )}
              </div>
            </div>
          </div>
        ) : (
          <div className="flex items-center justify-center h-64">
            <div className="text-center">
              <div className="text-gray-500 mb-4">
                <TrendingUp className="h-16 w-16 mx-auto" />
              </div>
              <h3 className="text-lg font-medium text-gray-900 mb-2">Select a Restaurant</h3>
              <p className="text-gray-600">Choose a restaurant to view advanced analytics and correlations.</p>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}