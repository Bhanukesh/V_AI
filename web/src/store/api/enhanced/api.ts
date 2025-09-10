import { apiService } from "../generated/api";

export const enhancedApiService = apiService.enhanceEndpoints({
    addTagTypes: [
        'RESTAURANT',
        'RESTAURANT_KPI',
        'RESTAURANT_METRICS',
        'ANALYTICS',
    ],
    endpoints: {
        getRestaurants: {
            providesTags: ['RESTAURANT'],
        },
        getRestaurant: {
            providesTags: (result, error, arg) => 
                [{ type: 'RESTAURANT', id: arg.id }],
        },
        getRestaurantKpis: {
            providesTags: (result, error, arg) =>
                [{ type: 'RESTAURANT_KPI', id: arg.id }],
        },
        getRestaurantMetrics: {
            providesTags: (result, error, arg) =>
                [{ type: 'RESTAURANT_METRICS', id: arg.restaurantId }],
        },
        getCorrelationAnalysis: {
            providesTags: (result, error, arg) =>
                [{ type: 'ANALYTICS', id: `correlation-${arg.restaurantId}` }],
        },
        getAnalyticsData: {
            providesTags: (result, error, arg) =>
                [{ type: 'ANALYTICS', id: `data-${arg.restaurantId}` }],
        },
    }
});

export const {
  useGetRestaurantsQuery,
  useGetRestaurantQuery,
  useGetRestaurantKpisQuery,
  useGetRestaurantMetricsQuery,
  useGetCorrelationAnalysisQuery,
  useGetAnalyticsDataQuery,
} = enhancedApiService;