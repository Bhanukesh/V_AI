// API client for restaurant performance management
const API_BASE_URL = 'https://localhost:7583/api';

export interface Restaurant {
  id: number;
  name: string;
  address: string;
  phone?: string;
  email?: string;
  timeZoneId: string;
  createdAt: string;
  updatedAt: string;
  organizationId: number;
}

export interface RestaurantKpis {
  restaurantId: number;
  restaurantName: string;
  totalRevenue: number;
  averageRevenue: number;
  prepTimeAverage: number;
  tableTurnoverAverage: number;
  orderAccuracyAverage: number;
  customerSatisfactionAverage: number;
  totalTransactions: number;
  periodStart: string;
  periodEnd: string;
}

export interface MetricValue {
  id: number;
  metricName: string;
  value: number;
  timestamp: string;
  unit?: string;
  tags?: string;
}

export interface CorrelationData {
  metricName: string;
  displayName: string;
  coefficient: number;
  pValue: number;
  isSignificant: boolean;
  strength: string;
  direction: string;
  interpretation: string;
}

export interface CorrelationAnalysis {
  restaurantId: number;
  restaurantName: string;
  analysisDate: string;
  correlationType: string;
  correlations: CorrelationData[];
}

export interface AnalyticsData {
  restaurantId: number;
  restaurantName: string;
  dataPeriod: string;
  revenueDataPoints: number;
  metricDataPoints: number;
  availableMetrics: string[];
  dataQuality: string;
}

class ApiClient {
  private baseUrl: string;

  constructor(baseUrl: string = API_BASE_URL) {
    this.baseUrl = baseUrl;
  }

  private async fetchApi<T>(endpoint: string, options?: RequestInit): Promise<T> {
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers,
      },
      ...options,
    });

    if (!response.ok) {
      throw new Error(`API Error: ${response.status} ${response.statusText}`);
    }

    return response.json();
  }

  // Restaurant endpoints
  async getRestaurants(): Promise<Restaurant[]> {
    return this.fetchApi<Restaurant[]>('/restaurants');
  }

  async getRestaurant(id: number): Promise<Restaurant> {
    return this.fetchApi<Restaurant>(`/restaurants/${id}`);
  }

  async getRestaurantKpis(id: number, period: string = 'last_30d'): Promise<RestaurantKpis> {
    return this.fetchApi<RestaurantKpis>(`/restaurants/${id}/kpis?period=${period}`);
  }

  // Metrics endpoints
  async getRestaurantMetrics(
    restaurantId: number,
    options: {
      metricName?: string;
      startDate?: string;
      endDate?: string;
      page?: number;
      pageSize?: number;
    } = {}
  ): Promise<MetricValue[]> {
    const params = new URLSearchParams();
    if (options.metricName) params.append('metricName', options.metricName);
    if (options.startDate) params.append('startDate', options.startDate);
    if (options.endDate) params.append('endDate', options.endDate);
    if (options.page) params.append('page', options.page.toString());
    if (options.pageSize) params.append('pageSize', options.pageSize.toString());

    const query = params.toString() ? `?${params.toString()}` : '';
    return this.fetchApi<MetricValue[]>(`/restaurants/${restaurantId}/metrics${query}`);
  }

  // Analytics endpoints
  async getCorrelationAnalysis(restaurantId: number): Promise<CorrelationAnalysis> {
    return this.fetchApi<CorrelationAnalysis>(`/analytics/restaurants/${restaurantId}/correlations`);
  }

  async getAnalyticsData(restaurantId: number, days: number = 90): Promise<AnalyticsData> {
    return this.fetchApi<AnalyticsData>(`/analytics/restaurants/${restaurantId}/data?days=${days}`);
  }
}

export const apiClient = new ApiClient();