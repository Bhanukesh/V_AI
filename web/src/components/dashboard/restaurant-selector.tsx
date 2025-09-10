'use client';

import { useState, useEffect } from 'react';
import { ChevronDown } from 'lucide-react';
import { Restaurant, apiClient } from '@/lib/api-client';

interface RestaurantSelectorProps {
  selectedRestaurantId: number | null;
  onRestaurantChange: (restaurantId: number) => void;
}

export function RestaurantSelector({ selectedRestaurantId, onRestaurantChange }: RestaurantSelectorProps) {
  const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
  const [isOpen, setIsOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchRestaurants = async () => {
      try {
        const data = await apiClient.getRestaurants();
        setRestaurants(data);
        if (data.length > 0 && !selectedRestaurantId) {
          onRestaurantChange(data[0].id);
        }
      } catch (error) {
        console.error('Failed to fetch restaurants:', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchRestaurants();
  }, [selectedRestaurantId, onRestaurantChange]);

  const selectedRestaurant = restaurants.find(r => r.id === selectedRestaurantId);

  if (isLoading) {
    return (
      <div className="w-64 p-2 bg-white border rounded-md">
        <div className="animate-pulse h-6 bg-gray-200 rounded"></div>
      </div>
    );
  }

  return (
    <div className="relative w-64">
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="w-full flex items-center justify-between p-3 bg-white border border-gray-300 rounded-lg shadow-sm hover:border-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
      >
        <div className="flex flex-col items-start">
          <span className="text-sm font-medium text-gray-900">
            {selectedRestaurant?.name || 'Select Restaurant'}
          </span>
          {selectedRestaurant && (
            <span className="text-xs text-gray-500 truncate w-48">
              {selectedRestaurant.address}
            </span>
          )}
        </div>
        <ChevronDown className={`h-4 w-4 text-gray-400 transition-transform ${isOpen ? 'rotate-180' : ''}`} />
      </button>

      {isOpen && (
        <div className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-lg shadow-lg">
          <div className="py-1">
            {restaurants.map((restaurant) => (
              <button
                key={restaurant.id}
                onClick={() => {
                  onRestaurantChange(restaurant.id);
                  setIsOpen(false);
                }}
                className={`w-full px-3 py-2 text-left hover:bg-gray-50 focus:outline-none focus:bg-gray-50 ${
                  selectedRestaurantId === restaurant.id ? 'bg-blue-50 text-blue-700' : 'text-gray-900'
                }`}
              >
                <div className="flex flex-col">
                  <span className="font-medium">{restaurant.name}</span>
                  <span className="text-sm text-gray-500 truncate">
                    {restaurant.address}
                  </span>
                </div>
              </button>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}