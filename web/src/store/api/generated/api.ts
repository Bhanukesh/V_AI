/* eslint-disable -- Auto Generated File */
import { emptySplitApi as api } from "../empty-api";
const injectedRtkApi = api.injectEndpoints({
  endpoints: (build) => ({
    getRestaurants: build.query<GetRestaurantsApiResponse, GetRestaurantsApiArg>({
      query: () => ({ url: `/api/restaurants` }),
    }),
    getRestaurant: build.query<GetRestaurantApiResponse, GetRestaurantApiArg>({
      query: (queryArg) => ({ url: `/api/restaurants/${queryArg.id}` }),
    }),
    getRestaurantKpis: build.query<GetRestaurantKpisApiResponse, GetRestaurantKpisApiArg>({
      query: (queryArg) => ({ 
        url: `/api/restaurants/${queryArg.id}/kpis`, 
        params: { period: queryArg.period } 
      }),
    }),
    getRestaurantMetrics: build.query<GetRestaurantMetricsApiResponse, GetRestaurantMetricsApiArg>({
      query: (queryArg) => ({ 
        url: `/api/restaurants/${queryArg.restaurantId}/metrics`,
        params: {
          metricName: queryArg.metricName,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          page: queryArg.page,
          pageSize: queryArg.pageSize,
        }
      }),
    }),
    getCorrelationAnalysis: build.query<GetCorrelationAnalysisApiResponse, GetCorrelationAnalysisApiArg>({
      query: (queryArg) => ({ 
        url: `/api/analytics/restaurants/${queryArg.restaurantId}/correlations` 
      }),
    }),
    getAnalyticsData: build.query<GetAnalyticsDataApiResponse, GetAnalyticsDataApiArg>({
      query: (queryArg) => ({ 
        url: `/api/analytics/restaurants/${queryArg.restaurantId}/data`,
        params: { days: queryArg.days }
      }),
    }),
  }),
  overrideExisting: false,
});
export { injectedRtkApi as apiService };
export type GetRestaurantsApiResponse = /** status 200 */ Restaurant[];
export type GetRestaurantsApiArg = void;
export type GetRestaurantApiResponse = /** status 200 */ Restaurant;
export type GetRestaurantApiArg = {
  id: number;
};
export type GetRestaurantKpisApiResponse = /** status 200 */ RestaurantKpis;
export type GetRestaurantKpisApiArg = {
  id: number;
  period?: string;
};
export type GetRestaurantMetricsApiResponse = /** status 200 */ RestaurantMetricsResponse;
export type GetRestaurantMetricsApiArg = {
  restaurantId: number;
  metricName?: string;
  startDate?: string;
  endDate?: string;
  page?: number;
  pageSize?: number;
};
export type GetCorrelationAnalysisApiResponse = /** status 200 */ CorrelationAnalysis;
export type GetCorrelationAnalysisApiArg = {
  restaurantId: number;
};
export type GetAnalyticsDataApiResponse = /** status 200 */ AnalyticsData;
export type GetAnalyticsDataApiArg = {
  restaurantId: number;
  days?: number;
};
export type Restaurant = {
  id?: number;
  name?: string | null;
  address?: string | null;
  phone?: string | null;
  email?: string | null;
  managerId?: string | null;
};
export type RestaurantKpis = {
  restaurantId?: number;
  totalRevenue?: number;
  customerSatisfaction?: number;
  averageOrderValue?: number;
  orderAccuracy?: number;
  prepTime?: number;
  period?: string | null;
};
export type RestaurantMetricsResponse = {
  restaurantId?: number;
  metrics?: MetricData[];
  totalCount?: number;
  page?: number;
  pageSize?: number;
};
export type MetricData = {
  id?: number;
  restaurantId?: number;
  metricName?: string | null;
  value?: number;
  timestamp?: string;
  category?: string | null;
};
export type CorrelationAnalysis = {
  restaurantId?: number;
  correlations?: CorrelationPair[];
  totalDataPoints?: number;
  analysisTimestamp?: string;
};
export type CorrelationPair = {
  metric1?: string | null;
  metric2?: string | null;
  correlationCoefficient?: number;
  pValue?: number;
  strength?: string | null;
  significant?: boolean;
};
export type AnalyticsData = {
  restaurantId?: number;
  data?: AnalyticsPoint[];
  summary?: AnalyticsSummary;
};
export type AnalyticsPoint = {
  timestamp?: string;
  revenue?: number;
  customerSatisfaction?: number;
  prepTime?: number;
  orderAccuracy?: number;
};
export type AnalyticsSummary = {
  totalRevenue?: number;
  averageCustomerSatisfaction?: number;
  averagePrepTime?: number;
  averageOrderAccuracy?: number;
  dataPoints?: number;
};
export const {
  useGetRestaurantsQuery,
  useGetRestaurantQuery,
  useGetRestaurantKpisQuery,
  useGetRestaurantMetricsQuery,
  useGetCorrelationAnalysisQuery,
  useGetAnalyticsDataQuery,
} = injectedRtkApi;
